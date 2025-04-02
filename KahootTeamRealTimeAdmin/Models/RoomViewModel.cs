using System.ComponentModel.DataAnnotations;

namespace KahootTeamRealTimeAdmin.Models
{
    public class RoomViewModel
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}
