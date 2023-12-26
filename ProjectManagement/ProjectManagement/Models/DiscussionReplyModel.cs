using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class DiscussionReplyModel
    {
        public long Id { get; set; }
        public long? DiscussionId { get; set; }
        public string Reply { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}