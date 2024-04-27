using Microsoft.Extensions.DependencyInjection;
using AnyRivals.Application.Common.Models.Spotify;

namespace AnyRivals.Infrastructure.API.Spotify;
public class AccessTokenStorage
{
    private readonly IServiceProvider _serviceProvider;

    private AccessTokenDto? accessToken = null;
    public AccessTokenStorage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<AccessTokenDto> GetAccessToken()
    {
        if (accessToken == null)
        {
            accessToken = await Renew();
        }
        if ((DateTime.UtcNow - accessToken.ReceivedAt.AddSeconds(accessToken.ExpiresIn)).Minutes < 1)
        {
            accessToken = await Renew();
        }
        return accessToken;
    }

    public async Task<AccessTokenDto> Renew()
    {
        using var scope = _serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<SpotifyAuthorizationApiClient>();

        return await authService.GetAccessToken() ?? throw new Exception("Invalid Access Token");
    }
}
