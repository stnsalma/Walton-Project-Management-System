using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class DiscussionFileUploadModel
    {
        public long Id { get; set; }
        public long? DiscussionId { get; set; }
        public string FileUploadPath { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}