using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class DocManagementFileUploadModel
    {
        public long DocManagerFileId { get; set; }
        public long? FolderId { get; set; }
        public string ProjectName { get; set; }
        public string DocFilePath { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Size { get; set; }
    }
}