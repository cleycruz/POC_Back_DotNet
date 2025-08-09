using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Ports;
using System.Globalization;

namespace CarritoComprasAPI.Adapters.Secondary
{
    /// <summary>
    /// Implementación en memoria del repositorio de carritos para pruebas y desarrollo
    /// </summary>
    public class InMemoryCarritoRepository : ICarritoRepository
    {
        private static readonly List<Carrito> _carritos = new();
        private static int _nextId = 1;

        /// <summary>
        /// Obtiene el carrito de un usuario específico
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <returns>El carrito del usuario o null si no existe</returns>
        public Task<Carrito?> ObtenerPorUsuarioAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioCarrito.Value == usuarioId);
            return Task.FromResult(carrito);
        }

        /// <summary>
        /// Crea un nuevo carrito en el repositorio
        /// </summary>
        /// <param name="carrito">Carrito a crear</param>
        /// <returns>El carrito creado con su ID asignado</returns>
        public Task<Carrito> CrearAsync(Carrito carrito)
        {
            carrito.Id = _nextId++;
            carrito.FechaCreacion = DateTime.UtcNow;
            carrito.FechaActualizacion = DateTime.UtcNow;
            _carritos.Add(carrito);
            return Task.FromResult(carrito);
        }

        /// <summary>
        /// Actualiza un carrito existente en el repositorio
        /// </summary>
        /// <param name="carrito">Carrito con los datos actualizados</param>
        /// <returns>El carrito actualizado</returns>
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

        /// <summary>
        /// Elimina el carrito de un usuario específico
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <returns>True si se eliminó exitosamente, false si no existe</returns>
        public Task<bool> EliminarAsync(string usuarioId)
        {
            var carrito = _carritos.FirstOrDefault(c => c.UsuarioCarrito.Value == usuarioId);
            if (carrito == null)
                return Task.FromResult(false);

            _carritos.Remove(carrito);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Verifica si existe un carrito para el usuario especificado
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario</param>
        /// <returns>True si el carrito existe, false en caso contrario</returns>
        public Task<bool> ExisteAsync(string usuarioId)
        {
            var existe = _carritos.Any(c => c.UsuarioCarrito.Value == usuarioId);
            return Task.FromResult(existe);
        }
    }
}
