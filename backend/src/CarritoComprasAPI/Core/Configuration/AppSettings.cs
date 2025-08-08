using System.ComponentModel.DataAnnotations;

namespace CarritoComprasAPI.Core.Configuration;

/// <summary>
/// Configuración principal de la aplicación
/// </summary>
public class AppSettings
{
    public const string SectionName = "";

    [Required]
    public EnvironmentConfig Environment { get; set; } = new();

    [Required]
    public ApiConfig Api { get; set; } = new();

    [Required]
    public DatabaseConfig Database { get; set; } = new();

    [Required]
    public AuthenticationConfig Authentication { get; set; } = new();

    [Required]
    public CacheConfig Cache { get; set; } = new();

    [Required]
    public SecurityConfig Security { get; set; } = new();

    [Required]
    public FeaturesConfig Features { get; set; } = new();

    [Required]
    public ExternalConfig External { get; set; } = new();
}

/// <summary>
/// Configuración del entorno
/// </summary>
public class EnvironmentConfig
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsProduction { get; set; }

    public bool Debug { get; set; }
}

/// <summary>
/// Configuración de la API
/// </summary>
public class ApiConfig
{
    [Required]
    public string Version { get; set; } = "v1";

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    [Range(1, 65535)]
    public string Port { get; set; } = string.Empty;

    public bool EnableSwagger { get; set; }

    public bool EnableCors { get; set; } = true;

    [Required]
    public List<string> AllowedOrigins { get; set; } = new();

    /// <summary>
    /// Obtiene la URL completa de la API
    /// </summary>
    public string GetFullUrl() => BaseUrl.TrimEnd('/');

    /// <summary>
    /// Obtiene la URL de un endpoint específico
    /// </summary>
    public string GetEndpointUrl(string endpoint)
    {
        var cleanEndpoint = endpoint.TrimStart('/');
        return $"{GetFullUrl()}/{Version}/{cleanEndpoint}";
    }

    /// <summary>
    /// Valida que todas las URLs de orígenes permitidos sean válidas
    /// </summary>
    public bool ValidateAllowedOrigins()
    {
        return AllowedOrigins.All(origin => Uri.TryCreate(origin, UriKind.Absolute, out _));
    }
}

/// <summary>
/// Configuración de base de datos
/// </summary>
public class DatabaseConfig
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string Provider { get; set; } = "SqlServer";

    public bool EnableSensitiveDataLogging { get; set; }

    public bool EnableDetailedErrors { get; set; }

    [Range(1, 300)]
    public int CommandTimeout { get; set; } = 30;

    [Range(0, 10)]
    public int MaxRetryCount { get; set; } = 3;
}

/// <summary>
/// Configuración de autenticación
/// </summary>
public class AuthenticationConfig
{
    [Required]
    [MinLength(32)]
    public string JwtSecret { get; set; } = string.Empty;

    [Required]
    public string JwtIssuer { get; set; } = string.Empty;

    [Required]
    public string JwtAudience { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int JwtExpirationMinutes { get; set; } = 60;

    [Range(1, 365)]
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

/// <summary>
/// Configuración de caché
/// </summary>
public class CacheConfig
{
    [Required]
    public string Provider { get; set; } = "InMemory";

    public string ConnectionString { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int DefaultExpirationMinutes { get; set; } = 60;

    public bool EnableDistributedCache { get; set; }
}

/// <summary>
/// Configuración de seguridad
/// </summary>
public class SecurityConfig
{
    public bool EnableRateLimiting { get; set; } = true;

    [Range(1, 10000)]
    public int RateLimitRequests { get; set; } = 100;

    public string RateLimitWindow { get; set; } = "00:01:00";

    public bool EnableRequestLogging { get; set; } = true;

    public bool EnableResponseCompression { get; set; } = true;
}

/// <summary>
/// Configuración de características
/// </summary>
public class FeaturesConfig
{
    public bool EnableMetrics { get; set; } = true;

    public bool EnableHealthChecks { get; set; } = true;

    public bool EnableDetailedErrors { get; set; }

    public bool EnableSensitiveDataLogging { get; set; }
}

/// <summary>
/// Configuración de servicios externos
/// </summary>
public class ExternalConfig
{
    [Required]
    public ServiceConfig PaymentService { get; set; } = new();

    [Required]
    public ServiceConfig NotificationService { get; set; } = new();
}

/// <summary>
/// Configuración de servicios externos
/// </summary>
public class ServiceConfig
{
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Obtiene la URL completa del servicio
    /// </summary>
    public string GetServiceUrl(string endpoint = "")
    {
        var cleanEndpoint = endpoint.TrimStart('/');
        return string.IsNullOrEmpty(cleanEndpoint) 
            ? BaseUrl.TrimEnd('/') 
            : $"{BaseUrl.TrimEnd('/')}/{cleanEndpoint}";
    }
}
