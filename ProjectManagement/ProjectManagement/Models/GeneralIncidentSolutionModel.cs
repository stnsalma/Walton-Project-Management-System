using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentSolutionModel
    {
        public long SolutionId { get; set; }
        public long? GeneralIncidentId { get; set; }
        public string Solution { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string AddedRole { get; set; }
        public DateTime? DenyDate { get; set; }
        public string DenyRemark { get; set; }
    }
}