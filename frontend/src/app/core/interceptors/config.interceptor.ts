import { Injectable } from '@angular/core';
import { 
  HttpInterceptor, 
  HttpRequest, 
  HttpHandler, 
  HttpEvent,
  HttpErrorResponse 
} from '@angular/common/http';
import { Observable, throwError, timer } from 'rxjs';
import { retry, catchError, timeout } from 'rxjs/operators';
import { ConfigService } from '../services/config.service';

/**
 * Interceptor HTTP que aplica configuración automáticamente a todas las peticiones
 */
@Injectable()
export class ConfigInterceptor implements HttpInterceptor {

  constructor(private readonly configService: ConfigService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Aplicar configuración base
    const configuredRequest = this.applyConfiguration(request);

    return next.handle(configuredRequest).pipe(
      // Aplicar timeout basado en configuración
      timeout(this.configService.api.timeout),
      
      // Aplicar retry basado en configuración
      retry({
        count: this.configService.api.retryAttempts,
        delay: (error, retryCount) => {
          // Solo retry en errores de red, no en errores de validación
          if (error.status >= 400 && error.status < 500) {
            return throwError(() => error);
          }
          
          // Delay exponencial: 1s, 2s, 4s...
          const delayMs = Math.pow(2, retryCount - 1) * 1000;
          return timer(delayMs);
        }
      }),
      
      // Manejo de errores
      catchError((error: HttpErrorResponse) => {
        this.logError(error, request);
        return throwError(() => error);
      })
    );
  }

  private applyConfiguration(request: HttpRequest<any>): HttpRequest<any> {
    const config = this.configService;
    
    // Agregar headers de configuración
    const headers: { [key: string]: string } = {};

    // Agregar header de versión de API
    headers['X-API-Version'] = config.api.version;

    // Agregar header de aplicación
    headers['X-App-Name'] = config.app.name;
    headers['X-App-Version'] = config.app.version;

    // Agregar header de ambiente (solo en desarrollo/staging)
    if (!config.isProduction) {
      headers['X-Environment'] = config.getEnvironmentType();
    }

    // Agregar headers de CSRF si está habilitado
    if (config.security.enableCsrfProtection) {
      const csrfToken = this.getCsrfToken();
      if (csrfToken) {
        headers['X-CSRF-Token'] = csrfToken;
      }
    }

    // Crear request con headers
    let modifiedRequest = request.clone({
      setHeaders: headers
    });

    // Transformar URL relativas a absolutas usando la configuración
    if (!request.url.startsWith('http')) {
      const fullUrl = config.getApiUrl(request.url);
      modifiedRequest = modifiedRequest.clone({
        url: fullUrl
      });
    }

    return modifiedRequest;
  }

  private getCsrfToken(): string | null {
    // Obtener token CSRF del meta tag o cookie
    const metaTag = document.querySelector('meta[name="csrf-token"]') as HTMLMetaElement;
    if (metaTag) {
      return metaTag.content;
    }

    // Fallback: obtener de cookie
    const csrfRegex = /XSRF-TOKEN=([^;]+)/;
    const cookieMatch = csrfRegex.exec(document.cookie);
    return cookieMatch ? decodeURIComponent(cookieMatch[1]) : null;
  }

  private logError(error: HttpErrorResponse, request: HttpRequest<any>): void {
    if (!this.configService.logging.enableConsoleLogging) {
      return;
    }

    const errorInfo = {
      message: error.message,
      status: error.status,
      statusText: error.statusText,
      url: request.url,
      method: request.method,
      timestamp: new Date().toISOString()
    };

    if (this.configService.features.enableDetailedLogging) {
      console.group('❌ HTTP Error');
      console.error('Error details:', errorInfo);
      console.error('Original error:', error);
      console.error('Request details:', {
        headers: request.headers.keys().reduce((acc, key) => {
          acc[key] = request.headers.get(key);
          return acc;
        }, {} as any),
        body: request.body,
        params: request.params.keys().reduce((acc, key) => {
          acc[key] = request.params.get(key);
          return acc;
        }, {} as any)
      });
      console.groupEnd();
    } else {
      console.error('HTTP Error:', errorInfo);
    }
  }
}
