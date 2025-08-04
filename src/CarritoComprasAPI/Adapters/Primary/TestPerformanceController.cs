using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Performance;
using CarritoComprasAPI.Core.Logging;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controlador para pruebas de performance y alertas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestPerformanceController : BaseController
    {
        private readonly IPerformanceMetricsService _metricsService;
        private readonly IStructuredLogger _logger;

        public TestPerformanceController(
            IPerformanceMetricsService metricsService,
            IStructuredLogger logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Simula una operación lenta para generar alertas de warning (2-5 segundos)
        /// </summary>
        [HttpGet("slow-warning")]
        public async Task<IActionResult> SimulateSlowWarningOperation()
        {
            return await _metricsService.ExecuteWithMetrics(
                "TestPerformance.SlowWarning",
                async () =>
                {
                    _logger.LogOperacionDominio("SlowWarning", "TestPerformance", "warning", "Iniciando operación lenta (warning) - 3 segundos");
                    
                    // Simular trabajo pesado de 3 segundos (genera warning)
                    await Task.Delay(3000);
                    
                    _logger.LogOperacionDominio("SlowWarning", "TestPerformance", "warning", "Operación lenta (warning) completada");
                    
                    return Ok(new { 
                        message = "Operación lenta completada (warning)", 
                        duration = "3 segundos",
                        status = "warning"
                    });
                });
        }

        /// <summary>
        /// Simula una operación muy lenta para generar alertas críticas (>5 segundos)
        /// </summary>
        [HttpGet("slow-critical")]
        public async Task<IActionResult> SimulateSlowCriticalOperation()
        {
            return await _metricsService.ExecuteWithMetrics(
                "TestPerformance.SlowCritical",
                async () =>
                {
                    _logger.LogOperacionDominio("SlowCritical", "TestPerformance", "critical", "Iniciando operación muy lenta (critical) - 6 segundos");
                    
                    // Simular trabajo muy pesado de 6 segundos (genera critical)
                    await Task.Delay(6000);
                    
                    _logger.LogOperacionDominio("SlowCritical", "TestPerformance", "critical", "Operación muy lenta (critical) completada");
                    
                    return Ok(new { 
                        message = "Operación muy lenta completada (critical)", 
                        duration = "6 segundos",
                        status = "critical"
                    });
                });
        }

        /// <summary>
        /// Simula una operación que falla para probar alertas de fallas
        /// </summary>
        [HttpGet("simulate-failure")]
        public async Task<IActionResult> SimulateFailureOperation()
        {
            try
            {
                await _metricsService.ExecuteWithMetrics(
                    "TestPerformance.SimulatedFailure",
                    async () =>
                    {
                        _logger.LogOperacionDominio("SimulatedFailure", "TestPerformance", "failure", "Iniciando operación que fallará");
                        
                        await Task.Delay(1000);
                        
                        throw new InvalidOperationException("Operación simulada que falla para probar alertas");
                    });
                
                return Ok(); // No debería llegar aquí
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operación falló como se esperaba", 
                    error = ex.Message,
                    status = "failure"
                });
            }
        }

        /// <summary>
        /// Genera múltiples operaciones rápidas para comparar con las lentas
        /// </summary>
        [HttpGet("fast-operations/{count}")]
        public async Task<IActionResult> GenerateFastOperations(int count = 10)
        {
            var results = new List<object>();

            for (int i = 0; i < count; i++)
            {
                var result = await _metricsService.ExecuteWithMetrics(
                    "TestPerformance.FastOperation",
                    async () =>
                    {
                        // Simular trabajo rápido (10-50ms)
                        await Task.Delay(Random.Shared.Next(10, 50));
                        return new { operationNumber = i + 1, duration = "rápida" };
                    });
                
                results.Add(result);
            }

            return Ok(new { 
                message = $"Generadas {count} operaciones rápidas",
                results = results
            });
        }

        /// <summary>
        /// Genera tráfico mixto para probar el sistema completo
        /// </summary>
        [HttpPost("stress-test")]
        public async Task<IActionResult> StressTest([FromBody] StressTestRequest request)
        {
            var results = new List<object>();

            // Operaciones rápidas
            for (int i = 0; i < request.FastOperations; i++)
            {
                var result = await _metricsService.ExecuteWithMetrics(
                    "TestPerformance.StressTest.Fast",
                    async () =>
                    {
                        await Task.Delay(Random.Shared.Next(10, 100));
                        return new { type = "fast", number = i + 1 };
                    });
                results.Add(result);
            }

            // Operaciones warning
            for (int i = 0; i < request.WarningOperations; i++)
            {
                var result = await _metricsService.ExecuteWithMetrics(
                    "TestPerformance.StressTest.Warning",
                    async () =>
                    {
                        await Task.Delay(Random.Shared.Next(2500, 3500));
                        return new { type = "warning", number = i + 1 };
                    });
                results.Add(result);
            }

            // Operaciones críticas
            for (int i = 0; i < request.CriticalOperations; i++)
            {
                var result = await _metricsService.ExecuteWithMetrics(
                    "TestPerformance.StressTest.Critical",
                    async () =>
                    {
                        await Task.Delay(Random.Shared.Next(5500, 7000));
                        return new { type = "critical", number = i + 1 };
                    });
                results.Add(result);
            }

            return Ok(new
            {
                message = "Stress test completado",
                summary = new
                {
                    fastOperations = request.FastOperations,
                    warningOperations = request.WarningOperations,
                    criticalOperations = request.CriticalOperations,
                    totalOperations = results.Count
                }
            });
        }
    }

    public class StressTestRequest
    {
        public int FastOperations { get; set; } = 5;
        public int WarningOperations { get; set; } = 2;
        public int CriticalOperations { get; set; } = 1;
    }
}
