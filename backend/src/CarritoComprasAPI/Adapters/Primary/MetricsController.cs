using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Logging;
using System.Globalization;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controller para exposición de métricas de performance y salud del sistema
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MetricsController : BaseController
    {
        private readonly IPerformanceMetricsService _metricsService;
        private readonly IStructuredLogger _logger;
        
        // Constantes para evitar duplicación de strings literales
        private const string MetricsEntityType = "Metrics";
        private const string ErrorMessage = "Error interno del servidor";
        private const string SystemEntityType = "System";
        private const string HealthEntityType = "Health";

        public MetricsController(
            IPerformanceMetricsService metricsService,
            IStructuredLogger logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las métricas de performance del sistema
        /// </summary>
        /// <returns>Diccionario con todas las métricas de operaciones</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, OperationStats>), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetAllMetrics()
        {
            try
            {
                var metrics = _metricsService.GetAllMetrics();
                _logger.LogOperacionDominio("GetMetrics", MetricsEntityType, "All", "Retrieved all performance metrics");
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving metrics", ex);
                return StatusCode(500, ErrorMessage);
            }
        }

        /// <summary>
        /// Obtiene las métricas de una operación específica
        /// </summary>
        /// <param name="operationName">Nombre de la operación</param>
        /// <returns>Estadísticas de la operación solicitada</returns>
        [HttpGet("{operationName}")]
        [ProducesResponseType(typeof(OperationStats), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetOperationMetrics(string operationName)
        {
            try
            {
                var stats = _metricsService.GetOperationStats(operationName);
                
                if (stats == null)
                {
                    _logger.LogOperacionDominio("GetOperationMetrics", MetricsEntityType, operationName, "Operation not found");
                    return NotFound($"No se encontraron métricas para la operación: {operationName}");
                }

                _logger.LogOperacionDominio("GetOperationMetrics", MetricsEntityType, operationName, "Retrieved operation metrics");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving metrics for operation: {operationName}", ex);
                return StatusCode(500, ErrorMessage);
            }
        }

        /// <summary>
        /// Obtiene las métricas del sistema (CPU, memoria, etc.)
        /// </summary>
        /// <returns>Métricas actuales del sistema</returns>
        [HttpGet("system")]
        [ProducesResponseType(typeof(SystemMetrics), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetSystemMetrics()
        {
            try
            {
                var systemMetrics = _metricsService.GetSystemMetrics();
                _logger.LogOperacionDominio("GetSystemMetrics", SystemEntityType, "Current", "Retrieved system metrics");
                return Ok(systemMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving system metrics", ex);
                return StatusCode(500, ErrorMessage);
            }
        }

        /// <summary>
        /// Obtiene el estado de salud general del sistema basado en métricas
        /// </summary>
        /// <returns>Estado de salud consolidado</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(SystemHealthReport), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetHealthStatus()
        {
            try
            {
                var allMetrics = _metricsService.GetAllMetrics();
                var systemMetrics = _metricsService.GetSystemMetrics();
                
                var healthReport = new SystemHealthReport
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    OverallHealth = CalculateOverallHealth(allMetrics, systemMetrics),
                    SystemMetrics = systemMetrics,
                    OperationHealthStatus = allMetrics.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new OperationHealthInfo
                        {
                            Health = kvp.Value.GetHealthStatus(),
                            AverageDurationMs = kvp.Value.AverageDuration.TotalMilliseconds,
                            SuccessRate = kvp.Value.SuccessRate,
                            TotalExecutions = kvp.Value.TotalExecutions,
                            P95DurationMs = kvp.Value.GetPercentile(95)
                        })
                };

                _logger.LogOperacionDominio("GetHealthStatus", HealthEntityType, SystemEntityType, 
                    $"Health check completed - Status: {healthReport.OverallHealth}");
                
                return Ok(healthReport);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving health status", ex);
                return StatusCode(500, ErrorMessage);
            }
        }

        /// <summary>
        /// Reinicia todas las métricas del sistema
        /// </summary>
        /// <returns>Confirmación del reinicio</returns>
        [HttpPost("reset")]
        [ProducesResponseType(typeof(ResetResponse), 200)]
        [ProducesResponseType(500)]
        public IActionResult ResetMetrics()
        {
            try
            {
                _metricsService.Reset();
                _logger.LogOperacionDominio("ResetMetrics", MetricsEntityType, "All", "All metrics have been reset");
                
                var response = new ResetResponse 
                { 
                    Message = "Todas las métricas han sido reiniciadas", 
                    Timestamp = DateTimeOffset.UtcNow 
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error resetting metrics", ex);
                return StatusCode(500, ErrorMessage);
            }
        }

        /// <summary>
        /// Obtiene un resumen ejecutivo de las métricas más importantes
        /// </summary>
        /// <returns>Resumen de las métricas clave</returns>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(MetricsSummary), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetMetricsSummary()
        {
            try
            {
                var allMetrics = _metricsService.GetAllMetrics();
                var systemMetrics = _metricsService.GetSystemMetrics();

                var summary = new MetricsSummary
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    TotalOperations = allMetrics.Count,
                    TotalExecutions = allMetrics.Values.Sum(m => m.TotalExecutions),
                    AverageSuccessRate = allMetrics.Any() ? allMetrics.Values.Average(m => m.SuccessRate) : 0,
                    SlowestOperations = allMetrics
                        .OrderByDescending(kvp => kvp.Value.AverageDuration.TotalMilliseconds)
                        .Take(5)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AverageDuration.TotalMilliseconds),
                    MostFailedOperations = allMetrics
                        .Where(kvp => kvp.Value.FailedExecutions > 0)
                        .OrderByDescending(kvp => kvp.Value.FailedExecutions)
                        .Take(5)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.FailedExecutions),
                    SystemMemoryUsageMB = systemMetrics.MemoryUsageMB,
                    SystemCpuUsagePercent = systemMetrics.CpuUsagePercent
                };

                _logger.LogOperacionDominio("GetMetricsSummary", MetricsEntityType, "Summary", "Retrieved metrics summary");
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving metrics summary", ex);
                return StatusCode(500, ErrorMessage);
            }
        }

        private static PerformanceHealth CalculateOverallHealth(
            Dictionary<string, OperationStats> operationMetrics, 
            SystemMetrics systemMetrics)
        {
            // Evaluar salud del sistema
            if (systemMetrics.MemoryUsageMB > 1024 || systemMetrics.CpuUsagePercent > 80)
                return PerformanceHealth.Critical;

            // Evaluar salud de las operaciones
            if (!operationMetrics.Any())
                return PerformanceHealth.Healthy;

            var operationHealthStatuses = operationMetrics.Values
                .Select(stats => stats.GetHealthStatus())
                .ToList();

            if (operationHealthStatuses.Any(h => h == PerformanceHealth.Critical))
                return PerformanceHealth.Critical;

            if (operationHealthStatuses.Count(h => h == PerformanceHealth.Warning) > operationHealthStatuses.Count / 2)
                return PerformanceHealth.Warning;

            return PerformanceHealth.Healthy;
        }
    }

    /// <summary>
    /// Respuesta del reset de métricas
    /// </summary>
    public class ResetResponse
    {
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Reporte de salud del sistema
    /// </summary>
    public class SystemHealthReport
    {
        public DateTimeOffset Timestamp { get; set; }
        public PerformanceHealth OverallHealth { get; set; }
        public SystemMetrics SystemMetrics { get; set; } = new();
        public Dictionary<string, OperationHealthInfo> OperationHealthStatus { get; set; } = new();
    }

    /// <summary>
    /// Información de salud de una operación
    /// </summary>
    public class OperationHealthInfo
    {
        public PerformanceHealth Health { get; set; }
        public double AverageDurationMs { get; set; }
        public double SuccessRate { get; set; }
        public int TotalExecutions { get; set; }
        public double P95DurationMs { get; set; }
    }

    /// <summary>
    /// Resumen ejecutivo de métricas
    /// </summary>
    public class MetricsSummary
    {
        public DateTimeOffset Timestamp { get; set; }
        public int TotalOperations { get; set; }
        public int TotalExecutions { get; set; }
        public double AverageSuccessRate { get; set; }
        public Dictionary<string, double> SlowestOperations { get; set; } = new();
        public Dictionary<string, int> MostFailedOperations { get; set; } = new();
        public long SystemMemoryUsageMB { get; set; }
        public double SystemCpuUsagePercent { get; set; }
    }
}
