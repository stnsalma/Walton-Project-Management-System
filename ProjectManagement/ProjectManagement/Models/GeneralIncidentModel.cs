using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentModel
    {
        public long GeneralIncidentId { get; set; }
        public string GeneralIncidentTitle { get; set; }
        public string GeneralIncidentCategories { get; set; }
        public string GeneralIncidentDetails { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpadatedDate { get; set; }
        public string IsValid { get; set; }
        public DateTime? DiscloseDate { get; set; }
        public string AddedByName { get; set; }
        public string DisclosedByName { get; set; }
        public string DiscloseRemark { get; set; }
        public long? DisclosedBy { get; set; }
        public string Status { get; set; }
        public long? ReassignId { get; set; }
        public string ReassignRemark { get; set; }
        public DateTime? ReassignDate { get; set; }
        public long? ReassignedBy { get; set; }
        public string FileUploadPath { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
        public string RefferedRole { get; set; }
        public string RoleDescription { get; set; }
        public string ModelName { get; set; }
        public string Issues { get; set; }
        public long? SubmittedBy { get; set; }
        public string SubmittedByName { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string SubmitRemark { get; set; }
    }
}