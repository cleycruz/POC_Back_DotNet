using System.ComponentModel.DataAnnotations;

namespace CarritoComprasAPI.Models;

public class Carrito
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string UsuarioId { get; set; } = string.Empty;
    
    public List<CarritoItem> Items { get; set; } = new();
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public DateTime? FechaActualizacion { get; set; }
    
    public decimal Total => Items.Sum(item => item.Subtotal);
    
    public int CantidadTotalItems => Items.Sum(item => item.Cantidad);
    
    public bool EstaVacio => !Items.Any();

    public void AgregarItem(Producto producto, int cantidad)
    {
        if (producto == null)
            throw new ArgumentNullException(nameof(producto));
        
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));
        
        if (cantidad > producto.Stock)
            throw new InvalidOperationException("No hay suficiente stock disponible");

        var itemExistente = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
        
        if (itemExistente != null)
        {
            var nuevaCantidad = itemExistente.Cantidad + cantidad;
            if (nuevaCantidad > producto.Stock)
                throw new InvalidOperationException("No hay suficiente stock disponible");
            
            itemExistente.ActualizarCantidad(nuevaCantidad);
        }
        else
        {
            var nuevoItem = new CarritoItem
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio,
                CarritoId = Id
            };
            
            Items.Add(nuevoItem);
        }
        
        FechaActualizacion = DateTime.UtcNow;
    }

    public void RemoverItem(int productoId)
    {
        var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
        {
            Items.Remove(item);
            FechaActualizacion = DateTime.UtcNow;
        }
    }

    public void ActualizarCantidadItem(int productoId, int nuevaCantidad)
    {
        var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null)
            throw new InvalidOperationException("El producto no está en el carrito");
        
        if (nuevaCantidad <= 0)
        {
            RemoverItem(productoId);
        }
        else
        {
            item.ActualizarCantidad(nuevaCantidad);
            FechaActualizacion = DateTime.UtcNow;
        }
    }

    public void LimpiarCarrito()
    {
        Items.Clear();
        FechaActualizacion = DateTime.UtcNow;
    }

    public decimal CalcularDescuento(decimal porcentajeDescuento)
    {
        if (porcentajeDescuento < 0 || porcentajeDescuento > 100)
            throw new ArgumentException("El porcentaje de descuento debe estar entre 0 y 100");
        
        return Total * (porcentajeDescuento / 100);
    }

    public decimal CalcularTotalConDescuento(decimal porcentajeDescuento)
    {
        return Total - CalcularDescuento(porcentajeDescuento);
    }
}
