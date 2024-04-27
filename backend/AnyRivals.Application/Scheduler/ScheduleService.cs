using Quartz;

namespace AnyRivals.Application.Scheduler;
public class ScheduleService : IScheduleService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public ScheduleService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task Schedule((IJobDetail jobDetail, ITrigger trigger) jobInfo)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(jobInfo.jobDetail, jobInfo.trigger);
    }
}
