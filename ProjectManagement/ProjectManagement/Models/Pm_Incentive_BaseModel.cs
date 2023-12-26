using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pm_Incentive_BaseModel
    {
        public long Id { get; set; }
        public string IncentiveName { get; set; }
        public decimal? Amount { get; set; }
        public long? ActiveRole { get; set; }
    }
}