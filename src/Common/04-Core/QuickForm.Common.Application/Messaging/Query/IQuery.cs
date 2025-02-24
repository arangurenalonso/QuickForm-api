using MediatR;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Application;

public interface IQuery<TResponse> : IRequest<ResultT<TResponse>>;
