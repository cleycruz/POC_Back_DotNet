import { Injectable } from '@angular/core';
import { UseCase } from './use-case.interface';
import { IProductoRepository } from '../../domain/repositories/producto.repository.interface';
import { Producto } from '../../domain/entities/producto.entity';

export interface ObtenerProductosRequest {
  // Criterios de filtrado opcionales
  categoria?: string;
  soloDisponibles?: boolean;
  busqueda?: string;
}

export interface ObtenerProductosResponse {
  productos: Producto[];
  total: number;
}

/**
 * Caso de uso para obtener la lista de productos
 */
@Injectable({
  providedIn: 'root'
})
export class ObtenerProductosUseCase implements UseCase<ObtenerProductosRequest, ObtenerProductosResponse> {
  
  constructor(
    private readonly productoRepository: IProductoRepository
  ) {}

  async execute(request: ObtenerProductosRequest): Promise<ObtenerProductosResponse> {
    let productos: Producto[];

    // Aplicar filtros según la request
    if (request.categoria) {
      productos = await this.productoRepository.findByCategoria(request.categoria);
    } else if (request.busqueda) {
      productos = await this.productoRepository.findByNombre(request.busqueda);
    } else if (request.soloDisponibles) {
      productos = await this.productoRepository.findDisponibles();
    } else {
      productos = await this.productoRepository.findAll();
    }

    return {
      productos,
      total: productos.length
    };
  }
}
