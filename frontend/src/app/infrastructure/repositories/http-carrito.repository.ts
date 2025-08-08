import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { ICarritoRepository } from '../../domain/repositories/carrito.repository.interface';
import { Carrito } from '../../domain/entities/carrito.entity';
import { CarritoId } from '../../domain/value-objects/carrito-id.vo';
import { ConfigService } from '../../core/services/config.service';

/**
 * Implementación HTTP del repositorio de carrito
 */
@Injectable({
  providedIn: 'root'
})
export class HttpCarritoRepository implements ICarritoRepository {
  private carritoActivo: Carrito | null = null;

  constructor(
    private readonly http: HttpClient,
    private readonly configService: ConfigService
  ) {}

  private getUrl(endpoint: string = ''): string {
    return this.configService.getApiUrl(`carrito${endpoint ? '/' + endpoint : ''}`);
  }

  async findById(id: CarritoId): Promise<Carrito | null> {
    try {
      const dto = await firstValueFrom(
        this.http.get<CarritoDto>(this.getUrl(id.value.toString()))
      );
      return this.mapper.toDomain(dto);
    } catch (error) {
      console.error('Error al obtener carrito por ID:', error);
      return null;
    }
  }

  async findActive(): Promise<Carrito | null> {
    try {
      // Si ya tenemos un carrito activo en memoria, devolverlo
      if (this.carritoActivo) {
        return this.carritoActivo;
      }

      // Intentar obtener carrito activo del servidor
      const dto = await firstValueFrom(
        this.http.get<CarritoDto>(this.getUrl('activo'))
      );
      
      this.carritoActivo = this.mapper.toDomain(dto);
      return this.carritoActivo;
    } catch (error) {
      console.error('Error al obtener carrito activo:', error);
      return null;
    }
  }

  async save(carrito: Carrito): Promise<void> {
    try {
      const dto = this.mapper.toDto(carrito);
      
      if (carrito.id.value > 0) {
        // Actualizar carrito existente
        await firstValueFrom(
          this.http.put(this.getUrl(carrito.id.value.toString()), dto)
        );
      } else {
        // Crear nuevo carrito
        const nuevoCarritoDto = await firstValueFrom(
          this.http.post<CarritoDto>(this.getUrl(), dto)
        );
        // Actualizar el carrito activo con el ID del servidor
        this.carritoActivo = this.mapper.toDomain(nuevoCarritoDto);
      }
    } catch (error) {
      console.error('Error al guardar carrito:', error);
      throw new Error('No se pudo guardar el carrito');
    }
  }

  async delete(id: CarritoId): Promise<void> {
    try {
      await firstValueFrom(
        this.http.delete(this.getUrl(id.value.toString()))
      );
      
      // Si eliminamos el carrito activo, limpiarlo de memoria
      if (this.carritoActivo?.id.equals(id)) {
        this.carritoActivo = null;
      }
    } catch (error) {
      console.error('Error al eliminar carrito:', error);
      throw new Error('No se pudo eliminar el carrito');
    }
  }

  async create(): Promise<Carrito> {
    try {
      const nuevoCarrito = Carrito.crear();
      const dto = this.mapper.toDto(nuevoCarrito);
      
      const carritoCreado = await firstValueFrom(
        this.http.post<CarritoDto>(this.getUrl(), dto)
      );
      
      this.carritoActivo = this.mapper.toDomain(carritoCreado);
      return this.carritoActivo!;
    } catch (error) {
      console.error('Error al crear carrito:', error);
      throw new Error('No se pudo crear el carrito');
    }
  }

  async exists(id: CarritoId): Promise<boolean> {
    try {
      await firstValueFrom(
        this.http.head(this.getUrl(id.value.toString()))
      );
      return true;
    } catch (error) {
      console.error('Error al verificar existencia del carrito:', error);
      return false;
    }
  }
}
