using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestUSBCableInfoModel
    {
        public long? HwTestUSBCableInfoId { get; set; }
        public Nullable<long> HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public string Length { get; set; }
        public string Type { get; set; }
        public string NumberOfWire { get; set; }
        public Nullable<bool> AlFoilPaper { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }
        public HttpPostedFileBase HwQcDocUpload { get; set; }
        public string QcDocUploadPath { get; set; }
        public string ImageExtension { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}