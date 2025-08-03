using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Mediator;
using CarritoComprasAPI.Core.Commands.Productos;
using CarritoComprasAPI.Core.Queries.Productos;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Adapters.Primary
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private const string ErrorInternoServidor = "Error interno del servidor";

        public ProductosController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerTodos()
        {
            try
            {
                var query = new ObtenerTodosProductosQuery();
                var productos = await _mediator.Send(query);
                var productosDto = productos.Select(MapearADto);
                return Ok(productosDto);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
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
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Categoria = producto.Categoria
            };
        }
    }
}
