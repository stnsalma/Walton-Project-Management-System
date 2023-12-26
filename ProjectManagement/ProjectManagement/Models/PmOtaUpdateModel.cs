using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmOtaUpdateModel
    {

        public long PmOtaUpdateId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public string RunningOtaSWVersion { get; set; }
        public bool IsForCustomerSatisfaction { get; set; }
        public bool IsMarketIssue { get; set; }
        public bool IsExistingMinorIssue { get; set; }
        public DateTime? OtaUpdateStartDate { get; set; }
        public DateTime? OtaUpdateEndDate { get; set; }
        public DateTime? OtaUpdatePublishDate { get; set; }
        public string OtaUpdateLog { get; set; }
        public bool? IsPmHeadApprove { get; set; }
        public string PmHeadRemarks { get; set; }
        public string CurrentOtaSwVersion { get; set; }
        public DateTime? BinaryFileRequestDate { get; set; }
        public bool? IsSoftwareTested { get; set; }
        public bool? IsHardwareTested { get; set; }
        public bool? IsOtaUploadToServer { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}