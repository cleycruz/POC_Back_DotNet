using System.Globalization;
namespace CarritoComprasAPI.Core.Configuration
{
    /// <summary>
    /// Constantes de dominio y reglas de negocio para el sistema de carrito de compras.
    /// Estas constantes centralizan los valores mágicos y facilitan el mantenimiento.
    /// </summary>
    public static class BusinessConstants
    {
        #region Producto Constants
        
        /// <summary>
        /// Longitud máxima permitida para el nombre de un producto
        /// </summary>
        public const int PRODUCTO_NOMBRE_LONGITUD_MAXIMA = 100;
        
        /// <summary>
        /// Longitud mínima requerida para el nombre de un producto
        /// </summary>
        public const int PRODUCTO_NOMBRE_LONGITUD_MINIMA = 3;
        
        /// <summary>
        /// Longitud máxima permitida para la descripción de un producto
        /// </summary>
        public const int PRODUCTO_DESCRIPCION_LONGITUD_MAXIMA = 500;
        
        /// <summary>
        /// Precio mínimo permitido para un producto
        /// </summary>
        public const decimal PRODUCTO_PRECIO_MINIMO = 0.01m;
        
        /// <summary>
        /// Precio máximo permitido para un producto
        /// </summary>
        public const decimal PRODUCTO_PRECIO_MAXIMO = 999999.99m;
        
        /// <summary>
        /// Stock mínimo requerido para alertas
        /// </summary>
        public const int PRODUCTO_STOCK_MINIMO_ALERTA = 5;
        
        /// <summary>
        /// Stock máximo permitido por producto
        /// </summary>
        public const int PRODUCTO_STOCK_MAXIMO = 10000;
        
        #endregion

        #region Carrito Constants
        
        /// <summary>
        /// Cantidad mínima que se puede agregar a un carrito
        /// </summary>
        public const int CARRITO_CANTIDAD_MINIMA = 1;
        
        /// <summary>
        /// Cantidad máxima que se puede agregar de un producto al carrito
        /// </summary>
        public const int CARRITO_CANTIDAD_MAXIMA = 1000;
        
        /// <summary>
        /// Número máximo de productos diferentes en un carrito
        /// </summary>
        public const int CARRITO_ITEMS_MAXIMO = 50;
        
        /// <summary>
        /// Valor total máximo permitido para un carrito
        /// </summary>
        public const decimal CARRITO_TOTAL_MAXIMO = 100000m;
        
        #endregion

        #region Categoria Constants
        
        /// <summary>
        /// Longitud máxima permitida para el nombre de una categoría
        /// </summary>
        public const int CATEGORIA_NOMBRE_LONGITUD_MAXIMA = 50;
        
        /// <summary>
        /// Longitud mínima requerida para el nombre de una categoría
        /// </summary>
        public const int CATEGORIA_NOMBRE_LONGITUD_MINIMA = 2;
        
        #endregion

        #region Cache Constants
        
        /// <summary>
        /// Tiempo de expiración por defecto para cache (en minutos)
        /// </summary>
        public const int CACHE_EXPIRACION_DEFECTO_MINUTOS = 15;
        
        /// <summary>
        /// Tiempo de expiración para cache de productos (en minutos)
        /// </summary>
        public const int CACHE_PRODUCTOS_EXPIRACION_MINUTOS = 30;
        
        /// <summary>
        /// Tiempo de expiración para cache de carritos (en minutos)
        /// </summary>
        public const int CACHE_CARRITOS_EXPIRACION_MINUTOS = 5;
        
        #endregion

        #region Validation Messages
        
        /// <summary>
        /// Mensajes de validación centralizados para consistencia
        /// </summary>
        public static class ValidationMessages
        {
            public const string PRODUCTO_NOMBRE_REQUERIDO = "El nombre del producto es requerido";
            public const string PRODUCTO_NOMBRE_LONGITUD_INVALIDA = "El nombre del producto debe tener entre {0} y {1} caracteres";
            public const string PRODUCTO_PRECIO_INVALIDO = "El precio debe estar entre {0} y {1}";
            public const string PRODUCTO_STOCK_INVALIDO = "El stock debe estar entre 0 y {0}";
            
            public const string CARRITO_CANTIDAD_INVALIDA = "La cantidad debe estar entre {0} y {1}";
            public const string CARRITO_PRODUCTO_REQUERIDO = "El producto es requerido";
            public const string CARRITO_USUARIO_REQUERIDO = "El usuario es requerido";
            
            public const string CATEGORIA_NOMBRE_INVALIDO = "La categoría debe tener entre {0} y {1} caracteres";
        }
        
        #endregion
    }
}
