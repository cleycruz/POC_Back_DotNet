using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Services;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        /// <summary>
        /// Obtiene el carrito de un usuario
        /// </summary>
        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<CarritoDto>> ObtenerCarrito(string usuarioId)
        {
            var carrito = await _carritoService.ObtenerCarritoAsync(usuarioId);
            if (carrito == null)
                return NotFound($"Carrito para usuario {usuarioId} no encontrado");

            return Ok(carrito);
        }

        /// <summary>
        /// Agrega un item al carrito
        /// </summary>
        [HttpPost("{usuarioId}/items")]
        public async Task<ActionResult<CarritoDto>> AgregarItem(string usuarioId, [FromBody] AgregarItemCarritoDto itemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var carrito = await _carritoService.AgregarItemAsync(usuarioId, itemDto);
                return Ok(carrito);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un item en el carrito
        /// </summary>
        [HttpPut("{usuarioId}/items/{itemId}")]
        public async Task<ActionResult<CarritoDto>> ActualizarItem(string usuarioId, int itemId, [FromBody] ActualizarItemCarritoDto itemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var carrito = await _carritoService.ActualizarItemAsync(usuarioId, itemId, itemDto);
            if (carrito == null)
                return NotFound($"Item {itemId} no encontrado en el carrito del usuario {usuarioId}");

            return Ok(carrito);
        }

        /// <summary>
        /// Elimina un item del carrito
        /// </summary>
        [HttpDelete("{usuarioId}/items/{itemId}")]
        public async Task<IActionResult> EliminarItem(string usuarioId, int itemId)
        {
            var eliminado = await _carritoService.EliminarItemAsync(usuarioId, itemId);
            if (!eliminado)
                return NotFound($"Item {itemId} no encontrado en el carrito del usuario {usuarioId}");

            return NoContent();
        }

        /// <summary>
        /// Vacía el carrito de un usuario
        /// </summary>
        [HttpDelete("{usuarioId}")]
        public async Task<IActionResult> VaciarCarrito(string usuarioId)
        {
            var vaciado = await _carritoService.VaciarCarritoAsync(usuarioId);
            if (!vaciado)
                return NotFound($"Carrito para usuario {usuarioId} no encontrado");

            return NoContent();
        }

        /// <summary>
        /// Obtiene el total del carrito
        /// </summary>
        [HttpGet("{usuarioId}/total")]
        public async Task<ActionResult<decimal>> ObtenerTotal(string usuarioId)
        {
            var total = await _carritoService.CalcularTotalAsync(usuarioId);
            return Ok(new { UsuarioId = usuarioId, Total = total });
        }
    }
}
