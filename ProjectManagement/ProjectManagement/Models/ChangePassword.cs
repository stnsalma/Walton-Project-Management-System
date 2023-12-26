using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ProjectManagement.Models
{
    public class ChangePassword
    {
        [Required]
        [Remote("CheckPasswordChange", "Home")]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "New Password Does not match")]
        public string ConfirmNewPassword { get; set; }
    }
}