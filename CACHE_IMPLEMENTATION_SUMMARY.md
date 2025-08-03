# Sistema de Caché Implementado

## Resumen
Se ha implementado completamente un sistema de caché basado en memoria para optimizar las operaciones de consulta (queries) en la arquitectura CQRS del carrito de compras.

## Componentes Implementados

### 1. Infraestructura de Caché (`Core/Caching/CacheService.cs`)
- **ICacheService**: Interfaz principal para operaciones de caché
- **MemoryCacheService**: Implementación basada en IMemoryCache de .NET
- **CacheConfiguration**: Configuración de políticas de expiración
- **CacheKeys**: Utilidad para generar claves consistentes
- **ConcurrentHashSet**: Implementación thread-safe para tracking de claves

#### Características:
- Serialización JSON automática
- Gestión de expiración configurable
- Tracking de claves para operaciones por patrón
- Cleanup automático cuando expiran elementos

### 2. Decorador de Query Handlers (`Core/Caching/CachedQueryDecorator.cs`)
- **CachedQueryHandlerDecorator**: Patrón decorador para agregar caché transparente
- **ICacheInvalidationService**: Servicio para invalidación selectiva de caché
- **CacheInvalidationService**: Implementación con invalidación inteligente

#### Funcionamiento:
1. Verifica si el caché está habilitado
2. Intenta obtener resultado del caché (cache hit)
3. Si no existe, ejecuta el query original (cache miss)
4. Almacena el resultado en caché con expiración apropiada

### 3. Query Handlers con Caché (`Core/Queries/Cached/CachedQueryHandlers.cs`)
Implementaciones específicas para queries frecuentes:
- **CachedObtenerTodosProductosQueryHandler**: Caché de lista completa de productos
- **CachedObtenerProductoPorIdQueryHandler**: Caché por producto individual
- **CachedBuscarProductosPorCategoriaQueryHandler**: Caché por categoría
- **CachedObtenerCarritoPorUsuarioQueryHandler**: Caché de carritos por usuario

### 4. Invalidación Automática (`Core/EventHandlers/Caching/CacheInvalidationHandlers.cs`)
Event handlers que invalidan caché automáticamente:

#### Eventos de Productos:
- **ProductoCreado**: Invalida lista completa
- **ProductoActualizado**: Invalida producto específico + listas
- **ProductoEliminado**: Invalida producto específico + listas
- **PrecioProductoCambiado**: Invalida producto específico
- **StockProductoCambiado**: Invalida producto específico

#### Eventos de Carritos:
- **CarritoCreado**: Invalida caché del usuario
- **ItemAgregadoAlCarrito**: Invalida caché del usuario
- **ItemEliminadoDelCarrito**: Invalida caché del usuario
- **CantidadItemCarritoActualizada**: Invalida caché del usuario
- **CarritoVaciado**: Invalida caché del usuario

### 5. Controller de Administración (`Adapters/Primary/CacheController.cs`)
API REST para gestión manual del caché:
- `GET /api/cache/config`: Configuración actual
- `GET /api/cache/exists/{key}`: Verificar existencia de clave
- `DELETE /api/cache/productos`: Invalidar productos
- `DELETE /api/cache/productos/{id}`: Invalidar producto específico
- `DELETE /api/cache/carritos`: Invalidar carritos
- `DELETE /api/cache/carritos/{usuarioId}`: Invalidar carrito específico
- `DELETE /api/cache/all`: Invalidar todo el caché
- `GET /api/cache/stats`: Estadísticas básicas

## Configuración

### Políticas de Expiración
```csharp
public class CacheConfiguration
{
    public bool EnableCaching { get; set; } = true;
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan ProductosExpiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan CarritosExpiration { get; set; } = TimeSpan.FromMinutes(15);
}
```

### Registro en DI Container (`Program.cs`)
```csharp
// Servicios base
builder.Services.AddMemoryCache();

// Servicios de caché
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<ICacheInvalidationService, CacheInvalidationService>();
builder.Services.AddSingleton<CacheConfiguration>();

// Event handlers de invalidación
builder.Services.AddScoped<IDomainEventHandler<ProductoCreado>, ProductoCreadoCacheHandler>();
// ... otros handlers
```

## Estrategias de Caché

### 1. Cache-Aside Pattern
- Los handlers verifican caché antes de ejecutar queries
- En cache miss, ejecutan query y almacenan resultado
- En cache hit, devuelven resultado directamente

### 2. Invalidación Inteligente
- **Granular**: Invalida claves específicas cuando es posible
- **Por patrón**: Invalida grupos relacionados cuando es necesario
- **Event-driven**: Automática basada en eventos de dominio

### 3. Claves Consistentes
```csharp
public static class CacheKeys
{
    public static string TodosLosProductos => "productos:todos";
    public static string ProductoPorId(int id) => $"productos:id:{id}";
    public static string ProductosPorCategoria(string categoria) => $"productos:categoria:{categoria}";
    public static string CarritoPorUsuario(string usuarioId) => $"carritos:usuario:{usuarioId}";
}
```

## Beneficios Implementados

### 1. Rendimiento
- **Reducción de latencia**: Queries frecuentes sirven desde memoria
- **Menos carga en repositorios**: Evita recálculos innecesarios
- **Escalabilidad mejorada**: Mejor throughput en operaciones de lectura

### 2. Transparencia
- **Sin cambios en controllers**: Los endpoints existentes ya tienen caché
- **Decorador no intrusivo**: Se puede habilitar/deshabilitar fácilmente
- **Compatibilidad total**: Funciona con toda la arquitectura CQRS existente

### 3. Consistencia
- **Invalidación automática**: Los cambios invalidan caché relevante
- **Event-driven**: Usa los eventos de dominio existentes
- **Granularidad apropiada**: Invalida solo lo necesario

### 4. Observabilidad
- **Logging detallado**: Cache hits/misses registrados
- **API de administración**: Inspección y control manual
- **Configuración flexible**: Tiempos de expiración ajustables

## Pruebas
El archivo `test-cache.http` contiene 21 pruebas que verifican:
- Configuración del caché
- Cache hits y misses
- Invalidación automática y manual
- Operaciones de administración

## Integración con Arquitectura Existente

### CQRS
- ✅ Mantiene separación Commands/Queries
- ✅ Solo optimiza el lado de lectura (Queries)
- ✅ No afecta comandos ni eventos de dominio

### Hexagonal Architecture
- ✅ Caché como cross-cutting concern
- ✅ No viola boundaries arquitecturales
- ✅ Implementado como decorador en capa de aplicación

### Domain Events
- ✅ Usa eventos existentes para invalidación
- ✅ No introduce acoplamientos nuevos
- ✅ Mantiene consistencia eventual

## Estado Actual
✅ **COMPLETAMENTE IMPLEMENTADO** - El sistema de caché está funcional y listo para uso en producción.

## Próximos Pasos Opcionales
1. Métricas de rendimiento (cache hit ratio)
2. Caché distribuido (Redis) para escalabilidad
3. Compresión de datos cacheados
4. TTL dinámico basado en patrones de uso
