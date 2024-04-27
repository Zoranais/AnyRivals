using AnyRivals.Application.Common.Interfaces.API;
using AnyRivals.Application.Common.Models.Spotify;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AnyRivals.Infrastructure.API.Spotify;
public class SpotifyApiClient: ISpotifyApiClient
{
    private readonly HttpClient _client;
    private readonly AccessTokenStorage _tokenStorage;

    public SpotifyApiClient(HttpClient client, AccessTokenStorage tokenStorage)
    {
        _client = client;
        _tokenStorage = tokenStorage;
    }

    public async Task<ICollection<TrackObjectDto>> GetPlaylistById(string id)
    {
        var token = await _tokenStorage.GetAccessToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"{token.TokenType} {token.AccessToken}");
        PlaylistDto? playlist = null;
        List<TrackObjectDto> tracks = [];

        do
        {
            var response = await _client.GetAsync(playlist?.Next ?? $"playlists/{id}/tracks?limit=50");
            playlist = await response.Content.ReadFromJsonAsync<PlaylistDto>(SpotifyConstants.SerializerSettings);

            if (playlist is not null)
            {
                tracks.AddRange(playlist.Items.Select(x => x.Track));
            }
        }
        while (playlist?.Next is not null);

        return tracks;
    }
}
