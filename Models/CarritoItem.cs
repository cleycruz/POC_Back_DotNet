namespace CarritoComprasAPI.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
    }
}
