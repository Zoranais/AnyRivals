using AnyRivals.Application.Common.Models.Spotify;

namespace AnyRivals.Application.Common.Interfaces.API;
public interface ISpotifyApiClient
{
    Task<ICollection<TrackObjectDto>> GetPlaylistById(string id);
}
