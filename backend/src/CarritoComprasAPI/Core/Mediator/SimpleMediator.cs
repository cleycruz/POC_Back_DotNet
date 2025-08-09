using CarritoComprasAPI.Core.Commands;
using CarritoComprasAPI.Core.Queries;
using CarritoComprasAPI.Core.Domain.Events;
using System.Globalization;

namespace CarritoComprasAPI.Core.Mediator
{
    /// <summary>
    /// Interfaz para el patrón Mediator en CQRS
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Envía un comando para su procesamiento
        /// </summary>
        /// <typeparam name="TResponse">Tipo de respuesta del comando</typeparam>
        /// <param name="command">Comando a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado del procesamiento del comando</returns>
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Envía una consulta para su procesamiento
        /// </summary>
        /// <typeparam name="TResponse">Tipo de respuesta de la consulta</typeparam>
        /// <param name="query">Consulta a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado del procesamiento de la consulta</returns>
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementación simple del patrón Mediator para CQRS
    /// </summary>
    public class SimpleMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Inicializa una nueva instancia de SimpleMediator
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios para resolver handlers</param>
        /// <exception cref="ArgumentNullException">Lanzado cuando serviceProvider es null</exception>
        public SimpleMediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Procesa un comando usando el handler apropiado
        /// </summary>
        /// <typeparam name="TResponse">Tipo de respuesta del comando</typeparam>
        /// <param name="command">Comando a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado del procesamiento del comando</returns>
        /// <exception cref="ArgumentNullException">Lanzado cuando command es null</exception>
        /// <exception cref="InvalidOperationException">Lanzado cuando no se encuentra el handler o el método Handle</exception>
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

        /// <summary>
        /// Procesa una consulta usando el handler apropiado
        /// </summary>
        /// <typeparam name="TResponse">Tipo de respuesta de la consulta</typeparam>
        /// <param name="query">Consulta a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado del procesamiento de la consulta</returns>
        /// <exception cref="ArgumentNullException">Lanzado cuando query es null</exception>
        /// <exception cref="InvalidOperationException">Lanzado cuando no se encuentra el handler o el método Handle</exception>
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

    /// <summary>
    /// Extensiones para configurar el mediator y handlers CQRS
    /// </summary>
    public static class MediatorExtensions
    {
        /// <summary>
        /// Registra el SimpleMediator en el contenedor de dependencias
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services)
        {
            services.AddScoped<IMediator, SimpleMediator>();
            return services;
        }

        /// <summary>
        /// Registra todos los handlers CQRS (Command, Query y Domain Event handlers)
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>La colección de servicios para encadenamiento</returns>
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
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.ItemEliminadoDelCarrito>>(
                provider => provider.GetRequiredService<EventSourcing.DomainEventToEventStoreBridge>());
            services.AddScoped<IDomainEventHandler<Domain.Events.Carrito.CarritoVaciado>>(
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
