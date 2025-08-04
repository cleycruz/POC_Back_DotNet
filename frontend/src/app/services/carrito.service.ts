import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { 
  ProductoDto, 
  CarritoDto, 
  CrearProductoDto, 
  ActualizarProductoDto, 
  AgregarItemCarritoDto, 
  ActualizarItemCarritoDto 
} from '../models/carrito.models';

@Injectable({
  providedIn: 'root'
})
export class CarritoService {
  private readonly baseUrl = 'http://localhost:5063/api';
  
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  // Subject para notificar cambios en el carrito
  private readonly carritoSubject = new BehaviorSubject<CarritoDto | null>(null);
  public carrito$ = this.carritoSubject.asObservable();

  constructor(private readonly http: HttpClient) { }

  // Método privado para actualizar el subject del carrito
  private updateCarritoSubject(carrito: CarritoDto | null): void {
    this.carritoSubject.next(carrito);
  }

  // Productos
  getProductos(): Observable<ProductoDto[]> {
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/Productos`);
  }

  getProducto(id: number): Observable<ProductoDto> {
    return this.http.get<ProductoDto>(`${this.baseUrl}/Productos/${id}`);
  }

  getProductosByCategoria(categoria: string): Observable<ProductoDto[]> {
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/Productos/categoria/${categoria}`);
  }

  createProducto(producto: CrearProductoDto): Observable<ProductoDto> {
    return this.http.post<ProductoDto>(`${this.baseUrl}/Productos`, producto, this.httpOptions);
  }

  updateProducto(id: number, producto: ActualizarProductoDto): Observable<ProductoDto> {
    return this.http.put<ProductoDto>(`${this.baseUrl}/Productos/${id}`, producto, this.httpOptions);
  }

  deleteProducto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Productos/${id}`);
  }

  // Carrito
  getCarrito(usuarioId: string): Observable<CarritoDto> {
    return this.http.get<CarritoDto>(`${this.baseUrl}/Carrito/${usuarioId}`).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  addToCarrito(usuarioId: string, request: AgregarItemCarritoDto): Observable<CarritoDto> {
    return this.http.post<CarritoDto>(`${this.baseUrl}/Carrito/${usuarioId}/items`, request, this.httpOptions).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  updateCarritoItem(usuarioId: string, itemId: number, request: ActualizarItemCarritoDto): Observable<CarritoDto> {
    return this.http.put<CarritoDto>(`${this.baseUrl}/Carrito/${usuarioId}/items/${itemId}`, request, this.httpOptions).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  removeFromCarrito(usuarioId: string, itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Carrito/${usuarioId}/items/${itemId}`).pipe(
      tap(() => {
        // Después de eliminar, recargar el carrito
        this.getCarrito(usuarioId).subscribe();
      })
    );
  }

  clearCarrito(usuarioId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Carrito/${usuarioId}`).pipe(
      tap(() => this.updateCarritoSubject(null))
    );
  }

  getCarritoTotal(usuarioId: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Carrito/${usuarioId}/total`);
  }

  // Método para simular usuario actual (en una app real esto vendría de autenticación)
  getCurrentUserId(): string {
    return "usuario1"; // Usuario por defecto para esta demo
  }
}
