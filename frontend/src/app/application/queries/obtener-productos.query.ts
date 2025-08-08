import { BaseQuery } from './query.base';

/**
 * Query para obtener todos los productos
 */
export class ObtenerProductosQuery extends BaseQuery {
  constructor(
    public readonly categoria?: string,
    public readonly soloDisponibles?: boolean,
    public readonly busqueda?: string
  ) {
    super();
  }
}
