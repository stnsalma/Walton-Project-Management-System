using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwFieldTestDetailModel
    {
        public long SwFieldTestDetailId { get; set; }
        public long ProjectMasterId { get; set; }
        public Nullable<long> SwFieldTestId { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string Location { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public string Condition_Op_TT_dbm { get; set; }
        public string Condition_Op_TT_Bar { get; set; }
        public string Condition_Op_RB_dbm { get; set; }
        public string Condition_Op_RB_Bar { get; set; }
        public string Condition_Op_BL_dbm { get; set; }
        public string Condition_Op_BL_Bar { get; set; }
        public string Condition_Op_AT_dbm { get; set; }
        public string Condition_Op_AT_Bar { get; set; }
        public string Ref_Op_TT_dbm { get; set; }
        public string Ref_Op_TT_Bar { get; set; }
        public string Ref_Op_RB_dbm { get; set; }
        public string Ref_Op_RB_Bar { get; set; }
        public string Ref_Op_BL_dbm { get; set; }
        public string Ref_Op_BL_Bar { get; set; }
        public string Ref_Op_AT_dbm { get; set; }
        public string Ref_Op_AT_Bar { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}