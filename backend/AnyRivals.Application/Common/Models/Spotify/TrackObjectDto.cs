using System.Text.Json.Serialization;

namespace AnyRivals.Application.Common.Models.Spotify;
public class TrackObjectDto
{
    public string Name { get; set; }

    [JsonPropertyName("preview_url")]
    public string PreviewUrl { get; set; }

    public ICollection<ArtistObjectDto> Artists { get; set; } = [];

}