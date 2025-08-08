/**
 * Interfaz base para todos los casos de uso
 */
export interface UseCase<TRequest, TResponse> {
  execute(request: TRequest): Promise<TResponse>;
}

/**
 * Interfaz para casos de uso sin respuesta
 */
export interface UseCaseVoid<TRequest> {
  execute(request: TRequest): Promise<void>;
}
