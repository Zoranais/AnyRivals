namespace AnyRivals.Application.Common.Interfaces.Data;
public interface IMigrationContext
{
    Task MigrateAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);
}
