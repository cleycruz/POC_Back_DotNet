# Script para reemplazar tokens en archivos de configuración del backend (Windows)
# Uso: .\replace-backend-tokens.ps1 -Environment <environment> [-BasePath <path>]

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "development",
    
    [Parameter(Mandatory=$false)]
    [string]$BasePath = ".\"
)

$ErrorActionPreference = "Stop"

$ConfigFile = Join-Path $BasePath "appsettings.$Environment.json"

Write-Host "🔧 Reemplazando tokens de configuración para ambiente: $Environment" -ForegroundColor Green
Write-Host "📁 Archivo de configuración: $ConfigFile" -ForegroundColor Yellow

# Verificar que el archivo de configuración existe
if (-not (Test-Path $ConfigFile)) {
    Write-Host "❌ Error: No se encontró el archivo de configuración $ConfigFile" -ForegroundColor Red
    exit 1
}

# Función para reemplazar tokens
function Replace-Token {
    param(
        [string]$Token,
        [string]$Value,
        [string]$File
    )
    
    if ($Value) {
        Write-Host "🔄 Reemplazando $Token -> $Value" -ForegroundColor Cyan
        $content = Get-Content $File -Raw
        $content = $content -replace "\{\{$Token\}\}", $Value
        Set-Content -Path $File -Value $content -NoNewline
    } else {
        Write-Host "⚠️  Warning: Variable de entorno $Token no definida" -ForegroundColor Yellow
    }
}

Write-Host "📋 Variables de entorno disponibles:" -ForegroundColor Magenta
Write-Host "   BACKEND_API_BASE_URL: $($env:BACKEND_API_BASE_URL ?? 'NO DEFINIDA')"
Write-Host "   BACKEND_API_PORT: $($env:BACKEND_API_PORT ?? 'NO DEFINIDA')"
Write-Host "   BACKEND_DATABASE_CONNECTION_STRING: $($env:BACKEND_DATABASE_CONNECTION_STRING ?? 'NO DEFINIDA')"
Write-Host "   BACKEND_JWT_SECRET: $($env:BACKEND_JWT_SECRET ?? 'NO DEFINIDA')"
Write-Host "   BACKEND_JWT_ISSUER: $($env:BACKEND_JWT_ISSUER ?? 'NO DEFINIDA')"
Write-Host "   BACKEND_JWT_AUDIENCE: $($env:BACKEND_JWT_AUDIENCE ?? 'NO DEFINIDA')"

# Reemplazar tokens en el archivo de configuración
Write-Host "🔄 Procesando $ConfigFile..." -ForegroundColor Blue

# Configuración de API
Replace-Token "API_BASE_URL" $env:BACKEND_API_BASE_URL $ConfigFile
Replace-Token "API_PORT" $env:BACKEND_API_PORT $ConfigFile
Replace-Token "API_VERSION" $env:BACKEND_API_VERSION $ConfigFile

# Configuración de base de datos
Replace-Token "DATABASE_CONNECTION_STRING" $env:BACKEND_DATABASE_CONNECTION_STRING $ConfigFile
Replace-Token "DATABASE_PROVIDER" $env:BACKEND_DATABASE_PROVIDER $ConfigFile
Replace-Token "DATABASE_COMMAND_TIMEOUT" $env:BACKEND_DATABASE_COMMAND_TIMEOUT $ConfigFile

# Configuración de autenticación
Replace-Token "JWT_SECRET" $env:BACKEND_JWT_SECRET $ConfigFile
Replace-Token "JWT_ISSUER" $env:BACKEND_JWT_ISSUER $ConfigFile
Replace-Token "JWT_AUDIENCE" $env:BACKEND_JWT_AUDIENCE $ConfigFile
Replace-Token "JWT_EXPIRATION_MINUTES" $env:BACKEND_JWT_EXPIRATION_MINUTES $ConfigFile

