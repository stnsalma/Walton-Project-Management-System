using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcNewInnovationModel
    {
        public long NewInnovationId { get; set; }
        public string ProjectName { get; set; }
        public string AssignedBy { get; set; }
        public string Description { get; set; }
        public string WorkType { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? EffectiveDate { get; set; }
        public bool? IsApprovedForIncentive { get; set; }
        public long? Added { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedDate { get; set; }
        public int IsRemoved { get; set; }
        public List<string> RefernceModules { get; set; }
        public string RefernceModules1 { get; set; }
        public string Months { get; set; }
        public string Years { get; set; }
        public string UserFullName { get; set; }
        public string IsApprovedForIncentives { get; set; }

        public string Persons { get; set; }
        public Decimal FinalAmount { get; set; }//BaseAmount
        public Decimal BaseAmount { get; set; }//BaseAmount
    }
}