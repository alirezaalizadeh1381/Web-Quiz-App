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

        public ApplicationUser User { get; set; }
        public Test Test { get; set; }
    }
}
