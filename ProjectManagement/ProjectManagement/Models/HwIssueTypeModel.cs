using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwIssueTypeModel
    {
        public long HwIssueTypeId { get; set; }
        public long HwIssueMasterId { get; set; }
        public string IssueTypeName { get; set; }
    }
}