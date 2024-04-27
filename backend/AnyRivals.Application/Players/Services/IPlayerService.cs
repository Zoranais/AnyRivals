using AnyRivals.Application.Common.Models.DTOs;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Application.Players.Services;
public interface IPlayerService
{
    Task<GameDto> AddPlayer(string gameId, Player player);

    Task RemovePlayer(int gameId, int playerId);
}
