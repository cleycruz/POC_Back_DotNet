using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Logging;
using System.Globalization;

namespace CarritoComprasAPI.Core.UseCases
{
    /// <summary>
    /// Casos de uso para la gestión de productos con métricas de rendimiento integradas
    /// </summary>
    public class ProductoUseCases : IProductoUseCases
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IPerformanceMetricsService _metricsService;
        private readonly IStructuredLogger _structuredLogger;

        // Constantes para evitar duplicación de strings literales
        private const string ProductoEntityType = "Producto";
        private const string ObtenerTodosOperation = "ObtenerTodos";
        private const string ObtenerPorIdOperation = "ObtenerPorId";
        private const string CrearOperation = "Crear";
        private const string ActualizarOperation = "Actualizar";
        private const string EliminarOperation = "Eliminar";
        private const string BuscarPorCategoriaOperation = "BuscarPorCategoria";
        private const string OperationKey = "Operation";

        /// <summary>
        /// Inicializa una nueva instancia de ProductoUseCases
        /// </summary>
        /// <param name="productoRepository">Repositorio de productos</param>
        /// <param name="metricsService">Servicio de métricas de rendimiento</param>
        /// <param name="structuredLogger">Logger estructurado</param>
        /// <exception cref="ArgumentNullException">Lanzado cuando algún parámetro es null</exception>
        public ProductoUseCases(
            IProductoRepository productoRepository, 
            IPerformanceMetricsService metricsService,
            IStructuredLogger structuredLogger)
        {
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _structuredLogger = structuredLogger ?? throw new ArgumentNullException(nameof(structuredLogger));
        }

