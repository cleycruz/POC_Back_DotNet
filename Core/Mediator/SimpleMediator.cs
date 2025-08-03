using CarritoComprasAPI.Core.Commands;
using CarritoComprasAPI.Core.Queries;
using CarritoComprasAPI.Core.Domain.Events;

namespace CarritoComprasAPI.Core.Mediator
{
    // Mediator/Dispatcher para CQRS
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    }

    // Implementación simple del mediator
    public class SimpleMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public SimpleMediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
            
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for command {commandType.Name}");
            }

            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found for command {commandType.Name}");
            }

            var result = await (Task<TResponse>)handleMethod.Invoke(handler, new object[] { command, cancellationToken })!;
            return result;
        }

        public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for query {queryType.Name}");
            }

            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found for query {queryType.Name}");
            }

            var result = await (Task<TResponse>)handleMethod.Invoke(handler, new object[] { query, cancellationToken })!;
            return result;
        }
    }

    // Extensions para registrar el mediator
    public static class MediatorExtensions
    {
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services)
        {
            services.AddScoped<IMediator, SimpleMediator>();
            return services;
        }

        public static IServiceCollection AddCqrsHandlers(this IServiceCollection services)
        {
            // Registrar Domain Event Handlers
            RegisterDomainEventHandlers(services);
            // Registrar Command Handlers
            services.AddScoped<Commands.Productos.CrearProductoCommandHandler>();
            services.AddScoped<Commands.Productos.ActualizarProductoCommandHandler>();
            services.AddScoped<Commands.Productos.EliminarProductoCommandHandler>();
            
            services.AddScoped<Commands.Carrito.AgregarItemCarritoCommandHandler>();
            services.AddScoped<Commands.Carrito.ActualizarCantidadItemCommandHandler>();
            services.AddScoped<Commands.Carrito.EliminarItemCarritoCommandHandler>();
            services.AddScoped<Commands.Carrito.VaciarCarritoCommandHandler>();

            // Registrar Query Handlers
            services.AddScoped<Queries.Productos.ObtenerTodosProductosQueryHandler>();
            services.AddScoped<Queries.Productos.ObtenerProductoPorIdQueryHandler>();
            services.AddScoped<Queries.Productos.BuscarProductosPorCategoriaQueryHandler>();
            services.AddScoped<Queries.Productos.BuscarProductosQueryHandler>();
            
            services.AddScoped<Queries.Carrito.ObtenerCarritoPorUsuarioQueryHandler>();
            services.AddScoped<Queries.Carrito.ObtenerTotalCarritoQueryHandler>();
            services.AddScoped<Queries.Carrito.ObtenerResumenCarritoQueryHandler>();
            services.AddScoped<Queries.Carrito.ObtenerItemsCarritoQueryHandler>();

            // Registrar handlers con sus interfaces
            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);

            return services;
        }

        private static void RegisterCommandHandlers(IServiceCollection services)
        {
            // Productos
            services.AddScoped<ICommandHandler<Commands.Productos.CrearProductoCommand, Core.Domain.Producto>>(
                provider => provider.GetRequiredService<Commands.Productos.CrearProductoCommandHandler>());
            
            services.AddScoped<ICommandHandler<Commands.Productos.ActualizarProductoCommand, Core.Domain.Producto?>>(
                provider => provider.GetRequiredService<Commands.Productos.ActualizarProductoCommandHandler>());
            
            services.AddScoped<ICommandHandler<Commands.Productos.EliminarProductoCommand, bool>>(
                provider => provider.GetRequiredService<Commands.Productos.EliminarProductoCommandHandler>());

            // Carrito
            services.AddScoped<ICommandHandler<Commands.Carrito.AgregarItemCarritoCommand, Core.Domain.Carrito>>(
                provider => provider.GetRequiredService<Commands.Carrito.AgregarItemCarritoCommandHandler>());
            
            services.AddScoped<ICommandHandler<Commands.Carrito.ActualizarCantidadItemCommand, Core.Domain.Carrito>>(
                provider => provider.GetRequiredService<Commands.Carrito.ActualizarCantidadItemCommandHandler>());
            
            services.AddScoped<ICommandHandler<Commands.Carrito.EliminarItemCarritoCommand, bool>>(
                provider => provider.GetRequiredService<Commands.Carrito.EliminarItemCarritoCommandHandler>());
            
            services.AddScoped<ICommandHandler<Commands.Carrito.VaciarCarritoCommand, bool>>(
                provider => provider.GetRequiredService<Commands.Carrito.VaciarCarritoCommandHandler>());
        }

        private static void RegisterQueryHandlers(IServiceCollection services)
        {
            // Productos
            services.AddScoped<IQueryHandler<Queries.Productos.ObtenerTodosProductosQuery, IEnumerable<Core.Domain.Producto>>>(
                provider => provider.GetRequiredService<Queries.Productos.ObtenerTodosProductosQueryHandler>());
            
            services.AddScoped<IQueryHandler<Queries.Productos.ObtenerProductoPorIdQuery, Core.Domain.Producto?>>(
                provider => provider.GetRequiredService<Queries.Productos.ObtenerProductoPorIdQueryHandler>());
            
            services.AddScoped<IQueryHandler<Queries.Productos.BuscarProductosPorCategoriaQuery, IEnumerable<Core.Domain.Producto>>>(
                provider => provider.GetRequiredService<Queries.Productos.BuscarProductosPorCategoriaQueryHandler>());
            
            services.AddScoped<IQueryHandler<Queries.Productos.BuscarProductosQuery, Queries.Productos.ProductosPaginadosResult>>(
                provider => provider.GetRequiredService<Queries.Productos.BuscarProductosQueryHandler>());

            // Carrito
            services.AddScoped<IQueryHandler<Queries.Carrito.ObtenerCarritoPorUsuarioQuery, Core.Domain.Carrito?>>(
                provider => provider.GetRequiredService<Queries.Carrito.ObtenerCarritoPorUsuarioQueryHandler>());
            
            services.AddScoped<IQueryHandler<Queries.Carrito.ObtenerTotalCarritoQuery, decimal>>(
                provider => provider.GetRequiredService<Queries.Carrito.ObtenerTotalCarritoQueryHandler>());
            
            services.AddScoped<IQueryHandler<Queries.Carrito.ObtenerResumenCarritoQuery, Queries.Carrito.ResumenCarritoResult>>(
                provider => provider.GetRequiredService<Queries.Carrito.ObtenerResumenCarritoQueryHandler>());
            
            services.AddScoped<IQueryHandler<Queries.Carrito.ObtenerItemsCarritoQuery, IEnumerable<Queries.Carrito.ItemCarritoDetalleResult>>>(
                provider => provider.GetRequiredService<Queries.Carrito.ObtenerItemsCarritoQueryHandler>());
        }

        private static void RegisterDomainEventHandlers(IServiceCollection services)
        {
            // Registrar el puente automático para eventos específicos
            services.AddScoped<EventSourcing.DomainEventToEventStoreBridge>();
            
            // Registrar el bridge como handler para eventos específicos de productos
            services.AddScoped<IDomainEventHandler<Domain.Events.Productos.ProductoCreado>>(
                provider => provider.GetRequiredService<EventSourcing.DomainEventToEventStoreBridge>());
            services.AddScoped<IDomainEventHandler<Domain.Events.Productos.ProductoEliminado>>(
                provider => provider.GetRequiredService<EventSourcing.DomainEventToEventStoreBridge>());

            // Registrar el bridge como handler para eventos específicos de carrito
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.CarritoCreado>>(
                provider => provider.GetRequiredService<EventSourcing.DomainEventToEventStoreBridge>());
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.ItemAgregadoAlCarrito>>(
                provider => provider.GetRequiredService<EventSourcing.DomainEventToEventStoreBridge>());

            // Registrar handlers específicos de productos
            services.AddScoped<EventHandlers.Productos.ProductoCreadoHandler>();
            services.AddScoped<IDomainEventHandler<Domain.Events.Productos.ProductoCreado>>(
                provider => provider.GetRequiredService<EventHandlers.Productos.ProductoCreadoHandler>());

            services.AddScoped<EventHandlers.Productos.ProductoEliminadoHandler>();
            services.AddScoped<IDomainEventHandler<Domain.Events.Productos.ProductoEliminado>>(
                provider => provider.GetRequiredService<EventHandlers.Productos.ProductoEliminadoHandler>());

            // Registrar handlers específicos de carrito
            services.AddScoped<EventHandlers.Carrito.CarritoCreadoHandler>();
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.CarritoCreado>>(
                provider => provider.GetRequiredService<EventHandlers.Carrito.CarritoCreadoHandler>());

            services.AddScoped<EventHandlers.Carrito.ItemAgregadoAlCarritoHandler>();
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.ItemAgregadoAlCarrito>>(
                provider => provider.GetRequiredService<EventHandlers.Carrito.ItemAgregadoAlCarritoHandler>());

            // Registrar handlers de cache
            services.AddScoped<EventHandlers.Caching.ProductoCreadoCacheHandler>();
            services.AddScoped<IDomainEventHandler<Domain.Events.Productos.ProductoCreado>>(
                provider => provider.GetRequiredService<EventHandlers.Caching.ProductoCreadoCacheHandler>());

            services.AddScoped<EventHandlers.Caching.ItemAgregadoAlCarritoCacheHandler>();
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.ItemAgregadoAlCarrito>>(
                provider => provider.GetRequiredService<EventHandlers.Caching.ItemAgregadoAlCarritoCacheHandler>());
        }
    }
}
