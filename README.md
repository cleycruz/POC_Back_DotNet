# 🛒 Carrito de Compras - Aplicación Full Stack

API REST para gestión de carrito de compras desarrollada con arquitectura **DDD/CQRS** y **.NET 9**, con frontend **Angular** e infraestructura automatizada usando **Terraform** en **Azure**.

## 🏗️ Arquitectura del Proyecto

```
📁 carrito-compras/
├── 📁 backend/                      # Backend API (.NET 9)
│   ├── 📁 src/CarritoComprasAPI/     # API principal con DDD/CQRS
│   │   ├── 📁 Core/                  # Dominio y lógica de negocio
│   │   ├── 📁 Adapters/              # Controladores y repositorios
│   │   ├── 📁 Models/                # Modelos de datos
│   │   └── 📁 Services/              # Servicios de aplicación
│   ├── 📁 tests/                     # Pruebas unitarias e integración
│   │   └── 📁 CarritoComprasAPI.UnitTests/
│   └── 📁 docs/                      # Documentación del backend
├── 📁 frontend/                     # Aplicación cliente (Angular)
│   ├── 📁 src/app/                   # Código fuente Angular
│   └── 📁 dist/                      # Build de producción
├── 📁 infrastructure/               # Infraestructura como código
│   ├── 📁 terraform/                # Módulos de Terraform
│   ├── 📁 docker/                   # Configuraciones Docker
│   └── 📁 azure/                    # Templates ARM/Bicep
├── 📁 docs/                         # Documentación del proyecto
├── 📄 CarritoCompras.sln            # Solución principal
└── 📄 docker-compose.yml           # Orquestación local
```

## 🚀 Tecnologías Implementadas

### Backend (.NET 9)
- **Arquitectura**: DDD (Domain Driven Design) + CQRS + Event Sourcing
- **Patrones**: Repository, Mediator, Clean Architecture, Hexagonal
- **Validación**: FluentValidation con reglas de negocio
- **Logging**: Structured Logging para trazabilidad
- **Cache**: In-Memory Caching con invalidación automática
- **Métricas**: Performance monitoring y alertas automáticas
- **Testing**: xUnit, FluentAssertions, Moq

### Frontend (Angular)
- **Framework**: Angular 17+ con TypeScript
- **UI/UX**: Angular Material + componentes personalizados
- **Estado**: RxJS para gestión reactiva del estado
- **Testing**: Jasmine + Karma para pruebas unitarias
- **Build**: Angular CLI con optimizaciones de producción

### Infraestructura
- **Containerización**: Docker multi-stage builds
- **Orquestación**: Azure Container Apps
- **IaC**: Terraform modular con mejores prácticas
- **CI/CD**: GitHub Actions para pipelines automatizados
- **Monitoreo**: Application Insights para observabilidad
- **Cloud**: Azure (App Service, SQL Database, Storage)

## 🔧 Inicio Rápido

### Prerrequisitos
```bash
# .NET 9 SDK
dotnet --version  # 9.0.x

# Node.js y Angular CLI
node --version     # 18.x+
npm install -g @angular/cli

# Docker (opcional para desarrollo local)
docker --version

# Terraform (para infraestructura)
terraform --version
```

### Desarrollo Local

1. **Clonar y configurar**:
```bash
git clone <repository-url>
cd carrito-compras
```

2. **Backend API**:
```bash
cd backend
dotnet restore
dotnet build
cd src/CarritoComprasAPI
dotnet run
```
> 🌐 API disponible en: `https://localhost:7001`

3. **Frontend Angular**:
```bash
cd frontend
npm install
npm start
```
> 🌐 App disponible en: `http://localhost:4200`

4. **Docker Compose (Todo junto)**:
```bash
docker-compose up --build
```

## 🧪 Testing y Calidad

### Backend Testing
```bash
cd backend

# Ejecutar todas las pruebas
dotnet test

# Con cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Tests específicos por categoría
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

### Frontend Testing
```bash
cd frontend

# Pruebas unitarias
npm test

# Pruebas e2e
npm run e2e

