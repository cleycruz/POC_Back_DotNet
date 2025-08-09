using CarritoComprasAPI.Core.Configuration;
using System.Globalization;

namespace CarritoComprasAPI.Core.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que encapsula el nombre de un producto con validaciones de dominio.
    /// Garantiza que el nombre cumpla con las reglas de negocio establecidas.
    /// </summary>
    /// <remarks>
    /// Este Value Object implementa el patrón DDD para encapsular la lógica de validación
    /// del nombre del producto, evitando primitive obsession y centralizando las reglas.
    /// </remarks>
    public sealed record ProductoNombre
    {
        /// <summary>
        /// Obtiene el valor del nombre del producto validado
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Constructor privado para garantizar que solo se puedan crear instancias
        /// válidas a través del método factory Crear()
        /// </summary>
        /// <param name="value">Valor del nombre ya validado</param>
        private ProductoNombre(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Método factory para crear una instancia de ProductoNombre con validaciones de dominio
        /// </summary>
        /// <param name="valor">Nombre del producto a validar</param>
        /// <returns>Nueva instancia de ProductoNombre validada</returns>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando:
        /// - El valor es nulo, vacío o solo espacios en blanco
        /// - La longitud excede el máximo permitido
        /// - La longitud es menor al mínimo requerido
        /// </exception>
        /// <example>
        /// <code>
        /// var nombre = ProductoNombre.Crear("Laptop Gaming RGB");
        /// Console.WriteLine(nombre.Value); // "Laptop Gaming RGB"
        /// </code>
        /// </example>
        public static ProductoNombre Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException(BusinessConstants.ValidationMessages.PRODUCTO_NOMBRE_REQUERIDO, nameof(valor));
            
            var nombreTrimmed = valor.Trim();
            
            if (nombreTrimmed.Length < BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MINIMA)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.PRODUCTO_NOMBRE_LONGITUD_INVALIDA, 
                        BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MINIMA, 
                        BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA), 
                    nameof(valor));
            
            if (nombreTrimmed.Length > BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.PRODUCTO_NOMBRE_LONGITUD_INVALIDA, 
                        BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MINIMA, 
                        BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA), 
                    nameof(valor));

            return new ProductoNombre(nombreTrimmed);
        }

        /// <summary>
        /// Conversión implícita de ProductoNombre a string
        /// </summary>
        /// <param name="nombre">Instancia de ProductoNombre</param>
        public static implicit operator string(ProductoNombre nombre) => nombre.Value;
        
        /// <summary>
        /// Conversión explícita de string a ProductoNombre con validaciones
        /// </summary>
        /// <param name="valor">Valor string a convertir</param>
        public static explicit operator ProductoNombre(string valor) => Crear(valor);

        /// <summary>
        /// Representación en string del nombre del producto
        /// </summary>
        /// <returns>Valor del nombre</returns>
        public override string ToString() => Value;
    }

    /// <summary>
    /// Value Object que encapsula el precio de un producto con validaciones de dominio.
    /// Garantiza que el precio sea válido según las reglas de negocio establecidas.
    /// </summary>
    /// <remarks>
    /// Implementa validaciones para evitar precios negativos o cero, y establece
    /// límites máximos para prevenir errores de entrada de datos.
    /// </remarks>
    public sealed record Precio
    {
        /// <summary>
        /// Obtiene el valor del precio validado
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// Constructor privado para garantizar creación solo a través del método factory
        /// </summary>
        /// <param name="value">Valor del precio ya validado</param>
        private Precio(decimal value)
        {
            Value = value;
        }

        /// <summary>
        /// Método factory para crear una instancia de Precio con validaciones de dominio
        /// </summary>
        /// <param name="valor">Precio a validar</param>
        /// <returns>Nueva instancia de Precio validada</returns>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando el precio es menor o igual a cero, o excede el máximo permitido
        /// </exception>
        /// <example>
        /// <code>
        /// var precio = Precio.Crear(299.99m);
        /// Console.WriteLine(precio.Value); // 299.99
        /// </code>
        /// </example>
        public static Precio Crear(decimal valor)
        {
            if (valor < BusinessConstants.PRODUCTO_PRECIO_MINIMO)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.PRODUCTO_PRECIO_INVALIDO, 
                        BusinessConstants.PRODUCTO_PRECIO_MINIMO, 
                        BusinessConstants.PRODUCTO_PRECIO_MAXIMO), 
                    nameof(valor));
                    
            if (valor > BusinessConstants.PRODUCTO_PRECIO_MAXIMO)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.PRODUCTO_PRECIO_INVALIDO, 
                        BusinessConstants.PRODUCTO_PRECIO_MINIMO, 
                        BusinessConstants.PRODUCTO_PRECIO_MAXIMO), 
                    nameof(valor));

            return new Precio(valor);
        }

        /// <summary>
        /// Conversión implícita de Precio a decimal
        /// </summary>
        /// <param name="precio">Instancia de Precio</param>
        public static implicit operator decimal(Precio precio) => precio.Value;
        
        /// <summary>
        /// Conversión explícita de decimal a Precio con validaciones
        /// </summary>
        /// <param name="valor">Valor decimal a convertir</param>
        public static explicit operator Precio(decimal valor) => Crear(valor);

        /// <summary>
        /// Representación en string del precio con formato de moneda
        /// </summary>
        /// <returns>Precio formateado como moneda</returns>
        public override string ToString() => Value.ToString("C", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Value Object que encapsula el stock de un producto con operaciones de dominio.
    /// Proporciona métodos para gestionar el inventario de manera segura.
    /// </summary>
    /// <remarks>
    /// Este Value Object incluye operaciones específicas del dominio como verificación
    /// de disponibilidad, reducción y aumento de stock con validaciones.
    /// </remarks>
    public sealed record Stock
    {
        /// <summary>
        /// Obtiene el valor del stock validado
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Constructor privado para garantizar creación solo a través del método factory
        /// </summary>
        /// <param name="value">Valor del stock ya validado</param>
        private Stock(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Método factory para crear una instancia de Stock con validaciones de dominio
        /// </summary>
        /// <param name="valor">Cantidad de stock a validar</param>
        /// <returns>Nueva instancia de Stock validada</returns>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando el stock es negativo o excede el máximo permitido
        /// </exception>
        /// <example>
        /// <code>
        /// var stock = Stock.Crear(100);
        /// if (stock.TieneCantidad(5)) 
        /// {
        ///     var nuevoStock = stock.Reducir(5);
        /// }
        /// </code>
        /// </example>
        public static Stock Crear(int valor)
        {
            if (valor < 0)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.PRODUCTO_STOCK_INVALIDO, 
                        BusinessConstants.PRODUCTO_STOCK_MAXIMO), 
                    nameof(valor));
                    
            if (valor > BusinessConstants.PRODUCTO_STOCK_MAXIMO)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.PRODUCTO_STOCK_INVALIDO, 
                        BusinessConstants.PRODUCTO_STOCK_MAXIMO), 
                    nameof(valor));

            return new Stock(valor);
        }

        /// <summary>
        /// Conversión implícita de Stock a int
        /// </summary>
        /// <param name="stock">Instancia de Stock</param>
        public static implicit operator int(Stock stock) => stock.Value;
        
        /// <summary>
        /// Conversión explícita de int a Stock con validaciones
        /// </summary>
        /// <param name="valor">Valor entero a convertir</param>
        public static explicit operator Stock(int valor) => Crear(valor);

        /// <summary>
        /// Verifica si hay suficiente stock para una cantidad específica
        /// </summary>
        /// <param name="cantidad">Cantidad requerida</param>
        /// <returns>True si hay suficiente stock, false en caso contrario</returns>
        public bool TieneCantidad(int cantidad) => Value >= cantidad;
        
        /// <summary>
        /// Reduce el stock en la cantidad especificada
        /// </summary>
        /// <param name="cantidad">Cantidad a reducir</param>
        /// <returns>Nueva instancia de Stock con la cantidad reducida</returns>
        /// <exception cref="ArgumentException">Si la cantidad resulta en stock negativo</exception>
        public Stock Reducir(int cantidad) => Crear(Value - cantidad);
        
        /// <summary>
        /// Aumenta el stock en la cantidad especificada
        /// </summary>
        /// <param name="cantidad">Cantidad a aumentar</param>
        /// <returns>Nueva instancia de Stock con la cantidad aumentada</returns>
        /// <exception cref="ArgumentException">Si la cantidad excede el máximo permitido</exception>
        public Stock Aumentar(int cantidad) => Crear(Value + cantidad);
        
        /// <summary>
        /// Verifica si el stock está por debajo del mínimo de alerta
        /// </summary>
        /// <returns>True si el stock está bajo, false en caso contrario</returns>
        public bool EstaBajo() => Value < BusinessConstants.PRODUCTO_STOCK_MINIMO_ALERTA;

        /// <summary>
        /// Representación en string del stock
        /// </summary>
        /// <returns>Valor del stock como string</returns>
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Value Object que encapsula la categoría de un producto con validaciones de dominio.
    /// Garantiza que la categoría cumpla con las reglas de negocio establecidas.
    /// </summary>
    /// <remarks>
    /// Valida la longitud y contenido de la categoría para mantener consistencia
    /// en la clasificación de productos.
    /// </remarks>
    public sealed record Categoria
    {
        /// <summary>
        /// Obtiene el valor de la categoría validada
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Constructor privado para garantizar creación solo a través del método factory
        /// </summary>
        /// <param name="value">Valor de la categoría ya validado</param>
        private Categoria(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Método factory para crear una instancia de Categoria con validaciones de dominio
        /// </summary>
        /// <param name="valor">Nombre de la categoría a validar</param>
        /// <returns>Nueva instancia de Categoria validada</returns>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando la categoría es nula, vacía, muy corta o excede la longitud máxima
        /// </exception>
        /// <example>
        /// <code>
        /// var categoria = Categoria.Crear("Electrónicos");
        /// Console.WriteLine(categoria.Value); // "Electrónicos"
        /// </code>
        /// </example>
        public static Categoria Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException(BusinessConstants.ValidationMessages.CATEGORIA_NOMBRE_INVALIDO, nameof(valor));
            
            var categoriaTrimmed = valor.Trim();
            
            if (categoriaTrimmed.Length < BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MINIMA || 
                categoriaTrimmed.Length > BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MAXIMA)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.CATEGORIA_NOMBRE_INVALIDO, 
                        BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MINIMA, 
                        BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MAXIMA), 
                    nameof(valor));

            return new Categoria(categoriaTrimmed);
        }

        /// <summary>
        /// Conversión implícita de Categoria a string
        /// </summary>
        /// <param name="categoria">Instancia de Categoria</param>
        public static implicit operator string(Categoria categoria) => categoria.Value;
        
        /// <summary>
        /// Conversión explícita de string a Categoria con validaciones
        /// </summary>
        /// <param name="valor">Valor string a convertir</param>
        public static explicit operator Categoria(string valor) => Crear(valor);

        /// <summary>
        /// Representación en string de la categoría
        /// </summary>
        /// <returns>Valor de la categoría</returns>
        public override string ToString() => Value;
    }

    /// <summary>
    /// Value Object que encapsula el identificador de usuario con validaciones de dominio.
    /// Garantiza que el ID del usuario sea válido y cumpla con las restricciones establecidas.
    /// </summary>
    /// <remarks>
    /// Este Value Object valida que el ID de usuario tenga un formato válido
    /// y una longitud apropiada para los sistemas de autenticación.
    /// </remarks>
    public sealed record UsuarioId
    {
        /// <summary>
        /// Obtiene el valor del ID de usuario validado
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Constructor privado para garantizar creación solo a través del método factory
        /// </summary>
        /// <param name="value">Valor del ID de usuario ya validado</param>
        private UsuarioId(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Método factory para crear una instancia de UsuarioId con validaciones de dominio
        /// </summary>
        /// <param name="valor">ID de usuario a validar</param>
        /// <returns>Nueva instancia de UsuarioId validada</returns>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando el ID es nulo, vacío o excede la longitud máxima
        /// </exception>
        /// <example>
        /// <code>
        /// var usuarioId = UsuarioId.Crear("user123@empresa.com");
        /// Console.WriteLine(usuarioId.Value); // "user123@empresa.com"
        /// </code>
        /// </example>
        public static UsuarioId Crear(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException(BusinessConstants.ValidationMessages.CARRITO_USUARIO_REQUERIDO, nameof(valor));
            
            var usuarioTrimmed = valor.Trim();
            
            if (usuarioTrimmed.Length > BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA) // Reutilizamos la constante
                throw new ArgumentException($"El ID de usuario no puede exceder {BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA} caracteres", nameof(valor));

            return new UsuarioId(usuarioTrimmed);
        }

        /// <summary>
        /// Conversión implícita de UsuarioId a string
        /// </summary>
        /// <param name="usuarioId">Instancia de UsuarioId</param>
        public static implicit operator string(UsuarioId usuarioId) => usuarioId.Value;
        
        /// <summary>
        /// Conversión explícita de string a UsuarioId con validaciones
        /// </summary>
        /// <param name="valor">Valor string a convertir</param>
        public static explicit operator UsuarioId(string valor) => Crear(valor);

        /// <summary>
        /// Representación en string del ID de usuario
        /// </summary>
        /// <returns>Valor del ID de usuario</returns>
        public override string ToString() => Value;
    }

    /// <summary>
    /// Value Object que encapsula la cantidad de productos con validaciones de dominio y operaciones específicas.
    /// Garantiza que las cantidades sean válidas y proporciona métodos para manipulación segura.
    /// </summary>
    /// <remarks>
    /// Este Value Object incluye operaciones específicas del dominio para aumentar y reducir
    /// cantidades de manera segura, con validaciones de límites.
    /// </remarks>
    public sealed record Cantidad
    {
        /// <summary>
        /// Obtiene el valor de la cantidad validada
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Constructor privado para garantizar creación solo a través del método factory
        /// </summary>
        /// <param name="value">Valor de la cantidad ya validado</param>
        private Cantidad(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Método factory para crear una instancia de Cantidad con validaciones de dominio
        /// </summary>
        /// <param name="valor">Cantidad a validar</param>
        /// <returns>Nueva instancia de Cantidad validada</returns>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando la cantidad es menor al mínimo o excede el máximo permitido
        /// </exception>
        /// <example>
        /// <code>
        /// var cantidad = Cantidad.Crear(5);
        /// var nuevaCantidad = cantidad.Aumentar(3); // Resultado: 8
        /// </code>
        /// </example>
        public static Cantidad Crear(int valor)
        {
            if (valor < BusinessConstants.CARRITO_CANTIDAD_MINIMA)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.CARRITO_CANTIDAD_INVALIDA, 
                        BusinessConstants.CARRITO_CANTIDAD_MINIMA, 
                        BusinessConstants.CARRITO_CANTIDAD_MAXIMA), 
                    nameof(valor));

            if (valor > BusinessConstants.CARRITO_CANTIDAD_MAXIMA)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, BusinessConstants.ValidationMessages.CARRITO_CANTIDAD_INVALIDA, 
                        BusinessConstants.CARRITO_CANTIDAD_MINIMA, 
                        BusinessConstants.CARRITO_CANTIDAD_MAXIMA), 
                    nameof(valor));

            return new Cantidad(valor);
        }

        /// <summary>
        /// Conversión implícita de Cantidad a int
        /// </summary>
        /// <param name="cantidad">Instancia de Cantidad</param>
        public static implicit operator int(Cantidad cantidad) => cantidad.Value;
        
        /// <summary>
        /// Conversión explícita de int a Cantidad con validaciones
        /// </summary>
        /// <param name="valor">Valor entero a convertir</param>
        public static explicit operator Cantidad(int valor) => Crear(valor);

        /// <summary>
        /// Aumenta la cantidad en el incremento especificado
        /// </summary>
        /// <param name="incremento">Cantidad a aumentar</param>
        /// <returns>Nueva instancia de Cantidad con el incremento aplicado</returns>
        /// <exception cref="ArgumentException">Si el resultado excede el máximo permitido</exception>
        public Cantidad Aumentar(int incremento) => Crear(Value + incremento);
        
        /// <summary>
        /// Reduce la cantidad en el decremento especificado
        /// </summary>
        /// <param name="decremento">Cantidad a reducir</param>
        /// <returns>Nueva instancia de Cantidad con el decremento aplicado</returns>
        /// <exception cref="ArgumentException">Si el resultado es menor al mínimo permitido</exception>
        public Cantidad Reducir(int decremento) => Crear(Value - decremento);

        /// <summary>
        /// Representación en string de la cantidad
        /// </summary>
        /// <returns>Valor de la cantidad como string</returns>
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
}
