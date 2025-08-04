import { Component, OnInit } from '@angular/core';
import { CarritoService } from '../../services/carrito.service';
import { DialogService } from '../../services/dialog.service';
import { CarritoDto, CarritoItemDto } from '../../models/carrito.models';

@Component({
  selector: 'app-carrito',
  templateUrl: './carrito.component.html',
  styleUrls: ['./carrito.component.css']
})
export class CarritoComponent implements OnInit {
  carrito: CarritoDto | null = null;
  loading = false;

  constructor(
    private readonly carritoService: CarritoService,
    private readonly dialogService: DialogService
  ) { }

  ngOnInit(): void {
    this.loadCarrito();
  }

  loadCarrito(): void {
    this.loading = true;
    const usuarioId = this.carritoService.getCurrentUserId();
    
    this.carritoService.getCarrito(usuarioId).subscribe({
      next: (carrito: CarritoDto) => {
        this.carrito = carrito;
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error al cargar carrito:', error);
        this.loading = false;
      }
    });
  }

  updateQuantity(item: CarritoItemDto, newQuantity: number): void {
    if (newQuantity <= 0) {
      this.removeItem(item);
      return;
    }

    const usuarioId = this.carritoService.getCurrentUserId();
    this.carritoService.updateCarritoItem(usuarioId, item.id, {
      cantidad: newQuantity
    }).subscribe({
      next: (updatedCarrito: CarritoDto) => {
        this.carrito = updatedCarrito;
      },
      error: (error: any) => {
        console.error('Error al actualizar cantidad:', error);
        this.dialogService.error('No se pudo actualizar la cantidad del producto. Por favor, inténtalo nuevamente.');
      }
    });
  }

  removeItem(item: CarritoItemDto): void {
    this.dialogService.confirm(
      'Eliminar producto',
      `¿Estás seguro de que quieres eliminar "${item.nombreProducto}" del carrito?`,
      'Eliminar',
      'Cancelar'
    ).subscribe(confirmed => {
      if (confirmed) {
        const usuarioId = this.carritoService.getCurrentUserId();
        this.carritoService.removeFromCarrito(usuarioId, item.id).subscribe({
          next: () => {
            this.loadCarrito(); // Recargar el carrito después de eliminar
          },
          error: (error: any) => {
            console.error('Error al eliminar item:', error);
            this.dialogService.error('No se pudo eliminar el producto del carrito. Por favor, inténtalo nuevamente.');
          }
        });
      }
    });
  }

  clearCart(): void {
    this.dialogService.confirm(
      'Vaciar carrito',
      '¿Estás seguro de que quieres eliminar todos los productos del carrito? Esta acción no se puede deshacer.',
      'Vaciar carrito',
      'Cancelar'
    ).subscribe(confirmed => {
      if (confirmed) {
        const usuarioId = this.carritoService.getCurrentUserId();
        this.carritoService.clearCarrito(usuarioId).subscribe({
          next: () => {
            this.loadCarrito(); // Recargar el carrito
            this.dialogService.success('Carrito vaciado exitosamente');
          },
          error: (error: any) => {
            console.error('Error al vaciar carrito:', error);
            this.dialogService.error('No se pudo vaciar el carrito. Por favor, inténtalo nuevamente.');
          }
        });
      }
    });
  }

  getTotalItems(): number {
    if (!this.carrito?.items) {
      return 0;
    }
    return this.carrito.items.reduce((total, item) => total + item.cantidad, 0);
  }

  checkout(): void {
    if (this.carrito?.items?.length && this.carrito.items.length > 0) {
      this.dialogService.success(
        `¡Gracias por tu compra! Total: $${this.carrito?.total?.toFixed(2) || '0.00'}`,
        '¡Compra realizada!'
      ).subscribe(() => {
        this.clearCart();
      });
    } else {
      this.dialogService.info('Tu carrito está vacío. Agrega algunos productos antes de realizar la compra.');
    }
  }

  continueShopping(): void {
    // Navegamos de vuelta a la página de productos
    window.location.href = '/productos';
  }

  startShopping(): void {
    this.continueShopping();
  }

  trackByItemId(index: number, item: CarritoItemDto): number {
    return item.id;
  }
}