        /// <summary>
        /// Obtiene todos los productos disponibles con métricas de rendimiento
        /// </summary>
        /// <returns>Una colección de todos los productos</returns>
        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return await _metricsService.ExecuteWithMetrics(
                "ProductoUseCases.ObtenerTodos",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(ObtenerTodosOperation, ProductoEntityType, "All", "Iniciando obtención de todos los productos");
                        
                        var productos = await _productoRepository.ObtenerTodosAsync();
                        var count = productos.Count();
                        
                        _structuredLogger.LogOperacionDominio(ObtenerTodosOperation, ProductoEntityType, "All", 
                            $"Se obtuvieron {count} productos exitosamente", new { Count = count });
                        
                        return productos;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(ObtenerTodosOperation, ex, new { Operation = "ObtenerTodosProductos" });
                        throw;
                    }
                },
                new Dictionary<string, object> { [OperationKey] = "GetAllProducts" }
            );
        }

        /// <summary>
        /// Obtiene un producto específico por su identificador
        /// </summary>
        /// <param name="id">Identificador del producto</param>
        /// <returns>El producto encontrado o null si no existe o el ID es inválido</returns>
        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _metricsService.ExecuteWithMetrics(
                "ProductoUseCases.ObtenerPorId",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(ObtenerPorIdOperation, ProductoEntityType, id, "Iniciando búsqueda de producto por ID");
                        
                        if (id <= 0)
                        {
                            _structuredLogger.LogValidacion(ProductoEntityType, false, new[] { $"ID de producto inválido: {id}" });
                            return null;
                        }

                        var producto = await _productoRepository.ObtenerPorIdAsync(id);
                        
                        if (producto == null)
                        {
                            _structuredLogger.LogOperacionDominio(ObtenerPorIdOperation, ProductoEntityType, id, "Producto no encontrado");
                        }
                        else
                        {
                            _structuredLogger.LogOperacionDominio(ObtenerPorIdOperation, ProductoEntityType, id, "Producto encontrado exitosamente",
                                new { ProductoNombre = producto.Nombre.Value });
                        }

                        return producto;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(ObtenerPorIdOperation, ex, new { ProductoId = id });
                        throw;
                    }
                },
                new Dictionary<string, object> { ["ProductoId"] = id }
            );
        }

        /// <summary>
        /// Crea un nuevo producto en el sistema
        /// </summary>
        /// <param name="producto">Producto a crear</param>
        /// <returns>El producto creado con su ID asignado</returns>
        /// <exception cref="Exception">Lanzada cuando ocurre un error durante la creación</exception>
        public async Task<Producto> CrearAsync(Producto producto)
        {
            return await _metricsService.ExecuteWithMetrics(
                "ProductoUseCases.Crear",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(CrearOperation, ProductoEntityType, "New", "Iniciando creación de nuevo producto",
                            new { ProductoNombre = producto.Nombre.Value });
                        
                        // Las validaciones ahora están en los Value Objects del producto
                        
                        var productoCreado = await _productoRepository.CrearAsync(producto);
                        
                        _structuredLogger.LogOperacionDominio(CrearOperation, ProductoEntityType, productoCreado.Id, "Producto creado exitosamente",
                            new { ProductoId = productoCreado.Id, ProductoNombre = productoCreado.Nombre.Value });
                        
                        return productoCreado;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(CrearOperation, ex, new 
                        { 
                            ProductoNombre = producto.Nombre.Value,
                            ProductoPrecio = producto.PrecioProducto.Value,
                            ProductoStock = producto.StockProducto.Value 
                        });
                        throw;
                    }
                },
                new Dictionary<string, object> 
                { 
                    ["ProductoNombre"] = producto.Nombre.Value,
                    [OperationKey] = "CreateProduct"
                }
            );
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="id">Identificador del producto a actualizar</param>
        /// <param name="productoActualizado">Datos actualizados del producto</param>
        /// <returns>El producto actualizado o null si no existe</returns>
        /// <exception cref="Exception">Lanzada cuando ocurre un error durante la actualización</exception>
        public async Task<Producto?> ActualizarAsync(int id, Producto productoActualizado)
        {
            return await _metricsService.ExecuteWithMetrics(
                "ProductoUseCases.Actualizar",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(ActualizarOperation, ProductoEntityType, id, "Iniciando actualización de producto");
                        
                        var productoExistente = await _productoRepository.ObtenerPorIdAsync(id);
                        if (productoExistente == null)
                        {
                            _structuredLogger.LogOperacionDominio(ActualizarOperation, ProductoEntityType, id, "Producto no encontrado para actualizar");
                            return null;
                        }

                        // Las validaciones ahora están en los Value Objects del producto
                        
                        // Mantener el ID y fecha de creación originales
                        productoActualizado.Id = id;
                        productoActualizado.FechaCreacion = productoExistente.FechaCreacion;
                        
                        var resultado = await _productoRepository.ActualizarAsync(productoActualizado);
                        
                        if (resultado != null)
                        {
                            _structuredLogger.LogOperacionDominio(ActualizarOperation, ProductoEntityType, id, "Producto actualizado exitosamente",
                                new 
                                { 
                                    ProductoNombre = resultado.Nombre.Value,
                                    ProductoPrecio = resultado.PrecioProducto.Value,
                                    ProductoStock = resultado.StockProducto.Value
                                });
                        }
                        
                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(ActualizarOperation, ex, new { ProductoId = id });
                        throw;
                    }
                },
                new Dictionary<string, object> 
                { 
                    ["ProductoId"] = id,
                    [OperationKey] = "UpdateProduct"
                }
            );
        }

        /// <summary>
        /// Elimina un producto del sistema
        /// </summary>
        /// <param name="id">Identificador del producto a eliminar</param>
        /// <returns>True si se eliminó exitosamente, false si no existe</returns>
        /// <exception cref="Exception">Lanzada cuando ocurre un error durante la eliminación</exception>
        public async Task<bool> EliminarAsync(int id)
        {
            return await _metricsService.ExecuteWithMetrics(
                "ProductoUseCases.Eliminar",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(EliminarOperation, ProductoEntityType, id, "Iniciando eliminación de producto");
                        
                        var existe = await _productoRepository.ExisteAsync(id);
                        if (!existe)
                        {
                            _structuredLogger.LogOperacionDominio(EliminarOperation, ProductoEntityType, id, "Producto no encontrado para eliminar");
                            return false;
                        }

                        var resultado = await _productoRepository.EliminarAsync(id);
                        
                        if (resultado)
                        {
                            _structuredLogger.LogOperacionDominio(EliminarOperation, ProductoEntityType, id, "Producto eliminado exitosamente");
                        }
                        else
                        {
                            _structuredLogger.LogOperacionDominio(EliminarOperation, ProductoEntityType, id, "No se pudo eliminar el producto");
                        }

                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(EliminarOperation, ex, new { ProductoId = id });
                        throw;
                    }
                },
                new Dictionary<string, object> 
                { 
                    ["ProductoId"] = id,
                    [OperationKey] = "DeleteProduct"
                }
            );
        }

        /// <summary>
        /// Busca productos por categoría
        /// </summary>
        /// <param name="categoria">Categoría a buscar</param>
        /// <returns>Productos que pertenecen a la categoría especificada, o colección vacía si la categoría es inválida</returns>
        /// <exception cref="Exception">Lanzada cuando ocurre un error durante la búsqueda</exception>
        public async Task<IEnumerable<Producto>> BuscarPorCategoriaAsync(string categoria)
        {
            return await _metricsService.ExecuteWithMetrics(
                "ProductoUseCases.BuscarPorCategoria",
                async () =>
                {
                    try
                    {
                        _structuredLogger.LogOperacionDominio(BuscarPorCategoriaOperation, ProductoEntityType, categoria, "Iniciando búsqueda por categoría");
                        
                        if (string.IsNullOrWhiteSpace(categoria))
                        {
                            _structuredLogger.LogValidacion(ProductoEntityType, false, new[] { "Categoría de búsqueda vacía o nula" });
                            return Enumerable.Empty<Producto>();
                        }

                        var productos = await _productoRepository.BuscarPorCategoriaAsync(categoria);
                        var count = productos.Count();
                        
                        _structuredLogger.LogOperacionDominio(BuscarPorCategoriaOperation, ProductoEntityType, categoria, 
                            $"Se encontraron {count} productos en la categoría", new { Count = count, Categoria = categoria });
                        
                        return productos;
                    }
                    catch (Exception ex)
                    {
                        _structuredLogger.LogError(BuscarPorCategoriaOperation, ex, new { Categoria = categoria });
                        throw;
                    }
                },
                new Dictionary<string, object> 
                { 
                    ["Categoria"] = categoria,
                    [OperationKey] = "SearchByCategory"
                }
            );
        }

    }
}
