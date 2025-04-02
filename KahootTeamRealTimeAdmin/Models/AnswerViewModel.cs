using System.ComponentModel.DataAnnotations;

namespace KahootTeamRealTimeAdmin.Models
{
    public class AnswerViewModel
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Content { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

    }
}
