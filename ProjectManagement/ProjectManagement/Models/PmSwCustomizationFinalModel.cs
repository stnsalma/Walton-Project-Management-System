using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmSwCustomizationFinalModel
    {
        public long PmSwCustomizationFinalId { get; set; }
        public string PmSwCustomizationFinalMenu { get; set; }
        public string PmSwCustomizationFinalPath { get; set; }
        public string PmSwCustomizationFinalSettings { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long AssignUserId { get; set; }
        public Nullable<long> Added { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }

        public string PONumber { get; set; }
    }
}