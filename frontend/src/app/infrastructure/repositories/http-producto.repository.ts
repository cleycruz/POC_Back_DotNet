import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { IProductoRepository } from '../../domain/repositories/producto.repository.interface';
import { Producto } from '../../domain/entities/producto.entity';
import { ProductoId } from '../../domain/value-objects/producto-id.vo';
import { Dinero } from '../../domain/value-objects/dinero.vo';
import { Stock } from '../../domain/value-objects/stock.vo';
import { ConfigService } from '../../core/services/config.service';

/**
 * Implementación HTTP del repositorio de productos
 */
@Injectable({
  providedIn: 'root'
})
export class HttpProductoRepository implements IProductoRepository {

  constructor(
    private readonly http: HttpClient,
    private readonly configService: ConfigService
  ) {}

  private getUrl(endpoint: string = ''): string {
    return this.configService.getApiUrl(`productos${endpoint ? '/' + endpoint : ''}`);
  }

  private mapFromDto(dto: any): Producto {
    return new Producto(
      new ProductoId(dto.id),
      {
        nombre: dto.nombre,
        descripcion: dto.descripcion,
        precio: Dinero.create(dto.precio),
        stock: Stock.create(dto.stock),
        categoria: dto.categoria,
        fechaCreacion: new Date(dto.fechaCreacion || new Date()),
        fechaActualizacion: dto.fechaActualizacion ? new Date(dto.fechaActualizacion) : undefined
      }
    );
  }

  async findById(id: ProductoId): Promise<Producto | null> {
    try {
      const response = await firstValueFrom(
        this.http.get<any>(this.getUrl(id.value.toString()))
      );
      return this.mapFromDto(response);
    } catch (error) {
      console.error('Error al obtener producto por ID:', error);
      return null;
    }
  }

  async findAll(): Promise<Producto[]> {
    try {
      const response = await firstValueFrom(
        this.http.get<any[]>(this.getUrl())
      );
      return response.map(dto => this.mapFromDto(dto));
    } catch (error) {
      console.error('Error al obtener productos:', error);
      return [];
    }
  }

  async findByNombre(nombre: string): Promise<Producto[]> {
    try {
      const response = await firstValueFrom(
        this.http.get<any[]>(this.getUrl(`buscar?nombre=${encodeURIComponent(nombre)}`))
      );
      return response.map(dto => this.mapFromDto(dto));
    } catch (error) {
      console.error('Error al buscar productos por nombre:', error);
      return [];
    }
  }

  async findByCategoria(categoria: string): Promise<Producto[]> {
    try {
      const response = await firstValueFrom(
        this.http.get<any[]>(this.getUrl(`categoria/${categoria}`))
      );
      return response.map(dto => this.mapFromDto(dto));
    } catch (error) {
      console.error('Error al buscar productos por categoría:', error);
      return [];
    }
  }

  async findDisponibles(): Promise<Producto[]> {
    try {
      const response = await firstValueFrom(
        this.http.get<any[]>(this.getUrl('disponibles'))
      );
      return response.map(dto => this.mapFromDto(dto));
    } catch (error) {
      console.error('Error al obtener productos disponibles:', error);
      return [];
    }
  }

  async save(producto: Producto): Promise<void> {
    try {
      const dto = {
        id: producto.id.value,
        nombre: producto.nombre,
        precio: producto.precio.value,
        descripcion: producto.descripcion,
        categoria: producto.categoria,
        stock: producto.stock.value,
        fechaCreacion: producto.fechaCreacion,
        fechaActualizacion: producto.fechaActualizacion
      };

      if (producto.id.value > 0) {
        await firstValueFrom(
          this.http.put(this.getUrl(producto.id.value.toString()), dto)
        );
      } else {
        await firstValueFrom(
          this.http.post(this.getUrl(), dto)
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
        this.http.delete(this.getUrl(id.value.toString()))
      );
    } catch (error) {
      console.error('Error al eliminar producto:', error);
      throw new Error('No se pudo eliminar el producto');
    }
  }

  async exists(id: ProductoId): Promise<boolean> {
    try {
      await firstValueFrom(
        this.http.head(this.getUrl(id.value.toString()))
      );
      return true;
    } catch (error) {
      console.error('Error al verificar existencia de producto:', error);
      return false;
    }
  }
}
