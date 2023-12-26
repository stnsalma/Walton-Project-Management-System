using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PostCommentModel
    {
        public long PostCommentId { get; set; }
        public long? SwQcAllProjectIssueId { get; set; }
        public string Message { get; set; }
        public long? CommentedBy { get; set; }
        public DateTime? CommentedDate { get; set; }
        public bool? IsApproved { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }

        //extra property
        public string CommenterName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}