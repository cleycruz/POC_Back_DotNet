using System.ComponentModel.DataAnnotations;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Domain.Events.Productos;

namespace CarritoComprasAPI.Core.Domain
{
    public class Producto : DomainEntity
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;
        
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
        
        [Required(ErrorMessage = "La categoría es requerida")]
        [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        public string Categoria { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Métodos de dominio
        public bool TieneStock(int cantidad = 1)
        {
            return Stock >= cantidad;
        }

        public void ReducirStock(int cantidad)
        {
            if (!TieneStock(cantidad))
                throw new InvalidOperationException($"Stock insuficiente. Stock actual: {Stock}, cantidad solicitada: {cantidad}");
            
            var stockAnterior = Stock;
            Stock -= cantidad;

            // Publicar evento de cambio de stock
            RaiseDomainEvent(new StockProductoCambiado(
                Id, Nombre, stockAnterior, Stock, "Reducción por venta"));

            // Publicar evento si se queda sin stock
            if (Stock == 0)
            {
                RaiseDomainEvent(new ProductoSinStock(Id, Nombre, Categoria));
            }
        }

        public void AumentarStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));
            
            var stockAnterior = Stock;
            Stock += cantidad;

            // Publicar evento de cambio de stock
            RaiseDomainEvent(new StockProductoCambiado(
                Id, Nombre, stockAnterior, Stock, "Aumento de inventario"));
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0", nameof(nuevoPrecio));
            
            var precioAnterior = Precio;
            Precio = nuevoPrecio;

            // Calcular porcentaje de cambio
            var porcentajeCambio = precioAnterior > 0 
                ? ((nuevoPrecio - precioAnterior) / precioAnterior) * 100 
                : 0;

            // Publicar evento de cambio de precio
            RaiseDomainEvent(new PrecioProductoCambiado(
                Id, Nombre, precioAnterior, nuevoPrecio, porcentajeCambio));
        }

        /// <summary>
        /// Método factory para crear un nuevo producto
        /// </summary>
        public static Producto Crear(string nombre, string descripcion, decimal precio, int stock, string categoria)
        {
            var producto = new Producto
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                Stock = stock,
                Categoria = categoria,
                FechaCreacion = DateTime.UtcNow
            };

            // Publicar evento de creación
            producto.RaiseDomainEvent(new ProductoCreado(
                producto.Id, nombre, descripcion, precio, stock, categoria, producto.FechaCreacion));

            return producto;
        }

        /// <summary>
        /// Método para marcar el producto como eliminado
        /// </summary>
        public void MarcarComoEliminado()
        {
            RaiseDomainEvent(new ProductoEliminado(Id, Nombre, Categoria));
        }
    }
}
