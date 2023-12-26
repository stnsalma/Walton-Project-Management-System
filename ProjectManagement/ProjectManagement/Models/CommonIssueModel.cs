using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CommonIssueModel
    {
        public long CommonIssueId { get; set; }
        public long ProjectMasterId { get; set; }
        public string IssueTitle { get; set; }
        public string Component { get; set; }
        public string Description { get; set; }
        public long CreatorUserId { get; set; }
        public string CreatorUserRole { get; set; }
        public long? SolverUserId { get; set; }
        public string SolverUserRole { get; set; }
        public string CurrentlyWorkingRole { get; set; }
        public string ReferenceFlow { get; set; }
        public int? NoOfTimesRefered { get; set; }
        public string ReferenceRemarks { get; set; }
        public bool? IsIgnored { get; set; }
        public long? IgnoredBy { get; set; }
        public string IgnoreComment { get; set; }
        public string SolutionComment { get; set; }
        public bool IsSolved { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //Custom Prop
        public string CreatorName { get; set; }
        public string ProjectName { get; set; }
        public string RoleFullName { get; set; }
        public string FormatedReferenceRemark { get; set; }
        public string FormatedReferenceFlow { get; set; }
        public string StickyStatus { get; set; }
        public int? OrderNuber { get; set; }
    }
}