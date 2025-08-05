#!/bin/bash

# Script de Monitoreo de Progreso de Cobertura
# Ejecuta pruebas, genera reportes y muestra métricas de progreso

echo "🚀 INICIANDO ANÁLISIS DE COBERTURA DE CÓDIGO"
echo "=============================================="

# Directorio base
PROJECT_DIR="/Users/cley/Documents/carrito-compras"
cd "$PROJECT_DIR"

# Limpiar resultados anteriores
echo "🧹 Limpiando resultados anteriores..."
rm -rf TestResults/

# Ejecutar todas las pruebas con cobertura
echo "🧪 Ejecutando todas las pruebas..."
dotnet test CarritoCompras.sln --collect:"XPlat Code Coverage" --results-directory:"./TestResults" --verbosity minimal

# Verificar si las pruebas pasaron
if [ $? -ne 0 ]; then
    echo "❌ ERROR: Las pruebas fallaron. Abortando análisis."
    exit 1
fi

# Generar reporte de cobertura
echo "📊 Generando reporte de cobertura..."
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReports" -reporttypes:"Html;TextSummary"

# Extraer métricas clave del reporte
SUMMARY_FILE="TestResults/CoverageReports/Summary.txt"
if [ -f "$SUMMARY_FILE" ]; then
    echo ""
    echo "📈 RESUMEN DE COBERTURA"
    echo "======================"
    
    # Extraer métricas principales
    LINE_COVERAGE=$(grep "Line coverage:" "$SUMMARY_FILE" | sed 's/.*Line coverage: //' | sed 's/%//')
    BRANCH_COVERAGE=$(grep "Branch coverage:" "$SUMMARY_FILE" | sed 's/.*Branch coverage: //' | sed 's/%.*//')
    METHOD_COVERAGE=$(grep "Method coverage:" "$SUMMARY_FILE" | sed 's/.*Method coverage: //' | sed 's/%.*//')
    TOTAL_TESTS=$(grep "total:" "$SUMMARY_FILE" -A 10 | grep -o "total: [0-9]*" | head -1 | sed 's/total: //')
    
    echo "📏 Cobertura de Líneas: ${LINE_COVERAGE}%"
    echo "🌳 Cobertura de Branches: ${BRANCH_COVERAGE}%"
    echo "🔧 Cobertura de Métodos: ${METHOD_COVERAGE}%"
    echo "🧪 Total de Pruebas: ${TOTAL_TESTS}"
    
    # Calcular progreso hacia meta del 80%
    TARGET=80
    LINE_PROGRESS=$(echo "scale=1; $LINE_COVERAGE * 100 / $TARGET" | bc)
    BRANCH_PROGRESS=$(echo "scale=1; $BRANCH_COVERAGE * 100 / $TARGET" | bc)
    METHOD_PROGRESS=$(echo "scale=1; $METHOD_COVERAGE * 100 / $TARGET" | bc)
    
    echo ""
    echo "🎯 PROGRESO HACIA META 80%"
    echo "=========================="
    echo "📏 Líneas: ${LINE_PROGRESS}% completado"
    echo "🌳 Branches: ${BRANCH_PROGRESS}% completado"
    echo "🔧 Métodos: ${METHOD_PROGRESS}% completado"
    
    # Mostrar top 10 componentes con mejor cobertura
    echo ""
    echo "🏆 TOP 10 COMPONENTES CON MEJOR COBERTURA"
    echo "========================================"
    grep -E "  .*[0-9]+(\.[0-9]+)?%" "$SUMMARY_FILE" | grep -v "0%" | sort -k2 -nr | head -10
    
    # Mostrar componentes sin cobertura que necesitan atención
    echo ""
    echo "⚠️  COMPONENTES SIN COBERTURA (PRIORIDAD)"
    echo "========================================"
    grep -E "  .*0%" "$SUMMARY_FILE" | head -10
    
    # Verificar si la API está corriendo para obtener métricas de deuda técnica
    echo ""
    echo "💰 MÉTRICAS DE DEUDA TÉCNICA"
    echo "==========================="
    
    # Intentar obtener métricas de la API
    API_RESPONSE=$(curl -s -f http://localhost:5010/api/metrics/technical-debt 2>/dev/null)
    if [ $? -eq 0 ]; then
        echo "$API_RESPONSE" | jq '
        {
            "Deuda Total (horas)": .totalDebtHours,
            "Costo Estimado (USD)": .estimatedCostUSD,
            "Nivel de Severidad": .severityLevel,
            "Distribución": .debtByCategory
        }'
        
        # Calcular reducción potencial de deuda técnica
        CURRENT_DEBT=$(echo "$API_RESPONSE" | jq -r '.totalDebtHours')
        COVERAGE_DEBT=$(echo "$API_RESPONSE" | jq -r '.debtByCategory["Baja Cobertura"]')
        COVERAGE_DEBT_HOURS=$(echo "scale=1; $COVERAGE_DEBT / 60" | bc)
        
        # Estimar reducción basada en mejora de cobertura
        POTENTIAL_REDUCTION=$(echo "scale=1; $COVERAGE_DEBT_HOURS * $LINE_COVERAGE / 100" | bc)
        REMAINING_DEBT=$(echo "scale=1; $CURRENT_DEBT - $POTENTIAL_REDUCTION" | bc)
        
        echo ""
        echo "📉 ANÁLISIS DE REDUCCIÓN DE DEUDA"
        echo "==============================="
        echo "💡 Deuda por baja cobertura: ${COVERAGE_DEBT_HOURS}h"
        echo "✅ Reducción lograda: ${POTENTIAL_REDUCTION}h"
        echo "📊 Deuda restante estimada: ${REMAINING_DEBT}h"
        
    else
        echo "⚠️  API no disponible. Inicie la aplicación para ver métricas de deuda técnica:"
        echo "   cd backend/src/CarritoComprasAPI && dotnet run --urls=\"http://localhost:5010\""
    fi
    
    echo ""
    echo "🔗 ENLACES ÚTILES"
    echo "================"
    echo "📊 Reporte HTML: file://$PROJECT_DIR/TestResults/CoverageReports/index.html"
    echo "📋 Resumen detallado: $SUMMARY_FILE"
    echo "📈 Dashboard métricas: http://localhost:5010/api/metrics/dashboard (si API activa)"
    
    echo ""
    echo "✅ ANÁLISIS COMPLETADO"
    echo "====================="
    echo "Para continuar mejorando la cobertura, consulte:"
    echo "📖 Plan de mejora: backend/docs/PLAN_MEJORA_COBERTURA.md"
    
else
    echo "❌ ERROR: No se pudo generar el reporte de cobertura"
    exit 1
fi
