using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BtrcRawModel
    {
        public long BtrcRawId { get; set; }
        public long? BabtRawId { get; set; }
        public string NocNo { get; set; }
        public DateTime? NocApplyDate { get; set; }
        public DateTime? AppxNocReceiveDate { get; set; }
        public DateTime? NocReceiveDate { get; set; }
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocValidityDate { get; set; }
        public long? NocImeiQuantity { get; set; }
        public long? RemainingQuantity { get; set; }
        public long? AllocateableFrom { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ApplicationId { get; set; }
    }
}