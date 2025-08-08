using CarritoComprasAPI.Core.EventSourcing.Store;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.EventSourcing
{
    /// <summary>
    /// Excepción de concurrencia para Event Sourcing
    /// </summary>
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message) { }
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Repositorio base para agregados con Event Sourcing
    /// </summary>
    public interface IEventSourcedRepository<T> where T : class, IEventSourcedAggregate
    {
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task SaveAsync(T aggregate, CancellationToken cancellationToken = default);
        Task<IEnumerable<StoredEvent>> GetHistoryAsync(string id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementación base del repositorio con Event Sourcing
    /// </summary>
    public abstract class EventSourcedRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourcedAggregate
    {
        private readonly IEventStore _eventStore;
        private readonly IAppLogger _logger;

        protected EventSourcedRepository(IEventStore eventStore, IAppLogger logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract T CreateAggregate();

        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var events = await _eventStore.GetEventsAsync(id, 0, cancellationToken);
                if (!events.Any())
                    return null;

                var aggregate = CreateAggregate();
                aggregate.LoadFromHistory(events);
                return aggregate;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar el agregado {typeof(T).Name} con ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task SaveAsync(T aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            if (!aggregate.UncommittedEvents.Any())
            {
                _logger.LogInformation($"No hay eventos sin confirmar para el agregado {typeof(T).Name} con ID {aggregate.Id}");
                return;
            }

            try
            {
                var expectedVersion = aggregate.Version - aggregate.UncommittedEvents.Count;
                await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.UncommittedEvents, expectedVersion, cancellationToken);
                
                aggregate.MarkEventsAsCommitted();

                _logger.LogInformation($"Guardados {aggregate.UncommittedEvents.Count} eventos para el agregado {typeof(T).Name} con ID {aggregate.Id}");
            }
            catch (ConcurrencyException)
            {
                _logger.LogWarning($"Conflicto de concurrencia al guardar agregado {typeof(T).Name} con ID {aggregate.Id}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar agregado {typeof(T).Name} con ID {aggregate.Id}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetHistoryAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
                return Enumerable.Empty<StoredEvent>();

            try
            {
                // Como IEventStore.GetEventsAsync devuelve EventBase, necesitamos convertir o usar GetAllStoredEventsAsync
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                return allEvents.Where(e => e.AggregateId == id).OrderBy(e => e.Version);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener historial del agregado {typeof(T).Name} con ID {id}: {ex.Message}");
                throw;
            }
        }

        protected abstract T CreateEmptyAggregate();
    }

    /// <summary>
    /// Servicio para consultas de auditoría
    /// </summary>
    public interface IAuditQueryService
    {
        Task<IEnumerable<StoredEvent>> GetAllEventsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
        Task<IEnumerable<StoredEvent>> GetEventsByTypeAsync(string eventType, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<StoredEvent>> GetEventsByUserAsync(string userId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<StoredEvent>> GetEventsByAggregateAsync(string aggregateId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StoredEvent>> GetRecentEventsAsync(int hours = 24, CancellationToken cancellationToken = default);
        Task<AuditReport> GenerateAuditReportAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }

    // Nota: AuditQueryService original removido para usar SimpleAuditQueryService
    // que es compatible con la interfaz IEventStore actual

    /// <summary>
    /// Reporte de auditoría
    /// </summary>
    public class AuditReport
    {
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
        public int TotalEventos { get; set; }
        public Dictionary<string, int> EventosPorTipo { get; set; } = new();
        public Dictionary<string, int> EventosPorUsuario { get; set; } = new();
        public Dictionary<DateTime, int> EventosPorDia { get; set; } = new();
        public List<StoredEvent> EventosRecientes { get; set; } = new();
        public DateTime GeneradoEn { get; set; } = DateTime.UtcNow;
    }
}
