import { Injectable } from '@angular/core';
import { UseCaseVoid } from './use-case.interface';
import { ICarritoRepository } from '../../domain/repositories/carrito.repository.interface';
import { IProductoRepository } from '../../domain/repositories/producto.repository.interface';
import { CarritoDomainService } from '../../domain/services/carrito-domain.service';
import { ProductoId } from '../../domain/value-objects/producto-id.vo';
import { Cantidad } from '../../domain/value-objects/cantidad.vo';

export interface AgregarProductoAlCarritoRequest {
  productoId: number;
  cantidad: number;
}

/**
 * Caso de uso para agregar un producto al carrito
 */
@Injectable({
  providedIn: 'root'
})
export class AgregarProductoAlCarritoUseCase implements UseCaseVoid<AgregarProductoAlCarritoRequest> {
  
  constructor(
    private readonly carritoRepository: ICarritoRepository,
    private readonly productoRepository: IProductoRepository,
    private readonly carritoDomainService: CarritoDomainService
  ) {}

  async execute(request: AgregarProductoAlCarritoRequest): Promise<void> {
    // Validar entrada
    if (request.cantidad <= 0) {
      throw new Error('La cantidad debe ser mayor a cero');
    }

    // Crear value objects
    const productoId = ProductoId.create(request.productoId);
    const cantidad = Cantidad.create(request.cantidad);

    // Obtener o crear carrito
    let carrito = await this.carritoRepository.findActive();
    if (!carrito) {
      carrito = await this.carritoRepository.create();
    }

    // Obtener producto
    const producto = await this.productoRepository.findById(productoId);
    if (!producto) {
      throw new Error('Producto no encontrado');
    }

    // Usar el servicio de dominio para aplicar reglas de negocio
    this.carritoDomainService.agregarProductoAlCarrito(carrito, producto, cantidad);

    // Guardar cambios
    await this.carritoRepository.save(carrito);
  }
}
