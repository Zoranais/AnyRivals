using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Infrastructure.Data.Configurations;
internal class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder
            .HasOne(x => x.AnsweredBy)
            .WithMany()
            .HasForeignKey(x => x.AnsweredById);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.Answers);

        builder.Property(x => x.Value)
            .HasMaxLength(100);
    }
}
