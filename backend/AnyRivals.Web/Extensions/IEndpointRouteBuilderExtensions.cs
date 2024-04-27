using MediatR;
using Microsoft.AspNetCore.Mvc;
using AnyRivals.Application.Games.Commands.CreateGame;
using AnyRivals.Application.Games.Queries;
using AnyRivals.Application.Hubs;

namespace AnyRivals.Web.Extensions;

public static class IEndpointRouteBuilderExtensions
{
    public static void MapSignalRHubs(this IEndpointRouteBuilder app)
    {
        app.MapHub<GameHub>("/GameHub");
    }

    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/game", async ([FromBody] CreateGameCommand dto, IMediator mediator) =>
        {
            return Results.Created("api/game", await mediator.Send(dto));
        });

        app.MapGet("api/game/isExist/{gameId}", async (string gameId, IMediator mediator) =>
        {
            var query = new CheckGameExistanceQuery { Id = gameId };

            return Results.Ok(await mediator.Send(query));
        });

        app.MapGet("api/game", async ([FromBody] GetGamesPageQuery dto, IMediator mediator) =>
        {
            return Results.Ok(await mediator.Send(dto));
        });
    }
}
