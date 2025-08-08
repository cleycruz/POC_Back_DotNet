using System.ComponentModel.DataAnnotations;

namespace CarritoComprasAPI.Models;

public class Producto
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Descripcion { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
    
    public bool EstaDisponible => Stock > 0;
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public DateTime? FechaActualizacion { get; set; }

    public void ActualizarStock(int cantidad)
    {
        if (Stock + cantidad < 0)
            throw new InvalidOperationException("No hay suficiente stock disponible");
        
        Stock += cantidad;
        FechaActualizacion = DateTime.UtcNow;
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0");
        
        Precio = nuevoPrecio;
        FechaActualizacion = DateTime.UtcNow;
    }
}
