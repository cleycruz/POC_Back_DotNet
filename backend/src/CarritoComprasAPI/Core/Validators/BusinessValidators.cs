using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Core.Validators
{
    /// <summary>
    /// Servicio de validaciones de negocio para productos
    /// </summary>
    public interface IProductoBusinessValidator
    {
        Task<ValidationResult<bool>> ValidateCreateAsync(string nombre, string categoria, decimal precio, int stock);
        Task<ValidationResult<bool>> ValidateUpdateAsync(int id, string nombre, string categoria, decimal precio, int stock);
        Task<ValidationResult<bool>> ValidateDeleteAsync(int id);
        Task<ValidationResult<bool>> ValidateStockAvailabilityAsync(int productoId, int cantidadRequerida);
    }

    public class ProductoBusinessValidator : IProductoBusinessValidator
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICarritoRepository _carritoRepository;
        private readonly IAppLogger _logger;

        // Categorías permitidas
        private readonly string[] _categoriasPermitidas = {
            "Electrónicos", "Ropa", "Hogar", "Deportes", "Libros", 
            "Juguetes", "Belleza", "Automóvil", "Música", "Alimentación"
        };

        public ProductoBusinessValidator(
            IProductoRepository productoRepository,
            ICarritoRepository carritoRepository,
            IAppLogger logger)
        {
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ValidationResult<bool>> ValidateCreateAsync(string nombre, string categoria, decimal precio, int stock)
        {
            var errors = new List<string>();

            // Validar nombre único
            var productos = await _productoRepository.ObtenerTodosAsync();
            if (productos.Any(p => p.Nombre.Value.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"Ya existe un producto con el nombre '{nombre}'");
            }

            // Validar categoría permitida
            if (!_categoriasPermitidas.Contains(categoria, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"La categoría '{categoria}' no está permitida. Categorías válidas: {string.Join(", ", _categoriasPermitidas)}");
            }

            // Validar precio razonable
            if (precio > 999999.99m)
            {
                errors.Add("El precio no puede exceder $999,999.99");
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de negocio al crear producto: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }

        public async Task<ValidationResult<bool>> ValidateUpdateAsync(int id, string nombre, string categoria, decimal precio, int stock)
        {
            var errors = new List<string>();

            // Verificar que el producto existe
            var producto = await _productoRepository.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                errors.Add($"No se encontró el producto con ID {id}");
                return ValidationResult<bool>.Failure(errors);
            }

            // Validar nombre único (excluyendo el producto actual)
            var productos = await _productoRepository.ObtenerTodosAsync();
            if (productos.Any(p => p.Id != id && p.Nombre.Value.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"Ya existe otro producto con el nombre '{nombre}'");
            }

            // Validar categoría permitida
            if (!_categoriasPermitidas.Contains(categoria, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"La categoría '{categoria}' no está permitida. Categorías válidas: {string.Join(", ", _categoriasPermitidas)}");
            }

            // Validar precio razonable
            if (precio > 999999.99m)
            {
                errors.Add("El precio no puede exceder $999,999.99");
            }

            // Validar que no se reduzca el stock por debajo de lo que está en carritos
            if (stock < producto.StockProducto.Value)
            {
                var carritoItemsCount = await GetTotalProductoEnCarritos(id);
                if (stock < carritoItemsCount)
                {
                    errors.Add($"No se puede reducir el stock a {stock} porque hay {carritoItemsCount} unidades en carritos de usuarios");
                }
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de negocio al actualizar producto {id}: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }

        public async Task<ValidationResult<bool>> ValidateDeleteAsync(int id)
        {
            var errors = new List<string>();

            // Verificar que el producto existe
            var producto = await _productoRepository.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                errors.Add($"No se encontró el producto con ID {id}");
                return ValidationResult<bool>.Failure(errors);
            }

            // Verificar que no está en carritos
            var carritoItemsCount = await GetTotalProductoEnCarritos(id);
            if (carritoItemsCount > 0)
            {
                errors.Add($"No se puede eliminar el producto porque está presente en {carritoItemsCount} carritos de usuarios");
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de negocio al eliminar producto {id}: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }

        public async Task<ValidationResult<bool>> ValidateStockAvailabilityAsync(int productoId, int cantidadRequerida)
        {
            var errors = new List<string>();

            var producto = await _productoRepository.ObtenerPorIdAsync(productoId);
            if (producto == null)
            {
                errors.Add($"No se encontró el producto con ID {productoId}");
                return ValidationResult<bool>.Failure(errors);
            }

            if (producto.StockProducto.Value < cantidadRequerida)
            {
                errors.Add($"Stock insuficiente. Disponible: {producto.StockProducto.Value}, Requerido: {cantidadRequerida}");
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de stock para producto {productoId}: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }

        private Task<int> GetTotalProductoEnCarritos(int productoId)
        {
            // Esta es una implementación simplificada
            // En un escenario real, necesitarías un método en el repositorio para esto
            try
            {
                // Simulamos obtener todos los carritos y contar el producto
                // En producción, esto debería ser una consulta optimizada
                return Task.FromResult(0); // Placeholder - implementar según necesidades específicas
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener total de producto {productoId} en carritos: {ex.Message}");
                return Task.FromResult(0);
            }
        }
    }

    /// <summary>
    /// Servicio de validaciones de negocio para carritos
    /// </summary>
    public interface ICarritoBusinessValidator
    {
        Task<ValidationResult<bool>> ValidateAddItemAsync(string usuarioId, int productoId, int cantidad);
        Task<ValidationResult<bool>> ValidateUpdateQuantityAsync(string usuarioId, int productoId, int nuevaCantidad);
        Task<ValidationResult<bool>> ValidateRemoveItemAsync(string usuarioId, int productoId);
    }

    public class CarritoBusinessValidator : ICarritoBusinessValidator
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IProductoBusinessValidator _productoValidator;
        private readonly IAppLogger _logger;

        private const int MAX_ITEMS_PER_CART = 50;
        private const decimal MAX_CART_VALUE = 50000.00m;

        public CarritoBusinessValidator(
            ICarritoRepository carritoRepository,
            IProductoRepository productoRepository,
            IProductoBusinessValidator productoValidator,
            IAppLogger logger)
        {
            _carritoRepository = carritoRepository ?? throw new ArgumentNullException(nameof(carritoRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _productoValidator = productoValidator ?? throw new ArgumentNullException(nameof(productoValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ValidationResult<bool>> ValidateAddItemAsync(string usuarioId, int productoId, int cantidad)
        {
            var errors = new List<string>();

            // Validar stock disponible
            var stockValidation = await _productoValidator.ValidateStockAvailabilityAsync(productoId, cantidad);
            if (!stockValidation.IsValid)
            {
                errors.AddRange(stockValidation.Errors);
                return ValidationResult<bool>.Failure(errors);
            }

            // Obtener carrito existente
            var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
            
            if (carrito != null)
            {
                // Validar límite de items únicos en carrito
                var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
                if (itemExistente == null && carrito.Items.Count >= MAX_ITEMS_PER_CART)
                {
                    errors.Add($"No se puede agregar más productos. Límite máximo: {MAX_ITEMS_PER_CART} productos diferentes");
                }

                // Validar cantidad total del producto si ya existe
                if (itemExistente != null)
                {
                    var nuevaCantidadTotal = itemExistente.CantidadItem.Value + cantidad;
                    if (nuevaCantidadTotal > 100) // Límite por producto
                    {
                        errors.Add($"No se puede agregar {cantidad} unidades. Ya tienes {itemExistente.CantidadItem.Value} y el límite es 100 por producto");
                    }
                }

                // Validar valor total del carrito
                var producto = await _productoRepository.ObtenerPorIdAsync(productoId);
                if (producto != null)
                {
                    var valorAdicional = producto.PrecioProducto.Value * cantidad;
                    if (carrito.Total + valorAdicional > MAX_CART_VALUE)
                    {
                        errors.Add($"No se puede agregar el producto. El valor total del carrito excedería ${MAX_CART_VALUE:N2}");
                    }
                }
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de negocio al agregar item al carrito: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }

        public async Task<ValidationResult<bool>> ValidateUpdateQuantityAsync(string usuarioId, int productoId, int nuevaCantidad)
        {
            var errors = new List<string>();

            // Validar stock disponible
            var stockValidation = await _productoValidator.ValidateStockAvailabilityAsync(productoId, nuevaCantidad);
            if (!stockValidation.IsValid)
            {
                errors.AddRange(stockValidation.Errors);
            }

            // Validar que el item existe en el carrito
            var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
            if (carrito == null)
            {
                errors.Add("No se encontró el carrito del usuario");
                return ValidationResult<bool>.Failure(errors);
            }

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
            {
                errors.Add("El producto no está en el carrito");
                return ValidationResult<bool>.Failure(errors);
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de negocio al actualizar cantidad: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }

        public async Task<ValidationResult<bool>> ValidateRemoveItemAsync(string usuarioId, int productoId)
        {
            var errors = new List<string>();

            var carrito = await _carritoRepository.ObtenerPorUsuarioAsync(usuarioId);
            if (carrito == null)
            {
                errors.Add("No se encontró el carrito del usuario");
                return ValidationResult<bool>.Failure(errors);
            }

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
            {
                errors.Add("El producto no está en el carrito");
                return ValidationResult<bool>.Failure(errors);
            }

            if (errors.Any())
            {
                _logger.LogWarning($"Errores de validación de negocio al eliminar item: {string.Join(", ", errors)}");
                return ValidationResult<bool>.Failure(errors);
            }

            return ValidationResult<bool>.Success(true);
        }
    }
}
