using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Adapters.Secondary
{
    public class InMemoryProductoRepository : IProductoRepository
    {
        private static readonly List<Producto> _productos = new()
        {
            new Producto { Id = 1, Nombre = "Laptop Dell XPS 13", Descripcion = "Laptop premium Dell XPS 13", Precio = 1299.99m, Stock = 10, Categoria = "Electrónicos" },
            new Producto { Id = 2, Nombre = "Mouse Logitech MX Master", Descripcion = "Mouse inalámbrico Logitech MX Master", Precio = 89.99m, Stock = 25, Categoria = "Accesorios" },
            new Producto { Id = 3, Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado mecánico con iluminación RGB", Precio = 149.99m, Stock = 15, Categoria = "Accesorios" },
            new Producto { Id = 4, Nombre = "Monitor 4K 27\"", Descripcion = "Monitor 4K de 27 pulgadas", Precio = 399.99m, Stock = 8, Categoria = "Electrónicos" }
        };
        private static int _nextId = 5;

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
                .Where(p => p.Categoria.Contains(categoria, StringComparison.OrdinalIgnoreCase))
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
