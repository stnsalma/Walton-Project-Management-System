using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProcessCostMonthWiseModel
    {
        public long Id { get; set; }
        public string VariantName { get; set; }
        public string Month { get; set; }
        public string ProcessCost { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Year { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
    }
}