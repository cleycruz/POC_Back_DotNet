/**
 * Interfaz base para todas las queries
 */
export interface Query {
  readonly queryId: string;
  readonly timestamp: Date;
}

/**
 * Clase base para queries
 */
export abstract class BaseQuery implements Query {
  public readonly queryId: string;
  public readonly timestamp: Date;

  constructor() {
    this.queryId = this.generateId();
    this.timestamp = new Date();
  }

  private generateId(): string {
    return `qry-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
  }
}
