import { Injectable } from '@angular/core';
import { QueryHandler } from './handler.interface';
import { ObtenerCarritoQuery } from '../queries/obtener-carrito.query';
import { ObtenerCarritoUseCase, ObtenerCarritoResponse } from '../use-cases/obtener-carrito.use-case';

/**
 * Handler para la query ObtenerCarrito
 */
@Injectable({
  providedIn: 'root'
})
export class ObtenerCarritoHandler implements QueryHandler<ObtenerCarritoQuery, ObtenerCarritoResponse> {
  
  constructor(
    private readonly obtenerCarritoUseCase: ObtenerCarritoUseCase
  ) {}

  async handle(query: ObtenerCarritoQuery): Promise<ObtenerCarritoResponse> {
    return await this.obtenerCarritoUseCase.execute({});
  }
}
