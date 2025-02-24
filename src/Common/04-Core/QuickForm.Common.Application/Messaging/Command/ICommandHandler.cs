using MediatR;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Application;
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ResultT<TResponse>>
    where TCommand : ICommand<TResponse>;
