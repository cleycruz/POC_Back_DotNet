using System.Text.Json;

namespace CarritoComprasAPI.Core.EventSourcing
{
    /// <summary>
    /// Evento base para Event Sourcing con información de auditoría
    /// </summary>
    public abstract record EventBase
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public string EventType { get; init; }
        public string AggregateId { get; init; } = string.Empty;
        public string AggregateType { get; init; } = string.Empty;
        public long Version { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public string UserId { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string IpAddress { get; init; } = string.Empty;
        public string UserAgent { get; init; } = string.Empty;
        public Dictionary<string, object> Metadata { get; init; } = new();

        protected EventBase()
        {
            EventType = GetType().Name;
        }

        protected EventBase(string aggregateId, string aggregateType, long version)
        {
            EventType = GetType().Name;
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            Version = version;
        }
    }

    /// <summary>
    /// Evento almacenado en el Event Store
    /// </summary>
    public class StoredEvent
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string AggregateId { get; set; } = string.Empty;
        public string AggregateType { get; set; } = string.Empty;
        public long Version { get; set; }
        public string Data { get; set; } = string.Empty;
        public string Metadata { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public StoredEvent() { }

        public StoredEvent(EventBase @event, string serializedData, string serializedMetadata)
        {
            EventId = @event.EventId;
            EventType = @event.EventType;
            AggregateId = @event.AggregateId;
            AggregateType = @event.AggregateType;
            Version = @event.Version;
            Data = serializedData;
            Metadata = serializedMetadata;
            OccurredOn = @event.OccurredOn;
            UserId = @event.UserId;
            UserName = @event.UserName;
            IpAddress = @event.IpAddress;
            UserAgent = @event.UserAgent;
        }

        public T DeserializeData<T>() where T : EventBase
        {
            return JsonSerializer.Deserialize<T>(Data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? throw new InvalidOperationException($"No se pudo deserializar el evento {EventType}");
        }

        public Dictionary<string, object> DeserializeMetadata()
        {
            if (string.IsNullOrEmpty(Metadata))
                return new Dictionary<string, object>();

            return JsonSerializer.Deserialize<Dictionary<string, object>>(Metadata, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Interfaz para agregados que soportan Event Sourcing
    /// </summary>
    public interface IEventSourcedAggregate
    {
        string Id { get; }
        long Version { get; }
        IReadOnlyList<EventBase> UncommittedEvents { get; }
        void ApplyEvent(EventBase @event);
        void LoadFromHistory(IEnumerable<EventBase> events);
        void MarkEventsAsCommitted();
    }

    /// <summary>
    /// Clase base para agregados con Event Sourcing
    /// </summary>
    public abstract class EventSourcedAggregateRoot : IEventSourcedAggregate
    {
        private readonly List<EventBase> _uncommittedEvents = new();
        private long _version = 0;

        public abstract string Id { get; protected set; }
        public long Version => _version;
        public IReadOnlyList<EventBase> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        protected void RaiseEvent(EventBase @event)
        {
            var eventWithVersion = @event with { Version = _version + 1 };
            ApplyEvent(eventWithVersion);
            _uncommittedEvents.Add(eventWithVersion);
        }

        public abstract void ApplyEvent(EventBase @event);

        public void LoadFromHistory(IEnumerable<EventBase> events)
        {
            foreach (var @event in events.OrderBy(e => e.Version))
            {
                ApplyEvent(@event);
                _version = @event.Version;
            }
        }

        public void MarkEventsAsCommitted()
        {
            _uncommittedEvents.Clear();
        }

        protected void When(EventBase @event)
        {
            var method = GetType().GetMethod("When", new[] { @event.GetType() });
            method?.Invoke(this, new object[] { @event });
        }
    }

    /// <summary>
    /// Contexto de usuario para auditoría
    /// </summary>
    public class AuditContext
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public Dictionary<string, object> AdditionalData { get; set; } = new();

        public static AuditContext Empty => new();

        public static AuditContext Create(string userId, string userName, string ipAddress = "", string userAgent = "")
        {
            return new AuditContext
            {
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
        }
    }

    /// <summary>
    /// Proveedor de contexto de auditoría
    /// </summary>
    public interface IAuditContextProvider
    {
        AuditContext GetCurrentContext();
    }

    /// <summary>
    /// Implementación del proveedor de contexto de auditoría
    /// </summary>
    public class HttpAuditContextProvider : IAuditContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpAuditContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public AuditContext GetCurrentContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return AuditContext.Empty;

            return AuditContext.Create(
                userId: httpContext.User?.Identity?.Name ?? "anonymous",
                userName: httpContext.User?.FindFirst("name")?.Value ?? "Anonymous User",
                ipAddress: GetClientIpAddress(httpContext),
                userAgent: httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? ""
            );
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            // Intentar obtener la IP real del cliente considerando proxies
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
