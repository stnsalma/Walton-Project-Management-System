using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectPoFeedbackModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectModel { get; set; }
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public string FeedBack { get; set; }
        public long? AddedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AllowReorder { get; set; }
        public string AddedByName { get; set; }
        public string FileUploadPath { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
        public string FeedbackRole { get; set; }
        public string Department { get; set; }
        public string SourcingComment { get; set; }
        public DateTime? SourcingCommentDate { get; set; }
        public long? SourcingCommentBy { get; set; }
        public string SourcingCommentByName { get; set; }
        public string SourcingAllowReorder { get; set; }
        public long? ManagementCommentBy { get; set; }
        public string ManagementComment { get; set; }
        public DateTime? ManagementCommentDate { get; set; }
        public string ManagementDecision { get; set; }
        public string OnBehalfOf { get; set; }
    }
}