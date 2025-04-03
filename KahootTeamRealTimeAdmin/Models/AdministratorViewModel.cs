using System.ComponentModel.DataAnnotations;

namespace KahootTeamRealTimeAdmin.Models
{
    public class AdministratorViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
