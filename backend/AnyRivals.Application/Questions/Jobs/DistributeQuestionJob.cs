using Quartz;
using AnyRivals.Application.Common.Helpers;
using AnyRivals.Application.Common.Interfaces;
using AnyRivals.Application.Questions.Services;

namespace AnyRivals.Application.Questions.Jobs;
public class DistributeQuestionJob : IJob
{
    private readonly IQuestionService _questionService;

    public DistributeQuestionJob(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var questionId = (int)dataMap["questionId"];

        await _questionService.DistributeQuestion(questionId);
    }

    public static (IJobDetail jobDetail, ITrigger trigger) Create(int questionId, TimeSpan delay)
    {
        var job = JobBuilder
            .Create<DistributeQuestionJob>()
            .Build();

        job.JobDataMap.Put("questionId", questionId);

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity(TriggerKeyHelper
                .CreateDistributeAnswerKey(questionId))
            .StartAt(DateTime.UtcNow + delay)
            .Build();

        return (job, trigger);
    }
}