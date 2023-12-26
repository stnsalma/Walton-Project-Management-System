using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LcOpeningPermissionOtherFileModel
    {
        public long Id { get; set; }
        public long? LcOtherPermissionId { get; set; }
        public string FilePath { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
    }
}