using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ChinaQcInspectionsClearanceModel
    {
        public ChinaQcInspectionsClearanceModel()
        {
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public long Id { get; set; }
        public long MainId { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string Orders { get; set; }
        public long? OrderQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? InspectionStartDate { get; set; }
        public string MaterialType { get; set; }
        public int? LotNo { get; set; }
        public long? LotQuantity { get; set; }
        public int? NoOfTimeOfInspection { get; set; }
        public string InspectionAttachment { get; set; }
        public string InspectionStatus { get; set; }
        public string ClearanceStatus { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? AddedDate { get; set; }

        //
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string RoleDetails { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string ToMail { get; set; }
        public string FromMail { get; set; }
        public string CcMail { get; set; }
        public string MyStatus { get; set; }
        //   
        public Nullable<long> MailListId { get; set; }
        public string Remarks { get; set; }
        public string RequestSent { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? RequestDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? TimeOfAction { get; set; }
        public int TimeDelay { get; set; }
        public string Details { get; set; }
        public string BtnDetails1 { get; set; }
        public string BtnDetails2 { get; set; }
        public int ChinaQcInspectionCount { get; set; }
    }
}