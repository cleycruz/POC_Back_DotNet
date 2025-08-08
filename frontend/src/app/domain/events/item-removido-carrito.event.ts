import { BaseDomainEvent } from './domain-event.base';
import { CarritoId } from '../value-objects/carrito-id.vo';
import { ProductoId } from '../value-objects/producto-id.vo';

/**
 * Evento que se dispara cuando se remueve un item del carrito
 */
export class ItemRemovidoDelCarritoEvent extends BaseDomainEvent {
  public readonly productoId: ProductoId;

  constructor(carritoId: CarritoId, productoId: ProductoId) {
    super(carritoId.toString(), 'ItemRemovidoDelCarrito');
    this.productoId = productoId;
  }
}
