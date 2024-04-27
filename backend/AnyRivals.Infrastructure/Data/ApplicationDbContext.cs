using Microsoft.EntityFrameworkCore;
using AnyRivals.Domain.Entities;
using System.Reflection;
using AnyRivals.Application.Common.Interfaces.Data;

namespace AnyRivals.Infrastructure.Data;
public class ApplicationDbContext : DbContext, IMigrationContext, IDataAccessContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Game> Games { get; set; }

    public DbSet<Question> Questions { get; set; }

    public DbSet<Player> Players { get; set; }

    public Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default) 
        => Database.GetPendingMigrationsAsync(cancellationToken);

    public Task MigrateAsync(CancellationToken cancellationToken = default) 
        => Database.MigrateAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
