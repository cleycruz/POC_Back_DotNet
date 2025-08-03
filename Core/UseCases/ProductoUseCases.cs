using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.UseCases
{
    public class ProductoUseCases : IProductoUseCases
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IAppLogger _logger;

        public ProductoUseCases(IProductoRepository productoRepository, IAppLogger logger)
        {
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los productos");
                var productos = await _productoRepository.ObtenerTodosAsync();
                _logger.LogInformation("Se obtuvieron {Count} productos", productos.Count());
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                throw;
            }
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo producto con ID: {ProductoId}", id);
                
                if (id <= 0)
                {
                    _logger.LogWarning("ID de producto inválido: {ProductoId}", id);
                    return null;
                }

                var producto = await _productoRepository.ObtenerPorIdAsync(id);
                
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", id);
                }
                else
                {
                    _logger.LogInformation("Producto {ProductoNombre} encontrado", producto.Nombre.Value);
                }

                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {ProductoId}", id);
                throw;
            }
        }

        public async Task<Producto> CrearAsync(Producto producto)
        {
            try
            {
                _logger.LogInformation("Creando nuevo producto: {ProductoNombre}", producto.Nombre.Value);
                
                // Las validaciones ahora están en los Value Objects del producto
                
                var productoCreado = await _productoRepository.CrearAsync(producto);
                _logger.LogInformation("Producto creado exitosamente con ID: {ProductoId}", productoCreado.Id);
                
                return productoCreado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto: {ProductoNombre}", producto.Nombre.Value);
                throw;
            }
        }

        public async Task<Producto?> ActualizarAsync(int id, Producto productoActualizado)
        {
            try
            {
                _logger.LogInformation("Actualizando producto con ID: {ProductoId}", id);
                
                var productoExistente = await _productoRepository.ObtenerPorIdAsync(id);
                if (productoExistente == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado para actualizar", id);
                    return null;
                }

                // Las validaciones ahora están en los Value Objects del producto
                
                // Mantener el ID y fecha de creación originales
                productoActualizado.Id = id;
                productoActualizado.FechaCreacion = productoExistente.FechaCreacion;
                
                var resultado = await _productoRepository.ActualizarAsync(productoActualizado);
                _logger.LogInformation("Producto {ProductoId} actualizado exitosamente", id);
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {ProductoId}", id);
                throw;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando producto con ID: {ProductoId}", id);
                
                var existe = await _productoRepository.ExisteAsync(id);
                if (!existe)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado para eliminar", id);
                    return false;
                }

                var resultado = await _productoRepository.EliminarAsync(id);
                
                if (resultado)
                {
                    _logger.LogInformation("Producto {ProductoId} eliminado exitosamente", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar el producto {ProductoId}", id);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {ProductoId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Producto>> BuscarPorCategoriaAsync(string categoria)
        {
            try
            {
                _logger.LogInformation("Buscando productos por categoría: {Categoria}", categoria);
                
                if (string.IsNullOrWhiteSpace(categoria))
                {
                    _logger.LogWarning("Categoría de búsqueda vacía o nula");
                    return Enumerable.Empty<Producto>();
                }

                var productos = await _productoRepository.BuscarPorCategoriaAsync(categoria);
                _logger.LogInformation("Se encontraron {Count} productos en la categoría {Categoria}", 
                    productos.Count(), categoria);
                
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos por categoría: {Categoria}", categoria);
                throw;
            }
        }

    }
}
