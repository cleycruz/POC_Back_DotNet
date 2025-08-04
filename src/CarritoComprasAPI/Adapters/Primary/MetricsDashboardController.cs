using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Alerting;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controlador para el dashboard de métricas y monitoreo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsDashboardController : BaseController
    {
        private readonly IPerformanceMetricsService _metricsService;
        private readonly IAlertingService _alertingService;

        public MetricsDashboardController(
            IPerformanceMetricsService metricsService,
            IAlertingService alertingService)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _alertingService = alertingService ?? throw new ArgumentNullException(nameof(alertingService));
        }

        /// <summary>
        /// Obtiene el estado de salud general del sistema
        /// </summary>
        [HttpGet("health")]
        public IActionResult GetHealthStatus()
        {
            try
            {
                var systemMetrics = _metricsService.GetSystemMetrics();
                var allOperationMetrics = _metricsService.GetAllMetrics();
                var activeAlerts = _alertingService.GetActiveAlerts();

                var overallHealth = CalculateOverallHealth(allOperationMetrics, activeAlerts);

                var healthResponse = new
                {
                    Status = overallHealth.ToString(),
                    Timestamp = DateTimeOffset.UtcNow,
                    System = new
                    {
                        CpuUsage = $"{systemMetrics.CpuUsagePercent:F1}%",
                        MemoryUsage = $"{systemMetrics.MemoryUsageMB} MB",
                        AvailableMemory = $"{systemMetrics.AvailableMemoryMB} MB",
                        ThreadCount = systemMetrics.ThreadCount,
                        TotalAllocatedBytes = systemMetrics.TotalAllocatedBytes
                    },
                    Operations = allOperationMetrics.Select(kvp => new
                    {
                        Name = kvp.Key,
                        Health = kvp.Value.GetHealthStatus().ToString(),
                        TotalExecutions = kvp.Value.TotalExecutions,
                        SuccessRate = $"{kvp.Value.SuccessRate:F1}%",
                        AverageDuration = $"{kvp.Value.AverageDuration.TotalMilliseconds:F0}ms",
                        MaxDuration = $"{kvp.Value.MaxDuration.TotalMilliseconds:F0}ms",
                        P95 = $"{kvp.Value.GetPercentile(95):F0}ms"
                    }).ToList(),
                    ActiveAlerts = activeAlerts.Select(alert => new
                    {
                        Id = alert.Id,
                        Operation = alert.OperationName,
                        Type = alert.AlertType.ToString(),
                        Severity = alert.Severity.ToString(),
                        Message = alert.Message,
                        CreatedAt = alert.CreatedAt
                    }).ToList(),
                    Summary = new
                    {
                        TotalOperations = allOperationMetrics.Count,
                        HealthyOperations = allOperationMetrics.Count(kvp => kvp.Value.GetHealthStatus() == PerformanceHealth.Healthy),
                        WarningOperations = allOperationMetrics.Count(kvp => kvp.Value.GetHealthStatus() == PerformanceHealth.Warning),
                        CriticalOperations = allOperationMetrics.Count(kvp => kvp.Value.GetHealthStatus() == PerformanceHealth.Critical),
                        TotalActiveAlerts = activeAlerts.Count,
                        CriticalAlerts = activeAlerts.Count(a => a.Severity == AlertSeverity.Critical),
                        WarningAlerts = activeAlerts.Count(a => a.Severity == AlertSeverity.Warning)
                    }
                };

                return Ok(healthResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error obteniendo estado de salud", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene métricas detalladas de una operación específica
        /// </summary>
        [HttpGet("operations/{operationName}")]
        public IActionResult GetOperationMetrics(string operationName)
        {
            try
            {
                var stats = _metricsService.GetOperationStats(operationName);
                if (stats == null)
                {
                    return NotFound(new { error = $"No se encontraron métricas para la operación '{operationName}'" });
                }

                var response = new
                {
                    OperationName = stats.OperationName,
                    Health = stats.GetHealthStatus().ToString(),
                    Statistics = new
                    {
                        TotalExecutions = stats.TotalExecutions,
                        SuccessfulExecutions = stats.SuccessfulExecutions,
                        FailedExecutions = stats.FailedExecutions,
                        SuccessRate = $"{stats.SuccessRate:F2}%"
                    },
                    Performance = new
                    {
                        AverageDuration = $"{stats.AverageDuration.TotalMilliseconds:F2}ms",
                        MinDuration = $"{stats.MinDuration.TotalMilliseconds:F2}ms",
                        MaxDuration = $"{stats.MaxDuration.TotalMilliseconds:F2}ms",
                        P50 = $"{stats.GetPercentile(50):F2}ms",
                        P95 = $"{stats.GetPercentile(95):F2}ms",
                        P99 = $"{stats.GetPercentile(99):F2}ms"
                    },
                    Timeline = new
                    {
                        FirstExecution = stats.FirstExecution,
                        LastExecution = stats.LastExecution,
                        TotalDuration = stats.TotalDuration
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error obteniendo métricas de operación", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las alertas activas
        /// </summary>
        [HttpGet("alerts")]
        public IActionResult GetActiveAlerts()
        {
            try
            {
                var alerts = _alertingService.GetActiveAlerts();
                
                var response = new
                {
                    TotalAlerts = alerts.Count,
                    CriticalAlerts = alerts.Count(a => a.Severity == AlertSeverity.Critical),
                    WarningAlerts = alerts.Count(a => a.Severity == AlertSeverity.Warning),
                    Alerts = alerts.Select(alert => new
                    {
                        Id = alert.Id,
                        OperationName = alert.OperationName,
                        Type = alert.AlertType.ToString(),
                        Severity = alert.Severity.ToString(),
                        Message = alert.Message,
                        Threshold = alert.Threshold,
                        CurrentValue = alert.CurrentValue,
                        CreatedAt = alert.CreatedAt,
                        Status = alert.Status.ToString(),
                        Duration = DateTimeOffset.UtcNow - alert.CreatedAt
                    }).OrderByDescending(a => a.CreatedAt).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error obteniendo alertas", details = ex.Message });
            }
        }

        /// <summary>
        /// Debug: Obtiene todas las alertas (activas y resueltas) para debugging
        /// </summary>
        [HttpGet("alerts/debug")]
        public IActionResult GetAllAlertsDebug()
        {
            try
            {
                // Necesito acceder a todas las alertas para debug
                var alertingServiceType = _alertingService.GetType();
                var activeAlertsField = alertingServiceType.GetField("_activeAlerts", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (activeAlertsField?.GetValue(_alertingService) is List<Alert> allAlerts)
                {
                    var response = new
                    {
                        TotalAlerts = allAlerts.Count,
                        ActiveAlerts = allAlerts.Count(a => a.Status == AlertStatus.Active),
                        ResolvedAlerts = allAlerts.Count(a => a.Status == AlertStatus.Resolved),
                        Alerts = allAlerts.Select(alert => new
                        {
                            Id = alert.Id,
                            OperationName = alert.OperationName,
                            Type = alert.AlertType.ToString(),
                            Severity = alert.Severity.ToString(),
                            Message = alert.Message,
                            Threshold = alert.Threshold,
                            CurrentValue = alert.CurrentValue,
                            CreatedAt = alert.CreatedAt,
                            Status = alert.Status.ToString(),
                            ResolvedAt = alert.ResolvedAt,
                            Duration = alert.Status == AlertStatus.Active ? 
                                DateTimeOffset.UtcNow - alert.CreatedAt : 
                                (alert.ResolvedAt - alert.CreatedAt)
                        }).OrderByDescending(a => a.CreatedAt).ToList()
                    };

                    return Ok(response);
                }
                
                return Ok(new { message = "No se pudo acceder a las alertas para debug" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error en debug de alertas", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene métricas del sistema en tiempo real
        /// </summary>
        [HttpGet("system")]
        public IActionResult GetSystemMetrics()
        {
            try
            {
                var systemMetrics = _metricsService.GetSystemMetrics();
                
                var response = new
                {
                    Timestamp = systemMetrics.Timestamp,
                    Cpu = new
                    {
                        UsagePercent = systemMetrics.CpuUsagePercent,
                        Status = GetResourceStatus(systemMetrics.CpuUsagePercent, 70, 90)
                    },
                    Memory = new
                    {
                        UsedMB = systemMetrics.MemoryUsageMB,
                        AvailableMB = systemMetrics.AvailableMemoryMB,
                        UsagePercent = systemMetrics.AvailableMemoryMB > 0 ? 
                            (double)systemMetrics.MemoryUsageMB / (systemMetrics.MemoryUsageMB + systemMetrics.AvailableMemoryMB) * 100 : 0,
                        Status = GetMemoryStatus(systemMetrics.MemoryUsageMB, systemMetrics.AvailableMemoryMB)
                    },
                    Threads = new
                    {
                        Count = systemMetrics.ThreadCount,
                        Status = GetResourceStatus(systemMetrics.ThreadCount, 100, 200)
                    },
                    GarbageCollection = systemMetrics.GcCollectionCounts,
                    TotalAllocatedBytes = systemMetrics.TotalAllocatedBytes
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error obteniendo métricas del sistema", details = ex.Message });
            }
        }

        /// <summary>
        /// Reinicia todas las métricas (solo para desarrollo/testing)
        /// </summary>
        [HttpPost("reset")]
        public IActionResult ResetMetrics()
        {
            try
            {
                _metricsService.Reset();
                return Ok(new { message = "Métricas reiniciadas exitosamente", timestamp = DateTimeOffset.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error reiniciando métricas", details = ex.Message });
            }
        }

        private static PerformanceHealth CalculateOverallHealth(
            Dictionary<string, OperationStats> operationMetrics, 
            List<Alert> activeAlerts)
        {
            // Si hay alertas críticas, el sistema está crítico
            if (activeAlerts.Any(a => a.Severity == AlertSeverity.Critical))
                return PerformanceHealth.Critical;

            // Si hay operaciones críticas, el sistema está crítico
            if (operationMetrics.Any(kvp => kvp.Value.GetHealthStatus() == PerformanceHealth.Critical))
                return PerformanceHealth.Critical;

            // Si hay alertas de warning o operaciones con warning, el sistema tiene warning
            if (activeAlerts.Any(a => a.Severity == AlertSeverity.Warning) ||
                operationMetrics.Any(kvp => kvp.Value.GetHealthStatus() == PerformanceHealth.Warning))
                return PerformanceHealth.Warning;

            return PerformanceHealth.Healthy;
        }

        private static string GetResourceStatus(double value, double warningThreshold, double criticalThreshold)
        {
            if (value >= criticalThreshold) return "Critical";
            if (value >= warningThreshold) return "Warning";
            return "Healthy";
        }

        private static string GetMemoryStatus(long usedMB, long availableMB)
        {
            if (availableMB == 0) return "Critical";
            
            var usagePercent = (double)usedMB / (usedMB + availableMB) * 100;
            return GetResourceStatus(usagePercent, 70, 90);
        }
    }
}
