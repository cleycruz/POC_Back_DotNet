// Tipos para la configuración de environments
export interface EnvironmentConfig {
  production: boolean;
  development: boolean;
  testing: boolean;
  
  api: ApiConfig;
  features: FeatureFlags;
  security: SecurityConfig;
  logging: LoggingConfig;
  cache: CacheConfig;
  external: ExternalServicesConfig;
  app: AppConfig;
}

export interface ApiConfig {
  baseUrl: string;
  timeout: number;
  retryAttempts: number;
  version: string;
}

export interface FeatureFlags {
  enableDebugMode: boolean;
  enableMockData: boolean;
  enableAnalytics: boolean;
  enablePerformanceMonitoring: boolean;
  enableDetailedLogging: boolean;
}

export interface SecurityConfig {
  enableHttpsRedirect: boolean;
  jwtTokenExpiration: number;
  refreshTokenExpiration: number;
  sessionTimeout: number;
  enableCsrfProtection: boolean;
}

export interface LoggingConfig {
  level: 'debug' | 'info' | 'warn' | 'error';
  enableConsoleLogging: boolean;
  enableRemoteLogging: boolean;
  logRetentionDays: number;
}

export interface CacheConfig {
  defaultTtl: number;
  maxCacheSize: number;
  enableLocalStorage: boolean;
  enableSessionStorage: boolean;
}

export interface ExternalServicesConfig {
  analyticsUrl: string;
  errorReportingUrl: string;
  cdnUrl: string;
}

export interface AppConfig {
  name: string;
  version: string;
  defaultLanguage: string;
  supportedLanguages: string[];
  itemsPerPage: number;
  maxCartItems: number;
}

// Enum para los tipos de ambiente
export enum EnvironmentType {
  DEVELOPMENT = 'development',
  TESTING = 'testing',
  STAGING = 'staging',
  PRODUCTION = 'production'
}

// Configuración específica por ambiente
export interface EnvironmentMetadata {
  type: EnvironmentType;
  displayName: string;
  description: string;
  debugMode: boolean;
  hotReload: boolean;
}
