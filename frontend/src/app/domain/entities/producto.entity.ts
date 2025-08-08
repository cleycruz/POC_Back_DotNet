import { Entity } from './entity.base';
import { ProductoId } from '../value-objects/producto-id.vo';
import { Dinero } from '../value-objects/dinero.vo';
import { Stock } from '../value-objects/stock.vo';
import { ProductoCreatedEvent } from '../events/producto-created.event';
import { ProductoUpdatedEvent } from '../events/producto-updated.event';

export interface ProductoProps {
  nombre: string;
  descripcion?: string;
  precio: Dinero;
  stock: Stock;
  fechaCreacion: Date;
  fechaActualizacion?: Date;
  categoria?: string;
}

/**
 * Entidad Producto del dominio
 * Representa un producto en el carrito de compras
 */
export class Producto extends Entity<ProductoId> {
  private _nombre: string;
  private _descripcion?: string;
  private _precio: Dinero;
  private _stock: Stock;
  private _fechaCreacion: Date;
  private _fechaActualizacion?: Date;
  private _categoria?: string;
  private _domainEvents: any[] = [];

  constructor(id: ProductoId, props: ProductoProps) {
    super(id);
    this._nombre = props.nombre;
    this._descripcion = props.descripcion;
    this._precio = props.precio;
    this._stock = props.stock;
    this._fechaCreacion = props.fechaCreacion;
    this._fechaActualizacion = props.fechaActualizacion;
    this._categoria = props.categoria;
  }

  // Getters
  public get nombre(): string {
    return this._nombre;
  }

  public get descripcion(): string | undefined {
    return this._descripcion;
  }

  public get precio(): Dinero {
    return this._precio;
  }

  public get stock(): Stock {
    return this._stock;
  }

  public get fechaCreacion(): Date {
    return this._fechaCreacion;
  }

  public get fechaActualizacion(): Date | undefined {
    return this._fechaActualizacion;
  }

  public get categoria(): string | undefined {
    return this._categoria;
  }

  public get estaDisponible(): boolean {
    return this._stock.value > 0;
  }

  // Métodos de dominio
  public actualizarPrecio(nuevoPrecio: Dinero): void {
    if (nuevoPrecio.value <= 0) {
      throw new Error('El precio debe ser mayor a cero');
    }
    
    this._precio = nuevoPrecio;
    this._fechaActualizacion = new Date();
    this.addDomainEvent(new ProductoUpdatedEvent(this.id, { precio: nuevoPrecio }));
  }

  public actualizarStock(nuevoStock: Stock): void {
    this._stock = nuevoStock;
    this._fechaActualizacion = new Date();
    this.addDomainEvent(new ProductoUpdatedEvent(this.id, { stock: nuevoStock }));
  }

  public reducirStock(cantidad: number): void {
    if (cantidad <= 0) {
      throw new Error('La cantidad debe ser mayor a cero');
    }

    if (!this.tieneStockSuficiente(cantidad)) {
      throw new Error('Stock insuficiente');
    }

    const nuevoStock = new Stock(this._stock.value - cantidad);
    this.actualizarStock(nuevoStock);
  }

  public aumentarStock(cantidad: number): void {
    if (cantidad <= 0) {
      throw new Error('La cantidad debe ser mayor a cero');
    }

    const nuevoStock = new Stock(this._stock.value + cantidad);
    this.actualizarStock(nuevoStock);
  }

  public tieneStockSuficiente(cantidad: number): boolean {
    return this._stock.value >= cantidad;
  }

  public actualizarInformacion(props: Partial<ProductoProps>): void {
    if (props.nombre) this._nombre = props.nombre;
    if (props.descripcion !== undefined) this._descripcion = props.descripcion;
    if (props.categoria !== undefined) this._categoria = props.categoria;
    
    this._fechaActualizacion = new Date();
    this.addDomainEvent(new ProductoUpdatedEvent(this.id, props));
  }

  // Factory method
  public static crear(props: Omit<ProductoProps, 'fechaCreacion'>): Producto {
    const id = ProductoId.generate();
    const productProps: ProductoProps = {
      ...props,
      fechaCreacion: new Date()
    };
    
    const producto = new Producto(id, productProps);
    producto.addDomainEvent(new ProductoCreatedEvent(id, productProps));
    
    return producto;
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
