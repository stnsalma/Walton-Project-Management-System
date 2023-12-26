using System;

namespace ProjectManagement.Models
{
    public class ProjectLcModel
    {
        public long ProjectLcId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectOrderId { get; set; }
        public string LcNo { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? BankOpeningDate { get; set; }
        public DateTime? SupplierDraftDate { get; set; }
        public DateTime? LcPassDate { get; set; }
        public DateTime? SampleSendDate { get; set; }
        public DateTime? BtrcNocDate { get; set; }
        public DateTime? NocReceiveDate { get; set; }
        public bool? IsComplete { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal? LcValue { get; set; }
        public string Currency { get; set; }
        public decimal? BdtLcAmount { get; set; }

        //Cutom Properties
        public string AddedByName { get; set; }
        public string ProjectName { get; set; }
        public string PoNo { get; set; }
        public int? OrderNo { get; set; }
        public DateTime PoDate { get; set; }
        public string StrLcOpeningDate { get; set; }
    }
}