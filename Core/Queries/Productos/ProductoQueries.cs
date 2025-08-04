using CarritoComprasAPI.Core.Queries;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.UseCases;

namespace CarritoComprasAPI.Core.Queries.Productos
{
    /// <summary>
    /// Consulta para obtener todos los productos disponibles
    /// </summary>
    public record ObtenerTodosProductosQuery() : IQuery<IEnumerable<Producto>>;

    /// <summary>
    /// Handler para procesar la consulta de todos los productos
    /// </summary>
    public class ObtenerTodosProductosQueryHandler : IQueryHandler<ObtenerTodosProductosQuery, IEnumerable<Producto>>
    {
        private readonly IProductoUseCases _productoUseCases;
        private readonly IAppLogger _logger;

        /// <summary>
        /// Inicializa una nueva instancia del handler
        /// </summary>
        /// <param name="productoUseCases">Casos de uso de productos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        /// <exception cref="ArgumentNullException">Lanzado cuando algún parámetro es null</exception>
        public ObtenerTodosProductosQueryHandler(IProductoUseCases productoUseCases, IAppLogger logger)
        {
            _productoUseCases = productoUseCases ?? throw new ArgumentNullException(nameof(productoUseCases));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> Handle(ObtenerTodosProductosQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Query CQRS: Obteniendo todos los productos via UseCases");
                
                var productos = await _productoUseCases.ObtenerTodosAsync();
                
                _logger.LogInformation("Query CQRS: Se obtuvieron {Count} productos via UseCases", productos.Count());
                
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Query CQRS: Error al obtener todos los productos");
                throw;
            }
        }
    }

    // Query para obtener producto por ID
    public record ObtenerProductoPorIdQuery(int Id) : IQuery<Producto?>;

    // Handler para obtener producto por ID
    public class ObtenerProductoPorIdQueryHandler : IQueryHandler<ObtenerProductoPorIdQuery, Producto?>
    {
        private readonly IProductoUseCases _productoUseCases;
        private readonly IAppLogger _logger;

        public ObtenerProductoPorIdQueryHandler(IProductoUseCases productoUseCases, IAppLogger logger)
        {
            _productoUseCases = productoUseCases ?? throw new ArgumentNullException(nameof(productoUseCases));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Producto?> Handle(ObtenerProductoPorIdQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Query CQRS: Obteniendo producto con ID: {ProductoId} via UseCases", query.Id);
                
                if (query.Id <= 0)
                {
                    _logger.LogWarning("Query CQRS: ID de producto inválido: {ProductoId}", query.Id);
                    return null;
                }

                var producto = await _productoUseCases.ObtenerPorIdAsync(query.Id);
                
                if (producto == null)
                {
                    _logger.LogWarning("Query CQRS: Producto con ID {ProductoId} no encontrado", query.Id);
                }
                else
                {
                    _logger.LogInformation("Query CQRS: Producto {ProductoNombre} encontrado via UseCases", producto.Nombre.Value);
                }

                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Query CQRS: Error al obtener producto con ID: {ProductoId}", query.Id);
                throw;
            }
        }
    }

    // Query para buscar productos por categoría
    public record BuscarProductosPorCategoriaQuery(string Categoria) : IQuery<IEnumerable<Producto>>;

    // Handler para buscar por categoría
    public class BuscarProductosPorCategoriaQueryHandler : IQueryHandler<BuscarProductosPorCategoriaQuery, IEnumerable<Producto>>
    {
        private readonly IProductoUseCases _productoUseCases;
        private readonly IAppLogger _logger;

        public BuscarProductosPorCategoriaQueryHandler(IProductoUseCases productoUseCases, IAppLogger logger)
        {
            _productoUseCases = productoUseCases ?? throw new ArgumentNullException(nameof(productoUseCases));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> Handle(BuscarProductosPorCategoriaQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Query CQRS: Buscando productos por categoría: {Categoria} via UseCases", query.Categoria);
                
                if (string.IsNullOrWhiteSpace(query.Categoria))
                {
                    _logger.LogWarning("Query CQRS: Categoría de búsqueda vacía o nula");
                    return Enumerable.Empty<Producto>();
                }

                var productos = await _productoUseCases.BuscarPorCategoriaAsync(query.Categoria);
                
                _logger.LogInformation("Query CQRS: Se encontraron {Count} productos en la categoría {Categoria} via UseCases", 
                    productos.Count(), query.Categoria);
                
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Query CQRS: Error al buscar productos por categoría: {Categoria}", query.Categoria);
                throw;
            }
        }
    }

    // Query para obtener productos con filtros avanzados
    public record BuscarProductosQuery(
        string? Nombre = null,
        string? Categoria = null,
        decimal? PrecioMinimo = null,
        decimal? PrecioMaximo = null,
        bool? EnStock = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<ProductosPaginadosResult>;

    // Resultado paginado
    public record ProductosPaginadosResult(
        IEnumerable<Producto> Productos,
        int TotalElementos,
        int PaginaActual,
        int TotalPaginas
    );

    // Handler para búsqueda avanzada
    public class BuscarProductosQueryHandler : IQueryHandler<BuscarProductosQuery, ProductosPaginadosResult>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public BuscarProductosQueryHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductosPaginadosResult> Handle(BuscarProductosQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Búsqueda avanzada de productos con filtros");

                // Obtener todos los productos primero (en una implementación real, esto sería optimizado en la base de datos)
                var todosLosProductos = await _repository.ObtenerTodosAsync();
                
                // Aplicar filtros
                var productosFiltrados = todosLosProductos.AsQueryable();

                if (!string.IsNullOrWhiteSpace(query.Nombre))
                {
                    productosFiltrados = productosFiltrados.Where(p => 
                        p.Nombre.Value.Contains(query.Nombre, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(query.Categoria))
                {
                    productosFiltrados = productosFiltrados.Where(p => 
                        p.CategoriaProducto.Value.Contains(query.Categoria, StringComparison.OrdinalIgnoreCase));
                }

                if (query.PrecioMinimo.HasValue)
                {
                    productosFiltrados = productosFiltrados.Where(p => p.PrecioProducto.Value >= query.PrecioMinimo.Value);
                }

                if (query.PrecioMaximo.HasValue)
                {
                    productosFiltrados = productosFiltrados.Where(p => p.PrecioProducto.Value <= query.PrecioMaximo.Value);
                }

                if (query.EnStock.HasValue && query.EnStock.Value)
                {
                    productosFiltrados = productosFiltrados.Where(p => p.StockProducto.Value > 0);
                }

                var totalElementos = productosFiltrados.Count();
                var totalPaginas = (int)Math.Ceiling((double)totalElementos / query.PageSize);

                // Aplicar paginación
                var productosPaginados = productosFiltrados
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                _logger.LogInformation("Búsqueda completada: {Total} productos encontrados", totalElementos);

                return new ProductosPaginadosResult(
                    productosPaginados,
                    totalElementos,
                    query.PageNumber,
                    totalPaginas
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda avanzada de productos");
                throw;
            }
        }
    }
}
