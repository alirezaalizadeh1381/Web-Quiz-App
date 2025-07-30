using AspNetCoreGeneratedDocument;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebQuizApp.Models;
using WebQuizApp.Repositories.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace WebQuizApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
       private readonly IUnitOfWork _unitOfWork;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }

        // Test management
        public async Task<IActionResult> Tests()
        {
            var tests = await _unitOfWork.Tests.GetAllAsync();
            return View(tests);
        }

        public IActionResult CreateTest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTest(Test test)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Tests.AddAsync(test);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Tests");
            }
            return View(test);
        }

        public async Task<IActionResult> EditTest(int id)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(id);
            if (test == null) return NotFound();
            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTest(int id , Test test)
        {
            if (id != test.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _unitOfWork.Tests.Update(test);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Tests");
            }
            return View(test);
        }

        [HttpGet("DeleteTest/{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(id);
            if (test == null) return NotFound();
            return View(test);
        }

        [HttpPost("DeleteTest/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTestConfirmed(int id)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(id);
            if (test != null)
            {
                _unitOfWork.Tests.Remove(test);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction("Tests");
        }

        // Question Management
        [HttpGet("Questions")]
        public async Task<IActionResult> Questions(int testId)
        {
            ViewBag.Test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (ViewBag.Test == null) return NotFound();

            var questions = await _unitOfWork.Questions.GetByTestIdAsync(testId);
            return View(questions);
        }

        public async Task<IActionResult> CreateQuestion(int testId)
        {
            var test = await _unitOfWork.Tests.GetByIdAsync(testId);
            if (test == null) return NotFound();

            return View(new Question { TestId = testId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Questions.AddAsync(question);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Questions", "Admin", new { testId = question.TestId });
            }
            return View();
        }

        public async Task<IActionResult> EditQuestion(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null) return NotFound();
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(int id, Question question)
        {
            if (id != question.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _unitOfWork.Questions.Update(question);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Questions), new { testId = question.TestId });
            }
            return View(question);
        }

        [HttpGet("DeleteQuestion/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null) return NotFound();
            return View(question);
        }

        [HttpPost("DeleteQuestion/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionConfirmed(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question != null)
            {
                var testId = question.TestId;
                _unitOfWork.Questions.Remove(question);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Questions", "Admin" , new { testId });
            }
            return RedirectToAction("Tests");
        }
    }
}
