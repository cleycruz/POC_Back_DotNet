using CarritoComprasAPI.Core.Alerting;
using CarritoComprasAPI.Core.Logging;

namespace CarritoComprasAPI.Core.BackgroundServices
{
    /// <summary>
    /// Servicio en segundo plano para evaluación automática de alertas
    /// </summary>
    public class AlertEvaluationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AlertEvaluationBackgroundService> _logger;
        private readonly TimeSpan _evaluationInterval = TimeSpan.FromSeconds(30); // Evaluar cada 30 segundos

        public AlertEvaluationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<AlertEvaluationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alert Evaluation Background Service iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var alertingService = scope.ServiceProvider.GetRequiredService<IAlertingService>();
                    
                    // Evaluar métricas y generar alertas
                    await alertingService.EvaluateMetricsAsync();
                    
                    // Limpiar alertas resueltas antiguas
                    alertingService.ClearResolvedAlerts();
                    
                    _logger.LogDebug("Evaluación de alertas completada");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la evaluación de alertas");
                }

                try
                {
                    await Task.Delay(_evaluationInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Se esperaba la cancelación, salir del bucle
                    break;
                }
            }

            _logger.LogInformation("Alert Evaluation Background Service detenido");
        }
    }
}
