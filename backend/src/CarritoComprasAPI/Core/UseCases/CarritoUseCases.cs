using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Logging;
using System.Globalization;

namespace CarritoComprasAPI.Core.UseCases
{
    /// <summary>
    /// Casos de uso para la gestión del carrito de compras con métricas de rendimiento integradas
    /// </summary>
    public class CarritoUseCases : ICarritoUseCases
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IPerformanceMetricsService _metricsService;
        private readonly IStructuredLogger _structuredLogger;

        // Constantes para evitar duplicación de strings literales
        private const string CarritoEntityType = "Carrito";
        private const string ObtenerCarritoOperation = "ObtenerCarrito";
        private const string AgregarItemOperation = "AgregarItem";
        private const string ActualizarCantidadOperation = "ActualizarCantidad";
        private const string EliminarItemOperation = "EliminarItem";
        private const string LimpiarCarritoOperation = "LimpiarCarrito";
        private const string OperationKey = "Operation";
        private const string ResultadoExitoso = "Exitoso";
        private const string CarritoNoEncontrado = "CarritoNoEncontrado";

        /// <summary>
        /// Inicializa una nueva instancia de CarritoUseCases
        /// </summary>
        /// <param name="carritoRepository">Repositorio de carritos</param>
        /// <param name="productoRepository">Repositorio de productos</param>
        /// <param name="metricsService">Servicio de métricas de rendimiento</param>
        /// <param name="structuredLogger">Logger estructurado</param>
        /// <exception cref="ArgumentNullException">Lanzado cuando algún parámetro es null</exception>
        public CarritoUseCases(
            ICarritoRepository carritoRepository, 
            IProductoRepository productoRepository,
            IPerformanceMetricsService metricsService,
            IStructuredLogger structuredLogger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _structuredLogger = structuredLogger ?? throw new ArgumentNullException(nameof(structuredLogger));
        }

