using CarritoComprasAPI.Core.Domain.ValueObjects;

namespace CarritoComprasAPI.Core.Domain
{
    public class CarritoItem
    {
        public int Id { get; internal set; }
        public int ProductoId { get; internal set; }
        public Producto? Producto { get; internal set; }
        public Cantidad CantidadItem { get; internal set; } = Cantidad.Crear(1);
        public Precio PrecioUnitario { get; internal set; } = Precio.Crear(1.0m);
        
        public decimal Subtotal => CantidadItem.Value * PrecioUnitario.Value;
        public DateTime FechaAgregado { get; internal set; } = DateTime.UtcNow;

        // Métodos de dominio
        public void ActualizarCantidad(int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(nuevaCantidad));
            
            CantidadItem = Cantidad.Crear(nuevaCantidad);
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0", nameof(nuevoPrecio));
            
            PrecioUnitario = Precio.Crear(nuevoPrecio);
        }

        public bool EsDelProducto(int productoId)
        {
            return ProductoId == productoId;
        }
    }
}
