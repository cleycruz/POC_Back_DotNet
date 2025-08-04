using System.ComponentModel.DataAnnotations;
using CarritoComprasAPI.Core.Configuration;

namespace CarritoComprasAPI.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de productos con validaciones integradas
    /// </summary>
    public class ProductoDto
    {
        /// <summary>
        /// Identificador único del producto
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; } = string.Empty;
        
        /// <summary>
        /// Descripción del producto
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;
        
        /// <summary>
        /// Precio del producto
        /// </summary>
        public decimal Precio { get; set; }
        
        /// <summary>
        /// Stock disponible
        /// </summary>
        public int Stock { get; set; }
        
        /// <summary>
        /// Categoría del producto
        /// </summary>
        public string Categoria { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para creación de productos con validaciones de dominio
    /// </summary>
    public class CrearProductoDto
    {
        /// <summary>
        /// Nombre del producto a crear
        /// </summary>
        [Required(ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_NOMBRE_REQUERIDO)]
        [StringLength(BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA, 
            MinimumLength = BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MINIMA,
            ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_NOMBRE_LONGITUD_INVALIDA)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional del producto
        /// </summary>
        [StringLength(BusinessConstants.PRODUCTO_DESCRIPCION_LONGITUD_MAXIMA, 
            ErrorMessage = "La descripción no puede exceder {1} caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Precio del producto
        /// </summary>
        [Required(ErrorMessage = "El precio es requerido")]
        [Range((double)BusinessConstants.PRODUCTO_PRECIO_MINIMO, (double)BusinessConstants.PRODUCTO_PRECIO_MAXIMO, 
            ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_PRECIO_INVALIDO)]
        public decimal Precio { get; set; }

        /// <summary>
        /// Stock inicial del producto
        /// </summary>
        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, BusinessConstants.PRODUCTO_STOCK_MAXIMO, 
            ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_STOCK_INVALIDO)]
        public int Stock { get; set; }

        /// <summary>
        /// Categoría del producto
        /// </summary>
        [Required(ErrorMessage = "La categoría es requerida")]
        [StringLength(BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MAXIMA, 
            MinimumLength = BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MINIMA,
            ErrorMessage = BusinessConstants.ValidationMessages.CATEGORIA_NOMBRE_INVALIDO)]
        public string Categoria { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualización de productos con validaciones opcionales
    /// </summary>
    public class ActualizarProductoDto
    {
        /// <summary>
        /// Nuevo nombre del producto (opcional)
        /// </summary>
        [StringLength(BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MAXIMA, 
            MinimumLength = BusinessConstants.PRODUCTO_NOMBRE_LONGITUD_MINIMA,
            ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_NOMBRE_LONGITUD_INVALIDA)]
        public string? Nombre { get; set; }

        /// <summary>
        /// Nueva descripción del producto (opcional)
        /// </summary>
        [StringLength(BusinessConstants.PRODUCTO_DESCRIPCION_LONGITUD_MAXIMA, 
            ErrorMessage = "La descripción no puede exceder {1} caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Nuevo precio del producto (opcional)
        /// </summary>
        [Range((double)BusinessConstants.PRODUCTO_PRECIO_MINIMO, (double)BusinessConstants.PRODUCTO_PRECIO_MAXIMO, 
            ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_PRECIO_INVALIDO)]
        public decimal? Precio { get; set; }

        /// <summary>
        /// Nuevo stock del producto (opcional)
        /// </summary>
        [Range(0, BusinessConstants.PRODUCTO_STOCK_MAXIMO, 
            ErrorMessage = BusinessConstants.ValidationMessages.PRODUCTO_STOCK_INVALIDO)]
        public int? Stock { get; set; }

        /// <summary>
        /// Nueva categoría del producto (opcional)
        /// </summary>
        [StringLength(BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MAXIMA, 
            MinimumLength = BusinessConstants.CATEGORIA_NOMBRE_LONGITUD_MINIMA,
            ErrorMessage = BusinessConstants.ValidationMessages.CATEGORIA_NOMBRE_INVALIDO)]
        public string? Categoria { get; set; }
    }
}
