using CarritoComprasAPI.Core.Domain.ValueObjects;

namespace CarritoComprasAPI.Core.Domain
{
    /// <summary>
    /// Representa un elemento dentro del carrito de compras
    /// </summary>
    public class CarritoItem
    {
        /// <summary>
        /// Identificador único del item
        /// </summary>
        public int Id { get; internal set; }
        
        /// <summary>
        /// Identificador del producto
        /// </summary>
        public int ProductoId { get; internal set; }
        
        /// <summary>
        /// Referencia al producto
        /// </summary>
        public Producto? Producto { get; internal set; }
        
        /// <summary>
        /// Cantidad del producto en el carrito
        /// </summary>
        public Cantidad CantidadItem { get; internal set; } = Cantidad.Crear(1);
        
        /// <summary>
        /// Precio unitario del producto
        /// </summary>
        public Precio PrecioUnitario { get; internal set; } = Precio.Crear(1.0m);
        
        /// <summary>
        /// Subtotal calculado (cantidad * precio unitario)
        /// </summary>
        public decimal Subtotal => CantidadItem.Value * PrecioUnitario.Value;
        
        /// <summary>
        /// Fecha cuando se agregó el item al carrito
        /// </summary>
        public DateTime FechaAgregado { get; internal set; } = DateTime.UtcNow;

        /// <summary>
        /// Actualiza la cantidad del item en el carrito
        /// </summary>
        /// <param name="nuevaCantidad">Nueva cantidad (debe ser mayor a 0)</param>
        /// <exception cref="ArgumentException">Lanzado cuando la cantidad es menor o igual a 0</exception>
        public void ActualizarCantidad(int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(nuevaCantidad));
            
            CantidadItem = Cantidad.Crear(nuevaCantidad);
        }

        /// <summary>
        /// Actualiza el precio unitario del item
        /// </summary>
        /// <param name="nuevoPrecio">Nuevo precio unitario (debe ser mayor a 0)</param>
        /// <exception cref="ArgumentException">Lanzado cuando el precio es menor o igual a 0</exception>
        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0", nameof(nuevoPrecio));
            
            PrecioUnitario = Precio.Crear(nuevoPrecio);
        }

        /// <summary>
        /// Verifica si este item corresponde al producto especificado
        /// </summary>
        /// <param name="productoId">Identificador del producto a verificar</param>
        /// <returns>True si el item corresponde al producto, false en caso contrario</returns>
        public bool EsDelProducto(int productoId)
        {
            return ProductoId == productoId;
        }
    }
}
