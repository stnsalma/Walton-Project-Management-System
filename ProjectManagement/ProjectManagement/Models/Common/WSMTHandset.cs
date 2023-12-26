using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class WSMTHandset
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string BOMPattern { get; set; }
        public string RBSYModel { get; set; }
        public string OracleModel { get; set; }
        public string OrderNo { get; set; }
        public string Production_Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}