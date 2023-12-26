using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManagement.Models
{
    public class OpinionModel
    {
        public long OpinionId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string OpinionText { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //Custom Properties
        public string WebServerUrl { get; set; }
        public string UserFullName { get; set; }
        public string ProjectName { get; set; }

    }
}