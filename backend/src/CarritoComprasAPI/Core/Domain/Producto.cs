using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Domain.Events.Productos;
using CarritoComprasAPI.Core.Domain.ValueObjects;
using System.Globalization;

namespace CarritoComprasAPI.Core.Domain
{
    public class Producto : DomainEntity
    {
        public int Id { get; internal set; }
        public ProductoNombre Nombre { get; internal set; } = ProductoNombre.Crear("Producto");
        public string Descripcion { get; internal set; } = string.Empty;
        public Precio PrecioProducto { get; internal set; } = Precio.Crear(1.0m);
        public Stock StockProducto { get; internal set; } = Stock.Crear(0);
        public Categoria CategoriaProducto { get; internal set; } = Categoria.Crear("General");
        
        public DateTime FechaCreacion { get; internal set; } = DateTime.UtcNow;

        // Métodos de dominio
        public bool TieneStock(int cantidad = 1)
        {
            return StockProducto.TieneCantidad(cantidad);
        }

        public void ReducirStock(int cantidad)
        {
            if (!TieneStock(cantidad))
                throw new InvalidOperationException($"Stock insuficiente. Stock actual: {StockProducto.Value}, cantidad solicitada: {cantidad}");
            
            var stockAnterior = StockProducto.Value;
            StockProducto = StockProducto.Reducir(cantidad);

            // Publicar evento de cambio de stock
            RaiseDomainEvent(new StockProductoCambiado(
                Id, Nombre.Value, stockAnterior, StockProducto.Value, "Reducción por venta"));

            // Publicar evento si se queda sin stock
            if (StockProducto.Value == 0)
            {
                RaiseDomainEvent(new ProductoSinStock(Id, Nombre.Value, CategoriaProducto.Value));
            }
        }

        public void AumentarStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));
            
            var stockAnterior = StockProducto.Value;
            StockProducto = StockProducto.Aumentar(cantidad);

            // Publicar evento de cambio de stock
            RaiseDomainEvent(new StockProductoCambiado(
                Id, Nombre.Value, stockAnterior, StockProducto.Value, "Aumento de inventario"));
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            var nuevoPrecioVO = Precio.Crear(nuevoPrecio); // Validación automática
            
            var precioAnterior = PrecioProducto.Value;
            PrecioProducto = nuevoPrecioVO;

            // Calcular porcentaje de cambio
            var porcentajeCambio = precioAnterior > 0 
                ? ((nuevoPrecio - precioAnterior) / precioAnterior) * 100 
                : 0;

            // Publicar evento de cambio de precio
            RaiseDomainEvent(new PrecioProductoCambiado(
                Id, Nombre.Value, precioAnterior, nuevoPrecio, porcentajeCambio));
        }

        /// <summary>
        /// Actualiza información básica del producto
        /// </summary>
        public void ActualizarInformacion(string nombre, string descripcion, string categoria)
        {
            // Las validaciones ahora están en los Value Objects
            var nuevoNombre = ProductoNombre.Crear(nombre);
            var nuevaCategoria = Categoria.Crear(categoria);
            
            // Validación adicional para descripción
            if (descripcion?.Length > 500)
                throw new ArgumentException("La descripción no puede exceder 500 caracteres", nameof(descripcion));

            Nombre = nuevoNombre;
            Descripcion = descripcion ?? string.Empty;
            CategoriaProducto = nuevaCategoria;
        }

        /// <summary>
        /// Método factory para crear un nuevo producto
        /// </summary>
        public static Producto Crear(string nombre, string descripcion, decimal precio, int stock, string categoria)
        {
            var producto = new Producto
            {
                Nombre = ProductoNombre.Crear(nombre),
                Descripcion = descripcion ?? string.Empty,
                PrecioProducto = Precio.Crear(precio),
                StockProducto = Stock.Crear(stock),
                CategoriaProducto = Categoria.Crear(categoria),
                FechaCreacion = DateTime.UtcNow
            };

            // Publicar evento de creación
            producto.RaiseDomainEvent(new ProductoCreado(
                producto.Id, 
                producto.Nombre.Value, 
                producto.Descripcion, 
                producto.PrecioProducto.Value, 
                producto.StockProducto.Value, 
                producto.CategoriaProducto.Value, 
                producto.FechaCreacion));

            return producto;
        }

        /// <summary>
        /// Método para marcar el producto como eliminado
        /// </summary>
        public void MarcarComoEliminado()
        {
            RaiseDomainEvent(new ProductoEliminado(Id, Nombre.Value, CategoriaProducto.Value));
        }
    }
}
