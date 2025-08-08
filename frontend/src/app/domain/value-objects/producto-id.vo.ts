import { ValueObject } from './value-object.base';

/**
 * Value Object para el ID de un producto
 */
export class ProductoId extends ValueObject<number> {
  constructor(value: number) {
    super(value);
    this.validate(value);
  }

  protected validate(value: number): void {
    if (!Number.isInteger(value) || value <= 0) {
      throw new Error('ProductoId debe ser un número entero positivo');
    }
  }

  public static create(value: number): ProductoId {
    return new ProductoId(value);
  }

  public static generate(): ProductoId {
    // En un caso real, esto vendría del backend o sería un GUID
    const randomId = Math.floor(Math.random() * 1000000) + 1;
    return new ProductoId(randomId);
  }

  public toString(): string {
    return this._value.toString();
  }
}
