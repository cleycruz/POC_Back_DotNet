using CarritoComprasAPI.Core.Ports;

namespace CarritoComprasAPI.Adapters.Secondary
{
    public class ConsoleLogger : IAppLogger
    {
        public void LogInformation(string message, params object[] args)
        {
            Console.WriteLine($"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {string.Format(message, args)}");
        }

        public void LogWarning(string message, params object[] args)
        {
            Console.WriteLine($"[WARN] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {string.Format(message, args)}");
        }

        public void LogError(string message, params object[] args)
        {
            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {string.Format(message, args)}");
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {string.Format(message, args)}");
            Console.WriteLine($"Exception: {exception.Message}");
            if (exception.StackTrace != null)
            {
                Console.WriteLine($"StackTrace: {exception.StackTrace}");
            }
        }
    }
}
