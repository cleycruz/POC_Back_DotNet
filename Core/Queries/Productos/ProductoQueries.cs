using CarritoComprasAPI.Core.Queries;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.Queries.Productos
{
    // Query para obtener todos los productos
    public record ObtenerTodosProductosQuery() : IQuery<IEnumerable<Producto>>;

    // Handler para obtener todos los productos
    public class ObtenerTodosProductosQueryHandler : IQueryHandler<ObtenerTodosProductosQuery, IEnumerable<Producto>>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public ObtenerTodosProductosQueryHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> Handle(ObtenerTodosProductosQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los productos");
                
                var productos = await _repository.ObtenerTodosAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} productos", productos.Count());
                
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                throw;
            }
        }
    }

    // Query para obtener producto por ID
    public record ObtenerProductoPorIdQuery(int Id) : IQuery<Producto?>;

    // Handler para obtener producto por ID
    public class ObtenerProductoPorIdQueryHandler : IQueryHandler<ObtenerProductoPorIdQuery, Producto?>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public ObtenerProductoPorIdQueryHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Producto?> Handle(ObtenerProductoPorIdQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo producto con ID: {ProductoId}", query.Id);
                
                if (query.Id <= 0)
                {
                    _logger.LogWarning("ID de producto inválido: {ProductoId}", query.Id);
                    return null;
                }

                var producto = await _repository.ObtenerPorIdAsync(query.Id);
                
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", query.Id);
                }
                else
                {
                    _logger.LogInformation("Producto {ProductoNombre} encontrado", producto.Nombre);
                }

                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {ProductoId}", query.Id);
                throw;
            }
        }
    }

    // Query para buscar productos por categoría
    public record BuscarProductosPorCategoriaQuery(string Categoria) : IQuery<IEnumerable<Producto>>;

    // Handler para buscar por categoría
    public class BuscarProductosPorCategoriaQueryHandler : IQueryHandler<BuscarProductosPorCategoriaQuery, IEnumerable<Producto>>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public BuscarProductosPorCategoriaQueryHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> Handle(BuscarProductosPorCategoriaQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Buscando productos por categoría: {Categoria}", query.Categoria);
                
                if (string.IsNullOrWhiteSpace(query.Categoria))
                {
                    _logger.LogWarning("Categoría de búsqueda vacía o nula");
                    return Enumerable.Empty<Producto>();
                }

                var productos = await _repository.BuscarPorCategoriaAsync(query.Categoria);
                
                _logger.LogInformation("Se encontraron {Count} productos en la categoría {Categoria}", 
                    productos.Count(), query.Categoria);
                
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos por categoría: {Categoria}", query.Categoria);
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
                        p.Nombre.Contains(query.Nombre, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(query.Categoria))
                {
                    productosFiltrados = productosFiltrados.Where(p => 
                        p.Categoria.Contains(query.Categoria, StringComparison.OrdinalIgnoreCase));
                }

                if (query.PrecioMinimo.HasValue)
                {
                    productosFiltrados = productosFiltrados.Where(p => p.Precio >= query.PrecioMinimo.Value);
                }

                if (query.PrecioMaximo.HasValue)
                {
                    productosFiltrados = productosFiltrados.Where(p => p.Precio <= query.PrecioMaximo.Value);
                }

                if (query.EnStock.HasValue && query.EnStock.Value)
                {
                    productosFiltrados = productosFiltrados.Where(p => p.Stock > 0);
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
