using System.Collections.Generic;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Spare
{
    public class SpareViewModel
    {
        public SpareViewModel()
        {
            SpareNameModel=new SpareNameModel();
            SpareNameModels=new List<SpareNameModel>();
            SpareOrderModel=new SpareOrderModel();
            SpareOrderModels=new List<SpareOrderModel>();
            ProjectPurchaseOrderFormModel=new ProjectPurchaseOrderFormModel();
            ProjectMasterModel=new ProjectMasterModel();
        }

        public SpareNameModel SpareNameModel { get; set; }
        public List<SpareNameModel> SpareNameModels { get; set; }
        public SpareOrderModel SpareOrderModel { get; set; }
        public List<SpareOrderModel> SpareOrderModels { get; set; }
        public ProjectPurchaseOrderFormModel ProjectPurchaseOrderFormModel { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
    }
}