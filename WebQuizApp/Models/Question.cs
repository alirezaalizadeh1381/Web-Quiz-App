using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebQuizApp.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }

        [Required]
        [StringLength(500)]
        public string? Content { get; set; }

        
        public string? CodeSnippet { get; set; }

        [Required]
        public string? CorrectAnswer { get; set; }
        [ValidateNever]

        public Test? Test { get; set; }
    }
}
