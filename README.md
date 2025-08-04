# 🛒 Carrito de Compras - Full Stack Application

Sistema completo de carrito de compras desarrollado con **.NET Core** (Backend) y **Angular** (Frontend), con infraestructura automatizada usando **Terraform** en **Azure**.

## 🏗️ Arquitectura

```
├── 🔧 Backend (.NET Core 9)     - API REST con DDD, CQRS, Event Sourcing
├── 🌐 Frontend (Angular)        - SPA con Angular Material
├── 🏗️ Infrastructure (Terraform) - Infraestructura como código en Azure
└── 📚 Documentation            - Documentación técnica y de usuario
```

## 🚀 Inicio Rápido

### Prerrequisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- [Docker](https://www.docker.com/)
- [Terraform](https://www.terraform.io/)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/)

### Desarrollo Local

#### Con Docker Compose (Recomendado)
```bash
# Clonar y navegar al proyecto
git clone <repository>
cd carrito-compras

# Ejecutar toda la aplicación
docker-compose up --build
```

#### Desarrollo Manual

**Backend:**
```bash
cd backend
dotnet restore
dotnet run --project src/CarritoComprasAPI
# API disponible en: https://localhost:5001
```

**Frontend:**
```bash
cd frontend
npm install
npm start
# App disponible en: http://localhost:4200
```

### Testing

**Backend:**
```bash
cd backend
dotnet test
```

**Frontend:**
```bash
cd frontend
npm test
npm run e2e
```

## 🏗️ Despliegue en Azure

### Configurar Infrastructure

```bash
cd infrastructure/terraform

# Inicializar Terraform
terraform init

# Planificar despliegue
terraform plan

# Aplicar infraestructura
terraform apply
```

### Variables de Entorno

Crear `terraform.tfvars`:
```hcl
resource_group_name = "rg-carrito-compras-prod"
location           = "East US"
environment        = "prod"
app_name          = "carrito-compras"
```

## 📁 Estructura del Proyecto

### Backend
- **src/CarritoComprasAPI**: Aplicación principal
- **tests/**: Pruebas unitarias e integración
- **docs/**: Documentación específica del backend

### Frontend
- **src/app**: Código fuente Angular
- **tests/**: Pruebas unitarias
- **e2e/**: Pruebas end-to-end

### Infrastructure
- **terraform/**: Infraestructura como código
- **docker/**: Dockerfiles y configuraciones
- **azure/**: Plantillas ARM/Bicep alternativas

## 🔧 Tecnologías

### Backend
- **.NET Core 9**
- **Clean Architecture (DDD)**
- **CQRS + Event Sourcing**
- **FluentValidation**
- **xUnit + FluentAssertions**

### Frontend
- **Angular 17+**
- **Angular Material**
- **RxJS**
- **Jest + Cypress**

### Infrastructure
- **Azure App Service**
- **Azure SQL Database**
- **Azure Storage**
- **Application Insights**
- **Terraform**

## 📖 Documentación

- [Arquitectura del Backend](./backend/docs/)
- [Arquitectura del Frontend](./frontend/docs/)
- [Guía de Despliegue](./docs/deployment/)
- [API Documentation](./docs/api/)

## 🤝 Contribución

1. Fork el proyecto
2. Crear branch de feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👥 Equipo

- **Backend**: Arquitectura DDD con .NET Core
- **Frontend**: SPA moderna con Angular
- **DevOps**: Infraestructura automatizada con Terraform
