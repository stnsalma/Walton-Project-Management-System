using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectVariantModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string ProjectVariantName { get; set; }
        public string TotalOrderQuantity { get; set; }
        public string ProjectVariantQuantity { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsLocked { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int? OrderNumber { get; set; }
        public string VariantByRamRom { get; set; }
    }
}