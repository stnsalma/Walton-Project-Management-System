using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Home
{
    public class VmUpdateUserProfile
    {
        public VmUpdateUserProfile()
        {
            CmnUserModel = new CmnUserModel();
            //ChangePassword = new ChangePassword();
        }
        public CmnUserModel CmnUserModel { get; set; }
        //public ChangePassword ChangePassword { get; set; }
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