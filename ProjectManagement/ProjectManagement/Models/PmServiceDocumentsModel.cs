using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmServiceDocumentsModel
    {
        public long PmServiceDocumentId { get; set; }
        public long ProjectAssignId { get; set; }
        public long ProjectMasterId { get; set; }


        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }


    }
}