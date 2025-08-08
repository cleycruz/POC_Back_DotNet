using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Configuration;

namespace CarritoComprasAPI.Controllers;

/// <summary>
/// Controlador para gestión de información de configuración
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(IConfigurationService configurationService, ILogger<ConfigurationController> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene información del entorno actual
    /// </summary>
    /// <returns>Información del entorno</returns>
    [HttpGet("environment")]
    public IActionResult GetEnvironmentInfo()
    {
        try
        {
            var environmentInfo = _configurationService.GetEnvironmentInfo();
            return Ok(environmentInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información del entorno");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene la URL base de la API
    /// </summary>
    /// <returns>URL base de la API</returns>
    [HttpGet("api-url")]
    public IActionResult GetApiUrl()
    {
        try
        {
            var apiUrl = _configurationService.GetApiUrl();
            return Ok(new { apiUrl = apiUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener URL de la API");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Valida la configuración actual
    /// </summary>
    /// <returns>Resultado de validación</returns>
    [HttpGet("validate")]
    public IActionResult ValidateConfiguration()
    {
        try
        {
            var (isValid, errors) = _configurationService.ValidateConfiguration();
            return Ok(new 
            { 
                isValid = isValid, 
                errors = errors,
                validationTime = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar configuración");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene información de configuración para desarrollo
    /// Solo disponible en ambientes no productivos
    /// </summary>
    /// <returns>Información de configuración</returns>
    [HttpGet("dev-info")]
    public IActionResult GetDevelopmentInfo()
    {
        try
        {
            if (_configurationService.Environment.IsProduction)
            {
                return Forbid("Esta información no está disponible en producción");
            }

            var devInfo = new
            {
                Environment = _configurationService.Environment.Name,
                Debug = _configurationService.Environment.Debug,
                Api = new
                {
                    _configurationService.Api.BaseUrl,
                    _configurationService.Api.Port,
                    _configurationService.Api.Version,
                    _configurationService.Api.EnableSwagger,
                    _configurationService.Api.EnableCors
                },
                Database = new
                {
                    _configurationService.Database.Provider,
                    _configurationService.Database.CommandTimeout,
                    _configurationService.Database.MaxRetryCount,
                    _configurationService.Database.EnableDetailedErrors,
                    _configurationService.Database.EnableSensitiveDataLogging
                },
                Cache = new
                {
                    _configurationService.Cache.Provider,
                    _configurationService.Cache.DefaultExpirationMinutes,
                    _configurationService.Cache.EnableDistributedCache
                },
                Features = new
                {
                    _configurationService.Features.EnableMetrics,
                    _configurationService.Features.EnableHealthChecks,
                    _configurationService.Features.EnableDetailedErrors,
                    _configurationService.Features.EnableSensitiveDataLogging
                }
            };

            return Ok(devInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información de desarrollo");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
