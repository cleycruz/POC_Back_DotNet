namespace CarritoComprasAPI.Core.Metrics
{
    /// <summary>
    /// Reporte completo de métricas de código
    /// </summary>
    public class CodeMetricsReport
    {
        public DateTime GeneratedAt { get; set; }
        public string AssemblyName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public BasicCodeMetrics BasicMetrics { get; set; } = new();
        public CyclomaticComplexityReport CyclomaticComplexity { get; set; } = new();
        public MaintainabilityMetrics MaintainabilityMetrics { get; set; } = new();
        public ArchitecturalMetrics ArchitecturalMetrics { get; set; } = new();
    }

    /// <summary>
    /// Métricas básicas de código
    /// </summary>
    public class BasicCodeMetrics
    {
        public int TotalClasses { get; set; }
        public int TotalInterfaces { get; set; }
        public int TotalEnums { get; set; }
        public int TotalMethods { get; set; }
        public int TotalProperties { get; set; }
        public int NamespaceCount { get; set; }
        
        public int TotalTypes => TotalClasses + TotalInterfaces + TotalEnums;
        public double MethodsPerClass => TotalClasses > 0 ? (double)TotalMethods / TotalClasses : 0;
        public double PropertiesPerClass => TotalClasses > 0 ? (double)TotalProperties / TotalClasses : 0;
    }

    /// <summary>
    /// Reporte de complejidad ciclomática
    /// </summary>
    public class CyclomaticComplexityReport
    {
        public DateTime AnalyzedAt { get; set; }
        public List<MethodComplexityInfo> Methods { get; set; } = new();
        public double AverageComplexity { get; set; }
        public int MaxComplexity { get; set; }
        public List<MethodComplexityInfo> HighComplexityMethods { get; set; } = new();
        public Dictionary<string, int> ComplexityDistribution { get; set; } = new();
        
        public int TotalMethods => Methods.Count;
        public double ComplexityStandardDeviation => CalculateStandardDeviation();
        public string OverallComplexityLevel => GetOverallComplexityLevel();

        private double CalculateStandardDeviation()
        {
            if (Methods.Count == 0) return 0;
            
            var variance = Methods.Sum(m => Math.Pow(m.CyclomaticComplexity - AverageComplexity, 2)) / Methods.Count;
            return Math.Sqrt(variance);
        }

        private string GetOverallComplexityLevel()
        {
            return AverageComplexity switch
            {
                <= 5 => "Baja",
                <= 10 => "Moderada",
                <= 15 => "Alta",
                _ => "Muy Alta"
            };
        }
    }

    /// <summary>
    /// Información de complejidad de un método
    /// </summary>
    public class MethodComplexityInfo
    {
        public string TypeName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public int CyclomaticComplexity { get; set; }
        public string ComplexityLevel { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public int ParameterCount { get; set; }
        
        public string FullMethodName => $"{TypeName}.{MethodName}";
        public bool IsHighComplexity => CyclomaticComplexity > 10;
        public bool IsVeryHighComplexity => CyclomaticComplexity > 20;
        public double ComplexityDensity => LineCount > 0 ? (double)CyclomaticComplexity / LineCount : 0;
    }

    /// <summary>
    /// Métricas de cobertura de código
    /// </summary>
    public class CodeCoverageMetrics
    {
        public DateTime CollectedAt { get; set; }
        public decimal LineCoverage { get; set; }
        public decimal BranchCoverage { get; set; }
        public decimal MethodCoverage { get; set; }
        
        public int TotalLines { get; set; }
        public int CoveredLines { get; set; }
        public int UncoveredLines => TotalLines - CoveredLines;
        
        public int TotalBranches { get; set; }
        public int CoveredBranches { get; set; }
        public int UncoveredBranches => TotalBranches - CoveredBranches;
        
        public int TotalMethods { get; set; }
        public int CoveredMethods { get; set; }
        public int UncoveredMethods => TotalMethods - CoveredMethods;
        
        public Dictionary<string, decimal> ComponentCoverage { get; set; } = new();
        
        public string CoverageLevel => LineCoverage switch
        {
            >= 80 => "Excelente",
            >= 60 => "Buena",
            >= 40 => "Regular",
            >= 20 => "Baja",
            _ => "Muy Baja"
        };
    }

    /// <summary>
    /// Métricas de mantenibilidad
    /// </summary>
    public class MaintainabilityMetrics
    {
        public double AverageCyclomaticComplexity { get; set; }
        public int HighComplexityMethodsCount { get; set; }
        public double MaintainabilityIndex { get; set; }
        public double TechnicalDebtRatio { get; set; }
        
        public string MaintainabilityLevel => MaintainabilityIndex switch
        {
            >= 80 => "Excelente",
            >= 60 => "Buena",
            >= 40 => "Regular",
            >= 20 => "Baja",
            _ => "Muy Baja"
        };
        
        public bool RequiresRefactoring => MaintainabilityIndex < 60 || HighComplexityMethodsCount > 5;
    }

    /// <summary>
    /// Métricas arquitectónicas
    /// </summary>
    public class ArchitecturalMetrics
    {
        public int LayerCount { get; set; }
        public int ComponentCount { get; set; }
        public int DependencyCount { get; set; }
        public double AbstractionLevel { get; set; }
        public double InstabilityIndex { get; set; }
        
        public string ArchitecturalHealth => CalculateArchitecturalHealth();
        
        private string CalculateArchitecturalHealth()
        {
            var score = 0;
            
            // Evaluar capas (3-5 capas es ideal)
            if (LayerCount >= 3 && LayerCount <= 5) score += 25;
            else if (LayerCount >= 2 && LayerCount <= 6) score += 15;
            else score += 5;
            
            // Evaluar abstracción (30-70% es ideal)
            if (AbstractionLevel >= 0.3 && AbstractionLevel <= 0.7) score += 25;
            else if (AbstractionLevel >= 0.2 && AbstractionLevel <= 0.8) score += 15;
            else score += 5;
            
            // Evaluar estabilidad (baja inestabilidad es mejor)
            if (InstabilityIndex <= 0.3) score += 25;
            else if (InstabilityIndex <= 0.5) score += 15;
            else score += 5;
            
            // Evaluar componentes (menos es mejor para mantenibilidad)
            if (ComponentCount <= 10) score += 25;
            else if (ComponentCount <= 20) score += 15;
            else score += 5;
            
            return score switch
            {
                >= 85 => "Excelente",
                >= 70 => "Buena",
                >= 50 => "Regular",
                >= 30 => "Baja",
                _ => "Deficiente"
            };
        }
    }

    /// <summary>
    /// Reporte de deuda técnica
    /// </summary>
    public class TechnicalDebtReport
    {
        public DateTime CalculatedAt { get; set; }
        public double TotalDebtMinutes { get; set; }
        public double TotalDebtHours { get; set; }
        public double TotalDebtDays { get; set; }
        public double EstimatedCostUSD { get; set; }
        public string SeverityLevel { get; set; } = string.Empty;
        public Dictionary<string, double> DebtByCategory { get; set; } = new();
        
        public double DebtPercentage => CalculateDebtPercentage();
        public string PriorityAction => GetPriorityAction();
        
        private double CalculateDebtPercentage()
        {
            // Estimación del porcentaje de deuda sobre el tiempo total del proyecto
            var estimatedProjectHours = 2000; // Ejemplo: 2000 horas de proyecto
            return TotalDebtHours / estimatedProjectHours * 100;
        }
        
        private string GetPriorityAction()
        {
            return SeverityLevel switch
            {
                "Crítica" => "Refactoring inmediato requerido",
                "Alta" => "Planificar refactoring en próximo sprint",
                "Moderada" => "Considerar refactoring gradual",
                "Baja" => "Monitorear y mantener calidad actual",
                _ => "Evaluación requerida"
            };
        }
    }

    /// <summary>
    /// Resumen ejecutivo de métricas
    /// </summary>
    public class MetricsExecutiveSummary
    {
        public DateTime GeneratedAt { get; set; }
        public string OverallHealthScore { get; set; } = string.Empty;
        public List<string> KeyStrengths { get; set; } = new();
        public List<string> CriticalIssues { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public Dictionary<string, string> KPIs { get; set; } = new();
        
        public string ProjectStatus => DetermineProjectStatus();
        
        private string DetermineProjectStatus()
        {
            var criticalCount = CriticalIssues.Count;
            var strengthCount = KeyStrengths.Count;
            
            return (criticalCount, strengthCount) switch
            {
                (0, >= 5) => "Excelente estado",
                (0, >= 3) => "Buen estado",
                (1, >= 3) => "Estado aceptable",
                (var c, var s) when c <= 2 && s >= 2 => "Requiere atención",
                _ => "Requiere intervención urgente"
            };
        }
    }
}
