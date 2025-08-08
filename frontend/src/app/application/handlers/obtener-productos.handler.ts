import { Injectable } from '@angular/core';
import { QueryHandler } from './handler.interface';
import { ObtenerProductosQuery } from '../queries/obtener-productos.query';
import { ObtenerProductosUseCase, ObtenerProductosResponse } from '../use-cases/obtener-productos.use-case';

/**
 * Handler para la query ObtenerProductos
 */
@Injectable({
  providedIn: 'root'
})
export class ObtenerProductosHandler implements QueryHandler<ObtenerProductosQuery, ObtenerProductosResponse> {
  
  constructor(
    private readonly obtenerProductosUseCase: ObtenerProductosUseCase
  ) {}

  async handle(query: ObtenerProductosQuery): Promise<ObtenerProductosResponse> {
    return await this.obtenerProductosUseCase.execute({
      categoria: query.categoria,
      soloDisponibles: query.soloDisponibles,
      busqueda: query.busqueda
    });
  }
}
