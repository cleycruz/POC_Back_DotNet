/**
 * Clase base para todas las entidades del dominio
 * Proporciona funcionalidad común como identidad y equality
 */
export abstract class Entity<T> {
  protected readonly _id: T;

  constructor(id: T) {
    this._id = id;
  }

  public get id(): T {
    return this._id;
  }

  public equals(entity: Entity<T>): boolean {
    if (!(entity instanceof Entity)) {
      return false;
    }

    if (this === entity) {
      return true;
    }

    return this._id === entity._id;
  }

  public static isEntity(obj: any): obj is Entity<any> {
    return obj instanceof Entity;
  }
}
