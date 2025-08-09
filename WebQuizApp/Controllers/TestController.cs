using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Security.Claims;
using System.Text.Json;
using WebQuizApp.Data;
using WebQuizApp.Models;
using WebQuizApp.ViewModels;


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


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get all active tests
            var tests = await _context.Tests
                .Where(t => t.IsActive)
                .ToListAsync();

            // Get latest results using a subquery that EF can translate
            var latestResults = await (
                from r in _context.UserTestResults
                where r.UserId == userId
                where r.AttemptDate == _context.UserTestResults
                .Where(r2 => r2.TestId == r.TestId && r2.UserId == userId)
                .Max(r2 => r2.AttemptDate)
                select r).ToListAsync();

            // Create a view model
            var model = tests.Select(test =>
            {
                var result = latestResults.FirstOrDefault(r => r.TestId == test.Id);
                return new TestWithStatusViewModel
                {
                    Test = test,
                    Passed = result?.Passed,
                    Score = result?.Score
                };
            }).ToList();
       
            return View(model);
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

        [HttpPost("SubmitTest")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTest(int testId, Dictionary<int, string> answers)
        {

            var uniqueAnswers = new Dictionary<int, string>();

            if (answers != null)
            {
                foreach (var answer in answers)
                {
                    // Take the last answer if duplicates exist
                    uniqueAnswers[answer.Key] = answer.Value;
                }
            }

            // Continue with the uniqueAnswers dictionary instead of the original
            if (uniqueAnswers.Count == 0)
            {
                _logger.LogWarning("Invalid or missing answers submitted for test ID {TestId}", testId);
                return BadRequest("Invalid or missing answers.");
            }

         
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var test = await _context.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null) return NotFound();

            if(test.Questions == null || test.Questions.Count == 0)
            {
                return BadRequest("This test has no questions.");
            }

            // Calculate score
            int correctCount = 0;
            var comparison = test.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase; 
            foreach (var question in test.Questions)
            {
               if(uniqueAnswers.TryGetValue(question.Id , out var userAnswer))
                {
                    var ua = userAnswer?.Trim() ?? string.Empty;
                    var ca = question.CorrectAnswer?.Trim() ?? string.Empty;
                    if (string.Equals(ua, ca, comparison))
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
                AttemptDate = DateTime.UtcNow,
                UserAnswers = uniqueAnswers.Select(a => new UserAnswer
                {
                    QuestionId = a.Key,
                    Answer = a.Value?.Trim()
                }).ToList()
            };
            
            _context.UserTestResults.Add(result);
            await _context.SaveChangesAsync(); // First Save
            
            _logger.LogInformation("User {UserId} submitted test ID {TestId} with score {Score}" , userId , testId, score);
            return RedirectToAction("TestResult", new { resultId = result.Id });
        }

        [HttpGet("TestResult/{resultId}")]
        [Authorize]
        public async Task<IActionResult> TestResult(int resultId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _context.UserTestResults
                .Include(r => r.Test)
                .ThenInclude(t => t.Questions)
                .FirstOrDefaultAsync(r => r.Id == resultId && r.UserId == userId);

            if (result == null) return NotFound();

            // Fetch user answers for this result
            var userAnswers = await _context.UserAnswers
                .Where(ua => ua.ResultId == resultId)
                .GroupBy(ua => ua.QuestionId)
                .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault())
                .ToDictionaryAsync(ua => ua.QuestionId, ua => ua);

            var comparison = result.Test.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            // Build answer reviews 
            var answerReviews = new List<AnswerReview>();
            foreach (var question in result.Test.Questions)
            {
                userAnswers.TryGetValue(question.Id, out var userAnswer);

                var ua = userAnswer?.Answer?.Trim() ?? string.Empty;
                var ca = question.CorrectAnswer?.Trim() ?? string.Empty;

                answerReviews.Add(new AnswerReview
                {
                    QuestionContent = question.Content,
                    CodeSnippet = question.CodeSnippet,
                    UserAnswer = string.IsNullOrWhiteSpace(userAnswer?.Answer) ? "No answer provided" : userAnswer.Answer,
                    CorrectAnswer = question.CorrectAnswer,
                    IsCorrect =   !string.IsNullOrEmpty(userAnswer?.Answer) && string.Equals(ua, ca , comparison)
                });
            }

            // Create view model
            var viewModel = new TestResultViewModel
            {
                ResultId = result.Id,
                TestId = result.TestId,
                TestName = result.Test.Name,
                Score = result.Score,
                Passed = result.Passed,
                AttemptDate = result.AttemptDate,
                AnswerReviews = answerReviews
            };
            return View(viewModel);
        }
    }
}
