﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
﻿using System.Data.Entity.Core.Metadata.Edm;
﻿using System.Data.Entity.Infrastructure;
﻿using System.Data.Entity.Migrations;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
﻿using System.Web.UI;
﻿using System.Web.UI.WebControls.Expressions;
using Antlr.Runtime;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
﻿using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
﻿using Oracle.ManagedDataAccess.Client;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
﻿using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
﻿using ProjectManagement.Models.Common;
﻿using ProjectManagement.ViewModels.AftersalesPm;
using ProjectManagement.ViewModels.Commercial;
﻿using ProjectManagement.ViewModels.Common;
﻿using Incentive = ProjectManagement.ViewModels.Commercial.Incentive;
﻿using ProjectMaster = ProjectManagement.DAL.DbModel.ProjectMaster;

//using PagedList;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class CommercialRepository : ICommercialRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;
        private readonly WCMSEntities _wcmsEntities;
        private readonly RBSYNERGYEntities _rbsynergyEntities;

        public CommercialRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
            _wcmsEntities = new WCMSEntities();
            _wcmsEntities.Configuration.LazyLoadingEnabled = false;
            _rbsynergyEntities = new RBSYNERGYEntities();
            _rbsynergyEntities.Configuration.LazyLoadingEnabled = false;
        }

        #region Update Methods

        public long UpdateProjectBtrcNoc(ProjectBtrcNocModel projectBtrcNocModel, bool isFileNull, long userId)
        {
            //var dbData = _dbEntities.ProjcetBtrcNocs.FirstOrDefault(i => i.ProjectBtrcNocId == projectBtrcNocModel.ProjectBtrcNocId);
            //if (dbData != null)
            //{
            //    if (!isFileNull)
            //        dbData.FilePath = projectBtrcNocModel.FilePath; //If file is updated then this path should change, otherwise it should not update in database.
            //    dbData.Updated = userId;
            //    dbData.UpdatedDate = DateTime.Now;

            //    /*If any onther column added in db table then here update code will be added*/
            //    _dbEntities.Entry(dbData).State = EntityState.Modified;
            //    _dbEntities.SaveChanges();
            //    return dbData.ProjectBtrcNocId;
            //}
            return 0;
        }

        public long UpdateProjectMaster(ProjectMasterModel model, long userId)
        {
            try
            {
                var dbData = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == model.ProjectMasterId);
                if (dbData != null)
                {
                    dbData.ApproxProjectFinishDate = model.ApproxProjectFinishDate;
                    dbData.ApproxProjectOrderDate = model.ApproxProjectOrderDate;
                    dbData.BackCamSensor = model.BackCamSensor;
                    dbData.ApproximatePrice = model.ApproximatePrice;
                    dbData.BackCam = model.BackCam;
                    dbData.BackCamBsi = model.BackCamBsi;
                    dbData.BateeryPossibleSupplierNames = model.BateeryPossibleSupplierNames;
                    dbData.BatteryCoverFinishingType = model.BatteryCoverFinishingType;
                    dbData.BatteryCoverFinishingType = model.BatteryCoverFinishingType;
                    dbData.BatteryRating = model.BatteryRating;
                    dbData.BatterySupplierName = model.BatterySupplierName;
                    dbData.BatteryType = model.BatteryType;
                    dbData.Cdma = model.Cdma;
                    dbData.ChargerRating = model.ChargerRating;
                    dbData.ChargerSupplierName = model.ChargerSupplierName;
                    dbData.ChipsetBit = model.ChipsetBit;
                    dbData.ChipsetCore = model.ChipsetCore;
                    dbData.ChipsetFrequency = model.ChipsetFrequency;
                    dbData.ChipsetName = model.ChipsetName;
                    dbData.Color = model.Color;
                    dbData.Compass = model.Compass;
                    dbData.CpuName = model.CpuName;
                    dbData.DisplayResulution = model.DisplayResulution;
                    dbData.DisplaySpeciality = model.DisplaySpeciality;
                    dbData.EarphoneConfirmPrice = model.EarphoneConfirmPrice;
                    dbData.EarphoneSupplierName = model.EarphoneSupplierName;
                    dbData.FinalPrice = model.FinalPrice;
                    dbData.FlashLight = model.FlashLight;
                    dbData.FourthGenFdd = model.FourthGenFdd;
                    dbData.FourthGenTdd = model.FourthGenTdd;

                    dbData.ApproxShipmentDate = model.ApproxShipmentDate;
                    dbData.BackCamera = model.BackCamera;
                    dbData.Battery = model.Battery;
                    dbData.Chipset = model.Chipset;
                    dbData.DisplayName = model.DisplayName;
                    dbData.DisplaySize = model.DisplaySize;
                    dbData.FrontCam = model.FrontCam;
                    dbData.FrontCamBsi = model.FrontCamBsi;
                    dbData.FrontCamSensor = model.FrontCamSensor;
                    dbData.FrontCamera = model.FrontCamera;
                    dbData.Gps = model.Gps;
                    dbData.Gsensor = model.Gsensor;
                    dbData.Gyroscope = model.Gyroscope;
                    dbData.HallSensor = model.HallSensor;
                    dbData.HousingFinalVendorName = model.HousingFinalVendorName;
                    dbData.HousingVendorName = model.HousingVendorName;
                    dbData.IsProjectManagerAssigned = model.IsProjectManagerAssigned;
                    dbData.LcdFinalVendor = model.LcdFinalVendor;
                    dbData.LcdVendor = model.LcdVendor;
                    dbData.Lsensor = model.Lsensor;
                    dbData.MemoryBrandName = model.MemoryBrandName;
                    dbData.NumberOfSample = model.NumberOfSample;
                    dbData.OrderQuantity = model.OrderQuantity;
                    dbData.OsName = model.OsName;
                    dbData.OsVersion = model.OsVersion;
                    dbData.Otg = model.Otg;
                    dbData.OtgCable = model.OtgCable;
                    dbData.PcbaFinalVendor = model.PcbaFinalVendor;
                    dbData.PcbaVendorName = model.PcbaVendorName;
                    dbData.ProcessorClock = model.ProcessorClock;
                    dbData.ProcessorName = model.ProcessorName;
                    dbData.ProjectName = model.ProjectName;
                    dbData.ProjectNameForScreening = model.ProjectNameForScreening;
                    dbData.ProjectType = model.ProjectType;
                    dbData.ProjectTypeId = model.ProjectTypeId;
                    dbData.Psensor = model.Psensor;
                    dbData.Ram = model.Ram;
                    dbData.Rom = model.Rom;
                    dbData.SecondGen = model.SecondGen;
                    dbData.SimSlotNumber = model.SimSlotNumber;
                    dbData.SlotType = model.SlotType;
                    dbData.SourcingType = model.SourcingType;
                    dbData.SpecialSensor = model.SpecialSensor;
                    dbData.SupplierId = model.SupplierId;
                    dbData.SupplierModelName = model.SupplierModelName;
                    dbData.SupplierName = model.SupplierName;
                    dbData.SupplierTrustLevel = model.SupplierTrustLevel;
                    dbData.ThirdGen = model.ThirdGen;
                    dbData.ThreeLayerScreenProtector = model.ThreeLayerScreenProtector;
                    dbData.TpFinalVendor = model.TpFinalVendor;
                    dbData.TpVendor = model.TpVendor;
                    dbData.Updated = userId;
                    dbData.UpdatedDate = DateTime.Now;
                    dbData.SwotAnalysisBy = model.SwotAnalysisBy;
                    dbData.SwotAnalysisDate = model.SwotAnalysisDate;
                    dbData.SwotOpportunityRemarks = model.SwotOpportunityRemarks;
                    _dbEntities.Entry(dbData).State = EntityState.Modified;

                }
                _dbEntities.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long UpdateProjectShipment(ProjectOrderShipmentModel model, long userId, List<ProjectMasterModel> issueList1)
        {
            ProjectOrderShipment shipment = GenereticRepo<ProjectOrderShipment>.GetById(_dbEntities,
                model.ProjectOrderShipmentId);
            shipment.ProjectMasterId = model.ProjectMasterId;
            shipment.ProjectPurchaseOrderFormId = model.ProjectPurchaseOrderFormId;
            shipment.ShipmentApproxDate = model.ShipmentApproxDate;
            shipment.ShipmentFinalDate = model.ShipmentFinalDate;
            shipment.AirportReleaseDate = model.AirportReleaseDate;
            shipment.AriportArrivalDate = model.AriportArrivalDate;
            shipment.BankNocDate = model.BankNocDate;
            shipment.ChainaInspectionDate = model.ChainaInspectionDate;
            shipment.CnfDate = model.CnfDate;
            shipment.CnfPayOrderDate = model.CnfPayOrderDate;
            shipment.FlightDepartureDate = model.FlightDepartureDate;
            shipment.ForwarderDate = model.ForwarderDate;
            shipment.WarehouseEntryDate = model.WarehouseEntryDate;
            shipment.Updated = userId;
            shipment.UpdatedDate = DateTime.Now;
            shipment.ShipmentType = model.ShipmentType;
            shipment.ProjectManagerClearanceDate = model.ProjectManagerClearanceDate;
            shipment.CostingDate = model.CostingDate;
            shipment.MarketReleaseDate = model.MarketReleaseDate;
            shipment.ShipmentPercentage = model.ShipmentPercentage;
            shipment.IsFinalShipment = model.IsFinalShipment;
            shipment.ChinaIqcPassHundredPercent = model.ChinaIqcPassHundredPercent;
            shipment.FocQuantity = model.FocQuantity;
            shipment.OrderShipmentQuantity = model.OrderShipmentQuantity;
            //if (model.ChinaIqcFail == null)
            //{
            //    shipment.ChinaIqcFail = 0;
            //}
            //else
            //{
            //    shipment.ChinaIqcFail = model.ChinaIqcFail;
            //}

            //shipment.ManagementApproval = model.ManagementApproval;
            //shipment.ManagementApprovalDate = model.ManagementApprovalDate;

            //ProjectOrderShipment projectOrderShipment = GenericMapper<ProjectOrderShipmentModel, ProjectOrderShipment>.GetDestination(model);
            try
            {
                GenereticRepo<ProjectOrderShipment>.Update(_dbEntities, shipment);

                //--new Finish Good add--//

                long proOrderId = 0;
                //proOrderId = projectOrderShipment.ProjectOrderShipmentId;
                proOrderId = model.ProjectOrderShipmentId;

                var proOrderIds = proOrderId;

                if (issueList1 != null && proOrderIds != 0)
                {
                    foreach (var proMds in issueList1)
                    {
                        var pModels =
                            (from pm in _dbEntities.ProjectMasters
                             where pm.ProjectMasterId == proMds.ProjectMasterId
                             select pm).FirstOrDefault();

                        var pModelsExist =
                            (from pm in _dbEntities.ShipmentFinishGoodModels
                             where pm.ProjectMasterId == proMds.ProjectMasterId && pm.ApproxFinishGoodManufactureQty == proMds.ApproxFinishGoodManufactureQty
                             && pm.ProjectOrderShipmentId == proOrderId
                             select pm).FirstOrDefault();
                        if (pModelsExist == null)
                        {
                            var finishGood = new ShipmentFinishGoodModel();
                            finishGood.ProjectMasterId = model.ProjectMasterId;
                            finishGood.ProjectOrderShipmentId = proOrderIds;

                            finishGood.FinishGoodProjectMasterId = proMds.ProjectMasterId;
                            finishGood.FinishGoodModel = pModels.ProjectModel;
                            finishGood.FinishGoodModelOrderNumber = pModels.OrderNuber;
                            finishGood.ApproxFinishGoodManufactureQty = proMds.ApproxFinishGoodManufactureQty;
                            finishGood.Added = userId;
                            finishGood.AddedDate = DateTime.Now;
                            _dbEntities.ShipmentFinishGoodModels.Add(finishGood);
                            _dbEntities.SaveChanges();
                        }
                    }
                }
                //--end Finish Good---//

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long UpdatePhPcbaInfo(PhPcbaInfoModel model, long userId)
        {
            try
            {
                var dbData = _dbEntities.PhPcbaInfos.FirstOrDefault(i => i.PhPcbaInfoId == model.PhPcbaInfoId);
                if (dbData != null)
                {
                    dbData.VendorName = model.VendorName;
                    dbData.Updated = userId;
                    dbData.FinalVendor = model.FinalVendor;
                    dbData.UpdatedDate = DateTime.Now;
                    _dbEntities.Entry(dbData).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                    return dbData.PhPcbaInfoId;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long UpdatePhAccessory(PhAccessoryModel model, long userId)
        {
            var dbData = _dbEntities.PhAccessories.FirstOrDefault(i => i.PhAccessoriesId == model.PhAccessoriesId);
            if (dbData != null)
            {
                dbData.FlashLight = Convert.ToBoolean(model.FlashLight);
                dbData.OtgCable = Convert.ToBoolean(model.OtgCable);
                dbData.ThreeLayerScreenProtector = Convert.ToBoolean(model.ThreeLayerScreenProtector);
                dbData.BatteryCoverFinishingType = model.BatteryCoverFinishingType;
                dbData.BatteryCoverLogoType = model.BatteryCoverLogoType;
                dbData.ChargerRating = model.ChargerRating;
                dbData.ChargerSupplierName = model.ChargerSupplierName;
                dbData.EarphoneConfirmPrice = Convert.ToDecimal(model.EarphoneConfirmPrice);
                dbData.EarphoneSupplierName = model.EarphoneSupplierName;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;

            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhCamInfo(PhCamInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhCamInfos.FirstOrDefault(i => i.PhCamInfoId == model.PhCamInfoId);
            if (dbData != null)
            {
                dbData.BackCam = model.BackCam;
                dbData.BackCamBsi = model.BackCamBsi;
                dbData.BackCamSensor = model.BackCamSensor;
                dbData.FrontCam = model.FrontCam;
                dbData.FrontCamBsi = model.FrontCamBsi;
                dbData.FrontCamSensor = model.FrontCamSensor;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhChipsetInfo(PhChipsetInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhChipsetInfos.FirstOrDefault(i => i.PhChipsetInfoId == model.PhChipsetInfoId);
            if (dbData != null)
            {
                dbData.Bit = Convert.ToInt32(model.Bit);
                dbData.ChipsetFrequency = model.ChipsetFrequency;
                dbData.ChipsetName = model.ChipsetName;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhHousingInfo(PhHousingInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhHousingInfos.FirstOrDefault(i => i.PhHousingInfoId == model.PhHousingInfoId);
            if (dbData != null)
            {
                dbData.VendorName = model.VendorName;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhMemoryInfo(PhMemoryInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhMemoryInfos.FirstOrDefault(i => i.PhMemoryInfoId == model.PhMemoryInfoId);
            if (dbData != null)
            {
                dbData.BrandName = model.BrandName;
                dbData.Ram = model.Ram;
                dbData.Rom = model.Rom;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhNetworkFreqAndBand(PhNetworkFreqAndBandModel model, long userId)
        {
            var dbData =
                _dbEntities.PhNetworkFreqAndBands.FirstOrDefault(
                    i => i.PhNetworkFreqAndBandsId == model.PhNetworkFreqAndBandsId);
            if (dbData != null)
            {
                dbData.Cdma = model.Cdma;
                dbData.FourthGenFdd = model.FourthGenFdd;
                dbData.FourthGenTdd = model.FourthGenTdd;
                dbData.SecondGen = model.SecondGen;
                dbData.ThirdGen = model.ThirdGen;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhSensorAndOther(PhSensorAndOtherModel model, long userId)
        {
            var dbData =
                _dbEntities.PhSensorAndOthers.FirstOrDefault(
                    i => i.PhSensorAndOthersInfoId == model.PhSensorAndOthersInfoId);
            if (dbData != null)
            {
                dbData.Compass = model.Compass;
                dbData.Gps = model.Gps;
                dbData.Gsensor = model.Gsensor;
                dbData.Gyroscope = model.Gyroscope;
                dbData.HallSensor = model.HallSensor;
                dbData.Lsensor = model.Lsensor;
                dbData.Otg = model.Otg;
                dbData.Psensor = model.Psensor;
                dbData.SpecialSensor = model.SpecialSensor;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhOperatingSyModel(PhOperatingSyModel model, long userId)
        {
            var dbData = _dbEntities.PhOperatingSys.FirstOrDefault(i => i.PhOsId == model.PhOsId);
            if (dbData != null)
            {
                dbData.OsName = model.OsName;
                dbData.Version = model.Version;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhBatteryInfoModel(PhBatteryInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhBatteryInfoes.FirstOrDefault(i => i.PhBatteryInfoId == model.PhBatteryInfoId);
            if (dbData != null)
            {
                dbData.BatteryRating = model.BatteryRating;
                dbData.BatterySupplierName = model.BatterySupplierName;
                dbData.BatteryType = model.BatteryType;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhColorInfoModel(PhColorInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhColorInfoes.FirstOrDefault(i => i.PhColorInfoId == model.PhColorInfoId);
            if (dbData != null)
            {
                dbData.Color = model.Color;
                dbData.Quantity = model.Quantity;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdatePhTpLcdInfo(PhTpLcdInfoModel model, long userId)
        {
            var dbData = _dbEntities.PhTpLcdInfos.FirstOrDefault(i => i.PhTpLcdInfoId == model.PhTpLcdInfoId);
            if (dbData != null)
            {
                dbData.DisplayResulution = model.DisplayResulution;
                dbData.DisplaySize = model.DisplaySize;
                dbData.DisplaySpeciality = model.DisplaySpeciality;
                dbData.LcdVendor = model.LcdVendor;
                dbData.TpVendor = model.TpVendor;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();
            return 1;
        }

        public long UpdateProjectCriticalControlPointModel(ProjectCriticalControlPointModel model, long userId)
        {
            var dbData =
                _dbEntities.ProjectCriticalControlPoints.FirstOrDefault(
                    i => i.ProjectCriticalControlPointId == model.ProjectCriticalControlPointId);
            if (dbData != null)
            {
                dbData.BackCamera = model.BackCamera;
                dbData.BackCoverFinishing = model.BackCoverFinishing;
                dbData.BackCoverMaterial = model.BackCoverMaterial;
                dbData.BackSideScreenPro = model.BackSideScreenPro;
                dbData.BackSideThermalPaper = model.BackSideThermalPaper;
                dbData.Battery = model.Battery;
                dbData.BothSimFourG = model.BothSimFourG;
                dbData.BsiSensor = model.BsiSensor;
                dbData.Charger = model.Charger;
                dbData.Earphone = model.Earphone;
                dbData.Flash = model.Flash;
                dbData.FlipCover = model.FlipCover;
                dbData.FreeScreenProOnGb = model.FreeScreenProOnGb;
                dbData.FronCamera = model.FronCamera;
                dbData.LogoPrintType = model.LogoPrintType;
                dbData.OtgCable = model.OtgCable;
                dbData.RawDesignFileOfId = model.RawDesignFileOfId;
                dbData.SarAndCcc = model.SarAndCcc;
                dbData.ThreeLayerScreenProOnPhone = model.ThreeLayerScreenProOnPhone;
                dbData.UsbCableLength = model.UsbCableLength;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;
                _dbEntities.SaveChanges();
                return dbData.ProjectCriticalControlPointId;
            }
            return 0;
        }

        public long UpdateProjectProformaInvoice(ProjectProformaInvoiceModel model, long userId)
        {
            var dbData =
                _dbEntities.ProjectProformaInvoices.FirstOrDefault(
                    i => i.ProjectProformaInvoiceId == model.ProjectProformaInvoiceId);
            if (dbData != null)
            {
                dbData.PiNo = model.PiNo;
                dbData.PiDate = model.PiDate;
                dbData.FilePath = model.FilePath;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;
                _dbEntities.SaveChanges();
                return dbData.ProjectProformaInvoiceId;
            }
            return 0;
        }

        public long UpdateProjectOrder(ProjectOrderModel model, long userId)
        {
            var dbData =
                _dbEntities.ProjectOrders.FirstOrDefault(
                    i => i.ProjectOrderId == model.ProjectOrderId);
            if (dbData != null)
            {
                dbData.PoNo = model.PoNo;
                dbData.PoDate = model.PoDate;
                dbData.OrderQuantity = model.OrderQuantity;
                _dbEntities.Entry(dbData).State = EntityState.Modified;
                _dbEntities.SaveChanges();
                return dbData.ProjectOrderId;
            }
            return 0;
        }

        public long UpdatePrice(ProjectPriceModel model, long userId)
        {
            var dbData = _dbEntities.ProjectPrices.FirstOrDefault(i => i.ProjectPriceId == model.ProjectPriceId);
            if (dbData != null)
            {
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;
                dbData.Price = model.Price;
                dbData.Updated = userId;
                dbData.UpdatedDate = DateTime.Now;

                _dbEntities.Entry(dbData).State = EntityState.Modified;
                _dbEntities.SaveChanges();
                return dbData.ProjectPriceId;
            }
            return 0;
        }

        public void UpdatePoNoFromLc(string pono, long projectOrderId, bool lc1, bool lc2)
        {
            //var model = (from v in _dbEntities.ProjectPurchaseOrderForms
            //             where v.ProjectPurchaseOrderFormId == projectOrderId
            //             select new ProjectPurchaseOrderFormModel
            //             {
            //                 ProjectPurchaseOrderFormId = v.ProjectPurchaseOrderFormId,
            //                 PurchaseOrderNumber = v.PurchaseOrderNumber,
            //                 Receiver = v.Receiver,
            //                 ProjectMasterId = v.ProjectMasterId,
            //                 CompanyName = v.CompanyName,
            //                 CompanyAddress = v.CompanyAddress,
            //                 Subject = v.Subject,
            //                 DescriptionHeader = v.DescriptionHeader,
            //                 DescriptionBody = v.DescriptionBody,
            //                 Signature = v.Signature,
            //                 Quantity = v.Quantity,
            //                 Color = v.Color,
            //                 Value = v.Value,
            //                 PoDate = v.PoDate,
            //                 IsCompleted = v.IsCompleted,
            //                 IsCompletedDate = v.IsCompletedDate,
            //                 PoCategory = v.PoCategory,
            //                 Added = v.Added,
            //                 AddedDate = v.AddedDate,
            //                 Updated = v.Updated,
            //                 UpdatedDate = v.UpdatedDate,
            //                 IsSpareConfirmedDate = v.IsSpareConfirmedDate,
            //                 IsSpareSubmittedDate = v.IsSpareSubmittedDate,
            //                 IsSpareSubmittedRemark = v.IsSpareSubmittedRemark,
            //                 PiDate = v.PiDate,
            //                 ReminderMailFor18Month = v.ReminderMailFor18Month,
            //                 AfterSalesPmComment = v.AfterSalesPmComment,
            //                 ProcessTeamComment = v.ProcessTeamComment,
            //                 QcComment = v.QcComment,
            //                 FocStatus = v.FocStatus,
            //                 IsApprovedByCommercial = v.IsApprovedByCommercial,
            //                 InchargeComment = v.InchargeComment,
            //                 OrderDate = v.OrderDate,
            //                 MarketClearanceDate = v.MarketClearanceDate
            //             }).FirstOrDefault();



            var model = _dbEntities.ProjectPurchaseOrderForms.FirstOrDefault(i => i.ProjectPurchaseOrderFormId == projectOrderId);

            if (model != null && pono != null)
            {
                //var exist = model.PurchaseOrderNumber.StartsWith(pono);
                if (lc1 && !lc2)
                {
                    model.PurchaseOrderNumber = pono;
                    //Mapper.CreateMap<ProjectPurchaseOrderFormModel, ProjectPurchaseOrderForm>();
                    //var m = Mapper.Map<ProjectPurchaseOrderForm>(model);
                    _dbEntities.ProjectPurchaseOrderForms.AddOrUpdate(model);
                    _dbEntities.SaveChanges();
                }
                if (!lc1 && lc2)
                {
                    model.PurchaseOrderNumber = model.PurchaseOrderNumber + "," + pono;
                    //Mapper.CreateMap<ProjectPurchaseOrderFormModel, ProjectPurchaseOrderForm>();
                    //var m = Mapper.Map<ProjectPurchaseOrderForm>(model);
                    _dbEntities.ProjectPurchaseOrderForms.AddOrUpdate(model);
                    _dbEntities.SaveChanges();
                }
            }
        }

        public long UpdateProjectLc(ProjectLcModel model, long userId)
        {
            ProjectLc lc = GenereticRepo<ProjectLc>.GetById(_dbEntities, model.ProjectLcId);
            lc.ProjectMasterId = model.ProjectMasterId;
            lc.BankOpeningDate = model.BankOpeningDate;
            lc.BtrcNocDate = model.BtrcNocDate;
            lc.LcPassDate = model.LcPassDate;
            lc.NocReceiveDate = model.NocReceiveDate;
            lc.OpeningDate = model.OpeningDate;
            lc.ProjectOrderId = model.ProjectOrderId;
            lc.SampleSendDate = model.SampleSendDate;
            lc.SupplierDraftDate = model.SupplierDraftDate;
            lc.LcNo = model.LcNo;
            lc.Updated = userId;
            lc.UpdatedDate = DateTime.Now;
            try
            {
                _dbEntities.Entry(lc).State = EntityState.Modified;
                _dbEntities.SaveChanges();
                return lc.ProjectLcId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion

        #region Get Methods

        public List<Brand> GetBrands()
        {
            var v = _dbEntities.Brands.ToList();
            return v;
        }


        public ProjectLcModel GetProjectLc(long lcId = 0, long projectId = 0)
        {
            var projectLc = new ProjectLc();
            if (lcId > 0)
            {
                projectLc = GenereticRepo<ProjectLc>.GetById(_dbEntities, lcId);
            }
            else if (projectId > 0)
            {
                projectLc = GenereticRepo<ProjectLc>.Get(_dbEntities, lc => lc.ProjectMasterId == projectId);
            }
            ProjectLcModel model = GenericMapper<ProjectLc, ProjectLcModel>.GetDestination(projectLc);
            return model;
        }

        public List<ProjectPurchaseOrderFormModel> GetProjectOrderModels(long projectId = 0)
        {
            List<ProjectPurchaseOrderForm> projectOrders = projectId > 0
                ? GenereticRepo<ProjectPurchaseOrderForm>.GetList(_dbEntities,
                    order => order.ProjectMasterId == projectId)
                : GenereticRepo<ProjectPurchaseOrderForm>.GetList(_dbEntities);
            List<ProjectPurchaseOrderFormModel> models =
                GenericMapper<ProjectPurchaseOrderForm, ProjectPurchaseOrderFormModel>.GetDestinationList(projectOrders);
            return models;

        }

        public List<Accessory> GetAllAccessories()
        {
            List<Accessory> result = _dbEntities.Accessories.ToList();
            return result;
        }

        public List<ProjectLcModel> GetProjectLcModels()
        {
            var projectLcModels = (from lc in _dbEntities.ProjectLcs
                                   join master in _dbEntities.ProjectMasters on lc.ProjectMasterId equals master.ProjectMasterId
                                   join order in _dbEntities.ProjectPurchaseOrderForms on lc.ProjectOrderId equals
                                       order.ProjectPurchaseOrderFormId
                                   join user in _dbEntities.CmnUsers on lc.Added equals user.CmnUserId
                                   where lc.IsComplete == false //&& lc.Added == addedById
                                   select new ProjectLcModel
                                   {
                                       ProjectLcId = lc.ProjectLcId,
                                       ProjectMasterId = lc.ProjectMasterId,
                                       ProjectOrderId = lc.ProjectOrderId,
                                       Added = lc.Added,
                                       AddedByName = user.UserFullName,
                                       PoDate = order.PoDate ?? new DateTime(),
                                       PoNo = order.PurchaseOrderNumber,
                                       ProjectName = master.ProjectName,
                                       AddedDate = lc.AddedDate,
                                       BankOpeningDate = lc.BankOpeningDate,
                                       Updated = lc.Updated,
                                       UpdatedDate = lc.UpdatedDate,
                                       BtrcNocDate = lc.BtrcNocDate,
                                       LcPassDate = lc.LcPassDate,
                                       NocReceiveDate = lc.NocReceiveDate,
                                       OpeningDate = lc.OpeningDate,
                                       SampleSendDate = lc.SampleSendDate,
                                       SupplierDraftDate = lc.SupplierDraftDate,
                                       OrderNo = master.OrderNuber,
                                       LcNo = lc.LcNo,
                                       LcValue = lc.LcValue,
                                       Currency = lc.Currency,
                                       BdtLcAmount = lc.BdtLcAmount
                                   }).ToList();
            return projectLcModels;
        }

        public List<ProjectLcModel> GetProjectLcsByDateRange(DateTime from, DateTime to)
        {
            from = from.AddSeconds(-1);
            var v =
                _dbEntities.ProjectLcs.Where(x => x.OpeningDate > from && x.OpeningDate <= to)
                    .Select(x => new ProjectLcModel
                    {
                        ProjectLcId = x.ProjectLcId,
                        ProjectMasterId = x.ProjectMasterId,
                        ProjectName = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectMasterId).Select(m => m.ProjectName).FirstOrDefault(),
                        OrderNo = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectMasterId).Select(m => m.OrderNuber).FirstOrDefault(),
                        PoNo = _dbEntities.ProjectPurchaseOrderForms.Where(m => m.ProjectPurchaseOrderFormId == x.ProjectOrderId).Select(m => m.PurchaseOrderNumber).FirstOrDefault(),
                        LcNo = x.LcNo,
                        LcValue = x.LcValue,
                        OpeningDate = x.OpeningDate,
                        AddedByName = _dbEntities.CmnUsers.Where(m => m.CmnUserId == x.Added).Select(m => m.UserFullName).FirstOrDefault(),
                        AddedDate = x.AddedDate
                    }).ToList();
            return v;
        }

        //public List<ProjectOrderShipmentModel> GetShipmentModels(long addedById)
        //{
        //    var projectShipmentList = (from shipment in _dbEntities.ProjectOrderShipments
        //                               join master in _dbEntities.ProjectMasters on shipment.ProjectMasterId equals (master.ProjectMasterId) into group1
        //                               from g1 in group1.DefaultIfEmpty()
        //                               join purchaseOrder in _dbEntities.ProjectPurchaseOrderForms on shipment.ProjectPurchaseOrderFormId equals (purchaseOrder.ProjectPurchaseOrderFormId) into group2
        //                               from g2 in group2.DefaultIfEmpty()
        //                               join user in _dbEntities.CmnUsers on shipment.Added equals (user.CmnUserId) into group3
        //                               from g3 in group3.DefaultIfEmpty()
        //                               where shipment.IsComplete == false && g1.IsActive
        //                               select new ProjectOrderShipmentModel
        //                               {
        //                                   ProjectOrderShipmentId = shipment.ProjectOrderShipmentId,
        //                                   ProjectPurchaseOrderFormId = shipment.ProjectPurchaseOrderFormId,
        //                                   ProjectMasterId = g1.ProjectMasterId,
        //                                   Added = shipment.Added,
        //                                   AddedByName = g3.UserFullName,
        //                                   AddedDate = shipment.AddedDate,
        //                                   AirportReleaseDate = shipment.AirportReleaseDate,
        //                                   AriportArrivalDate = shipment.AriportArrivalDate,
        //                                   BankNocDate = shipment.BankNocDate,
        //                                   ChainaInspectionDate = shipment.ChainaInspectionDate,
        //                                   CnfDate = shipment.CnfDate,
        //                                   CnfPayOrderDate = shipment.CnfPayOrderDate,
        //                                   CostingDate = shipment.CostingDate,
        //                                   FlightDepartureDate = shipment.FlightDepartureDate,
        //                                   ForwarderDate = shipment.ForwarderDate,
        //                                   MarketReleaseDate = shipment.MarketReleaseDate,
        //                                   PoDate = g2.PoDate == null ? DateTime.MinValue : (DateTime)g2.PoDate,
        //                                   PoNo = g2.PurchaseOrderNumber,
        //                                   ProjectName = g1.ProjectName,
        //                                   ShipmentApproxDate = shipment.ShipmentApproxDate,
        //                                   ShipmentFinalDate = shipment.ShipmentFinalDate,
        //                                   Updated = shipment.Updated,
        //                                   UpdatedDate = shipment.UpdatedDate,
        //                                   WarehouseEntryDate = shipment.WarehouseEntryDate,
        //                                   PoWiseShipmentNumber = shipment.PoWiseShipmentNumber,
        //                                   PoCount = g1.OrderNuber
        //                               }).ToList();
        //    foreach (var model in projectShipmentList)
        //    {
        //        model.PoOrdinal = CommonConversion.AddOrdinal(model.PoCount) + " Purchase Order";
        //        model.ShipmentNoOrdinal = CommonConversion.AddOrdinal(model.PoWiseShipmentNumber) + " Shipment";
        //    }
        //    return projectShipmentList;
        //}

        public List<ProjectOrderShipmentModel> GetShipmentModels(long addedById)
        {
            var projectShipmentList = _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(@"
            select 
            case when ps.ProjectOrderShipmentId in (select sm.ProjectOrderShipmentId 
            FROM [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sm where sm.ProjectOrderShipmentId=ps.ProjectOrderShipmentId) then 'YES' end as FinishGoodCheck,

            ps.ProjectOrderShipmentId,ps.ProjectPurchaseOrderFormId,pm.ProjectMasterId,ps.Added,cu.UserFullName as AddedByName,ps.AddedDate,ps.AirportReleaseDate,ps.AriportArrivalDate,ps.BankNocDate,ps.ChainaInspectionDate,ps.CnfDate,ps.CnfPayOrderDate,
            ps.CostingDate,ps.FlightDepartureDate,ps.ForwarderDate,ps.MarketReleaseDate,po.PoDate as PoDate1,po.PurchaseOrderNumber as PoNo,pm.ProjectName,ps.ShipmentApproxDate,ps.ShipmentFinalDate,ps.Updated,ps.UpdatedDate,
            ps.WarehouseEntryDate,ps.PoWiseShipmentNumber,pm.OrderNuber as PoCount,ps.IsFinalShipment

            from 

            CellPhoneProject.dbo.ProjectOrderShipments ps
            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectPurchaseOrderFormId=ps.ProjectPurchaseOrderFormId
            left join CellPhoneProject.dbo.CmnUsers cu on cu.Added=ps.Added

            where ps.IsComplete = 'false' and pm.IsActive='true'

            order by ps.ProjectOrderShipmentId desc").ToList();
            foreach (var model in projectShipmentList)
            {
                model.PoOrdinal = CommonConversion.AddOrdinal(model.PoCount) + " Purchase Order";
                model.ShipmentNoOrdinal = CommonConversion.AddOrdinal(model.PoWiseShipmentNumber) + " Shipment";
            }
            return projectShipmentList;
        }
        public bool GetFnishGoodData(long ProjectOrderShipmentId)
        {
            List<ProjectOrderShipmentModel> getIncentiveReports = null;
            if (ProjectOrderShipmentId != 0)
            {

                string getIncentiveReportQuery = string.Format(@"select sm.ProjectOrderShipmentId 
                  FROM [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sm    
                  left join  [CellPhoneProject].[dbo].[ProjectOrderShipments] ps  on ps.ProjectOrderShipmentId=sm.ProjectOrderShipmentId
                where sm.ProjectOrderShipmentId='{0}'", ProjectOrderShipmentId);
                getIncentiveReports =
                   _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(getIncentiveReportQuery).ToList();

            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        //        public List<ProjectOrderShipmentModel> GetClosedShipmentModels(long addedById)
        //        {
        //            var projectShipmentList = (from shipment in _dbEntities.ProjectOrderShipments
        //                                       join master in _dbEntities.ProjectMasters on shipment.ProjectMasterId equals (master.ProjectMasterId) into group1
        //                                       from g1 in group1.DefaultIfEmpty()

        //                                       join purchaseOrder in _dbEntities.ProjectPurchaseOrderForms on shipment.ProjectPurchaseOrderFormId
        //                                       equals (purchaseOrder.ProjectPurchaseOrderFormId) into group2
        //                                       from g2 in group2.DefaultIfEmpty()

        //                                       join user in _dbEntities.CmnUsers on shipment.Added equals (user.CmnUserId) into group3
        //                                       from g3 in group3.DefaultIfEmpty()

        //                                       where shipment.IsComplete == true && g1.IsActive
        //                                       select new ProjectOrderShipmentModel
        //                                       {
        //                                           ProjectOrderShipmentId = shipment.ProjectOrderShipmentId,
        //                                           ProjectPurchaseOrderFormId = shipment.ProjectPurchaseOrderFormId,
        //                                           ProjectMasterId = g1.ProjectMasterId,
        //                                           Added = shipment.Added,
        //                                           AddedByName = g3.UserFullName,
        //                                           AddedDate = shipment.AddedDate,
        //                                           AirportReleaseDate = shipment.AirportReleaseDate,
        //                                           AriportArrivalDate = shipment.AriportArrivalDate,
        //                                           BankNocDate = shipment.BankNocDate,
        //                                           ChainaInspectionDate = shipment.ChainaInspectionDate,
        //                                           CnfDate = shipment.CnfDate,
        //                                           CnfPayOrderDate = shipment.CnfPayOrderDate,
        //                                           CostingDate = shipment.CostingDate,
        //                                           FlightDepartureDate = shipment.FlightDepartureDate,
        //                                           ForwarderDate = shipment.ForwarderDate,
        //                                           MarketReleaseDate = shipment.MarketReleaseDate,
        //                                           PoDate = g2.PoDate == null ? DateTime.MinValue : (DateTime)g2.PoDate,
        //                                           PoNo = g2.PurchaseOrderNumber,
        //                                           ProjectName = g1.ProjectName,
        //                                           ShipmentApproxDate = shipment.ShipmentApproxDate,
        //                                           ShipmentFinalDate = shipment.ShipmentFinalDate,
        //                                           Updated = shipment.Updated,
        //                                           UpdatedDate = shipment.UpdatedDate,
        //                                           WarehouseEntryDate = shipment.WarehouseEntryDate,
        //                                           PoWiseShipmentNumber = shipment.PoWiseShipmentNumber,
        //                                           PoCount = g1.OrderNuber,

        //                                       }).ToList();

        //            foreach (var model in projectShipmentList)
        //            {
        //                var fnGd = _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(@"select top 1 sm.ProjectOrderShipmentId 
        //                            FROM [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sm    
        //                            left join  [CellPhoneProject].[dbo].[ProjectOrderShipments] ps  on ps.ProjectOrderShipmentId=sm.ProjectOrderShipmentId
        //                            where sm.ProjectOrderShipmentId={0} ", model.ProjectOrderShipmentId).FirstOrDefault();
        //                // bool alreadyExist = fnGd.Contains(fnGd.ProjectOrderShipmentId);
        //                if (fnGd != null)
        //                {
        //                    if (fnGd.ProjectOrderShipmentId == model.ProjectOrderShipmentId)
        //                    {
        //                        model.FinishGoodCheck = "YES";
        //                    }
        //                    else
        //                    {
        //                        model.FinishGoodCheck = "NO";
        //                    }
        //                }

        //            }

        //            foreach (var model in projectShipmentList)
        //            {
        //                model.PoOrdinal = CommonConversion.AddOrdinal(model.PoCount) + " Purchase Order";
        //                model.ShipmentNoOrdinal = CommonConversion.AddOrdinal(model.PoWiseShipmentNumber) + " Shipment";

        ////                var fnGd = _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(@"select top 1 sm.ProjectOrderShipmentId 
        ////                FROM [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sm    
        ////                left join  [CellPhoneProject].[dbo].[ProjectOrderShipments] ps  on ps.ProjectOrderShipmentId=sm.ProjectOrderShipmentId
        ////                where sm.ProjectOrderShipmentId={0} ", model.ProjectOrderShipmentId).FirstOrDefault();
        ////                // bool alreadyExist = fnGd.Contains(fnGd.ProjectOrderShipmentId);
        ////                if (fnGd != null)
        ////                {
        ////                    if (fnGd.ProjectOrderShipmentId == model.ProjectOrderShipmentId)
        ////                    {
        ////                        model.FinishGoodCheck = "YES";
        ////                    }
        ////                    else
        ////                    {
        ////                        model.FinishGoodCheck = "NO";
        ////                    }
        ////                }


        //                //var isSaveCheck = false;
        //                //isSaveCheck = GetFnishGoodData(model.ProjectOrderShipmentId);

        //                //if (isSaveCheck==true)
        //                //{
        //                //    model.FinishGoodCheck = "YES";
        //                //}
        //                //else
        //                //{
        //                //    model.FinishGoodCheck = "NO";
        //                //}

        //            }
        //            return projectShipmentList;
        //        }
        public List<ProjectOrderShipmentModel> GetClosedShipmentModels(long addedById)
        {
            var projectShipmentList = _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(@"
            select 
            case when ps.ProjectOrderShipmentId in (select sm.ProjectOrderShipmentId 
            FROM [CellPhoneProject].[dbo].[ShipmentFinishGoodModel] sm where sm.ProjectOrderShipmentId=ps.ProjectOrderShipmentId) then 'YES' end as FinishGoodCheck,

            ps.ProjectOrderShipmentId,ps.ProjectPurchaseOrderFormId,pm.ProjectMasterId,ps.Added,cu.UserFullName as AddedByName,ps.AddedDate,ps.AirportReleaseDate,ps.AriportArrivalDate,ps.BankNocDate,ps.ChainaInspectionDate,ps.CnfDate,ps.CnfPayOrderDate,
            ps.CostingDate,ps.FlightDepartureDate,ps.ForwarderDate,ps.MarketReleaseDate,po.PoDate as PoDate1,po.PurchaseOrderNumber as PoNo,pm.ProjectName,ps.ShipmentApproxDate,ps.ShipmentFinalDate,ps.Updated,ps.UpdatedDate,
            ps.WarehouseEntryDate,ps.PoWiseShipmentNumber,pm.OrderNuber as PoCount,ps.IsFinalShipment

            from 

            CellPhoneProject.dbo.ProjectOrderShipments ps
            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms po on po.ProjectPurchaseOrderFormId=ps.ProjectPurchaseOrderFormId
            left join CellPhoneProject.dbo.CmnUsers cu on cu.Added=ps.Added

            where ps.IsComplete = 'true' and pm.IsActive='true'

            order by ps.ProjectOrderShipmentId desc").ToList();

            foreach (var model in projectShipmentList)
            {
                model.PoOrdinal = CommonConversion.AddOrdinal(model.PoCount) + " Purchase Order";
                model.ShipmentNoOrdinal = CommonConversion.AddOrdinal(model.PoWiseShipmentNumber) + " Shipment";

            }
            return projectShipmentList;
        }
        public ProjectLc CloseLc(long id)
        {
            var dbLc = _dbEntities.ProjectLcs.FirstOrDefault(i => i.ProjectLcId == id);
            if (dbLc != null)
            {
                dbLc.IsComplete = true;
                dbLc.Updated = Convert.ToInt64(HttpContext.Current.User.Identity.Name);
                dbLc.UpdatedDate = DateTime.Now;
                try
                {
                    _dbEntities.Entry(dbLc).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                    return dbLc;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public ProjectOrderShipment CloseShipment(long id)
        {
            var dbShipment = _dbEntities.ProjectOrderShipments.FirstOrDefault(i => i.ProjectOrderShipmentId == id);
            if (dbShipment != null)
            {
                dbShipment.IsComplete = true;
                try
                {
                    _dbEntities.Entry(dbShipment).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                    return dbShipment;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public void DeleteShipment(long id)
        {
            var dbShipment = _dbEntities.ProjectOrderShipments.FirstOrDefault(i => i.ProjectOrderShipmentId == id);
            if (dbShipment != null)
            {
                _dbEntities.ProjectOrderShipments.Remove(dbShipment);
                _dbEntities.SaveChanges();
            }
        }

        public ProjectBabtModel GetProjectBabtModel(long id)
        {
            ProjcetBabt babt = GenereticRepo<ProjcetBabt>.GetById(_dbEntities, id);
            ProjectBabtModel model = GenericMapper<ProjcetBabt, ProjectBabtModel>.GetDestination(babt);
            return model;
        }

        public List<ProjectBabtModel> GetAllBabt()
        {
            var models = (from babt in _dbEntities.ProjcetBabts
                          join master in _dbEntities.ProjectMasters on babt.ProjectMasterId equals master.ProjectMasterId

                          join orderForm in _dbEntities.ProjectPurchaseOrderForms on babt.ProjectPurchaseOrderFormId equals
                              orderForm.ProjectPurchaseOrderFormId
                          join user in _dbEntities.CmnUsers on babt.PmAssignId equals user.CmnUserId
                          join babtRaw in _dbEntities.BabtRaws on master.ProjectName equals babtRaw.ProjectName

                              into temp
                          from tac in temp.DefaultIfEmpty()
                          where babt.TacNo == null
                          select new ProjectBabtModel
                          {
                              ProjectMasterId = master.ProjectMasterId,
                              OrderNo = master.OrderNuber + "",
                              ProjectBabtId = babt.ProjectBabtId,
                              ProjectName = master.ProjectName,
                              PurchaseOrderNumber = orderForm.PurchaseOrderNumber,
                              PoDate = orderForm.AddedDate,
                              ProjectManagerName = user.UserFullName,
                              PmImeiRangeRequestDate = babt.PmImeiRangeRequestDate,
                              RequestedImeiQuantity = babt.RequestedImeiQuantity ?? 0,
                              PurchaseOrderQuantity = orderForm.Quantity,
                              ProjectPurchaseOrderFormId = orderForm.ProjectPurchaseOrderFormId,
                              BabtRawId = tac.BabtRawId,
                              RemainingRawImei = tac.RemainingImei
                          }

                ).ToList();


            foreach (var model in models)
            {
                model.OrderNo = CommonConversion.AddOrdinal(Convert.ToInt32(model.OrderNo)) + " Order";
            }
            return models;
        }

        public bool UpdateBabtWithTac(ProjectBabtModel model)
        {
            ProjcetBabt dbBabt = _dbEntities.ProjcetBabts.OrderByDescending(i => i.ProjectBabtId)
                .FirstOrDefault(i => i.ProjectBabtId == model.ProjectBabtId);
            if (dbBabt != null)
            {
                dbBabt.TacNo = model.TacNo;
                dbBabt.ImeiRangeFrom = model.ImeiRangeFrom;
                dbBabt.ImeiRangeTo = model.ImeiRangeTo;
                dbBabt.RangeToPmDate = DateTime.Now;
                _dbEntities.Entry(dbBabt).State = EntityState.Modified;
            }
            try
            {

                _dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ProjectBtrcNocModel> GetBtrcNocRequestList()
        {
            var btrcNocList = (from noc in _dbEntities.ProjectBtrcNocs
                               join master in _dbEntities.ProjectMasters on noc.ProjectMasterId equals master.ProjectMasterId

                               join form in _dbEntities.ProjectPurchaseOrderForms on noc.ProjectPurchaseOrderFormId equals
                                   form.ProjectPurchaseOrderFormId

                               join user in _dbEntities.CmnUsers on noc.ProjectAssignId equals user.CmnUserId

                               where noc.IsDocUploaded == true && master.IsActive
                               select new ProjectBtrcNocModel
                               {
                                   ProjectBrtcNocId = noc.ProjectBtrcNocId,
                                   ProjectName = master.ProjectName,
                                   PoNo = form.PurchaseOrderNumber,
                                   ProjectManagerName = user.UserFullName,
                                   PurchaseOrderQuantity = form.Quantity
                               }
                ).ToList();
            return btrcNocList;
        }

        public List<ProjectBrtcNocModel> GetProjectsForBtrcNoc()
        {
            var masterList = (from noc in _dbEntities.ProjectBtrcNocs
                              join master in _dbEntities.ProjectMasters on noc.ProjectMasterId equals master.ProjectMasterId
                              join purchaseOrderForm in _dbEntities.ProjectPurchaseOrderForms on noc.ProjectPurchaseOrderFormId equals
                                  purchaseOrderForm.ProjectPurchaseOrderFormId
                              where noc.IsNocComplete == null || noc.IsNocComplete == false
                              select new ProjectBrtcNocModel
                              {
                                  ProjectBrtcNocId = noc.ProjectBtrcNocId,
                                  ProjectName = master.ProjectName + " - (" + purchaseOrderForm.PurchaseOrderNumber + ")"
                              }).ToList();
            return masterList;
        }

        public bool SaveBtrcNocs(VmBtrcNoc model)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                model.BtrcRawModel.Added = userId;
                model.BtrcRawModel.AddedDate = DateTime.Now;
                long btrcRawId = SaveBtrcRaw(model.BtrcRawModel);
                if (btrcRawId > 0)
                {
                    foreach (var customBtrcProjectModel in model.ProjectBtrcNocModel.CustomBtrcProjectModels)
                    {
                        var dbNoc =
                            _dbEntities.ProjectBtrcNocs.OrderByDescending(i => i.ProjectBtrcNocId)
                                .FirstOrDefault(i => i.ProjectBtrcNocId == customBtrcProjectModel.NocTableId);
                        if (dbNoc != null)
                        {
                            dbNoc.IsNocComplete = true;
                            dbNoc.Updated = userId;
                            dbNoc.UpdatedDate = DateTime.Now;
                            dbNoc.BtrcRawId = btrcRawId;
                            _dbEntities.Entry(dbNoc).State = EntityState.Modified;
                        }
                    }
                    //babt.Updated = userId;
                    //babt.UpdatedDate = DateTime.Now;
                    //babt.RemainingImei = babt.RemainingImei - model.BtrcRawModel.NocImeiQuantity;
                    //babt.RegisterableFrom = babt.RegisterableFrom + model.BtrcRawModel.NocImeiQuantity;
                    //_dbEntities.Entry(babt).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                    return true;
                }
                //var babt = _dbEntities.BabtRaws.FirstOrDefault(i => i.BabtRawId == model.BtrcRawModel.BabtRawId);
                //if (babt != null)
                //{
                //    var reg = babt.RegisterableFrom;
                //    if (babt.RemainingImei > model.BtrcRawModel.NocImeiQuantity)
                //    {


                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private long SaveBtrcRaw(BtrcRawModel btrcRawModel)
        {
            try
            {
                BtrcRaw btrc = GenericMapper<BtrcRawModel, BtrcRaw>.GetDestination(btrcRawModel);
                _dbEntities.Entry(btrc).State = EntityState.Added;
                _dbEntities.SaveChanges();
                return btrc.BtrcRawId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<ProjectMasterModel> GetAllProjects()
        {
            var allProjects =
                _dbEntities.ProjectMasters.Where(
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

        public List<ProjectMasterModel> GetAllProjectWithOrderNumber()
        {
            var allProjects =
                _dbEntities.ProjectMasters.Where(
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
                        OrderNuber = i.OrderNuber


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

        public ProjectMasterModel GetProjectMasterModel(long projectId)
        {
            if (projectId <= 0)
                return new ProjectMasterModel();
            ProjectMaster projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
            var config = new MapperConfiguration(c => c.CreateMap<ProjectMaster, ProjectMasterModel>());
            var map = config.CreateMapper();
            var project = map.Map<ProjectMasterModel>(projectMaster);
            var retValue = project;

            retValue.OrderNumberOrdinal = retValue.OrderNuber != null
                ? CommonConversion.AddOrdinal((int)retValue.OrderNuber) + " Order"
                : string.Empty;
            if (!string.IsNullOrWhiteSpace(retValue.OrderNumberOrdinal))
            {
                retValue.ProjectName = retValue.ProjectName + " (" + retValue.OrderNumberOrdinal + ")";
            }

            return retValue;
        }

        public ProjectMasterModel GetProjectMasterModelForPm(long projectId)
        {
            if (projectId <= 0)
                return new ProjectMasterModel();
            ProjectMaster projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
            var config = new MapperConfiguration(c => c.CreateMap<ProjectMaster, ProjectMasterModel>());
            var map = config.CreateMapper();
            var project = map.Map<ProjectMasterModel>(projectMaster);
            var retValue = project;


            var poCats =
                     _dbEntities.ProjectPurchaseOrderForms.Where(i => i.ProjectMasterId == projectId)
                         .Select(j => new ProjectPurchaseOrderFormModel
                         {
                             PoCategory = j.PoCategory
                         }).FirstOrDefault();

            retValue.OrderNumberOrdinal = retValue.OrderNuber != null
                ? CommonConversion.AddOrdinal((int)retValue.OrderNuber) + " Order"
                : string.Empty;
            if (!string.IsNullOrWhiteSpace(retValue.OrderNumberOrdinal))
            {
                if (poCats != null)
                    retValue.ProjectName = retValue.ProjectName + " (" + retValue.OrderNumberOrdinal + ")" + " (" + poCats.PoCategory + ")";
            }

            return retValue;
        }

        public PhAccessoryModel GetPhAccessoryModel(long phAccessoryId = 0, long projectId = 0)
        {
            var dbPhAccessory = new PhAccessory();
            if (phAccessoryId > 0)
            {
                dbPhAccessory = _dbEntities.PhAccessories.FirstOrDefault(i => i.PhAccessoriesId == phAccessoryId);
            }
            else if (projectId > 0)
            {
                dbPhAccessory = _dbEntities.PhAccessories.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhAccessory, PhAccessoryModel>());
            var mapper = config.CreateMapper();
            var accessory = mapper.Map<PhAccessoryModel>(dbPhAccessory);
            var returnValue = accessory;
            return returnValue;
        }

        public PhCamInfoModel GetPhCamInfoModel(long camInfoId = 0, long projectId = 0)
        {
            var dbPhCamInfo = new PhCamInfo();
            if (camInfoId > 0)
            {
                dbPhCamInfo = _dbEntities.PhCamInfos.FirstOrDefault(i => i.PhCamInfoId == camInfoId);
            }
            else if (projectId > 0)
            {
                dbPhCamInfo = _dbEntities.PhCamInfos.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhCamInfo, PhCamInfoModel>());
            var mapper = config.CreateMapper();
            var camInfoModel = mapper.Map<PhCamInfoModel>(dbPhCamInfo);
            var returnValue = camInfoModel;
            return returnValue;
        }

        public PhChipsetInfoModel GetPhChipsetInfoModel(long phChipsetInfoId = 0, long projectId = 0)
        {
            var phChipsetInfo = new PhChipsetInfo();
            if (phChipsetInfoId > 0)
            {
                phChipsetInfo = _dbEntities.PhChipsetInfos.FirstOrDefault(i => i.PhChipsetInfoId == phChipsetInfoId);
            }
            else if (projectId > 0)
            {
                phChipsetInfo = _dbEntities.PhChipsetInfos.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhChipsetInfo, PhChipsetInfoModel>());
            var mapper = config.CreateMapper();
            var chipsetInfoModel = mapper.Map<PhChipsetInfoModel>(phChipsetInfo);
            var returnValue = chipsetInfoModel;
            return returnValue;
        }

        public PhHousingInfoModel GetPhHousingInfoModel(long phHousingInfoId = 0, long projectId = 0)
        {
            var housingInfo = new PhHousingInfo();
            if (phHousingInfoId > 0)
            {
                housingInfo = _dbEntities.PhHousingInfos.FirstOrDefault(i => i.PhHousingInfoId == phHousingInfoId);
            }
            else if (projectId > 0)
            {
                housingInfo = _dbEntities.PhHousingInfos.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhHousingInfo, PhHousingInfoModel>());
            var mapper = config.CreateMapper();
            var phHousingInfoModel = mapper.Map<PhHousingInfoModel>(housingInfo);
            var returnValue = phHousingInfoModel;
            return returnValue;
        }

        public PhMemoryInfoModel GetPhMemoryInfoModel(long phMemoryInfoId = 0, long projectId = 0)
        {
            var memoryInfo = new PhMemoryInfo();
            if (phMemoryInfoId > 0)
            {
                memoryInfo = _dbEntities.PhMemoryInfos.FirstOrDefault(i => i.PhMemoryInfoId == phMemoryInfoId);
            }
            else if (projectId > 0)
            {
                memoryInfo = _dbEntities.PhMemoryInfos.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhMemoryInfo, PhMemoryInfoModel>());
            var mapper = config.CreateMapper();
            var phMemoryInfoModel = mapper.Map<PhMemoryInfoModel>(memoryInfo);
            var returnValue = phMemoryInfoModel;
            return returnValue;
        }

        public PhNetworkFreqAndBandModel GetPhNetworkFreqAndBandModel(long phNetworkFreqId = 0, long projectId = 0)
        {
            var networkFreqAndBand = new PhNetworkFreqAndBand();
            if (phNetworkFreqId > 0)
            {
                networkFreqAndBand =
                    _dbEntities.PhNetworkFreqAndBands.FirstOrDefault(i => i.PhNetworkFreqAndBandsId == phNetworkFreqId);
            }
            else if (projectId > 0)
            {
                networkFreqAndBand =
                    _dbEntities.PhNetworkFreqAndBands.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhNetworkFreqAndBand, PhNetworkFreqAndBandModel>());
            var mapper = config.CreateMapper();
            var phNetworkFreqAndBandModel = mapper.Map<PhNetworkFreqAndBandModel>(networkFreqAndBand);
            var returnValue = phNetworkFreqAndBandModel;
            return returnValue;
        }

        public PhPcbaInfoModel GetPhPcbaInfoModel(long phPcbaInfoId = 0, long projectId = 0)
        {
            var phPcbaInfo = new PhPcbaInfo();
            if (phPcbaInfoId > 0)
            {
                phPcbaInfo = _dbEntities.PhPcbaInfos.FirstOrDefault(i => i.PhPcbaInfoId == phPcbaInfoId);
            }
            else if (projectId > 0)
            {
                phPcbaInfo = _dbEntities.PhPcbaInfos.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhPcbaInfo, PhPcbaInfoModel>());
            var mapper = config.CreateMapper();
            var pcbaInfoModel = mapper.Map<PhPcbaInfoModel>(phPcbaInfo);
            var returnValue = pcbaInfoModel;
            return returnValue;
        }

        public PhSensorAndOtherModel GetPhSensorAndOtherModel(long phSensorId = 0, long projectId = 0)
        {
            var phSensorAndOther = new PhSensorAndOther();
            if (phSensorId > 0)
            {
                phSensorAndOther =
                    _dbEntities.PhSensorAndOthers.FirstOrDefault(i => i.PhSensorAndOthersInfoId == phSensorId);
            }
            else if (projectId > 0)
            {
                phSensorAndOther = _dbEntities.PhSensorAndOthers.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhSensorAndOther, PhSensorAndOtherModel>());
            var mapper = config.CreateMapper();
            var phSensorAndOtherModel = mapper.Map<PhSensorAndOtherModel>(phSensorAndOther);
            var returnValue = phSensorAndOtherModel;
            return returnValue;
        }

        public PhTpLcdInfoModel GetPhTpLcdInfoModel(long phTpLcdInfoId = 0, long projectId = 0)
        {
            var phTpLcdInfo = new PhTpLcdInfo();
            if (phTpLcdInfoId > 0)
            {
                phTpLcdInfo = _dbEntities.PhTpLcdInfos.FirstOrDefault(i => i.PhTpLcdInfoId == phTpLcdInfoId);
            }
            else if (projectId > 0)
            {
                phTpLcdInfo = _dbEntities.PhTpLcdInfos.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhTpLcdInfo, PhTpLcdInfoModel>());
            var mapper = config.CreateMapper();
            var phTpLcdInfoModel = mapper.Map<PhTpLcdInfoModel>(phTpLcdInfo);
            var returnValue = phTpLcdInfoModel;
            return returnValue;
        }

        public PhOperatingSyModel GetPhOperatingSyModel(long phOsId = 0, long projectId = 0)
        {
            var phOperatingSy = new PhOperatingSy();
            if (phOsId > 0)
            {
                phOperatingSy = _dbEntities.PhOperatingSys.FirstOrDefault(i => i.PhOsId == phOsId);
            }
            else if (projectId > 0)
            {
                phOperatingSy = _dbEntities.PhOperatingSys.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhOperatingSy, PhOperatingSyModel>());
            var mapper = config.CreateMapper();
            var phOperatingSyModel = mapper.Map<PhOperatingSyModel>(phOperatingSy);
            var returnValue = phOperatingSyModel;
            return returnValue;
        }

        public PhBatteryInfoModel GetPhBatteryInfoModel(long phBatteryInfoId = 0, long projectId = 0)
        {
            var phBatteryInfo = new PhBatteryInfo();
            if (phBatteryInfoId > 0)
            {
                phBatteryInfo = _dbEntities.PhBatteryInfoes.FirstOrDefault(i => i.PhBatteryInfoId == phBatteryInfoId);
            }
            else if (projectId > 0)
            {
                phBatteryInfo = _dbEntities.PhBatteryInfoes.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhBatteryInfo, PhBatteryInfoModel>());
            var mapper = config.CreateMapper();
            var phBatteryInfoModel = mapper.Map<PhBatteryInfoModel>(phBatteryInfo);
            var returnValue = phBatteryInfoModel;
            return returnValue;
        }

        public PhColorInfoModel GetPhColorInfoModel(long phColorInfoId = 0, long projectId = 0)
        {
            var phBatteryInfo = new PhColorInfo();
            if (phColorInfoId > 0)
            {
                phBatteryInfo = _dbEntities.PhColorInfoes.FirstOrDefault(i => i.PhColorInfoId == phColorInfoId);
            }
            else if (projectId > 0)
            {
                phBatteryInfo = _dbEntities.PhColorInfoes.FirstOrDefault(i => i.ProjectMasterId == projectId);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PhColorInfo, PhColorInfoModel>());
            var mapper = config.CreateMapper();
            var phColorInfoModel = mapper.Map<PhColorInfoModel>(phBatteryInfo);
            var returnValue = phColorInfoModel;
            return returnValue;
        }

        public ProjectCriticalControlPointModel GetProjectCriticalControlPointModel(
            long projectCriticalControlPointId = 0,
            long projectId = 0)
        {
            var projectCriticalControlPointModel = new ProjectCriticalControlPointModel();
            if (projectCriticalControlPointId > 0)
            {
                projectCriticalControlPointModel =
                    _dbEntities.ProjectCriticalControlPoints.Where(
                        i => i.ProjectCriticalControlPointId == projectCriticalControlPointId)
                        .Select(ccp => new ProjectCriticalControlPointModel
                        {
                            ProjectCriticalControlPointId = ccp.ProjectCriticalControlPointId,
                            ProjectMasterId = ccp.ProjectMasterId,
                            OtgCable = ccp.OtgCable,
                            BackCamera = ccp.BackCamera,
                            BackCoverFinishing = ccp.BackCoverFinishing,
                            BackCoverMaterial = ccp.BackCoverMaterial,
                            BackSideScreenPro = ccp.BackSideScreenPro,
                            BackSideThermalPaper = ccp.BackSideThermalPaper,
                            Battery = ccp.Battery,
                            BothSimFourG = ccp.BothSimFourG,
                            BsiSensor = ccp.BsiSensor,
                            Charger = ccp.Charger,
                            Earphone = ccp.Earphone,
                            Flash = ccp.Flash,
                            FlipCover = ccp.FlipCover,
                            FreeScreenProOnGb = ccp.FreeScreenProOnGb,
                            FronCamera = ccp.FronCamera,
                            LogoPrintType = ccp.LogoPrintType,
                            RawDesignFileOfId = ccp.RawDesignFileOfId,
                            SarAndCcc = ccp.SarAndCcc,
                            ThreeLayerScreenProOnPhone = ccp.ThreeLayerScreenProOnPhone,
                            UsbCableLength = ccp.UsbCableLength
                        }).FirstOrDefault();
            }
            else if (projectId > 0)
            {
                projectCriticalControlPointModel =
                    _dbEntities.ProjectCriticalControlPoints.Where(i => i.ProjectMasterId == projectId)
                        .Select(ccp => new ProjectCriticalControlPointModel
                        {
                            ProjectCriticalControlPointId = ccp.ProjectCriticalControlPointId,
                            ProjectMasterId = ccp.ProjectMasterId,
                            OtgCable = ccp.OtgCable,
                            BackCamera = ccp.BackCamera,
                            BackCoverFinishing = ccp.BackCoverFinishing,
                            BackCoverMaterial = ccp.BackCoverMaterial,
                            BackSideScreenPro = ccp.BackSideScreenPro,
                            BackSideThermalPaper = ccp.BackSideThermalPaper,
                            Battery = ccp.Battery,
                            BothSimFourG = ccp.BothSimFourG,
                            BsiSensor = ccp.BsiSensor,
                            Charger = ccp.Charger,
                            Earphone = ccp.Earphone,
                            Flash = ccp.Flash,
                            FlipCover = ccp.FlipCover,
                            FreeScreenProOnGb = ccp.FreeScreenProOnGb,
                            FronCamera = ccp.FronCamera,
                            LogoPrintType = ccp.LogoPrintType,
                            RawDesignFileOfId = ccp.RawDesignFileOfId,
                            SarAndCcc = ccp.SarAndCcc,
                            ThreeLayerScreenProOnPhone = ccp.ThreeLayerScreenProOnPhone,
                            UsbCableLength = ccp.UsbCableLength
                        }).FirstOrDefault();
            }
            return projectCriticalControlPointModel;
        }

        public ProjectProformaInvoiceModel GetProjectProformaInvoiceModel(long projectProformaInvoiceId = 0,
            long projectId = 0)
        {
            var projectProformaInvoiceModel = new ProjectProformaInvoiceModel();
            var manager = new FileManager();
            if (projectProformaInvoiceId > 0)
            {
                projectProformaInvoiceModel =
                    _dbEntities.ProjectProformaInvoices.Where(
                        i => i.ProjectProformaInvoiceId == projectProformaInvoiceId)
                        .Select(model => new ProjectProformaInvoiceModel
                        {
                            ProjectProformaInvoiceId = model.ProjectProformaInvoiceId,
                            ProjectMasterId = model.ProjectMasterId,
                            PiNo = model.PiNo,
                            PiDate = model.PiDate,
                            FilePath = model.FilePath
                        })
                        .FirstOrDefault();
            }
            else if (projectId > 0)
            {
                projectProformaInvoiceModel =
                    _dbEntities.ProjectProformaInvoices.Where(i => i.ProjectMasterId == projectId)
                        .Select(model => new ProjectProformaInvoiceModel
                        {
                            ProjectProformaInvoiceId = model.ProjectProformaInvoiceId,
                            ProjectMasterId = model.ProjectMasterId,
                            PiNo = model.PiNo,
                            PiDate = model.PiDate,
                            FilePath = model.FilePath
                        })
                        .FirstOrDefault();

            }
            if (projectProformaInvoiceModel != null)
            {
                var filePath = manager.GetFile(projectProformaInvoiceModel.FilePath);
                projectProformaInvoiceModel.FilePath = filePath;
                projectProformaInvoiceModel.FileExtension = manager.GetExtension(filePath);
            }
            return projectProformaInvoiceModel;
        }

        public ProjectOrderModel GetProjectOrderModel(long projectOrderId = 0, long projectId = 0)
        {
            var projectOrderModel = new ProjectOrderModel();
            if (projectOrderId > 0)
            {
                projectOrderModel =
                    _dbEntities.ProjectOrders.Where(i => i.ProjectOrderId == projectOrderId)
                        .Select(model => new ProjectOrderModel
                        {
                            ProjectOrderId = model.ProjectOrderId,
                            ProjectMasterId = model.ProjectMasterId,
                            OrderQuantity = model.OrderQuantity,
                            PoDate = model.PoDate,
                            PoNo = model.PoNo
                        })
                        .FirstOrDefault();
            }
            else if (projectId > 0)
            {
                projectOrderModel =
                    _dbEntities.ProjectOrders.Where(i => i.ProjectMasterId == projectId)
                        .Select(model => new ProjectOrderModel
                        {
                            ProjectOrderId = model.ProjectOrderId,
                            ProjectMasterId = model.ProjectMasterId,
                            OrderQuantity = model.OrderQuantity,
                            PoDate = model.PoDate,
                            PoNo = model.PoNo
                        })
                        .FirstOrDefault();
            }
            return projectOrderModel;
        }

        public ProjectPriceModel GetProjectPrice(long priceId)
        {
            var model = new ProjectPriceModel();
            if (priceId > 0)
            {
                model =
                    _dbEntities.ProjectPrices.Where(i => i.ProjectPriceId == priceId)
                        .Select(projectPrice => new ProjectPriceModel
                        {
                            ProjectPriceId = projectPrice.ProjectPriceId,
                            ProjectMasterId = projectPrice.ProjectMasterId,
                            Price = projectPrice.Price,
                            PriceDate = projectPrice.PriceDate,
                            PriceStage = projectPrice.PriceStage
                        })
                        .FirstOrDefault();
            }
            return model;
        }

        public ProjectOrderShipmentModel GetProjectOrderShipment(long projectOrderShipmentId = 0, long projectId = 0)
        {
            var orderShipment = new ProjectOrderShipment();
            if (projectOrderShipmentId > 0)
            {
                orderShipment = GenereticRepo<ProjectOrderShipment>.GetById(_dbEntities,
                    projectOrderShipmentId);
            }
            else if (projectId > 0)
            {
                orderShipment = GenereticRepo<ProjectOrderShipment>.Get(_dbEntities,
                    shipment => shipment.ProjectMasterId == projectId);
            }
            ProjectOrderShipmentModel model =
                GenericMapper<ProjectOrderShipment, ProjectOrderShipmentModel>.GetDestination(orderShipment);
            return model;
        }

        public ProjectBtrcNocModel GetProjectBtrcNoc(long projectBtrcNocId = 0, long projectId = 0)
        {
            //    if (projectBtrcNocId > 0)
            //    {
            //        var dbData = _dbEntities.ProjcetBtrcNocs.Where(i => i.ProjectBtrcNocId == projectBtrcNocId)
            //                .Select(noc => new ProjectBtrcNocModel
            //                {
            //                    ProjectBtrcNocId = noc.ProjectBtrcNocId,
            //                    ProjectMasterId = noc.ProjectMasterId,
            //                    FilePath = noc.FilePath
            //                }).FirstOrDefault();
            //        return dbData;
            //    }
            //    if (projectId > 0)
            //    {
            //        var dbData = _dbEntities.ProjcetBtrcNocs.Where(i => i.ProjectMasterId == projectId)
            //            .Select(noc => new ProjectBtrcNocModel
            //            {
            //                ProjectBtrcNocId = noc.ProjectBtrcNocId,
            //                ProjectMasterId = noc.ProjectMasterId,
            //                FilePath = noc.FilePath
            //            }).FirstOrDefault();
            //        return dbData;
            //    }
            return new ProjectBtrcNocModel();
        }

        #endregion

        #region Save Methods

        public long SaveProjectLc(ProjectLcModel model, long userId)
        {
            model.IsComplete = false;
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;
            ProjectLc lc = GenericMapper<ProjectLcModel, ProjectLc>.GetDestination(model);
            try
            {
                var result = GenereticRepo<ProjectLc>.Add(_dbEntities, lc, 0);
                return result.ProjectLcId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string SaveOpeningProject(ProjectMasterModel projectMasterModel, long userId)
        {
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            try
            {
                projectMasterModel.Updated = userId;
                projectMasterModel.UpdatedDate = DateTime.Now;
                projectMasterModel.Added = userId;
                projectMasterModel.AddedDate = DateTime.Now;
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectModel, ProjectMaster>());
                var mapper = config.CreateMapper();
                var projectMaster = mapper.Map<ProjectMaster>(projectMasterModel);
                var value = projectMaster;
                value.IsActive = true;
                _dbEntities.ProjectMasters.Add(value);
                _dbEntities.SaveChanges();
                var usrInfo = user != null ? "<br/>Project Created By: " + user.UserFullName : "";
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
                var body =
                    string.Format(
                        @"
inform you that, A new project has been created in Walton Project Management System By Commercial section.<br/><br/><b>Project Name: " +
                        projectMasterModel.ProjectName + "</b>" + usrInfo + time);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(new List<string>(new[] { "CM" }),
                    new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS" }), "NEW PROJECT( " + projectMasterModel.ProjectName + " )", body);
                return "Successfully Saved Project";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public long SavePrice(ProjectPriceModel model, long userId)
        {
            var projectPrice = new ProjectPrice
            {
                Price = model.Price,
                PriceDate = DateTime.Now,
                PriceStage = "OpeningPrice",
                ProjectMasterId = model.ProjectMasterId,
                Added = userId,
                AddedDate = DateTime.Now,
                Updated = userId,
                UpdatedDate = DateTime.Now
            };
            _dbEntities.ProjectPrices.Add(projectPrice);
            _dbEntities.SaveChanges();
            return projectPrice.ProjectPriceId;
        }

        public long SavePhPcbaInfo(PhPcbaInfoModel model, long userId)
        {
            model.Added = userId;
            model.Updated = userId;
            model.AddedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;
            var config = new MapperConfiguration(c => c.CreateMap<PhPcbaInfoModel, PhPcbaInfo>());
            var mapper = config.CreateMapper();
            var phPcbaInfo = mapper.Map<PhPcbaInfo>(model);
            _dbEntities.PhPcbaInfos.Add(phPcbaInfo);
            _dbEntities.SaveChanges();
            return phPcbaInfo.PhPcbaInfoId;
        }

        public long SavePhAccessory(PhAccessoryModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhAccessoryModel, PhAccessory>());
            var mapper = config.CreateMapper();
            var phAccessory = mapper.Map<PhAccessory>(model);
            _dbEntities.PhAccessories.Add(phAccessory);
            _dbEntities.SaveChanges();

            return phAccessory.PhAccessoriesId;
        }

        public long SavePhCamInfo(PhCamInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhCamInfoModel, PhCamInfo>());
            var mapper = config.CreateMapper();
            var phCamInfo = mapper.Map<PhCamInfo>(model);
            _dbEntities.PhCamInfos.Add(phCamInfo);
            _dbEntities.SaveChanges();
            return phCamInfo.PhCamInfoId;
        }

        public long SavePhChipsetInfo(PhChipsetInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhChipsetInfoModel, PhChipsetInfo>());
            var mapper = config.CreateMapper();
            var phChipsetInfo = mapper.Map<PhChipsetInfo>(model);
            _dbEntities.PhChipsetInfos.Add(phChipsetInfo);
            _dbEntities.SaveChanges();
            return phChipsetInfo.PhChipsetInfoId;
        }

        public long SavePhHousingInfo(PhHousingInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;
            var config = new MapperConfiguration(c => c.CreateMap<PhHousingInfoModel, PhHousingInfo>());
            var mapper = config.CreateMapper();
            var phHousingInfo = mapper.Map<PhHousingInfo>(model);
            _dbEntities.PhHousingInfos.Add(phHousingInfo);
            _dbEntities.SaveChanges();
            return phHousingInfo.PhHousingInfoId;
        }

        public long SavePhMemoryInfo(PhMemoryInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhMemoryInfoModel, PhMemoryInfo>());
            var mapper = config.CreateMapper();
            var phMemoryInfo = mapper.Map<PhMemoryInfo>(model);
            _dbEntities.PhMemoryInfos.Add(phMemoryInfo);
            _dbEntities.SaveChanges();
            return phMemoryInfo.PhMemoryInfoId;
        }

        public long SavePhNetworkFreqAndBand(PhNetworkFreqAndBandModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;
            var config = new MapperConfiguration(c => c.CreateMap<PhNetworkFreqAndBandModel, PhNetworkFreqAndBand>());
            var mapper = config.CreateMapper();
            var phNetworkFreqAndBand = mapper.Map<PhNetworkFreqAndBand>(model);
            _dbEntities.PhNetworkFreqAndBands.Add(phNetworkFreqAndBand);
            _dbEntities.SaveChanges();
            return phNetworkFreqAndBand.PhNetworkFreqAndBandsId;
        }

        public long SavePhSensorAndOther(PhSensorAndOtherModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhSensorAndOtherModel, PhSensorAndOther>());
            var mapper = config.CreateMapper();
            var phSensorAndOther = mapper.Map<PhSensorAndOther>(model);
            _dbEntities.PhSensorAndOthers.Add(phSensorAndOther);
            _dbEntities.SaveChanges();
            return phSensorAndOther.PhSensorAndOthersInfoId;
        }

        public long SavePhOperatingSyModel(PhOperatingSyModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhOperatingSyModel, PhOperatingSy>());
            var mapper = config.CreateMapper();
            var phOperatingSy = mapper.Map<PhOperatingSy>(model);
            _dbEntities.PhOperatingSys.Add(phOperatingSy);
            _dbEntities.SaveChanges();
            return phOperatingSy.PhOsId;
        }

        public long SavePhBatteryInfoModel(PhBatteryInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhBatteryInfoModel, PhBatteryInfo>());
            var mapper = config.CreateMapper();
            var phBatteryInfo = mapper.Map<PhBatteryInfo>(model);
            _dbEntities.PhBatteryInfoes.Add(phBatteryInfo);
            _dbEntities.SaveChanges();
            return phBatteryInfo.PhBatteryInfoId;
        }

        public long SavePhColorInfoModel(PhColorInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<PhColorInfoModel, PhColorInfo>());
            var mapper = config.CreateMapper();
            var phColorInfo = mapper.Map<PhColorInfo>(model);
            _dbEntities.PhColorInfoes.Add(phColorInfo);
            _dbEntities.SaveChanges();
            return phColorInfo.PhColorInfoId;
        }

        public long SavePhTpLcdInfo(PhTpLcdInfoModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;
            var config = new MapperConfiguration(c => c.CreateMap<PhTpLcdInfoModel, PhTpLcdInfo>());
            var mapper = config.CreateMapper();
            var phTpLcdInfo = mapper.Map<PhTpLcdInfo>(model);
            _dbEntities.PhTpLcdInfos.Add(phTpLcdInfo);
            _dbEntities.SaveChanges();
            return phTpLcdInfo.PhTpLcdInfoId;
        }

        public long SaveCriticalControlPoint(ProjectCriticalControlPointModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config =
                new MapperConfiguration(
                    c => c.CreateMap<ProjectCriticalControlPointModel, ProjectCriticalControlPoint>());
            var mapper = config.CreateMapper();
            var projectCriticalControlPoint = mapper.Map<ProjectCriticalControlPoint>(model);
            _dbEntities.ProjectCriticalControlPoints.Add(projectCriticalControlPoint);
            _dbEntities.SaveChanges();
            var id = projectCriticalControlPoint.ProjectCriticalControlPointId;
            return id;
        }

        public long SaveProjectProformaInvoice(ProjectProformaInvoiceModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<ProjectProformaInvoiceModel, ProjectProformaInvoice>());
            var mapper = config.CreateMapper();
            var projectProformaInvoice = mapper.Map<ProjectProformaInvoice>(model);
            _dbEntities.ProjectProformaInvoices.Add(projectProformaInvoice);
            _dbEntities.SaveChanges();
            var id = projectProformaInvoice.ProjectProformaInvoiceId;
            return id;
        }

        public long SaveProjectOrder(ProjectOrderModel model, long userId)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.Updated = userId;
            model.UpdatedDate = DateTime.Now;

            var config = new MapperConfiguration(c => c.CreateMap<ProjectOrderModel, ProjectOrder>());
            var mapper = config.CreateMapper();
            var projectOrder = mapper.Map<ProjectOrder>(model);
            _dbEntities.ProjectOrders.Add(projectOrder);
            _dbEntities.SaveChanges();
            var id = projectOrder.ProjectOrderId;
            return id;
        }

        public long SaveProjectBtrcNoc(ProjectBtrcNocModel model, long userId)
        {
            //model.Added = userId;
            //model.AddedDate = DateTime.Now;
            //model.Updated = userId;
            //model.UpdatedDate = DateTime.Now;
            //var config = new MapperConfiguration(c => c.CreateMap<ProjectBtrcNocModel, ProjcetBtrcNoc>());
            //var mapper = config.CreateMapper();
            //var btrcNoc = mapper.Map<ProjcetBtrcNoc>(model);
            //_dbEntities.ProjcetBtrcNocs.Add(btrcNoc);
            //_dbEntities.SaveChanges();
            //var id = btrcNoc.ProjectBtrcNocId;

            //return id;
            return 0;
        }

        public long SaveProjectShipment(ProjectOrderShipmentModel model, long userId, List<ProjectMasterModel> issueList1)
        {
            model.Added = userId;
            model.AddedDate = DateTime.Now;
            model.IsComplete = false;

            //if (model.ChinaIqcFail==null)
            //{
            //    model.ChinaIqcFail = 0;
            //}

            ProjectOrderShipment projectOrderShipment =
                GenericMapper<ProjectOrderShipmentModel, ProjectOrderShipment>.GetDestination(model);

            try
            {
                var shipmentNo =
                    _dbEntities.ProjectOrderShipments.Count(
                        i =>
                            i.ProjectMasterId == projectOrderShipment.ProjectMasterId &&
                            i.ProjectPurchaseOrderFormId == projectOrderShipment.ProjectPurchaseOrderFormId);
                projectOrderShipment.PoWiseShipmentNumber = shipmentNo + 1;
                GenereticRepo<ProjectOrderShipment>.Add(_dbEntities, projectOrderShipment);


                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                var project = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == model.ProjectMasterId);
                var usrInfo = user != null ? "<br/>Shipment Created By: " + user.UserFullName : "";
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
                if (user != null && project != null)
                {
                    var body =
                        string.Format(
                            @"This is to inform you that, A new shipment has been created in Walton Project Management System By Commercial section.<br/><br/><b>Project Name: " +
                            project.ProjectName + "</b>" + usrInfo + time);
                    var mail = new MailSendFromPms();
                    var result = mail.SendMail(new List<string>(new[] { "CM" }),
                        new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS" }), "NEW SHIPMENT( " + project.ProjectName + " )",
                        body);
                }

                //--new Finish Good add--//

                long proOrderId = 0;
                proOrderId = projectOrderShipment.ProjectOrderShipmentId;

                var proOrderIds = proOrderId;

                if (issueList1 != null && proOrderIds != 0)
                {
                    foreach (var proMds in issueList1)
                    {
                        var pModels =
                            (from pm in _dbEntities.ProjectMasters
                             where pm.ProjectMasterId == proMds.ProjectMasterId
                             select pm).FirstOrDefault();

                        var finishGood = new ShipmentFinishGoodModel();
                        finishGood.ProjectMasterId = projectOrderShipment.ProjectMasterId;
                        finishGood.ProjectOrderShipmentId = proOrderIds;

                        finishGood.FinishGoodProjectMasterId = proMds.ProjectMasterId;
                        finishGood.FinishGoodModel = pModels.ProjectModel;
                        finishGood.FinishGoodModelOrderNumber = pModels.OrderNuber;
                        finishGood.ApproxFinishGoodManufactureQty = proMds.ApproxFinishGoodManufactureQty;
                        finishGood.Added = userId;
                        finishGood.AddedDate = DateTime.Now;
                        _dbEntities.ShipmentFinishGoodModels.Add(finishGood);
                        _dbEntities.SaveChanges();

                    }
                }
                //--end Finish Good---//

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long SaveProjectPurchaseOrderFormModel(VmProjectPurchaseOrder model)
        {
            int? orderno = 0;
            try
            {

                //model.ProjectMasterId = projectMaster.ProjectMasterId;


                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == model.ProjectPurchaseOrderFormModel.Added);
                //var projectMasterModel = GetProjectMasterModel(model.ProjectMasterId);
                ProjectPurchaseOrderForm orderForm = GenericMapper<ProjectPurchaseOrderFormModel, ProjectPurchaseOrderForm>.GetDestination(model.ProjectPurchaseOrderFormModel);
                var master = new ProjectMaster();

                #region data clone

                if (model.IsReorder)
                {
                    if (model.ApproximateFinishDateForReorder != null)
                    {
                        var projectMaster =
                            _dbEntities.ProjectMasters.Where(i => i.ProjectName == model.ProjectPurchaseOrderFormModel.ProjectName)
                                .OrderByDescending(i => i.ProjectMasterId)
                                .FirstOrDefault();

                        if (projectMaster != null)
                        {
                            orderno = projectMaster.OrderNuber == null ? 1 : projectMaster.OrderNuber + 1;
                            var order =
                                _dbEntities.ProjectPurchaseOrderForms.Any(
                                    i => i.ProjectMasterId == projectMaster.ProjectMasterId);
                            if (order)
                            {
                                //====Add project model by default for feture phone====
                                if (projectMaster.ProjectType == "Feature")
                                {
                                    var nameParts = projectMaster.ProjectName.Split(' ');
                                    if (nameParts.Length > 1)
                                    {
                                        var firstPart =
                                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameParts[0].ToLower());
                                        master.ProjectModel = firstPart + " " + nameParts[1];
                                    }
                                    else
                                    {
                                        master.ProjectModel = projectMaster.ProjectName;
                                    }
                                }
                                //----o----
                                master.ProjectName = projectMaster.ProjectName;
                                if (user != null) master.Added = user.CmnUserId;
                                master.AddedDate = DateTime.Now;
                                master.OrderNuber = orderno;
                                master.ApproxProjectFinishDate = model.ApproximateFinishDateForReorder;
                                master.ApproxProjectOrderDate = model.ApproximatePoDate;
                                master.ApproxShipmentDate = model.ApproximateShipmentDate;
                                master.IsScreenTestComplete = projectMaster.IsScreenTestComplete;
                                master.BackCamera = projectMaster.BackCamera;
                                master.Battery = projectMaster.Battery;
                                master.Chipset = projectMaster.Chipset;
                                master.DisplayName = projectMaster.DisplayName;
                                master.ProcessorClock = projectMaster.ProcessorClock;
                                master.DisplaySize = projectMaster.DisplaySize;
                                master.FrontCamera = projectMaster.FrontCamera;
                                master.IsActive = true;
                                master.NumberOfSample = projectMaster.NumberOfSample;
                                //master.IsApproved = projectMaster.IsApproved;
                                master.IsNew = projectMaster.IsNew;
                                //master.IsProjectManagerAssigned = projectMaster.IsProjectManagerAssigned;
                                //master.IsScreenTestComplete = projectMaster.IsScreenTestComplete;
                                master.OsName = projectMaster.OsName;
                                master.OsVersion = projectMaster.OsVersion;
                                master.ProcessorName = projectMaster.ProcessorName;
                                master.ManagentComment = projectMaster.ManagentComment;
                                master.ProjectStatus = projectMaster.ProjectStatus;
                                master.SupplierName = projectMaster.SupplierName;
                                master.SupplierModelName = projectMaster.SupplierModelName;
                                master.SupplierTrustLevel = projectMaster.SupplierTrustLevel;
                                master.ProjectType = projectMaster.ProjectType;
                                master.Ram = projectMaster.Ram;
                                master.Rom = projectMaster.Rom;
                                master.SimSlotNumber = projectMaster.SimSlotNumber;
                                master.SlotType = projectMaster.SlotType;
                                master.ApproximatePrice = model.ApproximatePrice;
                                master.BackCam = projectMaster.BackCam;
                                master.BackCamBsi = projectMaster.BackCamBsi;
                                master.BackCamSensor = projectMaster.BackCamSensor;
                                master.BateeryPossibleSupplierNames = projectMaster.BateeryPossibleSupplierNames;
                                master.BatteryCoverFinishingType = projectMaster.BatteryCoverFinishingType;
                                master.BatteryCoverLogoType = projectMaster.BatteryCoverLogoType;
                                master.BatteryRating = projectMaster.BatteryRating;
                                master.BatterySupplierName = projectMaster.BatterySupplierName;
                                master.BatteryType = projectMaster.BatteryType;
                                master.Cdma = projectMaster.Cdma;
                                master.ChargerRating = projectMaster.ChargerRating;
                                master.ChargerSupplierName = projectMaster.ChargerSupplierName;
                                master.ChipsetBit = projectMaster.ChipsetBit;
                                master.ChipsetCore = projectMaster.ChipsetCore;
                                master.ChipsetFrequency = projectMaster.ChipsetFrequency;
                                master.ChipsetName = projectMaster.ChipsetName;
                                master.Color = projectMaster.Color;
                                master.Compass = projectMaster.Compass;
                                master.CpuName = projectMaster.CpuName;
                                master.DisplayResulution = projectMaster.DisplayResulution;
                                master.DisplaySpeciality = projectMaster.DisplaySpeciality;
                                master.EarphoneConfirmPrice = projectMaster.EarphoneConfirmPrice;
                                master.EarphoneSupplierName = projectMaster.EarphoneSupplierName;
                                master.FinalPrice = model.FinalPrice;
                                master.FlashLight = projectMaster.FlashLight;
                                master.FourthGenFdd = projectMaster.FourthGenFdd;
                                master.FourthGenTdd = projectMaster.FourthGenTdd;
                                master.ProjectNameForScreening = projectMaster.ProjectNameForScreening;
                                master.FrontCam = projectMaster.FrontCam;
                                master.FrontCamBsi = projectMaster.FrontCamBsi;
                                master.FrontCamSensor = projectMaster.FrontCamSensor;
                                master.GivenSampleToScreening = projectMaster.GivenSampleToScreening;
                                master.Gps = projectMaster.Gps;
                                master.Gsensor = projectMaster.Gsensor;
                                master.Gyroscope = projectMaster.Gyroscope;
                                master.HallSensor = projectMaster.HallSensor;
                                master.HousingFinalVendorName = projectMaster.HousingFinalVendorName;
                                master.HousingVendorName = projectMaster.HousingVendorName;
                                master.IsReorder = true;
                                master.LcdFinalVendor = projectMaster.LcdFinalVendor;
                                master.LcdVendor = projectMaster.LcdVendor;
                                master.Lsensor = projectMaster.Lsensor;
                                master.MemoryBrandName = projectMaster.MemoryBrandName;
                                master.OrderQuantity = projectMaster.OrderQuantity;
                                master.Otg = projectMaster.Otg;
                                master.OtgCable = projectMaster.OtgCable;
                                master.PcbaFinalVendor = projectMaster.PcbaFinalVendor;
                                master.PcbaVendorName = projectMaster.PcbaVendorName;
                                master.ProjectTypeId = projectMaster.ProjectTypeId;
                                master.Psensor = projectMaster.Psensor;
                                //master.RevisedStatus = projectMaster.RevisedStatus;
                                master.ScreeningCommentFromCommercial = projectMaster.ScreeningCommentFromCommercial;
                                master.SecondGen = projectMaster.SecondGen;
                                master.SourcingType = !string.IsNullOrWhiteSpace(orderForm.PoCategory) ? orderForm.PoCategory : projectMaster.SourcingType;
                                master.SpecialSensor = projectMaster.SpecialSensor;
                                master.SupplierId = projectMaster.SupplierId;
                                master.ThirdGen = projectMaster.ThirdGen;
                                master.ThreeLayerScreenProtector = projectMaster.ThreeLayerScreenProtector;
                                master.TpFinalVendor = projectMaster.TpFinalVendor;
                                master.TpVendor = projectMaster.TpVendor;
                                master.Updated = projectMaster.Updated;
                                master.UpdatedDate = projectMaster.UpdatedDate;
                                master.OrderQuantity = model.ProjectPurchaseOrderFormModel.Quantity;
                                //===SWOT fields===
                                master.SwotAnalysisBy = projectMaster.SwotAnalysisBy;
                                master.SwotAnalysisDate = projectMaster.SwotAnalysisDate;
                                master.SwotOpportunityRemarks = projectMaster.SwotOpportunityRemarks;
                                //-----o------
                                //master.IsFinallyClosed = projectMaster.IsFinallyClosed;
                                _dbEntities.ProjectMasters.Add(master);
                                //_dbEntities.SaveChanges();
                                //orderForm.ProjectMasterId = master.ProjectMasterId;
                            }
                            else
                            {
                                return -2;
                            }

                        }
                    }
                    else
                    {
                        return -3;
                    }

                }
                else
                {
                    master =
                        _dbEntities.ProjectMasters.FirstOrDefault(
                            i => i.ProjectName == model.ProjectPurchaseOrderFormModel.ProjectName && i.IsActive);
                    if (master == null) return -1;
                    master.OrderQuantity = model.ProjectPurchaseOrderFormModel.Quantity;
                    bool hasOrder = _dbEntities.ProjectPurchaseOrderForms.Any(i => i.ProjectMasterId == master.ProjectMasterId);// this checks if an order exists and reorder checkbox checked or not
                    if (hasOrder) return -1;//  -1 means An order has been exist for this project, please check the Is Reorder checkbox for create a RE-ORDER
                    long pId = master.ProjectMasterId;
                    orderForm.ProjectMasterId = pId;
                    //_dbEntities.ProjectMasters.AddOrUpdate(projectMaster);//update ProjectMaster to insert 
                    //transactional data save

                }

                #endregion


                _dbEntities.ProjectPurchaseOrderForms.Add(orderForm);
                using (var transaction = _dbEntities.Database.BeginTransaction())
                {
                    try
                    {
                        _dbEntities.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return -4;
                    }
                }

                //var res = GenereticRepo<ProjectPurchaseOrderForm>.Add(_dbEntities, orderForm, 0).ProjectPurchaseOrderFormId;
                //===PO creation mail
                var usrInfo = user != null ? "<br/>Created By: " + user.UserFullName : "";
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
                var purchaseOrderNo = "<b>Purchase Order No. : </b>" + model.ProjectPurchaseOrderFormModel.PurchaseOrderNumber + "<br/>";
                var messageForPm = "<b>N.B.</b> For PM team - Please create relevant variant in variant creation panel for this project.<br/><br/>";
                var body =
                    string.Format(
                    @"This is to inform you that, A new purchase order has been created in Walton Project Management System By Commercial section.<br/><br/>" + (master.ProjectType == "Smart" ? messageForPm : "") + "<b>Project Name: " +
                        model.ProjectPurchaseOrderFormModel.ProjectName + "</b><br/>" + purchaseOrderNo + usrInfo + time);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(new List<string>(new[] { "CM" }),
                    new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS", "PM" }), "New Purchase Order ( " + model.ProjectPurchaseOrderFormModel.ProjectName + " )",
                    body);
                // ==Smt capacity warning mail
                if (model.SendSmtCapacityWarningMail)
                {
                    var subject = "ATTENTION!!! SMT capacity warning  ( " + model.ProjectPurchaseOrderFormModel.ProjectName + ", Order Quantity - " + model.ProjectPurchaseOrderFormModel.Quantity + ")";
                    body =
                    string.Format(
                        @"<span style='color:red;font-size:150%'>Attention!!!Attention!!!Attention!!!</span><br/>This is to inform you that, A new purchase order has been created in WPMS which will cross the SMT capacity.<br/><br/><b>Project Name: " +
                        model.ProjectPurchaseOrderFormModel.ProjectName + "</b><br/>" + purchaseOrderNo + usrInfo + time);
                    mail.SendMail(new List<string>(new[] { "CM" }),
                    new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS" }), subject,
                    body);

                    var podate = model.ProjectPurchaseOrderFormModel.PoDate ?? DateTime.MinValue;
                    var warehouseentryquantity =
                        WarehouseEntryQuantityThisMonth(podate,
                            model.ProjectPurchaseOrderFormModel.ProjectName);//running warehouse
                    var projectnameLower = model.ProjectPurchaseOrderFormModel.ProjectName.ToLower();
                    var addmonth = 0;
                    if (projectnameLower.Contains("primo"))
                    {
                        addmonth = 4;
                    }
                    else
                    {
                        addmonth = 3;
                    }
                    var warehouseentrymonth = podate.AddMonths(addmonth).ToString("MMM", CultureInfo.InvariantCulture);
                    var warehouseentryyear = podate.AddMonths(addmonth).ToString("yyyy", CultureInfo.InvariantCulture);
                    orderno = orderno == 0 ? 1 : orderno;
                    var smtExceed = new SmtCapacityExceedLog
                    {
                        SmtCapacityCrossedForModel = model.ProjectPurchaseOrderFormModel.ProjectName,
                        OrderNo = orderno,
                        RunningSmtQuantity = warehouseentryquantity,
                        Month = warehouseentrymonth,
                        Year = warehouseentryyear,
                        OrderQuantity = model.ProjectPurchaseOrderFormModel.Quantity.ToString(),
                        AddedDate = DateTime.Now,
                        PoDate = podate,
                        ProjectMasterId = model.ProjectPurchaseOrderFormModel.ProjectMasterId
                    };
                    _dbEntities.SmtCapacityExceedLogs.Add(smtExceed);
                    _dbEntities.SaveChanges();
                }
                //----o----
                //=====Save feature phone variant directly to order quantity details====
                if (master.ProjectType == "Feature")
                {

                    var orderQuantityDetails = new ProjectOrderQuantityDetail
                    {
                        ProjectMasterId = master.ProjectMasterId,
                        OrderQuantity = Convert.ToInt64(model.ProjectPurchaseOrderFormModel.Quantity),
                        RamVendor = master.RamVendor,
                        RomVendor = master.RomVendor,
                        IsActive = true,
                        ProjectModel = master.ProjectModel,
                        AddedBy = master.Added,
                        AddedDate = DateTime.Now
                    };
                    _dbEntities.ProjectOrderQuantityDetails.Add(orderQuantityDetails);
                    _dbEntities.SaveChanges();
                }
                //---o----
                return orderForm.ProjectPurchaseOrderFormId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool SaveProjectPurchaseOrderHandsetModel(List<ProjectPurchaseOrderHandsetModel> models)
        {
            List<ProjectPurchaseOrderHandset> orderHandsets =
                GenericMapper<ProjectPurchaseOrderHandsetModel, ProjectPurchaseOrderHandset>.GetDestinationList(models);
            try
            {
                GenereticRepo<ProjectPurchaseOrderHandset>.AddList(_dbEntities, orderHandsets);
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public bool SaveProjectPurchaseOrderConditionModel(List<ProjectPurchaseOrderConditionModel> models)
        {
            List<ProjectPurchaseOrderCondition> orderHandsets =
                GenericMapper<ProjectPurchaseOrderConditionModel, ProjectPurchaseOrderCondition>.GetDestinationList(
                    models);
            try
            {
                GenereticRepo<ProjectPurchaseOrderCondition>.AddList(_dbEntities, orderHandsets);
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public List<ProjectPurchaseOrderConditionModel> GetPredefinedPurhcaseOrderConditions()
        {
            var models = new List<ProjectPurchaseOrderConditionModel>();
            var model = new ProjectPurchaseOrderConditionModel { Statement = @"Payment Terms: 100% LC at Sight" };
            models.Add(model);
            model = new ProjectPurchaseOrderConditionModel { Statement = @"2% SWAP" };
            models.Add(model);
            model = new ProjectPurchaseOrderConditionModel { Statement = @"Shipment term: FCA Hong Kong" };
            models.Add(model);
            model = new ProjectPurchaseOrderConditionModel { Statement = @"Port Destination: Dhaka Airport, Bangladesh" };
            models.Add(model);
            model = new ProjectPurchaseOrderConditionModel { Statement = @"Partial shipment: Allowed" };
            models.Add(model);
            model = new ProjectPurchaseOrderConditionModel { Statement = @"Transshipment: Allowed" };
            models.Add(model);
            model = new ProjectPurchaseOrderConditionModel { Statement = @"Insurance: Arranged By WALTON" };
            models.Add(model);
            return models;
        }

        public List<ProjectPurchaseOrderFormModel> GetUnclosedPoList(long projectId = 0)
        {
            List<ProjectPurchaseOrderFormModel> formModels = (from orderForm in _dbEntities.ProjectPurchaseOrderForms
                                                              join projectMaster in _dbEntities.ProjectMasters on orderForm.ProjectMasterId equals
                                                                  projectMaster.ProjectMasterId
                                                              where
                                                                  //orderForm.IsCompleted == false &&
                                                               projectMaster.IsActive
                                                              orderby orderForm.AddedDate
                                                              select new ProjectPurchaseOrderFormModel
                                                              {
                                                                  ProjectPurchaseOrderFormId = orderForm.ProjectPurchaseOrderFormId,
                                                                  ProjectMasterId = projectMaster.ProjectMasterId,
                                                                  PoDate = orderForm.PoDate,
                                                                  PurchaseOrderNumber = orderForm.PurchaseOrderNumber,
                                                                  ProjectName = projectMaster.ProjectName,
                                                                  CompanyName = orderForm.CompanyName,
                                                                  DescriptionHeader = orderForm.DescriptionHeader,
                                                                  Added = orderForm.Added,
                                                                  AddedDate = orderForm.AddedDate,
                                                                  Color = orderForm.Color,
                                                                  CompanyAddress = orderForm.CompanyAddress,
                                                                  DescriptionBody = orderForm.DescriptionBody,
                                                                  PoCategory = orderForm.PoCategory,
                                                                  IsCompleted = orderForm.IsCompleted,
                                                                  Quantity = orderForm.Quantity,
                                                                  Receiver = orderForm.Receiver,
                                                                  Signature = orderForm.Signature,
                                                                  Subject = orderForm.Subject,
                                                                  Updated = orderForm.Updated,
                                                                  UpdatedDate = orderForm.UpdatedDate,
                                                                  Value = orderForm.Value,
                                                                  OrderNumber = (int)projectMaster.OrderNuber,
                                                                  RepeatOrderApproved = orderForm.RepeatOrderApproved,
                                                                  ApprovedDate = orderForm.ApprovedDate,
                                                                  ApprovedBy = orderForm.ApprovedBy
                                                              }).ToList();
            if (projectId > 0) formModels = formModels.Where(i => i.ProjectMasterId == projectId).ToList();
            if (formModels.Any())
            {
                foreach (var model in formModels)
                {
                    model.ProjectName = model.ProjectName + "  (" + CommonConversion.AddOrdinal(model.OrderNumber) +
                                        " Order)";
                }
            }
            return formModels;
        }

        public List<ProjectPurchaseOrderFormModel> GetAllPoList()
        {
            List<ProjectPurchaseOrderFormModel> formModels = (from orderForm in _dbEntities.ProjectPurchaseOrderForms
                                                              join projectMaster in _dbEntities.ProjectMasters on orderForm.ProjectMasterId equals
                                                                  projectMaster.ProjectMasterId
                                                              where projectMaster.IsActive
                                                              orderby orderForm.AddedDate
                                                              select new ProjectPurchaseOrderFormModel
                                                              {
                                                                  ProjectPurchaseOrderFormId = orderForm.ProjectPurchaseOrderFormId,
                                                                  ProjectMasterId = projectMaster.ProjectMasterId,
                                                                  PoDate = orderForm.PoDate,
                                                                  PurchaseOrderNumber = orderForm.PurchaseOrderNumber,
                                                                  ProjectName = projectMaster.ProjectName,
                                                                  CompanyName = orderForm.CompanyName,
                                                                  DescriptionHeader = orderForm.DescriptionHeader,
                                                                  Added = orderForm.Added,
                                                                  AddedDate = orderForm.AddedDate,
                                                                  Color = orderForm.Color,
                                                                  CompanyAddress = orderForm.CompanyAddress,
                                                                  DescriptionBody = orderForm.DescriptionBody,
                                                                  PoCategory = orderForm.PoCategory,
                                                                  IsCompleted = orderForm.IsCompleted,
                                                                  Quantity = orderForm.Quantity,
                                                                  Receiver = orderForm.Receiver,
                                                                  Signature = orderForm.Signature,
                                                                  Subject = orderForm.Subject,
                                                                  Updated = orderForm.Updated,
                                                                  UpdatedDate = orderForm.UpdatedDate,
                                                                  Value = orderForm.Value,
                                                                  OrderNumber = (int)projectMaster.OrderNuber,
                                                                  RepeatOrderApproved = orderForm.RepeatOrderApproved,
                                                                  ApprovedDate = orderForm.ApprovedDate,
                                                                  ApprovedBy = orderForm.ApprovedBy
                                                              }).ToList();
            if (formModels.Any())
            {
                foreach (var model in formModels)
                {
                    model.ProjectName = model.ProjectName + "  (" + CommonConversion.AddOrdinal(model.OrderNumber) +
                                        " Order)";
                }
            }
            return formModels;
        }

        public ProjectPurchaseOrderFormModel GetPurchaseOrderById(long id)
        {
            var orderForm = GenereticRepo<ProjectPurchaseOrderForm>.GetById(_dbEntities, id);
            var model = GenericMapper<ProjectPurchaseOrderForm, ProjectPurchaseOrderFormModel>.GetDestination(orderForm);
            if (model != null)
            {
                model.ProjectName = GenereticRepo<ProjectMaster>.GetById(_dbEntities, model.ProjectMasterId).ProjectName;
                model.OrderNumber = GenereticRepo<ProjectMaster>.GetById(_dbEntities, model.ProjectMasterId).OrderNuber;
            }
            return model;
        }

        public ProjectPurchaseOrderForm GetPurchaseOrderByIdAsNoTracking(long id)
        {
            var model =
                _dbEntities.ProjectPurchaseOrderForms.AsNoTracking().FirstOrDefault(x => x.ProjectPurchaseOrderFormId == id);
            return model;
        }

        public List<ProjectPurchaseOrderConditionModel> GetPurchaseOrderConditionsByOrder(long orderId)
        {
            var conditions = GenereticRepo<ProjectPurchaseOrderCondition>.GetList(_dbEntities,
                condition => condition.ProjectPurchaseOrderFormId == orderId);
            var models =
                GenericMapper<ProjectPurchaseOrderCondition, ProjectPurchaseOrderConditionModel>.GetDestinationList(
                    conditions);
            return models;
        }

        public void SaveProjectPurchaseOrderConditionLogs(long poId, long logAddedBy)
        {
            var oldConditions =
                _dbEntities.ProjectPurchaseOrderConditions.Where(x => x.ProjectPurchaseOrderFormId == poId).ToList();
            foreach (var o in oldConditions)
            {
                ProjectPurchaseOrderConditionLog m = GenericMapper<ProjectPurchaseOrderCondition, ProjectPurchaseOrderConditionLog>.GetDestination(o);
                m.LogAddedBy = logAddedBy;
                m.LogAddedDate = DateTime.Now;
                _dbEntities.ProjectPurchaseOrderConditionLogs.Add(m);
                _dbEntities.SaveChanges();
            }
        }

        public bool UpdateProjectPurchaseOrderFormModel(ProjectPurchaseOrderFormModel projectPurchaseOrderFormModel, DateTime? approximateFinishDate)
        {
            try
            {
                ProjectPurchaseOrderForm form =
                    GenericMapper<ProjectPurchaseOrderFormModel, ProjectPurchaseOrderForm>.GetDestination(
                        projectPurchaseOrderFormModel);
                GenereticRepo<ProjectPurchaseOrderForm>.Update(_dbEntities, form);

                using (var entities = new CellPhoneProjectEntities())
                {
                    var masterModel =
                        entities.ProjectMasters.FirstOrDefault(
                            i => i.ProjectMasterId == projectPurchaseOrderFormModel.ProjectMasterId);
                    if (masterModel != null)
                    {
                        masterModel.ApproxShipmentDate = projectPurchaseOrderFormModel.ApproxShipmentDate;
                        masterModel.ApproxProjectFinishDate = approximateFinishDate;
                        masterModel.SourcingType = projectPurchaseOrderFormModel.PoCategory;
                        masterModel.OrderQuantity = projectPurchaseOrderFormModel.Quantity;
                        entities.Entry(masterModel).State = EntityState.Modified;
                        entities.SaveChanges();
                        entities.Dispose();
                        //====if feature phone update order quantity in details table===
                        if (masterModel.ProjectType == "Feature")
                        {
                            var orQuDe =
                                _dbEntities.ProjectOrderQuantityDetails.FirstOrDefault(x => x.ProjectMasterId == masterModel.ProjectMasterId);
                            if (orQuDe != null)
                            {
                                orQuDe.OrderQuantity = projectPurchaseOrderFormModel.Quantity;
                                orQuDe.UpdatedBy = projectPurchaseOrderFormModel.Updated;
                                orQuDe.UpdatedDate = DateTime.Now;
                                _dbEntities.ProjectOrderQuantityDetails.AddOrUpdate(orQuDe);
                                _dbEntities.SaveChanges();
                            }
                        }
                        //----O----
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var v = ex;
                return false;
            }
        }

        public void SaveProjectPurchaseOrderFormLog(ProjectPurchaseOrderForm model, long logAddedBy)
        {
            ProjectPurchaseOrderFormLog m = GenericMapper<ProjectPurchaseOrderForm, ProjectPurchaseOrderFormLog>.GetDestination(model);
            m.LogAddedBy = logAddedBy;
            m.LogAddedDate = DateTime.Now;
            _dbEntities.ProjectPurchaseOrderFormLogs.Add(m);
            _dbEntities.SaveChanges();
        }
        public bool UpdateProjectPurchaseOrderConditionModel(long formId,
            List<ProjectPurchaseOrderConditionModel> projectPurchaseOrderConditionModels)
        {
            try
            {
                if (projectPurchaseOrderConditionModels.Any())
                {
                    var prevConditions =
                        _dbEntities.ProjectPurchaseOrderConditions.Where(i => i.ProjectPurchaseOrderFormId == formId)
                            .ToList();
                    _dbEntities.ProjectPurchaseOrderConditions.RemoveRange(prevConditions);
                    List<ProjectPurchaseOrderCondition> condition =
                        GenericMapper<ProjectPurchaseOrderConditionModel, ProjectPurchaseOrderCondition>
                            .GetDestinationList(projectPurchaseOrderConditionModels);
                    _dbEntities.ProjectPurchaseOrderConditions.AddRange(condition);
                    _dbEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveSupplier(SupplierModel model)
        {
            try
            {
                var config = new MapperConfiguration(c => c.CreateMap<SupplierModel, Supplier>());
                var mapper = config.CreateMapper();
                var supplier = mapper.Map<Supplier>(model);
                _dbEntities.Suppliers.Add(supplier);
                _dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public SupplierModel GetSupplier(long id)
        {
            SupplierModel model;
            try
            {
                Supplier supplier = GenereticRepo<Supplier>.GetById(_dbEntities, id);
                model = GenericMapper<Supplier, SupplierModel>.GetDestination(supplier);
            }
            catch (Exception)
            {
                return new SupplierModel();
            }
            return model;
        }

        public List<SupplierModel> GeTAllSuppliers()
        {
            try
            {
                List<Supplier> suppliers = GenereticRepo<Supplier>.GetList(_dbEntities);
                List<SupplierModel> models = GenericMapper<Supplier, SupplierModel>.GetDestinationList(suppliers);
                return models;
            }
            catch (Exception)
            {
                return new List<SupplierModel>();
            }
        }

        public bool UpdateSupplier(SupplierModel model, long userId)
        {
            try
            {
                Supplier supplier = GenereticRepo<Supplier>.GetById(_dbEntities, model.SupplierId);
                supplier.SupplierName = model.SupplierName;
                supplier.SupplierAddress = model.SupplierAddress;
                supplier.Email = model.Email;
                supplier.Phone = model.Phone;
                supplier.Updated = userId;
                supplier.UpdatedDate = DateTime.Now;
                supplier.EstablishmentDate = model.EstablishmentDate;
                supplier.HasCompanyIdh = model.HasCompanyIdh;
                _dbEntities.Entry(supplier).State = EntityState.Modified;
                _dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ProjectMasterModel> GetAllCreatedProjects()
        {
            List<ProjectMasterModel> projectMasterModels = (from projectMaster in _dbEntities.ProjectMasters
                                                            join cmnUser in _dbEntities.CmnUsers on projectMaster.Added equals cmnUser.CmnUserId
                                                                into temp
                                                            from user in temp.DefaultIfEmpty()
                                                            //where projectMaster.IsActive
                                                            select new ProjectMasterModel
                                                            {
                                                                ProjectMasterId = projectMaster.ProjectMasterId,
                                                                ProjectName = projectMaster.ProjectName,
                                                                Added = projectMaster.Added,
                                                                AddedDate = projectMaster.AddedDate,
                                                                ApproxProjectFinishDate = projectMaster.ApproxProjectFinishDate,
                                                                ApproxProjectOrderDate = projectMaster.ApproxProjectOrderDate,
                                                                ApproxShipmentDate = projectMaster.ApproxShipmentDate,
                                                                BackCamera = projectMaster.BackCamera,
                                                                Battery = projectMaster.Battery,
                                                                Chipset = projectMaster.Chipset,
                                                                DisplayName = projectMaster.DisplayName,
                                                                ProcessorClock = projectMaster.ProcessorClock,
                                                                DisplaySize = projectMaster.DisplaySize,
                                                                FrontCamera = projectMaster.FrontCamera,
                                                                IsActive = projectMaster.IsActive,
                                                                NumberOfSample = projectMaster.NumberOfSample,
                                                                IsApproved = projectMaster.IsApproved,
                                                                IsNew = projectMaster.IsNew,
                                                                IsProjectManagerAssigned = projectMaster.IsProjectManagerAssigned,
                                                                IsScreenTestComplete = projectMaster.IsScreenTestComplete,
                                                                OsName = projectMaster.OsName,
                                                                OsVersion = projectMaster.OsVersion,
                                                                ProcessorName = projectMaster.ProcessorName,
                                                                ManagentComment = projectMaster.ManagentComment,
                                                                ProjectStatus = projectMaster.ProjectStatus,
                                                                SupplierName = projectMaster.SupplierName,
                                                                SupplierModelName = projectMaster.SupplierModelName,
                                                                SupplierTrustLevel = projectMaster.SupplierTrustLevel,
                                                                ProjectType = projectMaster.ProjectType,
                                                                Ram = projectMaster.Ram,
                                                                Rom = projectMaster.Rom,
                                                                SimSlotNumber = projectMaster.SimSlotNumber,
                                                                SlotType = projectMaster.SlotType,
                                                                AddedName = user.UserFullName,
                                                                ApproximatePrice = projectMaster.ApproximatePrice,
                                                                BackCam = projectMaster.BackCam,
                                                                BackCamBsi = projectMaster.BackCamBsi,
                                                                BackCamSensor = projectMaster.BackCamSensor,
                                                                BateeryPossibleSupplierNames = projectMaster.BateeryPossibleSupplierNames,
                                                                BatteryCoverFinishingType = projectMaster.BatteryCoverFinishingType,
                                                                BatteryCoverLogoType = projectMaster.BatteryCoverLogoType,
                                                                BatteryRating = projectMaster.BatteryRating,
                                                                BatterySupplierName = projectMaster.BatterySupplierName,
                                                                BatteryType = projectMaster.BatteryType,
                                                                Cdma = projectMaster.Cdma,
                                                                ChargerRating = projectMaster.ChargerRating,
                                                                ChargerSupplierName = projectMaster.ChargerSupplierName,
                                                                ChipsetBit = projectMaster.ChipsetBit,
                                                                ChipsetCore = projectMaster.ChipsetCore,
                                                                ChipsetFrequency = projectMaster.ChipsetFrequency,
                                                                ChipsetName = projectMaster.ChipsetName,
                                                                Color = projectMaster.Color,
                                                                Compass = projectMaster.Compass,
                                                                CpuName = projectMaster.CpuName,
                                                                DisplayResulution = projectMaster.DisplayResulution,
                                                                DisplaySpeciality = projectMaster.DisplaySpeciality,
                                                                EarphoneConfirmPrice = projectMaster.EarphoneConfirmPrice,
                                                                EarphoneSupplierName = projectMaster.EarphoneSupplierName,
                                                                FinalPrice = projectMaster.FinalPrice,
                                                                FlashLight = projectMaster.FlashLight,
                                                                FourthGenFdd = projectMaster.FourthGenFdd,
                                                                FourthGenTdd = projectMaster.FourthGenTdd,
                                                                ProjectNameForScreening = projectMaster.ProjectNameForScreening,
                                                                FrontCam = projectMaster.FrontCam,
                                                                FrontCamBsi = projectMaster.FrontCamBsi,
                                                                FrontCamSensor = projectMaster.FrontCamSensor,
                                                                GivenSampleToScreening = projectMaster.GivenSampleToScreening,
                                                                Gps = projectMaster.Gps,
                                                                Date = null,
                                                                Gsensor = projectMaster.Gsensor,
                                                                Gyroscope = projectMaster.Gyroscope,
                                                                HallSensor = projectMaster.HallSensor,
                                                                HousingFinalVendorName = projectMaster.HousingFinalVendorName,
                                                                HousingVendorName = projectMaster.HousingVendorName,
                                                                IsReorder = projectMaster.IsReorder,
                                                                LcdFinalVendor = projectMaster.LcdFinalVendor,
                                                                LcdVendor = projectMaster.LcdVendor,
                                                                Lsensor = projectMaster.Lsensor,
                                                                MemoryBrandName = projectMaster.MemoryBrandName,
                                                                OrderQuantity = projectMaster.OrderQuantity,
                                                                Otg = projectMaster.Otg,
                                                                OtgCable = projectMaster.OtgCable,
                                                                PcbaFinalVendor = projectMaster.PcbaFinalVendor,
                                                                PcbaVendorName = projectMaster.PcbaVendorName,
                                                                ProjectTypeId = projectMaster.ProjectTypeId,
                                                                Psensor = projectMaster.Psensor,
                                                                RevisedStatus = projectMaster.RevisedStatus,
                                                                ScreeningCommentFromCommercial = projectMaster.ScreeningCommentFromCommercial,
                                                                SecondGen = projectMaster.SecondGen,
                                                                SourcingType = projectMaster.SourcingType,
                                                                SpecialSensor = projectMaster.SpecialSensor,
                                                                SupplierId = projectMaster.SupplierId,
                                                                ThirdGen = projectMaster.ThirdGen,
                                                                ThreeLayerScreenProtector = projectMaster.ThreeLayerScreenProtector,
                                                                TpFinalVendor = projectMaster.TpFinalVendor,
                                                                TpVendor = projectMaster.TpVendor,
                                                                Updated = projectMaster.Updated,
                                                                UpdatedDate = projectMaster.UpdatedDate,
                                                                OrderNuber = projectMaster.OrderNuber,
                                                                UpdatedName = _dbEntities.CmnUsers.Where(y => y.CmnUserId == projectMaster.Updated).Select(y => y.UserFullName).FirstOrDefault(),
                                                                SwotAnalysisBy = projectMaster.SwotAnalysisBy,
                                                                SwotAnalysisDate = projectMaster.SwotAnalysisDate,
                                                                SwotOpportunityRemarks = projectMaster.SwotOpportunityRemarks,
                                                                BrandId = projectMaster.BrandId,
                                                                ActivationBy = projectMaster.ActivationBy,
                                                                ActivationDate = projectMaster.ActivationDate,
                                                                DeactivatedBy = projectMaster.DeactivatedBy,
                                                                DeactivationDate = projectMaster.DeactivationDate,
                                                                ActivationDeactivationRemarks = projectMaster.ActivationDeactivationRemarks
                                                            }
                ).ToList();
            foreach (var model in projectMasterModels)
            {
                var ext = !string.IsNullOrWhiteSpace(model.SourcingType) ? " / " + model.SourcingType : "";
                if (model.OrderNuber != null)
                {
                    model.ProjectActualName = model.ProjectName;
                    model.ProjectName = model.ProjectName + " (" + CommonConversion.AddOrdinal((int)model.OrderNuber) +
                                        " Order)" + ext;
                }

            }
            return projectMasterModels;
        }

        public long SaveBabt(BabtRawModel model)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                model.TotalImei = 999999;
                model.RemainingImei = model.TotalImei;
                model.RegisterableFrom = 0;
                BabtRaw babt = GenericMapper<BabtRawModel, BabtRaw>.GetDestination(model);
                _dbEntities.BabtRaws.Add(babt);
                _dbEntities.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public BabtRawModel GetBabt(long id)
        {
            BabtRaw babt = GenereticRepo<BabtRaw>.GetById(_dbEntities, id);
            BabtRawModel model = GenericMapper<BabtRaw, BabtRawModel>.GetDestination(babt);
            return model;
        }

        public List<BabtRawModel> GetBabts()
        {
            var models = (from babtRaw in _dbEntities.BabtRaws
                          join master in _dbEntities.ProjectMasters on babtRaw.ProjectMasterId equals master.ProjectMasterId
                          join user in _dbEntities.CmnUsers on babtRaw.Added equals user.CmnUserId
                          select new BabtRawModel
                          {
                              ProjectMasterId = babtRaw.ProjectMasterId,
                              ProjectName = master.ProjectName,
                              Added = babtRaw.Added,
                              AddedDate = babtRaw.AddedDate,
                              BabtRawId = babtRaw.BabtRawId,
                              AddedBy = user.UserFullName,
                              TacNo = babtRaw.TacNo,
                              ReceiveDate = babtRaw.ReceiveDate,
                              RegisterableFrom = babtRaw.RegisterableFrom,
                              RemainingImei = babtRaw.RemainingImei,
                              RequestDate = babtRaw.RequestDate,
                              TotalImei = babtRaw.TotalImei,
                              Updated = babtRaw.Updated,
                              UpdatedDate = babtRaw.UpdatedDate
                          }).ToList();
            return models;
        }

        public List<VmCompletedNoc> GetCompletedNocs()
        {
            List<VmCompletedNoc> list = (from projectBtrcNoc in _dbEntities.ProjectBtrcNocs
                                         join btrcRaw in _dbEntities.BtrcRaws on projectBtrcNoc.BtrcRawId equals btrcRaw.BtrcRawId
                                         join projectMaster in _dbEntities.ProjectMasters on projectBtrcNoc.ProjectMasterId equals
                                             projectMaster.ProjectMasterId


                                         select new VmCompletedNoc
                                         {
                                             ProjectMasterId = projectBtrcNoc.ProjectMasterId,
                                             BtrcRawId = btrcRaw.BtrcRawId,
                                             PurchaseOrderId = projectBtrcNoc.ProjectPurchaseOrderFormId,
                                             ProjectName = projectMaster.ProjectName,
                                             PurchaseOrderQuantity = 0,
                                             IsNocComplete = projectBtrcNoc.IsNocComplete,
                                             NocNo = btrcRaw.NocNo,
                                             ProjectBtrcNocId = projectBtrcNoc.ProjectBtrcNocId
                                         }).ToList();
            return list;
        }

        public VmImeiRange GetCustomImeiRange(long projectId, long orderId, long quantity)
        {
            var range = new VmImeiRange();
            var projectInfo = (from purchaseOrderForm in _dbEntities.ProjectPurchaseOrderForms
                               join master in _dbEntities.ProjectMasters on purchaseOrderForm.ProjectMasterId equals
                                   master.ProjectMasterId

                               where purchaseOrderForm.ProjectPurchaseOrderFormId == orderId
                               select new
                               {
                                   purchaseOrderForm.ProjectPurchaseOrderFormId,
                                   purchaseOrderForm.Quantity,
                                   master.ProjectMasterId,
                                   master.ProjectName
                               }).FirstOrDefault();
            if (projectInfo != null)
            {
                range.ProjectMasterId = projectInfo.ProjectMasterId;
                range.PurchaseOrderQuantiy = projectInfo.Quantity;
                range.ProjectMasterId = projectInfo.ProjectMasterId;
                range.ProjectName = projectInfo.ProjectName;
                range.PurchaseOrderFormId = projectInfo.ProjectPurchaseOrderFormId;
            }
            var babtInfo = (from babtRaw in _dbEntities.BabtRaws
                            where babtRaw.ProjectName == projectInfo.ProjectName
                            select new
                            {
                                babtRaw.BabtRawId,
                                babtRaw.TacNo,
                                babtRaw.RegisterableFrom,
                                babtRaw.RemainingImei
                            }).FirstOrDefault();
            if (babtInfo != null)
            {
                range.RemainingQuantity = babtInfo.RemainingImei;
                range.AllocatedFrom = babtInfo.RegisterableFrom;
                range.BabtRawId = babtInfo.BabtRawId;
                range.TacNo = babtInfo.TacNo;
                //range.SampleStartImei = range.TacNo + range.AllocatedFrom.ToString().PadLeft(6, '0')+"X";
                range.RequestedQuantity = quantity;
                //range.SampleEndImei = range.TacNo + (range.AllocatedFrom + quantity-1).ToString().PadLeft(6, '0')+"X";
            }
            range.GivenQuantity = (long)(range.PurchaseOrderQuantiy * 2);
            return range;
        }

        public long UpdateBabt(BabtRawModel model)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                var dbBabtRaw = GenereticRepo<BabtRaw>.GetById(_dbEntities, model.BabtRawId);
                if (dbBabtRaw != null)
                {
                    dbBabtRaw.TacNo = model.TacNo;
                    dbBabtRaw.ProjectMasterId = model.ProjectMasterId;
                    dbBabtRaw.RequestDate = model.RequestDate;
                    dbBabtRaw.ReceiveDate = model.ReceiveDate;
                    dbBabtRaw.Updated = userId;
                    dbBabtRaw.UpdatedDate = DateTime.Now;
                    _dbEntities.Entry(dbBabtRaw).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                }
                return model.BabtRawId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool SaveImeiRange(VmImeiRange model)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                var dbProjectBabt =
                    _dbEntities.ProjcetBabts.FirstOrDefault(
                        i =>
                            i.ProjectMasterId == model.ProjectMasterId &&
                            i.ProjectPurchaseOrderFormId == model.PurchaseOrderFormId);
                var dbBabt = _dbEntities.BabtRaws.FirstOrDefault(i => i.BabtRawId == model.BabtRawId);
                if (dbProjectBabt != null && dbBabt != null)
                {
                    dbProjectBabt.BabtRawId = model.BabtRawId;
                    dbProjectBabt.TacNo = model.TacNo;
                    dbProjectBabt.ImeiRangeFrom = model.SampleStartImei;
                    dbProjectBabt.ImeiRangeTo = model.SampleEndImei;
                    dbProjectBabt.RangeToPmDate = DateTime.Now;
                    dbProjectBabt.Updated = userId;
                    dbProjectBabt.UpdatedDate = DateTime.Now;
                    dbProjectBabt.GivenQuantityFromCommercial = model.GivenQuantity;
                    _dbEntities.Entry(dbProjectBabt).State = EntityState.Modified;


                    dbBabt.RemainingImei = dbBabt.RemainingImei - model.GivenQuantity;
                    dbBabt.RegisterableFrom = dbBabt.RegisterableFrom + model.GivenQuantity;
                    dbBabt.Updated = userId;
                    dbBabt.UpdatedDate = DateTime.Now;
                    _dbEntities.Entry(dbBabt).State = EntityState.Modified;

                    _dbEntities.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ProjectMasterModel> GetProjectBySupplierId(long supplierId = 0)
        {
            List<ProjectMaster> masters = GenereticRepo<ProjectMaster>.GetList(_dbEntities,
                master => master.ProjectStatus != "REJECTED");
            if (supplierId > 0) masters = masters.Where(i => i.SupplierId == supplierId).ToList();
            List<ProjectMasterModel> models =
                GenericMapper<ProjectMaster, ProjectMasterModel>.GetDestinationList(masters);

            foreach (var model in models)
            {
                model.ProjectName = model.ProjectName + " -->> (" + CommonConversion.AddOrdinal(model.OrderNuber) +
                                    " Order)";
            }


            return models;
        }

        public long ScreeningRequest(long projectId, long quantity, string sampleType, string remarks)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                var projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
                if (userId <= 0 || projectMaster == null) return 0;
                projectMaster.GivenSampleToScreening = quantity;
                projectMaster.ProjectStatus = "PARTIAL";
                projectMaster.Updated = userId;
                projectMaster.UpdatedDate = DateTime.Now;
                _dbEntities.ProjectMasters.AddOrUpdate(projectMaster);

                var assign = new HwQcInchargeAssign
                {
                    ProjectMasterId = projectId,
                    IsScreeningTest = true,
                    IsRunningTest = false,
                    IsFinishedGoodTest = false,
                    TestPhase = "SAMPLESENT",
                    HwQcInchargeAssignDate = DateTime.Now,
                    SentSampleQuantity = quantity,
                    Added = userId,
                    AddedDate = DateTime.Now,
                    Updated = userId,
                    UpdatedDate = DateTime.Now,
                    SampleSetSentDate = DateTime.Now,
                    ProjectManagerSampleType = sampleType,
                    ProjectManagerAssignComment = remarks
                };
                _dbEntities.HwQcInchargeAssigns.Add(assign);
                _dbEntities.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<string> GetCpuCores()
        {
            List<string> cpuList = _dbEntities.CpuCores.Select(i => i.CpuCoreName).ToList();
            return cpuList;
        }

        public string CheckProjectName(string projectName)
        {
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                if (_dbEntities.ProjectMasters.Any(i => i.ProjectName == projectName)) return "Project already exist";
            }
            return "";
        }

        public ProjectMasterModel GetProjectModel(long? projectId)
        {
            if (projectId <= 0)
                return new ProjectMasterModel();
            ProjectMaster projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
            List<AccessoriesPrice> accessoriesprices = _dbEntities.AccessoriesPrices.Where(x => x.ProjectMasterId == projectId).ToList();
            List<ProjectImage> projectImages = _dbEntities.ProjectImages.Where(x => x.ProjectId == projectId).ToList();
            var config = new MapperConfiguration(c => c.CreateMap<ProjectMaster, ProjectMasterModel>());
            var map = config.CreateMapper();
            var project = map.Map<ProjectMasterModel>(projectMaster);


            config = new MapperConfiguration(c => c.CreateMap<AccessoriesPrice, AccessoriesPricesModel>());
            map = config.CreateMapper();
            var accessoriespricemodels = map.Map<List<AccessoriesPricesModel>>(accessoriesprices);
            project.AccessoriesPrices = accessoriespricemodels;

            config = new MapperConfiguration(c => c.CreateMap<ProjectImage, ProjectImageModel>());
            map = config.CreateMapper();
            var proimg = map.Map<List<ProjectImageModel>>(projectImages);
            foreach (var img in proimg)
            {
                var manager = new FileManager();
                img.ImagePath = manager.GetFile(img.ImagePath);
            }
            project.ProjectImageModels = proimg;

            return project;
        }

        public decimal UpdateProject(ProjectMasterModel model, long userId)
        {
            try
            {
                var projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == model.ProjectMasterId);
                //====Project log data in project master tracker===
                //Saving previous data before saving new data in tracker
                ProjectMasterTracker tracker = GenericMapper<ProjectMaster, ProjectMasterTracker>.GetDestination(projectMaster);
                tracker.TrackerAddedDate = DateTime.Now;
                tracker.TrackerAddedBy = userId;
                _dbEntities.ProjectMasterTrackers.Add(tracker);
                _dbEntities.SaveChanges();
                //Saving updated data tracker
                Mapper.CreateMap<ProjectMasterModel, ProjectMasterTracker>();
                var v = Mapper.Map<ProjectMasterTracker>(model);
                v.TrackerAddedBy = userId;
                v.TrackerAddedDate = DateTime.Now;
                _dbEntities.ProjectMasterTrackers.Add(v);
                _dbEntities.SaveChanges();
                //----END----

                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                var usrInfo = user != null ? "<br/>Project Created By: " + user.UserFullName : "";
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();


                ProjectMasterLog log = GenericMapper<ProjectMaster, ProjectMasterLog>.GetDestination(projectMaster);
                if (projectMaster != null && projectMaster.ProjectNameForScreening == projectMaster.ProjectName &&
                    projectMaster.ProjectName != model.ProjectName && model.ProjectName != model.ProjectNameForScreening &&
                    !string.IsNullOrWhiteSpace(model.ProjectName) && model.IsFinallyClosed != true)
                {
                    string projectName = model.ProjectName.Trim();

                    var body =
                        string.Format(
                            @"This is to inform you that, our company finally made a decision to set a project's final name (Brand Name). This project's final name has been decided by our company's responsible personel. The new data has been updated successfully in 'Walton Project Management System (WPMS)' By Commercial section.<br/><br/><b>Project Name: " +
                            projectName + "</b>" + usrInfo + time);
                    var mail = new MailSendFromPms();
                    mail.SendMail(new List<string>(new[] { "CM" }),
                        new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS" }), "NEW PROJECT( " + projectName.Trim() + " )",
                        body);
                }
                else if (projectMaster != null &&
                         (projectMaster.ProjectNameForScreening != projectMaster.ProjectName && projectMaster.ProjectName != model.ProjectName.Trim()) && model.IsFinallyClosed != true)
                {
                    var body =
                        string.Format(
                            @"This is to inform you that, an existing project's name has been updated.<br/><br/><b>New Project Name: " +
                            model.ProjectName + "</b><br/>Previous Project Name : " + projectMaster.ProjectName.Trim() + usrInfo +
                            time);
                    var mail = new MailSendFromPms();
                    var result = mail.SendMail(new List<string>(new[] { "CM" }),
                        new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS", "BIHEAD" }),
                        "Data Change of existing project ( " + model.ProjectName + " )", body);
                }
                if (projectMaster != null)
                {
                    projectMaster.ApproxProjectFinishDate = model.ApproxProjectFinishDate;
                    projectMaster.ApproxProjectOrderDate = model.ApproxProjectOrderDate;
                    projectMaster.BackCamSensor = model.BackCamSensor;
                    projectMaster.ApproximatePrice = model.ApproximatePrice;
                    projectMaster.BackCam = model.BackCam;
                    projectMaster.BackCamBsi = model.BackCamBsi;
                    projectMaster.BateeryPossibleSupplierNames = model.BateeryPossibleSupplierNames;
                    projectMaster.BatteryCoverFinishingType = model.BatteryCoverFinishingType;
                    projectMaster.BatteryCoverLogoType = model.BatteryCoverLogoType;
                    projectMaster.BatteryRating = model.BatteryRating;
                    projectMaster.BatterySupplierName = model.BatterySupplierName;
                    projectMaster.BatteryType = model.BatteryType;
                    projectMaster.Cdma = model.Cdma;
                    projectMaster.ChargerRating = model.ChargerRating;
                    projectMaster.ChargerSupplierName = model.ChargerSupplierName;
                    projectMaster.ChipsetBit = model.ChipsetBit;
                    projectMaster.ChipsetCore = model.ChipsetCore;
                    projectMaster.ChipsetFrequency = model.ChipsetFrequency;
                    projectMaster.ChipsetName = model.ChipsetName;
                    projectMaster.Color = model.Color;
                    projectMaster.Compass = model.Compass;
                    projectMaster.CpuName = model.CpuName;
                    projectMaster.DisplayResulution = model.DisplayResulution;
                    projectMaster.DisplaySpeciality = model.DisplaySpeciality;
                    projectMaster.EarphoneConfirmPrice = model.EarphoneConfirmPrice;
                    projectMaster.EarphoneSupplierName = model.EarphoneSupplierName;
                    projectMaster.FinalPrice = model.FinalPrice;
                    projectMaster.FlashLight = model.FlashLight;
                    projectMaster.FourthGenFdd = model.FourthGenFdd;
                    projectMaster.FourthGenTdd = model.FourthGenTdd;

                    projectMaster.ApproxShipmentDate = model.ApproxShipmentDate;
                    projectMaster.BackCamera = model.BackCamera;
                    projectMaster.Battery = model.Battery;
                    projectMaster.Chipset = model.Chipset;
                    projectMaster.DisplayName = model.DisplayName;
                    projectMaster.DisplaySize = model.DisplaySize;
                    projectMaster.FrontCam = model.FrontCam;
                    projectMaster.FrontCamBsi = model.FrontCamBsi;
                    projectMaster.FrontCamSensor = model.FrontCamSensor;
                    projectMaster.FrontCamera = model.FrontCamera;
                    projectMaster.Gps = model.Gps;
                    projectMaster.Gsensor = model.Gsensor;
                    projectMaster.Gyroscope = model.Gyroscope;
                    projectMaster.HallSensor = model.HallSensor;
                    projectMaster.HousingFinalVendorName = model.HousingFinalVendorName;
                    projectMaster.HousingVendorName = model.HousingVendorName;
                    //dbData.IsProjectManagerAssigned = model.IsProjectManagerAssigned;
                    projectMaster.LcdFinalVendor = model.LcdFinalVendor;
                    projectMaster.LcdVendor = model.LcdVendor;
                    projectMaster.Lsensor = model.Lsensor;
                    projectMaster.MemoryBrandName = model.MemoryBrandName;
                    projectMaster.NumberOfSample = model.NumberOfSample;
                    projectMaster.OrderQuantity = model.OrderQuantity;
                    projectMaster.OsName = model.OsName;
                    projectMaster.OsVersion = model.OsVersion;
                    projectMaster.Otg = model.Otg;
                    projectMaster.OtgCable = model.OtgCable;
                    projectMaster.PcbaFinalVendor = model.PcbaFinalVendor;
                    projectMaster.PcbaVendorName = model.PcbaVendorName;
                    projectMaster.ProcessorClock = model.ProcessorClock;
                    projectMaster.ProcessorName = model.ProcessorName;
                    projectMaster.ProjectName = model.ProjectName.Trim();
                    projectMaster.ProjectNameForScreening = model.ProjectNameForScreening;
                    projectMaster.ProjectType = model.ProjectType;
                    projectMaster.ProjectTypeId = model.ProjectTypeId;
                    projectMaster.Psensor = model.Psensor;
                    projectMaster.Ram = model.Ram;
                    projectMaster.Rom = model.Rom;
                    projectMaster.SecondGen = model.SecondGen;
                    projectMaster.SimSlotNumber = model.SimSlotNumber;
                    projectMaster.SlotType = model.SlotType;
                    projectMaster.SourcingType = model.SourcingType;
                    projectMaster.SpecialSensor = model.SpecialSensor;
                    projectMaster.SupplierId = model.SupplierId;
                    projectMaster.SupplierModelName = model.SupplierModelName;
                    projectMaster.SupplierName = model.SupplierName;
                    projectMaster.SupplierTrustLevel = model.SupplierTrustLevel;
                    projectMaster.ThirdGen = model.ThirdGen;
                    projectMaster.ThreeLayerScreenProtector = model.ThreeLayerScreenProtector;
                    projectMaster.TpFinalVendor = model.TpFinalVendor;
                    projectMaster.TpVendor = model.TpVendor;
                    projectMaster.Updated = userId;
                    projectMaster.UpdatedDate = DateTime.Now;
                    projectMaster.CameraVendor = model.CameraVendor;
                    projectMaster.RamVendor = model.RamVendor;
                    projectMaster.RomVendor = model.RomVendor;
                    projectMaster.AnotherPrice = model.AnotherPrice;
                    projectMaster.PricingRemarks = model.PricingRemarks;
                    projectMaster.SwotAnalysisBy = model.SwotAnalysisBy;
                    projectMaster.SwotAnalysisDate = model.SwotAnalysisDate;
                    projectMaster.SwotOpportunityRemarks = model.SwotOpportunityRemarks;
                    projectMaster.ProjectStatus = model.ProjectStatus;
                    projectMaster.IsFinallyClosed = model.IsFinallyClosed;
                    projectMaster.BrandId = model.BrandId;
                    projectMaster.DeactivatedBy = model.DeactivatedBy;
                    projectMaster.DeactivationDate = model.DeactivationDate;
                    projectMaster.ActivationBy = model.ActivationBy;
                    projectMaster.ActivationDate = model.ActivationDate;
                    projectMaster.ActivationDeactivationRemarks = model.ActivationDeactivationRemarks;
                    projectMaster.IsActive = model.IsActive;
                    projectMaster.PsApprovalBy = model.PsApprovalBy;
                    projectMaster.PsApprovalDate = model.PsApprovalDate;
                    projectMaster.PsRemarks = model.PsRemarks;
                    projectMaster.CeoApprovalBy = model.CeoApprovalBy;
                    projectMaster.CeoApprovalDate = model.CeoApprovalDate;
                    projectMaster.CeoRemarks = model.CeoRemarks;
                    projectMaster.ChecklistEditPermission = model.ChecklistEditPermission;
                    projectMaster.BiApprovalBy = model.BiApprovalBy;
                    projectMaster.BiApprovalDate = model.BiApprovalDate;
                    projectMaster.BiRemarks = model.BiRemarks;
                    projectMaster.AddOrUpdateRemarks = model.AddOrUpdateRemarks;
                    _dbEntities.Entry(projectMaster).State = EntityState.Modified;
                    _dbEntities.ProjectMasterLogs.Add(log);
                    _dbEntities.SaveChanges();
                    //=====file upload====
                    try
                    {
                        var moduleDirectory = model.ProjectName;
                        var userDirectory = "ProjectImages";
                        if (model.ProjectImageModels.Count > 0)
                        {
                            foreach (var f in model.ProjectImageModels)
                            {
                                if (f.Id == 0)
                                {
                                    var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectImageModel, ProjectImage>());
                                    var mapper = config.CreateMapper();
                                    var proimg = mapper.Map<ProjectImage>(f);
                                    var manager = new FileManager();
                                    f.ImagePath = manager.DocManagementUpload(userDirectory, moduleDirectory,
                                        f.PostedFile);
                                    proimg.ProjectId = model.ProjectMasterId;
                                    proimg.OrderNo = model.OrderNuber;
                                    proimg.ProjectModel = model.ProjectName;
                                    proimg.ImagePath = f.ImagePath;
                                    proimg.Remarks = f.Remarks;
                                    proimg.AddedBy = userId;
                                    proimg.AddedDate = DateTime.Now;
                                    if (f.ImagePath != null && f.ImagePath != "failed")
                                    {
                                        _dbEntities.ProjectImages.Add(proimg);
                                    }
                                }
                                if (f.Id > 0)
                                {
                                    var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectImageModel, ProjectImage>());
                                    var mapper = config.CreateMapper();
                                    var proimg = mapper.Map<ProjectImage>(f);
                                    var manager = new FileManager();
                                    var newImgPath = manager.DocManagementUpload(userDirectory, moduleDirectory,
                                        f.PostedFile);
                                    f.ImagePath = (f.ImagePath != newImgPath && newImgPath!="failed") ? newImgPath : f.ImagePath;
                                    proimg.ProjectId = model.ProjectMasterId;
                                    proimg.OrderNo = model.OrderNuber;
                                    proimg.ProjectModel = model.ProjectName;
                                    proimg.ImagePath = f.ImagePath;
                                    proimg.Remarks = f.Remarks;
                                    proimg.UpdatedBy = userId;
                                    proimg.UpdatedDate = DateTime.Now;
                                    if (f.ImagePath != null && f.ImagePath != "failed")
                                    {
                                        _dbEntities.ProjectImages.AddOrUpdate(proimg);
                                    }
                                }
                            }
                            _dbEntities.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    //=========
                    //===Project Updated Mail===
                    var body =
                    string.Format(
                        @"This is to inform you that,Data of Project: <b>" +
                        model.ProjectName + " (Project ID: " + model.ProjectMasterId + ")</b>,<br/>has been changed by : <b>" + user.UserFullName + "</b>. Check tracker table for further details.");
                    var mail = new MailSendFromPms();
                    mail.SendMail(new List<string>(new[] { "" }),
                        new List<string>(new[] { "" }),
                        "Data Change of existing project ( " + model.ProjectName + " )", body);
                    //-----O----
                }
                List<AccessoriesPrice> dbDataAccessories = _dbEntities.AccessoriesPrices.Where(i => i.ProjectMasterId == model.ProjectMasterId).ToList();
                if (UpdateAccessoriesPrices(model.ProjectMasterId, model.AccessoriesPrices))
                    return 1;
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool UpdateAccessoriesPrices(long prid, List<AccessoriesPricesModel> pricemodels)
        {
            try
            {
                if (pricemodels.Any())
                {
                    var prevPrices = _dbEntities.AccessoriesPrices.Where(i => i.ProjectMasterId == prid).ToList();
                    _dbEntities.AccessoriesPrices.RemoveRange(prevPrices);
                    var config = new MapperConfiguration(cfg => cfg.CreateMap<AccessoriesPricesModel, AccessoriesPrice>()
                        .ForMember(d => d.ProjectMasterId, o => o.MapFrom(m => (m.ProjectMasterId > 0 ? m.ProjectMasterId : prid))));
                    var mapper = config.CreateMapper();
                    List<AccessoriesPrice> newprices = mapper.Map<List<AccessoriesPrice>>(pricemodels);
                    //GenericMapper<AccessoriesPricesModel, AccessoriesPrice>

                    //.GetDestinationList(pricemodels);
                    _dbEntities.AccessoriesPrices.AddRange(newprices);
                    _dbEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public long SaveProject(ProjectMasterModel model, long userId)
        {
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            try
            {
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                model.ProjectName = model.ProjectName.Trim(' ').Replace("\t", string.Empty);
                //model.ProjectModel = model.ProjectModel.Trim(' ').Replace("\t", string.Empty);
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectMasterModel, ProjectMaster>());
                var mapper = config.CreateMapper();
                var projectMaster = mapper.Map<ProjectMaster>(model);
                projectMaster.OrderNuber = 1;
                var value = projectMaster;
                value.IsActive = true;
                //==add project model of feature phone by default==
                if (projectMaster.ProjectType == "Feature")
                {
                    var nameParts = projectMaster.ProjectName.Split(' ');
                    if (nameParts.Length > 1)
                    {
                        var firstPart =
                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameParts[0].ToLower());
                        value.ProjectModel = firstPart + " " + nameParts[1];
                    }
                    else
                    {
                        value.ProjectModel = projectMaster.ProjectName;
                    }
                }
                //----O---
                _dbEntities.ProjectMasters.Add(value);
                _dbEntities.SaveChanges();

                //Project log in project master tracker
                ProjectMasterTracker tracker = GenericMapper<ProjectMaster, ProjectMasterTracker>.GetDestination(value);
                tracker.TrackerAddedDate = DateTime.Now;
                tracker.TrackerAddedBy = userId;
                _dbEntities.ProjectMasterTrackers.Add(tracker);
                _dbEntities.SaveChanges();
                //----END----

                //Add in WCMS

                //--END---

                //Accessory Prices
                config = new MapperConfiguration(cfg => cfg.CreateMap<AccessoriesPricesModel, AccessoriesPrice>());
                mapper = config.CreateMapper();
                foreach (var item in model.AccessoriesPrices)
                {
                    var priceentity = mapper.Map<AccessoriesPrice>(item);
                    priceentity.ProjectMasterId = value.ProjectMasterId;
                    priceentity.AddedBy = userId;
                    priceentity.AddedDate = DateTime.Now;
                    _dbEntities.AccessoriesPrices.Add(priceentity);
                }
                //=====file upload====
                try
                {
                    var moduleDirectory = value.ProjectName;
                    var userDirectory = "ProjectImages";
                    config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectImageModel, ProjectImage>());
                    mapper = config.CreateMapper();
                    if (model.ProjectImageModels.Count > 0)
                    {
                        foreach (var f in model.ProjectImageModels)
                        { 
                            var proimg = mapper.Map<ProjectImage>(f);
                            var manager = new FileManager();
                            f.ImagePath = manager.IncidentUpload(userDirectory, moduleDirectory,
                                f.PostedFile);
                            proimg.ProjectId = value.ProjectMasterId;
                            proimg.OrderNo = value.OrderNuber;
                            proimg.ProjectModel = value.ProjectName;
                            proimg.ImagePath = f.ImagePath;
                            proimg.Remarks = f.Remarks;
                            proimg.AddedBy = userId;
                            proimg.AddedDate = DateTime.Now;

                            _dbEntities.ProjectImages.Add(proimg);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                //=========

                _dbEntities.SaveChanges();
                var usrInfo = user != null ? "<br/>Project Created By: " + user.UserFullName : "";
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
                string projectName;
                if (model.ProjectNameForScreening.Equals(model.ProjectName))
                {
                    projectName =
                        string.Format(
                            @"Project Name has not been selected yet, The initial screening name for this project is '{0}'",
                            model.ProjectNameForScreening);
                }
                else
                {
                    projectName = model.ProjectName;
                }

                var body =
                    string.Format(
                        @"This is to inform you that, A new project has been created in Walton Project Management System By Commercial section.<br/><br/><b>Project Name: " +
                        projectName + "</b>" + usrInfo + time);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(new List<string>(new[] { "CM" }),
                    new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "HWHEAD", "PS", "BIHEAD" }), "NEW PROJECT( " + projectName + " )", body);
                return value.ProjectMasterId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long GetProjectId(string projectName)
        {
            var master = _dbEntities.ProjectMasters.Where(i => i.ProjectName == projectName)
                .OrderByDescending(i => i.ProjectMasterId)
                .FirstOrDefault();
            if (master != null)
            {
                long projectid =
                    master
                        .ProjectMasterId;
                return projectid;
            }
            return 0;
        }

        public List<ProjectMasterModel> GetProjectsForPurchaseOrder()
        {
            var allProjects =
                _dbEntities.ProjectMasters.Where(
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
                        OrderNuber = i.OrderNuber


                    }).OrderBy(i => i.ProjectName).ThenBy(i => i.ProjectMasterId).ToList();
            return allProjects;
        }

        public List<HwInchargeIssueModel> GetScreeningIssues(long id)
        {
            List<HwInchargeIssue> dbModels = GenereticRepo<HwInchargeIssue>.GetList(_dbEntities,
                issue => issue.ProjectMasterId == id && issue.IsReviewd != true);
            List<HwInchargeIssueModel> models =
                GenericMapper<HwInchargeIssue, HwInchargeIssueModel>.GetDestinationList(dbModels);
            return models;
        }

        public int SaveScreeningIssues(VmScreeningIssues model)
        {
            try
            {
                long projectId = model.ProjectMasterId;
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                if (model.HwInchargeIssueModels.Any())
                {
                    foreach (var x in model.HwInchargeIssueModels)
                    {
                        var issue =
                            _dbEntities.HwInchargeIssues.FirstOrDefault(
                                i => i.HwInchargeIssuesId == x.HwInchargeIssuesId);
                        if (issue != null)
                        {
                            issue.CommercialDecision = x.CommercialDecision;
                            issue.Remarks = x.Remarks;
                            issue.UpdatedDate = DateTime.Now;
                            issue.UpdatedBy = userId;
                            _dbEntities.Entry(issue).State = EntityState.Modified;
                        }
                    }
                    if (model.HwInchargeIssueModels.All(i => i.CommercialDecision != null))
                    {
                        var master = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
                        if (master != null)
                        {
                            master.ProjectStatus = "PARTIAL";
                            master.Updated = userId;
                            master.UpdatedDate = DateTime.Now;
                            master.ScreeningIssueReviewDate = DateTime.Now;
                            _dbEntities.Entry(master).State = EntityState.Modified;
                        }
                    }
                }
                else if (model.HwInchargeIssueModels.Count <= 0 && model.ProjectMasterId > 0)
                {
                    var master = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
                    if (master != null)
                    {
                        master.ProjectStatus = "PARTIAL";
                        master.Updated = userId;
                        master.UpdatedDate = DateTime.Now;
                        _dbEntities.Entry(master).State = EntityState.Modified;
                    }
                }
                _dbEntities.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }
            return 1;
        }

        public List<ProjectMasterModel> GetAllProjectsForStatus()
        {
            var allProjects = _dbEntities.ProjectMasters.Where(i => i.IsActive).Select(i => new ProjectMasterModel
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
        #endregion

        public void ScreeningIssueNotification(long projectMasterId, long userId)
        {
            try
            {
                var notifications = new List<Notification>();
                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                var projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectMasterId);
                string[] roles = { "CM", "MM", "PS", "HWHEAD" };
                var viewers =
                    _dbEntities.CmnUsers.Where(i => roles.Contains(i.RoleName) && i.IsActive).ToList();
                if (viewers.Any())
                {
                    foreach (var viewer in viewers)
                    {
                        var notification = new Notification
                        {
                            ProjectMasterId = projectMasterId,
                            AddedBy = userId,
                            Role = viewer.RoleName,
                            Message =
                                "A project's screening issues has been reviewed by " + user.UserFullName +
                                ". Project Name : " + projectMaster.ProjectName,
                            Added = DateTime.Now,
                            IsViewd = false,
                            ViewerId = (int?)viewer.CmnUserId,
                            AdditionalMessage = ""
                        };
                        _dbEntities.Notifications.Add(notification);
                    }
                    _dbEntities.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                var ex = exception.Message;
            }
        }

        public SelectListItem ClosePurchaseOrder(DateTime marketClearanceDate, long proOrdersId, long proIds, string BdIqcResult)
        {
            long userId;
            long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
            var result = new SelectListItem();
            try
            {
                var shipments =
                    _dbEntities.ProjectOrderShipments.Where(
                        i => i.ProjectPurchaseOrderFormId == proOrdersId).ToList();
                if (shipments.Any())
                {
                    foreach (var smt in shipments)
                    {
                        smt.IsComplete = true;
                        smt.Updated = userId;
                        smt.UpdatedDate = DateTime.Now;
                        _dbEntities.ProjectOrderShipments.AddOrUpdate(smt);
                        _dbEntities.SaveChanges();
                    }
                }
                //if (!shipments.Any() || !(shipments.Any(i => i.IsComplete != true)))
                //{

                var form = _dbEntities.ProjectPurchaseOrderForms.FirstOrDefault(i => i.ProjectPurchaseOrderFormId == proOrdersId);
                if (form != null)
                {
                    form.IsCompleted = true;
                    //form.Updated = userId;
                    //form.UpdatedDate = DateTime.Now;
                    form.IsCompletedDate = DateTime.Now;
                    form.MarketClearanceDate = marketClearanceDate;
                    form.BdIqcResult = BdIqcResult;
                    //form.OrderColorRatioWithQty = OrderColorRatioWithQty;
                    _dbEntities.Entry(form).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                    result.Value = "0";
                    result.Text = string.Format(
                            @"<div class='alert alert-success alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'></button>
                        <strong>Message:</strong> Purchase order closed successfully
                    </div>");

                    //========Project Closing Status Update=====
                    var master =
                        _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == form.ProjectMasterId);
                    if (master != null)
                    {
                        master.ProjectClosingStatus = "MARKET_CLEARANCE";
                        //master.Updated = userId;
                        //master.UpdatedDate = DateTime.Now;
                        master.ProjectClosedBy = Convert.ToString(userId);
                        master.ProjectClosingDate = DateTime.Now;
                        _dbEntities.Entry(master).State = EntityState.Modified;
                        _dbEntities.SaveChanges();
                    }

                    CreateNotificationAndEmailForPurchaseOrderClosing(form.ProjectMasterId);
                    //========Project Close Penalty====
                    string query = string.Format(@"select ppf.ProjectPurchaseOrderFormId,ppf.ProjectMasterId, pm.ProjectName, ppf.PoDate ,pm.OrderNuber, DATEDIFF(MONTH,ppf.PoDate,GETDATE()) as PoCreatedBeforeMonth,
                                           DATEDIFF(DAY, DATEADD(MONTH,7,ppf.PoDate),GETDATE()) as DaysPassedAfterSevenMonth,
                                           DATEDIFF(DAY, DATEADD(MONTH,7,ppf.PoDate),GETDATE())*50 as Penalty 
                                           from ProjectPurchaseOrderForms ppf
                                           inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                           where ppf.ProjectPurchaseOrderFormId={0} and DATEDIFF(MONTH,ppf.PoDate,GETDATE())>7", proOrdersId);
                    var model = _dbEntities.Database.SqlQuery<ProjectClosePenaltyModel>(query).FirstOrDefault();
                    if (model != null)
                    {
                        model.OrderNumber = _dbEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectMasterId)
                            .Select(x => x.OrderNuber)
                            .FirstOrDefault();
                        model.IsCompletedDate = form.IsCompletedDate;
                        Mapper.CreateMap<ProjectClosePenaltyModel, ProjectClosePenalty>();
                        var save = Mapper.Map<ProjectClosePenalty>(model);
                        _dbEntities.ProjectClosePenaltys.Add(save);
                        if (model.ProjectMasterId != null)
                        {
                            _dbEntities.SaveChanges();
                        }
                    }
                }
                else
                {
                    result.Value = "-1";
                    result.Text = string.Format(
                            @"<div class='alert alert-danger alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'></button>
                        <strong>Message:</strong> Unable to close purchase order. Purchase order not found !!!
                    </div>");
                }

                //}
                //                else
                //                {
                //                    result.Value = "-1";
                //                    result.Text = string.Format(
                //                                @"<div class='alert alert-info alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'></button>
                //                        <strong>Message:</strong> Failed to close the PO. All shipment of this PO must be closed !!!
                //                    </div>");
                //                }
            }
            catch (Exception ex)
            {
                result.Value = "-1";
                result.Text = string.Format(
                                @"<div class='alert alert-danger alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'></button>
                        <strong>Message:</strong> Error !!! <br>{0}<br>Please contact with administrator...
                    </div>", ex.Message);
            }
            return result;
        }

        public List<BtrcRawListModel> GetBtrcRowModels()
        {
            List<BtrcRawListModel> models = (from btrcRaw in _dbEntities.BtrcRaws
                                             join noc in _dbEntities.ProjectBtrcNocs on btrcRaw.BtrcRawId equals noc.BtrcRawId

                                             join orderForm in _dbEntities.ProjectPurchaseOrderForms on noc.ProjectPurchaseOrderFormId equals
                                                 orderForm.ProjectPurchaseOrderFormId

                                             join projectMaster in _dbEntities.ProjectMasters on noc.ProjectMasterId equals
                                                 projectMaster.ProjectMasterId

                                             select new BtrcRawListModel
                                             {
                                                 BtrcRawId = btrcRaw.BtrcRawId,
                                                 NocNo = btrcRaw.NocNo,
                                                 NocApplyDate = btrcRaw.NocApplyDate,
                                                 AppxNocReceiveDate = btrcRaw.AppxNocReceiveDate,
                                                 NocReceiveDate = btrcRaw.NocReceiveDate,
                                                 NocIssueDate = btrcRaw.NocIssueDate,
                                                 NocValidityDate = btrcRaw.NocValidityDate,
                                                 Added = btrcRaw.Added,
                                                 AddedDate = btrcRaw.AddedDate,
                                                 Updated = btrcRaw.Updated,
                                                 UpdatedDate = btrcRaw.UpdatedDate,
                                                 ApplicationId = btrcRaw.ApplicationId,
                                                 ProjectBtrcNocId = noc.ProjectBtrcNocId,
                                                 ProjectPurchaseOrderId = orderForm.ProjectPurchaseOrderFormId,
                                                 PurchaseOrderNumber = orderForm.PurchaseOrderNumber,
                                                 ProjectMasterId = projectMaster.ProjectMasterId,
                                                 ProjectName = projectMaster.ProjectName,
                                                 OrderNumber = projectMaster.OrderNuber,
                                                 //OrderNumberOrdinal = CommonConversion.AddOrdinal(projectMaster.OrderNuber) + " Order"
                                             }).ToList();
            foreach (var model in models)
            {
                model.OrderNumberOrdinal = CommonConversion.AddOrdinal(model.OrderNumber) + " Order";
            }
            return models;
        }
        private void CreateNotificationAndEmailForPurchaseOrderClosing(long projectMasterId)
        {
            var projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectMasterId);
            if (projectMaster != null)
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                var closedBy = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                string[] userRoles = { "MM", "CM", "PM", "PMHEAD", "HW", "HWHEAD", "QC", "QCHEAD", "PS" };
                var users = _dbEntities.CmnUsers.Where(i => userRoles.Contains(i.RoleName) && i.IsActive && i.CmnUserId != userId).ToList();
                if (users.Any())
                {
                    var commonMessage = CommonConversion.AddOrdinal(projectMaster.OrderNuber) + " order of " +
                                        projectMaster.ProjectName + " has been closed";
                    foreach (var user in users)
                    {
                        var notification = new Notification
                        {
                            ProjectMasterId = projectMasterId,
                            Message = commonMessage + " by " + closedBy.UserFullName,
                            AdditionalMessage = "",
                            Role = user.RoleName,
                            IsViewd = false,
                            AddedBy = userId,
                            Added = DateTime.Now
                        };
                        _dbEntities.Notifications.Add(notification);
                    }
                    _dbEntities.SaveChanges();


                    var usrInfo = closedBy != null ? "<br/>Action taken by: " + closedBy.UserFullName : "<br/>Forwarded By: Unknown";
                    string projectName = projectMaster.ProjectName;
                    string time = "<br/>Action taken On: " + DateTime.Now.ToLongDateString();
                    var body =
                        string.Format(
                            @"This is to inform you that, {0}<br/><br/><b>Project Name: " +
                            projectName + "</b>" + usrInfo + time, commonMessage);


                    var mail = new MailSendFromPms();
                    var result = mail.SendMail(userRoles.ToList(), new List<long>(), "PO Close( " + projectName + " )", body);
                }

            }
        }

        #region warehouse
        public bool GetWarehouseDetail(int orderNumber, long projectMasterId, string projectName, long projectOrderShipmentId,
         long projectPurchaseOrderFormId, string purchaseOrderNumber, long quantity, DateTime shipmentDate,
         DateTime warehouseDate, long warehouseQuantity)
        {

            //DateTime shipmentDate1;
            //DateTime warehouseDate1;

            //DateTime.TryParseExact(shipmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
            //    out shipmentDate1);

            //DateTime.TryParseExact(warehouseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
            //    out warehouseDate1);

            string shipmentDate1 = shipmentDate.ToString("yyyy-MM-dd");
            string warehouseDate1 = warehouseDate.ToString("yyyy-MM-dd");

            List<Custom_Warehouse_Details> getWarehouseDetails = null;
            if (projectMasterId > 0)
            {
                string getWarehouseQuery = string.Format(@"         
                select  [ProjectMasterId],[ProjectName],[OrderNumber],[ProjectOrderShipmentId],[ShipmentDate],[ProjectPurchaseOrderFormId],[PurchaseOrderNumber],[Quantity],[WarehouseQuantity],[WarehouseDate]
                from [CellPhoneProject].[dbo].[WarehouseDetails] where OrderNumber='{0}' and ProjectMasterId='{1}' and  ProjectName='{2}' and ProjectOrderShipmentId='{3}' and ShipmentDate='{4}'
                and ProjectPurchaseOrderFormId='{5}' and PurchaseOrderNumber='{6}' and Quantity='{7}' and WarehouseQuantity='{8}' and WarehouseDate='{9}' ",
                orderNumber, projectMasterId, projectName, projectOrderShipmentId, shipmentDate1, projectPurchaseOrderFormId, purchaseOrderNumber, quantity, warehouseQuantity, warehouseDate1);

                getWarehouseDetails =
                   _dbEntities.Database.SqlQuery<Custom_Warehouse_Details>(getWarehouseQuery).ToList();


            }

            if (getWarehouseDetails != null && getWarehouseDetails.Count != 0)
            {
                return true;
            }
            return false;
        }
        //        public bool GetShipmentTotalQuantity(long projectMasterId, string purchaseOrderNumber, DateTime shipmentDate, long quantity)
        //        {
        //            var shipmentDate1 = shipmentDate.ToString("yyyy-MM-dd");
        //            List<Custom_Warehouse_Details> getWarehouseDetails = null;
        //            if (projectMasterId>0)
        //            {
        //                string getWarehouseQuery = string.Format(@"select sum(WarehouseQuantity) as WarehouseQuantity from [CellPhoneProject].[dbo].[WarehouseDetails] where ProjectMasterId='{0}' and PurchaseOrderNumber='{1}' and ShipmentDate='{2}'
        //                and Quantity='{3}' ", projectMasterId,purchaseOrderNumber,shipmentDate1,quantity);

        //                getWarehouseDetails =
        //                    _dbEntities.Database.SqlQuery<Custom_Warehouse_Details>(getWarehouseQuery).ToList();
        //            }
        //            if (getWarehouseDetails != null && getWarehouseDetails.Count != 0)
        //            {
        //                return true;
        //            }
        //            return false;
        //        }
        public List<Custom_Warehouse_Details> GetShipmentTotalQuantity(long projectMasterId, string purchaseOrderNumber, DateTime shipmentDate,
                  long ShipmentQty)
        {
            var shipmentDate1 = shipmentDate.ToString("yyyy-MM-dd");
            string getPrchOrd = string.Format(@"select sum(WarehouseQuantity) as WarehouseQuantity from [CellPhoneProject].[dbo].[WarehouseDetails] where ProjectMasterId='{0}' and PurchaseOrderNumber='{1}' and ShipmentDate='{2}'
               and Quantity='{3}' ", projectMasterId, purchaseOrderNumber, shipmentDate1, ShipmentQty);
            var getAllPrcOrd = _dbEntities.Database.SqlQuery<Custom_Warehouse_Details>(getPrchOrd).ToList();

            return getAllPrcOrd;
        }

        public List<ProjectPurchaseOrderFormModel> GetPurchaseOrders(long proId)
        {

            string getPrchOrd = string.Format(@"select * from CellPhoneProject.dbo.ProjectPurchaseOrderForms where ProjectMasterId={0}", proId);
            var getAllPrcOrd = _dbEntities.Database.SqlQuery<ProjectPurchaseOrderFormModel>(getPrchOrd).ToList();

            return getAllPrcOrd;
        }
        public List<ProjectOrderShipmentModel> GetShipments(long proIds, string purchaseOrderNo)
        {
            string getShipment =
                string.Format(
                    @"select pos.ChainaInspectionDate,pos.ProjectMasterId,ppo.ProjectPurchaseOrderFormId,pos.ProjectOrderShipmentId from [CellPhoneProject].[dbo].ProjectPurchaseOrderForms ppo
                      join [CellPhoneProject].[dbo].[ProjectOrderShipments] pos on ppo.ProjectMasterId=pos.ProjectMasterId
                      where pos.ProjectMasterId='{0}' and ppo.PurchaseOrderNumber='{1}'",
                    proIds, purchaseOrderNo);
            var getShipmentsData = _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(getShipment).ToList();
            return getShipmentsData;
        }
        public List<ProjectPurchaseOrderFormModel> GetShipmentQuantity(long proIds, string purchaseOrderNo, string shipmentDate)
        {
            string getShipment =
                string.Format(
                    @"select ppo.Quantity,pos.ProjectMasterId,ppo.ProjectPurchaseOrderFormId,pos.ProjectOrderShipmentId,pm.ProjectName,pm.OrderNuber as OrderNumber from [CellPhoneProject].[dbo].ProjectPurchaseOrderForms ppo
                      join [CellPhoneProject].[dbo].[ProjectOrderShipments] pos on ppo.ProjectMasterId=pos.ProjectMasterId
                      join  [CellPhoneProject].[dbo].[ProjectMasters] pm on pm.ProjectMasterId=ppo.ProjectMasterId
                      where pos.ProjectMasterId='{0}' and ppo.PurchaseOrderNumber='{1}' and ppo.ProjectPurchaseOrderFormId=pos.ProjectPurchaseOrderFormId
                      and pos.ChainaInspectionDate='{2}'",
                    proIds, purchaseOrderNo, shipmentDate);
            var getShipmentsData = _dbEntities.Database.SqlQuery<ProjectPurchaseOrderFormModel>(getShipment).ToList();
            return getShipmentsData;
        }

        public string SaveWarehouseDetails(List<Custom_Warehouse_Details> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            foreach (var insResult in results)
            {
                var model = new WarehouseDetail
                {
                    ProjectMasterId = insResult.ProjectMasterId,
                    ProjectName = insResult.ProjectName,
                    OrderNumber = insResult.OrderNumber,
                    PurchaseOrderNumber = insResult.PurchaseOrderNumber,
                    Quantity = insResult.Quantity,
                    WarehouseQuantity = insResult.WarehouseQuantity,
                    WarehouseDate = insResult.WarehouseDate,
                    ProjectPurchaseOrderFormId = insResult.ProjectPurchaseOrderFormId,
                    ProjectOrderShipmentId = insResult.ProjectOrderShipmentId,
                    ShipmentDate = insResult.ShipmentDate,
                    Added = userId,
                    AddedDate = DateTime.Now

                };
                _dbEntities.WarehouseDetails.AddOrUpdate(model);
            }
            _dbEntities.SaveChanges();

            return "ok";
        }

        #endregion

        #region VENDOR AUTOCOMPLETE

        public List<VmIncentivePolicy> GetUserInfoSpare(long monIds, long yearIds)
        {
            _dbEntities.Database.CommandTimeout = 6000;
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            where cm.rolename in ('SPR') and cm.IsActive=1", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public List<ProjectMasterModel> GetVendorList(string vendor, string type)
        {
            var model = new List<ProjectMasterModel>();
            if (type == "camera")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.CameraVendor.StartsWith(vendor)
                         group v by v.CameraVendor into y
                         select new ProjectMasterModel
                             {
                                 CameraVendor = y.Key
                             }).ToList();
            }
            if (type == "rom")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.RomVendor.StartsWith(vendor)
                         group v by v.RomVendor into y
                         select new ProjectMasterModel
                         {
                             RomVendor = y.Key
                         }).ToList();
            }
            if (type == "ram")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.RamVendor.StartsWith(vendor)
                         group v by v.RamVendor into y
                         select new ProjectMasterModel
                         {
                             RamVendor = y.Key
                         }).ToList();
            }
            if (type == "pcba")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.PcbaFinalVendor.StartsWith(vendor)
                         group v by v.PcbaFinalVendor into y
                         select new ProjectMasterModel
                         {
                             PcbaFinalVendor = y.Key
                         }).ToList();
            }
            if (type == "tp")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.TpFinalVendor.StartsWith(vendor)
                         group v by v.TpFinalVendor into y
                         select new ProjectMasterModel
                         {
                             TpFinalVendor = y.Key
                         }).ToList();
            }
            if (type == "lcd")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.LcdFinalVendor.StartsWith(vendor)
                         group v by v.LcdFinalVendor into y
                         select new ProjectMasterModel
                         {
                             LcdFinalVendor = y.Key
                         }).ToList();
            }
            if (type == "earphone")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.EarphoneSupplierName.StartsWith(vendor)
                         group v by v.EarphoneSupplierName into y
                         select new ProjectMasterModel
                         {
                             EarphoneSupplierName = y.Key
                         }).ToList();
            }
            if (type == "charger")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.ChargerSupplierName.StartsWith(vendor)
                         group v by v.ChargerSupplierName into y
                         select new ProjectMasterModel
                         {
                             ChargerSupplierName = y.Key
                         }).ToList();
            }
            if (type == "battery")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.BatterySupplierName.StartsWith(vendor)
                         group v by v.BatterySupplierName into y
                         select new ProjectMasterModel
                         {
                             BatterySupplierName = y.Key
                         }).ToList();
            }
            if (type == "housing")
            {
                model = (from v in _dbEntities.ProjectMasters
                         where v.HousingFinalVendorName.StartsWith(vendor)
                         group v by v.HousingFinalVendorName into y
                         select new ProjectMasterModel
                         {
                             HousingFinalVendorName = y.Key
                         }).ToList();
            }
            return model;
        }
        #endregion

        //CAST(ase.OrderNumber AS VARCHAR(10) ) as OrderNumber//CAST(OrderNumber AS VARCHAR(10)) as 

        public bool UpdateBulkProject(BulkUpdateModel model)
        {
            try
            {
                long user = 0;
                long.TryParse(HttpContext.Current.User.Identity.Name, out user);
                long pId = 0;
                long.TryParse(model.ProjectId, out pId);
                ProjectMaster projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pId);
                var ll = model.ProjectOrders.Length;
                for (long orderNumber = 0; orderNumber < ll; orderNumber++)
                {
                    long ppId = 0;
                    long.TryParse(model.ProjectOrders[orderNumber], out ppId);
                    ProjectMaster master = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == ppId);
                    if (projectMaster != null)
                    {
                        master.ProjectName = projectMaster.ProjectName;
                        if (user != null) master.Updated = user;
                        //master.AddedDate = DateTime.Now;
                        //master.OrderNuber = projectMaster.OrderNuber == null ? 1 : projectMaster.OrderNuber + 1;
                        //master.ApproxProjectFinishDate = model.ApproximateFinishDateForReorder;
                        //master.ApproxProjectOrderDate = model.ApproximatePoDate;
                        //master.ApproxShipmentDate = model.ApproximateShipmentDate;
                        //master.IsScreenTestComplete = projectMaster.IsScreenTestComplete;
                        master.BackCamera = projectMaster.BackCamera;
                        master.Battery = projectMaster.Battery;
                        master.Chipset = projectMaster.Chipset;
                        master.DisplayName = projectMaster.DisplayName;
                        master.ProcessorClock = projectMaster.ProcessorClock;
                        master.DisplaySize = projectMaster.DisplaySize;
                        master.FrontCamera = projectMaster.FrontCamera;
                        //master.IsActive = true;
                        //master.NumberOfSample = projectMaster.NumberOfSample;
                        //master.IsApproved = projectMaster.IsApproved;
                        //master.IsNew = projectMaster.IsNew;
                        //master.IsProjectManagerAssigned = projectMaster.IsProjectManagerAssigned;
                        //master.IsScreenTestComplete = projectMaster.IsScreenTestComplete;
                        master.OsName = projectMaster.OsName;
                        master.OsVersion = projectMaster.OsVersion;
                        master.ProcessorName = projectMaster.ProcessorName;
                        master.ManagentComment = projectMaster.ManagentComment;
                        master.ProjectStatus = projectMaster.ProjectStatus;
                        master.SupplierName = projectMaster.SupplierName;
                        master.SupplierModelName = projectMaster.SupplierModelName;
                        master.SupplierTrustLevel = projectMaster.SupplierTrustLevel;
                        master.ProjectType = projectMaster.ProjectType;
                        master.Ram = projectMaster.Ram;
                        master.Rom = projectMaster.Rom;
                        master.SimSlotNumber = projectMaster.SimSlotNumber;
                        master.SlotType = projectMaster.SlotType;
                        //master.ApproximatePrice = model.ApproximatePrice;
                        master.BackCam = projectMaster.BackCam;
                        master.BackCamBsi = projectMaster.BackCamBsi;
                        master.BackCamSensor = projectMaster.BackCamSensor;
                        master.BateeryPossibleSupplierNames = projectMaster.BateeryPossibleSupplierNames;
                        master.BatteryCoverFinishingType = projectMaster.BatteryCoverFinishingType;
                        master.BatteryCoverLogoType = projectMaster.BatteryCoverLogoType;
                        master.BatteryRating = projectMaster.BatteryRating;
                        master.BatterySupplierName = projectMaster.BatterySupplierName;
                        master.BatteryType = projectMaster.BatteryType;
                        master.Cdma = projectMaster.Cdma;
                        master.ChargerRating = projectMaster.ChargerRating;
                        master.ChargerSupplierName = projectMaster.ChargerSupplierName;
                        master.ChipsetBit = projectMaster.ChipsetBit;
                        master.ChipsetCore = projectMaster.ChipsetCore;
                        master.ChipsetFrequency = projectMaster.ChipsetFrequency;
                        master.ChipsetName = projectMaster.ChipsetName;
                        master.Color = projectMaster.Color;
                        master.Compass = projectMaster.Compass;
                        master.CpuName = projectMaster.CpuName;
                        master.DisplayResulution = projectMaster.DisplayResulution;
                        master.DisplaySpeciality = projectMaster.DisplaySpeciality;
                        master.EarphoneConfirmPrice = projectMaster.EarphoneConfirmPrice;
                        master.EarphoneSupplierName = projectMaster.EarphoneSupplierName;
                        //master.FinalPrice = model.FinalPrice;
                        master.FlashLight = projectMaster.FlashLight;
                        master.FourthGenFdd = projectMaster.FourthGenFdd;
                        master.FourthGenTdd = projectMaster.FourthGenTdd;
                        master.ProjectNameForScreening = projectMaster.ProjectNameForScreening;
                        master.FrontCam = projectMaster.FrontCam;
                        master.FrontCamBsi = projectMaster.FrontCamBsi;
                        master.FrontCamSensor = projectMaster.FrontCamSensor;
                        master.GivenSampleToScreening = projectMaster.GivenSampleToScreening;
                        master.Gps = projectMaster.Gps;
                        master.Gsensor = projectMaster.Gsensor;
                        master.Gyroscope = projectMaster.Gyroscope;
                        master.HallSensor = projectMaster.HallSensor;
                        master.HousingFinalVendorName = projectMaster.HousingFinalVendorName;
                        master.HousingVendorName = projectMaster.HousingVendorName;
                        //master.IsReorder = true;
                        master.LcdFinalVendor = projectMaster.LcdFinalVendor;
                        master.LcdVendor = projectMaster.LcdVendor;
                        master.Lsensor = projectMaster.Lsensor;
                        master.MemoryBrandName = projectMaster.MemoryBrandName;
                        //master.OrderQuantity = projectMaster.OrderQuantity;
                        master.Otg = projectMaster.Otg;
                        master.OtgCable = projectMaster.OtgCable;
                        master.PcbaFinalVendor = projectMaster.PcbaFinalVendor;
                        master.PcbaVendorName = projectMaster.PcbaVendorName;
                        master.ProjectTypeId = projectMaster.ProjectTypeId;
                        master.Psensor = projectMaster.Psensor;
                        //master.RevisedStatus = projectMaster.RevisedStatus;
                        //master.ScreeningCommentFromCommercial = projectMaster.ScreeningCommentFromCommercial;
                        master.SecondGen = projectMaster.SecondGen;
                        //master.SourcingType = !string.IsNullOrWhiteSpace(orderForm.PoCategory) ? orderForm.PoCategory : projectMaster.SourcingType;
                        master.SpecialSensor = projectMaster.SpecialSensor;
                        master.SupplierId = projectMaster.SupplierId;
                        master.ThirdGen = projectMaster.ThirdGen;
                        master.ThreeLayerScreenProtector = projectMaster.ThreeLayerScreenProtector;
                        master.TpFinalVendor = projectMaster.TpFinalVendor;
                        master.TpVendor = projectMaster.TpVendor;
                        master.Updated = projectMaster.Updated;
                        master.UpdatedDate = projectMaster.UpdatedDate;
                        master.SwotAnalysisBy = projectMaster.SwotAnalysisBy;
                        master.SwotAnalysisDate = projectMaster.SwotAnalysisDate;
                        master.SwotOpportunityRemarks = projectMaster.SwotOpportunityRemarks;
                        //master.OrderQuantity = model.ProjectPurchaseOrderFormModel.Quantity;
                        _dbEntities.Entry(master).State = EntityState.Modified;
                        //===TRACEKR==
                        ProjectMasterTracker tracker = GenericMapper<ProjectMaster, ProjectMasterTracker>.GetDestination(master);
                        tracker.TrackerAddedDate = DateTime.Now;
                        _dbEntities.ProjectMasterTrackers.Add(tracker);
                        //-------

                        var log = new ProjectUpdateLog
                        {
                            Comment = "Updated from WPMS using Project bulk update system.",
                            UpdatedBy = user,
                            UpdatedDate = DateTime.Now,
                            UpdatedFromProjectMasterId = projectMaster.ProjectMasterId,
                            UpdatedProjectMasterId = master.ProjectMasterId
                        };
                        _dbEntities.Entry(log).State = EntityState.Added;
                    }
                }
                _dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public VmWarehouseEntry GetCommercialWarehouseEvent()
        {
            var model = new VmWarehouseEntry();
            var vmCommercialEvents = (from shipment in _dbEntities.ProjectOrderShipments
                                      join projectMaster in _dbEntities.ProjectMasters on shipment.ProjectMasterId equals
                                          projectMaster.ProjectMasterId
                                      join orderForm in _dbEntities.ProjectPurchaseOrderForms on projectMaster.ProjectMasterId equals
                                          orderForm.ProjectMasterId
                                      where shipment.WarehouseEntryDate != null
                                      select new VmCommercialEvent
                                      {
                                          //Month = shipment.WarehouseEntryDate!=null ? DateTime.Parse(shipment.WarehouseEntryDate.ToString()).ToShortDateString():"",
                                          ModelName = projectMaster.ProjectName,
                                          ProuductType = projectMaster.ProjectType,
                                          OrderNumber = projectMaster.OrderNuber.ToString(),
                                          OrderQuantity = projectMaster.OrderQuantity,
                                          OrderDate = orderForm.PoDate,
                                          InspectionDate = shipment.ChainaInspectionDate,
                                          WareHouseReceiveDate = shipment.WarehouseEntryDate
                                      }).ToList();
            foreach (var cmEvent in vmCommercialEvents)
            {
                cmEvent.Month = Convert.ToDateTime(cmEvent.WareHouseReceiveDate).ToString("MMM-yy");
                cmEvent.OrderNumber = CommonConversion.AddOrdinal(Convert.ToInt32(cmEvent.OrderNumber)) + " Order";
            }
            model.MonthList = vmCommercialEvents.Select(i => i.Month).Distinct().ToList();
            //model.ProductTypeList = vmCommercialEvents.Select(i => i.ProuductType).Distinct().ToList();
            //model.MonthlyTotal = vmCommercialEvents.Select(i => i.OrderQuantity).GroupBy(new { }).Sum().ToList();
            model.CommercialEvents = vmCommercialEvents;
            return model;
        }

        public VmWarehouseEntry GetCommercialWarehouseEventList(DateTime? fromDate, DateTime? toDate, string searchString)
        {
            var model = new VmWarehouseEntry();
            var vmCommercialEvents = (from shipment in _dbEntities.ProjectOrderShipments
                                      join projectMaster in _dbEntities.ProjectMasters on shipment.ProjectMasterId equals
                                          projectMaster.ProjectMasterId
                                      join orderForm in _dbEntities.ProjectPurchaseOrderForms on projectMaster.ProjectMasterId equals
                                          orderForm.ProjectMasterId
                                      where shipment.WarehouseEntryDate != null
                                      select new VmCommercialEvent
                                      {
                                          //Month = shipment.WarehouseEntryDate!=null ? DateTime.Parse(shipment.WarehouseEntryDate.ToString()).ToShortDateString():"",
                                          ModelName = projectMaster.ProjectName,
                                          ProuductType = projectMaster.ProjectType,
                                          OrderNumber = projectMaster.OrderNuber.ToString(),
                                          OrderQuantity = projectMaster.OrderQuantity,
                                          OrderDate = orderForm.PoDate,
                                          InspectionDate = shipment.ChainaInspectionDate,
                                          WareHouseReceiveDate = shipment.WarehouseEntryDate,
                                          PoWiseShipmentNumber = (int)shipment.PoWiseShipmentNumber,
                                          ProjectPurchaseOrderFormId = shipment.ProjectPurchaseOrderFormId
                                      }).ToList();
            foreach (var cmEvent in vmCommercialEvents)
            {
                cmEvent.Month = Convert.ToDateTime(cmEvent.WareHouseReceiveDate).ToString("MMM-yy");
                cmEvent.OrderNumber = CommonConversion.AddOrdinal(Convert.ToInt32(cmEvent.OrderNumber)) + " Order";
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                vmCommercialEvents = vmCommercialEvents.Where(m => m.ModelName.ToLower().Contains(searchString) || m.ProuductType.Contains(searchString)).ToList();
            }

            if (fromDate != null && fromDate > DateTime.MinValue)
                vmCommercialEvents = vmCommercialEvents.Where(m => m.WareHouseReceiveDate >= fromDate).ToList();
            if (toDate != null && toDate < DateTime.MaxValue)
                vmCommercialEvents = vmCommercialEvents.Where(m => m.WareHouseReceiveDate <= toDate).ToList();

            model.MonthList = vmCommercialEvents.OrderBy(x => x.WareHouseReceiveDate).Select(i => i.Month).Distinct().ToList();


            var summarydata = vmCommercialEvents.GroupBy(l => new { l.Month, l.ProuductType, l.ProjectPurchaseOrderFormId })
                .Select(gr => new TypeSummary
            {
                MonthName = gr.Key.Month,
                TypeName = gr.Key.ProuductType,
                ProjectPurchaseOrderFormId = gr.Key.ProjectPurchaseOrderFormId,
                TotalQty = (double)(gr.Sum(l => l.OrderQuantity) / gr.Count())
            });

            model.ProductTypeList = summarydata.GroupBy(l => new { l.MonthName, l.TypeName })
                .Select(cl => new TypeSummary
                {
                    MonthName = cl.Key.MonthName,
                    TypeName = cl.Key.TypeName,
                    TotalQty = (double)cl.Sum(c => c.TotalQty)
                }).ToList();

            model.CommercialEvents = vmCommercialEvents;

            return model;
        }

        public List<CreateFocForAftersalesPmModel> GetAftersalesPmFoc()
        {
            //            var nocList = _dbEntities.Database.SqlQuery<CreateFocForAftersalesPmModel>(@"select top 100 Id,ProjectId,ProjectName,SpareName,OrderNumber,PoDate,PoCategory,EmployeeCode,Supplier,FocConfirmedDate,Quantity,Remarks,InventoryEntryDate,UnitPrice,ShipmentQuantity, case when InventoryEntryDate is not null then CONVERT(varchar(12), DATENAME(MONTH, InventoryEntryDate)) else null end as MonthNames,
            //case when InventoryEntryDate is not null then DATEPART(mm,InventoryEntryDate) else null end as MonthNos,
            //case when InventoryEntryDate is not null then DATEPART(YEAR,InventoryEntryDate) else null end as Years  from  [CellPhoneProject].[dbo].[CreateFocForAftersalesPm]
            //order by Id desc").Take(100).ToList();
            var nocList = _dbEntities.Database.SqlQuery<CreateFocForAftersalesPmModel>(@"select top 100 Id,ProjectId,ProjectName,SpareName,OrderNumber,PoDate,PoCategory,EmployeeCode,Supplier,FocConfirmedDate,Quantity,Remarks,InventoryEntryDate,UnitPrice,ShipmentQuantity, case when InventoryEntryDate is not null then CONVERT(varchar(12), DATENAME(MONTH, InventoryEntryDate)) else null end as MonthNames,
case when InventoryEntryDate is not null then DATEPART(mm,InventoryEntryDate) else null end as MonthNos,
case when InventoryEntryDate is not null then DATEPART(YEAR,InventoryEntryDate) else null end as Years  from  [CellPhoneProject].[dbo].[CreateFocForAftersalesPm]
order by Id desc").Take(100).ToList();
            return nocList;
        }

        public string UpdateFocForAftersalesPm(VmAftersalesPmFoc focUpdate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updatedAssembly = (from c in _dbEntities.CreateFocForAftersalesPms
                                   where c.Id == focUpdate.Id
                                   select c).FirstOrDefault();

            updatedAssembly.InventoryEntryDate = focUpdate.InventoryEntryDate;
            updatedAssembly.UnitPrice = focUpdate.UnitPrice;
            updatedAssembly.ShipmentQuantity = focUpdate.ShipmentQuantity;
            updatedAssembly.Month = focUpdate.MonthNames;
            updatedAssembly.MonNum = Convert.ToInt32(focUpdate.MonthNos);
            updatedAssembly.Year = Convert.ToInt64(focUpdate.Years);
            updatedAssembly.Updated = userId;
            updatedAssembly.UpdatedDate = DateTime.Now;
            _dbEntities.CreateFocForAftersalesPms.AddOrUpdate(updatedAssembly);
            _dbEntities.SaveChanges();
            return "OK";
        }

        public List<VmPendingTac> GetPendingTacList()
        {
            var pendings =
                _dbEntities.ProjectMasters.Where(
                    i => !_dbEntities.BabtRaws.Select(j => j.ProjectName).Contains(i.ProjectName)).ToList();

            List<VmPendingTac> pendingTacs = pendings.Select(pending => new VmPendingTac { ProjectName = pending.ProjectName.Trim() }).ToList();
            pendingTacs = pendingTacs.OrderBy(i => i.ProjectName).DistinctBy(i => i.ProjectName).ToList();
            return pendingTacs;
        }

        public ProjectMasterModel GetProjectByName(string projectName)
        {
            ProjectMaster master = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectName == projectName);
            if (master != null)
            {
                var model = new ProjectMasterModel
                {
                    ProblemName = master.ProjectName,
                    ProjectMasterId = master.ProjectMasterId
                };
                return model;
            }
            return new ProjectMasterModel();
        }

        #region Incentive Upto August 2019
        public List<VmIncentivePolicy> GetVmIncentivePolicy()
        {
            string getinsPolicyQuery = string.Format(@"select * from [CellPhoneProject].[dbo].[IncentiveParameter] where IsActive=1");
            var getinsPolicies =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getinsPolicyQuery).ToList();
            return getinsPolicies;
        }
        public List<VmIncentivePolicy> GetIncentiveOrders(long monIds, long yearIds)
        {
            //            string getIncentiveOrdersQuery = string.Format(@"      
            //            select sum(Orders) as Orders, DATENAME(MONTH,GETDATE()) AS Month,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year from
            //            (SELECT count([ProjectPurchaseOrderFormId]) as Orders, DATENAME(MONTH,GETDATE()) as MM,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS YYYY
            //            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] where DATENAME(MONTH,AddedDate)=DATENAME(MONTH,GETDATE()) 
            //            and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) group by AddedDate) as C");

            //            string getIncentiveOrdersQuery = string.Format(@"      
            //            Select * from(
            //            SELECT count([ProjectPurchaseOrderFormId]) as Orders,
            //            DATEPART(mm,AddedDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, AddedDate)) as MonName,
            //            CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
            //            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms]
            //            where DATEPART(mm,AddedDate)={0}
            //            and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) 
            //            group by DATEPART(mm,AddedDate),CONVERT(varchar(12),DATENAME(MONTH, AddedDate))
            //            ) as C",monIds);

            //            string getIncentiveOrdersQuery = string.Format(@"      
            //            Select * from(
            //            SELECT count([ProjectPurchaseOrderFormId]) as Orders,
            //            DATEPART(mm,AddedDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, AddedDate)) as MonName,
            //            CONVERT(varchar(4),DATEPART(yy, AddedDate)) as Year
            //            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms]
            //            where DATEPART(mm,AddedDate)={0}
            //            and DATENAME(YEAR,AddedDate)={1}
            //            group by DATEPART(mm,AddedDate),CONVERT(varchar(12),DATENAME(MONTH, AddedDate)),DATEPART(yy, AddedDate)
            //            ) as C", monIds, yearIds);
            _dbEntities.Database.CommandTimeout = 6000;
            string getIncentiveOrdersQuery = string.Format(@"      
            Select * from(
            SELECT count([ProjectPurchaseOrderFormId]) as Orders,
            DATEPART(mm,PoDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, PoDate)) as MonName,
            CONVERT(varchar(4),DATEPART(yy, PoDate)) as Year
            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] pf 
            inner join [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterId=pf.ProjectMasterId 
            where DATEPART(mm,PoDate)={0}
            and DATENAME(YEAR,PoDate)={1} and pm.IsActive=1
            group by DATEPART(mm,PoDate),CONVERT(varchar(12),DATENAME(MONTH, PoDate)),DATEPART(yy, PoDate)
            ) as C", monIds, yearIds);

            var getIncentiveOrders =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveOrdersQuery).ToList();
            return getIncentiveOrders;
        }

        public List<VmIncentivePolicy> GetIncentiveLcs(long monIds, long yearIds)
        {
            _dbEntities.Database.CommandTimeout = 6000;
            string getLcForIncentive = string.Format(@"Select * from(
            SELECT count(ProjectLcId) as PerLc,
            DATEPART(mm,LcPassDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, LcPassDate)) as MonName,
            CONVERT(varchar(4),DATEPART(yy, LcPassDate)) as Year
            from [CellPhoneProject].[dbo].[ProjectLcs]
            where DATEPART(mm,LcPassDate)={0}
            and DATENAME(YEAR,LcPassDate)={1}
            group by DATEPART(mm,LcPassDate),CONVERT(varchar(12),DATENAME(MONTH, LcPassDate)),DATEPART(yy, LcPassDate)
            ) as C", monIds, yearIds);

            var getLcs = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getLcForIncentive).ToList();
            return getLcs;
        }

        public List<VmIncentivePolicy> GetPrimarySales(long monIds, long yearIds)
        {        

            string oradb = "Data Source=(DESCRIPTION="
                                        + "(ADDRESS=(PROTOCOL=TCP)(HOST=test)(PORT=1test521))"
                                        + "(CONNECT_DATA=(SERVICE_NAME=PROD)));"
                                        + "User Id=test;Password=test#;";
        
            OracleConnection con = new OracleConnection();
            OracleCommand cmd = new OracleCommand();          

            con.ConnectionString = "Data Source=(DESCRIPTION="
                                           + "(ADDRESS=(PROTOCOL=TCP)(HOST=test)(PORT=test))"
                                           + "(CONNECT_DATA=(SERVICE_NAME=test)));"
                                           + "User Id=test;Password=test#;";
            con.Open();
            cmd.CommandText = string.Format(@"SELECT YEAR,MONTH, SUM(SALES_AMT) SALES_AMT FROM (SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=86 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {1} GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) UNION ALL  SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=223 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {1} GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) ) GROUP BY YEAR,MONTH", monIds, yearIds);
            cmd.Connection = con;
            cmd.CommandTimeout = 6000;

            OracleDataReader dr = cmd.ExecuteReader();

            var incentives = new List<VmIncentivePolicy>();
            var getIncentiveOrders1 = new VmIncentivePolicy();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    getIncentiveOrders1 = new VmIncentivePolicy
                    {
                        Year = (dr["YEAR"]).ToString(),
                        Month = (dr["MONTH"]).ToString(),
                        SalesAmt = Convert.ToDecimal(dr["SALES_AMT"])

                    };
                    incentives.Add(getIncentiveOrders1);
                }
            }
            return incentives;
        }

        public List<VmIncentivePolicy> GetFeaturePhoneService(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            var featureServiceList = new List<VmIncentivePolicy>();
            var featurePhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //                string getFeaturePhoneServiceQuery = string.Format(@"      
                //                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,CONVERT(varchar(12),
                //                 DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
                //                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0}
                //                and DATENAME(YEAR,sm.ServicePlaceDate)=DATENAME(YEAR,GETDATE()) and sm.Model like '%Olvio%'
                //                group by DATEPART(mm,sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds);
                string getFeaturePhoneServiceQuery = string.Format(@"      
                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,
                CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, sm.ServicePlaceDate)) as Year
                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0}
                and DATENAME(YEAR,sm.ServicePlaceDate)={1}
                and (sm.Model like '%Olvio%' or sm.Model like '%Classic%' or sm.Model like '%Excel%')
                group by DATEPART(mm,sm.ServicePlaceDate),DATEPART(yy, sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds, yearIds);
                var command = new SqlCommand(getFeaturePhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    featurePhone = new VmIncentivePolicy
                    {
                        TotalServiceIMEI = reader["ServiceIMEI"].ToString(),
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    featureServiceList.Add(featurePhone);
                }
                connection.Close();

            }
            return featureServiceList;
        }

        public List<VmIncentivePolicy> GetFeaturePhoneSales(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var featureServiceList = new List<VmIncentivePolicy>();
            var featurePhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //                string getFeaturePhoneServiceQuery = string.Format(@"           
                //                select * from
                //                (SELECT count(distinct tdd.BarCode) as BarCode, DATEPART(mm,tdd.DistributionDate) as MonNum,CONVERT(varchar(12),
                //                 DATENAME(MONTH, tdd.DistributionDate)) as MonName,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
                //                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATEPART(mm,tdd.DistributionDate)={0}
                //                and DATENAME(YEAR,tdd.DistributionDate)=DATENAME(YEAR,GETDATE()) and tdd.Model like '%Olvio%' 
                //                group by DATEPART(mm,tdd.DistributionDate),CONVERT(varchar(12),DATENAME(MONTH, tdd.DistributionDate))) as C", monIds);

                string getFeaturePhoneServiceQuery = string.Format(@"           
                select * from
                (SELECT count(distinct tdd.BarCode) as BarCode, DATEPART(mm,tdd.DistributionDate) as MonNum,CONVERT(varchar(12),
                DATENAME(MONTH, tdd.DistributionDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, tdd.DistributionDate)) as Year
                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATEPART(mm,tdd.DistributionDate)={0}
                and DATENAME(YEAR,tdd.DistributionDate)={1} and (tdd.Model like '%Olvio%' or tdd.Model like '%Classic%' or tdd.Model like '%Excel%')
                group by DATEPART(mm,tdd.DistributionDate),DATEPART(yy, tdd.DistributionDate),CONVERT(varchar(12),DATENAME(MONTH, tdd.DistributionDate))) as C", monIds, yearIds);

                var command = new SqlCommand(getFeaturePhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    featurePhone = new VmIncentivePolicy
                    {
                        TotalSalesBarcode = reader["BarCode"].ToString(),
                        //Month = reader["Month"].ToString(),
                        //Year = reader["Year"].ToString()
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    featureServiceList.Add(featurePhone);
                }
                connection.Close();

            }
            return featureServiceList;
        }

        public List<VmIncentivePolicy> GetSmartPhoneService(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            var smartServiceList = new List<VmIncentivePolicy>();
            var smartPhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //                string getSmartPhoneServiceQuery = string.Format(@"      
                //                select sum(IMEI) as ServiceIMEI, DATENAME(MONTH,GETDATE()) AS Month,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year from
                //                (SELECT count(distinct sm.IME) as IMEI, DATENAME(MONTH,GETDATE()) as MM,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS YYYY
                //                from WSMS.[dbo].ServiceMaster sm where DATENAME(MONTH,sm.ServicePlaceDate)=DATENAME(MONTH,GETDATE()) 
                //                and DATENAME(YEAR,sm.ServicePlaceDate)=DATENAME(YEAR,GETDATE()) 
                //                and (sm.Model like '%Primo%' or sm.Model like '%Walpad%')
                //                group by sm.ServicePlaceDate) as C");
                //                string getSmartPhoneServiceQuery = string.Format(@"      
                //                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,CONVERT(varchar(12),
                //                DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
                //                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0}
                //                and DATENAME(YEAR,sm.ServicePlaceDate)=DATENAME(YEAR,GETDATE())  and (sm.Model like '%Primo%' or sm.Model like '%Walpad%')
                //                group by DATEPART(mm,sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds);

                string getSmartPhoneServiceQuery = string.Format(@"      
                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,CONVERT(varchar(12),
                DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, sm.ServicePlaceDate)) as Year
                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0} and DATENAME(YEAR,sm.ServicePlaceDate)={1}
                and (sm.Model like '%Primo%' or sm.Model like '%Walpad%')
                group by DATEPART(mm,sm.ServicePlaceDate),DATEPART(yy, sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds, yearIds);
                var command = new SqlCommand(getSmartPhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    smartPhone = new VmIncentivePolicy
                    {
                        TotalServiceIMEI = reader["ServiceIMEI"].ToString(),
                        //Month = reader["Month"].ToString(),
                        //Year = reader["Year"].ToString()
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    smartServiceList.Add(smartPhone);
                }
                connection.Close();

            }
            return smartServiceList;
        }
        public List<VmIncentivePolicy> GetSmartPhoneSales(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var smartServiceList = new List<VmIncentivePolicy>();
            var smartPhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //                string getSmartPhoneServiceQuery = string.Format(@"      
                //                select sum(BarCode) as BarCode, DATENAME(MONTH,GETDATE()) AS Month,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year from
                //                (SELECT count(distinct tdd.BarCode) as BarCode, DATENAME(MONTH,GETDATE()) as MM,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS YYYY
                //                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATENAME(MONTH,tdd.DistributionDate)=DATENAME(MONTH,GETDATE()) 
                //                and DATENAME(YEAR,tdd.DistributionDate)=DATENAME(YEAR,GETDATE()) and (tdd.Model like '%Primo%' or tdd.Model like '%Walpad%') group by tdd.DistributionDate) as C");

                string getSmartPhoneServiceQuery = string.Format(@"      
                select * from
                (SELECT count(distinct tdd.BarCode) as BarCode, DATEPART(mm,tdd.DistributionDate) as MonNum,CONVERT(varchar(12),
                 DATENAME(MONTH, tdd.DistributionDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, tdd.DistributionDate)) as Year
                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATEPART(mm,tdd.DistributionDate)={0}
               and DATENAME(YEAR, tdd.DistributionDate)={1} and (tdd.Model like '%Primo%' or tdd.Model like '%Walpad%')
                group by DATEPART(mm,tdd.DistributionDate),DATEPART(yy, tdd.DistributionDate),CONVERT(varchar(12),DATENAME(MONTH, tdd.DistributionDate))) as C", monIds, yearIds);

                var command = new SqlCommand(getSmartPhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    smartPhone = new VmIncentivePolicy
                    {
                        TotalSalesBarcode = reader["BarCode"].ToString(),
                        //Month = reader["Month"].ToString(),
                        //Year = reader["Year"].ToString()
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    smartServiceList.Add(smartPhone);
                }
                connection.Close();

            }
            return smartServiceList;
        }

        public List<VmIncentivePolicy> GetCmUserList(long monIds, long yearIds)
        {

            //if (monIds == 1)
            //{
            //    monIds = 12;
            //    yearIds = yearIds - 1;
            //}
            //else if(monIds==0 && yearIds==0)
            //{
            //    monIds = 0;
            //    yearIds = 0;
            //}
            //else
            //{
            //    monIds = monIds - 1;
            //    yearIds = yearIds;
            //}
            _dbEntities.Database.CommandTimeout = 6000;

            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename in ('CM','CMHEAD') and cm.IsActive=1 and iss.Category=1
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }
        public List<VmIncentivePolicy> GetCmUserList1(long monIds, long yearIds)
        {
            _dbEntities.Database.CommandTimeout = 6000;
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename='CM' and cm.IsActive=1
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public List<VmIncentivePolicy> GetCmUserList2(long monIds, long yearIds)
        {
            _dbEntities.Database.CommandTimeout = 6000;
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename='CM' and cm.IsActive=1 and iss.Category=2
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public List<VmIncentivePolicy> GetCmUserList3(long monIds, long yearIds)
        {
            _dbEntities.Database.CommandTimeout = 6000;
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename in ('CM','CMHEAD') and cm.IsActive=1 
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }
        public string GetSaveIncentive(string month, string monNum, string year, string totalAmount, List<Incentive> results, List<CmIncentiveModel> results2)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var existing = _dbEntities.Incentives.Where(p => month.Contains(p.Month)).ToList();

            foreach (var insResult in results)
            {
                // if (existing.FirstOrDefault(p => p.Month == month && p.Year == Convert.ToInt64(year) && p.DepartmentName.Contains("CM")) == null)
                //  {

                bool isExist = CheckCmIncentive(monNum, year, insResult.Id);

                if (!isExist)
                {
                    var model = new DAL.DbModel.Incentive
                    {
                        Month = month,
                        MonNum = Convert.ToInt32(monNum),
                        Year = Convert.ToInt64(year),
                        TotalAmount = Convert.ToDecimal(totalAmount),
                        ThisMonthAmount = Convert.ToDecimal(insResult.ThisMonthAmount),
                        Amount = Convert.ToDecimal(insResult.FinalAmount),
                        UserId = insResult.Id,
                        DepartmentName = insResult.Role,
                        Percentage = Convert.ToInt64(insResult.Percentage),
                        TotalIncentive = Convert.ToDecimal(insResult.Incentives),
                        FixedIncentive = insResult.FixedIncentive,
                        AddedAmount = insResult.AddedAmount,
                        AmountDeduction = insResult.AmountDeduction,
                        Remarks = insResult.Remarks,
                        DeductionRemarks = insResult.DeductionRemarks,
                        AmountCarry = Convert.ToInt64(insResult.CarryOver),
                        TotalReward = Convert.ToDecimal(insResult.TotalReward),
                        Reward = Convert.ToDecimal(insResult.Reward),
                        TotalPenalties = Convert.ToDecimal(insResult.TotalPenalties),
                        Penalties = Convert.ToDecimal(insResult.Penalties),

                        Added = userId,
                        AddedDate = DateTime.Now
                    };
                    _dbEntities.Incentives.AddOrUpdate(model);
                    _dbEntities.SaveChanges();
                }

                // }
            }
            foreach (var insResult in results2)
            {
                bool isExist = CheckCmMaterialPass(monNum, year, insResult.ProjectMasterId);

                if (!isExist)
                {
                    var mods = new Cm_MaterialPassChinaIqcIncentive();
                    mods.ProjectMasterId = insResult.ProjectMasterId;
                    mods.ProjectName = insResult.ProjectName;
                    mods.Orders = insResult.Orders;
                    mods.PoCategory = insResult.PoCategory;
                    mods.PoQuantity = insResult.PoQuantity;
                    mods.LotNumber = insResult.LotNumber;
                    mods.LotQuantity = insResult.LotQuantity;
                    mods.ProjectManagerClearanceDate = insResult.ProjectManagerClearanceDate;
                    mods.ChinaIqcPassHundredPercent = insResult.ChinaIqcPassHundredPercent;
                    mods.NoOfTimeInspection = insResult.NoOfTimeInspection;
                    mods.Amount = insResult.Amount;
                    mods.MonNum = Convert.ToInt32(monNum);
                    mods.Year = Convert.ToInt64(year);
                    mods.Added = userId;
                    mods.AddedDate = DateTime.Now;
                    _dbEntities.Cm_MaterialPassChinaIqcIncentive.AddOrUpdate(mods);
                    _dbEntities.SaveChanges();
                }

            }
            _dbEntities.SaveChanges();
            return "ok";
        }

        public bool CheckCmMaterialPass(string monNum, string year, long? projectMasterId)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<VmIncentivePolicy> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year) from [CellPhoneProject].[dbo].[Cm_MaterialPassChinaIqcIncentive] where ProjectMasterId={2} and Year={1} and  MonNum={0}", MonNum, year, projectMasterId);
                getIncentiveReports =
                   _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveCmPenaltiesAndRewardData(string month, string monNum, string year)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            long monIds;
            long.TryParse(monNum, out monIds);

            long yearIds;
            long.TryParse(year, out yearIds);
            _dbEntities.Database.CommandTimeout = 6000;

            var ckdSkdDetails = _dbEntities.CmPenaltiesAndRewardCkdSkdDetails(monIds, yearIds).ToList();
            foreach (var details in ckdSkdDetails)
            {
                var model = new Cm_CkdSkdRewardAndPenalties();
                model.ProjectMasterID = details.ProjectMasterID;
                model.ProjectModel = details.ProjectName;
                model.ProjectType = details.ProjectType;
                model.ShipmentType = details.ShipmentType;
                model.Orders = details.Orders;
                model.PoDate = details.PoDate;
                model.WarehouseEntryDate = details.WarehouseEntryDate;
                model.DaysDiff = Convert.ToInt32(details.DaysDiff);
                model.EffectiveDays = Convert.ToInt32(details.EffectiveDays);
                model.DeductPoint = Convert.ToInt32(details.DeductPoint);
                model.DaysDiffForDeduct = Convert.ToInt32(details.DaysDiffForDeduct);
                model.AmountDeduct = details.AmountDeduct;
                model.RewardPoint = Convert.ToInt32(details.RewardPoint);
                model.DaysDiffForReward = Convert.ToInt32(details.DaysDiffForReward);
                model.AmountReward = details.AmountReward;
                model.IsFinalShipment = details.IsFinalShipment;
                model.MonNum = Convert.ToInt32(monNum);
                model.Year = Convert.ToInt64(year);
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Cm_CkdSkdRewardAndPenalties.AddOrUpdate(model);
            }

            var repeatDetails = _dbEntities.CmPenaltiesAndRewardRepeatDetails(monIds, yearIds);
            foreach (var details in repeatDetails)
            {
                var model = new Cm_CkdSkdRewardAndPenalties();
                model.ProjectMasterID = details.ProjectMasterID;
                model.ProjectModel = details.ProjectName;
                model.ProjectType = details.ProjectType;
                model.ShipmentType = details.ShipmentType;
                model.Orders = details.Orders;
                model.PoDate = details.PoDate;
                model.WarehouseEntryDate = details.WarehouseEntryDate;
                model.DaysDiff = Convert.ToInt32(details.DaysDiff);
                model.EffectiveDays = Convert.ToInt32(details.EffectiveDays);
                model.DeductPoint = Convert.ToInt32(details.DeductPoint);
                model.DaysDiffForDeduct = Convert.ToInt32(details.DaysDiffForDeduct);
                model.AmountDeduct = details.AmountDeduct;
                model.RewardPoint = Convert.ToInt32(details.RewardPoint);
                model.DaysDiffForReward = Convert.ToInt32(details.DaysDiffForReward);
                model.AmountReward = details.AmountReward;
                model.IsFinalShipment = details.IsFinalShipment;
                model.MonNum = Convert.ToInt32(monNum);
                model.Year = Convert.ToInt64(year);
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Cm_CkdSkdRewardAndPenalties.AddOrUpdate(model);
            }


            string proEv14 = string.Format(@"select D.ProjectMasterID,D.ProjectModel,D.SourcingType,D.WpmsOrders,D.WarehouseEntryDate,D.ExtendedWarehouseDate,cast(D.OrderQuantity as bigint) as OrderQuantity,cast(D.TotalProductionQuantity as bigint) as TotalProductionQuantity,
            cast(D.EffectiveDays as bigint) as EffectiveDays,cast(D.RewardPercentage as bigint) as RewardPercentage,cast(D.ExistedPercentage as bigint) as ExistedPercentage,cast(D.RewardAmount as bigint) as RewardAmount
            from
            (select distinct C.ProjectMasterID,C.ProjectModel,C.SourcingType,C.WpmsOrders,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.OrderQuantity,C.TotalProductionQuantity,C.EffectiveDays,C.RewardPercentage,C.ExistedPercentage,
            case when C.ExistedPercentage>=C.RewardPercentage then 2100 else 0 end as RewardAmount
            from
            (
            select B.ProjectMasterID,B.ProjectModel,B.SourcingType,B.WpmsOrders,B.WarehouseEntryDate,B.ExtendedWarehouseDate,B.OrderQuantity,B.TotalProductionQuantity,B.EffectiveDays,B.RewardPercentage,
            ((100 * B.TotalProductionQuantity)/OrderQuantity) as ExistedPercentage,B.IsFinalShipment
		            from 
			            (
				            select A.ProjectMasterID,A.ProjectModel,A.SourcingType,A.WpmsOrders,A.WarehouseEntryDate,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity,count(tbi.Barcode) as TotalProductionQuantity,RewardPercentage=95,A.EffectiveDays
				            from 
				            (
					            select AA.ProjectMasterID,AA.ProjectModel,AA.SourcingType,AA.WpmsOrders,AA.WarehouseEntryDate,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate) as ExtendedWarehouseDate,AA.IsFinalShipment,AA.OrderQuantity,AA.EffectiveDays from
					            (
						            select distinct ps.ProjectMasterID,pdd.ProjectModel,pm.SourcingType,('Order '+ cast(pm.OrderNuber as varchar(10))) as WpmsOrders,ps.WarehouseEntryDate,ps.IsFinalShipment,pdd.OrderQuantity,case when pm.SourcingType='SKD' then 30  when  pm.SourcingType='CKD' then 45 end as EffectiveDays
						            from [CellPhoneProject].[dbo].[ProjectOrderShipments] ps
						            left join [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterID=ps.ProjectMasterID
						            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pdd.ProjectMasterID=ps.ProjectMasterID
						            where pm.IsActive=1	and				
							            ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
					            )AA where DATEPART(mm,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))={0} and  DATENAME(YEAR,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))={1}

				            )A
				            left join RBSYNERGY.dbo.tblBarcodeInv tbi on tbi.UpdatedBy=A.WpmsOrders  and tbi.Model=A.ProjectModel
				            where tbi.Model=A.ProjectModel and PrintDate between A.WarehouseEntryDate and DATEADD(day, A.EffectiveDays, A.WarehouseEntryDate)
				            group by  A.ProjectMasterID,A.ProjectModel,A.WpmsOrders,A.WarehouseEntryDate,A.IsFinalShipment,A.OrderQuantity,A.SourcingType,A.EffectiveDays,A.ExtendedWarehouseDate
		            )B

            )C where C.ExistedPercentage>=C.RewardPercentage)D", monIds, yearIds);

            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();
            foreach (var details in proEvent14)
            {
                var model = new Cm_CkdSkdRewardAndPenalties();
                model.ProjectMasterID = details.ProjectMasterID;
                model.ProjectModel = details.ProjectName;
                model.ProjectType = details.ProjectType;
                model.ShipmentType = details.ShipmentType;
                model.SourcingType = details.SourcingType;
                model.Orders = details.WpmsOrders;
                model.PoDate = details.PoDate;
                model.WarehouseEntryDate = details.WarehouseEntryDate;
                model.ExtendedWarehouseDate = details.ExtendedWarehouseDate;
                model.DaysDiff = Convert.ToInt32(details.DaysDiff);
                model.OrderQuantity = Convert.ToInt32(details.OrderQuantity);
                model.RewardPercentage = Convert.ToInt32(details.RewardPercentage);
                model.ExistedPercentage = Convert.ToInt32(details.ExistedPercentage);
                model.TotalProductionQuantity = Convert.ToInt32(details.TotalProductionQuantity);
                model.EffectiveDays = Convert.ToInt32(details.EffectiveDays);
                model.DeductPoint = Convert.ToInt32(details.DeductPoint);
                model.DaysDiffForDeduct = Convert.ToInt32(details.DaysDiffForDeduct);
                model.AmountDeduct = details.AmountDeduct;
                model.RewardPoint = Convert.ToInt32(details.RewardPoint);
                model.DaysDiffForReward = Convert.ToInt32(details.DaysDiffForReward);
                model.AmountReward = details.RewardAmount;
                model.IsFinalShipment = details.IsFinalShipment;
                model.MonNum = Convert.ToInt32(monNum);
                model.Year = Convert.ToInt64(year);
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Cm_CkdSkdRewardAndPenalties.AddOrUpdate(model);
            }

            string proSales = string.Format(@"select distinct E.ProjectmasterID,E.ProjectModel,E.Orders,E.tblBarcodeOrder,E.WarehouseEntryDate,E.ExtendedWarehouseDate,cast(E.EffectiveDays as bigint) as EffectiveDays,cast(E.OrderQuantity as bigint) as OrderQuantity,
            cast(E.TotalTblBarcodeIMEI as bigint) as TotalTblBarcodeIMEI,cast(E.TotalSalesOut as bigint) as TotalSalesOut,cast(E.RewardPercentage as bigint) as RewardPercentage,
            cast(E.ExistedPercentage as bigint) as ExistedPercentage, cast(E.RewardAmount as bigint) as RewardAmount
	            from 
	            ( 
	             SELECT D.ProjectmasterID,D.ProjectModel,D.Orders,D.tblBarcodeOrder,D.WarehouseEntryDate,D.ExtendedWarehouseDate,D.EffectiveDays,D.OrderQuantity, D.TotalTblBarcodeIMEI,D.TotalSalesOut,D.RewardPercentage,D.ExistedPercentage,
	             case when D.ExistedPercentage>=D.RewardPercentage then 3500 else 0 end as RewardAmount
	              FROM
	               (
		              select C.ProjectmasterId,C.ProjectModel,C.Orders,C.tblBarcodeOrder,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.EffectiveDays,C.OrderQuantity, C.TotalTblBarcodeIMEI,C.TotalSalesOut,C.RewardPercentage,
		              ((100 * C.TotalSalesOut)/OrderQuantity) as ExistedPercentage,IsFinalShipment  from
			            ( 
			               select B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,B.ExtendedWarehouseDate,EffectiveDays=120, sum(TotalTblBarcodeIMEI) as TotalTblBarcodeIMEI,sum(TotalSalesOut) as TotalSalesOut,RewardPercentage=95,IsFinalShipment,B.OrderQuantity  from
					            ( 
					               select A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.ExtendedWarehouseDate, count(A.Barcode) as TotalTblBarcodeIMEI,case when A.TddBarcode is not null and A.TddBarcode !='' then 1 else 0 end as TotalSalesOut,IsFinalShipment,A.OrderQuantity  from
							             (
								             select distinct proM.ProjectMasterId,proM.ProjectModel,proM.Orders,proM.ShipmentType,proM.WarehouseEntryDate,proM.ExtendedWarehouseDate,proM.ShipmentPercentage,proM.IsFinalShipment,
								             tbl.Model,tbl.Barcode,tbl.Barcode2,tbl.DateAdded,tbl.UpdatedBy as tblBarcodeOrder,tdd.Barcode as TddBarcode,proM.OrderQuantity from 
								            (
										            select distinct ps.ProjectMasterId,pdd.ProjectModel, ('Order '+ cast(pm.OrderNuber as varchar(10))) as Orders,ps.ShipmentType,ps.WarehouseEntryDate,DATEADD(day, 120, ps.WarehouseEntryDate) AS ExtendedWarehouseDate,ps.ShipmentPercentage,ps.IsFinalShipment,pdd.OrderQuantity
										            FROM [CellPhoneProject].[dbo].[ProjectOrderShipments] ps 
										            left join CellphoneProject.dbo.ProjectMasters pm on ps.ProjectMasterId=pm.ProjectMasterId
										            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pm.ProjectMasterID=pdd.ProjectMasterID
										            where DATEPART(mm,DATEADD(day, 120, ps.WarehouseEntryDate))={0} and  DATENAME(YEAR,DATEADD(day, 120, ps.WarehouseEntryDate))={1} and  pm.IsActive=1	and	
										            ps.WarehouseEntryDate in (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
								            )proM
								            left join [RBSYNERGY].[dbo].[tblBarCodeInv] tbl on proM.ProjectModel=tbl.Model and RTRIM(tbl.UpdatedBy)=RTRIM(proM.Orders)
								            left join [RBSYNERGY].[dbo].tblDealerDistributionDetails tdd on tbl.Barcode =tdd.Barcode and
								            tdd.DistributionDate between proM.WarehouseEntryDate and  DATEADD(day, 120, proM.WarehouseEntryDate)

								            where proM.ProjectModel=tbl.Model 
							            )A
						            group by A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.Barcode,A.TddBarcode,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity
					             )B
					             group by B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,ExtendedWarehouseDate,IsFinalShipment,B.OrderQuantity
			               )C 
	               )D 
   
               )E where E.RewardAmount>0", monIds, yearIds);

            var proSalesList = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proSales).ToList();
            foreach (var details in proSalesList)
            {
                var model = new Cm_CkdSkdRewardAndPenalties();
                model.ProjectMasterID = details.ProjectMasterID;
                model.ProjectModel = details.ProjectName;
                model.ProjectType = details.ProjectType;
                model.ShipmentType = details.ShipmentType;
                model.SourcingType = details.SourcingType;
                model.Orders = details.WpmsOrders;
                model.PoDate = details.PoDate;
                model.WarehouseEntryDate = details.WarehouseEntryDate;
                model.ExtendedWarehouseDate = details.ExtendedWarehouseDate;
                model.DaysDiff = Convert.ToInt32(details.DaysDiff);
                model.OrderQuantity = Convert.ToInt32(details.OrderQuantity);
                model.RewardPercentage = Convert.ToInt32(details.RewardPercentage);
                model.ExistedPercentage = Convert.ToInt32(details.ExistedPercentage);
                model.TotalProductionQuantity = Convert.ToInt32(details.TotalProductionQuantity);
                model.EffectiveDays = Convert.ToInt32(details.EffectiveDays);
                model.DeductPoint = Convert.ToInt32(details.DeductPoint);
                model.DaysDiffForDeduct = Convert.ToInt32(details.DaysDiffForDeduct);
                model.AmountDeduct = details.AmountDeduct;
                model.RewardPoint = Convert.ToInt32(details.RewardPoint);
                model.DaysDiffForReward = Convert.ToInt32(details.DaysDiffForReward);
                model.AmountReward = details.RewardAmount;
                model.IsFinalShipment = details.IsFinalShipment;
                model.TotalTblBarcodeIMEI = details.TotalTblBarcodeIMEI;
                model.TotalSalesOut = details.TotalSalesOut;
                model.MonNum = Convert.ToInt32(monNum);
                model.Year = Convert.ToInt64(year);
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Cm_CkdSkdRewardAndPenalties.AddOrUpdate(model);
            }

            _dbEntities.SaveChanges();

            return "ok";
        }

        public string SaveCmOthersIncentive(List<Cm_OthersIncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model = new Cm_OthersIncentive();

                model.OthersType = insResult.OthersType;
                model.Amount = insResult.Amount;
                model.Remarks = insResult.Remarks;
                model.DeductAmount = insResult.DeductAmount;
                model.DeductRemarks = insResult.DeductRemarks;
                model.EffectiveMonth = insResult.EffectiveMonth;
                model.FinalAmount = insResult.FinalAmount;
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Cm_OthersIncentive.Add(model);
            }
            _dbEntities.SaveChanges();

            return "ok";
        }
        public bool CheckCmIncentive(string monId, string yearId, string epmId)
        {
            int MonNum = Convert.ToInt32(monId);
            List<VmIncentivePolicy> getIncentiveReports = null;
            if (MonNum > 0 && yearId != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year) from [CellPhoneProject].[dbo].[Incentive] where UserId={2} and Year={1} and  MonNum={0}", MonNum, yearId, epmId);
                getIncentiveReports =
                   _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string GetSaveIncentive21(string month, string monNum, string year, string totalAmount21, List<Incentive> results21, List<Incentive> results31)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var existing = _dbEntities.Incentives.Where(p => month.Contains(p.Month)).ToList();

            foreach (var insResult in results21)
            {
                bool isExist = CheckCmIncentive(monNum, year, insResult.EmployeeCode);

                if (!isExist)
                {
                    var model = new DAL.DbModel.Incentive
                    {
                        Month = month,
                        MonNum = Convert.ToInt32(monNum),
                        Year = Convert.ToInt64(year),
                        UserId = insResult.EmployeeCode,
                        DepartmentName = insResult.Role,
                        Percentage = Convert.ToInt64(insResult.Percentage),
                        FixedIncentive = insResult.FixedIncentive,
                        AddedAmount = Convert.ToDecimal(insResult.AddedAmount),
                        Remarks = insResult.Remarks,
                        AmountDeduction = Convert.ToDecimal(insResult.AmountDeduction),
                        DeductionRemarks = insResult.DeductionRemarks,
                        AmountCarry = Convert.ToInt64(insResult.CarryOver),

                        TotalReward = Convert.ToDecimal(insResult.TotalReward),
                        Reward = Convert.ToDecimal(insResult.Reward),
                        TotalPenalties = Convert.ToDecimal(insResult.TotalPenalties),
                        Penalties = Convert.ToDecimal(insResult.Penalties),

                        TotalAmount = Convert.ToDecimal(insResult.TotalReward) - Convert.ToDecimal(insResult.TotalPenalties),
                        ThisMonthAmount = Convert.ToDecimal(insResult.TotalReward) - Convert.ToDecimal(insResult.TotalPenalties),
                        Amount = (Convert.ToDecimal(insResult.Reward) + Convert.ToDecimal(insResult.AddedAmount)) - (Convert.ToDecimal(insResult.Penalties) + Convert.ToDecimal(insResult.AmountDeduction)),
                        TotalIncentive = (Convert.ToDecimal(insResult.Reward) + Convert.ToDecimal(insResult.AddedAmount)) - (Convert.ToDecimal(insResult.Penalties) + Convert.ToDecimal(insResult.AmountDeduction)),

                        SpecialAmount = insResult.SpecialAmount,
                        SpecialRemarks = insResult.SpecialRemarks,
                        Added = userId,
                        AddedDate = DateTime.Now
                    };
                    _dbEntities.Incentives.AddOrUpdate(model);
                }
            }

            foreach (var insResult in results31)
            {
                bool isExist = CheckCmIncentive(monNum, year, insResult.EmployeeCode);
                if (!isExist)
                {
                    var model = new DAL.DbModel.Incentive
                    {
                        Month = month,
                        MonNum = Convert.ToInt32(monNum),
                        Year = Convert.ToInt64(year),

                        UserId = insResult.EmployeeCode,
                        DepartmentName = "SPR",
                        Remarks = insResult.Remarks,
                        TotalAmount = Convert.ToDecimal(insResult.Incentives),
                        ThisMonthAmount = Convert.ToDecimal(insResult.Incentives),
                        TotalIncentive = (Convert.ToDecimal(insResult.ImportedSpareValue) + Convert.ToDecimal(insResult.GivenHandsetValue)) * Convert.ToDecimal(0.0017),
                        // TotalIncentive = Convert.ToDecimal(insResult.Incentives),
                        TotalImportedSpareValue = Convert.ToDecimal(insResult.ImportedSpareValue),
                        TotalGivenHandsetValue = Convert.ToDecimal(insResult.GivenHandsetValue),

                        SpecialAmount = insResult.SpecialAmount,
                        SpecialRemarks = insResult.SpecialRemarks,
                        Added = userId,
                        AddedDate = DateTime.Now
                    };
                    _dbEntities.Incentives.AddOrUpdate(model);
                }

            }
            _dbEntities.SaveChanges();

            return "ok";
        }

        public List<VmIncentivePolicy> GetIncentiveReport(string monId, long yearIds)
        {
            int MonNum = Convert.ToInt32(monId);

            //            string getIncentiveReportQuery = string.Format(@"select cm.UserFullName,ii.UserId,ii.TotalAmount,ii.Amount,ii.TotalIncentive,ii.FixedIncentive,ii.Percentage,ii.Month from CellPhoneProject.dbo.CmnUsers cm 
            //            left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
            //            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD','SPRHEAD') and cm.IsActive=1 and ii.MonNum={0}  and ii.Year={1} order by ii.Percentage desc", MonNum, yearIds);

            //            string getIncentiveReportQuery = string.Format(@"select cm.UserFullName,ii.UserId,
            //            ii.TotalAmount,ii.Amount,ii.TotalIncentive,ii.FixedIncentive,ii.Percentage,ii.Month,ii.Remarks,ii.AddedAmount,ii.AmountDeduction,ii.DeductionRemarks,ii.TotalReward,ii.Reward,ii.TotalPenalties,ii.Penalties
            //            from CellPhoneProject.dbo.CmnUsers cm left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
            //            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD','SPR') and cm.IsActive=1 and ii.MonNum={0}  and ii.Year={1} order by ii.Percentage desc", MonNum, yearIds);

            string getIncentiveReportQuery = string.Format(@"select UserFullName,UserId,TotalAmount,Amount,TotalIncentive,FixedIncentive,Percentage,Month,Remarks,AddedAmount,AmountDeduction,DeductionRemarks,TotalReward,Reward,
            TotalPenalties,Penalties,case when SpecialAmount is null then 0 else SpecialAmount end SpecialAmount,SpecialRemarks from
            (	
	            select cm.UserFullName,ii.UserId,ii.TotalAmount,ii.Amount,
	            case when ii.SpecialAmount is null then ii.TotalIncentive else ii.SpecialAmount end TotalIncentive,
	            ii.FixedIncentive,ii.Percentage,ii.Month,ii.Remarks,ii.AddedAmount,ii.AmountDeduction,ii.DeductionRemarks,ii.TotalReward,ii.Reward,
	            ii.TotalPenalties,ii.Penalties,cast(ii.SpecialAmount as decimal(18,2)) as  SpecialAmount,ii.SpecialRemarks
	            from CellPhoneProject.dbo.CmnUsers cm left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
	            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD','SPR') and cm.IsActive=1 
	            and ii.MonNum={0}  and ii.Year={1} 
            )A order by Percentage desc", MonNum, yearIds);

            var getIncentiveReports =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }
        public List<VmIncentivePolicy> GetIncentiveReport1(string monId, long yearIds)
        {
            int MonNum = Convert.ToInt32(monId);

            string getIncentiveReportQuery = string.Format(@"select top 1 ii.Month,CONVERT(varchar(10), ii.Year) as Year from CellPhoneProject.dbo.CmnUsers cm 
            left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD') and cm.IsActive=1 and ii.MonNum={0} and  ii.Year={1}", MonNum, yearIds);
            var getIncentiveReports =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }
        public List<VmIncentivePolicy> GetPreparedUserName()
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string getIncentiveReportQuery = string.Format(@"select UserFullName,EmployeeCode  FROM [CellPhoneProject].[dbo].CmnUsers where CmnUserId={0}", userId);
            var getIncentiveReports =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }
        public bool GetCheckDate(string monId, string yearId)
        {
            int MonNum = Convert.ToInt32(monId);
            List<VmIncentivePolicy> getIncentiveReports = null;
            if (MonNum > 0 && yearId != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year) from [CellPhoneProject].[dbo].[Incentive] where DepartmentName in ('CM','CMHEAD','SPR') and Year={1} and  MonNum={0}", MonNum, yearId);
                getIncentiveReports =
                   _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();


            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;

        }

        public List<VmIncentivePolicy> GetIncentiveSeaShipmentFulls(long monIds, long yearIds)
        {
            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentFull from CellPhoneProject.dbo.ProjectMasters pm 
            //join CellPhoneProject.dbo.ProjectOrderShipments pos on pm.ProjectMasterId=pos.ProjectMasterId where DATEDIFF(month, pm.ApproxShipmentDate, pos.WarehouseEntryDate)<=1
            //and pos.WarehouseEntryDate between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-2, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)", monIds, yearIds);
            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentFull from
            //            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Sea' and
            //            ((DATEDIFF(month, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=1) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity
            //
            //            having COUNT(*)=1)pp", monIds, yearIds);
            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentFull from
(select  pm.ProjectMasterId,pm.ProjectName,pos.VesselDate,pos.WarehouseEntryDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount,
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate))) as DaysDiff,DAY(EOMONTH(pos.WarehouseEntryDate)) AS DaysInMonth,pos.ShipmentPercentage

from CellPhoneProject.dbo.ProjectMasters pm

join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Sea' and
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate)<=DAY(EOMONTH(pos.WarehouseEntryDate))) ) and pos.ShipmentPercentage='Full'
group by pm.ProjectMasterId,pos.VesselDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity,pm.ProjectName,pos.ShipmentPercentage)A", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }
        public List<VmIncentivePolicy> GetIncentiveSeaShipmentPartials(long monIds, long yearIds)
        {

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentPartial from
            //            (select distinct pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,wd.Quantity WrQuantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //            join CellPhoneProject.dbo.WarehouseDetails wd on wd.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pm.ApproxShipmentDate)='{0}' and  DATENAME(YEAR,pm.ApproxShipmentDate)='{1}' and pos.ShipmentType='Sea' and
            //            ((DATEDIFF(month, pm.ApproxShipmentDate, wd.WarehouseDate)<=1) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,wd.WarehouseDate,wd.WarehouseQuantity,ppo.Quantity,wd.Quantity
            //
            //            having COUNT(*)>1) pp", monIds, yearIds);

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentPartial from
            //            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Sea' and
            //            ((DATEDIFF(month, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=1) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity
            //
            //            having COUNT(*)>1)pp", monIds, yearIds);

            _dbEntities.Database.CommandTimeout = 6000;
            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentPartial from
(select  pm.ProjectMasterId,pm.ProjectName,pos.VesselDate,pos.WarehouseEntryDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount,
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate))) as DaysDiff,DAY(EOMONTH(pos.WarehouseEntryDate)) AS DaysInMonth,pos.ShipmentPercentage

from CellPhoneProject.dbo.ProjectMasters pm

join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Sea' and 
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate)<=DAY(EOMONTH(pos.WarehouseEntryDate)))) and pos.ShipmentPercentage='Partial'

group by pm.ProjectMasterId,pos.VesselDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity,pm.ProjectName,pos.ShipmentPercentage)A", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }

        public List<VmIncentivePolicy> GetIncentiveAirShipmentFulls(long monIds, long yearIds)
        {

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentFull from
            //            (select distinct pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,wd.Quantity WrQuantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //            join CellPhoneProject.dbo.WarehouseDetails wd on wd.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pm.ApproxShipmentDate)='{0}' and  DATENAME(YEAR,pm.ApproxShipmentDate)='{1}' and pos.ShipmentType='Air' and
            //            ((DATEDIFF(day, pm.ApproxShipmentDate, wd.WarehouseDate)<=15) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,wd.WarehouseDate,wd.WarehouseQuantity,ppo.Quantity,wd.Quantity
            //
            //            having COUNT(*)=1) pp", monIds, yearIds);


            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentFull from
            //            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Air' and
            //            ((DATEDIFF(day, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=15) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity
            //
            //            having COUNT(*)=1)pp", monIds, yearIds);

            _dbEntities.Database.CommandTimeout = 6000;

            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentFull from
(select  pm.ProjectMasterId,pm.ProjectName,pos.VesselDate,pos.WarehouseEntryDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount,
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate))) as DaysDiff,pos.ShipmentPercentage

from CellPhoneProject.dbo.ProjectMasters pm

join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Air' and
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate)<=15)) and pos.ShipmentPercentage='Full'

group by pm.ProjectMasterId,pos.VesselDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity,pm.ProjectName,pos.ShipmentPercentage)A", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }
        public List<VmIncentivePolicy> GetIncentiveAirShipmentPartials(long monIds, long yearIds)
        {

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentPartial from
            //            (select distinct pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,wd.Quantity WrQuantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //            join CellPhoneProject.dbo.WarehouseDetails wd on wd.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pm.ApproxShipmentDate)='{0}' and  DATENAME(YEAR,pm.ApproxShipmentDate)='{1}' and pos.ShipmentType='Air' and
            //            ((DATEDIFF(day, pm.ApproxShipmentDate, wd.WarehouseDate)<=15) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,wd.WarehouseDate,wd.WarehouseQuantity,ppo.Quantity,wd.Quantity
            //
            //            having COUNT(*)>1) pp", monIds, yearIds);

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentPartial from
            //            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Air' and
            //            ((DATEDIFF(day, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=15) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity
            //
            //            having COUNT(*)>1)pp", monIds, yearIds);

            _dbEntities.Database.CommandTimeout = 6000;

            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentPartial from
(select  pm.ProjectMasterId,pm.ProjectName,pos.VesselDate,pos.WarehouseEntryDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount,
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate))) as DaysDiff,pos.ShipmentPercentage

from CellPhoneProject.dbo.ProjectMasters pm

join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Air' and
((DATEDIFF(day, pos.VesselDate, pos.WarehouseEntryDate)<=15)) and pos.ShipmentPercentage='Partial'

group by pm.ProjectMasterId,pos.VesselDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity,pm.ProjectName,pos.ShipmentPercentage)A", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }
        public List<NinetyFiveProductionRewardModel> CmPenaltiesCkdSkd(string monNum, string year)
        {
            long monIds;
            long yearIds;
            long.TryParse(monNum, out monIds);
            long.TryParse(year, out yearIds);

            //            string proEv14 = string.Format(@"select C.ProjectMasterID,C.ProjectName,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.WarehouseEntryDate, cast(C.DaysDiff as bigint) as DaysDiff,cast(C.EffectiveDays as bigint) as EffectiveDays,
            //            cast(C.DeductPoint as bigint) as DeductPoint,cast(C.DaysDiffForDeduct as bigint) as DaysDiffForDeduct,cast(C.AmountDeduct as bigint) as AmountDeduct,C.IsFinalShipment 
            //	            from
            //	            (
            //			            select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,cast(B.OrderNuber as varchar(50)) as Orders,B.PoDate,B.WarehouseEntryDate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
            //			            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
            //			            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,B.IsFinalShipment
            //
            //			            from
            //
            //				            (
            //					            select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntryDate, A.DaysDiff,A.EffectiveDays,
            //					            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
            //					            A.IsFinalShipment
            //					            from
            //					            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
            //					            case when pm.ProjectType='Smart' then 150  when  pm.ProjectType='Feature' then 150 end as EffectiveDays, DeductPoint=100,ps.IsFinalShipment
            //
            //					            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
            //					            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
            //					            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            //					            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.OrderNuber=1 and ps.IsFinalShipment='Yes'
            //					            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
            //				            )A
            //
            //		                    where DaysDiff>=0
            //		            )B 
            //	            )C where AmountDeduct>0 order by C.ProjectName asc", monIds, yearIds);

            _dbEntities.Database.CommandTimeout = 6000;

            string proEv14 = string.Format(@"
            select D.ProjectMasterID,D.ProjectName,D.ProjectType,D.ShipmentType,D.SourcingType,D.Orders,D.PoDate,D.WarehouseEntryDate,D.DaysDiff,D.EffectiveDays,D.DeductPoint,D.DaysDiffForDeduct,D.AmountDeduct,D.IsFinalShipment,
            ca.RefundAmount,ca.EffectiveMonth,ca.IsRefund from 
		            (select C.ProjectMasterID,C.ProjectName,C.ProjectType,C.ShipmentType,C.SourcingType,C.Orders,C.PoDate,C.WarehouseEntryDate, cast(C.DaysDiff as bigint) as DaysDiff,cast(C.EffectiveDays as bigint) as EffectiveDays,
		            cast(C.DeductPoint as bigint) as DeductPoint,cast(C.DaysDiffForDeduct as bigint) as DaysDiffForDeduct,cast(C.AmountDeduct as bigint) as AmountDeduct,C.IsFinalShipment
		            from
		            (
				            select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.SourcingType,B.ShipmentType,cast(B.OrderNuber as varchar(50)) as Orders,B.PoDate,B.WarehouseEntryDate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
				            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
				            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,B.IsFinalShipment
				            from

					            (
						            select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.SourcingType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntryDate, A.DaysDiff,A.EffectiveDays,
						            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
						            A.IsFinalShipment
						            from
						            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.SourcingType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
						            case when pm.ProjectType='Smart' then 150  when  pm.ProjectType='Feature' then 150 end as EffectiveDays, DeductPoint=100,ps.IsFinalShipment

						            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
						            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
						            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
						            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.OrderNuber=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1
						            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
					            )A
					            where DaysDiff>=0
			            )B 
		            )C 
	
	                where C.AmountDeduct>0
	            )D 
            left join CellPhoneProject.dbo.Cm_RefundProjectAmount ca on ca.ProjectModel =D.ProjectName and D.Orders=ca.Orders and D.AmountDeduct=ca.DeductedAmount
            order by D.ProjectName asc", monIds, yearIds);
            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();

            return proEvent14;
        }
        public List<NinetyFiveProductionRewardModel> CmPenaltiesRepeatOrder(string monNum, string year)
        {
            long monIds;
            long yearIds;
            long.TryParse(monNum, out monIds);
            long.TryParse(year, out yearIds);

            //            string proEv14 = string.Format(@"select C.ProjectMasterID,C.ProjectName,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.WarehouseEntryDate, cast(C.DaysDiff as bigint) as DaysDiff,cast(C.EffectiveDays as bigint) as EffectiveDays,
            //            cast(C.DeductPoint as bigint) as DeductPoint,cast(C.DaysDiffForDeduct as bigint) as DaysDiffForDeduct,cast(C.AmountDeduct as bigint) as AmountDeduct,C.IsFinalShipment 
            //            from
            //                    (select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,cast(B.OrderNuber as varchar(50)) as Orders,B.PoDate,B.WarehouseEntryDate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
            //                    case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
            //                    case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,
            //                        B.IsFinalShipment
            //                         from
            //				            (select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntryDate, A.DaysDiff,A.EffectiveDays,
            //				            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
            //				            A.IsFinalShipment
            //				            from
            //				            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
            //				            case when ps.ShipmentType='Air' then 80  when  ps.ShipmentType='Sea' then 100 end as EffectiveDays, DeductPoint=100,ps.IsFinalShipment
            //
            //				            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
            //				            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
            //				            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            //				            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.OrderNuber !=1 and ps.IsFinalShipment='Yes'
            //				            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
            //				            )A 
            //		            where DaysDiff>=0
            //	                )B 
            //            )C where AmountDeduct>0 order by C.ProjectName asc", monIds, yearIds);

            _dbEntities.Database.CommandTimeout = 6000;

            string proEv14 = string.Format(@"
            select D.ProjectMasterID,D.ProjectName,D.ProjectType,D.ShipmentType,D.Orders,D.SourcingType,D.PoDate,D.WarehouseEntryDate,D.DaysDiff,D.EffectiveDays,D.DeductPoint,D.DaysDiffForDeduct,D.AmountDeduct,D.IsFinalShipment,
            ca.RefundAmount,ca.EffectiveMonth,ca.IsRefund from 		
	            (select C.ProjectMasterID,C.ProjectName,C.ProjectType,C.ShipmentType,C.Orders,C.SourcingType,C.PoDate,C.WarehouseEntryDate, cast(C.DaysDiff as bigint) as DaysDiff,cast(C.EffectiveDays as bigint) as EffectiveDays,
	            cast(C.DeductPoint as bigint) as DeductPoint,cast(C.DaysDiffForDeduct as bigint) as DaysDiffForDeduct,cast(C.AmountDeduct as bigint) as AmountDeduct,C.IsFinalShipment 
	            from
			            (select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,B.SourcingType,cast(B.OrderNuber as varchar(50)) as Orders,B.PoDate,B.WarehouseEntryDate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
			            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
			            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,B.IsFinalShipment
					            from
					            (select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.SourcingType,A.OrderNuber,A.PoDate,A.WarehouseEntryDate, A.DaysDiff,A.EffectiveDays,
					            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
					            A.IsFinalShipment
					            from
					            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
					            case when ps.ShipmentType='Air' then 80  when  ps.ShipmentType='Sea' then 100 end as EffectiveDays, DeductPoint=100,ps.IsFinalShipment

					            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
					            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
					            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
					            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.OrderNuber !=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1
					            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
					            )A 
			            where DaysDiff>=0
		            )B 
	            )C where AmountDeduct>0
            )D 
            left join CellPhoneProject.dbo.Cm_RefundProjectAmount ca on ca.ProjectModel =D.ProjectName and D.Orders=ca.Orders and D.AmountDeduct=ca.DeductedAmount
            order by D.ProjectName asc", monIds, yearIds);
            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();

            return proEvent14;
        }
        #endregion

        #region Spare & Incentive  Upto August 2019

        public List<ProjectMasterModel> GetSpareProjectList()
        {
            string proEv = string.Format(@"select pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber from CellPhoneProject.dbo.ProjectMasters pm
            where pm.IsActive=1");

            var proEvent = _dbEntities.Database.SqlQuery<ProjectMasterModel>(proEv).ToList();


            foreach (var project in proEvent)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return proEvent;
        }
        public List<ProjectMasterModel> GetProjectWiseOrderForSpare(long proId)
        {
            string proEv = string.Format(@"select ProjectMasterId,OrderNuber from CellPhoneProject.dbo.ProjectMasters
            where ProjectMasterId='{0}'", proId);

            var proEvent = _dbEntities.Database.SqlQuery<ProjectMasterModel>(proEv).ToList();
            return proEvent;
        }
        public ProjectOrderShipmentModel GetWarehouseReceiveDate(long proId)
        {
            var projectOrder = new ProjectOrderShipmentModel();
            var proOrderShipment = (from ppo in _dbEntities.ProjectOrderShipments

                                    where ppo.ProjectMasterId == proId
                                    select new
                                    {
                                        ppo.ProjectMasterId,
                                        ppo.WarehouseEntryDate,
                                        ppo.ShipmentType

                                    }).FirstOrDefault();

            if (proOrderShipment != null)
            {
                projectOrder.ProjectMasterId = proOrderShipment.ProjectMasterId;
                projectOrder.WarehouseEntryDate = proOrderShipment.WarehouseEntryDate;
                projectOrder.ShipmentType = proOrderShipment.ShipmentType;
            }

            return projectOrder;
        }
        public bool CheckSpareDataAlreadySaved(List<SpareClaimModel> results)
        {
            var spareClaim = new List<SpareClaimModel>();
            foreach (var insResult in results)
            {
                string getSpareClaim = string.Format(@"select * from CellPhoneProject.dbo.SpareClaim where ProjectId='{0}' and SpareClaimDate='{1}' and Quantity='{2}' and WarehouseReceiveDate='{3}'
                ", insResult.ProjectId, insResult.SpareClaimDate, insResult.Quantity, insResult.WarehouseReceiveDate);
                spareClaim =
                   _dbEntities.Database.SqlQuery<SpareClaimModel>(getSpareClaim).ToList();

            }

            if (spareClaim != null && spareClaim.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string SaveSpareClaimDatas(List<SpareClaimModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            foreach (var insResult in results)
            {
                var query1 = (from c in _dbEntities.ProjectMasters
                              where c.ProjectMasterId == insResult.ProjectId
                              select c).FirstOrDefault();

                var query2 = (from c in _dbEntities.ProjectPurchaseOrderForms
                              where c.ProjectMasterId == insResult.ProjectId
                              select c).FirstOrDefault();

                var query3 = (from c in _dbEntities.ProjectMasters
                              where c.ProjectMasterId == insResult.ProjectId
                              select c).FirstOrDefault();

                var model1 = new SpareClaim();
                model1.ProjectId = insResult.ProjectId;
                model1.ProjectName = query3.ProjectName;
                model1.OrderNumber = query1.OrderNuber;
                model1.PoCategory = query2.PoCategory;
                //model1.ShipmentWithOrder = insResult.ShipmentWithOrder;
                model1.WarehouseReceiveDate = insResult.WarehouseReceiveDate;
                model1.SpareClaimDate = insResult.SpareClaimDate;
                model1.Remarks = insResult.Remarks;
                model1.Quantity = insResult.Quantity;
                model1.Status = "NEW";
                model1.Added = userId;
                model1.AddedDate = DateTime.Now;

                _dbEntities.SpareClaims.Add(model1);

            }

            _dbEntities.SaveChanges();

            return "ok";
        }
        public List<SpareClaimModel> GetPreviousSpareDatas(long proId)
        {
            string proEv = string.Format(@"select * from CellPhoneProject.dbo.SpareClaim
            where ProjectId='{0}'", proId);

            var proEvent = _dbEntities.Database.SqlQuery<SpareClaimModel>(proEv).ToList();
            return proEvent;
        }
        public bool CheckSpareIncentiveData(long monIds, long yearIds)
        {

            var spareClaim = new List<SpareClaimModel>();

            string getSpareClaim = string.Format(@" select MonNum,CONVERT(varchar(10),Year) as Year 
                    from [CellPhoneProject].[dbo].[Incentive] where DepartmentName='SPRHEAD' and  MonNum='{0}' and Year='{1}' 
                ", monIds, yearIds);
            spareClaim =
               _dbEntities.Database.SqlQuery<SpareClaimModel>(getSpareClaim).ToList();

            if (spareClaim != null && spareClaim.Count != 0)
            {
                return true;
            }
            return false;
        }
        public List<SpareClaimModel> GetNewSpareComplain()
        {

            string getNew = string.Format(@"SELECT ProjectId,ProjectName,OrderNumber,PoCategory,SpareClaimDate,WarehouseReceiveDate,Quantity,Remarks,CASE WHEN DATEDIFF(m,SpareClaimDate, WarehouseReceiveDate)<=2 THEN 1000 ELSE 0 END AS 'SpareClaimIncentive',
            DATEDIFF(m,SpareClaimDate, WarehouseReceiveDate) as MonthRange,month(WarehouseReceiveDate) as MonNum,DATENAME(MONTH,WarehouseReceiveDate) as Month, year(WarehouseReceiveDate) as YearName
            from CellPhoneProject.dbo.SpareClaim where Status='NEW' ");
            var getNewSpare = _dbEntities.Database.SqlQuery<SpareClaimModel>(getNew).ToList();
            return getNewSpare;
        }
        public string SaveSpareApprovedData(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks,
   string status)
        {

            String userIdentity =
            HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            DateTime date1;
            DateTime.TryParseExact(spareClaimDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date1);

            DateTime spareClaimDate1 = date1;

            DateTime date2;
            DateTime.TryParseExact(warehouseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date2);

            DateTime warehouseDate1 = date2;

            var dbModel1 =
            _dbEntities.SpareClaims.FirstOrDefault(
            x =>
            x.ProjectId == proIds && x.SpareClaimDate == spareClaimDate1 &&
            x.WarehouseReceiveDate == warehouseDate1 && x.Quantity == quantity && x.Remarks == remarks);

            if (dbModel1 != null)
            {
                dbModel1.Status = status;
                dbModel1.Updated = userId;
                dbModel1.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbModel1).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();

            return "ok";

        }

        public string SaveSpareDeclinedData(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks,
            string status)
        {
            throw new NotImplementedException();
        }
        public List<SpareClaimModel> GetTotalSpareClaim(string monId, string yearId)
        {
            string getTotalSpare = string.Format(@"select count(*) as TotalSpareClaim from CellPhoneProject.dbo.SpareClaim where status='APPROVED' 
            and month(WarehouseReceiveDate)='{0}' and year(WarehouseReceiveDate)='{1}' ", monId, yearId);
            var getTotalSpareData = _dbEntities.Database.SqlQuery<SpareClaimModel>(getTotalSpare).ToList();
            return getTotalSpareData;
        }

        public List<SpareClaimModel> GetUserInfoForSpareClaims(long monIds, long yearIds)
        {
            string getUserQuery = string.Format(@"
            SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iit.UserId,iit.ThisMonthAmount,
            iit.DepartmentName,iit.TotalIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=cm.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename='SPRHEAD' and cm.IsActive=1 ", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<SpareClaimModel>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public string SaveMonthlyIncentiveForSpareClaims(List<SpareClaimModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId1 = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model1 = new DAL.DbModel.Incentive();
                model1.ThisMonthAmount = insResult.ThisMonthAmount;
                model1.TotalIncentive = insResult.TotalIncentive;
                model1.DepartmentName = insResult.RoleName;
                model1.Remarks = insResult.Remarks;
                model1.AddedAmount = insResult.AddedAmount;
                model1.DeductionRemarks = insResult.DeductionRemarks;
                model1.AmountDeduction = insResult.AmountDeduction;
                model1.Month = insResult.Month;
                model1.MonNum = insResult.MonNum;
                model1.Year = insResult.YearName;
                model1.UserId = insResult.UserId;

                model1.Added = userId1;
                model1.AddedDate = DateTime.Now;

                _dbEntities.Incentives.Add(model1);

            }

            _dbEntities.SaveChanges();

            return "ok";
        }

        public string SaveCmPenaltiesCkdSkd(NinetyFiveProductionRewardModel refundSave)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var isSaveCheck = false;
            isSaveCheck = GetCmRefundData(refundSave);

            if (refundSave.IsRefund != "NO")
            {
                if (!isSaveCheck)
                {
                    Cm_RefundProjectAmount rnd = new Cm_RefundProjectAmount();
                    rnd.ProjectMasterId = refundSave.ProjectMasterID;
                    rnd.ProjectModel = refundSave.ProjectName.Trim();
                    rnd.ProjectType = refundSave.ProjectType;
                    rnd.ShipmentType = refundSave.ShipmentType;
                    rnd.SourcingType = refundSave.SourcingType;
                    rnd.Orders = refundSave.Orders.Trim();
                    rnd.PoDate = refundSave.PoDate;
                    rnd.WarehouseEntryDate = refundSave.WarehouseEntryDate;
                    rnd.DaysDiff = Convert.ToInt32(refundSave.DaysDiff);
                    rnd.EffectiveDays = Convert.ToInt32(refundSave.EffectiveDays);
                    rnd.DeductPoint = refundSave.DeductPoint;
                    rnd.DaysDiffForDeduct = refundSave.DaysDiffForDeduct;
                    rnd.DeductedAmount = refundSave.AmountDeduct;
                    rnd.IsRefund = refundSave.IsRefund;
                    rnd.RefundAmount = Convert.ToInt64(refundSave.RefundAmount1);
                    rnd.RefundPercentage = 70;
                    rnd.EffectiveMonth = refundSave.EffectiveMonth;
                    rnd.ProjectSourchingType = refundSave.SourcingType;
                    rnd.RoleName = "CM";
                    rnd.Month = Convert.ToDateTime(refundSave.EffectiveMonth).ToString("MMMM");
                    rnd.Year = Convert.ToDateTime(refundSave.EffectiveMonth).Year;
                    rnd.MonNum = Convert.ToDateTime(refundSave.EffectiveMonth).Month;

                    rnd.Added = userId;
                    rnd.AddedDate = DateTime.Now;

                    //////version//
                    _dbEntities.Cm_RefundProjectAmount.Add(rnd);
                    _dbEntities.SaveChanges();
                }
            }
            else if (refundSave.IsRefund == "NO")
            {
                if (isSaveCheck)
                {
                    var updatedAssembly = (from c in _dbEntities.Cm_RefundProjectAmount
                                           where c.ProjectMasterId == refundSave.ProjectMasterID && c.ProjectModel == refundSave.ProjectName
                                           select c).FirstOrDefault();

                    _dbEntities.Cm_RefundProjectAmount.Remove(updatedAssembly);
                    _dbEntities.SaveChanges();
                    //////version//

                }
            }
            return "OK";
        }

        public bool GetCmRefundData(NinetyFiveProductionRewardModel refundSave)
        {
            List<NinetyFiveProductionRewardModel> getIncentiveReports = null;
            if (refundSave.ProjectName.Trim() != null && refundSave.Orders.Trim() != null)
            {
                string getIncentiveReportQuery = string.Format(@"select top 1 ProjectModel,Orders,RefundAmount from [CellPhoneProject].[dbo].[Cm_RefundProjectAmount]
                where ProjectMasterID ='" + refundSave.ProjectMasterID + "' and ProjectModel ='" + refundSave.ProjectName.Trim() + "' and Orders='" + refundSave.Orders + "' and  RefundAmount='" + Convert.ToInt64(refundSave.RefundAmount1) + "' and PoDate='" + refundSave.PoDate + "' and WarehouseEntryDate='" + refundSave.WarehouseEntryDate + "'  ");

                getIncentiveReports =
                   _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(getIncentiveReportQuery).ToList();

            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveCmPenaltiesRepeatOrder(NinetyFiveProductionRewardModel refundSave)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var isSaveCheck = false;
            isSaveCheck = GetCmRefundData(refundSave);

            if (refundSave.IsRefund != "NO")
            {
                if (!isSaveCheck)
                {
                    Cm_RefundProjectAmount rnd = new Cm_RefundProjectAmount();
                    rnd.ProjectMasterId = refundSave.ProjectMasterID;
                    rnd.ProjectModel = refundSave.ProjectName.Trim();
                    rnd.ProjectType = refundSave.ProjectType;
                    rnd.ShipmentType = refundSave.ShipmentType;
                    rnd.SourcingType = refundSave.SourcingType;
                    rnd.Orders = refundSave.Orders.Trim();
                    rnd.PoDate = refundSave.PoDate;
                    rnd.WarehouseEntryDate = refundSave.WarehouseEntryDate;
                    rnd.DaysDiff = Convert.ToInt32(refundSave.DaysDiff);
                    rnd.EffectiveDays = Convert.ToInt32(refundSave.EffectiveDays);
                    rnd.DeductPoint = refundSave.DeductPoint;
                    rnd.DaysDiffForDeduct = refundSave.DaysDiffForDeduct;
                    rnd.DeductedAmount = refundSave.AmountDeduct;
                    rnd.IsRefund = refundSave.IsRefund;
                    rnd.RefundAmount = Convert.ToInt64(refundSave.RefundAmount1);
                    rnd.RefundPercentage = 70;
                    rnd.EffectiveMonth = refundSave.EffectiveMonth;
                    rnd.ProjectSourchingType = refundSave.SourcingType;
                    rnd.RoleName = "CM";
                    rnd.Month = Convert.ToDateTime(refundSave.EffectiveMonth).ToString("MMMM");
                    rnd.Year = Convert.ToDateTime(refundSave.EffectiveMonth).Year;
                    rnd.MonNum = Convert.ToDateTime(refundSave.EffectiveMonth).Month;

                    rnd.Added = userId;
                    rnd.AddedDate = DateTime.Now;

                    //////version//
                    _dbEntities.Cm_RefundProjectAmount.Add(rnd);
                    _dbEntities.SaveChanges();
                }
            }
            else if (refundSave.IsRefund == "NO")
            {
                if (isSaveCheck)
                {
                    var updatedAssembly = (from c in _dbEntities.Cm_RefundProjectAmount
                                           where c.ProjectMasterId == refundSave.ProjectMasterID && c.ProjectModel == refundSave.ProjectName
                                           select c).FirstOrDefault();

                    _dbEntities.Cm_RefundProjectAmount.Remove(updatedAssembly);
                    _dbEntities.SaveChanges();
                    //////version//

                }
            }
            return "OK";
        }

        public List<Cm_OthersIncentiveModel> GetOthersIncentive(long monIds, long yearIds)
        {
            string query = String.Format(@"select OthersType,Amount,Remarks,DeductAmount,DeductRemarks,EffectiveMonth,FinalAmount 
            FROM [CellPhoneProject].[dbo].[Cm_OthersIncentive] where DATEPART(mm,EffectiveMonth)={0} and  DATENAME(YEAR,EffectiveMonth)={1} ", monIds, yearIds);

            var qList = _dbEntities.Database.SqlQuery<Cm_OthersIncentiveModel>(query).ToList();

            return qList;
        }

        #endregion

        #region Incentive From September 2019
        public List<VmIncentivePolicy> GetVmIncentivePolicyNew()
        {
            string getinsPolicyQuery = string.Format(@"select * from [CellPhoneProject].[dbo].[IncentiveParameter] where IsActive=1");
            var getinsPolicies =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getinsPolicyQuery).ToList();
            return getinsPolicies;
        }
        public List<VmIncentivePolicy> GetIncentiveOrdersNew(long monIds, long yearIds)
        {
            //            string getIncentiveOrdersQuery = string.Format(@"      
            //            select sum(Orders) as Orders, DATENAME(MONTH,GETDATE()) AS Month,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year from
            //            (SELECT count([ProjectPurchaseOrderFormId]) as Orders, DATENAME(MONTH,GETDATE()) as MM,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS YYYY
            //            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] where DATENAME(MONTH,AddedDate)=DATENAME(MONTH,GETDATE()) 
            //            and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) group by AddedDate) as C");

            //            string getIncentiveOrdersQuery = string.Format(@"      
            //            Select * from(
            //            SELECT count([ProjectPurchaseOrderFormId]) as Orders,
            //            DATEPART(mm,AddedDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, AddedDate)) as MonName,
            //            CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
            //            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms]
            //            where DATEPART(mm,AddedDate)={0}
            //            and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) 
            //            group by DATEPART(mm,AddedDate),CONVERT(varchar(12),DATENAME(MONTH, AddedDate))
            //            ) as C",monIds);

            //            string getIncentiveOrdersQuery = string.Format(@"      
            //            Select * from(
            //            SELECT count([ProjectPurchaseOrderFormId]) as Orders,
            //            DATEPART(mm,AddedDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, AddedDate)) as MonName,
            //            CONVERT(varchar(4),DATEPART(yy, AddedDate)) as Year
            //            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms]
            //            where DATEPART(mm,AddedDate)={0}
            //            and DATENAME(YEAR,AddedDate)={1}
            //            group by DATEPART(mm,AddedDate),CONVERT(varchar(12),DATENAME(MONTH, AddedDate)),DATEPART(yy, AddedDate)
            //            ) as C", monIds, yearIds);

            string getIncentiveOrdersQuery = string.Format(@"      
            Select * from(
            SELECT count([ProjectPurchaseOrderFormId]) as Orders,
            DATEPART(mm,PoDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, PoDate)) as MonName,
            CONVERT(varchar(4),DATEPART(yy, PoDate)) as Year
            from [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms]
            where DATEPART(mm,PoDate)={0}
            and DATENAME(YEAR,PoDate)={1}
            group by DATEPART(mm,PoDate),CONVERT(varchar(12),DATENAME(MONTH, PoDate)),DATEPART(yy, PoDate)
            ) as C", monIds, yearIds);

            var getIncentiveOrders =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveOrdersQuery).ToList();
            return getIncentiveOrders;
        }

        public List<VmIncentivePolicy> GetIncentiveLcsNew(long monIds, long yearIds)
        {
            string getLcForIncentive = string.Format(@"Select * from(
            SELECT count(ProjectLcId) as PerLc,
            DATEPART(mm,LcPassDate) as MonNum,CONVERT(varchar(12), DATENAME(MONTH, LcPassDate)) as MonName,
            CONVERT(varchar(4),DATEPART(yy, LcPassDate)) as Year
            from [CellPhoneProject].[dbo].[ProjectLcs]
            where DATEPART(mm,LcPassDate)={0}
            and DATENAME(YEAR,LcPassDate)={1}
            group by DATEPART(mm,LcPassDate),CONVERT(varchar(12),DATENAME(MONTH, LcPassDate)),DATEPART(yy, LcPassDate)
            ) as C", monIds, yearIds);

            var getLcs = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getLcForIncentive).ToList();
            return getLcs;
        }

        public List<VmIncentivePolicy> GetPrimarySalesNew(long monIds, long yearIds)
        {

            string oradb = "Data Source=(DESCRIPTION="
                                            + "(ADDRESS=(PROTOCOL=TCP)(HOST=test)(PORT=test))"
                                            + "(CONNECT_DATA=(SERVICE_NAME=test)));"
                                            + "User Id=test;Password=test#;";
            OracleConnection con = OracleDbConnection.GetOldConnection();
            OracleCommand cmd = new OracleCommand();
            //cmd.CommandText = "SELECT YEAR,MONTH, SUM(SALES_AMT) SALES_AMT FROM (SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=86 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND  TRUNC(B.ACTUAL_SHIPMENT_DATE) BETWEEN  trunc (sysdate, 'mm')/*current month*/ AND SYSDATE GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) UNION ALL  SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE))" + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=646 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND  TRUNC(B.ACTUAL_SHIPMENT_DATE) BETWEEN  trunc (sysdate, 'mm')/*current month*/ AND SYSDATE GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) ) GROUP BY YEAR,MONTH";
            //cmd.CommandText =  string.Format(@"SELECT YEAR,MONTH, SUM(SALES_AMT) SALES_AMT FROM (SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) "+@"""MONTH"""+", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=86 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND  TRUNC(B.ACTUAL_SHIPMENT_DATE) BETWEEN  trunc(sysdate, 'YEAR')     AND add_months(trunc(sysdate, 'YEAR'), 12)-1/24/60/60 GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) UNION ALL  SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) "+@"""YEAR"""+", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) "+@"""MONTH"""+", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=646 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND  TRUNC(B.ACTUAL_SHIPMENT_DATE) BETWEEN  trunc(sysdate, 'YEAR')     AND add_months(trunc(sysdate, 'YEAR'), 12)-1/24/60/60 GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) ) GROUP BY YEAR,MONTH",monIds);
            // cmd.CommandText = string.Format(@"SELECT YEAR,MONTH, SUM(SALES_AMT) SALES_AMT FROM (SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=86 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {1} GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) UNION ALL  SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=646 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {1} GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) ) GROUP BY YEAR,MONTH", monIds, yearIds);
            cmd.CommandText = string.Format(@"SELECT YEAR,MONTH, SUM(SALES_AMT) SALES_AMT FROM (SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=86 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {1} GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) UNION ALL  SELECT  EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""YEAR""" + ", EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) " + @"""MONTH""" + ", (SUM((NVL(B.ORDERED_QUANTITY,0))*B.UNIT_SELLING_PRICE)) SALES_AMT FROM  OE_ORDER_HEADERS_ALL  A,OE_ORDER_LINES_ALL  B WHERE  A.ORG_ID=B.ORG_ID   AND   A.HEADER_ID= B.HEADER_ID AND  A.ORG_ID=223 AND  A.BOOKED_FLAG='Y' AND  A.ORDER_CATEGORY_CODE='ORDER' AND B.ACTUAL_SHIPMENT_DATE IS NOT NULL AND EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {0} AND EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) = {1} GROUP BY EXTRACT(YEAR FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) , EXTRACT(MONTH FROM TO_DATE(B.ACTUAL_SHIPMENT_DATE)) ) GROUP BY YEAR,MONTH", monIds, yearIds);


            cmd.Connection = con;
            cmd.CommandTimeout = 6000;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            var incentives = new List<VmIncentivePolicy>();
            var getIncentiveOrders1 = new VmIncentivePolicy();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    getIncentiveOrders1 = new VmIncentivePolicy
                    {
                        Year = (dr["YEAR"]).ToString(),
                        Month = (dr["MONTH"]).ToString(),
                        SalesAmt = Convert.ToDecimal(dr["SALES_AMT"])

                    };
                    incentives.Add(getIncentiveOrders1);
                }
            }
            return incentives;
        }

        public List<VmIncentivePolicy> GetFeaturePhoneServiceNew(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            var featureServiceList = new List<VmIncentivePolicy>();
            var featurePhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //                string getFeaturePhoneServiceQuery = string.Format(@"      
                //                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,CONVERT(varchar(12),
                //                 DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
                //                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0}
                //                and DATENAME(YEAR,sm.ServicePlaceDate)=DATENAME(YEAR,GETDATE()) and sm.Model like '%Olvio%'
                //                group by DATEPART(mm,sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds);
                string getFeaturePhoneServiceQuery = string.Format(@"      
                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,
                CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, sm.ServicePlaceDate)) as Year
                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0}
                and DATENAME(YEAR,sm.ServicePlaceDate)={1}
                and (sm.Model like '%Olvio%' or sm.Model like '%Classic%' or sm.Model like '%Excel%')
                group by DATEPART(mm,sm.ServicePlaceDate),DATEPART(yy, sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds, yearIds);
                var command = new SqlCommand(getFeaturePhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    featurePhone = new VmIncentivePolicy
                    {
                        TotalServiceIMEI = reader["ServiceIMEI"].ToString(),
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    featureServiceList.Add(featurePhone);
                }
                connection.Close();

            }
            return featureServiceList;
        }

        public List<VmIncentivePolicy> GetFeaturePhoneSalesNew(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var featureServiceList = new List<VmIncentivePolicy>();
            var featurePhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //                string getFeaturePhoneServiceQuery = string.Format(@"           
                //                select * from
                //                (SELECT count(distinct tdd.BarCode) as BarCode, DATEPART(mm,tdd.DistributionDate) as MonNum,CONVERT(varchar(12),
                //                 DATENAME(MONTH, tdd.DistributionDate)) as MonName,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
                //                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATEPART(mm,tdd.DistributionDate)={0}
                //                and DATENAME(YEAR,tdd.DistributionDate)=DATENAME(YEAR,GETDATE()) and tdd.Model like '%Olvio%' 
                //                group by DATEPART(mm,tdd.DistributionDate),CONVERT(varchar(12),DATENAME(MONTH, tdd.DistributionDate))) as C", monIds);

                string getFeaturePhoneServiceQuery = string.Format(@"           
                select * from
                (SELECT count(distinct tdd.BarCode) as BarCode, DATEPART(mm,tdd.DistributionDate) as MonNum,CONVERT(varchar(12),
                DATENAME(MONTH, tdd.DistributionDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, tdd.DistributionDate)) as Year
                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATEPART(mm,tdd.DistributionDate)={0}
                and DATENAME(YEAR,tdd.DistributionDate)={1} and (tdd.Model like '%Olvio%' or tdd.Model like '%Classic%' or tdd.Model like '%Excel%')
                group by DATEPART(mm,tdd.DistributionDate),DATEPART(yy, tdd.DistributionDate),CONVERT(varchar(12),DATENAME(MONTH, tdd.DistributionDate))) as C", monIds, yearIds);

                var command = new SqlCommand(getFeaturePhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    featurePhone = new VmIncentivePolicy
                    {
                        TotalSalesBarcode = reader["BarCode"].ToString(),
                        //Month = reader["Month"].ToString(),
                        //Year = reader["Year"].ToString()
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    featureServiceList.Add(featurePhone);
                }
                connection.Close();

            }
            return featureServiceList;
        }

        public List<VmIncentivePolicy> GetSmartPhoneServiceNew(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            var smartServiceList = new List<VmIncentivePolicy>();
            var smartPhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //                string getSmartPhoneServiceQuery = string.Format(@"      
                //                select sum(IMEI) as ServiceIMEI, DATENAME(MONTH,GETDATE()) AS Month,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year from
                //                (SELECT count(distinct sm.IME) as IMEI, DATENAME(MONTH,GETDATE()) as MM,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS YYYY
                //                from WSMS.[dbo].ServiceMaster sm where DATENAME(MONTH,sm.ServicePlaceDate)=DATENAME(MONTH,GETDATE()) 
                //                and DATENAME(YEAR,sm.ServicePlaceDate)=DATENAME(YEAR,GETDATE()) 
                //                and (sm.Model like '%Primo%' or sm.Model like '%Walpad%')
                //                group by sm.ServicePlaceDate) as C");
                //                string getSmartPhoneServiceQuery = string.Format(@"      
                //                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,CONVERT(varchar(12),
                //                DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year
                //                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0}
                //                and DATENAME(YEAR,sm.ServicePlaceDate)=DATENAME(YEAR,GETDATE())  and (sm.Model like '%Primo%' or sm.Model like '%Walpad%')
                //                group by DATEPART(mm,sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds);

                string getSmartPhoneServiceQuery = string.Format(@"      
                select * from (SELECT count(distinct sm.IME) as ServiceIMEI, DATEPART(mm,sm.ServicePlaceDate) as MonNum,CONVERT(varchar(12),
                DATENAME(MONTH, sm.ServicePlaceDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, sm.ServicePlaceDate)) as Year
                from WSMS.[dbo].ServiceMaster sm where DATEPART(mm,sm.ServicePlaceDate)={0} and DATENAME(YEAR,sm.ServicePlaceDate)={1}
                and (sm.Model like '%Primo%' or sm.Model like '%Walpad%')
                group by DATEPART(mm,sm.ServicePlaceDate),DATEPART(yy, sm.ServicePlaceDate),CONVERT(varchar(12),DATENAME(MONTH, sm.ServicePlaceDate))) as C", monIds, yearIds);
                var command = new SqlCommand(getSmartPhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    smartPhone = new VmIncentivePolicy
                    {
                        TotalServiceIMEI = reader["ServiceIMEI"].ToString(),
                        //Month = reader["Month"].ToString(),
                        //Year = reader["Year"].ToString()
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    smartServiceList.Add(smartPhone);
                }
                connection.Close();

            }
            return smartServiceList;
        }
        public List<VmIncentivePolicy> GetSmartPhoneSalesNew(long monIds, long yearIds)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var smartServiceList = new List<VmIncentivePolicy>();
            var smartPhone = new VmIncentivePolicy();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //                string getSmartPhoneServiceQuery = string.Format(@"      
                //                select sum(BarCode) as BarCode, DATENAME(MONTH,GETDATE()) AS Month,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS Year from
                //                (SELECT count(distinct tdd.BarCode) as BarCode, DATENAME(MONTH,GETDATE()) as MM,CAST(YEAR(GETDATE()) AS VARCHAR(4)) AS YYYY
                //                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATENAME(MONTH,tdd.DistributionDate)=DATENAME(MONTH,GETDATE()) 
                //                and DATENAME(YEAR,tdd.DistributionDate)=DATENAME(YEAR,GETDATE()) and (tdd.Model like '%Primo%' or tdd.Model like '%Walpad%') group by tdd.DistributionDate) as C");

                string getSmartPhoneServiceQuery = string.Format(@"      
                select * from
                (SELECT count(distinct tdd.BarCode) as BarCode, DATEPART(mm,tdd.DistributionDate) as MonNum,CONVERT(varchar(12),
                 DATENAME(MONTH, tdd.DistributionDate)) as MonName,CONVERT(varchar(4),DATEPART(yy, tdd.DistributionDate)) as Year
                from RBSYNERGY.[dbo].tblDealerDistributionDetails tdd where DATEPART(mm,tdd.DistributionDate)={0}
               and DATENAME(YEAR, tdd.DistributionDate)={1} and (tdd.Model like '%Primo%' or tdd.Model like '%Walpad%')
                group by DATEPART(mm,tdd.DistributionDate),DATEPART(yy, tdd.DistributionDate),CONVERT(varchar(12),DATENAME(MONTH, tdd.DistributionDate))) as C", monIds, yearIds);

                var command = new SqlCommand(getSmartPhoneServiceQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    smartPhone = new VmIncentivePolicy
                    {
                        TotalSalesBarcode = reader["BarCode"].ToString(),
                        //Month = reader["Month"].ToString(),
                        //Year = reader["Year"].ToString()
                        Month = reader["MonName"].ToString(),
                        MonNum = Convert.ToInt32(reader["MonNum"]),
                        Year = reader["Year"].ToString()
                    };
                    smartServiceList.Add(smartPhone);
                }
                connection.Close();

            }
            return smartServiceList;
        }

        public List<VmIncentivePolicy> GetCmUserListNew(long monIds, long yearIds)
        {

            //if (monIds == 1)
            //{
            //    monIds = 12;
            //    yearIds = yearIds - 1;
            //}
            //else if(monIds==0 && yearIds==0)
            //{
            //    monIds = 0;
            //    yearIds = 0;
            //}
            //else
            //{
            //    monIds = monIds - 1;
            //    yearIds = yearIds;
            //}


            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename in ('CM','CMHEAD') and cm.IsActive=1 and iss.Category=1
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }
        public List<VmIncentivePolicy> GetCmUserList1New(long monIds, long yearIds)
        {
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename='CM' and cm.IsActive=1
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public List<VmIncentivePolicy> GetCmUserList2New(long monIds, long yearIds)
        {
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename='CM' and cm.IsActive=1 and iss.Category=2
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public List<VmIncentivePolicy> GetCmUserList3New(long monIds, long yearIds)
        {
            string getUserQuery = string.Format(@"SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iss.EmployeeCode,iss.CarryAmount,iss.Share,iit.Amount,iit.UserId,iit.ThisMonthAmount,
            iit.AmountCarry,iit.DepartmentName,iit.Percentage,iit.TotalAmount,iit.TotalIncentive,iit.FixedIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].IncentiveShare iss on cm.EmployeeCode=iss.EmployeeCode
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=iss.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename in ('CM','CMHEAD') and cm.IsActive=1 
            order by iss.Share desc", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(
                    getUserQuery).ToList();
            return getUserList;
        }


        public string GetSaveIncentiveNew(string month, string monNum, string year, string totalAmount, List<Incentive> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var existing = _dbEntities.Incentives.Where(p => month.Contains(p.Month)).ToList();

            foreach (var insResult in results)
            {

                // if (existing.FirstOrDefault(p => p.Month == month && p.Year == Convert.ToInt64(year) && p.DepartmentName.Contains("CM")) == null)
                //  {
                var model = new DAL.DbModel.Incentive
                {
                    Month = month,
                    MonNum = Convert.ToInt32(monNum),
                    Year = Convert.ToInt64(year),
                    TotalAmount = Convert.ToDecimal(totalAmount),
                    ThisMonthAmount = Convert.ToDecimal(insResult.ThisMonthAmount),
                    Amount = Convert.ToDecimal(insResult.FinalAmount),
                    UserId = insResult.Id,
                    DepartmentName = insResult.Role,
                    Percentage = Convert.ToInt64(insResult.Percentage),
                    TotalIncentive = Convert.ToDecimal(insResult.Incentives),
                    FixedIncentive = insResult.FixedIncentive,
                    AddedAmount = insResult.AddedAmount,
                    AmountDeduction = insResult.AmountDeduction,
                    Remarks = insResult.Remarks,
                    DeductionRemarks = insResult.DeductionRemarks,
                    AmountCarry = Convert.ToInt64(insResult.CarryOver),
                    Added = userId,
                    AddedDate = DateTime.Now
                };
                _dbEntities.Incentives.AddOrUpdate(model);
                // }

            }
            _dbEntities.SaveChanges();

            return "ok";

        }


        public string GetSaveIncentive21New(string month, string monNum, string year, string totalAmount21, List<Incentive> results21)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var existing = _dbEntities.Incentives.Where(p => month.Contains(p.Month)).ToList();



            foreach (var insResult in results21)
            {

                //if (existing.FirstOrDefault(p => p.Month == month && p.DepartmentName.Contains("CM") && p.Year == Convert.ToInt64(year)) == null)
                // {
                var model = new DAL.DbModel.Incentive
                {
                    Month = month,
                    MonNum = Convert.ToInt32(monNum),
                    Year = Convert.ToInt64(year),
                    TotalAmount = Convert.ToDecimal(totalAmount21),
                    ThisMonthAmount = Convert.ToDecimal(insResult.ThisMonthAmount),
                    Amount = Convert.ToDecimal(insResult.FinalAmount),
                    UserId = insResult.Id,
                    DepartmentName = insResult.Role,
                    Percentage = Convert.ToInt64(insResult.Percentage),
                    TotalIncentive = Convert.ToDecimal(insResult.Incentives),
                    FixedIncentive = insResult.FixedIncentive,
                    AddedAmount = insResult.AddedAmount,
                    AmountDeduction = insResult.AmountDeduction,
                    Remarks = insResult.Remarks,
                    DeductionRemarks = insResult.DeductionRemarks,
                    AmountCarry = Convert.ToInt64(insResult.CarryOver),
                    Added = userId,
                    AddedDate = DateTime.Now
                };
                _dbEntities.Incentives.AddOrUpdate(model);
                // }

            }
            _dbEntities.SaveChanges();

            return "ok";
        }

        public List<VmIncentivePolicy> GetIncentiveReportNew(string monId, long yearIds)
        {
            int MonNum = Convert.ToInt32(monId);

            //            string getIncentiveReportQuery = string.Format(@"select cm.UserFullName,ii.UserId,ii.TotalAmount,ii.Amount,ii.TotalIncentive,ii.FixedIncentive,ii.Percentage,ii.Month from CellPhoneProject.dbo.CmnUsers cm 
            //            left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
            //            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD','SPRHEAD') and cm.IsActive=1 and ii.MonNum={0}  and ii.Year={1} order by ii.Percentage desc", MonNum, yearIds);
            string getIncentiveReportQuery = string.Format(@"select cm.UserFullName,ii.UserId,
            case when DepartmentName='SPRHEAD' then ThisMonthAmount else 
            ii.TotalAmount end as TotalAmount,ii.Amount,ii.TotalIncentive,ii.FixedIncentive,ii.Percentage,ii.Month,ii.Remarks,ii.AddedAmount
            from CellPhoneProject.dbo.CmnUsers cm left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD','SPRHEAD') and cm.IsActive=1 and ii.MonNum={0}  and ii.Year={1} order by ii.Percentage desc", MonNum, yearIds);

            var getIncentiveReports =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }
        public List<VmIncentivePolicy> GetIncentiveReport1New(string monId, long yearIds)
        {
            int MonNum = Convert.ToInt32(monId);

            string getIncentiveReportQuery = string.Format(@"select top 1 ii.Month,CONVERT(varchar(10), ii.Year) as Year from CellPhoneProject.dbo.CmnUsers cm 
            left join CellPhoneProject.dbo.Incentive ii on cm.EmployeeCode=ii.UserId
            where cm.EmployeeCode=ii.UserId and cm.RoleName in ('CM','CMHEAD') and cm.IsActive=1 and ii.MonNum={0} and  ii.Year={1}", MonNum, yearIds);
            var getIncentiveReports =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }
        public List<VmIncentivePolicy> GetPreparedUserNameNew()
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string getIncentiveReportQuery = string.Format(@"select UserFullName,EmployeeCode  FROM [CellPhoneProject].[dbo].CmnUsers where CmnUserId={0}", userId);
            var getIncentiveReports =
                _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }
        public bool GetCheckDateNew(string monId, string yearId)
        {
            int MonNum = Convert.ToInt32(monId);
            List<VmIncentivePolicy> getIncentiveReports = null;
            if (MonNum > 0 && yearId != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year) from [CellPhoneProject].[dbo].[Incentive] where DepartmentName='CM' and Year={1} and  MonNum={0}", MonNum, yearId);
                getIncentiveReports =
                   _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getIncentiveReportQuery).ToList();


            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;

        }

        public List<VmIncentivePolicy> GetIncentiveSeaShipmentFullsNew(long monIds, long yearIds)
        {
            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentFull from CellPhoneProject.dbo.ProjectMasters pm 
            //join CellPhoneProject.dbo.ProjectOrderShipments pos on pm.ProjectMasterId=pos.ProjectMasterId where DATEDIFF(month, pm.ApproxShipmentDate, pos.WarehouseEntryDate)<=1
            //and pos.WarehouseEntryDate between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-2, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)", monIds, yearIds);
            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentFull from
(select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount

 from CellPhoneProject.dbo.ProjectMasters pm

join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Sea' and
((DATEDIFF(month, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=1) )

group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity

having COUNT(*)=1)pp", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }
        public List<VmIncentivePolicy> GetIncentiveSeaShipmentPartialsNew(long monIds, long yearIds)
        {

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentPartial from
            //            (select distinct pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,wd.Quantity WrQuantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //            join CellPhoneProject.dbo.WarehouseDetails wd on wd.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pm.ApproxShipmentDate)='{0}' and  DATENAME(YEAR,pm.ApproxShipmentDate)='{1}' and pos.ShipmentType='Sea' and
            //            ((DATEDIFF(month, pm.ApproxShipmentDate, wd.WarehouseDate)<=1) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,wd.WarehouseDate,wd.WarehouseQuantity,ppo.Quantity,wd.Quantity
            //
            //            having COUNT(*)>1) pp", monIds, yearIds);

            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as SeaShipmentPartial from
            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount

             from CellPhoneProject.dbo.ProjectMasters pm

            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Sea' and
            ((DATEDIFF(month, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=1) )

            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity

            having COUNT(*)>1)pp", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }

        public List<VmIncentivePolicy> GetIncentiveAirShipmentFullsNew(long monIds, long yearIds)
        {

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentFull from
            //            (select distinct pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,wd.Quantity WrQuantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //            join CellPhoneProject.dbo.WarehouseDetails wd on wd.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pm.ApproxShipmentDate)='{0}' and  DATENAME(YEAR,pm.ApproxShipmentDate)='{1}' and pos.ShipmentType='Air' and
            //            ((DATEDIFF(day, pm.ApproxShipmentDate, wd.WarehouseDate)<=15) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,wd.WarehouseDate,wd.WarehouseQuantity,ppo.Quantity,wd.Quantity
            //
            //            having COUNT(*)=1) pp", monIds, yearIds);


            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentFull from
            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount

             from CellPhoneProject.dbo.ProjectMasters pm

            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Air' and
            ((DATEDIFF(day, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=15) )

            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity

            having COUNT(*)=1)pp", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }
        public List<VmIncentivePolicy> GetIncentiveAirShipmentPartialsNew(long monIds, long yearIds)
        {

            //            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentPartial from
            //            (select distinct pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,wd.Quantity WrQuantity,count(*) as ShipmentCount
            //
            //             from CellPhoneProject.dbo.ProjectMasters pm
            //
            //            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //
            //            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId
            //            join CellPhoneProject.dbo.WarehouseDetails wd on wd.ProjectMasterId=pm.ProjectMasterId
            //
            //            where DATEPART(mm,pm.ApproxShipmentDate)='{0}' and  DATENAME(YEAR,pm.ApproxShipmentDate)='{1}' and pos.ShipmentType='Air' and
            //            ((DATEDIFF(day, pm.ApproxShipmentDate, wd.WarehouseDate)<=15) )
            //
            //            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,wd.WarehouseDate,wd.WarehouseQuantity,ppo.Quantity,wd.Quantity
            //
            //            having COUNT(*)>1) pp", monIds, yearIds);

            string getSeaShipmentFullForIncentive = string.Format(@"select count(*) as AirShipmentPartial from
            (select  pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,ppo.Quantity,count(*) as ShipmentCount

             from CellPhoneProject.dbo.ProjectMasters pm

            join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId

            join CellPhoneProject.dbo.ProjectOrderShipments pos on pos.ProjectMasterId=pm.ProjectMasterId

            where DATEPART(mm,pos.WarehouseEntryDate)='{0}' and  DATENAME(YEAR,pos.WarehouseEntryDate)='{1}' and pos.ShipmentType='Air' and
            ((DATEDIFF(day, pos.WarehouseEntryDate, pm.ApproxShipmentDate)<=15) )

            group by pm.ProjectMasterId,pm.ApproxShipmentDate,ppo.PurchaseOrderNumber,pos.WarehouseEntryDate,ppo.Quantity

            having COUNT(*)>1)pp", monIds, yearIds);

            var getSeaShipmentFulls = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(getSeaShipmentFullForIncentive).ToList();
            return getSeaShipmentFulls;
        }


        #endregion

        #region Spare & Incentive from September 2019
        public List<ProjectMasterModel> GetSpareProjectListNew()
        {
            string proEv = string.Format(@"select pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber from CellPhoneProject.dbo.ProjectMasters pm
            where pm.IsActive=1");

            var proEvent = _dbEntities.Database.SqlQuery<ProjectMasterModel>(proEv).ToList();


            foreach (var project in proEvent)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return proEvent;
        }
        public List<ProjectMasterModel> GetProjectWiseOrderForSpareNew(long proId)
        {
            string proEv = string.Format(@"select ProjectMasterId,OrderNuber from CellPhoneProject.dbo.ProjectMasters
            where ProjectMasterId='{0}'", proId);

            var proEvent = _dbEntities.Database.SqlQuery<ProjectMasterModel>(proEv).ToList();
            return proEvent;
        }
        public ProjectOrderShipmentModel GetWarehouseReceiveDateNew(long proId)
        {
            var projectOrder = new ProjectOrderShipmentModel();
            var proOrderShipment = (from ppo in _dbEntities.ProjectOrderShipments

                                    where ppo.ProjectMasterId == proId
                                    select new
                                    {
                                        ppo.ProjectMasterId,
                                        ppo.WarehouseEntryDate,
                                        ppo.ShipmentType

                                    }).FirstOrDefault();

            if (proOrderShipment != null)
            {
                projectOrder.ProjectMasterId = proOrderShipment.ProjectMasterId;
                projectOrder.WarehouseEntryDate = proOrderShipment.WarehouseEntryDate;
                projectOrder.ShipmentType = proOrderShipment.ShipmentType;
            }

            return projectOrder;
        }
        public bool CheckSpareDataAlreadySavedNew(List<SpareClaimModel> results)
        {
            var spareClaim = new List<SpareClaimModel>();
            foreach (var insResult in results)
            {
                string getSpareClaim = string.Format(@"select * from CellPhoneProject.dbo.SpareClaim where ProjectId='{0}' and SpareClaimDate='{1}' and Quantity='{2}' and WarehouseReceiveDate='{3}'
                ", insResult.ProjectId, insResult.SpareClaimDate, insResult.Quantity, insResult.WarehouseReceiveDate);
                spareClaim =
                   _dbEntities.Database.SqlQuery<SpareClaimModel>(getSpareClaim).ToList();

            }

            if (spareClaim != null && spareClaim.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string SaveSpareClaimDatasNew(List<SpareClaimModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            foreach (var insResult in results)
            {
                var query1 = (from c in _dbEntities.ProjectMasters
                              where c.ProjectMasterId == insResult.ProjectId
                              select c).FirstOrDefault();

                var query2 = (from c in _dbEntities.ProjectPurchaseOrderForms
                              where c.ProjectMasterId == insResult.ProjectId
                              select c).FirstOrDefault();

                var query3 = (from c in _dbEntities.ProjectMasters
                              where c.ProjectMasterId == insResult.ProjectId
                              select c).FirstOrDefault();

                var model1 = new SpareClaim();
                model1.ProjectId = insResult.ProjectId;
                model1.ProjectName = query3.ProjectName;
                model1.OrderNumber = query1.OrderNuber;
                model1.PoCategory = query2.PoCategory;
                //model1.ShipmentWithOrder = insResult.ShipmentWithOrder;
                model1.WarehouseReceiveDate = insResult.WarehouseReceiveDate;
                model1.SpareClaimDate = insResult.SpareClaimDate;
                model1.Remarks = insResult.Remarks;
                model1.Quantity = insResult.Quantity;
                model1.Status = "NEW";
                model1.Added = userId;
                model1.AddedDate = DateTime.Now;

                _dbEntities.SpareClaims.Add(model1);

            }

            _dbEntities.SaveChanges();

            return "ok";
        }
        public List<SpareClaimModel> GetPreviousSpareDatasNew(long proId)
        {
            string proEv = string.Format(@"select * from CellPhoneProject.dbo.SpareClaim
            where ProjectId='{0}'", proId);

            var proEvent = _dbEntities.Database.SqlQuery<SpareClaimModel>(proEv).ToList();
            return proEvent;
        }
        public bool CheckSpareIncentiveDataNew(long monIds, long yearIds)
        {

            var spareClaim = new List<SpareClaimModel>();

            string getSpareClaim = string.Format(@" select MonNum,CONVERT(varchar(10),Year) as Year 
                    from [CellPhoneProject].[dbo].[Incentive] where DepartmentName='SPRHEAD' and  MonNum='{0}' and Year='{1}' 
                ", monIds, yearIds);
            spareClaim =
               _dbEntities.Database.SqlQuery<SpareClaimModel>(getSpareClaim).ToList();

            if (spareClaim != null && spareClaim.Count != 0)
            {
                return true;
            }
            return false;
        }
        public List<SpareClaimModel> GetNewSpareComplainNew()
        {

            string getNew = string.Format(@"SELECT ProjectId,ProjectName,OrderNumber,PoCategory,SpareClaimDate,WarehouseReceiveDate,Quantity,Remarks,CASE WHEN DATEDIFF(m,SpareClaimDate, WarehouseReceiveDate)<=2 THEN 1000 ELSE 0 END AS 'SpareClaimIncentive',
            DATEDIFF(m,SpareClaimDate, WarehouseReceiveDate) as MonthRange,month(WarehouseReceiveDate) as MonNum,DATENAME(MONTH,WarehouseReceiveDate) as Month, year(WarehouseReceiveDate) as YearName
            from CellPhoneProject.dbo.SpareClaim where Status='NEW' ");
            var getNewSpare = _dbEntities.Database.SqlQuery<SpareClaimModel>(getNew).ToList();
            return getNewSpare;
        }
        public string SaveSpareApprovedDataNew(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks,
        string status)
        {

            String userIdentity =
            HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            DateTime date1;
            DateTime.TryParseExact(spareClaimDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date1);

            DateTime spareClaimDate1 = date1;

            DateTime date2;
            DateTime.TryParseExact(warehouseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date2);

            DateTime warehouseDate1 = date2;

            var dbModel1 =
            _dbEntities.SpareClaims.FirstOrDefault(
            x =>
            x.ProjectId == proIds && x.SpareClaimDate == spareClaimDate1 &&
            x.WarehouseReceiveDate == warehouseDate1 && x.Quantity == quantity && x.Remarks == remarks);

            if (dbModel1 != null)
            {
                dbModel1.Status = status;
                dbModel1.Updated = userId;
                dbModel1.UpdatedDate = DateTime.Now;
                _dbEntities.Entry(dbModel1).State = EntityState.Modified;
            }
            _dbEntities.SaveChanges();

            return "ok";

        }

        public string SaveSpareDeclinedDataNew(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks,
            string status)
        {
            throw new NotImplementedException();
        }
        public List<SpareClaimModel> GetTotalSpareClaimNew(string monId, string yearId)
        {
            string getTotalSpare = string.Format(@"select count(*) as TotalSpareClaim from CellPhoneProject.dbo.SpareClaim where status='APPROVED' 
            and month(WarehouseReceiveDate)='{0}' and year(WarehouseReceiveDate)='{1}' ", monId, yearId);
            var getTotalSpareData = _dbEntities.Database.SqlQuery<SpareClaimModel>(getTotalSpare).ToList();
            return getTotalSpareData;
        }

        public List<SpareClaimModel> GetUserInfoForSpareClaimsNew(long monIds, long yearIds)
        {
            string getUserQuery = string.Format(@"
            SELECT cm.UserFullName,cm.UserName,cm.EmployeeCode,cm.RoleName,iit.UserId,iit.ThisMonthAmount,
            iit.DepartmentName,iit.TotalIncentive,iit.AddedAmount,iit.Remarks,iit.AmountDeduction,iit.DeductionRemarks
            FROM [CellPhoneProject].[dbo].[CmnUsers] cm
            left join [CellPhoneProject].[dbo].[Incentive] iit on iit.UserId=cm.EmployeeCode and iit.MonNum={0} and iit.Year={1}
            where cm.rolename='SPRHEAD' and cm.IsActive=1 ", monIds, yearIds);
            var getUserList =
                _dbEntities.Database.SqlQuery<SpareClaimModel>(
                    getUserQuery).ToList();
            return getUserList;
        }

        public string SaveMonthlyIncentiveForSpareClaimsNew(List<SpareClaimModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId1 = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model1 = new DAL.DbModel.Incentive();
                model1.ThisMonthAmount = insResult.ThisMonthAmount;
                model1.TotalIncentive = insResult.TotalIncentive;
                model1.DepartmentName = insResult.RoleName;
                model1.Remarks = insResult.Remarks;
                model1.AddedAmount = insResult.AddedAmount;
                model1.DeductionRemarks = insResult.DeductionRemarks;
                model1.AmountDeduction = insResult.AmountDeduction;
                model1.Month = insResult.Month;
                model1.MonNum = insResult.MonNum;
                model1.Year = insResult.YearName;
                model1.UserId = insResult.UserId;

                model1.Added = userId1;
                model1.AddedDate = DateTime.Now;

                _dbEntities.Incentives.Add(model1);

            }

            _dbEntities.SaveChanges();

            return "ok";
        }

        #endregion

        #region CKD Lock

        public long WarehouseEntryQuantityThisMonth(DateTime poDate, string projectName)
        {
            string query =
                string.Format(
                    @"select SUM(ppf.Quantity) as ParallelWarehouseEntryQuantity from CellPhoneProject.dbo.ProjectMasters pm
inner join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf on pm.ProjectMasterId=ppf.ProjectMasterId
where  pm.SourcingType='CKD' 
and pm.IsActive=1 
and ppf.IsCompleted=0 
and MONTH(DATEADD(MONTH,CASE WHEN (select top 1 ProjectType from ProjectMasters where projectName='{0}')='Smart' THEN 4 ELSE 3 END,CONVERT(datetime2,'{1}',102)))=MONTH(DATEADD(MONTH,CASE WHEN pm.ProjectType='Smart' THEN 4 ELSE 3 END,ppf.PoDate))
and YEAR(DATEADD(MONTH,CASE WHEN (select top 1 ProjectType from ProjectMasters where projectName='{0}')='Smart' THEN 4 ELSE 3 END,CONVERT(datetime2,'{1}',102)))=YEAR(DATEADD(MONTH,CASE WHEN pm.ProjectType='Smart' THEN 4 ELSE 3 END,ppf.PoDate))", projectName, poDate.ToString("yyyy-MM-dd"));
            long? quantity = _dbEntities.Database.SqlQuery<long?>(query).FirstOrDefault();
            return quantity ?? 0;
        }
        #endregion

        #region Jigs & Fixtures

        public void SaveOrUpdateJigsAndFixtures(List<JigsAndFixtureModel> model)
        {
            foreach (var m in model)
            {
                Mapper.CreateMap<JigsAndFixtureModel, JigsAndFixture>();
                var jigsAndFixture = Mapper.Map<JigsAndFixture>(m);
                _dbEntities.JigsAndFixtures.AddOrUpdate(jigsAndFixture);
                _dbEntities.SaveChanges();
            }
        }

        public List<JigsAndFixtureModel> GetJigsAndFixtureModelsByProjectId(long projectId)
        {
            var model =
                _dbEntities.JigsAndFixtures.Where(x => x.ProjectMasterId == projectId).Select(x => new JigsAndFixtureModel
                {
                    JigsFixtureId = x.JigsFixtureId,
                    ProjectMasterId = x.ProjectMasterId,
                    JigsAndFixtureName = x.JigsAndFixtureName,
                    Type = x.Type,
                    Price = x.Price,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    AddedBy = x.AddedBy,
                    AddedDate = x.AddedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).ToList();
            return model;
        }

        #endregion

        #region ChargerPo

        public ChargerPoModel SaveUpdteChargerPoModel(ChargerPoModel model)
        {
            Mapper.CreateMap<ChargerPoModel, ChargerPos>();
            var chargerPo = Mapper.Map<ChargerPos>(model);
            _dbEntities.ChargerPos.AddOrUpdate(chargerPo);
            _dbEntities.SaveChanges();
            model.Id = chargerPo.Id;
            return model;
        }

        public ChargerPoModel GetChargerPoModelById(long id)
        {
            var model = _dbEntities.ChargerPos.Where(m => m.Id == id).Select(m => new ChargerPoModel
            {
                Id = m.Id,
                ChargerPoNo = m.ChargerPoNo,
                OrderNo = m.OrderNo,
                SupplierName = m.SupplierName,
                SupplierAddress = m.SupplierAddress,
                ChargerPoDate = m.ChargerPoDate,
                InvoiceNo = m.InvoiceNo,
                SlNo = m.SlNo,
                Description = m.Description,
                Quantity = m.Quantity,
                VoltageRating = m.VoltageRating,
                CurrentRating = m.CurrentRating,
                PortType = m.PortType,
                ChargerType = m.ChargerType,
                Remarks = m.Remarks,
                CountryOfOrigin = m.CountryOfOrigin,
                AddedBy = m.AddedBy,
                AddedDate = m.AddedDate,
                UpdatedBy = m.UpdatedBy,
                UpdatedDate = m.UpdatedDate
            }).FirstOrDefault();
            return model;
        }

        public List<ChargerPoModel> GetAllChargerPoModels()
        {
            var model = _dbEntities.ChargerPos.Select(m => new ChargerPoModel
            {
                Id = m.Id,
                ChargerPoNo = m.ChargerPoNo,
                OrderNo = m.OrderNo,
                SupplierName = m.SupplierName,
                SupplierAddress = m.SupplierAddress,
                ChargerPoDate = m.ChargerPoDate,
                InvoiceNo = m.InvoiceNo,
                SlNo = m.SlNo,
                Description = m.Description,
                Quantity = m.Quantity,
                VoltageRating = m.VoltageRating,
                CurrentRating = m.CurrentRating,
                PortType = m.PortType,
                ChargerType = m.ChargerType,
                Remarks = m.Remarks,
                CountryOfOrigin = m.CountryOfOrigin,
                AddedBy = m.AddedBy,
                AddedDate = m.AddedDate,
                UpdatedBy = m.UpdatedBy,
                UpdatedDate = m.UpdatedDate
            }).ToList();
            return model;
        }

        public EarphonePoModel SaveUpdateEarphonePoModel(EarphonePoModel model)
        {
            Mapper.CreateMap<EarphonePoModel, EarphonePos>();
            var v = Mapper.Map<EarphonePos>(model);
            _dbEntities.EarphonePos.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            model.Id = v.Id;
            return model;
        }

        public List<EarphonePoModel> GetAllEarphonePoModels()
        {
            var model = _dbEntities.EarphonePos.Select(m => new EarphonePoModel
            {
                Id = m.Id,
                EarphonePoNo = m.EarphonePoNo,
                EarphonePoDate = m.EarphonePoDate,
                InvoiceNo = m.InvoiceNo,
                CountryOfOrigin = m.CountryOfOrigin,
                Description = m.Description,
                Quantity = m.Quantity,
                AddedBy = m.AddedBy,
                AddedDate = m.AddedDate,
                UpdatedBy = m.UpdatedBy,
                UpdatedDate = m.UpdatedDate,
                SupplierName = m.SupplierName,
                SupplierAddress = m.SupplierAddress
            }).ToList();
            return model;
        }

        public EarphonePoModel GetEarphonePoModelById(long id)
        {
            var model = _dbEntities.EarphonePos.Where(m => m.Id == id).Select(m => new EarphonePoModel
            {
                Id = m.Id,
                EarphonePoNo = m.EarphonePoNo,
                EarphonePoDate = m.EarphonePoDate,
                InvoiceNo = m.InvoiceNo,
                CountryOfOrigin = m.CountryOfOrigin,
                Description = m.Description,
                Quantity = m.Quantity,
                AddedBy = m.AddedBy,
                AddedDate = m.AddedDate,
                UpdatedBy = m.UpdatedBy,
                UpdatedDate = m.UpdatedDate,
                SupplierName = m.SupplierName,
                SupplierAddress = m.SupplierAddress
            }).FirstOrDefault();
            return model;
        }

        public List<VmIncentivePolicy> GetRewardAndPenalties(long monIds, long yearIds)
        {
            List<VmIncentivePolicy> cmList = new List<VmIncentivePolicy>();

            _dbEntities.Database.CommandTimeout = 6000;
            //ckd, skd
            //            string proEv1 = string.Format(@"select cast(sum(C.AmountDeduct) as decimal(16,2)) as TotalDeduction, cast(sum(C.AmountReward) as decimal(16,2)) as TotalReward from
            //            (select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,B.OrderNuber,B.PoDate,B.WarehouseEntrydate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
            //            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
            //            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,
            //            B.RewardPoint,
            //            case when B.DaysDiffForReward is null then 0 else B.DaysDiffForReward end as DaysDiffForReward,
            //            case when B.AmountReward is null then 0 else B.AmountReward end as AmountReward,B.IsFinalShipment,TotalRefund=0
            //             from
            //            (select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntrydate, A.DaysDiff,A.EffectiveDays,
            //            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
            //            A.RewardPoint,case when EffectiveDays>DaysDiff then (EffectiveDays-DaysDiff)  end as DaysDiffForReward,case when EffectiveDays>DaysDiff then  (EffectiveDays-DaysDiff) * RewardPoint end as AmountReward,A.IsFinalShipment
            //            from
            //            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
            //            case when pm.ProjectType='Smart' then 150  when  pm.ProjectType='Feature' then 150 end as EffectiveDays, DeductPoint=70, RewardPoint=350,ps.IsFinalShipment
            //
            //            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
            //            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
            //            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            //            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} 
            //            and pm.OrderNuber=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1
            //            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
            //            )A where DaysDiff>=0)B )C", monIds, yearIds);
            // var proEvent1 = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(proEv1).ToList();

            var proEv1 = _dbEntities.GetCmCkdSkdRewardAndPenalties(monIds, yearIds).ToList();

            foreach (var project in proEv1)
            {

                VmIncentivePolicy items = new VmIncentivePolicy();
                items.TotalDeduction = project.TotalDeduction;
                items.TotalReward = project.TotalReward;
                items.TotalRefund = 0;
                cmList.Add(items);
            }
            //repeat
            //            string proEv12 = string.Format(@"select cast(sum(C.AmountDeduct) as decimal(16,2)) as TotalDeduction, cast(sum(C.AmountReward) as decimal(16,2)) as TotalReward from
            //            (select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,B.OrderNuber,B.PoDate,B.WarehouseEntrydate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
            //            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
            //            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,
            //            B.RewardPoint,
            //            case when B.DaysDiffForReward is null then 0 else B.DaysDiffForReward end as DaysDiffForReward,
            //            case when B.AmountReward is null then 0 else B.AmountReward end as AmountReward ,B.IsFinalShipment,TotalRefund=0
            //             from
            //            (select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntrydate, A.DaysDiff,A.EffectiveDays,
            //            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
            //            A.RewardPoint,case when EffectiveDays>DaysDiff then (EffectiveDays-DaysDiff)  end as DaysDiffForReward,case when EffectiveDays>DaysDiff then  (EffectiveDays-DaysDiff) * RewardPoint end as AmountReward,A.IsFinalShipment
            //            from
            //            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
            //            case when ps.ShipmentType='Air' then 80  when  ps.ShipmentType='Sea' then 100 end as EffectiveDays, DeductPoint=70, RewardPoint=350,ps.IsFinalShipment
            //
            //            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
            //            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
            //            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            //            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.OrderNuber !=1 and pm.IsActive=1 and ps.IsFinalShipment='Yes'
            //            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
            //            )A where DaysDiff>=0)B)C", monIds, yearIds);

            var proEvent12 = _dbEntities.GetCmRepeatRewardAndPenalties(monIds, yearIds).ToList();
            foreach (var project in proEvent12)
            {

                VmIncentivePolicy items = new VmIncentivePolicy();
                items.TotalDeduction = project.TotalDeduction;
                items.TotalReward = project.TotalReward;
                items.TotalRefund = project.TotalRefund;
                cmList.Add(items);
            }
            //95 percent product reward
            string proEv13 = string.Format(@"select cast(TotalDeduction as decimal(16,2)) as TotalDeduction, cast(sum(D.RewardAmount) as decimal(16,2)) as TotalReward, cast(sum(D.TotalRefund) as decimal(16,2)) as TotalRefund from
            (select distinct C.ProjectMasterID,C.ProjectModel,C.SourcingType,C.WpmsOrders,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.OrderQuantity,C.TotalProductionQuantity,C.EffectiveDays,C.RewardPercentage,C.ExistedPercentage,
             case when C.ExistedPercentage>=C.RewardPercentage then 2100 else 0 end as RewardAmount,TotalDeduction=0,TotalRefund=0
                 from
	             (
		            select B.ProjectMasterID,B.ProjectModel,B.SourcingType,B.WpmsOrders,B.WarehouseEntryDate,B.ExtendedWarehouseDate,B.OrderQuantity,B.TotalProductionQuantity,B.EffectiveDays,B.RewardPercentage,
		            ((100 * B.TotalProductionQuantity)/OrderQuantity) as ExistedPercentage,B.IsFinalShipment
			              from 
				             (
					              select A.ProjectMasterID,A.ProjectModel,A.SourcingType,A.WpmsOrders,A.WarehouseEntryDate,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity,count(tbi.Barcode) as TotalProductionQuantity,RewardPercentage=95,A.EffectiveDays
					              from 
						            (
							            select AA.ProjectMasterID,AA.ProjectModel,AA.SourcingType,AA.WpmsOrders,AA.WarehouseEntryDate,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate) as ExtendedWarehouseDate,AA.IsFinalShipment,AA.OrderQuantity,AA.EffectiveDays from
							            (
								            select distinct ps.ProjectMasterID,pdd.ProjectModel,pm.SourcingType,('Order '+ cast(pm.OrderNuber as varchar(10))) as WpmsOrders,ps.WarehouseEntryDate,ps.IsFinalShipment,pdd.OrderQuantity,case when pm.SourcingType='SKD' then 30  when  pm.SourcingType='CKD' then 45 end as EffectiveDays
								            from [CellPhoneProject].[dbo].[ProjectOrderShipments] ps
								            left join [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterID=ps.ProjectMasterID
								            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pdd.ProjectMasterID=ps.ProjectMasterID
								            where pm.IsActive=1 and					
								            ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
							            )AA where DATEPART(mm,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))={0} and  DATENAME(YEAR,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))={1}

						            )A
						            left join RBSYNERGY.dbo.tblBarcodeInv tbi on tbi.UpdatedBy=A.WpmsOrders  and tbi.Model=A.ProjectModel
						            where tbi.Model=A.ProjectModel and PrintDate between A.WarehouseEntryDate and DATEADD(day, A.EffectiveDays, A.WarehouseEntryDate)
						            group by  A.ProjectMasterID,A.ProjectModel,A.WpmsOrders,A.WarehouseEntryDate,A.IsFinalShipment,A.OrderQuantity,A.SourcingType,A.EffectiveDays,A.ExtendedWarehouseDate
				            )B

                  )C where C.ExistedPercentage>=C.RewardPercentage)D  group by D.TotalDeduction", monIds, yearIds);

            var proEvent13 = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(proEv13).ToList();

            //  var proEvent13 = _dbEntities.GetCmTotalProductionReward(monIds, yearIds).ToList();
            foreach (var project in proEvent13)
            {
                VmIncentivePolicy items = new VmIncentivePolicy();
                items.TotalDeduction = project.TotalDeduction;
                items.TotalReward = project.TotalReward;
                items.TotalRefund = project.TotalRefund;
                cmList.Add(items);
            }
            //95 percent sales out
            string proEv14 = string.Format(@"select  cast(TotalDeduction as decimal(16,2)) as TotalDeduction, cast(sum(E.RewardAmount) as decimal(16,2)) as TotalReward, cast(sum(E.TotalRefund) as decimal(16,2)) as TotalRefund from
            (SELECT distinct D.ProjectmasterId,D.ProjectModel,D.Orders,D.tblBarcodeOrder,D.WarehouseEntryDate,D.ExtendedWarehouseDate,D.EffectiveDays,D.OrderQuantity, D.TotalTblBarcodeIMEI,D.TotalSalesOut,D.RewardPercentage,D.ExistedPercentage,
             case when D.ExistedPercentage>=D.RewardPercentage then 3500 else 0 end as RewardAmount,TotalDeduction=0,TotalRefund=0
              FROM
               (
                  select C.ProjectmasterId,C.ProjectModel,C.Orders,C.tblBarcodeOrder,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.EffectiveDays,C.OrderQuantity, C.TotalTblBarcodeIMEI,C.TotalSalesOut,C.RewardPercentage,
	              ((100 * C.TotalSalesOut)/OrderQuantity) as ExistedPercentage,IsFinalShipment  from
	                ( 
		               select B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,B.ExtendedWarehouseDate,EffectiveDays=120, sum(TotalTblBarcodeIMEI) as TotalTblBarcodeIMEI,sum(TotalSalesOut) as TotalSalesOut,RewardPercentage=95,IsFinalShipment,B.OrderQuantity  from
				            ( 
				               select A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.ExtendedWarehouseDate, count(A.Barcode) as TotalTblBarcodeIMEI,case when A.TddBarcode is not null and A.TddBarcode !='' then 1 else 0 end as TotalSalesOut,IsFinalShipment,A.OrderQuantity  from
						             (
							             select distinct proM.ProjectMasterId,proM.ProjectModel,proM.Orders,proM.ShipmentType,proM.WarehouseEntryDate,proM.ExtendedWarehouseDate,proM.ShipmentPercentage,proM.IsFinalShipment,
							             tbl.Model,tbl.Barcode,tbl.Barcode2,tbl.DateAdded,tbl.UpdatedBy as tblBarcodeOrder,tdd.Barcode as TddBarcode,proM.OrderQuantity from 
							            (
									            select distinct ps.ProjectMasterId,pdd.ProjectModel, ('Order '+ cast(pm.OrderNuber as varchar(10))) as Orders,ps.ShipmentType,ps.WarehouseEntryDate,DATEADD(day, 120, ps.WarehouseEntryDate) AS ExtendedWarehouseDate,ps.ShipmentPercentage,ps.IsFinalShipment,pdd.OrderQuantity
									            FROM [CellPhoneProject].[dbo].[ProjectOrderShipments] ps 
									            left join CellphoneProject.dbo.ProjectMasters pm on ps.ProjectMasterId=pm.ProjectMasterId
									            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pm.ProjectMasterID=pdd.ProjectMasterID
									            where DATEPART(mm,DATEADD(day, 120, ps.WarehouseEntryDate))={0} and  DATENAME(YEAR,DATEADD(day, 120, ps.WarehouseEntryDate))={1} and pm.IsActive=1 and 
									             ps.WarehouseEntryDate in (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
							            )proM
							            left join [RBSYNERGY].[dbo].[tblBarCodeInv] tbl on proM.ProjectModel=tbl.Model and RTRIM(tbl.UpdatedBy)=RTRIM(proM.Orders)
							            left join [RBSYNERGY].[dbo].tblDealerDistributionDetails tdd on tbl.Barcode =tdd.Barcode and
							            tdd.DistributionDate between proM.WarehouseEntryDate and  DATEADD(day, 120, proM.WarehouseEntryDate)

							            where proM.ProjectModel=tbl.Model 
						            )A
					            group by A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.Barcode,A.TddBarcode,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity
				             )B
				             group by B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,ExtendedWarehouseDate,IsFinalShipment,B.OrderQuantity
		               )C 
               )D where  D.ExistedPercentage>=D.RewardPercentage)E  group by E.TotalDeduction", monIds, yearIds);

            var proEvent14 = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(proEv14).ToList();
            //var proEvent14 = _dbEntities.GetCmTotalSalesOutReward(monIds, yearIds).ToList();

            foreach (var project in proEvent14)
            {
                VmIncentivePolicy items = new VmIncentivePolicy();
                items.TotalDeduction = project.TotalDeduction;
                items.TotalReward = project.TotalReward;
                items.TotalRefund = project.TotalRefund;
                cmList.Add(items);
            }
            string proEv15 = string.Format(@"select A.TotalRefund, cast(A.TotalDeduction as decimal(16,2)) as TotalDeduction,cast(sum(A.TotalReward) as decimal(16,2)) as TotalReward from
         (select sum(cast(RefundAmount as decimal(16,2))) as TotalRefund,TotalDeduction=0,TotalReward=0  
          FROM [CellPhoneProject].[dbo].[Cm_RefundProjectAmount] where MonNum={0} and Year={1})A group by A.TotalRefund,A.TotalDeduction,A.TotalReward", monIds, yearIds);

            var proEvent15 = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(proEv15).ToList();
            foreach (var project in proEvent15)
            {
                VmIncentivePolicy items = new VmIncentivePolicy();
                items.TotalDeduction = project.TotalDeduction;
                items.TotalReward = project.TotalReward;
                items.TotalRefund = project.TotalRefund;
                cmList.Add(items);
            }

            VmIncentivePolicy itemsTotal = new VmIncentivePolicy();

            itemsTotal.TotalDeduction1 = cmList.Sum(i => i.TotalDeduction);
            itemsTotal.TotalReward1 = cmList.Sum(i => i.TotalReward);
            itemsTotal.TotalRefund1 = cmList.Sum(i => i.TotalRefund);

            cmList.Add(itemsTotal);

            return cmList;
        }

        public List<VmIncentivePolicy> GetChinaIqcIncentive(long monIds, long yearIds)
        {

            _dbEntities.Database.CommandTimeout = 6000;

            string proEv14 = string.Format(@"	
             select ProjectMasterId,ProjectName,Orders,PoCategory,PoQuantity,LotNumber,LotQuantity,ProjectManagerClearanceDate,ChinaIqcPassHundredPercent,NoOfTimeInspection,
             cast(Amount as decimal(18,2)) as Amount
             from
              (  select ProjectMasterId,ProjectName,Orders,PoCategory,PoQuantity,LotNumber,LotQuantity,ProjectManagerClearanceDate,ChinaIqcPassHundredPercent,NoOfTimeInspection,
	              case when Amount is null then 0 else Amount end as Amount from
	              (	
		            select [ProjectMasterId],[ProjectName],Orders,[PoCategory],[PoQuantity],[LotNumber],[LotQuantity],[ProjectManagerClearanceDate],ChinaIqcPassHundredPercent,NoOfTimeInspection,
		            case when NoOfTimeInspection=1 and ChinaIqcPassHundredPercent='Yes' then 3500 when NoOfTimeInspection=2 and ChinaIqcPassHundredPercent='No' then -700
		            when NoOfTimeInspection=3 and ChinaIqcPassHundredPercent='No' then -2100 end as Amount
		            FROM [CellPhoneProject].[dbo].[RawMaterialInspection]
		            where DATEPART(mm,[ProjectManagerClearanceDate])='{0}' and  DATENAME(YEAR,[ProjectManagerClearanceDate])='{1}' 
	             )A
             )B
            ", monIds, yearIds);

            var proEvent14 = _dbEntities.Database.SqlQuery<VmIncentivePolicy>(proEv14).ToList();

            return proEvent14;
        }

        public List<NinetyFiveProductionRewardModel> CmPenaltiesAndRewardCkdSkd(string MonNum, string Year)
        {
            long monIds;
            long yearIds;
            long.TryParse(MonNum, out monIds);
            long.TryParse(Year, out yearIds);
            _dbEntities.Database.CommandTimeout = 6000;

            //            string proEv14 = string.Format(@"select C.ProjectMasterID,C.ProjectName,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.WarehouseEntryDate, cast(C.DaysDiff as bigint) as DaysDiff,cast(C.EffectiveDays as bigint) as EffectiveDays,
            //            cast(C.DeductPoint as bigint) as DeductPoint,cast(C.DaysDiffForDeduct as bigint) as DaysDiffForDeduct,cast(C.AmountDeduct as bigint) as AmountDeduct,cast(C.RewardPoint as bigint) as RewardPoint,
            //            cast(C.DaysDiffForReward as bigint) as DaysDiffForReward,cast(C.AmountReward as bigint) as AmountReward,C.IsFinalShipment 
            //             from
            //            (select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,cast(B.OrderNuber as varchar(50)) as Orders,B.PoDate,B.WarehouseEntryDate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
            //            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
            //            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,
            //            B.RewardPoint,
            //            case when B.DaysDiffForReward is null then 0 else B.DaysDiffForReward end as DaysDiffForReward,
            //            case when B.AmountReward is null then 0 else B.AmountReward end as AmountReward ,B.IsFinalShipment
            //             from
            //            (select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntryDate, A.DaysDiff,A.EffectiveDays,
            //            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
            //            A.RewardPoint,case when EffectiveDays>DaysDiff then (EffectiveDays-DaysDiff)  end as DaysDiffForReward,case when EffectiveDays>DaysDiff then  (EffectiveDays-DaysDiff) * RewardPoint end as AmountReward,A.IsFinalShipment
            //            from
            //            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
            //            case when pm.ProjectType='Smart' then 150  when  pm.ProjectType='Feature' then 150 end as EffectiveDays, DeductPoint=70, RewardPoint=350,ps.IsFinalShipment
            //
            //            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
            //            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
            //            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            //            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.IsActive=1 and pm.OrderNuber=1 and ps.IsFinalShipment='Yes'
            //            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
            //            )A where DaysDiff>=0)B )C order by C.ProjectName asc", monIds, yearIds);

            //            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();

            List<NinetyFiveProductionRewardModel> ckdSkdList = new List<NinetyFiveProductionRewardModel>();

            var proEventFeature = _dbEntities.CmPenaltiesAndRewardCkdSkdDetails(monIds, yearIds).ToList();
            foreach (var details in proEventFeature)
            {
                var model = new NinetyFiveProductionRewardModel();
                model.ProjectMasterID = details.ProjectMasterID;
                model.ProjectName = details.ProjectName;
                model.ProjectType = details.ProjectType;
                model.ShipmentType = details.ShipmentType;
                model.Orders = details.Orders;
                model.PoDate = details.PoDate;
                model.WarehouseEntryDate = details.WarehouseEntryDate;
                model.DaysDiff = Convert.ToInt32(details.DaysDiff);
                model.EffectiveDays = Convert.ToInt32(details.EffectiveDays);
                model.DeductPoint = Convert.ToInt32(details.DeductPoint);
                model.DaysDiffForDeduct = Convert.ToInt32(details.DaysDiffForDeduct);
                model.AmountDeduct = details.AmountDeduct;
                model.RewardPoint = Convert.ToInt32(details.RewardPoint);
                model.DaysDiffForReward = Convert.ToInt32(details.DaysDiffForReward);
                model.AmountReward = details.AmountReward;
                model.IsFinalShipment = details.IsFinalShipment;

                ckdSkdList.Add(model);
            }


            return ckdSkdList;
        }

        public List<NinetyFiveProductionRewardModel> CmPenaltiesAndRewardRepeatOrder(string monNum, string year)
        {
            long monIds;
            long yearIds;
            long.TryParse(monNum, out monIds);
            long.TryParse(year, out yearIds);
            _dbEntities.Database.CommandTimeout = 6000;

            //            string proEv14 = string.Format(@"select C.ProjectMasterID,C.ProjectName,C.ProjectType,C.ShipmentType,C.Orders,C.PoDate,C.WarehouseEntryDate, cast(C.DaysDiff as bigint) as DaysDiff,cast(C.EffectiveDays as bigint) as EffectiveDays,
            //            cast(C.DeductPoint as bigint) as DeductPoint,cast(C.DaysDiffForDeduct as bigint) as DaysDiffForDeduct,cast(C.AmountDeduct as bigint) as AmountDeduct,cast(C.RewardPoint as bigint) as RewardPoint,
            //            cast(C.DaysDiffForReward as bigint) as DaysDiffForReward,cast(C.AmountReward as bigint) as AmountReward,C.IsFinalShipment 
            //             from
            //            (select distinct B.ProjectMasterId,B.ProjectName,B.ProjectType,B.ShipmentType,cast(B.OrderNuber as varchar(50)) as Orders,B.PoDate,B.WarehouseEntryDate, B.DaysDiff,B.EffectiveDays,B.DeductPoint,
            //            case when B.DaysDiffForDeduct is null then 0 else B.DaysDiffForDeduct end as DaysDiffForDeduct,
            //            case when B.AmountDeduct is null then 0 else B.AmountDeduct end as AmountDeduct,
            //            B.RewardPoint,
            //            case when B.DaysDiffForReward is null then 0 else B.DaysDiffForReward end as DaysDiffForReward,
            //            case when B.AmountReward is null then 0 else B.AmountReward end as AmountReward ,B.IsFinalShipment
            //             from
            //            (select A.ProjectMasterId,A.ProjectName,A.ProjectType,A.ShipmentType,A.OrderNuber,A.PoDate,A.WarehouseEntryDate, A.DaysDiff,A.EffectiveDays,
            //            A.DeductPoint,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays)  end as DaysDiffForDeduct,case when DaysDiff>EffectiveDays then (DaysDiff-EffectiveDays) * DeductPoint end as AmountDeduct,
            //            A.RewardPoint,case when EffectiveDays>DaysDiff then (EffectiveDays-DaysDiff)  end as DaysDiffForReward,case when EffectiveDays>DaysDiff then  (EffectiveDays-DaysDiff) * RewardPoint end as AmountReward,A.IsFinalShipment
            //            from
            //            (select distinct ppf.ProjectMasterId,pm.ProjectName,pm.ProjectType,ps.ShipmentType,pm.OrderNuber,ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysDiff,
            //            case when ps.ShipmentType='Air' then 80  when  ps.ShipmentType='Sea' then 100 end as EffectiveDays, DeductPoint=70, RewardPoint=350,ps.IsFinalShipment
            //
            //            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
            //            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
            //            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
            //            where DATEPART(mm,ps.WarehouseEntryDate)={0} and  DATENAME(YEAR,ps.WarehouseEntryDate)={1} and pm.IsActive=1  and pm.OrderNuber !=1 and ps.IsFinalShipment='Yes'
            //            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
            //            )A where DaysDiff>=0)B )C order by C.ProjectName asc", monIds, yearIds);

            //            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();
            List<NinetyFiveProductionRewardModel> ckdSkdList = new List<NinetyFiveProductionRewardModel>();

            var proEventFeature = _dbEntities.CmPenaltiesAndRewardRepeatDetails(monIds, yearIds).ToList();
            foreach (var details in proEventFeature)
            {
                var model = new NinetyFiveProductionRewardModel();
                model.ProjectMasterID = details.ProjectMasterID;
                model.ProjectName = details.ProjectName;
                model.ProjectType = details.ProjectType;
                model.ShipmentType = details.ShipmentType;
                model.Orders = details.Orders;
                model.PoDate = details.PoDate;
                model.WarehouseEntryDate = details.WarehouseEntryDate;
                model.DaysDiff = Convert.ToInt32(details.DaysDiff);
                model.EffectiveDays = Convert.ToInt32(details.EffectiveDays);
                model.DeductPoint = Convert.ToInt32(details.DeductPoint);
                model.DaysDiffForDeduct = Convert.ToInt32(details.DaysDiffForDeduct);
                model.AmountDeduct = details.AmountDeduct;
                model.RewardPoint = Convert.ToInt32(details.RewardPoint);
                model.DaysDiffForReward = Convert.ToInt32(details.DaysDiffForReward);
                model.AmountReward = details.AmountReward;
                model.IsFinalShipment = details.IsFinalShipment;

                ckdSkdList.Add(model);
            }
            return ckdSkdList;
        }

        public List<NinetyFiveProductionRewardModel> CmRewardNinetyFiveProduction(string monNum, string year)
        {
            long monIds;
            long yearIds;
            long.TryParse(monNum, out monIds);
            long.TryParse(year, out yearIds);

            _dbEntities.Database.CommandTimeout = 6000;
            string proEv14 = string.Format(@"select D.ProjectMasterID,D.ProjectModel,D.SourcingType,D.WpmsOrders,D.WarehouseEntryDate,D.ExtendedWarehouseDate,cast(D.OrderQuantity as bigint) as OrderQuantity,cast(D.TotalProductionQuantity as bigint) as TotalProductionQuantity,
            cast(D.EffectiveDays as bigint) as EffectiveDays,cast(D.RewardPercentage as bigint) as RewardPercentage,cast(D.ExistedPercentage as bigint) as ExistedPercentage,cast(D.RewardAmount as bigint) as RewardAmount
            from
            (select distinct C.ProjectMasterID,C.ProjectModel,C.SourcingType,C.WpmsOrders,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.OrderQuantity,C.TotalProductionQuantity,C.EffectiveDays,C.RewardPercentage,C.ExistedPercentage,
            case when C.ExistedPercentage>=C.RewardPercentage then 2100 else 0 end as RewardAmount
            from
            (
            select B.ProjectMasterID,B.ProjectModel,B.SourcingType,B.WpmsOrders,B.WarehouseEntryDate,B.ExtendedWarehouseDate,B.OrderQuantity,B.TotalProductionQuantity,B.EffectiveDays,B.RewardPercentage,
            ((100 * B.TotalProductionQuantity)/OrderQuantity) as ExistedPercentage,B.IsFinalShipment
		            from 
			            (
				            select A.ProjectMasterID,A.ProjectModel,A.SourcingType,A.WpmsOrders,A.WarehouseEntryDate,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity,count(tbi.Barcode) as TotalProductionQuantity,RewardPercentage=95,A.EffectiveDays
				            from 
				            (
					            select AA.ProjectMasterID,AA.ProjectModel,AA.SourcingType,AA.WpmsOrders,AA.WarehouseEntryDate,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate) as ExtendedWarehouseDate,AA.IsFinalShipment,AA.OrderQuantity,AA.EffectiveDays from
					            (
						            select distinct ps.ProjectMasterID,pdd.ProjectModel,pm.SourcingType,('Order '+ cast(pm.OrderNuber as varchar(10))) as WpmsOrders,ps.WarehouseEntryDate,ps.IsFinalShipment,pdd.OrderQuantity,case when pm.SourcingType='SKD' then 30  when  pm.SourcingType='CKD' then 45 end as EffectiveDays
						            from [CellPhoneProject].[dbo].[ProjectOrderShipments] ps
						            left join [CellPhoneProject].[dbo].ProjectMasters pm on pm.ProjectMasterID=ps.ProjectMasterID
						            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pdd.ProjectMasterID=ps.ProjectMasterID
						            where pm.IsActive=1	and				
							            ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
					            )AA where DATEPART(mm,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))={0} and  DATENAME(YEAR,DATEADD(day, AA.EffectiveDays, AA.WarehouseEntryDate))={1}

				            )A
				            left join RBSYNERGY.dbo.tblBarcodeInv tbi on tbi.UpdatedBy=A.WpmsOrders  and tbi.Model=A.ProjectModel
				            where tbi.Model=A.ProjectModel and PrintDate between A.WarehouseEntryDate and DATEADD(day, A.EffectiveDays, A.WarehouseEntryDate)
				            group by  A.ProjectMasterID,A.ProjectModel,A.WpmsOrders,A.WarehouseEntryDate,A.IsFinalShipment,A.OrderQuantity,A.SourcingType,A.EffectiveDays,A.ExtendedWarehouseDate
		            )B

            )C where C.ExistedPercentage>=C.RewardPercentage)D", monIds, yearIds);

            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();

            return proEvent14;
        }

        public List<NinetyFiveProductionRewardModel> CmRewardNinetyFiveSalesOut(string monNum, string year)
        {
            long monIds;
            long yearIds;
            long.TryParse(monNum, out monIds);
            long.TryParse(year, out yearIds);
            _dbEntities.Database.CommandTimeout = 6000;
            string proEv14 = string.Format(@"select distinct E.ProjectmasterID,E.ProjectModel,E.Orders,E.tblBarcodeOrder,E.WarehouseEntryDate,E.ExtendedWarehouseDate,cast(E.EffectiveDays as bigint) as EffectiveDays,cast(E.OrderQuantity as bigint) as OrderQuantity,
            cast(E.TotalTblBarcodeIMEI as bigint) as TotalTblBarcodeIMEI,cast(E.TotalSalesOut as bigint) as TotalSalesOut,cast(E.RewardPercentage as bigint) as RewardPercentage,
            cast(E.ExistedPercentage as bigint) as ExistedPercentage, cast(E.RewardAmount as bigint) as RewardAmount
	            from 
	            ( 
	             SELECT D.ProjectmasterID,D.ProjectModel,D.Orders,D.tblBarcodeOrder,D.WarehouseEntryDate,D.ExtendedWarehouseDate,D.EffectiveDays,D.OrderQuantity, D.TotalTblBarcodeIMEI,D.TotalSalesOut,D.RewardPercentage,D.ExistedPercentage,
	             case when D.ExistedPercentage>=D.RewardPercentage then 3500 else 0 end as RewardAmount
	              FROM
	               (
		              select C.ProjectmasterId,C.ProjectModel,C.Orders,C.tblBarcodeOrder,C.WarehouseEntryDate,C.ExtendedWarehouseDate,C.EffectiveDays,C.OrderQuantity, C.TotalTblBarcodeIMEI,C.TotalSalesOut,C.RewardPercentage,
		              ((100 * C.TotalSalesOut)/OrderQuantity) as ExistedPercentage,IsFinalShipment  from
			            ( 
			               select B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,B.ExtendedWarehouseDate,EffectiveDays=120, sum(TotalTblBarcodeIMEI) as TotalTblBarcodeIMEI,sum(TotalSalesOut) as TotalSalesOut,RewardPercentage=95,IsFinalShipment,B.OrderQuantity  from
					            ( 
					               select A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.ExtendedWarehouseDate, count(A.Barcode) as TotalTblBarcodeIMEI,case when A.TddBarcode is not null and A.TddBarcode !='' then 1 else 0 end as TotalSalesOut,IsFinalShipment,A.OrderQuantity  from
							             (
								             select distinct proM.ProjectMasterId,proM.ProjectModel,proM.Orders,proM.ShipmentType,proM.WarehouseEntryDate,proM.ExtendedWarehouseDate,proM.ShipmentPercentage,proM.IsFinalShipment,
								             tbl.Model,tbl.Barcode,tbl.Barcode2,tbl.DateAdded,tbl.UpdatedBy as tblBarcodeOrder,tdd.Barcode as TddBarcode,proM.OrderQuantity from 
								            (
										            select distinct ps.ProjectMasterId,pdd.ProjectModel, ('Order '+ cast(pm.OrderNuber as varchar(10))) as Orders,ps.ShipmentType,ps.WarehouseEntryDate,DATEADD(day, 120, ps.WarehouseEntryDate) AS ExtendedWarehouseDate,ps.ShipmentPercentage,ps.IsFinalShipment,pdd.OrderQuantity
										            FROM [CellPhoneProject].[dbo].[ProjectOrderShipments] ps 
										            left join CellphoneProject.dbo.ProjectMasters pm on ps.ProjectMasterId=pm.ProjectMasterId
										            left join [CellPhoneProject].[dbo].[ProjectOrderQuantityDetails] pdd on pm.ProjectMasterID=pdd.ProjectMasterID
										            where DATEPART(mm,DATEADD(day, 120, ps.WarehouseEntryDate))={0} and  DATENAME(YEAR,DATEADD(day, 120, ps.WarehouseEntryDate))={1} and  pm.IsActive=1	and	
										            ps.WarehouseEntryDate in (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments  where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate asc)
								            )proM
								            left join [RBSYNERGY].[dbo].[tblBarCodeInv] tbl on proM.ProjectModel=tbl.Model and RTRIM(tbl.UpdatedBy)=RTRIM(proM.Orders)
								            left join [RBSYNERGY].[dbo].tblDealerDistributionDetails tdd on tbl.Barcode =tdd.Barcode and
								            tdd.DistributionDate between proM.WarehouseEntryDate and  DATEADD(day, 120, proM.WarehouseEntryDate)

								            where proM.ProjectModel=tbl.Model 
							            )A
						            group by A.ProjectmasterId,A.ProjectModel,A.Orders,A.tblBarcodeOrder,A.WarehouseEntryDate,A.Barcode,A.TddBarcode,A.ExtendedWarehouseDate,A.IsFinalShipment,A.OrderQuantity
					             )B
					             group by B.ProjectmasterId,B.ProjectModel,B.Orders,B.tblBarcodeOrder,B.WarehouseEntryDate,ExtendedWarehouseDate,IsFinalShipment,B.OrderQuantity
			               )C 
	               )D 
   
               )E where E.RewardAmount>0", monIds, yearIds);

            var proEvent14 = _dbEntities.Database.SqlQuery<NinetyFiveProductionRewardModel>(proEv14).ToList();

            return proEvent14;
        }

        #endregion

        #region Lc Permission

        public LcOpeningPermissionModel AddToLcPermission(LcOpeningPermissionModel lcPermissionModel)
        {
            Mapper.CreateMap<LcOpeningPermissionModel, LCOpeningPermission>();
            var lcPermissionEntity = Mapper.Map<LCOpeningPermission>(lcPermissionModel);

            _dbEntities.LCOpeningPermissions.AddOrUpdate(lcPermissionEntity);
            _dbEntities.SaveChanges();
            lcPermissionModel.Id = lcPermissionEntity.Id;
            var lcPermissionEntityId = lcPermissionEntity.Id;
            if (lcPermissionModel.LcAmount.Any())
            {
                foreach (var item in lcPermissionModel.LcAmount)
                {

                    var lcAmountEntity = new LcPermissionAmount();
                    lcAmountEntity.Amount = item;
                    lcAmountEntity.LcPermissionId = lcPermissionEntityId;

                    _dbEntities.LcPermissionAmounts.AddOrUpdate(lcAmountEntity);
                    _dbEntities.SaveChanges();

                }
            }
            return lcPermissionModel;
        }

        public List<LcOpeningPermissionModel> GetLcPermissionList()
        {
            var lcPermissionModel = _dbEntities.LCOpeningPermissions.Where(v => v.IsActive != false).Select(v => new LcOpeningPermissionModel
            {
                Id = v.Id,
                ProjectMasterId = v.ProjectMasterId,
                CompanyName = v.CompanyName,
                OpeningDate = v.OpeningDate,
                SupplierName = v.SupplierName,
                SupplierGrade = v.SupplierGrade,
                Product = v.Product,
                Model = v.Model,
                OrderNo = v.OrderNo,
                PreviousOrderQunatity = v.PreviousOrderQunatity,
                OrderQuantity = v.OrderQuantity,
                StockQuantity = v.StockQuantity,
                PipelineQuantity = v.PipeLineQuantity,
                ApproxDateOfShipment = v.ApproxDateOfShipment,
                TotalAmount = v.TotalAmount,
                IsApproved = v.IsApproved,
                LcAmount = v.LcAmount,
                Currency = v.Currency,
                TtiPerLine = v.TtiPerLine,
                UnitPrice = v.UnitPrice,
                OraclePoNo = v.OraclePoNo,
                AddedBy = v.AddedBy,
                AddedDate = v.AddedDate,
                Remarks = v.Remarks,
                AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                ApprovedBy = v.ApprovedBy,
                ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.ApprovedBy).Select(x => x.UserFullName).FirstOrDefault(),
                ApprovedDate = v.ApprovedDate,
                ApprovedByRemarks = v.ApprovedByRemarks,
                CheckedBy = v.CheckedBy,
                CheckedDate = v.CheckedDate,
                VerifiedBy = v.VerifiedBy,
                VerifyDate = v.VerifyDate,
                WarehouseReceiveDate = v.WarehouseReceiveDate,
                ShipmentConfirmDate = v.ShipmentConfirmDate,
                SourcingApprovalBy = v.SourcingApprovalBy,
                SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.SourcingApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                SourcingApprovalDate = v.SourcingApprovalDate,
                SourcingRemarks = v.SourcingRemarks,
                CeoApprovalBy = v.CeoApprovalBy,
                CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.CeoApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                CeoApprovalDate = v.CeoApprovalDate,
                CeoRemarks = v.CeoRemarks,
                FinanceApprovalBy = v.FinanceApprovalBy,
                FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.FinanceApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                FinanceApprovalDate = v.FinanceApprovalDate,
                FinanceRemarks = v.FinanceRemarks,
                AccountsApprovalBy = v.AccountsApprovalBy,
                AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AccountsApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                AccountsApprovalDate = v.AccountsApprovalDate,
                AccountsRemarks = v.AccountsRemarks,
                AcknowledgeDate = v.AcknowledgeDate,
                AcknowledgeRemarks = v.AcknowledgeRemarks,
                AcknowledgedBy = v.AcknowledgedBy,
                AcknowledgedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AcknowledgedBy).Select(x => x.UserFullName).FirstOrDefault(),
                BiApprovalBy = v.BiApprovalBy,
                BiApprovalDate = v.BiApprovalDate,
                BiRemarks = v.BiRemarks,
                BiApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.BiApprovalBy).Select(x => x.UserFullName).FirstOrDefault()
            }).ToList();
            return lcPermissionModel;

        }

        public List<LcOpeningPermissionOtherProductModel> GetLcPermissionOtherProductList()
        {
            var query = string.Format("select * from LcOpeningPermissionOtherProducts");
            var list = _dbEntities.Database.SqlQuery<LcOpeningPermissionOtherProductModel>(query).ToList();
            foreach (var l in list)
            {
                l.AddedByName =
                    _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.AddedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.ApprovedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.SourcingApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.CeoApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.FinanceApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.AccountsApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.AcknowledgedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.AcknowledgedBy)
                    .Select(x => x.UserFullName).FirstOrDefault();
                l.BiApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.BiApprovalBy)
                    .Select(x => x.UserFullName).FirstOrDefault();
            }
            return list;
        }

        public List<LcOpeningPermissionOtherProductModel> GetTtPendingLc()
        {
            var query = string.Format("select * from LcOpeningPermissionOtherProducts where TtDate is NULL and ApprovedBy is not null");
            var list = _dbEntities.Database.SqlQuery<LcOpeningPermissionOtherProductModel>(query).ToList();
            foreach (var l in list)
            {
                l.AddedByName =
                    _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.AddedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.ApprovedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.SourcingApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.CeoApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.FinanceApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.AccountsApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                l.AcknowledgedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.AcknowledgedBy)
                    .Select(x => x.UserFullName).FirstOrDefault();
                l.BiApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == l.BiApprovalBy)
                    .Select(x => x.UserFullName).FirstOrDefault();
            }
            return list;
        }

        public LcOpeningPermissionModel GetLcPermissionDetailsById(long id)
        {
            var lcPermissionModel = _dbEntities.LCOpeningPermissions.Where(v => v.Id == id).Select(v => new LcOpeningPermissionModel
            {
                Id = v.Id,
                ProjectMasterId = v.ProjectMasterId,
                CompanyName = v.CompanyName,
                OpeningDate = v.OpeningDate,
                SupplierName = v.SupplierName,
                SupplierGrade = v.SupplierGrade,
                Product = v.Product,
                Model = v.Model,
                OrderNo = v.OrderNo,
                PreviousOrderQunatity = v.PreviousOrderQunatity,
                OrderQuantity = v.OrderQuantity,
                StockQuantity = v.StockQuantity,
                PipelineQuantity = v.PipeLineQuantity,
                ApproxDateOfShipment = v.ApproxDateOfShipment,
                TotalAmount = v.TotalAmount,
                IsApproved = v.IsApproved,
                LcAmount = v.LcAmount,
                Currency = v.Currency,
                TtiPerLine = v.TtiPerLine,
                UnitPrice = v.UnitPrice,
                OraclePoNo = v.OraclePoNo,
                AddedBy = v.AddedBy,
                AddedDate = v.AddedDate,
                Remarks = v.Remarks,
                AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                ApprovedBy = v.ApprovedBy,
                ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.ApprovedBy).Select(x => x.UserFullName).FirstOrDefault(),
                ApprovedDate = v.ApprovedDate,
                ApprovedByRemarks = v.ApprovedByRemarks,
                CheckedBy = v.CheckedBy,
                CheckedDate = v.CheckedDate,
                VerifiedBy = v.VerifiedBy,
                VerifyDate = v.VerifyDate,
                WarehouseReceiveDate = v.WarehouseReceiveDate,
                ShipmentConfirmDate = v.ShipmentConfirmDate,
                SourcingApprovalBy = v.SourcingApprovalBy,
                SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.SourcingApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                SourcingApprovalDate = v.SourcingApprovalDate,
                SourcingRemarks = v.SourcingRemarks,
                CeoApprovalBy = v.CeoApprovalBy,
                CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.CeoApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                CeoApprovalDate = v.CeoApprovalDate,
                CeoRemarks = v.CeoRemarks,
                FinanceApprovalBy = v.FinanceApprovalBy,
                FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.FinanceApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                FinanceApprovalDate = v.FinanceApprovalDate,
                FinanceRemarks = v.FinanceRemarks,
                AccountsApprovalBy = v.AccountsApprovalBy,
                AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AccountsApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                AccountsApprovalDate = v.AccountsApprovalDate,
                AccountsRemarks = v.AccountsRemarks,
                AcknowledgeDate = v.AcknowledgeDate,
                AcknowledgeRemarks = v.AcknowledgeRemarks,
                AcknowledgedBy = v.AcknowledgedBy,
                AcknowledgedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AcknowledgedBy).Select(x => x.UserFullName).FirstOrDefault(),
                BiApprovalBy = v.BiApprovalBy,
                BiApprovalDate = v.BiApprovalDate,
                BiRemarks = v.BiRemarks
            }).FirstOrDefault();
            return lcPermissionModel;
        }

        public LcOpeningPermissionModel UpdateApprovalStatus(long id, string checkedValue)
        {
            var approvalStatus = checkedValue.ToUpper();
            var permissionModel = new LcOpeningPermissionModel();
            var entity = _dbEntities.LCOpeningPermissions.FirstOrDefault(x => x.Id == id);
            try
            {
                if (entity != null)
                {
                    if (approvalStatus == "APPROVED")
                    {
                        entity.IsApproved = true;
                        entity.IsRejected = false;
                    }
                    else if (approvalStatus == "REJECTED")
                    {
                        entity.IsRejected = true;
                        entity.IsApproved = false;
                    }
                    entity.UpdatedDate = DateTime.Now;

                    _dbEntities.Entry(entity).State = EntityState.Modified;
                    _dbEntities.SaveChanges();

                    Mapper.CreateMap<LCOpeningPermission, LcOpeningPermissionModel>();
                    permissionModel = Mapper.Map<LcOpeningPermissionModel>(entity);
                }
                else
                {
                    return permissionModel;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }



            return permissionModel;


            //dbModel1.Status = status;
            //dbModel1.Updated = userId;
            //dbModel1.UpdatedDate = DateTime.Now;
            //_dbEntities.Entry(dbModel1).State = EntityState.Modified;
        }

        public LcOpeningPermissionModel UpdateLcOpeningPermissionModel(LcOpeningPermissionModel m)
        {
            Mapper.CreateMap<LcOpeningPermissionModel, LCOpeningPermission>();
            var v = Mapper.Map<LCOpeningPermission>(m);
            _dbEntities.LCOpeningPermissions.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            return m;
        }

        public LcOpeningPermissionOtherProductModel GetLcOpeningPermissionOtherProductById(long id)
        {
            var v = _dbEntities.LcOpeningPermissionOtherProducts.FirstOrDefault(x => x.Id == id);
            LcOpeningPermissionOtherProductModel m = GenericMapper<LcOpeningPermissionOtherProduct, LcOpeningPermissionOtherProductModel>.GetDestination(v);
            if (m != null)
            {
                m.AddedByName =
                    _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                m.ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ApprovedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                m.SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SourcingApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                m.CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.CeoApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                m.FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.FinanceApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                m.AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AccountsApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
            }

            return m;
        }

        public long SaveLcOpeningOtherProduct(LcOpeningPermissionOtherProductModel m)
        {
            Mapper.CreateMap<LcOpeningPermissionOtherProductModel, LcOpeningPermissionOtherProduct>();
            var v = Mapper.Map<LcOpeningPermissionOtherProduct>(m);
            _dbEntities.LcOpeningPermissionOtherProducts.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            return v.Id;
        }

        public LcOpeningPermissionModel GetLcOpeningPermissionByProjectId(long projectId)
        {
            var lc =
                _dbEntities.LCOpeningPermissions.Where(v => v.ProjectMasterId == projectId)
                    .Select(v => new LcOpeningPermissionModel
                    {
                        Id = v.Id,
                        ProjectMasterId = v.ProjectMasterId,
                        CompanyName = v.CompanyName,
                        OpeningDate = v.OpeningDate,
                        SupplierName = v.SupplierName,
                        SupplierGrade = v.SupplierGrade,
                        Product = v.Product,
                        Model = v.Model,
                        OrderNo = v.OrderNo,
                        PreviousOrderQunatity = v.PreviousOrderQunatity,
                        OrderQuantity = v.OrderQuantity,
                        StockQuantity = v.StockQuantity,
                        PipelineQuantity = v.PipeLineQuantity,
                        ApproxDateOfShipment = v.ApproxDateOfShipment,
                        TotalAmount = v.TotalAmount,
                        IsApproved = v.IsApproved,
                        LcAmount = v.LcAmount,
                        Currency = v.Currency,
                        TtiPerLine = v.TtiPerLine,
                        UnitPrice = v.UnitPrice,
                        OraclePoNo = v.OraclePoNo,
                        AddedBy = v.AddedBy,
                        AddedDate = v.AddedDate,
                        Remarks = v.Remarks,
                        AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                        ApprovedBy = v.ApprovedBy,
                        ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.ApprovedBy).Select(x => x.UserFullName).FirstOrDefault(),
                        ApprovedDate = v.ApprovedDate,
                        ApprovedByRemarks = v.ApprovedByRemarks,
                        CheckedBy = v.CheckedBy,
                        CheckedDate = v.CheckedDate,
                        VerifiedBy = v.VerifiedBy,
                        VerifyDate = v.VerifyDate,
                        WarehouseReceiveDate = v.WarehouseReceiveDate,
                        ShipmentConfirmDate = v.ShipmentConfirmDate,
                        SourcingApprovalBy = v.SourcingApprovalBy,
                        SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.SourcingApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                        SourcingApprovalDate = v.SourcingApprovalDate,
                        SourcingRemarks = v.SourcingRemarks,
                        CeoApprovalBy = v.CeoApprovalBy,
                        CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.CeoApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                        CeoApprovalDate = v.CeoApprovalDate,
                        CeoRemarks = v.CeoRemarks,
                        FinanceApprovalBy = v.FinanceApprovalBy,
                        FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.FinanceApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                        FinanceApprovalDate = v.FinanceApprovalDate,
                        FinanceRemarks = v.FinanceRemarks,
                        AccountsApprovalBy = v.AccountsApprovalBy,
                        AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AccountsApprovalBy).Select(x => x.UserFullName).FirstOrDefault(),
                        AccountsApprovalDate = v.AccountsApprovalDate,
                        AccountsRemarks = v.AccountsRemarks,
                        Rate = v.Rate,
                        BiApprovalBy = v.BiApprovalBy,
                        BiApprovalDate = v.BiApprovalDate,
                        BiRemarks = v.BiRemarks
                    }).OrderByDescending(v => v.AddedDate).Take(1).FirstOrDefault();
            return lc;
        }

        public void SaveLcOpeningPermissionLog(LcOpeningPermissionModel m)
        {
            LcOpeningPermissionLog model = GenericMapper<LcOpeningPermissionModel, LcOpeningPermissionLog>.GetDestination(m);
            model.LcPermissionId = m.Id;
            model.LogAddedBy = m.UpdatedBy;
            model.LogAddedDate = DateTime.Now;
            _dbEntities.LcOpeningPermissionLogs.Add(model);
            _dbEntities.SaveChanges();
        }

        public List<LcOpeningPermissionModel> GetHandsetLcApprovalsByDateRange(DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            var v =
                _dbEntities.LCOpeningPermissions.Where(x => x.ApprovedDate >= fromDate && x.ApprovedDate <= toDate).Select(x => new LcOpeningPermissionModel
                {
                    Id = x.Id,
                    ProjectMasterId = x.ProjectMasterId,
                    CompanyName = x.CompanyName,
                    OpeningDate = x.OpeningDate,
                    SupplierName = x.SupplierName,
                    SupplierGrade = x.SupplierGrade,
                    Product = x.Product,
                    Model = x.Model,
                    OrderNo = x.OrderNo,
                    PreviousOrderQunatity = x.PreviousOrderQunatity,
                    StockQuantity = x.StockQuantity,
                    PipelineQuantity = x.PipeLineQuantity,
                    OrderQuantity = x.OrderQuantity,
                    TotalAmount = x.TotalAmount,
                    ApproxDateOfShipment = x.ApproxDateOfShipment,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                    AddedBy = x.AddedBy,
                    AddedByName = _dbEntities.CmnUsers.Where(y => y.CmnUserId == x.AddedBy).Select(y => y.UserFullName).FirstOrDefault(),
                    IsActive = x.IsActive,
                    IsApproved = x.IsApproved,
                    ApprovedBy = x.ApprovedBy,
                    ApprovedDate = x.ApprovedDate,
                    ApprovedByRemarks = x.ApprovedByRemarks,
                    IsRejected = x.IsRejected,
                    CheckedBy = x.CheckedBy,
                    CheckedDate = x.CheckedDate,
                    VerifiedBy = x.VerifiedBy,
                    VerifyDate = x.VerifyDate,
                    Remarks = x.Remarks,
                    TtiPerLine = x.TtiPerLine,
                    LcAmount = x.LcAmount,
                    UnitPrice = x.UnitPrice,
                    OraclePoNo = x.OraclePoNo,
                    WarehouseReceiveDate = x.WarehouseReceiveDate,
                    ShipmentConfirmDate = x.ShipmentConfirmDate,
                    UpdatedBy = x.UpdatedBy,
                    SourcingApprovalBy = x.SourcingApprovalBy,
                    SourcingApprovalDate = x.SourcingApprovalDate,
                    SourcingRemarks = x.SourcingRemarks,
                    CeoApprovalBy = x.CeoApprovalBy,
                    CeoApprovalDate = x.CeoApprovalDate,
                    CeoRemarks = x.CeoRemarks,
                    AccountsApprovalBy = x.AccountsApprovalBy,
                    AccountsApprovalDate = x.AccountsApprovalDate,
                    AccountsRemarks = x.AccountsRemarks,
                    FinanceApprovalBy = x.FinanceApprovalBy,
                    FinanceApprovalDate = x.FinanceApprovalDate,
                    FinanceRemarks = x.FinanceRemarks,
                    AcknowledgedBy = x.AcknowledgedBy,
                    AcknowledgeDate = x.AcknowledgeDate,
                    AcknowledgeRemarks = x.AcknowledgeRemarks,
                    Rate = x.Rate,
                    Currency = x.Currency,
                    BiApprovalBy = x.BiApprovalBy,
                    BiApprovalDate = x.BiApprovalDate,
                    BiRemarks = x.BiRemarks
                }).ToList();
            return v;
        }

        public List<LcOpeningPermissionOtherProductModel> GetLcOpeningPermissionOtherProdWithinDateRange(DateTime from,
            DateTime to)
        {
            to = to.AddDays(1).AddSeconds(-1);
            var v =
                _dbEntities.LcOpeningPermissionOtherProducts.Where(x => x.ApprovedDate >= from && x.ApprovedDate <= to).Select(x => new LcOpeningPermissionOtherProductModel
                {
                    Id = x.Id,
                    ProductType = x.ProductType,
                    Product = x.Product,
                    CompanyName = x.CompanyName,
                    OpeningDate = x.OpeningDate,
                    SupplierGrade = x.SupplierGrade,
                    SupplierName = x.SupplierName,
                    Model = x.Model,
                    OrderNo = x.OrderNo,
                    OtherProductLcForTheProject = x.OtherProductLcForTheProject,
                    FreeOfCostClaimValue = x.FreeOfCostClaimValue,
                    PreviousOrderQunatity = x.PreviousOrderQunatity,
                    StockQuantity = x.StockQuantity,
                    PipeLineQuantity = x.PipeLineQuantity,
                    OrderQuantity = x.OrderQuantity,
                    TotalAmount = x.TotalAmount,
                    ApproxDateOfShipment = x.ApproxDateOfShipment,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                    AddedBy = x.AddedBy,
                    AddedByName = _dbEntities.CmnUsers.Where(y => y.CmnUserId == x.AddedBy).Select(y => y.UserFullName).FirstOrDefault(),
                    IsActive = x.IsActive,
                    IsApproved = x.IsApproved,
                    ApprovedBy = x.ApprovedBy,
                    ApprovedDate = x.ApprovedDate,
                    ApprovedByRemarks = x.ApprovedByRemarks,
                    IsRejected = x.IsRejected,
                    CheckedBy = x.CheckedBy,
                    CheckedDate = x.CheckedDate,
                    VerifiedBy = x.VerifiedBy,
                    VerifyDate = x.VerifyDate,
                    Remarks = x.Remarks,
                    TtiPerLine = x.TtiPerLine,
                    LcAmount = x.LcAmount,
                    UnitPrice = x.UnitPrice,
                    OraclePoNo = x.OraclePoNo,
                    WarehouseReceiveDate = x.WarehouseReceiveDate,
                    ShipmentConfirmDate = x.ShipmentConfirmDate,
                    UpdatedBy = x.UpdatedBy,
                    SourcingApprovalBy = x.SourcingApprovalBy,
                    SourcingApprovalDate = x.SourcingApprovalDate,
                    SourcingRemarks = x.SourcingRemarks,
                    CeoApprovalBy = x.CeoApprovalBy,
                    CeoApprovalDate = x.CeoApprovalDate,
                    CeoRemarks = x.CeoRemarks,
                    AccountsApprovalBy = x.AccountsApprovalBy,
                    AccountsApprovalDate = x.AccountsApprovalDate,
                    AccountsRemarks = x.AccountsRemarks,
                    FinanceApprovalBy = x.FinanceApprovalBy,
                    FinanceApprovalDate = x.FinanceApprovalDate,
                    FinanceRemarks = x.FinanceRemarks,
                    RelevantWaltonProjects = x.RelevantWaltonProjects,
                    ProductProfile = x.ProductProfile,
                    AcknowledgedBy = x.AcknowledgedBy,
                    AcknowledgeDate = x.AcknowledgeDate,
                    AcknowledgeRemarks = x.AcknowledgeRemarks,
                    Rate = x.Rate,
                    Currency = x.Currency,
                    BiApprovalBy = x.BiApprovalBy,
                    BiApprovalDate = x.BiApprovalDate,
                    BiRemarks = x.BiRemarks,
                    TtDate = x.TtDate,
                    TtNumber = x.TtNumber,
                    TtValue = x.TtValue
                }).ToList();
            return v;
        }
        #endregion

        #region order wise multiple price

        public List<OrderWiseMultiplePriceModel> GetallOrderWiseMultiplePrice()
        {
            var v = _dbEntities.OrderWiseMultiplePrices.Select(x => new OrderWiseMultiplePriceModel
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.ProjectName).FirstOrDefault(),
                OrderNumber = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.OrderNuber).FirstOrDefault(),
                Quantity = x.Quantity,
                OrderQuantity = _dbEntities.ProjectPurchaseOrderForms.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.Quantity).FirstOrDefault(),
                Price = x.Price,
                Remarks = x.Remarks,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            }).ToList();
            return v;
        }

        public OrderWiseMultiplePriceModel GetOrderWiseMultiplePriceById(long id)
        {
            var v = _dbEntities.OrderWiseMultiplePrices.Where(x => x.Id == id).Select(x => new OrderWiseMultiplePriceModel
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.ProjectName).FirstOrDefault(),
                OrderNumber = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.OrderNuber).FirstOrDefault(),
                Quantity = x.Quantity,
                OrderQuantity = _dbEntities.ProjectPurchaseOrderForms.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.Quantity).FirstOrDefault(),
                Price = x.Price,
                Remarks = x.Remarks,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            }).FirstOrDefault();
            return v;
        }

        public void SaveOrderWiseMultiplePrice(OrderWiseMultiplePriceModel orderWiseMultiplePrice)
        {
            Mapper.CreateMap<OrderWiseMultiplePriceModel, OrderWiseMultiplePrice>();
            var v = Mapper.Map<OrderWiseMultiplePrice>(orderWiseMultiplePrice);
            _dbEntities.OrderWiseMultiplePrices.Add(v);
            _dbEntities.SaveChanges();
        }

        public OrderWiseMultiplePriceModel UpdateOrderWiseMultiplePrice(OrderWiseMultiplePriceModel orderWiseMultiplePrice)
        {
            Mapper.CreateMap<OrderWiseMultiplePriceModel, OrderWiseMultiplePrice>();
            var v = Mapper.Map<OrderWiseMultiplePrice>(orderWiseMultiplePrice);
            _dbEntities.OrderWiseMultiplePrices.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            return orderWiseMultiplePrice;
        }
        #endregion

        public VmImeiDataBase GetProjectBabtList(VmImeiDataBase model)
        {
            List<ProjectBabtModel> models = (from babt in _dbEntities.ProjcetBabts
                                             join master in _dbEntities.ProjectMasters on babt.ProjectMasterId equals master.ProjectMasterId
                                             where babt.UpdatedDate >= model.StartDate && babt.UpdatedDate <= model.EndDate
                                             orderby babt.UpdatedDate, master.ProjectModel, babt.TacNo
                                             select new ProjectBabtModel
                                             {
                                                 TacNo = babt.TacNo,
                                                 ProjectName = master.ProjectModel,
                                                 ImeiRangeTo = babt.ImeiRangeTo,
                                                 ImeiRangeFrom = babt.ImeiRangeFrom,
                                                 UpdatedDate = babt.UpdatedDate
                                             }).ToList();
            model.ProjcetBabts = models;
            return model;
        }

        public List<ProjectMasterModel> GetAllProductModel()
        {
            //    var allProjects = _dbEntities.Database.SqlQuery<ProjectMasterModel>(@" select ProjectMasterId, ProjectModel,ProjectName, OrderNuber from CellPhoneProject.dbo.ProjectMasters
            //    where ProjectModel is not null and ProjectStatus = 'APPROVED' and IsActive=1 order by ProjectModel asc").ToList();

            var allProjects = _dbEntities.Database.SqlQuery<ProjectMasterModel>(@"
            select po.ProjectMasterId, po.ProjectModel, cast(po.OrderQuantity as decimal(18,2)) as OrderQuantity,pm.OrderNuber from
            CellPhoneProject.dbo.[ProjectOrderQuantityDetails] po
            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterID=po.ProjectMasterID
            where po.ProjectModel is not null and pm.ProjectStatus = 'APPROVED' and pm.IsActive=1 and po.IsActive=1 order by ProjectModel asc").ToList();

            foreach (var project in allProjects)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectModel = project.ProjectModel + " -> (" + project.OrderNumberOrdinal + ") (Order Qty: " + project.OrderQuantity + ")";
                }
            }
            return allProjects;
        }

        public List<ProjectOrderShipmentModel> GetFinishGoodDetails(long proShipOrder)
        {
            var nocList = _dbEntities.Database.SqlQuery<ProjectOrderShipmentModel>(@"select [ProjectOrderShipmentId]
              ,[FinishGoodProjectMasterId]
              ,[FinishGoodModel]
              ,[FinishGoodModelOrderNumber]
              ,[ApproxFinishGoodManufactureQty] from [CellPhoneProject].[dbo].[ShipmentFinishGoodModel]
             where ProjectOrderShipmentId={0} ", proShipOrder).ToList();
            return nocList;
        }

        #region IDH LC
        public List<LC_IDH_Final_BOMModel> GetLcIdhFinalBomModelByVariantId(long id)
        {
            var v = _dbEntities.LC_IDH_Final_BOM.Where(x => x.VariantId == id).ToList();
            var remainingIdhLc = new List<LC_IDH_Final_BOMModel>();
            foreach (var item in v)
            {
                LC_IDH_Final_BOMModel model = GenericMapper<LC_IDH_Final_BOM, LC_IDH_Final_BOMModel>.GetDestination(item);
                var prevOrderQuantity = _dbEntities.LC_IDH_Details.Where(x => x.VariantId == id && x.LcIdhFinalBomId == item.Id).Select(x => x.OrderQuantity).Sum() ?? 0;
                model.RemainingQuantity = (int?)(item.TotalQuantityConsideringWastage - prevOrderQuantity);
                if (model.RemainingQuantity != 0)
                {
                    remainingIdhLc.Add(model);
                }
            }
            return remainingIdhLc;
        }

        public List<LC_IDH_Final_BOM> GetLcIdhFinalBomsByVariantId(long id)
        {
            var v = _dbEntities.LC_IDH_Final_BOM.Where(x => x.VariantId == id).ToList();
            return v;
        }

        public LC_IDH_Final_BOM GetIDHFinalBomInfoBySpareId(long id)
        {
            var v = _dbEntities.LC_IDH_Final_BOM.FirstOrDefault(x => x.Id == id);
            return v;
        }

        public List<LC_IDH_DetailsModel> GetPrevIdhDetailsByVariantId(long variantId)
        {
            var idhDetails = _dbEntities.LC_IDH_Details.Where(x => x.VariantId == variantId).ToList();
            var idhDetailList = new List<LC_IDH_DetailsModel>();
            foreach (var v in idhDetails)
            {
                LC_IDH_DetailsModel model = GenericMapper<LC_IDH_Details, LC_IDH_DetailsModel>.GetDestination(v);
                model.MaterialCoding =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.MaterialCoding).FirstOrDefault();
                model.MaterialName =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.MaterialName).FirstOrDefault();
                model.Specification =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.Specification).FirstOrDefault();
                model.InventoryCode =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.InventoryCode).FirstOrDefault();
                model.Vendor =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.Vendor).FirstOrDefault();
                model.TotalQuantity =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.TotalQuantity).FirstOrDefault();
                model.TotalQuantityConsideringWastage =
                    _dbEntities.LC_IDH_Final_BOM.Where(x => x.Id == v.LcIdhFinalBomId).Select(x => x.TotalQuantityConsideringWastage).FirstOrDefault();
                idhDetailList.Add(model);
            }
            return idhDetailList;
        }

        public void SaveIdhBom(LC_IDH_Final_BOMModel model)
        {
            Mapper.CreateMap<LC_IDH_Final_BOMModel, LC_IDH_Final_BOM>();
            var save = Mapper.Map<LC_IDH_Final_BOM>(model);
            _dbEntities.LC_IDH_Final_BOM.Add(save);
            _dbEntities.SaveChanges();
        }

        //public long SaveIdhLcMaster(LC_IDH_Masters model)
        //{
        //    var v=_dbEntities.LC_IDH_Masters.Add(model);
        //    _dbEntities.SaveChanges();
        //    return v.Id;
        //}

        public void SaveIdhLcDetails(LC_IDH_Details model)
        {
            _dbEntities.LC_IDH_Details.Add(model);
            _dbEntities.SaveChanges();
        }

        public int? GetLastOrderSerialInIdhDetails(long variantId)
        {
            var v =
                _dbEntities.LC_IDH_Details.Where(x => x.VariantId == variantId)
                    .OrderByDescending(x => x.OrderSerial)
                    .Take(1)
                    .FirstOrDefault();
            int? orderSerial = 0;
            orderSerial = v != null ? v.OrderSerial : orderSerial;
            return orderSerial;
        }
        #endregion

        #region CMBTRC Milon vai
        public List<ModelListForIMEIDownload> GetModelList(DateTime fromDate, DateTime todate)
        {
            try
            {
                _rbsynergyEntities.Database.CommandTimeout = 6000;
                string query = string.Format(@"Select d.Model from tblBarCodeInv d 
                                                where CAST(d.PrintDate as date) between '{0}' and '{1}'
                                                Group by d.Model", fromDate, todate);
                var data = _rbsynergyEntities.Database.SqlQuery<ModelListForIMEIDownload>(query).ToList();
                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }



        public List<ModelListForIMEIDownload> GetModelWiseReportData(DateTime fromDate, DateTime todate, string modelname)
        {
            try
            {
                _rbsynergyEntities.Database.CommandTimeout = 6000;

                var query = string.Format(@"Select ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS SERIAL_NO,
                                            CASE WHEN Model LIKE '%Axino%' THEN 'Marcel Mobile'	else 'Walton Mobile' end as Brand
                                            ,LEFT(BarCode, 8)as [IMEI_TAC_1],''[IMEI_TAC_2],''[IMEI_TAC_3],''[IMEI_TAC_4],BarCode as IMEI1,BarCode2 as IMEI2,''IMEI3,''IMEI4 from tblBarCodeInv
                                            where Model='{2}'
                                            and CAST(PrintDate as date) between '{0}' and '{1}'", fromDate, todate, modelname);

                var data = _rbsynergyEntities.Database.SqlQuery<ModelListForIMEIDownload>(query).ToList();

                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }//end
        #endregion

        public List<FobPriceUpdateLog> GetFobPriceUpdateLogByProjectId(long projectId)
        {
            var query = string.Format(@"select cast(FinalPrice as nvarchar) as FinalPrice
,cast(case when cast(UpdatedDate as Date) is null then cast(AddedDate as date) else cast(UpdatedDate as Date) end as nvarchar) as UpdatedDate
from ProjectMasterTrackers
where ProjectMasterId={0} and FinalPrice is not null
group by FinalPrice,cast(UpdatedDate as Date),cast(AddedDate as date)
order by cast(UpdatedDate as Date)", projectId);
            var v = _dbEntities.Database.SqlQuery<FobPriceUpdateLog>(query).ToList();
            return v;
        }
    }
    public class FobPriceUpdateLog
    {
        public string FinalPrice { get; set; }
        public string UpdatedDate { get; set; }
    }
}