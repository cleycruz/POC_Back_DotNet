using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Alerting;
using CarritoComprasAPI.Core.BackgroundServices;
using CarritoComprasAPI.Core.Logging;

namespace CarritoComprasAPI.Core.Configuration
{
    /// <summary>
    /// Métodos de extensión para configurar servicios de performance y alertas
    /// </summary>
    public static class PerformanceServiceExtensions
    {
        /// <summary>
        /// Registra todos los servicios relacionados con performance y alertas
        /// </summary>
        public static IServiceCollection AddPerformanceAndAlerting(this IServiceCollection services)
        {
            // Registrar servicios de performance como Singleton para persistir métricas entre requests
            services.AddSingleton<IPerformanceMetricsService>(provider => 
            {
                // Crear logger usando el provider para resolver dependencias por request cuando sea necesario
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var logger = new StructuredLogger(loggerFactory.CreateLogger<StructuredLogger>());
                return new PerformanceMetricsService(logger);
            });
            
            // Registrar servicios de alertas como Singleton para persistir alertas entre requests
            services.AddSingleton<IAlertingService>(provider =>
            {
                var metricsService = provider.GetRequiredService<IPerformanceMetricsService>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var logger = new StructuredLogger(loggerFactory.CreateLogger<StructuredLogger>());
                return new AlertingService(metricsService, logger);
            });
            
            // Registrar servicio en segundo plano para evaluación de alertas
            services.AddHostedService<AlertEvaluationBackgroundService>();

            return services;
        }
    }
}
