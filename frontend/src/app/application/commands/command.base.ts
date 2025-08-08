/**
 * Interfaz base para todos los commands
 */
export interface Command {
  readonly commandId: string;
  readonly timestamp: Date;
}

/**
 * Clase base para commands
 */
export abstract class BaseCommand implements Command {
  public readonly commandId: string;
  public readonly timestamp: Date;

  constructor() {
    this.commandId = this.generateId();
    this.timestamp = new Date();
  }

  private generateId(): string {
    return `cmd-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
  }
}
