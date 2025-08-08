// Domain Exports
export * from './entities/entity.base';
export * from './entities/producto.entity';
export * from './entities/carrito.entity';
export * from './entities/carrito-item.entity';

export * from './value-objects/value-object.base';
export * from './value-objects/producto-id.vo';
export * from './value-objects/carrito-id.vo';
export * from './value-objects/dinero.vo';
export * from './value-objects/stock.vo';
export * from './value-objects/cantidad.vo';

export * from './events/domain-event.base';
export * from './events/producto-created.event';
export * from './events/producto-updated.event';
export * from './events/carrito-created.event';
export * from './events/item-agregado-carrito.event';
export * from './events/item-removido-carrito.event';

export * from './repositories/producto.repository.interface';
export * from './repositories/carrito.repository.interface';

export * from './services/carrito-domain.service';
