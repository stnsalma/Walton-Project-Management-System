using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class CommonStatusObject
    {
        public string UserType { get; set; }
        public string Detail { get; set; }
        public int IsMarge { get; set; }
        public DateTime? ActionDate { get; set; }
        public string MargeTo { get; set; }
    }
}