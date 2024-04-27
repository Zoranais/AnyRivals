using AnyRivals.Domain.Common;
using MediatR;

namespace AnyRivals.Application.Common.Interfaces.Handlers;
public interface IDomainEventHandler<in TNotification>: INotificationHandler<TNotification> where TNotification : DomainEvent
{
}
