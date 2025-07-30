using Microsoft.EntityFrameworkCore;
using WebQuizApp.Data;
using WebQuizApp.Models;
using WebQuizApp.Repositories.Interfaces;

namespace WebQuizApp.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<IEnumerable<Question>> GetByTestIdAsync(int testId)
        {
            return await _context.Questions
                .Where(q => q.TestId == testId)
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            // Handle empty string as null
            if (string.IsNullOrWhiteSpace(question.CodeSnippet))
            {
                question.CodeSnippet = null;
            }
            await _context.Questions .AddAsync(question);
        }

        public void Update(Question question)
        {
            _context.Questions.Update(question);
        }

        public void Remove(Question question) 
        {
            _context.Questions.Remove(question);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Questions.AnyAsync(q => q.Id == id);
        }
    }
}
