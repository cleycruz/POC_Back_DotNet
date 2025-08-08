import { BaseDomainEvent } from './domain-event.base';
import { ProductoId } from '../value-objects/producto-id.vo';
import { ProductoProps } from '../entities/producto.entity';

/**
 * Evento que se dispara cuando se actualiza un producto
 */
export class ProductoUpdatedEvent extends BaseDomainEvent {
  public readonly changes: Partial<ProductoProps>;

  constructor(productoId: ProductoId, changes: Partial<ProductoProps>) {
    super(productoId.toString(), 'ProductoUpdated');
    this.changes = changes;
  }
}
