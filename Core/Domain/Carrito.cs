using System.ComponentModel.DataAnnotations;

namespace CarritoComprasAPI.Core.Domain
{
    public class Carrito
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El ID de usuario es requerido")]
        public string UsuarioId { get; set; } = string.Empty;
        
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
        
        public decimal Total => Items.Sum(item => item.Subtotal);
        public int CantidadItems => Items.Count;
        public int CantidadProductos => Items.Sum(item => item.Cantidad);
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Métodos de dominio
        public void AgregarItem(Producto producto, int cantidad)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));
            
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

            if (!producto.TieneStock(cantidad))
                throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre}");

            var itemExistente = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
            
            if (itemExistente != null)
            {
                itemExistente.ActualizarCantidad(itemExistente.Cantidad + cantidad);
            }
            else
            {
                var nuevoItem = new CarritoItem
                {
                    ProductoId = producto.Id,
                    Producto = producto,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                };
                Items.Add(nuevoItem);
            }

            FechaActualizacion = DateTime.UtcNow;
        }

        public void ActualizarCantidadItem(int productoId, int nuevaCantidad)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
                throw new InvalidOperationException($"El producto con ID {productoId} no está en el carrito");

            if (nuevaCantidad <= 0)
            {
                EliminarItem(productoId);
                return;
            }

            item.ActualizarCantidad(nuevaCantidad);
            FechaActualizacion = DateTime.UtcNow;
        }

        public void EliminarItem(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                Items.Remove(item);
                FechaActualizacion = DateTime.UtcNow;
            }
        }

        public void Vaciar()
        {
            Items.Clear();
            FechaActualizacion = DateTime.UtcNow;
        }

        public bool TieneItems()
        {
            return Items.Any();
        }

        public CarritoItem? ObtenerItem(int productoId)
        {
            return Items.FirstOrDefault(i => i.ProductoId == productoId);
        }
    }
}
