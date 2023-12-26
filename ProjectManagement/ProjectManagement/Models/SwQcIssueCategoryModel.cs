using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcIssueCategoryModel
    {
        public long SwQcIssueCategorytId { get; set; }
        public string QcCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSmart { get; set; }
        public bool? IsFeature { get; set; }
        public bool? IsWalpad { get; set; }
        public bool? IsTab { get; set; }
        public bool? IsPowerbank { get; set; }
        public int? OrdersOfIssues { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}