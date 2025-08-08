import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, retry, map } from 'rxjs/operators';
import { ProductoDto, CrearProductoDto, ActualizarProductoDto } from '../../application/dtos/carrito.dto';
import { ConfigService } from '../../core/services/config.service';

/**
 * Servicio HTTP para productos
 * Maneja la comunicación con la API de productos
 */
@Injectable({
  providedIn: 'root'
})
export class ProductosHttpService {
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(
    private readonly http: HttpClient,
    private readonly configService: ConfigService
  ) {}

  /**
   * Obtiene todos los productos
   */
  public getProductos(): Observable<ProductoDto[]> {
    const url = this.configService.getApiUrl('productos');
    return this.http.get<ProductoDto[]>(url)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Obtiene un producto por ID
   */
  public getProducto(id: number): Observable<ProductoDto> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.get<ProductoDto>(url)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Busca productos por nombre
   */
  public buscarProductos(nombre: string): Observable<ProductoDto[]> {
    const url = this.configService.getApiUrl('productos/buscar');
    return this.http.get<ProductoDto[]>(url, {
      params: { nombre }
    }).pipe(
      retry(2),
      catchError(this.handleError)
    );
  }

  /**
   * Obtiene productos por categoría
   */
  public getProductosPorCategoria(categoria: string): Observable<ProductoDto[]> {
    const url = this.configService.getApiUrl(`productos/categoria/${categoria}`);
    return this.http.get<ProductoDto[]>(url)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Obtiene productos disponibles
   */
  public getProductosDisponibles(): Observable<ProductoDto[]> {
    const url = this.configService.getApiUrl('productos/disponibles');
    return this.http.get<ProductoDto[]>(url)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Crea un nuevo producto
   */
  public crearProducto(producto: CrearProductoDto): Observable<ProductoDto> {
    const url = this.configService.getApiUrl('productos');
    return this.http.post<ProductoDto>(url, producto, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Actualiza un producto existente
   */
  public actualizarProducto(id: number, producto: ActualizarProductoDto): Observable<ProductoDto> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.put<ProductoDto>(url, producto, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Elimina un producto
   */
  public eliminarProducto(id: number): Observable<void> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.delete<void>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Verifica si un producto existe
   */
  public existeProducto(id: number): Observable<boolean> {
    const url = this.configService.getApiUrl(`productos/${id}`);
    return this.http.get<ProductoDto>(url)
      .pipe(
        map(() => true), // Si la petición es exitosa, el producto existe
        catchError(() => of(false)) // Si hay error, el producto no existe
      );
  }

  /**
   * Maneja errores HTTP
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Error desconocido';
    
    if (error.error instanceof ErrorEvent) {
      // Error del lado del cliente
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Error del lado del servidor
      errorMessage = `Código de error: ${error.status}\nMensaje: ${error.message}`;
    }
    
    console.error('Error en ProductosHttpService:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
