using MediatR;
using QuickForm.Common.Domain;

namespace QuickForm.Common.Application;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ResultT<TResponse>>
    where TQuery : IQuery<TResponse>;
