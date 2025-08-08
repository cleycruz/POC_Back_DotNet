# 🚀 Frontend Angular - Carrito de Compras

## ✅ COMANDO SIMPLE QUE FUNCIONA AL 100%

### Para ejecutar la aplicación frontend con un comando simple:

```bash
cd /Users/cley/Documents/carrito-compras/frontend && node /Users/cley/Documents/carrito-compras/frontend/node_modules/@angular/cli/bin/ng.js serve --port 4200
```

### 🎯 Comando aún más simple (copia y pega):

```bash
cd frontend && node "$(pwd)/node_modules/@angular/cli/bin/ng.js" serve --port 4200
```

### 📝 Scripts npm alternativos (desde el directorio frontend):
```bash
npm run dev     # Servidor de desarrollo con auto-open
npm run serve   # Servidor de desarrollo  
npm run build   # Compilar para producción
npm run test    # Ejecutar tests
```

### 🌐 URLs importantes:
- **Frontend**: http://localhost:4200 ✅ FUNCIONANDO
- **Backend API**: http://localhost:5063/api

### � Estado actual:
- ✅ Backend .NET API: Funcionando en puerto 5063
- ✅ Frontend Angular: Funcionando en puerto 4200
- ✅ Conexión frontend-backend: Configurada
- ✅ DDD Architecture: Implementada y funcional

### 🛠️ Solución de problemas:

1. **Si el puerto está ocupado**:
   ```bash
   lsof -ti:4200 | xargs kill -9
   ```

2. **Si faltan dependencias**:
   ```bash
   cd /Users/cley/Documents/carrito-compras/frontend && npm install
   ```

### 🎉 ¡Todo listo para desarrollar!
   ```

2. **Si hay problemas con npm**:
   ```bash
   rm -rf node_modules package-lock.json
   npm install
   ```

3. **Para reiniciar completamente**:
   ```bash
   # Terminal 1: Backend
   cd backend/src/CarritoComprasAPI
   dotnet run
   
   # Terminal 2: Frontend
   cd frontend
   INIT_CWD=$(pwd) node node_modules/@angular/cli/bin/ng.js serve --port 4200
   ```

### 🎯 Función principal:
Este frontend permite:
- Ver lista de productos disponibles
- Agregar productos al carrito
- Gestionar cantidad de productos
- Ver resumen del carrito
- Crear, editar y eliminar productos

### ⚠️ Importante:
- Asegúrate de que el backend esté ejecutándose en el puerto 5063
- El frontend se conecta automáticamente a `http://localhost:5063/api`
- Todos los productos con stock > 0 aparecerán como disponibles
