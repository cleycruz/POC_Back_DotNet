import { ValueObject } from './value-object.base';

/**
 * Value Object para representar el stock de un producto
 */
export class Stock extends ValueObject<number> {
  constructor(value: number) {
    super(value);
    this.validate(value);
  }

  protected validate(value: number): void {
    if (!Number.isInteger(value) || value < 0) {
      throw new Error('El stock debe ser un número entero no negativo');
    }
  }

  public static create(value: number): Stock {
    return new Stock(value);
  }

  public static empty(): Stock {
    return new Stock(0);
  }

  public isEmpty(): boolean {
    return this._value === 0;
  }

  public hasStock(): boolean {
    return this._value > 0;
  }

  public isGreaterThan(quantity: number): boolean {
    return this._value > quantity;
  }

  public isGreaterOrEqualThan(quantity: number): boolean {
    return this._value >= quantity;
  }

  public canReduce(quantity: number): boolean {
    return this._value >= quantity && quantity > 0;
  }

  public reduce(quantity: number): Stock {
    if (!this.canReduce(quantity)) {
      throw new Error('No se puede reducir el stock: cantidad inválida o insuficiente');
    }
    return new Stock(this._value - quantity);
  }

  public increase(quantity: number): Stock {
    if (quantity <= 0) {
      throw new Error('La cantidad debe ser positiva');
    }
    return new Stock(this._value + quantity);
  }

  public override toString(): string {
    return this._value.toString();
  }
}
