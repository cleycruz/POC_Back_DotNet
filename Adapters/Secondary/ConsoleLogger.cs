using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Adapters.Secondary
{
    /// <summary>
    /// Implementación del logger que escribe mensajes a la consola
    /// </summary>
    public class ConsoleLogger : IAppLogger
    {
        /// <summary>
        /// Registra un mensaje informativo en la consola
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        /// <param name="args">Argumentos para formatear el mensaje</param>
        public void LogInformation(string message, params object[] args)
        {
            var formattedMessage = FormatMessage(message, args);
            Console.WriteLine($"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
        }

        /// <summary>
        /// Registra un mensaje de advertencia en la consola
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        /// <param name="args">Argumentos para formatear el mensaje</param>
        public void LogWarning(string message, params object[] args)
        {
            var formattedMessage = FormatMessage(message, args);
            Console.WriteLine($"[WARN] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
        }

        /// <summary>
        /// Registra un mensaje de error en la consola
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        /// <param name="args">Argumentos para formatear el mensaje</param>
        public void LogError(string message, params object[] args)
        {
            var formattedMessage = FormatMessage(message, args);
            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
        }

        /// <summary>
        /// Registra un mensaje de error con excepción en la consola
        /// </summary>
        /// <param name="exception">Excepción a registrar</param>
        /// <param name="message">Mensaje a registrar</param>
        /// <param name="args">Argumentos para formatear el mensaje</param>
        public void LogError(Exception exception, string message, params object[] args)
        {
            var formattedMessage = FormatMessage(message, args);
            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
            Console.WriteLine($"Exception: {exception.Message}");
            if (exception.StackTrace != null)
            {
                Console.WriteLine($"StackTrace: {exception.StackTrace}");
            }
        }

        private static string FormatMessage(string message, params object[] args)
        {
            if (args?.Length == 0)
                return message;

            try
            {
                // Convert .NET-style placeholders to string.Format placeholders
                var formattedTemplate = message;
                var placeholderIndex = 0;
                
                // Replace common .NET placeholders with indexed ones
                var commonPlaceholders = new[] { 
                    "{Count}", "{Id}", "{Name}", "{Message}", "{Error}", "{Value}", 
                    "{Nombre}", "{ProductoId}", "{UsuarioId}", "{Cantidad}" 
                };
                foreach (var placeholder in commonPlaceholders)
                {
                    if (args != null && formattedTemplate.Contains(placeholder) && placeholderIndex < args.Length)
                    {
                        formattedTemplate = formattedTemplate.Replace(placeholder, $"{{{placeholderIndex}}}");
                        placeholderIndex++;
                    }
                }

                return args != null ? string.Format(formattedTemplate, args) : formattedTemplate;
            }
            catch
            {
                // If formatting fails, just return the original message
                return message;
            }
        }
    }
}
