using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PMAcknowledgementModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<long> PlanId { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public string AllType { get; set; }
        public string ProcessType { get; set; }
        public Nullable<System.DateTime> S_Date { get; set; }
        public Nullable<System.DateTime> E_Date { get; set; }
        public Nullable<System.DateTime> AcknowledgeDateText { get; set; }
        public string AcknowledgeStatus { get; set; }
    }
}