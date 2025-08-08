import { BaseCommand } from './command.base';

/**
 * Command para actualizar la cantidad de un producto en el carrito
 */
export class ActualizarCantidadProductoCommand extends BaseCommand {
  constructor(
    public readonly productoId: number,
    public readonly nuevaCantidad: number
  ) {
    super();
  }
}
