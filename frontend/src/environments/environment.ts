// Environment para desarrollo
import { EnvironmentConfig } from './environment.types';

export const environment: EnvironmentConfig = {
  production: false,
  development: true,
  testing: false,
  
  // API Configuration
  api: {
    baseUrl: 'http://localhost:5063/', // Backend .NET HTTPS port
    // Alternative: 'https://localhost:7005/api' (HTTP port if HTTPS fails)
    timeout: 30000,
    retryAttempts: 3,
    version: 'v1'
  },
  
  // Feature Flags
  features: {
    enableDebugMode: true,
    enableMockData: false,
    enableAnalytics: false,
    enablePerformanceMonitoring: true,
    enableDetailedLogging: true
  },
  
  // Security Configuration
  security: {
    enableHttpsRedirect: false,
    jwtTokenExpiration: 3600000, // 1 hora en ms
    refreshTokenExpiration: 604800000, // 7 días en ms
    sessionTimeout: 1800000, // 30 minutos en ms
    enableCsrfProtection: true
  },
  
  // Logging Configuration
  logging: {
    level: 'debug' as const,
    enableConsoleLogging: true,
    enableRemoteLogging: false,
    logRetentionDays: 7
  },
  
  // Cache Configuration
  cache: {
    defaultTtl: 300000, // 5 minutos en ms
    maxCacheSize: 100,
    enableLocalStorage: true,
    enableSessionStorage: true
  },
  
  // External Services (URLs públicas solamente)
  external: {
    analyticsUrl: '',
    errorReportingUrl: '',
    cdnUrl: 'http://localhost:4200/assets'
  },
  
  // Application Configuration
  app: {
    name: 'Carrito de Compras - DEV',
    version: '1.0.0-dev',
    defaultLanguage: 'es',
    supportedLanguages: ['es', 'en'],
    itemsPerPage: 10,
    maxCartItems: 50
  }
};
