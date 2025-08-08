import { Injectable } from '@angular/core';
import { UseCase } from './use-case.interface';
import { ICarritoRepository } from '../../domain/repositories/carrito.repository.interface';
import { Carrito } from '../../domain/entities/carrito.entity';

export interface ObtenerCarritoRequest {
  // Podría incluir filtros o configuraciones específicas
}

export interface ObtenerCarritoResponse {
  carrito: Carrito | null;
  total: number;
  cantidadItems: number;
}

/**
 * Caso de uso para obtener el carrito actual
 */
@Injectable({
  providedIn: 'root'
})
export class ObtenerCarritoUseCase implements UseCase<ObtenerCarritoRequest, ObtenerCarritoResponse> {
  
  constructor(
    private readonly carritoRepository: ICarritoRepository
  ) {}

  async execute(request: ObtenerCarritoRequest): Promise<ObtenerCarritoResponse> {
    const carrito = await this.carritoRepository.findActive();

    if (!carrito) {
      return {
        carrito: null,
        total: 0,
        cantidadItems: 0
      };
    }

    return {
      carrito,
      total: carrito.total.value,
      cantidadItems: carrito.cantidadItems
    };
  }
}
