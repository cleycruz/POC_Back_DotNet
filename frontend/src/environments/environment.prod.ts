// Environment para producción
import { EnvironmentConfig } from './environment.types';

export const environment: EnvironmentConfig = {
  production: true,
  development: false,
  testing: false,
  
  // API Configuration
  api: {
    baseUrl: '#{API_BASE_URL}#', // Will be replaced during deployment
    timeout: 30000,
    retryAttempts: 3,
    version: 'v1'
  },
  
  // Feature Flags
  features: {
    enableDebugMode: false,
    enableMockData: false,
    enableAnalytics: true,
    enablePerformanceMonitoring: true,
    enableDetailedLogging: false
  },
  
  // Security Configuration
  security: {
    enableHttpsRedirect: true,
    jwtTokenExpiration: 3600000, // 1 hora en ms
    refreshTokenExpiration: 604800000, // 7 días en ms
    sessionTimeout: 1800000, // 30 minutos en ms
    enableCsrfProtection: true
  },
  
  // Logging Configuration
  logging: {
    level: 'error' as const,
    enableConsoleLogging: false,
    enableRemoteLogging: true,
    logRetentionDays: 30
  },
  
  // Cache Configuration
  cache: {
    defaultTtl: 600000, // 10 minutos en ms
    maxCacheSize: 200,
    enableLocalStorage: true,
    enableSessionStorage: true
  },
  
  // External Services (URLs públicas solamente)
  external: {
    analyticsUrl: '#{ANALYTICS_URL}#',
    errorReportingUrl: '#{ERROR_REPORTING_URL}#',
    cdnUrl: '#{CDN_URL}#'
  },
  
  // Application Configuration
  app: {
    name: 'Carrito de Compras',
    version: '1.0.0',
    defaultLanguage: 'es',
    supportedLanguages: ['es', 'en'],
    itemsPerPage: 20,
    maxCartItems: 100
  }
};
