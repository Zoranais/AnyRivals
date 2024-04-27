using AnyRivals.Domain.Common;

namespace AnyRivals.Domain.Entities;
public class Player: BaseEntity
{
    public string? ConnectionId { get; set; }

    public string Name { get; set; }

    public int Score { get; set; }

    public int GameId { get; set; }

    public Game Game { get; set; }
}
