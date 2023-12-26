using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class MkOtherBrandModelModel
    {
        public long Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}