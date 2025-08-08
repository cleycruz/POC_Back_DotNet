# Configuración de variables de ambiente para PowerShell
# Se ejecuta antes del build para reemplazar tokens de configuración

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("development", "test", "staging", "production", "cleanup", "restore", "help")]
    [string]$Action = "setup"
)

# Función para reemplazar tokens en archivos
function Replace-Tokens {
    param([string]$FilePath)
    
    Write-Host "Reemplazando tokens en: $FilePath" -ForegroundColor Yellow
    
    if (-not (Test-Path $FilePath)) {
        Write-Warning "Archivo no encontrado: $FilePath"
        return
    }
    
    $content = Get-Content $FilePath -Raw
    
    # Verificar y reemplazar variables de ambiente
    if ($env:API_BASE_URL) {
        $content = $content -replace '#{API_BASE_URL}#', $env:API_BASE_URL
        Write-Host "✅ API_BASE_URL reemplazada: $($env:API_BASE_URL)" -ForegroundColor Green
    } else {
        Write-Warning "API_BASE_URL no está definida"
    }
    
    if ($env:CDN_URL) {
        $content = $content -replace '#{CDN_URL}#', $env:CDN_URL
        Write-Host "✅ CDN_URL reemplazada: $($env:CDN_URL)" -ForegroundColor Green
    } else {
        Write-Warning "CDN_URL no está definida"
    }
    
    if ($env:ANALYTICS_URL) {
        $content = $content -replace '#{ANALYTICS_URL}#', $env:ANALYTICS_URL
        Write-Host "✅ ANALYTICS_URL reemplazada: $($env:ANALYTICS_URL)" -ForegroundColor Green
    } else {
        Write-Warning "ANALYTICS_URL no está definida"
    }
    
    if ($env:ERROR_REPORTING_URL) {
        $content = $content -replace '#{ERROR_REPORTING_URL}#', $env:ERROR_REPORTING_URL
        Write-Host "✅ ERROR_REPORTING_URL reemplazada: $($env:ERROR_REPORTING_URL)" -ForegroundColor Green
    } else {
        Write-Warning "ERROR_REPORTING_URL no está definida"
    }
    
    # Reemplazos específicos para staging
    if ($env:STAGING_API_BASE_URL) {
        $content = $content -replace '#{STAGING_API_BASE_URL}#', $env:STAGING_API_BASE_URL
        Write-Host "✅ STAGING_API_BASE_URL reemplazada: $($env:STAGING_API_BASE_URL)" -ForegroundColor Green
    }
    
    if ($env:STAGING_CDN_URL) {
        $content = $content -replace '#{STAGING_CDN_URL}#', $env:STAGING_CDN_URL
        Write-Host "✅ STAGING_CDN_URL reemplazada: $($env:STAGING_CDN_URL)" -ForegroundColor Green
    }
    
    if ($env:STAGING_ANALYTICS_URL) {
        $content = $content -replace '#{STAGING_ANALYTICS_URL}#', $env:STAGING_ANALYTICS_URL
        Write-Host "✅ STAGING_ANALYTICS_URL reemplazada: $($env:STAGING_ANALYTICS_URL)" -ForegroundColor Green
    }
    
    if ($env:STAGING_ERROR_REPORTING_URL) {
        $content = $content -replace '#{STAGING_ERROR_REPORTING_URL}#', $env:STAGING_ERROR_REPORTING_URL
        Write-Host "✅ STAGING_ERROR_REPORTING_URL reemplazada: $($env:STAGING_ERROR_REPORTING_URL)" -ForegroundColor Green
    }
    
    Set-Content -Path $FilePath -Value $content -Encoding UTF8
}

# Función para mostrar información del ambiente
function Show-EnvironmentInfo {
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "🚀 Configuración de Variables de Ambiente" -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "Environment: $($env:ENVIRONMENT ?? 'development')" -ForegroundColor White
    Write-Host "API_BASE_URL: $($env:API_BASE_URL ?? 'No definida')" -ForegroundColor White
    Write-Host "CDN_URL: $($env:CDN_URL ?? 'No definida')" -ForegroundColor White
    Write-Host "ANALYTICS_URL: $($env:ANALYTICS_URL ?? 'No definida')" -ForegroundColor White
    Write-Host "ERROR_REPORTING_URL: $($env:ERROR_REPORTING_URL ?? 'No definida')" -ForegroundColor White
    Write-Host "Node Environment: $($env:NODE_ENV ?? 'No definida')" -ForegroundColor White
    Write-Host "==================================================" -ForegroundColor Cyan
}

