// Modelos basados en la API OpenAPI
export interface ProductoDto {
  id: number;
  nombre: string;
  descripcion: string;
  precio: number;
  stock: number;
  categoria: string;
}

export interface CarritoItemDto {
  id: number;
  productoId: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface CarritoDto {
  id: number;
  usuarioId: string;
  items: CarritoItemDto[];
  total: number;
  fechaCreacion: string;
  fechaActualizacion: string;
}

export interface CrearProductoDto {
  nombre: string;
  descripcion?: string;
  precio: number;
  stock: number;
  categoria: string;
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

export interface ActualizarItemCarritoDto {
  cantidad: number;
}

// Aliases para compatibilidad
export type Producto = ProductoDto;
export type ItemCarrito = CarritoItemDto;
export type Carrito = CarritoDto;
export type CreateProductoRequest = CrearProductoDto;
export type UpdateProductoRequest = ActualizarProductoDto;
export type AddToCarritoRequest = AgregarItemCarritoDto;
export type UpdateCarritoItemRequest = ActualizarItemCarritoDto;
