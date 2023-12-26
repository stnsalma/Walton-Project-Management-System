using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentAttachmentModel
    {
        public long GeneralIncidentAttachmentId { get; set; }
        public long? GeneralIncidentId { get; set; }
        public string AttachmentUrl { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}