import { Producto } from '../entities/producto.entity';
import { ProductoId } from '../value-objects/producto-id.vo';

/**
 * Interfaz del repositorio de productos (definida en el dominio)
 * Esta interfaz define el contrato que debe cumplir cualquier implementación
 */
export interface IProductoRepository {
  /**
   * Obtiene un producto por su ID
   */
  findById(id: ProductoId): Promise<Producto | null>;

  /**
   * Obtiene todos los productos
   */
  findAll(): Promise<Producto[]>;

  /**
   * Busca productos por nombre
   */
  findByNombre(nombre: string): Promise<Producto[]>;

  /**
   * Busca productos por categoría
   */
  findByCategoria(categoria: string): Promise<Producto[]>;

  /**
   * Busca productos disponibles (con stock)
   */
  findDisponibles(): Promise<Producto[]>;

  /**
   * Guarda un producto (crear o actualizar)
   */
  save(producto: Producto): Promise<void>;

  /**
   * Elimina un producto
   */
  delete(id: ProductoId): Promise<void>;

  /**
   * Verifica si existe un producto
   */
  exists(id: ProductoId): Promise<boolean>;
}
