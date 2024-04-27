using AnyRivals.Domain.Common;

namespace AnyRivals.Domain.Entities;
public class Answer: BaseEntity
{
    public string? Value { get; set; }

    public Player AnsweredBy { get; set; }
    public int AnsweredById { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
