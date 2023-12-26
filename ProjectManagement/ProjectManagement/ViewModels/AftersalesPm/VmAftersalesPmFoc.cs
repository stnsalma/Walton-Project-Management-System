using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.AftersalesPm
{
    public class VmAftersalesPmFoc
    {
        public VmAftersalesPmFoc()
        {
            ProjectMasterModels = new List<ProjectMasterModel>();
            ProjectMasterModel = new ProjectMasterModel();
            CreateFocForAftersalesPmModel=new CreateFocForAftersalesPmModel();
            CreateFocForAftersalesPmModels=new List<CreateFocForAftersalesPmModel>();
            CmnUserModel=new CmnUserModel();
            CmnUserModels=new List<CmnUserModel>();
            SpareNameModel=new SpareNameModel();
            SpareNameModels=new List<SpareNameModel>();
        }

        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public CreateFocForAftersalesPmModel CreateFocForAftersalesPmModel { get; set; }
        public List<CreateFocForAftersalesPmModel> CreateFocForAftersalesPmModels { get;set;}
        public CmnUserModel CmnUserModel { get; set; }
        public List<CmnUserModel> CmnUserModels { get; set; }
        public SpareNameModel SpareNameModel { get; set; }
        public List<SpareNameModel> SpareNameModels { get; set; } 
        //
        public long? Id { get; set; }
        public long? SpareId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string SpareName { get; set; }//SparePartsName
        public string SparePartsName { get; set; }
        public int? OrderNumber { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoCategory { get; set; }
        public string EmployeeCode { get; set; }
        public long? AsmUserId { get; set; }
        public string SupplierName { get; set; }
        public string Supplier { get; set; }
        public string Remarks { get; set; }
        [Required(ErrorMessage = "FOC Confirmed Date is Required")]
        public DateTime? FocConfirmedDate { get; set; }
        [Required(ErrorMessage = "Inventory Entry Date is Required")]
        public DateTime? InventoryEntryDate { get; set; }
        [Required(ErrorMessage = "Unit Price (BDT) is Required")]
        public decimal? UnitPrice { get; set; }
        [Required(ErrorMessage = "Quantity is Required")]
        public long? Quantity { get; set; }
       [Required(ErrorMessage = "Shipment Quantity is Required")] 
        public long? ShipmentQuantity { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }

        public string MonthNames { get; set; }
        public string MonthNos { get; set; }
        public string Years { get; set; }
    }
}