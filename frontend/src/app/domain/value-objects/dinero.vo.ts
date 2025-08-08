import { ValueObject } from './value-object.base';

/**
 * Value Object para representar dinero/precio
 */
export class Dinero extends ValueObject<number> {
  constructor(value: number) {
    super(value);
    this.validate(value);
  }

  protected validate(value: number): void {
    if (!Number.isFinite(value) || value < 0) {
      throw new Error('El dinero debe ser un número positivo');
    }
  }

  public static create(value: number): Dinero {
    return new Dinero(value);
  }

  public static zero(): Dinero {
    return new Dinero(0);
  }

  public add(other: Dinero): Dinero {
    return new Dinero(this._value + other._value);
  }

  public subtract(other: Dinero): Dinero {
    const result = this._value - other._value;
    if (result < 0) {
      throw new Error('El resultado no puede ser negativo');
    }
    return new Dinero(result);
  }

  public multiply(factor: number): Dinero {
    if (factor < 0) {
      throw new Error('El factor debe ser positivo');
    }
    return new Dinero(this._value * factor);
  }

  public isGreaterThan(other: Dinero): boolean {
    return this._value > other._value;
  }

  public isLessThan(other: Dinero): boolean {
    return this._value < other._value;
  }

  public isZero(): boolean {
    return this._value === 0;
  }

  public toFormattedString(currency: string = '$'): string {
    return `${currency}${this._value.toFixed(2)}`;
  }

  public override toString(): string {
    return this._value.toString();
  }
}
