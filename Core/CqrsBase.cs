namespace CarritoComprasAPI.Core.Commands
{
    // Marcador para commands - operaciones que modifican estado
    public interface ICommand<TResponse>
    {
    }

    // Command sin respuesta específica
    public interface ICommand : ICommand<Unit>
    {
    }

    // Resultado unitario para commands que no retornan datos
    public struct Unit
    {
        public static readonly Unit Value = new();
    }

    // Handler base para commands
    public interface ICommandHandler<in TCommand, TResponse> 
        where TCommand : ICommand<TResponse>
    {
        Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
    }

    // Handler para commands sin respuesta
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
        where TCommand : ICommand<Unit>
    {
    }
}

namespace CarritoComprasAPI.Core.Queries
{
    // Marcador para queries - operaciones de consulta
    public interface IQuery<TResponse>
    {
    }

    // Handler base para queries
    public interface IQueryHandler<in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
    }
}
