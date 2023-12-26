using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class HwIssueMasterModel
    {
        public long HwIssueMasterId { get; set; }
        public string IssueName { get; set; }
    }
}