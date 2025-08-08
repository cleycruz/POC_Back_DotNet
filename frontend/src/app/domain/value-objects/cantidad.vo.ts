import { ValueObject } from './value-object.base';

/**
 * Value Object para representar la cantidad de un item
 */
export class Cantidad extends ValueObject<number> {
  constructor(value: number) {
    super(value);
    this.validate(value);
  }

  protected validate(value: number): void {
    if (!Number.isInteger(value) || value <= 0) {
      throw new Error('La cantidad debe ser un número entero positivo');
    }
  }

  public static create(value: number): Cantidad {
    return new Cantidad(value);
  }

  public static one(): Cantidad {
    return new Cantidad(1);
  }

  public increase(): Cantidad {
    return new Cantidad(this._value + 1);
  }

  public decrease(): Cantidad {
    if (this._value <= 1) {
      throw new Error('La cantidad no puede ser menor a 1');
    }
    return new Cantidad(this._value - 1);
  }

  public isGreaterThan(other: Cantidad): boolean {
    return this._value > other._value;
  }

  public isEqual(other: Cantidad): boolean {
    return this._value === other._value;
  }

  public override toString(): string {
    return this._value.toString();
  }
}
