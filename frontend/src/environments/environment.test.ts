// Environment para testing
import { EnvironmentConfig } from './environment.types';

export const environment: EnvironmentConfig = {
  production: false,
  development: false,
  testing: true,
  
  // API Configuration
  api: {
    baseUrl: 'http://localhost:5000/api', // Test server
    timeout: 10000,
    retryAttempts: 1,
    version: 'v1'
  },
  
  // Feature Flags
  features: {
    enableDebugMode: true,
    enableMockData: true,
    enableAnalytics: false,
    enablePerformanceMonitoring: false,
    enableDetailedLogging: true
  },
  
  // Security Configuration
  security: {
    enableHttpsRedirect: false,
    jwtTokenExpiration: 300000, // 5 minutos en ms (para testing)
    refreshTokenExpiration: 600000, // 10 minutos en ms (para testing)
    sessionTimeout: 600000, // 10 minutos en ms (para testing)
    enableCsrfProtection: false // Deshabilitado para pruebas
  },
  
  // Logging Configuration
  logging: {
    level: 'debug' as const,
    enableConsoleLogging: true,
    enableRemoteLogging: false,
    logRetentionDays: 1
  },
  
  // Cache Configuration
  cache: {
    defaultTtl: 60000, // 1 minuto en ms
    maxCacheSize: 50,
    enableLocalStorage: false, // Evitar interferencia entre tests
    enableSessionStorage: false
  },
  
  // External Services (URLs públicas solamente)
  external: {
    analyticsUrl: '',
    errorReportingUrl: '',
    cdnUrl: 'http://localhost:4200/assets'
  },
  
  // Application Configuration
  app: {
    name: 'Carrito de Compras - TEST',
    version: '1.0.0-test',
    defaultLanguage: 'es',
    supportedLanguages: ['es', 'en'],
    itemsPerPage: 5,
    maxCartItems: 10
  }
};
