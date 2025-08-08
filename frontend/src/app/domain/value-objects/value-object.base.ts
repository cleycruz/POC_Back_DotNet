/**
 * Clase base para todos los Value Objects
 * Los Value Objects son inmutables y se comparan por valor
 */
export abstract class ValueObject<T> {
  protected readonly _value: T;

  constructor(value: T) {
    this._value = value;
  }

  public get value(): T {
    return this._value;
  }

  public equals(vo: ValueObject<T>): boolean {
    if (!(vo instanceof ValueObject)) {
      return false;
    }

    return JSON.stringify(this._value) === JSON.stringify(vo._value);
  }

  protected abstract validate(value: T): void;

  public static isValueObject(obj: any): obj is ValueObject<any> {
    return obj instanceof ValueObject;
  }
}
