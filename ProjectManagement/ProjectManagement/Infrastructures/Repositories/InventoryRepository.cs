using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using AutoMapper;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class InventoryRepository:IInventoryRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;

        public InventoryRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
        }

        public string SaveFocClaimBomDetailModel(string receiveQuantity, string receiveRemarks, long id)
        {
            long userId;
            long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
            var model =
                _dbeEntities.FocClaimBomDetails.Where(x => x.FocClaimId == id).Select(x => new FocClaimBomDetailModel
                {
                    FocClaimId = x.FocClaimId,
                    RawMaterialId = x.RawMaterialId,
                    ProjectMasterId = x.ProjectMasterId,
                    ProjectPurchaseOrderFormId = x.ProjectPurchaseOrderFormId,
                    ProjectName = x.ProjectName,
                    ProjectType = x.ProjectType,
                    Orders=x.Orders,
                    PoCategory = x.PoCategory,
                    PoQuantity = x.PoQuantity,
                    LotNumber = x.LotNumber,
                    LotQuantity = x.LotQuantity,
                    BOMType = x.BOMType,
                    BOMName = x.BOMName,
                    Color = x.Color,
                    ItemQuantity = x.ItemQuantity,
                    BomRemarks = x.BomRemarks,
                    Added = x.Added,
                    AddedDate = x.AddedDate
                }).FirstOrDefault();
            if (model != null)
            {
                model.ReceiveQuantity = receiveQuantity;
                model.ReceiveRemarks = receiveRemarks;
                model.Updated = userId;
                model.UpdatedDate = DateTime.Now;

                Mapper.CreateMap<FocClaimBomDetailModel, FocClaimBomDetail>();
                var v = Mapper.Map<FocClaimBomDetail>(model);
                _dbeEntities.FocClaimBomDetails.AddOrUpdate(v);
                _dbeEntities.SaveChanges();
                return "success";
            }
            return "failed";
        }
    }
}