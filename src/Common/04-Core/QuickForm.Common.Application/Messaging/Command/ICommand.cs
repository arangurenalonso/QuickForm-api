using MediatR;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Application;
public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<ResultT<TResponse>>, IBaseCommand;
