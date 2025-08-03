using CarritoComprasAPI.Models;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> ObtenerTodosAsync();
        Task<ProductoDto?> ObtenerPorIdAsync(int id);
        Task<ProductoDto> CrearAsync(CrearProductoDto productoDto);
        Task<ProductoDto?> ActualizarAsync(int id, ActualizarProductoDto productoDto);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<ProductoDto>> BuscarPorCategoriaAsync(string categoria);
    }

    public class ProductoService : IProductoService
    {
        private static readonly List<Producto> _productos = new()
        {
            new Producto { Id = 1, Nombre = "Laptop Dell", Descripcion = "Laptop Dell XPS 13", Precio = 1299.99m, Stock = 10, Categoria = "Electrónicos" },
            new Producto { Id = 2, Nombre = "Mouse Logitech", Descripcion = "Mouse inalámbrico Logitech MX Master", Precio = 89.99m, Stock = 25, Categoria = "Accesorios" },
            new Producto { Id = 3, Nombre = "Teclado Mecánico", Descripcion = "Teclado mecánico RGB", Precio = 149.99m, Stock = 15, Categoria = "Accesorios" },
            new Producto { Id = 4, Nombre = "Monitor 4K", Descripcion = "Monitor 4K 27 pulgadas", Precio = 399.99m, Stock = 8, Categoria = "Electrónicos" }
        };
        private static int _nextId = 5;

        public Task<IEnumerable<ProductoDto>> ObtenerTodosAsync()
        {
            var productosDto = _productos.Select(MapearADto);
            return Task.FromResult(productosDto);
        }

        public Task<ProductoDto?> ObtenerPorIdAsync(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(producto != null ? MapearADto(producto) : null);
        }

        public Task<ProductoDto> CrearAsync(CrearProductoDto productoDto)
        {
            var producto = new Producto
            {
                Id = _nextId++,
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                Precio = productoDto.Precio,
                Stock = productoDto.Stock,
                Categoria = productoDto.Categoria,
                FechaCreacion = DateTime.UtcNow
            };

            _productos.Add(producto);
            return Task.FromResult(MapearADto(producto));
        }

        public Task<ProductoDto?> ActualizarAsync(int id, ActualizarProductoDto productoDto)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            if (producto == null) return Task.FromResult<ProductoDto?>(null);

            if (!string.IsNullOrEmpty(productoDto.Nombre))
                producto.Nombre = productoDto.Nombre;
            if (!string.IsNullOrEmpty(productoDto.Descripcion))
                producto.Descripcion = productoDto.Descripcion;
            if (productoDto.Precio.HasValue)
                producto.Precio = productoDto.Precio.Value;
            if (productoDto.Stock.HasValue)
                producto.Stock = productoDto.Stock.Value;
            if (!string.IsNullOrEmpty(productoDto.Categoria))
                producto.Categoria = productoDto.Categoria;

            return Task.FromResult<ProductoDto?>(MapearADto(producto));
        }

        public Task<bool> EliminarAsync(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            if (producto == null) return Task.FromResult(false);

            _productos.Remove(producto);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ProductoDto>> BuscarPorCategoriaAsync(string categoria)
        {
            var productos = _productos
                .Where(p => p.Categoria.Contains(categoria, StringComparison.OrdinalIgnoreCase))
                .Select(MapearADto);
            return Task.FromResult(productos);
        }

        private static ProductoDto MapearADto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Categoria = producto.Categoria
            };
        }
    }
}
