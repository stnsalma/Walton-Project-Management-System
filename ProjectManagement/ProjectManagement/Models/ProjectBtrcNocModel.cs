using System;
using System.Collections.Generic;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectBtrcNocModel
    {
        public ProjectBtrcNocModel()
        {
            CustomBtrcProjectModels = new List<CustomBtrcProjectModel>();
        }
        public long ProjectBrtcNocId { get; set; }
        public long ProjectMasterId { get; set; }
        public long BtrcRawId { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public long ProjectAssignId { get; set; }
        public string FinalSampleImei { get; set; }
        public bool? IsDocUploaded { get; set; }
        public bool? IsNocComplete { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string SampleImeiFromBtrcCommercial { get; set; }
        public Nullable<decimal> QuantityFromBtrcCommercial { get; set; }

        //user defined properties
        public string ProjectName { get; set; }
        public string PoNo { get; set; }
        public string ProjectManagerName { get; set; }
        public DateTime? PoDate { get; set; }


        //User defined
        public List<CustomBtrcProjectModel> CustomBtrcProjectModels { get; set; }
        public long? PurchaseOrderQuantity { get; set; }

    }
}