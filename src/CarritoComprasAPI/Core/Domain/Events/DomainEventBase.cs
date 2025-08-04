namespace CarritoComprasAPI.Core.Domain.Events
{
    /// <summary>
    /// Evento base de dominio
    /// </summary>
    public abstract record DomainEvent
    {
        /// <summary>
        /// Identificador único del evento
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();
        
        /// <summary>
        /// Fecha y hora cuando ocurrió el evento
        /// </summary>
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        
        /// <summary>
        /// Tipo del evento basado en el nombre de la clase
        /// </summary>
        public string EventType { get; init; }

        /// <summary>
        /// Constructor base que inicializa el tipo de evento
        /// </summary>
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

        /// <summary>
        /// Colección de solo lectura de eventos de dominio pendientes
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Agrega un evento de dominio a la colección de eventos pendientes
        /// </summary>
        /// <param name="domainEvent">Evento de dominio a agregar</param>
        protected void RaiseDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Limpia todos los eventos de dominio pendientes
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Indica si la entidad tiene eventos de dominio pendientes
        /// </summary>
        public bool HasDomainEvents => _domainEvents.Any();
    }

    /// <summary>
    /// Handler para procesar eventos de dominio específicos
    /// </summary>
    /// <typeparam name="TDomainEvent">Tipo de evento de dominio a manejar</typeparam>
    public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        /// <summary>
        /// Procesa un evento de dominio
        /// </summary>
        /// <param name="domainEvent">Evento de dominio a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Despachador de eventos de dominio para publicar eventos a múltiples handlers
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Despacha múltiples eventos de dominio a sus respectivos handlers
        /// </summary>
        /// <param name="events">Colección de eventos a despachar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Despacha un evento de dominio individual a sus handlers
        /// </summary>
        /// <param name="domainEvent">Evento a despachar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        Task DispatchEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
