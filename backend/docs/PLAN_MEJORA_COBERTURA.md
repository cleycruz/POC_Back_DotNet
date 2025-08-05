# Plan de Mejora de Cobertura de Código

## 📊 Estado Actual (Después de las Mejoras Básicas)

### Métricas de Cobertura
- **Líneas**: 9.9% (objetivo: 80%+)
- **Branches**: 6% (objetivo: 80%+)
- **Métodos**: 11.3% (objetivo: 80%+)
- **Pruebas Ejecutándose**: 71 pruebas ✅

### Deuda Técnica
- **Total**: 75.5 horas ($7,550 USD)
- **Distribución**: 94% por baja cobertura, 4% violaciones, 2% duplicación

## 🎯 Progreso Realizado

### ✅ Completado
1. **Infraestructura de Testing**: Configuración completa con xUnit, FluentAssertions, AutoFixture
2. **Pruebas de Dominio Básicas**: 52 pruebas funcionando para Value Objects y Entities
3. **Framework de Excepciones**: Manejo consistente de errores de dominio
4. **Métricas Automatizadas**: Sistema completo de análisis de calidad y deuda técnica

### 📈 Mejoras Logradas
- **+115% cobertura de líneas** (4.6% → 9.9%)
- **+71% cobertura de branches** (3.5% → 6%)
- **+35% cobertura de métodos** (8.4% → 11.3%)
- **+37% más pruebas** (52 → 71 tests)

## 🚀 Plan de Continuación

### Fase 1: Completar Capa de Dominio (Meta: 15-20% cobertura)
**Prioridad**: Alta | **Tiempo estimado**: 4-6 horas

#### Componentes con Cobertura Actual:
- ✅ **Carrito**: 91.5% - Excelente
- ✅ **UsuarioId**: 68.7% - Bueno
- ✅ **Categoria**: 57.1% - Bueno
- 🔄 **CarritoItem**: 50% - Mejorar
- 🔄 **ProductoNombre**: 50% - Mejorar
- 🔄 **Stock**: 44% - Mejorar
- 🔄 **Precio**: 43.4% - Mejorar
- 🔄 **Producto**: 41.6% - Mejorar
- 🔄 **Cantidad**: 40% - Mejorar

#### Acciones Pendientes:
1. **Completar Value Objects**: Añadir pruebas de validación, operaciones y edge cases
2. **Expandir Entities**: Probar todas las reglas de negocio y comportamientos
3. **Domain Events**: Probar eventos de dominio (0% cobertura actual)
4. **Domain Exceptions**: Probar todas las excepciones de dominio (0% cobertura)

### Fase 2: Capa de Aplicación (Meta: 25-35% cobertura)
**Prioridad**: Alta | **Tiempo estimado**: 6-8 horas

#### Componentes Sin Cobertura (0%):
- 🔄 **Commands**: Todos los CommandHandlers (Carrito, Productos)
- 🔄 **Queries**: Todos los QueryHandlers
- 🔄 **Use Cases**: Lógica de casos de uso
- 🔄 **Validators**: Validaciones con FluentValidation
- 🔄 **Event Handlers**: Manejo de eventos de dominio

#### Estrategia:
1. **Commands**: Probar cada comando con casos válidos e inválidos
2. **Queries**: Probar consultas con diferentes parámetros
3. **Validation**: Probar todas las reglas de validación
4. **Integration**: Probar integración entre componentes

### Fase 3: Capa de Infraestructura (Meta: 40-50% cobertura)
**Prioridad**: Media | **Tiempo estimado**: 8-10 horas

#### Componentes Prioritarios:
- 🔄 **Repositories**: InMemoryCarritoRepository, InMemoryProductoRepository (0%)
- 🔄 **Caching**: MemoryCacheService, Cache Handlers (0%)
- 🔄 **Event Sourcing**: EventStore, Repository patterns (0%)
- 🔄 **Logging**: ConsoleLogger y servicios de logging (0%)

### Fase 4: Capa de Presentación (Meta: 60-70% cobertura)  
**Prioridad**: Media | **Tiempo estimado**: 6-8 horas

#### Controllers Actuales:
- ✅ **CarritoController**: 24.1% - Expandir
- 🔄 **ProductosController**: 0% - Implementar
- 🔄 **MetricsController**: 0% - Implementar
- 🔄 **Otros Controllers**: 0% - Implementar

### Fase 5: Pruebas de Integración (Meta: 75-80% cobertura)
**Prioridad**: Media | **Tiempo estimado**: 10-12 horas

#### Tipos de Pruebas:
1. **API Integration**: Pruebas end-to-end de endpoints
2. **Database Integration**: Pruebas con base de datos real
3. **Cache Integration**: Pruebas de invalidación y sincronización
4. **Event Integration**: Pruebas de flujo completo de eventos

### Fase 6: Pruebas de Aceptación (Meta: 80%+ cobertura)
**Prioridad**: Baja | **Tiempo estimado**: 8-10 horas

#### Escenarios de Negocio:
1. **Gestión de Carrito**: Crear, agregar, modificar, eliminar
2. **Gestión de Productos**: CRUD completo
3. **Flujos de Error**: Manejo de errores y excepciones
4. **Performance**: Pruebas de carga y stress

## 🛠️ Próximas Acciones Inmediatas

### 1. Completar Value Objects (Próxima sesión)
```bash
# Crear pruebas exhaustivas para:
- PrecioTests_Comprehensive.cs
- CantidadTests_Comprehensive.cs
- StockTests_Comprehensive.cs
```

### 2. Expandir Domain Entities
```bash
# Mejorar pruebas existentes:
- ProductoTests_BusinessRules.cs
- CarritoItemTests_Operations.cs
```

### 3. Implementar Domain Events Testing
```bash
# Crear nuevas pruebas:
- DomainEventsTests.cs
- EventDispatcherTests.cs
```

## 📋 Comandos de Monitoreo

### Ejecutar Pruebas y Cobertura:
```bash
cd /Users/cley/Documents/carrito-compras
dotnet test CarritoCompras.sln --collect:"XPlat Code Coverage" --results-directory:"./TestResults"
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReports" -reporttypes:"Html;TextSummary"
```

### Verificar Métricas:
```bash
curl -s http://localhost:5010/api/metrics/technical-debt | jq
curl -s http://localhost:5010/api/metrics/coverage | jq
```

## 🎯 Metas por Fases

| Fase | Meta Cobertura | Tiempo | Reducción Deuda |
|------|----------------|--------|-----------------|
| 1    | 15-20%        | 4-6h   | ~10-15%        |
| 2    | 25-35%        | 6-8h   | ~20-30%        |
| 3    | 40-50%        | 8-10h  | ~40-50%        |
| 4    | 60-70%        | 6-8h   | ~60-70%        |
| 5    | 75-80%        | 10-12h | ~80-85%        |
| 6    | 80%+          | 8-10h  | ~90%+          |

**Total estimado**: 42-54 horas de desarrollo
**Reducción de deuda**: De 75.5h a <10h (85%+ reducción)
**ROI**: >$5,000 USD en valor de calidad de código
