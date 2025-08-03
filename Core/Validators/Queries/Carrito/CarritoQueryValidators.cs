using FluentValidation;
using CarritoComprasAPI.Core.Queries.Carrito;

namespace CarritoComprasAPI.Core.Validators.Queries.Carrito
{
    /// <summary>
    /// Validador para la query de obtener carrito por usuario
    /// </summary>
    public class ObtenerCarritoPorUsuarioQueryValidator : AbstractValidator<ObtenerCarritoPorUsuarioQuery>
    {
        public ObtenerCarritoPorUsuarioQueryValidator()
        {
            RuleFor(x => x.UsuarioId)
                .NotEmpty()
                .WithMessage("El ID del usuario es obligatorio")
                .MaximumLength(50)
                .WithMessage("El ID del usuario no puede exceder 50 caracteres")
                .MinimumLength(3)
                .WithMessage("El ID del usuario debe tener al menos 3 caracteres")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .WithMessage("El ID del usuario solo puede contener letras, números, guiones y guiones bajos");
        }
    }
}
