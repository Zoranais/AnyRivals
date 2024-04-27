using MediatR;

namespace AnyRivals.Application.Common.Interfaces.Handlers;
public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
{
}
