using FluentValidation;
using AnyRivals.Domain.Enums;

namespace AnyRivals.Application.Games.Commands.CreateGame;
public class CreateGameValidator: AbstractValidator<CreateGameCommand>
{
    public CreateGameValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(36);

        RuleFor(x => x.TotalRounds)
            .LessThanOrEqualTo(36)
            .GreaterThanOrEqualTo(3);

        RuleFor(x => x.Source)
            .NotNull()
            .Must((game, prop) => ValidateSource(prop, game.GameType));

        RuleFor(x => x.GamePassword)
            .MaximumLength(16);
    }

    private bool ValidateSource(string source, GameType gameType)
    {
        if (gameType != GameType.Custom)
        {
            return source.StartsWith("https://open.spotify.com/playlist/");
        }
        else
        {
            // TODO: Validate json
            return true;
        }
    }
}
