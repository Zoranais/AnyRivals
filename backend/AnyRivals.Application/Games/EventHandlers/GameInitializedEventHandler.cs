using AnyRivals.Application.Common.Interfaces.Handlers;
using AnyRivals.Application.Hubs;
using AnyRivals.Domain.Events.GameEvents;
using Microsoft.AspNetCore.SignalR;

namespace AnyRivals.Application.Games.EventHandlers;
public class GameInitializedEventHandler : IDomainEventHandler<GameInitializedEvent>
{
    private readonly IHubContext<GameHub> _hub;

    public GameInitializedEventHandler(IHubContext<GameHub> hub)
    {
        _hub = hub;
    }

    public async Task Handle(GameInitializedEvent notification, CancellationToken cancellationToken)
    {
        await _hub.Clients.Group(notification.Game.Id.ToString()).SendAsync("GameInitialized");
    }
}
