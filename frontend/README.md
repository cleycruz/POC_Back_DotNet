# Carrito de Compras - Frontend Angular

Esta es una aplicación Angular que consume las APIs de un sistema de carrito de compras.

## 🚀 Características

- **Gestión de Productos**: Ver, crear, editar y eliminar productos
- **Carrito de Compras**: Agregar productos, modificar cantidades, eliminar items
- **Interfaz Responsiva**: Diseño adaptable para móviles y escritorio
- **Operaciones CRUD completas**: Todas las operaciones básicas del carrito

## 📋 Prerrequisitos

- Node.js (versión 18 o superior)
- npm
- API del backend corriendo en `http://localhost:5063`

## 🛠️ Instalación

1. Instalar dependencias:
```bash
npm install
```

2. Asegúrate de que tu API backend esté corriendo en `http://localhost:5063`

3. Ejecutar la aplicación:
```bash
npm start
```

La aplicación estará disponible en `http://localhost:4200`

## 🏗️ Estructura del Proyecto

```
src/
├── app/
│   ├── components/
│   │   ├── productos/           # Gestión de productos
│   │   └── carrito/             # Carrito de compras
│   ├── services/
│   │   └── carrito.service.ts   # Servicio para API calls
│   ├── models/
│   │   └── carrito.models.ts    # Interfaces TypeScript
│   ├── app.component.*          # Componente principal
│   ├── app.module.ts            # Módulo principal
│   └── app-routing.module.ts    # Configuración de rutas
├── styles.css                   # Estilos globales
└── index.html                   # Página principal
```

## 🔌 APIs Consumidas

La aplicación consume las siguientes APIs del backend en `http://localhost:5063/api`:

### Productos
- `GET /api/Productos` - Obtener todos los productos
- `GET /api/Productos/{id}` - Obtener un producto específico
- `GET /api/Productos/categoria/{categoria}` - Obtener productos por categoría
- `POST /api/Productos` - Crear nuevo producto
- `PUT /api/Productos/{id}` - Actualizar producto
- `DELETE /api/Productos/{id}` - Eliminar producto

### Carrito
- `GET /api/Carrito/{usuarioId}` - Obtener carrito del usuario
- `POST /api/Carrito/{usuarioId}/items` - Agregar item al carrito
- `PUT /api/Carrito/{usuarioId}/items/{itemId}` - Actualizar cantidad de item
- `DELETE /api/Carrito/{usuarioId}/items/{itemId}` - Eliminar item del carrito
- `DELETE /api/Carrito/{usuarioId}` - Vaciar carrito completo
- `GET /api/Carrito/{usuarioId}/total` - Obtener total del carrito

## 🎨 Funcionalidades

### Página de Productos
- Lista todos los productos disponibles
- Permite crear nuevos productos
- Editar productos existentes
- Eliminar productos
- Agregar productos al carrito con validación de stock

### Página de Carrito
- Muestra todos los items en el carrito
- Modificar cantidades de productos
- Eliminar items individuales
- Vaciar carrito completo
- Resumen con total de la compra
- Proceso de checkout (simulado)

## 🎯 Características Técnicas

- **Angular**: Framework principal
- **TypeScript**: Tipado estático
- **RxJS**: Manejo de observables para HTTP requests
- **CSS Grid & Flexbox**: Layout responsivo
- **HttpClient**: Para comunicación con la API
- **Router**: Navegación entre páginas

## 🔧 Configuración de la API

La URL base de la API se configura en `src/app/services/carrito.service.ts`:

```typescript
private baseUrl = 'http://localhost:5063/api';
```

Si tu API backend corre en una URL diferente, modifica esta línea.

## 📱 Diseño Responsivo

La aplicación está optimizada para:
- **Desktop**: Layout en grid con múltiples columnas
- **Tablet**: Layout adaptado con menos columnas
- **Móvil**: Layout en una sola columna con navegación optimizada

## 🚀 Comandos Disponibles

```bash
# Desarrollo
npm start                 # Ejecutar en modo desarrollo

# Construcción
npm run build            # Construir para producción
npm run build:dev        # Construir para desarrollo

# Testing
npm test                 # Ejecutar tests unitarios
npm run test:watch       # Tests en modo watch

# Linting
npm run lint             # Ejecutar linter
```

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Notas

- La aplicación simula un usuario con ID = "usuario1" para las operaciones del carrito
- En una aplicación real, esto se manejaría con un sistema de autenticación
- Los errores se muestran en consola y mediante alerts (en producción usar un sistema de notificaciones más robusto)
- El backend usa `usuarioId` como string, no como número
- La estructura de datos sigue exactamente los DTOs definidos en la API OpenAPI

## 🔗 Enlaces

- [Documentación de Angular](https://angular.io/docs)
- [RxJS](https://rxjs.dev/)
- [TypeScript](https://www.typescriptlang.org/)
