using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BulkUpdateModel
    {
        public string ProjectId { get; set; }
        public string[] ProjectOrders { get; set; }
    }
}