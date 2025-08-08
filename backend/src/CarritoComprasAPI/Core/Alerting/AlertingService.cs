using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Logging;

namespace CarritoComprasAPI.Core.Alerting
{
    /// <summary>
    /// Servicio de alertas para monitoreo de rendimiento
    /// </summary>
    public interface IAlertingService
    {
        /// <summary>
        /// Evalúa métricas y genera alertas si es necesario
        /// </summary>
        Task EvaluateMetricsAsync();
        
        /// <summary>
        /// Obtiene alertas activas
        /// </summary>
        List<Alert> GetActiveAlerts();
        
        /// <summary>
        /// Limpia alertas resueltas
        /// </summary>
        void ClearResolvedAlerts();
    }

    /// <summary>
    /// Implementación del servicio de alertas
    /// </summary>
    public class AlertingService : IAlertingService
    {
        private readonly IPerformanceMetricsService _metricsService;
        private readonly IStructuredLogger _logger;
        private readonly List<Alert> _activeAlerts = new();
        private readonly object _alertsLock = new();

        // Configuración de umbrales
        private const double WarningThresholdSeconds = 2.0;
        private const double CriticalThresholdSeconds = 5.0;
        private const double SuccessRateWarningThreshold = 95.0;
        private const double SuccessRateCriticalThreshold = 90.0;

