using CarritoComprasAPI.Core.Commands;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.Commands.Productos
{
    // Command para crear producto
    public record CrearProductoCommand(
        string Nombre,
        string Descripcion,
        decimal Precio,
        int Stock,
        string Categoria
    ) : ICommand<Producto>;

    // Handler para crear producto
    public class CrearProductoCommandHandler : ICommandHandler<CrearProductoCommand, Producto>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public CrearProductoCommandHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Producto> Handle(CrearProductoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creando producto: {Nombre}", command.Nombre);

                // Validaciones de negocio
                ValidarCommand(command);

                // Crear entidad de dominio
                var producto = new Producto
                {
                    Nombre = command.Nombre,
                    Descripcion = command.Descripcion,
                    Precio = command.Precio,
                    Stock = command.Stock,
                    Categoria = command.Categoria
                };

                // Persistir
                var productoCreado = await _repository.CrearAsync(producto);
                
                _logger.LogInformation("Producto creado exitosamente con ID: {ProductoId}", productoCreado.Id);
                
                return productoCreado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto: {Nombre}", command.Nombre);
                throw;
            }
        }

        private static void ValidarCommand(CrearProductoCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Nombre))
                throw new ArgumentException("El nombre del producto es requerido");

            if (command.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0");

            if (command.Stock < 0)
                throw new ArgumentException("El stock no puede ser negativo");

            if (string.IsNullOrWhiteSpace(command.Categoria))
                throw new ArgumentException("La categoría del producto es requerida");
        }
    }

    // Command para actualizar producto
    public record ActualizarProductoCommand(
        int Id,
        string? Nombre,
        string? Descripcion,
        decimal? Precio,
        int? Stock,
        string? Categoria
    ) : ICommand<Producto?>;

    // Handler para actualizar producto
    public class ActualizarProductoCommandHandler : ICommandHandler<ActualizarProductoCommand, Producto?>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public ActualizarProductoCommandHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Producto?> Handle(ActualizarProductoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Actualizando producto con ID: {ProductoId}", command.Id);

                var productoExistente = await _repository.ObtenerPorIdAsync(command.Id);
                if (productoExistente == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", command.Id);
                    return null;
                }

                // Aplicar cambios solo si se proporcionan
                if (!string.IsNullOrEmpty(command.Nombre))
                    productoExistente.Nombre = command.Nombre;
                
                if (!string.IsNullOrEmpty(command.Descripcion))
                    productoExistente.Descripcion = command.Descripcion;
                
                if (command.Precio.HasValue && command.Precio > 0)
                    productoExistente.ActualizarPrecio(command.Precio.Value);
                
                if (command.Stock.HasValue && command.Stock >= 0)
                    productoExistente.Stock = command.Stock.Value;
                
                if (!string.IsNullOrEmpty(command.Categoria))
                    productoExistente.Categoria = command.Categoria;

                var productoActualizado = await _repository.ActualizarAsync(productoExistente);
                
                _logger.LogInformation("Producto {ProductoId} actualizado exitosamente", command.Id);
                
                return productoActualizado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {ProductoId}", command.Id);
                throw;
            }
        }
    }

    // Command para eliminar producto
    public record EliminarProductoCommand(int Id) : ICommand<bool>;

    // Handler para eliminar producto
    public class EliminarProductoCommandHandler : ICommandHandler<EliminarProductoCommand, bool>
    {
        private readonly IProductoRepository _repository;
        private readonly IAppLogger _logger;

        public EliminarProductoCommandHandler(IProductoRepository repository, IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(EliminarProductoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Eliminando producto con ID: {ProductoId}", command.Id);

                var existe = await _repository.ExisteAsync(command.Id);
                if (!existe)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", command.Id);
                    return false;
                }

                var resultado = await _repository.EliminarAsync(command.Id);
                
                if (resultado)
                {
                    _logger.LogInformation("Producto {ProductoId} eliminado exitosamente", command.Id);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {ProductoId}", command.Id);
                throw;
            }
        }
    }
}
