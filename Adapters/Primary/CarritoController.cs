using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Ports;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Adapters.Primary
{
    [ApiController]
    [Route("api/carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoUseCases _carritoUseCases;

        public CarritoController(ICarritoUseCases carritoUseCases)
        {
            _carritoUseCases = carritoUseCases ?? throw new ArgumentNullException(nameof(carritoUseCases));
        }

        /// <summary>
        /// Obtiene el carrito de un usuario
        /// </summary>
        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<CarritoDto>> ObtenerCarrito(string usuarioId)
        {
            try
            {
                var carrito = await _carritoUseCases.ObtenerCarritoAsync(usuarioId);
                var carritoDto = MapearADto(carrito);
                return Ok(carritoDto);
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
        /// Agrega un item al carrito
        /// </summary>
        [HttpPost("{usuarioId}/items")]
        public async Task<ActionResult<CarritoDto>> AgregarItem(string usuarioId, [FromBody] AgregarItemCarritoDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var carrito = await _carritoUseCases.AgregarItemAsync(usuarioId, itemDto.ProductoId, itemDto.Cantidad);
                var carritoDto = MapearADto(carrito);
                
                return Ok(carritoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un item en el carrito
        /// </summary>
        [HttpPut("{usuarioId}/items/{productoId}")]
        public async Task<ActionResult<CarritoDto>> ActualizarCantidad(string usuarioId, int productoId, [FromBody] ActualizarCantidadDto cantidadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var carrito = await _carritoUseCases.ActualizarCantidadAsync(usuarioId, productoId, cantidadDto.Cantidad);
                var carritoDto = MapearADto(carrito);
                
                return Ok(carritoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un item del carrito
        /// </summary>
        [HttpDelete("{usuarioId}/items/{productoId}")]
        public async Task<ActionResult> EliminarItem(string usuarioId, int productoId)
        {
            try
            {
                var eliminado = await _carritoUseCases.EliminarItemAsync(usuarioId, productoId);
                
                if (!eliminado)
                    return NotFound(new { message = "Item no encontrado en el carrito" });

                return NoContent();
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
        /// Vacía el carrito completo
        /// </summary>
        [HttpDelete("{usuarioId}")]
        public async Task<ActionResult> VaciarCarrito(string usuarioId)
        {
            try
            {
                var vaciado = await _carritoUseCases.VaciarCarritoAsync(usuarioId);
                
                if (!vaciado)
                    return NotFound(new { message = "Carrito no encontrado para el usuario" });

                return NoContent();
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
        /// Obtiene el total del carrito
        /// </summary>
        [HttpGet("{usuarioId}/total")]
        public async Task<ActionResult<decimal>> ObtenerTotal(string usuarioId)
        {
            try
            {
                var total = await _carritoUseCases.ObtenerTotalAsync(usuarioId);
                return Ok(new { total });
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

        // Métodos de mapeo
        private static CarritoDto MapearADto(Core.Domain.Carrito carrito)
        {
            return new CarritoDto
            {
                Id = carrito.Id,
                UsuarioId = carrito.UsuarioId,
                Items = carrito.Items.Select(item => new CarritoItemDto
                {
                    Id = item.Id,
                    ProductoId = item.ProductoId,
                    ProductoNombre = item.Producto?.Nombre ?? "Producto no encontrado",
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Subtotal = item.Subtotal
                }).ToList(),
                Total = carrito.Total,
                CantidadItems = carrito.CantidadItems,
                CantidadProductos = carrito.CantidadProductos,
                FechaCreacion = carrito.FechaCreacion,
                FechaActualizacion = carrito.FechaActualizacion
            };
        }
    }
}
