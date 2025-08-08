import { Injectable } from '@angular/core';
import { CommandHandler } from './handler.interface';
import { AgregarProductoAlCarritoCommand } from '../commands/agregar-producto-carrito.command';
import { AgregarProductoAlCarritoUseCase } from '../use-cases/agregar-producto-carrito.use-case';

/**
 * Handler para el command AgregarProductoAlCarrito
 */
@Injectable({
  providedIn: 'root'
})
export class AgregarProductoAlCarritoHandler implements CommandHandler<AgregarProductoAlCarritoCommand> {
  
  constructor(
    private readonly agregarProductoUseCase: AgregarProductoAlCarritoUseCase
  ) {}

  async handle(command: AgregarProductoAlCarritoCommand): Promise<void> {
    await this.agregarProductoUseCase.execute({
      productoId: command.productoId,
      cantidad: command.cantidad
    });
  }
}
