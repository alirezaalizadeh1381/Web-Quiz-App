namespace WebQuizApp.Models
{
    public class HomeViewModel
    {
        public int TotalTests { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalParticipants { get; set; }
        public List<Test>? FeaturedTests { get; set; }
    }
}
