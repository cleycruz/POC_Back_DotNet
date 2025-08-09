using CarritoComprasAPI.Core.Domain.Events.Productos;
using CarritoComprasAPI.Core.Domain.Events.Carrito;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Caching;
using CarritoComprasAPI.Core.Ports;
using System.Globalization;

namespace CarritoComprasAPI.Core.EventHandlers.Caching
{
    /// <summary>
    /// Handler para invalidar caché cuando se crea un producto
    /// </summary>
    public class ProductoCreadoCacheHandler : IDomainEventHandler<ProductoCreado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public ProductoCreadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(ProductoCreado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateProductoCache();
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se actualiza un producto
    /// </summary>
    public class ProductoActualizadoCacheHandler : IDomainEventHandler<ProductoActualizado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public ProductoActualizadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(ProductoActualizado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateProductoCache(domainEvent.ProductoId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se elimina un producto
    /// </summary>
    public class ProductoEliminadoCacheHandler : IDomainEventHandler<ProductoEliminado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public ProductoEliminadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(ProductoEliminado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateProductoCache(domainEvent.ProductoId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando cambia el precio de un producto
    /// </summary>
    public class PrecioProductoCambiadoCacheHandler : IDomainEventHandler<PrecioProductoCambiado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public PrecioProductoCambiadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(PrecioProductoCambiado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateProductoCache(domainEvent.ProductoId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando cambia el stock de un producto
    /// </summary>
    public class StockProductoCambiadoCacheHandler : IDomainEventHandler<StockProductoCambiado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public StockProductoCambiadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(StockProductoCambiado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateProductoCache(domainEvent.ProductoId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se crea un carrito
    /// </summary>
    public class CarritoCreadoCacheHandler : IDomainEventHandler<CarritoCreado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public CarritoCreadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(CarritoCreado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateCarritoCache(domainEvent.UsuarioId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se agrega un item al carrito
    /// </summary>
    public class ItemAgregadoAlCarritoCacheHandler : IDomainEventHandler<ItemAgregadoAlCarrito>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public ItemAgregadoAlCarritoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(ItemAgregadoAlCarrito domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateCarritoCache(domainEvent.UsuarioId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se elimina un item del carrito
    /// </summary>
    public class ItemEliminadoDelCarritoCacheHandler : IDomainEventHandler<ItemEliminadoDelCarrito>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public ItemEliminadoDelCarritoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(ItemEliminadoDelCarrito domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateCarritoCache(domainEvent.UsuarioId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se actualiza la cantidad de un item
    /// </summary>
    public class CantidadItemCarritoActualizadaCacheHandler : IDomainEventHandler<CantidadItemCarritoActualizada>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public CantidadItemCarritoActualizadaCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(CantidadItemCarritoActualizada domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateCarritoCache(domainEvent.UsuarioId);
        }
    }

    /// <summary>
    /// Handler para invalidar caché cuando se vacía un carrito
    /// </summary>
    public class CarritoVaciadoCacheHandler : IDomainEventHandler<CarritoVaciado>
    {
        private readonly ICacheInvalidationService _cacheInvalidation;

        public CarritoVaciadoCacheHandler(ICacheInvalidationService cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation ?? throw new ArgumentNullException(nameof(cacheInvalidation));
        }

        public async Task Handle(CarritoVaciado domainEvent, CancellationToken cancellationToken = default)
        {
            await _cacheInvalidation.InvalidateCarritoCache(domainEvent.UsuarioId);
        }
    }
}
