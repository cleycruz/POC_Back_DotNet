using CarritoComprasAPI.Core.Commands;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.Validators;

namespace CarritoComprasAPI.Core.Commands.Carrito
{
    // Command para agregar item al carrito
    public record AgregarItemCarritoCommand(
        string UsuarioId,
        int ProductoId,
        int Cantidad
    ) : ICommand<Domain.Carrito>;

    // Handler para agregar item al carrito
    public class AgregarItemCarritoCommandHandler : ICommandHandler<AgregarItemCarritoCommand, Domain.Carrito>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ICarritoBusinessValidator _businessValidator;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IAppLogger _logger;

        public AgregarItemCarritoCommandHandler(
            ICarritoRepository carritoRepository,
            IProductoRepository productoRepository,
            ICarritoBusinessValidator businessValidator,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _businessValidator = businessValidator ?? throw new ArgumentNullException(nameof(businessValidator));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Domain.Carrito> Handle(AgregarItemCarritoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Agregando {Cantidad} unidades del producto {ProductoId} al carrito del usuario {UsuarioId}",
                    command.Cantidad, command.ProductoId, command.UsuarioId);

                // Validaciones de negocio
                var businessValidation = await _businessValidator.ValidateAddItemAsync(
                    command.UsuarioId, command.ProductoId, command.Cantidad);
                
                if (!businessValidation.IsValid)
                {
                    throw new BusinessValidationException(businessValidation.Errors);
                }

                // Obtener producto
                var producto = await _productoRepository.ObtenerPorIdAsync(command.ProductoId);
                if (producto == null)
                {
                    throw new InvalidOperationException($"Producto con ID {command.ProductoId} no encontrado");
                }

                // Obtener o crear carrito
                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(command.UsuarioId);
                if (carrito == null)
                {
                    // Usar método factory que dispara el evento CarritoCreado
                    carrito = Domain.Carrito.Crear(command.UsuarioId);
                    carrito = await _carritoRepository.CrearAsync(carrito);
                    
                    // Despachar eventos de creación
                    await _eventDispatcher.DispatchAndClearEvents(carrito, cancellationToken);
                }

                // Validar stock disponible
                var cantidadEnCarrito = carrito.ObtenerItem(command.ProductoId)?.Cantidad ?? 0;
                var cantidadTotal = cantidadEnCarrito + command.Cantidad;

                if (!producto.TieneStock(cantidadTotal))
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad en carrito: {cantidadEnCarrito}, cantidad solicitada: {command.Cantidad}");
                }

                // Agregar item al carrito
                carrito.AgregarItem(producto, command.Cantidad);

