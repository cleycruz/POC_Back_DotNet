import { Entity } from './entity.base';
import { CarritoId } from '../value-objects/carrito-id.vo';
import { ProductoId } from '../value-objects/producto-id.vo';
import { Dinero } from '../value-objects/dinero.vo';
import { Cantidad } from '../value-objects/cantidad.vo';
import { CarritoItem } from './carrito-item.entity';
import { CarritoCreatedEvent } from '../events/carrito-created.event';
import { ItemAgreagadoAlCarritoEvent } from '../events/item-agregado-carrito.event';
import { ItemRemovidoDelCarritoEvent } from '../events/item-removido-carrito.event';

export interface CarritoProps {
  fechaCreacion: Date;
  fechaActualizacion?: Date;
}

/**
 * Aggregate Root - Carrito de Compras
 * Representa el carrito de compras del cliente
 */
export class Carrito extends Entity<CarritoId> {
  private readonly _fechaCreacion: Date;
  private _fechaActualizacion?: Date;
  private _items: Map<string, CarritoItem> = new Map();
  private _domainEvents: any[] = [];

  constructor(id: CarritoId, props: CarritoProps) {
    super(id);
    this._fechaCreacion = props.fechaCreacion;
    this._fechaActualizacion = props.fechaActualizacion;
  }

  // Getters
  public get fechaCreacion(): Date {
    return this._fechaCreacion;
  }

  public get fechaActualizacion(): Date | undefined {
    return this._fechaActualizacion;
  }

  public get items(): CarritoItem[] {
    return Array.from(this._items.values());
  }

  public get cantidadItems(): number {
    return this._items.size;
  }

  public get cantidadTotalProductos(): number {
    return this.items.reduce((total, item) => total + item.cantidad.value, 0);
  }

  public get total(): Dinero {
    return this.items.reduce(
      (total, item) => total.add(item.subtotal),
      Dinero.zero()
    );
  }

  public get estaVacio(): boolean {
    return this._items.size === 0;
  }

  // Métodos de dominio
  public agregarItem(
    productoId: ProductoId,
    productoNombre: string,
    cantidad: Cantidad,
    precioUnitario: Dinero
  ): void {
    const productoKey = productoId.toString();
    
    if (this._items.has(productoKey)) {
      // Si el producto ya existe, aumentar la cantidad
      const itemExistente = this._items.get(productoKey)!;
      const nuevaCantidad = Cantidad.create(itemExistente.cantidad.value + cantidad.value);
      itemExistente.actualizarCantidad(nuevaCantidad);
    } else {
      // Crear nuevo item
      const nuevoItem = CarritoItem.crear(
        this.id,
        productoId,
        productoNombre,
        cantidad,
        precioUnitario
      );
      this._items.set(productoKey, nuevoItem);
    }

    this._fechaActualizacion = new Date();
    this.addDomainEvent(new ItemAgreagadoAlCarritoEvent(this.id, productoId, cantidad));
  }

  public removerItem(productoId: ProductoId): void {
    const productoKey = productoId.toString();
    
    if (!this._items.has(productoKey)) {
      throw new Error('El producto no está en el carrito');
    }

    this._items.delete(productoKey);
    this._fechaActualizacion = new Date();
    this.addDomainEvent(new ItemRemovidoDelCarritoEvent(this.id, productoId));
  }

  public actualizarCantidadItem(productoId: ProductoId, nuevaCantidad: Cantidad): void {
    const productoKey = productoId.toString();
    
    if (!this._items.has(productoKey)) {
      throw new Error('El producto no está en el carrito');
    }

    const item = this._items.get(productoKey)!;
    item.actualizarCantidad(nuevaCantidad);
    this._fechaActualizacion = new Date();
  }

  public obtenerItem(productoId: ProductoId): CarritoItem | undefined {
    return this._items.get(productoId.toString());
  }

  public tieneProducto(productoId: ProductoId): boolean {
    return this._items.has(productoId.toString());
  }

  public vaciar(): void {
    this._items.clear();
    this._fechaActualizacion = new Date();
  }

  // Factory method
  public static crear(): Carrito {
    const id = CarritoId.generate();
    const carrito = new Carrito(id, { fechaCreacion: new Date() });
    carrito.addDomainEvent(new CarritoCreatedEvent(id));
    return carrito;
  }

  // Domain events management
  public getDomainEvents(): any[] {
    return [...this._domainEvents];
  }

  public clearDomainEvents(): void {
    this._domainEvents = [];
  }

  private addDomainEvent(event: any): void {
    this._domainEvents.push(event);
  }
}
