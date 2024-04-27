using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Infrastructure.Data.Configurations;
internal class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder
            .HasMany(x => x.Rounds)
            .WithOne(x => x.Game)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Players)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.CurrentRound)
            .WithOne()
            .HasForeignKey<Game>(x => x.CurrentRoundId)
            .IsRequired(false);

        builder.Property(x => x.Name)
            .HasMaxLength(36);

        builder.Property(x => x.GamePassword)
            .HasMaxLength(16)
            .IsRequired(false);
    }
}
