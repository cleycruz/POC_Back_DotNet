namespace CarritoComprasAPI.Core.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para el nombre del producto
    /// </summary>
    public sealed record ProductoNombre
    {
        public string Value { get; }

        private ProductoNombre(string value)
        {
            Value = value;
        }

        public static ProductoNombre Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(valor));
            
            if (valor.Length > 100)
                throw new ArgumentException("El nombre del producto no puede exceder 100 caracteres", nameof(valor));

            return new ProductoNombre(valor.Trim());
        }

        public static implicit operator string(ProductoNombre nombre) => nombre.Value;
        public static explicit operator ProductoNombre(string valor) => Crear(valor);

        public override string ToString() => Value;
    }

    /// <summary>
    /// Value Object para el precio
    /// </summary>
    public sealed record Precio
    {
        public decimal Value { get; }

        private Precio(decimal value)
        {
            Value = value;
        }

        public static Precio Crear(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0", nameof(valor));

            return new Precio(valor);
        }

        public static implicit operator decimal(Precio precio) => precio.Value;
        public static explicit operator Precio(decimal valor) => Crear(valor);

        public override string ToString() => Value.ToString("C");
    }

    /// <summary>
    /// Value Object para el stock
    /// </summary>
    public sealed record Stock
    {
        public int Value { get; }

        private Stock(int value)
        {
            Value = value;
        }

        public static Stock Crear(int valor)
        {
            if (valor < 0)
                throw new ArgumentException("El stock no puede ser negativo", nameof(valor));

            return new Stock(valor);
        }

        public static implicit operator int(Stock stock) => stock.Value;
        public static explicit operator Stock(int valor) => Crear(valor);

        public bool TieneCantidad(int cantidad) => Value >= cantidad;
        public Stock Reducir(int cantidad) => Crear(Value - cantidad);
        public Stock Aumentar(int cantidad) => Crear(Value + cantidad);

        public override string ToString() => Value.ToString();
    }

    /// <summary>
    /// Value Object para la categoría
    /// </summary>
    public sealed record Categoria
    {
        public string Value { get; }

        private Categoria(string value)
        {
            Value = value;
        }

        public static Categoria Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("La categoría no puede estar vacía", nameof(valor));
            
            if (valor.Length > 50)
                throw new ArgumentException("La categoría no puede exceder 50 caracteres", nameof(valor));

            return new Categoria(valor.Trim());
        }

        public static implicit operator string(Categoria categoria) => categoria.Value;
        public static explicit operator Categoria(string valor) => Crear(valor);

        public override string ToString() => Value;
    }

    /// <summary>
    /// Value Object para el ID del usuario
    /// </summary>
    public sealed record UsuarioId
    {
        public string Value { get; }

        private UsuarioId(string value)
        {
            Value = value;
        }

        public static UsuarioId Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El ID de usuario no puede estar vacío", nameof(valor));
            
            if (valor.Length > 100)
                throw new ArgumentException("El ID de usuario no puede exceder 100 caracteres", nameof(valor));

            return new UsuarioId(valor.Trim());
        }

        public static implicit operator string(UsuarioId usuarioId) => usuarioId.Value;
        public static explicit operator UsuarioId(string valor) => Crear(valor);

        public override string ToString() => Value;
    }

    /// <summary>
    /// Value Object para cantidad de productos
    /// </summary>
    public sealed record Cantidad
    {
        public int Value { get; }

        private Cantidad(int value)
        {
            Value = value;
        }

        public static Cantidad Crear(int valor)
        {
            if (valor <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(valor));

            if (valor > 1000)
                throw new ArgumentException("La cantidad no puede exceder 1000 unidades", nameof(valor));

            return new Cantidad(valor);
        }

        public static implicit operator int(Cantidad cantidad) => cantidad.Value;
        public static explicit operator Cantidad(int valor) => Crear(valor);

        public Cantidad Aumentar(int incremento) => Crear(Value + incremento);
        public Cantidad Reducir(int decremento) => Crear(Value - decremento);

        public override string ToString() => Value.ToString();
    }
}
