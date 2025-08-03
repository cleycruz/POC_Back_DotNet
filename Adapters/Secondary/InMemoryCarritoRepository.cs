using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Adapters.Secondary
{
    public class InMemoryCarritoRepository : ICarritoRepository
    {
        private static readonly List<Carrito> _carritos = new();
        private static int _nextId = 1;

        public Task<Carrito?> ObtenerPorUsuarioAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioCarrito.Value == usuarioId);
            return Task.FromResult(carrito);
        }

        public Task<Carrito> CrearAsync(Carrito carrito)
        {
            carrito.Id = _nextId++;
            carrito.FechaCreacion = DateTime.UtcNow;
            carrito.FechaActualizacion = DateTime.UtcNow;
            _carritos.Add(carrito);
            return Task.FromResult(carrito);
        }

        public Task<Carrito> ActualizarAsync(Carrito carrito)
        {
            var index = _carritos.FindIndex(c => c.Id == carrito.Id);
            if (index != -1)
            {
                carrito.FechaActualizacion = DateTime.UtcNow;
                _carritos[index] = carrito;
            }
            return Task.FromResult(carrito);
        }

        public Task<bool> EliminarAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioCarrito.Value == usuarioId);
            if (carrito == null)
                return Task.FromResult(false);

            _carritos.Remove(carrito);
            return Task.FromResult(true);
        }

        public Task<bool> ExisteAsync(string usuarioId)
        {
            var existe = _carritos.Any(c => c.UsuarioCarrito.Value == usuarioId);
            return Task.FromResult(existe);
        }
    }
}
