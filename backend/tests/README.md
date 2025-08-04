# Estructura de Pruebas - Carrito de Compras API

## 📋 Resumen

Este documento describe la nueva estructura de pruebas implementada siguiendo las mejores prácticas de .NET y Clean Architecture. La organización separa las pruebas por responsabilidades y capas arquitectónicas.

## 🏗️ Estructura de Proyectos de Prueba

### 📁 CarritoComprasAPI.UnitTests.Domain
**Propósito**: Pruebas unitarias de la lógica de dominio (entidades, value objects, reglas de negocio)

**Características**:
- Sin dependencias externas
- Pruebas rápidas y aisladas
- Focus en lógica de negocio pura

**Tecnologías**:
- xUnit
- FluentAssertions
- AutoFixture

**Ejemplo de estructura**:
```
Common/
  SampleDomainTests.cs
Models/              // Cuando se implementen los modelos
  CarritoTests.cs
  ProductoTests.cs
ValueObjects/
  ...
```

### 📁 CarritoComprasAPI.UnitTests.Application
**Propósito**: Pruebas unitarias de casos de uso (comandos, queries, handlers)

**Características**:
- Usa mocks para dependencias
- Prueba orquestación y flujo de negocio
- Valida interacciones entre componentes

**Tecnologías**:
- xUnit
- FluentAssertions
- AutoFixture + AutoMoq
- Moq

**Ejemplo de estructura**:
```
Common/
  SampleApplicationTests.cs
Commands/            // Cuando se implementen
  AgregarProductoCarritoCommandHandlerTests.cs
Queries/
  ObtenerCarritoQueryHandlerTests.cs
```

### 📁 CarritoComprasAPI.UnitTests.Infrastructure
**Propósito**: Pruebas de componentes de infraestructura (repositorios, servicios externos)

**Características**:
- Usa base de datos en memoria
- Puede usar Testcontainers para dependencias reales
- Prueba persistencia y acceso a datos

**Tecnologías**:
- xUnit
- FluentAssertions
- Entity Framework Core InMemory
- Testcontainers.MsSql

**Ejemplo de estructura**:
```
Common/
  SampleInfrastructureTests.cs
Repositories/        // Cuando se implementen
  CarritoRepositoryTests.cs
Services/
  ...
```

### 📁 CarritoComprasAPI.IntegrationTests
**Propósito**: Pruebas de integración completas de la API

**Características**:
- Usa WebApplicationFactory
- Prueba endpoints completos
- Puede usar Testcontainers para BD real

**Tecnologías**:
- xUnit
- FluentAssertions
- ASP.NET Core Testing
- Testcontainers.MsSql

**Ejemplo de estructura**:
```
Common/
  SampleIntegrationTests.cs
Controllers/         // Cuando se implementen
  CarritoControllerIntegrationTests.cs
```

### 📁 CarritoComprasAPI.AcceptanceTests
**Propósito**: Pruebas de aceptación BDD con SpecFlow

**Características**:
- Escenarios en lenguaje natural (Gherkin)
- Pruebas end-to-end
- Puede incluir pruebas de UI con Selenium

**Tecnologías**:
- xUnit
- FluentAssertions
- SpecFlow (Gherkin/BDD)
- Selenium WebDriver

**Ejemplo de estructura**:
```
Common/
  SampleAcceptanceTests.cs
Features/            // Cuando se implementen
  GestionCarrito.feature
StepDefinitions/
  GestionCarritoStepDefinitions.cs
```

### 📁 CarritoComprasAPI.TestUtilities
**Propósito**: Utilidades compartidas entre todos los proyectos de prueba

**Características**:
- Builders y factories para datos de prueba
- Helpers y fixtures comunes
- Configuraciones compartidas

**Tecnologías**:
- Todas las anteriores (como referencia común)
- Bogus (generación de datos fake)

**Ejemplo de estructura**:
```
Tests/
  SampleUtilitiesTests.cs
Builders/            // Cuando se implementen
  CarritoBuilder.cs
  ProductoBuilder.cs
Fixtures/
  DatabaseFixture.cs
Helpers/
  FixtureHelper.cs
```

## 🚀 Comandos Útiles

### Ejecutar todas las pruebas
```bash
dotnet test CarritoCompras.sln
```

### Ejecutar pruebas por categoría
```bash
# Solo pruebas de dominio
dotnet test backend/tests/CarritoComprasAPI.UnitTests.Domain

# Solo pruebas de aplicación
dotnet test backend/tests/CarritoComprasAPI.UnitTests.Application

# Solo pruebas de infraestructura
dotnet test backend/tests/CarritoComprasAPI.UnitTests.Infrastructure

# Solo pruebas de integración
dotnet test backend/tests/CarritoComprasAPI.IntegrationTests

# Solo pruebas de aceptación
dotnet test backend/tests/CarritoComprasAPI.AcceptanceTests
```

### Ejecutar con cobertura de código
```bash
dotnet test CarritoCompras.sln --collect:"XPlat Code Coverage"
```

## 📊 Métricas y Reporting

### Coverage Report
Para generar reportes de cobertura, se recomienda usar `reportgenerator`:

```bash
# Instalar herramienta
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generar reporte
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report
```

### Test Results
Los resultados se pueden exportar en formato TRX:

```bash
dotnet test --logger:trx --results-directory:test-results
```

## 🔧 Configuración de CI/CD

### GitHub Actions Example
```yaml
- name: Run Domain Tests
  run: dotnet test backend/tests/CarritoComprasAPI.UnitTests.Domain --no-restore

- name: Run Application Tests
  run: dotnet test backend/tests/CarritoComprasAPI.UnitTests.Application --no-restore

- name: Run Infrastructure Tests
  run: dotnet test backend/tests/CarritoComprasAPI.UnitTests.Infrastructure --no-restore

- name: Run Integration Tests
  run: dotnet test backend/tests/CarritoComprasAPI.IntegrationTests --no-restore

- name: Run Acceptance Tests
  run: dotnet test backend/tests/CarritoComprasAPI.AcceptanceTests --no-restore
```

## 🎯 Próximos Pasos

1. **Implementar modelos base** en el proyecto principal
2. **Migrar pruebas existentes** a los proyectos correspondientes
3. **Crear pruebas de ejemplo** en cada categoría
4. **Configurar pipeline CI/CD** con ejecución por categorías
5. **Implementar métricas de cobertura** de código
6. **Documentar patrones de prueba** específicos del dominio

## 📚 Referencias

- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/best-practices)
- [Clean Architecture Testing](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SpecFlow Documentation](https://docs.specflow.org/)
- [Testcontainers for .NET](https://testcontainers.com/languages/dotnet/)

---

## ✅ Estado Actual

- [x] Estructura de proyectos creada
- [x] Dependencias configuradas
- [x] Pruebas de ejemplo implementadas
- [x] Compilación exitosa
- [x] Ejecución de pruebas correcta
- [ ] Migración de pruebas existentes
- [ ] Implementación de modelos de dominio
- [ ] Configuración de CI/CD
