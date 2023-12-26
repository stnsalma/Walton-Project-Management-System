using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class MaterialWastageRepository : IMaterialWastageRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public MaterialWastageRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }
        public bool GetMaterialWastageReportByMonthAndYear(int monthNumber, int yearNumber)
        {
            var isExist = _dbEntities.MaterialWastageMasters.Any(i => i.MonthNumber == monthNumber && i.YearNumber == yearNumber);
            return isExist;

        }

        public MaterialWastageReportTopSheetViewModel GetMaterailWastageTopSheet(long id)
        {
            var materialMaster = _dbEntities.MaterialWastageMasters.FirstOrDefault(i => i.Id == id);
            if (materialMaster == null)
            {
                return null;
            }
            var model = new MaterialWastageReportTopSheetViewModel
            {
                CompanyName = "Walton Digi- Tech Industries (Mobile)",
                UnitName = "Cell Phone Manufacturing Unit",
                Address = "H#00013, Block- B, Building- 03, (2nd & 3rd Floor)",
                Adderes2 = "Ward- 02, Boroichuti, Kaliakoir, Gazipur",
                MonthName = "Summary of "+materialMaster.MonthName+" "+ materialMaster.YearNumber +" Wastage Value (Project Wise)"
            };
            List<WastageParticular> particulars = (from materialWastageDetail in _dbEntities.MaterialWastageDetails
                                                   where materialWastageDetail.MaterialWastageMasterId == id
                                                   group materialWastageDetail by materialWastageDetail.BOMType into g
                                                   select new WastageParticular
                                                   {
                                                       Particular = g.Key,
                                                       PriceValue = g.Sum(i => i.TotalPrice)
                                                   }).ToList();
            model.Particulars = particulars;

            WastageParticular particular = new WastageParticular
            {
                Particular = "Total", PriceValue = particulars.Sum(i=>i.PriceValue)
            };

            model.Particulars.Add(particular);
            var recommendations = _dbEntities.MaterialWastageRecommendations.Where(i => i.MaterialWastageMasterId == id && i.RecommendationType=="APPROVED").ToList();


            model.CreatorList = (from master in _dbEntities.MaterialWastageMasters
                                 join user in _dbEntities.CmnUsers on master.AddedBy equals user.CmnUserId
                                 where master.Id==id
                                 select user.UserFullName).ToList();
            model.InChargeList = recommendations.Where(i => i.UserType == "INCHARGE").Select(i=>i.RecommendedBy).ToList();
            model.CooList = recommendations.Where(i => i.UserType == "COO").Select(i=>i.RecommendedBy).ToList();
            model.ApprovalList = recommendations.Where(i => i.UserType == "MANAGEMENT").Select(i=>i.RecommendedBy).ToList();
            model.ApprovalList = recommendations.Where(i => i.UserType == "MANAGEMENT").Select(i=>i.RecommendedBy).ToList();
            model.DeputyCooList = recommendations.Where(i => i.UserType == "DCOO").Select(i => i.RecommendedBy).ToList();
            return model;
        }
    }
}