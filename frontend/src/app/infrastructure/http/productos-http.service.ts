import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ProductoDto, CrearProductoDto, ActualizarProductoDto } from '../../application/dtos/carrito.dto';

/**
 * Servicio HTTP para productos
 * Maneja la comunicación con la API de productos
 */
@Injectable({
  providedIn: 'root'
})
export class ProductosHttpService {
  private readonly baseUrl = 'http://localhost:5063/api/productos';
  
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private readonly http: HttpClient) {}

  /**
   * Obtiene todos los productos
   */
  public getProductos(): Observable<ProductoDto[]> {
    return this.http.get<ProductoDto[]>(this.baseUrl)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Obtiene un producto por ID
   */
  public getProducto(id: number): Observable<ProductoDto> {
    return this.http.get<ProductoDto>(`${this.baseUrl}/${id}`)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Busca productos por nombre
   */
  public buscarProductos(nombre: string): Observable<ProductoDto[]> {
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/buscar`, {
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
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/categoria/${categoria}`)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Obtiene productos disponibles
   */
  public getProductosDisponibles(): Observable<ProductoDto[]> {
    return this.http.get<ProductoDto[]>(`${this.baseUrl}/disponibles`)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  /**
   * Crea un nuevo producto
   */
  public crearProducto(producto: CrearProductoDto): Observable<ProductoDto> {
    return this.http.post<ProductoDto>(this.baseUrl, producto, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Actualiza un producto existente
   */
  public actualizarProducto(id: number, producto: ActualizarProductoDto): Observable<ProductoDto> {
    return this.http.put<ProductoDto>(`${this.baseUrl}/${id}`, producto, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Elimina un producto
   */
  public eliminarProducto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Verifica si un producto existe
   */
  public existeProducto(id: number): Observable<boolean> {
    return this.http.head(`${this.baseUrl}/${id}`)
      .pipe(
        catchError(() => throwError(() => false))
      ) as Observable<boolean>;
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
