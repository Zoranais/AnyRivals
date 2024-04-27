using AnyRivals.Domain.Common;

namespace AnyRivals.Domain.Entities;
public class AvailableAnswer
{
    public string Text { get; set; }

    // To allow multiple correct answers with different rewards in custom games
    public double CorrectnessCoefficient { get; set; }
}
