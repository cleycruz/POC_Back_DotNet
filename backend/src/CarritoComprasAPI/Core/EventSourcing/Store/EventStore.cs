using System.Text.Json;
using CarritoComprasAPI.Core.Ports;
using System.Globalization;

namespace CarritoComprasAPI.Core.EventSourcing.Store
{
    /// <summary>
    /// Interfaz para el repositorio de eventos
    /// </summary>
    public interface IEventStore
    {
        Task<IEnumerable<EventBase>> GetEventsAsync(string aggregateId, long fromVersion = 0, CancellationToken cancellationToken = default);
        Task SaveEventsAsync(string aggregateId, IEnumerable<EventBase> events, long expectedVersion, CancellationToken cancellationToken = default);
        Task<IEnumerable<StoredEvent>> GetAllStoredEventsAsync();
    }

    /// <summary>
    /// Implementación simplificada en memoria del Event Store
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<string, List<StoredEvent>> _events = new();
        private readonly List<StoredEvent> _allEvents = new();
        private readonly object _lock = new object();

        public Task<IEnumerable<EventBase>> GetEventsAsync(string aggregateId, long fromVersion = 0, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (!_events.ContainsKey(aggregateId))
                {
                    return Task.FromResult(Enumerable.Empty<EventBase>());
                }

                var events = _events[aggregateId]
                    .Where(e => e.Version >= fromVersion)
                    .Select(DeserializeEvent)
                    .Where(e => e != null)
                    .Cast<EventBase>()
                    .ToList();

                return Task.FromResult(events.AsEnumerable());
            }
        }

        public Task SaveEventsAsync(string aggregateId, IEnumerable<EventBase> events, long expectedVersion, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(aggregateId))
                throw new ArgumentException("AggregateId no puede ser nulo o vacío", nameof(aggregateId));

            if (events == null || !events.Any())
                throw new ArgumentException("Debe proporcionar al menos un evento", nameof(events));

            lock (_lock)
            {
                // Verificar versión esperada para control de concurrencia
                if (!_events.ContainsKey(aggregateId))
                {
                    _events[aggregateId] = new List<StoredEvent>();
                }

                var existingEvents = _events[aggregateId];
                var currentVersion = existingEvents.Count;

                if (expectedVersion > 0 && currentVersion != expectedVersion)
                {
                    throw new InvalidOperationException(
                        $"Conflicto de concurrencia para agregado {aggregateId}. Versión esperada: {expectedVersion}, versión actual: {currentVersion}");
                }

                // Procesar cada evento
                var newVersion = currentVersion;
                foreach (var eventData in events)
                {
                    newVersion++;

                    var storedEvent = new StoredEvent
                    {
                        AggregateId = aggregateId,
                        EventId = eventData.EventId,
                        EventType = eventData.GetType().Name,
                        AggregateType = eventData.AggregateType,
                        Data = JsonSerializer.Serialize(eventData, eventData.GetType()),
                        Version = newVersion,
                        OccurredOn = eventData.OccurredOn,
                        
                        // Datos de auditoría
                        UserId = eventData.UserId ?? "system",
                        UserName = eventData.UserName ?? "Sistema",
                        IpAddress = eventData.IpAddress ?? "localhost",
                        UserAgent = eventData.UserAgent ?? "Unknown",
                        Metadata = JsonSerializer.Serialize(eventData.Metadata ?? new Dictionary<string, object>())
                    };

                    existingEvents.Add(storedEvent);
                    _allEvents.Add(storedEvent);
                }

                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<StoredEvent>> GetAllStoredEventsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_allEvents.AsEnumerable());
            }
        }

        private EventBase? DeserializeEvent(StoredEvent storedEvent)
        {
            try
            {
                // Crear un tipo básico que podamos deserializar
                var baseEvent = JsonSerializer.Deserialize<Dictionary<string, object>>(storedEvent.Data);
                if (baseEvent == null) return null;

                // Crear un evento base con la información disponible
                return new GenericEventBase
                {
                    EventId = storedEvent.EventId,
                    AggregateId = storedEvent.AggregateId,
                    AggregateType = storedEvent.AggregateType,
                    Version = storedEvent.Version,
                    OccurredOn = storedEvent.OccurredOn,
                    UserId = storedEvent.UserId,
                    UserName = storedEvent.UserName,
                    IpAddress = storedEvent.IpAddress,
                    UserAgent = storedEvent.UserAgent,
                    Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(storedEvent.Metadata) ?? new()
                };
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Evento genérico para deserialización
    /// </summary>
    public record GenericEventBase : EventBase
    {
        public GenericEventBase() : base("", "", 0) { }
    }
}
