using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Mediator;
using CarritoComprasAPI.Core.Commands.Productos;
using CarritoComprasAPI.Core.Queries.Productos;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.Core.Logging;
using CarritoComprasAPI.Core.Configuration;
using CarritoComprasAPI.DTOs;
using System.Globalization;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controlador REST para la gestión de productos del sistema de carrito de compras.
    /// Implementa operaciones CRUD con validaciones de dominio, logging estructurado y métricas de performance.
    /// </summary>
    /// <remarks>
    /// Este controlador sigue los principios de DDD y Arquitectura Hexagonal:
    /// - Utiliza Value Objects para validaciones
    /// - Implementa CQRS para separar lecturas de escrituras
    /// - Registra Domain Events automáticamente
    /// - Proporciona logging estructurado para auditoría y debugging
    /// </remarks>
    [Route("api/productos")]
    [ApiController]
    [Produces("application/json")]
    public class ProductosController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IStructuredLogger _structuredLogger;

        /// <summary>
        /// Constructor del controlador de productos
        /// </summary>
        /// <param name="mediator">Mediator para enviar commands y queries</param>
        /// <param name="structuredLogger">Logger estructurado para auditoría y métricas</param>
        /// <exception cref="ArgumentNullException">Si algún parámetro es nulo</exception>
        public ProductosController(IMediator mediator, IStructuredLogger structuredLogger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _structuredLogger = structuredLogger ?? throw new ArgumentNullException(nameof(structuredLogger));
        }

        /// <summary>
        /// Obtiene todos los productos disponibles en el sistema
        /// </summary>
        /// <returns>Lista de productos con información completa</returns>
        /// <response code="200">Lista de productos obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        /// <example>
        /// GET /api/productos
        /// 
        /// Respuesta:
        /// [
        ///   {
        ///     "id": 1,
        ///     "nombre": "Laptop Gaming",
        ///     "precio": 1299.99,
        ///     "stock": 15,
        ///     "categoria": "Electrónicos"
        ///   }
        /// ]
        /// </example>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerTodos()
        {
            var requestId = Guid.NewGuid().ToString();
            var usuarioId = ObtenerUsuarioActual();

            return await _structuredLogger.EjecutarConLogging(
                "ObtenerTodosProductos",
                usuarioId,
                requestId,
                async () =>
                {
                    var query = new ObtenerTodosProductosQuery();
                    var productos = await _mediator.Send(query);
                    var productosDto = productos.Select(MapearADto).ToList();
                    
                    _structuredLogger.LogCache("ProductosQuery", "todos_productos");
                    
                    return Ok(productosDto);
                });
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el contexto HTTP
        /// </summary>
        /// <returns>ID del usuario o "anonimo" si no está autenticado</returns>
        private string ObtenerUsuarioActual()
        {
            // En un sistema real esto vendría del JWT token o claims
            return HttpContext.User?.Identity?.Name ?? "usuario_anonimo";
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id)
        {
            try
            {
                var query = new ObtenerProductoPorIdQuery(id);
                var producto = await _mediator.Send(query);
                
                if (producto == null)
                    return NotFound($"Producto con ID {id} no encontrado");
                
                return Ok(MapearADto(producto));
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Crear([FromBody] CrearProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new CrearProductoCommand(
                    dto.Nombre,
                    dto.Descripcion,
                    dto.Precio,
                    dto.Stock,
                    dto.Categoria);
                
                var productoCreado = await _mediator.Send(command);
                var productoDto = MapearADto(productoCreado);
                
                return CreatedAtAction(
                    nameof(ObtenerPorId), 
                    new { id = productoCreado.Id }, 
                    productoDto);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> Actualizar(int id, [FromBody] ActualizarProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new ActualizarProductoCommand(
                    id,
                    dto.Nombre,
                    dto.Descripcion,
                    dto.Precio,
                    dto.Stock,
                    dto.Categoria);
                
                var productoActualizado = await _mediator.Send(command);
                
                if (productoActualizado == null)
                    return NotFound($"Producto con ID {id} no encontrado");
                
                return Ok(MapearADto(productoActualizado));
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var command = new EliminarProductoCommand(id);
                var eliminado = await _mediator.Send(command);
                
                if (!eliminado)
                    return NotFound($"Producto con ID {id} no encontrado");
                
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Busca productos por categoría
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> BuscarPorCategoria(string categoria)
        {
            try
            {
                var query = new BuscarProductosPorCategoriaQuery(categoria);
                var productos = await _mediator.Send(query);
                var productosDto = productos.Select(MapearADto);
                return Ok(productosDto);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        private static ProductoDto MapearADto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre.Value,
                Descripcion = producto.Descripcion,
                Precio = producto.PrecioProducto.Value,
                Stock = producto.StockProducto.Value,
                EstaDisponible = producto.StockProducto.Value > 0,
                FechaCreacion = producto.FechaCreacion,
                FechaActualizacion = null, // El dominio actual no tiene FechaActualizacion
                Categoria = producto.CategoriaProducto.Value
            };
        }
    }
}
