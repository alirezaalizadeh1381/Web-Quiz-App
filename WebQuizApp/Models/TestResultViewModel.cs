namespace WebQuizApp.Models
{
    public class TestResultViewModel
    {
        public int ResultId { get; set; }
        public int TestId { get; set; }
        public string? TestName { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
        public DateTime AttemptDate { get; set; }
        public List<AnswerReview> AnswerReviews { get; set; } = new();
    }

    public class AnswerReview
    {
        public string? QuestionContent { get; set; }
        public string? CodeSnippet { get; set; }
        public string? UserAnswer { get; set; }
        public string? CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}

