using System.ComponentModel.DataAnnotations;

namespace CarritoComprasAPI.Core.Domain
{
    public class CarritoItem
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductoId { get; set; }
        
        public Producto? Producto { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }
        
        public decimal Subtotal => Cantidad * PrecioUnitario;
        
        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;

        // Métodos de dominio
        public void ActualizarCantidad(int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(nuevaCantidad));
            
            Cantidad = nuevaCantidad;
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0", nameof(nuevoPrecio));
            
            PrecioUnitario = nuevoPrecio;
        }

        public bool EsDelProducto(int productoId)
        {
            return ProductoId == productoId;
        }
    }
}
