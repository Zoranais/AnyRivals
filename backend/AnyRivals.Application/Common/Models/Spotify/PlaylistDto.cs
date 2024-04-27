namespace AnyRivals.Application.Common.Models.Spotify;
public class PlaylistDto
{
    public ICollection<PlaylistTrackObjectDto> Items { get; set; } = [];

    public string? Next { get; set; }
}
