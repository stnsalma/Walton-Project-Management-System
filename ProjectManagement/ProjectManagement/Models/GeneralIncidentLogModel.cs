using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentLogModel
    {
        public long LogId { get; set; }
        public long? GeneralIncidentId { get; set; }
        public string GeneralIncidentTitle { get; set; }
        public string GeneralIncidentCategories { get; set; }
        public string GeneralIncidentDetails { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpadatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? ForwaredBy { get; set; }
        public DateTime? ForwardedDate { get; set; }
        public string ForwardRemark { get; set; }
        public string ForwardByRole { get; set; }
        public string ForwardByName { get; set; }
        public string AddedByName { get; set; }
        public DateTime? DiscloseDate { get; set; }
        public string RefferedRole { get; set; }
        public string RoleDescription { get; set; }
        public string FileUploadPath { get; set; }
        public string Status { get; set; }
        public string ModelName { get; set; }
        public string Issues { get; set; }
    }
}