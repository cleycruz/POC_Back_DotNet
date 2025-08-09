using System.Globalization;
namespace CarritoComprasAPI.Core.Commands
{
    /// <summary>
    /// Interfaz base para comandos que modifican estado y retornan una respuesta
    /// </summary>
    /// <typeparam name="TResponse">Tipo de respuesta del comando</typeparam>
    public interface ICommand<TResponse>
    {
    }

    /// <summary>
    /// Interfaz para comandos que no retornan respuesta específica
    /// </summary>
    public interface ICommand : ICommand<Unit>
    {
    }

    /// <summary>
    /// Resultado unitario para comandos que no retornan datos específicos
    /// </summary>
    public struct Unit
    {
        /// <summary>
        /// Instancia única del valor unitario
        /// </summary>
        public static readonly Unit Value = new();
    }

    /// <summary>
    /// Handler base para procesar comandos con respuesta
    /// </summary>
    /// <typeparam name="TCommand">Tipo de comando a procesar</typeparam>
    /// <typeparam name="TResponse">Tipo de respuesta del comando</typeparam>
    public interface ICommandHandler<in TCommand, TResponse> 
        where TCommand : ICommand<TResponse>
    {
        /// <summary>
        /// Procesa el comando especificado
        /// </summary>
        /// <param name="command">Comando a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado del procesamiento del comando</returns>
        Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Handler para comandos sin respuesta específica
    /// </summary>
    /// <typeparam name="TCommand">Tipo de comando a procesar</typeparam>
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
        where TCommand : ICommand<Unit>
    {
    }
}

namespace CarritoComprasAPI.Core.Queries
{
    /// <summary>
    /// Interfaz base para consultas que no modifican estado
    /// </summary>
    /// <typeparam name="TResponse">Tipo de respuesta de la consulta</typeparam>
    public interface IQuery<TResponse>
    {
    }

    /// <summary>
    /// Handler base para procesar consultas
    /// </summary>
    /// <typeparam name="TQuery">Tipo de consulta a procesar</typeparam>
    /// <typeparam name="TResponse">Tipo de respuesta de la consulta</typeparam>
    public interface IQueryHandler<in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        /// <summary>
        /// Procesa la consulta especificada
        /// </summary>
        /// <param name="query">Consulta a procesar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la consulta</returns>
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
    }
}
