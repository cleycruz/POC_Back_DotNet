# Implementación de FluentValidation

## 📋 Resumen

Se ha implementado exitosamente **FluentValidation** en la aplicación de carrito de compras, agregando una capa robusta de validaciones tanto a nivel de entrada (DTOs/Commands) como de lógica de negocio.

## 🚀 Características Implementadas

### 1. **Validaciones de Entrada (FluentValidation)**
- ✅ Validadores para todos los comandos de productos
- ✅ Validadores para todos los comandos de carrito  
- ✅ Validadores para queries principales
- ✅ Mensajes de error personalizados en español
- ✅ Reglas de validación específicas por campo

### 2. **Validaciones de Negocio**
- ✅ Validador de negocio para productos (`IProductoBusinessValidator`)
- ✅ Validador de negocio para carritos (`ICarritoBusinessValidator`)
- ✅ Validaciones complejas como nombres únicos, categorías permitidas
- ✅ Validaciones de stock y límites de carrito

### 3. **Middleware de Manejo de Errores**
- ✅ `ValidationExceptionMiddleware` para capturar excepciones globalmente
- ✅ Respuestas de error estandarizadas en formato JSON
- ✅ Diferenciación entre errores de validación y errores de negocio

## 📁 Estructura de Archivos

```
Core/Validators/
├── Commands/
│   ├── Productos/
│   │   └── ProductoValidators.cs
│   └── Carrito/
│       └── CarritoValidators.cs
├── Queries/
│   ├── Productos/
│   │   └── ProductoQueryValidators.cs
│   └── Carrito/
│       └── CarritoQueryValidators.cs
├── BusinessValidators.cs
├── ValidationBehavior.cs
└── ValidationExceptionMiddleware.cs
```

## 🎯 Validadores Implementados

### **Comandos de Productos**

#### `CrearProductoCommandValidator`
- **Nombre**: Obligatorio, 2-100 caracteres
- **Descripción**: Obligatoria, 10-500 caracteres  
- **Precio**: Mayor a 0, menor a $1,000,000, máximo 2 decimales
- **Stock**: No negativo, máximo 10,000 unidades
- **Categoría**: Obligatoria, 2-50 caracteres, solo letras y espacios

#### `ActualizarProductoCommandValidator`
- Mismas validaciones que crear, pero aplicadas condicionalmente
- Validaciones solo cuando el campo no es nulo/vacío

#### `EliminarProductoCommandValidator`
- **ID**: Debe ser mayor a 0

### **Comandos de Carrito**

#### `AgregarItemCarritoCommandValidator`
- **UsuarioId**: Obligatorio, 3-50 caracteres, alfanumérico con guiones
- **ProductoId**: Mayor a 0
- **Cantidad**: 1-100 unidades

#### `ActualizarCantidadItemCommandValidator`
- **UsuarioId**: Obligatorio, 3-50 caracteres, alfanumérico con guiones
- **ProductoId**: Mayor a 0
- **Cantidad**: 1-100 unidades

#### Validadores similares para eliminar item y vaciar carrito

### **Queries**

#### `ObtenerProductoPorIdQueryValidator`
- **ID**: Debe ser mayor a 0

#### `BuscarProductosPorCategoriaQueryValidator`
- **Categoría**: Obligatoria, 2-50 caracteres, solo letras

#### `ObtenerCarritoPorUsuarioQueryValidator`
- **UsuarioId**: Obligatorio, 3-50 caracteres, alfanumérico

## 🔧 Validaciones de Negocio

### **ProductoBusinessValidator**

#### `ValidateCreateAsync`
- Verifica nombre único en la base de datos
- Valida categorías permitidas contra lista predefinida
- Aplica límites de precio razonables

#### `ValidateUpdateAsync`  
- Verifica nombre único excluyendo producto actual
- Valida que no se reduzca stock por debajo de items en carritos
- Mismas validaciones de categoría y precio

#### `ValidateDeleteAsync`
- Verifica que el producto no esté en carritos activos

#### `ValidateStockAvailabilityAsync`
- Verifica disponibilidad de stock para cantidad solicitada

### **CarritoBusinessValidator**

#### `ValidateAddItemAsync`
- Valida stock disponible del producto
- Verifica límite de productos únicos por carrito (50)
- Controla cantidad máxima por producto (100)
- Valida valor máximo total del carrito ($50,000)

#### `ValidateUpdateQuantityAsync`
- Verifica que el item existe en el carrito
- Valida stock disponible para nueva cantidad

#### `ValidateRemoveItemAsync`  
- Confirma que el item existe en el carrito

## ⚙️ Configuración

### **Program.cs**
```csharp
// Registrar FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Registrar validadores de negocio
builder.Services.AddScoped<IProductoBusinessValidator, ProductoBusinessValidator>();
builder.Services.AddScoped<ICarritoBusinessValidator, CarritoBusinessValidator>();

// Registrar middleware de validación
app.UseMiddleware<ValidationExceptionMiddleware>();
```

### **Paquetes NuGet**
```xml
<PackageReference Include="FluentValidation" Version="12.0.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
```

## 🎨 Respuestas de Error

### **Formato de Respuesta**
```json
{
  "statusCode": 400,
  "message": "Errores de validación",
  "errors": [
    {
      "field": "Nombre",
      "message": "El nombre del producto es obligatorio",
      "code": "NotEmptyValidator"
    }
  ],
  "timestamp": "2025-08-03T10:30:00.000Z"
}
```

### **Tipos de Error**
- **400 Bad Request**: Errores de validación de entrada
- **422 Unprocessable Entity**: Errores de validación de negocio  
- **400 Bad Request**: Parámetros nulos o inválidos
- **500 Internal Server Error**: Errores del sistema

## 🧪 Características Destacadas

### **1. Validación Multinivel**
- **Nivel 1**: Validación de formato y estructura (FluentValidation)
- **Nivel 2**: Validación de reglas de negocio (BusinessValidators)
- **Nivel 3**: Manejo global de excepciones (Middleware)

### **2. Mensajes Localizados**
- Todos los mensajes están en español
- Mensajes específicos y descriptivos
- Códigos de error para facilitar manejo por cliente

### **3. Integración con Arquitectura**
- Respeta principios de Arquitectura Hexagonal
- Integrado con patrón CQRS
- Compatible con sistema de eventos de dominio
- Funciona con capa de caché existente

### **4. Validaciones Inteligentes**
- Validación condicional (solo cuando campo está presente)
- Validaciones que consultan base de datos cuando necesario
- Límites de negocio configurables
- Categorías predefinidas

## 🔍 Categorías Permitidas

Las siguientes categorías están permitidas para productos:
- Electrónicos
- Ropa  
- Hogar
- Deportes
- Libros
- Juguetes
- Belleza
- Automóvil
- Música
- Alimentación

## 🚦 Estado del Proyecto

✅ **Compilación exitosa** sin errores  
✅ **Aplicación ejecutándose** en `http://localhost:5063`  
✅ **Validaciones funcionando** en todos los endpoints  
✅ **Middleware configurado** para manejo de errores  
✅ **Documentación completa** de implementación  

## 📝 Próximos Pasos Sugeridos

1. **Testing**: Crear tests unitarios para todos los validadores
2. **Localización**: Agregar soporte para múltiples idiomas
3. **Configuración**: Mover límites y categorías a configuración externa
4. **Métricas**: Agregar logging de validaciones para monitoreo
5. **Performance**: Optimizar validaciones que consultan base de datos

La implementación de FluentValidation está completa y lista para uso en producción, proporcionando una sólida capa de validación que mejora significativamente la robustez y confiabilidad de la aplicación.
