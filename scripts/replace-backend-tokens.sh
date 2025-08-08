#!/bin/bash

# Script para reemplazar tokens en archivos de configuración del backend
# Uso: ./replace-backend-tokens.sh <environment> [base_path]

set -e

ENVIRONMENT=${1:-development}
BASE_PATH=${2:-"./"}
CONFIG_FILE="${BASE_PATH}/appsettings.${ENVIRONMENT}.json"

echo "🔧 Reemplazando tokens de configuración para ambiente: $ENVIRONMENT"
echo "📁 Archivo de configuración: $CONFIG_FILE"

# Verificar que el archivo de configuración existe
if [ ! -f "$CONFIG_FILE" ]; then
    echo "❌ Error: No se encontró el archivo de configuración $CONFIG_FILE"
    exit 1
fi

# Función para reemplazar tokens
replace_token() {
    local token=$1
    local value=$2
    local file=$3
    
    if [ -n "$value" ]; then
        echo "🔄 Reemplazando $token -> $value"
        sed -i.bak "s|{{${token}}}|${value}|g" "$file"
    else
        echo "⚠️  Warning: Variable de entorno $token no definida"
    fi
}

# Función para limpiar archivos de respaldo
cleanup_backup_files() {
    find "$BASE_PATH" -name "*.bak" -delete 2>/dev/null || true
    echo "🧹 Archivos de respaldo limpiados"
}

echo "📋 Variables de entorno disponibles:"
echo "   BACKEND_API_BASE_URL: ${BACKEND_API_BASE_URL:-'NO DEFINIDA'}"
echo "   BACKEND_API_PORT: ${BACKEND_API_PORT:-'NO DEFINIDA'}"
echo "   BACKEND_DATABASE_CONNECTION_STRING: ${BACKEND_DATABASE_CONNECTION_STRING:-'NO DEFINIDA'}"
echo "   BACKEND_JWT_SECRET: ${BACKEND_JWT_SECRET:-'NO DEFINIDA'}"
echo "   BACKEND_JWT_ISSUER: ${BACKEND_JWT_ISSUER:-'NO DEFINIDA'}"
echo "   BACKEND_JWT_AUDIENCE: ${BACKEND_JWT_AUDIENCE:-'NO DEFINIDA'}"
echo "   BACKEND_CACHE_CONNECTION_STRING: ${BACKEND_CACHE_CONNECTION_STRING:-'NO DEFINIDA'}"

# Reemplazar tokens en el archivo de configuración
echo "🔄 Procesando $CONFIG_FILE..."

# Configuración de API
replace_token "API_BASE_URL" "$BACKEND_API_BASE_URL" "$CONFIG_FILE"
replace_token "API_PORT" "$BACKEND_API_PORT" "$CONFIG_FILE"
replace_token "API_VERSION" "$BACKEND_API_VERSION" "$CONFIG_FILE"

# Configuración de base de datos
replace_token "DATABASE_CONNECTION_STRING" "$BACKEND_DATABASE_CONNECTION_STRING" "$CONFIG_FILE"
replace_token "DATABASE_PROVIDER" "$BACKEND_DATABASE_PROVIDER" "$CONFIG_FILE"
replace_token "DATABASE_COMMAND_TIMEOUT" "$BACKEND_DATABASE_COMMAND_TIMEOUT" "$CONFIG_FILE"

# Configuración de autenticación
replace_token "JWT_SECRET" "$BACKEND_JWT_SECRET" "$CONFIG_FILE"
replace_token "JWT_ISSUER" "$BACKEND_JWT_ISSUER" "$CONFIG_FILE"
replace_token "JWT_AUDIENCE" "$BACKEND_JWT_AUDIENCE" "$CONFIG_FILE"
replace_token "JWT_EXPIRATION_MINUTES" "$BACKEND_JWT_EXPIRATION_MINUTES" "$CONFIG_FILE"

# Configuración de caché
replace_token "CACHE_PROVIDER" "$BACKEND_CACHE_PROVIDER" "$CONFIG_FILE"
replace_token "CACHE_CONNECTION_STRING" "$BACKEND_CACHE_CONNECTION_STRING" "$CONFIG_FILE"
replace_token "CACHE_EXPIRATION_MINUTES" "$BACKEND_CACHE_EXPIRATION_MINUTES" "$CONFIG_FILE"

# Configuración de seguridad
replace_token "SECURITY_RATE_LIMIT_REQUESTS" "$BACKEND_SECURITY_RATE_LIMIT_REQUESTS" "$CONFIG_FILE"
replace_token "SECURITY_RATE_LIMIT_WINDOW" "$BACKEND_SECURITY_RATE_LIMIT_WINDOW" "$CONFIG_FILE"

# Configuración de servicios externos
replace_token "PAYMENT_SERVICE_URL" "$BACKEND_PAYMENT_SERVICE_URL" "$CONFIG_FILE"
replace_token "PAYMENT_SERVICE_API_KEY" "$BACKEND_PAYMENT_SERVICE_API_KEY" "$CONFIG_FILE"
replace_token "NOTIFICATION_SERVICE_URL" "$BACKEND_NOTIFICATION_SERVICE_URL" "$CONFIG_FILE"
replace_token "NOTIFICATION_SERVICE_API_KEY" "$BACKEND_NOTIFICATION_SERVICE_API_KEY" "$CONFIG_FILE"

# Configuración de características específicas por ambiente
case $ENVIRONMENT in
    "production")
        replace_token "ENVIRONMENT_IS_PRODUCTION" "true" "$CONFIG_FILE"
        replace_token "ENVIRONMENT_DEBUG" "false" "$CONFIG_FILE"
        replace_token "API_ENABLE_SWAGGER" "false" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_DETAILED_ERRORS" "false" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "false" "$CONFIG_FILE"
        ;;
    "staging")
        replace_token "ENVIRONMENT_IS_PRODUCTION" "true" "$CONFIG_FILE"
        replace_token "ENVIRONMENT_DEBUG" "false" "$CONFIG_FILE"
        replace_token "API_ENABLE_SWAGGER" "true" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_DETAILED_ERRORS" "true" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "false" "$CONFIG_FILE"
        ;;
    "test")
        replace_token "ENVIRONMENT_IS_PRODUCTION" "false" "$CONFIG_FILE"
        replace_token "ENVIRONMENT_DEBUG" "true" "$CONFIG_FILE"
        replace_token "API_ENABLE_SWAGGER" "true" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_DETAILED_ERRORS" "true" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "true" "$CONFIG_FILE"
        ;;
    *)
        replace_token "ENVIRONMENT_IS_PRODUCTION" "false" "$CONFIG_FILE"
        replace_token "ENVIRONMENT_DEBUG" "true" "$CONFIG_FILE"
        replace_token "API_ENABLE_SWAGGER" "true" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_DETAILED_ERRORS" "true" "$CONFIG_FILE"
        replace_token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "true" "$CONFIG_FILE"
        ;;
esac

# Limpiar archivos de respaldo
cleanup_backup_files

echo "✅ Configuración actualizada exitosamente para el ambiente: $ENVIRONMENT"
echo "📄 Archivo procesado: $CONFIG_FILE"
echo ""
echo "🔍 Para verificar la configuración:"
echo "   cat $CONFIG_FILE"
echo ""
echo "🚀 El backend está listo para ejecutarse en el ambiente: $ENVIRONMENT"
