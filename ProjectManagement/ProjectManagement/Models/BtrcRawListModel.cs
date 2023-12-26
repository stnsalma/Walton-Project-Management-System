using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BtrcRawListModel
    {
        public long BtrcRawId { get; set; }
        public string NocNo { get; set; }
        public DateTime? NocApplyDate { get; set; }
        public DateTime? AppxNocReceiveDate { get; set; }
        public DateTime? NocReceiveDate { get; set; }
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocValidityDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ApplicationId { get; set; }
        public long? ProjectBtrcNocId { get; set; }
        public long? ProjectPurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string OrderNumberOrdinal { get; set; }
    }
}