using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Domain.Events.Productos;
using CarritoComprasAPI.Core.Domain.Events.Carrito;
using CarritoComprasAPI.Core.EventSourcing.Events;
using CarritoComprasAPI.Core.EventSourcing.Store;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.EventSourcing
{
    /// <summary>
    /// Puente automático que intercepta Domain Events y los persiste en el Event Store para auditoría
    /// </summary>
    public class DomainEventToEventStoreBridge : 
        IDomainEventHandler<ProductoCreado>,
        IDomainEventHandler<ProductoEliminado>,
        IDomainEventHandler<CarritoCreado>,
        IDomainEventHandler<ItemAgregadoAlCarrito>
    {
        private readonly IEventStore _eventStore;
        private readonly IAuditContextProvider _auditContextProvider;
        private readonly IAppLogger _logger;

        public DomainEventToEventStoreBridge(
            IEventStore eventStore,
            IAuditContextProvider auditContextProvider,
            IAppLogger logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _auditContextProvider = auditContextProvider ?? throw new ArgumentNullException(nameof(auditContextProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Maneja evento ProductoCreado
        /// </summary>
        public async Task Handle(ProductoCreado domainEvent, CancellationToken cancellationToken = default)
        {
            await HandleGeneric(domainEvent, cancellationToken);
        }

        /// <summary>
        /// Maneja evento ProductoEliminado
        /// </summary>
        public async Task Handle(ProductoEliminado domainEvent, CancellationToken cancellationToken = default)
        {
            await HandleGeneric(domainEvent, cancellationToken);
        }

        /// <summary>
        /// Maneja evento CarritoCreado
        /// </summary>
        public async Task Handle(CarritoCreado domainEvent, CancellationToken cancellationToken = default)
        {
            await HandleGeneric(domainEvent, cancellationToken);
        }

        /// <summary>
        /// Maneja evento ItemAgregadoAlCarrito
        /// </summary>
        public async Task Handle(ItemAgregadoAlCarrito domainEvent, CancellationToken cancellationToken = default)
        {
            await HandleGeneric(domainEvent, cancellationToken);
        }

        /// <summary>
        /// Lógica genérica para manejar cualquier Domain Event convirtiéndolo automáticamente a Event Store Event
        /// </summary>
        private async Task HandleGeneric(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("[AUDIT-BRIDGE] Interceptando Domain Event: {EventType}", domainEvent.GetType().Name);

                // Obtener contexto de auditoría actual
                var auditContext = _auditContextProvider.GetCurrentContext();

                // Convertir Domain Event a Event Store Event
                var eventStoreEvent = ConvertToAuditEvent(domainEvent, auditContext);

                if (eventStoreEvent != null)
                {
                    // Generar un aggregate ID para el evento
                    var aggregateId = GenerateAggregateId(domainEvent);
                    
                    // Persistir en Event Store para auditoría
                    await _eventStore.SaveEventsAsync(aggregateId, new[] { eventStoreEvent }, 0, cancellationToken);
                    _logger.LogInformation("[AUDIT-BRIDGE] Domain Event {EventType} persistido en Event Store", domainEvent.GetType().Name);
                }
                else
                {
                    _logger.LogWarning("[AUDIT-BRIDGE] No se pudo convertir Domain Event: {EventType}", domainEvent.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AUDIT-BRIDGE] Error al procesar Domain Event {EventType}: {ErrorMessage}", domainEvent.GetType().Name, ex.Message);
                // No relanzamos la excepción para no afectar el flujo principal
            }
        }

        /// <summary>
        /// Convierte un Domain Event específico a su correspondiente Event Store Event
        /// </summary>
        private EventBase? ConvertToAuditEvent(DomainEvent domainEvent, AuditContext auditContext)
        {
            return domainEvent switch
            {
                // Eventos de Productos
                ProductoCreado productoCreado => new ProductoCreadoEvent(
                    productoCreado.ProductoId,
                    productoCreado.Nombre,
                    productoCreado.Descripcion,
                    productoCreado.Precio,
                    productoCreado.Stock,
                    productoCreado.Categoria
                ),

                ProductoActualizado productoActualizado => new ProductoActualizadoEvent(
                    productoActualizado.ProductoId,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                )
                {
                    NombreAnterior = productoActualizado.NombreAnterior,
                    NombreNuevo = productoActualizado.NombreNuevo,
                    PrecioAnterior = productoActualizado.PrecioAnterior,
                    PrecioNuevo = productoActualizado.PrecioNuevo,
                    StockAnterior = productoActualizado.StockAnterior,
                    StockNuevo = productoActualizado.StockNuevo,
                    CategoriaAnterior = productoActualizado.CategoriaAnterior,
                    CategoriaNueva = productoActualizado.CategoriaNueva
                },

                ProductoEliminado productoEliminado => new ProductoEliminadoEvent(
                    productoEliminado.ProductoId,
                    productoEliminado.Nombre,
                    string.Empty, // descripcion - no disponible en Domain Event
                    0, // precio - no disponible en Domain Event
                    0, // stock - no disponible en Domain Event
                    productoEliminado.Categoria,
                    "Eliminado", // motivo por defecto
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                ),

                // Eventos de Carrito
                CarritoCreado carritoCreado => new CarritoCreadoEvent(
                    carritoCreado.UsuarioId,
                    carritoCreado.UsuarioId
                ),

                ItemAgregadoAlCarrito itemAgregado => new ItemAgregadoAlCarritoEvent(
                    itemAgregado.UsuarioId, // carritoId
                    itemAgregado.UsuarioId,
                    itemAgregado.ProductoId,
                    itemAgregado.NombreProducto,
                    itemAgregado.PrecioUnitario,
                    itemAgregado.Cantidad,
                    itemAgregado.Subtotal,
                    0, // cantidadAnterior
                    true, // esNuevoItem
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                ),

                ItemEliminadoDelCarrito itemEliminado => new ItemEliminadoDelCarritoEvent(
                    itemEliminado.UsuarioId, // carritoId
                    itemEliminado.UsuarioId,
                    itemEliminado.ProductoId,
                    itemEliminado.NombreProducto,
                    itemEliminado.Cantidad,
                    0, // precioUnitario - no disponible en Domain Event
                    "Eliminado por usuario",
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                ),

                CantidadItemCarritoActualizada cantidadActualizada => new CantidadItemActualizadaEvent(
                    cantidadActualizada.UsuarioId, // carritoId
                    cantidadActualizada.UsuarioId,
                    cantidadActualizada.ProductoId,
                    cantidadActualizada.NombreProducto,
                    cantidadActualizada.CantidadAnterior,
                    cantidadActualizada.CantidadNueva,
                    0, // precioUnitario - no disponible en Domain Event
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                ),

                // Evento genérico para Domain Events no mapeados específicamente
                _ => new OperacionDominioEvent(
                    domainEvent.GetType().Name,
                    ExtractEntityFromDomainEvent(domainEvent),
                    ExtractEntityIdFromDomainEvent(domainEvent),
                    System.Text.Json.JsonSerializer.Serialize(domainEvent)
                )
            };
        }

        /// <summary>
        /// Genera un aggregate ID para el Domain Event
        /// </summary>
        private string GenerateAggregateId(DomainEvent domainEvent)
        {
            return domainEvent switch
            {
                ProductoCreado pc => $"producto-{pc.ProductoId}",
                ProductoActualizado pa => $"producto-{pa.ProductoId}",
                ProductoEliminado pe => $"producto-{pe.ProductoId}",
                CarritoCreado cc => $"carrito-{cc.UsuarioId}",
                ItemAgregadoAlCarrito iac => $"carrito-{iac.UsuarioId}",
                ItemEliminadoDelCarrito iec => $"carrito-{iec.UsuarioId}",
                CantidadItemCarritoActualizada cica => $"carrito-{cica.UsuarioId}",
                _ => $"domain-event-{domainEvent.Id}"
            };
        }

        /// <summary>
        /// Extrae el nombre de la entidad del Domain Event
        /// </summary>
        private static string ExtractEntityFromDomainEvent(DomainEvent domainEvent)
        {
            return domainEvent switch
            {
                ProductoCreado or ProductoActualizado or ProductoEliminado => "Producto",
                CarritoCreado or ItemAgregadoAlCarrito or ItemEliminadoDelCarrito or CantidadItemCarritoActualizada => "Carrito",
                _ => domainEvent.GetType().Name.Replace("Event", "").Replace("Evento", "")
            };
        }

        /// <summary>
        /// Extrae el ID de la entidad del Domain Event
        /// </summary>
        private static string ExtractEntityIdFromDomainEvent(DomainEvent domainEvent)
        {
            return domainEvent switch
            {
                ProductoCreado pc => pc.ProductoId.ToString(),
                ProductoActualizado pa => pa.ProductoId.ToString(),
                ProductoEliminado pe => pe.ProductoId.ToString(),
                CarritoCreado cc => cc.UsuarioId,
                ItemAgregadoAlCarrito iac => $"{iac.UsuarioId}:{iac.ProductoId}",
                ItemEliminadoDelCarrito iec => $"{iec.UsuarioId}:{iec.ProductoId}",
                CantidadItemCarritoActualizada cica => $"{cica.UsuarioId}:{cica.ProductoId}",
                _ => domainEvent.Id.ToString()
            };
        }
    }
}
