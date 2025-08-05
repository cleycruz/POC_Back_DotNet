#!/bin/bash

# 📊 Script de Análisis de Métricas Automatizadas
# Carrito de Compras - Sistema de Calidad de Código

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuración
PROJECT_NAME="CarritoComprasAPI"
BASE_URL="http://localhost:5063"
OUTPUT_DIR="./reports"
COVERAGE_THRESHOLD=80
COMPLEXITY_THRESHOLD=10
DEBT_THRESHOLD=24

echo -e "${BLUE}🚀 INICIANDO ANÁLISIS DE MÉTRICAS AUTOMATIZADAS${NC}"
echo -e "${BLUE}================================================${NC}"
echo ""

# Función para mostrar progreso
show_progress() {
    echo -e "${CYAN}▶ $1${NC}"
}

# Función para mostrar éxito
show_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

# Función para mostrar advertencia
show_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Función para mostrar error
show_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Verificar que la API esté corriendo
check_api_status() {
    show_progress "Verificando estado de la API..."
    
    if curl -s -f "$BASE_URL/api/metrics/report" > /dev/null 2>&1; then
        show_success "API de métricas está corriendo correctamente"
    else
        show_error "La API no está disponible en $BASE_URL"
        echo -e "${YELLOW}Por favor, ejecute: dotnet run --project backend/src/CarritoComprasAPI${NC}"
        exit 1
    fi
}

# Crear directorio de reportes
setup_output_directory() {
    show_progress "Configurando directorio de reportes..."
    mkdir -p "$OUTPUT_DIR"
    show_success "Directorio de reportes creado: $OUTPUT_DIR"
}

# Obtener métricas de complejidad ciclomática
analyze_cyclomatic_complexity() {
    show_progress "Analizando complejidad ciclomática..."
    
    local response=$(curl -s "$BASE_URL/api/metrics/cyclomatic-complexity")
    local avg_complexity=$(echo "$response" | jq -r '.averageComplexity')
    local total_methods=$(echo "$response" | jq -r '.totalMethods')
    local high_complexity=$(echo "$response" | jq -r '.highComplexityMethods | length')
    
    echo "$response" > "$OUTPUT_DIR/cyclomatic-complexity.json"
    
    echo -e "${PURPLE}📊 COMPLEJIDAD CICLOMÁTICA:${NC}"
    echo -e "   • Complejidad promedio: ${YELLOW}$avg_complexity${NC}"
    echo -e "   • Total de métodos: ${YELLOW}$total_methods${NC}"
    echo -e "   • Métodos de alta complejidad: ${YELLOW}$high_complexity${NC}"
    
    if (( $(echo "$avg_complexity > $COMPLEXITY_THRESHOLD" | bc -l) )); then
        show_warning "Complejidad promedio ($avg_complexity) supera el umbral ($COMPLEXITY_THRESHOLD)"
        return 1
    else
        show_success "Complejidad dentro del rango aceptable"
        return 0
    fi
}

# Obtener métricas de cobertura
analyze_code_coverage() {
    show_progress "Analizando cobertura de código..."
    
    local response=$(curl -s "$BASE_URL/api/metrics/code-coverage")
    local line_coverage=$(echo "$response" | jq -r '.lineCoverage')
    local branch_coverage=$(echo "$response" | jq -r '.branchCoverage')
    local method_coverage=$(echo "$response" | jq -r '.methodCoverage')
    
    echo "$response" > "$OUTPUT_DIR/code-coverage.json"
    
    echo -e "${PURPLE}📈 COBERTURA DE CÓDIGO:${NC}"
    echo -e "   • Cobertura de líneas: ${YELLOW}${line_coverage}%${NC}"
    echo -e "   • Cobertura de ramas: ${YELLOW}${branch_coverage}%${NC}"
    echo -e "   • Cobertura de métodos: ${YELLOW}${method_coverage}%${NC}"
    
    if (( $(echo "$line_coverage < $COVERAGE_THRESHOLD" | bc -l) )); then
        show_warning "Cobertura de líneas ($line_coverage%) está bajo el umbral ($COVERAGE_THRESHOLD%)"
        return 1
    else
        show_success "Cobertura de código aceptable"
        return 0
    fi
}