        /// <summary>
        /// Obtiene el carrito de un usuario específico, creándolo si no existe
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <returns>El carrito del usuario</returns>
        /// <exception cref="ArgumentException">Lanzado cuando el usuarioId es inválido</exception>
        public async Task<Carrito> ObtenerCarritoAsync(string usuarioId)
        {
            return await _metricsService.ExecuteWithMetrics(
                "CarritoUseCases.ObtenerCarrito",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(ObtenerCarritoOperation, CarritoEntityType, usuarioId, "Iniciando obtención de carrito");
                        
                        ValidarUsuarioId(usuarioId);

                        var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                        
                        if (carrito == null)
                        {
                            _structuredLogger.LogOperacionDominio(ObtenerCarritoOperation, CarritoEntityType, usuarioId, "Carrito no encontrado, creando nuevo carrito");
                            carrito = Carrito.Crear(usuarioId);
                            carrito = await _carritoRepository.CrearAsync(carrito);
                        }

                        _structuredLogger.LogOperacionDominio(ObtenerCarritoOperation, CarritoEntityType, usuarioId, 
                            $"Carrito obtenido exitosamente con {carrito.CantidadItems} items", 
                            new { ItemCount = carrito.CantidadItems, CarritoId = carrito.Id });
                        
                        return carrito;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(ObtenerCarritoOperation, ex, new { UsuarioId = usuarioId });
                        throw;
                    }
                },
                new Dictionary<string, object> 
                { 
                    ["UsuarioId"] = usuarioId,
                    [OperationKey] = "GetCart"
                }
            );
        }

        /// <summary>
        /// Agrega un item al carrito del usuario
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <param name="productoId">Identificador del producto a agregar</param>
        /// <param name="cantidad">Cantidad del producto a agregar</param>
        /// <returns>El carrito actualizado</returns>
        /// <exception cref="ArgumentException">Lanzado cuando los parámetros son inválidos</exception>
        /// <exception cref="InvalidOperationException">Lanzado cuando el producto no existe o no hay stock suficiente</exception>
        public async Task<Carrito> AgregarItemAsync(string usuarioId, int productoId, int cantidad)
        {
            return await _metricsService.ExecuteWithMetrics(
                "CarritoUseCases.AgregarItem",
                async () =>
                {
                    _structuredLogger.LogOperacionDominio(
                        AgregarItemOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Agregando {cantidad} unidades del producto {productoId} al carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId, ProductoId = productoId, Cantidad = cantidad }
                    );
                    
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
                    var cantidadEnCarrito = carrito.ObtenerItem(productoId)?.CantidadItem.Value ?? 0;
                    var cantidadTotal = cantidadEnCarrito + cantidad;
                    
                    if (!producto.TieneStock(cantidadTotal))
                    {
                        throw new InvalidOperationException(
                            $"Stock insuficiente. Stock disponible: {producto.StockProducto.Value}, cantidad en carrito: {cantidadEnCarrito}, cantidad solicitada: {cantidad}");
                    }

                    // Agregar item al carrito
                    carrito.AgregarItem(producto, cantidad);

                    // Guardar carrito
                    var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);
                    
                    _structuredLogger.LogOperacionDominio(
                        AgregarItemOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Item agregado exitosamente al carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId, ProductoId = productoId, Resultado = ResultadoExitoso }
                    );
                    
                    return carritoActualizado;
                }
            );
        }

        /// <summary>
        /// Actualiza la cantidad de un item en el carrito
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <param name="productoId">Identificador del producto a actualizar</param>
        /// <param name="cantidad">Nueva cantidad del producto</param>
        /// <returns>El carrito actualizado</returns>
        /// <exception cref="ArgumentException">Lanzado cuando los parámetros son inválidos</exception>
        /// <exception cref="InvalidOperationException">Lanzado cuando el carrito o item no existe</exception>
        public async Task<Carrito> ActualizarCantidadAsync(string usuarioId, int productoId, int cantidad)
        {
            return await _metricsService.ExecuteWithMetrics(
                "CarritoUseCases.ActualizarCantidad",
                async () =>
                {
                    _structuredLogger.LogOperacionDominio(
                        ActualizarCantidadOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Actualizando cantidad del producto {productoId} a {cantidad} en el carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId, ProductoId = productoId, Cantidad = cantidad }
                    );
                    
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
                            $"Stock insuficiente. Stock disponible: {producto.StockProducto.Value}, cantidad solicitada: {cantidad}");
                    }

                    // Actualizar cantidad
                    carrito.ActualizarCantidadItem(productoId, cantidad);

                    var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);
                    
                    _structuredLogger.LogOperacionDominio(
                        ActualizarCantidadOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Cantidad actualizada exitosamente en el carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId, ProductoId = productoId, Resultado = ResultadoExitoso }
                    );
                    
                    return carritoActualizado;
                }
            );
        }

        /// <summary>
        /// Elimina un item específico del carrito
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <param name="productoId">Identificador del producto a eliminar</param>
        /// <returns>True si se eliminó exitosamente, false si el carrito o item no existe</returns>
        /// <exception cref="ArgumentException">Lanzado cuando los parámetros son inválidos</exception>
        public async Task<bool> EliminarItemAsync(string usuarioId, int productoId)
        {
            return await _metricsService.ExecuteWithMetrics(
                "CarritoUseCases.EliminarItem",
                async () =>
                {
                    _structuredLogger.LogOperacionDominio(
                        EliminarItemOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Eliminando producto {productoId} del carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId, ProductoId = productoId }
                    );
                    
                    ValidarUsuarioId(usuarioId);

                    var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                    if (carrito == null)
                    {
                        _structuredLogger.LogOperacionDominio(
                            EliminarItemOperation,
                            CarritoEntityType,
                            usuarioId,
                            $"Carrito no encontrado para el usuario {usuarioId}",
                            new { UsuarioId = usuarioId, Resultado = CarritoNoEncontrado }
                        );
                        return false;
                    }

                    await EliminarItemInternoAsync(carrito, productoId);
                    
                    _structuredLogger.LogOperacionDominio(
                        EliminarItemOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Item eliminado exitosamente del carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId, ProductoId = productoId, Resultado = ResultadoExitoso }
                    );
                    
                    return true;
                }
            );
        }

        /// <summary>
        /// Vacía completamente el carrito del usuario
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <returns>True si se vació exitosamente, false si el carrito no existe</returns>
        /// <exception cref="ArgumentException">Lanzado cuando el usuarioId es inválido</exception>
        public async Task<bool> VaciarCarritoAsync(string usuarioId)
        {
            return await _metricsService.ExecuteWithMetrics(
                "CarritoUseCases.VaciarCarrito",
                async () =>
                {
                    _structuredLogger.LogOperacionDominio(
                        LimpiarCarritoOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Vaciando carrito del usuario {usuarioId}",
                        new { UsuarioId = usuarioId }
                    );
                    
                    ValidarUsuarioId(usuarioId);

                    var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
                    if (carrito == null)
                    {
                        _structuredLogger.LogOperacionDominio(
                            LimpiarCarritoOperation,
                            CarritoEntityType,
                            usuarioId,
                            $"Carrito no encontrado para el usuario {usuarioId}",
                            new { UsuarioId = usuarioId, Resultado = CarritoNoEncontrado }
                        );
                        return false;
                    }

                    carrito.Vaciar();
                    await _carritoRepository.ActualizarAsync(carrito);
                    
                    _structuredLogger.LogOperacionDominio(
                        LimpiarCarritoOperation,
                        CarritoEntityType,
                        usuarioId,
                        $"Carrito vaciado exitosamente para el usuario {usuarioId}",
                        new { UsuarioId = usuarioId, Resultado = ResultadoExitoso }
                    );
                    
                    return true;
                }
            );
        }

        /// <summary>
        /// Obtiene el total del carrito del usuario
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <returns>El total del carrito o 0 si no existe</returns>
        /// <exception cref="ArgumentException">Lanzado cuando el usuarioId es inválido</exception>
        public async Task<decimal> ObtenerTotalAsync(string usuarioId)
        {
            return await _metricsService.ExecuteWithMetrics(
                "CarritoUseCases.ObtenerTotal",
                async () =>
                {
                    _structuredLogger.LogOperacionDominio(
                        "ObtenerTotal",
                        CarritoEntityType,
                        usuarioId,
                        $"Obteniendo total del carrito para usuario: {usuarioId}",
                        new { UsuarioId = usuarioId }
                    );
                    
                    var carrito = await ObtenerCarritoAsync(usuarioId);
                    var total = carrito.Total;
                    
                    _structuredLogger.LogOperacionDominio(
                        "ObtenerTotal",
                        CarritoEntityType,
                        usuarioId,
                        $"Total calculado para usuario {usuarioId}: {total:C}",
                        new { UsuarioId = usuarioId, Total = total, Resultado = ResultadoExitoso }
                    );
                    
                    return total;
                }
            );
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
