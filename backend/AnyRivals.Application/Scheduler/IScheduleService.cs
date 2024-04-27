using Quartz;

namespace AnyRivals.Application.Scheduler;
public interface IScheduleService
{
    Task Schedule((IJobDetail jobDetail, ITrigger trigger) jobInfo);
}
