import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { 
  EnvironmentConfig, 
  EnvironmentType, 
  ApiConfig, 
  FeatureFlags, 
  SecurityConfig,
  LoggingConfig,
  CacheConfig,
  AppConfig
} from '../../../environments/environment.types';

/**
 * Servicio centralizado para gestionar la configuración de la aplicación
 * Proporciona acceso type-safe a todas las variables de ambiente
 */
@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private readonly config: EnvironmentConfig = environment;

  constructor() {
    this.validateConfiguration();
    this.logEnvironmentInfo();
  }

  // Getters para acceso fácil a las configuraciones
  get isProduction(): boolean {
    return this.config.production;
  }

  get isDevelopment(): boolean {
    return this.config.development;
  }

  get isTesting(): boolean {
    return this.config.testing;
  }

  get api(): ApiConfig {
    return this.config.api;
  }

  get features(): FeatureFlags {
    return this.config.features;
  }

  get security(): SecurityConfig {
    return this.config.security;
  }

  get logging(): LoggingConfig {
    return this.config.logging;
  }

  get cache(): CacheConfig {
    return this.config.cache;
  }

  get app(): AppConfig {
    return this.config.app;
  }

  // Métodos utilitarios
  getApiUrl(endpoint: string): string {
    const baseUrl = this.config.api.baseUrl.replace(/\/$/, '');
    const cleanEndpoint = endpoint.replace(/^\//, '');
    return `${baseUrl}/${this.config.api.version}/${cleanEndpoint}`;
  }

  getFullApiUrl(endpoint: string): string {
    const baseUrl = this.config.api.baseUrl.replace(/\/$/, '');
    const cleanEndpoint = endpoint.replace(/^\//, '');
    return `${baseUrl}/${cleanEndpoint}`;
  }

  isFeatureEnabled(feature: keyof FeatureFlags): boolean {
    return this.config.features[feature];
  }

  getEnvironmentType(): EnvironmentType {
    if (this.config.production) return EnvironmentType.PRODUCTION;
    if (this.config.testing) return EnvironmentType.TESTING;
    if (this.config.development) return EnvironmentType.DEVELOPMENT;
    return EnvironmentType.STAGING;
  }

  getEnvironmentDisplayName(): string {
    const type = this.getEnvironmentType();
    const names = {
      [EnvironmentType.DEVELOPMENT]: 'Desarrollo',
      [EnvironmentType.TESTING]: 'Testing',
      [EnvironmentType.STAGING]: 'Staging', 
      [EnvironmentType.PRODUCTION]: 'Producción'
    };
    return names[type];
  }

  // Validación de configuración
  private validateConfiguration(): void {
    const errors: string[] = [];

    // Validar API configuration
    if (!this.config.api.baseUrl) {
      errors.push('API baseUrl is required');
    }

    if (this.config.api.timeout <= 0) {
      errors.push('API timeout must be positive');
    }

    // Validar configuración de seguridad
    if (this.config.security.jwtTokenExpiration <= 0) {
      errors.push('JWT token expiration must be positive');
    }

    // Validar configuración de cache
    if (this.config.cache.defaultTtl <= 0) {
      errors.push('Cache TTL must be positive');
    }

    if (errors.length > 0) {
      console.error('Configuration validation errors:', errors);
      throw new Error(`Invalid configuration: ${errors.join(', ')}`);
    }
  }

  // Log información del ambiente
  private logEnvironmentInfo(): void {
    if (this.config.logging.enableConsoleLogging) {
      const info = {
        environment: this.getEnvironmentDisplayName(),
        version: this.config.app.version,
        apiBaseUrl: this.config.api.baseUrl,
        debugMode: this.config.features.enableDebugMode,
        timestamp: new Date().toISOString()
      };

      console.group('🚀 Application Configuration');
      console.table(info);
      console.groupEnd();
    }
  }

  // Método para obtener configuración completa (útil para debugging)
  getFullConfig(): EnvironmentConfig {
    if (!this.config.features.enableDebugMode) {
      throw new Error('Full config access is only available in debug mode');
    }
    return { ...this.config };
  }

  // Método para verificar si estamos en un ambiente específico
  isEnvironment(type: EnvironmentType): boolean {
    return this.getEnvironmentType() === type;
  }

  // Métodos para configuraciones específicas de dominio
  getMaxCartItems(): number {
    return this.config.app.maxCartItems;
  }

  getItemsPerPage(): number {
    return this.config.app.itemsPerPage;
  }

  getSessionTimeout(): number {
    return this.config.security.sessionTimeout;
  }

  getCacheTtl(): number {
    return this.config.cache.defaultTtl;
  }

  // Método para reemplazar tokens de configuración (útil para CI/CD)
  static replaceConfigTokens(config: any, replacements: Record<string, string>): any {
    const configStr = JSON.stringify(config);
    let replacedStr = configStr;
    
    Object.entries(replacements).forEach(([token, value]) => {
      const tokenPattern = new RegExp(`#{${token}}#`, 'g');
      replacedStr = replacedStr.replace(tokenPattern, value);
    });
    
    return JSON.parse(replacedStr);
  }
}
