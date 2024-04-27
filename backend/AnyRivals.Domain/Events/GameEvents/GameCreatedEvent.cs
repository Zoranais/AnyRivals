using AnyRivals.Domain.Common;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Domain.Events.GameEvents;
public sealed class GameCreatedEvent : DomainEvent
{
    public Game Game { get; }

    public GameCreatedEvent(Game game)
    {
        Game = game;
    }
}
