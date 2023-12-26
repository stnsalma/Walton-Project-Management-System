using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class IssueCommentsDetailModel
    {
        public long PostCommentId { get; set; }
        public long? SwQcAllProjectIssueId { get; set; }
        public string IssueName { get; set; }
        public string Message { get; set; }
        public long? CommentedBy { get; set; }
        public DateTime? CommentedDate { get; set; }
        public string PurchaseOrderOrdinals { get; set; }
        public string ProjectName { get; set; }

        //custom property
        public string CommenterName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}