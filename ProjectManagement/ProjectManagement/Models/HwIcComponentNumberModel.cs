using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwIcComponentNumberModel
    {
        public long IcComponentNumberId { get; set; }
        public long ItemComponentId { get; set; }
        public string IcComponentNumber { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}