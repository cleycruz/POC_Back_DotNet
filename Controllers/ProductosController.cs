using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Services;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerTodos()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound($"Producto con ID {id} no encontrado");

            return Ok(producto);
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Crear([FromBody] CrearProductoDto productoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var producto = await _productoService.CrearAsync(productoDto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> Actualizar(int id, [FromBody] ActualizarProductoDto productoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var producto = await _productoService.ActualizarAsync(id, productoDto);
            if (producto == null)
                return NotFound($"Producto con ID {id} no encontrado");

            return Ok(producto);
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _productoService.EliminarAsync(id);
            if (!eliminado)
                return NotFound($"Producto con ID {id} no encontrado");

            return NoContent();
        }

        /// <summary>
        /// Busca productos por categoría
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> BuscarPorCategoria(string categoria)
        {
            var productos = await _productoService.BuscarPorCategoriaAsync(categoria);
            return Ok(productos);
        }
    }
}
