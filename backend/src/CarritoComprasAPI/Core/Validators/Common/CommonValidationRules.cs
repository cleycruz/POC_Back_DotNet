using FluentValidation;

namespace CarritoComprasAPI.Core.Validators.Common
{
    /// <summary>
    /// Reglas de validación comunes reutilizables para evitar duplicación de código
    /// </summary>
    public static class CommonValidationRules
    {
        #region Usuario ID Validations
        
        /// <summary>
        /// Validación estándar para ID de usuario
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidUsuarioId<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("El ID del usuario es obligatorio")
                .Length(3, 50)
                .WithMessage("El ID del usuario debe tener entre 3 y 50 caracteres")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .WithMessage("El ID del usuario solo puede contener letras, números, guiones y guiones bajos");
        }

        #endregion

        #region Producto ID Validations
        
        /// <summary>
        /// Validación estándar para ID de producto
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidProductoId<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");
        }

        #endregion

        #region Producto Fields Validations
        
        /// <summary>
        /// Validación estándar para nombre de producto
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidProductoNombre<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("El nombre del producto es obligatorio")
                .Length(2, 100)
                .WithMessage("El nombre del producto debe tener entre 2 y 100 caracteres");
        }

        /// <summary>
        /// Validación estándar para nombre de producto nullable
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidProductoNombreNullable<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("El nombre del producto es obligatorio")
                .Length(2, 100)
                .WithMessage("El nombre del producto debe tener entre 2 y 100 caracteres");
        }

        /// <summary>
        /// Validación estándar para descripción de producto
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidProductoDescripcion<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("La descripción del producto es obligatoria")
                .Length(10, 500)
                .WithMessage("La descripción del producto debe tener entre 10 y 500 caracteres");
        }

        /// <summary>
        /// Validación estándar para descripción de producto nullable
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidProductoDescripcionNullable<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("La descripción del producto es obligatoria")
                .Length(10, 500)
                .WithMessage("La descripción del producto debe tener entre 10 y 500 caracteres");
        }

        /// <summary>
        /// Validación estándar para precio de producto
        /// </summary>
        public static IRuleBuilderOptions<T, decimal> ValidProductoPrecio<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("El precio del producto debe ser mayor a 0")
                .LessThan(1000000)
                .WithMessage("El precio del producto no puede exceder $1,000,000")
                .Must(precio => Math.Round(precio, 2) == precio)
                .WithMessage("El precio no puede tener más de 2 decimales");
        }

        /// <summary>
        /// Validación estándar para precio de producto opcional
        /// </summary>
        public static IRuleBuilderOptions<T, decimal?> ValidProductoPrecioOpcional<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("El precio del producto debe ser mayor a 0")
                .LessThan(1000000)
                .WithMessage("El precio del producto no puede exceder $1,000,000")
                .Must(precio => precio == null || Math.Round(precio.Value, 2) == precio.Value)
                .WithMessage("El precio no puede tener más de 2 decimales");
        }

        /// <summary>
        /// Validación estándar para stock de producto
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidProductoStock<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock del producto no puede ser negativo")
                .LessThan(10000)
                .WithMessage("El stock del producto no puede exceder 10,000 unidades");
        }

        /// <summary>
        /// Validación estándar para stock de producto opcional
        /// </summary>
        public static IRuleBuilderOptions<T, int?> ValidProductoStockOpcional<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock del producto no puede ser negativo")
                .LessThan(10000)
                .WithMessage("El stock del producto no puede exceder 10,000 unidades");
        }

        /// <summary>
        /// Validación estándar para categoría de producto
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidProductoCategoria<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("La categoría del producto es obligatoria")
                .Length(2, 50)
                .WithMessage("La categoría del producto debe tener entre 2 y 50 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$")
                .WithMessage("La categoría solo puede contener letras y espacios");
        }

        /// <summary>
        /// Validación estándar para categoría de producto nullable
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidProductoCategoriaNullable<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("La categoría del producto es obligatoria")
                .Length(2, 50)
                .WithMessage("La categoría del producto debe tener entre 2 y 50 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$")
                .WithMessage("La categoría solo puede contener letras y espacios");
        }

        #endregion

        #region Cantidad Validations
        
        /// <summary>
        /// Validación estándar para cantidad de productos en carrito
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidCantidadCarrito<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("La cantidad no puede exceder 100 unidades por producto");
        }

        #endregion

        #region Query Validations
        
        /// <summary>
        /// Validación estándar para paginación
        /// </summary>
        public static IRuleBuilderOptions<T, int?> ValidPaginacion<T>(this IRuleBuilder<T, int?> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage($"{fieldName} debe ser mayor a 0");
        }

        /// <summary>
        /// Validación estándar para tamaño de página
        /// </summary>
        public static IRuleBuilderOptions<T, int?> ValidTamanoPagina<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("El tamaño de página debe ser mayor a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El tamaño de página no puede superar 100 elementos");
        }

        /// <summary>
        /// Validación estándar para categoría en queries
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidCategoriaQuery<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Length(2, 50)
                .WithMessage("La categoría debe tener entre 2 y 50 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("La categoría solo puede contener letras y espacios");
        }

        #endregion
    }


}
