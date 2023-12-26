using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcBatteryTestModel
    {
        public SwQcBatteryTestModel()
        {
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public int IsRemoved { get; set; }
        public long BatteryId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> SwQcHeadAssignId { get; set; }
        public Nullable<long> SwQcAssignId { get; set; }
        public Nullable<long> TestPhaseID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string CheckPoints { get; set; }
        public int? BatterymAh { get; set; }
        public string HundredToNighty { get; set; }
        public string NightyToEighty { get; set; }
        public string EightyToSeventy { get; set; }
        public string SeventyToSixty { get; set; }
        public string SixtyToFifty { get; set; }
        public string FiftyToFourty { get; set; }
        public string FourtyToThirty { get; set; }
        public string ThirtyToTwenty { get; set; }
        public string TwentyToTen { get; set; }
        public string TenToZero { get; set; }
        public string AverageFullDischarge { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string IssueScenario { get; set; }
        public string ExpectedOutcome { get; set; }
        public string WaltonQcStatus { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public string Upload { get; set; }
        public List<string> UploadedFile { get; set; }
    }
}