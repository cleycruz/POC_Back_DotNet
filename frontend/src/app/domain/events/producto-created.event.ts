import { BaseDomainEvent } from './domain-event.base';
import { ProductoId } from '../value-objects/producto-id.vo';
import { ProductoProps } from '../entities/producto.entity';

/**
 * Evento que se dispara cuando se crea un producto
 */
export class ProductoCreatedEvent extends BaseDomainEvent {
  public readonly productProps: ProductoProps;

  constructor(productoId: ProductoId, productProps: ProductoProps) {
    super(productoId.toString(), 'ProductoCreated');
    this.productProps = productProps;
  }
}
