import { Injectable } from '@angular/core';
import { Carrito } from '../entities/carrito.entity';
import { Producto } from '../entities/producto.entity';
import { ProductoId } from '../value-objects/producto-id.vo';
import { Cantidad } from '../value-objects/cantidad.vo';

/**
 * Servicio de dominio para operaciones del carrito
 * Contiene lógica de negocio que no pertenece a una entidad específica
 */
@Injectable({
  providedIn: 'root'
})
export class CarritoDomainService {
  
  /**
   * Agrega un producto al carrito verificando reglas de negocio
   */
  public agregarProductoAlCarrito(
    carrito: Carrito,
    producto: Producto,
    cantidad: Cantidad
  ): void {
    // Regla de negocio: verificar disponibilidad
    if (!producto.estaDisponible) {
      throw new Error('El producto no está disponible');
    }

    // Regla de negocio: verificar stock suficiente
    if (!producto.tieneStockSuficiente(cantidad.value)) {
      throw new Error('Stock insuficiente para la cantidad solicitada');
    }

    // Regla de negocio: límite máximo de cantidad por producto
    const cantidadActualEnCarrito = carrito.tieneProducto(producto.id) 
      ? carrito.obtenerItem(producto.id)!.cantidad.value 
      : 0;
    
    const cantidadTotal = cantidadActualEnCarrito + cantidad.value;
    const limiteMaximo = 10; // Regla de negocio: máximo 10 unidades por producto

    if (cantidadTotal > limiteMaximo) {
      throw new Error(`No se puede agregar más de ${limiteMaximo} unidades del mismo producto`);
    }

    // Si todas las validaciones pasan, agregar al carrito
    carrito.agregarItem(
      producto.id,
      producto.nombre,
      cantidad,
      producto.precio
    );
  }

  /**
   * Calcula descuentos aplicables al carrito
   */
  public calcularDescuentos(carrito: Carrito): number {
    // Reglas de negocio para descuentos
    let descuento = 0;

    // Descuento por cantidad de items
    if (carrito.cantidadItems >= 5) {
      descuento += 0.1; // 10% de descuento
    }

    // Descuento por monto total
    if (carrito.total.value >= 1000) {
      descuento += 0.05; // 5% adicional
    }

    return Math.min(descuento, 0.25); // Máximo 25% de descuento
  }

  /**
   * Valida si el carrito puede ser procesado para checkout
   */
  public validarCarritoParaCheckout(carrito: Carrito): { esValido: boolean; errores: string[] } {
    const errores: string[] = [];

    if (carrito.estaVacio) {
      errores.push('El carrito está vacío');
    }

    if (carrito.total.isZero()) {
      errores.push('El total del carrito debe ser mayor a cero');
    }

    // Validar que todos los productos tengan stock
    for (const item of carrito.items) {
      // En un caso real, aquí verificaríamos el stock actual del producto
      // Por ahora asumimos que está validado
    }

    return {
      esValido: errores.length === 0,
      errores
    };
  }

  /**
   * Calcula el peso total del carrito (ejemplo de regla de negocio adicional)
   */
  public calcularPesoTotal(carrito: Carrito): number {
    // En un caso real, los productos tendrían peso
    // Por ahora calculamos un peso estimado
    return carrito.cantidadTotalProductos * 0.5; // 0.5kg por producto
  }
}
