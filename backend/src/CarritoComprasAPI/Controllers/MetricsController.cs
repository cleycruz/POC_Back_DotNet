using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Metrics;
using System.Text.Json;
using System.Globalization;

namespace CarritoComprasAPI.Controllers
{
    /// <summary>
    /// Controlador para métricas de calidad de código y complejidad ciclomática
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MetricsController : ControllerBase
    {
        private readonly ICodeMetricsService _metricsService;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(
            ICodeMetricsService metricsService,
            ILogger<MetricsController> logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene un reporte completo de métricas de código
        /// </summary>
        /// <returns>Reporte completo de métricas</returns>
        [HttpGet("report")]
        [ProducesResponseType(typeof(CodeMetricsReport), 200)]
        public async Task<ActionResult<CodeMetricsReport>> GetMetricsReport()
        {
            try
            {
                _logger.LogInformation("Generando reporte completo de métricas");
                var report = await _metricsService.GenerateMetricsReportAsync();
                
                _logger.LogInformation("Reporte de métricas generado exitosamente");
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de métricas");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene análisis específico de complejidad ciclomática
        /// </summary>
        /// <returns>Reporte de complejidad ciclomática</returns>
        [HttpGet("cyclomatic-complexity")]
        [ProducesResponseType(typeof(CyclomaticComplexityReport), 200)]
        public async Task<ActionResult<CyclomaticComplexityReport>> GetCyclomaticComplexity()
        {
            try
            {
                _logger.LogInformation("Analizando complejidad ciclomática");
                var complexity = await _metricsService.AnalyzeCyclomaticComplexityAsync();
                
                _logger.LogInformation("Análisis de complejidad completado. Promedio: {Average:F2}", 
                    complexity.AverageComplexity);
                
                return Ok(complexity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al analizar complejidad ciclomática");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene métricas de cobertura de código
        /// </summary>
        /// <returns>Métricas de cobertura</returns>
        [HttpGet("code-coverage")]
        [ProducesResponseType(typeof(CodeCoverageMetrics), 200)]
        public async Task<ActionResult<CodeCoverageMetrics>> GetCodeCoverage()
        {
            try
            {
                _logger.LogInformation("Obteniendo métricas de cobertura de código");
                var coverage = await _metricsService.GetCodeCoverageMetricsAsync();
                
                _logger.LogInformation("Métricas de cobertura obtenidas. Cobertura: {Coverage:F1}%", 
                    coverage.LineCoverage);
                
                return Ok(coverage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas de cobertura");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene cálculo de deuda técnica
        /// </summary>
        /// <returns>Reporte de deuda técnica</returns>
        [HttpGet("technical-debt")]
        [ProducesResponseType(typeof(TechnicalDebtReport), 200)]
        public async Task<ActionResult<TechnicalDebtReport>> GetTechnicalDebt()
        {
            try
            {
                _logger.LogInformation("Calculando deuda técnica");
                var debt = await _metricsService.CalculateTechnicalDebtAsync();
                
                _logger.LogInformation("Deuda técnica calculada: {Hours:F1} horas ({Severity})", 
                    debt.TotalDebtHours, debt.SeverityLevel);
                
                return Ok(debt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular deuda técnica");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene resumen ejecutivo de métricas
        /// </summary>
        /// <returns>Resumen ejecutivo</returns>
        [HttpGet("executive-summary")]
        [ProducesResponseType(typeof(MetricsExecutiveSummary), 200)]
        public async Task<ActionResult<MetricsExecutiveSummary>> GetExecutiveSummary()
        {
            try
            {
                _logger.LogInformation("Generando resumen ejecutivo de métricas");
                
                var report = await _metricsService.GenerateMetricsReportAsync();
                var coverage = await _metricsService.GetCodeCoverageMetricsAsync();
                var debt = await _metricsService.CalculateTechnicalDebtAsync();
                
                var summary = GenerateExecutiveSummary(report, coverage, debt);
                
                _logger.LogInformation("Resumen ejecutivo generado: {Status}", summary.ProjectStatus);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar resumen ejecutivo");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene métricas de métodos con alta complejidad
        /// </summary>
        /// <param name="threshold">Umbral de complejidad (por defecto 10)</param>
        /// <returns>Lista de métodos con alta complejidad</returns>
        [HttpGet("high-complexity-methods")]
        [ProducesResponseType(typeof(List<MethodComplexityInfo>), 200)]
        public async Task<ActionResult<List<MethodComplexityInfo>>> GetHighComplexityMethods([FromQuery] int threshold = 10)
        {
            try
            {
                _logger.LogInformation("Obteniendo métodos con complejidad alta (umbral: {Threshold})", threshold);
                
                var complexityReport = await _metricsService.AnalyzeCyclomaticComplexityAsync();
                var highComplexityMethods = complexityReport.Methods
                    .Where(m => m.CyclomaticComplexity > threshold)
                    .OrderByDescending(m => m.CyclomaticComplexity)
                    .ToList();

                _logger.LogInformation("Encontrados {Count} métodos con alta complejidad", 
                    highComplexityMethods.Count);
                
                return Ok(highComplexityMethods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métodos de alta complejidad");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Exporta métricas completas en formato JSON
        /// </summary>
        /// <returns>Archivo JSON con todas las métricas</returns>
        [HttpGet("export")]
        public async Task<IActionResult> ExportMetrics()
        {
            try
            {
                _logger.LogInformation("Exportando métricas completas");
                
                var report = await _metricsService.GenerateMetricsReportAsync();
                var coverage = await _metricsService.GetCodeCoverageMetricsAsync();
                var debt = await _metricsService.CalculateTechnicalDebtAsync();
                
                var export = new
                {
                    GeneratedAt = DateTime.UtcNow,
                    MetricsReport = report,
                    CodeCoverage = coverage,
                    TechnicalDebt = debt,
                    ExecutiveSummary = GenerateExecutiveSummary(report, coverage, debt)
                };

                var json = JsonSerializer.Serialize(export, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var fileName = $"code-metrics-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
                
                _logger.LogInformation("Métricas exportadas exitosamente");
                return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar métricas");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene tendencias históricas de métricas (simulado)
        /// </summary>
        /// <param name="days">Número de días hacia atrás (por defecto 30)</param>
        /// <returns>Tendencias históricas</returns>
        [HttpGet("trends")]
        public async Task<ActionResult<object>> GetMetricsTrends([FromQuery] int days = 30)
        {
            try
            {
                _logger.LogInformation("Generando tendencias de métricas para {Days} días", days);
                
                var currentMetrics = await _metricsService.GenerateMetricsReportAsync();
                
                // Simular datos históricos para demostración
                var trends = GenerateSimulatedTrends(currentMetrics, days);
                
                _logger.LogInformation("Tendencias generadas exitosamente");
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar tendencias");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        private MetricsExecutiveSummary GenerateExecutiveSummary(
            CodeMetricsReport report, 
            CodeCoverageMetrics coverage, 
            TechnicalDebtReport debt)
        {
            var summary = new MetricsExecutiveSummary
            {
                GeneratedAt = DateTime.UtcNow
            };

            // Calcular puntuación general
            var scores = new List<int>();
            
            // Puntuación de complejidad
            var complexityScore = report.CyclomaticComplexity.AverageComplexity switch
            {
                <= 5 => 100,
                <= 10 => 80,
                <= 15 => 60,
                <= 20 => 40,
                _ => 20
            };
            scores.Add(complexityScore);

            // Puntuación de cobertura
            var coverageScore = (int)coverage.LineCoverage * 100 / 80; // 80% target
            scores.Add(Math.Min(100, coverageScore));

            // Puntuación de mantenibilidad
            var maintainabilityScore = (int)report.MaintainabilityMetrics.MaintainabilityIndex;
            scores.Add(maintainabilityScore);

            var overallScore = scores.Average();
            summary.OverallHealthScore = overallScore switch
            {
                >= 90 => "Excelente",
                >= 80 => "Muy Bueno",
                >= 70 => "Bueno",
                >= 60 => "Regular",
                >= 50 => "Deficiente",
                _ => "Crítico"
            };

            // Fortalezas
            if (report.CyclomaticComplexity.AverageComplexity <= 8)
                summary.KeyStrengths.Add("Complejidad ciclomática bajo control");
            
            if (coverage.LineCoverage >= 60)
                summary.KeyStrengths.Add("Cobertura de código aceptable");
            
            if (report.MaintainabilityMetrics.MaintainabilityIndex >= 70)
                summary.KeyStrengths.Add("Alto índice de mantenibilidad");
            
            if (report.ArchitecturalMetrics.AbstractionLevel >= 0.3)
                summary.KeyStrengths.Add("Buen nivel de abstracción");

            // Problemas críticos
            if (report.CyclomaticComplexity.HighComplexityMethods.Count > 10)
                summary.CriticalIssues.Add($"Demasiados métodos complejos ({report.CyclomaticComplexity.HighComplexityMethods.Count})");
            
            if (coverage.LineCoverage < 40)
                summary.CriticalIssues.Add($"Cobertura de código muy baja ({coverage.LineCoverage:F1}%)");
            
            if (debt.SeverityLevel == "Crítica")
                summary.CriticalIssues.Add($"Deuda técnica crítica ({debt.TotalDebtDays:F1} días)");

            // Recomendaciones
            if (report.CyclomaticComplexity.AverageComplexity > 10)
                summary.Recommendations.Add("Refactorizar métodos con alta complejidad ciclomática");
            
            if (coverage.LineCoverage < 80)
                summary.Recommendations.Add("Aumentar cobertura de pruebas unitarias");
            
            if (debt.TotalDebtDays > 5)
                summary.Recommendations.Add("Implementar plan de reducción de deuda técnica");

            // KPIs clave
            summary.KPIs = new Dictionary<string, string>
            {
                ["Complejidad Promedio"] = $"{report.CyclomaticComplexity.AverageComplexity:F1}",
                ["Cobertura de Líneas"] = $"{coverage.LineCoverage:F1}%",
                ["Índice Mantenibilidad"] = $"{report.MaintainabilityMetrics.MaintainabilityIndex:F0}",
                ["Deuda Técnica"] = $"{debt.TotalDebtHours:F1}h",
                ["Métodos Complejos"] = report.CyclomaticComplexity.HighComplexityMethods.Count.ToString(CultureInfo.InvariantCulture),
                ["Salud Arquitectónica"] = report.ArchitecturalMetrics.ArchitecturalHealth
            };

            return summary;
        }

        private object GenerateSimulatedTrends(CodeMetricsReport currentMetrics, int days)
        {
            var dates = Enumerable.Range(0, days)
                .Select(i => DateTime.UtcNow.AddDays(-i))
                .Reverse()
                .ToList();

            var random = new Random(42); // Seed fijo para consistencia

            return new
            {
                Period = $"Últimos {days} días",
                Dates = dates,
                ComplexityTrend = dates.Select(d => 
                    currentMetrics.CyclomaticComplexity.AverageComplexity + 
                    (random.NextDouble() - 0.5) * 2).ToList(),
                CoverageTrend = dates.Select(d => 
                    Math.Max(0, (double)currentMetrics.BasicMetrics.TotalMethods * 0.095 + 
                    (random.NextDouble() - 0.5) * 5)).ToList(),
                TechnicalDebtTrend = dates.Select(d => 
                    Math.Max(0, 50 + (random.NextDouble() - 0.5) * 20)).ToList()
            };
        }
    }
}
