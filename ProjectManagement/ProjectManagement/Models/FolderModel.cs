using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class FolderModel
    {
        public long FolderId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string FolderName { get; set; }
        public long? Parent { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Size { get; set; }
    }
}