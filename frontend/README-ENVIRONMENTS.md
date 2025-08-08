# 🔧 Configuración de Variables de Ambiente - Angular

Este documento explica cómo gestionar variables de ambiente en la aplicación Angular del carrito de compras.

## 📋 Índice

- [Estructura de Archivos](#estructura-de-archivos)
- [Configuración por Ambiente](#configuración-por-ambiente)
- [Variables Disponibles](#variables-disponibles)
- [Uso en la Aplicación](#uso-en-la-aplicación)
- [Scripts de Deployment](#scripts-de-deployment)
- [Mejores Prácticas](#mejores-prácticas)

## 📁 Estructura de Archivos

```
frontend/
├── src/
│   ├── environments/
│   │   ├── environment.ts              # Desarrollo (default)
│   │   ├── environment.prod.ts         # Producción
│   │   ├── environment.staging.ts      # Staging
│   │   ├── environment.test.ts         # Testing
│   │   └── environment.types.ts        # Tipos TypeScript
│   └── app/
│       └── core/
│           ├── services/
│           │   └── config.service.ts   # Servicio centralizado
│           ├── interceptors/
│           │   └── config.interceptor.ts # Interceptor HTTP
│           └── guards/
│               └── config.guard.ts     # Guard de validación
├── scripts/
│   ├── setup-environment.sh           # Script Bash
│   └── setup-environment.ps1          # Script PowerShell
├── .env.example                       # Ejemplo de variables
├── .env.test                          # Variables para testing
├── .env.staging                       # Variables para staging
└── .env.production                    # Variables para producción
```

## 🌍 Configuración por Ambiente

### Development (Desarrollo)
- **Archivo**: `environment.ts`
- **Puerto**: 4200
- **Debug**: Habilitado
- **API**: `https://localhost:7001/api`
- **Logging**: Detallado en consola

### Test (Pruebas)
- **Archivo**: `environment.test.ts`
- **Puerto**: 4201
- **Debug**: Habilitado
- **API**: `http://localhost:5000/api`
- **Mock Data**: Habilitado

### Staging (Pre-producción)
- **Archivo**: `environment.staging.ts`
- **Puerto**: 4202
- **Debug**: Habilitado (limitado)
- **API**: Variable `#{STAGING_API_BASE_URL}#`
- **Analytics**: Habilitado

### Production (Producción)
- **Archivo**: `environment.prod.ts`
- **Puerto**: 4203
- **Debug**: Deshabilitado
- **API**: Variable `#{API_BASE_URL}#`
- **Optimizaciones**: Todas habilitadas

## 🔧 Variables Disponibles

### API Configuration
```typescript
api: {
  baseUrl: string;        // URL base de la API
  timeout: number;        // Timeout en milisegundos
  retryAttempts: number;  // Número de reintentos
  version: string;        // Versión de la API
}
```

### Feature Flags
```typescript
features: {
  enableDebugMode: boolean;           // Modo debug
  enableMockData: boolean;            // Datos simulados
  enableAnalytics: boolean;           // Analytics
  enablePerformanceMonitoring: boolean; // Monitoreo
  enableDetailedLogging: boolean;     // Logging detallado
}
```

### Security Configuration
```typescript
security: {
  enableHttpsRedirect: boolean;       // Redirección HTTPS
  jwtTokenExpiration: number;         // Expiración JWT (ms)
  refreshTokenExpiration: number;     // Expiración refresh (ms)
  sessionTimeout: number;             // Timeout sesión (ms)
  enableCsrfProtection: boolean;      // Protección CSRF
}
```

### Logging Configuration
```typescript
logging: {
  level: 'debug' | 'info' | 'warn' | 'error'; // Nivel de log
  enableConsoleLogging: boolean;      // Log en consola
  enableRemoteLogging: boolean;       // Log remoto
  logRetentionDays: number;          // Días de retención
}
```

### External Services
```typescript
external: {
  analyticsUrl: string;      // URL de analytics
  errorReportingUrl: string; // URL de reportes de error
  cdnUrl: string;           // URL del CDN
}
```

## 🚀 Uso en la Aplicación

### 1. Inyectar ConfigService

```typescript
import { Component } from '@angular/core';
import { ConfigService } from '../core/services/config.service';

@Component({
  selector: 'app-example',
  template: `<div>App: {{ appName }}</div>`
})
export class ExampleComponent {
  appName: string;

  constructor(private configService: ConfigService) {
    this.appName = this.configService.app.name;
  }
}
```

### 2. Usar en Servicios HTTP

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../core/services/config.service';

@Injectable()
export class ApiService {
  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getData() {
    const url = this.configService.getApiUrl('data');
    return this.http.get(url);
  }
}
```

### 3. Verificar Feature Flags

```typescript
if (this.configService.isFeatureEnabled('enableAnalytics')) {
  // Enviar eventos de analytics
}
```

### 4. Obtener URLs de API

```typescript
// URL completa con versión: /api/v1/productos
const url = this.configService.getApiUrl('productos');

// URL sin versión: /productos
const directUrl = this.configService.getFullApiUrl('productos');
```

## 📜 Scripts de Deployment

### Bash (Linux/macOS)
```bash
# Configurar variables de ambiente
export ENVIRONMENT=production
export API_BASE_URL=https://api.miapp.com
export CDN_URL=https://cdn.miapp.com

# Ejecutar script de configuración
./scripts/setup-environment.sh

# Construir aplicación
npm run build:prod
```

### PowerShell (Windows)
```powershell
# Configurar variables de ambiente
$env:ENVIRONMENT='production'
$env:API_BASE_URL='https://api.miapp.com'
$env:CDN_URL='https://cdn.miapp.com'

# Ejecutar script de configuración
.\scripts\setup-environment.ps1

# Construir aplicación
npm run build:prod
```

### Comandos NPM Disponibles

```bash
# Desarrollo
npm run start:dev     # Puerto 4200
npm run build:dev

# Testing
npm run start:test    # Puerto 4201
npm run build:test

# Staging
npm run start:staging # Puerto 4202
npm run build:staging

# Producción
npm run start:prod    # Puerto 4203
npm run build:prod

# Análisis de bundle
npm run analyze
```

## 🛡️ Seguridad y Mejores Prácticas

### ✅ Lo que SÍ incluir en environment files:
- URLs públicas (APIs, CDNs)
- Feature flags
- Configuraciones de timeout
- Configuraciones de logging
- Configuraciones de cache

### ❌ Lo que NO incluir en environment files:
- API keys o tokens
- Contraseñas
- Secretos de aplicación
- Información sensible de usuarios

### 🔐 Para Información Sensible:
1. **Variables de ambiente del sistema**
2. **Azure Key Vault** (recomendado)
3. **Variables de ambiente de CI/CD**
4. **Configuración server-side**

### 📋 Validación de Configuración

El `ConfigGuard` valida automáticamente:
- Configuración completa al iniciar
- URLs de API válidas
- Configuraciones específicas por ambiente
- Advertencias de seguridad

## 🔄 Flujo de CI/CD

### 1. Build Time
```bash
# 1. Configurar variables de ambiente
export API_BASE_URL="https://api.production.com"

# 2. Ejecutar script de reemplazo
./scripts/setup-environment.sh

# 3. Construir aplicación
npm run build:prod

# 4. Deploy artifacts
```

### 2. Reemplazo de Tokens

Los archivos de environment pueden contener tokens:
```typescript
api: {
  baseUrl: '#{API_BASE_URL}#'  // Será reemplazado
}
```

Que se reemplazan durante el deployment:
```bash
sed -i "s|#{API_BASE_URL}#|${API_BASE_URL}|g" environment.prod.ts
```

## 🐛 Debugging

### Verificar Configuración Actual
```typescript
// Solo en modo debug
console.table(this.configService.getFullConfig());
```

### Ver Información del Ambiente
```typescript
console.log('Ambiente:', this.configService.getEnvironmentDisplayName());
console.log('Tipo:', this.configService.getEnvironmentType());
console.log('API URL:', this.configService.api.baseUrl);
```

### Logs de Configuración
La aplicación muestra automáticamente información de configuración en la consola del navegador durante el inicio (solo en desarrollo).

## 📞 Soporte

Si tienes problemas con la configuración:

1. **Verificar environment file correcto**
2. **Validar sintaxis TypeScript**
3. **Revisar variables de ambiente del sistema**
4. **Consultar logs de la aplicación**
5. **Usar el ConfigGuard para validación automática**

---

**Nota**: Esta configuración sigue las mejores prácticas de Angular y está optimizada para diferentes ambientes de deployment.
