# 📊 Sistema de Métricas Automatizadas - Carrito Compras

## 🎯 Visión General

Este sistema implementa análisis automatizado de **complejidad ciclomática** y métricas de calidad de código, proporcionando insights en tiempo real sobre el estado del codebase.

## ✨ Características Principales

### 🔄 Complejidad Ciclomática
- **Análisis automático** de la complejidad de métodos
- **Categorización por niveles**: Baja, Moderada, Alta, Muy Alta
- **Identificación de hot-spots** que requieren refactoring
- **Métricas de distribución** por tipo de complejidad

### 📈 Métricas de Cobertura
- **Cobertura de líneas**, ramas y métodos
- **Análisis de tests unitarios** y de integración
- **Reportes visuales** con ReportGenerator
- **Thresholds automáticos** de calidad

### 💳 Deuda Técnica
- **Cálculo automático** del tiempo de resolución
- **Priorización por severidad**: Baja, Moderada, Alta, Crítica
- **Estimaciones de esfuerzo** basadas en patrones de código
- **Tendencias y proyecciones**

### 🔧 Índice de Mantenibilidad
- **Análisis Halstead** de complejidad
- **Métricas de acoplamiento** entre clases
- **Evaluación de la cohesión** de módulos
- **Recomendaciones específicas** de mejora

## 🚀 Instalación y Configuración

### Prerrequisitos
```bash
# .NET 9 SDK
dotnet --version  # >= 9.0.0

# ReportGenerator (para cobertura)
dotnet tool install -g dotnet-reportgenerator-globaltool

# PowerShell (para automatización)
pwsh --version  # >= 7.0
```

### Configuración del Proyecto
```bash
# 1. Instalar dependencias de análisis
cd backend/src/CarritoComprasAPI
dotnet add package Microsoft.CodeAnalysis.Metrics
dotnet add package coverlet.collector
dotnet add package ReportGenerator

# 2. Restaurar paquetes
dotnet restore

# 3. Ejecutar primera vez
dotnet run
```

## 📊 Dashboard Interactivo

### Acceso al Dashboard
```bash
# Iniciar la aplicación
dotnet run --project backend/src/CarritoComprasAPI

# Abrir en navegador
http://localhost:5000/dashboard
# o
http://localhost:5000/metrics-dashboard.html
```

### Funcionalidades del Dashboard

#### 🎯 Métricas Principales
- **Cobertura de Código**: Porcentaje visual con categorización por colores
- **Complejidad Ciclomática**: Promedio con distribución detallada
- **Deuda Técnica**: Tiempo estimado de resolución
- **Índice de Mantenibilidad**: Score de 0-100 con recomendaciones

#### 📈 Visualizaciones Avanzadas
- **Gráficos de progreso** con animaciones CSS
- **Distribución de complejidad** por categorías
- **Tendencias temporales** con indicadores de mejora
- **Alertas automáticas** cuando las métricas degradan

#### 🔄 Actualización en Tiempo Real
- **Auto-refresh** cada 5 minutos
- **Botón de actualización manual** para análisis inmediato
- **Estado de carga** con indicadores visuales
- **Cache inteligente** para optimizar rendimiento

## 🛠️ API REST de Métricas

### Endpoints Disponibles

#### Análisis General
```http
GET /api/metrics/report
```
**Respuesta**: Reporte completo con todas las métricas

#### Complejidad Ciclomática
```http
GET /api/metrics/cyclomatic-complexity
```
**Respuesta**:
```json
{
  "totalMethods": 45,
  "averageComplexity": 3.2,
  "maxComplexity": 12,
  "complexityDistribution": {
    "Baja (1-5)": 38,
    "Moderada (6-10)": 6,
    "Alta (11-15)": 1,
    "Muy Alta (16+)": 0
  },
  "highComplexityMethods": [
    {
      "methodName": "ProcessOrder",
      "className": "CarritoController",
      "complexity": 12,
      "recommendation": "Dividir en métodos más pequeños"
    }
  ]
}
```