# Obtener métricas de deuda técnica
analyze_technical_debt() {
    show_progress "Analizando deuda técnica..."
    
    local response=$(curl -s "$BASE_URL/api/metrics/technical-debt")
    local total_debt=$(echo "$response" | jq -r '.totalDebtHours')
    local severity=$(echo "$response" | jq -r '.severityLevel')
    local issues_count=$(echo "$response" | jq -r '.topIssues | length')
    
    echo "$response" > "$OUTPUT_DIR/technical-debt.json"
    
    echo -e "${PURPLE}💳 DEUDA TÉCNICA:${NC}"
    echo -e "   • Total de horas: ${YELLOW}${total_debt}h${NC}"
    echo -e "   • Nivel de severidad: ${YELLOW}$severity${NC}"
    echo -e "   • Número de issues: ${YELLOW}$issues_count${NC}"
    
    if (( $(echo "$total_debt > $DEBT_THRESHOLD" | bc -l) )); then
        show_warning "Deuda técnica ($total_debt h) supera el umbral ($DEBT_THRESHOLD h)"
        return 1
    else
        show_success "Deuda técnica bajo control"
        return 0
    fi
}

# Obtener reporte completo
generate_comprehensive_report() {
    show_progress "Generando reporte completo..."
    
    local response=$(curl -s "$BASE_URL/api/metrics/report")
    echo "$response" > "$OUTPUT_DIR/comprehensive-report.json"
    
    local maintainability=$(echo "$response" | jq -r '.maintainabilityMetrics.maintainabilityIndex')
    
    echo -e "${PURPLE}🔧 MÉTRICAS ADICIONALES:${NC}"
    echo -e "   • Índice de mantenibilidad: ${YELLOW}$maintainability${NC}"
    
    show_success "Reporte completo generado"
}

# Exportar datos para análisis externo
export_metrics_data() {
    show_progress "Exportando datos para análisis externo..."
    
    curl -s "$BASE_URL/api/metrics/export" > "$OUTPUT_DIR/metrics-export.json"
    
    show_success "Datos exportados a $OUTPUT_DIR/metrics-export.json"
}

