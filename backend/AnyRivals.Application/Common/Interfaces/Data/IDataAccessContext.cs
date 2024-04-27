using Microsoft.EntityFrameworkCore;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Application.Common.Interfaces.Data;
public interface IDataAccessContext
{
    public DbSet<Game> Games { get; }

    public DbSet<Question> Questions { get; }

    public DbSet<Player> Players { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
