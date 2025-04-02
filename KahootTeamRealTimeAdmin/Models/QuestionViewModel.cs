using System.ComponentModel.DataAnnotations;

namespace KahootTeamRealTimeAdmin.Models
{
    public class QuestionViewModel
    {
        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public Guid? RoomId { get; set; }
    }
}