                // Guardar carrito
                var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);

                // Despachar eventos de dominio
                await _eventDispatcher.DispatchAndClearEvents(carritoActualizado, cancellationToken);

                _logger.LogInformation("Item agregado exitosamente al carrito del usuario {UsuarioId}", command.UsuarioId);

                return carritoActualizado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar item al carrito. Usuario: {UsuarioId}, Producto: {ProductoId}, Cantidad: {Cantidad}",
                    command.UsuarioId, command.ProductoId, command.Cantidad);
                throw;
            }
        }
    }

    // Command para actualizar cantidad de item
    public record ActualizarCantidadItemCommand(
        string UsuarioId,
        int ProductoId,
        int Cantidad
    ) : ICommand<Domain.Carrito>;

    // Handler para actualizar cantidad
    public class ActualizarCantidadItemCommandHandler : ICommandHandler<ActualizarCantidadItemCommand, Domain.Carrito>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IAppLogger _logger;

        public ActualizarCantidadItemCommandHandler(
            ICarritoRepository carritoRepository,
            IProductoRepository productoRepository,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Domain.Carrito> Handle(ActualizarCantidadItemCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Actualizando cantidad del producto {ProductoId} a {Cantidad} en el carrito del usuario {UsuarioId}",
                    command.ProductoId, command.Cantidad, command.UsuarioId);

                ValidarCommand(command);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(command.UsuarioId);
                if (carrito == null)
                {
                    throw new InvalidOperationException($"Carrito no encontrado para el usuario {command.UsuarioId}");
                }

                if (command.Cantidad <= 0)
                {
                    // Si la cantidad es 0 o negativa, eliminar el item
                    carrito.EliminarItem(command.ProductoId);
                }
                else
                {
                    // Validar que el producto existe
                    var producto = await _productoRepository.ObtenerPorIdAsync(command.ProductoId);
                    if (producto == null)
                    {
                        throw new InvalidOperationException($"Producto con ID {command.ProductoId} no encontrado");
                    }

                    // Validar stock
                    if (!producto.TieneStock(command.Cantidad))
                    {
                        throw new InvalidOperationException(
                            $"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad solicitada: {command.Cantidad}");
                    }

                    // Actualizar cantidad
                    carrito.ActualizarCantidadItem(command.ProductoId, command.Cantidad);
                }

                var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);

                // Despachar eventos de dominio
                await _eventDispatcher.DispatchAndClearEvents(carritoActualizado, cancellationToken);

                _logger.LogInformation("Cantidad actualizada exitosamente en el carrito del usuario {UsuarioId}", command.UsuarioId);

                return carritoActualizado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cantidad en carrito. Usuario: {UsuarioId}, Producto: {ProductoId}, Cantidad: {Cantidad}",
                    command.UsuarioId, command.ProductoId, command.Cantidad);
                throw;
            }
        }

        private static void ValidarCommand(ActualizarCantidadItemCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }

    // Command para eliminar item del carrito
    public record EliminarItemCarritoCommand(
        string UsuarioId,
        int ProductoId
    ) : ICommand<bool>;

    // Handler para eliminar item
    public class EliminarItemCarritoCommandHandler : ICommandHandler<EliminarItemCarritoCommand, bool>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IAppLogger _logger;

        public EliminarItemCarritoCommandHandler(
            ICarritoRepository carritoRepository,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(EliminarItemCarritoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Eliminando producto {ProductoId} del carrito del usuario {UsuarioId}",
                    command.ProductoId, command.UsuarioId);

                ValidarCommand(command);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(command.UsuarioId);
                if (carrito == null)
                {
                    _logger.LogWarning("Carrito no encontrado para el usuario {UsuarioId}", command.UsuarioId);
                    return false;
                }

                carrito.EliminarItem(command.ProductoId);
                var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);

                // Despachar eventos de dominio
                await _eventDispatcher.DispatchAndClearEvents(carritoActualizado, cancellationToken);

                _logger.LogInformation("Item eliminado exitosamente del carrito del usuario {UsuarioId}", command.UsuarioId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar item del carrito. Usuario: {UsuarioId}, Producto: {ProductoId}",
                    command.UsuarioId, command.ProductoId);
                throw;
            }
        }

        private static void ValidarCommand(EliminarItemCarritoCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }

    // Command para vaciar carrito
    public record VaciarCarritoCommand(string UsuarioId) : ICommand<bool>;

    // Handler para vaciar carrito
    public class VaciarCarritoCommandHandler : ICommandHandler<VaciarCarritoCommand, bool>
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IAppLogger _logger;

        public VaciarCarritoCommandHandler(
            ICarritoRepository carritoRepository,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(VaciarCarritoCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Vaciando carrito del usuario {UsuarioId}", command.UsuarioId);

                ValidarCommand(command);

                var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(command.UsuarioId);
                if (carrito == null)
                {
                    _logger.LogWarning("Carrito no encontrado para el usuario {UsuarioId}", command.UsuarioId);
                    return false;
                }

                carrito.Vaciar();
                var carritoActualizado = await _carritoRepository.ActualizarAsync(carrito);

                // Despachar eventos de dominio
                await _eventDispatcher.DispatchAndClearEvents(carritoActualizado, cancellationToken);

                _logger.LogInformation("Carrito vaciado exitosamente para el usuario {UsuarioId}", command.UsuarioId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al vaciar carrito del usuario: {UsuarioId}", command.UsuarioId);
                throw;
            }
        }

        private static void ValidarCommand(VaciarCarritoCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.UsuarioId))
                throw new ArgumentException("El ID de usuario es requerido");
        }
    }
}
