using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebQuizApp.Data;
using WebQuizApp.Models;

namespace WebQuizApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManger;
        public HomeController(ApplicationDbContext context , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManger = userManager;
        }

        
        public async Task<IActionResult> Index()
{
    var model = new HomeViewModel
    {
        TotalTests = await _context.Tests.CountAsync(),
        TotalQuestions = await _context.Questions.CountAsync(),
        TotalParticipants = await _context.UserTestResults
            .Select(r => r.UserId)
            .Distinct()
            .CountAsync(),
        FeaturedTests = await _context.Tests
            .Include(t => t.Questions)
            .Include(t => t.UserTestResults) // Include UserTestResults
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.UserTestResults.Count) // Now this will work
            .Take(3)
            .ToListAsync()
    };

    return View(model);
}

        /**public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }**/
    }
}
