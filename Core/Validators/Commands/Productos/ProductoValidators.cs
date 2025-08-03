using FluentValidation;
using CarritoComprasAPI.Core.Commands.Productos;

namespace CarritoComprasAPI.Core.Validators.Commands.Productos
{
    /// <summary>
    /// Validador para el comando de crear producto
    /// </summary>
    public class CrearProductoCommandValidator : AbstractValidator<CrearProductoCommand>
    {
        public CrearProductoCommandValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre del producto es obligatorio")
                .MaximumLength(100)
                .WithMessage("El nombre del producto no puede exceder 100 caracteres")
                .MinimumLength(2)
                .WithMessage("El nombre del producto debe tener al menos 2 caracteres");

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción del producto es obligatoria")
                .MaximumLength(500)
                .WithMessage("La descripción del producto no puede exceder 500 caracteres")
                .MinimumLength(10)
                .WithMessage("La descripción del producto debe tener al menos 10 caracteres");

            RuleFor(x => x.Precio)
                .GreaterThan(0)
                .WithMessage("El precio del producto debe ser mayor a 0")
                .LessThan(1000000)
                .WithMessage("El precio del producto no puede exceder $1,000,000")
                .Must(precio => Math.Round(precio, 2) == precio)
                .WithMessage("El precio no puede tener más de 2 decimales");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock del producto no puede ser negativo")
                .LessThan(10000)
                .WithMessage("El stock del producto no puede exceder 10,000 unidades");

            RuleFor(x => x.Categoria)
                .NotEmpty()
                .WithMessage("La categoría del producto es obligatoria")
                .MaximumLength(50)
                .WithMessage("La categoría del producto no puede exceder 50 caracteres")
                .MinimumLength(2)
                .WithMessage("La categoría del producto debe tener al menos 2 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$")
                .WithMessage("La categoría solo puede contener letras y espacios");
        }
    }

    /// <summary>
    /// Validador para el comando de actualizar producto
    /// </summary>
    public class ActualizarProductoCommandValidator : AbstractValidator<ActualizarProductoCommand>
    {
        public ActualizarProductoCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del producto debe ser mayor a 0");

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre del producto es obligatorio")
                .MaximumLength(100)
                .WithMessage("El nombre del producto no puede exceder 100 caracteres")
                .MinimumLength(2)
                .WithMessage("El nombre del producto debe tener al menos 2 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Nombre));

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción del producto es obligatoria")
                .MaximumLength(500)
                .WithMessage("La descripción del producto no puede exceder 500 caracteres")
                .MinimumLength(10)
                .WithMessage("La descripción del producto debe tener al menos 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Precio)
                .GreaterThan(0)
                .WithMessage("El precio del producto debe ser mayor a 0")
                .LessThan(1000000)
                .WithMessage("El precio del producto no puede exceder $1,000,000")
                .Must(precio => precio == null || Math.Round(precio.Value, 2) == precio.Value)
                .WithMessage("El precio no puede tener más de 2 decimales")
                .When(x => x.Precio.HasValue);

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock del producto no puede ser negativo")
                .LessThan(10000)
                .WithMessage("El stock del producto no puede exceder 10,000 unidades")
                .When(x => x.Stock.HasValue);

            RuleFor(x => x.Categoria)
                .NotEmpty()
                .WithMessage("La categoría del producto es obligatoria")
                .MaximumLength(50)
                .WithMessage("La categoría del producto no puede exceder 50 caracteres")
                .MinimumLength(2)
                .WithMessage("La categoría del producto debe tener al menos 2 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$")
                .WithMessage("La categoría solo puede contener letras y espacios")
                .When(x => !string.IsNullOrEmpty(x.Categoria));
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
