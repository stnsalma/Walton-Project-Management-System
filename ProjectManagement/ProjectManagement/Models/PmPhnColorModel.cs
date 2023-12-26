using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmPhnColorModel
    {
        public long PmPhnColorID { get; set; }
        public long ProjectAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string PmPhnNumberOfColor { get; set; }
        public string PmPhnColorName { get; set; }


        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }



    }
}