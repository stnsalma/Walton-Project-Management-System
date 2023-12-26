using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using AutoMapper;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class IqcRepository : IIqcRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;
        private readonly MRPEntities _mrpEntities;

        public IqcRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _mrpEntities = new MRPEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
        }

        public List<ProjectOrderQuantityDetailModel> GetVariantsByProjectId(long id)
        {
            var model = (from v in _dbeEntities.ProjectOrderQuantityDetails
                         where v.ProjectMasterId == id
                         select new ProjectOrderQuantityDetailModel
                         {
                             Id = v.Id,
                             ProjectMasterId = v.ProjectMasterId,
                             ProjectModel = v.ProjectModel,
                             OrderQuantity = v.OrderQuantity,
                             OrderNumber = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == v.ProjectMasterId).Select(x => x.OrderNuber).FirstOrDefault()
                         }).ToList();
            foreach (var m in model)
            {
                var ordinal = "th";
                switch (m.OrderNumber)
                {
                    case 1:
                        ordinal = "st";
                        break;
                    case 2:
                        ordinal = "nd";
                        break;
                    case 3:
                        ordinal = "rd";
                        break;
                }
                m.ProjectModel = m.ProjectModel + " (" + m.OrderNumber + ordinal + " order)";
            }
            return model;
        }

        public ProjectOrderQuantityDetailModel GetVariantById(long id)
        {
            var model = (from v in _dbeEntities.ProjectOrderQuantityDetails
                         where v.Id == id
                         select new ProjectOrderQuantityDetailModel
                         {
                             Id = v.Id,
                             ProjectMasterId = v.ProjectMasterId,
                             ProjectPurchaseOrderFormId = _dbeEntities.ProjectPurchaseOrderForms.Where(x => x.ProjectMasterId == v.ProjectMasterId).Select(x => x.ProjectPurchaseOrderFormId).FirstOrDefault(),
                             ProjectName = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == v.ProjectMasterId).Select(x => x.ProjectName).FirstOrDefault(),
                             ProjectModel = v.ProjectModel,
                             OrderQuantity = v.OrderQuantity,
                             OrderNumber = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == v.ProjectMasterId).Select(x => x.OrderNuber).FirstOrDefault()
                         }).FirstOrDefault();
            return model;
        }

        public List<ProjectOrderQuantityDetailModel> GetAllVariants()
        {
            var model = (from v in _dbeEntities.ProjectOrderQuantityDetails
                         select new ProjectOrderQuantityDetailModel
                         {
                             Id = v.Id,
                             ProjectMasterId = v.ProjectMasterId,
                             ProjectModel = v.ProjectModel,
                             OrderQuantity = v.OrderQuantity,
                             OrderNumber = _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == v.ProjectMasterId).Select(x => x.OrderNuber).FirstOrDefault(),
                             RamVendor = v.RamVendor,
                             RomVendor = v.RomVendor,
                             AddedBy = v.AddedBy,
                             AddedByName = _dbeEntities.CmnUsers.Where(x=>x.CmnUserId==v.AddedBy).Select(x=>x.UserFullName).FirstOrDefault(),
                             AddedDate = v.AddedDate,
                             BTRCPush = v.BTRCPush,
                             IsActive = v.IsActive,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == v.UpdatedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             UpdatedDate = v.UpdatedDate,
                             VariantClosingBy = v.VariantClosingBy,
                             VariantClosingByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == v.VariantClosingBy).Select(x => x.UserFullName).FirstOrDefault(),
                             VariantClosingDate = v.VariantClosingDate,
                             ClosingRemarks = v.ClosingRemarks
                         }).ToList();
            foreach (var m in model)
            {
                var ordinal = "th";
                switch (m.OrderNumber)
                {
                    case 1:
                        ordinal = "st";
                        break;
                    case 2:
                        ordinal = "nd";
                        break;
                    case 3:
                        ordinal = "rd";
                        break;
                }
                m.ProjectModel = m.ProjectModel + " (" + m.OrderNumber + ordinal + " order)";
            }
            return model;
        }

        public List<ForeignIqcModel> GetForeignIqcModels()
        {
            var model = (from v in _dbeEntities.ForeignIqcs
                         select new ForeignIqcModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             VariantId = v.VariantId,
                             LotNo = v.LotNo,
                             LotQuantity = v.LotQuantity,
                             NoOfInspectionTime = v.NoOfInspectionTime,
                             AllMaterialPassed = v.AllMaterialPassed,
                             ManagementApproved = v.ManagementApproved,
                             ManagementApproveDate = v.ManagementApproveDate,
                             SourcingApproved = v.SourcingApproved,
                             SupportingDoc = v.SupportingDoc,
                             Remarks = v.Remarks,
                             IqcStartDate = v.IqcStartDate,
                             WarehouseReceiveDate = v.WarehouseReceiveDate,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate,
                             ShipmentNo = v.ShipmentNo,
                             OrderNumber =
                                 _dbeEntities.ProjectMasters.Where(x => x.ProjectMasterId == v.ProjectId)
                                     .Select(x => x.OrderNuber)
                                     .FirstOrDefault(),
                                     ProjectModel = _dbeEntities.ProjectOrderQuantityDetails.Where(x=>x.Id==v.VariantId).Select(x=>x.ProjectModel).FirstOrDefault()
                         }).ToList();
            foreach (var m in model)
            {
                var ordinal = "th";
                switch (m.OrderNumber)
                {
                    case 1:
                        ordinal = "st";
                        break;
                    case 2:
                        ordinal = "nd";
                        break;
                    case 3:
                        ordinal = "rd";
                        break;
                }
                m.ProjectModel = m.ProjectModel + " (" + m.OrderNumber + ordinal + " order)";
            }
            return model;
        }

        public List<BomModel> GetBomByProjectModel(string projectName)
        {
            var model = (from b in _mrpEntities.BOMs
                         join p in _mrpEntities.BomProductModels on b.BomProductModelId equals p.Id
                         where p.Model == projectName
                         select new BomModel
                         {
                             Id = b.Id,
                             BomProductModelId = b.BomProductModelId,
                             InventoryItemId = b.InventoryItemId,
                             InventoryItemCode = b.InventoryItemCode,
                             ItemName = b.ItemName,
                             Description = b.Description,
                             Company = b.Company,
                             Component = b.Component,
                             RequiredPerUnit = b.RequiredPerUnit,
                             SpareItemCode = b.SpareItemCode,
                             SpareDescription = b.SpareDescription,
                             ItemType = b.ItemType,
                             AssemblyCode = b.AssemblyCode,
                             Uom = b.Uom,
                             ProductType = b.ProductType,
                             Color = b.Color,
                             BOMType = b.BOMType,
                             AddedBy = b.AddedBy,
                             AddedDate = b.AddedDate
                         }).ToList();
            return model;
        }

        public List<BdIqcBomPassRecordModel> GetBdIqcBomPassRecordByVariantId(long id)
        {
            var model = (from v in _dbeEntities.BdIqcBomPassRecords
                         where v.VariantId == id
                         select new BdIqcBomPassRecordModel
                         {
                             Id = v.Id,
                             BomId = v.BomId,
                             VariantId = v.VariantId,
                             ProjectId = v.ProjectId,
                             Description = v.Description,
                             SpareDescription = v.SpareDescription,
                             BOMType = v.BOMType,
                             BomQuantity = v.BomQuantity,
                             BomPassedQuantity = v.BomPassedQuantity,
                             BomFailQuantity = v.BomFailQuantity,
                             Remarks = v.Remarks,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).ToList();
            return model;
        }

        public List<ForeignIqcBomPassRecordModel> GetForeignIqcBomPassRecordByVariantId(long id)
        {
            var model = (from v in _dbeEntities.ForeignIqcBomPassRecords
                         where v.VariantId == id
                         select new ForeignIqcBomPassRecordModel
                         {
                             Id = v.Id,
                             ForeignIqcId = v.ForeignIqcId,
                             BomId = v.BomId,
                             VariantId = v.VariantId,
                             ProjectId = v.ProjectId,
                             Description = v.Description,
                             SpareDescription = v.SpareDescription,
                             BOMType = v.BOMType,
                             BomQuantity = v.BomQuantity,
                             BomPassedQuantity = v.BomPassedQuantity,
                             BomFailQuantity = v.BomFailQuantity,
                             Remarks = v.Remarks,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).ToList();
            return model;
        }

        public List<ForeignIqcBomPassRecordModel> GetForeignIqcBomPassRecordByForIqcId(long id)
        {
            var model = (from v in _dbeEntities.ForeignIqcBomPassRecords
                         where v.ForeignIqcId == id
                         select new ForeignIqcBomPassRecordModel
                         {
                             Id = v.Id,
                             ForeignIqcId = v.ForeignIqcId,
                             BomId = v.BomId,
                             VariantId = v.VariantId,
                             ProjectId = v.ProjectId,
                             Description = v.Description,
                             SpareDescription = v.SpareDescription,
                             BOMType = v.BOMType,
                             BomQuantity = v.BomQuantity,
                             BomPassedQuantity = v.BomPassedQuantity,
                             BomFailQuantity = v.BomFailQuantity,
                             Remarks = v.Remarks,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).ToList();
            return model;
        }

        public BdIqcModel GetBdIqcByVariantId(long id)
        {
            var model = (from v in _dbeEntities.BdIqcs
                         where v.VariantId == id
                         select new BdIqcModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             VariantId = v.VariantId,
                             LotNo = v.LotNo,
                             LotQuantity = v.LotQuantity,
                             NoOfInspectionTime = v.NoOfInspectionTime,
                             AllMaterialPassed = v.AllMaterialPassed,
                             ManagementApproved = v.ManagementApproved,
                             ManagementApproveDate = v.ManagementApproveDate,
                             SourcingApproved = v.SourcingApproved,
                             SupportingDoc = v.SupportingDoc,
                             Remarks = v.Remarks,
                             IqcStartDate = v.IqcStartDate,
                             WarehouseReceiveDate = v.WarehouseReceiveDate,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).FirstOrDefault();
            return model;
        }

        public ForeignIqcModel GetForeignIqcByVariantId(long id)
        {
            var model = (from v in _dbeEntities.ForeignIqcs
                         where v.VariantId == id
                         select new ForeignIqcModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             VariantId = v.VariantId,
                             LotNo = v.LotNo,
                             LotQuantity = v.LotQuantity,
                             NoOfInspectionTime = v.NoOfInspectionTime,
                             AllMaterialPassed = v.AllMaterialPassed,
                             ManagementApproved = v.ManagementApproved,
                             ManagementApproveDate = v.ManagementApproveDate,
                             SourcingApproved = v.SourcingApproved,
                             SupportingDoc = v.SupportingDoc,
                             Remarks = v.Remarks,
                             IqcStartDate = v.IqcStartDate,
                             WarehouseReceiveDate = v.WarehouseReceiveDate,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate,
                             ShipmentNo = v.ShipmentNo
                         }).FirstOrDefault();
            return model;
        }

        public ForeignIqcModel GetForIqcByVariantIdAndInspNo(long? variantid, string insno)
        {
            var model = (from v in _dbeEntities.ForeignIqcs
                         where v.VariantId == variantid && v.NoOfInspectionTime == insno
                         select new ForeignIqcModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             VariantId = v.VariantId,
                             LotNo = v.LotNo,
                             LotQuantity = v.LotQuantity,
                             NoOfInspectionTime = v.NoOfInspectionTime,
                             AllMaterialPassed = v.AllMaterialPassed,
                             ManagementApproved = v.ManagementApproved,
                             ManagementApproveDate = v.ManagementApproveDate,
                             SourcingApproved = v.SourcingApproved,
                             SupportingDoc = v.SupportingDoc,
                             Remarks = v.Remarks,
                             IqcStartDate = v.IqcStartDate,
                             WarehouseReceiveDate = v.WarehouseReceiveDate,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate,
                             ShipmentNo = v.ShipmentNo
                         }).FirstOrDefault();
            return model;
        }

        public BdIqcModel SaveBdIqc(BdIqcModel model)
        {
            Mapper.CreateMap<BdIqcModel, BdIqc>();
            var v = Mapper.Map<BdIqc>(model);
            _dbeEntities.BdIqcs.AddOrUpdate(v);
            _dbeEntities.SaveChanges();
            model.Id = v.Id;
            return model;
        }

        public ForeignIqcModel SaveForeignIqc(ForeignIqcModel model)
        {
            Mapper.CreateMap<ForeignIqcModel, ForeignIqc>();
            var v = Mapper.Map<ForeignIqc>(model);
            _dbeEntities.ForeignIqcs.AddOrUpdate(v);
            _dbeEntities.SaveChanges();
            model.Id = v.Id;
            return model;
        }

        public void SaveBdIqcBomPassRecords(List<BdIqcBomPassRecordModel> model)
        {
            foreach (var m in model)
            {
                Mapper.CreateMap<BdIqcBomPassRecordModel, BdIqcBomPassRecord>();
                var v = Mapper.Map<BdIqcBomPassRecord>(m);
                _dbeEntities.BdIqcBomPassRecords.AddOrUpdate(v);
                _dbeEntities.SaveChanges();
            }
        }

        public void SaveForeignIqcBomPassRecords(List<ForeignIqcBomPassRecordModel> model)
        {
            foreach (var m in model)
            {
                Mapper.CreateMap<ForeignIqcBomPassRecordModel, ForeignIqcBomPassRecord>();
                var v = Mapper.Map<ForeignIqcBomPassRecord>(m);
                _dbeEntities.ForeignIqcBomPassRecords.AddOrUpdate(v);
                _dbeEntities.SaveChanges();
            }
        }
    }
}