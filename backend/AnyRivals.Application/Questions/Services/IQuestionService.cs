namespace AnyRivals.Application.Questions.Services;
public interface IQuestionService
{
    Task DistributeQuestion(int questionId);
}
