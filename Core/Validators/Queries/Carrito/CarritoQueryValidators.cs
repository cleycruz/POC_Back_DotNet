using FluentValidation;
using CarritoComprasAPI.Core.Queries.Carrito;
using CarritoComprasAPI.Core.Validators.Common;

namespace CarritoComprasAPI.Core.Validators.Queries.Carrito
{
    /// <summary>
    /// Validador para la query de obtener carrito por usuario
    /// </summary>
    public class ObtenerCarritoPorUsuarioQueryValidator : AbstractValidator<ObtenerCarritoPorUsuarioQuery>
    {
        public ObtenerCarritoPorUsuarioQueryValidator()
        {
            RuleFor(x => x.UsuarioId).ValidUsuarioId();
        }
    }
}
