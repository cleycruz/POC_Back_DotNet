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
import { ConfigService } from '../core/services/config.service';

@Injectable({
  providedIn: 'root'
})
export class CarritoService {
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  // Subject para notificar cambios en el carrito
  private readonly carritoSubject = new BehaviorSubject<CarritoDto | null>(null);
  public carrito$ = this.carritoSubject.asObservable();

  constructor(
    private readonly http: HttpClient,
    private readonly configService: ConfigService
  ) { }

  // Método privado para actualizar el subject del carrito
  private updateCarritoSubject(carrito: CarritoDto | null): void {
    this.carritoSubject.next(carrito);
  }

  // Productos
  getProductos(): Observable<ProductoDto[]> {
    const url = this.configService.getApiUrl('productos');
    return this.http.get<ProductoDto[]>(url);
  }

  getProducto(id: number): Observable<ProductoDto> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.get<ProductoDto>(url);
  }

  getProductosByCategoria(categoria: string): Observable<ProductoDto[]> {
    const url = this.configService.getApiUrl(`productos/categoria/${categoria}`);
    return this.http.get<ProductoDto[]>(url);
  }

  createProducto(producto: CrearProductoDto): Observable<ProductoDto> {
    const url = this.configService.getApiUrl('productos');
    return this.http.post<ProductoDto>(url, producto, this.httpOptions);
  }

  updateProducto(id: number, producto: ActualizarProductoDto): Observable<ProductoDto> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.put<ProductoDto>(url, producto, this.httpOptions);
  }

  deleteProducto(id: number): Observable<void> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.delete<void>(url);
  }

  // Carrito
  getCarrito(usuarioId: string): Observable<CarritoDto> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}`);
    return this.http.get<CarritoDto>(url).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  getCarritoTotal(usuarioId: string): Observable<number> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}/total`);
    return this.http.get<number>(url);
  }

  getCarritoResumen(usuarioId: string): Observable<any> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}/resumen`);
    return this.http.get<any>(url);
  }

  getCarritoItems(usuarioId: string): Observable<CarritoItemDto[]> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}/items`);
    return this.http.get<CarritoItemDto[]>(url);
  }

  addToCarrito(usuarioId: string, request: AgregarItemCarritoDto): Observable<CarritoDto> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}/items`);
    return this.http.post<CarritoDto>(url, request, this.httpOptions).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  updateCarritoItem(usuarioId: string, productoId: number, request: ActualizarCantidadDto): Observable<CarritoDto> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}/items/${productoId}`);
    return this.http.put<CarritoDto>(url, request, this.httpOptions).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  removeFromCarrito(usuarioId: string, productoId: number): Observable<CarritoDto> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}/items/${productoId}`);
    return this.http.delete<CarritoDto>(url).pipe(
      tap(carrito => this.updateCarritoSubject(carrito))
    );
  }

  clearCarrito(usuarioId: string): Observable<void> {
    const url = this.configService.getApiUrl(`carrito/${usuarioId}`);
    return this.http.delete<void>(url).pipe(
      tap(() => this.updateCarritoSubject(null))
    );
  }

  // Método para simular usuario actual (en una app real esto vendría de autenticación)
  getCurrentUserId(): string {
    return "usuario1"; // Usuario por defecto para esta demo
  }
}
