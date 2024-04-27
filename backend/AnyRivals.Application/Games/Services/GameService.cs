using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using AnyRivals.Application.Common.Constants;
using AnyRivals.Application.Common.Exceptions;
using AnyRivals.Application.Common.Helpers;
using AnyRivals.Application.Common.Models.DTOs;
using AnyRivals.Application.Hubs;
using AnyRivals.Application.Questions.Jobs;
using AnyRivals.Application.Scheduler;
using AnyRivals.Domain.Entities;
using AnyRivals.Domain.Enums;
using AnyRivals.Application.Common.Interfaces.Data;
namespace AnyRivals.Application.Games.Services;
public class GameService : IGameService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IHubContext<GameHub> _hub;
    private readonly IMapper _mapper;
    private readonly IDataAccessContext _context;
    private readonly IScheduleService _scheduleService;

    public GameService(
        IHubContext<GameHub> hub,
        IMapper mapper,
        ISchedulerFactory schedulerFactory,
        IDataAccessContext context,
        IScheduleService scheduleService)
    {
        _hub = hub;
        _mapper = mapper;
        _schedulerFactory = schedulerFactory;
        _context = context;
        _scheduleService = scheduleService;
    }

    public async Task SubmitAnswer(int gameId, Answer answer)
    {
        var game = NotFoundException.ThrowIfNull(await GetGameAsync(gameId));
        var player = NotFoundException.ThrowIfNull(game.Players.FirstOrDefault(x => x.Id == answer.AnsweredById));
        var currentRound = game.CurrentRound
            ?? throw new BadRequestException("You can't answer when the round is not in progress.");

        if (!currentRound.Answers.Any(x => x.Id == player.Id))
        {
            currentRound.Answers.Add(answer);
            await _context.SaveChangesAsync();

            if (currentRound.Answers.Count == game.Players.Count)
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                await scheduler.UnscheduleJob(TriggerKeyHelper.CreateRevealAnswerKey(game.Id));
                await RevealResults(game.Id);
            }
        }
    }

    public async Task StartGame(int gameId)
    {
        var game = NotFoundException.ThrowIfNull(
            await _context.Games
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == gameId));

        game.CurrentRoundId = _context.Questions
            .Where(x => x.GameId == game.Id)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (game.Players.Count <= 1)
        {
            throw new BadRequestException("ZeroFriendsIssue(((");
        }

        if (game.State != GameState.Waiting)
        {
            throw new BadRequestException("Invalid game state");
        }

        game.State = GameState.Running;

        await _context.SaveChangesAsync();

        // Call question distribution backround proccess
        await _scheduleService.Schedule(DistributeQuestionJob.Create(game.CurrentRoundId ?? 0,
            TimeSpan.FromSeconds(GameConstants.DISTRIBUTE_DELAY_IN_SECONDS)));

        await _hub.Clients.Group(gameId.ToString()).SendAsync("GameStarting");
    }

    public async Task RevealResults(int gameId)
    {
        var game = NotFoundException.ThrowIfNull(
            await _context.Games
                .Include(x => x.CurrentRound)
                    .ThenInclude(x => x.Answers)
                        .ThenInclude(x => x.AnsweredBy)
                .Include(x => x.Rounds)
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == gameId));

        var round = NotFoundException.ThrowIfNull(game.CurrentRound);

        UpdatePlayerScores(round, 1);  // TODO: Add current round num

        game.Rounds.Remove(round);
        game.CurrentRound = game.Rounds.FirstOrDefault();

        var playerDtos = _mapper.Map<IEnumerable<PlayerDto>>(game.Players);
        await _hub.Clients.Group(gameId.ToString()).SendAsync("RevealAnswer", round.AvailableAnswers.Where(x => x.CorrectnessCoefficient > 0), playerDtos);

        await _context.SaveChangesAsync();

        if (game.CurrentRound != null)
        {
            await _scheduleService.Schedule(DistributeQuestionJob.Create(game.CurrentRoundId ?? 0,
                TimeSpan.FromSeconds(GameConstants.DISTRIBUTE_DELAY_IN_SECONDS)));
            return;
        }

        await EndGame(gameId);
    }

    public async Task EndGame(int gameId)
    {
        var game = NotFoundException.ThrowIfNull(await _context.Games.FirstOrDefaultAsync(x => x.Id == gameId));

        game.State = GameState.Ended;
        await _context.SaveChangesAsync();
        await _hub.Clients.Group(gameId.ToString()).SendAsync("GameEnded");
    }

    private void UpdatePlayerScores(Question question, int roundNumber)
    {
        int answerCount = 1;
        foreach (var answer in question.Answers)
        {
            var coefficient = question.AvailableAnswers.FirstOrDefault(x => x.Text == answer.Value)?.CorrectnessCoefficient ?? 0;

            answer.AnsweredBy.Score += CalculateScore(answerCount, roundNumber, coefficient);

            if (coefficient > 0)
            {
                answerCount++;
            }
        }
    }

    private int CalculateScore(int answerNum, int round, double coefficient)
    {
        var roundModifier = 1 + round / 10.0;
        var answerNumModifier = coefficient > 0 ? 1 + answerNum / 10.0 : 1;

        return (int)(GameConstants.BASE_SCORE / answerNumModifier * roundModifier * coefficient);
    }

    private async Task<Game?> GetGameAsync(int id)
    {
        return await _context.Games
            .Include(x => x.Players)
            .Include(x => x.Rounds)
                .ThenInclude(x => x.Answers)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
