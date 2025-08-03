using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoComprasAPI.Core.Domain.Events
{
    /// <summary>
    /// Evento base de dominio
    /// </summary>
    public abstract record DomainEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string EventType { get; init; }

        protected DomainEvent()
        {
            EventType = GetType().Name;
        }
    }

    /// <summary>
    /// Entidad base que puede publicar eventos de dominio
    /// </summary>
    public abstract class DomainEntity
    {
        private readonly List<DomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public bool HasDomainEvents => _domainEvents.Any();
    }

    /// <summary>
    /// Handler para eventos de dominio
    /// </summary>
    public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Despachador de eventos de dominio
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
        Task DispatchEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
