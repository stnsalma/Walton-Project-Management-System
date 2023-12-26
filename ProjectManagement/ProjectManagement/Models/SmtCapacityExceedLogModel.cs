using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SmtCapacityExceedLogModel
    {
        public long Id { get; set; }
        public string SmtCapacityCrossedForModel { get; set; }
        public int? OrderNo { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string OrderQuantity { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? RunningSmtQuantity { get; set; }
        public long? ProjectMasterId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PoDate { get; set; }
    }
}