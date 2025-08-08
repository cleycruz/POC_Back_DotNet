import { Injectable } from '@angular/core';
import { Producto, ProductoProps } from '../../domain/entities/producto.entity';
import { ProductoId } from '../../domain/value-objects/producto-id.vo';
import { Dinero } from '../../domain/value-objects/dinero.vo';
import { Stock } from '../../domain/value-objects/stock.vo';
import { ProductoDto, CrearProductoDto, ActualizarProductoDto } from '../../application/dtos/carrito.dto';

/**
 * Mapper para convertir entre Producto del dominio y DTOs
 */
@Injectable({
  providedIn: 'root'
})
export class ProductoMapper {

  /**
   * Convierte un DTO a entidad de dominio
   */
  public toDomain(dto: ProductoDto): Producto {
    const id = ProductoId.create(dto.id);
    
    const props: ProductoProps = {
      nombre: dto.nombre,
      descripcion: dto.descripcion,
      precio: Dinero.create(dto.precio),
      stock: Stock.create(dto.stock),
      fechaCreacion: new Date(dto.fechaCreacion),
      fechaActualizacion: dto.fechaActualizacion ? new Date(dto.fechaActualizacion) : undefined,
      categoria: dto.categoria
    };

    return new Producto(id, props);
  }

  /**
   * Convierte una entidad de dominio a DTO
   */
  public toDto(producto: Producto): ProductoDto {
    return {
      id: producto.id.value,
      nombre: producto.nombre,
      descripcion: producto.descripcion,
      precio: producto.precio.value,
      stock: producto.stock.value,
      estaDisponible: producto.estaDisponible,
      fechaCreacion: producto.fechaCreacion.toISOString(),
      fechaActualizacion: producto.fechaActualizacion?.toISOString(),
      categoria: producto.categoria
    };
  }

  /**
   * Convierte un CreateDTO a props para crear entidad
   */
  public fromCreateDto(dto: CrearProductoDto): Omit<ProductoProps, 'fechaCreacion'> {
    return {
      nombre: dto.nombre,
      descripcion: dto.descripcion,
      precio: Dinero.create(dto.precio),
      stock: Stock.create(dto.stock),
      categoria: dto.categoria
    };
  }

  /**
   * Convierte un UpdateDTO a props parciales
   */
  public fromUpdateDto(dto: ActualizarProductoDto): Partial<ProductoProps> {
    const props: Partial<ProductoProps> = {};

    if (dto.nombre !== undefined) props.nombre = dto.nombre;
    if (dto.descripcion !== undefined) props.descripcion = dto.descripcion;
    if (dto.precio !== undefined) props.precio = Dinero.create(dto.precio);
    if (dto.stock !== undefined) props.stock = Stock.create(dto.stock);
    if (dto.categoria !== undefined) props.categoria = dto.categoria;

    return props;
  }

  /**
   * Convierte múltiples DTOs a entidades de dominio
   */
  public toDomainList(dtos: ProductoDto[]): Producto[] {
    return dtos.map(dto => this.toDomain(dto));
  }

  /**
   * Convierte múltiples entidades a DTOs
   */
  public toDtoList(productos: Producto[]): ProductoDto[] {
    return productos.map(producto => this.toDto(producto));
  }
}
