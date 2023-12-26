
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CommonParamModel
    {
        public long UomParamId { get; set; }
        public string Uom { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public string ParamVersion { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}