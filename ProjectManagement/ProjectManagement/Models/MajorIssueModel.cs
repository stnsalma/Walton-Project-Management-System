using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class MajorIssueModel
    {
        public long MajorIssueId { get; set; }
        public string ServicePoint { get; set; }
        public long IncidentId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public System.DateTime ReceivedDate { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string ModelName { get; set; }
        public string ProblemDescription { get; set; }
        public string ProblemNames { get; set; }
    }
}