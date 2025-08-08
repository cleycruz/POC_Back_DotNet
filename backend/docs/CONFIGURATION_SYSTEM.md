# Sistema de Configuración del Backend - Carrito de Compras API

## 📋 Descripción

Este sistema proporciona una gestión centralizada y tipada de la configuración para la API del carrito de compras, incluyendo soporte para múltiples ambientes, validación automática y reemplazo de tokens para deployments.

## 🏗️ Arquitectura del Sistema

### Componentes Principales

1. **AppSettings.cs** - Clases de configuración fuertemente tipadas
2. **ConfigurationService.cs** - Servicio para gestión centralizada de configuración
3. **ConfigurationController.cs** - Endpoints para información de configuración
4. **appsettings.{Environment}.json** - Archivos de configuración por ambiente
5. **Scripts de Deployment** - Para reemplazo automático de tokens

### Flujo de Configuración

```
Archivos JSON → Binding → AppSettings → ConfigurationService → Aplicación
                    ↓
               Validación Automática
```

## 🔧 Configuración por Ambientes

### Ambientes Soportados

- **Development** (`appsettings.Development.json`)
- **Test** (`appsettings.Test.json`)
- **Staging** (`appsettings.Staging.json`)
- **Production** (`appsettings.json`)

### Estructura de Configuración

```json
{
  "Environment": {
    "Name": "development",
    "IsProduction": false,
    "Debug": true
  },
  "Api": {
    "Version": "v1",
    "BaseUrl": "https://localhost:7005/api",
    "Port": "7005",
    "EnableSwagger": true,
    "EnableCors": true,
    "AllowedOrigins": []
  },
  "Database": {
    "ConnectionString": "...",
    "Provider": "SqlServer",
    "CommandTimeout": 30,
    "MaxRetryCount": 3
  },
  "Authentication": {
    "JwtSecret": "...",
    "JwtIssuer": "...",
    "JwtAudience": "...",
    "JwtExpirationMinutes": 120
  },
  "Cache": {
    "Provider": "InMemory",
    "DefaultExpirationMinutes": 30,
    "EnableDistributedCache": false
  },
  "Security": {
    "EnableRateLimiting": true,
    "RateLimitRequests": 100,
    "RateLimitWindow": "00:01:00"
  },
  "Features": {
    "EnableMetrics": true,
    "EnableHealthChecks": true,
    "EnableDetailedErrors": false,
    "EnableSensitiveDataLogging": false
  },
  "External": {
    "PaymentService": {
      "BaseUrl": "...",
      "ApiKey": "...",
      "TimeoutSeconds": 30
    },
    "NotificationService": {
      "BaseUrl": "...",
      "ApiKey": "...",
      "TimeoutSeconds": 15
    }
  }
}
```

## 🚀 Uso del ConfigurationService

### Inyección de Dependencias

```csharp
public class ProductoController : ControllerBase
{
    private readonly IConfigurationService _config;

    public ProductoController(IConfigurationService config)
    {
        _config = config;
    }

    public async Task<IActionResult> Get()
    {
        var apiUrl = _config.GetApiUrl("productos");
        var isProduction = _config.Environment.IsProduction;
        
        // ... lógica del controlador
    }
}
```

### Métodos Disponibles

```csharp
// Propiedades de configuración
_config.Api.BaseUrl
_config.Database.ConnectionString
_config.Authentication.JwtSecret
_config.Environment.IsProduction

// Métodos útiles
_config.GetApiUrl(endpoint)
_config.GetExternalServiceUrl(serviceName, endpoint)
_config.ValidateConfiguration()
_config.GetEnvironmentInfo()
```

## 📊 Endpoints de Configuración

### GET /api/configuration/environment
Obtiene información del ambiente actual.

```json
{
  "name": "development",
  "isProduction": false,
  "debug": true,
  "apiVersion": "v1",
  "apiBaseUrl": "https://localhost:7005/api",
  "databaseProvider": "SqlServer",
  "cacheProvider": "InMemory",
  "swaggerEnabled": true,
  "validationErrors": []
}
```

### GET /api/configuration/api-url
Obtiene la URL base de la API.

```json
{
  "apiUrl": "https://localhost:7005/api"
}
```

