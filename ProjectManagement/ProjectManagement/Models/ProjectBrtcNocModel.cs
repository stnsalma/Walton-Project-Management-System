using System;

namespace ProjectManagement.Models
{
    public class ProjectBrtcNocModel
    {
        public long ProjectBrtcNocId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public long ProjectAssignId { get; set; }
        public string FinalSampleImei { get; set; }
        public bool? IsDocUploaded { get; set; }
        public DateTime? BtrcNocApplyDate { get; set; }
        public DateTime? ApproxBtrcNocReceiveDate { get; set; }
        public DateTime? BtrcNocReceiveDate { get; set; }
        public DateTime? SpareComNocSendingDate { get; set; }
        public string NocNo { get; set; }
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocvalidityDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }


        //custom pro
        public string ProjectName { get; set; }
        public string PurchaseOrderNo { get; set; }
    }
}