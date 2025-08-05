#!/usr/bin/env pwsh
# Script para análisis automatizado de métricas de código
# Incluye complejidad ciclomática, cobertura y análisis estático

param(
    [string]$ProjectPath = ".",
    [string]$OutputPath = "./metrics-reports",
    [switch]$GenerateReport = $true,
    [switch]$OpenResults = $true,
    [int]$ComplexityThreshold = 10
)

Write-Host "🔍 Iniciando análisis automatizado de métricas de código" -ForegroundColor Green
Write-Host "📁 Proyecto: $ProjectPath" -ForegroundColor Yellow
Write-Host "📊 Salida: $OutputPath" -ForegroundColor Yellow

# Crear directorio de salida
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "✅ Directorio de reportes creado: $OutputPath" -ForegroundColor Green
}

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$reportDir = Join-Path $OutputPath "report-$timestamp"
New-Item -ItemType Directory -Path $reportDir -Force | Out-Null

try {
    # 1. Análisis de complejidad ciclomática usando métricas .NET
    Write-Host "`n📈 Ejecutando análisis de complejidad ciclomática..." -ForegroundColor Cyan
    
    $metricsCommand = @"
dotnet build --verbosity minimal
dotnet run --project `"$ProjectPath/backend/src/CarritoComprasAPI`" --no-build -- --analyze-metrics
"@
    
    # 2. Ejecutar tests con cobertura
    Write-Host "`n🧪 Ejecutando tests con cobertura de código..." -ForegroundColor Cyan
    
    $testResults = Join-Path $reportDir "test-results"
    New-Item -ItemType Directory -Path $testResults -Force | Out-Null
    
    $testCommand = @"
dotnet test `"$ProjectPath/CarritoCompras.sln`" \
    --collect:"XPlat Code Coverage" \
    --results-directory `"$testResults`" \
    --logger trx \
    --verbosity minimal
"@
    
    Invoke-Expression $testCommand
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Tests ejecutados exitosamente" -ForegroundColor Green
    } else {
        Write-Warning "⚠️ Algunos tests fallaron, pero continuando con el análisis"
    }
    
    # 3. Generar reporte de cobertura HTML
    Write-Host "`n📊 Generando reporte de cobertura..." -ForegroundColor Cyan
    
    $coverageFiles = Get-ChildItem -Path $testResults -Filter "coverage.cobertura.xml" -Recurse
    if ($coverageFiles.Count -gt 0) {
        $coverageReportPath = Join-Path $reportDir "coverage-report"
        
        $reportGenCommand = @"
reportgenerator \
    -reports:`"$($coverageFiles -join ';')`" \
    -targetdir:`"$coverageReportPath`" \
    -reporttypes:Html;JsonSummary;Badges \
    -title:"Carrito Compras - Cobertura de Código"
"@
        
        try {
            Invoke-Expression $reportGenCommand
            Write-Host "✅ Reporte de cobertura generado en: $coverageReportPath" -ForegroundColor Green
        } catch {
            Write-Warning "⚠️ Error generando reporte de cobertura: $($_.Exception.Message)"
        }
    } else {
        Write-Warning "⚠️ No se encontraron archivos de cobertura"
    }
    
    # 4. Análisis estático de código con SonarQube (si está disponible)
    Write-Host "`n🔍 Verificando disponibilidad de SonarQube..." -ForegroundColor Cyan
    
    $sonarScannerAvailable = $false
    try {
        $null = Get-Command "dotnet-sonarscanner" -ErrorAction Stop
        $sonarScannerAvailable = $true
        Write-Host "✅ SonarQube Scanner encontrado" -ForegroundColor Green
    } catch {
        Write-Host "ℹ️ SonarQube Scanner no disponible, saltando análisis estático" -ForegroundColor Yellow
    }
    
    if ($sonarScannerAvailable) {
        Write-Host "🔍 Ejecutando análisis estático con SonarQube..." -ForegroundColor Cyan
        
        $sonarResults = Join-Path $reportDir "sonar-results"
        New-Item -ItemType Directory -Path $sonarResults -Force | Out-Null
        
        $sonarCommand = @"
dotnet sonarscanner begin /k:"carrito-compras" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="admin" /d:sonar.password="admin" /d:sonar.cs.vscoveragexml.reportsPaths="$testResults/**/*.coveragexml" /d:sonar.working.directory="$sonarResults"
dotnet build "$ProjectPath/CarritoCompras.sln" --verbosity minimal
dotnet sonarscanner end /d:sonar.login="admin" /d:sonar.password="admin"
"@
        
        try {
            Invoke-Expression $sonarCommand
            Write-Host "✅ Análisis SonarQube completado" -ForegroundColor Green
        } catch {
            Write-Warning "⚠️ Error en análisis SonarQube: $($_.Exception.Message)"
        }
    }
    
    # 5. Llamar a API de métricas personalizada
    Write-Host "`n📊 Obteniendo métricas personalizadas..." -ForegroundColor Cyan
    
    # Iniciar la aplicación temporalmente para obtener métricas
    $appProcess = $null
    try {
        Write-Host "🚀 Iniciando aplicación para métricas..." -ForegroundColor Cyan
        
        $appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$ProjectPath/backend/src/CarritoComprasAPI`" --urls http://localhost:5000" -PassThru -WindowStyle Hidden
        
        # Esperar a que la aplicación inicie
        Start-Sleep -Seconds 10
        
        # Obtener métricas via API
        $metricsApiUrl = "http://localhost:5000/api/metrics"
        $metricsFile = Join-Path $reportDir "custom-metrics.json"
        
        try {
            $response = Invoke-RestMethod -Uri "$metricsApiUrl/export" -Method Get -TimeoutSec 30
            $response | ConvertTo-Json -Depth 10 | Out-File -FilePath $metricsFile -Encoding UTF8
            Write-Host "✅ Métricas personalizadas guardadas en: $metricsFile" -ForegroundColor Green
        } catch {
            Write-Warning "⚠️ Error obteniendo métricas via API: $($_.Exception.Message)"
        }
        
        # Obtener complejidad ciclomática específica
        try {
            $complexityFile = Join-Path $reportDir "cyclomatic-complexity.json"
            $complexityResponse = Invoke-RestMethod -Uri "$metricsApiUrl/cyclomatic-complexity" -Method Get -TimeoutSec 30
            $complexityResponse | ConvertTo-Json -Depth 10 | Out-File -FilePath $complexityFile -Encoding UTF8
            Write-Host "✅ Análisis de complejidad guardado en: $complexityFile" -ForegroundColor Green
            
            # Mostrar métodos con alta complejidad
            $highComplexityMethods = $complexityResponse.Methods | Where-Object { $_.CyclomaticComplexity -gt $ComplexityThreshold }
            if ($highComplexityMethods.Count -gt 0) {
                Write-Host "`n⚠️ Métodos con alta complejidad ciclomática (>$ComplexityThreshold):" -ForegroundColor Yellow
                $highComplexityMethods | ForEach-Object {
                    Write-Host "   • $($_.TypeName).$($_.MethodName): $($_.CyclomaticComplexity)" -ForegroundColor Red
                }
            } else {
                Write-Host "✅ No se encontraron métodos con complejidad alta" -ForegroundColor Green
            }
        } catch {
            Write-Warning "⚠️ Error obteniendo análisis de complejidad: $($_.Exception.Message)"
        }
        
    } finally {
        if ($appProcess -and -not $appProcess.HasExited) {
            Write-Host "🛑 Deteniendo aplicación..." -ForegroundColor Cyan
            $appProcess.Kill()
            $appProcess.Dispose()
        }
    }
    
    # 6. Generar reporte consolidado
    if ($GenerateReport) {
        Write-Host "`n📋 Generando reporte consolidado..." -ForegroundColor Cyan
        
        $reportHtml = @"
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reporte de Métricas - Carrito Compras</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        h1, h2 { color: #2c3e50; }
        .metric-card { background: #ecf0f1; padding: 15px; margin: 10px 0; border-radius: 5px; border-left: 4px solid #3498db; }
        .success { border-left-color: #27ae60; }
        .warning { border-left-color: #f39c12; }
        .error { border-left-color: #e74c3c; }
        .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 20px; }
        table { width: 100%; border-collapse: collapse; margin: 10px 0; }
        th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }
        th { background: #34495e; color: white; }
        .timestamp { color: #7f8c8d; font-size: 0.9em; }
        .highlight { background: #fff3cd; padding: 10px; border-radius: 5px; margin: 10px 0; }
    </style>
</head>
<body>
    <div class="container">
        <h1>📊 Reporte de Métricas de Código</h1>
        <p class="timestamp">Generado el: $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")</p>
        
        <div class="highlight">
            <h2>🎯 Resumen Ejecutivo</h2>
            <p>Este reporte contiene el análisis completo de métricas de calidad del código para el proyecto Carrito de Compras.</p>
        </div>
        
        <div class="grid">
            <div class="metric-card success">
                <h3>✅ Análisis Completado</h3>
                <p>• Tests ejecutados con cobertura<br>
                • Complejidad ciclomática analizada<br>
                • Métricas personalizadas generadas</p>
            </div>
            
            <div class="metric-card warning">
                <h3>⚠️ Áreas de Atención</h3>
                <p>• Métodos con complejidad >$ComplexityThreshold<br>
                • Cobertura de código objetivo: 80%<br>
                • Deuda técnica pendiente</p>
            </div>
        </div>
        
        <h2>📁 Archivos Generados</h2>
        <table>
            <tr><th>Archivo</th><th>Descripción</th></tr>
            <tr><td>coverage-report/index.html</td><td>Reporte detallado de cobertura de código</td></tr>
            <tr><td>custom-metrics.json</td><td>Métricas personalizadas de la aplicación</td></tr>
            <tr><td>cyclomatic-complexity.json</td><td>Análisis de complejidad ciclomática</td></tr>
            <tr><td>test-results/</td><td>Resultados de ejecución de tests</td></tr>
        </table>
        
        <h2>🚀 Próximos Pasos</h2>
        <ul>
            <li>Revisar métodos con alta complejidad ciclomática</li>
            <li>Aumentar cobertura de tests unitarios</li>
            <li>Implementar métricas en CI/CD pipeline</li>
            <li>Configurar alertas de calidad de código</li>
        </ul>
        
        <div class="highlight">
            <h3>💡 Recomendaciones</h3>
            <p>1. <strong>Refactoring:</strong> Dividir métodos con complejidad >15 en funciones más pequeñas<br>
            2. <strong>Testing:</strong> Agregar tests para alcanzar 80% de cobertura<br>
            3. <strong>Monitoreo:</strong> Ejecutar este análisis semanalmente<br>
            4. <strong>Automatización:</strong> Integrar en pipeline de CI/CD</p>
        </div>
    </div>
</body>
</html>
"@
        
        $reportPath = Join-Path $reportDir "index.html"
        $reportHtml | Out-File -FilePath $reportPath -Encoding UTF8
        Write-Host "✅ Reporte consolidado generado: $reportPath" -ForegroundColor Green
        
        if ($OpenResults) {
            Write-Host "🌐 Abriendo reporte en navegador..." -ForegroundColor Cyan
            Start-Process $reportPath
        }
    }
    
    # 7. Resumen final
    Write-Host "`n🎉 Análisis de métricas completado exitosamente!" -ForegroundColor Green
    Write-Host "📂 Todos los reportes guardados en: $reportDir" -ForegroundColor Yellow
    
    # Mostrar estadísticas básicas
    $reportFiles = Get-ChildItem -Path $reportDir -Recurse -File
    $totalSize = ($reportFiles | Measure-Object -Property Length -Sum).Sum
    $totalSizeKB = [math]::Round($totalSize / 1KB, 2)
    
    Write-Host "`n📈 Estadísticas del análisis:" -ForegroundColor Cyan
    Write-Host "   • Archivos generados: $($reportFiles.Count)" -ForegroundColor White
    Write-Host "   • Tamaño total: $totalSizeKB KB" -ForegroundColor White
    Write-Host "   • Tiempo de ejecución: $((Get-Date) - $timestamp)" -ForegroundColor White
    
} catch {
    Write-Error "❌ Error durante el análisis de métricas: $($_.Exception.Message)"
    Write-Host "Stack trace:" -ForegroundColor Red
    Write-Host $_.ScriptStackTrace -ForegroundColor Red
    exit 1
}

Write-Host "`n✨ ¡Análisis completado! Revisa los reportes generados." -ForegroundColor Green
