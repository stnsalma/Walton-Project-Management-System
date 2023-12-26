using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace ProjectManagement.Infrastructures.Helper
{
    public class CommercialEvents
    {
        public long id { get; set; }
        public string text { get; set; }
        public DateTime eventstart { get; set; }
        public DateTime eventend { get; set; }
    }
}