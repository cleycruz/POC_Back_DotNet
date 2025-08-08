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
  ActualizarCantidadDto,
  CarritoItemDto
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
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/productos`);
  }

  getProducto(id: number): Observable<ProductoDto> {
    return this.http.get<ProductoDto>(`${this.baseUrl}/productos/${id}`);
  }

  getProductosByCategoria(categoria: string): Observable<ProductoDto[]> {
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/productos/categoria/${categoria}`);
  }

  createProducto(producto: CrearProductoDto): Observable<ProductoDto> {
    return this.http.post<ProductoDto>(`${this.baseUrl}/productos`, producto, this.httpOptions);
  }

  updateProducto(id: number, producto: ActualizarProductoDto): Observable<ProductoDto> {
    return this.http.put<ProductoDto>(`${this.baseUrl}/productos/${id}`, producto, this.httpOptions);
  }

  deleteProducto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/productos/${id}`);
  }

  // Carrito
  getCarrito(usuarioId: string): Observable<CarritoDto> {
    return this.http.get<CarritoDto>(`${this.baseUrl}/carrito/${usuarioId}`).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  getCarritoTotal(usuarioId: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/carrito/${usuarioId}/total`);
  }

  getCarritoResumen(usuarioId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/carrito/${usuarioId}/resumen`);
  }

  getCarritoItems(usuarioId: string): Observable<CarritoItemDto[]> {
    return this.http.get<CarritoItemDto[]>(`${this.baseUrl}/carrito/${usuarioId}/items`);
  }

  addToCarrito(usuarioId: string, request: AgregarItemCarritoDto): Observable<CarritoDto> {
    return this.http.post<CarritoDto>(`${this.baseUrl}/carrito/${usuarioId}/items`, request, this.httpOptions).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  updateCarritoItem(usuarioId: string, productoId: number, request: ActualizarCantidadDto): Observable<CarritoDto> {
    return this.http.put<CarritoDto>(`${this.baseUrl}/carrito/${usuarioId}/items/${productoId}`, request, this.httpOptions).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  removeFromCarrito(usuarioId: string, productoId: number): Observable<CarritoDto> {
    return this.http.delete<CarritoDto>(`${this.baseUrl}/carrito/${usuarioId}/items/${productoId}`).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  clearCarrito(usuarioId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/carrito/${usuarioId}`).pipe(
      tap(() => this.updateCarritoSubject(null))
    );
  }

  // Método para simular usuario actual (en una app real esto vendría de autenticación)
  getCurrentUserId(): string {
    return "usuario1"; // Usuario por defecto para esta demo
  }
}
