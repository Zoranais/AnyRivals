using AnyRivals.Application.Common.Helpers;
using AnyRivals.Application.Common.Interfaces.Data;
using AnyRivals.Application.Common.Interfaces.Handlers;
using AnyRivals.Application.Games.Jobs;
using AnyRivals.Domain.Entities;
using AnyRivals.Domain.Enums;
using AnyRivals.Domain.Events.GameEvents;
using Quartz;

namespace AnyRivals.Application.Games.Commands.CreateGame;
public record CreateGameCommand : ICommand<string>
{
    public string Name { get; set; }

    public string? GamePassword { get; set; }

    public GameType GameType { get; set; }

    public string Source { get; set; }

    public int TotalRounds { get; set; }
}

public class CreateGameCommandHandler : ICommandHandler<CreateGameCommand, string>
{
    private readonly IDataAccessContext _context;
    private readonly ISchedulerFactory _schedulerFactory;

    public CreateGameCommandHandler(IDataAccessContext context, ISchedulerFactory schedulerFactory)
    {
        _context = context;
        _schedulerFactory = schedulerFactory;
    }

    public async Task<string> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = CreateGame();
        game.Name = request.Name;
        game.TotalRounds = request.TotalRounds;
        game.GamePassword = request.GamePassword;
        game.GameType = request.GameType;
        game.Source = request.Source;

        game.AddDomainEvent(new GameCreatedEvent(game));
        await _context.SaveChangesAsync(cancellationToken);

        await Schedule(DestroyInactiveGameJob
            .Create(game.Id, TimeSpan.FromMinutes(10)));

        return game.ExternalId;
    }

    private Game CreateGame()
    {
        string id = string.Empty;
        do
        {
            id = GameIdHelper.GenerateGameId();
        }
        while (_context.Games.Any(x => x.ExternalId == id));

        var game = new Game { ExternalId = id };
        _context.Games.Add(game);

        return game;
    }

    private async Task Schedule((IJobDetail jobDetail, Quartz.ITrigger trigger) jobInfo)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(jobInfo.jobDetail, jobInfo.trigger);
    }
}
