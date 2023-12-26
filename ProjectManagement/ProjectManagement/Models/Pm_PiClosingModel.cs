using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pm_PiClosingModel
    {
        public Pm_PiClosingModel()
        {
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        [Required(ErrorMessage = "ProjectName is Required")]
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoCategory { get; set; }
        public string EmployeeCode { get; set; }
        public long? ProjectManagerUserId { get; set; }
        [Required(ErrorMessage = "Remarks is Required")]
        public string Remarks { get; set; }
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
        public string MonthNo { get; set; }
        public string MonthName { get; set; }
        public string Year { get; set; }
        public decimal? DeductionAmount { get; set; }
        public decimal? FinalAmount { get; set; }
    }
}