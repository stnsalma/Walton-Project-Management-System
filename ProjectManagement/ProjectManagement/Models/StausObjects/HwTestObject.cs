using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class HwTestObject
    {
        public long ProjectMasterId { get; set; }
        public string HwTestName { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ForwardedDate { get; set; }
    }
}