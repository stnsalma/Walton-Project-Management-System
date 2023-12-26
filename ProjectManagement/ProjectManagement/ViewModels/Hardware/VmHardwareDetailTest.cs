using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Hardware
{
    public class VmHardwareDetailTest
    {
        public VmHardwareDetailTest()
        {
            ProjectMasterModel = new ProjectMasterModel();
            HwItemizationModel=new HwItemizationModel();
            HwItemizationModels=new List<HwItemizationModel>();
            HwQcAssignCustomMasterModel = new HwQcAssignCustomMasterModel();
            HwTestPcbModel = new HwTestPcbModel();
            HwTestPcbAModel=new HwTestPcbAModel(); 
            HwTestCameraInfoModel=new HwTestCameraInfoModel();
            HwTestTpLcdInfoModel=new HwTestTpLcdInfoModel();
            HwTestSoundInfoModel=new HwTestSoundInfoModel();
            HwTestFPCandSIMSlotInfoModel=new HwTestFPCandSIMSlotInfoModel();
            HwTestBatteryInfoModel=new HwTestBatteryInfoModel();
            HwTestChargerInfoModel=new HwTestChargerInfoModel();
            HwTestUSBCableInfoModel=new HwTestUSBCableInfoModel();
            HwTestEarphoneInterfaceInfoModel=new HwTestEarphoneInterfaceInfoModel();
            HwTestChargingInfoModel=new HwTestChargingInfoModel();
            HwTestHousingInfoModel=new HwTestHousingInfoModel();
            HwTestCrossMatchInfoModel=new HwTestCrossMatchInfoModel();
            HwTestOverallResultModel=new HwTestOverallResultModel();
            HwTestCustomModel=new HwTestCustomModel();
            HwProjectMasterCustomModel=new HwProjectMasterCustomModel();
            HwFgBatteryTestMasterModel = new HwFgBatteryTestMasterModel();
            HwFgBatteryTestConditionModel=new HwFgBatteryTestConditionModel();
            HwFgBatteryTestResultModel=new HwFgBatteryTestResultModel();
            HwFgChargerTestModel=new HwFgChargerTestModel();
            HwFgChargerDetailModel=new HwFgChargerDetailModel();
            HwFgUsbCableTestModel=new HwFgUsbCableTestModel();
            HwFgUsbTestDetailModel=new HwFgUsbTestDetailModel();
            HwChipsetModel=new HwChipsetModel();
            HwFlashIcModel=new HwFlashIcModel();
            HwFrontCameraIcModel=new HwFrontCameraIcModel();
            HwBackCameraIcModel=new HwBackCameraIcModel();
            HwRfModel=new HwRfModel();
            HwPmu1IcModel=new HwPmu1IcModel();
            HwBatteryTestCustomModel=new HwBatteryTestCustomModel();
            BatteryTestResultSummaryModel=new BatteryTestResultSummaryModel();
            HwItemComponentModel=new HwItemComponentModel();
            HwItemComponentModels=new List<HwItemComponentModel>();
            HwIcComponentNumberModel=new HwIcComponentNumberModel();
            HwIcComponentNumberModels=new List<HwIcComponentNumberModel>();
            GetHwItemizationModels=new List<GetHwItemizationModel>();
            HwFieldTestMasterModel=new HwFieldTestMasterModel();
            HwFieldTestModel=new HwFieldTestModel();
            HwFieldTestModels=new List<HwFieldTestModel>();
        }

        public ProjectMasterModel ProjectMasterModel { get; set; }
        public HwItemizationModel HwItemizationModel { get; set; }
        public List<HwItemizationModel> HwItemizationModels { get; set; }
        public HwQcAssignCustomMasterModel HwQcAssignCustomMasterModel { get; set; }
        public HwTestPcbModel HwTestPcbModel { get; set; }
        public HwTestPcbAModel HwTestPcbAModel { get; set; }
        public HwTestCameraInfoModel HwTestCameraInfoModel { get; set; }
        public HwTestTpLcdInfoModel HwTestTpLcdInfoModel { get; set; }
        public HwTestSoundInfoModel HwTestSoundInfoModel { get; set; }
        public HwTestFPCandSIMSlotInfoModel HwTestFPCandSIMSlotInfoModel { get; set; }
        public HwTestBatteryInfoModel HwTestBatteryInfoModel { get; set; }
        public HwTestChargerInfoModel HwTestChargerInfoModel { get; set; }
        public HwTestUSBCableInfoModel HwTestUSBCableInfoModel { get; set; }
        public HwTestEarphoneInterfaceInfoModel HwTestEarphoneInterfaceInfoModel { get; set; }
        public HwTestChargingInfoModel HwTestChargingInfoModel { get; set; }
        public HwTestHousingInfoModel HwTestHousingInfoModel { get; set; }
        public HwTestCrossMatchInfoModel HwTestCrossMatchInfoModel { get; set; }
        public HwTestOverallResultModel HwTestOverallResultModel { get; set; }
        public HwTestCustomModel HwTestCustomModel { get; set; }
        public HwProjectMasterCustomModel HwProjectMasterCustomModel { get; set; }
        public HwFgBatteryTestMasterModel HwFgBatteryTestMasterModel { get; set; }
        public HwFgBatteryTestConditionModel HwFgBatteryTestConditionModel { get; set; }
        public HwFgBatteryTestResultModel HwFgBatteryTestResultModel { get; set; }
        public HwFgChargerTestModel HwFgChargerTestModel { get; set; }
        public HwFgChargerDetailModel HwFgChargerDetailModel { get; set; }
        public HwFgUsbCableTestModel HwFgUsbCableTestModel { get; set; }
        public HwFgUsbTestDetailModel HwFgUsbTestDetailModel { get; set; }
        public HwChipsetModel HwChipsetModel { get; set; }
        public HwFlashIcModel HwFlashIcModel { get; set; }
        public HwFrontCameraIcModel HwFrontCameraIcModel { get; set; }
        public HwBackCameraIcModel HwBackCameraIcModel { get; set; }
        public HwRfModel HwRfModel { get; set; }
        public HwPmu1IcModel HwPmu1IcModel { get; set; }
        public HwBatteryTestCustomModel HwBatteryTestCustomModel { get; set; }
        public BatteryTestResultSummaryModel BatteryTestResultSummaryModel { get; set; }
        public HwItemComponentModel HwItemComponentModel { get; set; }
        public List<HwItemComponentModel> HwItemComponentModels { get; set; }
        public HwIcComponentNumberModel HwIcComponentNumberModel { get; set; }
        public List<HwIcComponentNumberModel> HwIcComponentNumberModels { get; set; }
        public List<GetHwItemizationModel> GetHwItemizationModels { get; set; }
        public HwFieldTestMasterModel HwFieldTestMasterModel { get; set; }
        public HwFieldTestModel HwFieldTestModel { get; set; }
        public List<HwFieldTestModel> HwFieldTestModels { get; set; } 
    }
}