using Microsoft.EntityFrameworkCore;
using Quartz;
using AnyRivals.Application.Common.Interfaces.Data;

namespace AnyRivals.Application.Games.Jobs;
public class DestroyInactiveGameJob : IJob
{
    private readonly IDataAccessContext _context;

    public DestroyInactiveGameJob(IDataAccessContext context)
    {
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var gameId = (int)dataMap["gameId"];

        var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == gameId);

        if (game != null && game.Players.Count == 0)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
    }

    public static (IJobDetail jobDetail, ITrigger trigger) Create(int gameId, TimeSpan delay)
    {
        var job = JobBuilder
            .Create<DestroyInactiveGameJob>()
            .Build();

        job.JobDataMap.Put("gameId", gameId);

        var trigger = TriggerBuilder
            .Create()
            .StartAt(DateTime.UtcNow + delay)
            .Build();

        return (job, trigger);
    }
}
