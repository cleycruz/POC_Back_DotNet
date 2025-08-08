/**
 * Interfaz base para Command Handlers
 */
export interface CommandHandler<TCommand, TResult = void> {
  handle(command: TCommand): Promise<TResult>;
}

/**
 * Interfaz base para Query Handlers
 */
export interface QueryHandler<TQuery, TResult> {
  handle(query: TQuery): Promise<TResult>;
}
