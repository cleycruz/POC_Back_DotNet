using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace CarritoComprasAPI.Core.Configuration;

/// <summary>
/// Servicio para gestión centralizada de configuración
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Configuración de la aplicación
    /// </summary>
    AppSettings Settings { get; }

    /// <summary>
    /// Configuración de API
    /// </summary>
    ApiConfig Api { get; }

    /// <summary>
    /// Configuración de base de datos
    /// </summary>
    DatabaseConfig Database { get; }

    /// <summary>
    /// Configuración de autenticación
    /// </summary>
    AuthenticationConfig Authentication { get; }

    /// <summary>
    /// Configuración de caché
    /// </summary>
    CacheConfig Cache { get; }

    /// <summary>
    /// Configuración de seguridad
    /// </summary>
    SecurityConfig Security { get; }

    /// <summary>
    /// Configuración de características
    /// </summary>
    FeaturesConfig Features { get; }

    /// <summary>
    /// Configuración de servicios externos
    /// </summary>
    ExternalConfig External { get; }

    /// <summary>
    /// Ambiente actual
    /// </summary>
    EnvironmentConfig Environment { get; }

    /// <summary>
    /// Obtiene la URL completa de la API
    /// </summary>
    string GetApiUrl(string endpoint = "");

    /// <summary>
    /// Obtiene la URL de un servicio externo
    /// </summary>
    string GetExternalServiceUrl(string serviceName, string endpoint = "");

    /// <summary>
    /// Valida toda la configuración
    /// </summary>
    (bool IsValid, List<string> Errors) ValidateConfiguration();

    /// <summary>
    /// Obtiene información del entorno actual
    /// </summary>
    EnvironmentInfo GetEnvironmentInfo();

    /// <summary>
    /// Verifica si la configuración es válida
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Errores de validación
    /// </summary>
    Collection<string> ValidationErrors { get; }
}