#### Cobertura de Código
```http
GET /api/metrics/code-coverage
```
**Respuesta**:
```json
{
  "lineCoverage": 84.5,
  "branchCoverage": 78.2,
  "methodCoverage": 92.1,
  "totalLines": 2450,
  "coveredLines": 2070,
  "uncoveredFiles": ["Models/ComplexEntity.cs"]
}
```

#### Deuda Técnica
```http
GET /api/metrics/technical-debt
```
**Respuesta**:
```json
{
  "totalDebtHours": 12.5,
  "severityLevel": "Moderada",
  "topIssues": [
    {
      "issue": "Método con alta complejidad",
      "file": "Controllers/CarritoController.cs",
      "estimatedHours": 4.0,
      "priority": "Alta"
    }
  ]
}
```

#### Exportación de Datos
```http
GET /api/metrics/export
```
**Respuesta**: JSON completo con todas las métricas para análisis externo

## 🤖 Automatización con PowerShell

### Script Principal
```bash
# Ejecutar análisis completo
./scripts/analyze-metrics.ps1

# Con parámetros específicos
./scripts/analyze-metrics.ps1 -Project "CarritoComprasAPI" -OutputPath "./reports"
```

### Funcionalidades del Script
- **Ejecución de tests** con cobertura automática
- **Generación de reportes HTML** con ReportGenerator
- **Llamadas a API** para métricas en tiempo real
- **Consolidación de resultados** en reporte unificado
- **Notificaciones** cuando se detectan degradaciones

### Configuración Personalizada
```powershell
# En analyze-metrics.ps1
$Config = @{
    CoverageThreshold = 80
    ComplexityThreshold = 10
    DebtThreshold = 24  # horas
    MaintainabilityThreshold = 70
}
```

## 🔄 Integración CI/CD

### GitHub Actions

El workflow `.github/workflows/code-metrics.yml` ejecuta automáticamente:

1. **En cada Push/PR**:
   - Análisis de complejidad ciclomática
   - Ejecución de tests con cobertura
   - Cálculo de deuda técnica
   - Validación de thresholds

2. **Reportes Automáticos**:
   - Comentarios en PRs con métricas
   - Badges de estado en README
   - Notificaciones en Teams/Slack

3. **Quality Gates**:
   - Bloqueo de merge si cobertura < 80%
   - Advertencias si complejidad > 10
   - Alertas si deuda técnica aumenta

### Configuración del Workflow
```yaml
name: Code Quality Metrics

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  metrics:
    runs-on: ubuntu-latest
    steps:
      - name: Analyze Code Metrics
        run: |
          dotnet test --collect:"XPlat Code Coverage"
          ./scripts/analyze-metrics.ps1
          
      - name: Upload Reports
        uses: actions/upload-artifact@v3
        with:
          name: metrics-report
          path: reports/
```

## 📈 SonarQube Integration

### Configuración
```xml
<!-- sonarqube.proj -->
<Project>
  <PropertyGroup>
    <SonarQubeTestProject>false</SonarQubeTestProject>
    <SonarQubeCoverageReportPaths>coverage/SonarQube.xml</SonarQubeCoverageReportPaths>
  </PropertyGroup>
</Project>
```

### Análisis Avanzado
- **Duplicación de código**
- **Vulnerabilidades de seguridad**
- **Code smells** automáticos
- **Maintainability rating**

## 🎯 Interpretación de Métricas

### Complejidad Ciclomática

| Rango | Nivel | Interpretación | Acción Recomendada |
|-------|-------|---------------|-------------------|
| 1-5 | Baja | ✅ Código simple y mantenible | Mantener |
| 6-10 | Moderada | ⚠️ Complejidad aceptable | Monitorear |
| 11-15 | Alta | 🔶 Refactoring recomendado | Dividir método |
| 16+ | Muy Alta | 🔴 Refactoring urgente | Reestructurar |

