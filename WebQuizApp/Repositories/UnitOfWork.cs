using WebQuizApp.Data;
using WebQuizApp.Repositories.Interfaces;

namespace WebQuizApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITestRepository _testRepository;
        private IQuestionRepository _questionRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITestRepository Tests =>
            _testRepository ??= new TestRepository(_context);

        public IQuestionRepository Questions =>
            _questionRepository ??= new QuestionRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
