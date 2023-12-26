using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class Vm_PiClosing
    {
        public Vm_PiClosing()
        {
            ProjectMasterModels = new List<ProjectMasterModel>();
            ProjectMasterModel = new ProjectMasterModel();
            PmPiClosingModel=new Pm_PiClosingModel();
            PmPiClosingModels=new List<Pm_PiClosingModel>();
        }

        public List<Pm_PiClosingModel> PmPiClosingModels { get; set; }
        public Pm_PiClosingModel PmPiClosingModel { get; set; } 
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public long ProjectMasterId { get; set; }
        public long Id { get; set; }
        //public List<FilesDetail> FilesDetails { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoCategory { get; set; }
        public string EmployeeCode { get; set; }
        public long? ProjectManagerUserId { get; set; }
        public decimal? Amount { get; set; }
         [Required(ErrorMessage = "Remarks is Required")]
        public string Remarks { get; set; }
      
        public decimal? DeductionAmount { get; set; }
        public string D_Remarks { get; set; }
        public decimal? FinalAmount { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public long? Year { get; set; }
        public string DepartmentName { get; set; }
        public string UploadedFile { get; set; }
        public HttpPostedFileBase UploadedFilePath { get; set; }
           [Required(ErrorMessage = "ClosingType is Required")]
        public string ClosingType { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        [Required(ErrorMessage = "ClosingDate is Required")]
        public DateTime? ClosingDate { get; set; }
        public long? ClosingAmount { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }
    }
}