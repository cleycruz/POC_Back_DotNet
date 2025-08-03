namespace CarritoComprasAPI.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
        public decimal Total => Items.Sum(item => item.Subtotal);
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}
