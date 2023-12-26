using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcGlassProtectorTestModel
    {
        public SwQcGlassProtectorTestModel()
        {
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public long Id { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public long? SwQcAssignId { get; set; }
        public long? TestPhaseID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string IssueScenario { get; set; }
        public string ExpectedOutcome { get; set; }
        public string WaltonQcStatus { get; set; }
        public string Upload { get; set; }
        public long? IssueSerial { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}