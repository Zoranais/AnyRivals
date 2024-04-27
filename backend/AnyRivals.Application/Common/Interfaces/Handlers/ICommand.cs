using MediatR;

namespace AnyRivals.Application.Common.Interfaces.Handlers;
public interface ICommand<out TResponse>: IRequest<TResponse>
{
}
