using AnyRivals.Application.Common.Interfaces.API;
using AnyRivals.Application.Common.Interfaces.Data;
using AnyRivals.Application.Common.Interfaces.Handlers;
using AnyRivals.Application.Common.Models.Spotify;
using AnyRivals.Domain.Entities;
using AnyRivals.Domain.Enums;
using AnyRivals.Domain.Events.GameEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AnyRivals.Application.Games.EventHandlers;
public class GameCreatedEventHandler : IDomainEventHandler<GameCreatedEvent>
{
    private readonly ILogger<GameCreatedEventHandler> _logger;
    private readonly IDataAccessContext _context;
    private readonly ISpotifyApiClient _spotifyClient;

    public GameCreatedEventHandler(
        ILogger<GameCreatedEventHandler> logger,
        IDataAccessContext context,
        ISpotifyApiClient spotifyClient)
    {
        _logger = logger;
        _context = context;
        _spotifyClient = spotifyClient;
    }

    public async Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
    {
        var game = notification.Game;

        switch (game.GameType)
        {
            case GameType.MusicGuess:
            case GameType.AuthorGuess:
            case GameType.MixedGuess:
                await InitMusicGuessGame(game);
                break;
            case GameType.Custom:
                // TODO: Parse json
                break;
            default:
                break;
        }

        game.State = GameState.Waiting;
        game.AddDomainEvent(new GameInitializedEvent(game));
        await _context.SaveChangesAsync();
        _logger.LogInformation("Game {ID}:{ExternalID} initialized.", game.Id, game.ExternalId);
    }

    private async Task InitMusicGuessGame(Game game)
    {
        var id = game.Source.Replace("https://open.spotify.com/playlist/", string.Empty)
            .Split('?')
            .FirstOrDefault()
            ?? string.Empty;

        var tracks = (await _spotifyClient.GetPlaylistById(id)).Where(x => x.PreviewUrl != null).ToList();

        var trackPool = tracks.OrderBy(x => Random.Shared.Next()).ToList();

        for (int i = 0; i < game.TotalRounds; i++)
        {
            var round = new Question();

            switch (game.GameType)
            {
                case GameType.AuthorGuess:
                    round.Type = QuestionType.SpotifyAuthorGuess;
                    break;
                case GameType.MixedGuess:
                    round.Type = Random.Shared.Next(0, 2) == 1 ? QuestionType.SpotifyMusicGuess : QuestionType.SpotifyAuthorGuess;
                    break;
                default:
                    round.Type = QuestionType.SpotifyMusicGuess;
                    break;
            }

            if (trackPool.Count == 0)
            {
                trackPool =
                [..new List<TrackObjectDto>(tracks)
                    .OrderBy(x => Random.Shared.Next())];
            }
            var correctAnswer = trackPool.First();
            var randomTracks = GetRandomUniqueTracks(tracks, correctAnswer, 4);

            trackPool.Remove(correctAnswer);

            round.AvailableAnswers = randomTracks
                .Select(x => new AvailableAnswer
                {
                    Text = round.Type == QuestionType.SpotifyMusicGuess
                        ? x.Name
                        : string.Join(", ", x.Artists.Select(x => x.Name)),
                    CorrectnessCoefficient = x == correctAnswer ? 1 : 0
                })
                .OrderBy(x => Random.Shared.Next())
                .ToList();

            round.Source = correctAnswer.PreviewUrl;

            game.Rounds.Add(round);
        }
    }

    private List<TrackObjectDto> GetRandomUniqueTracks(IList<TrackObjectDto> tracks, TrackObjectDto correctAnswer, int count)
    {
        var randomTracks = new List<TrackObjectDto>(count) { correctAnswer };

        while (randomTracks.Count < count)
        {
            var item = tracks[Random.Shared.Next(0, tracks.Count)];

            if (!randomTracks.Contains(item))
            {
                randomTracks.Add(item);
            }
        }

        return randomTracks;
    }
}
