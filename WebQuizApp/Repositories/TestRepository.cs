using Microsoft.EntityFrameworkCore;
using WebQuizApp.Data;
using WebQuizApp.Models;
using WebQuizApp.Repositories.Interfaces;

namespace WebQuizApp.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly ApplicationDbContext _context;

        public TestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Test> GetByIdAsync(int id)
        {
            return await _context.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Test>> GetAllAsync()
        {
            return await _context.Tests
                .Include(t => t.Questions)
                .ToListAsync();
        }

        public async Task AddAsync(Test test)
        {
            await _context.Tests.AddAsync(test);
        }

        public void Update(Test test)
        {
            _context.Tests.Update(test);
        }

        public void Remove(Test test)
        {
            _context.Tests.Remove(test);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tests.AnyAsync(t => t.Id == id);
        }
    }
}
