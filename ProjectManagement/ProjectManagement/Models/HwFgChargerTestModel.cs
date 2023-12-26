using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgChargerTestModel
    {
        public long HwFgChargerTestId { get; set; }
        public long HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public string ChargerType { get; set; }
        public string InputSpec { get; set; }
        public string OutputSpec { get; set; }
        public string TestCondition { get; set; }
        public string CcModeTemperature { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> OverallTestResultStatus { get; set; }
        public string Recommendation { get; set; }
        public Nullable<long> PreparedBy { get; set; }
        public Nullable<long> CheckedBy { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}