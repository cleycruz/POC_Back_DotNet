using System.Collections.Concurrent;
using System.Diagnostics;
using CarritoComprasAPI.Core.Logging;
using System.Globalization;

namespace CarritoComprasAPI.Core.Performance
{
    /// <summary>
    /// Servicio de métricas de performance para monitoreo en tiempo real
    /// </summary>
    public interface IPerformanceMetricsService
    {
        /// <summary>
        /// Inicia el seguimiento de una operación
        /// </summary>
        IDisposable TrackOperation(string operationName, Dictionary<string, object>? context = null);
        
        /// <summary>
        /// Registra una métrica personalizada
        /// </summary>
        void RecordMetric(string metricName, double value, Dictionary<string, object>? tags = null);
        
        /// <summary>
        /// Obtiene estadísticas de una operación
        /// </summary>
        OperationStats? GetOperationStats(string operationName);
        
        /// <summary>
        /// Obtiene todas las métricas actuales
        /// </summary>
        Dictionary<string, OperationStats> GetAllMetrics();
        
        /// <summary>
        /// Reinicia las métricas
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Obtiene métricas del sistema
        /// </summary>
        SystemMetrics GetSystemMetrics();
    }

    /// <summary>
    /// Implementación del servicio de métricas de performance
    /// </summary>
    public class PerformanceMetricsService : IPerformanceMetricsService
    {
        private readonly ConcurrentDictionary<string, OperationStats> _operationStats = new();
        private readonly ConcurrentDictionary<string, List<double>> _customMetrics = new();
        private readonly IStructuredLogger _logger;
        private readonly object _lock = new();

        public PerformanceMetricsService(IStructuredLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IDisposable TrackOperation(string operationName, Dictionary<string, object>? context = null)
        {
            return new OperationTracker(operationName, this, _logger, context);
        }

        public void RecordMetric(string metricName, double value, Dictionary<string, object>? tags = null)
        {
            _customMetrics.AddOrUpdate(metricName,
                new List<double> { value },
                (key, existing) =>
                {
                    lock (_lock)
                    {
                        existing.Add(value);
                        if (existing.Count > 1000) // Mantener solo las últimas 1000 métricas
                        {
                            existing.RemoveRange(0, 100);
                        }
                        return existing;
                    }
                });

            _logger.LogPerformance($"Custom Metric: {metricName}", TimeSpan.FromMilliseconds(value), tags);
        }

        public OperationStats? GetOperationStats(string operationName)
        {
            return _operationStats.TryGetValue(operationName, out var stats) ? stats : null;
        }

        public Dictionary<string, OperationStats> GetAllMetrics()
        {
            return new Dictionary<string, OperationStats>(_operationStats);
        }

        public void Reset()
        {
            _operationStats.Clear();
            _customMetrics.Clear();
            _logger.LogOperacionDominio("Reset", "PerformanceMetrics", "All", "All performance metrics have been reset");
        }

        public SystemMetrics GetSystemMetrics()
        {
            var process = Process.GetCurrentProcess();
            
            return new SystemMetrics
            {
                Timestamp = DateTimeOffset.UtcNow,
                CpuUsagePercent = GetCpuUsage(),
                MemoryUsageMB = process.WorkingSet64 / 1024 / 1024,
                ThreadCount = process.Threads.Count,
                HandleCount = process.HandleCount,
                GcCollectionCounts = new Dictionary<int, int>
                {
                    { 0, GC.CollectionCount(0) },
                    { 1, GC.CollectionCount(1) },
                    { 2, GC.CollectionCount(2) }
                },
                TotalAllocatedBytes = GC.GetTotalAllocatedBytes(),
                AvailableMemoryMB = GC.GetTotalMemory(false) / 1024 / 1024
            };
        }

        internal void RecordOperation(string operationName, TimeSpan duration, bool success, Dictionary<string, object>? context)
        {
            _operationStats.AddOrUpdate(operationName,
                new OperationStats(operationName, duration, success),
                (key, existing) => existing.AddExecution(duration, success));

            _logger.LogPerformance(operationName, duration, new { Success = success, Context = context });

            // Alertar sobre operaciones lentas
            if (duration.TotalMilliseconds > 2000)
            {
                _logger.LogOperacionDominio("SlowOperation", "Performance", operationName, 
                    $"Operation took {duration.TotalMilliseconds:F2}ms", context);
            }
        }

        private static double GetCpuUsage()
        {
            // Simplificado - en un entorno real usarías PerformanceCounter o similar
            var process = Process.GetCurrentProcess();
            return process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount / 100.0;
        }
    }

    /// <summary>
    /// Tracker de operaciones para uso con using statement
    /// </summary>
    public class OperationTracker : IDisposable
    {
        private readonly string _operationName;
        private readonly PerformanceMetricsService _metricsService;
        private readonly IStructuredLogger _logger;
        private readonly Dictionary<string, object>? _context;
        private readonly Stopwatch _stopwatch;
        private bool _disposed;

