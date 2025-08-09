using CarritoComprasAPI.Core.Caching;
using System.Globalization;

namespace CarritoComprasAPI.Core.Configuration
{
    /// <summary>
    /// Configuración centralizada de la aplicación
    /// </summary>
    public class ApplicationConfiguration
    {
        public CacheConfiguration Cache { get; set; } = new();
        public EventSourcingConfiguration EventSourcing { get; set; } = new();
        public ValidationConfiguration Validation { get; set; } = new();
    }

    /// <summary>
    /// Configuración específica de Event Sourcing
    /// </summary>
    public class EventSourcingConfiguration
    {
        public bool EnableAuditBridge { get; set; } = true;
        public bool EnableEventStore { get; set; } = true;
        public int MaxEventsPerAggregate { get; set; } = 1000;
    }

    /// <summary>
    /// Configuración específica de validaciones
    /// </summary>
    public class ValidationConfiguration
    {
        public bool EnableBusinessValidation { get; set; } = true;
        public bool EnableFluentValidation { get; set; } = true;
        public bool ThrowOnValidationFailure { get; set; } = true;
    }

    /// <summary>
    /// Extensiones para registro de configuración
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApplicationConfiguration(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var appConfig = new ApplicationConfiguration();
            configuration.Bind("Application", appConfig);
            
            services.AddSingleton(appConfig);
            services.AddSingleton(appConfig.Cache);
            services.AddSingleton(appConfig.EventSourcing);
            services.AddSingleton(appConfig.Validation);
            
            return services;
        }
    }
}
