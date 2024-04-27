using AnyRivals.Domain.Common;
using AnyRivals.Domain.Enums;

namespace AnyRivals.Domain.Entities;
public class Game: BaseEntity
{
    public string ExternalId { get; set; }

    public string Name { get; set; }

    public int TotalRounds { get; set; }

    public string? GamePassword { get; set; }

    public string Source { get; set; }

    public GameState State { get; set; } = GameState.Initializing;

    public GameType GameType { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public ICollection<Question> Rounds { get; set; } = [];

    public Question? CurrentRound { get; set; }

    public int? CurrentRoundId { get; set; }
}
