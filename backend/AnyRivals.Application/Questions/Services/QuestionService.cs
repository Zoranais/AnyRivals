using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using AnyRivals.Application.Common.Constants;
using AnyRivals.Application.Common.Exceptions;
using AnyRivals.Application.Common.Models.DTOs;
using AnyRivals.Application.Hubs;
using AnyRivals.Application.Questions.Jobs;
using AnyRivals.Application.Scheduler;
using System.Runtime;
using AnyRivals.Application.Common.Interfaces.Data;

namespace AnyRivals.Application.Questions.Services;
public class QuestionService : IQuestionService
{
    private readonly IDataAccessContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<GameHub> _hub;
    private readonly IScheduleService _scheduleService;

    public QuestionService(
        IDataAccessContext context,
        IMapper mapper,
        IHubContext<GameHub> hub,
        IScheduleService scheduleService)
    {
        _context = context;
        _mapper = mapper;
        _hub = hub;
        _scheduleService = scheduleService;
    }

    public async Task DistributeQuestion(int questionId)
    {
        var question = NotFoundException.ThrowIfNull(
            await _context.Questions.FirstOrDefaultAsync(x => x.Id == questionId));

        var roundDto = _mapper.Map<RoundDto>(question);

        await _hub.Clients.Group(question.GameId.ToString()).SendAsync("DistributeQuestion", roundDto);

        // Schedule reveal
        await _scheduleService.Schedule(RevealAnswerJob.Create(question.GameId,
            TimeSpan.FromSeconds(GameConstants.REVEAL_DELAY_IN_SECONDS)));
    }
}
