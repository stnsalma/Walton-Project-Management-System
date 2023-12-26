using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AftersalesPm_SupplierFeedBackModel
    {
        public AftersalesPm_SupplierFeedBackModel()
        {
            FilesDetails=new List<FilesDetail>();
            FilesDetails1=new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public List<FilesDetail> FilesDetails1 { get; set; }
        public long Id { get; set; }
        public Nullable<long> IssueVerificationId { get; set; }
        public string Details { get; set; }
        public string Attachment { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}