namespace WebQuizApp.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ITestRepository Tests { get;}
        IQuestionRepository Questions { get;}
        Task<int> CompleteAsync();
    }
}
