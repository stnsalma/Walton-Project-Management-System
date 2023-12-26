using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LcOpeningOtherProductViewModel
    {
        public LcOpeningOtherProductViewModel()
        {
            LcOpeningPermissionOtherProductModel=new LcOpeningPermissionOtherProductModel();
            LcOpeningPermissionOtherFileModels=new List<LcOpeningPermissionOtherFileModel>();
            LcOpeningPermissionOtherFileModel=new LcOpeningPermissionOtherFileModel();
            LcOpeningOtherProductPriceModels=new List<LcOpeningOtherProductPriceModel>();
        }

        public LcOpeningPermissionOtherProductModel LcOpeningPermissionOtherProductModel { get; set; }
        public List<LcOpeningPermissionOtherFileModel> LcOpeningPermissionOtherFileModels { get; set; }
        public LcOpeningPermissionOtherFileModel LcOpeningPermissionOtherFileModel { get; set; }
        public List<LcOpeningOtherProductPriceModel> LcOpeningOtherProductPriceModels { get; set; }
    }
}