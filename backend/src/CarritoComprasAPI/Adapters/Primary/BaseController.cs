using Microsoft.AspNetCore.Mvc;

namespace CarritoComprasAPI.Adapters.Primary
{
    /// <summary>
    /// Controlador base que contiene funcionalidades comunes para todos los controladores
    /// </summary>
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected const string ErrorInternoServidor = "Error interno del servidor";

        /// <summary>
        /// Maneja las excepciones generales y devuelve una respuesta de error apropiada
        /// </summary>
        protected ActionResult HandleException(Exception ex, string operacion = "operación")
        {
            // Log del error podría ir aquí si tuviéramos logging
            return StatusCode(500, $"Error interno del servidor durante la {operacion}");
        }

        /// <summary>
        /// Maneja las excepciones específicas con mensaje personalizado
        /// </summary>
        protected ActionResult HandleException(Exception ex, string mensaje, string detalle)
        {
            return StatusCode(500, new { mensaje, detalle });
        }

        /// <summary>
        /// Maneja las operaciones async de forma segura
        /// </summary>
        protected async Task<ActionResult<T>> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                var result = await operation();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Maneja las operaciones async que pueden devolver ActionResult
        /// </summary>
        protected async Task<ActionResult<T>> ExecuteAsync<T>(Func<Task<ActionResult<T>>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Maneja las operaciones async que no devuelven datos
        /// </summary>
        protected async Task<ActionResult> ExecuteAsync(Func<Task> operation)
        {
            try
            {
                await operation();
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
