using AnyRivals.Application.Common.Models.Spotify;
using System.Net.Http.Json;

namespace AnyRivals.Infrastructure.API.Spotify;
public class SpotifyAuthorizationApiClient
{
    private readonly HttpClient _client;
    public SpotifyAuthorizationApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<AccessTokenDto?> GetAccessToken()
    {
        var response = await _client.PostAsync("token", new FormUrlEncodedContent(SpotifyConstants.AccessTokenFormData));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AccessTokenDto>(SpotifyConstants.SerializerSettings);
    }
}
