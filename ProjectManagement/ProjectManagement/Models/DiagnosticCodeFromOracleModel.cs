using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class DiagnosticCodeFromOracleModel
    {
        public long IssueId { get; set; }
        public long? DiagnosticCodeId { get; set; }
        public string DiagonsticCodeName { get; set; }
    }
}