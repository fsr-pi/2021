using System.Threading.Tasks;

namespace CommandQueryCore
{
  public interface ICommandHandler<TCommand>
  {    
    Task Handle(TCommand command);
  }

  public interface ICommandHandler<TCommand, TKey>
  {
    Task<TKey> Handle(TCommand command);
  }
}
