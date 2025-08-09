using CarritoComprasAPI.Core.Domain.Events;
using System.Globalization;

namespace CarritoComprasAPI.Core.Domain.Events.Carrito
{
    /// <summary>
    /// Evento disparado cuando se crea un nuevo carrito
    /// </summary>
    public record CarritoCreado(
        string UsuarioId,
        DateTime FechaCreacion
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se agrega un item al carrito
    /// </summary>
    public record ItemAgregadoAlCarrito(
        string UsuarioId,
        int ProductoId,
        string NombreProducto,
        int Cantidad,
        decimal PrecioUnitario,
        decimal Subtotal
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se actualiza la cantidad de un item en el carrito
    /// </summary>
    public record CantidadItemCarritoActualizada(
        string UsuarioId,
        int ProductoId,
        string NombreProducto,
        int CantidadAnterior,
        int CantidadNueva,
        decimal SubtotalAnterior,
        decimal SubtotalNuevo
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se elimina un item del carrito
    /// </summary>
    public record ItemEliminadoDelCarrito(
        string UsuarioId,
        int ProductoId,
        string NombreProducto,
        int Cantidad,
        decimal SubtotalPerdido
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se vacía el carrito
    /// </summary>
    public record CarritoVaciado(
        string UsuarioId,
        int CantidadItemsEliminados,
        decimal TotalPerdido
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se actualiza el total del carrito
    /// </summary>
    public record TotalCarritoActualizado(
        string UsuarioId,
        decimal TotalAnterior,
        decimal TotalNuevo,
        int CantidadItems
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando un carrito es abandonado (sin actividad por mucho tiempo)
    /// </summary>
    public record CarritoAbandonado(
        string UsuarioId,
        int CantidadItems,
        decimal TotalCarrito,
        DateTime UltimaActividad,
        TimeSpan TiempoAbandonado
    ) : DomainEvent;

    /// <summary>
    /// Evento disparado cuando se intenta agregar un producto sin stock suficiente
    /// </summary>
    public record ProductoSinStockSuficiente(
        string UsuarioId,
        int ProductoId,
        string NombreProducto,
        int CantidadSolicitada,
        int StockDisponible
    ) : DomainEvent;
}
