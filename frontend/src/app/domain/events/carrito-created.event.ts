import { BaseDomainEvent } from './domain-event.base';
import { CarritoId } from '../value-objects/carrito-id.vo';

/**
 * Evento que se dispara cuando se crea un carrito
 */
export class CarritoCreatedEvent extends BaseDomainEvent {
  constructor(carritoId: CarritoId) {
    super(carritoId.toString(), 'CarritoCreated');
  }
}
