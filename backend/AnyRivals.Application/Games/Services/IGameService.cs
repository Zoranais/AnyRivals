using AnyRivals.Domain.Entities;

namespace AnyRivals.Application.Games.Services;
public interface IGameService
{
    Task SubmitAnswer(int gameId, Answer answer);
    Task StartGame(int gameId);
    Task RevealResults(int gameId);
    Task EndGame(int gameId);
}
