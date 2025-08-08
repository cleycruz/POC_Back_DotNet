/**
 * DTOs para la capa de aplicación
 * Estos DTOs se usan para transferir datos entre capas sin exponer el dominio
 */

export interface ProductoDto {
  id: number;
  nombre: string;
  descripcion?: string;
  precio: number;
  stock: number;
  estaDisponible: boolean;
  fechaCreacion: string;
  fechaActualizacion?: string;
  categoria?: string;
}

export interface CarritoItemDto {
  id: number;
  productoId: number;
  productoNombre: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface CarritoDto {
  id: number;
  items: CarritoItemDto[];
  cantidadItems: number;
  cantidadTotalProductos: number;
  total: number;
  fechaCreacion: string;
  fechaActualizacion?: string;
}

export interface CrearProductoDto {
  nombre: string;
  descripcion?: string;
  precio: number;
  stock: number;
  categoria?: string;
}

export interface ActualizarProductoDto {
  nombre?: string;
  descripcion?: string;
  precio?: number;
  stock?: number;
  categoria?: string;
}

export interface AgregarItemCarritoDto {
  productoId: number;
  cantidad: number;
}

export interface ActualizarCantidadDto {
  cantidad: number;
}
