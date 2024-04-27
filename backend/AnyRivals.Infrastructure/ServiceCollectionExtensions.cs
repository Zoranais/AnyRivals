using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AnyRivals.Application.Common.Interfaces.API;
using AnyRivals.Infrastructure.API.Spotify;
using AnyRivals.Infrastructure.Data;
using AnyRivals.Infrastructure.Data.Interceptors;
using System.Net.Http.Headers;
using System.Text;
using AnyRivals.Application.Common.Interfaces.Data;

namespace AnyRivals.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration)
            .AddSpotifyHttpClient(configuration)
            .AddSpotifyAuthorizationHttpClient(configuration);

        services.AddSingleton<AccessTokenStorage>();

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnection"));
        });

        services.AddScoped<IMigrationContext>(x => x.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDataAccessContext>(x => x.GetRequiredService<ApplicationDbContext>());

        return services;
    }


    public static IServiceCollection AddSpotifyHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ISpotifyApiClient, SpotifyApiClient>((options) =>
        {
            options.BaseAddress = new Uri(configuration["SpotifyApiUrl"] ?? string.Empty);
        });

        return services;
    }
    public static IServiceCollection AddSpotifyAuthorizationHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<SpotifyAuthorizationApiClient>((options) =>
        {
            var clientId = configuration["Client_Id"] ?? string.Empty;
            var clientSecret = configuration["Client_Secret"] ?? string.Empty;
            options.BaseAddress = new Uri(configuration["SpotifyAuthApiUrl"] ?? string.Empty);
            options.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                $"{Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))}");
        });

        return services;
    }
}
