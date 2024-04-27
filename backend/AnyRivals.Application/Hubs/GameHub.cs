using Microsoft.AspNetCore.SignalR;
using AnyRivals.Application.Games.Services;
using AnyRivals.Application.Players.Services;
using AnyRivals.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace AnyRivals.Application.Hubs;
public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly IPlayerService _playerService;

    public GameHub(IGameService gameService, IPlayerService playerService)
    {
        _gameService = gameService;
        _playerService = playerService;
    }

    public async Task Join(string gameId, string name)
    {
        if (Context.Items.ContainsKey("game"))
        {
            await Leave();
        }

        var connectionId = Context.ConnectionId;

        var player = new Player { Name = name, Score = 0, ConnectionId = connectionId };
        var game = await _playerService.AddPlayer(gameId, player);

        // TODO: Save player state for reconnection
        ClearCurrentPlayerInfo();
        Context.Items.Add("game", game.Id);
        Context.Items.Add("id", player.Id);

        await Groups.AddToGroupAsync(connectionId, game.Id.ToString());
        await Clients.Client(connectionId).SendAsync("Connected", game);
    }

    public async Task Leave()
    {
        var playerInfo = GetCurrentPlayerInfo();

        if (playerInfo.IsValid())
        {
            await _playerService.RemovePlayer(playerInfo.GameId.Value, playerInfo.Id.Value);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, playerInfo.GameId.Value.ToString());
        }

        ClearCurrentPlayerInfo();
    }

    public async Task StartGame()
    {
        var playerInfo = GetCurrentPlayerInfo();

        if (playerInfo.IsValid())
        {
            // If StartGame would also implement RESTART functionality, it would be security issue (every random player would have a possiblity to drop games)
            await _gameService.StartGame(playerInfo.GameId.Value);
        }
    }

    public async Task Respond(string? value)
    {
        var playerInfo = GetCurrentPlayerInfo();
        if (!playerInfo.IsValid())
        {
            await Leave();
            return;
        }

        var answer = new Answer
        {
            Value = value,
            AnsweredAt = DateTime.UtcNow,
            AnsweredById = playerInfo.Id.Value,
        };

        await _gameService.SubmitAnswer(playerInfo.GameId.Value, answer);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Leave();
        await base.OnDisconnectedAsync(exception);
    }

    private PlayerInfo GetCurrentPlayerInfo()
    {
        Context.Items.TryGetValue("game", out var gameId);
        Context.Items.TryGetValue("id", out var id);

        return new PlayerInfo(id as int?, gameId as int?);
    }

    private void ClearCurrentPlayerInfo()
    {
        Context.Items.Remove("game");
        Context.Items.Remove("id");
    }
}

internal record PlayerInfo(int? Id, int? GameId)
{
    [MemberNotNullWhen(true, nameof(Id), nameof(GameId))]
    public bool IsValid() =>
        Id is not null && GameId is not null;
};