import { Entity } from './entity.base';
import { ProductoId } from '../value-objects/producto-id.vo';
import { CarritoId } from '../value-objects/carrito-id.vo';
import { Cantidad } from '../value-objects/cantidad.vo';
import { Dinero } from '../value-objects/dinero.vo';

export interface CarritoItemProps {
  carritoId: CarritoId;
  productoId: ProductoId;
  productoNombre: string;
  cantidad: Cantidad;
  precioUnitario: Dinero;
}

/**
 * Entidad CarritoItem del dominio
 * Representa un item dentro del carrito
 */
export class CarritoItem extends Entity<number> {
  private _carritoId: CarritoId;
  private _productoId: ProductoId;
  private _productoNombre: string;
  private _cantidad: Cantidad;
  private _precioUnitario: Dinero;

  constructor(id: number, props: CarritoItemProps) {
    super(id);
    this._carritoId = props.carritoId;
    this._productoId = props.productoId;
    this._productoNombre = props.productoNombre;
    this._cantidad = props.cantidad;
    this._precioUnitario = props.precioUnitario;
  }

  // Getters
  public get carritoId(): CarritoId {
    return this._carritoId;
  }

  public get productoId(): ProductoId {
    return this._productoId;
  }

  public get productoNombre(): string {
    return this._productoNombre;
  }

  public get cantidad(): Cantidad {
    return this._cantidad;
  }

  public get precioUnitario(): Dinero {
    return this._precioUnitario;
  }

  public get subtotal(): Dinero {
    return this._precioUnitario.multiply(this._cantidad.value);
  }

  // Métodos de dominio
  public actualizarCantidad(nuevaCantidad: Cantidad): void {
    this._cantidad = nuevaCantidad;
  }

  public aumentarCantidad(): void {
    this._cantidad = this._cantidad.increase();
  }

  public disminuirCantidad(): void {
    if (this._cantidad.value <= 1) {
      throw new Error('No se puede disminuir la cantidad por debajo de 1');
    }
    this._cantidad = this._cantidad.decrease();
  }

  public actualizarPrecio(nuevoPrecio: Dinero): void {
    this._precioUnitario = nuevoPrecio;
  }

  // Factory method
  public static crear(
    carritoId: CarritoId,
    productoId: ProductoId,
    productoNombre: string,
    cantidad: Cantidad,
    precioUnitario: Dinero
  ): CarritoItem {
    const id = Math.floor(Math.random() * 1000000) + 1; // En producción vendría del backend
    
    return new CarritoItem(id, {
      carritoId,
      productoId,
      productoNombre,
      cantidad,
      precioUnitario
    });
  }
}
