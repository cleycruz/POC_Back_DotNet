import { BaseCommand } from './command.base';

/**
 * Command para remover un producto del carrito
 */
export class RemoverProductoDelCarritoCommand extends BaseCommand {
  constructor(
    public readonly productoId: number
  ) {
    super();
  }
}
