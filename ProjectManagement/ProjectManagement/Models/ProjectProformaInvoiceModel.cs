using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectProformaInvoiceModel
    {
        public long ProjectProformaInvoiceId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string PiNo { get; set; }
        [Required]
        public DateTime PiDate { get; set; }

        public string FilePath { get; set; }
        public HttpPostedFileBase File { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string FileExtension { get; set; }

    }
}