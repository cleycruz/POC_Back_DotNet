using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Adapters.Secondary
{
    public class InMemoryProductoRepository : IProductoRepository
    {
        private static readonly List<Producto> _productos;
        private static int _nextId = 5;

        static InMemoryProductoRepository()
        {
            var productos = new List<Producto>
            {
                Producto.Crear("Laptop Dell XPS 13", "Laptop premium Dell XPS 13", 1299.99m, 10, "Electrónicos"),
                Producto.Crear("Mouse Logitech MX Master", "Mouse inalámbrico Logitech MX Master", 89.99m, 25, "Accesorios"),
                Producto.Crear("Teclado Mecánico RGB", "Teclado mecánico con iluminación RGB", 149.99m, 15, "Accesorios"),
                Producto.Crear("Monitor 4K 27\"", "Monitor 4K de 27 pulgadas", 399.99m, 8, "Electrónicos")
            };

            // Asignar IDs secuenciales para el repositorio en memoria
            for (int i = 0; i < productos.Count; i++)
            {
                productos[i].Id = i + 1;
            }

            _productos = productos;
        }

        public Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return Task.FromResult(_productos.AsEnumerable());
        }

        public Task<Producto?> ObtenerPorIdAsync(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(producto);
        }

        public Task<Producto> CrearAsync(Producto producto)
        {
            producto.Id = _nextId++;
            producto.FechaCreacion = DateTime.UtcNow;
            _productos.Add(producto);
            return Task.FromResult(producto);
        }

        public Task<Producto?> ActualizarAsync(Producto producto)
        {
            var index = _productos.FindIndex(p => p.Id == producto.Id);
            if (index == -1)
                return Task.FromResult<Producto?>(null);

            _productos[index] = producto;
            return Task.FromResult<Producto?>(producto);
        }

        public Task<bool> EliminarAsync(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return Task.FromResult(false);

            _productos.Remove(producto);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Producto>> BuscarPorCategoriaAsync(string categoria)
        {
            var productos = _productos
                .Where(p => p.CategoriaProducto.Value.Contains(categoria, StringComparison.OrdinalIgnoreCase))
                .AsEnumerable();
            return Task.FromResult(productos);
        }

        public Task<bool> ExisteAsync(int id)
        {
            var existe = _productos.Any(p => p.Id == id);
            return Task.FromResult(existe);
        }
    }
}
