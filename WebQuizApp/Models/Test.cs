using System.ComponentModel.DataAnnotations;

namespace WebQuizApp.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int PassingScore { get; set; } = 70; // Default passing score 70%
        public bool IsActive { get; set; } = true;

        public List<Question>? Questions { get; set; }
        public List<UserTestResult>? UserTestResults { get; set; }
    }
}
