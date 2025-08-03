using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Adapters.Primary
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoUseCases _productoUseCases;

        public ProductosController(IProductoUseCases productoUseCases)
        {
            _productoUseCases = productoUseCases ?? throw new ArgumentNullException(nameof(productoUseCases));
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerTodos()
        {
            try
            {
                var productos = await _productoUseCases.ObtenerTodosAsync();
                var productosDto = productos.Select(MapearADto);
                return Ok(productosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id)
        {
            try
            {
                var producto = await _productoUseCases.ObtenerPorIdAsync(id);
                
                if (producto == null)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return Ok(MapearADto(producto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Crear([FromBody] CrearProductoDto productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var producto = MapearAEntidad(productoDto);
                var productoCreado = await _productoUseCases.CrearAsync(producto);
                var resultado = MapearADto(productoCreado);

                return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> Actualizar(int id, [FromBody] ActualizarProductoDto productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var producto = MapearAEntidadActualizar(productoDto);
                var productoActualizado = await _productoUseCases.ActualizarAsync(id, producto);

                if (productoActualizado == null)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return Ok(MapearADto(productoActualizado));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
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
                var eliminado = await _productoUseCases.EliminarAsync(id);

                if (!eliminado)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
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
                var productos = await _productoUseCases.BuscarPorCategoriaAsync(categoria);
                var productosDto = productos.Select(MapearADto);
                return Ok(productosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // Métodos de mapeo
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

        private static Producto MapearAEntidad(CrearProductoDto dto)
        {
            return new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock,
                Categoria = dto.Categoria
            };
        }

        private static Producto MapearAEntidadActualizar(ActualizarProductoDto dto)
        {
            return new Producto
            {
                Nombre = dto.Nombre ?? string.Empty,
                Descripcion = dto.Descripcion ?? string.Empty,
                Precio = dto.Precio ?? 0,
                Stock = dto.Stock ?? 0,
                Categoria = dto.Categoria ?? string.Empty
            };
        }
    }
}
