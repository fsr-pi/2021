using CommandQueryCore;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contract.CommandHandlers
{
  public class ValidateCommandBeforeHandle<TCommand, TCommandHandler> : ICommandHandler<TCommand>
    where TCommandHandler : ICommandHandler<TCommand>
  {
    private readonly IEnumerable<IValidator<TCommand>> validators;
    private readonly TCommandHandler commandHandler;

    public ValidateCommandBeforeHandle(IEnumerable<IValidator<TCommand>> validators, TCommandHandler commandHandler)
    {
      this.validators = validators;
      this.commandHandler = commandHandler;
    }

    public async Task Handle(TCommand command)
    {
      if (validators.Any())
      {
        var context = new ValidationContext<TCommand>(command);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        if (failures.Count != 0)
          throw new ValidationException(failures);
      }
      await commandHandler.Handle(command);      
    }
  }

  public class ValidateCommandBeforeHandle<TCommand, TKey, TCommandHandler> : ICommandHandler<TCommand, TKey>
    where TCommandHandler : ICommandHandler<TCommand, TKey>
  {
    private readonly IEnumerable<IValidator<TCommand>> validators;
    private readonly TCommandHandler commandHandler;    

    public ValidateCommandBeforeHandle(IEnumerable<IValidator<TCommand>> validators, TCommandHandler commandHandler)
    {
      this.validators = validators;
      this.commandHandler = commandHandler;      
    }

    public async Task<TKey> Handle(TCommand command)
    {
      if (validators.Any())
      {
        var context = new ValidationContext<TCommand>(command);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        if (failures.Count != 0)
          throw new ValidationException(failures);
      }
      TKey key = await commandHandler.Handle(command);
      return key;
    }
  }
}
