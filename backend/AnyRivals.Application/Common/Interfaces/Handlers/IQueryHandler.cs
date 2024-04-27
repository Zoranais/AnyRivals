using MediatR;

namespace AnyRivals.Application.Common.Interfaces.Handlers;
public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
{
}
