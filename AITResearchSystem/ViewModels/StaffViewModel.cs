using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AITResearchSystem.ViewModels
{
    public class StaffViewModel
    {
        [Required(ErrorMessage = "The email is required")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is required")]
        [MinLength(4, ErrorMessage = "Invalid password!")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
