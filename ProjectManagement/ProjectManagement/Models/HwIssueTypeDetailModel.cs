using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwIssueTypeDetailModel
    {
        public long HwIssueTypeDetailId { get; set; }
        public long HwIssueTypeId { get; set; }
        public string IssueTypeDetailName { get; set; }
    }
}