using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LC_IDH_MastersModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> VariantId { get; set; }
        public string VariantName { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}