        public AlertingService(IPerformanceMetricsService metricsService, IStructuredLogger logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task EvaluateMetricsAsync()
        {
            try
            {
                var allMetrics = _metricsService.GetAllMetrics();
                var newAlerts = new List<Alert>();

                foreach (var kvp in allMetrics)
                {
                    var operationName = kvp.Key;
                    var stats = kvp.Value;

                    // Evaluar duración promedio
                    EvaluateAverageDuration(operationName, stats, newAlerts);
                    
                    // Evaluar percentil 95
                    EvaluatePercentile95(operationName, stats, newAlerts);
                    
                    // Evaluar tasa de éxito
                    EvaluateSuccessRate(operationName, stats, newAlerts);
                }

                // Actualizar alertas activas
                lock (_alertsLock)
                {
                    // Marcar alertas resueltas
                    foreach (var existingAlert in _activeAlerts.Where(a => a.Status == AlertStatus.Active))
                    {
                        if (!newAlerts.Any(na => na.OperationName == existingAlert.OperationName && 
                                                na.AlertType == existingAlert.AlertType))
                        {
                            existingAlert.Status = AlertStatus.Resolved;
                            existingAlert.ResolvedAt = DateTimeOffset.UtcNow;
                            
                            _logger.LogOperacionDominio(
                                "AlertResolved",
                                "Alerting",
                                existingAlert.OperationName,
                                $"Alerta resuelta: {existingAlert.Message}",
                                new { 
                                    AlertId = existingAlert.Id,
                                    AlertType = existingAlert.AlertType.ToString(),
                                    Severity = existingAlert.Severity.ToString()
                                }
                            );
                        }
                    }

                    // Agregar nuevas alertas
                    foreach (var newAlert in newAlerts)
                    {
                        if (!_activeAlerts.Any(a => a.OperationName == newAlert.OperationName && 
                                                   a.AlertType == newAlert.AlertType && 
                                                   a.Status == AlertStatus.Active))
                        {
                            _activeAlerts.Add(newAlert);
                            
                            _logger.LogOperacionDominio(
                                "AlertTriggered",
                                "Alerting",
                                newAlert.OperationName,
                                $"Nueva alerta: {newAlert.Message}",
                                new { 
                                    AlertId = newAlert.Id,
                                    AlertType = newAlert.AlertType.ToString(),
                                    Severity = newAlert.Severity.ToString(),
                                    Threshold = newAlert.Threshold,
                                    CurrentValue = newAlert.CurrentValue
                                }
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("AlertEvaluation", ex, new { });
            }
            
            return Task.CompletedTask;
        }

        private void EvaluateAverageDuration(string operationName, OperationStats stats, List<Alert> newAlerts)
        {
            var avgSeconds = stats.AverageDuration.TotalSeconds;
            
            if (avgSeconds >= CriticalThresholdSeconds)
            {
                newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationName = operationName,
                    AlertType = AlertType.AverageDuration,
                    Severity = AlertSeverity.Critical,
                    Message = $"Duración promedio crítica: {avgSeconds:F2}s (umbral: {CriticalThresholdSeconds}s)",
                    Threshold = CriticalThresholdSeconds,
                    CurrentValue = avgSeconds,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = AlertStatus.Active
                });
            }
            else if (avgSeconds >= WarningThresholdSeconds)
            {
                newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationName = operationName,
                    AlertType = AlertType.AverageDuration,
                    Severity = AlertSeverity.Warning,
                    Message = $"Duración promedio elevada: {avgSeconds:F2}s (umbral: {WarningThresholdSeconds}s)",
                    Threshold = WarningThresholdSeconds,
                    CurrentValue = avgSeconds,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = AlertStatus.Active
                });
            }
        }

        private void EvaluatePercentile95(string operationName, OperationStats stats, List<Alert> newAlerts)
        {
            var p95Seconds = stats.GetPercentile(95) / 1000.0; // Convertir de ms a segundos
            
            if (p95Seconds >= CriticalThresholdSeconds)
            {
                newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationName = operationName,
                    AlertType = AlertType.Percentile95,
                    Severity = AlertSeverity.Critical,
                    Message = $"Percentil 95 crítico: {p95Seconds:F2}s (umbral: {CriticalThresholdSeconds}s)",
                    Threshold = CriticalThresholdSeconds,
                    CurrentValue = p95Seconds,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = AlertStatus.Active
                });
            }
            else if (p95Seconds >= WarningThresholdSeconds)
            {
                newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationName = operationName,
                    AlertType = AlertType.Percentile95,
                    Severity = AlertSeverity.Warning,
                    Message = $"Percentil 95 elevado: {p95Seconds:F2}s (umbral: {WarningThresholdSeconds}s)",
                    Threshold = WarningThresholdSeconds,
                    CurrentValue = p95Seconds,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = AlertStatus.Active
                });
            }
        }

        private void EvaluateSuccessRate(string operationName, OperationStats stats, List<Alert> newAlerts)
        {
            var successRate = stats.SuccessRate;
            
            if (successRate <= SuccessRateCriticalThreshold)
            {
                newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationName = operationName,
                    AlertType = AlertType.SuccessRate,
                    Severity = AlertSeverity.Critical,
                    Message = $"Tasa de éxito crítica: {successRate:F1}% (umbral: {SuccessRateCriticalThreshold}%)",
                    Threshold = SuccessRateCriticalThreshold,
                    CurrentValue = successRate,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = AlertStatus.Active
                });
            }
            else if (successRate <= SuccessRateWarningThreshold)
            {
                newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    OperationName = operationName,
                    AlertType = AlertType.SuccessRate,
                    Severity = AlertSeverity.Warning,
                    Message = $"Tasa de éxito baja: {successRate:F1}% (umbral: {SuccessRateWarningThreshold}%)",
                    Threshold = SuccessRateWarningThreshold,
                    CurrentValue = successRate,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = AlertStatus.Active
                });
            }
        }

        public List<Alert> GetActiveAlerts()
        {
            lock (_alertsLock)
            {
                return _activeAlerts.Where(a => a.Status == AlertStatus.Active).ToList();
            }
        }

        public void ClearResolvedAlerts()
        {
            lock (_alertsLock)
            {
                _activeAlerts.RemoveAll(a => a.Status == AlertStatus.Resolved && 
                                            a.ResolvedAt?.AddMinutes(10) < DateTimeOffset.UtcNow);
            }
        }
    }

    /// <summary>
    /// Representa una alerta del sistema
    /// </summary>
    public class Alert
    {
        public string Id { get; set; } = string.Empty;
        public string OperationName { get; set; } = string.Empty;
        public AlertType AlertType { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public double Threshold { get; set; }
        public double CurrentValue { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public AlertStatus Status { get; set; }
    }

    /// <summary>
    /// Tipos de alertas
    /// </summary>
    public enum AlertType
    {
        AverageDuration,
        Percentile95,
        SuccessRate,
        SystemResource
    }

    /// <summary>
    /// Severidad de las alertas
    /// </summary>
    public enum AlertSeverity
    {
        Info,
        Warning,
        Critical
    }

    /// <summary>
    /// Estado de las alertas
    /// </summary>
    public enum AlertStatus
    {
        Active,
        Resolved
    }
}
