import { Carrito } from '../entities/carrito.entity';
import { CarritoId } from '../value-objects/carrito-id.vo';

/**
 * Interfaz del repositorio de carrito (definida en el dominio)
 */
export interface ICarritoRepository {
  /**
   * Obtiene un carrito por su ID
   */
  findById(id: CarritoId): Promise<Carrito | null>;

  /**
   * Obtiene el carrito activo del usuario
   */
  findActive(): Promise<Carrito | null>;

  /**
   * Guarda un carrito (crear o actualizar)
   */
  save(carrito: Carrito): Promise<void>;

  /**
   * Elimina un carrito
   */
  delete(id: CarritoId): Promise<void>;

  /**
   * Crea un nuevo carrito
   */
  create(): Promise<Carrito>;

  /**
   * Verifica si existe un carrito
   */
  exists(id: CarritoId): Promise<boolean>;
}
