using System.Net;
using System.Text.Json;
using CarritoComprasAPI.Core.Logging;
using FluentValidation;
using System.Globalization;

namespace CarritoComprasAPI.Core.Validators
{
    /// <summary>
    /// Middleware avanzado para manejo global de excepciones con logging estructurado
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStructuredLogger _logger;
        private readonly IHostEnvironment _environment;
        
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            IStructuredLogger logger,
            IHostEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = GetOrGenerateCorrelationId(context);
            var requestInfo = ExtractRequestInfo(context);
            
            var errorResponse = CreateErrorResponse(exception);
            
            // Log del error con contexto enriquecido
            _logger.LogError("Global Exception Handler", exception, new
            {
                CorrelationId = correlationId,
                RequestInfo = requestInfo,
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                ResponseStatusCode = errorResponse.StatusCode
            });

            context.Response.StatusCode = errorResponse.StatusCode;
            context.Response.ContentType = "application/json";

            var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static string GetOrGenerateCorrelationId(HttpContext context)
        {
            // Buscar correlation ID en headers
            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                return correlationId.FirstOrDefault() ?? GenerateCorrelationId();
            }

            var newCorrelationId = GenerateCorrelationId();
            context.Response.Headers["X-Correlation-ID"] = newCorrelationId;
            return newCorrelationId;
        }

        private static string GenerateCorrelationId()
        {
            return Guid.NewGuid().ToString("N")[..12];
        }

        private static RequestInfo ExtractRequestInfo(HttpContext context)
        {
            return new RequestInfo
            {
                Method = context.Request.Method,
                Path = context.Request.Path.Value ?? "unknown",
                QueryString = context.Request.QueryString.Value ?? string.Empty,
                Timestamp = DateTimeOffset.UtcNow,
                Headers = context.Request.Headers.ToDictionary(
                    h => h.Key,
                    h => h.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase)
            };
        }

        private ErrorResponse CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => CreateValidationErrorResponse(validationEx),
                ArgumentException argumentEx => CreateArgumentErrorResponse(argumentEx),
                InvalidOperationException operationEx => CreateOperationErrorResponse(operationEx),
                UnauthorizedAccessException => CreateUnauthorizedErrorResponse(),
                KeyNotFoundException notFoundEx => CreateNotFoundErrorResponse(notFoundEx),
                TimeoutException => CreateTimeoutErrorResponse(),
                _ => CreateGenericErrorResponse(exception)
            };
        }

        private static ErrorResponse CreateValidationErrorResponse(ValidationException validationException)
        {
            var validationErrors = validationException.Errors.Select(error => new ErrorDetail
            {
                Field = error.PropertyName,
                Message = error.ErrorMessage,
                Code = error.ErrorCode ?? "ValidationError"
            }).ToList();

            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Se encontraron errores de validación en la solicitud",
                Errors = validationErrors,
                Timestamp = DateTime.UtcNow
            };
        }

        private static ErrorResponse CreateArgumentErrorResponse(ArgumentException argumentException)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = argumentException.Message,
                Timestamp = DateTime.UtcNow
            };
        }

        private static ErrorResponse CreateOperationErrorResponse(InvalidOperationException operationException)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity,
                Message = operationException.Message,
                Timestamp = DateTime.UtcNow
            };
        }

        private static ErrorResponse CreateUnauthorizedErrorResponse()
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = "No tiene autorización para acceder a este recurso",
                Timestamp = DateTime.UtcNow
            };
        }

        private static ErrorResponse CreateNotFoundErrorResponse(KeyNotFoundException notFoundException)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundException.Message,
                Timestamp = DateTime.UtcNow
            };
        }

        private static ErrorResponse CreateTimeoutErrorResponse()
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.RequestTimeout,
                Message = "La operación tardó demasiado tiempo en completarse",
                Timestamp = DateTime.UtcNow
            };
        }

        private ErrorResponse CreateGenericErrorResponse(Exception exception)
        {
            var isDevelopment = _environment.IsDevelopment();
            
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = isDevelopment 
                    ? exception.Message 
                    : "Ha ocurrido un error interno. Por favor contacte al administrador del sistema.",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Información de la request para debugging
    /// </summary>
    public class RequestInfo
    {
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
    }

    /// <summary>
    /// Extensiones para registrar el middleware
    /// </summary>
    public static class GlobalExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Registra el middleware de manejo global de excepciones
        /// </summary>
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
