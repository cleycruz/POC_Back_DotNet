using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CarritoComprasAPI.DTOs
{
    /// <summary>
    /// DTO que representa un carrito de compras completo
    /// </summary>
    public class CarritoDto
    {
        /// <summary>
        /// Identificador único del carrito
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Identificador del usuario propietario del carrito
        /// </summary>
        public string UsuarioId { get; set; } = string.Empty;
        
        /// <summary>
        /// Lista de items en el carrito
        /// </summary>
        public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();
        
        /// <summary>
        /// Total del carrito
        /// </summary>
        public decimal Total { get; set; }
        
        /// <summary>
        /// Cantidad total de items en el carrito
        /// </summary>
        public int CantidadItems { get; set; }
        
        /// <summary>
        /// Cantidad de productos diferentes en el carrito
        /// </summary>
        public int CantidadProductos { get; set; }
        
        /// <summary>
        /// Fecha de creación del carrito
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        
        /// <summary>
        /// Fecha de última actualización del carrito
        /// </summary>
        public DateTime FechaActualizacion { get; set; }
    }

    /// <summary>
    /// DTO que representa un item dentro del carrito
    /// </summary>
    public class CarritoItemDto
    {
        /// <summary>
        /// Identificador único del item
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Identificador del producto
        /// </summary>
        public int ProductoId { get; set; }
        
        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string ProductoNombre { get; set; } = string.Empty;
        
        /// <summary>
        /// Cantidad del producto en el carrito
        /// </summary>
        public int Cantidad { get; set; }
        
        /// <summary>
        /// Precio unitario del producto
        /// </summary>
        public decimal PrecioUnitario { get; set; }
        
        /// <summary>
        /// Subtotal del item (cantidad * precio unitario)
        /// </summary>
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// DTO para agregar un item al carrito
    /// </summary>
    public class AgregarItemCarritoDto
    {
        /// <summary>
        /// Identificador del producto a agregar
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser mayor a 0")]
        public int ProductoId { get; set; }

        /// <summary>
        /// Cantidad del producto a agregar
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// DTO para actualizar la cantidad de un item en el carrito
    /// </summary>
    public class ActualizarCantidadDto
    {
        /// <summary>
        /// Nueva cantidad del item (0 para eliminar)
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        public int Cantidad { get; set; }
    }
}
