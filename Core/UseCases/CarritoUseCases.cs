using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.UseCases
{
    public class CarritoUseCases : ICarritoUseCases
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IAppLogger _logger;

        public CarritoUseCases(
            ICarritoRepository carritoRepository, 
            IProductoRepository productoRepository,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Carrito> ObtenerCarritoAsync(string usuarioId)
        {
            try
            {
                _logger.LogInformation("Obteniendo carrito para usuario: {UsuarioId}", usuarioId);
                
                ValidarUsuarioId(usuarioId);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                
                if (carrito == null)
                {
                    _logger.LogInformation("Carrito no encontrado para usuario {UsuarioId}, creando nuevo carrito", usuarioId);
                    carrito = new Carrito { UsuarioId = usuarioId };
                    carrito = await _carritoRepository.CrearAsync(carrito);
                }

                _logger.LogInformation("Carrito obtenido para usuario {UsuarioId} con {ItemCount} items", 
                    usuarioId, carrito.CantidadItems);
                
                return carrito;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito para usuario: {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<Carrito> AgregarItemAsync(string usuarioId, int productoId, int cantidad)
        {
            try
            {
                _logger.LogInformation("Agregando {Cantidad} unidades del producto {ProductoId} al carrito del usuario {UsuarioId}", 
                    cantidad, productoId, usuarioId);
                
                ValidarUsuarioId(usuarioId);
                ValidarCantidad(cantidad);

                // Obtener producto
                var producto = await _productoRepository.ObtenerPorIdAsync(productoId);
                if (producto == null)
                {
                    throw new InvalidOperationException($"Producto con ID {productoId} no encontrado");
                }

                // Obtener o crear carrito
                var carrito = await ObtenerCarritoAsync(usuarioId);

                // Validar stock disponible
                var cantidadEnCarrito = carrito.ObtenerItem(productoId)?.Cantidad ?? 0;
                var cantidadTotal = cantidadEnCarrito + cantidad;
                
                if (!producto.TieneStock(cantidadTotal))
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad en carrito: {cantidadEnCarrito}, cantidad solicitada: {cantidad}");
                }

                // Agregar item al carrito
                carrito.AgregarItem(producto, cantidad);

                // Guardar carrito
                var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);
                
                _logger.LogInformation("Item agregado exitosamente al carrito del usuario {UsuarioId}", usuarioId);
                
                return carritoActualizado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar item al carrito. Usuario: {UsuarioId}, Producto: {ProductoId}, Cantidad: {Cantidad}", 
                    usuarioId, productoId, cantidad);
                throw;
            }
        }

        public async Task<Carrito> ActualizarCantidadAsync(string usuarioId, int productoId, int cantidad)
        {
            try
            {
                _logger.LogInformation("Actualizando cantidad del producto {ProductoId} a {Cantidad} en el carrito del usuario {UsuarioId}", 
                    productoId, cantidad, usuarioId);
                
                ValidarUsuarioId(usuarioId);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                if (carrito == null)
                {
                    throw new InvalidOperationException($"Carrito no encontrado para el usuario {usuarioId}");
                }

                if (cantidad <= 0)
                {
                    // Si la cantidad es 0 o negativa, eliminar el item
                    return await EliminarItemInternoAsync(carrito, productoId);
                }

                // Validar que el producto existe
                var producto = await _productoRepository.ObtenerPorIdAsync(productoId);
                if (producto == null)
                {
                    throw new InvalidOperationException($"Producto con ID {productoId} no encontrado");
                }

                // Validar stock
                if (!producto.TieneStock(cantidad))
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad solicitada: {cantidad}");
                }

                // Actualizar cantidad
                carrito.ActualizarCantidadItem(productoId, cantidad);

                var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);
                
                _logger.LogInformation("Cantidad actualizada exitosamente en el carrito del usuario {UsuarioId}", usuarioId);
                
                return carritoActualizado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cantidad en carrito. Usuario: {UsuarioId}, Producto: {ProductoId}, Cantidad: {Cantidad}", 
                    usuarioId, productoId, cantidad);
                throw;
            }
        }

        public async Task<bool> EliminarItemAsync(string usuarioId, int productoId)
        {
            try
            {
                _logger.LogInformation("Eliminando producto {ProductoId} del carrito del usuario {UsuarioId}", 
                    productoId, usuarioId);
                
                ValidarUsuarioId(usuarioId);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                if (carrito == null)
                {
                    _logger.LogWarning("Carrito no encontrado para el usuario {UsuarioId}", usuarioId);
                    return false;
                }

                await EliminarItemInternoAsync(carrito, productoId);
                
                _logger.LogInformation("Item eliminado exitosamente del carrito del usuario {UsuarioId}", usuarioId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar item del carrito. Usuario: {UsuarioId}, Producto: {ProductoId}", 
                    usuarioId, productoId);
                throw;
            }
        }

        public async Task<bool> VaciarCarritoAsync(string usuarioId)
        {
            try
            {
                _logger.LogInformation("Vaciando carrito del usuario {UsuarioId}", usuarioId);
                
                ValidarUsuarioId(usuarioId);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                if (carrito == null)
                {
                    _logger.LogWarning("Carrito no encontrado para el usuario {UsuarioId}", usuarioId);
                    return false;
                }

                carrito.Vaciar();
                await _carritoRepository.ActualizarAsync(carrito);
                
                _logger.LogInformation("Carrito vaciado exitosamente para el usuario {UsuarioId}", usuarioId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al vaciar carrito del usuario: {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<decimal> ObtenerTotalAsync(string usuarioId)
        {
            try
            {
                _logger.LogInformation("Obteniendo total del carrito para usuario: {UsuarioId}", usuarioId);
                
                var carrito = await ObtenerCarritoAsync(usuarioId);
                var total = carrito.Total;
                
                _logger.LogInformation("Total calculado para usuario {UsuarioId}: {Total:C}", usuarioId, total);
                
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total del carrito para usuario: {UsuarioId}", usuarioId);
                throw;
            }
        }

        private async Task<Carrito> EliminarItemInternoAsync(Carrito carrito, int productoId)
        {
            carrito.EliminarItem(productoId);
            return await _carritoRepository.ActualizarAsync(carrito);
        }

        private static void ValidarUsuarioId(string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }

        private static void ValidarCantidad(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0");
        }
    }
}
