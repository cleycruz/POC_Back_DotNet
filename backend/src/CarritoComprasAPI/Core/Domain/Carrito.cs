using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Domain.Events.Carrito;
using CarritoComprasAPI.Core.Domain.ValueObjects;
using System.Globalization;

namespace CarritoComprasAPI.Core.Domain
{
    public class Carrito : DomainEntity
    {
        private readonly List<CarritoItem> _items = new();
        
        public int Id { get; internal set; }
        public UsuarioId UsuarioCarrito { get; internal set; } = UsuarioId.Crear("user");
        public IReadOnlyList<CarritoItem> Items => _items.AsReadOnly();
        
        public decimal Total => _items.Sum(item => item.Subtotal);
        public int CantidadItems => _items.Count;
        public int CantidadProductos => _items.Sum(item => item.CantidadItem.Value);
        
        public DateTime FechaCreacion { get; internal set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; internal set; } = DateTime.UtcNow;

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
                    UsuarioCarrito.Value, producto.Id, producto.Nombre.Value, cantidad, producto.StockProducto.Value));
                throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre.Value}");
            }

            var totalAnterior = Total;
            var itemExistente = _items.FirstOrDefault(i => i.ProductoId == producto.Id);
            
            if (itemExistente != null)
            {
                var cantidadAnterior = itemExistente.CantidadItem.Value;
                var subtotalAnterior = itemExistente.Subtotal;
                itemExistente.ActualizarCantidad(itemExistente.CantidadItem.Value + cantidad);

                // Publicar evento de cantidad actualizada
                RaiseDomainEvent(new CantidadItemCarritoActualizada(
                    UsuarioCarrito.Value, producto.Id, producto.Nombre.Value, 
                    cantidadAnterior, itemExistente.CantidadItem.Value,
                    subtotalAnterior, itemExistente.Subtotal));
            }
            else
            {
                var nuevoItem = new CarritoItem
                {
                    ProductoId = producto.Id,
                    Producto = producto,
                    CantidadItem = Cantidad.Crear(cantidad),
                    PrecioUnitario = producto.PrecioProducto
                };
                _items.Add(nuevoItem);

                // Publicar evento de item agregado
                RaiseDomainEvent(new ItemAgregadoAlCarrito(
                    UsuarioCarrito.Value, producto.Id, producto.Nombre.Value, 
                    cantidad, producto.PrecioProducto.Value, nuevoItem.Subtotal));
            }

            FechaActualizacion = DateTime.UtcNow;

            // Publicar evento de total actualizado
            RaiseDomainEvent(new TotalCarritoActualizado(
                UsuarioCarrito.Value, totalAnterior, Total, CantidadItems));
        }

        public void ActualizarCantidadItem(int productoId, int nuevaCantidad)
        {
            var item = _items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
                throw new InvalidOperationException($"El producto con ID {productoId} no está en el carrito");

            if (nuevaCantidad <= 0)
            {
                EliminarItem(productoId);
                return;
            }

            var totalAnterior = Total;
            var cantidadAnterior = item.CantidadItem.Value;
            var subtotalAnterior = item.Subtotal;
            
            item.ActualizarCantidad(nuevaCantidad);
            FechaActualizacion = DateTime.UtcNow;

            // Publicar evento de cantidad actualizada
            RaiseDomainEvent(new CantidadItemCarritoActualizada(
                UsuarioCarrito.Value, productoId, item.Producto?.Nombre ?? "Producto desconocido",
                cantidadAnterior, nuevaCantidad, subtotalAnterior, item.Subtotal));

            // Publicar evento de total actualizado
            RaiseDomainEvent(new TotalCarritoActualizado(
                UsuarioCarrito.Value, totalAnterior, Total, CantidadItems));
        }

        public void EliminarItem(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                var totalAnterior = Total;
                var nombreProducto = item.Producto?.Nombre.Value ?? "Producto desconocido";
                var cantidad = item.CantidadItem.Value;
                var subtotalPerdido = item.Subtotal;

                _items.Remove(item);
                FechaActualizacion = DateTime.UtcNow;

                // Publicar evento de item eliminado
                RaiseDomainEvent(new ItemEliminadoDelCarrito(
                    UsuarioCarrito.Value, productoId, nombreProducto, cantidad, subtotalPerdido));

                // Publicar evento de total actualizado
                RaiseDomainEvent(new TotalCarritoActualizado(
                    UsuarioCarrito.Value, totalAnterior, Total, CantidadItems));
            }
        }

        public void Vaciar()
        {
            if (!_items.Any()) return;

            var cantidadItemsEliminados = _items.Count;
            var totalPerdido = Total;

            _items.Clear();
            FechaActualizacion = DateTime.UtcNow;

            // Publicar evento de carrito vaciado
            RaiseDomainEvent(new CarritoVaciado(
                UsuarioCarrito.Value, cantidadItemsEliminados, totalPerdido));
        }

        public bool TieneItems()
        {
            return _items.Any();
        }

        public CarritoItem? ObtenerItem(int productoId)
        {
            return _items.FirstOrDefault(i => i.ProductoId == productoId);
        }

        /// <summary>
        /// Método factory para crear un nuevo carrito
        /// </summary>
        public static Carrito Crear(string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentException("El ID de usuario es requerido", nameof(usuarioId));

            var carrito = new Carrito
            {
                UsuarioCarrito = UsuarioId.Crear(usuarioId),
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
                    UsuarioCarrito.Value, CantidadItems, Total, FechaActualizacion, tiempoSinActividad));
            }
        }
    }
}
