using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Data.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Hardware;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class HardwareRepository : IHardwareRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;
        //  dbeEntities.Con

        public HardwareRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
        }


        //=================SET method starts
        #region SET
        public long SaveHwQcAssign(HwQcAssignModel model, long[] assignIds)
        {
            // var config = new MapperConfiguration(c => c.CreateMap<VmHardwareTest, HwQcAssign>());
            foreach (var assignId in assignIds)
            {
                model.HwQcUserId = assignId;

                Mapper.CreateMap<HwQcAssignModel, HwQcAssign>();
                //var map = config.CreateMapper();
                var hwqcassign = Mapper.Map<HwQcAssign>(model);
                _dbeEntities.HwQcAssigns.Add(hwqcassign);
            }

            _dbeEntities.SaveChanges();
            return model.HwQcAssignId;
        }

        public long SaveHwIssueComment(HwIssueCommentModel model)
        {
            MapperConfiguration configuration = new MapperConfiguration(config => config.CreateMap<HwIssueCommentModel, HwIssueComment>());
            IMapper mapper = configuration.CreateMapper();
            var destination = mapper.Map<HwIssueCommentModel, HwIssueComment>(model);


            Mapper.CreateMap<HwIssueCommentModel, HwIssueComment>();
            var issueComment = destination;//Mapper.Map<HwIssueComment>(model);
            _dbeEntities.HwIssueComments.Add(issueComment);
            _dbeEntities.SaveChanges();
            return model.HwIssueCommentId;
        }

        public void SavePcbMaterial(HwTestPcbModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestPcbModel, HwTestPcb>();
            var hwTestPcbMaterial = Mapper.Map<HwTestPcb>(model);
            _dbeEntities.HwTestPcbs.Add(hwTestPcbMaterial);
            _dbeEntities.SaveChanges();
        }
        public void SavePcbaComponentInfo(HwTestPcbAModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestPcbAModel, HwTestPcbA>();
            var hwTestPcbaComponentInfo = Mapper.Map<HwTestPcbA>(model);
            _dbeEntities.HwTestPcbAs.Add(hwTestPcbaComponentInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestCameraInfo(HwTestCameraInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestCameraInfoModel, HwTestCameraInfo>();
            var hwTestCameraInfo = Mapper.Map<HwTestCameraInfo>(model);
            _dbeEntities.HwTestCameraInfos.Add(hwTestCameraInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestTpLcdInfo(HwTestTpLcdInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestTpLcdInfoModel, HwTestTpLcdInfo>();
            var hwTestTpLcdInfo = Mapper.Map<HwTestTpLcdInfo>(model);
            _dbeEntities.HwTestTpLcdInfos.Add(hwTestTpLcdInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestSoundInfo(HwTestSoundInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestSoundInfoModel, HwTestSoundInfo>();
            var hwTestSoundInfo = Mapper.Map<HwTestSoundInfo>(model);
            _dbeEntities.HwTestSoundInfos.Add(hwTestSoundInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestFPCandSIMSlotInfo(HwTestFPCandSIMSlotInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestFPCandSIMSlotInfoModel, HwTestFPCandSIMSlotInfo>();
            var hwTestFPCandSIMSlotInfo = Mapper.Map<HwTestFPCandSIMSlotInfo>(model);
            _dbeEntities.HwTestFPCandSIMSlotInfos.Add(hwTestFPCandSIMSlotInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestBatteryInfo(HwTestBatteryInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestBatteryInfoModel, HwTestBatteryInfo>();
            var hwTestBatteryInfo = Mapper.Map<HwTestBatteryInfo>(model);
            _dbeEntities.HwTestBatteryInfos.Add(hwTestBatteryInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestChargerInfo(HwTestChargerInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestChargerInfoModel, HwTestChargerInfo>();
            var hwTestChargerInfo = Mapper.Map<HwTestChargerInfo>(model);
            _dbeEntities.HwTestChargerInfos.Add(hwTestChargerInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestUSBCableInfo(HwTestUSBCableInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestUSBCableInfoModel, HwTestUSBCableInfo>();
            var saveHwTestUSBCableInfo = Mapper.Map<HwTestUSBCableInfo>(model);
            _dbeEntities.HwTestUSBCableInfos.Add(saveHwTestUSBCableInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestEarphoneInterfaceInfo(HwTestEarphoneInterfaceInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestEarphoneInterfaceInfoModel, HwTestEarphoneInterfaceInfo>();
            var saveHwTestEarphoneInterfaceInfo = Mapper.Map<HwTestEarphoneInterfaceInfo>(model);
            _dbeEntities.HwTestEarphoneInterfaceInfos.Add(saveHwTestEarphoneInterfaceInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestChargingInfo(HwTestChargingInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestChargingInfoModel, HwTestChargingInfo>();
            var saveChargingInfo = Mapper.Map<HwTestChargingInfo>(model);
            _dbeEntities.HwTestChargingInfos.Add(saveChargingInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFgBatteryTestMaster(HwFgBatteryTestMasterModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE()  WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwFgBatteryTestMasterModel, HwFgBatteryTestMaster>();
            var hwFgBatteryTestMaster = Mapper.Map<HwFgBatteryTestMaster>(model);
            _dbeEntities.HwFgBatteryTestMasters.Add(hwFgBatteryTestMaster);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFgChargerTest(HwFgChargerTestModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwFgChargerTestModel, HwFgChargerTest>();
            var hwFgChargerTest = Mapper.Map<HwFgChargerTest>(model);
            _dbeEntities.HwFgChargerTests.Add(hwFgChargerTest);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFgUsbCableTest(HwFgUsbCableTestModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwFgUsbCableTestModel, HwFgUsbCableTest>();
            var hwFgUsbCable = Mapper.Map<HwFgUsbCableTest>(model);
            _dbeEntities.HwFgUsbCableTests.Add(hwFgUsbCable);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestHousingInfo(HwTestHousingInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestHousingInfoModel, HwTestHousingInfo>();
            var saveHwTestHousingInfo = Mapper.Map<HwTestHousingInfo>(model);
            _dbeEntities.HwTestHousingInfos.Add(saveHwTestHousingInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestCrossMatchInfo(HwTestCrossMatchInfoModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestCrossMatchInfoModel, HwTestCrossMatchInfo>();
            var saveHwTestCrossMatchInfo = Mapper.Map<HwTestCrossMatchInfo>(model);
            _dbeEntities.HwTestCrossMatchInfos.Add(saveHwTestCrossMatchInfo);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwTestOverallResult(HwTestOverallResultModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwTestOverallResultModel, HwTestOverallResult>();
            var saveHwTestOverallResult = Mapper.Map<HwTestOverallResult>(model);
            _dbeEntities.HwTestOverallResults.Add(saveHwTestOverallResult);
            _dbeEntities.SaveChanges();
        }

        public void SaveBatteryTestResultSummary(BatteryTestResultSummaryModel model)
        {
            Mapper.CreateMap<BatteryTestResultSummaryModel, BatteryTestResultSummary>();
            var batteryTestResultSummary = Mapper.Map<BatteryTestResultSummary>(model);
            _dbeEntities.BatteryTestResultSummarys.Add(batteryTestResultSummary);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFgBatteryTestCondition(HwFgBatteryTestConditionModel model)
        {
            Mapper.CreateMap<HwFgBatteryTestConditionModel, HwFgBatteryTestCondition>();
            var hwFgBatteryTestCondition = Mapper.Map<HwFgBatteryTestCondition>(model);
            _dbeEntities.HwFgBatteryTestConditions.Add(hwFgBatteryTestCondition);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFgBatteryTestResult(HwFgBatteryTestResultModel model)
        {
            Mapper.CreateMap<HwFgBatteryTestResultModel, HwFgBatteryTestResult>();
            var hwFgBatteryTestResult = Mapper.Map<HwFgBatteryTestResult>(model);
            _dbeEntities.HwFgBatteryTestResults.Add(hwFgBatteryTestResult);
            _dbeEntities.SaveChanges();
        }



        public void SaveHwFgChargerDetailTest(HwFgChargerDetailModel model)
        {
            Mapper.CreateMap<HwFgChargerDetailModel, HwFgChargerDetail>();
            var hwFgChargerDetail = Mapper.Map<HwFgChargerDetail>(model);
            _dbeEntities.HwFgChargerDetails.Add(hwFgChargerDetail);
            _dbeEntities.SaveChanges();
        }



        public void SaveHwFgUsbCableDetail(HwFgUsbTestDetailModel model)
        {
            Mapper.CreateMap<HwFgUsbTestDetailModel, HwFgUsbTestDetail>();
            var hwFgUsbCableDetail = Mapper.Map<HwFgUsbTestDetail>(model);
            _dbeEntities.HwFgUsbTestDetails.Add(hwFgUsbCableDetail);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwInchargeIssues(HwInchargeIssueModel model)
        {
            Mapper.CreateMap<HwInchargeIssueModel, HwInchargeIssue>();
            var hwInchargeIssue = Mapper.Map<HwInchargeIssue>(model);
            _dbeEntities.HwInchargeIssues.Add(hwInchargeIssue);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwItemizationModel(HwItemizationModel model)
        {
            _dbeEntities.Database.ExecuteSqlCommand("UPDATE HwQcAssigns SET Status='RUNNING',UpdatedDate=GETDATE() WHERE HwQcInchargeAssignId='" + model.HwQcInchargeAssignId + "'");
            Mapper.CreateMap<HwItemizationModel, HwItemization>();
            var hwItemization = Mapper.Map<HwItemization>(model);
            _dbeEntities.HwItemizations.Add(hwItemization);
            _dbeEntities.SaveChanges();
        }

        public void SaveItemComponentModel(HwItemComponentModel model)
        {
            Mapper.CreateMap<HwItemComponentModel, HwItemComponent>();
            var hwItemComponent = Mapper.Map<HwItemComponent>(model);
            _dbeEntities.HwItemComponents.Add(hwItemComponent);
            _dbeEntities.SaveChanges();
        }

        public void SaveIcComponentNumberModel(HwIcComponentNumberModel model)
        {
            Mapper.CreateMap<HwIcComponentNumberModel, HwIcComponentNumber>();
            var hwIcComponentNumbers = Mapper.Map<HwIcComponentNumber>(model);
            _dbeEntities.HwIcComponentNumbers.Add(hwIcComponentNumbers);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFieldTestMasterModel(HwFieldTestMasterModel model)
        {
            Mapper.CreateMap<HwFieldTestMasterModel, HwFieldTestMaster>();
            var hwFieldTestMaster = Mapper.Map<HwFieldTestMaster>(model);
            _dbeEntities.HwFieldTestMasters.Add(hwFieldTestMaster);
            _dbeEntities.SaveChanges();
        }

        public void SaveHwFieldTest(HwFieldTestModel model)
        {
            Mapper.CreateMap<HwFieldTestModel, HwFieldTest>();
            var hwfieldtest = Mapper.Map<HwFieldTest>(model);
            _dbeEntities.HwFieldTests.Add(hwfieldtest);
            _dbeEntities.SaveChanges();
        }

        public HwChipsetModel SaveHwChipsetIc(string chipsetVendor, string icNoSize, string chipsetCore, string chipsetSpeed, string pinType, int pinNumber, string newitemno, string itemcode, string remarks, long userId)
        {
            string query = string.Format(@"INSERT INTO HwChipsets (ChipsetVendor,ChipsetCore,ChipsetSpeed,IcNoSize,PinType,PinNumber,Remarks,Added,AddedDate,NewItemNo,ItemCode) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}',{7},'{8}','{9}','{10}')", chipsetVendor, chipsetCore, chipsetSpeed, icNoSize, pinType, pinNumber, remarks, userId, DateTime.Now, newitemno, itemcode);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var saveHwChipsetIc = _dbeEntities.Database.SqlQuery<HwChipsetModel>("select * from HwChipsets order by ChipsetId desc").FirstOrDefault();
            return saveHwChipsetIc;
        }

        public HwFlashIcModel SaveHwFlashIcModel(string flashIcBall, string flashIcRam, string flashIcRom, string flashIcTechnology, string flashIcVendor, string icNoSize, int pinNumber, string pinType, string remarks, long userId)
        {
            //HwFlashIcModel hwFlashIcModel = new HwFlashIcModel
            //{
            //    FlashIcBall = flashIcBall,
            //    FlashIcRam = flashIcRam,
            //    FlashIcRom = flashIcRom,
            //    FlashIcTechnology = flashIcTechnology,
            //    FlashIdVendor = flashIcVendor,
            //    IcNoSize = icNoSize,
            //    PinNumber = pinNumber,
            //    PinType = pinType,
            //    Remarks = remarks,
            //    Added = userId,
            //    AddedDate = DateTime.Now
            //};
            string query = string.Format(@"INSERT INTO HwFlashIcs (FlashIdVendor,IcNoSize,PinType,PinNumber,FlashIcTechnology,FlashIcRam,FlashIcRom,FlashIcBall,Remarks,Added,AddedDate) VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}',{9},'{10}')", flashIcVendor, icNoSize, pinType, pinNumber, flashIcTechnology, flashIcRam, flashIcRom, flashIcBall, remarks, userId, DateTime.Now);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var saveFlashIc = _dbeEntities.Database.SqlQuery<HwFlashIcModel>("select * from HwFlashIcs order by FlashIcId desc").FirstOrDefault();
            return saveFlashIc;
        }

        public HwRfModel SaveHwRfModel(string icNoSize, string rfVendor, int pinNumber, string pinType, string remarks, long userId)
        {
            string query =
                string.Format(@"INSERT INTO HwRfs (RfVendor,IcNoSize,PinType,PinNumber,Remarks,Added,AddedDate) VALUES ('{0}','{1}','{2}',{3},'{4}',{5},'{6}')"
                , rfVendor, icNoSize, pinType, pinNumber, remarks, userId, DateTime.Now);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var saveHwRfModel = _dbeEntities.Database.SqlQuery<HwRfModel>("SELECT * FROM HwRfs ORDER BY RfId DESC").FirstOrDefault();
            return saveHwRfModel;
        }

        public HwPmu1IcModel SaveHwPmu1IcModel(string icNoSize, string Pmu_1_Vendor, int pinNumber, string pinType, string newitemno, string itemcode, string remarks, long userId)
        {
            string query =
                string.Format(
                    @"INSERT INTO HwPmu1s (Pmu_1_Vendor,IcNoSize,PinType,PinNumber,Remarks,Added,AddedDate,NewItemNo,ItemCode) VALUES ('{0}','{1}','{2}',{3},'{4}',{5},'{6}','{7}','{8}')"
                    , Pmu_1_Vendor, icNoSize, pinType, pinNumber, remarks, userId, DateTime.Now, newitemno, itemcode);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var pmu1ic =
                _dbeEntities.Database.SqlQuery<HwPmu1IcModel>("SELECT * FROM HwPmu1s ORDER BY Pmu_1_Id DESC").FirstOrDefault();
            return pmu1ic;

        }

        public HwFrontCameraIcModel SaveFrontCameraIcModel(string icNoSize, string vendor, int pinNumber, string pinType, string remarks, long userId)
        {
            string query = string.Format(@"INSERT INTO HwFrontCameraIcs (FrontCameraVendor,IcNoSize,PinType,PinNumber,Remarks,Added,AddedDate) VALUES ('{0}','{1}','{2}',{3},'{4}',{5},'{6}')"
                    , vendor, icNoSize, pinType, pinNumber, remarks, userId, DateTime.Now);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var frontCameraIc = _dbeEntities.Database.SqlQuery<HwFrontCameraIcModel>("SELECT * FROM HwFrontCameraIcs ORDER BY FrontCameraIcId DESC").FirstOrDefault();
            return frontCameraIc;
        }

        public HwBackCameraIcModel SaveBackCameraIcModel(string icNoSize, string vendor, int pinNumber, string pinType, string remarks, long userId)
        {
            string query = string.Format(@"INSERT INTO HwBackCameraIcs (BackCameraVendor,IcNoSize,PinType,PinNumber,Remarks,Added,AddedDate) VALUES ('{0}','{1}','{2}',{3},'{4}',{5},'{6}')"
                    , vendor, icNoSize, pinType, pinNumber, remarks, userId, DateTime.Now);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var backCameraIc = _dbeEntities.Database.SqlQuery<HwBackCameraIcModel>("SELECT * FROM HwBackCameraIcs ORDER BY BackCameraIcId DESC").FirstOrDefault();
            return backCameraIc;
        }

        public void NotificationForProjectsReadyToForward(long hwQcInchargeAssignId, long userId)
        {
            var lquery = (from hqia in _dbeEntities.HwQcInchargeAssigns
                          where hqia.HwQcInchargeAssignId == hwQcInchargeAssignId
                          select new HwQcInchargeAssignModel
                          {
                              ProjectMasterId = hqia.ProjectMasterId,
                              IsScreeningTest = hqia.IsScreeningTest,
                              IsRunningTest = hqia.IsRunningTest,
                              IsFinishedGoodTest = hqia.IsFinishedGoodTest,
                              TestPhase = hqia.TestPhase
                          }).FirstOrDefault();
            // string s = Convert.ToString(lquery.TestPhase);
            if (lquery.TestPhase == "QCPASSED")
            {
                var projectMaster = _dbeEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == lquery.ProjectMasterId);
                if (projectMaster != null)
                {
                    //var projectName = projectMaster.ProjectName;
                    if (lquery.IsScreeningTest == true)
                    {
                        var notification = new Notification
                        {
                            ProjectMasterId = projectMaster.ProjectMasterId,
                            IsViewd = false,
                            Role = "HWHEAD",
                            Message = "Screening test completed , Project: " + projectMaster.ProjectName + " is ready to forward",
                            AdditionalMessage = "",
                            ViewerId = 11,
                            AddedBy = userId,
                            Added = DateTime.Now
                        };
                        _dbeEntities.Notifications.Add(notification);
                    }
                    if (lquery.IsRunningTest == true)
                    {
                        var notification = new Notification
                        {
                            ProjectMasterId = projectMaster.ProjectMasterId,
                            IsViewd = false,
                            Role = "HWHEAD",
                            Message = "Running test completed , Project: " + projectMaster.ProjectName + " is ready to forward",
                            AdditionalMessage = "",
                            ViewerId = 11,
                            AddedBy = userId,
                            Added = DateTime.Now
                        };
                        _dbeEntities.Notifications.Add(notification);
                    }
                    if (lquery.IsFinishedGoodTest == true)
                    {
                        var notification = new Notification
                        {
                            ProjectMasterId = projectMaster.ProjectMasterId,
                            IsViewd = false,
                            Role = "HWHEAD",
                            Message = "Finished goods test completed , Project: " + projectMaster.ProjectName + " is ready to forward",
                            AdditionalMessage = "",
                            ViewerId = 11,
                            AddedBy = userId,
                            Added = DateTime.Now
                        };
                        _dbeEntities.Notifications.Add(notification);
                    }
                }
                _dbeEntities.SaveChanges();
            }
        }

        public HwEngineerAssignModel SaveHwEngineerAssign(HwEngineerAssignModel model)
        {
            model.HwTestMasterId =
                _dbeEntities.HwTestInchargeAssigns.Where(x => x.HwTestInchargeAssignId == model.HwTestInchargeAssignId)
                    .Select(x => x.HwTestMasterId).FirstOrDefault();
            model.HwTestName =
                _dbeEntities.HwTestMasters.Where(x => x.HwTestMasterId == model.HwTestMasterId)
                    .Select(x => x.HwTestName)
                    .FirstOrDefault();
            model.ProjectName =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectMasterId)
                    .Select(x => x.ProjectName)
                    .FirstOrDefault();
            Mapper.CreateMap<HwEngineerAssignModel, HwEngineerAssign>();
            var m = Mapper.Map<HwEngineerAssign>(model);
            _dbeEntities.HwEngineerAssigns.Add(m);
            var hwinchargeassign =
                _dbeEntities.HwTestInchargeAssigns.FirstOrDefault(
                    x => x.HwTestInchargeAssignId == model.HwTestInchargeAssignId);
            hwinchargeassign.Status = "ASSIGNED";
            _dbeEntities.HwTestInchargeAssigns.AddOrUpdate(hwinchargeassign);
            _dbeEntities.SaveChanges();
            model.HwEngineerAssignId = m.HwEngineerAssignId;
            return model;
        }

        public void SaveHwTestFileUploadModel(HwTestFileUploadModel model)
        {
            if (model.HwEngineerAssignId != null)
            {
                //==Status update part
                var engineerassign = _dbeEntities.HwEngineerAssigns.FirstOrDefault(x => x.HwEngineerAssignId == model.HwEngineerAssignId);
                if (engineerassign!=null && model.HwTestInchargeAssignId != null)
                {
                   engineerassign.Status = "RUNNING";
                   engineerassign.UpdatedBy = model.AddedBy;
                   engineerassign.UpdatedDate = DateTime.Now;
                   _dbeEntities.HwEngineerAssigns.AddOrUpdate(engineerassign);
                } 
            }
            //==Save part
            Mapper.CreateMap<HwTestFileUploadModel, HwTestFileUpload>();
            var upload = Mapper.Map<HwTestFileUpload>(model);
            _dbeEntities.HwTestFileUploads.Add(upload);
            _dbeEntities.SaveChanges();
        }

        public HwEngineerAssignModel SubmitHwTest(long hwengineerassignId, long hwinchargeassignId, string result, string remarks, long userId)
        {
            //update Hw engineer assing
            var engineer = _dbeEntities.HwEngineerAssigns.FirstOrDefault(x => x.HwEngineerAssignId == hwengineerassignId);
            engineer.Status = hwinchargeassignId==0? "(SELF TEST) SUBMITTED" : "SUBMITTED";
            engineer.SubmittedBy = userId;
            engineer.SubmittedDate = DateTime.Now;
            engineer.Result = result;
            engineer.Remark = remarks;
            _dbeEntities.HwEngineerAssigns.AddOrUpdate(engineer);
            _dbeEntities.SaveChanges();
            //---------------------
            if (hwinchargeassignId != 0)
            {
                var incharge = _dbeEntities.HwTestInchargeAssigns.FirstOrDefault(x => x.HwTestInchargeAssignId == hwinchargeassignId);
                incharge.Status = "FORWARDPENDING";
                _dbeEntities.HwTestInchargeAssigns.AddOrUpdate(incharge);
                _dbeEntities.SaveChanges();
            }
            var model = new HwEngineerAssignModel
            {
                SubmittedByName = _dbeEntities.CmnUsers.Where(x=>x.CmnUserId==userId).Select(x=>x.UserFullName).FirstOrDefault(),
                HwTestName = engineer.HwTestName
            };
            return model;
        }

        public void SaveHwAdditionalInfo(HwTestAdditionalInfoModel model)
        {
            model.HwTestMasterId =
                _dbeEntities.HwEngineerAssigns.Where(x => x.HwEngineerAssignId == model.HwEngineerAssignId)
                    .Select(x => x.HwTestMasterId)
                    .FirstOrDefault();
            //==Save part
            Mapper.CreateMap<HwTestAdditionalInfoModel, HwTestAdditionalInfo>();
            var info = Mapper.Map<HwTestAdditionalInfo>(model);
            _dbeEntities.HwTestAdditionalInfos.Add(info);
            _dbeEntities.SaveChanges();
        }

        #endregion
        //===================================================================================GET methods==============================================================================
        #region GET

        public HwQcInchargeAssignModel GetHwQcInchargeAssignByAssignId(long id)
        {
            string query = string.Format(@"select * from HwQcInchargeAssigns where HwQcInchargeAssignId={0}", id);
            var exe = _dbeEntities.Database.SqlQuery<HwQcInchargeAssignModel>(query).FirstOrDefault();
            return exe;
        }

        public List<HwInchargeIssueModel> GetHwInchargeIssueModels(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwInchargeIssues where HwQcInchargeAssignId={0}",
                hwQcInchargeAssignId);
            var exe = _dbeEntities.Database.SqlQuery<HwInchargeIssueModel>(query).ToList();
            return exe;
        }

        public List<HwQcAssignCustomMasterModel> GetHwInchargeReceivableProjects()
        {
            string query = string.Format(@"select * from HwQcInchargeAssigns hqia inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId where hqia.TestPhase in ('SAMPLESENT')");
            var execute = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return execute;
        }


        public List<ProjectMasterModel> GetAllProjects()
        {
            List<ProjectMasterModel> allProjects = _dbeEntities.ProjectMasters.Select(i => new ProjectMasterModel
            {
                ProjectMasterId = i.ProjectMasterId,
                ProjectName = i.ProjectName,
                SupplierName = i.SupplierName,
                SupplierModelName = i.SupplierModelName,
                ProjectTypeId = i.ProjectTypeId,
                NumberOfSample = i.NumberOfSample,
                ApproxProjectFinishDate = (DateTime)i.ApproxProjectFinishDate,
                SupplierTrustLevel = i.SupplierTrustLevel,
                IsScreenTestComplete = i.IsScreenTestComplete,
                IsApproved = i.IsApproved,
                ApproxProjectOrderDate = i.ApproxProjectOrderDate,
                ApproxShipmentDate = i.ApproxShipmentDate

            }).ToList();
            return allProjects;
        }

        public List<ProjectMasterModel> GetAllProjectDistinctName()
        {
            List<ProjectMasterModel> allProjects = _dbeEntities.ProjectMasters.Select(i => new ProjectMasterModel
            {
                ProjectName = i.ProjectName
            }).DistinctBy(i => i.ProjectName).ToList();
            return allProjects;
        }

        public List<HwChipsetModel> GetAllHwChipsetModel()
        {
            List<HwChipsetModel> allChipsets = _dbeEntities.HwChipsets.Select(i => new HwChipsetModel
            {
                ChipsetId = i.ChipsetId,
                ChipsetVendor = i.ChipsetVendor,
                ChipsetCore = i.ChipsetCore,
                ChipsetSpeed = i.ChipsetSpeed,
                IcNoSize = i.IcNoSize,
                PinType = i.PinType,
                PinNumber = i.PinNumber,
                Remarks = i.Remarks
            }).OrderBy(i => i.IcNoSize).ToList();
            return allChipsets;
        }

        public List<HwFlashIcModel> GetAllFlashIcModel()
        {
            List<HwFlashIcModel> allflashIc = _dbeEntities.HwFlashIcs.Select(i => new HwFlashIcModel
            {
                FlashIcId = i.FlashIcId,
                FlashIdVendor = i.FlashIdVendor,
                FlashIcBall = i.FlashIcBall,
                FlashIcRam = i.FlashIcRam,
                FlashIcRom = i.FlashIcRom,
                FlashIcTechnology = i.FlashIcTechnology,
                IcNoSize = i.IcNoSize,
                PinType = i.PinType,
                PinNumber = i.PinNumber,
                Remarks = i.Remarks
            }).OrderBy(i => i.IcNoSize).ToList();
            return allflashIc;
        }

        public List<HwRfModel> GetAllRfModel()
        {
            List<HwRfModel> allRfs = _dbeEntities.HwRfs.Select(i => new HwRfModel
            {
                RfId = i.RfId,
                RfVendor = i.RfVendor,
                IcNoSize = i.IcNoSize,
                PinType = i.PinType,
                PinNumber = i.PinNumber,
                Added = i.Added,
                AddedDate = i.AddedDate,
                Updated = i.Updated,
                Remarks = i.Remarks
            }).OrderBy(i => i.IcNoSize).ToList();
            return allRfs;
        }

        public List<HwPmu1IcModel> GetAllPmu1IcModel()
        {
            List<HwPmu1IcModel> allPmu1IcModels = _dbeEntities.HwPmu1s.Select(i => new HwPmu1IcModel
            {
                Pmu_1_Id = i.Pmu_1_Id,
                Pmu_1_Vendor = i.Pmu_1_Vendor,
                IcNoSize = i.IcNoSize,
                PinType = i.PinType,
                PinNumber = i.PinNumber,
                Added = i.Added,
                AddedDate = i.AddedDate,
                Updated = i.Updated,
                UpdatedDate = i.UpdatedDate
            }).ToList();
            return allPmu1IcModels;
        }

        public List<HwFrontCameraIcModel> GetAllHwFrontCameraIcModel()
        {
            List<HwFrontCameraIcModel> allFrontCameraIc =
                _dbeEntities.HwFrontCameraIcs.Select(i => new HwFrontCameraIcModel
                {
                    FrontCameraIcId = i.FrontCameraIcId,
                    IcNoSize = i.IcNoSize,
                    FrontCameraVendor = i.FrontCameraVendor,
                    PinNumber = i.PinNumber,
                    PinType = i.PinType,
                    //Remarks = i.Remarks
                }).OrderBy(i => i.FrontCameraIcId).ToList();
            return allFrontCameraIc;
        }

        public List<HwBackCameraIcModel> GetAllHwBackCameraIcModel()
        {
            List<HwBackCameraIcModel> allBackCameraIc = _dbeEntities.HwBackCameraIcs.Select(i => new HwBackCameraIcModel
            {
                BackCameraIcId = i.BackCameraIcId,
                BackCameraVendor = i.BackCameraVendor,
                IcNoSize = i.IcNoSize,
                PinNumber = i.PinNumber,
                PinType = i.PinType,
                Remarks = i.Remarks
            }).ToList();
            return allBackCameraIc;
        }

        public CmnUserModel GetUserInfoByUserId(long userId)
        {
            FileManager manager = new FileManager();
            //string query = string.Format(@"select * from CmnUsers where CmnUserId={0}", userId);
            var cmnUser = (from cu in _dbeEntities.CmnUsers
                           where cu.CmnUserId == userId && cu.IsActive == true
                           select new CmnUserModel
                               {
                                   CmnUserId = cu.CmnUserId,
                                   UserName = cu.UserName,
                                   UserFullName = cu.UserFullName,
                                   EmployeeCode = cu.EmployeeCode,
                                   MobileNumber = cu.MobileNumber,
                                   Email = cu.Email,
                                   RoleName = cu.RoleName,
                                   ProfilePictureUrl = cu.ProfilePictureUrl
                               }).FirstOrDefault();

            if (cmnUser != null)
            {
                cmnUser.WebServerUrl = manager.GetFile(cmnUser.ProfilePictureUrl);
                //var getUserInfoByUserId = _dbeEntities.Database.SqlQuery<CmnUserModel>(query).FirstOrDefault();
                return cmnUser;
            }
            return new CmnUserModel();
        }
        public List<CmnUserModel> GetUsersForHwQcAssign()
        {
            string[] roleNames = new string[] { "HWHEAD", "HW" };
            var test = (from cu in _dbeEntities.CmnUsers
                        where roleNames.Contains(cu.RoleName) && cu.IsActive
                        select new CmnUserModel
                            {
                                CmnUserId = cu.CmnUserId,
                                UserName = cu.UserName,
                                UserFullName = cu.UserFullName,
                                RoleName = cu.RoleName,
                                EmployeeCode = cu.EmployeeCode,
                                Email = cu.Email,
                                MobileNumber = cu.MobileNumber
                            }).ToList();
            //string query = string.Format(@"Select * from CmnUsers where RoleName in('{0}','{1}')", "HWHEAD", "HW");
            //var getUsersForHwQcAssign = _dbeEntities.Database.SqlQuery<CmnUserModel>(query).ToList();
            return test;
        }

        public List<HwIssueMasterModel> GetAllHwIssueMaster()
        {
            var getAllHwIssueMaster = _dbeEntities.Database.SqlQuery<HwIssueMasterModel>(@"Select * from HwIssueMasters").ToList();
            return getAllHwIssueMaster;
        }

        public ProjectMasterModel GetProjectInfoByProjectId(long projectId)
        {
            var projectInfoByProjectMasterId = (from hqia in _dbeEntities.HwQcInchargeAssigns
                                                join pm in _dbeEntities.ProjectMasters on hqia.ProjectMasterId equals pm.ProjectMasterId
                                                where pm.ProjectMasterId == projectId
                                                select new ProjectMasterModel
                                                {
                                                    ProjectMasterId = pm.ProjectMasterId,
                                                    ProjectName = pm.ProjectName,
                                                    SupplierModelName = pm.SupplierModelName,
                                                    SupplierName = pm.SupplierName,
                                                    NumberOfSample = pm.NumberOfSample,
                                                    ProjectType = pm.ProjectType,
                                                    AddedDate = pm.AddedDate,
                                                    Rom = pm.Rom,
                                                    ProcessorName = pm.ProcessorName,
                                                    ProcessorClock = pm.ProcessorClock,
                                                    Ram = pm.Ram
                                                }).FirstOrDefault();
            //            var projectInfoByProjectMasterId = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(@"Select pm.* 
            //                                                                                            from HwQcInchargeAssigns hqia 
            //                                                                                            inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId 
            //                                                                                            where pm.ProjectMasterId='" + projectId + "'").FirstOrDefault();
            return projectInfoByProjectMasterId;
        }

        public int GetVerificationPendingCounts()
        {
            string query = string.Format(@"select distinct count(*)  as VerificationPending from (select distinct pm.ProjectMasterId,pm.ProjectName,pm.SupplierModelName,hqa.VerifierName, STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,hqa.HwQcInchargeAssignId,hqa.QcDocUploadPath from HwQcAssigns hqa 
                                                                     inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                     inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId
                                                                     inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId
                                                                     where hqa.Status='{0}') a", "QCSUBMITTED");
            var getVerificationPendingCount = _dbeEntities.Database.SqlQuery<int>(query).First();
            return getVerificationPendingCount;
        }

        public int GetScreeningForwardCounter()
        {
            string query = string.Format(@"select count(*) as ScreeningForward from HwQcInchargeAssigns where IsScreeningTest={0} and TestPhase='{1}'", 1, "QCPASSED");
            var getScreeningForwardCount = _dbeEntities.Database.SqlQuery<int>(query).First();
            return getScreeningForwardCount;
        }

        public int GetRunningForwardCounter()
        {
            string query = string.Format(@"select count(*) as RunningForward from HwQcInchargeAssigns where IsRunningTest={0} and TestPhase='{1}'", 1, "QCPASSED");
            var getRunningForwardCount = _dbeEntities.Database.SqlQuery<int>(query).First();
            return getRunningForwardCount;
        }

        public int GetFinishedGoodsForwardCounter()
        {
            string query = string.Format(@"select count(*) as FinishedGoodsForward from HwQcInchargeAssigns where IsFinishedGoodTest={0} and TestPhase='{1}'", 1, "QCPASSED");
            var getFgForwardCount = _dbeEntities.Database.SqlQuery<int>(query).First();
            return getFgForwardCount;
        }

        public HwQcTestCounterModel GetHwQcInchargeTestCounts(long hwUserIdWhoLoggedIn) //no more required
        {
            var getHwQcInchargeTestCounts = _dbeEntities.Database.SqlQuery<HwQcTestCounterModel>(@"select 
                                                (select count(*) from HwQcInchargeAssigns where IsScreeningTest=1 and TestPhase in ('NEW','ASSIGNED')) as ScreeningCounter, " +
                                                "(select count(*) from HwQcInchargeAssigns where IsRunningTest=1 and TestPhase in ('NEW','ASSIGNED')) as RunningTestCounter, " +
                                                "(select count(*) from HwQcInchargeAssigns where IsFinishedGoodTest=1 and TestPhase in ('NEW','ASSIGNED')) as FinishedGoodsCounter").FirstOrDefault();
            return getHwQcInchargeTestCounts;
        }

        public ProjectMasterModel GetProjectInfoByHwQcAssignId(long? HwQcAssignId)
        {
            var linqQuery = (from pm in _dbeEntities.ProjectMasters
                             join hqa in _dbeEntities.HwQcAssigns on pm.ProjectMasterId equals hqa.ProjectMasterId
                             where hqa.HwQcAssignId == HwQcAssignId
                             select new ProjectMasterModel
                             {
                                 ProjectMasterId = pm.ProjectMasterId,
                                 ProjectName = pm.ProjectName,
                                 SupplierModelName = pm.SupplierModelName,
                                 NumberOfSample = pm.NumberOfSample,
                                 ProjectType = pm.ProjectType,
                                 AddedDate = pm.AddedDate,
                                 Rom = pm.Rom,
                                 ProcessorName = pm.ProcessorName,
                                 ProcessorClock = pm.ProcessorClock,
                                 Ram = pm.Ram,
                                 Added = pm.Added,
                                 ApproxProjectFinishDate = pm.ApproxProjectFinishDate
                             }).FirstOrDefault();
            string query = string.Format(@"select * from ProjectMasters pm
                                           inner join  HwQcAssigns hqa on pm.ProjectMasterId=hqa.ProjectMasterId
                                           where hqa.HwQcAssignId={0}", HwQcAssignId);
            //var getProjectInfoByHwQcAssignId =
            //    _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).FirstOrDefault();
            return linqQuery;
        }

        public ProjectMasterModel GetProjectInfoByHwQcInchargeAssignId(long? hwQcInchargeAssignId)
        {
            var linqQuery = (from pm in _dbeEntities.ProjectMasters
                             join hqia in _dbeEntities.HwQcInchargeAssigns on pm.ProjectMasterId equals hqia.ProjectMasterId
                             where hqia.HwQcInchargeAssignId == hwQcInchargeAssignId
                             select new ProjectMasterModel
                             {
                                 ProjectMasterId = pm.ProjectMasterId,
                                 ProjectName = pm.ProjectName,
                                 SupplierModelName = pm.SupplierModelName,
                                 NumberOfSample = pm.NumberOfSample,
                                 ProjectType = pm.ProjectType,
                                 AddedDate = pm.AddedDate,
                                 Rom = pm.Rom,
                                 ProcessorName = pm.ProcessorName,
                                 ProcessorClock = pm.ProcessorClock,
                                 Ram = pm.Ram,
                                 Added = pm.Added,
                                 ApproxProjectFinishDate = pm.ApproxProjectFinishDate,
                                 OrderNuber = pm.OrderNuber
                             }).FirstOrDefault();
            string query = string.Format(@"select * from ProjectMasters pm
                                           inner join  HwQcAssigns hqa on pm.ProjectMasterId=hqa.ProjectMasterId
                                           where hqa.HwQcAssignId={0}", hwQcInchargeAssignId);
            //var getProjectInfoByHwQcAssignId =
            //    _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).FirstOrDefault();
            return linqQuery;
        }


        public CmnUserModel GetUserInfoByHwQcInchargeAssignedBy(long hwQcAssignId)
        {
            string query = string.Format(@"select * from CmnUsers cu
                                           inner join HwQcInchargeAssigns hqia on cu.CmnUserId=hqia.Added
                                           inner join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                           where hqa.HwQcAssignId={0}", hwQcAssignId);
            var getUserInfoByHwQcInchargeAssignedBy =
                _dbeEntities.Database.SqlQuery<CmnUserModel>(query).FirstOrDefault();
            return getUserInfoByHwQcInchargeAssignedBy;
        }

        public HwQcInchargeAssignModel GetTestPhaseByHwQcAssignId(long hwQcAssignId)
        {
            var query = (from hqia in _dbeEntities.HwQcInchargeAssigns
                         join hqa in _dbeEntities.HwQcAssigns on hqia.HwQcInchargeAssignId equals hqa.HwQcInchargeAssignId
                         where hqa.HwQcAssignId == hwQcAssignId
                         select new HwQcInchargeAssignModel
                         {
                             ProjectMasterId = hqia.ProjectMasterId,
                             IsScreeningTest = hqia.IsScreeningTest,
                             IsRunningTest = hqia.IsRunningTest,
                             IsFinishedGoodTest = hqia.IsFinishedGoodTest

                         }).FirstOrDefault();
            return query;
        }

        //================Screening test phase start===============

        //Methods for Hw Qc Incharge start
        public HwQcTestCounterModel GetHwQcTestCounts(long hwUserIdWhoLoggedIn) //no more required
        {
            string getHwQcTestCountsQuery = string.Format(@"select
                                                            (select count(*) from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                            where hqa.HwQcUserId={0} and hqia.IsScreeningTest=1 and hqa.Status not in ('QCPASSED','QCSUBMITTED','FORWARDED')) as ScreeningCounter,
                                                            (select count(*) from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                            where hqa.HwQcUserId={0} and hqia.IsRunningTest=1 and hqa.Status not in ('QCPASSED','QCSUBMITTED','FORWARDED')) as RunningTestCounter,
                                                            (select count(*) from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                            where hqa.HwQcUserId={0} and hqia.IsFinishedGoodTest=1 and hqa.Status not in ('QCPASSED','QCSUBMITTED','FORWARDED')) as FinishedGoodsCounter", hwUserIdWhoLoggedIn);
            var getHwQcTestCounts =
                _dbeEntities.Database.SqlQuery<HwQcTestCounterModel>(getHwQcTestCountsQuery).FirstOrDefault();
            return getHwQcTestCounts;
        }
        public List<ProjectMasterModel> GetProjectsAssignedToHwQcInchargeForScreening()
        {
            string query =
                string.Format(
                    @"Select pm.* from HwQcInchargeAssigns hqia  inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId  where  hqia.IsScreeningTest=1 and pm.ProjectStatus='PARTIAL' and hqia.TestPhase in ('NEW','ASSIGNED')");
            var listOfProjectByQcIncharge = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return listOfProjectByQcIncharge;
        }

        public List<ProjectMasterModel> GetProjectsAssignedToHwQcInchargeForRunning()
        {
            var listOfProjectByQcIncharge = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(@"Select pm.* 
                                                                                            from HwQcInchargeAssigns hqia 
                                                                                            inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId 
                                                                                            where hqia.IsRunningTest=1 and hqia.TestPhase in ('NEW','ASSIGNED')").ToList();
            return listOfProjectByQcIncharge;
        }


        public HwQcAssignModel GetHwQcInchargeAssignIdForScreening(long projectId)
        {
            var getHwQcInchargeAssignId = _dbeEntities.Database.SqlQuery<HwQcAssignModel>(@"Select hqia.HwQcInchargeAssignId,hqa.QcDocUploadPath from HwQcInchargeAssigns hqia 
                                                                                            inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId
                                                                                            left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId 
                                                                                            where pm.ProjectMasterId='" + projectId + "' and hqia.IsScreeningTest=1 and hqia.TestPhase not in('FINISHED','QCFAILED')").FirstOrDefault();
            return getHwQcInchargeAssignId;
        }


        public HwQcAssignModel GetHwQcInchargeAssignIdForRunning(long projectId)
        {
            string query = string.Format(@"Select top 1 hqia.HwQcInchargeAssignId from HwQcInchargeAssigns hqia where hqia.ProjectMasterId={0} and hqia.IsRunningTest={1} order by hqia.HwQcInchargeAssignId desc", projectId, 1);
            var getHwQcInchargeAssignId = _dbeEntities.Database.SqlQuery<HwQcAssignModel>(query).FirstOrDefault();
            return getHwQcInchargeAssignId;
        }



        public List<HwGetQcAssignedByInchargeModel> GetQcAssignedByInchargeAssignIdForScreening(long hwQcInchargeAssignId, int testStageScreening)
        {
            var getQcAssignedByInchargeAssignIdAndTestStage =
                _dbeEntities.Database.SqlQuery<HwGetQcAssignedByInchargeModel>(@"select cu.CmnUserId, cu.UserFullName,cu.Email,hqa.Status,hqa.HwQcAssignDate,Convert(date, hqa.DeadLineDate) as DeadLineDate from HwQcAssigns hqa
                                                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId
                                                                inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                where hqa.HwQcInchargeAssignId='" + hwQcInchargeAssignId + "' and hqia.IsScreeningTest='" + testStageScreening + "'").ToList();
            return getQcAssignedByInchargeAssignIdAndTestStage;
        }

        public List<HwGetQcAssignedByInchargeModel> GetQcAssignedByInchargeAssignIdForRunning(long hwQcInchargeAssignId, int testStageRunning)
        {
            string query =
                string.Format(@"select cu.CmnUserId, cu.UserFullName,cu.Email,hqa.Status,hqa.HwQcAssignDate,Convert(date, hqa.DeadLineDate) as DeadLineDate from HwQcAssigns hqa
                                                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId
                                                                inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                where hqa.HwQcInchargeAssignId={0} and hqia.IsRunningTest={1}", hwQcInchargeAssignId, testStageRunning);
            var getQcAssignedByInchargeAssignIdAndTestStage =
                _dbeEntities.Database.SqlQuery<HwGetQcAssignedByInchargeModel>(query).ToList();
            return getQcAssignedByInchargeAssignIdAndTestStage;
        }

        public List<HwQcAssignCustomMasterModel> GetScreeningTestProjectStatusForInchargeDashboard()
        {
            string query = string.Format(@"select distinct hqia.HwQcInchargeAssignId,pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,hqia.ProjectManagerSampleType,hqia.ReceivedSampleQuantity,
                                           hqia.SentSampleQuantity,hqia.ProjectManagerAssignComment,hqia.ReceiveSampleRemark,hqia.SampleSetSentDate,hqia.SampleSetReceiveDate,
                                           STUFF((select ', '+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId
                                           where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                           hqa.Status,hqa.QcSubmissionDate,hqa.VerifierName,hqa.VerificationDate from HwQcInchargeAssigns hqia
                                           left join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                           inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
										   where hqia.IsScreeningTest={0} and hqia.TestPhase not in('{1}','{2}')", 1, "FINISHED", "SAMPLESENT");
            var execute = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return execute;
        }

        public List<HwQcAssignCustomMasterModel> GetRunningTestProjectStatusForInchargeDashboard()
        {
            string query = string.Format(@"select distinct hqia.HwQcInchargeAssignId,pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,hqia.ProjectManagerSampleType,hqia.ReceivedSampleQuantity,
                                           hqia.SentSampleQuantity,hqia.ProjectManagerAssignComment,hqia.ReceiveSampleRemark,hqia.SampleSetSentDate,hqia.SampleSetReceiveDate,
                                           STUFF((select ', '+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId
                                           where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                           hqa.Status,hqa.QcSubmissionDate,hqa.VerifierName,hqa.VerificationDate from HwQcInchargeAssigns hqia
                                           left join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                           inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
										   where hqia.IsRunningTest={0} and hqia.TestPhase not in('{1}','{2}')", 1, "FINISHED", "SAMPLESENT");
            var execute = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return execute;
        }

        public List<HwQcAssignCustomMasterModel> GetFinishedGoodsTestProjectStatusForInchargeDashboard()
        {
            string query = string.Format(@"select distinct hqia.HwQcInchargeAssignId,pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,hqia.ProjectManagerSampleType,hqia.ReceivedSampleQuantity,
                                           hqia.SentSampleQuantity,hqia.ProjectManagerAssignComment,hqia.ReceiveSampleRemark,hqia.SampleSetSentDate,hqia.SampleSetReceiveDate,
                                           STUFF((select ', '+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId
                                           where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                           hqa.Status,hqa.QcSubmissionDate,hqa.VerifierName,hqa.VerificationDate from HwQcInchargeAssigns hqia
                                           left join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                           inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
										   where hqia.IsFinishedGoodTest={0} and hqia.TestPhase not in('{1}','{2}')", 1, "FINISHED", "SAMPLESENT");
            var execute = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return execute;
        }


        //Methods for QC start
        public List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForScreening(long hwQcUserId)
        {
            string query =
                string.Format(
                    @"Select distinct pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,hqa.HwQcAssignId from HwQcInchargeAssigns hqia inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId inner join HwQcAssigns hqa on hqia.HwQcInchargeAssignId= hqa.HwQcInchargeAssignId where hqa.HwQcUserId={0} and hqia.IsScreeningTest={1} and hqa.Status not in ('QCSUBMITTED','QCPASSED','QCFAILED','FORWARDED')", hwQcUserId, 1);
            var listOfProjectByQcIncharge = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return listOfProjectByQcIncharge;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForScreeningForDashBoard(long hwQcUserId)
        {
            string query =
                string.Format(@"Select hqa.HwQcAssignId,hqa.HwQcInchargeAssignId,pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,hqa.HwQcAssignDate,hqa.DeadLineDate,hqa.Status,hqa.QcSubmissionDate from HwQcAssigns hqa
                                inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqa.ProjectMasterId=pm.ProjectMasterId
                                where hqa.HwQcUserId={0} and hqia.IsScreeningTest=1 and hqia.TestPhase not in('FINISHED')", hwQcUserId);
            var getProjectsAssignedToHwQcForScreeningForDashBoard =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getProjectsAssignedToHwQcForScreeningForDashBoard;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForRunningForDashBoard(long hwQcUserId)
        {
            string query =
                string.Format(@"Select hqa.HwQcAssignId,hqa.HwQcInchargeAssignId,pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,hqa.HwQcAssignDate,hqa.DeadLineDate,hqa.Status,hqa.QcSubmissionDate from HwQcAssigns hqa
                                inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqa.ProjectMasterId=pm.ProjectMasterId
                                where hqa.HwQcUserId={0} and hqia.IsRunningTest=1 and hqia.TestPhase not in('FINISHED')", hwQcUserId);
            var getProjectsAssignedToHwQcForRunningForDashBoard =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getProjectsAssignedToHwQcForRunningForDashBoard;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForRunning(long hwQcUserId)
        {
            string getProjectsAssignedToHwQcForRunningQuery = string.Format(@"Select distinct pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,hqa.HwQcAssignId from HwQcInchargeAssigns hqia inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId inner join HwQcAssigns hqa on hqia.HwQcInchargeAssignId= hqa.HwQcInchargeAssignId where hqa.HwQcUserId={0} and hqia.IsRunningTest=1 and hqa.Status not in ('QCSUBMITTED','QCPASSED','QCFAILED','FORWARDED')", hwQcUserId);
            var listOfProjectByQcIncharge = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(getProjectsAssignedToHwQcForRunningQuery).ToList();
            return listOfProjectByQcIncharge;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForFinishedGoods(long hwQcUserId)
        {
            string getProjectsAssignedToHwQcForFinishedGoods = string.Format(@"Select distinct pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,hqa.HwQcAssignId from HwQcInchargeAssigns hqia inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId inner join HwQcAssigns hqa on hqia.HwQcInchargeAssignId= hqa.HwQcInchargeAssignId where hqa.HwQcUserId={0} and hqia.IsFinishedGoodTest=1 and hqa.Status not in ('QCSUBMITTED','QCPASSED','QCFAILED','FORWARDED')", hwQcUserId);
            var listOfProjectByQcIncharge = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(getProjectsAssignedToHwQcForFinishedGoods).ToList();
            return listOfProjectByQcIncharge;
        }

        public long GetHwQcAssignIdForAllTestByProject(long projectId, long hwQcUserId, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"Select hqa.HwQcAssignId
                         from HwQcInchargeAssigns hqia 
                         inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId 
                         inner join HwQcAssigns hqa on hqia.HwQcInchargeAssignId= hqa.HwQcInchargeAssignId            
                         left join CmnUsers cu on hqia.HwQcInchargeUserId=cu.CmnUserId
                         where hqa.HwQcUserId={0} and pm.ProjectMasterId={1}  and hqa.HwQcInchargeAssignId={2}
                         and hqa.Status not in ('QCSUBMITTED','QCPASSED')", hwQcUserId, projectId, hwQcInchargeAssignId);
            var getTestsAssignedToHwQc = _dbeEntities.Database.SqlQuery<long>(query).First();
            return getTestsAssignedToHwQc;
        }

        public List<HwIssueCommentModel> GetIssueCommentsByQcAssignId(long hwQcAssignId)
        {
            var getIssueCommentsByQcAssignId = _dbeEntities.Database.SqlQuery<HwIssueCommentModel>(@"  select hic.HwIssueCommentId,hic.HwQcAssignId,hic.ProjectMasterId,hic.IssueName,hic.IssueTypeName,hic.IssueTypeDetailName,hic.IssueComment,hic.IssueCommetDate,hic.VerifierComment,hic.IssueStatus 
                                                                                                      from HwIssueComments hic inner join HwQcAssigns hqa on hic.HwQcAssignId=hqa.HwQcAssignId where hic.HwQcAssignId='" + hwQcAssignId + "'").ToList();
            //var issueComment = (from s in _dbeEntities.HwIssueComments  where s.HwQcAssignId == hwQcAssignId select s).ToList();
            return getIssueCommentsByQcAssignId;
        }

        public HwQcAssignCustomMasterModel GetHwQcAssignDetailForVerifyByQcAssignId(long hwQcInchargeAssignId)
        {
            string getHwQcAssignDetailQuery = string.Format(@"select distinct pm.ProjectMasterId,pm.ProjectName,pm.SupplierModelName,hqa.VerifierName,STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,hqa.HwQcInchargeAssignId,hqa.QcDocUploadPath from HwQcAssigns hqa 
                                                                     inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                     inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId
                                                                     inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId
                                                                     where hqa.HwQcInchargeAssignId={0}", hwQcInchargeAssignId);

            var getHwQcAssignDetailForVerifyByQcAssignId =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(getHwQcAssignDetailQuery).FirstOrDefault();
            return getHwQcAssignDetailForVerifyByQcAssignId;
        }

        public List<HwQcAssignCustomMasterModel> GetHwQcInchargeProjectsForScreeningForward()
        {
            string getHwQcInchargeProjectsForForwardQuery = string.Format(@"select * from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId where hqia.TestPhase='{0}' and hqia.IsScreeningTest={1}", "QCPASSED", 1);
            var getHwQcInchargeProjectsForForward = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(getHwQcInchargeProjectsForForwardQuery).ToList();
            return getHwQcInchargeProjectsForForward;
        }

        public List<HwQcAssignCustomMasterModel> GetHwQcInchargeProjectsForRunningForward()
        {
            string query = string.Format(@"select * from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId where hqia.TestPhase='{0}' and hqia.IsRunningTest={1}", "QCPASSED", 1);
            var getHwQcInchargeProjectsForRunningForward =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwQcInchargeProjectsForRunningForward;
        }

        public List<HwQcAssignCustomMasterModel> GetHwQcInchargeProjectsForFinishedGoodsForward()
        {
            string query = string.Format(@"select * from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId where hqia.TestPhase='{0}' and hqia.IsFinishedGoodTest={1}", "QCPASSED", 1);
            var getHwQcInchargeProjectsForFinishedGoodsForward =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwQcInchargeProjectsForFinishedGoodsForward;
        }

        public List<HwQcAssignCustomMasterModel> GetQcPassedListByInchargeIdForForward(long hwQcInchargeAssignId)
        {
            string getQcPassedListByInchargeIdForForwardQuery = string.Format(@"select top 1 hqa.HwQcInchargeAssignId,STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = {0} for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,pm.ProjectName,hqa.QcDocUploadPath from HwQcAssigns hqa 
                                                                                inner join ProjectMasters pm on hqa.ProjectMasterId=pm.ProjectMasterId
                                                                                 inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId
                                                                                 where hqa.HwQcInchargeAssignId={0} and hqa.Status='QCPASSED'", hwQcInchargeAssignId);
            var getQcPassedListByInchargeIdForForward =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(getQcPassedListByInchargeIdForForwardQuery).ToList();
            return getQcPassedListByInchargeIdForForward;
        }

        public String GetQcUploadedDocument(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select QcDocUploadPath from HwQcAssigns where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getQcUploadedDocument = _dbeEntities.Database.SqlQuery<String>(query).FirstOrDefault();
            return getQcUploadedDocument;
        }

        public List<HwQcAssignCustomMasterModel> GetHwQcScreeningVerificationPending(long whologgedin)
        {
            string query = string.Format(@"select distinct STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                                                                       hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,hqa.QcSubmissionDate,hqa.Status from HwQcAssigns hqa 
                                                                                       inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                                       inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                                                                       inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId where hqia.IsScreeningTest={0} and hqa.Status='{1}' ", 1, "QCSUBMITTED");
            var getHwQcScreeningVerificationPending =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwQcScreeningVerificationPending;
        }

        public List<HwQcAssignCustomMasterModel> GetHwQcRunningVerificationPending(long whologgedin)
        {
            string query = string.Format(@"select distinct STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                                                                       hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,hqa.QcSubmissionDate,hqa.Status from HwQcAssigns hqa 
                                                                                       inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                                       inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                                                                       inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId where hqia.IsRunningTest={0} and hqa.Status='{1}' ", 1, "QCSUBMITTED");
            var getHwQcRunningVerificationPending =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwQcRunningVerificationPending;
        }

        public List<HwQcAssignCustomMasterModel> GetHwQcFinishedGoodsVerificationPending(long whologgedin)
        {
            string query = string.Format(@"select distinct STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                                                                       hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,hqa.QcSubmissionDate,hqa.Status from HwQcAssigns hqa 
                                                                                       inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                                       inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                                                                       inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId where hqia.IsFinishedGoodTest={0} and hqa.Status='{1}' ", 1, "QCSUBMITTED");
            var getHwQcRunningVerificationPending =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwQcRunningVerificationPending;
        }

        public HwTestPcbModel GetHwTestPcb(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestPcbs where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestPcb = _dbeEntities.Database.SqlQuery<HwTestPcbModel>(query).FirstOrDefault();
            return getHwTestPcb;
        }

        public HwTestPcbAModel GetHwTestPcbA(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestPcbAs where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestPcbA = _dbeEntities.Database.SqlQuery<HwTestPcbAModel>(query).FirstOrDefault();
            return getHwTestPcbA;
        }

        public HwTestCameraInfoModel GetHwTestCameraInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestCameraInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestCameraInfo = _dbeEntities.Database.SqlQuery<HwTestCameraInfoModel>(query).FirstOrDefault();
            return getHwTestCameraInfo;
        }

        public HwTestTpLcdInfoModel GetHwTestTpLcdInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestTpLcdInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTpLcdInfo = _dbeEntities.Database.SqlQuery<HwTestTpLcdInfoModel>(query).FirstOrDefault();
            return getHwTpLcdInfo;
        }

        public HwTestSoundInfoModel GetHwTestSoundInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestSoundInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestSoundInfo = _dbeEntities.Database.SqlQuery<HwTestSoundInfoModel>(query).FirstOrDefault();
            return getHwTestSoundInfo;
        }

        public HwTestFPCandSIMSlotInfoModel GetHwTestFpCandSimSlotInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestFPCandSimSlotInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var hwTestFPCandSimSlotInfo = _dbeEntities.Database.SqlQuery<HwTestFPCandSIMSlotInfoModel>(query).FirstOrDefault();
            return hwTestFPCandSimSlotInfo;
        }

        public HwTestBatteryInfoModel GetHwTestBatteryInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestBatteryInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var hwTestBatteryInfo = _dbeEntities.Database.SqlQuery<HwTestBatteryInfoModel>(query).FirstOrDefault();
            return hwTestBatteryInfo;
        }

        public HwTestChargerInfoModel GetHwTestChargerInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestChargerInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var hwTestChargerInfo = _dbeEntities.Database.SqlQuery<HwTestChargerInfoModel>(query).FirstOrDefault();
            return hwTestChargerInfo;
        }

        public HwTestUSBCableInfoModel GetHwTestUSBCableInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestUSBCableInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var hwTestUSBCableInfo = _dbeEntities.Database.SqlQuery<HwTestUSBCableInfoModel>(query).FirstOrDefault();
            return hwTestUSBCableInfo;
        }

        public HwTestEarphoneInterfaceInfoModel GetHwTestEarphoneInterfaceInfoModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestEarphoneInterfaceInfos where HwQcInchargeAssignId={0}",
                hwQcInchargeAssignId);
            var hwTestEarphone =
                _dbeEntities.Database.SqlQuery<HwTestEarphoneInterfaceInfoModel>(query).FirstOrDefault();
            return hwTestEarphone;
        }

        public HwProjectMasterCustomModel GetProjectAndAssignDetailByHwQcInchargeAssignId(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  where hqa.HwQcInchargeAssignId = {0} for xml path(''),type ).value('.','nvarchar(max)'),1,1,'')
										   as ScreeningDoneBy,pm.ProjectMasterId,pm.ProjectName,pm.DisplayName,pm.SupplierName,hqia.ReceivedSampleQuantity,pm.ProcessorName,pm.SupplierModelName,pm.ProcessorClock,pm.OsName,pm.Chipset,pm.OsVersion,pm.FrontCamera,
                                           pm.BackCamera,pm.DisplaySize,pm.Ram,cu.UserFullName as ProvidedByName,hqia.SampleSetReceiveDate as SampleProvidedDate,pm.OrderNuber
                                           from ProjectMasters pm
                                           inner join HwQcAssigns hqa on pm.ProjectMasterId=hqa.ProjectMasterId
                                           inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                           left join CmnUsers cu on hqia.HwQcInchargeAssignedBy=cu.CmnUserId
                                           where hqa.HwQcInchargeAssignId={0}
                                           group by pm.ProjectMasterId,pm.ProjectName,pm.DisplayName,pm.SupplierName,pm.ProcessorName,pm.SupplierModelName,hqia.ReceivedSampleQuantity,pm.ProcessorClock,pm.OsName,pm.Chipset,pm.OsVersion,pm.FrontCamera,pm.BackCamera,pm.DisplaySize,pm.Ram,cu.UserFullName,hqia.SampleSetReceiveDate,pm.OrderNuber", hwQcInchargeAssignId);
            var getProjectAndAssignDetailByHwQcAssignId =
                _dbeEntities.Database.SqlQuery<HwProjectMasterCustomModel>(query).FirstOrDefault();
            return getProjectAndAssignDetailByHwQcAssignId;
        }


        //Finished goods test GET starts
        public List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForFinishedGoodsForDashBoard(long hwQcUserId)
        {
            string query =
                string.Format(@"Select hqa.HwQcAssignId,hqa.HwQcInchargeAssignId,pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,hqa.HwQcAssignDate,hqa.DeadLineDate,hqa.Status,hqa.QcSubmissionDate from HwQcAssigns hqa
                                inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqa.ProjectMasterId=pm.ProjectMasterId
                                where hqa.HwQcUserId={0} and hqia.IsFinishedGoodTest=1 and hqia.TestPhase not in('FINISHED')", hwQcUserId);
            var getProjectsAssignedToHwQcForScreeningForDashBoard =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getProjectsAssignedToHwQcForScreeningForDashBoard;
        }
        public List<ProjectMasterModel> GetProjectsAssignedToHwQcInchargeForFinishedGoods()
        {
            var listOfProjectByQcIncharge = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(@"Select pm.* 
                                                                                            from HwQcInchargeAssigns hqia 
                                                                                            inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId 
                                                                                            where hqia.IsFinishedGoodTest=1 and hqia.TestPhase in ('NEW','ASSIGNED')").ToList();
            return listOfProjectByQcIncharge;
        }

        public HwQcAssignModel GetHwQcInchargeAssignIdForFinishedGoods(long projectId)
        {
            var getHwQcInchargeAssignId = _dbeEntities.Database.SqlQuery<HwQcAssignModel>(@"Select top 1 hqia.HwQcInchargeAssignId from HwQcInchargeAssigns hqia where hqia.ProjectMasterId='" + projectId + "' and hqia.IsFinishedGoodTest=1 order by hqia.HwQcInchargeAssignId desc").FirstOrDefault();
            return getHwQcInchargeAssignId;
        }

        public List<HwGetQcAssignedByInchargeModel> GetQcAssignedByInchargeAssignIdForFinishedGoods(long hwQcInchargeAssignId, int testStageRunning)
        {
            string query =
                string.Format(@"select cu.CmnUserId, cu.UserFullName,cu.Email,hqa.Status,hqa.HwQcAssignDate,hqa.DeadLineDate from HwQcAssigns hqa
                                                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId
                                                                inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                                where hqa.HwQcInchargeAssignId='{0}' and hqia.IsFinishedGoodTest='{1}'", hwQcInchargeAssignId, testStageRunning);
            var getQcAssignedByInchargeAssignIdAndTestStage =
                _dbeEntities.Database.SqlQuery<HwGetQcAssignedByInchargeModel>(query).ToList();
            return getQcAssignedByInchargeAssignIdAndTestStage;
        }

        public HwFgBatteryTestMasterModel GetHwFgBatteryTestMasterModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwFgBatteryTestMasters where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwFgBatteryTestMasterModel =
                _dbeEntities.Database.SqlQuery<HwFgBatteryTestMasterModel>(query).FirstOrDefault();
            return getHwFgBatteryTestMasterModel;
        }
        public BatteryTestResultSummaryModel GetBatteryTestResultSummaryModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from BatteryTestResultSummarys where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getBatteryTestResultSummaryModel =
                _dbeEntities.Database.SqlQuery<BatteryTestResultSummaryModel>(query).FirstOrDefault();
            return getBatteryTestResultSummaryModel;
        }

        public HwFgBatteryTestConditionModel GetHwFgBatteryTestConditionModel(long hwFgBatteryTestMasterId)
        {
            string query = string.Format(@"select * from HwFgBatteryTestConditions where HwFgBatteryTestMasterId={0}",
                hwFgBatteryTestMasterId);
            var getHwFgBatteryTestConditionModel =
                _dbeEntities.Database.SqlQuery<HwFgBatteryTestConditionModel>(query).FirstOrDefault();
            return getHwFgBatteryTestConditionModel;
        }

        public List<HwFgBatteryTestConditionModel> GetHwFgBatteryTestConditionModelList(long hwFgBatteryTestMasterId)
        {
            string query = string.Format(@"select * from HwFgBatteryTestConditions where HwFgBatteryTestMasterId={0}",
                hwFgBatteryTestMasterId);
            var getHwFgBatteryTestConditionModelList =
                _dbeEntities.Database.SqlQuery<HwFgBatteryTestConditionModel>(query).ToList();
            return getHwFgBatteryTestConditionModelList;
        }

        public List<HwFgBatteryTestResultModel> GetHwFgBatteryTestResultModelList(long hwFgBatteryTestConditionId)
        {
            string query = string.Format(@"select * from HwFgBatteryTestResults where HwFgBatteryTestConditionId={0}",
                hwFgBatteryTestConditionId);
            var getHwFgBatteryTestResultModelList =
                _dbeEntities.Database.SqlQuery<HwFgBatteryTestResultModel>(query).ToList();
            return getHwFgBatteryTestResultModelList;
        }

        public HwFgChargerTestModel GetHwFgChargerTestModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwFgChargerTests where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwFgChargerTestModel = _dbeEntities.Database.SqlQuery<HwFgChargerTestModel>(query).FirstOrDefault();
            return getHwFgChargerTestModel;
        }

        public List<HwFgChargerDetailModel> GetHwFgChargerDetailModel(long hwFgChargerTestId)
        {
            string query = string.Format(@"select * from HwFgChargerDetails where HwFgChargerTestId={0}",
                hwFgChargerTestId);
            var getHwFgChargerDetailModel = _dbeEntities.Database.SqlQuery<HwFgChargerDetailModel>(query).ToList();
            return getHwFgChargerDetailModel;
        }

        public HwFgUsbCableTestModel GetHwFgUsbCableTestModel(long? hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwFgUsbCableTests where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwFgUsbCableTestModel = _dbeEntities.Database.SqlQuery<HwFgUsbCableTestModel>(query).FirstOrDefault();
            return getHwFgUsbCableTestModel;
        }

        public List<HwFgUsbTestDetailModel> GetHwFgUsbTestDetailModelList(long hwFgUsbCableTestId)
        {
            string query = string.Format(@"select * from HwFgUsbTestDetails where HwFgUsbCableTestId={0}",
                hwFgUsbCableTestId);
            var getHwFgUsbTestDetailModelList = _dbeEntities.Database.SqlQuery<HwFgUsbTestDetailModel>(query).ToList();
            return getHwFgUsbTestDetailModelList;
        }

        public HwTestChargingInfoModel GetHwTestChargingInfoModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestChargingInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var hwTestChargingInfo = _dbeEntities.Database.SqlQuery<HwTestChargingInfoModel>(query).FirstOrDefault();
            return hwTestChargingInfo;
        }

        public HwTestHousingInfoModel GetHwTestHousingInfoModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestHousingInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestHousingInfoModel =
                _dbeEntities.Database.SqlQuery<HwTestHousingInfoModel>(query).FirstOrDefault();
            return getHwTestHousingInfoModel;
        }

        public HwTestCrossMatchInfoModel GetHwTestCrossMatchInfoModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestCrossMatchInfos where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestCrossMatchInfoModel =
                _dbeEntities.Database.SqlQuery<HwTestCrossMatchInfoModel>(query).FirstOrDefault();
            return getHwTestCrossMatchInfoModel;
        }

        public HwTestOverallResultModel GetHwTestOverallResultModel(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from HwTestOverallResult where HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestOverallResultModel =
                _dbeEntities.Database.SqlQuery<HwTestOverallResultModel>(query).FirstOrDefault();
            return getHwTestOverallResultModel;
        }

        public List<HwQcAssignCustomMasterModel> GetHwScreeningCompleteProjects()
        {
            string query = string.Format(@"select distinct pm.ProjectMasterId,hqa.HwQcInchargeAssignId, pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,pm.ProjectType,hqia.AddedDate,hqia.UpdatedDate
                                         from projectmasters pm 
                                         left join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                         where hqia.isscreeningtest={0} and hqa.Status='{1}'", 1, "FORWARDED");
            var getHwScreeningCompleteProjects = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwScreeningCompleteProjects;
        }

        public List<HwQcAssignCustomMasterModel> GetHwRunningCompleteProjects()
        {
            string query = string.Format(@"select distinct pm.ProjectMasterId,hqa.HwQcInchargeAssignId, pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,pm.ProjectType,hqia.AddedDate,hqia.UpdatedDate
                                         from projectmasters pm 
                                         left join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                         where hqia.isrunningtest={0} and hqa.Status='{1}'", 1, "FORWARDED");
            var getHwScreeningCompleteProjects = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwScreeningCompleteProjects;
        }

        public List<HwQcAssignCustomMasterModel> GetHwFinishedCompleteProjects()
        {
            string query = string.Format(@"select distinct pm.ProjectMasterId,hqa.HwQcInchargeAssignId, pm.ProjectName,pm.OrderNuber,pm.SupplierModelName,pm.ProjectType,hqia.AddedDate,hqia.UpdatedDate
                                         from projectmasters pm 
                                         left join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                         where hqia.isfinishedgoodtest={0} and hqa.Status='{1}'", 1, "FORWARDED");
            var getHwScreeningCompleteProjects = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getHwScreeningCompleteProjects;
        }

        public HwQcAssignCustomMasterModel GetReportInitialInfo(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"
                                          select pm.ProjectName,cu.UserFullName,hqia.IsFinishedGoodTest,hqia.IsRunningTest,
                                          hqia.HwQcInchargeAssignDate,hqia.ReceivedSampleQuantity,hqia.UpdatedDate,hqia.Remark 
                                          from ProjectMasters pm
                                          inner join ProjectPmAssigns ppa on pm.ProjectMasterId=ppa.ProjectMasterId
                                          inner join CmnUsers cu on ppa.ProjectManagerUserId=cu.CmnUserId
                                          inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                          left join ProjectOrderShipments pos on hqia.ProjectOrderShipmentId=pos.ProjectOrderShipmentId
                                          where  hqia.HwQcInchargeAssignId={0}
                                        ", hwQcInchargeAssignId);
            var getFgInitialInfo = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).FirstOrDefault();
            return getFgInitialInfo;
        }

        public List<CmnUserModel> GetHwTestedBy(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select cu.UserFullName,cu.CmnUserId,cu.UserName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId where hqa.HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestedBy = _dbeEntities.Database.SqlQuery<CmnUserModel>(query).ToList();
            return getHwTestedBy;
        }

        public CmnUserModel GetHwTestCheckedBy(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select distinct cu.UserFullName,cu.CmnUserId,cu.UserName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.VerifiedBy where hqa.HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwTestCheckedBy = _dbeEntities.Database.SqlQuery<CmnUserModel>(query).FirstOrDefault();
            return getHwTestCheckedBy;
        }


        public List<ProjectMasterModel> GetProjectListByItemNameForChipset(string icNoSize, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select * from ProjectMasters pm
                                           inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                           inner join HwTestPcbAs pcba on hqia.HwQcInchargeAssignId=pcba.HwQcInchargeAssignId
                                           where pcba.IcNoSize='{0}' and hqia.HwQcInchargeAssignId!={1}", icNoSize, hwQcInchargeAssignId);
            var getProjectListByItemNameForChipset = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return getProjectListByItemNameForChipset;
        }

        public List<ProjectMasterModel> GetProjectListByItemNameForFlashIc(string icNoSize, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select pm.ProjectName from ProjectMasters pm
                                           inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                           inner join HwTestPcbAs pcba on hqia.HwQcInchargeAssignId=pcba.HwQcInchargeAssignId
                                           where pcba.Flash_IcNoSize='{0}' and hqia.HwQcInchargeAssignId!={1}", icNoSize, hwQcInchargeAssignId);
            var getProjectListByItemNameForFlashIc = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return getProjectListByItemNameForFlashIc;
        }

        public List<ProjectMasterModel> GetProjectListByItemNameForPmu1Ic(string icNoSize, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select pm.ProjectName from ProjectMasters pm
                                           inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                           inner join HwTestPcbAs pcba on hqia.HwQcInchargeAssignId=pcba.HwQcInchargeAssignId
                                           where pcba.PMU1IC='{0}' and hqia.HwQcInchargeAssignId!={1}", icNoSize, hwQcInchargeAssignId);
            var getProjectListByItemNameForPmu1Ic = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return getProjectListByItemNameForPmu1Ic;
        }

        public List<ProjectMasterModel> GetProjectListByItemNameForRfIc(string icNoSize, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select pm.ProjectName from ProjectMasters pm
                                           inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                           inner join HwTestPcbAs pcba on hqia.HwQcInchargeAssignId=pcba.HwQcInchargeAssignId
                                           where pcba.RFIC='{0}' and hqia.HwQcInchargeAssignId!={1}", icNoSize, hwQcInchargeAssignId);
            var getProjectListByItemNameForPmu1Ic = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return getProjectListByItemNameForPmu1Ic;
        }
        public List<ProjectMasterModel> GetProjectListByItemNameForBackCamera(string icNoSize, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select pm.ProjectName from ProjectMasters pm
                                           inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                           inner join HwTestCameraInfos cam on hqia.HwQcInchargeAssignId=cam.HwQcInchargeAssignId
                                           where cam.BackCamera_IcNoSize='{0}' and hqia.HwQcInchargeAssignId!={1}", icNoSize, hwQcInchargeAssignId);
            var getProjectListByItemNameForBackCamera = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return getProjectListByItemNameForBackCamera;
        }

        public List<ProjectMasterModel> GetProjectListByItemNameForFrontCamera(string icNoSize, long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select pm.ProjectName from ProjectMasters pm
                                           inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                           inner join HwTestCameraInfos cam on hqia.HwQcInchargeAssignId=cam.HwQcInchargeAssignId
                                           where cam.FrontCamera_IcNoSize='{0}' and hqia.HwQcInchargeAssignId!={1}", icNoSize, hwQcInchargeAssignId);
            var getProjectListByItemNameForFrontCamera = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return getProjectListByItemNameForFrontCamera;
        }

        public HwBatteryTestCustomModel GetHwBatteryTestCustomModel(long hwQcInchargeAssignId)
        {
            string query = string.Format("select distinct bm.HwFgBatteryTestMasterId, pm.ProjectMasterId, pm.ProjectName,bm.BatteryCapacity,bm.BatteryCellVoltage,bm.MaxChargeVoltage,pm.NumberOfSample," +
                                         " bm.SampleQuantity_Battery,bm.SampleQuantity_Cell,pm.BatterySupplierName,bm.TestDate, bm.TestEnvironment_Temperature,bm.TestEnvironment_Humidity,bm.TestItem " +
                                         "from HwFgBatteryTestMasters bm inner join HwQcInchargeAssigns hqia on bm.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId " +
                                         "inner join ProjectMasters pm on hqia.ProjectMasterId=pm.ProjectMasterId where bm.HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getHwBatteryTestCustomModel =
                _dbeEntities.Database.SqlQuery<HwBatteryTestCustomModel>(query).FirstOrDefault();
            return getHwBatteryTestCustomModel;
        }

        public List<HwFgBatteryTestResultModel> GetHwFgBatteryTestResultByHwQcInchargeAssignId(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select bc.TestCondition,br.* from HwFgBatteryTestMasters bm
                                           inner join HwFgBatteryTestConditions bc on bm.HwFgBatteryTestMasterId=bc.HwFgBatteryTestMasterId
                                           inner join HwFgBatteryTestResults br on bc.HwFgBatteryTestConditionId=br.HwFgBatteryTestConditionId
                                           where bm.HwQcInchargeAssignId={0} order by bc.TestCondition,br.CycleNo,br.ItemName,br.ItemNo", hwQcInchargeAssignId);
            var exe = _dbeEntities.Database.SqlQuery<HwFgBatteryTestResultModel>(query).ToList();
            return exe;
        }

        public BatteryTestResultSummaryModel GetBatteryTestResultSummaryModelByHwQcInchargeId(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"SELECT * FROM BatteryTestResultSummarys WHERE HwQcInchargeAssignId={0}", hwQcInchargeAssignId);
            var getBatteryTestResultSummaryModel =
                _dbeEntities.Database.SqlQuery<BatteryTestResultSummaryModel>(query).FirstOrDefault();
            return getBatteryTestResultSummaryModel;
        }


        public CmnUserModel GetProjectManagerInfoByProjectid(long projectId)
        {
            string query =
                string.Format(
                    @"select * from CmnUsers cu inner join ProjectPmAssigns ppa on cu.CmnUserId=ppa.ProjectManagerUserId where ppa.ProjectMasterId={0}",
                    projectId);
            var get = _dbeEntities.Database.SqlQuery<CmnUserModel>(query).FirstOrDefault();
            return get;
        }

        public List<HwQcAssignCustomMasterModel> GetAllDocs(long hwQcInchargeAssignId)
        {
            string query = string.Format(@"select distinct QcDocUploadPath from HwQcAssigns where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestPcbs where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestPcbAs where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestCameraInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestTpLcdInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestSoundInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestFPCandSIMSlotInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestBatteryInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestChargerInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestUSBCableInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestEarphoneInterfaceInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestChargingInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestHousingInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestCrossMatchInfos where HwQcInchargeAssignId={0} and QcDocUploadPath is not null 
                                           union all
                                           select  qcdocuploadpath from HwTestOverallResult where HwQcInchargeAssignId={0} and QcDocUploadPath is not null", hwQcInchargeAssignId);
            var exe = _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return exe;
        }

        public List<HwItemComponentModel> GetHwItemComponentModels()
        {
            string query = string.Format(@"select * from HwItemComponents");
            var exec = _dbeEntities.Database.SqlQuery<HwItemComponentModel>(query).ToList();
            return exec;
        }

        public List<HwIcComponentNumberModel> GetHwIcComponentNumberModels(long hwItemComponentId)
        {
            string query = string.Format(@"select * from HwIcComponentNumbers where ItemComponentId={0}", hwItemComponentId);
            var exec = _dbeEntities.Database.SqlQuery<HwIcComponentNumberModel>(query).ToList();
            return exec;
        }

        public List<GetHwItemizationModel> GetHwItemizationModels(long hwqcinchargeassignId)
        {
            string query =
                string.Format(
                    @"select hi.HwItemizationId,hi.ProjectMasterId,hi.HwQcInchargeAssignId,hi.HwQcAssignId,hic.ItemComponentName,
                                         hi.YesNot,hicn.IcComponentNumber,hi.IcComponent_Vendor,hi.Compatibility,hi.Type,hi.Remarks,hi.ExistingItem,hi.SupplierCode,
                                         hi.AddedBy,hi.AddedDate,hi.UpdatedBy,hi.UpdatedDate
                                         from HwItemizations hi
                                         inner join HwItemComponents hic on hi.ItemComponentId=hic.ItemComponentId
                                         left join HwIcComponentNumbers hicn on hi.IcComponentNumberId=hicn.IcComponentNumberId where hi.HwQcInchargeAssignId={0}",
                    hwqcinchargeassignId);
            var exec = _dbeEntities.Database.SqlQuery<GetHwItemizationModel>(query).ToList();
            return exec;
        }

        public GetHwItemizationModel GetLatestHwItemizationModel()
        {
            string query =
                 string.Format(
                     @"select hi.HwItemizationId,hi.ProjectMasterId,hi.HwQcInchargeAssignId,hi.HwQcAssignId,hic.ItemComponentName,
                                         hi.YesNot,hicn.IcComponentNumber,hi.IcComponent_Vendor,hi.Compatibility,hi.Type,hi.Remarks,hi.ExistingItem,hi.SupplierCode,
                                         hi.AddedBy,hi.AddedDate,hi.UpdatedBy,hi.UpdatedDate
                                         from HwItemizations hi
                                         inner join HwItemComponents hic on hi.ItemComponentId=hic.ItemComponentId
                                         left join HwIcComponentNumbers hicn on hi.IcComponentNumberId=hicn.IcComponentNumberId order by AddedDate desc");
            var exec = _dbeEntities.Database.SqlQuery<GetHwItemizationModel>(query).FirstOrDefault();
            return exec;
        }

        public HwFieldTestMasterModel GetHwFieldTestMasterModel(long hwQcinchargeAssignId)
        {
            string query = string.Format(@"select * from HwFieldTestMasters where HwQcInchargeAssignId={0}", hwQcinchargeAssignId);
            var exec = _dbeEntities.Database.SqlQuery<HwFieldTestMasterModel>(query).FirstOrDefault();
            return exec;
        }

        public List<HwFieldTestModel> GetAllHwFieldTestModelByFieldTestMasterId(long fieldTestMasterId)
        {
            string query = string.Format(@"select * from HwFieldTests where FieldTestMasterId={0}", fieldTestMasterId);
            var exec = _dbeEntities.Database.SqlQuery<HwFieldTestModel>(query).ToList();
            return exec;
        }

        public List<CmnUserModel> GetHwEnginnersForAssign()
        {
            var hweng = (from m in _dbeEntities.CmnUsers
                         where m.RoleName == "HW" && m.IsActive
                         select new CmnUserModel
                         {
                             CmnUserId = m.CmnUserId,
                             UserFullName = m.UserFullName,
                             UserName = m.UserName
                         }).ToList();
            return hweng;
        }

        public List<HwEngineerAssignModel> GetHwEngineerAssignModels(long assignId)
        {
            var model = new List<HwEngineerAssignModel>();
            var v = (from m in _dbeEntities.HwEngineerAssigns
                     where m.Status != "SUBMITTED"
                     select new HwEngineerAssignModel
                {
                    HwEngineerAssignId = m.HwEngineerAssignId,
                    HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                    ProjectMasterId = m.ProjectMasterId,
                    ProjectName = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == m.ProjectMasterId).Select(x => x.ProjectName).FirstOrDefault(),
                    OrderNumber = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == m.ProjectMasterId).Select(x => x.OrderNuber).FirstOrDefault(),
                    HwEngineerIds = m.HwEngineerIds,
                    HwEngineerNames = m.HwEngineerNames,
                    HwInchargeRemark = m.HwInchargeRemark,
                    Remark = m.Remark,
                    Result = m.Result,
                    AddedBy = m.AddedBy,
                    AddedByName = _dbeEntities.CmnUsers.Where(x=>x.CmnUserId==m.AddedBy).Select(x=>x.UserFullName).FirstOrDefault(),
                    AddedDate = m.AddedDate,
                    SubmittedBy = m.SubmittedBy,
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    Status = m.Status,
                    HwTestMasterId = m.HwTestMasterId,
                    HwTestName = m.HwTestName
                }).ToList();
            foreach (var x in v)
            {
                x.ProjectName = x.ProjectName + " (Order "+x.OrderNumber+")";
                string[] ids = x.HwEngineerIds.Split(',');
                model.AddRange(from t in ids where assignId == Convert.ToInt64(t) select x);
            }
            return model;
        }

        public List<HwTestFileUploadModel> GetHwTestFileUploadModels(long hwinchargeassignId)
        {
            var model = (from m in _dbeEntities.HwTestFileUploads
                where m.HwTestInchargeAssignId == hwinchargeassignId
                select new HwTestFileUploadModel
                {
                    HwTestFileUploadId = m.HwTestFileUploadId,
                    HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                    HwEngineerAssignId = m.HwEngineerAssignId,
                    ProjectMasterId = m.ProjectMasterId,
                    FileUploadPath = m.FileUploadPath,
                    AddedBy = m.AddedBy,
                    AddedByName = _dbeEntities.CmnUsers.Where(x=>x.CmnUserId==m.AddedBy).Select(x=>x.UserFullName).FirstOrDefault(),
                    AddedDate = m.AddedDate,
                    Remarks = m.Remarks
                }).ToList();
            return model;
        }

        public List<HwTestFileUploadModel> GetFileByHwEngAssignId(long id)
        {
            var model = (from m in _dbeEntities.HwTestFileUploads
                         where m.HwEngineerAssignId == id
                         select new HwTestFileUploadModel
                         {
                             HwTestFileUploadId = m.HwTestFileUploadId,
                             HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                             HwEngineerAssignId = m.HwEngineerAssignId,
                             ProjectMasterId = m.ProjectMasterId,
                             FileUploadPath = m.FileUploadPath,
                             AddedBy = m.AddedBy,
                             AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             AddedDate = m.AddedDate,
                             Remarks = m.Remarks
                         }).ToList();
            return model;
        }

        public List<HwTestAdditionalInfoModel> GetHwTestAdditionalInfoModels(long hwinchargeassignId)
        {
            var model = (from v in _dbeEntities.HwTestAdditionalInfos
                where v.HwTestInchargeAssignId == hwinchargeassignId
                select new HwTestAdditionalInfoModel
                {
                    HwTestAdditionalInfoId = v.HwTestAdditionalInfoId,
                    HwEngineerAssignId = v.HwEngineerAssignId,
                    HwTestMasterId = v.HwTestMasterId,
                    HwTestInchargeAssignId = v.HwTestInchargeAssignId,
                    FieldName = v.FieldName,
                    FieldValue = v.FieldValue,
                    AddedBy = v.AddedBy,
                    AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == v.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    AddedDate = v.AddedDate
                }).ToList();
            return model;
        } 

        public HwTestFileUploadModel GetHwTestFileUploadModel(long fileuploadId)
        {
            var model = (from m in _dbeEntities.HwTestFileUploads
                where m.HwTestFileUploadId == fileuploadId
                select new HwTestFileUploadModel
                {
                    HwTestFileUploadId = m.HwTestFileUploadId,
                    HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                    HwEngineerAssignId = m.HwEngineerAssignId,
                    ProjectMasterId = m.ProjectMasterId,
                    FileUploadPath = m.FileUploadPath,
                    AddedBy = m.AddedBy,
                    AddedDate = m.AddedDate,
                    Remarks = m.Remarks
                }).FirstOrDefault();
            return model;
        }
        #endregion
        //GET ENDS


        //=======================Update Methods============================
        #region UPDATE
        public void UpdateHwQcIncharge(long hwQcInchargeAssignId, long? receivedSampleQuantity, string receiveSampleRemark)
        {
            string query = string.Format(@"update HwQcInchargeAssigns set TestPhase='NEW',SampleSetReceiveDate='{1}',ReceivedSampleQuantity={2},ReceiveSampleRemark='{3}' where HwQcInchargeAssignId={0}", hwQcInchargeAssignId, DateTime.Now, receivedSampleQuantity, receiveSampleRemark);
            _dbeEntities.Database.ExecuteSqlCommand(query);
        }


        public HwQcInchargeAssignModel UpdateHwQcInchargeProjectStatus(long hwQcInchargeAssignId, string remark, string status)
        {
            HwQcInchargeAssignModel updateHwQcInchargeProjectStatus;
            if (status == "FINISHED")
            {
                updateHwQcInchargeProjectStatus = _dbeEntities.Database.SqlQuery<HwQcInchargeAssignModel>(@"update  HwQcInchargeAssigns set TestPhase='" + status + "',Remark='" + remark + "',ForwardDate=GETDATE(),UpdatedDate=GETDATE() where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            }
            else
            {
                updateHwQcInchargeProjectStatus = _dbeEntities.Database.SqlQuery<HwQcInchargeAssignModel>(@"update  HwQcInchargeAssigns set TestPhase='" + status + "',Remark='" + remark + "',UpdatedDate=GETDATE() where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            }
            return updateHwQcInchargeProjectStatus;
        }

        public HwQcAssignModel UpdateHwQcDocUploadPath(string hwQcDocUploadPath, long hwQcInchargeAssignId)
        {
            var updateHwQcDocUploadPath = _dbeEntities.Database.SqlQuery<HwQcAssignModel>(@"update HwQcAssigns set QcDocUploadPath='" + hwQcDocUploadPath + "',HwDocUploadDate=GETDATE() where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwQcDocUploadPath;
        }

        public HwTestPcbModel UpdateHwTestPcbDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestPcbModel = _dbeEntities.Database.SqlQuery<HwTestPcbModel>(@"update HwTestPcbs set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestPcbModel;
        }

        public HwTestPcbAModel UpdateHwTestPcbADocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestPcbAModel = _dbeEntities.Database.SqlQuery<HwTestPcbAModel>(@"update HwTestPcbAs set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestPcbAModel;
        }

        public HwTestCameraInfoModel UpdateHwTestCameraInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestCameraInfoModel = _dbeEntities.Database.SqlQuery<HwTestCameraInfoModel>(@"update HwTestCameraInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestCameraInfoModel;
        }

        public HwTestTpLcdInfoModel UpdateHwTestTpLcdInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestTpLcdInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestTpLcdInfoModel>(@"update HwTestTpLcdInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestTpLcdInfoDocUploadPath;
        }

        public HwTestSoundInfoModel UpdateHwTestSoundInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestSoundInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestSoundInfoModel>(@"update HwTestSoundInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestSoundInfoDocUploadPath;
        }

        public HwTestFPCandSIMSlotInfoModel UpdateHwTestFPCandSIMSlotInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestFPCandSIMSlotInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestFPCandSIMSlotInfoModel>(@"update HwTestFPCandSIMSlotInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestFPCandSIMSlotInfoDocUploadPath;
        }

        public HwTestBatteryInfoModel UpdateHwTestBatteryInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestBatterylotInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestBatteryInfoModel>(@"update HwTestBatteryInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestBatterylotInfoDocUploadPath;
        }
        public HwTestChargerInfoModel UpdateHwTestChargerInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestChargerInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestChargerInfoModel>(@"update HwTestChargerInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestChargerInfoDocUploadPath;
        }
        public HwTestUSBCableInfoModel UpdateHwTestUSBCableInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestUSBCableInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestUSBCableInfoModel>(@"update HwTestUSBCableInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestUSBCableInfoDocUploadPath;
        }

        public HwTestEarphoneInterfaceInfoModel UpdateHwTestEarphoneInterfaceInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestEarphoneInterfaceInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestEarphoneInterfaceInfoModel>(@"update HwTestEarphoneInterfaceInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestEarphoneInterfaceInfoDocUploadPath;
        }

        public HwTestChargingInfoModel UpdateHwTestChargingInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestChargingInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestChargingInfoModel>(@"update HwTestChargingInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestChargingInfoDocUploadPath;
        }
        public HwTestHousingInfoModel UpdateHwTestHousingInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId)
        {
            var updateHwTestHousingInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestHousingInfoModel>(@"update HwTestHousingInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcInchargeAssignId='" + hwQcInchargeAssignId + "'").FirstOrDefault();
            return updateHwTestHousingInfoDocUploadPath;
        }
        public HwTestCrossMatchInfoModel UpdateHwTestCrossMatchInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcAssignId)
        {
            var updateHwTestCrossMatchInfoDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestCrossMatchInfoModel>(@"update HwTestCrossMatchInfos set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcAssignId='" + hwQcAssignId + "'").FirstOrDefault();
            return updateHwTestCrossMatchInfoDocUploadPath;
        }

        public HwTestOverallResultModel UpdateHwTestOverallResultDocUploadPath(string hwQcDocUploadPath, long? hwQcAssignId)
        {
            var updateHwTestOverallResultDocUploadPath = _dbeEntities.Database.SqlQuery<HwTestOverallResultModel>(@"update HwTestOverallResult set QcDocUploadPath='" + hwQcDocUploadPath + "' where HwQcAssignId='" + hwQcAssignId + "'").FirstOrDefault();
            return updateHwTestOverallResultDocUploadPath;
        }

        public long UpdateHwQcAssignStatusForQC(long hwQcInchargeAssignId, string status)
        {
            var updateQueryForHwQcAssignStatusUpdate = "";
            if (status == "QCSUBMITTED")
            {
                updateQueryForHwQcAssignStatusUpdate =
                   String.Format(@"Update HwQcAssigns set Status='{1}',QcSubmissionDate='{2}'
                                where HwQcInchargeAssignId=(
                                                            select top 1 hqa.HwQcInchargeAssignId from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
							                                where hqa.HwQcInchargeAssignId={0}
							                                )", hwQcInchargeAssignId, status, DateTime.Now);
            }
            else
            {
                updateQueryForHwQcAssignStatusUpdate =
                String.Format(@"Update HwQcAssigns set Status='{1}',UpdatedDate='{2}'
                                where HwQcInchargeAssignId=(
                                                            select top 1 hqa.HwQcInchargeAssignId from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
							                                where hqa.HwQcInchargeAssignId={0}
							                                )", hwQcInchargeAssignId, status, DateTime.Now);
            }
            //string updateQueryForHwQcInchargeAssignTestPhaseUpdate = string.Format(@"");
            var updateHwQcAssignStatusForQc = _dbeEntities.Database.ExecuteSqlCommand(updateQueryForHwQcAssignStatusUpdate);
            return updateHwQcAssignStatusForQc;
        }

        public void UpdateHwQcAssignStatusForQConForwardProject(long hwQcInchargeAssignId, long userId, string status)//for future use,but have to fix the status issue first
        {
            String updateQueryForHwQcAssignStatusUpdate =
                 String.Format(@"Update HwQcAssigns set Status='{1}',QcSubmissionDate='{2}',Updated={3} where HwQcInchargeAssignId={0}",
                     hwQcInchargeAssignId, status, DateTime.Now, userId);
            //string updateQueryForHwQcInchargeAssignTestPhaseUpdate = string.Format(@"");
            var updateHwQcAssignStatusForQc = _dbeEntities.Database.ExecuteSqlCommand(updateQueryForHwQcAssignStatusUpdate);
        }

        public long UpdateIssueCommentByQcVerifier(string verifierComment, string issueStatus, long hwIssueCommentId, long verifiedBy)
        {
            string updateIssueCommentQuery =
                string.Format(
                    @"update HwIssueComments set VerifierComment='{0}',IssueStatus='{1}',VerifiedBy={3} where HwIssueCommentId={2}",
                    verifierComment, issueStatus, hwIssueCommentId, verifiedBy);
            var updateIssueCommentByQcVerifier = _dbeEntities.Database.ExecuteSqlCommand(updateIssueCommentQuery);
            return updateIssueCommentByQcVerifier;
        }

        public long UpdateQcAssignStatByVerifier(long hwQcInchargeAssignId, long verifiedBy, string status, string verifierName)
        {
            string updateQcAssignStatByVerifierQuery = "";
            var updateQcAssignStatByVerifier = 0;
            if (status == "RUNNING")
            {
                updateQcAssignStatByVerifierQuery = string.Format(@"update HwQcAssigns set Status='{0}' where HwQcInchargeAssignId={1}", status, hwQcInchargeAssignId);
                updateQcAssignStatByVerifier = _dbeEntities.Database.ExecuteSqlCommand(updateQcAssignStatByVerifierQuery);
                return updateQcAssignStatByVerifier;
            }
            updateQcAssignStatByVerifierQuery = string.Format(@"update HwQcAssigns set VerifiedBy={0},Status='{1}',VerifierName='{3}',VerificationDate=GETDATE() where HwQcInchargeAssignId={2}", verifiedBy, status, hwQcInchargeAssignId, verifierName);
            updateQcAssignStatByVerifier = _dbeEntities.Database.ExecuteSqlCommand(updateQcAssignStatByVerifierQuery);
            return updateQcAssignStatByVerifier;
        }

        public void UpdateHwInchargeTestPhaseAfterAllQcDone(long hwQcInchargeAssignId, string status)
        {
            string numberOfQcAssignedByInchargeQuery = string.Format(@"select count(*) screeningAssignCounter from HwQcAssigns hqa
                  inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                  where hqa.HwQcInchargeAssignId={0}", hwQcInchargeAssignId);

            string numberOfQcPassedByQcQuery = string.Format(@"select count(*) from HwQcAssigns hqa 
                       inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId where hqa.Status='{1}'
                       and hqa.HwQcInchargeAssignId={0}",
                                        hwQcInchargeAssignId, "QCPASSED");

            string numberOfQcFailedByQcQuery = string.Format(@"select count(*) from HwQcAssigns hqa 
                             inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId where hqa.Status='{1}'
                             and hqa.HwQcInchargeAssignId={0}",
                             hwQcInchargeAssignId, "QCFAILED");

            string qcPassedUpdateQuery = string.Format(@"update HwQcInchargeAssigns set TestPhase='{0}' where HwQcInchargeAssignId={1}", "QCPASSED", hwQcInchargeAssignId);

            string qcFailedUpdateQuery = string.Format(@"update HwQcInchargeAssigns set TestPhase='{0}' where HwQcInchargeAssignId={1}", "QCFAILED", hwQcInchargeAssignId);

            string checkAndUpdateQuery = string.Format(@"if((" + numberOfQcAssignedByInchargeQuery + ")=(" + numberOfQcPassedByQcQuery + ")) begin " + qcPassedUpdateQuery + " end else if((" + numberOfQcAssignedByInchargeQuery + ")=((" + numberOfQcPassedByQcQuery + ")+(" + numberOfQcFailedByQcQuery + "))) begin " + qcFailedUpdateQuery + " end");

            _dbeEntities.Database.ExecuteSqlCommand(checkAndUpdateQuery);
        }



        public void UpdateProjectMasterScreenTestCompleteStatus(long projectId)
        {
            string updateProjectMasterScreenTestCompleteStatusQuery =
                string.Format(@"update ProjectMasters set IsScreenTestComplete=1,ProjectStatus='{1}' where ProjectMasterId={0}", projectId, "PARTIAL2");
            var updateProjectMasterScreenTestCompleteStatus =
                _dbeEntities.Database.ExecuteSqlCommand(updateProjectMasterScreenTestCompleteStatusQuery);
        }

        public void UpdateHwTestPcbMaterial(long? hwQcInchargeAssignId, string thickness, string materials, string recommendation, string comment, long? updated)
        {
            string query = string.Format(@"update HwTestPcbs set Thickness='{1}',Materials='{2}',Recommendation='{3}',Comment='{4}',Updated={5},UpdatedDate=GETDATE() where HwQcInchargeAssignId={0}", hwQcInchargeAssignId, thickness, materials, recommendation, comment, updated);
            var updateHwTestPcbMaterial = _dbeEntities.Database.ExecuteSqlCommand(query);
        }

        public void UpdateHwTestPcbA(HwTestPcbAModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestPcbAModel, HwTestPcbA>();
            var hwTestPcbaComponentInfo = _dbeEntities.HwTestPcbAs.Find(model.HwTestPcbAId);
            hwTestPcbaComponentInfo = Mapper.Map<HwTestPcbAModel, HwTestPcbA>(model, hwTestPcbaComponentInfo);
            _dbeEntities.Entry(hwTestPcbaComponentInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestCameraInfo(HwTestCameraInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestCameraInfoModel, HwTestCameraInfo>();
            var hwTestCameraInfo = _dbeEntities.HwTestCameraInfos.Find(model.HwTestCameraInfoId);
            hwTestCameraInfo = Mapper.Map<HwTestCameraInfoModel, HwTestCameraInfo>(model, hwTestCameraInfo);
            _dbeEntities.Entry(hwTestCameraInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestTpLcdInfo(HwTestTpLcdInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestTpLcdInfoModel, HwTestTpLcdInfo>();
            var hwTestTpLcdInfo = _dbeEntities.HwTestTpLcdInfos.Find(model.HwTestTpLcdInfoId);
            hwTestTpLcdInfo = Mapper.Map<HwTestTpLcdInfoModel, HwTestTpLcdInfo>(model, hwTestTpLcdInfo);
            _dbeEntities.Entry(hwTestTpLcdInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestSoundInfo(HwTestSoundInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestSoundInfoModel, HwTestSoundInfo>();
            var hwTestSoundInfo = _dbeEntities.HwTestSoundInfos.Find(model.HwTestSoundInfoId);
            hwTestSoundInfo = Mapper.Map<HwTestSoundInfoModel, HwTestSoundInfo>(model, hwTestSoundInfo);
            _dbeEntities.Entry(hwTestSoundInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestFPCandSIMSlotInfo(HwTestFPCandSIMSlotInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestFPCandSIMSlotInfoModel, HwTestFPCandSIMSlotInfo>();
            var hwTestFPCandSIMSlotInfo =
                _dbeEntities.HwTestFPCandSIMSlotInfos.Find(model.HwTestFpcConnectionAndSIMSlotInfoId);
            hwTestFPCandSIMSlotInfo = Mapper.Map<HwTestFPCandSIMSlotInfoModel, HwTestFPCandSIMSlotInfo>(model,
                hwTestFPCandSIMSlotInfo);
            _dbeEntities.Entry(hwTestFPCandSIMSlotInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestBatteryInfo(HwTestBatteryInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestBatteryInfoModel, HwTestBatteryInfo>();
            var hwTestBatteryInfo = _dbeEntities.HwTestBatteryInfos.Find(model.HwTestBatteryInfoId);
            hwTestBatteryInfo = Mapper.Map<HwTestBatteryInfoModel, HwTestBatteryInfo>(model, hwTestBatteryInfo);
            _dbeEntities.Entry(hwTestBatteryInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestChargerInfo(HwTestChargerInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestChargerInfoModel, HwTestChargerInfo>();
            var hwTestChargerInfo = _dbeEntities.HwTestChargerInfos.Find(model.HwTestChargerInfoId);
            hwTestChargerInfo = Mapper.Map<HwTestChargerInfoModel, HwTestChargerInfo>(model, hwTestChargerInfo);
            _dbeEntities.Entry(hwTestChargerInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestUSBCableInfo(HwTestUSBCableInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestUSBCableInfoModel, HwTestUSBCableInfo>();
            var hwUsbCableInfo = _dbeEntities.HwTestUSBCableInfos.Find(model.HwTestUSBCableInfoId);
            hwUsbCableInfo = Mapper.Map<HwTestUSBCableInfoModel, HwTestUSBCableInfo>(model, hwUsbCableInfo);
            _dbeEntities.Entry(hwUsbCableInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwFgBatteryTestMaster(HwFgBatteryTestMasterModel model)
        {
            Mapper.CreateMap<HwFgBatteryTestMasterModel, HwFgBatteryTestMaster>();
            var hwFgBatteryTestMaster = _dbeEntities.HwFgBatteryTestMasters.Find(model.HwFgBatteryTestMasterId);
            hwFgBatteryTestMaster = Mapper.Map<HwFgBatteryTestMasterModel, HwFgBatteryTestMaster>(model,
                hwFgBatteryTestMaster);
            _dbeEntities.Entry(hwFgBatteryTestMaster).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }
        public void UpdateBatteryTestResultSummary(BatteryTestResultSummaryModel model)
        {
            Mapper.CreateMap<BatteryTestResultSummaryModel, BatteryTestResultSummary>();
            var batteryTestResultSummary = _dbeEntities.BatteryTestResultSummarys.Find(model.BatteryTestResultSummaryId);
            batteryTestResultSummary = Mapper.Map<BatteryTestResultSummaryModel, BatteryTestResultSummary>(model,
                batteryTestResultSummary);
            _dbeEntities.Entry(batteryTestResultSummary).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwFgBatteryTestCondition(HwFgBatteryTestConditionModel model)
        {
            Mapper.CreateMap<HwFgBatteryTestConditionModel, HwFgBatteryTestCondition>();
            var updateHwFgBatteryTestCondition =
                _dbeEntities.HwFgBatteryTestConditions.Find(model.HwFgBatteryTestConditionId);
            updateHwFgBatteryTestCondition = Mapper.Map<HwFgBatteryTestConditionModel, HwFgBatteryTestCondition>(model,
                updateHwFgBatteryTestCondition);
            _dbeEntities.Entry(updateHwFgBatteryTestCondition).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwFgChargerTest(HwFgChargerTestModel model)
        {
            Mapper.CreateMap<HwFgChargerTestModel, HwFgChargerTest>();
            var updateHwFgChargerTest = _dbeEntities.HwFgChargerTests.Find(model.HwFgChargerTestId);
            updateHwFgChargerTest = Mapper.Map<HwFgChargerTestModel, HwFgChargerTest>(model, updateHwFgChargerTest);
            _dbeEntities.Entry(updateHwFgChargerTest).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwFgUsbCableTest(HwFgUsbCableTestModel model)
        {
            Mapper.CreateMap<HwFgUsbCableTestModel, HwFgUsbCableTest>();
            var updateHwFgUsbCableTest = _dbeEntities.HwFgUsbCableTests.Find(model.HwFgUsbCableTestId);
            updateHwFgUsbCableTest = Mapper.Map<HwFgUsbCableTestModel, HwFgUsbCableTest>(model, updateHwFgUsbCableTest);
            _dbeEntities.Entry(updateHwFgUsbCableTest).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestEarphoneInterfaceInfo(HwTestEarphoneInterfaceInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestEarphoneInterfaceInfoModel, HwTestEarphoneInterfaceInfo>();
            var updateHwTestEarphoneInterfaceInfo =
                _dbeEntities.HwTestEarphoneInterfaceInfos.Find(model.HwTestEarphoneInterfaceInfoId);
            updateHwTestEarphoneInterfaceInfo =
                Mapper.Map<HwTestEarphoneInterfaceInfoModel, HwTestEarphoneInterfaceInfo>(model,
                    updateHwTestEarphoneInterfaceInfo);
            _dbeEntities.Entry(updateHwTestEarphoneInterfaceInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestChargingInfo(HwTestChargingInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestChargingInfoModel, HwTestChargingInfo>();
            var updateHwTestChargingInfo = _dbeEntities.HwTestChargingInfos.Find(model.HwTestChargingInfoId);
            updateHwTestChargingInfo = Mapper.Map<HwTestChargingInfoModel, HwTestChargingInfo>(model,
                updateHwTestChargingInfo);
            _dbeEntities.Entry(updateHwTestChargingInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestHousingInfo(HwTestHousingInfoModel model)
        {
            model.QcDocUploadPath = (model.QcDocUploadPath == "../Content/UploadImage/") ? null : model.QcDocUploadPath;
            Mapper.CreateMap<HwTestHousingInfoModel, HwTestHousingInfo>();
            var updateHwTestHousingInfo = _dbeEntities.HwTestHousingInfos.Find(model.HwTestHousingInfoId);
            updateHwTestHousingInfo = Mapper.Map<HwTestHousingInfoModel, HwTestHousingInfo>(model,
                updateHwTestHousingInfo);
            _dbeEntities.Entry(updateHwTestHousingInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestCrossMatchInfo(HwTestCrossMatchInfoModel model)
        {
            Mapper.CreateMap<HwTestCrossMatchInfoModel, HwTestCrossMatchInfo>();
            var updateHwTestCrossMatchInfo = _dbeEntities.HwTestCrossMatchInfos.Find(model.HwTestCrossMAtchInfoId);
            updateHwTestCrossMatchInfo = Mapper.Map<HwTestCrossMatchInfoModel, HwTestCrossMatchInfo>(model,
                updateHwTestCrossMatchInfo);
            _dbeEntities.Entry(updateHwTestCrossMatchInfo).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwTestOverallResult(HwTestOverallResultModel model)
        {
            Mapper.CreateMap<HwTestOverallResultModel, HwTestOverallResult>();
            var updateHwTestOverallResult = _dbeEntities.HwTestOverallResults.Find(model.HwTestOverallResultId);
            updateHwTestOverallResult = Mapper.Map<HwTestOverallResultModel, HwTestOverallResult>(model,
                updateHwTestOverallResult);
            _dbeEntities.Entry(updateHwTestOverallResult).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }

        public void UpdateHwFieldTestMaster(HwFieldTestMasterModel model)
        {
            Mapper.CreateMap<HwFieldTestMasterModel, HwFieldTestMaster>();
            var updateHwFieldTestMaster = _dbeEntities.HwFieldTestMasters.Find(model.HwQcInchargeAssignId);
            updateHwFieldTestMaster = Mapper.Map<HwFieldTestMasterModel, HwFieldTestMaster>(model,
                updateHwFieldTestMaster);
            _dbeEntities.Entry(updateHwFieldTestMaster).State = EntityState.Modified;
            _dbeEntities.SaveChanges();
        }


        public HwChipsetModel UpdateHwChipset(long chipsetId, string chipsetVendor, string icNoSize, string chipsetCore, string chipsetSpeed, string pinType, int pinNumber, string remarks, long userId)
        {
            string query = string.Format(@"  update HwChipsets 
                                             set ChipsetVendor='{0}',ChipsetCore='{1}',ChipsetSpeed='{2}',IcNoSize='{3}',PinType='{4}',PinNumber={5},Remarks='{6}',
                                             Updated={7},UpdatedDate=GETDATE() 
                                             where ChipsetId={8}", chipsetVendor, chipsetCore, chipsetSpeed, icNoSize, pinType, pinNumber, remarks, userId, chipsetId);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var saveHwChipsetIc = _dbeEntities.Database.SqlQuery<HwChipsetModel>("select * from HwChipsets where ChipsetId='" + chipsetId + "'").FirstOrDefault();
            return saveHwChipsetIc;
        }

        public HwFlashIcModel UpdateHwFlashIcModel(long flashIcId, string flashIcBall, string flashIcRam, string flashIcRom,
            string flashIcTechnology, string flashIcVendor, string icNoSize, int pinNumber, string pinType,
            string remarks, long userId)
        {
            string query =
                string.Format(@"update HwFlashIcs set FlashIdVendor='{0}',IcNoSize='{1}',PinType='{2}',PinNumber={3},FlashIcTechnology='{4}',FlashIcRam='{5}',FlashIcRom='{6}',FlashIcBall='{7}'
                                ,Remarks='{8}',Updated={9},UpdatedDate='{10}' where FlashIcId={11}", flashIcVendor, icNoSize, pinType, pinNumber, flashIcTechnology, flashIcRam, flashIcRom, flashIcBall, remarks, userId, DateTime.Now, flashIcId);
            _dbeEntities.Database.ExecuteSqlCommand(query);
            var getUpdated =
                _dbeEntities.Database.SqlQuery<HwFlashIcModel>("select * from HwFlashIcs where FlashIcId='" + flashIcId + "'").FirstOrDefault();
            return getUpdated;
        }

        public void UpdateHwTestInchargeAssign(string remarks, long hwinchargeassignId = 0,long userId=0)
        {
            var model =_dbeEntities.HwTestInchargeAssigns.FirstOrDefault(x => x.HwTestInchargeAssignId == hwinchargeassignId);
            model.ForwardRemarks = remarks;
            model.ForwardedBy = 1;
            model.ForwrdedDate = DateTime.Now;
            model.Status = "FORWARDED";
            _dbeEntities.HwTestInchargeAssigns.AddOrUpdate(model);
            _dbeEntities.SaveChanges();
        }
        #endregion

        //====================Duplicate check==============================
        #region Duplicate check
        public int CheckDuplicateHwQcAssign(long hwQcUserId, long hwQcInchargeAssignId)
        {
            var isDuplicate =
                _dbeEntities.Database.SqlQuery<int>(
                    @"if exists (select hqa.* from HwQcAssigns hqa  where hqa.HwQcUserId='" + hwQcUserId + "' and hqa.HwQcInchargeAssignId='" + hwQcInchargeAssignId +
                    "') begin select 1 end else begin select 0 end").First();
            int duplicate = Convert.ToInt32(isDuplicate);

            return duplicate;
        }

        public HwTestCustomModel CheckDuplicateHwTest(long? hwQcInchargeAssignId)
        {
            string query = string.Format(@"select
                                           (select top 1 HwTestPcbId from HwTestPcbs where HwQcInchargeAssignId={0}) as HwTestPcbId,
                                           (select top 1 HwtestpcbaId from HwTestPcbAs where HwQcInchargeAssignId={0}) as HwtestpcbaId,
                                           (select top 1 HwTestCameraInfoId from HwTestCameraInfos where HwQcInchargeAssignId={0}) as HwTestCameraInfoId,
                                           (select top 1 HwTestTpLcdInfoId from HwTestTpLcdInfos where HwQcInchargeAssignId={0}) as HwTestTpLcdInfoId,
                                           (select top 1 HwTestSoundInfoId from HwTestSoundInfos where HwQcInchargeAssignId={0}) as HwTestSoundInfoId,
                                           (select top 1 HwTestFpcConnectionAndSIMSlotInfoId from HwTestFPCandSIMSlotInfos where HwQcInchargeAssignId={0}) as HwTestFpcConnectionAndSIMSlotInfoId,
                                           (select top 1 HwTestBatteryInfoId from HwTestBatteryInfos where HwQcInchargeAssignId={0}) as HwTestBatteryInfoId,
                                           (select top 1 HwTestChargerInfoId from HwTestChargerInfos where HwQcInchargeAssignId={0}) as HwTestChargerInfoId,
                                           (select top 1 HwTestUSBCableInfoId from HwTestUSBCableInfos where HwQcInchargeAssignId={0}) as HwTestUSBCableInfoId,
                                           (select top 1 HwTestEarphoneInterfaceInfoId from HwTestEarphoneInterfaceInfos where HwQcInchargeAssignId={0}) as HwTestEarphoneInterfaceInfoId,
                                           (select top 1 HwTestChargingInfoId from HwTestChargingInfos where HwQcInchargeAssignId={0}) as  HwTestChargingInfoId,
                                           (select top 1 HwTestHousingInfoId from HwTestHousingInfos where HwQcInchargeAssignId={0}) as HwTestHousingInfoId,
                                           (select top 1 HwTestCrossMatchInfoId from HwTestCrossMatchInfos where HwQcInchargeAssignId={0}) as HwTestCrossMatchInfoId,
                                           (select top 1 HwTestOverallResultId from HwTestOverallResult where HwQcInchargeAssignId={0}) as HwTestOverallResultId,
										   (select top 1 HwFgBatteryTestMasterId from HwFgBatteryTestMasters where HwQcInchargeAssignId={0}) as HwFgBatteryTestMasterId,
										   (select top 1 HwFgChargerTestId from HwFgChargerTests where HwQcInchargeAssignId={0}) as HwFgChargerTestId,
										   (select top 1 HwFgUsbCableTestId from HwFgUsbCableTests where HwQcInchargeAssignId={0}) as HwFgUsbCableTestId", hwQcInchargeAssignId);
            var checkDuplicateHwTest = _dbeEntities.Database.SqlQuery<HwTestCustomModel>(query).FirstOrDefault();
            return checkDuplicateHwTest;
        }
        #endregion

        //======================DELETE========================
        #region DELETE

        public void DeleteHwFgBatteryTestCondition(long hwFgTestConditionId)
        {
            HwFgBatteryTestConditionModel model = new HwFgBatteryTestConditionModel();
            model.HwFgBatteryTestConditionId = hwFgTestConditionId;
            Mapper.CreateMap<HwFgBatteryTestConditionModel, HwFgBatteryTestCondition>();
            var updateHwFgBatteryTestCondition =
                _dbeEntities.HwFgBatteryTestConditions.Find(model.HwFgBatteryTestConditionId);
            updateHwFgBatteryTestCondition = Mapper.Map<HwFgBatteryTestConditionModel, HwFgBatteryTestCondition>(model,
                updateHwFgBatteryTestCondition);
            _dbeEntities.Entry(updateHwFgBatteryTestCondition).State = EntityState.Deleted;
            _dbeEntities.SaveChanges();
        }
        #endregion

        #region HW Self Test

        public HwSelfTestModel SaveHwSelfTestModel(HwSelfTestModel model)
        {
            Mapper.CreateMap<HwSelfTestModel, HwSelfTest>();
            var m = Mapper.Map<HwSelfTest>(model);
            _dbeEntities.HwSelfTests.Add(m);
            _dbeEntities.SaveChanges();
            model.HwSelfTestId = m.HwSelfTestId;
            model.AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == model.AddedBy).Select(x => x.UserFullName).FirstOrDefault();
            return model;
        }

        public List<HwEngineerAssignModel> GetHwSelfTests(long addedby)
        {
            var model = (from m in _dbeEntities.HwEngineerAssigns
                where m.AddedBy == addedby && m.HwTestInchargeAssignId==null
                select new HwEngineerAssignModel
                {
                    HwEngineerAssignId = m.HwEngineerAssignId,
                    HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                    ProjectMasterId = m.ProjectMasterId,
                    ProjectName = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == m.ProjectMasterId).Select(x => x.ProjectName).FirstOrDefault(),
                    OrderNumber = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == m.ProjectMasterId).Select(x => x.OrderNuber).FirstOrDefault(),
                    HwEngineerIds = m.HwEngineerIds,
                    HwEngineerNames = m.HwEngineerNames,
                    HwInchargeRemark = m.HwInchargeRemark,
                    Remark = m.Remark,
                    Result = m.Result,
                    AddedBy = m.AddedBy,
                    AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    AddedDate = m.AddedDate,
                    SubmittedBy = m.SubmittedBy,
                    SubmittedDate = m.SubmittedDate,
                    SubmittedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.SubmittedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    Status = m.Status,
                    HwTestMasterId = m.HwTestMasterId,
                    HwTestName = m.HwTestName
                }).ToList();
            return model;
        }

        public HwEngineerAssignModel SaveEngineerAssignModelForSelfTest(HwEngineerAssignModel model)
        {
            Mapper.CreateMap<HwEngineerAssignModel, HwEngineerAssign>();
            var v = Mapper.Map<HwEngineerAssign>(model);
            _dbeEntities.HwEngineerAssigns.Add(v);
            _dbeEntities.SaveChanges();
            model.HwEngineerAssignId=v.HwEngineerAssignId;
            model = (from m in _dbeEntities.HwEngineerAssigns
                     where m.HwEngineerAssignId==v.HwEngineerAssignId
                     select new HwEngineerAssignModel
                     {
                         HwEngineerAssignId = m.HwEngineerAssignId,
                         HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                         ProjectMasterId = m.ProjectMasterId,
                         ProjectName = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == m.ProjectMasterId).Select(x => x.ProjectName).FirstOrDefault(),
                         OrderNumber = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == m.ProjectMasterId).Select(x => x.OrderNuber).FirstOrDefault(),
                         HwEngineerIds = m.HwEngineerIds,
                         HwEngineerNames = m.HwEngineerNames,
                         HwInchargeRemark = m.HwInchargeRemark,
                         Remark = m.Remark,
                         Result = m.Result,
                         AddedBy = m.AddedBy,
                         AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                         AddedDate = m.AddedDate,
                         SubmittedBy = m.SubmittedBy,
                         SubmittedDate = m.SubmittedDate,
                         SubmittedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.SubmittedBy).Select(x => x.UserFullName).FirstOrDefault(),
                         UpdatedBy = m.UpdatedBy,
                         UpdatedDate = m.UpdatedDate,
                         Status = m.Status,
                         HwTestMasterId = m.HwTestMasterId,
                         HwTestName = m.HwTestName
                     }).FirstOrDefault();
            return model;
        }
        #endregion
    }
}