### Cobertura de Código

| Porcentaje | Nivel | Estado | Acción |
|------------|-------|--------|--------|
| 90%+ | Excelente | ✅ | Mantener |
| 80-89% | Buena | ✅ | Mejorar gradualmente |
| 60-79% | Aceptable | ⚠️ | Aumentar tests |
| <60% | Insuficiente | 🔴 | Plan de mejora urgente |

### Deuda Técnica

| Tiempo | Severidad | Impacto | Prioridad |
|--------|-----------|---------|-----------|
| <8h | Baja | Mínimo | Cuando sea posible |
| 8-24h | Moderada | Medio | Próximo sprint |
| 24-72h | Alta | Alto | Esta iteración |
| >72h | Crítica | Crítico | Inmediato |

## 🔧 Personalización

### Añadir Nuevas Métricas

1. **Extender CodeMetricsService**:
```csharp
public async Task<CustomMetric> AnalyzeCustomMetricAsync()
{
    // Implementar lógica de análisis
    return new CustomMetric { /* datos */ };
}
```

2. **Crear Endpoint**:
```csharp
[HttpGet("custom-metric")]
public async Task<IActionResult> GetCustomMetric()
{
    var result = await _metricsService.AnalyzeCustomMetricAsync();
    return Ok(result);
}
```

3. **Actualizar Dashboard**:
```javascript
async function loadCustomMetric() {
    const response = await fetch('/api/metrics/custom-metric');
    const data = await response.json();
    updateCustomMetricCard(data);
}
```

### Configurar Alertas

1. **Teams/Slack Webhooks**:
```powershell
# En analyze-metrics.ps1
if ($Coverage -lt $CoverageThreshold) {
    Send-TeamsNotification -Message "Cobertura bajo threshold: $Coverage%"
}
```

2. **Email Notifications**:
```csharp
// En MetricsController
if (metrics.CoveragePercentage < threshold)
{
    await _emailService.SendAlertAsync("Coverage Alert", metrics);
}
```

## 📚 Referencias y Recursos

### Documentación Técnica
- [Complejidad Ciclomática - Wikipedia](https://en.wikipedia.org/wiki/Cyclomatic_complexity)
- [Microsoft.CodeAnalysis.Metrics](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.metrics)
- [ReportGenerator Documentation](https://reportgenerator.io/)

### Best Practices
- [Clean Code - Robert Martin](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [Refactoring - Martin Fowler](https://refactoring.com/)
- [SonarQube Quality Gate](https://docs.sonarqube.org/latest/user-guide/quality-gates/)

### Herramientas Complementarias
- **NDepend**: Análisis estático avanzado
- **PerfView**: Profiling de rendimiento
- **JetBrains dotTrace**: Análisis de memoria
- **Application Insights**: Monitoreo en producción

## 🔄 Roadmap de Mejoras

### Próximas Características
- [ ] **Machine Learning** para predicción de bugs
- [ ] **Análisis de rendimiento** automatizado
- [ ] **Integración con Azure DevOps** Boards
- [ ] **Reportes ejecutivos** con Power BI
- [ ] **Alertas proactivas** basadas en tendencias

### Optimizaciones Técnicas
- [ ] **Cache distribuido** para métricas
- [ ] **Procesamiento asíncrono** de análisis largos
- [ ] **API GraphQL** para consultas flexibles
- [ ] **WebSockets** para updates en tiempo real
- [ ] **Containerización** con Docker

---

## 🎉 ¡Listo para Usar!

```bash
# Inicio rápido
git clone <repository>
cd carrito-compras
dotnet run --project backend/src/CarritoComprasAPI
# Abrir http://localhost:5000/dashboard
```

**¡Tu sistema de métricas automatizadas está listo!** 🚀

¿Dudas? Consulta la [documentación completa](./docs/api/) o abre un [issue](../../issues/new).
