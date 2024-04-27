using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Infrastructure.Data.Configurations;
internal class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasOne(x => x.Game)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.GameId);

        builder.Property(x => x.Name)
            .HasMaxLength(36);
    }
}
