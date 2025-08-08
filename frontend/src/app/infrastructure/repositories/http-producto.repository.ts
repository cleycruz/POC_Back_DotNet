import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { IProductoRepository } from '../../domain/repositories/producto.repository.interface';
import { Producto } from '../../domain/entities/producto.entity';
import { ProductoId } from '../../domain/value-objects/producto-id.vo';
import { ProductoDto } from '../../application/dtos/carrito.dto';
import { ProductoMapper } from '../adapters/producto.mapper';

/**
 * Implementación HTTP del repositorio de productos
 */
@Injectable({
  providedIn: 'root'
})
export class HttpProductoRepository implements IProductoRepository {
  private readonly baseUrl = 'http://localhost:5063/api/productos';

  constructor(
    private readonly http: HttpClient,
    private readonly mapper: ProductoMapper
  ) {}

  async findById(id: ProductoId): Promise<Producto | null> {
    try {
      const dto = await firstValueFrom(
        this.http.get<ProductoDto>(`${this.baseUrl}/${id.value}`)
      );
      return this.mapper.toDomain(dto);
    } catch (error) {
      console.error('Error al obtener producto por ID:', error);
      return null;
    }
  }

  async findAll(): Promise<Producto[]> {
    try {
      const dtos = await firstValueFrom(
        this.http.get<ProductoDto[]>(this.baseUrl)
      );
      return dtos.map(dto => this.mapper.toDomain(dto));
    } catch (error) {
      console.error('Error al obtener productos:', error);
      return [];
    }
  }

  async findByNombre(nombre: string): Promise<Producto[]> {
    try {
      const dtos = await firstValueFrom(
        this.http.get<ProductoDto[]>(`${this.baseUrl}/buscar`, {
          params: { nombre }
        })
      );
      return dtos.map(dto => this.mapper.toDomain(dto));
    } catch (error) {
      console.error('Error al buscar productos por nombre:', error);
      return [];
    }
  }

  async findByCategoria(categoria: string): Promise<Producto[]> {
    try {
      const dtos = await firstValueFrom(
        this.http.get<ProductoDto[]>(`${this.baseUrl}/categoria/${categoria}`)
      );
      return dtos.map(dto => this.mapper.toDomain(dto));
    } catch (error) {
      console.error('Error al buscar productos por categoría:', error);
      return [];
    }
  }

  async findDisponibles(): Promise<Producto[]> {
    try {
      const dtos = await firstValueFrom(
        this.http.get<ProductoDto[]>(`${this.baseUrl}/disponibles`)
      );
      return dtos.map(dto => this.mapper.toDomain(dto));
    } catch (error) {
      console.error('Error al obtener productos disponibles:', error);
      return [];
    }
  }

  async save(producto: Producto): Promise<void> {
    try {
      const dto = this.mapper.toDto(producto);
      
      if (producto.id.value > 0) {
        // Actualizar
        await firstValueFrom(
          this.http.put(`${this.baseUrl}/${producto.id.value}`, dto)
        );
      } else {
        // Crear
        await firstValueFrom(
          this.http.post(this.baseUrl, dto)
        );
      }
    } catch (error) {
      console.error('Error al guardar producto:', error);
      throw new Error('No se pudo guardar el producto');
    }
  }

  async delete(id: ProductoId): Promise<void> {
    try {
      await firstValueFrom(
        this.http.delete(`${this.baseUrl}/${id.value}`)
      );
    } catch (error) {
      console.error('Error al eliminar producto:', error);
      throw new Error('No se pudo eliminar el producto');
    }
  }

  async exists(id: ProductoId): Promise<boolean> {
    try {
      await firstValueFrom(
        this.http.head(`${this.baseUrl}/${id.value}`)
      );
      return true;
    } catch (error) {
      return false;
    }
  }
}
