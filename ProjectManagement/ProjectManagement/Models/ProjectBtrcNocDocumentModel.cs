using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectBtrcNocDocumentModel
    {
        public long ProjectBtrcNocDocumentId { get; set; }
        public long ProjectBrtcNocId { get; set; }

        public HttpPostedFileBase btrcFiles { get; set; }


        public string FilePath { get; set; }
        public long ProjectMasterId { get; set; }
        public long Added { get; set; }
        public DateTime AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string WebServerPath { get; set; }
    
    
    }
}