# Configuración de caché
Replace-Token "CACHE_PROVIDER" $env:BACKEND_CACHE_PROVIDER $ConfigFile
Replace-Token "CACHE_CONNECTION_STRING" $env:BACKEND_CACHE_CONNECTION_STRING $ConfigFile
Replace-Token "CACHE_EXPIRATION_MINUTES" $env:BACKEND_CACHE_EXPIRATION_MINUTES $ConfigFile

# Configuración de seguridad
Replace-Token "SECURITY_RATE_LIMIT_REQUESTS" $env:BACKEND_SECURITY_RATE_LIMIT_REQUESTS $ConfigFile
Replace-Token "SECURITY_RATE_LIMIT_WINDOW" $env:BACKEND_SECURITY_RATE_LIMIT_WINDOW $ConfigFile

# Configuración de servicios externos
Replace-Token "PAYMENT_SERVICE_URL" $env:BACKEND_PAYMENT_SERVICE_URL $ConfigFile
Replace-Token "PAYMENT_SERVICE_API_KEY" $env:BACKEND_PAYMENT_SERVICE_API_KEY $ConfigFile
Replace-Token "NOTIFICATION_SERVICE_URL" $env:BACKEND_NOTIFICATION_SERVICE_URL $ConfigFile
Replace-Token "NOTIFICATION_SERVICE_API_KEY" $env:BACKEND_NOTIFICATION_SERVICE_API_KEY $ConfigFile

# Configuración de características específicas por ambiente
switch ($Environment) {
    "production" {
        Replace-Token "ENVIRONMENT_IS_PRODUCTION" "true" $ConfigFile
        Replace-Token "ENVIRONMENT_DEBUG" "false" $ConfigFile
        Replace-Token "API_ENABLE_SWAGGER" "false" $ConfigFile
        Replace-Token "FEATURES_ENABLE_DETAILED_ERRORS" "false" $ConfigFile
        Replace-Token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "false" $ConfigFile
    }
    "staging" {
        Replace-Token "ENVIRONMENT_IS_PRODUCTION" "true" $ConfigFile
        Replace-Token "ENVIRONMENT_DEBUG" "false" $ConfigFile
        Replace-Token "API_ENABLE_SWAGGER" "true" $ConfigFile
        Replace-Token "FEATURES_ENABLE_DETAILED_ERRORS" "true" $ConfigFile
        Replace-Token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "false" $ConfigFile
    }
    "test" {
        Replace-Token "ENVIRONMENT_IS_PRODUCTION" "false" $ConfigFile
        Replace-Token "ENVIRONMENT_DEBUG" "true" $ConfigFile
        Replace-Token "API_ENABLE_SWAGGER" "true" $ConfigFile
        Replace-Token "FEATURES_ENABLE_DETAILED_ERRORS" "true" $ConfigFile
        Replace-Token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "true" $ConfigFile
    }
    default {
        Replace-Token "ENVIRONMENT_IS_PRODUCTION" "false" $ConfigFile
        Replace-Token "ENVIRONMENT_DEBUG" "true" $ConfigFile
        Replace-Token "API_ENABLE_SWAGGER" "true" $ConfigFile
        Replace-Token "FEATURES_ENABLE_DETAILED_ERRORS" "true" $ConfigFile
        Replace-Token "FEATURES_ENABLE_SENSITIVE_DATA_LOGGING" "true" $ConfigFile
    }
}

Write-Host "✅ Configuración actualizada exitosamente para el ambiente: $Environment" -ForegroundColor Green
Write-Host "📄 Archivo procesado: $ConfigFile" -ForegroundColor Yellow
Write-Host ""
Write-Host "🔍 Para verificar la configuración:" -ForegroundColor Cyan
Write-Host "   Get-Content $ConfigFile" -ForegroundColor White
Write-Host ""
Write-Host "🚀 El backend está listo para ejecutarse en el ambiente: $Environment" -ForegroundColor Green
