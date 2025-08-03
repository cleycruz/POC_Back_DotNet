using CarritoComprasAPI.Models;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Services
{
    public interface ICarritoService
    {
        Task<CarritoDto?> ObtenerCarritoAsync(string usuarioId);
        Task<CarritoDto> AgregarItemAsync(string usuarioId, AgregarItemCarritoDto itemDto);
        Task<CarritoDto?> ActualizarItemAsync(string usuarioId, int itemId, ActualizarItemCarritoDto itemDto);
        Task<bool> EliminarItemAsync(string usuarioId, int itemId);
        Task<bool> VaciarCarritoAsync(string usuarioId);
        Task<decimal> CalcularTotalAsync(string usuarioId);
    }

    public class CarritoService : ICarritoService
    {
        private static readonly List<Carrito> _carritos = new();
        private static int _nextCarritoId = 1;
        private static int _nextItemId = 1;
        private readonly IProductoService _productoService;

        public CarritoService(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public Task<CarritoDto?> ObtenerCarritoAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);
            return Task.FromResult(carrito != null ? MapearADto(carrito) : null);
        }

        public async Task<CarritoDto> AgregarItemAsync(string usuarioId, AgregarItemCarritoDto itemDto)
        {
            var producto = await _productoService.ObtenerPorIdAsync(itemDto.ProductoId);
            if (producto == null)
                throw new ArgumentException("Producto no encontrado");

            if (producto.Stock < itemDto.Cantidad)
                throw new InvalidOperationException("Stock insuficiente");

            var carrito = _carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);
            if (carrito == null)
            {
                carrito = new Carrito
                {
                    Id = _nextCarritoId++,
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.UtcNow
                };
                _carritos.Add(carrito);
            }

            var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == itemDto.ProductoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += itemDto.Cantidad;
            }
            else
            {
                var nuevoItem = new CarritoItem
                {
                    Id = _nextItemId++,
                    ProductoId = itemDto.ProductoId,
                    Cantidad = itemDto.Cantidad,
                    PrecioUnitario = producto.Precio,
                    FechaAgregado = DateTime.UtcNow
                };
                carrito.Items.Add(nuevoItem);
            }

            carrito.FechaActualizacion = DateTime.UtcNow;
            return MapearADto(carrito);
        }

        public Task<CarritoDto?> ActualizarItemAsync(string usuarioId, int itemId, ActualizarItemCarritoDto itemDto)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);
            if (carrito == null) return Task.FromResult<CarritoDto?>(null);

            var item = carrito.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return Task.FromResult<CarritoDto?>(null);

            if (itemDto.Cantidad <= 0)
            {
                carrito.Items.Remove(item);
            }
            else
            {
                item.Cantidad = itemDto.Cantidad;
            }

            carrito.FechaActualizacion = DateTime.UtcNow;
            return Task.FromResult<CarritoDto?>(MapearADto(carrito));
        }

        public Task<bool> EliminarItemAsync(string usuarioId, int itemId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);
            if (carrito == null) return Task.FromResult(false);

            var item = carrito.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return Task.FromResult(false);

            carrito.Items.Remove(item);
            carrito.FechaActualizacion = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        public Task<bool> VaciarCarritoAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);
            if (carrito == null) return Task.FromResult(false);

            carrito.Items.Clear();
            carrito.FechaActualizacion = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        public Task<decimal> CalcularTotalAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);
            return Task.FromResult(carrito?.Total ?? 0);
        }

        private static CarritoDto MapearADto(Carrito carrito)
        {
            return new CarritoDto
            {
                Id = carrito.Id,
                UsuarioId = carrito.UsuarioId,
                Items = carrito.Items.Select(item => new CarritoItemDto
                {
                    Id = item.Id,
                    ProductoId = item.ProductoId,
                    NombreProducto = item.Producto?.Nombre ?? "Producto no encontrado",
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Subtotal = item.Subtotal
                }).ToList(),
                Total = carrito.Total,
                FechaCreacion = carrito.FechaCreacion,
                FechaActualizacion = carrito.FechaActualizacion
            };
        }
    }
}
