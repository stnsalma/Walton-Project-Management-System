using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pro_Type_Model
    {
        public long Id { get; set; }
        public string ProductionType { get; set; }
        public string Product { get; set; }
        public bool? IsActive { get; set; }
    }
}