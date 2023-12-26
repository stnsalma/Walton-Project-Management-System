using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmSwCustomizationModel
    {

        public long PmSwCustomizationId { get; set; }
        public long ProjectAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string CustomizationItemName { get; set; }

        public string PmSwCustomizationUploadPath { get; set; }

        public HttpPostedFileBase PmSwCustomizationUploadFile1 { get; set; }
        public HttpPostedFileBase PmSwCustomizationUploadFile2 { get; set; }
        public string PmSwCustomizationUploadPath2 { get; set; }

        public string SwExtension { get; set; }

        public string SwExtension2{ get; set; }



        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}