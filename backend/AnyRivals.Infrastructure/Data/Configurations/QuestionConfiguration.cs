using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Infrastructure.Data.Configurations;
internal class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    private readonly JsonSerializerSettings serializerSettings;

    public QuestionConfiguration()
    {
        serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
    }

    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .Property(x => x.AvailableAnswers)
            .HasConversion(
                x => JsonConvert.SerializeObject(x, serializerSettings), 
                x => JsonConvert.DeserializeObject<ICollection<AvailableAnswer>>(x, serializerSettings) ?? new List<AvailableAnswer>(0))
            .HasMaxLength(400);

        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.Rounds)
            .HasForeignKey(x => x.GameId);

        builder
            .HasMany(x => x.Answers)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Title)
            .HasMaxLength(36);

        builder.Property(x => x.Source)
            .HasMaxLength(300);
    }
}
