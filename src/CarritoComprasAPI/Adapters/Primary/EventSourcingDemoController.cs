using CarritoComprasAPI.Core.EventSourcing;
using CarritoComprasAPI.Core.EventSourcing.Store;
using CarritoComprasAPI.Core.EventSourcing.Events;
using Microsoft.AspNetCore.Mvc;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controlador para demostrar y probar el sistema de Event Sourcing
    /// Solo para fines de desarrollo y testing
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EventSourcingDemoController : ControllerBase
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger<EventSourcingDemoController> _logger;
        private const string CarritoTestId = "carrito-001";
        private const string UsuarioTestId = "user123";

        public EventSourcingDemoController(
            IEventStore eventStore,
            ILogger<EventSourcingDemoController> logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Test simple para verificar que el controlador responde
        /// </summary>
        [HttpGet("test")]
        public ActionResult<object> Test()
        {
            return Ok(new { mensaje = "EventSourcingDemoController está funcionando", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Genera eventos de muestra para probar el sistema de Event Sourcing
        /// </summary>
        /// <returns>Resultado de la generación de eventos</returns>
        [HttpPost("generar-eventos-muestra")]
        public async Task<ActionResult<object>> GenerarEventosMuestra()
        {
            try
            {
                _logger.LogInformation("Generando eventos de muestra para testing");

                var eventos = new List<EventBase>();

                // Eventos de productos
                eventos.Add(new ProductoCreadoEvent(
                    productoId: 1,
                    nombre: "Laptop Gaming",
                    descripcion: "Laptop para gaming de alta gama",
                    precio: 1500.00m,
                    stock: 10,
                    categoria: "Electrónicos"
                ));

                eventos.Add(new ProductoCreadoEvent(
                    productoId: 2,
                    nombre: "Mouse Inalámbrico",
                    descripcion: "Mouse ergonómico inalámbrico",
                    precio: 25.99m,
                    stock: 50,
                    categoria: "Accesorios"
                ));

                // Evento de stock cambio
                eventos.Add(new ProductoStockCambiadoEvent(
                    productoId: 1,
                    stockAnterior: 10,
                    stockNuevo: 8,
                    motivo: "Venta",
                    version: 2
                ));

                // Eventos de carritos
                eventos.Add(new CarritoCreadoEvent(
                    carritoId: CarritoTestId,
                    usuarioId: UsuarioTestId
                ));

                eventos.Add(new ItemAgregadoAlCarritoEvent(
                    carritoId: CarritoTestId,
                    usuarioId: UsuarioTestId,
                    productoId: 1,
                    nombreProducto: "Laptop Gaming",
                    precioUnitario: 1500.00m,
                    cantidad: 1,
                    subtotal: 1500.00m,
                    cantidadAnterior: 0,
                    esNuevoItem: true,
                    version: 2
                ));

                eventos.Add(new ItemAgregadoAlCarritoEvent(
                    carritoId: CarritoTestId,
                    usuarioId: UsuarioTestId,
                    productoId: 2,
                    nombreProducto: "Mouse Inalámbrico",
                    precioUnitario: 25.99m,
                    cantidad: 2,
                    subtotal: 51.98m,
                    cantidadAnterior: 0,
                    esNuevoItem: true,
                    version: 3
                ));

                // Eventos de sistema y auditoría
                eventos.Add(new UsuarioInicioSesionEvent(
                    usuarioId: UsuarioTestId,
                    nombreUsuario: "Juan Pérez",
                    ipOrigen: "192.168.1.100",
                    userAgent: "Mozilla/5.0",
                    esExitoso: true
                ));

                eventos.Add(new ErrorSistemaEvent(
                    tipoError: "ValidationException",
                    mensaje: "Error de validación en producto",
                    stackTrace: "Stack trace del error...",
                    operacion: "CrearProducto"
                ));

                // Guardar todos los eventos con contexto de auditoría
                foreach (var evento in eventos)
                {
                    await _eventStore.SaveEventsAsync(evento.AggregateId, new[] { evento }, 0);
                    
                    // Pequeña pausa para simular eventos en diferentes momentos
                    await Task.Delay(10);
                }

                var resultado = new
                {
                    mensaje = "Eventos de muestra generados exitosamente",
                    totalEventos = eventos.Count,
                    tipos = eventos.GroupBy(e => e.GetType().Name)
                                  .ToDictionary(g => g.Key, g => g.Count()),
                    agregados = eventos.GroupBy(e => e.AggregateId)
                                      .ToDictionary(g => g.Key, g => g.Count())
                };

                _logger.LogInformation("Generación de eventos completada: {TotalEventos} eventos creados", eventos.Count);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar eventos de muestra");
                return StatusCode(500, new { mensaje = "Error al generar eventos", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Genera un evento de producto simple
        /// </summary>
        /// <param name="request">Datos del producto</param>
        /// <returns>Resultado de la creación del evento</returns>
        [HttpPost("crear-producto-evento")]
        public async Task<ActionResult<object>> CrearProductoEvento([FromBody] CrearProductoRequest request)
        {
            try
            {
                _logger.LogInformation("Creando evento de producto: {Nombre}", request.Nombre);

                var evento = new ProductoCreadoEvent(
                    productoId: request.ProductoId,
                    nombre: request.Nombre,
                    descripcion: request.Descripcion,
                    precio: request.Precio,
                    stock: request.Stock,
                    categoria: request.Categoria
                );

                await _eventStore.SaveEventsAsync(evento.AggregateId, new[] { evento }, 0);

                var resultado = new
                {
                    mensaje = "Evento de producto creado exitosamente",
                    eventoId = evento.EventId,
                    tipo = evento.GetType().Name,
                    agregadoId = evento.AggregateId,
                    timestamp = evento.OccurredOn
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear evento de producto");
                return StatusCode(500, new { mensaje = "Error al crear evento", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Crea un evento de carrito
        /// </summary>
        /// <param name="request">Datos del carrito</param>
        /// <returns>Resultado de la creación del evento</returns>
        [HttpPost("crear-carrito-evento")]
        public async Task<ActionResult<object>> CrearCarritoEvento([FromBody] CrearCarritoRequest request)
        {
            try
            {
                _logger.LogInformation("Creando evento de carrito: {CarritoId}", request.CarritoId);

                var evento = new CarritoCreadoEvent(
                    carritoId: request.CarritoId,
                    usuarioId: request.UsuarioId
                );

                await _eventStore.SaveEventsAsync(evento.AggregateId, new[] { evento }, 0);

                var resultado = new
                {
                    mensaje = "Evento de carrito creado exitosamente",
                    eventoId = evento.EventId,
                    tipo = evento.GetType().Name,
                    agregadoId = evento.AggregateId,
                    timestamp = evento.OccurredOn
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear evento de carrito");
                return StatusCode(500, new { mensaje = "Error al crear evento", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información del estado del Event Store
        /// </summary>
        /// <returns>Estadísticas del Event Store</returns>
        [HttpGet("estado-event-store")]
        public ActionResult<object> GetEventStoreStatus()
        {
            try
            {
                // Para obtener todos los eventos, tendríamos que agregar un método específico al EventStore
                // Por ahora, simulamos algunas estadísticas básicas
                var estadisticas = new
                {
                    mensaje = "Event Store en funcionamiento",
                    tipo = "InMemoryEventStore",
                    estado = "Operativo",
                    nota = "Para estadísticas detalladas, usar el AuditoriaController",
                    timestamp = DateTime.UtcNow
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado del Event Store");
                return StatusCode(500, new { mensaje = "Error al obtener estado", detalle = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request para crear eventos de productos
    /// </summary>
    public class CrearProductoRequest
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request para crear eventos de carritos
    /// </summary>
    public class CrearCarritoRequest
    {
        public string CarritoId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
    }
}
