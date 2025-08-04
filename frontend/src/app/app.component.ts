import { Component, OnInit, OnDestroy } from '@angular/core';
import { CarritoService } from './services/carrito.service';
import { CarritoDto } from './models/carrito.models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Carrito de Compras';
  carrito: CarritoDto | null = null;
  private carritoSubscription?: Subscription;

  constructor(private readonly carritoService: CarritoService) { }

  ngOnInit(): void {
    // Suscribirse a los cambios del carrito
    this.carritoSubscription = this.carritoService.carrito$.subscribe(carrito => {
      this.carrito = carrito;
    });
    
    // Cargar el carrito inicial
    this.loadCarrito();
  }

  ngOnDestroy(): void {
    if (this.carritoSubscription) {
      this.carritoSubscription.unsubscribe();
    }
  }

  loadCarrito(): void {
    const usuarioId = this.carritoService.getCurrentUserId();
    this.carritoService.getCarrito(usuarioId).subscribe({
      error: (error: any) => {
        console.error('Error al cargar carrito en header:', error);
      }
    });
  }

  getTotalItems(): number {
    if (!this.carrito?.items) {
      return 0;
    }
    return this.carrito.items.reduce((total, item) => total + item.cantidad, 0);
  }
}
