using CarritoComprasAPI.Core.Queries;
using CarritoComprasAPI.Core.Caching;
using CarritoComprasAPI.Core.Ports;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CarritoComprasAPI.Core.Caching
{
    /// <summary>
    /// Decorador que agrega funcionalidad de caché a los query handlers
    /// </summary>
    public class CachedQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
        private readonly IQueryHandler<TQuery, TResponse> _inner;
        private readonly ICacheService _cacheService;
        private readonly CacheConfiguration _config;
        private readonly IAppLogger _logger;

        public CachedQueryHandlerDecorator(
            IQueryHandler<TQuery, TResponse> inner,
            ICacheService cacheService,
            CacheConfiguration config,
            IAppLogger logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default)
        {
            if (!_config.EnableCaching)
            {
                return await _inner.Handle(query, cancellationToken);
            }

            var cacheKey = GenerateCacheKey(query);
            var expiration = GetCacheExpiration(query);

            // Intentar obtener del caché
            var cachedResult = await _cacheService.GetAsync<TResponse>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation($"Cache HIT para query {typeof(TQuery).Name} con clave: {cacheKey}");
                return cachedResult;
            }

            _logger.LogInformation($"Cache MISS para query {typeof(TQuery).Name} con clave: {cacheKey}");

            // Ejecutar query original
            var result = await _inner.Handle(query, cancellationToken);

            // Almacenar en caché si el resultado no es nulo
            if (result != null)
            {
                await _cacheService.SetAsync(cacheKey, result, expiration);
                _logger.LogInformation($"Resultado almacenado en caché con clave: {cacheKey}, expiración: {expiration}");
            }

            return result!;
        }

        private string GenerateCacheKey(TQuery query)
        {
            var queryType = typeof(TQuery).Name;
            var properties = typeof(TQuery).GetProperties()
                .OrderBy(p => p.Name)
                .Select(p => $"{p.Name}:{p.GetValue(query)}")
                .ToArray();

            return $"{queryType}:{string.Join(":", properties)}";
        }

        private TimeSpan GetCacheExpiration(TQuery query)
        {
            return typeof(TQuery).Name switch
            {
                var name when name.Contains("Producto", StringComparison.OrdinalIgnoreCase) => _config.ProductosExpiration,
                var name when name.Contains("Carrito", StringComparison.OrdinalIgnoreCase) => _config.CarritosExpiration,
                _ => _config.DefaultExpiration
            };
        }
    }

    /// <summary>
    /// Servicio para invalidar caché basado en eventos de dominio
    /// </summary>
    public interface ICacheInvalidationService
    {
        Task InvalidateProductoCache(int? productoId = null);
        Task InvalidateCarritoCache(string? usuarioId = null);
        Task InvalidateAllCache();
    }

    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(ICacheService cacheService, ILogger<CacheInvalidationService> logger)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvalidateProductoCache(int? productoId = null)
        {
            try
            {
                if (productoId.HasValue)
                {
                    // Invalidar caché específico del producto
                    await _cacheService.RemoveAsync(CacheKeys.ProductoPorId(productoId.Value));
                    _logger.LogDebug("Cache invalidado para producto {ProductoId}", productoId.Value);
                }

                // Invalidar cachés relacionados con productos
                await _cacheService.RemoveAsync(CacheKeys.TodosLosProductos);
                await _cacheService.RemoveByPatternAsync(CacheKeys.ProductosPattern);
                
                _logger.LogDebug("Cache de productos invalidado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invalidar cache de productos");
            }
        }

        public async Task InvalidateCarritoCache(string? usuarioId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuarioId))
                {
                    // Invalidar caché específico del carrito del usuario
                    await _cacheService.RemoveAsync(CacheKeys.CarritoPorUsuario(usuarioId));
                    _logger.LogDebug("Cache invalidado para carrito del usuario {UsuarioId}", usuarioId);
                }
                
                // Invalidar todos los cachés de carritos
                await _cacheService.RemoveByPatternAsync(CacheKeys.CarritosPattern);
                _logger.LogDebug("Cache de carritos invalidado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invalidar cache de carritos");
            }
        }

        public async Task InvalidateAllCache()
        {
            try
            {
                await _cacheService.ClearAllAsync();
                _logger.LogInformation("Todo el cache ha sido invalidado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invalidar todo el cache");
            }
        }
    }
}
