using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestFileUploadModel
    {
        public long HwTestFileUploadId { get; set; }
        public long? HwEngineerAssignId { get; set; }
        public string FileUploadPath { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Remarks { get; set; }
        public long? HwTestInchargeAssignId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? HwSelfTestId { get; set; }
    }
}