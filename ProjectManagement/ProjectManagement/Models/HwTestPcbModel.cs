using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class HwTestPcbModel
    {
        public long HwTestPcbId { get; set; }
        public long? HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        [Required]                                                                                                                                                                                                                                                                                                                                                              
        public string Thickness { get; set; }
        [Required]
        public string Materials { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }
        public long? Added { get; set; }
        public string QcDocUploadPath { get; set; }
        public string ImageExtension { get; set; }
        public HttpPostedFileBase HwQcDocUpload { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}