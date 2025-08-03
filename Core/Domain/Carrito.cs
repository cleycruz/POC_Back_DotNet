using System.ComponentModel.DataAnnotations;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Domain.Events.Carrito;

namespace CarritoComprasAPI.Core.Domain
{
    public class Carrito : DomainEntity
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
            {
                // Publicar evento de stock insuficiente
                RaiseDomainEvent(new ProductoSinStockSuficiente(
                    UsuarioId, producto.Id, producto.Nombre, cantidad, producto.Stock));
                throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre}");
            }

            var totalAnterior = Total;
            var itemExistente = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
            
            if (itemExistente != null)
            {
                var cantidadAnterior = itemExistente.Cantidad;
                var subtotalAnterior = itemExistente.Subtotal;
                itemExistente.ActualizarCantidad(itemExistente.Cantidad + cantidad);

                // Publicar evento de cantidad actualizada
                RaiseDomainEvent(new CantidadItemCarritoActualizada(
                    UsuarioId, producto.Id, producto.Nombre, 
                    cantidadAnterior, itemExistente.Cantidad,
                    subtotalAnterior, itemExistente.Subtotal));
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

                // Publicar evento de item agregado
                RaiseDomainEvent(new ItemAgregadoAlCarrito(
                    UsuarioId, producto.Id, producto.Nombre, 
                    cantidad, producto.Precio, nuevoItem.Subtotal));
            }

            FechaActualizacion = DateTime.UtcNow;

            // Publicar evento de total actualizado
            RaiseDomainEvent(new TotalCarritoActualizado(
                UsuarioId, totalAnterior, Total, CantidadItems));
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

            var totalAnterior = Total;
            var cantidadAnterior = item.Cantidad;
            var subtotalAnterior = item.Subtotal;
            
            item.ActualizarCantidad(nuevaCantidad);
            FechaActualizacion = DateTime.UtcNow;

            // Publicar evento de cantidad actualizada
            RaiseDomainEvent(new CantidadItemCarritoActualizada(
                UsuarioId, productoId, item.Producto?.Nombre ?? "Producto desconocido",
                cantidadAnterior, nuevaCantidad, subtotalAnterior, item.Subtotal));

            // Publicar evento de total actualizado
            RaiseDomainEvent(new TotalCarritoActualizado(
                UsuarioId, totalAnterior, Total, CantidadItems));
        }

        public void EliminarItem(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                var totalAnterior = Total;
                var nombreProducto = item.Producto?.Nombre ?? "Producto desconocido";
                var cantidad = item.Cantidad;
                var subtotalPerdido = item.Subtotal;

                Items.Remove(item);
                FechaActualizacion = DateTime.UtcNow;

                // Publicar evento de item eliminado
                RaiseDomainEvent(new ItemEliminadoDelCarrito(
                    UsuarioId, productoId, nombreProducto, cantidad, subtotalPerdido));

                // Publicar evento de total actualizado
                RaiseDomainEvent(new TotalCarritoActualizado(
                    UsuarioId, totalAnterior, Total, CantidadItems));
            }
        }

        public void Vaciar()
        {
            if (!Items.Any()) return;

            var cantidadItemsEliminados = Items.Count;
            var totalPerdido = Total;

            Items.Clear();
            FechaActualizacion = DateTime.UtcNow;

            // Publicar evento de carrito vaciado
            RaiseDomainEvent(new CarritoVaciado(
                UsuarioId, cantidadItemsEliminados, totalPerdido));
        }

        public bool TieneItems()
        {
            return Items.Any();
        }

        public CarritoItem? ObtenerItem(int productoId)
        {
            return Items.FirstOrDefault(i => i.ProductoId == productoId);
        }

        /// <summary>
        /// Método factory para crear un nuevo carrito
        /// </summary>
        public static Carrito Crear(string usuarioId)
        {
            var carrito = new Carrito
            {
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            // Publicar evento de creación
            carrito.RaiseDomainEvent(new CarritoCreado(usuarioId, carrito.FechaCreacion));

            return carrito;
        }

        /// <summary>
        /// Verifica si el carrito está abandonado
        /// </summary>
        public void VerificarAbandonado(TimeSpan tiempoLimite)
        {
            var tiempoSinActividad = DateTime.UtcNow - FechaActualizacion;
            
            if (tiempoSinActividad > tiempoLimite && TieneItems())
            {
                RaiseDomainEvent(new CarritoAbandonado(
                    UsuarioId, CantidadItems, Total, FechaActualizacion, tiempoSinActividad));
            }
        }
    }
}