# Generar resumen ejecutivo
generate_executive_summary() {
    show_progress "Generando resumen ejecutivo..."
    
    local summary_file="$OUTPUT_DIR/executive-summary.md"
    
    cat > "$summary_file" << EOF
# 📊 Resumen Ejecutivo - Análisis de Métricas de Código

**Proyecto:** $PROJECT_NAME  
**Fecha de análisis:** $(date '+%Y-%m-%d %H:%M:%S')  
**Generado por:** Sistema de Métricas Automatizadas

## 🎯 Estado General

EOF

    # Evaluar estado general basado en las métricas
    local overall_status="✅ EXCELENTE"
    local warnings=0
    
    # Verificar cada métrica
    if ! analyze_cyclomatic_complexity > /dev/null 2>&1; then
        warnings=$((warnings + 1))
    fi
    
    if ! analyze_code_coverage > /dev/null 2>&1; then
        warnings=$((warnings + 1))
    fi
    
    if ! analyze_technical_debt > /dev/null 2>&1; then
        warnings=$((warnings + 1))
    fi
    
    if [ $warnings -eq 1 ]; then
        overall_status="⚠️ BUENO (con observaciones)"
    elif [ $warnings -eq 2 ]; then
        overall_status="🔶 NECESITA ATENCIÓN"
    elif [ $warnings -ge 3 ]; then
        overall_status="🔴 REQUIERE ACCIÓN INMEDIATA"
    fi
    
    echo "**Estado general:** $overall_status" >> "$summary_file"
    echo "" >> "$summary_file"
    
    cat >> "$summary_file" << EOF
## 📈 Métricas Principales

| Métrica | Valor | Estado | Umbral |
|---------|-------|--------|--------|
| Complejidad Ciclomática | $(curl -s "$BASE_URL/api/metrics/cyclomatic-complexity" | jq -r '.averageComplexity') | $([ $warnings -eq 0 ] && echo "✅" || echo "⚠️") | < $COMPLEXITY_THRESHOLD |
| Cobertura de Código | $(curl -s "$BASE_URL/api/metrics/code-coverage" | jq -r '.lineCoverage')% | $([ $warnings -eq 0 ] && echo "✅" || echo "⚠️") | > $COVERAGE_THRESHOLD% |
| Deuda Técnica | $(curl -s "$BASE_URL/api/metrics/technical-debt" | jq -r '.totalDebtHours')h | $([ $warnings -eq 0 ] && echo "✅" || echo "⚠️") | < $DEBT_THRESHOLD h |

## 🔧 Recomendaciones

EOF

    if [ $warnings -eq 0 ]; then
        echo "- ✅ **Excelente trabajo!** Todas las métricas están dentro de los rangos óptimos" >> "$summary_file"
        echo "- 🚀 Continuar con las buenas prácticas de desarrollo" >> "$summary_file"
        echo "- 📊 Mantener el monitoreo continuo de métricas" >> "$summary_file"
    else
        echo "- 🔍 **Revisar** las métricas que están fuera de los umbrales establecidos" >> "$summary_file"
        echo "- 📝 **Planificar** actividades de refactoring para mejorar la calidad" >> "$summary_file"
        echo "- 🧪 **Incrementar** la cobertura de tests unitarios" >> "$summary_file"
        echo "- 🔄 **Implementar** revisiones de código más estrictas" >> "$summary_file"
    fi
    
    cat >> "$summary_file" << EOF

## 📁 Archivos Generados

- \`comprehensive-report.json\` - Reporte completo con todas las métricas
- \`cyclomatic-complexity.json\` - Análisis detallado de complejidad
- \`code-coverage.json\` - Métricas de cobertura de tests
- \`technical-debt.json\` - Análisis de deuda técnica
- \`metrics-export.json\` - Datos en formato para análisis externo

## 🌐 Dashboard Web

Accede al dashboard interactivo en: $BASE_URL/dashboard

---
*Generado automáticamente por el Sistema de Métricas de Carrito de Compras*
EOF

    show_success "Resumen ejecutivo generado: $summary_file"
}

# Función principal
main() {
    echo -e "${BLUE}Proyecto: $PROJECT_NAME${NC}"
    echo -e "${BLUE}Fecha: $(date '+%Y-%m-%d %H:%M:%S')${NC}"
    echo ""
    
    # Ejecutar análisis
    check_api_status
    setup_output_directory
    
    echo ""
    echo -e "${BLUE}🔍 EJECUTANDO ANÁLISIS DE MÉTRICAS${NC}"
    echo -e "${BLUE}====================================${NC}"
    echo ""
    
    analyze_cyclomatic_complexity
    echo ""
    
    analyze_code_coverage
    echo ""
    
    analyze_technical_debt
    echo ""
    
    generate_comprehensive_report
    echo ""
    
    export_metrics_data
    echo ""
    
    generate_executive_summary
    echo ""
    
    echo -e "${GREEN}✨ ANÁLISIS COMPLETADO EXITOSAMENTE${NC}"
    echo -e "${GREEN}===================================${NC}"
    echo ""
    echo -e "${CYAN}📁 Reportes generados en: $OUTPUT_DIR${NC}"
    echo -e "${CYAN}🌐 Dashboard web: $BASE_URL/dashboard${NC}"
    echo -e "${CYAN}📊 API de métricas: $BASE_URL/api/metrics${NC}"
    echo ""
    echo -e "${YELLOW}💡 Próximos pasos:${NC}"
    echo -e "   1. Revisar el resumen ejecutivo: ${YELLOW}$OUTPUT_DIR/executive-summary.md${NC}"
    echo -e "   2. Abrir el dashboard web para análisis interactivo"
    echo -e "   3. Integrar con CI/CD para análisis automático"
    echo ""
}

# Ejecutar script principal
main "$@"
