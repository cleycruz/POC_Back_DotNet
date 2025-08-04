using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CarritoComprasAPI.Core.Validators
{
    /// <summary>
    /// Middleware para manejar excepciones de validación globalmente
    /// </summary>
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationExceptionMiddleware> _logger;

        public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió una excepción durante la ejecución de la solicitud");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Errores de validación";
                    response.Errors = validationEx.Errors.Select(e => new ErrorDetail
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage,
                        Code = e.ErrorCode
                    }).ToList();
                    break;

                case BusinessValidationException businessEx:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    response.Message = "Errores de validación de negocio";
                    response.Errors = businessEx.Errors.Select(error => new ErrorDetail
                    {
                        Field = "",
                        Message = error,
                        Code = "BUSINESS_VALIDATION"
                    }).ToList();
                    break;

                case ArgumentNullException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Parámetro requerido faltante";
                    response.Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = argEx.ParamName ?? "",
                            Message = argEx.Message,
                            Code = "REQUIRED_PARAMETER"
                        }
                    };
                    break;

                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Parámetro inválido";
                    response.Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = argEx.ParamName ?? "",
                            Message = argEx.Message,
                            Code = "INVALID_PARAMETER"
                        }
                    };
                    break;

                case InvalidOperationException invalidOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Operación inválida";
                    response.Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "",
                            Message = invalidOpEx.Message,
                            Code = "INVALID_OPERATION"
                        }
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Ha ocurrido un error interno del servidor";
                    response.Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "",
                            Message = "Error interno del servidor",
                            Code = "INTERNAL_ERROR"
                        }
                    };
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Respuesta de error estandarizada
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ErrorDetail> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Detalle de error individual
    /// </summary>
    public class ErrorDetail
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
