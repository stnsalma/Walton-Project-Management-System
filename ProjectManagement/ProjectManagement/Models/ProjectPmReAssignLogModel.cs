using System;

namespace ProjectManagement.Models
{
    public class ProjectPmReAssignLogModel
    {
        public long ProjectPmReAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string PONumber { get; set; }
        public DateTime InactiveDate { get; set; }
        public long InactiveUserId { get; set; }
        public long ActiveProjectManagerUserId { get; set; }
        public string ProjectHeadInActiveRemarks { get; set; }
        public string Status { get; set; }
        public DateTime? ApproxPmInchargeToPmFinishDate { get; set; }
    }
}