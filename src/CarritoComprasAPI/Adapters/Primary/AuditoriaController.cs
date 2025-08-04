using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.EventSourcing;
using CarritoComprasAPI.Core.EventSourcing.Store;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controlador para consultas de auditoría y Event Sourcing
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditQueryService _auditQueryService;
        private readonly ILogger<AuditoriaController> _logger;
        private const string ErrorInternoServidor = "Error interno del servidor";

        public AuditoriaController(IAuditQueryService auditQueryService, ILogger<AuditoriaController> logger)
        {
            _auditQueryService = auditQueryService ?? throw new ArgumentNullException(nameof(auditQueryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los eventos de auditoría con paginación
        /// </summary>
        /// <param name="skip">Número de eventos a omitir</param>
        /// <param name="take">Número de eventos a tomar</param>
        /// <returns>Lista paginada de eventos</returns>
        [HttpGet("eventos")]
        public async Task<ActionResult<IEnumerable<StoredEvent>>> GetAllEvents(
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 100)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los eventos de auditoría - skip: {Skip}, take: {Take}", skip, take);

                if (take > 1000)
                    take = 1000; // Límite máximo

                var events = await _auditQueryService.GetAllEventsAsync(skip, take);
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener eventos de auditoría");
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene eventos por tipo específico
        /// </summary>
        /// <param name="tipo">Tipo de evento a filtrar</param>
        /// <param name="desde">Fecha desde (opcional)</param>
        /// <param name="hasta">Fecha hasta (opcional)</param>
        /// <returns>Eventos filtrados por tipo</returns>
        [HttpGet("eventos/tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<StoredEvent>>> GetEventsByType(
            string tipo,
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            try
            {
                _logger.LogInformation("Obteniendo eventos por tipo: {Tipo}", tipo);

                var events = await _auditQueryService.GetEventsByTypeAsync(tipo, desde, hasta);
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener eventos por tipo {Tipo}", tipo);
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene eventos por usuario específico
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="desde">Fecha desde (opcional)</param>
        /// <param name="hasta">Fecha hasta (opcional)</param>
        /// <returns>Eventos del usuario</returns>
        [HttpGet("eventos/usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<StoredEvent>>> GetEventsByUser(
            string usuarioId,
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            try
            {
                _logger.LogInformation("Obteniendo eventos por usuario: {UsuarioId}", usuarioId);

                var events = await _auditQueryService.GetEventsByUserAsync(usuarioId, desde, hasta);
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener eventos por usuario {UsuarioId}", usuarioId);
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial completo de un agregado específico
        /// </summary>
        /// <param name="agregadoId">ID del agregado</param>
        /// <returns>Historial completo del agregado</returns>
        [HttpGet("eventos/agregado/{agregadoId}")]
        public async Task<ActionResult<IEnumerable<StoredEvent>>> GetEventsByAggregate(string agregadoId)
        {
            try
            {
                _logger.LogInformation("Obteniendo historial del agregado: {AgregadoId}", agregadoId);

                var events = await _auditQueryService.GetEventsByAggregateAsync(agregadoId);
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial del agregado {AgregadoId}", agregadoId);
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene eventos recientes del sistema
        /// </summary>
        /// <param name="horas">Número de horas hacia atrás (por defecto 24)</param>
        /// <returns>Eventos recientes</returns>
        [HttpGet("eventos/recientes")]
        public async Task<ActionResult<IEnumerable<StoredEvent>>> GetRecentEvents([FromQuery] int horas = 24)
        {
            try
            {
                _logger.LogInformation("Obteniendo eventos recientes de las últimas {Horas} horas", horas);

                if (horas > 168) // Máximo una semana
                    horas = 168;

                var events = await _auditQueryService.GetRecentEventsAsync(horas);
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener eventos recientes");
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Genera un reporte de auditoría para un período específico
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Reporte de auditoría detallado</returns>
        [HttpGet("reporte")]
        public async Task<ActionResult<AuditReport>> GenerateAuditReport(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta)
        {
            try
            {
                _logger.LogInformation("Generando reporte de auditoría del {Desde} al {Hasta}", desde, hasta);

                if (hasta <= desde)
                {
                    return BadRequest(new { mensaje = "La fecha 'hasta' debe ser posterior a la fecha 'desde'" });
                }

                var diferenciaDias = (hasta - desde).TotalDays;
                if (diferenciaDias > 90)
                {
                    return BadRequest(new { mensaje = "El período del reporte no puede exceder 90 días" });
                }

                var report = await _auditQueryService.GenerateAuditReportAsync(desde, hasta);
                
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de auditoría");
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene estadísticas resumidas del sistema de auditoría
        /// </summary>
        /// <returns>Estadísticas del sistema</returns>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetAuditStatistics()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de auditoría");

                var hoy = DateTime.UtcNow.Date;
                var haceSieteDias = hoy.AddDays(-7);

                var eventosRecientes = await _auditQueryService.GetRecentEventsAsync(24);
                var reporte = await _auditQueryService.GenerateAuditReportAsync(haceSieteDias, hoy.AddDays(1).AddMilliseconds(-1));

                var estadisticas = new
                {
                    EventosUltimas24Horas = eventosRecientes.Count(),
                    EventosUltimos7Dias = reporte.TotalEventos,
                    TiposEventosMasComunes = reporte.EventosPorTipo
                        .OrderByDescending(x => x.Value)
                        .Take(5)
                        .ToDictionary(x => x.Key, x => x.Value),
                    UsuariosMasActivos = reporte.EventosPorUsuario
                        .OrderByDescending(x => x.Value)
                        .Take(5)
                        .ToDictionary(x => x.Key, x => x.Value),
                    UltimoEvento = eventosRecientes.FirstOrDefault()?.OccurredOn,
                    TotalEventosSistema = (await _auditQueryService.GetAllEventsAsync(0, 1)).Any() ? "Más de 0 eventos" : "No hay eventos"
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de auditoría");
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }

        /// <summary>
        /// Búsqueda avanzada de eventos con múltiples filtros
        /// </summary>
        /// <param name="request">Parámetros de búsqueda</param>
        /// <returns>Eventos que coinciden con los criterios</returns>
        [HttpPost("buscar")]
        public async Task<ActionResult<IEnumerable<StoredEvent>>> SearchEvents([FromBody] SearchEventsRequest request)
        {
            try
            {
                _logger.LogInformation("Búsqueda avanzada de eventos iniciada");

                var allEvents = await _auditQueryService.GetAllEventsAsync(0, 10000);
                var query = allEvents.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(request.TipoEvento))
                    query = query.Where(e => e.EventType.Contains(request.TipoEvento, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(request.UsuarioId))
                    query = query.Where(e => e.UserId.Contains(request.UsuarioId, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(request.AgregadoId))
                    query = query.Where(e => e.AggregateId.Contains(request.AgregadoId, StringComparison.OrdinalIgnoreCase));

                if (request.FechaDesde.HasValue)
                    query = query.Where(e => e.OccurredOn >= request.FechaDesde.Value);

                if (request.FechaHasta.HasValue)
                    query = query.Where(e => e.OccurredOn <= request.FechaHasta.Value);

                if (!string.IsNullOrEmpty(request.IpAddress))
                    query = query.Where(e => e.IpAddress.Contains(request.IpAddress, StringComparison.OrdinalIgnoreCase));

                // Ordenar y paginar
                var results = query
                    .OrderByDescending(e => e.OccurredOn)
                    .Skip(request.Skip)
                    .Take(Math.Min(request.Take, 1000))
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda avanzada de eventos");
                return StatusCode(500, new { mensaje = ErrorInternoServidor, detalle = ex.Message });
            }
        }
    }

    /// <summary>
    /// Parámetros para búsqueda avanzada de eventos
    /// </summary>
    public class SearchEventsRequest
    {
        public string? TipoEvento { get; set; }
        public string? UsuarioId { get; set; }
        public string? AgregadoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? IpAddress { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
}
