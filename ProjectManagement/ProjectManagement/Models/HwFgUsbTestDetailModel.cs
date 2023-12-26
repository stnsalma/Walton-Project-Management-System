using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgUsbTestDetailModel
    {
        public long HwFgUsbTestDetailId { get; set; }
        public long HwFgUsbCableTestId { get; set; }
        public string TestTopic { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string StandardValue { get; set; }
        public Nullable<bool> Result { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}