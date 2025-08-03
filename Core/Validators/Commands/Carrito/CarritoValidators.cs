using FluentValidation;
using CarritoComprasAPI.Core.Commands.Carrito;

namespace CarritoComprasAPI.Core.Validators.Commands.Carrito
{
    /// <summary>
    /// Validador para el comando de agregar item al carrito
    /// </summary>
    public class AgregarItemCarritoCommandValidator : AbstractValidator<AgregarItemCarritoCommand>
    {
        public AgregarItemCarritoCommandValidator()
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

            RuleFor(x => x.ProductoId)
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("La cantidad no puede exceder 100 unidades por producto");
        }
    }

    /// <summary>
    /// Validador para el comando de actualizar cantidad de item en carrito
    /// </summary>
    public class ActualizarCantidadItemCommandValidator : AbstractValidator<ActualizarCantidadItemCommand>
    {
        public ActualizarCantidadItemCommandValidator()
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

            RuleFor(x => x.ProductoId)
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0)
                .WithMessage("La nueva cantidad debe ser mayor a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("La cantidad no puede exceder 100 unidades por producto");
        }
    }

    /// <summary>
    /// Validador para el comando de eliminar item del carrito
    /// </summary>
    public class EliminarItemCarritoCommandValidator : AbstractValidator<EliminarItemCarritoCommand>
    {
        public EliminarItemCarritoCommandValidator()
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

            RuleFor(x => x.ProductoId)
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");
        }
    }

    /// <summary>
    /// Validador para el comando de vaciar carrito
    /// </summary>
    public class VaciarCarritoCommandValidator : AbstractValidator<VaciarCarritoCommand>
    {
        public VaciarCarritoCommandValidator()
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
