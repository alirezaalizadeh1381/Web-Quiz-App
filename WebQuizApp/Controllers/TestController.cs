using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebQuizApp.Data;
using WebQuizApp.Models;

namespace WebQuizApp.Controllers
{
    [Authorize]
    [Route("Tests")]
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestController(ApplicationDbContext context , UserManager<ApplicationUser> userManger, ILogger<TestController> logger)
        {
            _context = context;
            _userManager = userManger;
            _logger = logger;
        }
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to access Tests/Index");
                return Challenge(); // This will redirect to login page
            }
            var tests = await _context.Tests
                .Include(t => t.Questions)
                .Include(t => t.UserTestResults)
                .Where(t => t.IsActive)
                .ToListAsync();
            return View(tests);
        }
        [HttpGet("TakeTest/{id}")]
        [Authorize]
        public async Task<IActionResult> TakeTest(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var test = await _context.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return NotFound();

            // Allow unlimited retakes
            return View(test);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitTest(int testId, Dictionary<int, string> answers)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var test = await _context.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null) return NotFound();

            // Calculate score
            int correctCount = 0;
            foreach (var question in test.Questions)
            {
                if (answers.TryGetValue(question.Id, out var userAnswer) &&
                    userAnswer.Trim().Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    correctCount++;
                }
            }

            int score = (int)Math.Round((double)correctCount / test.Questions.Count * 100);
            bool passed = score >= test.PassingScore;

            // Save new result (always create new record)
            var result = new UserTestResult
            {
                UserId = userId,
                TestId = testId,
                Score = score,
                Passed = passed,
                AttemptDate = DateTime.UtcNow
            };

            _context.UserTestResults.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction("TestResult", new { resultId = result.Id });
        }
        public async Task<IActionResult> TestResult(int resultId)
        {
            var result = await _context.UserTestResults
                .Include(r => r.Test)
                .FirstOrDefaultAsync(r => r.Id == resultId);

            if (result == null) return NotFound();

            return View(result);
        }
    }
}