/// <summary>
/// Implementación del servicio de configuración
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly AppSettings _settings;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IOptions<AppSettings> settings, ILogger<ConfigurationService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        // Validar configuración al inicializar
        var (isValid, errors) = ValidateConfiguration();
        ValidationErrors = new Collection<string>(errors);
        
        if (!isValid)
        {
            var errorMessage = $"Configuración inválida: {string.Join(", ", errors)}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("ConfigurationService inicializado correctamente para entorno: {Environment}", 
            _settings.Environment.Name);
    }

    public AppSettings Settings => _settings;
    public ApiConfig Api => _settings.Api;
    public DatabaseConfig Database => _settings.Database;
    public AuthenticationConfig Authentication => _settings.Authentication;
    public CacheConfig Cache => _settings.Cache;
    public SecurityConfig Security => _settings.Security;
    public FeaturesConfig Features => _settings.Features;
    public ExternalConfig External => _settings.External;
    public EnvironmentConfig Environment => _settings.Environment;

    public bool IsValid => !ValidationErrors.Any();
    public Collection<string> ValidationErrors { get; private set; } = new();

    public string GetApiUrl(string endpoint = "")
    {
        var baseUrl = _settings.Api.BaseUrl.TrimEnd('/');
        var port = !string.IsNullOrEmpty(_settings.Api.Port) ? $":{_settings.Api.Port}" : "";
        
        if (string.IsNullOrWhiteSpace(endpoint))
            return $"{baseUrl}{port}";
            
        var cleanEndpoint = endpoint.TrimStart('/');
        return $"{baseUrl}{port}/{cleanEndpoint}";
    }

    public string GetExternalServiceUrl(string serviceName, string endpoint = "")
    {
        var serviceUrl = serviceName.ToUpperInvariant() switch
        {
            "PAYMENT" or "PAYMENTSERVICE" => _settings.External.PaymentService?.BaseUrl,
            "NOTIFICATION" or "NOTIFICATIONSERVICE" => _settings.External.NotificationService?.BaseUrl,
            _ => throw new ArgumentException($"Servicio externo no encontrado: {serviceName}", nameof(serviceName))
        };

        if (string.IsNullOrWhiteSpace(serviceUrl))
            throw new InvalidOperationException($"URL del servicio {serviceName} no configurada");

        var baseUrl = serviceUrl.TrimEnd('/');
        
        if (string.IsNullOrWhiteSpace(endpoint))
            return baseUrl;
            
        var cleanEndpoint = endpoint.TrimStart('/');
        return $"{baseUrl}/{cleanEndpoint}";
    }

    public (bool IsValid, List<string> Errors) ValidateConfiguration()
    {
        var context = new ValidationContext(_settings);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(_settings, context, results, true);

        var errors = new List<string>();

        if (!isValid)
        {
            errors.AddRange(results.Select(r => r.ErrorMessage ?? "Error de validación desconocido"));
        }

        // Validaciones específicas de API
        ValidateApiSettings(errors);

        // Validaciones específicas de base de datos
        ValidateDatabaseSettings(errors);

        // Validaciones específicas de autenticación
        ValidateAuthenticationSettings(errors);

        // Validaciones específicas de caché
        ValidateCacheSettings(errors);

        // Validaciones específicas de seguridad
        ValidateSecuritySettings(errors);

        // Validaciones de entorno
        ValidateEnvironmentSettings(errors);

        return (errors.Count == 0, errors);
    }

    private void ValidateApiSettings(List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(_settings.Api.BaseUrl))
            errors.Add("API BaseUrl es requerida");

        if (string.IsNullOrWhiteSpace(_settings.Api.Port))
            errors.Add("API Port es requerido");

        if (string.IsNullOrWhiteSpace(_settings.Api.Version))
            errors.Add("API Version es requerida");
    }

    private void ValidateDatabaseSettings(List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(_settings.Database.Provider))
            errors.Add("Database Provider es requerido");

        if (string.IsNullOrWhiteSpace(_settings.Database.ConnectionString))
            errors.Add("Database ConnectionString es requerida");

        if (_settings.Database.CommandTimeout <= 0)
            errors.Add("Database CommandTimeout debe ser mayor que 0");

        if (_settings.Database.MaxRetryCount < 0)
            errors.Add("Database MaxRetryCount no puede ser negativo");
    }

    private void ValidateAuthenticationSettings(List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(_settings.Authentication.JwtSecret))
            errors.Add("Authentication JwtSecret es requerido");

        if (_settings.Authentication.JwtExpirationMinutes <= 0)
            errors.Add("Authentication JwtExpirationMinutes debe ser mayor que 0");

        if (string.IsNullOrWhiteSpace(_settings.Authentication.JwtIssuer))
            errors.Add("Authentication JwtIssuer es requerido");

        if (string.IsNullOrWhiteSpace(_settings.Authentication.JwtAudience))
            errors.Add("Authentication JwtAudience es requerido");

        if (_settings.Authentication.RefreshTokenExpirationDays <= 0)
            errors.Add("Authentication RefreshTokenExpirationDays debe ser mayor que 0");
    }

    private void ValidateCacheSettings(List<string> errors)
    {
        if (_settings.Cache.DefaultExpirationMinutes <= 0)
            errors.Add("Cache DefaultExpirationMinutes debe ser mayor que 0");

        if (string.IsNullOrWhiteSpace(_settings.Cache.Provider))
            errors.Add("Cache Provider es requerido");

        if (_settings.Cache.EnableDistributedCache && string.IsNullOrWhiteSpace(_settings.Cache.ConnectionString))
            errors.Add("Cache ConnectionString es requerida cuando EnableDistributedCache es true");
    }

    private void ValidateSecuritySettings(List<string> errors)
    {
        if (_settings.Security.RateLimitRequests <= 0)
            errors.Add("Security RateLimitRequests debe ser mayor que 0");

        if (string.IsNullOrWhiteSpace(_settings.Security.RateLimitWindow))
            errors.Add("Security RateLimitWindow es requerido");
    }

    private void ValidateEnvironmentSettings(List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(_settings.Environment.Name))
            errors.Add("Environment Name es requerido");

        // Validaciones específicas de producción
        if (_settings.Environment.IsProduction)
        {
            if (_settings.Authentication.JwtSecret.Contains("dev", StringComparison.OrdinalIgnoreCase))
                errors.Add("No se debe usar secreto de desarrollo en producción");

            if (_settings.Features.EnableSensitiveDataLogging)
                errors.Add("No se debe habilitar logging sensible en producción");

            if (_settings.Environment.Debug)
                errors.Add("Debug no debe estar habilitado en producción");
        }
    }

    public EnvironmentInfo GetEnvironmentInfo()
    {
        return new EnvironmentInfo
        {
            Name = _settings.Environment.Name,
            IsProduction = _settings.Environment.IsProduction,
            Debug = _settings.Environment.Debug,
            ApiVersion = _settings.Api.Version,
            ApiBaseUrl = GetApiUrl(),
            DatabaseProvider = _settings.Database.Provider,
            CacheProvider = _settings.Cache.Provider,
            SwaggerEnabled = _settings.Api.EnableSwagger,
            ValidationErrors = ValidationErrors
        };
    }
}

/// <summary>
/// Información del entorno actual
/// </summary>
public class EnvironmentInfo
{
    public string Name { get; set; } = string.Empty;
    public bool IsProduction { get; set; }
    public bool Debug { get; set; }
    public string ApiVersion { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string DatabaseProvider { get; set; } = string.Empty;
    public string CacheProvider { get; set; } = string.Empty;
    public bool SwaggerEnabled { get; set; }
    public Collection<string> ValidationErrors { get; set; } = new();
}
