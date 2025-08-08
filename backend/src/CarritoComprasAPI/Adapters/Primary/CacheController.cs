using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Caching;

namespace CarritoComprasAPI.Adapters.Primary
{
    [ApiController]
    [Route("api/cache")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ICacheInvalidationService _cacheInvalidation;
        private readonly CacheConfiguration _config;

        public CacheController(
            ICacheService cacheService,
            ICacheInvalidationService cacheInvalidation,
            CacheConfiguration config)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Obtiene la configuración actual del caché
        /// </summary>
        [HttpGet("config")]
        public ActionResult<object> ObtenerConfiguracion()
        {
            return Ok(new
            {
                Habilitado = _config.EnableCaching,
                ExpiracionPorDefecto = _config.DefaultExpiration.ToString(),
                ExpiracionProductos = _config.ProductosExpiration.ToString(),
                ExpiracionCarritos = _config.CarritosExpiration.ToString()
            });
        }

        /// <summary>
        /// Verifica si existe una clave en el caché
        /// </summary>
        [HttpGet("exists/{key}")]
        public async Task<ActionResult<bool>> ExisteClave(string key)
        {
            var exists = await _cacheService.ExistsAsync(key);
            return Ok(exists);
        }

        /// <summary>
        /// Invalida todo el caché de productos
        /// </summary>
        [HttpDelete("productos")]
        public async Task<ActionResult> InvalidarCacheProductos()
        {
            await _cacheInvalidation.InvalidateProductoCache();
            return Ok(new { message = "Cache de productos invalidado" });
        }

        /// <summary>
        /// Invalida el caché de un producto específico
        /// </summary>
        [HttpDelete("productos/{id}")]
        public async Task<ActionResult> InvalidarCacheProducto(int id)
        {
            await _cacheInvalidation.InvalidateProductoCache(id);
            return Ok(new { message = $"Cache del producto {id} invalidado" });
        }

        /// <summary>
        /// Invalida todo el caché de carritos
        /// </summary>
        [HttpDelete("carritos")]
        public async Task<ActionResult> InvalidarCacheCarritos()
        {
            await _cacheInvalidation.InvalidateCarritoCache();
            return Ok(new { message = "Cache de carritos invalidado" });
        }

        /// <summary>
        /// Invalida el caché de un carrito específico
        /// </summary>
        [HttpDelete("carritos/{usuarioId}")]
        public async Task<ActionResult> InvalidarCacheCarrito(string usuarioId)
        {
            await _cacheInvalidation.InvalidateCarritoCache(usuarioId);
            return Ok(new { message = $"Cache del carrito del usuario {usuarioId} invalidado" });
        }

        /// <summary>
        /// Invalida todo el caché
        /// </summary>
        [HttpDelete("all")]
        public async Task<ActionResult> InvalidarTodoElCache()
        {
            await _cacheInvalidation.InvalidateAllCache();
            return Ok(new { message = "Todo el cache ha sido invalidado" });
        }

        /// <summary>
        /// Elimina una clave específica del caché
        /// </summary>
        [HttpDelete("key/{key}")]
        public async Task<ActionResult> EliminarClave(string key)
        {
            await _cacheService.RemoveAsync(key);
            return Ok(new { message = $"Clave '{key}' eliminada del cache" });
        }

        /// <summary>
        /// Obtiene estadísticas básicas del caché (simuladas para el ejemplo)
        /// </summary>
        [HttpGet("stats")]
        public ActionResult<object> ObtenerEstadisticas()
        {
            // En una implementación real, estas estadísticas vendrían del servicio de caché
            return Ok(new
            {
                CacheHits = "N/A", // Se podría implementar contadores
                CacheMisses = "N/A",
                TotalKeys = "N/A",
                MemoryUsage = "N/A",
                Uptime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }
}
