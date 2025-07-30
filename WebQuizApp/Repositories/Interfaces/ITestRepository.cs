using WebQuizApp.Models;

namespace WebQuizApp.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<Test> GetByIdAsync(int id);
        Task<IEnumerable<Test>>GetAllAsync();
        Task AddAsync(Test test);
        void Update(Test test);
        void Remove(Test test);
        Task<bool> ExistsAsync(int id);
    }
}
