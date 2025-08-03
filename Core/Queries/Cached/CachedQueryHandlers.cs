using CarritoComprasAPI.Core.Queries.Productos;
using CarritoComprasAPI.Core.Queries.Carrito;
using CarritoComprasAPI.Core.Caching;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Core.Queries.Cached
{
    /// <summary>
    /// Query handler con caché para obtener todos los productos
    /// </summary>
    public class CachedObtenerTodosProductosQueryHandler : IQueryHandler<ObtenerTodosProductosQuery, IEnumerable<Producto>>
    {
        private readonly IQueryHandler<ObtenerTodosProductosQuery, IEnumerable<Producto>> _inner;
        private readonly ICacheService _cacheService;
        private readonly CacheConfiguration _config;
        private readonly IAppLogger _logger;

        public CachedObtenerTodosProductosQueryHandler(
            IQueryHandler<ObtenerTodosProductosQuery, IEnumerable<Producto>> inner,
            ICacheService cacheService,
            CacheConfiguration config,
            IAppLogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> Handle(ObtenerTodosProductosQuery query, CancellationToken cancellationToken = default)
        {
            if (!_config.EnableCaching)
            {
                return await _inner.Handle(query, cancellationToken);
            }

            var cacheKey = CacheKeys.TodosLosProductos;
            var cachedResult = await _cacheService.GetAsync<IEnumerable<Producto>>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation($"Cache HIT para productos - clave: {cacheKey}");
                return cachedResult;
            }

            _logger.LogInformation($"Cache MISS para productos - clave: {cacheKey}");
            var result = await _inner.Handle(query, cancellationToken);

            if (result != null)
            {
                await _cacheService.SetAsync(cacheKey, result, _config.ProductosExpiration);
                _logger.LogInformation($"Productos almacenados en caché - clave: {cacheKey}");
            }

            return result ?? Enumerable.Empty<Producto>();
        }
    }

    /// <summary>
    /// Query handler con caché para obtener producto por ID
    /// </summary>
    public class CachedObtenerProductoPorIdQueryHandler : IQueryHandler<ObtenerProductoPorIdQuery, Producto?>
    {
        private readonly IQueryHandler<ObtenerProductoPorIdQuery, Producto?> _inner;
        private readonly ICacheService _cacheService;
        private readonly CacheConfiguration _config;
        private readonly IAppLogger _logger;

        public CachedObtenerProductoPorIdQueryHandler(
            IQueryHandler<ObtenerProductoPorIdQuery, Producto?> inner,
            ICacheService cacheService,
            CacheConfiguration config,
            IAppLogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Producto?> Handle(ObtenerProductoPorIdQuery query, CancellationToken cancellationToken = default)
        {
            if (!_config.EnableCaching)
            {
                return await _inner.Handle(query, cancellationToken);
            }

            var cacheKey = CacheKeys.ProductoPorId(query.Id);
            var cachedResult = await _cacheService.GetAsync<Producto?>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation($"Cache HIT para producto ID {query.Id} - clave: {cacheKey}");
                return cachedResult;
            }

            _logger.LogInformation($"Cache MISS para producto ID {query.Id} - clave: {cacheKey}");
            var result = await _inner.Handle(query, cancellationToken);

            if (result != null)
            {
                await _cacheService.SetAsync(cacheKey, result, _config.ProductosExpiration);
                _logger.LogInformation($"Producto {query.Id} almacenado en caché - clave: {cacheKey}");
            }

            return result;
        }
    }

    /// <summary>
    /// Query handler con caché para buscar productos por categoría
    /// </summary>
    public class CachedBuscarProductosPorCategoriaQueryHandler : IQueryHandler<BuscarProductosPorCategoriaQuery, IEnumerable<Producto>>
    {
        private readonly IQueryHandler<BuscarProductosPorCategoriaQuery, IEnumerable<Producto>> _inner;
        private readonly ICacheService _cacheService;
        private readonly CacheConfiguration _config;
        private readonly IAppLogger _logger;

        public CachedBuscarProductosPorCategoriaQueryHandler(
            IQueryHandler<BuscarProductosPorCategoriaQuery, IEnumerable<Producto>> inner,
            ICacheService cacheService,
            CacheConfiguration config,
            IAppLogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> Handle(BuscarProductosPorCategoriaQuery query, CancellationToken cancellationToken = default)
        {
            if (!_config.EnableCaching)
            {
                return await _inner.Handle(query, cancellationToken);
            }

            var cacheKey = CacheKeys.ProductosPorCategoria(query.Categoria);
            var cachedResult = await _cacheService.GetAsync<IEnumerable<Producto>>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation($"Cache HIT para productos categoría '{query.Categoria}' - clave: {cacheKey}");
                return cachedResult;
            }

            _logger.LogInformation($"Cache MISS para productos categoría '{query.Categoria}' - clave: {cacheKey}");
            var result = await _inner.Handle(query, cancellationToken);

            if (result != null)
            {
                await _cacheService.SetAsync(cacheKey, result, _config.ProductosExpiration);
                _logger.LogInformation($"Productos categoría '{query.Categoria}' almacenados en caché - clave: {cacheKey}");
            }

            return result ?? Enumerable.Empty<Producto>();
        }
    }

    /// <summary>
    /// Query handler con caché para obtener carrito por usuario
    /// </summary>
    public class CachedObtenerCarritoPorUsuarioQueryHandler : IQueryHandler<ObtenerCarritoPorUsuarioQuery, Domain.Carrito?>
    {
        private readonly IQueryHandler<ObtenerCarritoPorUsuarioQuery, Domain.Carrito?> _inner;
        private readonly ICacheService _cacheService;
        private readonly CacheConfiguration _config;
        private readonly IAppLogger _logger;

        public CachedObtenerCarritoPorUsuarioQueryHandler(
            IQueryHandler<ObtenerCarritoPorUsuarioQuery, Domain.Carrito?> inner,
            ICacheService cacheService,
            CacheConfiguration config,
            IAppLogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Domain.Carrito?> Handle(ObtenerCarritoPorUsuarioQuery query, CancellationToken cancellationToken = default)
        {
            if (!_config.EnableCaching)
            {
                return await _inner.Handle(query, cancellationToken);
            }

            var cacheKey = CacheKeys.CarritoPorUsuario(query.UsuarioId);
            var cachedResult = await _cacheService.GetAsync<Domain.Carrito?>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation($"Cache HIT para carrito usuario '{query.UsuarioId}' - clave: {cacheKey}");
                return cachedResult;
            }

            _logger.LogInformation($"Cache MISS para carrito usuario '{query.UsuarioId}' - clave: {cacheKey}");
            var result = await _inner.Handle(query, cancellationToken);

            if (result != null)
            {
                await _cacheService.SetAsync(cacheKey, result, _config.CarritosExpiration);
                _logger.LogInformation($"Carrito usuario '{query.UsuarioId}' almacenado en caché - clave: {cacheKey}");
            }

            return result;
        }
    }
}
