using WebQuizApp.Models;

namespace WebQuizApp.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetByTestIdAsync(int testId);
        Task AddAsync(Question question);
        void Update(Question question);
        void Remove(Question question);
        Task<bool> ExistsAsync(int id);
    }
}
