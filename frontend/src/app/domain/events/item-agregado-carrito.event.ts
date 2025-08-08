import { BaseDomainEvent } from './domain-event.base';
import { CarritoId } from '../value-objects/carrito-id.vo';
import { ProductoId } from '../value-objects/producto-id.vo';
import { Cantidad } from '../value-objects/cantidad.vo';

/**
 * Evento que se dispara cuando se agrega un item al carrito
 */
export class ItemAgreagadoAlCarritoEvent extends BaseDomainEvent {
  public readonly productoId: ProductoId;
  public readonly cantidad: Cantidad;

  constructor(carritoId: CarritoId, productoId: ProductoId, cantidad: Cantidad) {
    super(carritoId.toString(), 'ItemAgregadoAlCarrito');
    this.productoId = productoId;
    this.cantidad = cantidad;
  }
}
