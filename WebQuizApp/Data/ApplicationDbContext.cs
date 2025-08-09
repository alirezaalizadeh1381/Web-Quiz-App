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
       public DbSet<UserAnswer> UserAnswers { get; set; }

        // In your ApplicationDbContext class
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // This line is CRITICAL
            base.OnModelCreating(builder);

            // Add your custom configurations here
            builder.Entity<UserTestResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.UserAnswers)
                    .WithOne(e => e.TestResult)
                    .HasForeignKey(e => e.ResultId);
            });

            builder.Entity<UserAnswer>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }


    }


}
