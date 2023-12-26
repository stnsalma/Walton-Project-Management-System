using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentCategoryModel
    {
        public long GeneralIncidentCategoryId { get; set; }
        public string GeneralIncidentCategoryName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}