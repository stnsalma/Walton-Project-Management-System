using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwFieldTestModel
    {
        public long SwFieldTestId { get; set; }
        public long ProjectMasterId { get; set; }
        public Nullable<long> SwQcInchargeAssignId { get; set; }
        public string IssueOf { get; set; }
        public string ComparedWith { get; set; }
        public string FieldTestAssignCommentFromIncharge { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}