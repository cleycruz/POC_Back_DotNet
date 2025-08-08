import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

// Services
import { ConfigService } from './services/config.service';

// Interceptors
import { ConfigInterceptor } from './interceptors/config.interceptor';

/**
 * Módulo Core que se debe importar solo una vez en AppModule
 * Contiene servicios singleton y configuración global
 */
@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  providers: [
    // Servicios singleton
    ConfigService,
    
    // HTTP Interceptors
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ConfigInterceptor,
      multi: true
    }
  ]
})
export class CoreModule {
  
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error(
        'CoreModule ya está cargado. Importar solo en AppModule.'
      );
    }
  }
}
