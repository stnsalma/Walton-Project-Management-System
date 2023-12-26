using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestMasterModel
    {
        public long HwTestMasterId { get; set; }
        public string HwTestName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedByRole { get; set; }
    }
}