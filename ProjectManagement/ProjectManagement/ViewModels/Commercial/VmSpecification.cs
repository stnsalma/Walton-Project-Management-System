using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmSpecification
    {
        public VmSpecification()
        {
            PhPcbaInfo = new PhPcbaInfoModel();
            ProjectMaster = new ProjectMasterModel();
            PhAccessory = new PhAccessoryModel();
            PhCamInfo = new PhCamInfoModel();
            PhChipsetInfo = new PhChipsetInfoModel();
            PhHousingInfo=new PhHousingInfoModel();
            PhMemoryInfo=new PhMemoryInfoModel();
            PhNetworkFreqAndBand = new PhNetworkFreqAndBandModel();
            PhSensorAndOther= new PhSensorAndOtherModel();
            PhTpLcdInfo= new PhTpLcdInfoModel();
            PhOperatingSyModel = new PhOperatingSyModel();
            PhBatteryInfoModel=new PhBatteryInfoModel();
            PhColorInfoModel = new PhColorInfoModel();
        }

        public int TabIdentity { get; set; }
        public ProjectMasterModel ProjectMaster { get; set; }
        public PhAccessoryModel PhAccessory { get; set; }
        public PhCamInfoModel PhCamInfo { get; set; }
        public PhChipsetInfoModel PhChipsetInfo { get; set; }
        public PhHousingInfoModel PhHousingInfo { get; set; }
        public PhMemoryInfoModel PhMemoryInfo { get; set; }
        public PhNetworkFreqAndBandModel PhNetworkFreqAndBand { get; set; }
        public PhPcbaInfoModel PhPcbaInfo { get; set; }
        public PhSensorAndOtherModel PhSensorAndOther { get; set; }
        public PhTpLcdInfoModel PhTpLcdInfo { get; set; }
        public PhOperatingSyModel PhOperatingSyModel { get; set; }
        public PhBatteryInfoModel PhBatteryInfoModel { get; set; }
        public PhColorInfoModel PhColorInfoModel { get; set; }
    }

    
}