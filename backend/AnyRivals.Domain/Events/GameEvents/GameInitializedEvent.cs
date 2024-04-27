using AnyRivals.Domain.Common;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Domain.Events.GameEvents;
public sealed class GameInitializedEvent: DomainEvent
{
    public Game Game { get; }

    public GameInitializedEvent(Game game)
    {
        Game = game;
    }
}
