using MediatR;

namespace AnyRivals.Application.Common.Interfaces.Handlers;
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
