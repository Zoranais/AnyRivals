using AnyRivals.Domain.Common;
using AnyRivals.Domain.Enums;

namespace AnyRivals.Domain.Entities;
public class Question : BaseEntity
{
    public string? Title { get; set; }

    public string? Source { get; set; }

    public int GameId { get; set; }

    public Game Game { get; set; }

    public QuestionType Type { get; set; }

    public ICollection<AvailableAnswer> AvailableAnswers { get; set; } = [];

    public ICollection<Answer> Answers { get; set; } = [];
}
