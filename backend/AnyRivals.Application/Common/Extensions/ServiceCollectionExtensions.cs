using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using AnyRivals.Application.Common.Behaviours;
using AnyRivals.Application.Games.Services;
using AnyRivals.Application.Players.Services;
using AnyRivals.Application.Questions.Services;
using AnyRivals.Application.Scheduler;
using System.Reflection;

namespace AnyRivals.Application.Common.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditBehaviour<,>));
        });

        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IQuestionService, QuestionService>();

        services.AddQuartz();
        services.AddQuartzHostedService();

        return services;
    }
}
