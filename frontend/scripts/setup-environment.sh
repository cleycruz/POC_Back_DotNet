#!/bin/bash

# Script para manejar variables de ambiente en deployment
# Se ejecuta antes del build para reemplazar tokens de configuración

set -e

# Función para reemplazar tokens en archivos
replace_tokens() {
    local file=$1
    echo "Reemplazando tokens en: $file"
    
    # Verificar si las variables de ambiente están definidas
    if [[ -z "${API_BASE_URL}" ]]; then
        echo "⚠️  Warning: API_BASE_URL no está definida"
    else
        sed -i "s|#{API_BASE_URL}#|${API_BASE_URL}|g" "$file"
        echo "✅ API_BASE_URL reemplazada: ${API_BASE_URL}"
    fi
    
    if [[ -z "${CDN_URL}" ]]; then
        echo "⚠️  Warning: CDN_URL no está definida"
    else
        sed -i "s|#{CDN_URL}#|${CDN_URL}|g" "$file"
        echo "✅ CDN_URL reemplazada: ${CDN_URL}"
    fi
    
    if [[ -z "${ANALYTICS_URL}" ]]; then
        echo "⚠️  Warning: ANALYTICS_URL no está definida"
    else
        sed -i "s|#{ANALYTICS_URL}#|${ANALYTICS_URL}|g" "$file"
        echo "✅ ANALYTICS_URL reemplazada: ${ANALYTICS_URL}"
    fi
    
    if [[ -z "${ERROR_REPORTING_URL}" ]]; then
        echo "⚠️  Warning: ERROR_REPORTING_URL no está definida"
    else
        sed -i "s|#{ERROR_REPORTING_URL}#|${ERROR_REPORTING_URL}|g" "$file"
        echo "✅ ERROR_REPORTING_URL reemplazada: ${ERROR_REPORTING_URL}"
    fi
}

# Función para mostrar información del ambiente
show_environment_info() {
    echo "=================================================="
    echo "🚀 Configuración de Variables de Ambiente"
    echo "=================================================="
    echo "Environment: ${ENVIRONMENT:-development}"
    echo "API_BASE_URL: ${API_BASE_URL:-'No definida'}"
    echo "CDN_URL: ${CDN_URL:-'No definida'}"
    echo "ANALYTICS_URL: ${ANALYTICS_URL:-'No definida'}"
    echo "ERROR_REPORTING_URL: ${ERROR_REPORTING_URL:-'No definida'}"
    echo "Node Environment: ${NODE_ENV:-'No definida'}"
    echo "=================================================="
}

# Función principal
main() {
    echo "🔧 Iniciando configuración de ambiente..."
    
    show_environment_info
    
    # Determinar el ambiente si no está definido
    if [[ -z "${ENVIRONMENT}" ]]; then
        if [[ "${NODE_ENV}" == "production" ]]; then
            ENVIRONMENT="production"
        elif [[ "${NODE_ENV}" == "test" ]]; then
            ENVIRONMENT="test"
        elif [[ "${NODE_ENV}" == "staging" ]]; then
            ENVIRONMENT="staging"
        else
            ENVIRONMENT="development"
        fi
        echo "📝 Ambiente determinado automáticamente: ${ENVIRONMENT}"
    fi
    
    # Crear backup de los archivos originales
    echo "📋 Creando backup de archivos de configuración..."
    if [[ -f "src/environments/environment.${ENVIRONMENT}.ts" ]]; then
        cp "src/environments/environment.${ENVIRONMENT}.ts" "src/environments/environment.${ENVIRONMENT}.ts.bak"
    fi
    
    # Reemplazar tokens en archivos de ambiente
    case $ENVIRONMENT in
        "production")
            echo "🏭 Configurando ambiente de PRODUCCIÓN..."
            replace_tokens "src/environments/environment.prod.ts"
            ;;
        "staging")
            echo "🎭 Configurando ambiente de STAGING..."
            replace_tokens "src/environments/environment.staging.ts"
            ;;
        "test")
            echo "🧪 Configurando ambiente de TESTING..."
            replace_tokens "src/environments/environment.test.ts"
            ;;
        *)
            echo "💻 Configurando ambiente de DESARROLLO..."
            echo "ℹ️  No se requieren reemplazos para desarrollo"
            ;;
    esac
    
    echo "✅ Configuración de ambiente completada"
}

# Función para limpiar archivos temporales
cleanup() {
    echo "🧹 Limpiando archivos temporales..."
    find src/environments -name "*.bak" -delete 2>/dev/null || true
}

# Función para restaurar archivos originales
restore() {
    echo "🔄 Restaurando archivos originales..."
    find src/environments -name "*.bak" | while read backup; do
        original="${backup%.bak}"
        mv "$backup" "$original"
        echo "✅ Restaurado: $original"
    done
}

# Manejar argumentos de línea de comandos
case "${1:-}" in
    "cleanup")
        cleanup
        ;;
    "restore")
        restore
        ;;
    "help"|"-h"|"--help")
        echo "Uso: $0 [cleanup|restore|help]"
        echo ""
        echo "Comandos:"
        echo "  (ninguno)  - Configurar variables de ambiente"
        echo "  cleanup    - Limpiar archivos temporales"
        echo "  restore    - Restaurar archivos originales"
        echo "  help       - Mostrar esta ayuda"
        echo ""
        echo "Variables de ambiente soportadas:"
        echo "  ENVIRONMENT           - Ambiente objetivo (development|test|staging|production)"
        echo "  API_BASE_URL         - URL base de la API"
        echo "  CDN_URL              - URL del CDN"
        echo "  ANALYTICS_URL        - URL del servicio de analytics"
        echo "  ERROR_REPORTING_URL  - URL del servicio de reporte de errores"
        ;;
    *)
        main
        ;;
esac
