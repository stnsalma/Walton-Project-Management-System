using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BdIqcModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string AllMaterialPassed { get; set; }
        public string NoOfInspectionTime { get; set; }
        public string ManagementApproved { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ManagementApproveDate { get; set; }
        public string SupportingDoc { get; set; }
        public string Remarks { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? IqcStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? WarehouseReceiveDate { get; set; }
        public string SourcingApproved { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? AddedBy { get; set; }
        public string LotNo { get; set; }
        public string LotQuantity { get; set; }
        public long? VariantId { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}