using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SampleTrackerModel
    {
        public long SampleTrackerId { get; set; }
        public int? NumberOfSample { get; set; }
        public string SampleSentToDept { get; set; }
        public string Role { get; set; }
        public bool? RoleisHead { get; set; }
        public long? SampleSentToPersonId { get; set; }
        public string SampleSentToPersonName { get; set; }
        public string SampleCategory { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedByName { get; set; }
        public string AddedByDept { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string IMEI { get; set; }
        public long? ProjectMasterId { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public long? ReceivedBy { get; set; }
        public string ReceivedByName { get; set; }
        public string ReturnStatus { get; set; }
        public string Purpose { get; set; }
        public long? ReturnedBy { get; set; }
        public string ReturnedByName { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string SupplierName { get; set; }
        public string Others { get; set; }
        public string AdditionalInfo { get; set; }
        public int? ReturnQuantity { get; set; }
        public int? SampleIssueQuantity { get; set; }
        public long? SampleIssuedBy { get; set; }
        public string SampleIssuedByName { get; set; }
        public string SampleIssuedByDept { get; set; }
        public DateTime? SampleIssueDate { get; set; }
        public string SampleIssuePurpose { get; set; }
        public string SampleIssueTag { get; set; }
        public int? InventoryReturnQuantity { get; set; }
        public DateTime? InventoryReturnDate { get; set; }
        public long? InventoryReturnedBy { get; set; }
        public string InventoryReturnedByName { get; set; }
        public string InventoryReturnRemarks { get; set; }
        public DateTime? InventoryReceiveDate { get; set; }
        public long? InventoryReceivedBy { get; set; }
        public string InventoryReceivedByName { get; set; }
        public string InventoryReceiveRemarks { get; set; }
        public long? SampleIssueReqTo { get; set; }
    }
}