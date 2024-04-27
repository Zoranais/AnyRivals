using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AnyRivals.Application.Common.Exceptions;
using AnyRivals.Application.Common.Models.DTOs;
using AnyRivals.Application.Hubs;
using AnyRivals.Domain.Entities;
using AnyRivals.Application.Common.Interfaces.Data;
namespace AnyRivals.Application.Players.Services;
public class PlayerService : IPlayerService
{
    private readonly IDataAccessContext _context;
    private readonly IHubContext<GameHub> _hub;
    private readonly IMapper _mapper;

    public PlayerService(IHubContext<GameHub> hub, IMapper mapper, IDataAccessContext context)
    {
        _hub = hub;
        _mapper = mapper;
        _context = context;
    }

    public async Task<GameDto> AddPlayer(string gameId, Player player)
    {
        var game = NotFoundException.ThrowIfNull(
            await _context.Games
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.ExternalId == gameId));

        if (game.Players.Any(x => x.Name == player.Name))
        {
            throw new BadRequestException("User with this name already joined this game");
        }

        game.Players.Add(player);
        await _context.SaveChangesAsync();

        await _hub.Clients.Group(gameId).SendAsync("PlayerJoined", _mapper.Map<PlayerDto>(player));

        return _mapper.Map<GameDto>(game);
    }

    public async Task RemovePlayer(int gameId, int playerId)
    {
        var player = NotFoundException.ThrowIfNull(
            await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId && x.GameId == gameId));

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        // TODO: Create domain event to this
        //if (game.Players.Any())
        //{
        //    await _hub.Clients.Group(gameId).SendAsync("PlayerDisconnected", player);
        //}
        //else
        //{
        //    _gameStorage.DeleteGame(gameId);
        //}
    }
}
