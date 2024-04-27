using System.Text.Json;

namespace AnyRivals.Infrastructure.API.Spotify;

public static class SpotifyConstants
{
    public static readonly IReadOnlyDictionary<string, string> AccessTokenFormData = new Dictionary<string, string>()
    {
        { "grant_type", "client_credentials" }
    };

    public static readonly JsonSerializerOptions SerializerSettings = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower  };

}
