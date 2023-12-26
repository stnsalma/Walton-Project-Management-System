using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    
    public class VmWarehouseDetails
    {
        public VmWarehouseDetails()
        {
            ProjectMasterModels=new List<ProjectMasterModel>();
            PurchaseOrderFormModel=new ProjectPurchaseOrderFormModel();
            PurchaseOrderFormModels=new List<ProjectPurchaseOrderFormModel>();
            ProjectOrderShipmentModel=new ProjectOrderShipmentModel();
            ProjectOrderShipmentModels=new List<ProjectOrderShipmentModel>();
        }
        public long Id { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNumber { get; set; }
 [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ChainaInspectionDate { get; set; }
          [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> ShipmentDate { get; set; }

        public string PurchaseOrderNumber { get; set; }
        public Nullable<long> Quantity { get; set; }
        public Nullable<long> WarehouseQuantity { get; set; }
        public Nullable<System.DateTime> WarehouseDate { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectPurchaseOrderFormModel PurchaseOrderFormModel{ get; set; }
        public List<ProjectPurchaseOrderFormModel> PurchaseOrderFormModels { get; set; }
        public ProjectOrderShipmentModel ProjectOrderShipmentModel { get; set; }
        public List<ProjectOrderShipmentModel> ProjectOrderShipmentModels { get; set; }
    }
}