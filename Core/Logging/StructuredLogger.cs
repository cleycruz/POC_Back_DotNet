using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Diagnostics;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.Logging
{
    /// <summary>
    /// Servicio de logging estructurado avanzado para operaciones de dominio
    /// Proporciona logging contextual con información detallada para análisis y debugging
    /// </summary>
    /// <remarks>
    /// Este servicio implementa logging estructurado siguiendo las mejores prácticas:
    /// - Contexto de operación con scopes
    /// - Datos estructurados para análisis
    /// - Información de performance y auditoría
    /// - Manejo de errores enriquecido
    /// - Logging específico por tipo de operación
    /// </remarks>
    public interface IStructuredLogger
    {
        /// <summary>
        /// Registra el inicio de una operación de dominio con contexto completo
        /// </summary>
        /// <param name="operacion">Nombre de la operación</param>
        /// <param name="usuarioId">ID del usuario que ejecuta la operación</param>
        /// <param name="requestId">ID único de la request</param>
        /// <param name="parametros">Parámetros adicionales de la operación</param>
        /// <returns>Scope disposable para agrupar logs relacionados</returns>
        IDisposable IniciarOperacion(string operacion, string usuarioId, string requestId, object? parametros = null);
        
        /// <summary>
        /// Registra el éxito de una operación con métricas de performance
        /// </summary>
        /// <param name="operacion">Nombre de la operación</param>
        /// <param name="duracion">Duración de la operación</param>
        /// <param name="resultado">Resultado de la operación</param>
        void LogExito(string operacion, TimeSpan duracion, object? resultado = null);
        
        /// <summary>
        /// Registra un error en la operación con contexto completo y enriquecido
        /// </summary>
        /// <param name="operacion">Nombre de la operación</param>
        /// <param name="excepcion">Excepción ocurrida</param>
        /// <param name="contexto">Contexto adicional del error</param>
        void LogError(string operacion, Exception excepcion, object? contexto = null);
        
        /// <summary>
        /// Registra información de validación de dominio con detalles enriquecidos
        /// </summary>
        /// <param name="entidad">Tipo de entidad validada</param>
        /// <param name="validacionExitosa">Si la validación fue exitosa</param>
        /// <param name="errores">Lista de errores de validación si los hay</param>
        void LogValidacion(string entidad, bool validacionExitosa, IEnumerable<string>? errores = null);
        
        /// <summary>
        /// Log específico para operaciones de dominio críticas
        /// </summary>
        void LogOperacionDominio(string operation, string entityType, object entityId, string details, object? context = null);

        /// <summary>
        /// Log específico para eventos de dominio
        /// </summary>
        void LogEventoDominio(string eventType, object eventData, string aggregateId);

        /// <summary>
        /// Log específico para métricas de performance con alertas
        /// </summary>
        void LogPerformance(string operation, TimeSpan duration, object? context = null);

        /// <summary>
        /// Log específico para operaciones de caché
        /// </summary>
        void LogOperacionCache(string operation, string cacheKey, bool hit, object? context = null);
        
        /// <summary>
        /// Registra eventos de dominio con contexto
        /// </summary>
        /// <param name="eventoTipo">Tipo de evento de dominio</param>
        /// <param name="entidadId">ID de la entidad que generó el evento</param>
        /// <param name="datosEvento">Datos del evento</param>
        void LogDomainEvent(string eventoTipo, object entidadId, object? datosEvento = null);
        
        /// <summary>
        /// Registra operaciones de caché con métricas
        /// </summary>
        /// <param name="operacion">Tipo de operación (Hit, Miss, Set, Invalidate)</param>
        /// <param name="clave">Clave del caché</param>
        /// <param name="duracion">Duración de la operación</param>
        void LogCache(string operacion, string clave, TimeSpan? duracion = null);
    }

    /// <summary>
    /// Implementación del servicio de logging estructurado usando ILogger de .NET
    /// </summary>
    public class StructuredLogger : IStructuredLogger
    {
        private readonly ILogger<StructuredLogger> _logger;

        /// <summary>
        /// Constructor del servicio de logging estructurado
        /// </summary>
        /// <param name="logger">Logger de .NET para la implementación base</param>
        public StructuredLogger(ILogger<StructuredLogger> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IDisposable IniciarOperacion(string operacion, string usuarioId, string requestId, object? parametros = null)
        {
            var scopeState = new Dictionary<string, object>
            {
                ["Operacion"] = operacion,
                ["UsuarioId"] = usuarioId,
                ["RequestId"] = requestId,
                ["Timestamp"] = DateTime.UtcNow,
                ["Parametros"] = parametros ?? new { }
            };

            _logger.LogInformation("Iniciando operación {Operacion} - Usuario: {UsuarioId} - Request: {RequestId} - Parámetros: {@Parametros}",
                operacion, usuarioId, requestId, parametros);

            return _logger.BeginScope(scopeState) ?? throw new InvalidOperationException("No se pudo crear el scope de logging");
        }

        /// <inheritdoc />
        public void LogExito(string operacion, TimeSpan duracion, object? resultado = null)
        {
            _logger.LogInformation("Operación {Operacion} completada exitosamente - Duración: {DuracionMs}ms - Resultado: {@Resultado}",
                operacion, duracion.TotalMilliseconds, resultado);
        }

        /// <inheritdoc />
        public void LogError(string operacion, Exception excepcion, object? contexto = null)
        {
            _logger.LogError(excepcion, 
                "Error en operación {Operacion} - Tipo: {TipoError} - Mensaje: {MensajeError} - Contexto: {@Contexto} - StackTrace: {StackTrace}",
                operacion, excepcion.GetType().Name, excepcion.Message, contexto, excepcion.StackTrace);
        }

        /// <inheritdoc />
        public void LogValidacion(string entidad, bool validacionExitosa, IEnumerable<string>? errores = null)
        {
            if (validacionExitosa)
            {
                _logger.LogDebug("Validación exitosa para {Entidad}", entidad);
            }
            else
            {
                _logger.LogWarning("Validación fallida para {Entidad} - Errores: {@Errores}", 
                    entidad, errores?.ToArray() ?? Array.Empty<string>());
            }
        }

        /// <inheritdoc />
        public void LogOperacionDominio(string operation, string entityType, object entityId, string details, object? context = null)
        {
            var domainContext = new
            {
                Operation = operation,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                Context = context,
                Timestamp = DateTimeOffset.UtcNow,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                MachineName = Environment.MachineName
            };
            
            _logger.LogInformation("Domain Operation: {Operation} on {EntityType}({EntityId}) - {Details} - Context: {@DomainContext}",
                operation, entityType, entityId, details, domainContext);
        }

        /// <inheritdoc />
        public void LogEventoDominio(string eventType, object eventData, string aggregateId)
        {
            var eventContext = new
            {
                EventType = eventType,
                AggregateId = aggregateId,
                EventData = eventData,
                Timestamp = DateTimeOffset.UtcNow,
                CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N")[..8]
            };
            
            _logger.LogInformation("Domain Event Published: {EventType} for Aggregate {AggregateId} - Data: {@EventContext}",
                eventType, aggregateId, eventContext);
        }

        /// <inheritdoc />
        public void LogPerformance(string operation, TimeSpan duration, object? context = null)
        {
            var performanceContext = new
            {
                Operation = operation,
                DurationMs = duration.TotalMilliseconds,
                DurationReadable = $"{duration.TotalMilliseconds:F2}ms",
                Context = context,
                Timestamp = DateTimeOffset.UtcNow,
                IsSlowOperation = duration.TotalMilliseconds > 1000
            };

            if (duration.TotalMilliseconds > 2000) // Operaciones muy lentas
            {
                _logger.LogWarning("SLOW OPERATION: {Operation} took {DurationMs}ms - {@PerformanceContext}",
                    operation, duration.TotalMilliseconds, performanceContext);
            }
            else if (duration.TotalMilliseconds > 1000) // Operaciones lentas
            {
                _logger.LogWarning("Slow Operation: {Operation} took {DurationMs}ms - {@PerformanceContext}",
                    operation, duration.TotalMilliseconds, performanceContext);
            }
            else
            {
                _logger.LogInformation("Performance: {Operation} completed in {DurationMs}ms - {@PerformanceContext}",
                    operation, duration.TotalMilliseconds, performanceContext);
            }
        }

        /// <inheritdoc />
        public void LogOperacionCache(string operation, string cacheKey, bool hit, object? context = null)
        {
            var cacheContext = new
            {
                Operation = operation,
                CacheKey = cacheKey,
                Hit = hit,
                Context = context,
                Timestamp = DateTimeOffset.UtcNow
            };
            
            _logger.LogDebug("Cache {Operation}: {CacheKey} - {HitMiss} - {@CacheContext}",
                operation, cacheKey, hit ? "HIT" : "MISS", cacheContext);
        }

        /// <inheritdoc />
        public void LogDomainEvent(string eventoTipo, object entidadId, object? datosEvento = null)
        {
            LogEventoDominio(eventoTipo, datosEvento ?? new object(), entidadId.ToString() ?? "unknown");
        }

        /// <inheritdoc />
        public void LogCache(string operacion, string clave, TimeSpan? duracion = null)
        {
            LogOperacionCache(operacion, clave, duracion.HasValue, new { Duracion = duracion?.TotalMilliseconds });
        }
    }

    /// <summary>
    /// Extensiones para simplificar el uso del structured logging
    /// </summary>
    public static class StructuredLoggerExtensions
    {
        /// <summary>
        /// Ejecuta una operación con logging automático de inicio, éxito/error y métricas
        /// </summary>
        /// <typeparam name="T">Tipo de resultado de la operación</typeparam>
        /// <param name="logger">Logger estructurado</param>
        /// <param name="operacion">Nombre de la operación</param>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="requestId">ID de la request</param>
        /// <param name="func">Función a ejecutar</param>
        /// <param name="parametros">Parámetros de la operación</param>
        /// <returns>Resultado de la operación</returns>
        public static async Task<T> EjecutarConLogging<T>(
            this IStructuredLogger logger,
            string operacion,
            string usuarioId,
            string requestId,
            Func<Task<T>> func,
            object? parametros = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            using var scope = logger.IniciarOperacion(operacion, usuarioId, requestId, parametros);
            
            try
            {
                var resultado = await func();
                stopwatch.Stop();
                
                logger.LogExito(operacion, stopwatch.Elapsed, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(operacion, ex, new { Duracion = stopwatch.Elapsed, Parametros = parametros });
                throw;
            }
        }

        /// <summary>
        /// Versión síncrona del método EjecutarConLogging
        /// </summary>
        public static T EjecutarConLogging<T>(
            this IStructuredLogger logger,
            string operacion,
            string usuarioId,
            string requestId,
            Func<T> func,
            object? parametros = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            using var scope = logger.IniciarOperacion(operacion, usuarioId, requestId, parametros);
            
            try
            {
                var resultado = func();
                stopwatch.Stop();
                
                logger.LogExito(operacion, stopwatch.Elapsed, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(operacion, ex, new { Duracion = stopwatch.Elapsed, Parametros = parametros });
                throw;
            }
        }
    }
}
