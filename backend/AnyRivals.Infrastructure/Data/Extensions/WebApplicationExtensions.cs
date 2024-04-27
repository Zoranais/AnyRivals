using AnyRivals.Application.Common.Interfaces.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AnyRivals.Infrastructure.Data.Extensions;
public static class WebApplicationExtensions
{
    public static async Task UseApplicationDbContext(this IHost app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<IMigrationContext>();
        await context.MigrateAsync();
    }
}
