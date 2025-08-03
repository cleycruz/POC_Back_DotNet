# Carrito de Compras API

Una API RESTful desarrollada en .NET 9 para gestionar un sistema de carrito de compras con operaciones CRUD básicas.

## Características

- ✅ Gestión de productos (CRUD completo)
- ✅ Gestión de carrito de compras por usuario
- ✅ Operaciones de carrito: agregar, actualizar, eliminar items
- ✅ Cálculo automático de totales
- ✅ Documentación automática con Swagger
- ✅ Validación de stock
- ✅ Búsqueda por categoría

## Tecnologías

- .NET 9
- ASP.NET Core Web API
- Swagger/OpenAPI
- Entity Framework (en memoria para este ejemplo)

## Endpoints

### Productos

- `GET /api/productos` - Obtener todos los productos
- `GET /api/productos/{id}` - Obtener producto por ID
- `POST /api/productos` - Crear nuevo producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto
- `GET /api/productos/categoria/{categoria}` - Buscar por categoría

### Carrito

- `GET /api/carrito/{usuarioId}` - Obtener carrito del usuario
- `POST /api/carrito/{usuarioId}/items` - Agregar item al carrito
- `PUT /api/carrito/{usuarioId}/items/{itemId}` - Actualizar cantidad de item
- `DELETE /api/carrito/{usuarioId}/items/{itemId}` - Eliminar item del carrito
- `DELETE /api/carrito/{usuarioId}` - Vaciar carrito
- `GET /api/carrito/{usuarioId}/total` - Obtener total del carrito

## Ejecutar la aplicación

```bash
cd /Users/cley/Documents/carrito/back
dotnet run
```

La aplicación estará disponible en:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001 (o http://localhost:5000)

## Ejemplos de uso

### Crear un producto
```bash
curl -X POST "https://localhost:5001/api/productos" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Smartphone Samsung",
    "descripcion": "Smartphone Samsung Galaxy S24",
    "precio": 899.99,
    "stock": 20,
    "categoria": "Electrónicos"
  }'
```

### Agregar item al carrito
```bash
curl -X POST "https://localhost:5001/api/carrito/usuario123/items" \
  -H "Content-Type: application/json" \
  -d '{
    "productoId": 1,
    "cantidad": 2
  }'
```

### Obtener carrito
```bash
curl "https://localhost:5001/api/carrito/usuario123"
```

## Estructura del proyecto

```
├── Controllers/
│   ├── ProductosController.cs
│   └── CarritoController.cs
├── Models/
│   ├── Producto.cs
│   ├── Carrito.cs
│   └── CarritoItem.cs
├── DTOs/
│   ├── ProductoDto.cs
│   └── CarritoDto.cs
├── Services/
│   ├── ProductoService.cs
│   └── CarritoService.cs
└── Program.cs
```

## Productos de ejemplo

La aplicación viene con algunos productos precargados:
1. Laptop Dell XPS 13 - $1,299.99
2. Mouse Logitech MX Master - $89.99
3. Teclado Mecánico RGB - $149.99
4. Monitor 4K 27" - $399.99

## Notas

- Esta es una implementación de ejemplo que utiliza almacenamiento en memoria
- Para producción, se recomienda usar una base de datos real
- Los datos se pierden al reiniciar la aplicación
- No incluye autenticación/autorización (simplificado para demo)
