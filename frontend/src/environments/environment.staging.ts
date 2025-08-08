// Environment para staging
import { EnvironmentConfig } from './environment.types';

export const environment: EnvironmentConfig = {
  production: false,
  development: false,
  testing: false,
  
  // API Configuration
  api: {
    baseUrl: '#{STAGING_API_BASE_URL}#', // Will be replaced during deployment
    timeout: 30000,
    retryAttempts: 3,
    version: 'v1'
  },
  
  // Feature Flags
  features: {
    enableDebugMode: true,
    enableMockData: false,
    enableAnalytics: true,
    enablePerformanceMonitoring: true,
    enableDetailedLogging: true
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
    level: 'info' as const,
    enableConsoleLogging: true,
    enableRemoteLogging: true,
    logRetentionDays: 14
  },
  
  // Cache Configuration
  cache: {
    defaultTtl: 300000, // 5 minutos en ms
    maxCacheSize: 150,
    enableLocalStorage: true,
    enableSessionStorage: true
  },
  
  // External Services (URLs públicas solamente)
  external: {
    analyticsUrl: '#{STAGING_ANALYTICS_URL}#',
    errorReportingUrl: '#{STAGING_ERROR_REPORTING_URL}#',
    cdnUrl: '#{STAGING_CDN_URL}#'
  },
  
  // Application Configuration
  app: {
    name: 'Carrito de Compras - STAGING',
    version: '1.0.0-staging',
    defaultLanguage: 'es',
    supportedLanguages: ['es', 'en'],
    itemsPerPage: 15,
    maxCartItems: 75
  }
};
