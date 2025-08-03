using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.UseCases;
using CarritoComprasAPI.Adapters.Secondary;
using CarritoComprasAPI.Core.Mediator;

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

// Casos de uso (core business logic) - mantenemos para compatibilidad
builder.Services.AddScoped<IProductoUseCases, ProductoUseCases>();
builder.Services.AddScoped<ICarritoUseCases, CarritoUseCases>();

// Registrar CQRS
builder.Services.AddSimpleMediator();
builder.Services.AddCqrsHandlers();

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
