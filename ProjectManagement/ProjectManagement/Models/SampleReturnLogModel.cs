using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SampleReturnLogModel
    {
        public long SampleReturnLogId { get; set; }
        public long SampleTrackerId { get; set; }
        public int ReturnQuantity { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}