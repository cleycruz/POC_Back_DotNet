using CarritoComprasAPI.Core.Domain.Events;
using System.Globalization;

namespace CarritoComprasAPI.Core.Domain.Events.Productos
{
    /// <summary>
    /// Evento disparado cuando se crea un nuevo producto
    /// </summary>
    public record ProductoCreado(
        int ProductoId,
        string Nombre,
        string Descripcion,
        decimal Precio,
        int Stock,
        string Categoria,
        DateTime FechaCreacion
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se actualiza un producto
    /// </summary>
    public record ProductoActualizado(
        int ProductoId,
        string NombreAnterior,
        string NombreNuevo,
        decimal PrecioAnterior,
        decimal PrecioNuevo,
        int StockAnterior,
        int StockNuevo,
        string CategoriaAnterior,
        string CategoriaNueva
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se elimina un producto
    /// </summary>
    public record ProductoEliminado(
        int ProductoId,
        string Nombre,
        string Categoria
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando el stock de un producto cambia
    /// </summary>
    public record StockProductoCambiado(
        int ProductoId,
        string NombreProducto,
        int StockAnterior,
        int StockNuevo,
        string Razon
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando un producto se queda sin stock
    /// </summary>
    public record ProductoSinStock(
        int ProductoId,
        string NombreProducto,
        string Categoria
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando el precio de un producto cambia
    /// </summary>
    public record PrecioProductoCambiado(
        int ProductoId,
        string NombreProducto,
        decimal PrecioAnterior,
        decimal PrecioNuevo,
        decimal PorcentajeCambio
    ) : DomainEvent;
}
