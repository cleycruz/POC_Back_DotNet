import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { ConfigService } from '../services/config.service';

/**
 * Guard funcional que verifica la configuración antes de permitir el acceso a rutas
 */
export const configGuard: CanActivateFn = (): boolean => {
  const configService = inject(ConfigService);
  const router = inject(Router);

  try {
    // Verificar que la configuración está válida
    validateConfiguration(configService);
    return true;
  } catch (error) {
    console.error('Error de configuración:', error);
    
    // En desarrollo, mostrar el error
    if (configService.isDevelopment) {
      alert(`Error de configuración: ${error}`);
    }
    
    // Redirigir a página de error o configuración
    router.navigate(['/error', { type: 'config' }]);
    return false;
  }
};

function validateConfiguration(config: ConfigService): void {
  // Validaciones críticas
  if (!config.api.baseUrl) {
    throw new Error('API baseUrl no está configurada');
  }
  
  if (config.api.timeout <= 0) {
    throw new Error('API timeout debe ser mayor a 0');
  }
  
  // Validaciones específicas por ambiente
  if (config.isProduction) {
    validateProductionConfig(config);
  }
  
  if (config.isDevelopment) {
    validateDevelopmentConfig(config);
  }
}

function validateProductionConfig(config: ConfigService): void {
  // En producción, ciertas configuraciones son obligatorias
  if (config.features.enableDebugMode) {
    console.warn('⚠️  Debug mode está habilitado en producción');
  }
  
  if (!config.security.enableHttpsRedirect) {
    throw new Error('HTTPS redirect debe estar habilitado en producción');
  }
  
  if (config.logging.enableConsoleLogging) {
    console.warn('⚠️  Console logging está habilitado en producción');
  }
}

function validateDevelopmentConfig(config: ConfigService): void {
  // En desarrollo, verificar configuraciones comunes
  if (!config.features.enableDebugMode) {
    console.warn('⚠️  Debug mode está deshabilitado en desarrollo');
  }
  
  if (config.security.enableHttpsRedirect) {
    console.warn('⚠️  HTTPS redirect está habilitado en desarrollo');
  }
}

/**
 * @deprecated Use configGuard instead
 * Guard de clase para compatibilidad con versiones anteriores
 */
@Injectable({
  providedIn: 'root'
})
export class ConfigGuard {
  constructor(
    private readonly configService: ConfigService,
    private readonly router: Router
  ) {}

  canActivate(): boolean {
    try {
      validateConfiguration(this.configService);
      return true;
    } catch (error) {
      console.error('Error de configuración:', error);
      
      if (this.configService.isDevelopment) {
        alert(`Error de configuración: ${error}`);
      }
      
      this.router.navigate(['/error', { type: 'config' }]);
      return false;
    }
  }
}
