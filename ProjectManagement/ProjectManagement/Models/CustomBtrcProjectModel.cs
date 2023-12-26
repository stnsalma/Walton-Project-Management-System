using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CustomBtrcProjectModel
    {
        public long NocTableId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string SampleImei { get; set; }
        public string Quantity { get; set; }
    }
}