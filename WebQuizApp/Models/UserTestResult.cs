using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WebQuizApp.Models
{
    public class UserTestResult
    {
        
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int TestId { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;
        public int Score { get; set; }
        public bool Passed { get; set; }

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Test? Test { get; set; }

        [InverseProperty(nameof(UserAnswer.TestResult))]
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
