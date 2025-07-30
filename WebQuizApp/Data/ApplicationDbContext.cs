using Microsoft.EntityFrameworkCore;
using WebQuizApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace WebQuizApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

       public DbSet<Test>Tests { get; set; }
       public DbSet<Question> Questions { get; set; }
       public DbSet<UserTestResult> UserTestResults { get; set; } 
    }
}
