using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestOverallResultModel
    {
        public long? HwTestOverallResultId { get; set; }
        public Nullable<long> HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public string AllEnvironmentAndReliabilityTest { get; set; }
        public string NonClassA_Material { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}