using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Adapters.Secondary;
using CarritoComprasAPI.Core.Mediator;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Caching;
using CarritoComprasAPI.Core.Validators;
using CarritoComprasAPI.Core.EventSourcing;
using CarritoComprasAPI.Core.EventSourcing.Store;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Carrito de Compras API", 
        Version = "v1",
        Description = "API para gestión de carrito de compras con Arquitectura Hexagonal"
    });
    
    // Incluir comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Registrar puertos (interfaces) con sus adaptadores
// Adaptadores secundarios (driven adapters)
builder.Services.AddSingleton<IProductoRepository, InMemoryProductoRepository>();
builder.Services.AddSingleton<ICarritoRepository, InMemoryCarritoRepository>();
builder.Services.AddSingleton<IAppLogger, ConsoleLogger>();

// Registrar servicios de caché
builder.Services.AddMemoryCache(); // Esto registra IMemoryCache

// Registrar CQRS (reemplaza los casos de uso tradicionales)
builder.Services.AddSimpleMediator();
builder.Services.AddCqrsHandlers();

// Registrar eventos de dominio
builder.Services.AddDomainEvents();

// Registrar Event Sourcing
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddScoped<IAuditContextProvider, HttpAuditContextProvider>();
builder.Services.AddScoped<IAuditQueryService, SimpleAuditQueryService>();
builder.Services.AddHttpContextAccessor(); // Necesario para HttpAuditContextProvider

// NOTA: Los Domain Event Handlers (incluido el bridge automático) se registran en AddCqrsHandlers()

// Registrar FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Registrar validadores de negocio
builder.Services.AddScoped<IProductoBusinessValidator, ProductoBusinessValidator>();
builder.Services.AddScoped<ICarritoBusinessValidator, CarritoBusinessValidator>();

// Registrar servicios de caché
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<ICacheInvalidationService, CacheInvalidationService>();
builder.Services.AddSingleton(new CacheConfiguration
{
    EnableCaching = true,
    DefaultExpiration = TimeSpan.FromMinutes(15),
    ProductosExpiration = TimeSpan.FromMinutes(30),
    CarritosExpiration = TimeSpan.FromMinutes(5)
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Registrar middleware de validación
app.UseMiddleware<ValidationExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Carrito de Compras API V1");
        c.RoutePrefix = string.Empty; // Swagger UI en la raíz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
