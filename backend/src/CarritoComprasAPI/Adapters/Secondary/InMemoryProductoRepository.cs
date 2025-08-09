using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using System.Globalization;

namespace CarritoComprasAPI.Adapters.Secondary
{
    /// <summary>
    /// Implementación en memoria del repositorio de productos para pruebas y desarrollo
    /// </summary>
    public class InMemoryProductoRepository : IProductoRepository
    {
        private static readonly List<Producto> _productos;
        private static int _nextId = 5;

        /// <summary>
        /// Constructor estático que inicializa el repositorio con datos de prueba
        /// </summary>
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

        /// <summary>
        /// Obtiene todos los productos disponibles
        /// </summary>
        /// <returns>Una colección de todos los productos</returns>
        public Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return Task.FromResult(_productos.AsEnumerable());
        }

        /// <summary>
        /// Obtiene un producto por su identificador
        /// </summary>
        /// <param name="id">Identificador del producto</param>
        /// <returns>El producto encontrado o null si no existe</returns>
        public Task<Producto?> ObtenerPorIdAsync(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(producto);
        }

        /// <summary>
        /// Crea un nuevo producto en el repositorio
        /// </summary>
        /// <param name="producto">Producto a crear</param>
        /// <returns>El producto creado con su ID asignado</returns>
        public Task<Producto> CrearAsync(Producto producto)
        {
            producto.Id = _nextId++;
            producto.FechaCreacion = DateTime.UtcNow;
            _productos.Add(producto);
            return Task.FromResult(producto);
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="producto">Producto con los datos actualizados</param>
        /// <returns>El producto actualizado o null si no existe</returns>
        public Task<Producto?> ActualizarAsync(Producto producto)
        {
            var index = _productos.FindIndex(p => p.Id == producto.Id);
            if (index == -1)
                return Task.FromResult<Producto?>(null);

            _productos[index] = producto;
            return Task.FromResult<Producto?>(producto);
        }

        /// <summary>
        /// Elimina un producto del repositorio
        /// </summary>
        /// <param name="id">Identificador del producto a eliminar</param>
        /// <returns>True si se eliminó exitosamente, false si no existe</returns>
        public Task<bool> EliminarAsync(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return Task.FromResult(false);

            _productos.Remove(producto);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Busca productos por categoría
        /// </summary>
        /// <param name="categoria">Categoría a buscar (búsqueda insensible a mayúsculas)</param>
        /// <returns>Productos que pertenecen a la categoría especificada</returns>
        public Task<IEnumerable<Producto>> BuscarPorCategoriaAsync(string categoria)
        {
            var productos = _productos
                .Where(p => p.CategoriaProducto.Value.Contains(categoria, StringComparison.OrdinalIgnoreCase))
                .AsEnumerable();
            return Task.FromResult(productos);
        }

        /// <summary>
        /// Verifica si existe un producto con el ID especificado
        /// </summary>
        /// <param name="id">Identificador del producto</param>
        /// <returns>True si el producto existe, false en caso contrario</returns>
        public Task<bool> ExisteAsync(int id)
        {
            var existe = _productos.Any(p => p.Id == id);
            return Task.FromResult(existe);
        }
    }
}
