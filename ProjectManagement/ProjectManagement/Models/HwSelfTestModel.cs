using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwSelfTestModel
    {
        public long HwSelfTestId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string HwTestName { get; set; }
        public string FileUploadPath { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}