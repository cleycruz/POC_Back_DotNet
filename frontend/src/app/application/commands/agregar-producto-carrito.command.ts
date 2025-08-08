import { BaseCommand } from './command.base';

/**
 * Command para agregar un producto al carrito
 */
export class AgregarProductoAlCarritoCommand extends BaseCommand {
  constructor(
    public readonly productoId: number,
    public readonly cantidad: number
  ) {
    super();
  }
}
