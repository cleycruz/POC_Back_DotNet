import { ValueObject } from './value-object.base';

/**
 * Value Object para el ID del carrito
 */
export class CarritoId extends ValueObject<number> {
  constructor(value: number) {
    super(value);
    this.validate(value);
  }

  protected validate(value: number): void {
    if (!Number.isInteger(value) || value <= 0) {
      throw new Error('CarritoId debe ser un número entero positivo');
    }
  }

  public static create(value: number): CarritoId {
    return new CarritoId(value);
  }

  public static generate(): CarritoId {
    const randomId = Math.floor(Math.random() * 1000000) + 1;
    return new CarritoId(randomId);
  }

  public override toString(): string {
    return this._value.toString();
  }
}
