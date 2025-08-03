using System.ComponentModel.DataAnnotations;

namespace CarritoComprasAPI.DTOs
{
    public class CarritoDto
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();
        public decimal Total { get; set; }
        public int CantidadItems { get; set; }
        public int CantidadProductos { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class CarritoItemDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class AgregarItemCarritoDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser mayor a 0")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    public class ActualizarCantidadDto
    {
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        public int Cantidad { get; set; }
    }
}
