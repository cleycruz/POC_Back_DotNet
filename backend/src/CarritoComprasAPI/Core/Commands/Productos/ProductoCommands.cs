using CarritoComprasAPI.Core.Commands;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.Validators;
using CarritoComprasAPI.Core.UseCases;
using System.Globalization;

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
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IProductoBusinessValidator _businessValidator;
        private readonly IAppLogger _logger;

        public CrearProductoCommandHandler(
            IProductoRepository repository, 
            IDomainEventDispatcher eventDispatcher,
            IProductoBusinessValidator businessValidator,
            IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _businessValidator = businessValidator ?? throw new ArgumentNullException(nameof(businessValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Producto> Handle(CrearProductoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creando producto: {Nombre}", command.Nombre);

                // Validaciones de negocio
                var businessValidation = await _businessValidator.ValidateCreateAsync(
                    command.Nombre, command.Categoria, command.Precio, command.Stock);
                
                if (!businessValidation.IsValid)
                {
                    throw new BusinessValidationException(businessValidation.Errors);
                }

                // Crear entidad de dominio usando método factory
                var producto = Producto.Crear(
                    command.Nombre,
                    command.Descripcion,
                    command.Precio,
                    command.Stock,
                    command.Categoria);

                // Persistir
                var productoCreado = await _repository.CrearAsync(producto);
                
                // Despachar eventos de dominio
                await _eventDispatcher.DispatchAndClearEvents(productoCreado, cancellationToken);
                
                _logger.LogInformation("Producto creado exitosamente con ID: {ProductoId}", productoCreado.Id);
                
                return productoCreado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto: {Nombre}", command.Nombre);
                throw;
            }
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
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IProductoBusinessValidator _businessValidator;
        private readonly IAppLogger _logger;

        public ActualizarProductoCommandHandler(
            IProductoRepository repository, 
            IDomainEventDispatcher eventDispatcher,
            IProductoBusinessValidator businessValidator,
            IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _businessValidator = businessValidator ?? throw new ArgumentNullException(nameof(businessValidator));
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

                // Preparar valores para validación (usar valores actuales si no se proporcionan nuevos)
                var nombre = command.Nombre ?? productoExistente.Nombre.Value;
                var categoria = command.Categoria ?? productoExistente.CategoriaProducto.Value;
                var precio = command.Precio ?? productoExistente.PrecioProducto.Value;
                var stock = command.Stock ?? productoExistente.StockProducto.Value;

                // Validaciones de negocio
                var businessValidation = await _businessValidator.ValidateUpdateAsync(
                    command.Id, nombre, categoria, precio, stock);
                
                if (!businessValidation.IsValid)
                {
                    throw new BusinessValidationException(businessValidation.Errors);
                }

                // Aplicar cambios solo si se proporcionan
                if (!string.IsNullOrEmpty(command.Nombre))
                    productoExistente.ActualizarInformacion(command.Nombre, productoExistente.Descripcion, productoExistente.CategoriaProducto.Value);
                
                if (!string.IsNullOrEmpty(command.Descripcion))
                    productoExistente.ActualizarInformacion(productoExistente.Nombre.Value, command.Descripcion, productoExistente.CategoriaProducto.Value);
                
                if (command.Precio.HasValue && command.Precio > 0)
                    productoExistente.ActualizarPrecio(command.Precio.Value);
                
                if (command.Stock.HasValue && command.Stock >= 0)
                {
                    if (command.Stock.Value > productoExistente.StockProducto.Value)
                        productoExistente.AumentarStock(command.Stock.Value - productoExistente.StockProducto.Value);
                    else if (command.Stock.Value < productoExistente.StockProducto.Value)
                        productoExistente.ReducirStock(productoExistente.StockProducto.Value - command.Stock.Value);
                }
                
                if (!string.IsNullOrEmpty(command.Categoria))
                    productoExistente.ActualizarInformacion(productoExistente.Nombre.Value, productoExistente.Descripcion, command.Categoria);

                var productoActualizado = await _repository.ActualizarAsync(productoExistente);
                
                if (productoActualizado == null)
                {
                    throw new InvalidOperationException($"Error al actualizar el producto con ID {command.Id}");
                }
                
                // Despachar eventos de dominio
                await _eventDispatcher.DispatchAndClearEvents(productoActualizado, cancellationToken);
                
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
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IProductoBusinessValidator _businessValidator;
        private readonly IAppLogger _logger;

        public EliminarProductoCommandHandler(
            IProductoRepository repository, 
            IDomainEventDispatcher eventDispatcher,
            IProductoBusinessValidator businessValidator,
            IAppLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _businessValidator = businessValidator ?? throw new ArgumentNullException(nameof(businessValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(EliminarProductoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Eliminando producto con ID: {ProductoId}", command.Id);

                // Validaciones de negocio
                var businessValidation = await _businessValidator.ValidateDeleteAsync(command.Id);
                if (!businessValidation.IsValid)
                {
                    throw new BusinessValidationException(businessValidation.Errors);
                }

                // Obtener el producto antes de eliminarlo para capturar sus datos
                var producto = await _repository.ObtenerPorIdAsync(command.Id);
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", command.Id);
                    return false;
                }

                // Marcar como eliminado para generar evento
                producto.MarcarComoEliminado();

                var resultado = await _repository.EliminarAsync(command.Id);
                
                if (resultado)
                {
                    // Despachar eventos de dominio
                    await _eventDispatcher.DispatchAndClearEvents(producto, cancellationToken);
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
