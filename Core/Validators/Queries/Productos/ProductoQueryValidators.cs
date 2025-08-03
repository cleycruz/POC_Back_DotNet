using FluentValidation;
using CarritoComprasAPI.Core.Queries.Productos;

namespace CarritoComprasAPI.Core.Validators.Queries.Productos
{
    /// <summary>
    /// Validador para la query de obtener producto por ID
    /// </summary>
    public class ObtenerProductoPorIdQueryValidator : AbstractValidator<ObtenerProductoPorIdQuery>
    {
        public ObtenerProductoPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");
        }
    }

    /// <summary>
    /// Validador para la query de buscar productos por categoría
    /// </summary>
    public class BuscarProductosPorCategoriaQueryValidator : AbstractValidator<BuscarProductosPorCategoriaQuery>
    {
        public BuscarProductosPorCategoriaQueryValidator()
        {
            RuleFor(x => x.Categoria)
                .NotEmpty()
                .WithMessage("La categoría es obligatoria")
                .MaximumLength(50)
                .WithMessage("La categoría no puede exceder 50 caracteres")
                .MinimumLength(2)
                .WithMessage("La categoría debe tener al menos 2 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$")
                .WithMessage("La categoría solo puede contener letras y espacios");
        }
    }
}