### GET /api/configuration/validate
Valida la configuración actual.

```json
{
  "isValid": true,
  "errors": [],
  "validationTime": "2024-01-15T10:30:00Z"
}
```

### GET /api/configuration/dev-info
Información detallada de configuración (solo en ambientes no productivos).

## 🔄 Sistema de Deployment

### Variables de Entorno

Todas las variables siguen el patrón `BACKEND_SECCION_PROPIEDAD`:

```bash
# API
BACKEND_API_BASE_URL=https://api.example.com
BACKEND_API_PORT=443
BACKEND_API_VERSION=v1

# Base de Datos
BACKEND_DATABASE_CONNECTION_STRING="Server=...;Database=...;"
BACKEND_DATABASE_PROVIDER=SqlServer

# Autenticación
BACKEND_JWT_SECRET=your-production-secret
BACKEND_JWT_ISSUER=CarritoComprasAPI
BACKEND_JWT_AUDIENCE=CarritoComprasApp

# Servicios Externos
BACKEND_PAYMENT_SERVICE_URL=https://api.payment.com
BACKEND_PAYMENT_SERVICE_API_KEY=prod-key
```

### Scripts de Deployment

#### Linux/macOS
```bash
# Reemplazar tokens para ambiente de producción
./scripts/replace-backend-tokens.sh production

# Reemplazar tokens para desarrollo
./scripts/replace-backend-tokens.sh development
```

#### Windows
```powershell
# Reemplazar tokens para ambiente de producción
.\scripts\replace-backend-tokens.ps1 -Environment production

# Reemplazar tokens para desarrollo
.\scripts\replace-backend-tokens.ps1 -Environment development
```

### Configuración por Ambiente

Los scripts automáticamente configuran valores específicos por ambiente:

**Development:**
- IsProduction: false
- Debug: true
- EnableSwagger: true
- EnableDetailedErrors: true
- EnableSensitiveDataLogging: true

**Production:**
- IsProduction: true
- Debug: false
- EnableSwagger: false
- EnableDetailedErrors: false
- EnableSensitiveDataLogging: false

## ✅ Validaciones Automáticas

El sistema incluye validaciones automáticas para:

### Configuración de API
- BaseUrl requerida
- Port válido
- Version requerida

### Configuración de Base de Datos
- ConnectionString requerida
- Provider requerido
- CommandTimeout > 0
- MaxRetryCount >= 0

### Configuración de Autenticación
- JwtSecret requerido (mínimo 32 caracteres)
- JwtIssuer y JwtAudience requeridos
- JwtExpirationMinutes > 0

### Validaciones de Seguridad
- Secreto de desarrollo no usado en producción
- Logging sensible deshabilitado en producción
- Debug deshabilitado en producción

## 🛠️ Desarrollo Local

### 1. Configurar Variables de Entorno

```bash
# Copiar archivo de ejemplo
cp backend/.env.example backend/.env

# Editar variables según necesidades
vi backend/.env
```

### 2. Ejecutar Aplicación

```bash
cd backend/src/CarritoComprasAPI
dotnet run --environment Development
```

### 3. Verificar Configuración

```bash
# Verificar configuración via API
curl https://localhost:7005/api/configuration/environment

# Validar configuración
curl https://localhost:7005/api/configuration/validate
```

## 🔍 Troubleshooting

### Error: "Configuration validation failed"

1. Verificar que todas las variables de entorno estén definidas
2. Ejecutar endpoint de validación: `/api/configuration/validate`
3. Revisar logs de la aplicación

### Error: "Connection string not found"

1. Verificar variable `BACKEND_DATABASE_CONNECTION_STRING`
2. Verificar formato de connection string
3. Verificar que el servidor de base de datos esté disponible

### Error: "JWT secret too short"

1. Verificar que `BACKEND_JWT_SECRET` tenga al menos 32 caracteres
2. Generar nuevo secreto seguro para producción

## 📚 Recursos Adicionales

- [Documentación de Configuration en .NET](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Validation en .NET](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [Options Pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)

## 🔗 Integración con Frontend

El sistema de configuración del backend está diseñado para integrarse perfectamente con el ConfigService del frontend Angular, proporcionando una experiencia consistente de configuración en toda la aplicación.
