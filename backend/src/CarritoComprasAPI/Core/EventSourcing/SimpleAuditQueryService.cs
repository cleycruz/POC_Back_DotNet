using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.EventSourcing.Store;
using System.Globalization;

namespace CarritoComprasAPI.Core.EventSourcing
{
    /// <summary>
    /// Servicio simplificado para consultas de auditoría
    /// </summary>
    public class SimpleAuditQueryService : IAuditQueryService
    {
        private readonly IEventStore _eventStore;
        private readonly IAppLogger _logger;

        public SimpleAuditQueryService(IEventStore eventStore, IAppLogger logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<StoredEvent>> GetAllEventsAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            try
            {
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                return allEvents
                    .OrderByDescending(e => e.OccurredOn)
                    .Skip(skip)
                    .Take(take);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los eventos: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetEventsByTypeAsync(string eventType, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                var filtered = allEvents.Where(e => e.EventType.Contains(eventType, StringComparison.OrdinalIgnoreCase));
                
                if (from.HasValue)
                    filtered = filtered.Where(e => e.OccurredOn >= from.Value);
                    
                if (to.HasValue)
                    filtered = filtered.Where(e => e.OccurredOn <= to.Value);
                    
                return filtered.OrderByDescending(e => e.OccurredOn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener eventos por tipo {eventType}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetEventsByUserAsync(string userId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                var filtered = allEvents.Where(e => e.UserId.Contains(userId, StringComparison.OrdinalIgnoreCase));
                
                if (from.HasValue)
                    filtered = filtered.Where(e => e.OccurredOn >= from.Value);
                    
                if (to.HasValue)
                    filtered = filtered.Where(e => e.OccurredOn <= to.Value);
                    
                return filtered.OrderByDescending(e => e.OccurredOn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener eventos por usuario {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetEventsByAggregateAsync(string aggregateId, CancellationToken cancellationToken = default)
        {
            try
            {
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                return allEvents
                    .Where(e => e.AggregateId == aggregateId)
                    .OrderBy(e => e.OccurredOn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener eventos por agregado {aggregateId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetRecentEventsAsync(int hours = 24, CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = DateTime.UtcNow.AddHours(-hours);
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                return allEvents
                    .Where(e => e.OccurredOn >= cutoff)
                    .OrderByDescending(e => e.OccurredOn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener eventos recientes: {ex.Message}");
                throw;
            }
        }

        public async Task<AuditReport> GenerateAuditReportAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            try
            {
                var allEvents = await _eventStore.GetAllStoredEventsAsync();
                var filteredEvents = allEvents
                    .Where(e => e.OccurredOn >= from && e.OccurredOn <= to)
                    .ToList();

                return new AuditReport
                {
                    PeriodoInicio = from,
                    PeriodoFin = to,
                    TotalEventos = filteredEvents.Count,
                    EventosPorTipo = filteredEvents
                        .GroupBy(e => e.EventType)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    EventosPorUsuario = filteredEvents
                        .GroupBy(e => e.UserId)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    EventosPorDia = filteredEvents
                        .GroupBy(e => e.OccurredOn.Date)
                        .ToDictionary(g => g.Key, g => g.Count())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar reporte de auditoría: {ex.Message}");
                throw;
            }
        }
    }
}
