using CarritoComprasAPI.Core.Queries;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using System.Globalization;

namespace CarritoComprasAPI.Core.Queries.Carrito
{
    // Query para obtener carrito por usuario
    public record ObtenerCarritoPorUsuarioQuery(string UsuarioId) : IQuery<Domain.Carrito?>;

    // Handler para obtener carrito
    public class ObtenerCarritoPorUsuarioQueryHandler : IQueryHandler<ObtenerCarritoPorUsuarioQuery, Domain.Carrito?>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IAppLogger _logger;

        public ObtenerCarritoPorUsuarioQueryHandler(ICarritoRepository carritoRepository, IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Domain.Carrito?> Handle(ObtenerCarritoPorUsuarioQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo carrito para usuario: {UsuarioId}", query.UsuarioId);

                ValidarQuery(query);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(query.UsuarioId);

                if (carrito == null)
                {
                    _logger.LogInformation("Carrito no encontrado para usuario {UsuarioId}", query.UsuarioId);
                }
                else
                {
                    _logger.LogInformation("Carrito obtenido para usuario {UsuarioId} con {ItemCount} items",
                        query.UsuarioId, carrito.CantidadItems);
                }

                return carrito;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito para usuario: {UsuarioId}", query.UsuarioId);
                throw;
            }
        }

        private static void ValidarQuery(ObtenerCarritoPorUsuarioQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }

    // Query para obtener total del carrito
    public record ObtenerTotalCarritoQuery(string UsuarioId) : IQuery<decimal>;

    // Handler para obtener total
    public class ObtenerTotalCarritoQueryHandler : IQueryHandler<ObtenerTotalCarritoQuery, decimal>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IAppLogger _logger;

        public ObtenerTotalCarritoQueryHandler(ICarritoRepository carritoRepository, IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<decimal> Handle(ObtenerTotalCarritoQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo total del carrito para usuario: {UsuarioId}", query.UsuarioId);

                ValidarQuery(query);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(query.UsuarioId);
                var total = carrito?.Total ?? 0;

                _logger.LogInformation("Total calculado para usuario {UsuarioId}: {Total:C}", query.UsuarioId, total);

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total del carrito para usuario: {UsuarioId}", query.UsuarioId);
                throw;
            }
        }

        private static void ValidarQuery(ObtenerTotalCarritoQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }

    // Query para obtener resumen del carrito (optimizada para mostrar información básica)
    public record ObtenerResumenCarritoQuery(string UsuarioId) : IQuery<ResumenCarritoResult>;

    // Resultado del resumen
    public record ResumenCarritoResult(
        string UsuarioId,
        int CantidadItems,
        int CantidadProductos,
        decimal Total,
        DateTime? FechaActualizacion
    );

    // Handler para resumen del carrito
    public class ObtenerResumenCarritoQueryHandler : IQueryHandler<ObtenerResumenCarritoQuery, ResumenCarritoResult>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IAppLogger _logger;

        public ObtenerResumenCarritoQueryHandler(ICarritoRepository carritoRepository, IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResumenCarritoResult> Handle(ObtenerResumenCarritoQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo resumen del carrito para usuario: {UsuarioId}", query.UsuarioId);

                ValidarQuery(query);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(query.UsuarioId);

                var resumen = new ResumenCarritoResult(
                    query.UsuarioId,
                    carrito?.CantidadItems ?? 0,
                    carrito?.CantidadProductos ?? 0,
                    carrito?.Total ?? 0,
                    carrito?.FechaActualizacion
                );

                _logger.LogInformation("Resumen obtenido para usuario {UsuarioId}: {CantidadItems} items, total {Total:C}",
                    query.UsuarioId, resumen.CantidadItems, resumen.Total);

                return resumen;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen del carrito para usuario: {UsuarioId}", query.UsuarioId);
                throw;
            }
        }

        private static void ValidarQuery(ObtenerResumenCarritoQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }

    // Query para obtener items del carrito con detalles de producto
    public record ObtenerItemsCarritoQuery(string UsuarioId) : IQuery<IEnumerable<ItemCarritoDetalleResult>>;

    // Resultado detallado de item
    public record ItemCarritoDetalleResult(
        int ProductoId,
        string ProductoNombre,
        string ProductoDescripcion,
        decimal PrecioUnitario,
        int Cantidad,
        decimal Subtotal,
        int StockDisponible,
        DateTime FechaAgregado
    );

    // Handler para items detallados
    public class ObtenerItemsCarritoQueryHandler : IQueryHandler<ObtenerItemsCarritoQuery, IEnumerable<ItemCarritoDetalleResult>>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IAppLogger _logger;

        public ObtenerItemsCarritoQueryHandler(
            ICarritoRepository carritoRepository,
            IProductoRepository productoRepository,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ItemCarritoDetalleResult>> Handle(ObtenerItemsCarritoQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo items detallados del carrito para usuario: {UsuarioId}", query.UsuarioId);

                ValidarQuery(query);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(query.UsuarioId);
                if (carrito == null || !carrito.TieneItems())
                {
                    return Enumerable.Empty<ItemCarritoDetalleResult>();
                }

                var itemsDetallados = new List<ItemCarritoDetalleResult>();

                foreach (var item in carrito.Items)
                {
                    // Obtener detalles actualizados del producto
                    var producto = await _productoRepository.ObtenerPorIdAsync(item.ProductoId);
                    if (producto != null)
                    {
                        itemsDetallados.Add(new ItemCarritoDetalleResult(
                            item.ProductoId,
                            producto.Nombre.Value,
                            producto.Descripcion,
                            item.PrecioUnitario,
                            item.CantidadItem.Value,
                            item.Subtotal,
                            producto.StockProducto.Value,
                            item.FechaAgregado
                        ));
                    }
                }

                _logger.LogInformation("Se obtuvieron {Count} items detallados para usuario {UsuarioId}",
                    itemsDetallados.Count, query.UsuarioId);

                return itemsDetallados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener items del carrito para usuario: {UsuarioId}", query.UsuarioId);
                throw;
            }
        }

        private static void ValidarQuery(ObtenerItemsCarritoQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }
}
