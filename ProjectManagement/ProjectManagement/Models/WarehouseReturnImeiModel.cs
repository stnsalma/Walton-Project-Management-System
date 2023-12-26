using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class WarehouseReturnImeiModel
    {
        public Guid DealerdistributionId { get; set; }
        public string BarCode { get; set; }
        public string BarCode2 { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Model { get; set; }
        public string DistributionDate { get; set; }
    }
}