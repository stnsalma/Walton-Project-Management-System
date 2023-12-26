using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;
using ProjectManagement.ViewModels.Commercial;
using ProjectManagement.ViewModels.Management;
using ProjectManagement.ViewModels.ProjectManager;
using ProjectManagement.ViewModels.Software;
using SignalRDemo.DAL;
using Incentive = ProjectManagement.DAL.DbModel.Incentive;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.Mvc;
using ProjectManagement.ViewModels.Global;
using System.IO;


namespace ProjectManagement.Infrastructures.Repositories
{
    public class ProjectManagerRepository : IProjectManagerRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;
        private readonly RBSYNERGYEntities _dbRBSYEntities;
        private readonly MRPEntities _dbMrpEntities;
        //  dbeEntities.Con
        //public int totalrow = 1;
        private int currentrow = 0;
        private int totalrow = 1;
        private int totalRow
        {
            get { return totalrow; }
            set
            {
                totalrow = value;

            }
        }
        private int currentRow
        {
            get { return currentrow; }
            set
            {
                currentrow = value;

            }
        }

        public ProjectManagerRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
            _dbRBSYEntities = new RBSYNERGYEntities();
            _dbRBSYEntities.Configuration.LazyLoadingEnabled = false;
            _dbMrpEntities = new MRPEntities();
            _dbMrpEntities.Configuration.LazyLoadingEnabled = false;
        }

        #region GetMthod
        public PmGiftBoxModel GetPmGiftBoxModel(long projectMasterId, long userId)
        {
            var pmGbofAModel =
                _dbeEntities.PmGiftBoxes.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId).Select(y => new PmGiftBoxModel
                {
                    PmGiftBoxId = y.PmGiftBoxId,
                    ProjectAssignId = y.ProjectAssignId,
                    ProjectMasterId = y.ProjectMasterId,
                    PmGbImageUploadPath = y.pmGbImageUploadPath,
                    Remarks = y.Remarks,
                    Added = y.Added,
                    AddedDate = y.AddedDate

                }).FirstOrDefault();

            return pmGbofAModel;
        }

        public PmLabelsModel GetPmLabelsModel(long projectMasterId, long userId)
        {
            var pmLabelsOfAModel =
                _dbeEntities.PmLabels.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId).Select(y => new PmLabelsModel
                {
                    ProjectAssignId = y.ProjectAssignId,
                    PmLabelId = y.PmLabelId,
                    ProjectMasterId = y.ProjectMasterId,
                    PmLabelImageUploadPath = y.pmLabelImageUploadPath,
                    Remarks = y.Remarks,
                    Added = y.Added,
                    AddedDate = y.AddedDate

                }).FirstOrDefault();

            return pmLabelsOfAModel;
        }

        public PmScreenProtectorModel GetPmScreenProtectorModel(long projectMasterId, long userId)
        {
            var pmScreenProtectorOfAModel =
                 _dbeEntities.PmScreenProtectors.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId).Select(y => new PmScreenProtectorModel
                 {
                     PmScreenProtectorId = y.PmScreenProtectorId,
                     ProjectAssignId = y.ProjectAssignId,
                     ProjectMasterId = y.ProjectMasterId,
                     PmScreenProtectorImageUploadPath = y.PmScreenProtectorImageUploadPath,
                     Remarks = y.Remarks,
                     Added = y.Added,
                     AddedDate = y.AddedDate

                 }).FirstOrDefault();

            return pmScreenProtectorOfAModel;
        }

        public PmServiceDocumentsModel GetPmServiceDocumentsModel(long projectMasterId)
        {
            var pmServceDocumentOfAModel =
                  _dbeEntities.PmServiceDocuments.Where(x => x.ProjectMasterId == projectMasterId).Select(y => new PmServiceDocumentsModel
                  {
                      PmServiceDocumentId = y.PmServiceDocumentId,
                      ProjectMasterId = y.ProjectMasterId,
                      ProjectAssignId = y.ProjectAssignId


                  }).FirstOrDefault();

            return pmServceDocumentOfAModel;
        }

        public PmSwCustomizationModel GetPmSwCustomizationModel(long projectMasterId)
        {
            var pmSwCustomizationOfAModel =
                   _dbeEntities.PmSwCustomizations.Where(x => x.ProjectMasterId == projectMasterId).Select(y => new PmSwCustomizationModel
                   {
                       // CustomizationItemName = y.CustomizationItemName,
                       PmSwCustomizationId = y.PmSwCustomizationId,
                       ProjectAssignId = y.ProjectAssignId,
                       ProjectMasterId = y.ProjectMasterId,
                       PmSwCustomizationUploadPath = y.PmSwCustomizationUploadPath,
                       PmSwCustomizationUploadPath2 = y.PmSwCustomizationUploadPath2



                   }).FirstOrDefault();

            return pmSwCustomizationOfAModel;
        }

        public PmIdModel GetPmIdModel(long projectMasterId, long userId)
        {
            var pmIdOfAModel =
                   _dbeEntities.PmIDs.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId).Select(y => new PmIdModel
                   {
                       PmIDId = y.PmIDId,
                       ProjectAssignId = y.ProjectAssignId,
                       ProjectMasterId = y.ProjectMasterId,
                       PmFinishingImageUploadPath = y.PmFinishingImageUploadPath,
                       PmIdDesignImageUploadPath = y.PmIdDesignImageUploadPath,
                       PmLogoTypeImageUploadPath = y.PmLogoTypeImageUploadPath,
                       PmModelPrintImageUploadPath = y.PmModelPrintImageUploadPath,
                       Remarks = y.Remarks,
                       Added = y.Added,
                       AddedDate = y.AddedDate

                   }).FirstOrDefault();

            return pmIdOfAModel;
        }

        public PmWalpaperModel GetPmWalpaperModel(long projectMasterId, long userId)
        {
            var pmWalpaperOfAModel =
                   _dbeEntities.PmWalpapers.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId).Select(y => new PmWalpaperModel
                   {

                       PmWalpaperId = y.PmWalpaperId,
                       ProjectMasterId = y.ProjectMasterId,
                       ProjectAssignId = y.ProjectAssignId,
                       WalpaperUpload1 = y.WalpaperUpload1,
                       WalpaperUpload2 = y.WalpaperUpload2,
                       WalpaperUpload3 = y.WalpaperUpload3,
                       WalpaperUpload4 = y.WalpaperUpload4,
                       WalpaperUpload5 = y.WalpaperUpload5,
                       WalpaperUpload6 = y.WalpaperUpload6,
                       WalpaperUpload7 = y.WalpaperUpload7,
                       Remarks = y.Remarks,
                       Added = y.Added,
                       AddedDate = y.AddedDate

                   }).FirstOrDefault();

            return pmWalpaperOfAModel;
        }

        public PmPhnAccessoriesModel GetPmPhnAccessoriesModel(long projectMasterId, long userId)
        {
            var pmPhnAccessoriesModel =
                _dbeEntities.PmPhnAccessories.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId)
                    .Select(y => new PmPhnAccessoriesModel
                    {
                        PmPhnAccessoriesID = y.PmPhnAccessoriesID,
                        ProjectMasterId = y.ProjectMasterId,
                        PmPhnAccessoriesEarphone = y.PmPhnAccessoriesEarphone,
                        PmPhnAccessoriesUSBCable = y.PmPhnAccessoriesUSBCable,
                        PmPhnAccessoriesCharger = y.PmPhnAccessoriesCharger,
                        PmPhnAccessoriesOTGCable = y.PmPhnAccessoriesOTGCable,
                        PmPhnAccessoriesBackCover = y.PmPhnAccessoriesBackCover,
                        PmPhnAccessoriesFlipCover = y.PmPhnAccessoriesFlipCover,
                        Remarks = y.Remarks,
                        Added = y.Added,
                        AddedDate = y.AddedDate

                    }).FirstOrDefault();
            return pmPhnAccessoriesModel;
        }
        public List<PmSwCustomizationInitialModel> GetPmSwCustomizationInitialModels(long projectMasterId)
        {

            var swCustomizationInitial = new List<PmSwCustomizationInitialModel>();


            var projectAssignId =
               _dbeEntities.ProjectPmAssigns.Where(x => x.ProjectMasterId == projectMasterId && x.Status == "ASSIGNED")
                   .Select(y => y.ProjectPmAssignId).FirstOrDefault();



            var assignuserId =
              _dbeEntities.ProjectPmAssigns.Where(x => x.ProjectMasterId == projectMasterId && x.Status == "ASSIGNED")
                  .Select(y => y.AssignUserId).FirstOrDefault();


            //get project type

            var projectType =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == projectMasterId)
                    .Select(y => y.ProjectType).FirstOrDefault();

            if (projectType == "Smart")
            {
                var pmSwCustomizationInitialOfAModel =
                    _dbeEntities.PmSwCustomizationInitials.Where(x => x.IsSmartPhone == true).ToList();

                Mapper.Initialize(cfg => cfg.CreateMap<PmSwCustomizationInitial, PmSwCustomizationInitialModel>());
                swCustomizationInitial = Mapper.Map<List<PmSwCustomizationInitial>, List<PmSwCustomizationInitialModel>>(pmSwCustomizationInitialOfAModel);


                foreach (var pmSwCustomizationInitial in swCustomizationInitial)
                {
                    pmSwCustomizationInitial.ProjectPmAssignId = projectAssignId;
                    pmSwCustomizationInitial.AssignUserId = assignuserId;
                    pmSwCustomizationInitial.ProjectMasterId = projectMasterId;
                }


                return swCustomizationInitial;

            }
            if (projectType == "Feature")
            {

                var pmSwCustomizationInitialOfAModel =
                    _dbeEntities.PmSwCustomizationInitials.Where(x => x.IsFeaturePhone == true).ToList();

                Mapper.Initialize(cfg => cfg.CreateMap<PmSwCustomizationInitial, PmSwCustomizationInitialModel>());
                swCustomizationInitial = Mapper.Map<List<PmSwCustomizationInitial>, List<PmSwCustomizationInitialModel>>(pmSwCustomizationInitialOfAModel);

                foreach (var pmSwCustomizationInitial in swCustomizationInitial)
                {
                    pmSwCustomizationInitial.ProjectPmAssignId = projectAssignId;
                    pmSwCustomizationInitial.AssignUserId = assignuserId;
                    pmSwCustomizationInitial.ProjectMasterId = projectMasterId;
                }

                return swCustomizationInitial;

            }
            if (projectType == "Tablet")
            {

                var pmSwCustomizationInitialOfAModel =
                    _dbeEntities.PmSwCustomizationInitials.Where(x => x.IsTablet == true).ToList();

                Mapper.Initialize(cfg => cfg.CreateMap<PmSwCustomizationInitial, PmSwCustomizationInitialModel>());
                swCustomizationInitial = Mapper.Map<List<PmSwCustomizationInitial>, List<PmSwCustomizationInitialModel>>(pmSwCustomizationInitialOfAModel);

                foreach (var pmSwCustomizationInitial in swCustomizationInitial)
                {
                    pmSwCustomizationInitial.ProjectPmAssignId = projectAssignId;
                    pmSwCustomizationInitial.AssignUserId = assignuserId;
                    pmSwCustomizationInitial.ProjectMasterId = projectMasterId;
                }

                return swCustomizationInitial;

            }
            if (projectType == "WindowsTab")
            {

                var pmSwCustomizationInitialOfAModel =
                    _dbeEntities.PmSwCustomizationInitials.Where(x => x.IsWindowsTablet == true).ToList();

                Mapper.Initialize(cfg => cfg.CreateMap<PmSwCustomizationInitial, PmSwCustomizationInitialModel>());
                swCustomizationInitial = Mapper.Map<List<PmSwCustomizationInitial>, List<PmSwCustomizationInitialModel>>(pmSwCustomizationInitialOfAModel);

                foreach (var pmSwCustomizationInitial in swCustomizationInitial)
                {
                    pmSwCustomizationInitial.ProjectPmAssignId = projectAssignId;
                    pmSwCustomizationInitial.AssignUserId = assignuserId;
                    pmSwCustomizationInitial.ProjectMasterId = projectMasterId;

                }

                return swCustomizationInitial;

            }


            return swCustomizationInitial;


        }

        public List<PmSwCustomizationFinalModel> GetPmSwCustomizationFinalModels(long projectMasterId, long userId)
        {
            string query = string.Format(@"select * from PmSwCustomizationFinal where ProjectMasterid={0} and Added={1}", projectMasterId, userId);
            var pmSwCustomizationFinal = _dbeEntities.Database.SqlQuery<PmSwCustomizationFinalModel>(query).ToList();
            return pmSwCustomizationFinal;
        }

        public List<HwQcInchargeAssignModel> GetHwQcInchargeAssignInfo(long projectId)
        {
            var hwQcAssignInfo = _dbeEntities.HwQcInchargeAssigns.Where(i => i.ProjectMasterId == projectId).ToList();

            Mapper.Initialize(cfg => cfg.CreateMap<HwQcInchargeAssign, HwQcInchargeAssignModel>());
            List<HwQcInchargeAssignModel> hwQcAssignresult = Mapper.Map<List<HwQcInchargeAssign>, List<HwQcInchargeAssignModel>>(hwQcAssignInfo);
            return hwQcAssignresult;

        }

        public PmAllFilesModel GetAllFilesModel(long projectId)
        {
            string query = string.Format(@"select pm.ProjectMasterId,pm.ProjectName,pmbi.ImageUpload1,pmbi.VideoUpload1,pmgb.pmGbImageUploadPath,pmid.PmFinishingImageUploadPath,
                                           pmid.PmIdDesignImageUploadPath,pmid.PmLogoTypeImageUploadPath,pmid.PmModelPrintImageUploadPath,
                                           pml.pmLabelImageUploadPath,pmacc.PmPhnAccessoriesEarphone,pmacc.PmPhnAccessoriesUSBCable,pmacc.PmPhnAccessoriesCharger,
                                           pmacc.PmPhnAccessoriesOTGCable,pmacc.PmPhnAccessoriesBackCover,pmacc.PmPhnAccessoriesFlipCover,
                                           pmsp.PmScreenProtectorImageUploadPath,pmw.WalpaperUpload1,pmw.WalpaperUpload2,pmw.WalpaperUpload3,
                                           pmw.WalpaperUpload4,pmw.WalpaperUpload5,pmw.WalpaperUpload6,pmw.WalpaperUpload7 from ProjectMasters pm
                                           left join PmBootImageAnimations pmbi on pm.ProjectMasterId=pmbi.ProjectMasterId
                                           left join PmGiftBoxes pmgb on pm.ProjectMasterId=pmgb.ProjectMasterId
                                           left join PmIDs pmid on pm.ProjectMasterId=pmid.ProjectMasterId
                                           left join PmLabels pml on pm.ProjectMasterId=pml.ProjectMasterId
                                           left join PmPhnAccessories pmacc on pm.ProjectMasterId=pmacc.ProjectMasterId
                                           left join PmScreenProtectors pmsp on pm.ProjectMasterId=pmsp.ProjectMasterId
                                           left join PmWalpapers pmw on pm.ProjectMasterId=pmw.ProjectMasterId
                                           where pm.ProjectMasterId={0}", projectId);
            var exe = _dbeEntities.Database.SqlQuery<PmAllFilesModel>(query).FirstOrDefault();
            return exe;
        }


        public List<SpareAnalysisReportMonitorModel> GetAnalysisReportMonitorModels()
        {
            string query = string.Format(@"SELECT * FROM SpareAnalysisReportMonitor WHERE ReceiveDate IS NULL");
            var exe = _dbeEntities.Database.SqlQuery<SpareAnalysisReportMonitorModel>(query).ToList();
            return exe;
        }

        #endregion

        #region update Method

        public long UpdateBootImageAnimationInfo(PmBootImageAnimationModel pmBootImageAnimationModel)
        {
            var pmBootImageAnimationId = pmBootImageAnimationModel.PmBootImageAnimationId;
            var remarks = pmBootImageAnimationModel.Remarks;

            if (pmBootImageAnimationId > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmBootImageAnimations.FirstOrDefault(x => x.PmBootImageAnimationId == pmBootImageAnimationId);

                const string userFileDirectory = "PM";

                const string moduleDirectory = "BootAnimation";
                if (getExisitingData != null)
                {

                    var imgForlog = getExisitingData.ImageUpload1;
                    var videoForlog = getExisitingData.VideoUpload1;
                    var rmksForlog = getExisitingData.Remarks;

                    PmBootImageAnimationsLog pmBootImageAnimationsLog = new PmBootImageAnimationsLog
                    {
                        PmBootImageAnimationId = getExisitingData.PmBootImageAnimationId,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        ImageUpload1 = imgForlog,
                        VideoUpload1 = videoForlog,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmBootImageAnimationsLogs.Add(pmBootImageAnimationsLog);

                    getExisitingData.ImageUpload1 = pmBootImageAnimationModel.ImageUploadFile != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmBootImageAnimationModel.ImageUploadFile) : getExisitingData.ImageUpload1;

                    getExisitingData.VideoUpload1 = pmBootImageAnimationModel.VideoUploadFile != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmBootImageAnimationModel.VideoUploadFile) : getExisitingData.VideoUpload1;

                    getExisitingData.Remarks = remarks;

                    _dbeEntities.PmBootImageAnimations.AddOrUpdate(getExisitingData);


                }
                _dbeEntities.SaveChanges();
            }
            return pmBootImageAnimationId;
        }

        public long UpdateGBinfo(PmGiftBoxModel pmGiftBoxModel)
        {
            var pmGiftBoxId = pmGiftBoxModel.PmGiftBoxId;
            var remarks = pmGiftBoxModel.Remarks;

            if (pmGiftBoxId > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmGiftBoxes.FirstOrDefault(x => x.PmGiftBoxId == pmGiftBoxId);
                const string userFileDirectory = "PM";

                const string moduleDirectory = "GBDesign";
                if (getExisitingData != null)
                {
                    var imgForlog = getExisitingData.pmGbImageUploadPath;
                    var rmksForlog = getExisitingData.Remarks;

                    PmGiftBoxesLog pmGiftBoxesLog = new PmGiftBoxesLog
                    {
                        PmGiftBoxId = getExisitingData.PmGiftBoxId,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        pmGbImageUploadPath = imgForlog,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmGiftBoxesLogs.Add(pmGiftBoxesLog);

                    getExisitingData.pmGbImageUploadPath = pmGiftBoxModel.GbDesignUploadFile != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmGiftBoxModel.GbDesignUploadFile) : getExisitingData.pmGbImageUploadPath;
                    getExisitingData.Remarks = remarks;

                    _dbeEntities.PmGiftBoxes.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return pmGiftBoxId;
        }

        public long UpdateLabelInfo(PmLabelsModel pmLabelsModel)
        {
            var pmLabelId = pmLabelsModel.PmLabelId;
            var remarks = pmLabelsModel.Remarks;
            if (pmLabelId > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmLabels.FirstOrDefault(x => x.PmLabelId == pmLabelId);
                const string userFileDirectory = "PM";

                const string moduleDirectory = "Label";
                if (getExisitingData != null)
                {

                    var imgForlog = getExisitingData.pmLabelImageUploadPath;
                    var rmksForlog = getExisitingData.Remarks;

                    PmLabelsLog pmLabelsLog = new PmLabelsLog
                    {
                        PmLabelId = getExisitingData.PmLabelId,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        pmLabelImageUploadPath = imgForlog,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmLabelsLogs.Add(pmLabelsLog);

                    getExisitingData.pmLabelImageUploadPath = pmLabelsModel.LabelImageUploadFile != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmLabelsModel.LabelImageUploadFile) : getExisitingData.pmLabelImageUploadPath;

                    getExisitingData.Remarks = remarks;

                    _dbeEntities.PmLabels.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return pmLabelId;
        }

        public long UpdateIdInfo(PmIdModel pmIdModel)
        {
            var pmIDid = pmIdModel.PmIDId;
            var remarks = pmIdModel.Remarks;
            if (pmIDid > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmIDs.FirstOrDefault(x => x.PmIDId == pmIDid);
                const string userFileDirectory = "PM";

                const string moduleDirectory = "ID";
                if (getExisitingData != null)
                {

                    var imgForlog = getExisitingData.PmFinishingImageUploadPath;
                    var imgForlog1 = getExisitingData.PmLogoTypeImageUploadPath;
                    var imgForlog2 = getExisitingData.PmModelPrintImageUploadPath;
                    var rmksForlog = getExisitingData.Remarks;

                    PmIDsLog pmIDsLog = new PmIDsLog
                    {
                        PmIDId = getExisitingData.PmIDId,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        PmFinishingImageUploadPath = imgForlog,
                        PmLogoTypeImageUploadPath = imgForlog1,
                        PmModelPrintImageUploadPath = imgForlog2,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmIDsLogs.Add(pmIDsLog);

                    getExisitingData.PmFinishingImageUploadPath = pmIdModel.PmFinishImageUpload != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmIdModel.PmFinishImageUpload) : getExisitingData.PmFinishingImageUploadPath;
                    getExisitingData.PmIdDesignImageUploadPath = pmIdModel.PmIdDesignImageUpload != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmIdModel.PmIdDesignImageUpload) : getExisitingData.PmIdDesignImageUploadPath;
                    getExisitingData.PmLogoTypeImageUploadPath = pmIdModel.PmLogoTypeImageUpload != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmIdModel.PmLogoTypeImageUpload) : getExisitingData.PmLogoTypeImageUploadPath;
                    getExisitingData.PmModelPrintImageUploadPath = pmIdModel.PmModelPrintImageUpload != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmIdModel.PmModelPrintImageUpload) : getExisitingData.PmModelPrintImageUploadPath;

                    getExisitingData.Remarks = remarks;

                    _dbeEntities.PmIDs.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return pmIDid;
        }

        public long UpdateScreenProtectorInfo(PmScreenProtectorModel pmScreenProtectorModel)
        {
            var pmScreenProtectorId = pmScreenProtectorModel.PmScreenProtectorId;
            var remarks = pmScreenProtectorModel.Remarks;

            if (pmScreenProtectorId > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmScreenProtectors.FirstOrDefault(x => x.PmScreenProtectorId == pmScreenProtectorId);
                const string userFileDirectory = "PM";

                const string moduleDirectory = "SCRNProtector";
                if (getExisitingData != null)
                {

                    var imgForlog = getExisitingData.PmScreenProtectorImageUploadPath;

                    var rmksForlog = getExisitingData.Remarks;

                    PmScreenProtectorsLog pmScreenProtectorsLog = new PmScreenProtectorsLog
                    {
                        PmScreenProtectorId = getExisitingData.PmScreenProtectorId,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        PmScreenProtectorImageUploadPath = imgForlog,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmScreenProtectorsLogs.Add(pmScreenProtectorsLog);

                    getExisitingData.PmScreenProtectorImageUploadPath = pmScreenProtectorModel.PmScreenProtectorImageUpload != null ? manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                        moduleDirectory, pmScreenProtectorModel.PmScreenProtectorImageUpload) : getExisitingData.PmScreenProtectorImageUploadPath;
                    getExisitingData.Remarks = remarks;

                    _dbeEntities.PmScreenProtectors.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return pmScreenProtectorId;
        }

        public long UpdateServiceDocInfo(PmServiceDocumentsModel pmServiceDocumentsModel)
        {
            var pmServiceDocumentId = pmServiceDocumentsModel.PmServiceDocumentId;

            if (pmServiceDocumentId > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmServiceDocuments.FirstOrDefault(x => x.PmServiceDocumentId == pmServiceDocumentId);

            }
            return pmServiceDocumentId;
        }

        public long UpdateSwCustomizationInfo(PmSwCustomizationModel pmSwCustomizationModel)
        {
            return 0;
        }

        public long UpdateWalPaperInfo(PmWalpaperModel pmWalpaperModel)
        {
            var wallpaperId = pmWalpaperModel.PmWalpaperId;
            var remarks = pmWalpaperModel.Remarks;
            if (wallpaperId > 0)
            {
                // var pastDataOfWallpaper = _dbeEntities.PmWalpapers.Where(i => i.PmWalpaperId == wallpaperID).ToList();


                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmWalpapers.FirstOrDefault(i => i.PmWalpaperId == wallpaperId);
                const string userFileDirectory = "PM";
                const string moduleDirectory = "WallPaper";
                if (getExisitingData != null)
                {

                    var imgForlog1 = getExisitingData.WalpaperUpload1;
                    var imgForlog2 = getExisitingData.WalpaperUpload2;
                    var imgForlog3 = getExisitingData.WalpaperUpload3;
                    var imgForlog4 = getExisitingData.WalpaperUpload4;
                    var imgForlog5 = getExisitingData.WalpaperUpload5;
                    var imgForlog6 = getExisitingData.WalpaperUpload6;
                    var imgForlog7 = getExisitingData.WalpaperUpload7;

                    var rmksForlog = getExisitingData.Remarks;

                    PmWalpapersLog pmWalpapersLog = new PmWalpapersLog
                    {
                        PmWalpaperId = getExisitingData.PmWalpaperId,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        WalpaperUpload1 = imgForlog1,
                        WalpaperUpload2 = imgForlog2,
                        WalpaperUpload3 = imgForlog3,
                        WalpaperUpload4 = imgForlog4,
                        WalpaperUpload5 = imgForlog5,
                        WalpaperUpload6 = imgForlog6,
                        WalpaperUpload7 = imgForlog7,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmWalpapersLogs.Add(pmWalpapersLog);

                    getExisitingData.WalpaperUpload1 = pmWalpaperModel.WalpaperFile1 == null
                        ? getExisitingData.WalpaperUpload1 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                             moduleDirectory, pmWalpaperModel.WalpaperFile1);
                    getExisitingData.WalpaperUpload2 = pmWalpaperModel.WalpaperFile2 == null
                        ? getExisitingData.WalpaperUpload2 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                           moduleDirectory, pmWalpaperModel.WalpaperFile2);
                    getExisitingData.WalpaperUpload3 = pmWalpaperModel.WalpaperFile3 == null
                        ? getExisitingData.WalpaperUpload3 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                             moduleDirectory, pmWalpaperModel.WalpaperFile3);
                    getExisitingData.WalpaperUpload4 = pmWalpaperModel.WalpaperFile4 == null
                        ? getExisitingData.WalpaperUpload4 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                             moduleDirectory, pmWalpaperModel.WalpaperFile4);
                    getExisitingData.WalpaperUpload5 = pmWalpaperModel.WalpaperFile5 == null
                        ? getExisitingData.WalpaperUpload5 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                           moduleDirectory, pmWalpaperModel.WalpaperFile5);
                    getExisitingData.WalpaperUpload6 = pmWalpaperModel.WalpaperFile6 == null
                        ? getExisitingData.WalpaperUpload6 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                           moduleDirectory, pmWalpaperModel.WalpaperFile6);
                    getExisitingData.WalpaperUpload7 = pmWalpaperModel.WalpaperFile7 == null
                        ? getExisitingData.WalpaperUpload7 : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory,
                         moduleDirectory, pmWalpaperModel.WalpaperFile7);
                    getExisitingData.Remarks = remarks;
                    _dbeEntities.PmWalpapers.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return wallpaperId;
        }

        public long UpdateAccessories(PmPhnAccessoriesModel pmPhnAccessoriesModel)
        {
            var accessoriesId = pmPhnAccessoriesModel.PmPhnAccessoriesID;
            var remarks = pmPhnAccessoriesModel.Remarks;

            if (accessoriesId > 0)
            {
                var manager = new FileManager();
                var getExisitingData = _dbeEntities.PmPhnAccessories.FirstOrDefault(x => x.PmPhnAccessoriesID == accessoriesId);
                const string userFileDirectory = "PM";
                const string moduleDirectory = "Accessories";
                if (getExisitingData != null)
                {
                    var imgForlog1 = getExisitingData.PmPhnAccessoriesEarphone;
                    var imgForlog2 = getExisitingData.PmPhnAccessoriesUSBCable;
                    var imgForlog3 = getExisitingData.PmPhnAccessoriesCharger;
                    var imgForlog4 = getExisitingData.PmPhnAccessoriesOTGCable;
                    var imgForlog5 = getExisitingData.PmPhnAccessoriesBackCover;
                    var imgForlog6 = getExisitingData.PmPhnAccessoriesFlipCover;


                    var rmksForlog = getExisitingData.Remarks;

                    PmPhnAccessoriesLog pmPhnAccessoriesLog = new PmPhnAccessoriesLog
                    {
                        PmPhnAccessoriesID = getExisitingData.PmPhnAccessoriesID,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        PmPhnAccessoriesEarphone = imgForlog1,
                        PmPhnAccessoriesUSBCable = imgForlog2,
                        PmPhnAccessoriesCharger = imgForlog3,
                        PmPhnAccessoriesOTGCable = imgForlog4,
                        PmPhnAccessoriesBackCover = imgForlog5,
                        PmPhnAccessoriesFlipCover = imgForlog6,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmPhnAccessoriesLogs.Add(pmPhnAccessoriesLog);

                    getExisitingData.PmPhnAccessoriesEarphone = pmPhnAccessoriesModel.PmPhnAccessoriesEarphoneFile == null ? getExisitingData.PmPhnAccessoriesEarphone : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory, moduleDirectory, pmPhnAccessoriesModel.PmPhnAccessoriesEarphoneFile);
                    getExisitingData.PmPhnAccessoriesUSBCable = pmPhnAccessoriesModel.PmPhnAccessoriesUSBCableFile == null ? getExisitingData.PmPhnAccessoriesUSBCable : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory, moduleDirectory, pmPhnAccessoriesModel.PmPhnAccessoriesUSBCableFile);
                    getExisitingData.PmPhnAccessoriesCharger = pmPhnAccessoriesModel.PmPhnAccessoriesChargerFile == null ? getExisitingData.PmPhnAccessoriesCharger : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory, moduleDirectory, pmPhnAccessoriesModel.PmPhnAccessoriesChargerFile);
                    getExisitingData.PmPhnAccessoriesOTGCable = pmPhnAccessoriesModel.PmPhnAccessoriesOTGCableFile == null ? getExisitingData.PmPhnAccessoriesOTGCable : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory, moduleDirectory, pmPhnAccessoriesModel.PmPhnAccessoriesOTGCableFile);
                    getExisitingData.PmPhnAccessoriesBackCover = pmPhnAccessoriesModel.PmPhnAccessoriesBackCoverFile == null ? getExisitingData.PmPhnAccessoriesBackCover : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory, moduleDirectory, pmPhnAccessoriesModel.PmPhnAccessoriesBackCoverFile);
                    getExisitingData.PmPhnAccessoriesFlipCover = pmPhnAccessoriesModel.PmPhnAccessoriesFlipCoverFile == null ? getExisitingData.PmPhnAccessoriesFlipCover : manager.Upload(getExisitingData.ProjectMasterId, userFileDirectory, moduleDirectory, pmPhnAccessoriesModel.PmPhnAccessoriesFlipCoverFile);
                    getExisitingData.Remarks = remarks;
                    _dbeEntities.PmPhnAccessories.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return accessoriesId;
        }

        public long UpdateCameraInfo(PmPhnCameraModel pmPhnCameraModel)
        {
            var cameraID = pmPhnCameraModel.PmPhnCameraID;
            var remarks = pmPhnCameraModel.Remarks;

            if (cameraID > 0)
            {
                var getExisitingData = _dbeEntities.PmPhnCameras.FirstOrDefault(x => x.PmPhnCameraID == cameraID);

                if (getExisitingData != null)
                {
                    var rmksForlog = getExisitingData.Remarks;

                    PmPhnCameraLog phnCameraLog = new PmPhnCameraLog
                    {
                        PmPhnCameraID = getExisitingData.PmPhnCameraID,
                        ProjectAssignId = getExisitingData.ProjectAssignId,
                        ProjectMasterId = getExisitingData.ProjectMasterId,
                        PmCameraHardwareActual = getExisitingData.PmCameraHardwareActual,
                        PmCameraSoftware = getExisitingData.PmCameraSoftware,
                        Remarks = rmksForlog,
                        Added = getExisitingData.Added,
                        AddedDate = getExisitingData.AddedDate,
                        Updated = getExisitingData.Updated,
                        UpdatedDate = getExisitingData.UpdatedDate
                    };
                    _dbeEntities.PmPhnCameraLogs.Add(phnCameraLog);


                    getExisitingData.PmCameraHardwareActual = pmPhnCameraModel.PmCameraHardwareActual;
                    getExisitingData.PmCameraSoftware = pmPhnCameraModel.PmCameraSoftware;

                    getExisitingData.Remarks = remarks;
                    _dbeEntities.PmPhnCameras.AddOrUpdate(getExisitingData);
                }
                _dbeEntities.SaveChanges();
            }
            return cameraID;
        }

        public long UpdatePmOtaInfo(PmOtaUpdateModel otaUpdateModel)
        {
            var pmOtaUpdateId = otaUpdateModel.PmOtaUpdateId;

            if (pmOtaUpdateId > 0)
            {
                var getExisitingData = _dbeEntities.PmOtaUpdates.FirstOrDefault(x => x.PmOtaUpdateId == pmOtaUpdateId);

                if (getExisitingData != null)
                {
                    getExisitingData.AddedBy = otaUpdateModel.AddedBy;
                    getExisitingData.AddedDate = otaUpdateModel.AddedDate;
                    getExisitingData.BinaryFileRequestDate = otaUpdateModel.BinaryFileRequestDate;
                    getExisitingData.CurrentOtaSwVersion = otaUpdateModel.CurrentOtaSwVersion;
                    getExisitingData.IsExistingMinorIssue = otaUpdateModel.IsExistingMinorIssue;
                    getExisitingData.IsForCustomerSatisfaction = otaUpdateModel.IsForCustomerSatisfaction;
                    getExisitingData.IsHardwareTested = otaUpdateModel.IsHardwareTested;
                    getExisitingData.IsMarketIssue = otaUpdateModel.IsMarketIssue;
                    getExisitingData.IsOtaUploadToServer = otaUpdateModel.IsOtaUploadToServer;
                    getExisitingData.IsPmHeadApprove = otaUpdateModel.IsPmHeadApprove;
                    getExisitingData.IsSoftwareTested = otaUpdateModel.IsSoftwareTested;
                    getExisitingData.OtaUpdateEndDate = otaUpdateModel.OtaUpdateEndDate;
                    getExisitingData.OtaUpdateLog = otaUpdateModel.OtaUpdateLog;
                    getExisitingData.OtaUpdatePublishDate = otaUpdateModel.OtaUpdatePublishDate;
                    getExisitingData.OtaUpdateStartDate = otaUpdateModel.OtaUpdateStartDate;
                    getExisitingData.PmHeadRemarks = otaUpdateModel.PmHeadRemarks;
                    getExisitingData.ProjectManagerUserId = otaUpdateModel.ProjectManagerUserId;
                    getExisitingData.ProjectMasterId = otaUpdateModel.ProjectMasterId;
                    getExisitingData.RunningOtaSWVersion = otaUpdateModel.RunningOtaSWVersion;
                    getExisitingData.UpdatedBy = otaUpdateModel.UpdatedBy;
                    getExisitingData.UpdatedDate = otaUpdateModel.UpdatedDate;


                    _dbeEntities.PmOtaUpdates.AddOrUpdate(getExisitingData);

                }
                _dbeEntities.SaveChanges();
            }
            return pmOtaUpdateId;
        }

        public void SubmitSpareAnalysisReport(long id, long userId)
        {
            string query =
                string.Format(
                    @"update SpareAnalysisReportMonitor set IsReportSubmitted={0},ReportSubmitDate=GETDATE(),SubmittedBy={2} WHERE SpareAnalysisId={1}",
                    1, id, userId);
            _dbeEntities.Database.ExecuteSqlCommand(query);
        }

        public void ReceiveSpareAnalysisReport(long id, long userId)
        {
            string query =
                string.Format(
                    @"update SpareAnalysisReportMonitor set ReceiveDate=GETDATE(),ReceivedBy={1} WHERE SpareAnalysisId={0}",
                     id, userId);
            _dbeEntities.Database.ExecuteSqlCommand(query);
        }

        #endregion


        #region save method
        public long SaveWalPaperInfo(PmWalpaperModel pmWalpaperModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmWalpaperModel, PmWalpaper>());
            var mapper = config.CreateMapper();
            var pmWalpaper = mapper.Map<PmWalpaper>(pmWalpaperModel);

            _dbeEntities.PmWalpapers.Add(pmWalpaper);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmWalpaper.ProjectMasterId,
                ProjectPmAssignId = pmWalpaper.ProjectAssignId,
                ProjectManagerUserId = pmWalpaper.Added,
                Added = pmWalpaper.Added,
                AddedDate = pmWalpaper.AddedDate,
                PmCategoryName = "PmWalpaper",
                AssignUserId = pmWalpaperModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.SaveChanges();
            return pmWalpaper.PmWalpaperId;
        }

        public long SaveSwCustomizationInfo(VmSoftwareCustomization model)
        {
            try
            {

                String userIdentity = HttpContext.Current.User.Identity.Name;

                long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

                List<PmSwCustomizationFinal> customizationFinal = GenericMapper<PmSwCustomizationFinalModel, PmSwCustomizationFinal>.GetDestinationList(model.PmSwCustomizationFinalModels);
                // List<PmSwCustomizationInitial> customizationFinal1 = GenericMapper<pm, PmSwCustomizationFinal>.GetDestinationList(model.PmSwCustomizationFinalModels);

                foreach (var initial in customizationFinal)
                {

                    initial.Added = userId;
                    initial.AddedDate = DateTime.Now;
                }
                //var   projectAssignId =
                //      _dbeEntities.ProjectPmAssigns.Where(
                //          x =>
                //              x.ProjectMasterId == model.PmSwCustomizationInitialModels.ProjectMasterId &&
                //              x.Status == "ASSIGNED")
                //          .Select(y => y.ProjectPmAssignId).FirstOrDefault();



                //var   assignuserId =
                //      _dbeEntities.ProjectPmAssigns.Where(
                //          x =>
                //              x.ProjectMasterId == model.PmSwCustomizationInitialModels.ProjectMasterId &&
                //              x.Status == "ASSIGNED")
                //          .Select(y => y.AssignUserId).FirstOrDefault();


                if (model.Others.Any())
                {
                    customizationFinal.AddRange(model.Others.Select(other => new PmSwCustomizationFinal
                    {
                        ProjectMasterId = model.PmSwCustomizationFinalModels[0].ProjectMasterId,
                        ProjectPmAssignId = model.PmSwCustomizationFinalModels[0].ProjectPmAssignId,
                        PmSwCustomizationFinalMenu = other.PmSwCustomizationFinalMenu,
                        PmSwCustomizationFinalSettings = other.PmSwCustomizationFinalSettings,
                        PmSwCustomizationFinalPath = other.PmSwCustomizationFinalPath,
                        Added = userId,
                        AddedDate = DateTime.Now
                    }));
                }
                _dbeEntities.PmSwCustomizationFinals.AddRange(customizationFinal);

                //////////PmProjectCompletedTaskLog add///////////////
                PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
                {
                    ProjectMasterId = model.PmSwCustomizationFinalModels[0].ProjectMasterId,
                    ProjectPmAssignId = model.PmSwCustomizationFinalModels[0].ProjectPmAssignId,
                    ProjectManagerUserId = userId,
                    Added = userId,
                    AddedDate = DateTime.Now,
                    PmCategoryName = "SwCustomization",
                    AssignUserId = model.PmSwCustomizationFinalModels[0].AssignUserId,

                };

                _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
                //////////PmProjectCompletedTaskLog add///////////////


                _dbeEntities.SaveChanges();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long SaveServiceDocInfo(PmServiceDocumentsModel pmServiceDocumentsModel)
        {
            throw new NotImplementedException();
        }

        public long SaveScreenProtectorInfo(PmScreenProtectorModel pmScreenProtectorModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmScreenProtectorModel, PmScreenProtector>());
            var mapper = config.CreateMapper();
            var pmScreenProtector = mapper.Map<PmScreenProtector>(pmScreenProtectorModel);

            _dbeEntities.PmScreenProtectors.Add(pmScreenProtector);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmScreenProtector.ProjectMasterId,
                ProjectPmAssignId = pmScreenProtector.ProjectAssignId,
                ProjectManagerUserId = pmScreenProtector.Added,
                Added = pmScreenProtector.Added,
                AddedDate = pmScreenProtector.AddedDate,
                PmCategoryName = "PmScreenProtector",
                AssignUserId = pmScreenProtectorModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.SaveChanges();
            return pmScreenProtector.PmScreenProtectorId;

        }

        public long SaveIdInfo(PmIdModel pmIdModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmIdModel, PmID>());
            var mapper = config.CreateMapper();
            var pmId = mapper.Map<PmID>(pmIdModel);

            _dbeEntities.PmIDs.Add(pmId);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmId.ProjectMasterId,
                ProjectPmAssignId = pmId.ProjectAssignId,
                ProjectManagerUserId = pmId.Added,
                Added = pmId.Added,
                AddedDate = pmId.AddedDate,
                PmCategoryName = "PmIDs",
                AssignUserId = pmIdModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.SaveChanges();
            return pmId.PmIDId;
        }

        public long SaveLabelInfo(PmLabelsModel pmLabelsModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmLabelsModel, PmLabel>());
            var mapper = config.CreateMapper();
            var pmLabel = mapper.Map<PmLabel>(pmLabelsModel);

            _dbeEntities.PmLabels.Add(pmLabel);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmLabel.ProjectMasterId,
                ProjectPmAssignId = pmLabel.ProjectAssignId,
                ProjectManagerUserId = pmLabel.Added,
                Added = pmLabel.Added,
                AddedDate = pmLabel.AddedDate,
                PmCategoryName = "PmLabel",
                AssignUserId = pmLabelsModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.SaveChanges();
            return pmLabel.PmLabelId;
        }

        public long SaveGbInfo(PmGiftBoxModel pmGiftBoxModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmGiftBoxModel, PmGiftBox>());
            var mapper = config.CreateMapper();
            var pmGB = mapper.Map<PmGiftBox>(pmGiftBoxModel);

            _dbeEntities.PmGiftBoxes.Add(pmGB);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmGB.ProjectMasterId,
                ProjectPmAssignId = pmGB.ProjectAssignId,
                ProjectManagerUserId = pmGB.Added,
                Added = pmGB.Added,
                AddedDate = pmGB.AddedDate,
                PmCategoryName = "PmGiftBox",
                AssignUserId = pmGiftBoxModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////


            _dbeEntities.SaveChanges();
            return pmGB.PmGiftBoxId;
        }

        public long SaveBootImageAnimationInfo(PmBootImageAnimationModel pmBootImageAnimationModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmBootImageAnimationModel, PmBootImageAnimation>());
            var mapper = config.CreateMapper();
            var pmBootImage = mapper.Map<PmBootImageAnimation>(pmBootImageAnimationModel);

            _dbeEntities.PmBootImageAnimations.Add(pmBootImage);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmBootImage.ProjectMasterId,
                ProjectPmAssignId = pmBootImage.ProjectAssignId,
                ProjectManagerUserId = pmBootImage.Added,
                Added = pmBootImage.Added,
                AddedDate = pmBootImage.AddedDate,
                PmCategoryName = "BootImageAndAnimation",
                AssignUserId = pmBootImageAnimationModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.SaveChanges();
            return pmBootImage.PmBootImageAnimationId;


        }


        public long SaveAccessoriesInfo(PmPhnAccessoriesModel pmPhnAccessoriesModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmPhnAccessoriesModel, PmPhnAccessory>());
            var mapper = config.CreateMapper();
            var pmAccesory = mapper.Map<PmPhnAccessory>(pmPhnAccessoriesModel);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmAccesory.ProjectMasterId,
                ProjectPmAssignId = pmAccesory.ProjectAssignId,
                ProjectManagerUserId = pmAccesory.Added,
                Added = pmAccesory.Added,
                AddedDate = pmAccesory.AddedDate,
                PmCategoryName = "PmAccessories",
                AssignUserId = pmPhnAccessoriesModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.PmPhnAccessories.Add(pmAccesory);
            _dbeEntities.SaveChanges();
            return pmAccesory.PmPhnAccessoriesID;
        }


        public long SaveCameraInfo(PmPhnCameraModel pmPhnCameraModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmPhnCameraModel, PmPhnCamera>());
            var mapper = config.CreateMapper();
            var pmCamera = mapper.Map<PmPhnCamera>(pmPhnCameraModel);

            //////////PmProjectCompletedTaskLog add///////////////
            PmProjectCompletedTaskLog pmProjectCompletedTaskLog = new PmProjectCompletedTaskLog
            {
                ProjectMasterId = pmCamera.ProjectMasterId,
                ProjectPmAssignId = pmCamera.ProjectAssignId,
                ProjectManagerUserId = pmCamera.Added,
                Added = pmCamera.Added,
                AddedDate = pmCamera.AddedDate,
                PmCategoryName = "PmPhnCamera",
                AssignUserId = pmPhnCameraModel.AssignUserId
            };

            _dbeEntities.PmProjectCompletedTaskLogs.Add(pmProjectCompletedTaskLog);
            //////////PmProjectCompletedTaskLog add///////////////

            _dbeEntities.PmPhnCameras.Add(pmCamera);
            _dbeEntities.SaveChanges();
            return pmCamera.PmPhnCameraID;
        }



        public long SavePmOtaUpdateInfo(PmOtaUpdateModel otaUpdateModel)
        {
            var config = new MapperConfiguration(c => c.CreateMap<PmOtaUpdateModel, PmOtaUpdate>());
            var mapper = config.CreateMapper();
            var pmOtaUpdate = mapper.Map<PmOtaUpdate>(otaUpdateModel);

            _dbeEntities.PmOtaUpdates.Add(pmOtaUpdate);
            _dbeEntities.SaveChanges();

            return pmOtaUpdate.PmOtaUpdateId;
        }

        public HwTestInchargeAssignModel SaveHwTestInchargeAssign(HwTestInchargeAssignModel model)
        {
            Mapper.CreateMap<HwTestInchargeAssignModel, HwTestInchargeAssign>();
            var m = Mapper.Map<HwTestInchargeAssign>(model);
            _dbeEntities.HwTestInchargeAssigns.Add(m);
            _dbeEntities.SaveChanges();
            model.HwTestInchargeAssignId = m.HwTestInchargeAssignId;
            model.AddedByName =
                _dbeEntities.CmnUsers.Where(x => x.CmnUserId == model.AddedBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            return model;
        }

        #endregion

        public ProjectMasterModel GetProjectMasterModel(long projectid)
        {
            var projectRepository = new Repository<ProjectMaster>(_dbeEntities);
            ProjectMaster projectMaster = projectRepository.Get(projectid);


            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectMaster, ProjectMasterModel>());
            var mapper = config.CreateMapper();
            var projectMasterModel = mapper.Map<ProjectMasterModel>(projectMaster);


            //projectMasterModel.OrderNumberOrdinal = projectMasterModel.OrderNuber != null
            //         ? CommonConversion.AddOrdinal((int)projectMasterModel.OrderNuber) + " Order"
            //         : string.Empty;
            //if (!string.IsNullOrWhiteSpace(projectMasterModel.OrderNumberOrdinal))
            //{
            //    projectMasterModel.ProjectName = projectMasterModel.ProjectName + " (" + projectMasterModel.OrderNumberOrdinal + ")";
            //}

            //foreach (var project in projectMasterModel)
            //{
            //    project.OrderNumberOrdinal = project.OrderNuber != null
            //        ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
            //        : string.Empty;
            //    if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
            //    {
            //        project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
            //    }

            //}

            return projectMasterModel;

        }
        public List<ProjectMasterModel> GetProjectMasterList()
        {
            var projectMasters = _dbeEntities.ProjectMasters.Where(x => x.IsProjectManagerAssigned != true && x.ProjectStatus == "APPROVED" && (_dbeEntities.ProjectPurchaseOrderForms.Any(i => i.ProjectMasterId == x.ProjectMasterId))).ToList();
            //var projectMasters = _dbeEntities.ProjectMasters.Where(x => x.IsProjectManagerAssigned != true && x.ProjectStatus == "APPROVED").ToList();

            //var projectMasters = (from pmaster in _dbeEntities.ProjectMasters
            //    join projectPurchaseOrderForm in _dbeEntities.ProjectPurchaseOrderForms on pmaster.ProjectMasterId
            //        equals projectPurchaseOrderForm.ProjectMasterId
            //    where pmaster.IsProjectManagerAssigned != true && pmaster.ProjectStatus == "APPROVED"
            //    select new{pmaster,projectPurchaseOrderForm.PurchaseOrderNumber} ).ToList();






            Mapper.Initialize(cfg => cfg.CreateMap<ProjectMaster, ProjectMasterModel>());
            List<ProjectMasterModel> listDest = Mapper.Map<List<ProjectMaster>, List<ProjectMasterModel>>(projectMasters);
            return listDest;
        }


        public List<ProjectMasterModel> GetNewProjectsList()
        {

            //            var projectMasters =
            //                _dbeEntities.Database.SqlQuery<ProjectMasterModel>(
            //                    @"select pm.ProjectMasterId,pm.ProjectName,pm.SupplierName,pm.ProjectType,pm.OsName,pm.OsVersion,pm.ApproxProjectFinishDate,pm.ApproxShipmentDate as LSD,po.PurchaseOrderNumber,pm.OrderNuber,
            //                    pm.NumberOfSample,pm.Chipset,pos.FlightDepartureDate as ShipmentTaken,po.PurchaseOrderNumber,po.PoCategory,po.PoDate from ProjectMasters pm  
            //                    left join  ProjectPurchaseOrderForms  po
            //                    on  pm.ProjectMasterId =po.ProjectMasterId
            //                    left join ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId  and pos.FlightDepartureDate in 
            //                    (select top 1 FlightDepartureDate from ProjectOrderShipments where pm.ProjectMasterId=ProjectMasterId order by FlightDepartureDate desc)
            //                    where (pm.IsProjectManagerAssigned != 1 or pm.IsProjectManagerAssigned is null) and  pm.ProjectStatus = 'APPROVED'
            //                    group by pm.ProjectMasterId,pm.ProjectName,pm.SupplierName,pm.ProjectType,pm.OsName,pm.OsVersion,pm.ApproxProjectFinishDate,pm.ApproxShipmentDate,po.PurchaseOrderNumber,pm.OrderNuber,
            //                    pm.NumberOfSample,pm.Chipset,pos.FlightDepartureDate,po.PurchaseOrderNumber,po.PoCategory,po.PoDate
            //                     order by pm.ProjectName,pm.ProjectMasterId").ToList();

            var projectMasters =
       _dbeEntities.Database.SqlQuery<ProjectMasterModel>(
           @"select pm.ProjectMasterId,pm.ProjectName,case when po.Quantity is null then 0 else po.Quantity end as OrderQuantities,pm.SourcingType,
            (select top 1 cu1.UserFullName from CellPhoneProject.dbo.ProjectMasters pm1
            left join  [CellPhoneProject].[dbo].[ProjectPmAssigns] ppa1 on pm1.ProjectMasterId=ppa1.ProjectMasterId  
            left join  [CellPhoneProject].[dbo].CmnUsers cu1 on cu1.CmnUserId=ppa1.ProjectManagerUserId      
            where pm1.ProjectName=pm.ProjectName and ppa1.ProjectManagerUserId is not null order by pm1.ProjectMasterId desc) as LastOrderPmName,

            (select top 1 ppa1.AssignDate from CellPhoneProject.dbo.ProjectMasters pm1
            left join  [CellPhoneProject].[dbo].[ProjectPmAssigns] ppa1 on pm1.ProjectMasterId=ppa1.ProjectMasterId  
            left join  [CellPhoneProject].[dbo].CmnUsers cu1 on cu1.CmnUserId=ppa1.ProjectManagerUserId      
            where pm1.ProjectName=pm.ProjectName and ppa1.ProjectManagerUserId is not null order by pm1.ProjectMasterId desc) as LastAssignDate,
            pm.SupplierName,pm.ProjectType,pm.OsName,pm.OsVersion,pm.ApproxProjectFinishDate,pm.ApproxShipmentDate as LSD,po.PurchaseOrderNumber,pm.OrderNuber,
            pm.NumberOfSample,pm.Chipset,pos.FlightDepartureDate as ShipmentTaken,po.PurchaseOrderNumber,po.PoCategory,po.PoDate 

            from CellPhoneProject.dbo.ProjectMasters pm 
            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on pm.ProjectMasterId =po.ProjectMasterId
            left join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId  
            and pos.FlightDepartureDate in (select top 1 pos1.FlightDepartureDate from CellPhoneProject.dbo.ProjectOrderShipments pos1 where pm.ProjectMasterId=pos1.ProjectMasterId order by pos1.FlightDepartureDate desc)
            left join CellPhoneProject.dbo.[ProjectOrderQuantityDetails] pod on pod.ProjectMasterId=pm.ProjectMasterId  

            where (pm.IsProjectManagerAssigned != 1 or pm.IsProjectManagerAssigned is null) and pm.ProjectStatus = 'APPROVED' and pm.IsActive=1
            group by pm.ProjectMasterId,pm.ProjectName,pm.SupplierName,pm.ProjectType,pm.OsName,pm.OsVersion,pm.ApproxProjectFinishDate,pm.ApproxShipmentDate,po.PurchaseOrderNumber,pm.OrderNuber,
            pm.NumberOfSample,pm.Chipset,pos.FlightDepartureDate,po.PurchaseOrderNumber,po.PoCategory,po.PoDate,pod.OrderQuantity,po.Quantity,pm.SourcingType
            order by pm.ProjectName,pm.ProjectMasterId").ToList();

            foreach (var project in projectMasters)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }
            }
            return projectMasters;
        }

        public List<PmCmnUserModel> GetPmCmnUsers()
        {
            var listOfPm = _dbeEntities.Database.SqlQuery<PmCmnUserModel>(@"select * from CmnUsers where RoleName in ('PM','PMHEAD') and IsActive=1 ").ToList();
            return listOfPm;
        }

        public string AssignProjectToProjectManager(long pMasterId, long pManagerId, string projectHeadRemarks, string purchaseOrderNumber  //, string pmAproDate
            )
        {
            String userIdentity =
           HttpContext.Current.User.Identity.Name;
            ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            //DateTime date;
            //DateTime.TryParseExact(pmAproDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            //    DateTimeStyles.None, out date);

            //DateTime ApproxPmInchargeToPmFinishDate1 = date;
            var pmAssign = new ProjectPmAssign
            {
                ProjectMasterId = pMasterId,
                PONumber = purchaseOrderNumber,
                AssignUserId = userId,
                ProjectManagerUserId = pManagerId,
                ProjectHeadRemarks = projectHeadRemarks,
                Status = "ASSIGNED",
                AssignDate = DateTime.Now
                // ApproxPmInchargeToPmFinishDate = ApproxPmInchargeToPmFinishDate1
            };

            _dbeEntities.ProjectPmAssigns.Add(pmAssign);
            string returnValue = "1";
            try
            {

                var updateProjectMster =
                    _dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectMasterId == pMasterId);
                if (updateProjectMster == null)
                {
                    throw new Exception("Master object not found");

                }
                updateProjectMster.IsProjectManagerAssigned = true;
                //_dbeEntities.ProjectMasters.AddOrUpdate(updateProjectMster);
                _dbeEntities.Entry(updateProjectMster).State = EntityState.Modified;
                _dbeEntities.SaveChanges();
            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }

            return returnValue;
        }

        public void InsertDataInBabt(long pMasterId, long pManagerId)
        {
            String userIdentity =
           HttpContext.Current.User.Identity.Name;
            ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            var model = new ProjectBabtModel();
            var poId =
                _dbeEntities.ProjectPurchaseOrderForms.Where(x => x.ProjectMasterId == pMasterId)
                    .Select(x => x.ProjectPurchaseOrderFormId)
                    .FirstOrDefault();
            if (poId != 0)
            {
                model.ProjectMasterId = pMasterId;
                model.ProjectPurchaseOrderFormId = poId;
                model.PmAssignId = pManagerId;
                model.PmImeiRangeRequestDate = DateTime.Now;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                var config = new MapperConfiguration(c => c.CreateMap<ProjectBabtModel, ProjcetBabt>());
                var mapper = config.CreateMapper();
                var babt = mapper.Map<ProjcetBabt>(model);
                _dbeEntities.ProjcetBabts.Add(babt);
                _dbeEntities.SaveChanges();
            }
        }

        public List<ProjectMasterModel> GetAssignedProjectList()
        {

            var getAssignedProject =
            _dbeEntities.Database.SqlQuery<ProjectMasterModel>(
             @"select cmm.UserFullName,pm.ProjectMasterId,pm.ProjectName,po.PoDate,po.PurchaseOrderNumber,
                ppa.AssignDate,ppa.ProjectManagerUserId,ppa.Status,pos.FlightDepartureDate as ShipmentTaken,pm.SupplierName,pm.ProjectType,pm.OsName,pm.OsVersion,pm.ApproxProjectOrderDate,
                pm.NumberOfSample,pm.SupplierTrustLevel,pm.Chipset,pm.ApproxShipmentDate as LSD,pm.OrderNuber
                from ProjectMasters pm 
                left join ProjectPurchaseOrderForms  po on  pm.ProjectMasterId =po.ProjectMasterId
                left join ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId 
			                 and pos.FlightDepartureDate in (select top 1 FlightDepartureDate from ProjectOrderShipments where pm.ProjectMasterId=ProjectMasterId order by FlightDepartureDate desc)
                left join ProjectPmAssigns ppa on pm.ProjectMasterId =ppa.ProjectMasterId
                left join CellPhoneProject.dbo.CmnUsers cmm on cmm.CmnUserId=ppa.ProjectManagerUserId

                where pm.IsProjectManagerAssigned = 1 and po.PurchaseOrderNumber is not null and ppa.Status in ('ASSIGNED') and ppa.ProjectManagerUserId > 0 
                group by pm.ProjectMasterId,pm.ProjectName,pm.SupplierName,pm.ProjectType,pm.OsName,pm.OsVersion,pm.ApproxProjectOrderDate,pm.NumberOfSample,pm.SupplierTrustLevel,pm.Chipset,pm.ApproxShipmentDate,
                ppa.AssignDate,ppa.ProjectManagerUserId,ppa.Status,pos.FlightDepartureDate,po.PoDate,po.PurchaseOrderNumber,pm.OrderNuber,cmm.UserFullName
                order by pm.ProjectMasterId").ToList();

            foreach (var project in getAssignedProject)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            //Mapper.Initialize(cfg => cfg.CreateMap<ProjectMaster, ProjectMasterModel>());
            //List<ProjectMasterModel> assignedList = Mapper.Map<List<ProjectMaster>, List<ProjectMasterModel>>(getAssignedProject);
            return getAssignedProject;
        }

        public ProjectPmAssignModel GetPmAssignInfo(long currentMasterId)
        {


            var getAssigendPm =
                _dbeEntities.ProjectPmAssigns.FirstOrDefault(x => x.ProjectMasterId == currentMasterId && x.Status == "ASSIGNED");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectPmAssign, ProjectPmAssignModel>());
            var mapper = config.CreateMapper();
            var assignedPm = mapper.Map<ProjectPmAssignModel>(getAssigendPm);
            return assignedPm;
        }

        public PmViewHwTestHybridModel GetPmViewHwTestHybridModelForScreening(long projectMasterId)
        {
            string query =
                string.Format(
                    @"select  pm.ProjectMasterId,HQIA.HwQcInchargeAssignId, pm.ProjectName,pm.SupplierName,pm.ProjectType, htpa.Chipset_Vendor
                                         ,htpa.Chipset_Speed,htpa.Chipset_Core,
                                         htpa.FlashIC_ROM,htpa.FlashIC_RAM,htci.FrontCamera_MPSW,
                                         htci.BackCamera_MPSW,hqia.Remark from projectmasters pm 
                                         inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwTestPcbAs htpa on hqia.HwQcInchargeAssignId=htpa.HwQcInchargeAssignId
                                         left join HwTestCameraInfos htci on hqia.HwQcInchargeAssignId=htci.HwQcInchargeAssignId
                                         where pm.ProjectMasterId={0} and hqia.isscreeningtest={1} and hqia.TestPhase='{2}'
										 order by hqia.AddedDate desc", projectMasterId, 1, "FINISHED");
            PmViewHwTestHybridModel getPmViewHwTestHybridModel =
                _dbeEntities.Database.SqlQuery<PmViewHwTestHybridModel>(query).FirstOrDefault();
            return getPmViewHwTestHybridModel;
        }

        public List<PmViewHwTestHybridModel> GetPmViewHwTestHybridModelForRunning(long projectMasterId)
        {
            string query =
                string.Format(
                    @"select  pm.ProjectMasterId,HQIA.HwQcInchargeAssignId, pm.ProjectName,pm.SupplierName,pm.ProjectType, htpa.Chipset_Vendor
                                         ,htpa.Chipset_Speed,htpa.Chipset_Core,
                                         htpa.FlashIC_ROM,htpa.FlashIC_RAM,htci.FrontCamera_MPSW,
                                         htci.BackCamera_MPSW,hqia.Remark from projectmasters pm 
                                         inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwTestPcbAs htpa on hqia.HwQcInchargeAssignId=htpa.HwQcInchargeAssignId
                                         left join HwTestCameraInfos htci on hqia.HwQcInchargeAssignId=htci.HwQcInchargeAssignId
                                         where pm.ProjectMasterId={0} and hqia.isrunningtest={1} and hqia.TestPhase='{2}'
										 order by hqia.AddedDate desc", projectMasterId, 1, "FINISHED");
            List<PmViewHwTestHybridModel> getPmViewHwTestHybridModel =
                _dbeEntities.Database.SqlQuery<PmViewHwTestHybridModel>(query).ToList();
            return getPmViewHwTestHybridModel;
        }

        public List<PmViewHwTestHybridModel> GetPmViewHwTestHybridModelForFinished(long projectMasterId)
        {
            string query =
                string.Format(
                    @"select pm.ProjectMasterId,hqa.HwQcInchargeAssignId, pm.ProjectName,pm.SupplierName,pm.ProjectType, htpa.Chipset_Vendor
                                         ,htpa.Chipset_Speed,htpa.Chipset_Core,
                                         htpa.FlashIC_ROM,htpa.FlashIC_RAM,htci.FrontCamera_MPSW,
                                         htci.BackCamera_MPSW,hqia.Remark from projectmasters pm 
                                         inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                         left join HwTestPcbAs htpa on hqa.HwQcAssignId=htpa.HwQcAssignId
                                         left join HwTestCameraInfos htci on hqa.HwQcAssignId=htci.HwQcAssignId
                                         where pm.ProjectMasterId={0} and hqia.isfinishedgoodtest={1} and hqa.Status='{2}'", projectMasterId, 1, "FORWARDED");
            List<PmViewHwTestHybridModel> getPmViewHwTestHybridModel =
                _dbeEntities.Database.SqlQuery<PmViewHwTestHybridModel>(query).ToList();
            return getPmViewHwTestHybridModel;
        }


        public PmCmnUserModel GetPmUserInfo(long pmUserId)
        {
            var pmUserInfo = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == pmUserId && x.IsActive == true).Select(j => new PmCmnUserModel
            {
                CmnUserId = j.CmnUserId,
                UserFullName = j.UserFullName,
                UserName = j.UserName
            }).FirstOrDefault();
            if (pmUserInfo == null) return new PmCmnUserModel
            {
                CmnUserId = 0,
                UserFullName = "",
                UserName = ""
            };
            return pmUserInfo;
        }

        public List<ProjectMasterModel> GetProjectMasterModelsByProjectManager(long pmUserId)
        {
            var masterModels = new List<ProjectMasterModel>();
            var pmProjectAssignedList = _dbeEntities.ProjectPmAssigns.Where(x => x.ProjectManagerUserId == pmUserId && x.Status == "ASSIGNED").Select(j => j.ProjectMasterId).ToList();

            if (pmProjectAssignedList.Any())
            {
                foreach (long l in pmProjectAssignedList)
                {
                    var master =
                        _dbeEntities.ProjectMasters.Where(i => i.ProjectMasterId == l && i.IsActive == true)
                            .Select(j => new ProjectMasterModel
                            {
                                ProjectMasterId = j.ProjectMasterId,
                                ProjectName = j.ProjectName,
                                ProjectType = j.ProjectType,
                                ProjectStatus = j.ProjectStatus,
                                ApproxProjectOrderDate = j.ApproxProjectOrderDate,
                                ApproxShipmentDate = j.ApproxShipmentDate,
                                SupplierName = j.SupplierName,
                                Chipset = j.Chipset,
                                OrderNuber = j.OrderNuber


                            }).FirstOrDefault();
                    if (master != null)
                    {
                        masterModels.Add(master);
                        var poCats =
                        _dbeEntities.ProjectPurchaseOrderForms.Where(i => i.ProjectMasterId == master.ProjectMasterId)
                            .Select(j => new ProjectPurchaseOrderFormModel
                            {
                                PoCategory = j.PoCategory
                            }).FirstOrDefault();

                        master.OrderNumberOrdinal = master.OrderNuber != null
                          ? CommonConversion.AddOrdinal((int)master.OrderNuber) + " Order"
                          : string.Empty;
                        if (!string.IsNullOrWhiteSpace(master.OrderNumberOrdinal))
                        {
                            if (poCats != null)
                                master.ProjectName = master.ProjectName + " (" + master.OrderNumberOrdinal + ")" + " (" + poCats.PoCategory + ")";
                        }
                    }
                }

            }

            return masterModels;

        }

        public List<ProjectMasterModel> GetAssignedProjectMasterInfo(long projectMasterId)
        {
            var assignedProjectMasterInfo =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == projectMasterId).Select(o => new ProjectMasterModel
                {
                    ProjectMasterId = o.ProjectMasterId,
                    ProjectName = o.ProjectName,
                }).ToList();

            return assignedProjectMasterInfo;
        }

        public PmBootImageAnimationModel GetPmBootImageAnimationModel(long projectMasterId, long userId)
        {


            var pmBootImgAnimationOfAProject =
                _dbeEntities.PmBootImageAnimations.Where(x => x.ProjectMasterId == projectMasterId && x.Added == userId).Select(y => new PmBootImageAnimationModel
                {
                    ImageUpload1 = y.ImageUpload1,
                    VideoUpload1 = y.VideoUpload1,
                    ProjectAssignId = y.ProjectAssignId,
                    PmBootImageAnimationId = y.PmBootImageAnimationId,
                    ProjectMasterId = y.ProjectMasterId,
                    Remarks = y.Remarks,
                    Added = y.Added,
                    AddedDate = y.AddedDate,

                }).FirstOrDefault();

            return pmBootImgAnimationOfAProject;
        }

        public SwQcInchargeAssignModel CheckSwQcInchargeDuplicateAssign(long projectMasterId)
        {
            string query = string.Format(@"select top 1 Status from SwQcInchargeAssigns where ProjectMasterId={0} order by AddedDate desc", projectMasterId);
            var exe = _dbeEntities.Database.SqlQuery<SwQcInchargeAssignModel>(query).FirstOrDefault();
            return exe;
        }

        public string AssignProjectPmToSwQcHead(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, string selectedSampleValue, long sampleNo, long userId, long swWcInchargeAssignUserId, long testPhasefrPm, long swVersionNumber, string versionName)
        {
            var roleName = _dbeEntities.Database.SqlQuery<CmnUserModel>(@"select RoleName from [CellPhoneProject].[dbo].[CmnUsers]
                where CmnUserId={0} ", userId).FirstOrDefault();
            var query = (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == pMasterId select pm).FirstOrDefault();

            SwQcHeadAssignsFromPm swQcInchargeAssign = new SwQcHeadAssignsFromPm();
            swQcInchargeAssign.ProjectMasterId = pMasterId;
            swQcInchargeAssign.ProjectName = query.ProjectName;
            swQcInchargeAssign.ProjectType = query.ProjectType;
            swQcInchargeAssign.OrderNumber = query.OrderNuber;
            swQcInchargeAssign.ProjectPmAssignId = pMAssignId;
            swQcInchargeAssign.ProjectOrderShipmentId = 1;
            swQcInchargeAssign.ProjectManagerUserId = userId;
            swQcInchargeAssign.ProjectManagerSampleNo = Convert.ToInt32(sampleNo);
            swQcInchargeAssign.ProjectManagerSampleType = selectedSampleValue;
            swQcInchargeAssign.SoftwareVersionName = versionName;
            swQcInchargeAssign.SoftwareVersionNo = Convert.ToInt32(swVersionNumber);
            swQcInchargeAssign.PriorityFromPm = "HIGH";
            if (roleName.RoleName == "ASPMHEAD" || roleName.RoleName == "ASPM")
            {
                swQcInchargeAssign.AssignCatagory = roleName.RoleName;
            }
            else
            {
                swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
            }
            swQcInchargeAssign.Status = "NEW";
            swQcInchargeAssign.PmToQcHeadAssignTime = DateTime.Now;
            swQcInchargeAssign.SwQcHeadUserId = swWcInchargeAssignUserId;
            swQcInchargeAssign.PmToQcHeadAssignComment = pmRemarks;
            swQcInchargeAssign.TestPhaseID = testPhasefrPm;
            swQcInchargeAssign.IsFinalPhaseMP = false;
            swQcInchargeAssign.Added = userId;
            swQcInchargeAssign.AddedDate = DateTime.Now;

            _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(swQcInchargeAssign);
            _dbeEntities.SaveChanges();

            if (swVersionNumber != 0)
            {
                if (testPhasefrPm == 5)
                {
                    List<SwQcIssueDetail> swQcHead = (from swQcHeads in _dbeEntities.SwQcIssueDetails
                                                      where
                                                          swQcHeads.SoftwareVersionNo == swVersionNumber && swQcHeads.ProjectName == query.ProjectName &&
                                                          swQcHeads.TestPhaseID == testPhasefrPm && swQcHeads.Demo == "Demo"
                                                      select swQcHeads).ToList();

                    foreach (var swQcIssueDetail in swQcHead)
                    {
                        swQcIssueDetail.SoftwareVersionName = versionName;

                        _dbeEntities.SwQcIssueDetails.AddOrUpdate(swQcIssueDetail);
                        _dbeEntities.SaveChanges();
                    }
                }
                else
                {
                    List<SwQcIssueDetail> swQcHead = (from swQcHeads in _dbeEntities.SwQcIssueDetails
                                                      where
                                                          swQcHeads.SoftwareVersionNo == swVersionNumber && swQcHeads.ProjectName == query.ProjectName &&
                                                          swQcHeads.TestPhaseID != 5 && swQcHeads.TestPhaseID != 10
                                                      select swQcHeads).ToList();

                    foreach (var swQcIssueDetail in swQcHead)
                    {
                        swQcIssueDetail.SoftwareVersionName = versionName;

                        _dbeEntities.SwQcIssueDetails.AddOrUpdate(swQcIssueDetail);
                        _dbeEntities.SaveChanges();
                    }
                }


            }

            return "ok";
        }
        public string CheckDuplicateAssignToHardware(long pMasterId, string runningTestValue, string finisTestValue)
        {
            string testInProgress = "0";
            string checkIsRunningTestInProgressQuery = string.Format(@"select count(*) as flag from hwqcinchargeassigns where ProjectMasterId={0} and IsRunningTest=1 and TestPhase not in('FINISHED')", pMasterId);
            var checkIsRunningTestInProgress = _dbeEntities.Database.SqlQuery<int>(checkIsRunningTestInProgressQuery).First();
            string checkIsFinishedTestInProgressQuery = string.Format(@"select count(*) as flag from hwqcinchargeassigns where ProjectMasterId={0} and IsFinishedGoodTest=1 and TestPhase not in('FINISHED')", pMasterId);
            var checkIsFinishedTestInProgress = _dbeEntities.Database.SqlQuery<int>(checkIsFinishedTestInProgressQuery).First();
            string checkRunningTestDoneQuery = string.Format(@"select count(*) as flag from hwqcinchargeassigns where ProjectMasterId={0} and IsRunningTest=1 and TestPhase in('FINISHED')", pMasterId);
            var checkRunningTestDone = _dbeEntities.Database.SqlQuery<int>(checkRunningTestDoneQuery).First();
            //======allow skipping running test from 2nd order=====
            var orderNumber =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == pMasterId)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault();
            if (orderNumber > 1)
            {
                checkRunningTestDone = 1;
            }
            //=====================================================
            if (runningTestValue == "1")
            {
                if (checkIsRunningTestInProgress > 0)
                {
                    testInProgress = "Another Running Test Already in Progress";
                }
                else if (checkIsFinishedTestInProgress > 0)
                {
                    testInProgress = "Finished Test in Progress";
                }
            }
            else if (finisTestValue == "1")
            {
                if (checkRunningTestDone > 0)
                {
                    if (checkIsRunningTestInProgress == 0)
                    {
                        if (checkIsFinishedTestInProgress > 0)
                        {
                            testInProgress = "Another Finished Test Already in Progress";
                        }
                    }
                    else
                    {
                        testInProgress = "Running Test in Progress";
                    }
                }
                else
                {
                    testInProgress = "Please Assign/Complete Running Test First";
                }
            }
            return testInProgress;
        }

        public string AssignProjectToHardWare(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId,
            string selectedSampleValue, long sampleNo, long userId, string runningTestValue, string finisTestValue, string poNumber)
        {
            Boolean runnigTest = false, finishTest = false;



            if (runningTestValue == "1")
            {
                runnigTest = true;
            }
            else if (finisTestValue == "1")
            {
                finishTest = true;
            }
            var hwQcInchargeAssign = new HwQcInchargeAssign
            {
                ProjectMasterId = pMasterId,
                ProjectPmAssignId = pMAssignId,
                HwQcInchargeAssignedBy = pMAssignId,
                ProjectManagerUserId = pmUserId,
                SentSampleQuantity = Convert.ToInt32(sampleNo),
                ProjectManagerSampleType = selectedSampleValue,
                PriorityFromPm = "HIGH",
                ApproxPmToHwDeliveryDate = DateTime.Now,
                ProjectManagerAssignComment = pmRemarks,
                IsScreeningTest = false,
                IsRunningTest = runnigTest,
                IsFinishedGoodTest = finishTest,
                Status = "NEW",
                TestPhase = "SAMPLESENT",
                SampleSetSentDate = DateTime.Now,
                HwQcInchargeAssignDate = DateTime.Now,
                Added = userId,
                AddedDate = DateTime.Now,
                Updated = userId,
                UpdatedDate = DateTime.Now

            };
            _dbeEntities.HwQcInchargeAssigns.AddOrUpdate(hwQcInchargeAssign);
            _dbeEntities.SaveChanges();

            return "Project Forwarded to Hardware";
        }

        public List<SwQcInchargeAssignModel> GetSwQcInchargeAssign(long projectId)
        {
            var getSwQcInchargeAssign =
                _dbeEntities.SwQcInchargeAssigns.Where(x => x.ProjectMasterId == projectId && x.Status == "RECOMMENDED").ToList();

            if (getSwQcInchargeAssign != null && getSwQcInchargeAssign.Count > 0)
            {

                Mapper.Initialize(cfg => cfg.CreateMap<SwQcInchargeAssign, SwQcInchargeAssignModel>());
                List<SwQcInchargeAssignModel> inchargeAssign = Mapper.Map<List<SwQcInchargeAssign>, List<SwQcInchargeAssignModel>>(getSwQcInchargeAssign);
                return inchargeAssign;
            }
            return new List<SwQcInchargeAssignModel>();
        }
        public Tuple<List<PmSwCustomizationFinalModel>, bool> GetSoftwareCustomizationDataList(long projectId)
        {
            bool isUpdateable = true;

            String userIdentity =
           HttpContext.Current.User.Identity.Name;
            ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            List<PmSwCustomizationFinalModel> models = _dbeEntities.PmSwCustomizationFinals.OrderByDescending(i => i.PmSwCustomizationFinalId)
                .Where(i => i.ProjectMasterId == projectId).Select(final => new PmSwCustomizationFinalModel
                {
                    ProjectMasterId = final.ProjectMasterId,
                    PmSwCustomizationFinalId = final.PmSwCustomizationFinalId,
                    PmSwCustomizationFinalMenu = final.PmSwCustomizationFinalMenu,
                    PmSwCustomizationFinalPath = final.PmSwCustomizationFinalPath,
                    PmSwCustomizationFinalSettings = final.PmSwCustomizationFinalSettings,
                    ProjectPmAssignId = final.ProjectPmAssignId,
                    Added = final.Added,
                    AddedDate = final.AddedDate

                }).ToList();
            if (!models.Any())
            {
                isUpdateable = false;
                models = new List<PmSwCustomizationFinalModel>();
                var initials = GetPmSwCustomizationInitialModels(projectId);
                foreach (var initial in initials)
                {

                    var model = new PmSwCustomizationFinalModel
                    {
                        ProjectMasterId = projectId,
                        ProjectPmAssignId = initial.ProjectPmAssignId,
                        AssignUserId = initial.AssignUserId,
                        PmSwCustomizationFinalMenu = initial.PmSwCustomizationMenu,
                        PmSwCustomizationFinalSettings = initial.PmSwCustomizationDefaultSetting,
                        PmSwCustomizationFinalPath = initial.PmSwCustomizationPath,
                        Added = userId,
                        AddedDate = DateTime.Now
                    };
                    models.Add(model);
                }
            }
            return new Tuple<List<PmSwCustomizationFinalModel>, bool>(models, isUpdateable);
        }

        public string SaveBtrcDocFiles(IEnumerable<VmPmToBtrcNocRequest> attachments)
        {
            try
            {
                var vmPmToBtrcNocRequests = attachments as VmPmToBtrcNocRequest[] ?? attachments.ToArray();
                if (vmPmToBtrcNocRequests.Any())
                {
                    var manager = new FileManager();
                    long userId;
                    long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                    var projectMasterId = vmPmToBtrcNocRequests[0].ProjectMasterId;
                    var smapleImei = vmPmToBtrcNocRequests[0].ProjectBtrcNocModel.FinalSampleImei;
                    long nocId = vmPmToBtrcNocRequests[0].ProjectBtrcNocModel.ProjectBrtcNocId;
                    long orderFormId = vmPmToBtrcNocRequests[0].ProjectBtrcNocModel.ProjectPurchaseOrderFormId;
                    const string userDirectory = "BTRC";
                    const string moduleDirectory = "NOC";
                    if (nocId > 0)
                    {
                        if (orderFormId <= 0) throw new Exception();
                        var dbProjectBrtcNoc = _dbeEntities.ProjectBtrcNocs.FirstOrDefault(i => i.ProjectBtrcNocId == nocId);
                        if (dbProjectBrtcNoc != null)
                        {
                            dbProjectBrtcNoc.FinalSampleImei = smapleImei;
                            dbProjectBrtcNoc.ProjectPurchaseOrderFormId = orderFormId;
                            dbProjectBrtcNoc.Updated = userId;
                            dbProjectBrtcNoc.UpdatedDate = DateTime.Now;
                            _dbeEntities.Entry(dbProjectBrtcNoc).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        if (orderFormId <= 0) throw new Exception();
                        var brtcNoc = new ProjectBtrcNoc
                        {
                            ProjectMasterId = projectMasterId,
                            ProjectAssignId = userId,
                            FinalSampleImei = smapleImei,
                            ProjectPurchaseOrderFormId = orderFormId,
                            IsDocUploaded = true
                        };
                        _dbeEntities.ProjectBtrcNocs.Add(brtcNoc);
                        _dbeEntities.SaveChanges();
                        nocId = brtcNoc.ProjectBtrcNocId;

                    }
                    foreach (var files in vmPmToBtrcNocRequests)
                    {
                        var uploadedFilePath = manager.Upload(projectMasterId, userDirectory, moduleDirectory, files.ProjectBtrcNocDocuments.btrcFiles);
                        var projectbtrcNocDoc = new ProjectBtrcNocDocument
                        {
                            ProjectMasterId = projectMasterId,
                            FilePath = uploadedFilePath,
                            AddedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            Added = userId,
                            Updated = userId,
                            ProjectBtrcNocId = nocId
                        };
                        var config = new MapperConfiguration(c => c.CreateMap<ProjectBtrcNocDocumentModel, ProjectBtrcNocDocument>());
                        var mapper = config.CreateMapper();
                        var btrcNOc = mapper.Map<ProjectBtrcNocDocument>(projectbtrcNocDoc);
                        _dbeEntities.ProjectBtrcNocDocuments.Add(btrcNOc);
                        _dbeEntities.SaveChanges();

                    }
                    return projectMasterId + "-" + orderFormId + "-" + smapleImei;
                }
            }
            catch (Exception)
            {
                return "no";
            }
            return "no";
        }


        public long GetUserIdByRoleName(string roleName)
        {
            long userId = 0;
            if (roleName != null)
            {
                userId = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == roleName && i.IsActive == true).CmnUserId;
            }
            return userId;
        }

        public List<FileShowModel> GetFilesServerPaths(long nocId)
        {
            List<FileShowModel> fileShowModels = new List<FileShowModel>();
            FileManager fileManager = new FileManager();
            var docs = _dbeEntities.ProjectBtrcNocDocuments.Where(i => i.ProjectBtrcNocId == nocId).ToList();
            if (docs.Any())
            {
                fileShowModels.AddRange(docs.Select(doc => new FileShowModel
                {
                    FileName = fileManager.GetFileName(doc.FilePath),
                    Path = fileManager.GetFile(doc.FilePath)
                }));
            }
            return fileShowModels;
        }

        public ProjectBtrcNocModel GetProjectBtrcNoc(long projectId = 0, long orderId = 0, string imei = null)
        {
            ProjectBtrcNoc brtcNoc = new ProjectBtrcNoc();
            if (projectId > 0 && orderId > 0)
            {
                brtcNoc = GenereticRepo<ProjectBtrcNoc>.Get(_dbeEntities,
                    noc => noc.ProjectMasterId == projectId && noc.ProjectPurchaseOrderFormId == orderId && noc.FinalSampleImei == imei);
            }
            //else if (projectId > 0)
            //{
            //    brtcNoc = GenereticRepo<ProjectBrtcNoc>.Get(_dbeEntities, noc => noc.ProjectMasterId == projectId);
            //}
            ProjectBtrcNocModel model = brtcNoc == null ? new ProjectBtrcNocModel() : GenericMapper<ProjectBtrcNoc, ProjectBtrcNocModel>.GetDestination(brtcNoc);
            return model;
        }

        public List<ProjectBtrcNocModel> GetBtrcNocByProjectId(long projectId)
        {
            var nocList = (from noc in _dbeEntities.ProjectBtrcNocs
                           join orderForm in _dbeEntities.ProjectPurchaseOrderForms on noc.ProjectPurchaseOrderFormId equals
                               orderForm.ProjectPurchaseOrderFormId
                           join master in _dbeEntities.ProjectMasters on noc.ProjectMasterId equals master.ProjectMasterId
                           where noc.ProjectMasterId == projectId
                           select new ProjectBtrcNocModel
                           {
                               ProjectBrtcNocId = noc.ProjectBtrcNocId,
                               ProjectMasterId = noc.ProjectMasterId,
                               ProjectPurchaseOrderFormId = noc.ProjectPurchaseOrderFormId,
                               ProjectAssignId = noc.ProjectAssignId,
                               PoNo = orderForm.PurchaseOrderNumber,
                               ProjectName = master.ProjectName,
                               FinalSampleImei = noc.FinalSampleImei
                           }).ToList();

            return nocList;
        }

        public bool UpdateSoftwareCustomization(VmSoftwareCustomization model)
        {
            try
            {

                String userIdentity =
          HttpContext.Current.User.Identity.Name;
                ;
                long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

                if (model.PmSwCustomizationFinalModels.Any())
                {
                    foreach (var finalModel in model.PmSwCustomizationFinalModels)
                    {
                        var dbData =
                            _dbeEntities.PmSwCustomizationFinals.FirstOrDefault(
                                i => i.PmSwCustomizationFinalId == finalModel.PmSwCustomizationFinalId);
                        if (dbData != null)
                        {
                            //dbData.ProjectMasterId = finalModel.ProjectMasterId;
                            dbData.PmSwCustomizationFinalMenu = finalModel.PmSwCustomizationFinalMenu;
                            dbData.PmSwCustomizationFinalSettings = finalModel.PmSwCustomizationFinalSettings;
                            dbData.PmSwCustomizationFinalPath = finalModel.PmSwCustomizationFinalPath;
                            dbData.Updated = userId;
                            dbData.UpdatedDate = DateTime.Now;
                            _dbeEntities.Entry(dbData).State = EntityState.Modified;
                        }
                    }

                    if (model.Others.Any())
                    {
                        foreach (var other in model.Others)
                        {
                            var final = new PmSwCustomizationFinal
                            {
                                ProjectMasterId = other.ProjectMasterId,
                                ProjectPmAssignId = model.PmSwCustomizationFinalModels[0].ProjectPmAssignId,
                                PmSwCustomizationFinalMenu = other.PmSwCustomizationFinalMenu,
                                PmSwCustomizationFinalPath = other.PmSwCustomizationFinalPath,
                                PmSwCustomizationFinalSettings = other.PmSwCustomizationFinalSettings,
                                Added = userId,
                                AddedDate = DateTime.Now
                            };
                            _dbeEntities.PmSwCustomizationFinals.Add(final);
                        }
                    }
                    _dbeEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PmPhnCameraModel GetCameraModel(long projectId, long userId)
        {
            PmPhnCamera camera = GenereticRepo<PmPhnCamera>.Get(_dbeEntities,
                phnCamera => phnCamera.ProjectMasterId == projectId && phnCamera.Added == userId);
            PmPhnCameraModel model = GenericMapper<PmPhnCamera, PmPhnCameraModel>.GetDestination(camera) ?? new PmPhnCameraModel();
            return model;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectForHwFgTestByProjectId(long projectMasterId)
        {
            string query =
                string.Format(@"select distinct  hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,
                                STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  
                                where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                hqa.VerifierName,hqia.Remark from HwQcInchargeAssigns hqia
                                inner join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId = hqa.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId 
                                where hqia.IsFinishedGoodTest=1 and pm.ProjectMasterId={0} and hqia.TestPhase='FINISHED'", projectMasterId);
            var getProjectForHwFgTestByProjectId =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getProjectForHwFgTestByProjectId;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectForHwScreeningTestByProjectId(long projectMasterId)
        {
            string query =
                string.Format(@"select distinct  hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,
                                STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  
                                where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                hqa.VerifierName,hqia.Remark from HwQcInchargeAssigns hqia
                                inner join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId = hqa.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId 
                                where hqia.IsScreeningTest=1 and pm.ProjectMasterId={0} and hqia.TestPhase='FINISHED'", projectMasterId);
            var getProjectForHwScreeningTestByProjectId =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getProjectForHwScreeningTestByProjectId;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectForHwRunningTestByProjectId(long projectMasterId)
        {
            string query =
                string.Format(@"select distinct  hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,
                                STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  
                                where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                hqa.VerifierName,hqia.Remark from HwQcInchargeAssigns hqia
                                inner join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId = hqa.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId 
                                where hqia.IsRunningTest=1 and pm.ProjectMasterId={0} and hqia.TestPhase='FINISHED'", projectMasterId);
            var getProjectForHwRunningTestByProjectId =
                _dbeEntities.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return getProjectForHwRunningTestByProjectId;
        }

        #region PM Delete or Newly assigned by PM Incharge

        public List<CmnUserModel> GetPmCmnUsersForAssign()
        {
            List<CmnUserModel> listOfPm = _dbeEntities.Database.SqlQuery<CmnUserModel>(@"select * from CmnUsers where RoleName in ('PM','PMHEAD') and IsActive=1").ToList();
            return listOfPm;
        }


        public string PmReassignFromPmIncharge(long pMasterId, string approxPmInchargeToPmFinishDate, string pmInchargeDeleteQcComment,
            string projectHeadRemarks, string multideleteValue, string multiReassignValue, string multideleteID, string poNumber)
        {
            String userIdentity =
             HttpContext.Current.User.Identity.Name;

            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            DateTime date;
            DateTime.TryParseExact(approxPmInchargeToPmFinishDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date);

            DateTime ApproxPmInchargeToPmFinishDate1 = date;

            long qcDeletedId = Convert.ToInt64(multideleteID);
            long qcActiveId = Convert.ToInt64(multiReassignValue);


            if (qcDeletedId != 0)
            {

                var dbModel1 =
                    _dbeEntities.ProjectPmAssigns.FirstOrDefault(
                        x =>
                            x.ProjectMasterId == pMasterId && x.PONumber == poNumber &&
                            x.ProjectManagerUserId == qcDeletedId);

                if (dbModel1 != null)
                {

                    dbModel1.Status = "INACTIVE";
                    dbModel1.ProjectHeadInactiveRemarks = pmInchargeDeleteQcComment;
                    dbModel1.InactiveDate = DateTime.Now;
                    _dbeEntities.Entry(dbModel1).State = EntityState.Modified;

                    _dbeEntities.SaveChanges();
                }

            }

            if (qcActiveId != 0)
            {

                var dbModel1 =
                    _dbeEntities.ProjectPmAssigns.FirstOrDefault(
                        x =>
                            x.ProjectMasterId == pMasterId && x.PONumber == poNumber &&
                            x.ProjectManagerUserId == qcActiveId);

                if (dbModel1 != null)
                {
                    dbModel1.Status = "ASSIGNED";
                    dbModel1.ProjectHeadInactiveRemarks = pmInchargeDeleteQcComment;
                    dbModel1.InactiveDate = DateTime.Now;
                    _dbeEntities.Entry(dbModel1).State = EntityState.Modified;
                    _dbeEntities.SaveChanges();
                }
                else
                {
                    var pmAssign = new ProjectPmAssign
                    {
                        ProjectMasterId = pMasterId,
                        PONumber = poNumber,
                        AssignUserId = userId,
                        ProjectManagerUserId = qcActiveId,
                        ProjectHeadRemarks = projectHeadRemarks,
                        Status = "ASSIGNED",
                        AssignDate = DateTime.Now,

                        ApproxPmInchargeToPmFinishDate = ApproxPmInchargeToPmFinishDate1
                    };

                    _dbeEntities.ProjectPmAssigns.AddOrUpdate(pmAssign);
                    _dbeEntities.SaveChanges();
                }
            }

            var deletedPm = new ProjectPmReAssignLog
            {
                ProjectMasterId = pMasterId,
                PONumber = poNumber,
                InactiveUserId = qcDeletedId,
                InactiveDate = DateTime.Now,
                ActiveProjectManagerUserId = qcActiveId,
                ProjectHeadInActiveRemarks = pmInchargeDeleteQcComment,
                Status = "ASSIGNED",
                ApproxPmInchargeToPmFinishDate = ApproxPmInchargeToPmFinishDate1
            };

            _dbeEntities.ProjectPmReAssignLogs.AddOrUpdate(deletedPm);

            _dbeEntities.SaveChanges();

            return "ok";
        }



        #endregion

        #region PM DashBoard

        public PmTestCounterModel GetPmTestCounts(long userId)
        {
            var getPmTestCounts = _dbeEntities.Database.SqlQuery<PmTestCounterModel>(@"select 
            (select count(*) from ProjectPmAssigns where ProjectManagerUserId = {0} and Status='ASSIGNED') as NewProjectCounter,
			 (select count(*) from ProjectPmAssigns ppa where ProjectMasterId in (Select ProjectMasterId from SwQcInchargeAssigns) and ProjectManagerUserId = {0}) as ProjectForwaredToSwCounter,

			  (select count(*) from ProjectPmAssigns ppa where ProjectMasterId in (Select ProjectMasterId from HwQcInchargeAssigns) and ProjectManagerUserId = {0}) as ProjectForwaredToHwCounter", userId).FirstOrDefault();


            return getPmTestCounts;
        }

        #endregion

        #region PMHEAD Report DashBoard
        public List<CmnUserModel> GetActivePmList()
        {
            List<CmnUser> list = _dbeEntities.CmnUsers.Where(x => (x.RoleName == "PM" || x.RoleName == "PMHEAD") && x.IsActive).ToList();

            List<CmnUserModel> models = GenericMapper<CmnUser, CmnUserModel>.GetDestinationList(list);
            PmReportDashBoardViewModel pmReportDash = new PmReportDashBoardViewModel();
            pmReportDash.CmnUserModels = models;
            return models;
        }
        public CmnUserModel GetUserInfoByUserId(long userId)
        {
            FileManager manager = new FileManager();
            string query = string.Format(@"select * from CmnUsers where CmnUserId={0}", userId);
            var getUserInfoByUserId = _dbeEntities.Database.SqlQuery<CmnUserModel>(query).FirstOrDefault();
            getUserInfoByUserId.WebServerUrl = manager.GetFile(getUserInfoByUserId.ProfilePictureUrl);
            return getUserInfoByUserId;
        }
        public List<PmReportDashBoardViewModel> GetAllProjectListDetailsForInchargeReport(string startValue, string endValue, string emplyCode)
        {

            string getAssignedProjectToPMStatusForInchargeDashboardQuery;

            if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
            {

                //                    getAssignedProjectToPMStatusForInchargeDashboardQuery = string.Format(@"select 
                //                    pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                //                    por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,
                //
                //
                //                    STUFF((SELECT ', '  + convert(varchar(10),ppa3.InactiveDate,120)   FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  InactiveDate1,
                //
                //                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CmnUsers cmn1 left join ProjectPmAssigns ppa1 on ppa1.ProjectManagerUserId=cmn1.CmnUserId and ppa1.ProjectMasterId=pm.ProjectMasterId and ppa1.Status in ('INACTIVE','ASSIGNED')
                //                    WHERE  ppa1.ProjectManagerUserId=cmn1.CmnUserId ORDER BY ppa1.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                //
                //                    STUFF((SELECT ', '  + convert(varchar(10),ppa3.AssignDate,120)   FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AssignDate1,
                //
                //                    STUFF((SELECT ', '  + ppa2.Status FROM  ProjectPmAssigns ppa2 where ppa2.ProjectMasterId=pm.ProjectMasterId and ppa2.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa2.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AllAssignedPmProjectStatus,
                //
                //                    STUFF((SELECT ', '  + ppa3.ProjectHeadRemarks FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  ProjectHeadRemarks,
                //
                //                    STUFF((SELECT ', '  + ppa3.ProjectHeadInactiveRemarks FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  ProjectHeadInactiveRemarks
                //
                //
                //                    from ProjectMasters pm left join ProjectPmAssigns ppa
                //                    on pm.ProjectMasterId=ppa.ProjectMasterId 
                //                    left join CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId
                //                    left join ProjectPurchaseOrderForms por on por.ProjectMasterId=pm.ProjectMasterId
                //
                //                    where ppa.Status in ('ASSIGNED','INACTIVE') and ppa.AssignDate between '{0} 00:00:01' And '{1} 23:59:59' 
                //                    group by 
                //                    pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                //                    por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber
                //                    order by pm.ProjectMasterId desc", startValue, endValue);
                //                getAssignedProjectToPMStatusForInchargeDashboardQuery = string.Format(@"select 
                //                    pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                //                    por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,
                //
                //
                //                    STUFF((SELECT ', '  + convert(varchar(10),ppa3.InactiveDate,120)   FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  InactiveDate1,
                //
                //                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CmnUsers cmn1 left join ProjectPmAssigns ppa1 on ppa1.ProjectManagerUserId=cmn1.CmnUserId and ppa1.ProjectMasterId=pm.ProjectMasterId and ppa1.Status in ('INACTIVE','ASSIGNED')
                //                    WHERE  ppa1.ProjectManagerUserId=cmn1.CmnUserId ORDER BY ppa1.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                //
                //                    STUFF((SELECT ', '  + convert(varchar(10),ppa3.AssignDate,120)   FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AssignDate1,
                //
                //                    STUFF((SELECT ', '  + ppa2.Status FROM  ProjectPmAssigns ppa2 where ppa2.ProjectMasterId=pm.ProjectMasterId and ppa2.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa2.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AllAssignedPmProjectStatus,
                //
                //                    STUFF((SELECT ', '  + ppa3.ProjectHeadRemarks FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  ProjectHeadRemarks,
                //
                //                    STUFF((SELECT ', '  + ppa3.ProjectHeadInactiveRemarks FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                //                    ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  ProjectHeadInactiveRemarks
                //
                //
                //                    from ProjectMasters pm left join ProjectPmAssigns ppa
                //                    on pm.ProjectMasterId=ppa.ProjectMasterId 
                //                    left join CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId
                //                    left join ProjectPurchaseOrderForms por on por.ProjectMasterId=pm.ProjectMasterId
                //
                //                    where ppa.Status in ('ASSIGNED','INACTIVE') and ppa.AssignDate between '{0} 00:00:01' And '{1} 23:59:59' 
                //                    group by 
                //                    pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                //                    por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber
                //                    order by pm.ProjectMasterId desc", startValue, endValue);
                //            }
                //            else
                //            {

                //                getAssignedProjectToPMStatusForInchargeDashboardQuery = string.Format(@"select pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                //                por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,cm.EmployeeCode as QcAssignedPersonID,ppa.InactiveDate,
                //                cm.UserFullName,ppa.Status,ppa.ProjectHeadRemarks,ppa.ProjectHeadInactiveRemarks,ppa.AssignDate
                //                from ProjectMasters pm left join ProjectPmAssigns ppa
                //                on pm.ProjectMasterId=ppa.ProjectMasterId 
                //                left join CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId
                //                left join ProjectPurchaseOrderForms por on por.ProjectMasterId=pm.ProjectMasterId
                //                where ppa.Status in ('ASSIGNED','INACTIVE') and ppa.AssignDate between '{0} 00:00:01' And '{1} 23:59:59'   and cm.EmployeeCode='{2}' group by 
                //                pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                //                por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,cm.UserFullName,ppa.Status,ppa.InactiveDate,ppa.ProjectHeadRemarks,cm.EmployeeCode,ppa.ProjectHeadInactiveRemarks,ppa.AssignDate
                //                order by pm.ProjectMasterId desc", startValue, endValue, emplyCode);


                //            }

                getAssignedProjectToPMStatusForInchargeDashboardQuery = string.Format(@"select 
                pm.ProjectMasterId,pm.ProjectName,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProcessorClock,
                por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,

                STUFF((SELECT ', '  + convert(varchar(10),ppa3.InactiveDate,120)   FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  InactiveDate1,

                STUFF((SELECT ', '  + cmn1.UserFullName FROM CmnUsers cmn1 left join ProjectPmAssigns ppa1 on ppa1.ProjectManagerUserId=cmn1.CmnUserId and ppa1.ProjectMasterId=pm.ProjectMasterId and ppa1.Status in ('INACTIVE','ASSIGNED')
                WHERE  ppa1.ProjectManagerUserId=cmn1.CmnUserId ORDER BY ppa1.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AssignedPerson,

                STUFF((SELECT ', '  + convert(varchar(10),ppa3.AssignDate,120)   FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AssignDate1,

                STUFF((SELECT ', '  + ppa2.Status FROM  ProjectPmAssigns ppa2 where ppa2.ProjectMasterId=pm.ProjectMasterId and ppa2.Status in ('INACTIVE','ASSIGNED')
                ORDER BY ppa2.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  AllAssignedPmProjectStatus,

                STUFF((SELECT ', '  + ppa3.ProjectHeadRemarks FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  ProjectHeadRemarks,

                STUFF((SELECT ', '  + ppa3.ProjectHeadInactiveRemarks FROM  ProjectPmAssigns ppa3 where ppa3.ProjectMasterId=pm.ProjectMasterId  and ppa3.Status in ('INACTIVE','ASSIGNED')
                ORDER BY ppa3.ProjectManagerUserId FOR XML PATH('')),1,2,'')  AS  ProjectHeadInactiveRemarks,sia.SwQcInchargeAssignId,sia.TestPhaseID,tp.TestPhaseName
							
                from ProjectMasters pm left join ProjectPmAssigns ppa
                on pm.ProjectMasterId=ppa.ProjectMasterId 
                left join CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId
                left join ProjectPurchaseOrderForms por on por.ProjectMasterId=pm.ProjectMasterId
                left join SwQcInchargeAssigns sia on sia.ProjectMasterId=ppa.ProjectMasterId and sia.ProjectPmAssignId=ppa.ProjectPmAssignId
                left join TestPhase tp on tp.TestPhaseID=sia.TestPhaseID
                where ppa.Status in ('ASSIGNED','INACTIVE') and ppa.AssignDate between '{0} 00:00:01' And '{1} 23:59:59' 
                group by 
                pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,sia.SwQcInchargeAssignId,sia.TestPhaseID,tp.TestPhaseName
                order by pm.ProjectMasterId desc", startValue, endValue);
            }
            else
            {

                getAssignedProjectToPMStatusForInchargeDashboardQuery = string.Format(@"select pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,cm.EmployeeCode as QcAssignedPersonID,ppa.InactiveDate,
                cm.UserFullName,ppa.Status,ppa.ProjectHeadRemarks,ppa.ProjectHeadInactiveRemarks,ppa.AssignDate,sia.SwQcInchargeAssignId,sia.TestPhaseID,tp.TestPhaseName

                from ProjectMasters pm left join ProjectPmAssigns ppa
                on pm.ProjectMasterId=ppa.ProjectMasterId 
                left join CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId
                left join ProjectPurchaseOrderForms por on por.ProjectMasterId=pm.ProjectMasterId
                left join SwQcInchargeAssigns sia on sia.ProjectMasterId=ppa.ProjectMasterId and sia.ProjectPmAssignId=ppa.ProjectPmAssignId
                left join TestPhase tp on tp.TestPhaseID=sia.TestPhaseID

                where ppa.Status in ('ASSIGNED','INACTIVE') and ppa.AssignDate between '{0} 00:00:01' And '{1} 23:59:59'    and cm.EmployeeCode='{2}' 
				
                group by 
                pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
                por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,cm.UserFullName,ppa.Status,ppa.InactiveDate,ppa.ProjectHeadRemarks,cm.EmployeeCode,
                ppa.ProjectHeadInactiveRemarks,ppa.AssignDate,sia.SwQcInchargeAssignId,sia.TestPhaseID,tp.TestPhaseName
                order by pm.ProjectMasterId desc", startValue, endValue, emplyCode);


            }
            var getAssignedProjectToPMStatusForInchargeDashboard =
                _dbeEntities.Database.SqlQuery<PmReportDashBoardViewModel>(
                    getAssignedProjectToPMStatusForInchargeDashboardQuery).ToList();


            foreach (var project in getAssignedProjectToPMStatusForInchargeDashboard)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return getAssignedProjectToPMStatusForInchargeDashboard;

        }

        /////////////Details of Pm Work History////
        public List<PmBootImageAnimationModel> GetPmBootImageAnimationModelsDetails(long projectId, string poNumber, string emplyCode)
        {

            var models = new List<PmBootImageAnimationModel>();
            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {
                    var pmBoot =
                 _dbeEntities.Database.SqlQuery<PmBootImageAnimationModel>(
                     @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmBootImageAnimations pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} ", projectId, poNumber).ToList();

                    models = pmBoot.ToList();
                }
                else
                {
                    var pmBoot1 =
               _dbeEntities.Database.SqlQuery<PmBootImageAnimationModel>(
                   @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmBootImageAnimations pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2} ", projectId, poNumber, emplyCode).ToList();

                    models = pmBoot1.ToList();
                }
            }



            return models;

        }

        public List<PmGiftBoxModel> GetPmGiftBoxModelsDetails(long projectId, string poNumber, string emplyCode)
        {

            var models = new List<PmGiftBoxModel>();
            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {
                    var pmGift =
                    _dbeEntities.Database.SqlQuery<PmGiftBoxModel>(
                    @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmGiftBoxes pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} ", projectId, poNumber).ToList();

                    models = pmGift.ToList();
                }
                else
                {
                    var pmGift =
               _dbeEntities.Database.SqlQuery<PmGiftBoxModel>(
                    @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmGiftBoxes pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2} ", projectId, poNumber, emplyCode).ToList();

                    models = pmGift.ToList();
                }
            }



            return models;

        }
        public List<PmLabelsModel> GetPmLabelsModelsDetails(long projectId, string poNumber, string emplyCode)
        {
            var models = new List<PmLabelsModel>();
            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {
                    var pmLabel =
                       _dbeEntities.Database.SqlQuery<PmLabelsModel>(
                           @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmLabels pgb
                                        left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                                        left join CmnUsers cu on cu.CmnUserId=pgb.Added
                                        where pgb.ProjectMasterId= {0} and ppa.PONumber={1} ", projectId, poNumber).ToList();

                    models = pmLabel.ToList();
                }
                else
                {
                    var pmLabel =
               _dbeEntities.Database.SqlQuery<PmLabelsModel>(
                     @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmLabels pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2} ", projectId, poNumber, emplyCode).ToList();

                    models = pmLabel.ToList();
                }
            }



            return models;
        }

        public List<PmIdModel> GetPmIdModelsDetails(long projectId, string poNumber, string emplyCode)
        {

            var models = new List<PmIdModel>();

            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {
                    var pmIds =
                  _dbeEntities.Database.SqlQuery<PmIdModel>(
                  @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmIDs pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1}", projectId, poNumber).ToList();

                    models = pmIds.ToList();
                }
                else
                {
                    var pmIds =
                 _dbeEntities.Database.SqlQuery<PmIdModel>(
                 @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmIDs pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2}", projectId, poNumber, emplyCode).ToList();

                    models = pmIds.ToList();
                }
            }



            return models;

        }
        public List<PmScreenProtectorModel> GetPmScreenProtectorModelsDetails(long projectId, string poNumber, string emplyCode)
        {

            var models = new List<PmScreenProtectorModel>();

            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {
                    var pmScreenPro =
             _dbeEntities.Database.SqlQuery<PmScreenProtectorModel>(
                 @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmScreenProtectors pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1}", projectId, poNumber).ToList();

                    models = pmScreenPro.ToList();
                }
                else
                {
                    var pmScreenPro =
             _dbeEntities.Database.SqlQuery<PmScreenProtectorModel>(
                 @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmScreenProtectors pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2}", projectId, poNumber, emplyCode).ToList();

                    models = pmScreenPro.ToList();
                }
            }



            return models;

        }
        public List<PmWalpaperModel> GetPmWalpaperModelsDetails(long projectId, string poNumber, string emplyCode)
        {
            var models = new List<PmWalpaperModel>();

            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {

                    var pmWal =
                       _dbeEntities.Database.SqlQuery<PmWalpaperModel>(
                           @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmWalpapers pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1}", projectId, poNumber).ToList();

                    models = pmWal.ToList();
                }
                else
                {
                    var pmWal =
                     _dbeEntities.Database.SqlQuery<PmWalpaperModel>(
                         @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmWalpapers pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2}", projectId, poNumber, emplyCode).ToList();

                    models = pmWal.ToList();
                }
            }

            return models;

        }

        public List<PmSwCustomizationFinalModel> GetPmSwCustomizationFinalModelsDetails(long projectId, string poNumber, string emplyCode)
        {
            var models = new List<PmSwCustomizationFinalModel>();

            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {

                    var pmSw =
               _dbeEntities.Database.SqlQuery<PmSwCustomizationFinalModel>(
                   @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmSwCustomizationFinal pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber = {1}", projectId, poNumber).ToList();

                    models = pmSw.ToList();
                }
                else
                {
                    var pmSw =
             _dbeEntities.Database.SqlQuery<PmSwCustomizationFinalModel>(
                 @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmSwCustomizationFinal pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2}", projectId, poNumber, emplyCode).ToList();

                    models = pmSw.ToList();
                }
            }

            return models;

        }

        public List<PmPhnAccessoriesModel> GetPmPhnAccessoriesModelsDetails(long projectId, string poNumber, string emplyCode)
        {

            var models = new List<PmPhnAccessoriesModel>();

            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {

                    var pmAc =
                      _dbeEntities.Database.SqlQuery<PmPhnAccessoriesModel>(
                    @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmPhnAccessories pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1}", projectId, poNumber).ToList();

                    models = pmAc.ToList();
                }
                else
                {
                    var pmAc =
                      _dbeEntities.Database.SqlQuery<PmPhnAccessoriesModel>(
                    @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmPhnAccessories pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2}", projectId, poNumber, emplyCode).ToList();

                    models = pmAc.ToList();
                }
            }

            return models;

        }

        public List<PmPhnCameraModel> GetPmPhnCameraModelsDetails(long projectId, string poNumber, string emplyCode)
        {


            var models = new List<PmPhnCameraModel>();

            if (projectId > 0)
            {

                if (String.IsNullOrEmpty(emplyCode) || emplyCode.Trim().Length == 0)
                {
                    var pmCamera =
                        _dbeEntities.Database.SqlQuery<PmPhnCameraModel>(
                            @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmPhnCamera pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1}", projectId, poNumber).ToList();

                    models = pmCamera.ToList();
                }
                else
                {
                    var pmCamera =
                        _dbeEntities.Database.SqlQuery<PmPhnCameraModel>(
                            @"select pgb.*,ppa.*,cu.CmnUserId,cu.UserFullName,cu.UserName,cu.EmployeeCode from PmPhnCamera pgb
                    left join ProjectPmAssigns ppa on pgb.ProjectMasterId=ppa.ProjectMasterId and pgb.Added=ppa.ProjectManagerUserId
                    left join CmnUsers cu on cu.CmnUserId=pgb.Added
                    where pgb.ProjectMasterId= {0} and ppa.PONumber={1} and cu.EmployeeCode={2}", projectId, poNumber,
                            emplyCode).ToList();

                    models = pmCamera.ToList();
                }
            }

            return models;
        }

        /////////////Details of Pm Work History////
        #endregion

        #region Pm Own Report DashBoard
        public List<PmReportDashBoardViewModel> GetAllProjectListDetailsForPMReport(string startValue, string endValue, long userId)
        {
            string getAssignedProjectToPMStatusForInchargeDashboardQuery;

            getAssignedProjectToPMStatusForInchargeDashboardQuery = string.Format(@"select ppa.ProjectManagerUserId,pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
            por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,cm.EmployeeCode as QcAssignedPersonID,ppa.InactiveDate,
            cm.UserFullName,ppa.Status,ppa.ProjectHeadRemarks,ppa.ProjectHeadInactiveRemarks,ppa.AssignDate,sia.SwQcInchargeAssignId,sia.TestPhaseID,tp.TestPhaseName

            from ProjectMasters pm left join ProjectPmAssigns ppa
            on pm.ProjectMasterId=ppa.ProjectMasterId 
            left join CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId
            left join ProjectPurchaseOrderForms por on por.ProjectMasterId=pm.ProjectMasterId
            left join SwQcInchargeAssigns sia on sia.ProjectMasterId=ppa.ProjectMasterId and sia.ProjectPmAssignId=ppa.ProjectPmAssignId
            left join TestPhase tp on tp.TestPhaseID=sia.TestPhaseID

            where ppa.Status in ('ASSIGNED','INACTIVE') 
            and ppa.AssignDate between '{0} 00:00:01' And '{1} 23:59:59'   and ppa.ProjectManagerUserId='{2}'  group by 
            ppa.ProjectManagerUserId,pm.ProjectMasterId,pm.BackCam,pm.BackCamera,pm.Battery,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.NumberOfSample,pm.ProjectName,pm.ProcessorClock,
            por.IsCompleted,por.PurchaseOrderNumber,por.ProjectMasterId,por.Quantity,por.PoDate,ppa.PONumber,pm.OrderNuber,cm.UserFullName,ppa.Status,ppa.InactiveDate,ppa.ProjectHeadRemarks,cm.EmployeeCode,
            ppa.ProjectHeadInactiveRemarks,ppa.AssignDate,sia.SwQcInchargeAssignId,sia.TestPhaseID,tp.TestPhaseName

            order by pm.ProjectMasterId desc", startValue, endValue, userId);


            var getAssignedProjectToPMStatusForInchargeDashboard =
                _dbeEntities.Database.SqlQuery<PmReportDashBoardViewModel>(
                    getAssignedProjectToPMStatusForInchargeDashboardQuery).ToList();


            foreach (var project in getAssignedProjectToPMStatusForInchargeDashboard)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return getAssignedProjectToPMStatusForInchargeDashboard;


        }
        #endregion

        #region Incentive Sept 2019
        public List<VmPmIncentivePolicy> GetUserPerameterList()
        {

            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,ipp.Parameter,ipp.ParameterName,ipp.ParameterValue,ipp.RoleName
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveParameter ipp on cm.RoleName=ipp.RoleName
            where cm.rolename in ('PM') and cm.IsActive=1");
            var getUserList =
                _dbeEntities.Database.SqlQuery<VmPmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public List<Pm_Incentive_BaseModel> GetPmIncentiveBase()
        {
            var models = new List<Pm_Incentive_BaseModel>();

            var insBase = (from pmBase in _dbeEntities.Pm_Incentive_Base
                           where pmBase.IncentiveName != "INVENTORY_MANAGEMENT" && pmBase.IncentiveName != "MARKET_ISSUE_SUPPORT" && pmBase.ActiveRole == 1
                           select new Pm_Incentive_BaseModel
                           {
                               IncentiveName = pmBase.IncentiveName,
                               Amount = pmBase.Amount,
                               Id = pmBase.Id
                           }).ToList();
            models = insBase;

            return models;
        }

        public List<CmnUserModel> GetPmUserList()
        {
            var models = new List<CmnUserModel>();
            var userLists = (from cmnUsers in _dbeEntities.CmnUsers
                             where (cmnUsers.RoleName == "PM" || cmnUsers.RoleName == "PMHEAD") && cmnUsers.IsActive == true
                             select new CmnUserModel
                             {
                                 UserFullName = cmnUsers.UserFullName,
                                 UserName = cmnUsers.UserName,
                                 EmployeeCode = cmnUsers.EmployeeCode,
                                 RoleName = cmnUsers.RoleName
                             }).ToList();

            models = userLists;
            return models;
        }

        public List<ProjectMasterModel> GetProjectMasterListForPmIncentive(string employeeCode)
        {

            // var models = new List<ProjectMasterModel>();

            var nocList = (from pm in _dbeEntities.ProjectMasters
                           join ppa in _dbeEntities.ProjectPmAssigns on pm.ProjectMasterId equals
                               ppa.ProjectMasterId
                           join cmn in _dbeEntities.CmnUsers on ppa.ProjectManagerUserId equals cmn.CmnUserId
                           where ppa.Status == "ASSIGNED" && cmn.EmployeeCode == employeeCode && pm.IsActive
                           select new ProjectMasterModel
                           {
                               ProjectMasterId = pm.ProjectMasterId,
                               ProjectName = pm.ProjectName,
                               OrderNuber = pm.OrderNuber,
                               //CmnUserId = ppa.ProjectManagerUserId,
                               //Status = ppa.Status,
                               //UserFullName = cmn.UserFullName,
                               //RoleName = cmn.RoleName
                           }).ToList();

            //models = nocList;

            //foreach (var project in models)
            //{
            //    project.OrderNumberOrdinal = project.OrderNuber != null
            //        ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
            //        : string.Empty;
            //    if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
            //    {
            //        project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
            //    }

            //}

            return nocList;

        }

        public string SavePmMonthlyIncentive(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            //  var existing = _dbeEntities.Pm_Incentive.Where(p => p.Month).ToList();

            foreach (var insResult in results)
            {

                //if (existing.FirstOrDefault(p => p.Month == month && p.Year == Convert.ToInt64(year)) == null)
                //{
                var model = new Pm_Incentive
                {
                    Month = insResult.Month,
                    MonNum = Convert.ToInt32(insResult.MonNum),
                    Year = Convert.ToInt64(insResult.Year),
                    FinalAmount = Convert.ToDecimal(insResult.FinalAmount),
                    ProjectName = insResult.ProjectName,
                    EmployeeCode = insResult.EmployeeCode,
                    Amount = Convert.ToDecimal(insResult.Amount),
                    Remarks = insResult.Remarks,
                    DeductionAmount = Convert.ToDecimal(insResult.DeductionAmount),
                    D_Remarks = insResult.D_Remarks,
                    ProjectId = Convert.ToInt64(insResult.ProjectId),
                    Pm_Incentive_Base_Id = Convert.ToInt64(insResult.Pm_Incentive_Base_Id),
                    Added = userId,
                    AddedDate = DateTime.Now
                };
                _dbeEntities.Pm_Incentive.AddOrUpdate(model);
                //  }
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<Pm_Po_IncentiveModel> GetPmPoIncentiveForSKD(string employeeCode)
        {
            //            string query =
            //               string.Format(@"select pm.ProjectName,pm.ProjectMasterId as ProjectId,pm.ProjectType,pma.ProjectManagerUserId,po.PoDate,po.PoCategory,pm.OrderNuber as OrderNumber,
            //                case when pm.ProjectType='Smart' then cast (5000 as decimal(18,2)) else cast (3000 as decimal(18,2)) end as Amount,cu.EmployeeCode
            //                from CellPhoneProject.dbo.ProjectPmAssigns pma 
            //                join CellPhoneProject.dbo.ProjectMasters pm on pma.ProjectMasterId=pm.ProjectMasterId  and pma.Status='ASSIGNED'
            //                join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectMasterId=pm.ProjectMasterId
            //                join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pma.ProjectManagerUserId
            //                where po.PoCategory in ('SKD','CKD') and pm.OrderNuber=1 and cu.EmployeeCode='{0}'
            //                and po.PoDate between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-4, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)", employeeCode);
            string query =
             string.Format(@"select pm.ProjectName,pm.ProjectMasterId as ProjectId,pm.ProjectType,pma.ProjectManagerUserId,po.PoDate,po.PoCategory,pm.OrderNuber as OrderNumber,
                case when pm.ProjectType='Smart' then cast (3500 as decimal(18,2)) else cast (2100 as decimal(18,2)) end as Amount,cu.EmployeeCode
                from CellPhoneProject.dbo.ProjectPmAssigns pma 
                join CellPhoneProject.dbo.ProjectMasters pm on pma.ProjectMasterId=pm.ProjectMasterId  and pma.Status='ASSIGNED'
                join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectMasterId=pm.ProjectMasterId
                join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pma.ProjectManagerUserId
                where pm.IsActive=1 and pm.OrderNuber=1 and cu.EmployeeCode='{0}'
                and po.PoDate between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-4, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)", employeeCode);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_Po_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        //per repeat order//
        public List<Pm_Po_IncentiveModel> GetPmPoIncentiveForPerOrder(string employeeCode)
        {
            string query =
               string.Format(@"select pm.ProjectName,pm.ProjectMasterId  as ProjectId,pm.ProjectType,pma.ProjectManagerUserId,po.PoDate,po.PoCategory,pm.OrderNuber as OrderNumber,
                case when pm.ProjectType='Smart' then cast (3500 as decimal(18,2)) else cast (2100 as decimal(18,2)) end as Amount,cu.EmployeeCode
                from CellPhoneProject.dbo.ProjectPmAssigns pma 
                join CellPhoneProject.dbo.ProjectMasters pm on pma.ProjectMasterId=pm.ProjectMasterId and pma.Status='ASSIGNED'
                join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectMasterId=pm.ProjectMasterId
                join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pma.ProjectManagerUserId
                where pm.IsActive=1 and pm.OrderNuber not in (1)  and cu.EmployeeCode='{0}'
                and po.PoDate between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-2, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)", employeeCode);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_Po_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }
        //Old policy upto Sept 2019 shipment related
        //        public List<Pm_Shipment_IncentiveModel> GetPmShipmentIncentive(string employeeCode)
        //        {
        //            string query =
        //               string.Format(@"select pm.ProjectMasterId  as ProjectId,pm.ProjectName,cu.EmployeeCode, CAST(pm.ProjectType AS nvarchar(50)) as ProjectType,pma.ProjectManagerUserId,pm.ApproxShipmentDate,pos.FlightDepartureDate,pm.OrderNuber as OrderNumber,
        //                case when pm.ProjectType='Smart' then cast (5000 as decimal(18,2)) else cast (3000 as decimal(18,2)) end as Amount,DATEDIFF(day, pm.ApproxShipmentDate, pos.FlightDepartureDate) AS NoOfDays
        //                from CellPhoneProject.dbo.ProjectPmAssigns pma 
        //                join CellPhoneProject.dbo.ProjectMasters pm on pma.ProjectMasterId=pm.ProjectMasterId  and pma.Status='ASSIGNED'
        //                join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pma.ProjectManagerUserId
        //                join [CellPhoneProject].[dbo].ProjectOrderShipments pos on pm.ProjectMasterId=pos.ProjectMasterId
        //                where   cu.EmployeeCode='{0}' and  ((DATEDIFF(day, pm.ApproxShipmentDate,pos.FlightDepartureDate)>0 or (DATEDIFF(day, pm.ApproxShipmentDate,pos.FlightDepartureDate))<-15))
        //                and pm.ApproxShipmentDate between  DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)
        //
        //                and pos.FlightDepartureDate=(SELECT TOP 1 FlightDepartureDate
        //                FROM [CellPhoneProject].[dbo].[ProjectOrderShipments] pos1
        //                join cellphoneproject.dbo.[ProjectPurchaseOrderForms] ppo on pos1.projectmasterid=ppo.projectmasterid where pos1.ProjectMasterId=pos.ProjectMasterId 
        //                group by ppo.PurchaseOrderNumber,FlightDepartureDate
        //                order by FlightDepartureDate asc)
        //
        //                group by pm.ProjectMasterId,pm.ProjectName,cu.EmployeeCode,pm.ProjectType,pma.ProjectManagerUserId,pm.ApproxShipmentDate,pos.FlightDepartureDate,pm.OrderNuber
        //                ", employeeCode);

        //            var getPmPoIncentiveModel =
        //                _dbeEntities.Database.SqlQuery<Pm_Shipment_IncentiveModel>(query).ToList();
        //            return getPmPoIncentiveModel;
        //        }


        //public string SaveShipmentIncentive(List<Custom_Pm_IncentiveModel> results)
        //{
        //    String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
        //    long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

        //    //  var existing = _dbeEntities.Pm_Incentive.Where(p => p.Month).ToList();

        //    foreach (var insResult in results)
        //    {

        //        //if (existing.FirstOrDefault(p => p.Month == month && p.Year == Convert.ToInt64(year)) == null)
        //        //{

        //        var model = new Pm_Shipment_Incentive();
        //        model.Month = insResult.Month;
        //        model.MonNum = insResult.MonNum;
        //        model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;
        //        model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);
        //        model.ProjectName = insResult.ProjectName;
        //        model.EmployeeCode = insResult.EmployeeCode;
        //        model.Amount = !string.IsNullOrWhiteSpace(insResult.Amount) ? Convert.ToDecimal(insResult.Amount) : 0;
        //        model.Remarks = insResult.Remarks;
        //        model.DeductionAmount = !string.IsNullOrWhiteSpace(insResult.DeductionAmount) ? Convert.ToDecimal(insResult.DeductionAmount) : 0;
        //        model.D_Remarks = insResult.D_Remarks;
        //        model.ProjectId = insResult.ProjectId;
        //        model.ApproxShipmentDate = insResult.ApproxShipmentDate != null ? Convert.ToDateTime(insResult.ApproxShipmentDate) : DateTime.MinValue;
        //        model.ChainaInspectionDate = insResult.FlightDepartureDate != null ? Convert.ToDateTime(insResult.FlightDepartureDate) : DateTime.MinValue;
        //        model.OrderNumber = insResult.OrderNumber;
        //        model.ProjectManagerUserId = insResult.ProjectManagerUserId;
        //        model.ProjectType = insResult.ProjectType;
        //        model.Added = userId;
        //        model.AddedDate = DateTime.Now;
        //        model.NoOfdays = insResult.NoOfdays;
        //        _dbeEntities.Pm_Shipment_Incentive.AddOrUpdate(model);
        //        //  }

        //    }
        //    _dbeEntities.SaveChanges();

        //    return "ok";
        //}
        //        public List<Custom_Pm_IncentiveModel> GetPmShipIncentive(string empCode, string monNum, string year)
        //        {
        //            long years = 0;
        //            long.TryParse(year, out years);
        //            int monNums = 0;
        //            int.TryParse(monNum, out monNums);

        //            string query =
        //             string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_Shipment_Incentive where EmployeeCode='{0}' 
        //                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
        //            var getPmPoIncentiveModel =
        //                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
        //            return getPmPoIncentiveModel;
        //        }

        public string SaveOthersIncentive(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var inResults in results)
            {
                var model = new Pm_Incentive();
                model.IncentiveType = "Others";
                model.ProjectId = inResults.ProjectId;
                model.ProjectName = inResults.ProjectName;
                model.Month = inResults.Month;
                model.MonNum = inResults.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(inResults.Year) ? Convert.ToInt64(inResults.Year) : 0;
                model.FinalAmount = !string.IsNullOrWhiteSpace(inResults.FinalAmount) ? Convert.ToDecimal(inResults.FinalAmount) : 0;
                model.EmployeeCode = inResults.EmployeeCode;
                model.Amount = !string.IsNullOrWhiteSpace(inResults.Amount) ? Convert.ToDecimal(inResults.Amount) : 0;
                model.Remarks = inResults.Remarks;
                model.DeductionAmount = !string.IsNullOrWhiteSpace(inResults.DeductionAmount) ? Convert.ToDecimal(inResults.DeductionAmount) : 0;
                model.D_Remarks = inResults.D_Remarks;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_Incentive.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<Pm_Incentive_BaseModel> GetPmInventoryAndMarketIssues()
        {
            var models = new List<Pm_Incentive_BaseModel>();

            var insBase = (from pmBase in _dbeEntities.Pm_Incentive_Base
                           where pmBase.IncentiveName == "INVENTORY_MANAGEMENT" || pmBase.IncentiveName == "MARKET_ISSUE_SUPPORT" && pmBase.ActiveRole == 1
                           select new Pm_Incentive_BaseModel
                           {
                               IncentiveName = pmBase.IncentiveName,
                               Amount = pmBase.Amount,
                               Id = pmBase.Id
                           }).ToList();
            models = insBase;

            return models;
        }

        public List<ProjectMasterModel> GetAllProjectsForPmIncentive()
        {
            var models = new List<ProjectMasterModel>();

            var nocList = (from pm in _dbeEntities.ProjectMasters
                           where pm.ProjectStatus == "APPROVED" && pm.IsActive
                           select new ProjectMasterModel
                           {
                               ProjectMasterId = pm.ProjectMasterId,
                               ProjectName = pm.ProjectName,
                               OrderNuber = pm.OrderNuber,
                           }).ToList();
            models = nocList;

            foreach (var project in models)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + "),";
                }

            }

            return models;
        }

        public string SaveMarketIssueIncentive(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            //  var existing = _dbeEntities.Pm_Incentive.Where(p => p.Month).ToList();

            foreach (var insResult in results)
            {

                //if (existing.FirstOrDefault(p => p.Month == month && p.Year == Convert.ToInt64(year)) == null)
                //{

                var model = new Pm_Incentive();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;
                model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);
                model.MultiProjectName = insResult.MultiProjectName;
                model.MultiProjectIds = insResult.MultiProjectIds;
                model.EmployeeCode = insResult.EmployeeCode;
                model.Amount = !string.IsNullOrWhiteSpace(insResult.Amount) ? Convert.ToDecimal(insResult.Amount) : 0;
                model.Remarks = insResult.Remarks;
                model.DeductionAmount = !string.IsNullOrWhiteSpace(insResult.DeductionAmount) ? Convert.ToDecimal(insResult.DeductionAmount) : 0;
                model.D_Remarks = insResult.D_Remarks;
                model.PersonNo = insResult.PersonNo;
                model.Pm_Incentive_Base_Id = insResult.Pm_Incentive_Base_Id;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_Incentive.AddOrUpdate(model);
                //  }

            }
            _dbeEntities.SaveChanges();

            return "ok";
        }
        public string SavePoMonthlyIncentive(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            //  var existing = _dbeEntities.Pm_Incentive.Where(p => p.Month).ToList();
            foreach (var insResult in results)
            {
                //if (existing.FirstOrDefault(p => p.Month == month && p.Year == Convert.ToInt64(year)) == null)
                //{

                var model = new Pm_Po_Incentive();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;
                model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);
                model.ProjectName = insResult.ProjectName;
                model.EmployeeCode = insResult.EmployeeCode;
                model.Amount = !string.IsNullOrWhiteSpace(insResult.Amount) ? Convert.ToDecimal(insResult.Amount) : 0;
                model.Remarks = insResult.Remarks;
                model.DeductionAmount = !string.IsNullOrWhiteSpace(insResult.DeductionAmount) ? Convert.ToDecimal(insResult.DeductionAmount) : 0;
                model.D_Remarks = insResult.D_Remarks;
                model.ProjectId = insResult.ProjectId;
                model.PoCategory = insResult.PoCategory;
                model.PoDate = insResult.PoDate != null ? Convert.ToDateTime(insResult.PoDate) : DateTime.MinValue;
                model.OrderNumber = insResult.OrderNumber;
                model.ProjectManagerUserId = insResult.ProjectManagerUserId;
                model.ProjectType = insResult.ProjectType;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_Po_Incentive.AddOrUpdate(model);
                //  }
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }
        public List<Custom_Pm_IncentiveModel> GetPmIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_Incentive where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }
        public List<Custom_Pm_IncentiveModel> GetPmPoIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_Po_Incentive where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }
        public string SaveTotalIncentive(string totalAmount, string totalPenalties, string empCode, string month, string monNum, string year)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            var model = new Incentive();
            model.ThisMonthAmount = Convert.ToDecimal(totalAmount);
            model.AmountDeduction = Convert.ToDecimal(totalPenalties);
            model.TotalIncentive = model.ThisMonthAmount - Convert.ToDecimal(totalPenalties);
            model.Month = month;
            model.MonNum = Convert.ToInt32(monNum);
            model.Year = Convert.ToInt64(year);
            model.UserId = empCode;
            model.DepartmentName = "PM";
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            _dbeEntities.Incentives.AddOrUpdate(model);
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public List<Custom_Pm_IncentiveModel> GetPmIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);


            //            string query =
            //                       string.Format(@"select distinct case when pmi.MultiProjectName is null then pmi.ProjectName else pmi.MultiProjectName end as ProjectName,pmi.PersonNo,pmi.FinalAmount as FinalAmount1,IncsTypes='CategoryWiseIncs.',
            //                    pmi.Amount as Amount1,pib.IncentiveName as Others,pmi.Remarks,pmi.DeductionAmount as DeductionAmount1,pmi.D_Remarks,ppo.PoCategory,ppo.PoDate,pos.FlightDepartureDate as ShipmentTaken, pm.ApproxShipmentDate as LSD 
            //                    from CellPhoneProject.dbo.Pm_Incentive pmi
            //                    left join CellPhoneProject.dbo.Pm_Incentive_Base pib on pmi.Pm_Incentive_Base_Id=pib.Id
            //                    left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on ppo.ProjectMasterId=pmi.ProjectId
            //                    left join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pmi.ProjectId and pos.FlightDepartureDate in
            //                    (select top 1 FlightDepartureDate from CellPhoneProject.dbo.ProjectOrderShipments where pmi.ProjectId=ProjectMasterId order by FlightDepartureDate desc)
            //                    left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=pmi.ProjectId 
            //                    where pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}' ", empCode, monNums, years);
            string query =
           string.Format(@"select ProjectName,PersonNo,FinalAmount1,IncsTypes,Amount1,Others,Remarks,DeductionAmount1,D_Remarks,PoCategory,PoDate,ShipmentTaken,LSD
            from	
            (
	            select distinct case when pmi.MultiProjectName is null then pmi.ProjectName else pmi.MultiProjectName end as ProjectName,pmi.PersonNo,pmi.FinalAmount as FinalAmount1,
	            case when pmi.IncentiveType is null then 'CategoryWiseIncs' else pmi.IncentiveType end as IncsTypes,
	            pmi.Amount as Amount1,pib.IncentiveName as Others,pmi.Remarks,pmi.DeductionAmount as DeductionAmount1,pmi.D_Remarks,ppo.PoCategory,ppo.PoDate,pos.FlightDepartureDate as ShipmentTaken, pm.ApproxShipmentDate as LSD 
	            from CellPhoneProject.dbo.Pm_Incentive pmi
	            left join CellPhoneProject.dbo.Pm_Incentive_Base pib on pmi.Pm_Incentive_Base_Id=pib.Id
	            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on ppo.ProjectMasterId=pmi.ProjectId
	            left join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pmi.ProjectId and pos.FlightDepartureDate in
	            (select top 1 FlightDepartureDate from CellPhoneProject.dbo.ProjectOrderShipments where pmi.ProjectId=ProjectMasterId order by FlightDepartureDate desc)
	            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=pmi.ProjectId 
	            where pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}'

            )A ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            foreach (var project in getPmPoIncentiveModel)
            {
                project.OrderNumberOrdinal = project.OrderNumber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmPoIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select distinct ppi.ProjectName,ppi.PoCategory,ppi.PoDate,pm.ApproxShipmentDate as LSD,ps.FlightDepartureDate as ShipmentTaken,ppi.OrderNumber,
                ppi.Amount as Amount1,ppi.Remarks,ppi.DeductionAmount as DeductionAmount1,ppi.D_Remarks,ppi.FinalAmount as FinalAmount1,IncsTypes='PoBaseAmount'
                from CellPhoneProject.dbo.Pm_Po_Incentive ppi
                left join CellPhoneProject.dbo.ProjectMasters pm on ppi.ProjectId=pm.ProjectMasterId
                left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppi.ProjectId  
                and ps.FlightDepartureDate in (select top 1 FlightDepartureDate from CellPhoneProject.dbo.ProjectOrderShipments where pm.ProjectMasterId=ProjectMasterId
                order by FlightDepartureDate desc)
                where ppi.EmployeeCode='{0}' and ppi.Year='{2}' and ppi.MonNum='{1}'", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();


            foreach (var project in getPmPoIncentiveModel)
            {
                project.OrderNumberOrdinal = project.OrderNumber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return getPmPoIncentiveModel;
        }
        public List<Custom_Pm_IncentiveModel> GetPreparedUserName()
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string getIncentiveReportQuery = string.Format(@"select UserFullName,EmployeeCode  FROM [CellPhoneProject].[dbo].CmnUsers where CmnUserId={0}", userId);
            var getIncentiveReports =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }

        public List<Custom_Pm_IncentiveModel> GetTotalFinalIncentiveOfPm(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select TotalIncentive as FinalAmount1 from CellPhoneProject.dbo.Incentive
                where UserId='{0}' and Year='{2}' and MonNum='{1}' ", empCode, monNums, years);


            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> PmIncentiveForAllPerson(string Month, string MonNum, string Year)
        {
            long years = 0;
            long.TryParse(Year, out years);
            int monNums = 0;
            int.TryParse(MonNum, out monNums);

            //            string query =
            //             string.Format(@"select cmu.UserFullName,cmu.EmployeeCode,inc.Month,inc.Year as Year1,inc.TotalIncentive,inc.AmountDeduction as DeductionAmount1 from CellPhoneProject.dbo.Incentive inc
            //                join CellPhoneProject.dbo.CmnUsers cmu on inc.UserId=cmu.EmployeeCode
            //                 where inc.DepartmentName='PM' and inc.MonNum='{0}' and inc.Year='{1}' ", monNums, years);

            string query =
            string.Format(@"select UserFullName,EmployeeCode,Month,Year1,TotalIncentive,case when DeductByIncharge is null then 0 else cast(DeductByIncharge as decimal(18,2)) end as DeductByIncharge,case when SystemDeduction is null then 0 else cast(SystemDeduction as decimal(18,2)) end as SystemDeduction from
(
	select cmu.UserFullName,cmu.EmployeeCode,inc.Month,inc.Year as Year1,inc.TotalIncentive,inc.AmountDeduction as DeductionAmount1,
	(
		select DeductByPm from
		(
			select sum(A.DeductByPm) as DeductByPm from 
		   (
			select sum(DeductAmount) as DeductByPm from CellPhoneProject.[dbo].[NinetyFivePercentProductionReward] where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
			union
			select sum(DeductAmount) as DeductByPm from CellPhoneProject.[dbo].[NinetyFivePercentSalesOutReward] where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
			union
			select sum(DeductionAmount) as DeductByPm from CellPhoneProject.[dbo].[Pm_Incentive] where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
			union
			select sum(DeductionAmount) as DeductByPm from CellPhoneProject.[dbo].[Pm_PiClosingIncentive] where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
			union
			select sum(DeductionAmount) as DeductByPm from CellPhoneProject.[dbo].[Pm_Po_Incentive] where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
			union
			select sum(DeductionAmount) as DeductByPm from CellPhoneProject.[dbo].[Pm_Accessories] where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
		   )A
		)B
	) as DeductByIncharge,
	(
		select sum(SystemDeduction) as SystemDeduction from 
		(
			select sum(TotalPenalties) as SystemDeduction from CellPhoneProject.[dbo].PmHead_VesselPenaltiesForPo where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
			UNION 
			select sum(DeductedAmount) as SystemDeduction from CellPhoneProject.[dbo].PmAndQcLsdToVesselData  where Employeecode=cmu.EmployeeCode and MonNum=inc.MonNum and Year=inc.Year
		)AA
	) as SystemDeduction

	from CellPhoneProject.dbo.Incentive inc
	join CellPhoneProject.dbo.CmnUsers cmu on inc.UserId=cmu.EmployeeCode
	where inc.DepartmentName='PM' and inc.MonNum='{0}' and inc.Year='{1}'
)CC ", monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public bool GetIncentiveTypeData(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_Incentive] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetPoIncentiveData(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_Po_Incentive] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetDocIncentiveData(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_DocumentUploadIncentive] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        //Old policy upto Sept 2019
        //        public bool GetShipmentIncentiveData(string employeeCode, int monNum, string year)
        //        {
        //            int MonNum = Convert.ToInt32(monNum);
        //            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
        //            if (MonNum > 0 && year != null)
        //            {
        //                string getIncentiveReportQuery = string.Format(@"
        //             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_Shipment_Incentive] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
        //                getIncentiveReports =
        //                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
        //            }

        //            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
        //            {
        //                return true;
        //            }
        //            return false;
        //        }
        //        public List<Pm_Po_IncentiveModel> GetPmPoIncentiveForCBU(string employeeCode)
        //        {
        //            string query =
        //               string.Format(@"select pm.ProjectName,pm.ProjectMasterId  as ProjectId,pm.ProjectType,pma.ProjectManagerUserId,po.PoDate,po.PoCategory,pm.OrderNuber as OrderNumber,
        //                case when pm.ProjectType='Smart' then cast (5000 as decimal(18,2)) else cast (3000 as decimal(18,2)) end as Amount,cu.EmployeeCode
        //                from CellPhoneProject.dbo.ProjectPmAssigns pma 
        //                join CellPhoneProject.dbo.ProjectMasters pm on pma.ProjectMasterId=pm.ProjectMasterId  and pma.Status='ASSIGNED'
        //                join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectMasterId=pm.ProjectMasterId
        //                join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pma.ProjectManagerUserId
        //                where po.PoCategory in ('CBU') and pm.OrderNuber=1 and cu.EmployeeCode='{0}'
        //                and po.PoDate between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-4, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)", employeeCode);
        //            var getPmPoIncentiveModel =
        //                _dbeEntities.Database.SqlQuery<Pm_Po_IncentiveModel>(query).ToList();
        //            return getPmPoIncentiveModel;
        //        }
        //        public List<Custom_Pm_IncentiveModel> GetPmShipIncentiveForPrint(string empCode, string monNum, string year)
        //        {
        //            long years = 0;
        //            long.TryParse(year, out years);
        //            int monNums = 0;
        //            int.TryParse(monNum, out monNums);

        //            string query =
        //           string.Format(@"select distinct psi.ProjectName,ppo.PoCategory,ppo.PoDate,psi.ApproxShipmentDate as LSD, psi.ChainaInspectionDate as ShipmentTaken,psi.OrderNumber,psi.NoOfdays as EarlierOrLateShipment,
        //                psi.Amount as Amount1,psi.Remarks,psi.DeductionAmount as DeductionAmount1,psi.D_Remarks,psi.FinalAmount as FinalAmount1 
        //				from CellPhoneProject.dbo.Pm_Shipment_Incentive psi
        //				left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on ppo.ProjectMasterId=psi.ProjectId
        //                where psi.EmployeeCode='{0}' and psi.Year='{2}' and psi.MonNum={1}", empCode, monNums, years);
        //            var getPmPoIncentiveModel =
        //                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();


        //            foreach (var project in getPmPoIncentiveModel)
        //            {
        //                project.OrderNumberOrdinal = project.OrderNumber != null
        //                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
        //                    : string.Empty;
        //                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
        //                {
        //                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
        //                }

        //            }

        //            return getPmPoIncentiveModel;
        //        }
        public bool GetTotalIncentiveData(string empCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),UserId from [CellPhoneProject].[dbo].[Incentive] where UserId='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, empCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();


            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<ProjectMasterModel> GetAllProjectsForOthers()
        {
            var models = new List<ProjectMasterModel>();

            var nocList = (from pm in _dbeEntities.ProjectMasters
                           where pm.ProjectStatus == "APPROVED" && pm.IsActive
                           orderby pm.ProjectName ascending
                           select new ProjectMasterModel
                           {
                               ProjectMasterId = pm.ProjectMasterId,
                               ProjectName = pm.ProjectName,
                               OrderNuber = pm.OrderNuber,
                           }).ToList();
            models = nocList;

            foreach (var project in models)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return models;
        }

        public List<Custom_Pm_IncentiveModel> GetAllIncentiveDataOfFourMonths(string empCode, string month1, string month2, string year)
        {
            List<Custom_Pm_IncentiveModel> cmList = new List<Custom_Pm_IncentiveModel>();

            string query1 =
         string.Format(@"select distinct case when pmi.MultiProjectName is null then pmi.ProjectName else pmi.MultiProjectName end as ProjectName,ppo.PoCategory,pmi.PersonNo,pib.IncentiveName as Others,
        ppo.PoDate,pos.FlightDepartureDate as ShipmentTaken, pm.ApproxShipmentDate as LSD,
        pmi.Month,pmi.MonNum,pmi.Amount as Amount1,pmi.Remarks,pmi.DeductionAmount as DeductionAmount1,pmi.D_Remarks,pmi.FinalAmount as FinalAmount1
        from CellPhoneProject.dbo.Pm_Incentive pmi
        left join CellPhoneProject.dbo.Pm_Incentive_Base pib on pmi.Pm_Incentive_Base_Id=pib.Id
        left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on ppo.ProjectMasterId=pmi.ProjectId
        left join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pmi.ProjectId and pos.FlightDepartureDate
            in (select top 1 FlightDepartureDate from CellPhoneProject.dbo.ProjectOrderShipments where pmi.ProjectId=ProjectMasterId order by FlightDepartureDate desc)
        left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=pmi.ProjectId 
        where pmi.EmployeeCode='{0}' and pmi.Year='{3}'
        and (pmi.MonNum >='{1}' and pmi.MonNum<='{2}')
        order by pmi.MonNum asc", empCode, month1, month2, year);

            var pmIncentive =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query1).ToList();
            foreach (var project in pmIncentive)
            {
                project.OrderNumberOrdinal = project.OrderNumber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            foreach (var cusPmInsList in pmIncentive)
            {
                var items = new Custom_Pm_IncentiveModel();
                items.ProjectName = cusPmInsList.ProjectName;
                items.PoCategory = cusPmInsList.PoCategory;
                items.PoDate = cusPmInsList.PoDate;
                items.LSD = cusPmInsList.LSD;
                items.ShipmentTaken = cusPmInsList.ShipmentTaken;
                items.EarlierOrLateShipment = cusPmInsList.EarlierOrLateShipment;
                items.OrderNumber = cusPmInsList.OrderNumber;
                items.Others = cusPmInsList.Others;
                items.PersonNo = cusPmInsList.PersonNo;
                items.Amount1 = cusPmInsList.Amount1;
                items.Remarks = cusPmInsList.Remarks;
                items.DeductionAmount1 = cusPmInsList.DeductionAmount1;
                items.D_Remarks = cusPmInsList.D_Remarks;
                items.FinalAmount1 = cusPmInsList.FinalAmount1;
                items.Month = cusPmInsList.Month;
                items.MonNum = cusPmInsList.MonNum;
                items.Year = cusPmInsList.Year;
                cmList.Add(items);
            }

            string query2 = string.Format(@"
            select distinct ppi.ProjectName,ppi.PoCategory,ppi.PoDate,pm.ApproxShipmentDate as LSD,ps.FlightDepartureDate as ShipmentTaken,ppi.OrderNumber,ppi.Month,ppi.MonNum,
            ppi.Amount as Amount1,ppi.Remarks,ppi.DeductionAmount as DeductionAmount1,ppi.D_Remarks,ppi.FinalAmount as FinalAmount1
            from CellPhoneProject.dbo.Pm_Po_Incentive ppi
            left join CellPhoneProject.dbo.ProjectMasters pm on ppi.ProjectId=pm.ProjectMasterId
            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppi.ProjectId  
            and ps.FlightDepartureDate in (select top 1 FlightDepartureDate from CellPhoneProject.dbo.ProjectOrderShipments where pm.ProjectMasterId=ProjectMasterId order by FlightDepartureDate desc)
            where ppi.EmployeeCode='{0}' and ppi.Year='{3}' and (ppi.MonNum >='{1}' and ppi.MonNum<='{2}') order by ppi.MonNum asc", empCode, month1, month2, year);

            var pmPo = _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query2).ToList();

            foreach (var project in pmPo)
            {
                project.OrderNumberOrdinal = project.OrderNumber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            foreach (var customPmPoList in pmPo)
            {
                var items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmPoList.ProjectName;
                items.PoCategory = customPmPoList.PoCategory;
                items.PoDate = customPmPoList.PoDate;
                items.LSD = customPmPoList.LSD;
                items.ShipmentTaken = customPmPoList.ShipmentTaken;
                items.EarlierOrLateShipment = customPmPoList.EarlierOrLateShipment;
                items.OrderNumber = customPmPoList.OrderNumber;
                items.Others = customPmPoList.Others;
                items.PersonNo = customPmPoList.PersonNo;
                items.Amount1 = customPmPoList.Amount1;
                items.Remarks = customPmPoList.Remarks;
                items.DeductionAmount1 = customPmPoList.DeductionAmount1;
                items.D_Remarks = customPmPoList.D_Remarks;
                items.FinalAmount1 = customPmPoList.FinalAmount1;
                items.Month = customPmPoList.Month;
                items.MonNum = customPmPoList.MonNum;
                items.Year = customPmPoList.Year;
                cmList.Add(items);

            }
            //old policy upto sept 2019
            //            string query3 = string.Format(@"select distinct psi.ProjectName,ppo.PoCategory,ppo.PoDate,psi.ApproxShipmentDate as LSD, psi.ChainaInspectionDate as ShipmentTaken,psi.OrderNumber,psi.NoOfdays as EarlierOrLateShipment,
            //            psi.Month,psi.MonNum,
            //            psi.Amount as Amount1,psi.Remarks,psi.DeductionAmount as DeductionAmount1,psi.D_Remarks,psi.FinalAmount as FinalAmount1 
            //            from CellPhoneProject.dbo.Pm_Shipment_Incentive psi
            //            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on ppo.ProjectMasterId=psi.ProjectId
            //            where psi.EmployeeCode='{0}' and psi.Year='{3}' and (psi.MonNum >='{1}' and psi.MonNum<='{2}') order by psi.MonNum asc", empCode, month1, month2, year);

            //            var pmShipment = _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query3).ToList();
            //            foreach (var project in pmShipment)
            //            {
            //                project.OrderNumberOrdinal = project.OrderNumber != null
            //                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
            //                    : string.Empty;
            //                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
            //                {
            //                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
            //                }

            //            }
            //            foreach (var customPmShipment in pmShipment)
            //            {
            //                var items = new Custom_Pm_IncentiveModel();
            //                items.ProjectName = customPmShipment.ProjectName;
            //                items.PoCategory = customPmShipment.PoCategory;
            //                items.PoDate = customPmShipment.PoDate;
            //                items.LSD = customPmShipment.LSD;
            //                items.ShipmentTaken = customPmShipment.ShipmentTaken;
            //                items.EarlierOrLateShipment = customPmShipment.EarlierOrLateShipment;
            //                items.OrderNumber = customPmShipment.OrderNumber;
            //                items.Others = customPmShipment.Others;
            //                items.PersonNo = customPmShipment.PersonNo;
            //                items.Amount1 = customPmShipment.Amount1;
            //                items.Remarks = customPmShipment.Remarks;
            //                items.DeductionAmount1 = customPmShipment.DeductionAmount1;
            //                items.D_Remarks = customPmShipment.D_Remarks;
            //                items.FinalAmount1 = customPmShipment.FinalAmount1;
            //                items.Month = customPmShipment.Month;
            //                items.MonNum = customPmShipment.MonNum;
            //                items.Year = customPmShipment.Year;
            //                cmList.Add(items);
            //            }

            //ViewBag.PmIncentiveForPrint = cmList;

            var cmListedVal = cmList.OrderBy(x => x.MonNum).ToList();

            return cmListedVal;
        }

        public List<Custom_Pm_IncentiveModel> GetTotalIncentiveForMonthRange(string empCode, string monthNum1, string monthNum2, string yearName)
        {

            string query1 =
         string.Format(@"select UserId as EmployeeCode,CONVERT(varchar(10),TotalIncentive) as TotalIncentive1,Month,CONVERT(varchar(10),Year) as Year FROM [CellPhoneProject].[dbo].[Incentive] where DepartmentName='PM' 
            and UserId='{0}' and Year='{3}' and (MonNum>='{1}' and MonNum<='{2}') order by MonNum asc", empCode, monthNum1, monthNum2, yearName);

            var pmIncentive =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query1).ToList();

            var cmListedVal = pmIncentive.OrderBy(x => x.MonNum).ToList();

            return cmListedVal;
        }

        public string SaveAccessoriesDetails(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var inResults in results)
            {
                var orderQuery =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == inResults.ProjectId)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault();

                var poQuery =
             _dbeEntities.ProjectPurchaseOrderForms.FirstOrDefault(x => x.ProjectMasterId == inResults.ProjectId);

                var empId =
              _dbeEntities.CmnUsers.FirstOrDefault(x => x.EmployeeCode == inResults.EmployeeCode);

                var model = new Pm_Accessories();
                model.ProjectId = inResults.ProjectId;
                model.ProjectName = inResults.ProjectName;
                model.OrderNumber = orderQuery;
                model.PoDate = poQuery.PoDate;
                model.PoCategory = poQuery.PoCategory;
                model.EmployeeCode = inResults.EmployeeCode;
                model.ProjectManagerUserId = empId.CmnUserId;
                model.IncentiveTypeForAccessories = inResults.IncentiveTypeForAccessories;
                model.Amount = !string.IsNullOrWhiteSpace(inResults.Amount) ? Convert.ToDecimal(inResults.Amount) : 0;
                model.Remarks = inResults.Remarks;
                model.DeductionAmount = !string.IsNullOrWhiteSpace(inResults.DeductionAmount) ? Convert.ToDecimal(inResults.DeductionAmount) : 0;
                model.D_Remarks = inResults.D_Remarks;
                model.FinalAmount = !string.IsNullOrWhiteSpace(inResults.FinalAmount) ? Convert.ToDecimal(inResults.FinalAmount) : 0;

                model.Month = inResults.Month;
                model.MonNum = inResults.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(inResults.Year) ? Convert.ToInt64(inResults.Year) : 0;
                model.DepartmentName = "PM";
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_Accessories.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public string SavePmPiClosing(Vm_PiClosing model)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var proId = model.ProjectMasterModel.ProjectMasterId;

            var orderQuery =
                _dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectMasterId == proId);
            var empId =
           _dbeEntities.CmnUsers.FirstOrDefault(x => x.CmnUserId == userId);
            var poQuery =
          _dbeEntities.ProjectPurchaseOrderForms.FirstOrDefault(x => x.ProjectMasterId == proId);

            var models = new Pm_PiClosing();

            models.ProjectId = proId;
            models.ProjectName = orderQuery.ProjectName;
            models.OrderNumber = orderQuery.OrderNuber;
            models.PoDate = poQuery.PoDate;
            models.PoCategory = poQuery.PoCategory;
            models.EmployeeCode = empId.EmployeeCode;
            models.ProjectManagerUserId = userId;
            models.Remarks = model.Remarks;
            models.UploadedFile = model.UploadedFile;
            models.ClosingDate = model.ClosingDate;
            models.ClosingType = model.ClosingType;
            models.ClosingAmount = 2000;
            models.Added = userId;
            models.AddedDate = DateTime.Now;

            _dbeEntities.Pm_PiClosing.AddOrUpdate(models);
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<Pm_PiClosingModel> GetPreviousPiClosingData()
        {
            string query1 =
            string.Format(@"select top 20 [ProjectId]
              ,[ProjectName]
              ,CONVERT(varchar(10),OrderNumber) as OrderNumber
              ,[PoDate]
              ,[PoCategory]
              ,[EmployeeCode]
              ,[ProjectManagerUserId]
              ,[Remarks]
              ,[UploadedFile]
              ,[ClosingType]
              ,[ClosingDate]
              ,[ClosingAmount]
              ,[AddedDate] FROM [CellPhoneProject].[dbo].[Pm_PiClosing] order by Id desc");

            var pmIncentive =
                _dbeEntities.Database.SqlQuery<Pm_PiClosingModel>(query1).ToList();

            return pmIncentive;
        }

        public List<Pm_PiClosingModel> GetPmPiIncentive(string employeeCode, string monthName, string monNum, string year)
        {
            //            string query1 =
            //            string.Format(@"SELECT ProjectId,ProjectName,CONVERT(varchar(10),OrderNumber) as OrderNumber,PoDate,PoCategory,EmployeeCode,ProjectManagerUserId,Remarks,UploadedFile,ClosingType,ClosingDate,ClosingAmount, month(ClosingDate) as MonthNo,
            //            datename(month, ClosingDate) as MonthName,year(ClosingDate) as Year FROM [CellPhoneProject].[dbo].[Pm_PiClosing]
            //            where EmployeeCode='{0}'  and  month(ClosingDate)='{1}' and year(ClosingDate)='{2}'", employeeCode, monNum,year);
            string query1 =
           string.Format(@"SELECT  ProjectId,ProjectName,PoDate,PoCategory,CONVERT(varchar(10),OrderNumber) as OrderNumber,ProjectManagerUserId,EmployeeCode,Remarks,ClosingType,ClosingDate,ClosingAmount,
           datename(month, ClosingDate) as MonthName
          FROM [CellPhoneProject].[dbo].[Pm_PiClosing]
            where EmployeeCode='{0}'  and  month(ClosingDate)='{1}' and year(ClosingDate)='{2}'", employeeCode, monNum, year);
            var pmIncentive =
                _dbeEntities.Database.SqlQuery<Pm_PiClosingModel>(query1).ToList();

            return pmIncentive;
        }

        public string SavePiDetails(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new Pm_PiClosingIncentive();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;
                model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);

                model.ProjectId = insResult.ProjectId;
                model.ProjectName = insResult.ProjectName;
                model.EmployeeCode = insResult.EmployeeCode;
                model.PoCategory = insResult.PoCategory;
                model.PoDate = insResult.PoDate;
                model.OrderNumber = insResult.OrderNumber;
                model.ProjectManagerUserId = insResult.ProjectManagerUserId;
                model.DepartmentName = "PM";
                model.Amount = !string.IsNullOrWhiteSpace(insResult.ClosingAmount) ? Convert.ToDecimal(insResult.ClosingAmount) : 0;
                model.Remarks = insResult.Remarks;
                model.DeductionAmount = !string.IsNullOrWhiteSpace(insResult.DeductionAmount) ? Convert.ToDecimal(insResult.DeductionAmount) : 0;
                model.D_Remarks = insResult.D_Remarks;
                model.ClosingType = insResult.ClosingType;
                model.ClosingDate = insResult.ClosingDate;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_PiClosingIncentive.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public bool GetPiData(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Pm_PiClosingIncentiveModel> getPiIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getPiIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year) as Year,EmployeeCode from [CellPhoneProject].[dbo].[Pm_PiClosingIncentive] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getPiIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Pm_PiClosingIncentiveModel>(getPiIncentiveReportQuery).ToList();

            }

            if (getPiIncentiveReports != null && getPiIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetAccessoriesSavedData(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Pm_PiClosingIncentiveModel> getPiIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getPiIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_Accessories] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getPiIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Pm_PiClosingIncentiveModel>(getPiIncentiveReportQuery).ToList();

            }

            if (getPiIncentiveReports != null && getPiIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<Custom_Pm_IncentiveModel> GetPmAccessoriesFinalIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_Accessories where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmPiFinalIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_PiClosingIncentive where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmAccessoriesIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
            string.Format(@"select pai.ProjectName,pai.PoCategory,pai.PoDate,OrderNumber,pai.Remarks,pai.Amount as Amount1, pai.FinalAmount as FinalAmount1,
            pai.DeductionAmount  as DeductionAmount1,pai.D_Remarks, ps.FlightDepartureDate as ShipmentTaken,pm.ApproxShipmentDate as LSD,IncsTypes='Accesssories'
            from CellPhoneProject.dbo.Pm_Accessories pai 
            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=pai.ProjectId 
            and ps.ProjectOrderShipmentId in (select top 1 ProjectOrderShipmentId from CellPhoneProject.dbo.ProjectOrderShipments where ProjectMasterId= pai.ProjectId
            order by ProjectOrderShipmentId desc) left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=pai.ProjectId
            where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum={1}", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            foreach (var project in getPmPoIncentiveModel)
            {
                project.OrderNumberOrdinal = project.OrderNumber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmPiIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
            string.Format(@"select pai.ProjectName,pai.PoCategory,pai.PoDate,OrderNumber,pai.Remarks,pai.Amount as Amount1, pai.FinalAmount as FinalAmount1,
            pai.DeductionAmount  as DeductionAmount1,pai.D_Remarks, ps.FlightDepartureDate as ShipmentTaken,pm.ApproxShipmentDate as LSD,IncsTypes='PiClosingIncs.'
            from CellPhoneProject.dbo.Pm_PiClosingIncentive pai 
            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=pai.ProjectId 
            and ps.ProjectOrderShipmentId in (select top 1 ProjectOrderShipmentId from CellPhoneProject.dbo.ProjectOrderShipments where ProjectMasterId= pai.ProjectId
            order by ProjectOrderShipmentId desc) left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=pai.ProjectId
            where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            foreach (var project in getPmPoIncentiveModel)
            {
                project.OrderNumberOrdinal = project.OrderNumber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNumber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return getPmPoIncentiveModel;
        }

        //new policies from sept 2019
        public List<NinetyFiveProductionRewardModel> GetProductionReward(string employeeCode, string monthName, string monNum, string year)
        {
            _dbeEntities.Database.CommandTimeout = 6000;
            string query1 =
          string.Format(@"select D.ProjectMasterID,D.ProjectModel,D.EmployeeCode,D.UserFullName,D.SourcingType,D.WpmsOrders,D.WarehouseEntryDate,D.ExtendedWarehouseDate,D.ProjectManagerUserId,D.OrderQuantity,
            D.TotalProductionQuantity,D.EffectiveDays,D.RewardPercentage,D.ExistedPercentage,cast(D.RewardAmount as bigint) as RewardAmount
            from
            (
               select distinct cast(C.ProjectMasterID as bigint) as ProjectMasterID,C.ProjectModel,C.EmployeeCode,C.UserFullName,C.SourcingType,C.WpmsOrders,C.WarehouseEntryDate,C.ExtendedWarehouseDate,cast(C.ProjectManagerUserId as bigint) as ProjectManagerUserId,
               cast(C.OrderQuantity as bigint) as OrderQuantity,cast(C.TotalProductionQuantity as bigint) as TotalProductionQuantity,cast(C.EffectiveDays as bigint) as EffectiveDays,cast(C.RewardPercentage as bigint) as RewardPercentage,cast(C.ExistedPercentage as bigint) as ExistedPercentage,
               case when C.ExistedPercentage>=C.RewardPercentage then 2100 else 0 end as RewardAmount
                 from
	             (
		            select B.ProjectMasterID,B.ProjectModel,B.EmployeeCode,B.UserFullName,B.SourcingType,B.WpmsOrders,B.WarehouseEntryDate,B.ExtendedWarehouseDate,B.ProjectManagerUserId,B.OrderQuantity,B.TotalProductionQuantity,B.EffectiveDays,B.RewardPercentage,
		            ((100 * B.TotalProductionQuantity)/OrderQuantity) as ExistedPercentage,B.IsFinalShipment
			              from 
				             (
					              select A.ProjectMasterID,A.ProjectModel,A.EmployeeCode,A.UserFullName,A.SourcingType,A.WpmsOrders,A.WarehouseEntryDate,A.ExtendedWarehouseDate,A.ProjectManagerUserId,A.IsFinalShipment,A.OrderQuantity,count(tbi.Barcode) as TotalProductionQuantity,RewardPercentage=95,A.EffectiveDays
					              from 
						            (
							            select AA.ProjectMasterID,AA.ProjectModel,AA.EmployeeCode,AA.UserFullName,AA.SourcingType,AA.WpmsOrders,AA.WarehouseEntryDate,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate) as ExtendedWarehouseDate,AA.ProjectManagerUserId,AA.IsFinalShipment,AA.OrderQuantity,AA.EffectiveDays from
							            (
								            select distinct ps.ProjectMasterID,pdd.ProjectModel,cm.EmployeeCode,cm.UserFullName,pm.SourcingType,('Order '+ cast(pm.OrderNuber as varchar(10))) as WpmsOrders,ps.WarehouseEntryDate,ppa.ProjectManagerUserId,ps.IsFinalShipment,pdd.OrderQuantity,case when pm.SourcingType like 'SKD' then 30  when  pm.SourcingType like 'CKD' then 45 end as EffectiveDays
								            from [CellPhoneProject].[dbo].[ProjectOrderShipments] ps
								            left join [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterID=ps.ProjectMasterID
								            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pdd.ProjectMasterID=ps.ProjectMasterID
								            left join [CellPhoneProject].[dbo].ProjectPmAssigns ppa on ppa.ProjectMasterID=pm.ProjectMasterID and ppa.Status='ASSIGNED'
								            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId and cm.IsActive=1  and cm.EmployeeCode='{0}'
								            where pm.IsActive=1 and							
								             ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
								             and  ppa.ProjectManagerUserId = (select  top 1  ProjectManagerUserId from  [CellPhoneProject].[dbo].ProjectPmAssigns  where ProjectMasterId=ppa.ProjectMasterId and Status='ASSIGNED' order by WarehouseEntryDate desc)

							            )AA where DATEPART(mm,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))='{1}' and  DATENAME(YEAR,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))='{2}' 

						            )A
						            left join RBSYNERGY.dbo.tblBarcodeInv tbi on tbi.UpdatedBy=A.WpmsOrders  and tbi.Model=A.ProjectModel
						            where tbi.Model=A.ProjectModel and PrintDate between A.WarehouseEntryDate and DATEADD(day, A.EffectiveDays, A.WarehouseEntryDate)
						            group by  A.ProjectMasterID,A.ProjectModel,A.WpmsOrders,A.WarehouseEntryDate,A.IsFinalShipment,A.OrderQuantity,A.SourcingType,A.EffectiveDays,A.ExtendedWarehouseDate,A.EmployeeCode,A.ProjectManagerUserId,A.UserFullName
				            )B

                  )C  
	  
            )D	 where D.RewardAmount>0 and D.EmployeeCode='{0}' order by ProjectMasterID asc", employeeCode, monNum, year);
            var pmIncentive = _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query1).ToList();

            // var mms = Convert.ToInt32(monNum);
            // long yers = Convert.ToInt64(year);

            //// string query1 = _dbeEntities.GetProductionRewardForPm(employeeCode, mms, yers).ToList();
            //// List<NinetyFiveProductionRewardModel> pmIncentive = _dbeEntities.GetProductionRewardForPm(employeeCode, mms, yers);
            // var pmIncentive = _dbeEntities.GetProductionRewardForPm(employeeCode, mms, yers).ToList();

            return pmIncentive;
        }

        public List<NinetyFiveProductionRewardModel> GetSoldOutRewardData(string employeeCode, string monthName, string monNum, string year)
        {
            _dbeEntities.Database.CommandTimeout = 6000;
            string query1 =
          string.Format(@" select E.ProjectmasterID,E.ProjectModel,E.EmployeeCode,E.UserFullName,E.Orders,E.tblBarcodeOrder,E.WarehouseEntryDate,E.ExtendedWarehouseDate,E.ProjectManagerUserId,E.EffectiveDays,
                        E.OrderQuantity,E.TotalTblBarcodeIMEI,E.TotalSalesOut,E.RewardPercentage,E.ExistedPercentage,cast(E.RewardAmount as bigint) as RewardAmount
                        from
                       (
 	                     SELECT cast(D.ProjectmasterId as bigint) as ProjectmasterID,D.ProjectModel,D.EmployeeCode,D.UserFullName,D.Orders,D.tblBarcodeOrder ,D.WarehouseEntryDate,D.ExtendedWarehouseDate,D.ProjectManagerUserId,cast(D.EffectiveDays as bigint) as EffectiveDays,
                            cast(D.OrderQuantity as bigint) as OrderQuantity, cast(D.TotalTblBarcodeIMEI as bigint) as TotalTblBarcodeIMEI,
                         cast(D.TotalSalesOut as bigint) as TotalSalesOut,cast(D.RewardPercentage as bigint) as RewardPercentage,cast(D.ExistedPercentage as bigint) as ExistedPercentage,
	                     case when D.ExistedPercentage>=D.RewardPercentage then 5000 else 0 end as RewardAmount
	                      FROM
	                       (
		                      select C.ProjectmasterId,C.ProjectModel,C.EmployeeCode,C.UserFullName,C.Orders,C.tblBarcodeOrder,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.ProjectManagerUserId,C.EffectiveDays,C.OrderQuantity, C.TotalTblBarcodeIMEI,C.TotalSalesOut,C.RewardPercentage,
		                      ((100 * C.TotalSalesOut)/OrderQuantity) as ExistedPercentage,IsFinalShipment  from
			                    ( 
			                       select B.ProjectmasterId,B.ProjectModel,B.EmployeeCode,B.UserFullName,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,B.ExtendedWarehouseDate,B.ProjectManagerUserId,EffectiveDays=120, sum(TotalTblBarcodeIMEI) as TotalTblBarcodeIMEI,sum(TotalSalesOut) as TotalSalesOut,RewardPercentage=95,IsFinalShipment,B.OrderQuantity  from
					                    ( 
					                       select A.ProjectmasterId,A.ProjectModel,A.EmployeeCode,A.UserFullName,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.ExtendedWarehouseDate,A.ProjectManagerUserId, count(A.Barcode) as TotalTblBarcodeIMEI,case when A.TddBarcode is not null and A.TddBarcode !='' then 1 else 0 end as TotalSalesOut,IsFinalShipment,A.OrderQuantity  from
							                     (
								                     select distinct proM.ProjectMasterId,proM.ProjectModel,proM.EmployeeCode,proM.UserFullName,proM.Orders,proM.ShipmentType,proM.WarehouseEntryDate,proM.ExtendedWarehouseDate,proM.ProjectManagerUserId,proM.ShipmentPercentage,proM.IsFinalShipment,
								                     tbl.Model,tbl.Barcode,tbl.Barcode2,tbl.DateAdded,tbl.UpdatedBy as tblBarcodeOrder,tdd.Barcode as TddBarcode,proM.OrderQuantity from 
								                    (
										                    select distinct ps.ProjectMasterId,pdd.ProjectModel,cm.EmployeeCode,cm.UserFullName, ('Order '+ cast(pm.OrderNuber as varchar(10))) as Orders,ps.ShipmentType,ps.WarehouseEntryDate,DATEADD(day, 120, ps.WarehouseEntryDate) AS ExtendedWarehouseDate,ppa.ProjectManagerUserId,ps.ShipmentPercentage,ps.IsFinalShipment,pdd.OrderQuantity
										                    FROM [CellPhoneProject].[dbo].[ProjectOrderShipments] ps 
										                    left join CellphoneProject.dbo.ProjectMasters pm on ps.ProjectMasterId=pm.ProjectMasterId
										                    left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pm.ProjectMasterID=pdd.ProjectMasterID
										                    left join [CellPhoneProject].[dbo].ProjectPmAssigns ppa on ppa.ProjectMasterID=pm.ProjectMasterID and ppa.Status='ASSIGNED'
										                    left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId and cm.IsActive=1 and cm.EmployeeCode='{0}'
										                    where pm.IsActive=1 and DATEPART(mm,DATEADD(day, 120, ps.WarehouseEntryDate))='{1}' and  DATENAME(YEAR,DATEADD(day, 120, ps.WarehouseEntryDate))='{2}' and 
										                    ps.WarehouseEntryDate in (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
								                    )proM
								                    left join [RBSYNERGY].[dbo].[tblBarCodeInv] tbl on proM.ProjectModel=tbl.Model and RTRIM(tbl.UpdatedBy)=RTRIM(proM.Orders)
								                    left join [RBSYNERGY].[dbo].tblDealerDistributionDetails tdd on tbl.Barcode =tdd.Barcode and
								                    tdd.DistributionDate between proM.WarehouseEntryDate and  DATEADD(day, 120, proM.WarehouseEntryDate)

								                    where proM.ProjectModel=tbl.Model 
							                    )A where A.EmployeeCode='{0}'
						                    group by A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.Barcode,A.TddBarcode,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity,A.EmployeeCode,A.UserFullName,A.ProjectManagerUserId
					                     )B
					                     group by B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,ExtendedWarehouseDate,IsFinalShipment,B.OrderQuantity,B.EmployeeCode,B.UserFullName,B.ProjectManagerUserId
			                       )C 
	                       )D
                        )E where E.RewardAmount>0 order by E.ProjectModel asc", employeeCode, monNum, year);
            //this.CommandTimeout = 300;
            var pmIncentive =
                _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query1).ToList();

            return pmIncentive;
        }
        public string SaveProductionRewardData(List<NinetyFiveProductionRewardModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            foreach (var insResult in results)
            {
                var model = new NinetyFivePercentProductionReward();

                model.ProjectMasterId = insResult.ProjectMasterID;
                model.ProjectModel = insResult.ProjectModel;
                model.EmployeeCode = insResult.EmployeeCode;
                model.UserFullName = insResult.UserFullName;
                model.SourcingType = insResult.SourcingType;
                model.WpmsOrders = insResult.WpmsOrders;
                model.WarehouseEntryDate = insResult.WarehouseEntryDate;
                model.ExtendedWarehouseDate = insResult.ExtendedWarehouseDate;
                model.UserId = insResult.ProjectManagerUserId;
                model.OrderQuantity = insResult.OrderQuantity;
                model.TotalProductionQuantity = insResult.TotalProductionQuantity;
                model.EffectiveDays = insResult.EffectiveDays;
                model.RewardPercentage = insResult.RewardPercentage;
                model.ExistedPercentage = insResult.ExistedPercentage;
                model.RewardAmount = insResult.RewardAmount;
                model.DeductAmount = insResult.DeductAmount;
                model.RoleName = "PM";

                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;

                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.NinetyFivePercentProductionRewards.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();

            return "ok";
        }
        public string SaveSalesOutRewardData(List<NinetyFiveProductionRewardModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new NinetyFivePercentSalesOutReward();

                model.ProjectMasterId = insResult.ProjectMasterID;
                model.ProjectModel = insResult.ProjectModel;
                model.EmployeeCode = insResult.EmployeeCode;
                model.UserFullName = insResult.UserFullName;
                model.Orders = insResult.Orders;
                model.WarehouseEntryDate = insResult.WarehouseEntryDate;
                model.ExtendedWarehouseDate = insResult.ExtendedWarehouseDate;
                model.UserId = insResult.ProjectManagerUserId;
                model.OrderQuantity = insResult.OrderQuantity;
                model.TotalTblBarcodeIMEI = insResult.TotalTblBarcodeIMEI;
                model.TotalSalesOut = insResult.TotalSalesOut;
                model.EffectiveDays = insResult.EffectiveDays;
                model.RewardPercentage = insResult.RewardPercentage;
                model.ExistedPercentage = insResult.ExistedPercentage;
                model.RewardAmount = insResult.RewardAmount;
                model.DeductAmount = insResult.DeductAmount;
                model.RoleName = "PM";

                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;

                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.NinetyFivePercentSalesOutRewards.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<NinetyFiveProductionRewardModel> GetPmAndQcLsdToVesselData(string employeeCode, string monthName, string monNum, string year)
        {
            _dbeEntities.Database.CommandTimeout = 6000;
            string query1 =
        string.Format(@"select * from
	          (
		        select distinct cast(A.ProjectMasterId as bigint) as ProjectMasterID,A.ProjectName,A.EmployeeCode,A.UserFullName,cast(A.ProjectManagerUserId as bigint) as ProjectManagerUserId,A.ProjectType,cast(A.Orders as varchar(10)) as Orders,A.PoDate,A.LSD,cast (A.PoVsLSDDiff as int) as PoVsLSDDiff,A.VesselDate,cast(A.LsdVsVesselDiffForDeduct as int) as LsdVsVesselDiffForDeduct,
		        cast(A.DeductPoint as bigint) as DeductPoint,cast((A.DeductPoint * A.LsdVsVesselDiffForDeduct) as bigint) as DeductedAmount,
	            cast(A.LsdVsVesselDiffForReward as int) as LsdVsVesselDiffForReward,cast(A.RewardPoint as bigint) as RewardPoint,cast((A.RewardPoint * A.LsdVsVesselDiffForReward) as bigint) as RewardAmount 
	            from 
		        (
				        select distinct ppf.ProjectMasterId,pm.ProjectName,cm.EmployeeCode,cm.UserFullName,ppa.ProjectManagerUserId,pm.ProjectType,pm.OrderNuber as Orders,ppf.PoDate,pm.ApproxShipmentDate as LSD,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
				        case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,
				        case when pm.ApproxShipmentDate>=ps.VesselDate then (DATEDIFF(day, ps.VesselDate, pm.ApproxShipmentDate)) else 0 end as LsdVsVesselDiffForReward,
				        ps.ShipmentType,				
				        DeductPoint=100,RewardPoint=500

				        from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
				        left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
				        left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
				        left join [CellPhoneProject].[dbo].ProjectPmAssigns ppa on ppa.ProjectMasterID=pm.ProjectMasterID and ppa.Status='ASSIGNED'
                        left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ppa.ProjectManagerUserId and cm.IsActive=1 and cm.EmployeeCode='{0}'
				        where pm.IsActive=1 and 
				        pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
				        and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)

				        and DATEPART(mm,ps.VesselDate)='{1}' and  DATENAME(YEAR,ps.VesselDate)='{2}'
		        )A where A.EmployeeCode='{0}'
		
	        )B where (B.DeductedAmount+B.RewardAmount)>0", employeeCode, monNum, year);
            var pmIncentive =
                _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query1).ToList();

            return pmIncentive;
        }

        public string SaveVesselRewardOrPenaltiesData(List<NinetyFiveProductionRewardModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new PmAndQcLsdToVesselData();

                model.ProjectMasterId = insResult.ProjectMasterID;
                model.ProjectModel = insResult.ProjectName;
                model.EmployeeCode = insResult.EmployeeCode;
                model.UserFullName = insResult.UserFullName;
                model.UserId = insResult.ProjectManagerUserId;
                model.Orders = insResult.Orders;
                model.ProjectType = insResult.ProjectType;
                model.PoDate = insResult.PoDate;
                model.LSD = insResult.LSD;
                model.PoVsLSDDiff = insResult.PoVsLSDDiff;
                model.VesselDate = insResult.VesselDate;
                model.LsdVsVesselDiffForDeduct = insResult.LsdVsVesselDiffForDeduct;
                model.DeductPoint = insResult.DeductPoint;
                model.DeductedAmount = insResult.DeductedAmount;
                model.LsdVsVesselDiffForReward = insResult.LsdVsVesselDiffForReward;
                model.RewardPoint = insResult.RewardPoint;
                model.RewardAmount = insResult.RewardAmount;
                model.RoleName = "PM";

                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;

                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.PmAndQcLsdToVesselDatas.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public bool ProductionRewardDataCheck(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<NinetyFiveProductionRewardModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[NinetyFivePercentProductionReward] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool SalesOutRewardDataCheck(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<NinetyFiveProductionRewardModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[NinetyFivePercentSalesOutReward] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool VesselRewardOrPenaltiesDataCheck(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<NinetyFiveProductionRewardModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[PmAndQcLsdToVesselData] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<Custom_Pm_IncentiveModel> GetPmHeadPercentage(string employeeCode, string monthName, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            var qq =
                (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == employeeCode select cc).FirstOrDefault();
            var getPmPoIncentiveModel = new List<Custom_Pm_IncentiveModel>();

            if (qq.RoleName == "PMHEAD")
            {
                string query =
            string.Format(@"
                  select case when TeamIncentive is null then 0 else TeamIncentive end as TeamIncentive,
                  case when InchargePecentage is null then 0 else InchargePecentage end as InchargePecentage from
                 (
                    select cast(TotalIncentive as decimal(18,2)) as TeamIncentive, cast(((TotalIncentive*30)/100) as decimal(18,2)) as InchargePecentage
                    from
                    (
	                    select sum(TotalIncentive) as TotalIncentive
	                    from CellPhoneProject.dbo.Incentive where  MonNum='{1}' and Year='{2}' and DepartmentName='PM'
                    )A
                 )B
                ", employeeCode, monNums, years);
                getPmPoIncentiveModel = _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            }
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmProductionRewardIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(RewardAmount) as decimal(18,2)) as FinalAmount1 from CellPhoneProject.dbo.NinetyFivePercentProductionReward where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmProductionDeductIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(DeductAmount) as decimal(18,2)) as Penalties from CellPhoneProject.dbo.NinetyFivePercentProductionReward where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmSalesOutRewardIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(RewardAmount) as decimal(18,2)) as FinalAmount1 from CellPhoneProject.dbo.NinetyFivePercentSalesOutReward where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmSalesOutDeductIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(DeductAmount) as decimal(18,2)) as Penalties from CellPhoneProject.dbo.NinetyFivePercentSalesOutReward where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmLsdToVesselRewardIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(RewardAmount)  as decimal(18,2)) as FinalAmount1 from CellPhoneProject.dbo.[PmAndQcLsdToVesselData] where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmLsdToVesselPenaltiesIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(DeductedAmount) as decimal(18,2)) as Penalties from CellPhoneProject.dbo.[PmAndQcLsdToVesselData] where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<NinetyFiveProductionRewardModel> GetPerPoRewardSumForPmHead(string employeeCode, string monthName, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            var qq =
                (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == employeeCode select cc).FirstOrDefault();
            string query = "";
            var getPmPoIncentiveModel = new List<NinetyFiveProductionRewardModel>();
            if (qq.RoleName == "PMHEAD")
            {
                query =
                string.Format(@"select cast(sum(A.PoReward) as bigint) as PoReward from
                (select distinct ppo.ProjectMasterId,pm.ProjectName,cast(pm.OrderNuber as varchar(10))+' Order' as Orders, ppo.Podate,PoReward=1000 from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo
                left join  [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterId=ppo.ProjectMasterId
                where pm.IsActive=1 and  DATEPART(mm,PoDate)='{0}' and  DATENAME(YEAR,PoDate)='{1}' )A", monNums, years);

                getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query).ToList();

            }

            return getPmPoIncentiveModel;
        }

        public List<NinetyFiveProductionRewardModel> GetPoDetailsPageForPmHead(string empCode, string month, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            var qq =
               (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == empCode select cc).FirstOrDefault();
            string query = "";
            var getPmPoIncentiveModel = new List<NinetyFiveProductionRewardModel>();

            if (qq.RoleName == "PMHEAD")
            {
                query =
                    string.Format(@"select ProjectMasterID,ProjectName,Orders,PoDate,cast(PoReward as bigint) as PoReward from
                    (select distinct ppo.ProjectMasterId as ProjectMasterID,pm.ProjectName,cast(pm.OrderNuber as varchar(10))+' Order' as Orders, ppo.PoDate,PoReward=1000 from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo
                    left join  [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterId=ppo.ProjectMasterId
                    where pm.IsActive=1 and DATEPART(mm,PoDate)='{0}' and  DATENAME(YEAR,PoDate)='{1}')A ORDER by PoDate asc", monNums, years);

                getPmPoIncentiveModel =
                    _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query).ToList();
            }

            return getPmPoIncentiveModel;
        }

        public string SavePoDetailsPageForPmHead(List<NinetyFiveProductionRewardModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var qq =
              (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == insResult.EmployeeCode select cc).FirstOrDefault();


                if (qq.RoleName == "PMHEAD")
                {
                    string query =
                 string.Format(@"select ProjectMasterID,ProjectName,Orders,PoDate,cast(PoReward as bigint) as PoReward from
                    (select distinct ppo.ProjectMasterId as ProjectMasterID,pm.ProjectName,cast(pm.OrderNuber as varchar(10))+' Order' as Orders, ppo.PoDate,PoReward=1000 from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo
                    left join  [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterId=ppo.ProjectMasterId
                    where pm.IsActive=1 and  DATEPART(mm,PoDate)='{0}' and  DATENAME(YEAR,PoDate)='{1}')A ORDER by PoDate asc", insResult.MonNum, insResult.Year);

                    var getPmPoIncentiveModel =
                          _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query).ToList();

                    foreach (var rewards in getPmPoIncentiveModel)
                    {
                        var model = new PmHead_PerPoIncentive();

                        model.ProjectMasterID = rewards.ProjectMasterID;
                        model.ProjectName = rewards.ProjectName;
                        model.PoDate = rewards.PoDate;
                        model.Orders = rewards.Orders;
                        model.PoReward = rewards.PoReward;
                        model.EmployeeCode = insResult.EmployeeCode;
                        model.RoleName = "PMHEAD";
                        model.MonNum = insResult.MonNum;
                        model.Month = insResult.Month;
                        model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;

                        model.Added = userId;
                        model.AddedDate = DateTime.Now;
                        _dbeEntities.PmHead_PerPoIncentive.AddOrUpdate(model);
                    }
                }

                _dbeEntities.SaveChanges();
            }


            return "ok";
        }

        public bool PoDetailsPageForPmHeadDataCheck(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<NinetyFiveProductionRewardModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[PmHead_PerPoIncentive] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(getIncentiveReportQuery).ToList();
            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<Custom_Pm_IncentiveModel> GetPmHead_PerPoIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(PoReward)  as decimal(18,2)) as FinalAmount1 from CellPhoneProject.dbo.[PmHead_PerPoIncentive] where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmHead_TeamPercentInc(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(HeadPercentages)  as decimal(18,2)) as FinalAmount1 from CellPhoneProject.dbo.[Pm_HeadTeamIncentivePercentage] where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<NinetyFiveProductionRewardModel> GetPenaltiesSumForPmHead(string employeeCode, string monthName, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            var qq =
                (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == employeeCode select cc).FirstOrDefault();
            string query = "";
            var getPmPoIncentiveModel = new List<NinetyFiveProductionRewardModel>();
            if (qq.RoleName == "PMHEAD")
            {

                //                query =
                //           string.Format(@"	
                //                select cast(sum(D.TotalPenalties) as bigint) as Penalties from
                //                (
                //	                select C.ProjectMasterID,C.ProjectName,(select EmployeeCode from CellPhoneProject.dbo.CmnUsers cu where RoleName='PMHEAD' AND IsActive=1) AS EmployeeCode,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.ApproxShipmentDate as LSD,
                //                    C.PoVsLSDDiff,C.VesselDate,C.LsdVsVesselDiffForDeduct,C.DeductPoint,C.FeatureBase,C.SmartBase,C.PoReward,C.PerDayDeduction, (C.PerDayDeduction*C.LsdVsVesselDiffForDeduct) AS TotalPenalties
                //	                 from
                //	                (
                //		                select B.ProjectMasterId as ProjectMasterID,B.ProjectName,B.ProjectType,B.ShipmentType ,B.OrderNuber as Orders,B.PoDate,B.ApproxShipmentDate,
                //		                B.PoVsLSDDiff,B.VesselDate,B.LsdVsVesselDiffForDeduct,B.DeductPoint,B.FeatureBase,B.SmartBase,B.PoReward,
                //		                case when ProjectType='Feature' then ((PoReward * DeductPoint)/ FeatureBase) when ProjectType='Smart' then ((PoReward * DeductPoint)/ SmartBase) end as PerDayDeduction	
                //		                 from
                //		                 (	
                //			                select distinct A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType ,A.OrderNuber,A.PoDate,A.ApproxShipmentDate,A.PoVsLSDDiff,A.VesselDate,A.LsdVsVesselDiffForDeduct,A.DeductPoint,FeatureBase=3000,SmartBase=5000,PoReward=1000
                //			                from
                //
                //				                (
                //					                select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.OrderNuber,ppf.PoDate,pm.ApproxShipmentDate,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
                //					                case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,
                //			
                //					                ps.ShipmentType,				
                //					                DeductPoint=100
                //
                //					                from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
                //					                left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
                //					                left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
                //					                where pm.IsActive=1 and
                //					                pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
                //					                and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
                //
                //					                and DATEPART(mm,ps.VesselDate)='{0}' and  DATENAME(YEAR,ps.VesselDate)='{1}'
                //				                )A  where A.LsdVsVesselDiffForDeduct>0
                //		                  )B
                //                      )C  
                //                  )D", monNums, years);

                query =
           string.Format(@" select cast(sum(E.Penalties) as bigint) as Penalties 
 from
 (
	select D.ProjectMasterID,D.ProjectName,D.EmployeeCode,D.ProjectType,D.ShipmentType,D.Orders,D.PoDate,D.LSD,
	D.PoVsLSDDiff,D.VesselDate,D.LsdVsVesselDiffForDeduct,D.DeductPoint,D.FeatureBase,D.SmartBase,D.PoReward,D.PerDayDeduction,D.Penalties1,D.PmPenal,
	case when D.PmPenal='no' and (D.Penalties1>1000) then D.PoReward else D.Penalties1 end as Penalties
	from
	(
	select C.ProjectMasterID,C.ProjectName,(select EmployeeCode from CellPhoneProject.dbo.CmnUsers cu where RoleName='PMHEAD' AND IsActive=1) AS EmployeeCode,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.ApproxShipmentDate as LSD,
	C.PoVsLSDDiff,C.VesselDate,C.LsdVsVesselDiffForDeduct,cast(C.DeductPoint as bigint) as DeductPoint,cast(C.FeatureBase as bigint) as FeatureBase,cast(C.SmartBase as bigint) as SmartBase,cast(C.PoReward as bigint) as PoReward,cast(C.PerDayDeduction as bigint) as PerDayDeduction, cast((C.PerDayDeduction * C.LsdVsVesselDiffForDeduct) as bigint) AS Penalties1,PmPenal

	from
	(
		select B.ProjectMasterId as ProjectMasterID,B.ProjectName,B.ProjectType,B.ShipmentType ,cast(B.OrderNuber as varchar(10))+' Order' as Orders,B.PoDate,B.ApproxShipmentDate,
		B.PoVsLSDDiff,B.VesselDate,B.LsdVsVesselDiffForDeduct,B.DeductPoint,B.FeatureBase,B.SmartBase,B.PoReward,
		case when ProjectType='Feature' then ((PoReward * DeductPoint)/ FeatureBase) when ProjectType='Smart' then ((PoReward * DeductPoint)/ SmartBase) end as PerDayDeduction,PmPenal	
			from
			(	
			  select distinct A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType ,A.OrderNuber,A.PoDate,A.ApproxShipmentDate,A.PoVsLSDDiff,A.VesselDate,A.LsdVsVesselDiffForDeduct,A.DeductPoint,FeatureBase=3000,SmartBase=5000,PoReward=1000,PmPenal
			  from
				(
					select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.OrderNuber,ppf.PoDate,pm.ApproxShipmentDate,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
					case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,ps.ShipmentType,DeductPoint=100,
					case when ppa.ProjectManagerUserId=cu.CmnUserId and cu.Rolename='PMHEAD' and cu.IsActive=1	then 'yes' else 'no' end as PmPenal
													
					from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
					left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
					left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
					left join CellPhoneProject.dbo.[ProjectPmAssigns] ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status='ASSIGNED'
					left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status='ASSIGNED' and cu.Rolename='PMHEAD' and cu.IsActive=1

					where  pm.IsActive=1 and
					pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
					and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)

					and DATEPART(mm,ps.VesselDate)='{0}' and  DATENAME(YEAR,ps.VesselDate)='{1}'
				)A  where A.LsdVsVesselDiffForDeduct>0
			)B
		)C

	 )D
)E", monNums, years);
                getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query).ToList();

            }

            return getPmPoIncentiveModel;
        }

        public List<NinetyFiveProductionRewardModel> GetRatioWisePenaltiesForPmHead(string empCode, string month, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            var qq =
               (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == empCode select cc).FirstOrDefault();
            string query = "";
            var getPmPoIncentiveModel = new List<NinetyFiveProductionRewardModel>();

            if (qq.RoleName == "PMHEAD")
            {
                //                query =
                //                    string.Format(@"select C.ProjectMasterID,C.ProjectName,(select EmployeeCode from CellPhoneProject.dbo.CmnUsers cu where RoleName='PMHEAD' AND IsActive=1) AS EmployeeCode,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.ApproxShipmentDate as LSD,
                //                    C.PoVsLSDDiff,C.VesselDate,C.LsdVsVesselDiffForDeduct,cast(C.DeductPoint as bigint) as DeductPoint,cast(C.FeatureBase as bigint) as FeatureBase,cast(C.SmartBase as bigint) as SmartBase,cast(C.PoReward as bigint) as PoReward,cast(C.PerDayDeduction as bigint) as PerDayDeduction, cast((C.PerDayDeduction*C.LsdVsVesselDiffForDeduct) as bigint) AS Penalties
                //	                 from
                //	                (
                //		                select B.ProjectMasterId as ProjectMasterID,B.ProjectName,B.ProjectType,B.ShipmentType ,cast(B.OrderNuber as varchar(10))+' Order' as Orders,B.PoDate,B.ApproxShipmentDate,
                //		                B.PoVsLSDDiff,B.VesselDate,B.LsdVsVesselDiffForDeduct,B.DeductPoint,B.FeatureBase,B.SmartBase,B.PoReward,
                //		                case when ProjectType='Feature' then ((PoReward * DeductPoint)/ FeatureBase) when ProjectType='Smart' then ((PoReward * DeductPoint)/ SmartBase) end as PerDayDeduction	
                //		                 from
                //		                 (	
                //			                select distinct A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType ,A.OrderNuber,A.PoDate,A.ApproxShipmentDate,A.PoVsLSDDiff,A.VesselDate,A.LsdVsVesselDiffForDeduct,A.DeductPoint,FeatureBase=3000,SmartBase=5000,PoReward=1000
                //			                from
                //
                //				                (
                //					                select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.OrderNuber,ppf.PoDate,pm.ApproxShipmentDate,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
                //					                case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,
                //			
                //					                ps.ShipmentType,				
                //					                DeductPoint=100
                //
                //					                from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
                //					                left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
                //					                left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
                //					                where  pm.IsActive=1 and
                //					                pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
                //					                and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
                //
                //					                and DATEPART(mm,ps.VesselDate)='{0}' and  DATENAME(YEAR,ps.VesselDate)='{1}'
                //				                )A  where A.LsdVsVesselDiffForDeduct>0
                //		                  )B
                //                      )C", monNums, years);
                query =
                   string.Format(@"
	                select D.ProjectMasterID,D.ProjectName,D.EmployeeCode,D.ProjectType,D.ShipmentType,D.Orders,D.PoDate,D.LSD,
	                D.PoVsLSDDiff,D.VesselDate,D.LsdVsVesselDiffForDeduct,D.DeductPoint,D.FeatureBase,D.SmartBase,D.PoReward,D.PerDayDeduction,D.Penalties1,D.PmPenal,
	                case when D.PmPenal='no' and (D.Penalties1>1000) then D.PoReward else D.Penalties1 end as Penalties
	                from
	                (
	                select C.ProjectMasterID,C.ProjectName,(select EmployeeCode from CellPhoneProject.dbo.CmnUsers cu where RoleName='PMHEAD' AND IsActive=1) AS EmployeeCode,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.ApproxShipmentDate as LSD,
	                C.PoVsLSDDiff,C.VesselDate,C.LsdVsVesselDiffForDeduct,cast(C.DeductPoint as bigint) as DeductPoint,cast(C.FeatureBase as bigint) as FeatureBase,cast(C.SmartBase as bigint) as SmartBase,cast(C.PoReward as bigint) as PoReward,cast(C.PerDayDeduction as bigint) as PerDayDeduction, cast((C.PerDayDeduction * C.LsdVsVesselDiffForDeduct) as bigint) AS Penalties1,PmPenal

	                from
	                (
		                select B.ProjectMasterId as ProjectMasterID,B.ProjectName,B.ProjectType,B.ShipmentType ,cast(B.OrderNuber as varchar(10))+' Order' as Orders,B.PoDate,B.ApproxShipmentDate,
		                B.PoVsLSDDiff,B.VesselDate,B.LsdVsVesselDiffForDeduct,B.DeductPoint,B.FeatureBase,B.SmartBase,B.PoReward,
		                case when ProjectType='Feature' then ((PoReward * DeductPoint)/ FeatureBase) when ProjectType='Smart' then ((PoReward * DeductPoint)/ SmartBase) end as PerDayDeduction,PmPenal	
			                from
			                (	
			                  select distinct A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType ,A.OrderNuber,A.PoDate,A.ApproxShipmentDate,A.PoVsLSDDiff,A.VesselDate,A.LsdVsVesselDiffForDeduct,A.DeductPoint,FeatureBase=3000,SmartBase=5000,PoReward=1000,PmPenal
			                  from
				                (
					                select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.OrderNuber,ppf.PoDate,pm.ApproxShipmentDate,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
					                case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,ps.ShipmentType,DeductPoint=100,
					                case when ppa.ProjectManagerUserId=cu.CmnUserId and cu.Rolename='PMHEAD' and cu.IsActive=1	then 'yes' else 'no' end as PmPenal
													
					                from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
					                left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
					                left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
					                left join CellPhoneProject.dbo.[ProjectPmAssigns] ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status='ASSIGNED'
					                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status='ASSIGNED' and cu.Rolename='PMHEAD' and cu.IsActive=1

					                where  pm.IsActive=1 and
					                pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
					                and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)

					                and DATEPART(mm,ps.VesselDate)='{0}' and  DATENAME(YEAR,ps.VesselDate)='{1}'
				                )A  where A.LsdVsVesselDiffForDeduct>0
			                )B
		                )C

	                 )D
                ", monNums, years);

                getPmPoIncentiveModel =
                    _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query).ToList();
            }

            return getPmPoIncentiveModel;
        }

        public bool PenaltiesDetailsPageForPmHeadCheck(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<NinetyFiveProductionRewardModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[PmHead_VesselPenaltiesForPo] where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(getIncentiveReportQuery).ToList();
            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SavePenaltiesDetailsPageForPmHead(List<NinetyFiveProductionRewardModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var qq =
              (from cc in _dbeEntities.CmnUsers where cc.EmployeeCode == insResult.EmployeeCode select cc).FirstOrDefault();


                if (qq.RoleName == "PMHEAD")
                {
                    //                    string query =
                    //                 string.Format(@"select C.ProjectMasterID,C.ProjectName,(select EmployeeCode from CellPhoneProject.dbo.CmnUsers cu where RoleName='PMHEAD' AND IsActive=1) AS EmployeeCode,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.ApproxShipmentDate as LSD,
                    //                    C.PoVsLSDDiff,C.VesselDate,C.LsdVsVesselDiffForDeduct,cast(C.DeductPoint as bigint) as DeductPoint,cast(C.FeatureBase as bigint) as FeatureBase,cast(C.SmartBase as bigint) as SmartBase,cast(C.PoReward as bigint) as PoReward,cast(C.PerDayDeduction as bigint) as PerDayDeduction, cast((C.PerDayDeduction * C.LsdVsVesselDiffForDeduct) as bigint) AS Penalties
                    //	                 from
                    //	                (
                    //		                select B.ProjectMasterId as ProjectMasterID,B.ProjectName,B.ProjectType,B.ShipmentType ,cast(B.OrderNuber as varchar(10))+' Order' as Orders,B.PoDate,B.ApproxShipmentDate,
                    //		                B.PoVsLSDDiff,B.VesselDate,B.LsdVsVesselDiffForDeduct,B.DeductPoint,B.FeatureBase,B.SmartBase,B.PoReward,
                    //		                case when ProjectType='Feature' then ((PoReward * DeductPoint)/ FeatureBase) when ProjectType='Smart' then ((PoReward * DeductPoint)/ SmartBase) end as PerDayDeduction	
                    //		                 from
                    //		                 (	
                    //			                select distinct A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType ,A.OrderNuber,A.PoDate,A.ApproxShipmentDate,A.PoVsLSDDiff,A.VesselDate,A.LsdVsVesselDiffForDeduct,A.DeductPoint,FeatureBase=3000,SmartBase=5000,PoReward=1000
                    //			                from
                    //
                    //				                (
                    //					                select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.OrderNuber,ppf.PoDate,pm.ApproxShipmentDate,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
                    //					                case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,
                    //			
                    //					                ps.ShipmentType,				
                    //					                DeductPoint=100
                    //
                    //					                from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
                    //					                left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
                    //					                left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
                    //					                where  pm.IsActive=1 and
                    //					                pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
                    //					                and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
                    //
                    //					                and DATEPART(mm,ps.VesselDate)='{0}' and  DATENAME(YEAR,ps.VesselDate)='{1}'
                    //				                )A  where A.LsdVsVesselDiffForDeduct>0
                    //		                  )B
                    //                      )C", insResult.MonNum, insResult.Year);

                    string query =
               string.Format(@"select D.ProjectMasterID,D.ProjectName,D.EmployeeCode,D.ProjectType,D.ShipmentType,D.Orders,D.PoDate,D.LSD,
                D.PoVsLSDDiff,D.VesselDate,D.LsdVsVesselDiffForDeduct,D.DeductPoint,D.FeatureBase,D.SmartBase,D.PoReward,D.PerDayDeduction,D.Penalties1,D.PmPenal,
                case when D.PmPenal='no' and (D.Penalties1>1000) then D.PoReward else D.Penalties1 end as Penalties
                from
                (
                select C.ProjectMasterID,C.ProjectName,(select EmployeeCode from CellPhoneProject.dbo.CmnUsers cu where RoleName='PMHEAD' AND IsActive=1) AS EmployeeCode,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.ApproxShipmentDate as LSD,
                C.PoVsLSDDiff,C.VesselDate,C.LsdVsVesselDiffForDeduct,cast(C.DeductPoint as bigint) as DeductPoint,cast(C.FeatureBase as bigint) as FeatureBase,cast(C.SmartBase as bigint) as SmartBase,cast(C.PoReward as bigint) as PoReward,cast(C.PerDayDeduction as bigint) as PerDayDeduction, cast((C.PerDayDeduction * C.LsdVsVesselDiffForDeduct) as bigint) AS Penalties1,PmPenal

                from
                (
	                select B.ProjectMasterId as ProjectMasterID,B.ProjectName,B.ProjectType,B.ShipmentType ,cast(B.OrderNuber as varchar(10))+' Order' as Orders,B.PoDate,B.ApproxShipmentDate,
	                B.PoVsLSDDiff,B.VesselDate,B.LsdVsVesselDiffForDeduct,B.DeductPoint,B.FeatureBase,B.SmartBase,B.PoReward,
	                case when ProjectType='Feature' then ((PoReward * DeductPoint)/ FeatureBase) when ProjectType='Smart' then ((PoReward * DeductPoint)/ SmartBase) end as PerDayDeduction,PmPenal	
		                from
		                (	
		                  select distinct A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType ,A.OrderNuber,A.PoDate,A.ApproxShipmentDate,A.PoVsLSDDiff,A.VesselDate,A.LsdVsVesselDiffForDeduct,A.DeductPoint,FeatureBase=3000,SmartBase=5000,PoReward=1000,PmPenal
		                  from
			                (
				                select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.OrderNuber,ppf.PoDate,pm.ApproxShipmentDate,((DATEDIFF(day, ppf.PoDate, pm.ApproxShipmentDate))) as PoVsLSDDiff,ps.VesselDate,
				                case when pm.ApproxShipmentDate<=ps.VesselDate then (DATEDIFF(day, pm.ApproxShipmentDate, ps.VesselDate)) else 0 end as LsdVsVesselDiffForDeduct,ps.ShipmentType,DeductPoint=100,
				                case when ppa.ProjectManagerUserId=cu.CmnUserId and cu.Rolename='PMHEAD' and cu.IsActive=1	then 'yes' else 'no' end as PmPenal
													
				                from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
				                left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
				                left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
			                    left join CellPhoneProject.dbo.[ProjectPmAssigns] ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status='ASSIGNED'
			                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status='ASSIGNED' and cu.Rolename='PMHEAD' and cu.IsActive=1

				                where  pm.IsActive=1 and
				                pm.ApproxShipmentDate = (select  top 1  ApproxShipmentDate from CellPhoneProject.dbo.ProjectMasters  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)
				                and ps.VesselDate= (select  top 1  VesselDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by ProjectOrderShipmentId asc)

				                and DATEPART(mm,ps.VesselDate)='{0}' and  DATENAME(YEAR,ps.VesselDate)='{1}'
			                )A  where A.LsdVsVesselDiffForDeduct>0
		                )B
                    )C

                 )D", insResult.MonNum, insResult.Year);
                    var getPmPoIncentiveModel =
                          _dbeEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(query).ToList();

                    foreach (var penalties in getPmPoIncentiveModel)
                    {
                        var model = new PmHead_VesselPenaltiesForPo();

                        model.ProjectMasterID = penalties.ProjectMasterID;
                        model.ProjectName = penalties.ProjectName;

                        model.ProjectType = penalties.ProjectType;
                        model.ShipmentType = penalties.ShipmentType;
                        model.Orders = penalties.Orders;
                        model.PoDate = penalties.PoDate;
                        model.LSD = penalties.LSD;
                        model.PoVsLSDDiff = penalties.PoVsLSDDiff;
                        model.VesselDate = penalties.VesselDate;
                        model.LsdVsVesselDiffForDeduct = penalties.LsdVsVesselDiffForDeduct;
                        model.DeductPoint = penalties.DeductPoint;
                        model.FeatureBase = penalties.FeatureBase;
                        model.SmartBase = penalties.SmartBase;
                        model.PoReward = penalties.PoReward;
                        model.PerDayDeduction = penalties.PerDayDeduction;
                        model.TotalPenalties = penalties.Penalties;

                        model.EmployeeCode = insResult.EmployeeCode;
                        model.RoleName = "PMHEAD";
                        model.MonNum = insResult.MonNum;
                        model.Month = insResult.Month;
                        model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;

                        model.Added = userId;
                        model.AddedDate = DateTime.Now;
                        _dbeEntities.PmHead_VesselPenaltiesForPo.AddOrUpdate(model);
                    }
                }

                _dbeEntities.SaveChanges();
            }


            return "ok";
        }

        public List<Custom_Pm_IncentiveModel> GetSumOfVesselPenaltiesIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cast(sum(TotalPenalties) as decimal(18,2)) as Penalties from CellPhoneProject.dbo.[PmHead_VesselPenaltiesForPo] where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetProductionIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
            string.Format(@"select ProjectMasterID,ProjectModel as ProjectName,EmployeeCode,UserFullName,WpmsOrders,SourcingType,
           WarehouseEntryDate,ExtendedWarehouseDate,UserId,OrderQuantity,TotalProductionQuantity,EffectiveDays,
           RewardPercentage,ExistedPercentage,RewardAmount,case when DeductAmount is not null then -DeductAmount else RewardAmount end as FinalAmount2,RoleName,IncsTypes='ProductionReward' from [CellPhoneProject].[dbo].[NinetyFivePercentProductionReward] pai
            where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetSalesOutIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
            string.Format(@"select ProjectMasterId as ProjectMasterID,ProjectModel as ProjectName,EmployeeCode,UserFullName,Orders,WarehouseEntryDate,
               ExtendedWarehouseDate,UserId,EffectiveDays,cast(OrderQuantity as bigint) as OrderQuantity,cast(TotalTblBarcodeIMEI as bigint) as TotalTblBarcodeIMEI,
               cast(TotalSalesOut as bigint) as TotalSalesOut,RewardPercentage,cast(ExistedPercentage as bigint) as ExistedPercentage,
               RewardAmount,case when DeductAmount is not null then -DeductAmount else RewardAmount end as FinalAmount2,IncsTypes='SalesOutReward' from [CellPhoneProject].[dbo].[NinetyFivePercentSalesOutReward] pai
            where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetVesselRewardOrPenaltiesForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            //            string query =
            //            string.Format(@"select ProjectMasterID,ProjectName,EmployeeCode,Orders,ProjectType,ShipmentType,PoDate,LSD,PoVsLSDDiff,VesselDate
            //                ,LsdVsVesselDiffForDeduct,cast(DeductPoint as bigint) as DeductPoint,cast(FeatureBase as bigint) as FeatureBase,cast(SmartBase as bigint) as SmartBase
            //                ,cast(PoReward as bigint) as PoReward,cast(PerDayDeduction as bigint) as PerDayDeduction
            //                ,cast(TotalPenalties as decimal(18,2)) as Penalties ,IncsTypes='VesselRewardOrPenalties' from [CellPhoneProject].[dbo].[PmAndQcLsdToVesselData] pai
            //                where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);
            string query =
  string.Format(@"select ProjectMasterId as ProjectMasterID
                  ,ProjectModel as ProjectName
                  ,EmployeeCode
                  ,UserFullName
                  ,UserId
                  ,Orders
                  ,ProjectType
                  ,PoDate
                  ,LSD
                  ,PoVsLSDDiff
                  ,VesselDate
                  ,LsdVsVesselDiffForDeduct
                  ,cast(DeductPoint as bigint) as DeductPoint
                  ,cast(DeductedAmount as decimal(18,2)) as DeductedAmount
                  ,LsdVsVesselDiffForReward
                  ,cast(RewardPoint as bigint) as RewardPoint
                  ,cast(RewardAmount as bigint) as RewardAmount,IncsTypes='VesselRewardOrPenalties' from [CellPhoneProject].[dbo].[PmAndQcLsdToVesselData] pai
                where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmHeadPerPoIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
            string.Format(@"select cast(ProjectMasterID as bigint) as ProjectMasterID,ProjectName,Orders,PoDate,cast(PoReward as bigint) as PoReward
             ,EmployeeCode,RoleName,IncsTypes='InchargePoIncs.' from [CellPhoneProject].[dbo].[PmHead_PerPoIncentive] pai
            where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmHeadVesselPenaltiesForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
            string.Format(@"select ProjectMasterID,ProjectName,EmployeeCode,Orders,ProjectType,ShipmentType,PoDate,LSD,PoVsLSDDiff,
            VesselDate,LsdVsVesselDiffForDeduct,cast(DeductPoint as bigint) as DeductPoint,cast(FeatureBase as bigint) as FeatureBase,cast(SmartBase as bigint) as SmartBase,
            cast(PoReward as bigint) as PoReward,cast(PerDayDeduction as bigint) as PerDayDeduction,cast(TotalPenalties as decimal(18,2)) as Penalties,
            IncsTypes='InchargeVesselPenalties' from [CellPhoneProject].[dbo].[PmHead_VesselPenaltiesForPo] pai
            where pai.EmployeeCode='{0}' and pai.Year='{2}' and pai.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public string SaveAccessoriesProject(List<Pm_IncentiveModel> issueList, string attachment, List<string> projectMasterId, List<string> projectName)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {

                var multiProName = "";
                var multiProIds = "";
                foreach (var pros in projectMasterId)
                {
                    long pros1 = Convert.ToInt64(pros);
                    var proNames =
                        (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == pros1 select pm)
                            .FirstOrDefault();

                    var res = proNames.ProjectName + " (Order " + Convert.ToString(proNames.OrderNuber) + ")";
                    multiProName = multiProName == null ? res : multiProName + "," + res;
                    multiProName = multiProName.TrimStart(',');

                    var res1 = pros;
                    multiProIds = multiProIds == null ? res1 : multiProIds + "," + res1;
                    multiProIds = multiProIds.TrimStart(',');
                }

                //var projects =
                //    (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == proId select pm)
                //        .FirstOrDefault();

                var modelss = new Pm_AccessoriesProject();
                //modelss.ProjectMasterId = proId;
                //modelss.ProjectName = projects.ProjectName;
                //modelss.Orders = projects.OrderNuber;
                modelss.MultiProjectName = multiProName;
                modelss.MultiProjectIds = multiProIds;
                modelss.SupportingDocument = issueList[0].SupportingDocument;
                modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                modelss.AccessoriesType = issueList[0].AccessoriesType;
                modelss.Remarks = issueList[0].Remarks;
                modelss.Added = userId;
                modelss.AddedDate = DateTime.Now;

                _dbeEntities.Pm_AccessoriesProject.Add(modelss);
                _dbeEntities.SaveChanges();

                _dbeEntities.SaveChanges();
            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }

            return returnValue;
        }

        public List<Pm_IncentiveModel> PmAccessoriesProjectList()
        {
            //var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 * from  [CellPhoneProject].[dbo].[Pm_AccessoriesProject] order by PmAccessoriesId desc").ToList();
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 pa.MultiProjectIds,pa.MultiProjectName,pa.EffectiveMonth,pa.AccessoriesType,pa.Remarks,cu.UserFullName as PmName,pa.SupportingDocument 
            from  [CellPhoneProject].[dbo].[Pm_AccessoriesProject] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by PmAccessoriesId desc").ToList();
            return proList;
        }

        public List<Pm_IncentiveModel> PmFollowUpFocMaterial()
        {
            // var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 * from  [CellPhoneProject].[dbo].[Pm_FollowUpFocMaterial] order by PmFocId desc").ToList();
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"
            select top 500 pm.EffectiveMonth,pm.SupportingDocument,pm.Remarks,cu.UserFullName as PmName from  [CellPhoneProject].[dbo].[Pm_FollowUpFocMaterial] pm
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pm.Added
            order by PmFocId desc").ToList();
            return proList;
        }

        public string SaveFollowUpFocMaterial(List<Pm_IncentiveModel> issueList, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                var modelss = new Pm_FollowUpFocMaterial();
                modelss.SupportingDocument = issueList[0].SupportingDocument;
                modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                modelss.Remarks = issueList[0].Remarks;
                modelss.Added = userId;
                modelss.AddedDate = DateTime.Now;

                _dbeEntities.Pm_FollowUpFocMaterial.Add(modelss);
                _dbeEntities.SaveChanges();

            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }
            return returnValue;
        }

        public List<Pm_IncentiveModel> PmPoFeedbackAndInfoUpdate()
        {
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 pa.EffectiveMonth,pa.SupportingDocument,pa.Remarks,cu.UserFullName as PmName 
            from  [CellPhoneProject].[dbo].[Pm_PoFeedback] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by PoFeedbackId desc").ToList();
            return proList;
        }

        public string SavePoFeedbackAndInfoUpdate(List<Pm_IncentiveModel> issueList, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                var modelss = new Pm_PoFeedback();
                modelss.SupportingDocument = issueList[0].SupportingDocument;
                modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                modelss.Remarks = issueList[0].Remarks;
                modelss.Added = userId;
                modelss.AddedDate = DateTime.Now;

                _dbeEntities.Pm_PoFeedback.Add(modelss);
                _dbeEntities.SaveChanges();

            }
            catch (Exception exception)
            {
                returnValue = exception.Message;
            }
            return returnValue;
        }

        public List<Pm_IncentiveModel> PmSupplierPenaltiesList()
        {
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@" select top 500 pa.MultiProjectIds,pa.MultiProjectName,pa.CurrencyType,pa.EffectiveMonth,pa.Amount,pa.Remarks,cu.UserFullName as PmName,pa.SupportingDocument  
            from  [CellPhoneProject].[dbo].[Pm_SupplierPenalties] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by SupplierPenaltiesId desc").ToList();
            return proList;
        }

        public string SaveSupplierPenalties(List<Pm_IncentiveModel> issueList, string attachment, List<string> projectMasterId, List<string> projectName)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                var multiProName = "";
                var multiProIds = "";
                foreach (var pros in projectMasterId)
                {
                    long pros1 = Convert.ToInt64(pros);
                    var proNames =
                        (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == pros1 select pm)
                            .FirstOrDefault();

                    var res = proNames.ProjectName + " (Order " + Convert.ToString(proNames.OrderNuber) + ")";
                    multiProName = multiProName == null ? res : multiProName + "," + res;
                    multiProName = multiProName.TrimStart(',');

                    var res1 = pros;
                    multiProIds = multiProIds == null ? res1 : multiProIds + "," + res1;
                    multiProIds = multiProIds.TrimStart(',');
                }

                var modelss = new Pm_SupplierPenalties();
                modelss.MultiProjectName = multiProName;
                modelss.MultiProjectIds = multiProIds;
                modelss.SupportingDocument = issueList[0].SupportingDocument;
                modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                modelss.Amount = issueList[0].Amount;
                modelss.Remarks = issueList[0].Remarks;
                modelss.CurrencyType = issueList[0].CurrencyType;
                modelss.Added = userId;
                modelss.AddedDate = DateTime.Now;
                _dbeEntities.Pm_SupplierPenalties.Add(modelss);
                _dbeEntities.SaveChanges();

            }
            catch (Exception exception)
            {
                returnValue = exception.Message;
            }
            return returnValue;
        }

        public List<Pm_IncentiveModel> PmGuidelinesList()
        {
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"
            select top 500 pa.ProjectMasterId,pa.ProjectName,pa.Orders,pa.EffectiveMonth,pa.Remarks,pa.SupportingDocument,cu.UserFullName as PmName 
            from  [CellPhoneProject].[dbo].[Pm_Guidelines] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by PmGuidelineId desc").ToList();
            return proList;
        }

        public string SavePmGuidelines(List<Pm_IncentiveModel> issueList, long proId, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {

                if (proId != 0)
                {
                    var projects =
                        (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == proId select pm)
                            .FirstOrDefault();

                    var modelss = new Pm_Guidelines();
                    modelss.ProjectMasterId = proId;
                    modelss.ProjectName = projects.ProjectName;
                    modelss.Orders = projects.OrderNuber;
                    modelss.SupportingDocument = issueList[0].SupportingDocument;
                    modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                    modelss.Remarks = issueList[0].Remarks;
                    modelss.Added = userId;
                    modelss.AddedDate = DateTime.Now;

                    _dbeEntities.Pm_Guidelines.Add(modelss);
                    _dbeEntities.SaveChanges();

                }

                _dbeEntities.SaveChanges();
            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }

            return returnValue;
        }

        public List<Pm_IncentiveModel> PmProjectMarketingSpecList()
        {
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 pa.ProjectMasterId,pa.ProjectName,pa.Orders,pa.EffectiveMonth,pa.Remarks,pa.SupportingDocument,cu.UserFullName as PmName 
            from  [CellPhoneProject].[dbo].[Pm_ProjectMarketingSpec] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by SpecId desc").ToList();
            return proList;
        }

        public string SavePmProjectMarketingSpec(List<Pm_IncentiveModel> issueList, long proId, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {

                if (proId != 0)
                {
                    var projects =
                        (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == proId select pm)
                            .FirstOrDefault();

                    var modelss = new Pm_ProjectMarketingSpec();
                    modelss.ProjectMasterId = proId;
                    modelss.ProjectName = projects.ProjectName;
                    modelss.Orders = projects.OrderNuber;
                    modelss.SupportingDocument = issueList[0].SupportingDocument;
                    modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                    modelss.Remarks = issueList[0].Remarks;
                    modelss.Added = userId;
                    modelss.AddedDate = DateTime.Now;

                    _dbeEntities.Pm_ProjectMarketingSpec.Add(modelss);
                    _dbeEntities.SaveChanges();

                }

                _dbeEntities.SaveChanges();
            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }

            return returnValue;
        }

        public List<Pm_IncentiveModel> PmPolicyUpdateList()
        {
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 pa.EffectiveMonth,pa.SupportingDocument,pa.Remarks,cu.UserFullName as PmName 
            from  [CellPhoneProject].[dbo].[Pm_PolicyUpdate] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by PolicyId desc").ToList();
            return proList;
        }

        public string SavePolicyUpdate(List<Pm_IncentiveModel> issueList, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                var modelss = new Pm_PolicyUpdate();
                modelss.SupportingDocument = issueList[0].SupportingDocument;
                modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                modelss.Remarks = issueList[0].Remarks;
                modelss.Added = userId;
                modelss.AddedDate = DateTime.Now;

                _dbeEntities.Pm_PolicyUpdate.Add(modelss);
                _dbeEntities.SaveChanges();

            }
            catch (Exception exception)
            {
                returnValue = exception.Message;
            }
            return returnValue;
        }

        public List<Pm_IncentiveModel> PmSampleHandsetManagementList()
        {
            var proList = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(@"select top 500 pa.EffectiveMonth,pa.SupportingDocument,pa.Remarks,cu.UserFullName as PmName  
            from  [CellPhoneProject].[dbo].[Pm_SampleHandsetManagement] pa
            left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pa.Added
            order by SampleHandsetId desc").ToList();
            return proList;
        }

        public string SaveSampleHandset(List<Pm_IncentiveModel> issueList, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                var modelss = new Pm_SampleHandsetManagement();
                modelss.SupportingDocument = issueList[0].SupportingDocument;
                modelss.EffectiveMonth = issueList[0].EffectiveMonth;
                modelss.Remarks = issueList[0].Remarks;
                modelss.Added = userId;
                modelss.AddedDate = DateTime.Now;

                _dbeEntities.Pm_SampleHandsetManagement.Add(modelss);
                _dbeEntities.SaveChanges();

            }
            catch (Exception exception)
            {
                returnValue = exception.Message;
            }
            return returnValue;
        }


        public List<Pm_IncentiveModel> GetAccessoriesProjectIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,case when Orders is null then 0 else Orders end as Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
              from 
              (
                select cast(ap.ProjectMasterId as varchar(100)) as ProjectIds, ap.MultiProjectName as MultiProjectName,ap.Orders,
                case when ap.SupportingDocument is not null or ap.SupportingDocument not in ('failed') then 'Yes' else 'No' end as IsDocumentUploaded,
                ap.EffectiveMonth,ap.AccessoriesType,ap.Remarks,im.Amount,cm.EmployeeCode,im.IncentiveName as IncentiveType
                FROM [CellPhoneProject].[dbo].[Pm_AccessoriesProject] ap
                left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
                left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Accessories Project'
                where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}')A where A.IsDocumentUploaded='Yes' ",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetFollowUpFocMaterialIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
             from 
            (
	            select ProjectIds='--', MultiProjectName='--',Orders=0,
	            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes' 
	            else 'No' end as IsDocumentUploaded,
	            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,im.Amount,cm.EmployeeCode,im.IncentiveName as IncentiveType
	            FROM [CellPhoneProject].[dbo].[Pm_FollowUpFocMaterial] ap
	            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
	            left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Foc Material'
	            where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
	            and ap.SupportingDocument <> ''
            )A where A.IsDocumentUploaded='Yes'   ",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetPoFeedbackIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
             from 
            (
	            select ProjectIds='--', MultiProjectName='--',Orders=0,
	            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes' 
	            else 'No' end as IsDocumentUploaded,
	            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,im.Amount,cm.EmployeeCode,im.IncentiveName as IncentiveType
	            FROM [CellPhoneProject].[dbo].[Pm_PoFeedback] ap
	            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
	            left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Po Feedback'
	            where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
	            and ap.SupportingDocument <> ''
            )A where A.IsDocumentUploaded='Yes'",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetSupplierPenaltiesIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType,CurrencyType
             from 
            (
	            select cast(ap.MultiProjectIds as varchar(100)) as ProjectIds, ap.[MultiProjectName] as MultiProjectName,Orders=0,
	            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes' 
	            else 'No' end as IsDocumentUploaded,
	            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,ap.Amount,cm.EmployeeCode,ap.CurrencyType,IncentiveType='Supplier Penalties'
	            FROM [CellPhoneProject].[dbo].[Pm_SupplierPenalties] ap
	            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
	            where  cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
	            and ap.SupportingDocument <> ''
            )A where A.IsDocumentUploaded='Yes'  ",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetPmGuidelinesIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
             from 
            (
	            select cast(ap.ProjectMasterId as varchar(100)) as ProjectIds, ap.ProjectName as MultiProjectName,ap.Orders,
	            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes' 

	            else 'No' end as IsDocumentUploaded,
	            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,im.Amount,cm.EmployeeCode,im.IncentiveName as IncentiveType
	            FROM [CellPhoneProject].[dbo].[Pm_Guidelines] ap
	            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
	            left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Pm Guidelines'
	            where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
	            and ap.SupportingDocument <> ''
            )A where A.IsDocumentUploaded='Yes'  ",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetProjectMarketingIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
             from 
            (
	            select cast(ap.ProjectMasterId as varchar(100)) as ProjectIds, ap.ProjectName as MultiProjectName,ap.Orders,
	            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes'	else 'No' end as IsDocumentUploaded,
	            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,
	            case when pm.ProjectType='Smart' and  im.IncentiveName='Project Marketing Spec_Smart' then im.Amount
	            when pm.ProjectType='Feature' and  im.IncentiveName='Project Marketing Spec_Feature' then im.Amount end as Amount,

	            cm.EmployeeCode,im.IncentiveName as IncentiveType
	            FROM [CellPhoneProject].[dbo].[Pm_ProjectMarketingSpec] ap
	            left join [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterId=ap.ProjectMasterId
	            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
	            left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Project Marketing Spec_Smart' or im.IncentiveName='Project Marketing Spec_Feature' 
	            where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
	            and ap.SupportingDocument <> ''
            )A where A.IsDocumentUploaded='Yes'  and A.Amount is not  null",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetPolicyUpdateIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
             from 
             (
	            select ProjectIds='--', MultiProjectName='--',Orders=0,
	            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes' 
	            else 'No' end as IsDocumentUploaded,
	            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,im.Amount,cm.EmployeeCode,im.IncentiveName as IncentiveType
	            FROM [CellPhoneProject].[dbo].[Pm_PolicyUpdate] ap
	            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
	            left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Policy Update'
	            where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
	            and ap.SupportingDocument <> ''
            )A where A.IsDocumentUploaded='Yes'  ",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetSampleHandsetIncentive(string employeeCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query =
             string.Format(@"select ProjectIds,MultiProjectName,Orders,IsDocumentUploaded,EffectiveMonth,AccessoriesType,Remarks,Amount,EmployeeCode,IncentiveType
	            from 
	            (
		            select ProjectIds='--', MultiProjectName='--',Orders=0,
		            case when ap.SupportingDocument is not null  or ap.SupportingDocument not in ('failed') then 'Yes' 
		            else 'No' end as IsDocumentUploaded,
		            ap.EffectiveMonth,AccessoriesType='--',ap.Remarks,im.Amount,cm.EmployeeCode,im.IncentiveName as IncentiveType
		            FROM [CellPhoneProject].[dbo].Pm_SampleHandsetManagement ap
		            left join [CellPhoneProject].[dbo].CmnUsers cm on cm.CmnUserId=ap.Added
		            left join [CellPhoneProject].[dbo].[Pm_Incentive_Base] im on im.IncentiveName='Sample Handset Management'
		            where im.ActiveRole=3 and cm.EmployeeCode='{0}' and   DATEPART(mm,ap.EffectiveMonth)='{1}' and  DATENAME(YEAR,ap.EffectiveMonth)='{2}'
		            and ap.SupportingDocument <> ''
	            )A where A.IsDocumentUploaded='Yes'  ",
            employeeCode, mon, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public string SaveAllDocumentDetails(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {

                var model = new Pm_DocumentUploadIncentive();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(insResult.Year) ? Convert.ToInt64(insResult.Year) : 0;
                model.Amount = Convert.ToDecimal(insResult.Amount);
                model.IncentiveType = insResult.IncentiveType;
                model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);
                //model.Orders = Convert.ToInt16(insResult.Orders);
                model.Orders = !string.IsNullOrWhiteSpace(insResult.Orders) ? Convert.ToInt16(insResult.Orders) : 0;
                model.EmployeeCode = insResult.EmployeeCode;
                model.Remarks = insResult.Remarks;
                model.MultiProjectIds = insResult.ProjectIds;
                model.MultiProjectName = insResult.MultiProjectName;
                model.EffectiveMonth = insResult.EffectiveMonth;
                model.AccessoriesType = insResult.AccessoriesType;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_DocumentUploadIncentive.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<Custom_Pm_IncentiveModel> GetPmDocIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_DocumentUploadIncentive where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetProClosingIncentive(string employeeCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@" select pm.ProjectMasterId, pm.ProjectName,pm.OrderNuber as Orders, pm.ProjectClosingDate,ppo.MarketClearanceDate,cm.EmployeeCode,pio.Amount
                from CellPhoneProject.dbo.ProjectMasters pm
                left join CellPhoneProject.[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId 
                left join CellPhoneProject.[dbo].CmnUsers cm on cast(cm.CmnUserId as varchar(100))=cast(pm.ProjectClosedBy as varchar(100))
                left join CellPhoneProject.[dbo].Pm_Incentive_Base pio on pio.IncentiveName='ProjectClosing' and pio.ActiveRole=4

                where DATEPART(mm,ppo.MarketClearanceDate)='{1}' and  DATENAME(YEAR,ppo.MarketClearanceDate)='{2}' and cm.EmployeeCode='{0}'
                order by ProjectMasterId desc", employeeCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public bool GetProClosingIncentiveData(string employeeCode, int monNum, long year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
                select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_ProjectClosingIncentive] 
                where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckTeamIncentivePercentage(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
                select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_HeadTeamIncentivePercentage] 
                where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveProjectClosingDetails(List<Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new Pm_ProjectClosingIncentive();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = insResult.Year;
                model.Amount = Convert.ToDecimal(insResult.Amount);
                model.FinalAmount = Convert.ToDecimal(insResult.Amount);
                model.Orders = Convert.ToInt16(insResult.Orders);
                model.EmployeeCode = insResult.EmployeeCode;
                model.ProjectMasterId = insResult.ProjectMasterId;
                model.ProjectName = insResult.ProjectName;
                model.ProjectClosingDate = insResult.ProjectClosingDate;
                model.MarketClearanceDate = insResult.MarketClearanceDate;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_ProjectClosingIncentive.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public List<Custom_Pm_IncentiveModel> GetPmProClosingIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_ProjectClosingIncentive where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetRawMaterialDelayUploadIncentive(string employeeCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            //            string query =
            //               string.Format(@"select rmi.ProjectMasterId,rmi.ProjectName,rmi.Orders,rmi.PoCategory,rmi.PoQuantity,rmi.ProjectManagerClearanceDate,
            //                rmi.AddedDate as RawMaterialAddedDate,
            //                DATEDIFF(day, rmi.ProjectManagerClearanceDate, rmi.AddedDate) AS DaysPassed,cm.EmployeeCode,pio.IncentiveName,pio.Amount
            //                from [CellPhoneProject].[dbo].[RawMaterialInspection] rmi
            //                left join CellPhoneProject.[dbo].CmnUsers cm on cm.CmnUserId =rmi.Added 
            //                left join CellPhoneProject.[dbo].Pm_Incentive_Base pio on pio.IncentiveName='Raw Material Delay Upload' and pio.ActiveRole=5
            //                where DATEDIFF(day, rmi.ProjectManagerClearanceDate,rmi.AddedDate)>5 
            //                and DATEPART(mm,rmi.ProjectManagerClearanceDate)='{1}' and  DATENAME(YEAR,rmi.ProjectManagerClearanceDate)='{2}' and cm.EmployeeCode='{0}'",
            //               employeeCode, monNums, years);
            string query =
             string.Format(@"select * from
                (
	                select rmi.ProjectMasterId,rmi.ProjectName,rmi.Orders,rmi.PoCategory,rmi.PoQuantity,rmi.ProjectManagerClearanceDate,rmi.AddedDate as RawMaterialAddedDate,
	                DATEDIFF(day, rmi.ProjectManagerClearanceDate, rmi.AddedDate) AS DaysPassed,cm.EmployeeCode,pio.IncentiveName,pio.Amount,
	                case when rmi.SupportingDocument is not null  or rmi.SupportingDocument not in ('failed') then 'Yes' else 'No' end as IsDocumentUploaded
	                from [CellPhoneProject].[dbo].[RawMaterialInspection] rmi
	                left join CellPhoneProject.[dbo].CmnUsers cm on cm.CmnUserId =rmi.Added 
	                left join CellPhoneProject.[dbo].Pm_Incentive_Base pio on pio.IncentiveName='Raw Material Delay Upload' and pio.ActiveRole=5
	                where DATEDIFF(day, rmi.ProjectManagerClearanceDate,rmi.AddedDate)>=5 and rmi.SupportingDocument <> ''
	                and DATEPART(mm,rmi.ProjectManagerClearanceDate)='{1}' and  DATENAME(YEAR,rmi.ProjectManagerClearanceDate)='{2}' and cm.EmployeeCode='{0}'
                )A where A.IsDocumentUploaded='Yes' ",
             employeeCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Pm_IncentiveModel> GetShipClearenceVsLsdIncentive(string employeeCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            //            string query =
            //               string.Format(@"select ProjectMasterId,ProjectName,Orders,ProjectType,PoCategory,EmployeeCode,ProjectManagerClearanceDate,LSD,DaysBeforeLsd,DaysAfterLsd,Reward,RealPenalties,Penalties,
            //	                case when Reward=0 then Penalties when Penalties=0 then Reward end as FinalAmount
            //	                from
            //	                (
            //	                    select ProjectMasterId,ProjectName,Orders,ProjectType,PoCategory,EmployeeCode,ProjectManagerClearanceDate,LSD,DaysBeforeLsd,DaysAfterLsd,Reward,-Penalties1 as RealPenalties,
            //		                case when Orders=1 and ProjectType='Smart' and Penalties1>14000 then -14000*0.2 
            //		                when Orders=1 and ProjectType='Feature'  and Penalties1>8400 then -8400*0.2 
            //		                when Orders!=1 and ProjectType='Smart' and Penalties1>7000 then -7000*0.2 
            //		                when Orders!=1 and ProjectType='Feature' and Penalties1>4200 then -4200*0.2 
            //		                else -Penalties1 end as Penalties
            //	                   from
            //	                    ( 
            //		                  select ProjectMasterId,ProjectName,Orders,ProjectType,PoCategory,EmployeeCode,ProjectManagerClearanceDate,LSD,DaysBeforeLsd,DaysAfterLsd,DaysBeforeLsd*100 as Reward,DaysAfterLsd*70 as Penalties1
            //		
            //		                  from
            //                          (
            //			                select rmi.ProjectMasterId,rmi.ProjectName,rmi.Orders,rmi.ProjectType,rmi.PoCategory,rmi.PoQuantity,cm.EmployeeCode,rmi.ProjectManagerClearanceDate,pm.ApproxShipmentDate as LSD,
            //			                case when rmi.ProjectManagerClearanceDate<pm.ApproxShipmentDate then DATEDIFF(day, rmi.ProjectManagerClearanceDate,pm.ApproxShipmentDate) else 0 end as DaysBeforeLsd,
            //			                case when rmi.ProjectManagerClearanceDate>pm.ApproxShipmentDate then DATEDIFF(day,pm.ApproxShipmentDate, rmi.ProjectManagerClearanceDate) else 0 end as DaysAfterLsd
            //
            //			                from [CellPhoneProject].[dbo].[RawMaterialInspection] rmi
            //			                left join [CellPhoneProject].[dbo].ProjectMasters pm on rmi.ProjectMasterId=pm.ProjectmasterId
            //			                left join CellPhoneProject.[dbo].CmnUsers cm on cm.CmnUserId =rmi.Added
            //			 
            //			                where 
            //			                DATEPART(mm,rmi.ProjectManagerClearanceDate)='{1}' and
            //			                DATENAME(YEAR,rmi.ProjectManagerClearanceDate)='{2}' and cm.EmployeeCode='{0}'
            //                         )A
            //                       )B 
            //                     )C order by Orders asc
            //                ",
            //               employeeCode, monNums, years);

            string query =
              string.Format(@"	select ProjectMasterId,ProjectName,Orders,ProjectType,PoCategory,EmployeeCode,ProjectManagerClearanceDate,LSD,DaysBeforeLsd,DaysAfterLsd,cast(Reward as decimal(18,2)) as Reward,cast(RealPenalties as decimal(18,2)) as RealPenalties,cast(Penalties as decimal(18,2)) as Penalties,
	            case when Reward=0 then Penalties when Penalties=0 then Reward end as FinalAmount
	            from
	            (
	                select ProjectMasterId,ProjectName,Orders,ProjectType,PoCategory,EmployeeCode,ProjectManagerClearanceDate,LSD,DaysBeforeLsd,DaysAfterLsd,Reward,-Penalties1 as RealPenalties,
		            case when Orders=1 and ProjectType='Smart' and Penalties1>14000*0.2 then -14000*0.2 
		            when Orders=1 and ProjectType='Feature'  and Penalties1>8400*0.2 then -8400*0.2 
		            when Orders!=1 and ProjectType='Smart' and Penalties1>7000*0.2 then -7000*0.2 
		            when Orders!=1 and ProjectType='Feature' and Penalties1>4200*0.2 then -4200*0.2 
		            else -Penalties1 end as Penalties
	               from
	                ( 
		              select ProjectMasterId,ProjectName,Orders,ProjectType,PoCategory,EmployeeCode,ProjectManagerClearanceDate,LSD,DaysBeforeLsd,DaysAfterLsd,DaysBeforeLsd*100 as Reward,DaysAfterLsd*70 as Penalties1
		
		              from
                      (
			            select rmi.ProjectMasterId,rmi.ProjectName,rmi.Orders,rmi.ProjectType,rmi.PoCategory,rmi.PoQuantity,cm.EmployeeCode,rmi.ProjectManagerClearanceDate,pm.ApproxShipmentDate as LSD,
			            case when rmi.ProjectManagerClearanceDate<pm.ApproxShipmentDate then DATEDIFF(day, rmi.ProjectManagerClearanceDate,pm.ApproxShipmentDate) else 0 end as DaysBeforeLsd,
			            case when rmi.ProjectManagerClearanceDate>pm.ApproxShipmentDate then DATEDIFF(day,pm.ApproxShipmentDate, rmi.ProjectManagerClearanceDate) else 0 end as DaysAfterLsd

			            from [CellPhoneProject].[dbo].[RawMaterialInspection] rmi
			            left join [CellPhoneProject].[dbo].ProjectMasters pm on rmi.ProjectMasterId=pm.ProjectmasterId
			            left join CellPhoneProject.[dbo].CmnUsers cm on cm.CmnUserId =rmi.Added
			 
			            where 
			            DATEPART(mm,rmi.ProjectManagerClearanceDate)='{1}' and
			            DATENAME(YEAR,rmi.ProjectManagerClearanceDate)='{2}' and cm.EmployeeCode='{0}'
                        and rmi.ProjectManagerClearanceDate = (select top 1 rr.ProjectManagerClearanceDate from [CellPhoneProject].[dbo].[RawMaterialInspection] rr where rr.ProjectName=rmi.ProjectName and 
						rr.Orders=rmi.Orders order by RawMaterialId desc)  
                     )A
                   )B 
                 )C order by Orders asc",
               employeeCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public bool GetRawUploadIncentiveData(string employeeCode, int monNum, long year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
                select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_RawMaterialUploadDelayPenalties] 
                where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveRawUploadDelayDetails(List<Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new Pm_RawMaterialUploadDelayPenalties();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = insResult.Year;
                model.Amount = Convert.ToDecimal(insResult.Amount);
                model.FinalAmount = Convert.ToDecimal(insResult.Amount);
                model.Orders = Convert.ToInt16(insResult.Orders);
                model.EmployeeCode = insResult.EmployeeCode;
                model.ProjectMasterId = insResult.ProjectMasterId;
                model.ProjectName = insResult.ProjectName;
                model.PoCategory = insResult.PoCategory;
                model.PoQuantity = insResult.PoQuantity;
                model.ProjectManagerClearanceDate = insResult.ProjectManagerClearanceDate;
                model.RawMaterialAddedDate = insResult.RawMaterialAddedDate;
                model.DaysPassed = insResult.DaysPassed;
                model.IncentiveName = insResult.IncentiveName;

                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_RawMaterialUploadDelayPenalties.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public string SaveHeadTeamIncentivePercentage(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new Pm_HeadTeamIncentivePercentage();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = Convert.ToInt64(insResult.Year);
                model.TeamIncentive = Convert.ToDecimal(insResult.TeamIncentive);
                model.HeadPercentages = Convert.ToDecimal(insResult.InchargePecentage);
                model.EmployeeCode = insResult.EmployeeCode;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_HeadTeamIncentivePercentage.AddOrUpdate(model);

            }
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public bool GetShipmentVsLsdIncentiveData(string employeeCode, int monNum, long year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
                select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].[Pm_ShipmentClearanceVsLsdIncentive] 
                where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports = _dbeEntities.Database.SqlQuery<Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveShipmentClearanceVsLsdDetails(List<Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new Pm_ShipmentClearanceVsLsdIncentive();
                model.Month = insResult.Month;
                model.MonNum = insResult.MonNum;
                model.Year = insResult.Year;
                model.Amount = Convert.ToDecimal(insResult.FinalAmount);
                model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);
                model.Orders = Convert.ToInt16(insResult.Orders);
                model.EmployeeCode = insResult.EmployeeCode;
                model.ProjectMasterId = insResult.ProjectMasterId;
                model.ProjectName = insResult.ProjectName;
                model.ProjectType = insResult.ProjectType;
                model.PoCategory = insResult.PoCategory;
                model.ProjectManagerClearanceDate = insResult.ProjectManagerClearanceDate;
                model.LSD = insResult.LSD;
                model.DaysBeforeLsd = insResult.DaysBeforeLsd;
                model.DaysAfterLsd = insResult.DaysAfterLsd;
                model.Reward = insResult.Reward;
                model.RealPenalties = insResult.RealPenalties;
                model.Penalties = insResult.Penalties;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.Pm_ShipmentClearanceVsLsdIncentive.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public List<Custom_Pm_IncentiveModel> GetPmRawUploadIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_RawMaterialUploadDelayPenalties where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPmShipmentClearanceVsLsdIncentive(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            string query =
               string.Format(@"select sum(FinalAmount) as FinalAmount1 from CellPhoneProject.dbo.Pm_ShipmentClearanceVsLsdIncentive where EmployeeCode='{0}' 
                    and MonNum='{1}' and Year='{2}' group by Month,MonNum,Year", empCode, monNums, years);
            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPm_DocumentUploadIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@" select IncentiveType as IncsTypes,[MultiProjectName] as ProjectName, case when  cast(Orders as varchar(50))=0 then '----' else cast(Orders as varchar(50)) +' Order' end as Orders,AccessoriesType,EffectiveMonth,Remarks,Amount as Amount1,FinalAmount as FinalAmount1 FROM [CellPhoneProject].[dbo].[Pm_DocumentUploadIncentive] pdu
              where pdu.MonNum='{1}' and pdu.Year='{2}' and pdu.EmployeeCode='{0}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPm_ProjectClosingIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select IncsTypes='Project Closing',ProjectName, case when  cast(Orders as varchar(50))=0 then '----' else cast(Orders as varchar(50)) +' Order' end as Orders,ProjectClosingDate,MarketClearanceDate,Amount as Amount1,FinalAmount as FinalAmount1
            from [CellPhoneProject].[dbo].[Pm_ProjectClosingIncentive] where MonNum='{1}' and Year='{2}' and EmployeeCode='{0}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPm_RawMaterialUpDelayPenaltiesForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@" select IncentiveName as IncsTypes,ProjectName, case when  cast(Orders as varchar(50))=0 then '----' else cast(Orders as varchar(50)) +' Order' end as Orders,PoCategory,PoQuantity,ProjectManagerClearanceDate,RawMaterialAddedDate,DaysPassed,Amount as Amount1,FinalAmount as FinalAmount1  
             FROM [CellPhoneProject].[dbo].[Pm_RawMaterialUploadDelayPenalties]  where MonNum='{1}' and Year='{2}' and EmployeeCode='{0}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPm_ShipmentClearanceVsLsdForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select IncsTypes='Pm_Shipment Clearance Vs Lsd',ProjectName, case when  cast(Orders as varchar(50))=0 then '----' else cast(Orders as varchar(50)) +' Order' end as Orders,ProjectType,PoCategory,ProjectManagerClearanceDate,LSD,Reward,Penalties,Amount as Amount1,FinalAmount as FinalAmount1
             FROM [CellPhoneProject].[dbo].[Pm_ShipmentClearanceVsLsdIncentive] where MonNum='{1}' and Year='{2}' and EmployeeCode='{0}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        #endregion

        #region PM order Quantity with Color Ratio

        public void SaveOrderQuantityWithColorModel(PmOrderQuantityWithColorModel model)
        {
            model.ProjectName =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectMasterId)
                    .Select(x => x.ProjectName)
                    .FirstOrDefault();
            model.OrderNumber =
                _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectMasterId)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault();
            Mapper.CreateMap<PmOrderQuantityWithColorModel, PmOrderQuantityWithColor>();
            var m = Mapper.Map<PmOrderQuantityWithColor>(model);
            _dbeEntities.PmOrderQuantityWithColors.Add(m);
            _dbeEntities.SaveChanges();
        }

        public List<PmOrderQuantityWithColorModel> GetOrderQuantityWithColorModel(long addedby)
        {
            var model = (from m in _dbeEntities.PmOrderQuantityWithColors
                         where m.AddedBy == addedby
                         select new PmOrderQuantityWithColorModel
                         {
                             PmOrderQuantityWithColorId = m.PmOrderQuantityWithColorId,
                             ProjectMasterId = m.ProjectMasterId,
                             ProjectName = m.ProjectName,
                             OrderNumber = m.OrderNumber,
                             Color = m.Color,
                             PmOrderQuantity = m.PmOrderQuantity,
                             ConcernPmComment = m.ConcernPmComment,
                             AddedBy = m.AddedBy,
                             AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             InventoryReceivedQuantity = m.InventoryReceivedQuantity,
                             InventoryReceivedAddedBy = m.InventoryReceivedAddedBy,
                             InventoryReceivedAddedName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReceivedAddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReceivedDate = m.InventoryReceivedDate,
                             CompleteProductionQuantity = m.CompleteProductionQuantity,
                             CompletedQuantityAddedBy = m.CompletedQuantityAddedBy,
                             CompletedQuantityAddedName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.CompletedQuantityAddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             CompletedQuantityAddedDate = m.CompletedQuantityAddedDate,
                             WarehouseReceivedQuantity = m.WarehouseReceivedQuantity,
                             WarehouseQuantityAddedBy = m.WarehouseQuantityAddedBy,
                             WarehouseQuantityAddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.WarehouseQuantityAddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             WarehouseQuantityAddedDate = m.WarehouseQuantityAddedDate,
                             ServiceCenterQuantity = m.ServiceCenterQuantity,
                             ServiceCenterQuantityAddedBy = m.ServiceCenterQuantityAddedBy,
                             ServiceCenterQuantityAddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.ServiceCenterQuantityAddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ServiceCenterQuantityAddedDate = m.ServiceCenterQuantityAddedDate,
                             ProductionTeamComment = m.ProductionTeamComment
                         }).ToList();
            return model;
        }

        public List<PmOrderQuantityWithColorModel> GetOrderWiseTotalCounts(string projectName)
        {
            var model = (from v in _dbeEntities.PmOrderQuantityWithColors
                         where v.ProjectName == projectName
                         group v by v.OrderNumber
                             into y
                             let total = (y.Sum(n => n.PmOrderQuantity))
                             select new PmOrderQuantityWithColorModel
                             {
                                 OrderNumber = y.Key,
                                 PmOrderQuantity = total
                             }).ToList();
            return model;
        }

        public List<PmOrderQuantityWithColorModel> GetOrderWiseCountsByProject(string projectName)
        {
            var model = (from v in _dbeEntities.PmOrderQuantityWithColors
                         where v.ProjectName == projectName
                         select new PmOrderQuantityWithColorModel
                         {
                             OrderNumber = v.OrderNumber,
                             ProjectName = v.ProjectName,
                             Color = v.Color,
                             PmOrderQuantity = v.PmOrderQuantity
                         }).ToList();
            return model;
        }

        public List<tblIMEIRecordModel> GetWareHouseQuantity(string projectName)
        {
            var models = new List<tblIMEIRecordModel>();
            String sqlconnectionstring = ConfigurationManager.ConnectionStrings["WCMSConnectionString"].ConnectionString;
            var conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format(@"SELECT Model,WO,Color,(select count(WO) where wo=wo) as Counts FROM tblIMEIRecord WHERE Model like '%{0}' group by Model,WO,Color order by WO desc", projectName);
            var cmd = new SqlCommand(query, conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var model = new tblIMEIRecordModel
                {
                    Color = Convert.ToString(rd["Color"]),
                    WO = Convert.ToString(rd["WO"]),
                    Counts = Convert.ToString(rd["Counts"]),
                    Model = Convert.ToString(rd["Model"])
                };
                models.Add(model);
            }
            return models;
        }

        public List<tblIMEIRecordModel> GetServiceCenterQuantity(string projectName)//using this model cause fields are same
        {
            var models = new List<tblIMEIRecordModel>();
            String sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format(@"SELECT ddd.Model,bci.Color,bci.updatedby as WO,count(ddd.Model) as Counts FROM tblDealerDistributionDetails ddd inner join dbo.tblBarCodeInv bci on ddd.barcode=bci.barcode where dealercode='5071' and ddd.model like '%{0}' group by ddd.Model,color,updatedby", projectName);
            var cmd = new SqlCommand(query, conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var model = new tblIMEIRecordModel
                {
                    Color = Convert.ToString(rd["Color"]),
                    WO = Convert.ToString(rd["WO"]),
                    Counts = Convert.ToString(rd["Counts"]),
                    Model = Convert.ToString(rd["Model"])
                };
                models.Add(model);
            }
            return models;
        }

        public List<PmOrderQuantityWithColorModel> GetColorsList(string color)
        {
            var colorList = (from v in _dbeEntities.PmOrderQuantityWithColors
                             where v.Color.StartsWith(color)
                             group v by v.Color into y
                             select new PmOrderQuantityWithColorModel
                             {
                                 Color = y.Key
                             }).ToList();
            return colorList;
        }
        #endregion

        #region Sample tracker report

        public List<SampleTrackerModel> SampleListByProjectName(string project)
        {
            var model = (from m in _dbeEntities.SampleTrackers
                         where m.Model == project
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             Role = m.SampleSentToDept,
                             RoleisHead = _dbeEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToDept = _dbeEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToPersonName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                         }).ToList();
            return model;
        }

        public List<SampleTrackerModel> DeptWiseSampleStatus(string roledesc)
        {
            string query =
                string.Format(
                    @"select st.* from SampleTrackers st inner join CmnUsers cu on st.SampleSentToPersonId=cu.CmnUserId inner join CmnRoles cr on cu.RoleName=cr.RoleName where cr.RoleDescription='{0}'",
                    roledesc);
            var model = _dbeEntities.Database.SqlQuery<SampleTrackerModel>(query).ToList();
            return model;
        }

        public List<SampleTrackerModel> PersonWiseSampleStatus(long id)
        {
            var model = (from m in _dbeEntities.SampleTrackers
                         where m.SampleSentToPersonId == id || m.AddedBy == id
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             Role = m.SampleSentToDept,
                             RoleisHead = _dbeEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToDept = _dbeEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToPersonName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                         }).ToList();
            return model;
        }
        #endregion

        #region New QcAssign Phase
        public List<SwQcTestPhaseModel> GetSwQcTestPhasesForPm()
        {
            List<SwQcTestPhase> list = _dbeEntities.SwQcTestPhases.ToList();

            List<SwQcTestPhaseModel> models = GenericMapper<SwQcTestPhase, SwQcTestPhaseModel>.GetDestinationList(list);
            var vmSwQc = new AssignedProjectListViewModel();
            vmSwQc.SwQcTestPhaseModels = models;
            return models;
        }
        public List<SwQcHeadAssignsFromPmModel> GetSwQcHeadAssignInfoForPm(long projectId)
        {
            string query = string.Format(@"
            select swQc.ProjectName,swQc.SwQcHeadAssignId,swQc.ProjectPmAssignId,swQc.PmToQcHeadAssignTime,swQc.TestPhaseID, swQc.PmToQcHeadAssignComment,swQc.ProjectMasterId,swQc.ProjectManagerSampleType,swQc.ProjectManagerSampleNo,
            swQc.SoftwareVersionName,swQc.SoftwareVersionNo,swTxtPh.TestPhaseName,swQc.SwQcHeadToPmSubmitTime,swQc.SwQcHeadToPmForwardComment,swQc.Status,swQc.IsFinalPhaseMP,
            case when swQc.IsFinalPhaseMP='true' then 'YES' else 'NO' end as IsFinalPhaseMPs
            from CellPhoneProject.dbo.SwQcHeadAssignsFromPm swQc
            left join CellPhoneProject.dbo.SwQcTestPhase swTxtPh on swqc.TestPhaseID=swTxtPh.TestPhaseID
            where swTxtPh.TestPhaseName not in ('Field (Network Test)','Accessories Test') and swQc.ProjectMasterId='{0}' order by swQc.PmToQcHeadAssignTime desc", projectId);
            var exe = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(query).ToList();
            return exe;
        }
        //        public string GetProjectMasterModelForPm(long projectId)
        //        {
        //            string query = string.Format(@"select swQc.PmToQcHeadAssignTime,swQc.PmToQcHeadAssignComment,swQc.ProjectMasterId,swQc.ProjectManagerSampleType,swQc.ProjectManagerSampleNo,
        //            swQc.SoftwareVersionName,swQc.SoftwareVersionNo,swTxtPh.TestPhaseName,swQc.SwQcHeadToPmSubmitTime,swQc.SwQcHeadToPmForwardComment,swQc.Status
        //            from CellPhoneProject.dbo.SwQcHeadAssignsFromPm swQc
        //            left join CellPhoneProject.dbo.SwQcTestPhase swTxtPh on swqc.TestPhaseID=swTxtPh.TestPhaseID
        //            where swQc.ProjectMasterId='{0}' order by swQc.PmToQcHeadAssignTime desc", projectId);
        //            var exe = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(query).ToString();
        //            return exe;
        //        }

        #endregion

        //public List<PMAcknowledgementModel> GetAllPMAcknowledgeList()
        //{
        //    List<PMAcknowledgementModel> list = new List<PMAcknowledgementModel>();
        //    PMAcknowledgementModel item = new PMAcknowledgementModel();
        //    var config = new MapperConfiguration(c => c.CreateMap<PMAcknowledgementModel, PMAcknowledgement>());
        //    var mapper = config.CreateMapper();
        //    SqlConnection con = new SqlConnection(CellPhoneLiveDatabase.ConnectionString);
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("select * from vw_MobileProduction where S_Date is not NULL", con);

        //        SqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            item = new PMAcknowledgementModel();
        //            item.ProjectId = long.Parse(dr["ProjectId"].ToString());
        //            item.PlanId = long.Parse(dr["PlanId"].ToString());
        //            item.OrderNumber = int.Parse(dr["OrderNumber"].ToString());
        //            item.PoCategory = dr["PoCategory"].ToString();
        //            item.ProjectName = dr["ProjectName"].ToString();
        //            item.AllType = dr["AllType"].ToString();
        //            item.ProcessType = dr["ProcessType"].ToString();
        //            item.S_Date = DateTime.Parse(dr["S_Date"].ToString());
        //            item.E_Date = DateTime.Parse(dr["E_Date"].ToString());
        //            item.AcknowledgeStatus="False";

        //            //list.Add(item);
        //            if (_dbeEntities.PMAcknowledgements.Where(x=>x.ProjectId ==item.ProjectId).Count()<0)
        //            {
        //                var pmAcknowledgement = mapper.Map<PMAcknowledgement>(item);
        //                _dbeEntities.PMAcknowledgements.Add(pmAcknowledgement);
        //            }
        //        }
        //        dr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        //lblMsg.Text = "Error --> " + ex.Message;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }

        //    _dbeEntities.SaveChanges();

        //    Mapper.Initialize(cfg => cfg.CreateMap<PMAcknowledgement, PMAcknowledgementModel>());
        //    var data = _dbeEntities.PMAcknowledgements;
        //    //Mapper.Map<List<PMAcknowledgement>, List<PMAcknowledgementModel>>(_dbeEntities.PMAcknowledgements.ToList().GroupBy(y => y.PlanId))
        //    list = (from d in data
        //            group d by d.PlanId
        //                into final
        //                let otherdata = final.FirstOrDefault()
        //                select new PMAcknowledgementModel
        //                {
        //            ProjectId = otherdata.ProjectId,
        //            PlanId = final.Key,
        //            OrderNumber = otherdata.OrderNumber,
        //            PoCategory = otherdata.PoCategory,
        //            ProjectName = otherdata.ProjectName,
        //            AllType = otherdata.AllType,
        //            ProcessType = otherdata.ProcessType,
        //            S_Date = otherdata.S_Date,
        //            E_Date = otherdata.E_Date,
        //            AcknowledgeStatus = otherdata.AcknowledgeStatus
        //                }).ToList();

        //    //if(list.Count>0)
        //    //    list.ad
        //    return list;
        //}

        public ProjectAcknowledgementViewModel GetAllProjectByPlanId(long planid)
        {
            var vm = new ProjectAcknowledgementViewModel();
            vm.PlanId = planid;
            vm.ProjectName = _dbeEntities.PMAcknowledgements.Where(x => x.PlanId == planid).FirstOrDefault().ProjectName;
            List<PMAcknowledgementModel> SMTlist = new List<PMAcknowledgementModel>();
            List<PMAcknowledgementModel> HousingList = new List<PMAcknowledgementModel>();
            List<PMAcknowledgementModel> Batterylist = new List<PMAcknowledgementModel>();
            List<PMAcknowledgementModel> AsseblyList = new List<PMAcknowledgementModel>();
            //var data = _dbeEntities.PMAcknowledgements;
            Mapper.Initialize(cfg => cfg.CreateMap<PMAcknowledgement, PMAcknowledgementModel>()
                .ForMember(d => d.AcknowledgeDateText, o => o.MapFrom(s => s.AcknowledgeDate != null ? DateTime.Parse(s.AcknowledgeDate.ToString()).ToString("yyyy/mm/dd") : DateTime.Today.ToString("yyyy/mm/dd"))));
            SMTlist = Mapper.Map<List<PMAcknowledgement>, List<PMAcknowledgementModel>>(_dbeEntities.PMAcknowledgements.Where(y => y.PlanId == planid && y.ProcessType == "SMT").ToList());
            HousingList = Mapper.Map<List<PMAcknowledgement>, List<PMAcknowledgementModel>>(_dbeEntities.PMAcknowledgements.Where(y => y.PlanId == planid && y.ProcessType == "HOUSING").ToList());
            Batterylist = Mapper.Map<List<PMAcknowledgement>, List<PMAcknowledgementModel>>(_dbeEntities.PMAcknowledgements.Where(y => y.PlanId == planid && y.ProcessType == "BATTERY").ToList());
            AsseblyList = Mapper.Map<List<PMAcknowledgement>, List<PMAcknowledgementModel>>(_dbeEntities.PMAcknowledgements.Where(y => y.PlanId == planid && ((y.ProcessType == "PACKING") || (y.ProcessType == "ASSEMBLY"))).ToList());
            vm.SMTAcknowledgements = SMTlist;
            vm.HousingAcknowledgements = HousingList;
            vm.BatteryAcknowledgements = Batterylist;
            vm.AssemblyAcknowledgements = AsseblyList;

            return vm;
        }

        public bool UpdatePMAcknowledge(ProjectAcknowledgementViewModel vmodel)
        {
            List<PMAcknowledgement> allack = new List<PMAcknowledgement>();
            Mapper.Initialize(cfg => cfg.CreateMap<PMAcknowledgementModel, PMAcknowledgement>());
            if (vmodel.SMTAcknowledgements != null)
            {
                var SMTlist = Mapper.Map<List<PMAcknowledgementModel>, List<PMAcknowledgement>>(vmodel.SMTAcknowledgements);
                allack.AddRange(SMTlist);
            }
            if (vmodel.BatteryAcknowledgements != null)
            {
                var Batterylist = Mapper.Map<List<PMAcknowledgementModel>, List<PMAcknowledgement>>(vmodel.BatteryAcknowledgements);
                allack.AddRange(Batterylist);
            }
            if (vmodel.HousingAcknowledgements != null)
            {
                var Housinglist = Mapper.Map<List<PMAcknowledgementModel>, List<PMAcknowledgement>>(vmodel.HousingAcknowledgements);
                allack.AddRange(Housinglist);
            }
            if (vmodel.AssemblyAcknowledgements != null)
            {
                var Assemblylist = Mapper.Map<List<PMAcknowledgementModel>, List<PMAcknowledgement>>(vmodel.AssemblyAcknowledgements);
                allack.AddRange(Assemblylist);
            }
            try
            {

                String userIdentity = HttpContext.Current.User.Identity.Name;
                long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

                var logdata = new AcknowledgeLog();
                logdata.AcknoledgeId = vmodel.PlanId;
                logdata.LogDate = DateTime.Today;
                logdata.PMId = userId;
                _dbeEntities.AcknowledgeLogs.Add(logdata);

                if (allack.Any())
                {
                    foreach (PMAcknowledgement item in allack)
                    {
                        _dbeEntities.PMAcknowledgements.AddOrUpdate(item);
                    }
                    _dbeEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<SwQcIssueDetailModel> GetSwQcIssueDetailsForPm(string projectId, string swqcInchargeId, string pmAssignId, string testPhaseId, DateTime pmAssignDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            long proId;
            long.TryParse(projectId, out proId);

            long swQcHeadId;
            long.TryParse(swqcInchargeId, out swQcHeadId);

            long pmAssignsId;
            long.TryParse(pmAssignId, out pmAssignsId);

            long testId;
            long.TryParse(testPhaseId, out testId);

            string tempDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", pmAssignDate);

            List<SwQcIssueDetailModel> query = new List<SwQcIssueDetailModel>();

            if (swQcHeadId != 0)
            {
                query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT distinct case when  sii.IssueSerial is null then 0 else sii.IssueSerial end as IssueSerial,cm.UserFullName,sii.SwQcIssueId,sii.SoftwareVersionNo,sii.IssueScenario,sii.ExpectedOutcome,sii.IssueDetails,sii.RefernceModule,sii.Frequency,sii.Attachment,sii.IssueType,sii.Result,sii.FilesUrl,sii.FilesDetail,sii.IsIssue,sii.IsFile,
            case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas
            FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] sii 
            left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sq on sii.SwQcHeadAssignId=sq.SwQcHeadAssignId and sii.ProjectMasterId=sq.ProjectMasterId
            left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sii.Added
            where (sii.RefernceModule not in ('Camera Automation','Monkey Test','CTS') or sii.RefernceModule is null) and sii.ProjectMasterId={0} and sii.ProjectPmAssignId={1} and sii.SwQcHeadAssignId={2}  
            and (sq.PmToQcHeadAssignTime like '%" + tempDate + "%')   and sii.TestPhaseID={4} order by IsIssue,IsFile ", proId, pmAssignsId, swQcHeadId, tempDate, testId).ToList();

            }
            else if (swQcHeadId == 0)
            {
                query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT distinct case when  sii.IssueSerial is null then 0 else sii.IssueSerial end as IssueSerial, cm.UserFullName,sii.SwQcIssueId,sii.SoftwareVersionNo,sii.IssueScenario,sii.ExpectedOutcome,sii.IssueDetails,sii.RefernceModule,sii.Frequency,sii.Attachment,sii.IssueType,sii.Result,sii.FilesUrl,sii.FilesDetail,sii.IsIssue,sii.IsFile,
            case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas
            FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] sii 
            left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sq on sii.SwQcHeadAssignId=sq.SwQcHeadAssignId and sii.ProjectMasterId=sq.ProjectMasterId
            left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sii.Added
            where (sii.RefernceModule not in ('Camera Automation','Monkey Test','CTS') or sii.RefernceModule is null) and sii.ProjectMasterId={0} and sii.ProjectPmAssignId={1} and sii.SwQcHeadAssignId={2}  
            and (sq.SwQcHeadToQcAssignTime like '%" + tempDate + "%')   and sii.TestPhaseID={4} order by IsIssue,IsFile ", proId, pmAssignsId, swQcHeadId, tempDate, testId).ToList();

            }


            return query;
        }

        public List<SwQcIssueDetailModel> GetSwQcCtsMonkeyOrCameraAutomationDataForPm(string projectId, string swqcInchargeId, string pmAssignId,
            string testPhaseId, DateTime pmAssignDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            long proId;
            long.TryParse(projectId, out proId);

            long swQcHeadId;
            long.TryParse(swqcInchargeId, out swQcHeadId);

            long pmAssignsId;
            long.TryParse(pmAssignId, out pmAssignsId);

            long testId;
            long.TryParse(testPhaseId, out testId);

            string tempDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", pmAssignDate);

            List<SwQcIssueDetailModel> query = new List<SwQcIssueDetailModel>();

            if (swQcHeadId != 0)
            {
                query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT distinct case when  sii.IssueSerial is null then 0 else sii.IssueSerial end as IssueSerial,cm.UserFullName,sii.SwQcIssueId,sii.SoftwareVersionNo,sii.IssueScenario,sii.ExpectedOutcome,sii.IssueDetails,sii.RefernceModule,sii.Frequency,sii.Attachment,sii.IssueType,sii.Result,sii.FilesUrl,sii.FilesDetail,sii.IsIssue,sii.IsFile,
             case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,case when IsApprovedForIncentive='true' then 'YES' else 'NO' end as IsApprovedForIncentives
            FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] sii 
            left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sq on sii.SwQcHeadAssignId=sq.SwQcHeadAssignId and sii.ProjectMasterId=sq.ProjectMasterId
            left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sii.Added
            where sii.RefernceModule in ('Camera Automation','Monkey Test','CTS') and sii.ProjectMasterId={0} and sii.ProjectPmAssignId={1} and sii.SwQcHeadAssignId={2}  
            and (sq.PmToQcHeadAssignTime like '%" + tempDate + "%') and sii.TestPhaseID={4}  order by IsIssue,IsFile ", proId, pmAssignsId, swQcHeadId, tempDate, testId).ToList();

            }
            else if (swQcHeadId == 0)
            {
                query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT distinct case when  sii.IssueSerial is null then 0 else sii.IssueSerial end as IssueSerial,cm.UserFullName,sii.SwQcIssueId,sii.SoftwareVersionNo,sii.IssueScenario,sii.ExpectedOutcome,sii.IssueDetails,sii.RefernceModule,sii.Frequency,sii.Attachment,sii.IssueType,sii.Result,sii.FilesUrl,sii.FilesDetail,sii.IsIssue,sii.IsFile,
             case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,case when IsApprovedForIncentive='true' then 'YES' else 'NO' end as IsApprovedForIncentives
            FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] sii 
            left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sq on sii.SwQcHeadAssignId=sq.SwQcHeadAssignId and sii.ProjectMasterId=sq.ProjectMasterId
            left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sii.Added
            where sii.RefernceModule in ('Camera Automation','Monkey Test','CTS') and sii.ProjectMasterId={0} and sii.ProjectPmAssignId={1} and sii.SwQcHeadAssignId={2}  
            and (sq.SwQcHeadToQcAssignTime like '%" + tempDate + "%') and sii.TestPhaseID={4}  order by IsIssue,IsFile ", proId, pmAssignsId, swQcHeadId, tempDate, testId).ToList();

            }

            return query;
        }

        public List<SwQcPersonalUseFindingsIssueDetailModel> GetPersonalUseFindingsForPm(string projectId, string swqcInchargeId, string pmAssignId, string testPhaseId,
            DateTime pmAssignDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            long proId;
            long.TryParse(projectId, out proId);

            long swQcHeadId;
            long.TryParse(swqcInchargeId, out swQcHeadId);

            long pmAssignsId;
            long.TryParse(pmAssignId, out pmAssignsId);

            long testId;
            long.TryParse(testPhaseId, out testId);

            // string tempDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", swQcHeadToQcAssignTime);

            var query = _dbeEntities.Database.SqlQuery<SwQcPersonalUseFindingsIssueDetailModel>(@"SELECT cm.UserFullName,spp.*,
            case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,case when IsApprovedForIncentive='true' then 'YES' else 'NO' end as IsApprovedForIncentives
            FROM [CellPhoneProject].[dbo].[SwQcPersonalUseFindingsIssueDetails] spp
           left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=spp.Added
            where spp.ProjectMasterId={0} and spp.ProjectPmAssignId={1} and spp.SwQcHeadAssignId={2}  order by IsIssue,IsFile ", proId, pmAssignsId, swQcHeadId, testId).ToList();

            return query;
        }

        public List<ProjectMasterModel> GetProjectListForSwQcHead()
        {
            var getSwProject = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(@"
            select distinct inchargeAssign.ProjectName
            from SwQcHeadAssignsFromPm inchargeAssign").ToList();

            foreach (var model in getSwProject)
            {
                model.ProjectName = model.ProjectName;

            }
            return getSwProject;
        }

        public List<SwQcIssueCategoryModel> GetIssueCategory()
        {
            var getSwProject = _dbeEntities.Database.SqlQuery<SwQcIssueCategoryModel>(@"select distinct [OrdersOfIssues],SwQcIssueCategorytId,QcCategoryName from [CellPhoneProject].[dbo].[SwQcIssueCategory]
            where [IsActive]='true' order by [OrdersOfIssues] asc").ToList();

            return getSwProject;
        }
        public List<SwQcIssueDetailModel> GetSwQcIssueDetailsForSupplier(string projectName, string moduleName, int projectOrders, int softVersionNo, string testPhases)
        {
            List<SwQcIssueDetailModel> query = null;

            if (projectName != null)
            {
                if (projectOrders == 0 && softVersionNo == 0 && moduleName == "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId], case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails]
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5 ", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders != 0 && softVersionNo == 0 && moduleName == "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId], case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,
                    SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and OrderNumber={2}", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders == 0 && softVersionNo != 0 && moduleName == "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId],case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and SoftwareVersionNo={3} ", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders == 0 && softVersionNo == 0 && moduleName != "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId],case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and RefernceModule like '%" + moduleName + "%'  ", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders != 0 && softVersionNo != 0 && moduleName == "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId], case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and OrderNumber={2} and SoftwareVersionNo={3} ", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders == 0 && softVersionNo != 0 && moduleName != "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId], case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and SoftwareVersionNo={3}  and RefernceModule like '%" + moduleName + "%'  ", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders != 0 && softVersionNo == 0 && moduleName != "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId],case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and OrderNumber={2}  and RefernceModule like '%" + moduleName + "%'  ", projectName, moduleName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders != 0 && softVersionNo != 0 && moduleName != "0" && testPhases != "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId], case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],[ExpectedOutcome]
                    ,[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,cm1.UserFullName,WaltonQcStatus,SupplierStatus
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added 
                    where ProjectName={0} and TestPhaseID !=5  and SoftwareVersionNo={3} and OrderNumber={2} and RefernceModule like '%" + moduleName + "%'    ", projectName, moduleName, projectOrders, softVersionNo).ToList();

                }
                else if (projectOrders == 0 && softVersionNo != 0 && moduleName == "0" && testPhases == "5")
                {
                    query = _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(@"SELECT [SwQcIssueId], case when IssueSerial is null then 0 else IssueSerial end as IssueSerial,[SwQcHeadAssignId],[SwQcAssignId],[ProjectPmAssignId],[ProjectMasterId],[OrderNumber],[ProjectName],[ProjectType],[IssueScenario],
                    [ExpectedOutcome],[IssueDetails],[RefernceModule],[Frequency],[IssueReproducePath],[Attachment],[IssueType],[Result],[TestPhaseID],[SoftwareVersionName],[SoftwareVersionNo],[FilesUrl],[FilesDetail],[Upload]
                    ,case when IsIssue='true' then 'Issue' else 'File' end as FileOrIssue,[WaltonQcComDate],[WaltonQcComment],[FixedVersion],[SupplierComDate],[SupplierComment],[WaltonPmComDate],[WaltonPmComment],[IsSmart],[IsFeature],
                    case when IsApprovedForChina='true' then 'YES' else 'NO' end as IsApprovedForChinas,SwQcIssueDetails.[Added],SwQcIssueDetails.[AddedDate],IsApprovedForChina,WaltonQcStatus,SupplierStatus,cm1.UserFullName,Demo
                    FROM [CellPhoneProject].[dbo].[SwQcIssueDetails] 
                    left join CellPhoneProject.dbo.CmnUsers cm1 on cm1.CmnUserId=[SwQcIssueDetails].Added
                   
                    where ProjectName={0} and SoftwareVersionNo={3} and TestPhaseID={4}  order by IssueSerial asc", projectName, moduleName, projectOrders, softVersionNo, testPhases).ToList();
                    //Demo='Demo'
                }
            }

            return query;
        }

        public string UpdateSwQcIssueDetailModelForSupplier(SwQcIssueDetailModel supplierUpdate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updatedAssembly = (from c in _dbeEntities.SwQcIssueDetails
                                   where c.SwQcIssueId == supplierUpdate.SwQcIssueId && c.SwQcAssignId == supplierUpdate.SwQcAssignId
                                   select c).FirstOrDefault();

            updatedAssembly.WaltonPmComDate = supplierUpdate.WaltonPmComDate;
            updatedAssembly.WaltonPmComment = supplierUpdate.WaltonPmComment;
            updatedAssembly.SupplierComDate = supplierUpdate.SupplierComDate;
            updatedAssembly.SupplierComment = supplierUpdate.SupplierComment;
            if (supplierUpdate.SupplierStatus != "SELECT")
            {
                updatedAssembly.SupplierStatus = supplierUpdate.SupplierStatus;
            }
            else
            {
                updatedAssembly.SupplierStatus = updatedAssembly.SupplierStatus;
            }
            updatedAssembly.Updated = userId;
            updatedAssembly.UpdatedDate = DateTime.Now;
            _dbeEntities.SwQcIssueDetails.AddOrUpdate(updatedAssembly);
            _dbeEntities.SaveChanges();

            //bool//
            var isSaveCheck = GetSupplierFeedbackData(supplierUpdate);

            if ((supplierUpdate.SupplierStatus == "NOT FIXED" ||
              supplierUpdate.SupplierStatus == "SUPPLIER CAN'T FIXED"
             || supplierUpdate.SupplierStatus == "FIXED" || supplierUpdate.SupplierStatus == "OPTIMIZED"
            || supplierUpdate.SupplierStatus == "IMPROVED" || supplierUpdate.SupplierStatus == "NEW ISSUE")
            && (updatedAssembly.WaltonQcStatus != "FIXED") && isSaveCheck != true)
            {

                var updatedAssembly1 = (from c in _dbeEntities.SwQcIssueDetails
                                        where c.SwQcIssueId == supplierUpdate.SwQcIssueId && c.SwQcAssignId == supplierUpdate.SwQcAssignId
                                        select c).FirstOrDefault();

                updatedAssembly1.SwQcHeadAssignId = updatedAssembly1.SwQcHeadAssignId;
                updatedAssembly1.SwQcAssignId = updatedAssembly1.SwQcAssignId;
                updatedAssembly1.ProjectPmAssignId = updatedAssembly1.ProjectPmAssignId;
                updatedAssembly1.ProjectMasterId = updatedAssembly1.ProjectMasterId;

                updatedAssembly1.OrderNumber = updatedAssembly1.OrderNumber;
                updatedAssembly1.ProjectName = updatedAssembly1.ProjectName;
                updatedAssembly1.ProjectType = updatedAssembly1.ProjectType;
                updatedAssembly1.IssueScenario = updatedAssembly1.IssueScenario;
                updatedAssembly1.ExpectedOutcome = updatedAssembly1.ExpectedOutcome;
                updatedAssembly1.IssueDetails = updatedAssembly1.IssueDetails;
                updatedAssembly1.RefernceModule = updatedAssembly1.RefernceModule;
                updatedAssembly1.Frequency = updatedAssembly1.Frequency;
                updatedAssembly1.IssueReproducePath = updatedAssembly1.IssueReproducePath;
                updatedAssembly1.Attachment = updatedAssembly1.Attachment;
                updatedAssembly1.IssueType = updatedAssembly1.IssueType;
                updatedAssembly1.Result = updatedAssembly1.Result;
                updatedAssembly1.TestPhaseID = updatedAssembly1.TestPhaseID;

                updatedAssembly1.SoftwareVersionNo = updatedAssembly1.SoftwareVersionNo + 1;

                if (updatedAssembly1.SoftwareVersionNo != 0)
                {
                    var softName = (from c in _dbeEntities.SwQcHeadAssignsFromPms
                                    where c.ProjectName == supplierUpdate.ProjectName //&& c.SwQcAssignId == supplierUpdate.SwQcAssignId
                                    && c.SoftwareVersionNo == updatedAssembly1.SoftwareVersionNo
                                    select c).FirstOrDefault();

                    if (softName != null)
                    {
                        updatedAssembly1.SoftwareVersionName = softName.SoftwareVersionName;
                    }
                    else
                    {
                        updatedAssembly1.SoftwareVersionName = "";
                    }
                }

                updatedAssembly1.WaltonQcComDate = updatedAssembly.WaltonQcComDate;
                updatedAssembly1.WaltonQcComment = updatedAssembly.WaltonQcComment;
                updatedAssembly1.WaltonQcStatus = updatedAssembly.WaltonQcStatus;

                //updatedAssembly1.WaltonQcComDate = null;
                //updatedAssembly1.WaltonQcComment = null;
                //updatedAssembly1.WaltonQcStatus = null;

                updatedAssembly1.SupplierComDate = supplierUpdate.SupplierComDate;
                updatedAssembly1.SupplierComment = supplierUpdate.SupplierComment;
                // updatedAssembly1.SupplierStatus = supplierUpdate.SupplierStatus;
                if (supplierUpdate.SupplierStatus != "SELECT")
                {
                    updatedAssembly1.SupplierStatus = supplierUpdate.SupplierStatus;

                }
                else
                {
                    updatedAssembly1.SupplierStatus = updatedAssembly.SupplierStatus;
                }

                updatedAssembly1.WaltonPmComDate = supplierUpdate.WaltonPmComDate;
                updatedAssembly1.WaltonPmComment = supplierUpdate.WaltonPmComment;


                updatedAssembly1.SupplierFeedbackForAppend = "SUPPLIER_FEEDBACK";

                updatedAssembly1.FilesUrl = updatedAssembly1.FilesUrl;
                updatedAssembly1.FilesDetail = updatedAssembly1.FilesDetail;
                updatedAssembly1.Upload = updatedAssembly1.Upload;
                updatedAssembly1.IsIssue = updatedAssembly1.IsIssue;
                updatedAssembly1.IsFile = updatedAssembly1.IsFile;
                updatedAssembly1.IsSmart = updatedAssembly1.IsSmart;
                updatedAssembly1.IsFeature = updatedAssembly1.IsFeature;
                updatedAssembly1.IsTab = updatedAssembly1.IsTab;
                updatedAssembly1.IsWalpad = updatedAssembly1.IsWalpad;
                updatedAssembly1.IsApprovedForChina = updatedAssembly1.IsApprovedForChina;

                updatedAssembly1.Updated = userId;
                updatedAssembly1.UpdatedDate = DateTime.Now;
                updatedAssembly1.IssueSerial = updatedAssembly.IssueSerial;

                _dbeEntities.SwQcIssueDetails.Add(updatedAssembly1);
                _dbeEntities.SaveChanges();

                SwQcIssueDetailsLog swQcIssue = new SwQcIssueDetailsLog();

                swQcIssue.ProjectMasterId = supplierUpdate.ProjectMasterId;
                swQcIssue.ProjectName = supplierUpdate.ProjectName;
                swQcIssue.OrderNumber = supplierUpdate.OrderNumber;
                swQcIssue.SwQcAssignId = supplierUpdate.SwQcAssignId;
                swQcIssue.SwQcHeadAssignId = supplierUpdate.SwQcHeadAssignId;
                swQcIssue.SwQcIssueId = supplierUpdate.SwQcIssueId;
                swQcIssue.WaltonQcComDate = updatedAssembly1.WaltonQcComDate;
                swQcIssue.WaltonQcComment = updatedAssembly1.WaltonQcComment;
                swQcIssue.WaltonQcStatus = updatedAssembly1.WaltonQcStatus;
                swQcIssue.WaltonPmComDate = supplierUpdate.WaltonPmComDate;
                swQcIssue.WaltonPmComment = supplierUpdate.WaltonPmComment;
                swQcIssue.SupplierComDate = supplierUpdate.SupplierComDate;
                swQcIssue.SupplierComment = supplierUpdate.SupplierComment;
                if (supplierUpdate.SupplierStatus != "SELECT")
                {
                    swQcIssue.SupplierStatus = supplierUpdate.SupplierStatus;
                }
                else
                {
                    swQcIssue.SupplierStatus = updatedAssembly1.SupplierStatus;
                }
                swQcIssue.SupplierFeedbackForAppend = "SUPPLIER_FEEDBACK";
                swQcIssue.Added = userId;
                swQcIssue.AddedDate = DateTime.Now;
                swQcIssue.IssueSerial = updatedAssembly.IssueSerial;

                _dbeEntities.SwQcIssueDetailsLogs.AddOrUpdate(swQcIssue);
                _dbeEntities.SaveChanges();
            }
            if
                ((supplierUpdate.SupplierStatus == "FIXED" && supplierUpdate.WaltonQcStatus == "FIXED") ||
                (updatedAssembly.SupplierStatus == "FIXED" && updatedAssembly.WaltonQcStatus == "FIXED") ||
                (updatedAssembly.SupplierStatus == "FIXED" && supplierUpdate.WaltonQcStatus == "FIXED") ||
                (supplierUpdate.SupplierStatus == "FIXED" && updatedAssembly.WaltonQcStatus == "FIXED"))
            {
                SwQcIssueDetailsLog swQcIssue = new SwQcIssueDetailsLog();

                swQcIssue.ProjectMasterId = supplierUpdate.ProjectMasterId;
                swQcIssue.ProjectName = supplierUpdate.ProjectName;
                swQcIssue.OrderNumber = supplierUpdate.OrderNumber;
                swQcIssue.SwQcAssignId = supplierUpdate.SwQcAssignId;
                swQcIssue.SwQcHeadAssignId = supplierUpdate.SwQcHeadAssignId;
                swQcIssue.SwQcIssueId = supplierUpdate.SwQcIssueId;
                swQcIssue.WaltonQcComDate = updatedAssembly.WaltonQcComDate;
                swQcIssue.WaltonQcComment = updatedAssembly.WaltonQcComment;
                swQcIssue.WaltonQcStatus = updatedAssembly.WaltonQcStatus;
                swQcIssue.WaltonPmComDate = supplierUpdate.WaltonPmComDate;
                swQcIssue.WaltonPmComment = supplierUpdate.WaltonPmComment;
                swQcIssue.SupplierComDate = supplierUpdate.SupplierComDate;
                swQcIssue.SupplierComment = supplierUpdate.SupplierComment;
                if (supplierUpdate.SupplierStatus != "SELECT")
                {
                    swQcIssue.SupplierStatus = supplierUpdate.SupplierStatus;
                }
                else
                {
                    swQcIssue.SupplierStatus = updatedAssembly.SupplierStatus;
                }
                //swQcIssue.SupplierFeedbackForAppend = "SUPPLIER_FEEDBACK";
                swQcIssue.Added = userId;
                swQcIssue.AddedDate = DateTime.Now;
                swQcIssue.IssueSerial = updatedAssembly.IssueSerial;

                _dbeEntities.SwQcIssueDetailsLogs.AddOrUpdate(swQcIssue);
                _dbeEntities.SaveChanges();
            }
            return "OK";
        }

        public List<SwQcAssignsFromQcHeadModel> GetSwQcsAssignsInfo(string projectName, int projectOrders, int softVersionNo, string testPhases)
        {
            List<SwQcAssignsFromQcHeadModel> swQcAssigns = null;

            if (projectName.Trim() != null)
            {
                if (projectOrders == 0 && softVersionNo == 0 && testPhases == "0")
                {
                    swQcAssigns = _dbeEntities.Database.SqlQuery<SwQcAssignsFromQcHeadModel>(@"
                    select distinct sqi.SwQcHeadAssignId,sqi.ProjectName,pm.SourcingType as PoCategory, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CellPhoneProject.dbo.CmnUsers cmn1 left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sw on sw.SwQcUserId=cmn1.CmnUserId 
                    and sw.SwQcHeadAssignId=sqi.SwQcHeadAssignId WHERE sw.SwQcUserId=cmn1.CmnUserId 
                    and sw.ProjectName={0}
                    and sw.Status not in ('INACTIVE') ORDER BY cmn1.UserFullName FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                    sqi.TestPhaseID,sp.TestPhaseName,
                    case when sqpm.SwQcHeadToQcAssignTime is null then (select top 1 SwQcHeadToQcAssignTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcHeadToQcAssignTime,
                    case when sqpm.SwQcFinishedTime is null then (select top 1 SwQcEndTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcFinishedTime

                    from CellPhoneProject.dbo.SwQcAssignsFromQcHead sqi
                    left join CellPhoneProject.dbo.SwQcHeadAssignsFromPm sqpm on sqpm.SwQcHeadAssignId=sqi.SwQcHeadAssignId
                    left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectName=sqi.ProjectName
                    left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sqi.SwQcUserId
                    left join CellPhoneProject.dbo.SwQcTestPhase sp on sp.TestPhaseID=sqi.TestPhaseID

                    where sqi.ProjectName={0}  and sqi.status in ('ASSIGNED','QCCOMPLETED','RECOMMENDED')    
                    and pm.SourcingType in (select top 1 SourcingType from CellPhoneProject.dbo.ProjectMasters where ProjectMasterId=sqpm.ProjectMasterId)                  
                    group by pm.ProjectMasterId,sqi.ProjectName,pm.SourcingType, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    sqpm.SwQcHeadToQcAssignTime,sqi.SwQcHeadToQcAssignTime,sqi.SwQcUserId,sqi.SwQcHeadAssignId,sqpm.SwQcFinishedTime,sqi.TestPhaseID,sp.TestPhaseName
                    order by sqi.PmToQcHeadAssignTime desc", projectName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders != 0 && softVersionNo == 0 && testPhases == "0")
                {
                    swQcAssigns = _dbeEntities.Database.SqlQuery<SwQcAssignsFromQcHeadModel>(@"
                    select distinct sqi.SwQcHeadAssignId,sqi.ProjectName,pm.SourcingType as PoCategory, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CellPhoneProject.dbo.CmnUsers cmn1 left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sw on sw.SwQcUserId=cmn1.CmnUserId 
                    and sw.SwQcHeadAssignId=sqi.SwQcHeadAssignId WHERE sw.SwQcUserId=cmn1.CmnUserId and sw.ProjectName={0} and sw.Status not in ('INACTIVE') ORDER BY cmn1.UserFullName FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                    sqi.TestPhaseID,sp.TestPhaseName,
                    case when sqpm.SwQcHeadToQcAssignTime is null then (select top 1 SwQcHeadToQcAssignTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcHeadToQcAssignTime,
                    case when sqpm.SwQcFinishedTime is null then (select top 1 SwQcEndTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcFinishedTime

                    from CellPhoneProject.dbo.SwQcAssignsFromQcHead sqi
                    left join CellPhoneProject.dbo.SwQcHeadAssignsFromPm sqpm on sqpm.SwQcHeadAssignId=sqi.SwQcHeadAssignId
                    left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectName=sqi.ProjectName
                    left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sqi.SwQcUserId
                    left join CellPhoneProject.dbo.SwQcTestPhase sp on sp.TestPhaseID=sqi.TestPhaseID

                    where sqi.ProjectName={0} and sqi.OrderNumber={1}  and sqi.status in ('ASSIGNED','QCCOMPLETED','RECOMMENDED')   
                    and pm.SourcingType in (select top 1 SourcingType from CellPhoneProject.dbo.ProjectMasters where ProjectMasterId=sqpm.ProjectMasterId)                    
                    group by pm.ProjectMasterId,sqi.ProjectName,pm.SourcingType, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    sqpm.SwQcHeadToQcAssignTime,sqi.SwQcHeadToQcAssignTime,sqi.SwQcUserId,sqi.SwQcHeadAssignId,sqpm.SwQcFinishedTime,sqi.TestPhaseID,sp.TestPhaseName
                    order by sqi.PmToQcHeadAssignTime desc", projectName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders == 0 && softVersionNo != 0 && testPhases == "0")
                {
                    swQcAssigns = _dbeEntities.Database.SqlQuery<SwQcAssignsFromQcHeadModel>(@"
                    select distinct sqi.SwQcHeadAssignId,sqi.ProjectName,pm.SourcingType as PoCategory, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CellPhoneProject.dbo.CmnUsers cmn1 left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sw on sw.SwQcUserId=cmn1.CmnUserId 
                    and sw.SwQcHeadAssignId=sqi.SwQcHeadAssignId WHERE sw.SwQcUserId=cmn1.CmnUserId and sw.ProjectName={0} and sw.Status not in ('INACTIVE') ORDER BY cmn1.UserFullName FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                    sqi.TestPhaseID,sp.TestPhaseName,
                    case when sqpm.SwQcHeadToQcAssignTime is null then (select top 1 SwQcHeadToQcAssignTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcHeadToQcAssignTime,
                    case when sqpm.SwQcFinishedTime is null then (select top 1 SwQcEndTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcFinishedTime

                    from CellPhoneProject.dbo.SwQcAssignsFromQcHead sqi
                    left join CellPhoneProject.dbo.SwQcHeadAssignsFromPm sqpm on sqpm.SwQcHeadAssignId=sqi.SwQcHeadAssignId
                    left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectName=sqi.ProjectName
                    left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sqi.SwQcUserId
                    left join CellPhoneProject.dbo.SwQcTestPhase sp on sp.TestPhaseID=sqi.TestPhaseID

                    where sqi.ProjectName={0} and sqi.SoftwareVersionNo={2} and sqi.status in ('ASSIGNED','QCCOMPLETED','RECOMMENDED')                     
                     and pm.SourcingType in (select top 1 SourcingType from CellPhoneProject.dbo.ProjectMasters where ProjectMasterId=sqpm.ProjectMasterId)   
                    group by pm.ProjectMasterId,sqi.ProjectName,pm.SourcingType, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    sqpm.SwQcHeadToQcAssignTime,sqi.SwQcHeadToQcAssignTime,sqi.SwQcUserId,sqi.SwQcHeadAssignId,sqpm.SwQcFinishedTime,sqi.TestPhaseID,sp.TestPhaseName
                    order by sqi.PmToQcHeadAssignTime desc", projectName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders != 0 && softVersionNo != 0 && testPhases == "0")
                {
                    swQcAssigns = _dbeEntities.Database.SqlQuery<SwQcAssignsFromQcHeadModel>(@"
                    select distinct sqi.SwQcHeadAssignId,sqi.ProjectName,pm.SourcingType as PoCategory, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CellPhoneProject.dbo.CmnUsers cmn1 left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sw on sw.SwQcUserId=cmn1.CmnUserId 
                    and sw.SwQcHeadAssignId=sqi.SwQcHeadAssignId WHERE sw.SwQcUserId=cmn1.CmnUserId and sw.ProjectName={0} and sw.Status not in ('INACTIVE') ORDER BY cmn1.UserFullName FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                    sqi.TestPhaseID,sp.TestPhaseName,
                    case when sqpm.SwQcHeadToQcAssignTime is null then (select top 1 SwQcHeadToQcAssignTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcHeadToQcAssignTime,
                    case when sqpm.SwQcFinishedTime is null then (select top 1 SwQcEndTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcFinishedTime

                    from CellPhoneProject.dbo.SwQcAssignsFromQcHead sqi
                    left join CellPhoneProject.dbo.SwQcHeadAssignsFromPm sqpm on sqpm.SwQcHeadAssignId=sqi.SwQcHeadAssignId
                    left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectName=sqi.ProjectName
                    left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sqi.SwQcUserId
                    left join CellPhoneProject.dbo.SwQcTestPhase sp on sp.TestPhaseID=sqi.TestPhaseID

                    where sqi.ProjectName={0} and sqi.OrderNumber={1} and sqi.SoftwareVersionNo={2} and sqi.status in ('ASSIGNED','QCCOMPLETED','RECOMMENDED') 
                    and pm.SourcingType in (select top 1 SourcingType from CellPhoneProject.dbo.ProjectMasters where ProjectMasterId=sqpm.ProjectMasterId)   
                    group by pm.ProjectMasterId,sqi.ProjectName,pm.SourcingType, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    sqpm.SwQcHeadToQcAssignTime,sqi.SwQcHeadToQcAssignTime,sqi.SwQcUserId,sqi.SwQcHeadAssignId,sqpm.SwQcFinishedTime,sqi.TestPhaseID,sp.TestPhaseName
                    order by sqi.PmToQcHeadAssignTime desc", projectName, projectOrders, softVersionNo).ToList();
                }
                else if (projectOrders == 0 && softVersionNo != 0 && testPhases != "0")
                {
                    swQcAssigns = _dbeEntities.Database.SqlQuery<SwQcAssignsFromQcHeadModel>(@"select distinct sqi.SwQcHeadAssignId,sqi.ProjectName,pm.SourcingType as PoCategory, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    STUFF((SELECT ', '  + cmn1.UserFullName FROM CellPhoneProject.dbo.CmnUsers cmn1 left join CellPhoneProject.dbo.SwQcAssignsFromQcHead sw on sw.SwQcUserId=cmn1.CmnUserId 
                    and sw.SwQcHeadAssignId=sqi.SwQcHeadAssignId WHERE sw.SwQcUserId=cmn1.CmnUserId and sw.ProjectName={0} and sw.Status not in ('INACTIVE') ORDER BY cmn1.UserFullName FOR XML PATH('')),1,2,'')  AS  AssignedPerson,
                    sqi.TestPhaseID,sp.TestPhaseName,
                    case when sqpm.SwQcHeadToQcAssignTime is null then (select top 1 SwQcHeadToQcAssignTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcHeadToQcAssignTime,
                    case when sqpm.SwQcFinishedTime is null then (select top 1 SwQcEndTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead
                    where SwQcHeadAssignId=sqi.SwQcHeadAssignId and TestPhaseID=sqi.TestPhaseID) else sqpm.SwQcHeadToQcAssignTime end as SwQcFinishedTime

                    from CellPhoneProject.dbo.SwQcAssignsFromQcHead sqi
                    left join CellPhoneProject.dbo.SwQcHeadAssignsFromPm sqpm on sqpm.SwQcHeadAssignId=sqi.SwQcHeadAssignId
                    left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectName=sqi.ProjectName
                    left join CellPhoneProject.dbo.CmnUsers cm on cm.CmnUserId=sqi.SwQcUserId
                    left join CellPhoneProject.dbo.SwQcTestPhase sp on sp.TestPhaseID=sqi.TestPhaseID

                    where sqi.ProjectName={0}  and sqi.SoftwareVersionNo={2} and sqi.TestPhaseID={3}  and sqi.status in ('ASSIGNED','QCCOMPLETED','RECOMMENDED') 
                   and pm.SourcingType in (select top 1 SourcingType from CellPhoneProject.dbo.ProjectMasters where ProjectMasterId=sqpm.ProjectMasterId)   
                    group by pm.ProjectMasterId,sqi.ProjectName,pm.SourcingType, sqi.OrderNumber, sqi.SoftwareVersionName,sqi.SoftwareVersionNo,sqi.PmToQcHeadAssignTime,
                    sqpm.SwQcHeadToQcAssignTime,sqi.SwQcHeadToQcAssignTime,sqi.SwQcUserId,sqi.SwQcHeadAssignId,sqpm.SwQcFinishedTime,sqi.TestPhaseID,sp.TestPhaseName
                    order by sqi.PmToQcHeadAssignTime desc", projectName, projectOrders, softVersionNo, testPhases).ToList();
                }

            }
            return swQcAssigns;
        }

        public List<SwQcAssignsFromQcHeadModel> GetSwQcHeadToQcAssignInfo(long projectId)
        {
            string query = string.Format(@"select distinct swQc.ProjectName,swQc.SwQcHeadAssignId,swQc.ProjectPmAssignId,swQc.PmToQcHeadAssignTime,swQc.TestPhaseID,swQc.ProjectMasterId,
            swQc.SoftwareVersionName,swQc.SoftwareVersionNo,swTxtPh.TestPhaseName,swQc.SwQcHeadToPmSubmitTime,swQc.SwQcHeadToPmForwardComment,
            (select top 1 Status from CellPhoneProject.dbo.SwQcAssignsFromQcHead where SwQcHeadAssignId=swQc.SwQcHeadAssignId and ProjectMasterId=swqc.ProjectMasterId
            and SwQcHeadToQcAssignTime =swQc.SwQcHeadToQcAssignTime) as Status,

            (select top 1 SwQcHeadToQcAssignTime from CellPhoneProject.dbo.SwQcAssignsFromQcHead where SwQcHeadAssignId=swQc.SwQcHeadAssignId and ProjectMasterId=swqc.ProjectMasterId
            and SwQcHeadToQcAssignTime =swQc.SwQcHeadToQcAssignTime) as SwQcHeadToQcAssignTime,

            (select top 1 SwQcHeadToQcAssignComment from CellPhoneProject.dbo.SwQcAssignsFromQcHead where SwQcHeadAssignId=swQc.SwQcHeadAssignId and ProjectMasterId=swqc.ProjectMasterId
            and SwQcHeadToQcAssignTime =swQc.SwQcHeadToQcAssignTime) as SwQcHeadToQcAssignComment,

            swQc.IsFinalPhaseMP,case when swQc.IsFinalPhaseMP='true' then 'YES' else 'NO' end as IsFinalPhaseMPs

            from CellPhoneProject.dbo.SwQcAssignsFromQcHead swQc
            left join CellPhoneProject.dbo.SwQcTestPhase swTxtPh on swqc.TestPhaseID=swTxtPh.TestPhaseID 
            where swQc.ProjectMasterId={0} and swQc.SwQcHeadAssignId=0 and swqc.Status not in ('INACTIVE') 
            group by swQc.ProjectName,swQc.SwQcHeadAssignId,swQc.ProjectPmAssignId,swQc.PmToQcHeadAssignTime,swQc.TestPhaseID,swQc.ProjectMasterId,
            swQc.SoftwareVersionName,swQc.SoftwareVersionNo,swTxtPh.TestPhaseName,swQc.SwQcHeadToPmSubmitTime,swQc.SwQcHeadToPmForwardComment,swQc.IsFinalPhaseMP,swQc.SwQcHeadToQcAssignTime
            order by SwQcHeadToQcAssignTime desc", projectId);
            var exe = _dbeEntities.Database.SqlQuery<SwQcAssignsFromQcHeadModel>(query).ToList();
            return exe;
        }

        public bool GetSupplierFeedbackData(SwQcIssueDetailModel supplierUpdate)
        {
            //int MonNum = Convert.ToInt32(monNum);
            List<SwQcIssueDetailModel> getIncentiveReports = null;
            if (supplierUpdate.IssueScenario != null && supplierUpdate.SoftwareVersionNo != 0)
            {
                //                string getIncentiveReportQuery = string.Format(@"select ProjectName from [CellPhoneProject].[dbo].[SwQcIssueDetails]
                //                where SwQcAssignId={0} and SwQcHeadAssignId={1} and SoftwareVersionNo='"+supplierUpdate.SoftwareVersionNo+"' + 1 and [SupplierFeedbackForAppend]='SUPPLIER_FEEDBACK' ", supplierUpdate.SwQcAssignId, supplierUpdate.SwQcHeadAssignId, supplierUpdate.SoftwareVersionNo);
                //                getIncentiveReports =
                //                   _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(getIncentiveReportQuery).ToList();

                var scenario = supplierUpdate.IssueScenario.Replace("'", "''");

                string getIncentiveReportQuery = string.Format(@"select top 1 ProjectName from [CellPhoneProject].[dbo].[SwQcIssueDetails]
                where  ProjectName ='" + supplierUpdate.ProjectName + "'  and IssueScenario ='{0}'  and SoftwareVersionNo='" + supplierUpdate.SoftwareVersionNo + "' + 1 and [SupplierFeedbackForAppend]='SUPPLIER_FEEDBACK' order by AddedDate desc ",
                scenario, supplierUpdate.ExpectedOutcome, supplierUpdate.SoftwareVersionNo);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<SwQcIssueDetailModel>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<SwQcHeadAssignsFromPmModel> GetProjectVersionName(string projectId, int swVersionNo, long testPhaseIds)
        {
            //            var query = _dbEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"            
            //            select distinct ProjectName as SoftwareVersionName from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}
            //            union 
            //            select distinct SoftwareVersionName from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}
            //            order by SoftwareVersionName asc", projectId).ToList();

            var proName = projectId.Replace("'", "''");

            var query = new List<SwQcHeadAssignsFromPmModel>();

            if (proName != null && swVersionNo == 0 && testPhaseIds == 0)
            {
                //                query = _dbEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"            
                //                select distinct ProjectName as SoftVersionName,SoftwareVersionNo=1,ProjectName as SoftwareVersionName from
                //                CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}
                //                union 
                //                select distinct SoftwareVersionName as SoftVersionName,SoftwareVersionNo,  'SW_V_'+ cast(SoftwareVersionNo as varchar) as SoftwareVersionName
                //                from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}
                //                order by SoftwareVersionNo asc", proName.Trim()).ToList();

                query = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"select distinct ProjectName as SoftVersionName,SoftwareVersionNo=1,ProjectName as SoftwareVersionName,TestPhaseID=0 from
                CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0} and TestPhaseID !=10
                union 
                select distinct SoftwareVersionName as SoftVersionName,SoftwareVersionNo,  case when TestPhaseID=5 then 'SW_V_'+ cast(SoftwareVersionNo as varchar)+'_Demo' 
                else 'SW_V_'+ cast(SoftwareVersionNo as varchar) end as SoftwareVersionName, case when  TestPhaseID !=5 then 0 else TestPhaseID end as TestPhaseID
                from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0} and TestPhaseID !=10 
                order by SoftwareVersionNo asc", proName.Trim()).ToList();
            }
            else if (proName != null && swVersionNo != 0 && testPhaseIds != 5)
            {
                //                query = _dbEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"            
                //               select distinct ProjectName as SoftVersionName,SoftwareVersionNo=1,ProjectName as SoftwareVersionName from
                //                CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}  
                //                union 
                //                select distinct SoftwareVersionName as SoftVersionName,SoftwareVersionNo,  'SW_V_'+ cast(SoftwareVersionNo as varchar) as SoftwareVersionName
                //                from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}  and SoftwareVersionNo={1}
                //                order by SoftwareVersionNo asc", proName.Trim(), swVersionNo).ToList();

                query = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"select distinct ProjectName as SoftVersionName,SoftwareVersionNo=1,ProjectName as SoftwareVersionName,TestPhaseID=0 from
                CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0} and TestPhaseID !=10
                union 
                select distinct SoftwareVersionName as SoftVersionName,SoftwareVersionNo,  
                case when TestPhaseID=5 then 'SW_V_'+ cast(SoftwareVersionNo as varchar)+'_Demo' else 'SW_V_'+ cast(SoftwareVersionNo as varchar) end as SoftwareVersionName,  case when  TestPhaseID !=5 then 0 else TestPhaseID end as TestPhaseID
                from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}   and SoftwareVersionNo={1} and TestPhaseID !=10
                order by SoftwareVersionNo asc", proName.Trim(), swVersionNo).ToList();
            }
            else if (proName != null && swVersionNo != 0 && testPhaseIds == 5)
            {
                query = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"select distinct ProjectName as SoftVersionName,SoftwareVersionNo=1,ProjectName as SoftwareVersionName,TestPhaseID=0 from
                CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0} and TestPhaseID !=10
                union 
                select distinct SoftwareVersionName as SoftVersionName,SoftwareVersionNo,  
                case when TestPhaseID=5 then 'SW_V_'+ cast(SoftwareVersionNo as varchar)+'_Demo' else 'SW_V_'+ cast(SoftwareVersionNo as varchar) end as SoftwareVersionName,  case when  TestPhaseID !=5 then 0 else TestPhaseID end as TestPhaseID
                from CellPhoneProject.dbo.SwQcHeadAssignsFromPm where ProjectName={0}  and SoftwareVersionNo={1} and TestPhaseID =5
                order by SoftwareVersionNo asc", proName.Trim(), swVersionNo).ToList();
            }


            return query;
        }

        public SwQcHeadAssignsFromPm GetAllVersionNameForPm(long swVerNo, long proId, long testPhases)
        {
            var proNames =
               (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == proId select pm.ProjectName)
                   .FirstOrDefault();
            var getSwQcInchargeTestCounts = new SwQcHeadAssignsFromPm();

            if (testPhases != 5)
            {
                getSwQcInchargeTestCounts = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPm>(@"select * from SwQcHeadAssignsFromPm
                where ProjectName={1} and SoftwareVersionNo={0} and TestPhaseID !=5 and  TestPhaseID !=10", swVerNo, proNames, testPhases).FirstOrDefault();
            }
            else if (testPhases == 5)
            {
                getSwQcInchargeTestCounts = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPm>(@"select * from SwQcHeadAssignsFromPm
                where ProjectName={1} and SoftwareVersionNo={0} and TestPhaseID =5", swVerNo, proNames, testPhases).FirstOrDefault();
            }


            return getSwQcInchargeTestCounts;
        }

        public SwQcHeadAssignsFromPm GetVersionNameForPm(long swVerNo, long proId)
        {
            var proNames =
                (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == proId select pm.ProjectName)
                    .FirstOrDefault();

            var getSwQcInchargeTestCounts = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPm>(@"select * from SwQcHeadAssignsFromPm

            where ProjectName={1} and SoftwareVersionNo={0} and TestPhaseID !=5", swVerNo, proNames).FirstOrDefault();


            return getSwQcInchargeTestCounts;
        }

        #region Supplier Feedback Excel
        public bool UpdateDbByExcel(string projectName, long softVersion, HttpPostedFileBase excelFile, string testPhaseIds)
        {
            var dbeEntities = new CellPhoneProjectEntities();

            long testPhaseId;
            long.TryParse(testPhaseIds, out testPhaseId);

            string ExcuteMsg = string.Empty;
            HttpPostedFileBase file = excelFile;
            //Extention Check
            if (excelFile.FileName.EndsWith("xls") || excelFile.FileName.EndsWith("xlsx") ||
                excelFile.FileName.EndsWith("XLS") ||
                excelFile.FileName.EndsWith("XLSX"))
            {
                //Null Exp Check
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    try
                    {
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            //var myStr = "SW_V_" + softVersion;
                            var myStr = "";

                            if (testPhaseId == 5)
                            {
                                myStr = "SW_V_" + softVersion + "_Demo";
                            }
                            else
                            {
                                myStr = "SW_V_" + softVersion;
                            }

                            ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == myStr);
                            if (workSheet == null)
                                return false;
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            var nullCount = 0;

                            for (int rowIterator = 3; rowIterator <= noOfRow; rowIterator++)
                            {
                                var swQcIssueDetail = new SwQcIssueDetail();

                                var serial = 0;
                                var supplierComment = "";
                                var supplierStatus = "";

                                serial = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value) == 0
                                    ? 0
                                    : Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value);

                                supplierStatus = workSheet.Cells[rowIterator, 13].Value == null
                                 ? null
                                 : workSheet.Cells[rowIterator, 13].Value.ToString();

                                supplierComment = workSheet.Cells[rowIterator, 14].Value == null
                                 ? null
                                 : workSheet.Cells[rowIterator, 14].Value.ToString();


                                var swData = new SwQcIssueDetail();

                                if (testPhaseId == 5)
                                {
                                    swData = (from c in dbeEntities.SwQcIssueDetails
                                              where c.IssueSerial == serial && c.ProjectName == projectName && c.SoftwareVersionNo == softVersion && c.TestPhaseID == testPhaseId
                                              select c).FirstOrDefault();
                                }
                                else
                                {
                                    swData = (from c in dbeEntities.SwQcIssueDetails
                                              where c.IssueSerial == serial && c.ProjectName == projectName && c.SoftwareVersionNo == softVersion && c.TestPhaseID != 5
                                              select c).FirstOrDefault();
                                }

                                if (swData == null || (supplierStatus == null && supplierComment == null))
                                {
                                    nullCount++;
                                    if (nullCount == (noOfRow - 2))
                                        return false;
                                    continue;

                                }

                                swData.SupplierStatus = supplierStatus;
                                swData.SupplierComment = supplierComment;

                                _dbeEntities.SwQcIssueDetails.AddOrUpdate(swData);

                                _dbeEntities.SaveChanges();
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public string AssignFieldAccessoriesPmToSwQcHead(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId,
            long sampleNo, long userId, long swWcInchargeAssignUserId, long testPhasefrPm, long swVersionNumber,
            string versionName, string accessoriesTest)
        {
            var roleName = _dbeEntities.Database.SqlQuery<CmnUserModel>(@"select RoleName from [CellPhoneProject].[dbo].[CmnUsers]
                where CmnUserId={0} ", userId).FirstOrDefault();
            var query = (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == pMasterId select pm).FirstOrDefault();

            var fieldTestId = (from ft in _dbeEntities.SwQcHeadAssignsFromPms
                               orderby ft.FieldTestID
                                   descending
                               select ft.FieldTestID).FirstOrDefault();
            if (fieldTestId == null)
            {
                fieldTestId = Convert.ToString(1);
            }
            else
            {
                fieldTestId = Convert.ToString(Convert.ToInt32(fieldTestId) + 1);
            }

            var testIds = (from ft in _dbeEntities.SwQcTestPhases
                           where ft.TestPhaseID == testPhasefrPm
                           select ft.TestPhaseName).FirstOrDefault();

            if (testIds.Trim() == "Field (Network Test)")
            {
                SwQcHeadAssignsFromPm swQcInchargeAssign = new SwQcHeadAssignsFromPm();

                swQcInchargeAssign.ProjectMasterId = pMasterId;
                swQcInchargeAssign.ProjectName = query.ProjectName;
                swQcInchargeAssign.ProjectType = query.ProjectType;
                swQcInchargeAssign.OrderNumber = query.OrderNuber;
                swQcInchargeAssign.ProjectPmAssignId = pMAssignId;
                swQcInchargeAssign.ProjectOrderShipmentId = 1;
                swQcInchargeAssign.ProjectManagerUserId = pmUserId;
                swQcInchargeAssign.ProjectManagerSampleNo = Convert.ToInt32(sampleNo);
                swQcInchargeAssign.SoftwareVersionName = versionName;
                swQcInchargeAssign.SoftwareVersionNo = Convert.ToInt32(swVersionNumber);
                swQcInchargeAssign.PriorityFromPm = "HIGH";
                // swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
                if (roleName.RoleName == "ASPMHEAD" || roleName.RoleName == "ASPM")
                {
                    swQcInchargeAssign.AssignCatagory = roleName.RoleName;
                }
                else
                {
                    swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
                }
                swQcInchargeAssign.Status = "NEW";
                swQcInchargeAssign.PmToQcHeadAssignTime = DateTime.Now;
                swQcInchargeAssign.SwQcHeadUserId = swWcInchargeAssignUserId;
                swQcInchargeAssign.PmToQcHeadAssignComment = pmRemarks;
                swQcInchargeAssign.TestPhaseID = testPhasefrPm;
                swQcInchargeAssign.FieldTestFrom = "PM";
                swQcInchargeAssign.FieldTestID = fieldTestId;
                swQcInchargeAssign.Added = userId;
                swQcInchargeAssign.AddedDate = DateTime.Now;


                _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(swQcInchargeAssign);
                _dbeEntities.SaveChanges();
            }
            else if (testIds.Trim() == "Accessories Test")
            {
                SwQcHeadAssignsFromPm swQcInchargeAssign = new SwQcHeadAssignsFromPm();

                swQcInchargeAssign.ProjectMasterId = pMasterId;
                swQcInchargeAssign.ProjectName = query.ProjectName;
                swQcInchargeAssign.ProjectType = query.ProjectType;
                swQcInchargeAssign.OrderNumber = query.OrderNuber;
                swQcInchargeAssign.ProjectPmAssignId = pMAssignId;
                swQcInchargeAssign.ProjectOrderShipmentId = 1;
                swQcInchargeAssign.ProjectManagerUserId = pmUserId;
                swQcInchargeAssign.ProjectManagerSampleNo = Convert.ToInt32(sampleNo);
                swQcInchargeAssign.PriorityFromPm = "HIGH";
                //swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
                if (roleName.RoleName == "ASPMHEAD" || roleName.RoleName == "ASPM")
                {
                    swQcInchargeAssign.AssignCatagory = roleName.RoleName;
                }
                else
                {
                    swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
                }
                swQcInchargeAssign.Status = "NEW";
                swQcInchargeAssign.PmToQcHeadAssignTime = DateTime.Now;
                swQcInchargeAssign.SwQcHeadUserId = swWcInchargeAssignUserId;
                swQcInchargeAssign.PmToQcHeadAssignComment = pmRemarks;
                swQcInchargeAssign.TestPhaseID = testPhasefrPm;
                swQcInchargeAssign.AccessoriesTestType = accessoriesTest;
                swQcInchargeAssign.Added = userId;
                swQcInchargeAssign.AddedDate = DateTime.Now;

                _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(swQcInchargeAssign);
                _dbeEntities.SaveChanges();
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<SwQcHeadAssignsFromPmModel> GetSwQcAccessoriesAssign(long projectId)
        {
            var query =
                _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"select SwQcHeadAssignId,ProjectMasterId,ProjectName,PmToQcHeadAssignTime,PmToQcHeadAssignComment,sp.TestPhaseID,sq.TestPhaseName,[AccessoriesTestType],ProjectManagerSampleNo,[SwQcHeadToPmSubmitTime],[SwQcHeadToPmForwardComment],
                Status,ProjectManagerSampleNo
                from CellPhoneProject.dbo.SwQcHeadAssignsFromPm sp
                left join CellPhoneProject.dbo.SwQcTestPhase sq on sq.TestPhaseID=sp.TestPhaseID
                 where (AccessoriesTestType is not null) and ProjectMasterId={0} ", projectId).ToList();
            return query;
        }

        public List<SwQcHeadAssignsFromPmModel> GetSwQcFieldAssignBy(long projectId)
        {
            var query =
               _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@" select SwQcHeadAssignId,ProjectMasterId,ProjectName,PmToQcHeadAssignTime,PmToQcHeadAssignComment,sp.TestPhaseID,sq.TestPhaseName,[SwQcHeadToPmSubmitTime],[SwQcHeadToPmForwardComment],
            Status,ProjectManagerSampleNo,SoftwareVersionName,SoftwareVersionNo
            from CellPhoneProject.dbo.SwQcHeadAssignsFromPm sp
            left join CellPhoneProject.dbo.SwQcTestPhase sq on sq.TestPhaseID=sp.TestPhaseID
             where [FieldTestFrom]='PM' and ProjectMasterId={0} ", projectId).ToList();
            return query;
        }

        #endregion

        public List<tblBarCodeInv> GetLatestIMEIs(DateTime sdate, DateTime edate, string modelname = "")
        {
            List<tblBarCodeInv> data = new List<tblBarCodeInv>();
            if (modelname == "")
                data = _dbRBSYEntities.tblBarCodeInvs.Where(x => x.DateAdded >= sdate && x.DateAdded <= edate).ToList();
            else
                data = _dbRBSYEntities.tblBarCodeInvs.Where(x => x.DateAdded >= sdate && x.DateAdded <= edate && x.Model == modelname).ToList();
            return data;
        }
        public List<SelectListItem> GetModelsFromBarCodeInv(DateTime sdate, DateTime edate)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var data = _dbRBSYEntities.tblBarCodeInvs.Where(x => x.DateAdded >= sdate && x.DateAdded <= edate).Select(y => y.Model).Distinct().ToList();
            //var orderdetails = _dbeEntities.ProjectOrderQuantityDetails.Where(y=>y.BTRCPush==false).Select(x => new { x.ProjectModel, x.ProjectMasterId }).Distinct().ToList();

            var final = (from mod in data
                         //join pr in orderdetails
                         //on mod.Model equals pr.ProjectModel
                         select new SelectListItem()
                         {
                             Value = (_dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectModel == mod) != null ? _dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectModel == mod).ProjectMasterId.ToString() : ""),
                             Text = mod
                         }).ToList();

            return final;
        }

        public ProjectMasterModel GetProjectMasterInfo(long pmid)
        {
            ProjectMasterModel result = new ProjectMasterModel();
            ProjectMaster data = _dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectMasterId == pmid);
            BTRCModel btrcdata = _dbeEntities.BTRCModels.FirstOrDefault(x => x.ProjectMasterId == pmid);
            Mapper.Initialize(cfg => cfg.CreateMap<ProjectMaster, ProjectMasterModel>()
                .ForMember(d => d.Motherboard, o => o.MapFrom(s => s.CpuName != "" ? s.CpuName : ""))
                .ForMember(d => d.MarketingName, o => o.MapFrom(s => btrcdata != null ? btrcdata.MarketingName : ""))
                .ForMember(d => d.ApplicationRef, o => o.MapFrom(s => btrcdata != null ? btrcdata.ApplicationRef : ""))
                .ForMember(d => d.BatteryCapacityTested, o => o.MapFrom(s => btrcdata != null ? btrcdata.BatteryCapacityTested : ""))
                .ForMember(d => d.ChargerAdapterType, o => o.MapFrom(s => btrcdata != null ? btrcdata.ChargerAdapterType : ""))
                .ForMember(d => d.NFC, o => o.MapFrom(s => btrcdata != null ? btrcdata.NFC : "No"))
                .ForMember(d => d.BlueTooth, o => o.MapFrom(s => btrcdata != null ? btrcdata.Bluetooth : ""))
                .ForMember(d => d.WLAN, o => o.MapFrom(s => btrcdata != null ? btrcdata.WLAN : ""))
                .ForMember(d => d.DataSpeed, o => o.MapFrom(s => btrcdata != null ? btrcdata.DataSpeed : ""))
                .ForMember(d => d.SARValue, o => o.MapFrom(s => btrcdata != null ? btrcdata.SARValue : ""))
                .ForMember(d => d.CameraResulution, o => o.MapFrom(s => btrcdata != null ? btrcdata.CameraResulution : ""))
                .ForMember(d => d.RadioInterface, o => o.MapFrom(s => btrcdata != null ? btrcdata.RadioInterface : ""))
                .ForMember(d => d.FourthGen, o => o.MapFrom(s => btrcdata != null ? btrcdata.FourthGen : ""))
                .ForMember(d => d.MarketingPeriod, o => o.MapFrom(s => btrcdata != null ? btrcdata.MarketingPeriod : ""))
                .ForMember(d => d.SerialNo, o => o.MapFrom(s => btrcdata != null ? btrcdata.SerialNo : ""))
                .ForMember(d => d.ModelID, o => o.MapFrom(s => btrcdata != null ? btrcdata.ModelID : 0))
                .ForMember(d => d.ShipmentMode, o => o.MapFrom(s => btrcdata != null ? btrcdata.ShipmentMode : ""))
                .ForMember(d => d.PriceBDT, o => o.MapFrom(s => s.FinalPrice > 0 ? CommonConversion.CurrencyConversion((decimal)s.FinalPrice, "USD", "BDT") : 0))
                .ForMember(d => d.MotherboardModel, o => o.MapFrom(s => s.ChipsetName != "" ? s.ChipsetName : "")));
            result = Mapper.Map<ProjectMasterModel>(data);
            return result;
        }
        public ProjectMaster GetProjectMasterInfo(string modelname)
        {
            ProjectMaster data = _dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectModel == modelname);
            return data;
        }
        public ClientSideResponse SaveBTRCModelInformation(BTRCRegistrationVM vmdata)
        {
            ClientSideResponse response = new ClientSideResponse();
            String userIdentity = HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            ProjectMasterModel prmodel = vmdata.ProjectMaster;
            var modelname = _dbeEntities.ProjectMasters.FirstOrDefault(x => x.ProjectMasterId == vmdata.ProjectMasterId).ProjectModel;

            #region ProjectMaster Update
            Mapper.Initialize(cfg => cfg.CreateMap<ProjectMasterModel, ProjectMaster>()
                .ForMember(d => d.CpuName, o => o.MapFrom(s => s.Motherboard != "" ? s.Motherboard : ""))
                .ForMember(d => d.ChipsetName, o => o.MapFrom(s => s.MotherboardModel != "" ? s.MotherboardModel : ""))
                .ForMember(d => d.ProjectAccessories, o => o.Ignore()));
            ProjectMaster prmasterentity = Mapper.Map<ProjectMaster>(prmodel);
            _dbeEntities.ProjectMasters.AddOrUpdate(prmasterentity);

            #endregion


            BTRCModel entity = new BTRCModel()
                {
                    ModelID = prmodel.ModelID,
                    Brand = "WALTON",
                    ProjectMasterId = prmodel.ProjectMasterId,
                    ProjectName = prmodel.ProjectName,
                    ProjectModel = prmodel.ProjectModel,
                    MarketingName = prmodel.MarketingName,
                    Color = prmodel.Color,
                    DeviceType = "Mobile Handset",
                    ApplicationRef = prmodel.ApplicationRef,
                    ContryOfOrigin = "China",
                    SupplierId = prmodel.SupplierId,
                    Manufacturer = prmodel.SupplierName,
                    SimSlotNumber = prmodel.SimSlotNumber,
                    BatteryRating = prmodel.BatteryRating,
                    BatteryCapacityTested = prmodel.BatteryCapacityTested,
                    ChargerAdapterType = prmodel.ChargerAdapterType,
                    ChargerRating = prmodel.ChargerRating,
                    ProcessorName = prmodel.ProcessorName,
                    Ram = prmodel.Ram,
                    Rom = prmodel.Rom,
                    NFC = prmodel.NFC,
                    Bluetooth = prmodel.BlueTooth,
                    WLAN = prmodel.WLAN,
                    DataSpeed = prmodel.DataSpeed,
                    SARValue = prmodel.SARValue,
                    FrontCamera = prmodel.FrontCamera,
                    BackCamera = prmodel.BackCamera,
                    CameraResulution = prmodel.CameraResulution,
                    RadioInterface = prmodel.RadioInterface,
                    SecondGen = prmodel.SecondGen,
                    ThirdGen = prmodel.ThirdGen,
                    FourthGen = prmodel.FourthGen,
                    Motherboard = prmodel.Chipset,
                    OSName = prmodel.OsName,
                    ShipmentMode = prmodel.ShipmentMode,
                    SourcingType = prmodel.SourcingType,
                    UnitPrice = prmodel.FinalPrice,
                    PriceBDT = prmodel.PriceBDT,
                    MarketingPeriod = prmodel.MarketingPeriod,
                    SerialNo = prmodel.SerialNo,
                    Added = userId,
                    AddedDate = DateTime.Now
                };
            _dbeEntities.BTRCModels.AddOrUpdate(entity);
            try
            {
                _dbeEntities.SaveChanges();
                response.Success = true;
            }

            catch (Exception ex)
            {
                var msg = ex.Message;
                response.Message = msg;
                response.Success = false;
            }
            return response;
        }

        public ClientSideResponse SaveBTRCData(DateTime sdate, DateTime edate, List<SelectListItem> models)
        {
            ClientSideResponse response = new ClientSideResponse();
            String userIdentity = HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            if (models.Count > 0)
            {
                foreach (var item in models)
                {

                    var modelname = item.Text;
                    List<tblBarCodeInv> imeidata = GetLatestIMEIs(sdate, edate, modelname);
                    foreach (var imei in imeidata)
                    {
                        BTRCIMEIRegistration entity = new BTRCIMEIRegistration()
                        {
                            BTRCModelId = Int64.Parse(item.Value),
                            ProjectModel = modelname,
                            IMEITac1 = imei.BarCode.Substring(0, 8),
                            IMEITac2 = imei.BarCode2.Substring(0, 8),
                            //IMEITac3=prmodel.i
                            //IMEITac4=prmodel.i
                            IMEI1 = imei.BarCode,
                            IMEI2 = imei.BarCode2,
                            //IMEI2=item.FinalPrice
                            //IMEI3=item.FinalPrice
                            //IMEI4=item.FinalPrice
                            Exported = false,
                            Added = userId,
                            AddedDate = DateTime.Now
                        };

                        _dbeEntities.BTRCIMEIRegistrations.Add(entity);
                        try
                        {
                            _dbeEntities.SaveChanges();
                        }

                        catch (Exception ex)
                        {
                            var msg = ex.Message;
                            continue;
                        }

                    }
                }
            }
            else
            {
                List<tblBarCodeInv> imeidata = GetLatestIMEIs(sdate, edate);
                foreach (var imei in imeidata)
                {
                    BTRCModel prmodel = _dbeEntities.BTRCModels.FirstOrDefault(x => x.ProjectModel == imei.Model);
                    BTRCIMEIRegistration entity = new BTRCIMEIRegistration()
                    {
                        BTRCModelId = prmodel.ModelID,
                        ProjectModel = prmodel.ProjectModel,
                        IMEITac1 = imei.BarCode.Substring(0, 8),
                        IMEITac2 = imei.BarCode2.Substring(0, 8),
                        //IMEITac3=prmodel.i
                        //IMEITac4=prmodel.i
                        IMEI1 = imei.BarCode,
                        IMEI2 = imei.BarCode2,
                        //IMEI2=item.FinalPrice
                        //IMEI3=item.FinalPrice
                        //IMEI4=item.FinalPrice
                        Exported = false,
                        Added = userId,
                        AddedDate = DateTime.Now
                    };

                    _dbeEntities.BTRCIMEIRegistrations.Add(entity);
                    try
                    {
                        _dbeEntities.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        continue;
                    }

                }
            }


            response.Success = true;
            return response;
        }
        public List<SelectListItem> GetModelsFromPMS()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var exclusivemodels = _dbeEntities.BTRCModels.Select(y => y.ProjectModel).Distinct().ToList();
            var data = _dbeEntities.ProjectOrderQuantityDetails.Where(x => x.ProjectModel != null && x.IsActive == true).Select(y => new { text = y.ProjectModel, value = y.ProjectMasterId }).Distinct().ToList();

            var final = (from mod in data
                         select new SelectListItem()
                         {
                             Value = mod.value.ToString(),
                             Text = mod.text.ToString()
                         }).ToList();
            return final;
        }

        public List<BTRCModel> GetBTRCModels()
        {
            List<BTRCModel> result = new List<BTRCModel>();
            result = _dbeEntities.BTRCModels.ToList();
            return result;
        }
        public BTRCModel GetBTRCModel(string projectmodel)
        {
            BTRCModel result = new BTRCModel();
            result = _dbeEntities.BTRCModels.FirstOrDefault(x => x.ProjectModel == projectmodel);
            return result;
        }

        public List<BTRCIMEIExportLog> GetBTRCExportLog()
        {
            List<BTRCIMEIExportLog> result = new List<BTRCIMEIExportLog>();
            result = _dbeEntities.BTRCIMEIExportLogs.ToList();
            return result;
        }

        public List<SwQcTestPhaseModel> GetSwQcTestPhaseForSupp()
        {
            var query = _dbeEntities.Database.SqlQuery<SwQcTestPhaseModel>(@"select * from [CellPhoneProject].[dbo].[SwQcTestPhase] where TestPhaseIsActive=1 and TestPhaseName not in ('Field (Network Test)')  order by TestPhaseID asc").ToList();
            return query;
        }

        public List<SwQcTestPhaseModel> GetSwQcTestPhaseForSuppDemo()
        {
            var query = _dbeEntities.Database.SqlQuery<SwQcTestPhaseModel>(@"select * from [CellPhoneProject].[dbo].[SwQcTestPhase] where TestPhaseIsActive=1 and TestPhaseName in ('Demo Test')  order by TestPhaseID asc").ToList();
            return query;
        }

        public List<ProjectMasterModel> GetAllProjects()
        {
            var allProjects =
               _dbeEntities.ProjectMasters.Where(
                   i =>
                       i.IsActive && i.ProjectStatus == "APPROVED" &&
                       (i.ProjectNameForScreening != i.ProjectName && i.ProjectName != null))
                   .Select(i => new ProjectMasterModel
                   {
                       ProjectMasterId = i.ProjectMasterId,
                       ProjectName = i.ProjectName,
                       SupplierName = i.SupplierName,
                       SupplierModelName = i.SupplierModelName,
                       ProjectTypeId = i.ProjectTypeId,
                       NumberOfSample = i.NumberOfSample,
                       ApproxProjectFinishDate = i.ApproxProjectFinishDate,
                       SupplierTrustLevel = i.SupplierTrustLevel,
                       IsScreenTestComplete = i.IsScreenTestComplete,
                       IsApproved = i.IsApproved,
                       ApproxProjectOrderDate = i.ApproxProjectOrderDate,
                       ApproxShipmentDate = i.ApproxShipmentDate,
                       OrderNuber = i.OrderNuber,
                       ApproximatePrice = i.ApproximatePrice,
                       FinalPrice = i.FinalPrice


                   }).OrderBy(i => i.ProjectName).ThenBy(i => i.ProjectMasterId).ToList();


            foreach (var project in allProjects)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " -->> (" + project.OrderNumberOrdinal + ")";
                }

            }
            return allProjects;
        }

        public ProjectMasterModel GetProjectDetails(long projectMasterId)
        {
            var projectMasters =
           _dbeEntities.Database.SqlQuery<ProjectMasterModel>(
               @"select distinct pm.ProjectMasterId,pm.ProjectModel as ProjectName,pm.ProjectType,pm.SourcingType,pm.OrderNuber,ppf.PoCategory,ppf.ProjectPurchaseOrderFormId,ppf.Quantity as PoQuantity from CellPhoneProject.dbo.ProjectMasters pm 
                left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf on pm.ProjectMasterId=ppf.ProjectMasterId
                where pm.ProjectMasterId={0} ", projectMasterId).FirstOrDefault();

            return projectMasters;
        }

        public List<ProjectMasterModel> AllBOMType()
        {
            var projectMasters =
        _dbMrpEntities.Database.SqlQuery<ProjectMasterModel>(
            @"select distinct BOMtype from  [WALTON.MRP].[dbo].[BOMs]").ToList();

            return projectMasters;
        }

        public List<ProjectMasterModel> GetBomName(long proIds, string bomsTypes, string projectNames)
        {
            var projectMasters =
    _dbMrpEntities.Database.SqlQuery<ProjectMasterModel>(
        @"select distinct bom.SpareDescription as BOMName,bom.BOMType,bpm.Model from [WALTON.MRP].[dbo].[BOMs] bom
	left join [WALTON.MRP].[dbo].[BomProductModel] bpm on bom.BomProductModelId=bpm.Id
	where bpm.Model={2} and bom.BOMType={1} ", proIds, bomsTypes, projectNames).ToList();

            return projectMasters;
        }

        public string SaveRawMaterialInspection(List<ProjectMasterModel> issueList, List<ProjectMasterModel> issueList1, long proId, string focChk1, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                long cc = 0;
                if (proId != 0)
                {
                    //foreach (var ii in issueList)
                    //{
                    //    var modelss = new RawMaterialInspection();
                    //    modelss.ProjectMasterId = ii.ProjectMasterId;
                    //    modelss.ProjectPurchaseOrderFormId = ii.ProjectPurchaseOrderFormId;
                    //    modelss.ProjectName = ii.ProjectName;
                    //    modelss.ProjectType = ii.ProjectType;
                    //    modelss.Orders = ii.OrderNuber;
                    //    modelss.PoCategory = ii.PoCategory;
                    //    modelss.PoQuantity = ii.PoQuantity;
                    //    modelss.LotNumber = ii.LotNumber;
                    //    modelss.LotQuantity = ii.LotQuantity;
                    //    modelss.ProjectManagerClearanceDate = ii.ProjectManagerClearanceDate;
                    //    modelss.ChinaIqcPassHundredPercent = ii.ChinaIqcPassHundredPercent;
                    //    modelss.NoOfTimeInspection = ii.NoOfTimeInspection;
                    //    modelss.ManagementApproval = ii.ManagementApproval;
                    //    modelss.ManagementApprovalDate = ii.ManagementApprovalDate;
                    //    modelss.SupportingDocument = attachment;
                    //    modelss.SourcingApproval = ii.SourcingApproval;
                    //    modelss.InspectionRemarks = ii.InspectionRemarks;
                    //    modelss.Added = userId;
                    //    modelss.AddedDate = DateTime.Now;

                    //    _dbeEntities.RawMaterialInspections.Add(modelss);
                    //    _dbeEntities.SaveChanges();

                    //    // modelss.RawMaterialId = ii.RawMaterialId;

                    //    cc = modelss.RawMaterialId;
                    //}

                    var modelss = new RawMaterialInspection();
                    modelss.ProjectMasterId = issueList[0].ProjectMasterId;
                    modelss.ProjectPurchaseOrderFormId = issueList[0].ProjectPurchaseOrderFormId;
                    modelss.ProjectName = issueList[0].ProjectName;
                    modelss.ProjectType = issueList[0].ProjectType;
                    modelss.Orders = issueList[0].OrderNuber;
                    modelss.PoCategory = issueList[0].PoCategory;
                    modelss.PoQuantity = issueList[0].PoQuantity;
                    modelss.LotNumber = issueList[0].LotNumber;
                    modelss.LotQuantity = issueList[0].LotQuantity;
                    modelss.ProjectManagerClearanceDate = issueList[0].ProjectManagerClearanceDate;

                    modelss.InspectionStartingDate = issueList[0].InspectionStartingDate;
                    modelss.MajorDelayReason = issueList[0].MajorDelayReason;
                    modelss.HardwareSampleReceive = issueList[0].HardwareSampleReceive;
                    modelss.InspectionMajorFailItems = issueList[0].InspectionMajorFailItems;
                    modelss.OrderColorRatioWithQty = issueList[0].OrderColorRatioWithQty;

                    modelss.ChinaIqcPassHundredPercent = issueList[0].ChinaIqcPassHundredPercent;
                    modelss.NoOfTimeInspection = issueList[0].NoOfTimeInspection;
                    modelss.ManagementApproval = issueList[0].ManagementApproval;
                    modelss.ManagementApprovalDate = issueList[0].ManagementApprovalDate;
                    modelss.SupportingDocument = attachment;
                    modelss.SourcingApproval = issueList[0].SourcingApproval;
                    modelss.InspectionRemarks = issueList[0].InspectionRemarks;
                    modelss.Added = userId;
                    modelss.AddedDate = DateTime.Now;

                    _dbeEntities.RawMaterialInspections.Add(modelss);
                    _dbeEntities.SaveChanges();

                    cc = modelss.RawMaterialId;

                    var kk = cc;
                    var qq =
                        (from ss in _dbeEntities.RawMaterialInspections where ss.RawMaterialId == kk select ss)
                            .FirstOrDefault();
                    if (kk != 0 && focChk1 == "true")
                    // if (focChk1 == "true")
                    {
                        foreach (var mod in issueList1)
                        {
                            // mod.RawMaterialId = qq.RawMaterialId;

                            var models = new FocClaimBomDetail();
                            models.RawMaterialId = qq.RawMaterialId;
                            models.ProjectMasterId = qq.ProjectMasterId;
                            models.ProjectPurchaseOrderFormId = qq.ProjectPurchaseOrderFormId;
                            models.ProjectName = qq.ProjectName;
                            models.ProjectType = qq.ProjectType;
                            models.Orders = qq.Orders;
                            models.PoCategory = qq.PoCategory;
                            models.PoQuantity = qq.PoQuantity;
                            models.LotNumber = qq.LotNumber;
                            models.LotQuantity = qq.LotQuantity;

                            models.BOMType = mod.BOMType;
                            models.BOMName = mod.BOMName;
                            models.Color = mod.Color;
                            models.ItemQuantity = mod.ItemQuantity;
                            models.BomRemarks = mod.BomRemarks;

                            models.Added = userId;
                            models.AddedDate = DateTime.Now;

                            _dbeEntities.FocClaimBomDetails.Add(models);
                            _dbeEntities.SaveChanges();
                        }
                    }

                }

                _dbeEntities.SaveChanges();
            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }

            return returnValue;

            //  return "OK";
        }

        public List<AssignProjectsViewModel> GetRawMaterialInspectionList()
        {
            var projectMasters = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select top 500 * from  [CellPhoneProject].[dbo].[RawMaterialInspection] order by RawMaterialId desc").ToList();

            return projectMasters;
        }

        public List<AssignProjectsViewModel> GetBomDetails(long rawMaterialId)
        {
            var projectMasters = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select * from  [CellPhoneProject].[dbo].[FocClaimBomDetails] where RawMaterialId={0}", rawMaterialId).ToList();

            return projectMasters;
        }

        public List<AssignProjectsViewModel> GetQcDelayReport(string projectName, string projectType, string startDate, string endDate, string EmployeeCode)
        {
            List<AssignProjectsViewModel> query = null;
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            if (projectName == "0" && projectType == "0" && startDate != "" && endDate != "" && EmployeeCode == "0")
            {
                query = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select distinct pm.ProjectName,pm.ProjectType,spp.PmToQcHeadAssignTime,spp.PmToQcHeadAssignComment,spp.ProjectManagerSampleType,spp.ProjectManagerSampleNo,spp.SoftwareVersionName,spp.SoftwareVersionNo,
                (select count(*) from CellPhoneProject.dbo.SwQcIssueDetails sdd where sdd.SoftwareVersionNo=spp.SoftwareVersionNo and sdd.WaltonQcStatus='NEW ISSUE' and sdd.ProjectName=spp.ProjectName) as NewIssue,
                spp.SwQcFinishedTime,spp.Status, case when DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) is null then DATEDIFF(day, spp.PmToQcHeadAssignTime, GETDATE())  else 
                DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) end as FeedbackDuration 

                from CellPhoneProject.dbo.ProjectMasters pm left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] spp on spp.ProjectName=pm.ProjectName
                where spp.TestPhaseId not in (7,10) and spp.PmToQcHeadAssignTime between {2} and {3} 
                order by pm.ProjectName, spp.SoftwareVersionNo asc", projectName, projectType, startDate, endDate).ToList();
            }

            else if (projectName != "0" && projectType == "0" && startDate != "" && endDate != "" && EmployeeCode == "0")
            {
                query = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select distinct pm.ProjectName,pm.ProjectType,spp.PmToQcHeadAssignTime,spp.PmToQcHeadAssignComment,spp.ProjectManagerSampleType,spp.ProjectManagerSampleNo,spp.SoftwareVersionName,spp.SoftwareVersionNo,
                (select count(*) from CellPhoneProject.dbo.SwQcIssueDetails sdd where sdd.SoftwareVersionNo=spp.SoftwareVersionNo and sdd.WaltonQcStatus='NEW ISSUE' and sdd.ProjectName=spp.ProjectName) as NewIssue,
                spp.SwQcFinishedTime,spp.Status, case when DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) is null then DATEDIFF(day, spp.PmToQcHeadAssignTime, GETDATE())  else 
                DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) end as FeedbackDuration 

                from CellPhoneProject.dbo.ProjectMasters pm left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] spp on spp.ProjectName=pm.ProjectName
                where spp.ProjectName={0} and spp.TestPhaseId not in (7,10) and spp.PmToQcHeadAssignTime between {2} and {3} 
                order by pm.ProjectName, spp.SoftwareVersionNo asc", projectName, projectType, startDate, endDate).ToList();
            }
            else if (projectName == "0" && projectType != "0" && startDate != "" && endDate != "" && EmployeeCode == "0")
            {
                query = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select distinct pm.ProjectName,pm.ProjectType,spp.PmToQcHeadAssignTime,spp.PmToQcHeadAssignComment,spp.ProjectManagerSampleType,spp.ProjectManagerSampleNo,spp.SoftwareVersionName,spp.SoftwareVersionNo,
                (select count(*) from CellPhoneProject.dbo.SwQcIssueDetails sdd where sdd.SoftwareVersionNo=spp.SoftwareVersionNo and sdd.WaltonQcStatus='NEW ISSUE' and sdd.ProjectName=spp.ProjectName) as NewIssue,
                spp.SwQcFinishedTime,spp.Status, case when DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) is null then DATEDIFF(day, spp.PmToQcHeadAssignTime, GETDATE())  else 
                DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) end as FeedbackDuration 

                from CellPhoneProject.dbo.ProjectMasters pm left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] spp on spp.ProjectName=pm.ProjectName
                where pm.ProjectType={1} and spp.TestPhaseId not in (7,10) and spp.PmToQcHeadAssignTime between {2} and {3} 
                order by pm.ProjectName, spp.SoftwareVersionNo asc", projectName, projectType, startDate, endDate).ToList();
            }
            else if (projectName == "0" && projectType == "0" && startDate != "" && endDate != "" && EmployeeCode != "0")
            {
                query = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select distinct pm.ProjectName,cm.UserFullName,cm.EmployeeCode,pm.ProjectType,spp.PmToQcHeadAssignTime,spp.PmToQcHeadAssignComment,spp.ProjectManagerSampleType,spp.ProjectManagerSampleNo,spp.SoftwareVersionName,spp.SoftwareVersionNo,
                (select count(*) from CellPhoneProject.dbo.SwQcIssueDetails sdd where sdd.SoftwareVersionNo=spp.SoftwareVersionNo and sdd.WaltonQcStatus='NEW ISSUE' and sdd.ProjectName=spp.ProjectName) as NewIssue,
                spp.SwQcFinishedTime,spp.Status, case when DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) is null then DATEDIFF(day, spp.PmToQcHeadAssignTime, GETDATE())  else 
                DATEDIFF(day, spp.PmToQcHeadAssignTime, spp.SwQcFinishedTime) end as FeedbackDuration 

                from CellPhoneProject.dbo.ProjectMasters pm left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] spp on spp.ProjectName=pm.ProjectName
                left join CellPhoneProject.dbo.CmnUsers cm on spp.ProjectManagerUserId=cm.CmnUserId
                where cm.EmployeeCode={4} and spp.TestPhaseId not in (7,10) and spp.PmToQcHeadAssignTime between {2} and {3} 
                order by pm.ProjectName, spp.SoftwareVersionNo asc", projectName, projectType, startDate, endDate, EmployeeCode).ToList();
            }
            return query;
        }

        public CmnUserModel GetRoleName(long userId)
        {
            var roleName = _dbeEntities.Database.SqlQuery<CmnUserModel>(@"select RoleName from [CellPhoneProject].[dbo].[CmnUsers]
                where CmnUserId={0} ", userId).FirstOrDefault();

            return roleName;
        }

        public List<ProjectMasterModel> GetProjectMasterModelsByAspm()
        {
            var proList = _dbeEntities.Database.SqlQuery<ProjectMasterModel>(@"select distinct A.ProjectName,(select top 1 ProjectMasterId  from CellPhoneProject.dbo.ProjectMasters where ProjectName=A.ProjectName and IsActive=1  order by ProjectMasterId desc) as ProjectMasterId
            from (select distinct ProjectName,ProjectMasterId from CellPhoneProject.dbo.ProjectMasters pm where ProjectMasterId is not null and IsActive=1)A
            order by ProjectName asc").ToList();

            return proList;
        }

        public AssignProjectsViewModel GetRawDetails(long proId)
        {
            var proList = _dbeEntities.Database.SqlQuery<AssignProjectsViewModel>(@"select * from  [CellPhoneProject].[dbo].[RawMaterialInspection] where
            RawMaterialId={0} ", proId).FirstOrDefault();

            return proList;
        }

        public string UpdateRawMaterialInspection(long proId, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "OK";
            try
            {
                if (proId != 0 && attachment != "")
                {
                    var query1 =
                        (from ss in _dbeEntities.RawMaterialInspections where ss.RawMaterialId == proId select ss)
                            .FirstOrDefault();

                    if (query1 != null)
                    {
                        query1.SupportingDocument = attachment;
                        query1.Updated = userId;
                        query1.UpdatedDate = DateTime.Now;

                        _dbeEntities.RawMaterialInspections.AddOrUpdate(query1);
                        _dbeEntities.SaveChanges();

                    }

                }

                _dbeEntities.SaveChanges();
            }
            catch (Exception exception)
            {

                returnValue = exception.Message;
            }
            return returnValue;
        }

        public string SaveNewFoc(long rawMatIds, string bomsTypes, string bomName, string bomQuantity, string color, string bomRemarks)
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            string returnValue = "OK";
            try
            {
                if (rawMatIds != 0)
                {
                    var qq =
                       (from ss in _dbeEntities.RawMaterialInspections where ss.RawMaterialId == rawMatIds select ss)
                           .FirstOrDefault();

                    if (qq != null)
                    {
                        var models = new FocClaimBomDetail();
                        models.RawMaterialId = qq.RawMaterialId;
                        models.ProjectMasterId = qq.ProjectMasterId;
                        models.ProjectPurchaseOrderFormId = qq.ProjectPurchaseOrderFormId;
                        models.ProjectName = qq.ProjectName;
                        models.ProjectType = qq.ProjectType;
                        models.Orders = qq.Orders;
                        models.PoCategory = qq.PoCategory;
                        models.PoQuantity = qq.PoQuantity;
                        models.LotNumber = qq.LotNumber;
                        models.LotQuantity = qq.LotQuantity;
                        models.BOMType = bomsTypes;
                        models.BOMName = bomName;
                        models.Color = color;
                        models.ItemQuantity = bomQuantity;
                        models.BomRemarks = bomRemarks;
                        models.Added = users;
                        models.AddedDate = DateTime.Now;

                        _dbeEntities.FocClaimBomDetails.Add(models);
                        _dbeEntities.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                returnValue = e.Message;
            }
            return returnValue;
        }
        #region Finish Good
        public List<FinishGoodVariantModel> GetShipmentDetailsForFinishGood()
        {
            var projectShipmentList = _dbeEntities.Database.SqlQuery<FinishGoodVariantModel>(@"
            select FinishGoodCheck,ProjectOrderShipmentId,ProjectPurchaseOrderFormId,ProjectMasterId,Added,AddedByName,AddedDate,PoDate1,PoNo,ProjectName,
            WarehouseEntryDate,PoWiseShipmentNumber,PoCount
            from	
            (	
	            select 	case when ps.ProjectOrderShipmentId in (select sm.ProjectOrderShipmentId 
	            FROM [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sm where sm.ProjectOrderShipmentId=ps.ProjectOrderShipmentId) then 'YES' end as FinishGoodCheck,

	            ps.ProjectOrderShipmentId,ps.ProjectPurchaseOrderFormId,pm.ProjectMasterId,ps.Added,cu.UserFullName as AddedByName,ps.AddedDate,ps.AirportReleaseDate,ps.AriportArrivalDate,ps.BankNocDate,ps.ChainaInspectionDate,ps.CnfDate,ps.CnfPayOrderDate,
	            ps.CostingDate,ps.FlightDepartureDate,ps.ForwarderDate,ps.MarketReleaseDate,po.PoDate as PoDate1,po.PurchaseOrderNumber as PoNo,pm.ProjectName,ps.ShipmentApproxDate,ps.ShipmentFinalDate,ps.Updated,ps.UpdatedDate,
	            ps.WarehouseEntryDate,ps.PoWiseShipmentNumber,pm.OrderNuber as PoCount

	            from 

	            CellPhoneProject.dbo.ProjectOrderShipments ps
	            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
	            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectPurchaseOrderFormId=ps.ProjectPurchaseOrderFormId
	            left join CellPhoneProject.dbo.CmnUsers cu on cu.Added=ps.Added

	            where ps.IsComplete = 'false' and pm.IsActive='true' 
            )A where FinishGoodCheck is not null
            order by ProjectOrderShipmentId desc").ToList();
            foreach (var model in projectShipmentList)
            {
                model.PoOrdinal = CommonConversion.AddOrdinal(model.PoCount) + " Purchase Order";
                model.ShipmentNoOrdinal = CommonConversion.AddOrdinal(model.PoWiseShipmentNumber) + " Shipment";
            }
            return projectShipmentList;
        }

        public List<FinishGoodVariantModel> GetFinishGoodDetails(long proShipOrder)
        {
            var nocList = _dbeEntities.Database.SqlQuery<FinishGoodVariantModel>(@"select sf.ProjectOrderShipmentId,sf.FinishGoodProjectMasterId,sf.FinishGoodModel,sf.FinishGoodModelOrderNumber,sf.ApproxFinishGoodManufactureQty,pod.OrderQuantity
            from [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sf
            left join  [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pod on pod.ProjectMasterId=sf.FinishGoodProjectMasterId
            where ProjectOrderShipmentId={0}", proShipOrder).ToList();
            return nocList;
        }

        public List<PmQcAssignModel> GetPmToQcHeadAssignModels(long userId)
        {

            //            String query = String.Format(@"select distinct
            //            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.Added) AssignUserName,
            //            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status not in ('INACTIVE')) ProjectManagerUserName,
            //            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.SwQcHeadUserId) QcInchargeUserName,
            //            case when ppa.ProjectPmAssignId is null then 0 else ppa.ProjectPmAssignId end as ProjectPmAssignId,
            //            case when ppa.ProjectManagerUserId is null then 0 else ppa.ProjectManagerUserId end as ProjectManagerUserId,
            //
            //            pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,
            //            pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.Battery,pm.SourcingType,pm.ProjectType, 
            //            case when sm.ProjectManagerSampleNo is null then 0 else sm.ProjectManagerSampleNo end as ProjectManagerSampleNo,
            //            sm.SwQcHeadAssignId,sm.AccessoriesTestType,tp.TestPhaseName,sm.SoftwareVersionName,
            //            sm.SoftwareVersionNo,sm.PmToQcHeadAssignComment,sm.PmToQcHeadAssignTime,sm.SwQcHeadUserId
            //
            //            from CellPhoneProject.[dbo].ProjectMasters pm 
            //            left join CellPhoneProject.[dbo].ProjectPmAssigns ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status not in ('INACTIVE')
            //            left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] sm on sm.ProjectMasterId=pm.ProjectMasterId
            //            left join CellPhoneProject.[dbo].SwQcTestPhase tp on tp.TestPhaseID=sm.TestPhaseID 
            //            left join CellPhoneProject.[dbo].CmnUsers cuu on cuu.CmnUserId=ppa.ProjectManagerUserId
            //            where sm.Status='NEW' and cuu.CmnUserId={0} order by sm.PmToQcHeadAssignTime desc",userId);

            String query = String.Format(@"select distinct
            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.Added) AssignUserName,
            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status not in ('INACTIVE')) ProjectManagerUserName,
            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.SwQcHeadUserId) QcInchargeUserName,
            case when ppa.ProjectPmAssignId is null then 0 else ppa.ProjectPmAssignId end as ProjectPmAssignId,
            case when ppa.ProjectManagerUserId is null then 0 else ppa.ProjectManagerUserId end as ProjectManagerUserId,

            pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,
            pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.Battery,pm.SourcingType,pm.ProjectType, 
            case when sm.ProjectManagerSampleNo is null then 0 else sm.ProjectManagerSampleNo end as ProjectManagerSampleNo,
            sm.SwQcHeadAssignId,sm.AccessoriesTestType,tp.TestPhaseName,sm.SoftwareVersionName,
            sm.SoftwareVersionNo,sm.PmToQcHeadAssignComment,sm.PmToQcHeadAssignTime,sm.SwQcHeadUserId

            from CellPhoneProject.[dbo].ProjectMasters pm 
            left join CellPhoneProject.[dbo].ProjectPmAssigns ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status not in ('INACTIVE')
            left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] sm on sm.ProjectMasterId=pm.ProjectMasterId
            left join CellPhoneProject.[dbo].SwQcTestPhase tp on tp.TestPhaseID=sm.TestPhaseID 
            left join CellPhoneProject.[dbo].CmnUsers cuu on cuu.CmnUserId=ppa.ProjectManagerUserId
            where sm.Status='NEW' and sm.Added={0} order by sm.PmToQcHeadAssignTime desc", userId);

            List<PmQcAssignModel> models = GenereticRepo<PmQcAssignModel>.GetList(_dbeEntities, query);

            foreach (var project in models)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }
            }
            return models;
        }

        public List<CmnUserModel> GetActiveQc()
        {
            List<CmnUser> list = _dbeEntities.CmnUsers.Where(x => (x.RoleName == "QC" || x.RoleName == "QCHEAD") && x.IsActive).ToList();

            List<CmnUserModel> models = GenericMapper<CmnUser, CmnUserModel>.GetDestinationList(list);
            VmSwQcSpecificationModified vmSwQc = new VmSwQcSpecificationModified();
            vmSwQc.CmnUserModels = models;
            return models;
        }

        public string UpdateInactiveAssignedProjectToQc(long proId, long swQcHeadIds)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var query1 = (from swQcHead in _dbeEntities.SwQcHeadAssignsFromPms
                          where swQcHead.SwQcHeadAssignId == swQcHeadIds && swQcHead.ProjectMasterId == proId
                          select swQcHead).FirstOrDefault();

            query1.Status = "INACTIVE";
            query1.Updated = userId;
            query1.UpdatedDate = DateTime.Now;

            _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(query1);
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public string UpdateQcheadToQcAssignedProjectForInactive(long proId, long swQcHeadIds)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var query1 = (from swQcHead in _dbeEntities.SwQcHeadAssignsFromPms
                          where swQcHead.SwQcHeadAssignId == swQcHeadIds && swQcHead.ProjectMasterId == proId
                          select swQcHead).FirstOrDefault();

            query1.Status = "INACTIVE";
            query1.Updated = userId;
            query1.UpdatedDate = DateTime.Now;

            _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(query1);
            _dbeEntities.SaveChanges();

            var query2 = (from swQcHead in _dbeEntities.SwQcAssignsFromQcHeads
                          where swQcHead.SwQcHeadAssignId == swQcHeadIds && swQcHead.ProjectMasterId == proId
                          select swQcHead).ToList();

            foreach (var qq2 in query2)
            {
                qq2.Status = "INACTIVE";
                qq2.Updated = userId;
                qq2.UpdatedDate = DateTime.Now;

                _dbeEntities.SwQcAssignsFromQcHeads.AddOrUpdate(qq2);
                _dbeEntities.SaveChanges();
            }
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public string AssignOsRequirementToSwQcHead(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, long userId, long swWcInchargeAssignUserId)
        {
            var roleName = _dbeEntities.Database.SqlQuery<CmnUserModel>(@"select RoleName from [CellPhoneProject].[dbo].[CmnUsers]
                where CmnUserId={0} ", userId).FirstOrDefault();
            var query = (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == pMasterId select pm).FirstOrDefault();

            SwQcHeadAssignsFromPm swQcInchargeAssign = new SwQcHeadAssignsFromPm();
            swQcInchargeAssign.ProjectMasterId = pMasterId;
            swQcInchargeAssign.ProjectName = query.ProjectName;
            swQcInchargeAssign.ProjectType = query.ProjectType;
            swQcInchargeAssign.OrderNumber = query.OrderNuber;
            swQcInchargeAssign.ProjectPmAssignId = pMAssignId;
            swQcInchargeAssign.ProjectOrderShipmentId = 1;
            swQcInchargeAssign.ProjectManagerUserId = userId;

            swQcInchargeAssign.PriorityFromPm = "HIGH";
            if (roleName.RoleName == "ASPMHEAD" || roleName.RoleName == "ASPM")
            {
                swQcInchargeAssign.AssignCatagory = roleName.RoleName;
            }
            else
            {
                swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
            }
            swQcInchargeAssign.Status = "NEWASSIGNED";
            swQcInchargeAssign.PmToQcHeadAssignTime = DateTime.Now;
            swQcInchargeAssign.SwQcHeadUserId = swWcInchargeAssignUserId;
            swQcInchargeAssign.PmToQcHeadAssignComment = pmRemarks;
            swQcInchargeAssign.IsFinalPhaseMP = false;
            swQcInchargeAssign.Added = userId;
            swQcInchargeAssign.AddedDate = DateTime.Now;

            _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(swQcInchargeAssign);
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public List<SwQcHeadAssignsFromPmModel> GetOsAssignInfoForPm(long projectId)
        {
            string query = string.Format(@"select swQc.ProjectMasterId,swQc.ProjectName,swQc.SwQcHeadAssignId,swQc.ProjectPmAssignId,swQc.PmToQcHeadAssignTime,swQc.PmToQcHeadAssignComment,
            swQc.SwQcHeadToPmSubmitTime,swQc.SwQcHeadToPmForwardComment,swQc.Status,swQc.SupportingDocument
            from CellPhoneProject.dbo.SwQcHeadAssignsFromPm swQc         
            where  swQc.ProjectMasterId='{0}' and Status in ('NEWASSIGNED','DONE','ASSIGNEDTOQC')
            order by swQc.PmToQcHeadAssignTime desc", projectId);
            var exe = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(query).ToList();
            return exe;
        }

        public List<PmQcAssignModel> GetQcHeadToQcAssignModels(long userId)
        {
            //            String query = String.Format(@"select distinct
            //            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.Added) AssignUserName,
            //            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status not in ('INACTIVE')) ProjectManagerUserName,
            //            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.SwQcHeadUserId) QcInchargeUserName,
            //            case when ppa.ProjectPmAssignId is null then 0 else ppa.ProjectPmAssignId end as ProjectPmAssignId,
            //            case when ppa.ProjectManagerUserId is null then 0 else ppa.ProjectManagerUserId end as ProjectManagerUserId,
            //
            //            pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,
            //            pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.Battery,pm.SourcingType,pm.ProjectType, 
            //            case when sm.ProjectManagerSampleNo is null then 0 else sm.ProjectManagerSampleNo end as ProjectManagerSampleNo,
            //            sm.SwQcHeadAssignId,sm.AccessoriesTestType,tp.TestPhaseName,sm.SoftwareVersionName,
            //            sm.SoftwareVersionNo,sm.PmToQcHeadAssignComment,sm.PmToQcHeadAssignTime,sm.SwQcHeadUserId, sm.SwQcHeadToQcAssignTime
            //
            //            from CellPhoneProject.[dbo].ProjectMasters pm 
            //            left join CellPhoneProject.[dbo].ProjectPmAssigns ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status not in ('INACTIVE')
            //            left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] sm on sm.ProjectMasterId=pm.ProjectMasterId
            //            left join CellPhoneProject.[dbo].SwQcTestPhase tp on tp.TestPhaseID=sm.TestPhaseID 
            //            left join CellPhoneProject.[dbo].CmnUsers cuu on cuu.CmnUserId=ppa.ProjectManagerUserId
            //            where sm.Status in ('QCCOMPLETED','ASSIGNED') and cuu.CmnUserId={0} order by sm.PmToQcHeadAssignTime desc", userId);

            String query = String.Format(@"select distinct
            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.Added) AssignUserName,
            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=ppa.ProjectManagerUserId and ppa.Status not in ('INACTIVE')) ProjectManagerUserName,
            (select UserFullName from CellPhoneProject.[dbo].CmnUsers cu where Cu.CmnUserId=sm.SwQcHeadUserId) QcInchargeUserName,
            case when ppa.ProjectPmAssignId is null then 0 else ppa.ProjectPmAssignId end as ProjectPmAssignId,
            case when ppa.ProjectManagerUserId is null then 0 else ppa.ProjectManagerUserId end as ProjectManagerUserId,

            pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber,pm.DisplayName,pm.DisplaySize,pm.ProcessorName,
            pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.BackCamera,pm.Ram,pm.Rom,pm.Battery,pm.SourcingType,pm.ProjectType, 
            case when sm.ProjectManagerSampleNo is null then 0 else sm.ProjectManagerSampleNo end as ProjectManagerSampleNo,
            sm.SwQcHeadAssignId,sm.AccessoriesTestType,tp.TestPhaseName,sm.SoftwareVersionName,
            sm.SoftwareVersionNo,sm.PmToQcHeadAssignComment,sm.PmToQcHeadAssignTime,sm.SwQcHeadUserId, sm.SwQcHeadToQcAssignTime

            from CellPhoneProject.[dbo].ProjectMasters pm 
            left join CellPhoneProject.[dbo].ProjectPmAssigns ppa on pm.ProjectMasterId=ppa.ProjectMasterId and ppa.Status not in ('INACTIVE')
            left join CellPhoneProject.[dbo].[SwQcHeadAssignsFromPm] sm on sm.ProjectMasterId=pm.ProjectMasterId
            left join CellPhoneProject.[dbo].SwQcTestPhase tp on tp.TestPhaseID=sm.TestPhaseID 
            left join CellPhoneProject.[dbo].CmnUsers cuu on cuu.CmnUserId=ppa.ProjectManagerUserId
            where sm.Status in ('QCCOMPLETED','ASSIGNED') and sm.Added={0} order by sm.PmToQcHeadAssignTime desc", userId);

            List<PmQcAssignModel> models = GenereticRepo<PmQcAssignModel>.GetList(_dbeEntities, query);

            foreach (var project in models)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }
            }
            return models;
        }
        #endregion

        #region China Qc Inspection Clearance
        public List<ChinaQcInspectionsClearanceModel> GetProjectListForChinaQc()
        {
            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"select distinct ProjectName from CellPhoneProject.dbo.ProjectMasters where ProjectStatus='APPROVED'").ToList();
            return query;
        }

        public List<ChinaQcInspectionsClearanceModel> GetProjectOrders(string projectName)
        {
            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            select ProjectMasterId,'Order '+cast(OrderNuber as varchar(50)) as Orders,ProjectName from CellPhoneProject.dbo.ProjectMasters where ProjectStatus='APPROVED'
            and ProjectName={0} AND IsActive=1
            order by ProjectName asc", projectName).ToList();
            return query;
        }

        public ChinaQcInspectionsClearanceModel GetProjectOrderQuantity(string projectMasterId)
        {
            long proIds;
            long.TryParse(projectMasterId, out proIds);

            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
             select sum(OrderQuantity) as OrderQuantity  from [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] 
             where ProjectMasterId={0} and IsActive=1", proIds).FirstOrDefault();
            return query;
        }

        public List<ChinaQcInspectionsClearanceModel> GetChinaInspectionDetails(string projectMasterId)
        {
            long proIds;
            long.TryParse(projectMasterId, out proIds);

            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            select top 1 pm.ProjectMasterId,pm.ProjectName,'Order '+cast(pm.OrderNuber as varchar(50)) as Orders,sum(pdd.OrderQuantity) as LotQuantity  from
            [CellPhoneProject].[dbo].ProjectMasters pm 
            inner join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pm.ProjectMasterId=pdd.ProjectMasterId
            where  pm.IsActive=1 and pm.ProjectMasterId={0} 
            group by pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber", proIds).ToList();
            return query;
        }

        public string SaveChinaQcInspectionClearanceDetails(List<ChinaQcInspectionsClearanceModel> issueList)
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);

            var chinaQc = new PmChinaQcInspectionsClearance();
            chinaQc.ProjectMasterId = issueList[0].ProjectMasterId;
            chinaQc.ProjectName = issueList[0].ProjectName;
            chinaQc.Orders = issueList[0].Orders;
            chinaQc.LotQuantity = issueList[0].LotQuantity;
            chinaQc.InspectionStartDate = issueList[0].InspectionStartDate;
            chinaQc.MaterialType = issueList[0].MaterialType;
            chinaQc.LotNo = issueList[0].LotNo;
            chinaQc.NoOfTimeOfInspection = issueList[0].NoOfTimeOfInspection;
            chinaQc.InspectionAttachment = issueList[0].InspectionAttachment;
            chinaQc.InspectionStatus = issueList[0].InspectionStatus;
            if (issueList[0].BtnDetails1=="report")
            {
                chinaQc.ClearanceStatus = "PENDING";
            }
            if (issueList[0].BtnDetails2 == "clearance")
            {
                chinaQc.ClearanceStatus = "APPROVED";
            }
            chinaQc.Added = users;
            chinaQc.AddedDate = DateTime.Now;
            _dbeEntities.PmChinaQcInspectionsClearances.Add(chinaQc);
            _dbeEntities.SaveChanges();

            #region mail 1

            if (chinaQc.Id > 0 && issueList[0].BtnDetails2 == "clearance")
            {

                var cmnUsers = (from pm in _dbeEntities.CmnUsers where pm.IsActive == true && pm.CmnUserId == users select pm).FirstOrDefault();

                List<string> toEmailIdList = new List<string>();
                toEmailIdList.Add(cmnUsers.Email);

                var ccEmailAdresses = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.CcMail=="YES" && pm.RoleNo==1 select pm.Email).ToList();
                var toLst = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.ToMail=="YES"  && pm.RoleNo==2 select pm.Email).ToList();

                foreach (var mm in toLst)
                {
                    toEmailIdList.Add(mm);
                }
               
                string body = string.Empty;

                body += "The shipment clearance of following model has been provided by <b>" + cmnUsers.UserFullName + "</b> on  <b>" + DateTime.Now
                    + ". </b>Please ensure necessary action from your side.<br/><br/><br/>";

                body += "<b>Shipment Clearance of " + issueList[0].ProjectName + " " + issueList[0].Orders + "</b><br/><br/>";

                body += " <table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 2 + " width = " + 500 + ">" +
                      "<tr bgcolor='#4da6ff' color='white'><td><b>Materials Type</b></td><td><b> LOT No.</b> </td><td><b> LOT Quantity</b> </td></tr>";

                body += "<tr><td>" + issueList[0].MaterialType + "</td><td> " + issueList[0].LotNo + "</td><td> " + issueList[0].LotQuantity + "</td></tr>";
                
                body += "</table>";
            

                var mailSendFromPms = new MailSendFromPms();

                mailSendFromPms.ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, "Shipment Clearance of " + issueList[0].ProjectName + " ", body);
            }
            #endregion

            #region mail 2

            if (chinaQc.Id > 0 && issueList[0].BtnDetails1 == "report")
            {

                var cmnUsers = (from pm in _dbeEntities.CmnUsers where pm.IsActive == true && pm.CmnUserId == users select pm).FirstOrDefault();

                List<string> toEmailIdList = new List<string>();
                toEmailIdList.Add(cmnUsers.Email);

                var ccEmailAdresses = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.CcMail == "YES" && pm.RoleNo == 1 select pm.Email).ToList();
                //var toLst = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.ToMail == "YES" && pm.RoleNo == 2 select pm.Email).ToList();

                //foreach (var mm in toLst)
                //{
                //    toEmailIdList.Add(mm);
                //}

                string body = string.Empty;

                body += "Due to fail in inspection report of the following model, the clearance request has been sent to you on <b>" + DateTime.Now
                    + ". </b>Please ensure necessary action from your side.<br/><br/><br/>";

                body += "<b>Inspection Status of " + issueList[0].ProjectName + " " + issueList[0].Orders + "</b><br/><br/>";

                body += " <table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 2 + " width = " + 500 + ">" +
                      "<tr bgcolor='#4da6ff' color='white'><td><b>Inspection Start Date</b></td><td><b>Materials Type</b></td><td><b> LOT No.</b> </td><td><b> LOT Quantity</b> </td><td><b> No of Time<br> of Inspection</b> </td><td><b> Inspection Status</b> </td></tr>";

                body += "<tr><td>" + issueList[0].InspectionStartDate + "</td><td>" + issueList[0].MaterialType + "</td><td> " + issueList[0].LotNo + "</td><td> " + issueList[0].LotQuantity + "</td><td> " + issueList[0].NoOfTimeOfInspection + "</td><td> " + issueList[0].InspectionStatus + "</td></tr>";

                body += "</table>";


                var mailSendFromPms = new MailSendFromPms();

                mailSendFromPms.ConstructAndSendEmail(ccEmailAdresses, toEmailIdList, "Request for Inspection Report Clearance of " + issueList[0].ProjectName + " ", body);
            }
            #endregion

            return "OK";
        }
        public List<ChinaQcInspectionsClearanceModel> GetChinaInspectionProjectDetails(string projectMasterId)
        {
            long proIds;
            long.TryParse(projectMasterId, out proIds);

            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            select * from
            [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearance] where ProjectMasterId={0} order by Id desc", proIds).ToList();

            var query2 = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            select pcq.ClearanceStatus,pcq.Id from [CellPhoneProject].[dbo].PmChinaQcInspectionsClearance pcq
            where pcq.ClearanceStatus='APPROVED' 
            AND pcq.Id not in (select MainId from [CellPhoneProject].[dbo].PmChinaQcInspectionApprovalLog pcc where pcc.MainId=pcq.Id)", proIds).ToList();

            if (query.Count!=0 && query2.Count==0)
            {
                foreach (var qq1 in query)
                {
                    qq1.Details = "NO";
                }
            }

            foreach (var qq1 in query)
            {
                foreach (var qq2 in query2)
                {
                    if (qq1.Id == qq2.Id)
                    {
                        qq1.Details = "YES";
                    }
                    else
                    {
                        qq1.Details = "NO";
                    }
                }
            }

            return query;
        }

        public List<ChinaQcInspectionsClearanceModel> GetChinaApprovalLog(string ids)
        {
            long proIds;
            long.TryParse(ids, out proIds);

            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            select Id,ProjectMasterId,RoleDetails,Name,UserFullName,EmployeeCode,RequestSent='Sent',case when ClearanceStatus is null then 'PENDING' else ClearanceStatus end as ClearanceStatus,RequestDate,TimeOfAction,TimeOfAction,case when TimeDelay is null then ((DATEDIFF(day,RequestDate,GETDATE())+1)) else TimeDelay end as TimeDelay,Remarks
            from
            (
	            select A.Id,A.ProjectMasterId,RequestDate,pcm.RoleDetails,pcm.Name,cu.UserFullName,pcm.EmployeeCode,RequestSent='Sent',pcc.ClearanceStatus,pcc.AddedDate as TimeOfAction,
	            (DATEDIFF(day,RequestDate,pcc.AddedDate)+1) as TimeDelay,pcc.Remarks
	            from 
	            (
		            select pcq.Id,pcq.ProjectMasterId,pcq.AddedDate as RequestDate
		            from [CellPhoneProject].[dbo].PmChinaQcInspectionsClearance pcq	
		            where pcq.Id={0}
	            )A 
	            left join  [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearanceMailList] pcm on pcm.IsActive=1 and RoleNo=1
	            left join [CellPhoneProject].[dbo].CmnUsers cu on cu.EmployeeCode=pcm.EmployeeCode 
	            left join [CellPhoneProject].[dbo].PmChinaQcInspectionApprovalLog pcc on pcc.Added=cu.CmnUserId and pcc.MainId={0}
            )B
            ", proIds).ToList();
            return query;
        }

        public ChinaQcInspectionsClearanceModel GetChinaApprovalStatus(string ids)
        {
            long proIds;
            long.TryParse(ids, out proIds);

            var query = _dbeEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            select * from
                [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearance] where Id={0}", proIds).FirstOrDefault();
            return query;
        }

        public string SaveShipmentDeniedData(long id, long projectMasterId, string Remarks)
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);

            var chinaQc = _dbeEntities.PmChinaQcInspectionsClearances.FirstOrDefault(x=>x.Id==id);
            chinaQc.ClearanceStatus = "NOTAPPROVED";
            chinaQc.DeniedReason = Remarks;
            chinaQc.Updated = users;
            chinaQc.UpdatedDate = DateTime.Now;
            _dbeEntities.PmChinaQcInspectionsClearances.AddOrUpdate(chinaQc);
            _dbeEntities.SaveChanges();

            #region mail

            if (chinaQc.Id > 0)
            {

                var cmnUsers = (from pm in _dbeEntities.CmnUsers where pm.IsActive == true && pm.CmnUserId == users select pm).FirstOrDefault();

                List<string> toEmailIdList = new List<string>();
                toEmailIdList.Add(cmnUsers.Email);

                var ccEmailAdresses = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.CcMail == "YES" && pm.RoleNo == 1 select pm.Email).ToList();
                var toLst = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.ToMail == "YES" && pm.RoleNo == 2 select pm.Email).ToList();

                foreach (var mm in toLst)
                {
                    toEmailIdList.Add(mm);
                }

                string body = string.Empty;

                body += "Due to the objection of providing the inspection report clearance of following model, the shipment cannot be proceeded further.<br/><br/><br/>";

                body += "<b>Shipment Cancellation of " + chinaQc.ProjectName + " " + chinaQc.Orders + "</b><br/><br/>";

                body += " <table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 2 + " width = " + 500 + ">" +
                      "<tr bgcolor='#4da6ff' color='white'><td><b>Inspection<br> Start Date</b></td><td><b>Materials Type</b></td><td><b> LOT No.</b> </td><td><b> LOT Quantity</b> </td><td><b>No of Time<br> of Inspection</b> </td><td><b>Inspection Status</b> </td></tr>";

                body += "<tr><td>" + chinaQc.InspectionStartDate + "</td><td>" + chinaQc.MaterialType + "</td><td> " + chinaQc.LotNo + "</td><td> " + chinaQc.LotQuantity + "</td><td> " + chinaQc.NoOfTimeOfInspection + "</td><td> " + chinaQc.InspectionStatus + "</td></tr>";

                body += "</table>";


                var mailSendFromPms = new MailSendFromPms();

                mailSendFromPms.ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, "Shipment Cancellation of " + chinaQc.ProjectName + " ", body);
            }
            #endregion

            return "OK";
        }

        public string SaveChinaShipmentClearance(long proIds)
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);

            var chinaQc = _dbeEntities.PmChinaQcInspectionsClearances.FirstOrDefault(x => x.Id == proIds);
            chinaQc.ClearanceStatus = "APPROVED";
            chinaQc.Updated = users;
            chinaQc.UpdatedDate = DateTime.Now;
            _dbeEntities.PmChinaQcInspectionsClearances.AddOrUpdate(chinaQc);
            _dbeEntities.SaveChanges();
            #region mail

            if (chinaQc.Id > 0)
            {

                var cmnUsers = (from pm in _dbeEntities.CmnUsers where pm.IsActive == true && pm.CmnUserId == users select pm).FirstOrDefault();

                List<string> toEmailIdList = new List<string>();
                toEmailIdList.Add(cmnUsers.Email);

                var ccEmailAdresses = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.CcMail == "YES" && pm.RoleNo == 1 select pm.Email).ToList();
                var toLst = (from pm in _dbeEntities.PmChinaQcInspectionsClearanceMailLists where pm.IsActive == true && pm.ToMail == "YES" && pm.RoleNo == 2 select pm.Email).ToList();

                foreach (var mm in toLst)
                {
                    toEmailIdList.Add(mm);
                }

                string body = string.Empty;

                body += "The shipment clearance of following model has been provided by <b>" + cmnUsers.UserFullName + "</b> on  <b>" + DateTime.Now
                    + ". </b>Please ensure necessary action from your side.<br/><br/><br/>";

                body += "<b>Shipment Clearance of " + chinaQc.ProjectName + " " + chinaQc.Orders + "</b><br/><br/>";

                body += " <table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 2 + " width = " + 500 + ">" +
                      "<tr bgcolor='#4da6ff' color='white'><td><b>Materials Type</b></td><td><b> LOT No.</b> </td><td><b> LOT Quantity</b> </td></tr>";

                body += "<tr><td>" + chinaQc.MaterialType + "</td><td> " + chinaQc.LotNo + "</td><td> " + chinaQc.LotQuantity + "</td></tr>";

                body += "</table>";


                var mailSendFromPms = new MailSendFromPms();

                mailSendFromPms.ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, "Shipment Clearance of " + chinaQc.ProjectName + " ", body);
            }
            #endregion

            return "OK";
        }

        #endregion
    }
}