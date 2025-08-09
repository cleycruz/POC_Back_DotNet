using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CarritoComprasAPI.Models;

public class CarritoItem
{
    public int Id { get; set; }
    
    public int CarritoId { get; set; }
    public Carrito Carrito { get; set; } = null!;
    
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }
    
    public decimal PrecioUnitario { get; set; }
    
    public decimal Subtotal => Cantidad * PrecioUnitario;
    
    public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;

    public void ActualizarCantidad(int nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0");
        
        if (Producto != null && nuevaCantidad > Producto.Stock)
            throw new InvalidOperationException("No hay suficiente stock disponible");
        
        Cantidad = nuevaCantidad;
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0");
        
        PrecioUnitario = nuevoPrecio;
    }
}
