using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestCameraInfoModel
    {
         public long? HwTestCameraInfoId { get; set; }
        public Nullable<long> HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public Nullable<long> BackCameraIcId { get; set; }
        public string BackCamera_IcNoSize { get; set; }
        public string BackCamera_Vendor { get; set; }
        public string BackCamera_PinType { get; set; }
        public string BackCamera_PinNumber { get; set; }
        public string BackCamera_Remark { get; set; }
        public string BackCamera_Model { get; set; }
        public string BackCamera_MPSW { get; set; }
        public string BackCamera_SensorInfo { get; set; }
        public string BackCamera_BSI { get; set; }
        public Nullable<long> FrontCameraIcId { get; set; }
        public string FrontCamera_IcNoSize { get; set; }
        public string FrontCamera_Vendor { get; set; }
        public string FrontCamera_PinType { get; set; }
        public string FrontCamera_PinNumber { get; set; }
        public string FrontCamera_Remark { get; set; }
        public string FrontCamera_Model { get; set; }
        public string FrontCamera_MPSW { get; set; }
        public string FrontCamera_SensorInfo { get; set; }
        public string FrontCamera_BSI { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }
        public string QcDocUploadPath { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public HttpPostedFileBase HwQcDocUpload { get; set; }
        public string ImageExtension { get; set; }
    }
}