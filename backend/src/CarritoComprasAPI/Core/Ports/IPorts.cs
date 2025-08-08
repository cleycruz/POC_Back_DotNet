using CarritoComprasAPI.Core.Domain;

namespace CarritoComprasAPI.Core.Ports
{
    // Puerto de salida - Para acceso a datos
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto> CrearAsync(Producto producto);
        Task<Producto?> ActualizarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<Producto>> BuscarPorCategoriaAsync(string categoria);
        Task<bool> ExisteAsync(int id);
    }

    // Puerto de salida - Para carrito
    public interface ICarritoRepository
    {
        Task<Carrito?> ObtenerPorUsuarioAsync(string usuarioId);
        Task<Carrito> CrearAsync(Carrito carrito);
        Task<Carrito> ActualizarAsync(Carrito carrito);
        Task<bool> EliminarAsync(string usuarioId);
        Task<bool> ExisteAsync(string usuarioId);
    }

    // Puerto de entrada - Casos de uso de productos
    public interface IProductoUseCases
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto> CrearAsync(Producto producto);
        Task<Producto?> ActualizarAsync(int id, Producto productoActualizado);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<Producto>> BuscarPorCategoriaAsync(string categoria);
    }

    // Puerto de entrada - Casos de uso de carrito
    public interface ICarritoUseCases
    {
        Task<Carrito> ObtenerCarritoAsync(string usuarioId);
        Task<Carrito> AgregarItemAsync(string usuarioId, int productoId, int cantidad);
        Task<Carrito> ActualizarCantidadAsync(string usuarioId, int productoId, int cantidad);
        Task<bool> EliminarItemAsync(string usuarioId, int productoId);
        Task<bool> VaciarCarritoAsync(string usuarioId);
        Task<decimal> ObtenerTotalAsync(string usuarioId);
    }

    // Puerto de salida - Para logging
    public interface IAppLogger
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
    }

    // Puerto de salida - Para eventos de dominio (opcional)
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T evento) where T : class;
    }
}
