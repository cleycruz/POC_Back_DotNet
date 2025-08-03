using Microsoft.AspNetCore.Mvc;
using CarritoComprasAPI.Core.Mediator;
using CarritoComprasAPI.Core.Commands.Carrito;
using CarritoComprasAPI.Core.Queries.Carrito;
using CarritoComprasAPI.Core.Domain;
using CarritoComprasAPI.DTOs;

namespace CarritoComprasAPI.Adapters.Primary
{
    [ApiController]
    [Route("api/carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private const string ErrorInternoServidor = "Error interno del servidor";

        public CarritoController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Obtiene el carrito de un usuario
        /// </summary>
        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<CarritoDto>> ObtenerCarrito(string usuarioId)
        {
            try
            {
                var query = new ObtenerCarritoPorUsuarioQuery(usuarioId);
                var carrito = await _mediator.Send(query);
                
                if (carrito == null)
                    return Ok(new CarritoDto { UsuarioId = usuarioId });
                
                var carritoDto = MapearADto(carrito);
                return Ok(carritoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Obtiene el total del carrito de un usuario
        /// </summary>
        [HttpGet("{usuarioId}/total")]
        public async Task<ActionResult<decimal>> ObtenerTotal(string usuarioId)
        {
            try
            {
                var query = new ObtenerTotalCarritoQuery(usuarioId);
                var total = await _mediator.Send(query);
                return Ok(total);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Obtiene un resumen del carrito de un usuario
        /// </summary>
        [HttpGet("{usuarioId}/resumen")]
        public async Task<ActionResult<object>> ObtenerResumen(string usuarioId)
        {
            try
            {
                var query = new ObtenerResumenCarritoQuery(usuarioId);
                var resumen = await _mediator.Send(query);
                return Ok(resumen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Obtiene los items del carrito de un usuario
        /// </summary>
        [HttpGet("{usuarioId}/items")]
        public async Task<ActionResult<IEnumerable<CarritoItem>>> ObtenerItems(string usuarioId)
        {
            try
            {
                var query = new ObtenerItemsCarritoQuery(usuarioId);
                var items = await _mediator.Send(query);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Agrega un item al carrito
        /// </summary>
        [HttpPost("{usuarioId}/items")]
        public async Task<ActionResult<CarritoDto>> AgregarItem(
            string usuarioId, 
            [FromBody] AgregarItemCarritoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new AgregarItemCarritoCommand(
                    usuarioId,
                    dto.ProductoId,
                    dto.Cantidad);
                
                var carrito = await _mediator.Send(command);
                var carritoDto = MapearADto(carrito);
                return Ok(carritoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un item en el carrito
        /// </summary>
        [HttpPut("{usuarioId}/items/{productoId}")]
        public async Task<ActionResult<CarritoDto>> ActualizarCantidad(
            string usuarioId, 
            int productoId, 
            [FromBody] ActualizarCantidadDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new ActualizarCantidadItemCommand(
                    usuarioId,
                    productoId,
                    dto.Cantidad);
                
                var carrito = await _mediator.Send(command);
                
                if (carrito == null)
                    return NotFound($"Item con producto ID {productoId} no encontrado en el carrito");
                
                var carritoDto = MapearADto(carrito);
                return Ok(carritoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Elimina un item del carrito
        /// </summary>
        [HttpDelete("{usuarioId}/items/{productoId}")]
        public async Task<ActionResult<CarritoDto>> EliminarItem(string usuarioId, int productoId)
        {
            try
            {
                var command = new EliminarItemCarritoCommand(usuarioId, productoId);
                var eliminado = await _mediator.Send(command);
                
                if (!eliminado)
                    return NotFound($"Item con producto ID {productoId} no encontrado en el carrito");
                
                // Obtener el carrito actualizado
                var query = new ObtenerCarritoPorUsuarioQuery(usuarioId);
                var carrito = await _mediator.Send(query);
                
                if (carrito == null)
                    return Ok(new CarritoDto { UsuarioId = usuarioId });
                
                var carritoDto = MapearADto(carrito);
                return Ok(carritoDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        /// <summary>
        /// Vacía el carrito de un usuario
        /// </summary>
        [HttpDelete("{usuarioId}")]
        public async Task<ActionResult> VaciarCarrito(string usuarioId)
        {
            try
            {
                var command = new VaciarCarritoCommand(usuarioId);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorInternoServidor);
            }
        }

        private static CarritoDto MapearADto(Carrito carrito)
        {
            return new CarritoDto
            {
                UsuarioId = carrito.UsuarioId,
                Items = carrito.Items?.Select(item => new CarritoItemDto
                {
                    ProductoId = item.ProductoId,
                    ProductoNombre = item.Producto?.Nombre ?? "Producto no disponible",
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Subtotal = item.Subtotal
                }).ToList() ?? new List<CarritoItemDto>(),
                Total = carrito.Total
            };
        }
    }
}
