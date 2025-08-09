using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Globalization;

namespace CarritoComprasAPI.Core.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
        Task ClearAllAsync();
    }

    /// <summary>
    /// Implementación en memoria del servicio de caché
    /// </summary>
    public class MemoryCacheService : ICacheService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly ConcurrentHashSet<string> _keys;
        private bool _disposed = false;

        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keys = new ConcurrentHashSet<string>();
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var value))
                {
                    if (value is string json)
                    {
                        return Task.FromResult(JsonSerializer.Deserialize<T>(json));
                    }
                    return Task.FromResult((T?)value);
                }
                return Task.FromResult(default(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener valor del cache para la clave: {Key}", key);
                return Task.FromResult(default(T));
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration.Value;
                }
                
                options.RegisterPostEvictionCallback((k, v, reason, state) =>
                {
                    _keys.TryRemove(k.ToString()!);
                });

                var serializedValue = JsonSerializer.Serialize(value);
                _memoryCache.Set(key, serializedValue, options);
                _keys.Add(key);
                
                _logger.LogDebug("Valor almacenado en cache: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al almacenar valor en cache para la clave: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _keys.TryRemove(key);
                _logger.LogDebug("Clave removida del cache: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover clave del cache: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = _keys.Where(k => k.Contains(pattern, StringComparison.OrdinalIgnoreCase)).ToList();
                
                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key);
                }
                
                _logger.LogDebug("Removidas {Count} claves del cache que coinciden con el patrón: {Pattern}", 
                    keysToRemove.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover claves por patrón: {Pattern}", pattern);
            }
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_keys.Contains(key) && _memoryCache.TryGetValue(key, out _));
        }

        public Task ClearAllAsync()
        {
            try
            {
                var keysToRemove = _keys.ToList();
                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                }
                _keys.Clear();
                
                _logger.LogInformation("Cache completamente limpio. Removidas {Count} claves", keysToRemove.Count);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar todo el cache");
                return Task.CompletedTask;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _keys?.Clear();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Configuración del sistema de caché
    /// </summary>
    public class CacheConfiguration
    {
        public bool EnableCaching { get; set; } = true;
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan ProductosExpiration { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan CarritosExpiration { get; set; } = TimeSpan.FromMinutes(15);
    }

    /// <summary>
    /// Utilidad para generar claves de caché consistentes
    /// </summary>
    public static class CacheKeys
    {
        private const string PRODUCTOS_PREFIX = "productos";
        private const string CARRITOS_PREFIX = "carritos";

        public static string TodosLosProductos => $"{PRODUCTOS_PREFIX}:todos";
        public static string ProductoPorId(int id) => $"{PRODUCTOS_PREFIX}:id:{id}";
        public static string ProductosPorCategoria(string categoria) => $"{PRODUCTOS_PREFIX}:categoria:{categoria.ToUpperInvariant()}";
        public static string CarritoPorUsuario(string usuarioId) => $"{CARRITOS_PREFIX}:usuario:{usuarioId}";
        
        public static string ProductosPattern => $"{PRODUCTOS_PREFIX}:*";
        public static string CarritosPattern => $"{CARRITOS_PREFIX}:*";
    }

    // Implementación de ConcurrentHashSet si no existe
    public class ConcurrentHashSet<T> : IDisposable where T : notnull
    {
        private readonly HashSet<T> _set = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public bool Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _set.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool TryRemove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _set.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.Contains(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<T> ToList()
        {
            _lock.EnterReadLock();
            try
            {
                return _set.ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.Where(predicate).ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _set.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }
    }
}
