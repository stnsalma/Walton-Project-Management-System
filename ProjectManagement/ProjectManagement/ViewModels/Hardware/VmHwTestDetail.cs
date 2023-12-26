using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Hardware
{
    public class VmHwTestDetail
    {
        public VmHwTestDetail()
        {
            HwTestInchargeAssignModel=new HwTestInchargeAssignModel();
            HwEngineerAssignModel=new HwEngineerAssignModel();
            HwTestFileUploadModels=new List<HwTestFileUploadModel>();
        }

        public HwTestInchargeAssignModel HwTestInchargeAssignModel { get; set; }
        public HwEngineerAssignModel HwEngineerAssignModel { get; set; }
        public List<HwTestFileUploadModel> HwTestFileUploadModels { get; set; }
    }
}