        public OperationTracker(
            string operationName, 
            PerformanceMetricsService metricsService, 
            IStructuredLogger logger,
            Dictionary<string, object>? context)
        {
            _operationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _stopwatch.Stop();
                _metricsService.RecordOperation(_operationName, _stopwatch.Elapsed, true, _context);
            }

            _disposed = true;
        }

        public void RecordFailure(Exception? exception = null)
        {
            if (_disposed) return;

            _stopwatch.Stop();
            _metricsService.RecordOperation(_operationName, _stopwatch.Elapsed, false, _context);
            
            if (exception != null)
            {
                _logger.LogError(_operationName, exception, _context);
            }
            
            _disposed = true;
        }
    }

    /// <summary>
    /// Estadísticas de una operación
    /// </summary>
    public class OperationStats
    {
        private readonly object _lock = new();
        private readonly List<double> _durations = new();

        public string OperationName { get; }
        public int TotalExecutions { get; private set; }
        public int SuccessfulExecutions { get; private set; }
        public int FailedExecutions { get; private set; }
        public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions * 100 : 0;
        public TimeSpan TotalDuration { get; private set; }
        public TimeSpan AverageDuration => TotalExecutions > 0 ? TimeSpan.FromTicks(TotalDuration.Ticks / TotalExecutions) : TimeSpan.Zero;
        public TimeSpan MinDuration { get; private set; } = TimeSpan.MaxValue;
        public TimeSpan MaxDuration { get; private set; } = TimeSpan.Zero;
        public DateTimeOffset FirstExecution { get; }
        public DateTimeOffset LastExecution { get; private set; }

        public OperationStats(string operationName, TimeSpan initialDuration, bool success)
        {
            OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
            FirstExecution = DateTimeOffset.UtcNow;
            AddExecution(initialDuration, success);
        }

        public OperationStats AddExecution(TimeSpan duration, bool success)
        {
            lock (_lock)
            {
                TotalExecutions++;
                if (success) SuccessfulExecutions++;
                else FailedExecutions++;

                TotalDuration = TotalDuration.Add(duration);
                LastExecution = DateTimeOffset.UtcNow;

                if (duration < MinDuration) MinDuration = duration;
                if (duration > MaxDuration) MaxDuration = duration;

                _durations.Add(duration.TotalMilliseconds);
                
                // Mantener solo las últimas 1000 ejecuciones para el cálculo de percentiles
                if (_durations.Count > 1000)
                {
                    _durations.RemoveRange(0, 100);
                }
            }

            return this;
        }

        public double GetPercentile(double percentile)
        {
            if (percentile < 0 || percentile > 100)
                throw new ArgumentException("Percentile must be between 0 and 100", nameof(percentile));

            lock (_lock)
            {
                if (_durations.Count == 0) return 0;

                var sortedDurations = _durations.OrderBy(d => d).ToList();
                var index = (int)Math.Ceiling(sortedDurations.Count * percentile / 100) - 1;
                index = Math.Max(0, Math.Min(index, sortedDurations.Count - 1));
                
                return sortedDurations[index];
            }
        }

        public PerformanceHealth GetHealthStatus()
        {
            var avgMs = AverageDuration.TotalMilliseconds;
            var p95 = GetPercentile(95);
            var successRate = SuccessRate;

            if (successRate < 95 || avgMs > 2000 || p95 > 5000)
                return PerformanceHealth.Critical;
            
            if (successRate < 99 || avgMs > 1000 || p95 > 2000)
                return PerformanceHealth.Warning;
            
            return PerformanceHealth.Healthy;
        }
    }

    /// <summary>
    /// Métricas del sistema
    /// </summary>
    public class SystemMetrics
    {
        public DateTimeOffset Timestamp { get; set; }
        public double CpuUsagePercent { get; set; }
        public long MemoryUsageMB { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public Dictionary<int, int> GcCollectionCounts { get; set; } = new();
        public long TotalAllocatedBytes { get; set; }
        public long AvailableMemoryMB { get; set; }
    }

    /// <summary>
    /// Estado de salud de performance
    /// </summary>
    public enum PerformanceHealth
    {
        Healthy,
        Warning,
        Critical
    }

    /// <summary>
    /// Extensiones para facilitar el uso de métricas
    /// </summary>
    public static class PerformanceExtensions
    {
        /// <summary>
        /// Ejecuta una función con seguimiento automático de performance
        /// </summary>
        public static async Task<T> ExecuteWithMetrics<T>(
            this IPerformanceMetricsService metricsService,
            string operationName,
            Func<Task<T>> operation,
            Dictionary<string, object>? context = null)
        {
            using var tracker = metricsService.TrackOperation(operationName, context);
            
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                if (tracker is OperationTracker opTracker)
                {
                    opTracker.RecordFailure(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Versión síncrona de ExecuteWithMetrics
        /// </summary>
        public static T ExecuteWithMetrics<T>(
            this IPerformanceMetricsService metricsService,
            string operationName,
            Func<T> operation,
            Dictionary<string, object>? context = null)
        {
            using var tracker = metricsService.TrackOperation(operationName, context);
            
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                if (tracker is OperationTracker opTracker)
                {
                    opTracker.RecordFailure(ex);
                }
                throw;
            }
        }
    }
}
