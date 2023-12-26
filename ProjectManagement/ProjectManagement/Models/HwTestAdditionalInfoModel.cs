using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestAdditionalInfoModel
    {
        public long HwTestAdditionalInfoId { get; set; }
        public long? HwEngineerAssignId { get; set; }
        public long? HwTestMasterId { get; set; }
        public long? HwTestInchargeAssignId { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string AddedByName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}