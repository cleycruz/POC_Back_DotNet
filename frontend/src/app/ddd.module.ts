import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

// Domain Interfaces
import { IProductoRepository } from '../domain/repositories/producto.repository.interface';
import { ICarritoRepository } from '../domain/repositories/carrito.repository.interface';

// Infrastructure Implementations
import { HttpProductoRepository } from '../infrastructure/repositories/http-producto.repository';
import { HttpCarritoRepository } from '../infrastructure/repositories/http-carrito.repository';

// Mappers
import { ProductoMapper } from '../infrastructure/adapters/producto.mapper';
import { CarritoMapper } from '../infrastructure/adapters/carrito.mapper';

// HTTP Services
import { ProductosHttpService } from '../infrastructure/http/productos-http.service';

// Domain Services
import { CarritoDomainService } from '../domain/services/carrito-domain.service';

// Use Cases
import { ObtenerProductosUseCase } from '../application/use-cases/obtener-productos.use-case';
import { AgregarProductoAlCarritoUseCase } from '../application/use-cases/agregar-producto-carrito.use-case';
import { ObtenerCarritoUseCase } from '../application/use-cases/obtener-carrito.use-case';

// Handlers
import { ObtenerProductosHandler } from '../application/handlers/obtener-productos.handler';
import { AgregarProductoAlCarritoHandler } from '../application/handlers/agregar-producto-carrito.handler';
import { ObtenerCarritoHandler } from '../application/handlers/obtener-carrito.handler';

/**
 * Módulo que configura la inyección de dependencias para DDD
 */
@NgModule({
  imports: [
    CommonModule,
    HttpClientModule
  ],
  providers: [
    // Repository Implementations
    {
      provide: IProductoRepository,
      useClass: HttpProductoRepository
    },
    {
      provide: ICarritoRepository,
      useClass: HttpCarritoRepository
    },
    
    // Mappers
    ProductoMapper,
    CarritoMapper,
    
    // HTTP Services
    ProductosHttpService,
    
    // Domain Services
    CarritoDomainService,
    
    // Use Cases
    ObtenerProductosUseCase,
    AgregarProductoAlCarritoUseCase,
    ObtenerCarritoUseCase,
    
    // Handlers
    ObtenerProductosHandler,
    AgregarProductoAlCarritoHandler,
    ObtenerCarritoHandler
  ]
})
export class DddModule {
  
  /**
   * Configuración para el módulo raíz
   */
  static forRoot() {
    return {
      ngModule: DddModule,
      providers: []
    };
  }
}
