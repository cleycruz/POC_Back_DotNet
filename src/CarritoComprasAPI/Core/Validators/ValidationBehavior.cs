using FluentValidation;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.Validators
{
    /// <summary>
    /// Comportamiento de validación para el pipeline de comandos y queries
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly IAppLogger _logger;

        public ValidationBehavior(
            IRequestHandler<TRequest, TResponse> inner,
            IEnumerable<IValidator<TRequest>> validators,
            IAppLogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            // Ejecutar validaciones si existen validadores
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Any())
                {
                    _logger.LogWarning($"Errores de validación en {typeof(TRequest).Name}: {string.Join(", ", failures.Select(f => f.ErrorMessage))}");
                    throw new ValidationException(failures);
                }

                _logger.LogInformation($"Validación exitosa para {typeof(TRequest).Name}");
            }

            // Ejecutar el handler interno
            return await _inner.Handle(request, cancellationToken);
        }
    }

    /// <summary>
    /// Interfaces auxiliares para el pipeline de validación
    /// </summary>
    public interface IRequest<out TResponse> { }

    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Excepción personalizada para errores de validación de negocio
    /// </summary>
    public class BusinessValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public BusinessValidationException(string message) : base(message)
        {
            Errors = new[] { message };
        }

        public BusinessValidationException(IEnumerable<string> errors) : base("Errores de validación de negocio")
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public BusinessValidationException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new[] { message };
        }
    }

    /// <summary>
    /// Resultado de validación personalizado
    /// </summary>
    public class ValidationResult<T>
    {
        public bool IsValid { get; }
        public T? Value { get; }
        public IEnumerable<string> Errors { get; }

        private ValidationResult(bool isValid, T? value, IEnumerable<string> errors)
        {
            IsValid = isValid;
            Value = value;
            Errors = errors ?? Enumerable.Empty<string>();
        }

        public static ValidationResult<T> Success(T value)
        {
            return new ValidationResult<T>(true, value, Enumerable.Empty<string>());
        }

        public static ValidationResult<T> Failure(IEnumerable<string> errors)
        {
            return new ValidationResult<T>(false, default, errors);
        }

        public static ValidationResult<T> Failure(string error)
        {
            return new ValidationResult<T>(false, default, new[] { error });
        }
    }
}
