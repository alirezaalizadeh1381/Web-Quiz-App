using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebQuizApp.Models;

[AllowAnonymous]  
public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        returnUrl ??= Url.Content("~/");  // Default to homepage if no returnUrl.
        return LocalRedirect(returnUrl);  // Safe redirect to prevent loops.
    }

    // Optional: Handle GET requests (some setups allow GET for logout, though POST is safer).
    public IActionResult OnGet()
    {
        return Redirect("~/");  // Redirect to homepage if accessed directly.
    }
}