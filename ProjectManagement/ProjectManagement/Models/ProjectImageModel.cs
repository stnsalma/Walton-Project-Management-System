using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectImageModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectModel { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string ImagePath { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public HttpPostedFileBase PostedFile { get; set; }
        public string Remarks { get; set; }
    }
}