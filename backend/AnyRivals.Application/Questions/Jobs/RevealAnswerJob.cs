﻿using Quartz;
using AnyRivals.Application.Common.Helpers;
using AnyRivals.Application.Games.Services;

namespace AnyRivals.Application.Questions.Jobs;
public class RevealAnswerJob : IJob
{
    private readonly IGameService _gameService;

    public RevealAnswerJob(IGameService gameService)
    {
        _gameService = gameService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var gameId = (int)dataMap["gameId"];

        await _gameService.RevealResults(gameId);
    }

    public static (IJobDetail jobDetail, ITrigger trigger) Create(int gameId, TimeSpan delay)
    {
        var job = JobBuilder
            .Create<RevealAnswerJob>()
            .Build();

        job.JobDataMap.Put("gameId", gameId);

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity(TriggerKeyHelper
                .CreateRevealAnswerKey(gameId))
            .StartAt(DateTime.UtcNow + delay)
            .Build();

        return (job, trigger);
    }
}
