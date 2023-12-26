using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwFieldTestAssignModel
    {
        public long SwFieldTestAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long SwFieldTestId { get; set; }
        public Nullable<long> SwQcUserId { get; set; }
        public string FieldTestAssignComment { get; set; }
        public Nullable<System.DateTime> FieldTestAssignDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}