using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class DiscussionModel
    {
        public long DiscussionId { get; set; }
        public string Comment { get; set; }
        public string AddedByName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}