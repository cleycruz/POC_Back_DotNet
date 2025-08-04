using FluentValidation;
using CarritoComprasAPI.Core.Queries.Productos;
using CarritoComprasAPI.Core.Validators.Common;

namespace CarritoComprasAPI.Core.Validators.Queries.Productos
{
    /// <summary>
    /// Validador para la query de obtener producto por ID
    /// </summary>
    public class ObtenerProductoPorIdQueryValidator : AbstractValidator<ObtenerProductoPorIdQuery>
    {
        public ObtenerProductoPorIdQueryValidator()
        {
            RuleFor(x => x.Id).ValidProductoId();
        }
    }

    /// <summary>
    /// Validador para la query de buscar productos por categoría
    /// </summary>
    public class BuscarProductosPorCategoriaQueryValidator : AbstractValidator<BuscarProductosPorCategoriaQuery>
    {
        public BuscarProductosPorCategoriaQueryValidator()
        {
            When(x => !string.IsNullOrEmpty(x.Categoria), () => {
                RuleFor(x => x.Categoria).ValidCategoriaQuery();
            });
        }
    }
}
