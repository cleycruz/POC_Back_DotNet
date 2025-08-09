using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Ports;
using System.Globalization;

namespace CarritoComprasAPI.Core.Domain.Events
{
    /// <summary>
    /// Implementación del despachador de eventos de dominio
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppLogger _logger;

        public DomainEventDispatcher(IServiceProvider serviceProvider, IAppLogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            if (events == null || !events.Any()) return;

            foreach (var domainEvent in events)
            {
                await DispatchEventAsync(domainEvent, cancellationToken);
            }
        }

        public async Task DispatchEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            if (domainEvent == null) return;

            _logger.LogInformation($"Despachando evento de dominio: {domainEvent.EventType} con ID: {domainEvent.Id}");

            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            try
            {
                // Obtener todos los handlers para este tipo de evento
                var handlers = _serviceProvider.GetServices(handlerType);

                if (!handlers.Any())
                {
                    _logger.LogWarning($"No se encontraron handlers para el evento: {domainEvent.EventType}");
                    return;
                }

                // Ejecutar todos los handlers
                var tasks = new List<Task>();
                foreach (var handler in handlers)
                {
                    var handleMethod = handlerType.GetMethod("Handle");
                    if (handleMethod != null)
                    {
                        var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                        tasks.Add(task);
                    }
                }

                await Task.WhenAll(tasks);
                _logger.LogInformation($"Evento {domainEvent.EventType} procesado exitosamente por {tasks.Count} handler(s)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al despachar evento {domainEvent.EventType}: {ex.Message}", ex);
                throw;
            }
        }
    }

    /// <summary>
    /// Extensiones para configurar eventos de dominio
    /// </summary>
    public static class DomainEventExtensions
    {
        /// <summary>
        /// Registra los servicios de eventos de dominio
        /// </summary>
        public static IServiceCollection AddDomainEvents(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            // Registrar todos los handlers de eventos de dominio automáticamente
            var assembly = typeof(DomainEventExtensions).Assembly;
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && 
                               i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, handlerType);
                }
            }

            return services;
        }

        /// <summary>
        /// Despacha eventos de dominio de una entidad y los limpia
        /// </summary>
        public static async Task DispatchAndClearEvents(
            this IDomainEventDispatcher dispatcher, 
            DomainEntity entity, 
            CancellationToken cancellationToken = default)
        {
            if (entity == null || !entity.HasDomainEvents) return;

            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();
            
            await dispatcher.DispatchEventsAsync(events, cancellationToken);
        }
    }
}
