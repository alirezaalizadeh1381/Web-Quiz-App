using WebQuizApp.Models;

namespace WebQuizApp.ViewModels
{
    public class TestWithStatusViewModel
    {
        public Test Test { get; set; }
        public bool? Passed { get; set; }
        public int? Score { get; set; }
    }
}
