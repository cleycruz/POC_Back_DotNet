using FluentValidation;
using CarritoComprasAPI.Core.Commands.Carrito;
using CarritoComprasAPI.Core.Validators.Common;
using System.Globalization;

namespace CarritoComprasAPI.Core.Validators.Commands.Carrito
{
    /// <summary>
    /// Validador para el comando de agregar item al carrito
    /// </summary>
    public class AgregarItemCarritoCommandValidator : AbstractValidator<AgregarItemCarritoCommand>
    {
        public AgregarItemCarritoCommandValidator()
        {
            RuleFor(x => x.UsuarioId).ValidUsuarioId();
            RuleFor(x => x.ProductoId).ValidProductoId();
            RuleFor(x => x.Cantidad).ValidCantidadCarrito();
        }
    }

    /// <summary>
    /// Validador para el comando de actualizar cantidad de item en carrito
    /// </summary>
    public class ActualizarCantidadItemCommandValidator : AbstractValidator<ActualizarCantidadItemCommand>
    {
        public ActualizarCantidadItemCommandValidator()
        {
            RuleFor(x => x.UsuarioId).ValidUsuarioId();
            RuleFor(x => x.ProductoId).ValidProductoId();
            RuleFor(x => x.Cantidad).ValidCantidadCarrito();
        }
    }

    /// <summary>
    /// Validador para el comando de eliminar item del carrito
    /// </summary>
    public class EliminarItemCarritoCommandValidator : AbstractValidator<EliminarItemCarritoCommand>
    {
        public EliminarItemCarritoCommandValidator()
        {
            RuleFor(x => x.UsuarioId).ValidUsuarioId();
            RuleFor(x => x.ProductoId).ValidProductoId();
        }
    }

    /// <summary>
    /// Validador para el comando de vaciar carrito
    /// </summary>
    public class VaciarCarritoCommandValidator : AbstractValidator<VaciarCarritoCommand>
    {
        public VaciarCarritoCommandValidator()
        {
            RuleFor(x => x.UsuarioId).ValidUsuarioId();
        }
    }
}
