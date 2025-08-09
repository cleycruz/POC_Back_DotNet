using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Adapters.Secondary;
using CarritoComprasAPI.Core.Mediator;
using CarritoComprasAPI.Core.Domain.Events;
using CarritoComprasAPI.Core.Caching;
using CarritoComprasAPI.Core.Validators;
using CarritoComprasAPI.Core.EventSourcing;
using CarritoComprasAPI.Core.EventSourcing.Store;
using CarritoComprasAPI.Core.Configuration;
using CarritoComprasAPI.Core.Logging;
using CarritoComprasAPI.Core.UseCases;
using CarritoComprasAPI.Core.Metrics;
using FluentValidation;
using System.Reflection;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar AppSettings desde appsettings.json
builder.Services.Configure<AppSettings>(builder.Configuration);

// Registrar ConfigurationService
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

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
builder.Services.AddScoped<IStructuredLogger, StructuredLogger>();

// Registrar UseCases (hexagonal architecture)
builder.Services.AddScoped<IProductoUseCases, ProductoUseCases>();
builder.Services.AddScoped<ICarritoUseCases, CarritoUseCases>();

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
    DefaultExpiration = TimeSpan.FromMinutes(BusinessConstants.CACHE_EXPIRACION_DEFECTO_MINUTOS),
    ProductosExpiration = TimeSpan.FromMinutes(BusinessConstants.CACHE_PRODUCTOS_EXPIRACION_MINUTOS),
    CarritosExpiration = TimeSpan.FromMinutes(BusinessConstants.CACHE_CARRITOS_EXPIRACION_MINUTOS)
});

// Registrar servicios de performance y alertas
builder.Services.AddPerformanceAndAlerting();

// Registrar servicios de métricas de código
builder.Services.AddScoped<ICodeMetricsService, CodeMetricsService>();

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

// Servir archivos estáticos para el dashboard
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure dashboard routes
app.MapGet("/", () => Results.Redirect("/metrics-dashboard.html"));
app.MapGet("/dashboard", () => Results.Redirect("/metrics-dashboard.html"));
app.MapGet("/metrics-dashboard", () => Results.Redirect("/metrics-dashboard.html"));

app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

// Para pruebas de integración
public partial class Program { }
