import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ObtenerProductosHandler } from '../../../application/handlers/obtener-productos.handler';
import { AgregarProductoAlCarritoHandler } from '../../../application/handlers/agregar-producto-carrito.handler';
import { ObtenerProductosQuery } from '../../../application/queries/obtener-productos.query';
import { AgregarProductoAlCarritoCommand } from '../../../application/commands/agregar-producto-carrito.command';
import { Producto } from '../../../domain/entities/producto.entity';

/**
 * Componente para mostrar y gestionar productos
 */
@Component({
  selector: 'app-productos',
  template: `
    <div class="productos-container">
      <h2>Productos Disponibles</h2>
      
      <!-- Filtros -->
      <div class="filtros">
        <input 
          type="text" 
          [(ngModel)]="busqueda" 
          (input)="filtrarProductos()"
          placeholder="Buscar productos...">
        
        <select [(ngModel)]="categoriaSeleccionada" (change)="filtrarProductos()">
          <option value="">Todas las categorías</option>
          <option *ngFor="let categoria of categorias" [value]="categoria">
            {{categoria}}
          </option>
        </select>
        
        <label>
          <input 
            type="checkbox" 
            [(ngModel)]="soloDisponibles" 
            (change)="filtrarProductos()">
          Solo productos disponibles
        </label>
      </div>

      <!-- Lista de productos -->
      <div class="productos-grid" *ngIf="!loading; else loadingTemplate">
        <div 
          class="producto-card" 
          *ngFor="let producto of productos"
          [class.no-disponible]="!producto.estaDisponible">
          
          <h3>{{producto.nombre}}</h3>
          <p class="descripcion">{{producto.descripcion}}</p>
          <p class="precio">{{producto.precio.toFormattedString()}}</p>
          <p class="stock">Stock: {{producto.stock.value}}</p>
          <p class="categoria" *ngIf="producto.categoria">{{producto.categoria}}</p>
          
          <div class="acciones">
            <input 
              type="number" 
              [(ngModel)]="cantidades[producto.id.value]" 
              min="1" 
              [max]="producto.stock.value"
              [disabled]="!producto.estaDisponible">
            
            <button 
              (click)="agregarAlCarrito(producto)"
              [disabled]="!producto.estaDisponible || loading"
              class="btn btn-primary">
              Agregar al Carrito
            </button>
          </div>
        </div>
      </div>

      <ng-template #loadingTemplate>
        <div class="loading">Cargando productos...</div>
      </ng-template>

      <!-- Mensaje de error -->
      <div class="error" *ngIf="error">
        {{error}}
      </div>
    </div>
  `,
  styles: [`
    .productos-container {
      padding: 20px;
    }

    .filtros {
      margin-bottom: 20px;
      display: flex;
      gap: 15px;
      align-items: center;
      flex-wrap: wrap;
    }

    .productos-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
    }

    .producto-card {
      border: 1px solid #ddd;
      border-radius: 8px;
      padding: 15px;
      background: white;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .producto-card.no-disponible {
      opacity: 0.6;
      background: #f5f5f5;
    }

    .precio {
      font-size: 1.2em;
      font-weight: bold;
      color: #27ae60;
    }

    .acciones {
      display: flex;
      gap: 10px;
      align-items: center;
      margin-top: 10px;
    }

    .btn {
      padding: 8px 16px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }

    .btn-primary {
      background: #3498db;
      color: white;
    }

    .btn:disabled {
      background: #bdc3c7;
      cursor: not-allowed;
    }

    .loading, .error {
      text-align: center;
      padding: 20px;
    }

    .error {
      color: #e74c3c;
    }
  `]
})
export class ProductosComponent implements OnInit, OnDestroy {
  public productos: Producto[] = [];
  public categorias: string[] = [];
  public loading = false;
  public error: string | null = null;
  
  // Filtros
  public busqueda = '';
  public categoriaSeleccionada = '';
  public soloDisponibles = false;
  
  // Cantidades para agregar al carrito
  public cantidades: { [key: number]: number } = {};
  
  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly obtenerProductosHandler: ObtenerProductosHandler,
    private readonly agregarProductoHandler: AgregarProductoAlCarritoHandler
  ) {}

  ngOnInit(): void {
    this.cargarProductos();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public async cargarProductos(): Promise<void> {
    try {
      this.loading = true;
      this.error = null;

      const query = new ObtenerProductosQuery(
        this.categoriaSeleccionada || undefined,
        this.soloDisponibles,
        this.busqueda || undefined
      );

      const response = await this.obtenerProductosHandler.handle(query);
      this.productos = response.productos;
      
      // Extraer categorías únicas
      this.categorias = [...new Set(
        this.productos
          .map(p => p.categoria)
          .filter(c => c != null)
      )] as string[];

      // Inicializar cantidades
      this.productos.forEach(producto => {
        this.cantidades[producto.id.value] = 1;
      });

    } catch (error) {
      this.error = 'Error al cargar productos';
      console.error('Error:', error);
    } finally {
      this.loading = false;
    }
  }

  public filtrarProductos(): void {
    this.cargarProductos();
  }

  public async agregarAlCarrito(producto: Producto): Promise<void> {
    try {
      const cantidad = this.cantidades[producto.id.value] || 1;
      
      const command = new AgregarProductoAlCarritoCommand(
        producto.id.value,
        cantidad
      );

      await this.agregarProductoHandler.handle(command);
      
      // Mostrar feedback al usuario
      alert(`Producto "${producto.nombre}" agregado al carrito`);
      
    } catch (error) {
      alert('Error al agregar producto al carrito');
      console.error('Error:', error);
    }
  }
}
