using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgUsbCableTestModel
    {
        public long HwFgUsbCableTestId { get; set; }
        public long HwQcAssignId { get; set; }
        public long? HwQcInchargeAssignId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<bool> FinalResult { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}