# Análisis de cobertura
npm run test:coverage
```

### Métricas y Calidad
- **Cobertura de Tests**: En implementación (objetivo: >80%)
- **Arquitectura**: DDD/CQRS con Event Sourcing
- **Performance**: Métricas automáticas y alertas
- **Logs**: Structured logging para trazabilidad
- **Validaciones**: FluentValidation en todas las capas

## ☁️ Despliegue en Azure

### 1. Configurar Infrastructure
```bash
cd infrastructure/terraform

# Configurar variables
cp terraform.tfvars.example terraform.tfvars
# Editar variables según el entorno

# Desplegar infraestructura
terraform init
terraform plan
terraform apply
```

### 2. Variables de Entorno Requeridas
```hcl
# terraform.tfvars
resource_group_name = "rg-carrito-compras-prod"
location           = "East US"
environment        = "production"
app_name          = "carrito-compras"
sql_admin_username = "sqladmin"
sql_admin_password = "ComplexPassword123!"
```

### 3. Pipeline CI/CD
El proyecto incluye workflows de GitHub Actions para:
- ✅ Build y testing automático
- ✅ Análisis de código con SonarQube
- ✅ Despliegue automático a Azure
- ✅ Pruebas de smoke post-despliegue

## 📚 Documentación Técnica

### Backend
- [Implementación DDD](backend/src/CarritoComprasAPI/Documentation/DDD_IMPLEMENTATION.md)
- [Event Sourcing](backend/src/CarritoComprasAPI/Documentation/DOMAIN_EVENTS_IMPLEMENTATION.md)
- [Validaciones FluentValidation](backend/src/CarritoComprasAPI/Documentation/FLUENTVALIDATION_IMPLEMENTATION.md)
- [Sistema de Cache](backend/src/CarritoComprasAPI/Documentation/CACHE_IMPLEMENTATION_SUMMARY.md)
- [Calidad de Código](backend/src/CarritoComprasAPI/Documentation/CODIGO_CALIDAD_IMPLEMENTATION.md)

### Arquitectura y Patrones
- **Clean Architecture**: Separación clara de responsabilidades
- **DDD**: Value Objects, Aggregates, Domain Events
- **CQRS**: Commands y Queries separados
- **Event Sourcing**: Auditoría completa de cambios
- **Hexagonal**: Puertos y adaptadores para flexibilidad

## 🔄 Historia del Proyecto

### Migración y Preservación de Git
✅ **Historia 100% Preservada**: La reorganización del workspace mantiene completamente la historia de Git:
- Todos los commits conservan autoría y fechas originales
- Historia de archivos accesible con `git log --follow <archivo>`
- Enlaces a commits/PRs en issues siguen funcionando
- Operaciones Git detectadas como "rename" preservan tracking

### Evolución del Proyecto
1. **Fase 1**: Implementación inicial con arquitectura DDD/CQRS
2. **Fase 2**: Adición de Event Sourcing y métricas de performance
3. **Fase 3**: Reorganización completa del workspace para escalabilidad
4. **Fase 4**: **[ACTUAL]** Implementación de testing exhaustivo (0% → >80% cobertura)

## 📈 Próximos Pasos

### En Desarrollo
1. 🧪 **Implementar tests unitarios completos** (objetivo principal actual)
2. 🔄 **Pipeline CI/CD completo con GitHub Actions**
3. 📊 **Dashboard de métricas y monitoreo avanzado**

### Roadmap
1. 🎨 **Completar frontend Angular con todas las funcionalidades**
2. ☁️ **Despliegue automático en Azure con Terraform**
3. 🚀 **Optimizaciones de performance y escalabilidad**
4. 📱 **Aplicación móvil con .NET MAUI**

## 🤝 Contribución

1. Fork el proyecto
2. Crear branch de feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push al branch (`git push origin feature/nueva-funcionalidad`)
5. Abrir Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

---

**Estado del Proyecto**: 🚧 En desarrollo activo
**Cobertura de Tests**: 🎯 Implementando (objetivo: >80%)
**Última Actualización**: Agosto 2025