# Función principal
function Start-EnvironmentSetup {
    Write-Host "🔧 Iniciando configuración de ambiente..." -ForegroundColor Blue
    
    Show-EnvironmentInfo
    
    # Determinar el ambiente si no está definido
    if (-not $env:ENVIRONMENT) {
        if ($env:NODE_ENV -eq "production") {
            $env:ENVIRONMENT = "production"
        } elseif ($env:NODE_ENV -eq "test") {
            $env:ENVIRONMENT = "test"
        } elseif ($env:NODE_ENV -eq "staging") {
            $env:ENVIRONMENT = "staging"
        } else {
            $env:ENVIRONMENT = "development"
        }
        Write-Host "📝 Ambiente determinado automáticamente: $($env:ENVIRONMENT)" -ForegroundColor Yellow
    }
    
    # Crear backup de los archivos originales
    Write-Host "📋 Creando backup de archivos de configuración..." -ForegroundColor Yellow
    $environmentFile = "src/environments/environment.$($env:ENVIRONMENT).ts"
    if (Test-Path $environmentFile) {
        Copy-Item $environmentFile "$environmentFile.bak" -Force
    }
    
    # Reemplazar tokens en archivos de ambiente
    switch ($env:ENVIRONMENT) {
        "production" {
            Write-Host "🏭 Configurando ambiente de PRODUCCIÓN..." -ForegroundColor Magenta
            Replace-Tokens "src/environments/environment.prod.ts"
        }
        "staging" {
            Write-Host "🎭 Configurando ambiente de STAGING..." -ForegroundColor Magenta
            Replace-Tokens "src/environments/environment.staging.ts"
        }
        "test" {
            Write-Host "🧪 Configurando ambiente de TESTING..." -ForegroundColor Magenta
            Replace-Tokens "src/environments/environment.test.ts"
        }
        default {
            Write-Host "💻 Configurando ambiente de DESARROLLO..." -ForegroundColor Magenta
            Write-Host "ℹ️  No se requieren reemplazos para desarrollo" -ForegroundColor Blue
        }
    }
    
    Write-Host "✅ Configuración de ambiente completada" -ForegroundColor Green
}

# Función para limpiar archivos temporales
function Clear-TemporaryFiles {
    Write-Host "🧹 Limpiando archivos temporales..." -ForegroundColor Yellow
    Get-ChildItem "src/environments" -Filter "*.bak" | Remove-Item -Force
    Write-Host "✅ Archivos temporales eliminados" -ForegroundColor Green
}

# Función para restaurar archivos originales
function Restore-OriginalFiles {
    Write-Host "🔄 Restaurando archivos originales..." -ForegroundColor Yellow
    Get-ChildItem "src/environments" -Filter "*.bak" | ForEach-Object {
        $original = $_.FullName -replace '\.bak$', ''
        Move-Item $_.FullName $original -Force
        Write-Host "✅ Restaurado: $original" -ForegroundColor Green
    }
}

# Función para mostrar ayuda
function Show-Help {
    Write-Host "Uso: .\setup-environment.ps1 [-Action <action>]" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Acciones disponibles:" -ForegroundColor White
    Write-Host "  setup      - Configurar variables de ambiente (por defecto)" -ForegroundColor Gray
    Write-Host "  cleanup    - Limpiar archivos temporales" -ForegroundColor Gray
    Write-Host "  restore    - Restaurar archivos originales" -ForegroundColor Gray
    Write-Host "  help       - Mostrar esta ayuda" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Variables de ambiente soportadas:" -ForegroundColor White
    Write-Host "  ENVIRONMENT           - Ambiente objetivo (development|test|staging|production)" -ForegroundColor Gray
    Write-Host "  API_BASE_URL         - URL base de la API" -ForegroundColor Gray
    Write-Host "  CDN_URL              - URL del CDN" -ForegroundColor Gray
    Write-Host "  ANALYTICS_URL        - URL del servicio de analytics" -ForegroundColor Gray
    Write-Host "  ERROR_REPORTING_URL  - URL del servicio de reporte de errores" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Ejemplo de uso:" -ForegroundColor White
    Write-Host "  `$env:ENVIRONMENT='production'" -ForegroundColor Gray
    Write-Host "  `$env:API_BASE_URL='https://api.miapp.com'" -ForegroundColor Gray
    Write-Host "  .\setup-environment.ps1" -ForegroundColor Gray
}

# Manejar argumentos de línea de comandos
switch ($Action) {
    "cleanup" {
        Clear-TemporaryFiles
    }
    "restore" {
        Restore-OriginalFiles
    }
    "help" {
        Show-Help
    }
    default {
        Start-EnvironmentSetup
    }
}
