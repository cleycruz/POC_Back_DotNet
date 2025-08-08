// Modelos basados en la API OpenAPI actualizada
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
  usuarioId: string;
  items: CarritoItemDto[];
  total: number;
  cantidadItems: number;
  cantidadProductos: number;
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

export interface ActualizarCantidadDto {
  cantidad: number;
}

// Nuevos modelos para funcionalidades adicionales
export interface CarritoItem {
  id: number;
  productoId: number;
  producto: ProductoDto;
  cantidadItem: number;
  precioUnitario: number;
  subtotal: number;
  fechaAgregado: string;
}

export interface CarritoResumen {
  usuarioId: string;
  totalItems: number;
  totalProductos: number;
  total: number;
  fechaUltimaActualizacion: string;
}

// Aliases para compatibilidad
export type Producto = ProductoDto;
export type ItemCarrito = CarritoItemDto;
export type Carrito = CarritoDto;
export type CreateProductoRequest = CrearProductoDto;
export type UpdateProductoRequest = ActualizarProductoDto;
export type AddToCarritoRequest = AgregarItemCarritoDto;
export type UpdateCarritoItemRequest = ActualizarCantidadDto;
