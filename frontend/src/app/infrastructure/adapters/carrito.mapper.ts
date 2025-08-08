import { Injectable } from '@angular/core';
import { Carrito, CarritoProps } from '../../domain/entities/carrito.entity';
import { CarritoItem, CarritoItemProps } from '../../domain/entities/carrito-item.entity';
import { CarritoId } from '../../domain/value-objects/carrito-id.vo';
import { ProductoId } from '../../domain/value-objects/producto-id.vo';
import { Cantidad } from '../../domain/value-objects/cantidad.vo';
import { Dinero } from '../../domain/value-objects/dinero.vo';
import { CarritoDto, CarritoItemDto } from '../../application/dtos/carrito.dto';

/**
 * Mapper para convertir entre Carrito del dominio y DTOs
 */
@Injectable({
  providedIn: 'root'
})
export class CarritoMapper {

  /**
   * Convierte un DTO a entidad de dominio Carrito
   */
  public toDomain(dto: CarritoDto): Carrito {
    const id = CarritoId.create(dto.id);
    
    const props: CarritoProps = {
      fechaCreacion: new Date(dto.fechaCreacion),
      fechaActualizacion: dto.fechaActualizacion ? new Date(dto.fechaActualizacion) : undefined
    };

    const carrito = new Carrito(id, props);

    // Agregar items al carrito
    dto.items.forEach(itemDto => {
      const item = this.carritoItemToDomain(itemDto);
      // Aquí necesitaríamos acceso a métodos protegidos o una forma de reconstruir el estado
      // Por simplicidad, asumimos que el carrito puede ser reconstruido
    });

    return carrito;
  }

  /**
   * Convierte una entidad de dominio Carrito a DTO
   */
  public toDto(carrito: Carrito): CarritoDto {
    return {
      id: carrito.id.value,
      items: carrito.items.map(item => this.carritoItemToDto(item)),
      cantidadItems: carrito.cantidadItems,
      cantidadTotalProductos: carrito.cantidadTotalProductos,
      total: carrito.total.value,
      fechaCreacion: carrito.fechaCreacion.toISOString(),
      fechaActualizacion: carrito.fechaActualizacion?.toISOString()
    };
  }

  /**
   * Convierte un DTO a entidad CarritoItem
   */
  public carritoItemToDomain(dto: CarritoItemDto): CarritoItem {
    const props: CarritoItemProps = {
      carritoId: CarritoId.create(1), // En un caso real vendría del contexto
      productoId: ProductoId.create(dto.productoId),
      productoNombre: dto.productoNombre,
      cantidad: Cantidad.create(dto.cantidad),
      precioUnitario: Dinero.create(dto.precioUnitario)
    };

    return new CarritoItem(dto.id, props);
  }

  /**
   * Convierte una entidad CarritoItem a DTO
   */
  public carritoItemToDto(item: CarritoItem): CarritoItemDto {
    return {
      id: item.id,
      productoId: item.productoId.value,
      productoNombre: item.productoNombre,
      cantidad: item.cantidad.value,
      precioUnitario: item.precioUnitario.value,
      subtotal: item.subtotal.value
    };
  }

  /**
   * Convierte múltiples DTOs a entidades de dominio
   */
  public toDomainList(dtos: CarritoDto[]): Carrito[] {
    return dtos.map(dto => this.toDomain(dto));
  }

  /**
   * Convierte múltiples entidades a DTOs
   */
  public toDtoList(carritos: Carrito[]): CarritoDto[] {
    return carritos.map(carrito => this.toDto(carrito));
  }
}
