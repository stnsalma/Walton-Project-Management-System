using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

namespace ProjectManagement.Models
{
    public class CmnUserModel
    {
        public CmnUserModel()
        {
           // ProjectMasterModels = new List<ProjectMasterModel>();
            FilesDetails = new List<FilesDetail>();
        }

        public List<FilesDetail> FilesDetails { get; set; }
        public string FilesDetail { get; set; }
        public string Upload { get; set; }



        public long CmnUserId { get; set; }
        [Required]
        [Display(Name = "User Full Name")]
        public string UserFullName { get; set; }
        [Required]
        [Remote("CheckUserName", "Home")]
        public string UserName { get; set; }
        //[RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 3 characters required")]
        [Required]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        //[Required(ErrorMessage = "Required")]
        //[StringLength(14, MinimumLength = 11, ErrorMessage = "Invalid Mobile Number.Mobile number is minimum 11 caracter and maximum 14 character")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        public string EmployeeCode { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public long? AssignBy { get; set; }
        public DateTime? AssignDate { get; set; }
        public DateTime? AssignStartDate { get; set; }
        public DateTime? AssignEndDate { get; set; }
        public string AssignRoles { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ExtendedRoleName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Designation { get; set; }
        public string LastLoginDateTime { get; set; }
        public Nullable<bool> IsRememberMailSend { get; set; }
        public string LastLoginIpAddress { get; set; }
        public Nullable<System.DateTime> LastPasswordUpdateDate { get; set; }
       
        public string ProfilePictureUrl { get; set; }
         [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfilePictureFile { get; set; }


         public string DeletedUsersList { get; set; }
         public int DeletedUserId { get; set; }
         public string DeletedEmpCode { get; set; }

        #region userUpdateAdd

        public Boolean IsUserInfoUpdated { get; set; }
        public Boolean IsPasswordUpdated { get; set; }

        [Required]
        public List<String> RoleList { get; set; }
        #endregion


        public string WebServerUrl { get; set; }
        //public List<ProjectMasterModel> ProjectMasterModels { get; set; }
      
        //new add for kpi
        public string Department { get; set; }
        public string Section { get; set; }
        public string LineManager { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string Status { get; set; }
        public decimal? ServiceLength { get; set; }
        public int? ServiceLength1 { get; set; }
        public int? TotalAverageScorePercent { get; set; }
    }
}