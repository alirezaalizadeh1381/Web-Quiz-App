using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuizApp.Models
{
    public class UserAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ResultId { get; set; }
        public int QuestionId { get; set; }
        public string? Answer {  get; set; }

        [ForeignKey(nameof(ResultId))]
        public UserTestResult? TestResult { get; set; }
    }
}
