using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CarritoComprasAPI.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
    
    public bool EstaDisponible { get; set; }
    
    public DateTime FechaCreacion { get; set; }
    
    public DateTime? FechaActualizacion { get; set; }
    
    public string Categoria { get; set; } = string.Empty;
}

public class CrearProductoDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
    
    public string Categoria { get; set; } = string.Empty;
}

public class ActualizarProductoDto
{
    [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string? Nombre { get; set; }
    
    [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string? Descripcion { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal? Precio { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int? Stock { get; set; }
    
    public string? Categoria { get; set; }
}
