using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Domain.Events.Productos;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.EventHandlers.Productos
{
    /// <summary>
    /// Handler para el evento de producto creado
    /// </summary>
    public class ProductoCreadoHandler : IDomainEventHandler<ProductoCreado>
    {
        private readonly IAppLogger _logger;

        public ProductoCreadoHandler(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProductoCreado domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Producto creado: {domainEvent.Nombre} (ID: {domainEvent.ProductoId}) " +
                                 $"en la categoría {domainEvent.Categoria} con precio ${domainEvent.Precio}");

            // Aquí se pueden agregar acciones adicionales como:
            // - Enviar notificaciones
            // - Actualizar índices de búsqueda
            // - Sincronizar con sistemas externos
            // - Generar reportes automáticos

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Handler para el evento de producto sin stock
    /// </summary>
    public class ProductoSinStockHandler : IDomainEventHandler<ProductoSinStock>
    {
        private readonly IAppLogger _logger;

        public ProductoSinStockHandler(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProductoSinStock domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"¡ALERTA! Producto sin stock: {domainEvent.NombreProducto} " +
                              $"(ID: {domainEvent.ProductoId}) de la categoría {domainEvent.Categoria}");

            // Acciones automatizadas cuando un producto se queda sin stock:
            // - Notificar al departamento de compras
            // - Enviar alertas por email
            // - Actualizar estado del producto en la web
            // - Generar orden de compra automática
            // - Notificar a usuarios que tienen el producto en wishlist

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Handler para el evento de cambio de precio
    /// </summary>
    public class PrecioProductoCambiadoHandler : IDomainEventHandler<PrecioProductoCambiado>
    {
        private readonly IAppLogger _logger;

        public PrecioProductoCambiadoHandler(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PrecioProductoCambiado domainEvent, CancellationToken cancellationToken = default)
        {
            var tipoMovimiento = domainEvent.PorcentajeCambio > 0 ? "aumento" : "reducción";
            var porcentajeAbs = Math.Abs(domainEvent.PorcentajeCambio);

            _logger.LogInformation($"Cambio de precio en {domainEvent.NombreProducto}: " +
                                 $"{tipoMovimiento} del {porcentajeAbs:F2}% " +
                                 $"(${domainEvent.PrecioAnterior} → ${domainEvent.PrecioNuevo})");

            // Acciones cuando cambia el precio:
            // - Notificar a usuarios interesados
            // - Actualizar carritos existentes
            // - Generar reportes de variación de precios
            // - Disparar campañas de marketing si es una reducción significativa

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Handler para el evento de producto eliminado
    /// </summary>
    public class ProductoEliminadoHandler : IDomainEventHandler<ProductoEliminado>
    {
        private readonly IAppLogger _logger;

        public ProductoEliminadoHandler(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProductoEliminado domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Producto eliminado: {domainEvent.Nombre} " +
                                 $"(ID: {domainEvent.ProductoId}) de la categoría {domainEvent.Categoria}");

            // Acciones cuando se elimina un producto:
            // - Remover de todos los carritos
            // - Archivar historial de ventas
            // - Notificar a sistemas externos
            // - Limpiar índices de búsqueda

            await Task.CompletedTask;
        }
    }
}
