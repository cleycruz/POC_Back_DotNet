using FluentValidation;
using CarritoComprasAPI.Core.Commands.Productos;
using CarritoComprasAPI.Core.Validators.Common;

namespace CarritoComprasAPI.Core.Validators.Commands.Productos
{
    /// <summary>
    /// Validador para el comando de crear producto
    /// </summary>
    public class CrearProductoCommandValidator : AbstractValidator<CrearProductoCommand>
    {
        public CrearProductoCommandValidator()
        {
            RuleFor(x => x.Nombre).ValidProductoNombre();
            RuleFor(x => x.Descripcion).ValidProductoDescripcion();
            RuleFor(x => x.Precio).ValidProductoPrecio();
            RuleFor(x => x.Stock).ValidProductoStock();
            RuleFor(x => x.Categoria).ValidProductoCategoria();
        }
    }

    /// <summary>
    /// Validador para el comando de actualizar producto
    /// </summary>
    public class ActualizarProductoCommandValidator : AbstractValidator<ActualizarProductoCommand>
    {
        public ActualizarProductoCommandValidator()
        {
            RuleFor(x => x.Id).ValidProductoId();

            // Validaciones condicionales - solo cuando el campo está presente
            When(x => !string.IsNullOrEmpty(x.Nombre), () => {
                RuleFor(x => x.Nombre).ValidProductoNombreNullable();
            });

            When(x => !string.IsNullOrEmpty(x.Descripcion), () => {
                RuleFor(x => x.Descripcion).ValidProductoDescripcionNullable();
            });

            When(x => x.Precio.HasValue, () => {
                RuleFor(x => x.Precio).ValidProductoPrecioOpcional();
            });

            When(x => x.Stock.HasValue, () => {
                RuleFor(x => x.Stock).ValidProductoStockOpcional();
            });

            When(x => !string.IsNullOrEmpty(x.Categoria), () => {
                RuleFor(x => x.Categoria).ValidProductoCategoriaNullable();
            });
        }
    }

    /// <summary>
    /// Validador para el comando de eliminar producto
    /// </summary>
    public class EliminarProductoCommandValidator : AbstractValidator<EliminarProductoCommand>
    {
        public EliminarProductoCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");
        }
    }
}
