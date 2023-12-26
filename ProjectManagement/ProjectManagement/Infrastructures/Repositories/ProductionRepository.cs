using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using System.Data.OleDb;
using System.Configuration;
using ProjectManagement.ViewModels.Commercial;
using ProjectManagement.ViewModels.Production;
using Excel = Microsoft.Office.Interop.Excel;


namespace ProjectManagement.Infrastructures.Repositories
{
    public class ProductionRepository : IProductionRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public ProductionRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }
        public List<ProductionTrackerModel> GetProductionTrackerModels()
        {
            string query = string.Format(@"select * from ProductionTrackers");
            var exe = _dbEntities.Database.SqlQuery<ProductionTrackerModel>(query).ToList();
            return exe;
        }
        public string ImportDataFromExcel2(string filepath)
        {
            try
            {
                var productions = new List<ProductionTrackerModel>();
                var xlApp = new Excel.Application();
                Excel.Workbook xWorkbook = xlApp.Workbooks.Open(filepath);
                Excel._Worksheet xSheet = xWorkbook.Sheets[1];
                Excel.Range xRange = xSheet.UsedRange;
                int countRow = xRange.Rows.Count;
                int countColumn = xRange.Columns.Count;

                for (int i = 2; i <= countRow; i++)
                {
                    string modelName = (xRange.Cells[i, 1].Value2).ToString();
                    string imei1 = (xRange.Cells[i, 2].Value2).ToString();
                    string imei2 = (xRange.Cells[i, 3].Value2).ToString();
                    string color = (xRange.Cells[i, 4].Value2).ToString();
                    string orderNo = (xRange.Cells[i, 5].Value2).ToString();
                    bool isTrial = Convert.ToBoolean(xRange.Cells[i, 6].Value2);
                    double productionDate = (xRange.Cells[i, 7].Value2);
                    DateTime parseProductionDate = DateTime.FromOADate(productionDate);
                    string softwareversion = (xRange.Cells[i, 8].Value2).ToString();
                    long addedBy;
                    long.TryParse(HttpContext.Current.User.Identity.Name, out addedBy);

                    if (productions.FindIndex(x => x.IMEI1 == imei1 || x.IMEI2 == imei2) == -1)
                    {
                        var production = new ProductionTrackerModel
                        {
                            ModelName = modelName,
                            IMEI1 = imei1,
                            IMEI2 = imei2,
                            Color = color,
                            OrderNo = orderNo,
                            IsTrial = isTrial,
                            ProductionDate = parseProductionDate,
                            SoftwareVersion = softwareversion,
                            AddedBy = addedBy,
                            AddedDate = DateTime.Now
                        };
                        productions.Add(production);
                    }

                    ////release com objects to fully kill excel process from running in the background
                    //Marshal.ReleaseComObject(xRange);
                    //Marshal.ReleaseComObject(xSheet);

                    ////close and release
                    //xWorkbook.Close();
                    //Marshal.ReleaseComObject(xWorkbook);

                    ////quit and release
                    //xlApp.Quit();
                    //Marshal.ReleaseComObject(xlApp);


                    List<ProductionTracker> dbProductionTrackers = _dbEntities.ProductionTrackers.ToList();

                    foreach (var production in productions)
                    {
                        int index = dbProductionTrackers.FindIndex(x => x.IMEI1 == production.IMEI1 || x.IMEI2 == production.IMEI2);
                        if (index == -1)
                        {
                            var dbProductionTracker = new ProductionTracker
                            {
                                ModelName = production.ModelName,
                                IMEI1 = production.IMEI1,
                                IMEI2 = production.IMEI2,
                                Color = production.Color,
                                OrderNo = production.OrderNo,
                                IsTrial = production.IsTrial,
                                ProductionDate = production.ProductionDate,
                                SoftwareVersion = production.SoftwareVersion,
                                AddedBy = production.AddedBy,
                                AddedDate = production.AddedDate
                            };
                            _dbEntities.ProductionTrackers.Add(dbProductionTracker);
                        }
                    }
                    _dbEntities.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "success";
        }
        public void ImportDataFromExcel(string filepath)
        {
            string ssqltable = "ProductionTrackers";
            string myexceldataquery = "select ModelName,IMEI1,IMEI2,Color,OrderNo,IsTrial,ProductionDate,SoftwareVersion from [Sheet1$]";
            var fileManager = new FileManager();
            var ext = fileManager.GetExtension(filepath);
            try
            {
                if (ext == ".xls" || ext == ".xlsx")
                {
                    string excelconnectionString = "";
                    if (ext == ".xls")
                    {
                        //For Excel 97-03
                        excelconnectionString =
                            "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
                    }
                    else if (ext == ".xlsx")
                    {
                        //For Excel 07 and greater
                        excelconnectionString =
                            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
                    }
                    excelconnectionString = String.Format(excelconnectionString, filepath, "yes");
                    string ssqlconnectionstring = ConfigurationManager.ConnectionStrings["ExcelImportConnectionString"].ConnectionString;
                    OleDbConnection conn = new OleDbConnection(excelconnectionString);
                    OleDbCommand cmd = new OleDbCommand(myexceldataquery, conn);
                    conn.Open();
                    //OleDbDataReader dr = cmd.ExecuteReader();
                    DataSet ds = new DataSet();
                    OleDbDataAdapter oda = new OleDbDataAdapter(myexceldataquery, conn);
                    conn.Close();
                    oda.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    dt.Columns.Add("AddedBy", typeof(Int64));
                    dt.Columns.Add("AddedDate", typeof(DateTime));
                    foreach (DataRow row in dt.Rows)
                    {
                        row["AddedBy"] = HttpContext.Current.User.Identity.Name;
                        row["AddedDate"] = DateTime.Now;
                    }
                    SqlBulkCopy bulkcopy = new SqlBulkCopy(ssqlconnectionstring);
                    bulkcopy.DestinationTableName = ssqltable;
                    bulkcopy.ColumnMappings.Add("ModelName", "ModelName");
                    bulkcopy.ColumnMappings.Add("IMEI1", "IMEI1");
                    bulkcopy.ColumnMappings.Add("IMEI2", "IMEI2");
                    bulkcopy.ColumnMappings.Add("Color", "Color");
                    bulkcopy.ColumnMappings.Add("OrderNo", "OrderNo");
                    bulkcopy.ColumnMappings.Add("IsTrial", "IsTrial");
                    bulkcopy.ColumnMappings.Add("ProductionDate", "ProductionDate");
                    bulkcopy.ColumnMappings.Add("SoftwareVersion", "SoftwareVersion");
                    bulkcopy.ColumnMappings.Add("AddedBy", "AddedBy");
                    bulkcopy.ColumnMappings.Add("AddedDate", "AddedDate");
                    conn.Open();
                    bulkcopy.WriteToServer(dt);
                    conn.Close();
                    string deleteDuplicateByImei1 = string.Format(
                            "delete from ProductionTrackers where productionid NOT IN (select MIN(productionid) FROM ProductionTrackers group by IMEI1)");
                    string deleteDuplicateByImei2 = string.Format(
                            "delete from ProductionTrackers where productionid NOT IN (select MIN(productionid) FROM ProductionTrackers group by IMEI2)");
                    _dbEntities.Database.ExecuteSqlCommand(deleteDuplicateByImei1);
                    _dbEntities.Database.ExecuteSqlCommand(deleteDuplicateByImei2);
                    //return  "File imported into sql server successfully.";   
                }
            }
            catch (Exception ex)
            {

            }

        }

        #region Production Event
        public List<VmAssemblyPackingProduction> GetProductionProject()
        {
            string proEv = string.Format(@"select ppo.*,pm.ProjectMasterId, pm.ProjectName, pm.OrderNuber as OrderNumber from CellPhoneProject.dbo.ProjectMasters pm
left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
where pm.ProjectMasterId=ppo.ProjectMasterId and ppo.PoCategory in ('SKD','CKD')");


            var proEvent = _dbEntities.Database.SqlQuery<VmAssemblyPackingProduction>(proEv).ToList();

            return proEvent;
        }
        public List<ProjectMasterModel> GetProductionProjectList()
        {
            string proEv = string.Format(@"select pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber,pm.SourcingType,ppo.PoCategory from CellPhoneProject.dbo.ProjectMasters pm
            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            where pm.ProjectMasterId=ppo.ProjectMasterId and ppo.PoCategory in ('SKD','CKD')");


            var proEvent = _dbEntities.Database.SqlQuery<ProjectMasterModel>(proEv).ToList();

            foreach (var model in proEvent)
            {
                var ext = !string.IsNullOrWhiteSpace(model.SourcingType) ? " / " + model.SourcingType : "";
                if (model.OrderNuber != null)
                {
                    model.ProjectName = model.ProjectName + " (" + CommonConversion.AddOrdinal((int)model.OrderNuber) +
                                        " Order)" + ext;
                }

            }

            return proEvent;
        }
        public List<ProjectMasterModel> GetProjectOrders(long proIds)
        {
            string proEv = string.Format(@"select pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber,ppo.PoCategory from CellPhoneProject.dbo.ProjectMasters pm
            left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            where pm.ProjectMasterId=ppo.ProjectMasterId and ppo.PoCategory in ('SKD','CKD') and pm.ProjectMasterId='{0}' ", proIds);


            var proEvent = _dbEntities.Database.SqlQuery<ProjectMasterModel>(proEv).ToList();

            return proEvent;
        }
        public ProjectMasterModel GetProjectPoCategory(long proIds)
        {
            var projectPoCat = new ProjectMasterModel();
            var projectInfo = (from pm in _dbEntities.ProjectMasters
                               join ppo in _dbEntities.ProjectPurchaseOrderForms on pm.ProjectMasterId equals
                                   ppo.ProjectMasterId

                               where (ppo.PoCategory == "SKD" || ppo.PoCategory == "CKD") && pm.ProjectMasterId == proIds
                               select new
                               {
                                   ppo.PoCategory,
                                   pm.ProjectName,
                                   pm.ProjectMasterId,
                                   pm.OrderNuber

                               }).FirstOrDefault();

            if (projectInfo != null)
            {
                projectPoCat.PoCategory = projectInfo.PoCategory;
                projectPoCat.ProjectMasterId = projectInfo.ProjectMasterId;
                projectPoCat.OrderNuber = projectInfo.OrderNuber;
                projectPoCat.ProjectName = projectInfo.ProjectName;
            }

            return projectPoCat;
        }
        public string AddedProjectPartialSaves(List<CustomPrdAssemblyAndPackingDetails> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var model1 = new AssemblyProductionEvent();
                model1.ProjectId = insResult.ProjectId;
                model1.ProjectName = insResult.ProjectName;
                model1.OrderNumber = Convert.ToInt32(insResult.OrderNumber);
                model1.PoCategory = insResult.PoCategory;
                model1.MaterialReceiveDate = insResult.MaterialReceiveDate;
                model1.IqcCompleteDate = insResult.IqcCompleteDate;
                model1.TrialProductionDate = insResult.TrialProductionDate;
                model1.SoftwareConfirmationDate = insResult.SoftwareConfirmationDate;
                model1.RnDClearanceDate = insResult.RnDClearanceDate;
                model1.AssemblyLineInformation = insResult.AssemblyLineInformation;
                model1.AssemblyProductionStartDate = insResult.AssemblyProductionStartDate;
                model1.AssemblyQuantity = Convert.ToInt64(insResult.AssemblyQuantity);
                model1.AssemblyPerDayCapacity = Convert.ToInt64(insResult.AssemblyPerDayCapacity);
                model1.AssemblyProductionEndDate = insResult.AssemblyProductionEndDate;
                model1.Status = "PARTIAL";
                model1.Added = userId;
                model1.AddedDate = DateTime.Now;
                _dbEntities.AssemblyProductionEvents.AddOrUpdate(model1);

                var model2 = new PackingProductionEvent();
                model2.ProjectId = insResult.ProjectId;
                model2.ProjectName = insResult.ProjectName;
                model2.OrderNumber = Convert.ToInt32(insResult.OrderNumber);
                model2.PoCategory = insResult.PoCategory;
                model2.MaterialReceiveDate = insResult.MaterialReceiveDate;
                model2.IqcCompleteDate = insResult.IqcCompleteDate;
                model2.TrialProductionDate = insResult.TrialProductionDate;
                model2.SoftwareConfirmationDate = insResult.SoftwareConfirmationDate;
                model2.RnDClearanceDate = insResult.RnDClearanceDate;
                model2.PackingLineInformation = insResult.PackingLineInformation;
                model2.PackingProductionStartDate = insResult.PackingProductionStartDate;
                model2.PackingQuantity = Convert.ToInt64(insResult.PackingQuantity);
                model2.PackingPerDayCapacity = Convert.ToInt64(insResult.PackingPerDayCapacity);
                model2.PackingProductionEndDate = insResult.PackingProductionEndDate;
                model2.Status = "PARTIAL";
                model2.Added = userId;
                model2.AddedDate = DateTime.Now;

                _dbEntities.PackingProductionEvents.AddOrUpdate(model2);

            }
            _dbEntities.SaveChanges();
            return "ok";
        }
        public List<CustomPrdAssemblyAndPackingDetails> GetAssemblyAndPackingSavedProject()
        {
            string query = string.Format(@"select ase.ProjectId,ase.ProjectName,CAST(ase.OrderNumber AS VARCHAR(10) ) as OrderNumber,ase.PoCategory,ase.MaterialReceiveDate,ase.IqcCompleteDate,ase.TrialProductionDate,ase.SoftwareConfirmationDate,ase.RnDClearanceDate,
            ase.AssemblyLineInformation,ase.AssemblyProductionStartDate,CAST(ase.AssemblyQuantity AS VARCHAR(10)) as AssemblyQuantity,CAST(ase.AssemblyPerDayCapacity AS VARCHAR(10)) as AssemblyPerDayCapacity,ase.AssemblyProductionEndDate,
            ppe.PackingLineInformation,ppe.PackingProductionStartDate,CAST(ppe.PackingQuantity AS VARCHAR(10)) as PackingQuantity,CAST(ppe.PackingPerDayCapacity AS VARCHAR(10)) as PackingPerDayCapacity,ppe.PackingProductionEndDate
            from CellPhoneProject.dbo.AssemblyProductionEvent ase 
            left join CellPhoneProject.dbo.PackingProductionEvent ppe on ase.ProjectId=ppe.ProjectId
            where ase.ProjectId=ppe.ProjectId and ase.OrderNumber=ppe.OrderNumber and ase.Status='PARTIAL' and ppe.Status='PARTIAL'  ");
            var exe = _dbEntities.Database.SqlQuery<CustomPrdAssemblyAndPackingDetails>(query).ToList();
            return exe;
        }
        public List<CustomPrdAssemblyAndPackingDetails> GetAssemblyAndPackingCompletedProject()
        {
            string query = string.Format(@"select ase.ProjectId,ase.ProjectName,CAST(ase.OrderNumber AS VARCHAR(10) ) as OrderNumber,ase.PoCategory,ase.MaterialReceiveDate,ase.IqcCompleteDate,ase.TrialProductionDate,ase.SoftwareConfirmationDate,ase.RnDClearanceDate,
            ase.AssemblyLineInformation,ase.AssemblyProductionStartDate,CAST(ase.AssemblyQuantity AS VARCHAR(10)) as AssemblyQuantity,CAST(ase.AssemblyPerDayCapacity AS VARCHAR(10)) as AssemblyPerDayCapacity,ase.AssemblyProductionEndDate,
            ppe.PackingLineInformation,ppe.PackingProductionStartDate,CAST(ppe.PackingQuantity AS VARCHAR(10)) as PackingQuantity,CAST(ppe.PackingPerDayCapacity AS VARCHAR(10)) as PackingPerDayCapacity,ppe.PackingProductionEndDate
            from CellPhoneProject.dbo.AssemblyProductionEvent ase 
            left join CellPhoneProject.dbo.PackingProductionEvent ppe on ase.ProjectId=ppe.ProjectId
            where ase.ProjectId=ppe.ProjectId and ase.OrderNumber=ppe.OrderNumber and ase.Status='COMPLETED' and ppe.Status='COMPLETED'  ");
            var exe = _dbEntities.Database.SqlQuery<CustomPrdAssemblyAndPackingDetails>(query).ToList();
            return exe;
        }
        public string UpdateAssemblyAndPackingTables(CustomPrdAssemblyAndPackingDetails assembAndPack)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updatedAssembly = (from c in _dbEntities.AssemblyProductionEvents
                                   where c.ProjectId == assembAndPack.ProjectId
                                   select c).FirstOrDefault();

            updatedAssembly.MaterialReceiveDate = assembAndPack.MaterialReceiveDate;
            updatedAssembly.IqcCompleteDate = assembAndPack.IqcCompleteDate;
            updatedAssembly.TrialProductionDate = assembAndPack.TrialProductionDate;
            updatedAssembly.SoftwareConfirmationDate = assembAndPack.SoftwareConfirmationDate;
            updatedAssembly.RnDClearanceDate = assembAndPack.RnDClearanceDate;
            updatedAssembly.AssemblyLineInformation = assembAndPack.AssemblyLineInformation;
            updatedAssembly.AssemblyProductionStartDate = assembAndPack.AssemblyProductionStartDate;
            updatedAssembly.AssemblyQuantity = Convert.ToInt64(assembAndPack.AssemblyQuantity);
            updatedAssembly.AssemblyPerDayCapacity = Convert.ToInt64(assembAndPack.AssemblyPerDayCapacity);
            updatedAssembly.AssemblyProductionEndDate = assembAndPack.AssemblyProductionEndDate;
            updatedAssembly.Updated = userId;
            updatedAssembly.UpdatedDate = DateTime.Now;

            _dbEntities.SaveChanges();

            var updatedPacking = (from c in _dbEntities.PackingProductionEvents
                                  where c.ProjectId == assembAndPack.ProjectId
                                  select c).FirstOrDefault();

            updatedPacking.MaterialReceiveDate = assembAndPack.MaterialReceiveDate;
            updatedPacking.IqcCompleteDate = assembAndPack.IqcCompleteDate;
            updatedPacking.TrialProductionDate = assembAndPack.TrialProductionDate;
            updatedPacking.SoftwareConfirmationDate = assembAndPack.SoftwareConfirmationDate;
            updatedPacking.RnDClearanceDate = assembAndPack.RnDClearanceDate;
            updatedPacking.PackingLineInformation = assembAndPack.PackingLineInformation;
            updatedPacking.PackingProductionStartDate = assembAndPack.PackingProductionStartDate;
            updatedPacking.PackingQuantity = Convert.ToInt64(assembAndPack.PackingQuantity);
            updatedPacking.PackingPerDayCapacity = Convert.ToInt64(assembAndPack.PackingPerDayCapacity);
            updatedPacking.PackingProductionEndDate = assembAndPack.PackingProductionEndDate;
            updatedPacking.Updated = userId;
            updatedPacking.UpdatedDate = DateTime.Now;

            _dbEntities.SaveChanges();


            return "ok";

        }
        public string UpdateAssemblyAndPackingTableStatuses(CustomPrdAssemblyAndPackingDetails assembAndPack)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updatedAssembly = (from c in _dbEntities.AssemblyProductionEvents
                                   where c.ProjectId == assembAndPack.ProjectId
                                   select c).FirstOrDefault();

            updatedAssembly.Status = assembAndPack.Status;
            updatedAssembly.Updated = userId;
            updatedAssembly.UpdatedDate = DateTime.Now;

            _dbEntities.SaveChanges();

            var updatedPacking = (from c in _dbEntities.PackingProductionEvents
                                  where c.ProjectId == assembAndPack.ProjectId
                                  select c).FirstOrDefault();

            updatedPacking.Status = assembAndPack.Status;
            updatedPacking.Updated = userId;
            updatedPacking.UpdatedDate = DateTime.Now;

            _dbEntities.SaveChanges();


            return "ok";

        }
        public string InsertProductionRemarks(CustomPrdAssemblyAndPackingDetails productionRemarksData)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            var updateData = (from c in _dbEntities.ProductionPlanRemarks
                              where c.ProductionDate == productionRemarksData.ProductionRemarksDate && c.IsCkd == true
                              select c).FirstOrDefault();

            if (updateData != null && updateData.ProductionDate == productionRemarksData.ProductionRemarksDate)
            {
                if (productionRemarksData.Remarks != null && productionRemarksData.Remarks != updateData.Remarks)
                {
                    //////update ProductionPlanRemark/////////////////
                    updateData.Remarks = productionRemarksData.Remarks;
                    updateData.Updated = userId;
                    updateData.UpdatedDate = DateTime.Now;
                    updateData.Added = userId;
                    updateData.AddedDate = DateTime.Now;
                    updateData.IsCkd = productionRemarksData.IsCkd;
                    updateData.IsCharger = productionRemarksData.IsCharger;
                    /////////////insert ProductionPlanRemarkLog/////////
                    var model1 = new ProductionPlanRemarksLog();
                    model1.Remarks = productionRemarksData.Remarks;
                    model1.ProductionDate = productionRemarksData.ProductionRemarksDate;
                    model1.Added = userId;
                    model1.AddedDate = DateTime.Now;
                    model1.IsCkd = productionRemarksData.IsCkd;
                    model1.IsCharger = productionRemarksData.IsCharger;
                    _dbEntities.ProductionPlanRemarksLogs.AddOrUpdate(model1);

                    _dbEntities.SaveChanges();
                }
            }
            else
            {
                var model = new ProductionPlanRemark();

                if (productionRemarksData.Remarks != null)
                {
                    /////////////insert ProductionPlanRemarks/////////
                    model.Remarks = productionRemarksData.Remarks;
                    model.ProductionDate = productionRemarksData.ProductionRemarksDate;
                    model.Added = userId;
                    model.AddedDate = DateTime.Now;
                    model.IsCkd = productionRemarksData.IsCkd;
                    model.IsCharger = productionRemarksData.IsCharger;
                    _dbEntities.ProductionPlanRemarks.AddOrUpdate(model);

                    /////////////insert ProductionPlanRemarkLog/////////
                    var model1 = new ProductionPlanRemarksLog();
                    model1.Remarks = productionRemarksData.Remarks;
                    model1.ProductionDate = productionRemarksData.ProductionRemarksDate;
                    model1.Added = userId;
                    model1.AddedDate = DateTime.Now;
                    model1.IsCkd = productionRemarksData.IsCkd;
                    model1.IsCharger = productionRemarksData.IsCharger;
                    _dbEntities.ProductionPlanRemarksLogs.AddOrUpdate(model1);
                }
                _dbEntities.SaveChanges();
            }
            return "ok";
        }
        public bool GetHolidayDatas(string dateForHoliday)
        {
            var dateForHoliday1 = Convert.ToDateTime(dateForHoliday);

            var govt = new GovernmentHolidayTableModel();
            var query = from c in _dbEntities.GovernmentHolidayTables
                        where c.HolidayDate == dateForHoliday1
                        select new
                        {
                            c.HolidayDate,
                            c.GovernmentHoliday,

                        };

            //foreach (var govtQu in query)
            //{

            //    if (govtQu.HolidayDate==dateForHoliday1)
            //    {
            //        govt.HolidayDate = govtQu.HolidayDate;
            //        govt.GovernmentHoliday = govtQu.GovernmentHoliday;
            //    }

            //}
            if (query.Any())
            {
                return true;
            }
            return false;

        }
        public List<GovernmentHolidayTableModel> GetHolidayDatasList()
        {
            string query = string.Format(@"select * from [CellPhoneProject].[dbo].[GovernmentHolidayTable]");
            var exe = _dbEntities.Database.SqlQuery<GovernmentHolidayTableModel>(query).ToList();
            return exe;
        }
        public List<LineInformationModel> SelectLineInfos()
        {
            string query = string.Format(@"select LineNumber from [CellPhoneProject].[dbo].[LineInformation]");
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }
        public List<CustomPrdAssemblyAndPackingDetails> GetAssemblyLineDatas(string assemblyStartDate, string assemblyEndDate)
        {
            string proEv = string.Format(@"select [AssemblyLineInformation],AssemblyProductionStartDate,AssemblyProductionEndDate from [CellPhoneProject].[dbo].[AssemblyProductionEvent] 
              where AssemblyProductionStartDate between '{0}'  and '{1}'
             or AssemblyProductionEndDate  between '{0}'  and '{1}'", assemblyStartDate, assemblyEndDate);


            var proEvent = _dbEntities.Database.SqlQuery<CustomPrdAssemblyAndPackingDetails>(proEv).ToList();

            return proEvent;
        }
        public List<CustomPrdAssemblyAndPackingDetails> GetPackingLineDatas(string packingStartDate, string packingEndDate)
        {

            string proEv = string.Format(@"select PackingLineInformation,PackingProductionStartDate,PackingProductionEndDate from [CellPhoneProject].[dbo].PackingProductionEvent 
              where PackingProductionStartDate between '{0}'  and '{1}'
             or PackingProductionEndDate  between '{0}'  and '{1}'", packingStartDate, packingEndDate);


            var proEvent = _dbEntities.Database.SqlQuery<CustomPrdAssemblyAndPackingDetails>(proEv).ToList();

            return proEvent;
        }
        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;

        }
        public string AddedChargerPlanData(List<CustomChargerProduction> results)
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

                var proPlan = new ProPlanTable();
                proPlan.AddadDate = DateTime.Now;
                proPlan.ProjectId = insResult.ProjectId;
                proPlan.IsCharger = true;
                proPlan.IsCkd = false;
                _dbEntities.ProPlanTables.Add(proPlan);
                _dbEntities.SaveChanges();

                var queryForPlan = (from c in _dbEntities.ProPlanTables
                                    where c.ProjectId == insResult.ProjectId && c.IsCharger == true
                                    select c).OrderByDescending(x => x.PlanId).FirstOrDefault();

                if (queryForPlan != null)
                {
                    if (insResult.SmtChk == true)
                    {
                        var model1 = new ChargerSMT();
                        model1.ProjectId = insResult.ProjectId;
                        model1.PlanId = queryForPlan.PlanId;
                        model1.ProjectName = insResult.ProjectName;
                        model1.OrderNumber = query1.OrderNuber;
                        model1.PoCategory = query2.PoCategory;
                        model1.MaterialReceiveStartDate = insResult.MaterialReceiveStartDateSmt;
                        model1.MaterialReceiveEndDate = insResult.MaterialReceiveEndDateSmt;
                        model1.IqcCompleteStartDate = insResult.IqcCompleteStartDateSmt;
                        model1.IqcCompleteEndDate = insResult.IqcCompleteEndDateSmt;
                        model1.TrialProductionStartDate = insResult.TrialProductionStartDateSmt;
                        model1.TrialProductionEndDate = insResult.TrialProductionEndDateSmt;
                        model1.SmtMassProductionStartDate = insResult.SmtMassProductionStartDateSmt;
                        model1.SmtMassProductionEndDate = insResult.SmtMassProductionEndDateSmt;
                        model1.TotalQuantity = insResult.ChargerSmtTotalQuantity;
                        model1.Status = "PARTIAL";
                        model1.Added = userId;
                        model1.AddedDate = DateTime.Now;

                        _dbEntities.ChargerSMTs.Add(model1);
                        _dbEntities.SaveChanges();
                        ////////////////////////

                        var query11 = (from c in _dbEntities.ChargerSMTs
                                       where c.ProjectId == insResult.ProjectId
                                       select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exe = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmt = GetDatesBetween(Convert.ToDateTime(insResult.SmtMassProductionStartDateSmt), Convert.ToDateTime(insResult.SmtMassProductionEndDateSmt));

                        var datesSmt1 = datesSmt.Except(exe);


                        var lineInfo = insResult.SmtAllLineNumber;
                        var newstring = "";

                        for (int i = 0; i < lineInfo.Length; i++)
                        {
                            if (char.IsUpper(lineInfo[i]))
                                newstring += " ";
                            newstring += lineInfo[i].ToString();
                        }

                        var stroreString = newstring;

                        var stroreStringSplit = stroreString.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit)
                        {

                            var model11 = new ChargerSMTLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            var query12 = (from c in _dbEntities.LineInformations
                                           where c.LineNumber == model11.LineNumber && c.Charger_SMT_Line_Capacity > 0
                                           select c).FirstOrDefault();



                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmt1)
                                {
                                    var query13 = (from c in _dbEntities.ChargerSMTLineCapacityDetails
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    model11.ChargerSMT_Id = query11.Id;
                                    model11.WorkingDate = smtDate;
                                    model11.TotalQuantity = insResult.ChargerSmtTotalQuantity;
                                    model11.PerDayCapacity = insResult.ChargerSmtPerDayCapacity;
                                    model11.LineCapacity = query12.Charger_SMT_Line_Capacity;
                                    //model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerSmtPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(insResult.ChargerSmtPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerSmtPerDayCapacity);
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;

                                    _dbEntities.ChargerSMTLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                    }//end of if statement
                    ///////////////////////////
                    if (insResult.HouseChk == true)
                    {
                        var model2 = new ChargerHousing();
                        model2.ProjectId = insResult.ProjectId;
                        model2.PlanId = queryForPlan.PlanId;
                        model2.ProjectName = insResult.ProjectName;
                        model2.OrderNumber = query1.OrderNuber;
                        model2.PoCategory = query2.PoCategory;
                        model2.MaterialReceiveStartDate = insResult.MaterialReceiveStartDateHousing;
                        model2.MaterialReceiveEndDate = insResult.MaterialReceiveEndDateHousing;
                        model2.IqcCompleteStartDate = insResult.IqcCompleteStartDateHousing;
                        model2.IqcCompleteEndDate = insResult.IqcCompleteEndDateHousing;
                        model2.TrialProductionStartDate = insResult.TrialProductionStartDateHousing;
                        model2.TrialProductionEndDate = insResult.TrialProductionEndDateHousing;
                        model2.HousingMassProductionStartDate = insResult.HousingMassProStartDateHousing;
                        model2.HousingMassProductionEndDate = insResult.HousingMassProEndtDateHousing;
                        model2.HousingReliabilityTestStartDate = insResult.HousingReliabilityStartDateHousing;
                        model2.HousingReliabilityTestEndDate = insResult.HousingReliabilityEndtDateHousing;
                        model2.TotalQuantity = insResult.ChargerHousingTotalQuantity;
                        model2.Status = "PARTIAL";
                        model2.Added = userId;
                        model2.AddedDate = DateTime.Now;
                        _dbEntities.ChargerHousings.Add(model2);
                        _dbEntities.SaveChanges();

                        var query11 = (from c in _dbEntities.ChargerHousings
                                       where c.ProjectId == insResult.ProjectId
                                       select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exe = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmt = GetDatesBetween(Convert.ToDateTime(insResult.HousingMassProStartDateHousing), Convert.ToDateTime(insResult.HousingMassProEndtDateHousing));

                        var datesSmt1 = datesSmt.Except(exe);


                        var lineInfo = insResult.HousingAllLineNumber;
                        var newstring = "";

                        for (int i = 0; i < lineInfo.Length; i++)
                        {
                            if (char.IsUpper(lineInfo[i]))
                                newstring += " ";
                            newstring += lineInfo[i].ToString();
                        }

                        var stroreString = newstring;

                        var stroreStringSplit = stroreString.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit)
                        {

                            var model11 = new ChargerHousingLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            var query12 = (from c in _dbEntities.LineInformations
                                           where c.LineNumber == model11.LineNumber && c.Charger_Housing_Line_Capacity > 0
                                           select c).FirstOrDefault();
                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmt1)
                                {
                                    var query13 = (from c in _dbEntities.ChargerHousingLineCapacityDetails
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    model11.ChargerHousing_Id = query11.Id;
                                    model11.WorkingDate = smtDate;
                                    model11.TotalQuantity = insResult.ChargerHousingTotalQuantity;
                                    model11.PerDayCapacity = insResult.ChargerHousingPerDayCapacity;
                                    model11.LineCapacity = query12.Charger_Housing_Line_Capacity;
                                    // model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerHousingPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(
                                                                            insResult.ChargerHousingPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerHousingPerDayCapacity);
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;

                                    _dbEntities.ChargerHousingLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                    }//end of if statement

                    if (insResult.AssemblyChk == true)
                    {
                        //////////
                        var model3 = new ChargerAssembly();
                        model3.ProjectId = insResult.ProjectId;
                        model3.PlanId = queryForPlan.PlanId;
                        model3.ProjectName = insResult.ProjectName;
                        model3.OrderNumber = query1.OrderNuber;
                        model3.PoCategory = query2.PoCategory;
                        model3.MaterialReceiveStartDate = insResult.MaterialReceiveStartDateAssembly;
                        model3.MaterialReceiveEndDate = insResult.MaterialReceiveEndDateAssembly;
                        model3.IqcCompleteStartDate = insResult.IqcCompleteStartDateAssembly;
                        model3.IqcCompleteEndDate = insResult.IqcCompleteEndDateAssembly;
                        model3.TrialProductionStartDate = insResult.TrialProductionStartDateAssembly;
                        model3.TrialProductionEndDate = insResult.TrialProductionEndDateAssembly;
                        model3.RandDConfirmationStartDate = insResult.RnDConfirmStartDateAssembly;
                        model3.RandDConfirmationEndDate = insResult.RnDConfirmEndDateAssembly;
                        model3.AssemblyProductionStartDate = insResult.AssembStartDateAssembly;
                        model3.AssemblyProductionEndDate = insResult.AssembEndDateAssembly;
                        model3.TotalQuantity = insResult.ChargerAssemblyTotalQuantity;
                        model3.Status = "PARTIAL";
                        model3.Added = userId;
                        model3.AddedDate = DateTime.Now;
                        _dbEntities.ChargerAssemblies.Add(model3);
                        _dbEntities.SaveChanges();
                        ///////
                        var query111 = (from c in _dbEntities.ChargerAssemblies
                                        where c.ProjectId == insResult.ProjectId
                                        select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exe1 = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmt11 = GetDatesBetween(Convert.ToDateTime(insResult.AssembStartDateAssembly), Convert.ToDateTime(insResult.AssembEndDateAssembly));

                        var datesSmt12 = datesSmt11.Except(exe1);


                        var lineInfo1 = insResult.AssemblyAllLineNumber;
                        var newstring1 = "";

                        for (int i = 0; i < lineInfo1.Length; i++)
                        {
                            if (char.IsUpper(lineInfo1[i]))
                                newstring1 += " ";
                            newstring1 += lineInfo1[i].ToString();
                        }

                        var stroreString1 = newstring1;

                        var stroreStringSplit1 = stroreString1.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit1)
                        {

                            var model11 = new ChargerAssemblyLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            var query12 = (from c in _dbEntities.LineInformations
                                           where c.LineNumber == model11.LineNumber && c.Charger_Assembly_Line_Capacity > 0
                                           select c).FirstOrDefault();
                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmt12)
                                {
                                    var query13 = (from c in _dbEntities.ChargerAssemblyLineCapacityDetails
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();
                                    model11.ChargerAssembly_Id = query111.Id;
                                    model11.WorkingDate = smtDate;
                                    model11.TotalQuantity = insResult.ChargerAssemblyTotalQuantity;
                                    model11.PerDayCapacity = insResult.ChargerAssemblyPerDayCapacity;
                                    model11.LineCapacity = query12.Charger_Assembly_Line_Capacity;
                                    // model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerAssemblyPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(insResult.ChargerAssemblyPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerAssemblyPerDayCapacity);
                                    }

                                    model11.LineInformation_Id = query12.Id;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;

                                    _dbEntities.ChargerAssemblyLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                    }//end of assembly
                }//end of ProPlan
            }

            _dbEntities.SaveChanges();

            return "ok";
        }

        //public ProjectMasterModel GetProjectAssemblyTypes(long proIds)
        //{
        //    var proMaster = new ProjectMasterModel();

        //    var assembInfo=(from pm in _dbEntities.ProjectMasters
        //                    join pal in _dbEntities.AssemblyAndPackingPerDayQties on pm.ProjectType equals pal.ProjectType
        //                    where pm.ProjectMasterId == proIds && pal.LineType == "Assembly"
        //                    select new
        //                    {
        //                        pal.ProjectType,
        //                        pal.LineType,
        //                        pal.Quantity
        //                    }).FirstOrDefault();
        //    if (assembInfo != null)
        //    {
        //        proMaster.ProjectType = assembInfo.ProjectType;
        //        proMaster.LineType = assembInfo.LineType;
        //        proMaster.Quantity = assembInfo.Quantity;
        //    }
        //    return proMaster;
        //}

        //public ProjectMasterModel GetProjectPackingTypes(long proIds)
        //{
        //    var proMaster = new ProjectMasterModel();

        //    var packingInfo = (from pm in _dbEntities.ProjectMasters
        //                      join pal in _dbEntities.AssemblyAndPackingPerDayQties on pm.ProjectType equals pal.ProjectType
        //                       where pm.ProjectMasterId == proIds && pal.LineType == "Packing"
        //                      select new
        //                      {
        //                          pal.ProjectType,
        //                          pal.LineType,
        //                          pal.Quantity
        //                      }).FirstOrDefault();
        //    if (packingInfo != null)
        //    {
        //        proMaster.ProjectType = packingInfo.ProjectType;
        //        proMaster.LineType = packingInfo.LineType;
        //        proMaster.Quantity = packingInfo.Quantity;
        //    }
        //    return proMaster;
        //}

        #region hi
        #endregion
        public List<ProductPlanModel> GetGrandChartDatas(List<string> results)
        {
            string query1 = string.Empty;
            string query2 = string.Empty;
            List<VmProductionPlan> exe = new List<VmProductionPlan>();
            query1 = string.Format(@"select 
            ase.ProjectId,ase.ProjectName,CAST(ase.OrderNumber AS VARCHAR(10) ) as OrderNumber,ase.PoCategory,ase.MaterialReceiveDate
            ,ase.IqcCompleteDate,ase.TrialProductionDate,ase.SoftwareConfirmationDate,ase.RnDClearanceDate,
            ase.AssemblyLineInformation,ase.AssemblyProductionStartDate,CAST(ase.AssemblyQuantity AS VARCHAR(10)) as AssemblyQuantity,CAST(ase.AssemblyPerDayCapacity AS VARCHAR(10)) as AssemblyPerDayCapacity,ase.AssemblyProductionEndDate,
            ppe.PackingLineInformation,ppe.PackingProductionStartDate,CAST(ppe.PackingQuantity AS VARCHAR(10)) as PackingQuantity,CAST(ppe.PackingPerDayCapacity AS VARCHAR(10)) as PackingPerDayCapacity,ppe.PackingProductionEndDate

            from CellPhoneProject.dbo.AssemblyProductionEvent ase 
            left join CellPhoneProject.dbo.PackingProductionEvent ppe on ase.ProjectId=ppe.ProjectId
            where ase.ProjectId=ppe.ProjectId and ase.OrderNumber=ppe.OrderNumber and ase.Status='COMPLETED' and ppe.Status='COMPLETED'
            group by 
            ase.ProjectId,ase.ProjectName,ase.OrderNumber,ase.PoCategory,ase.MaterialReceiveDate
            ,ase.IqcCompleteDate,ase.TrialProductionDate,ase.SoftwareConfirmationDate,ase.RnDClearanceDate,
            ase.AssemblyLineInformation,ase.AssemblyProductionStartDate,ase.AssemblyQuantity,ase.AssemblyPerDayCapacity,ase.AssemblyProductionEndDate,
            ppe.PackingLineInformation,ppe.PackingProductionStartDate,ppe.PackingQuantity,ppe.PackingPerDayCapacity,ppe.PackingProductionEndDate");


            List<CustomPrdAssemblyAndPackingDetails> exe1 = _dbEntities.Database.SqlQuery<CustomPrdAssemblyAndPackingDetails>(query1).ToList();
            var productPlanModels = new List<ProductPlanModel>();
            var prdAssemblyList = new List<CustomPrdAssemblyAndPackingDetails>();
            foreach (var ddts in results)
            {
                var datesArr = Convert.ToDateTime(ddts);
                var plan = new VmProductionPlan();
                // var productPlanModel = new ProductPlanModel();

                plan.ProductionDate = datesArr;

                foreach (var custm in exe1)
                {
                    var plan2 = new CustomPrdAssemblyAndPackingDetails();
                    if (datesArr.Date == Convert.ToDateTime(custm.MaterialReceiveDate).Date)
                    {
                        var index = prdAssemblyList.FindIndex(i => Convert.ToDateTime(i.MaterialReceiveDate).Date == datesArr.Date);
                        if (index == -1)
                        {
                            plan2.ProjectName = custm.ProjectName;
                            plan2.PoCategory = custm.PoCategory;
                            plan2.OrderNumber = custm.OrderNumber;
                            plan2.AssemblyQuantity = custm.AssemblyQuantity;
                            plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                            plan2.PackingQuantity = custm.PackingQuantity;
                            plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                            plan2.MaterialReceiveDate = custm.MaterialReceiveDate;
                            plan2.ProductionDate = datesArr.Date;

                            prdAssemblyList.Add(plan2);
                        }
                        else
                        {
                            prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                custm.AssemblyQuantity;
                            prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                custm.AssemblyPerDayCapacity;
                            prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                                custm.PackingQuantity;
                            prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                custm.PackingPerDayCapacity;

                        }

                    }
                    if (datesArr.Date == Convert.ToDateTime(custm.IqcCompleteDate).Date)
                    {
                        var index = prdAssemblyList.FindIndex(i => Convert.ToDateTime(i.IqcCompleteDate).Date == datesArr.Date);
                        if (index == -1)
                        {
                            plan2.ProjectName = custm.ProjectName;
                            plan2.PoCategory = custm.PoCategory;
                            plan2.OrderNumber = custm.OrderNumber;
                            plan2.AssemblyQuantity = custm.AssemblyQuantity;
                            plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                            plan2.PackingQuantity = custm.PackingQuantity;
                            plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                            plan2.IqcCompleteDate = custm.IqcCompleteDate;
                            plan2.ProductionDate = datesArr.Date;

                            prdAssemblyList.Add(plan2);
                        }
                        else
                        {
                            prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                              custm.AssemblyQuantity;
                            prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                custm.AssemblyPerDayCapacity;
                            prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                                custm.PackingQuantity;
                            prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                custm.PackingPerDayCapacity;

                        }

                    }

                    if (datesArr.Date == Convert.ToDateTime(custm.TrialProductionDate).Date)
                    {
                        var index = prdAssemblyList.FindIndex(i => Convert.ToDateTime(i.TrialProductionDate).Date == datesArr.Date);
                        if (index == -1)
                        {
                            plan2.ProjectName = custm.ProjectName;
                            plan2.PoCategory = custm.PoCategory;
                            plan2.OrderNumber = custm.OrderNumber;
                            plan2.AssemblyQuantity = custm.AssemblyQuantity;
                            plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                            plan2.PackingQuantity = custm.PackingQuantity;
                            plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                            plan2.TrialProductionDate = custm.TrialProductionDate;
                            plan2.ProductionDate = datesArr.Date;

                            prdAssemblyList.Add(plan2);
                        }
                        else
                        {
                            prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                              custm.AssemblyQuantity;
                            prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                custm.AssemblyPerDayCapacity;
                            prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                                custm.PackingQuantity;
                            prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                custm.PackingPerDayCapacity;

                        }

                    }
                    if (datesArr.Date == Convert.ToDateTime(custm.SoftwareConfirmationDate).Date)
                    {
                        var index = prdAssemblyList.FindIndex(i => Convert.ToDateTime(i.SoftwareConfirmationDate).Date == datesArr.Date);
                        if (index == -1)
                        {
                            plan2.ProjectName = custm.ProjectName;
                            plan2.PoCategory = custm.PoCategory;
                            plan2.OrderNumber = custm.OrderNumber;
                            plan2.AssemblyQuantity = custm.AssemblyQuantity;
                            plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                            plan2.PackingQuantity = custm.PackingQuantity;
                            plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                            plan2.SoftwareConfirmationDate = custm.SoftwareConfirmationDate;
                            plan2.ProductionDate = datesArr.Date;

                            prdAssemblyList.Add(plan2);
                        }
                        else
                        {
                            prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                              custm.AssemblyQuantity;
                            prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                custm.AssemblyPerDayCapacity;
                            prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                                custm.PackingQuantity;
                            prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                custm.PackingPerDayCapacity;


                        }

                    }
                    if (datesArr.Date == Convert.ToDateTime(custm.RnDClearanceDate).Date)
                    {
                        var index = prdAssemblyList.FindIndex(i => Convert.ToDateTime(i.RnDClearanceDate).Date == datesArr.Date);
                        if (index == -1)
                        {
                            plan2.ProjectName = custm.ProjectName;
                            plan2.PoCategory = custm.PoCategory;
                            plan2.OrderNumber = custm.OrderNumber;
                            plan2.AssemblyQuantity = custm.AssemblyQuantity;
                            plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                            plan2.PackingQuantity = custm.PackingQuantity;
                            plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                            plan2.RnDClearanceDate = custm.RnDClearanceDate;
                            plan2.ProductionDate = datesArr.Date;

                            prdAssemblyList.Add(plan2);
                        }
                        else
                        {
                            prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                              custm.AssemblyQuantity;
                            prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                custm.AssemblyPerDayCapacity;
                            prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                                custm.PackingQuantity;
                            prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                custm.PackingPerDayCapacity;

                        }
                    }
                    /////AssemblyLine///////////////
                    if (datesArr.Date >= custm.AssemblyProductionStartDate && datesArr.Date <= custm.AssemblyProductionEndDate)
                    {

                        var assembly = custm.AssemblyLineInformation;
                        var assembVal = assembly.Split(',').ToList();

                        for (int i = 0; i < assembVal.Count; i++)
                        {
                            plan2 = new CustomPrdAssemblyAndPackingDetails();
                            if (assembVal[i].Trim() == "Line-1")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (assembVal[i].Trim() == "Line-2")
                            {
                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (assembVal[i].Trim() == "Line-3")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (assembVal[i].Trim() == "Line-4")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (assembVal[i].Trim() == "Line-5")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (assembVal[i].Trim() == "Line-6")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            else if (assembVal[i].Trim() == "Line-7")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.AssemblyProductionStartDate <= datesArr.Date && datesArr.Date <= x.AssemblyProductionEndDate && x.AssemblyLine == assembVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.AssemblyProductionStartDate = custm.AssemblyProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.AssemblyLine = assembVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                            custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }

                        }


                    }
                    ///////AssemblyLine////////////

                    /////PackingLine///////////////
                    if (datesArr.Date >= custm.PackingProductionStartDate && datesArr.Date <= custm.PackingProductionEndDate)
                    {

                        var packing = custm.PackingLineInformation;
                        var packingVal = packing.Split(',').ToList();

                        for (int i = 0; i < packingVal.Count; i++)
                        {
                            plan2 = new CustomPrdAssemblyAndPackingDetails();
                            if (packingVal[i].Trim() == "Line-1")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (packingVal[i].Trim() == "Line-2")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (packingVal[i].Trim() == "Line-3")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (packingVal[i].Trim() == "Line-4")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (packingVal[i].Trim() == "Line-5")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (packingVal[i].Trim() == "Line-6")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }
                            if (packingVal[i].Trim() == "Line-7")
                            {

                                var index = prdAssemblyList.FindIndex(x => x.PackingProductionStartDate <= datesArr.Date && datesArr.Date <= x.PackingProductionEndDate && x.PackingLine == packingVal[i]);
                                if (index == -1)
                                {
                                    plan2.ProjectName = custm.ProjectName;
                                    plan2.PoCategory = custm.PoCategory;
                                    plan2.OrderNumber = custm.OrderNumber;
                                    plan2.AssemblyQuantity = custm.AssemblyQuantity;
                                    plan2.AssemblyPerDayCapacity = custm.AssemblyPerDayCapacity;
                                    plan2.PackingQuantity = custm.PackingQuantity;
                                    plan2.PackingPerDayCapacity = custm.PackingPerDayCapacity;
                                    plan2.PackingProductionStartDate = custm.PackingProductionStartDate;
                                    plan2.ProductionDate = datesArr.Date;
                                    plan2.PackingLine = packingVal[i].Trim();

                                    prdAssemblyList.Add(plan2);
                                }
                                else
                                {
                                    prdAssemblyList[index].ProjectName = prdAssemblyList[index].ProjectName + ", " +
                                                                         custm.ProjectName;
                                    prdAssemblyList[index].PoCategory = prdAssemblyList[index].PoCategory + ", " +
                                                                        custm.PoCategory;
                                    prdAssemblyList[index].OrderNumber = prdAssemblyList[index].OrderNumber + ", " +
                                                                         custm.OrderNumber;
                                    prdAssemblyList[index].AssemblyQuantity = prdAssemblyList[index].AssemblyQuantity + ", " +
                                                                      custm.AssemblyQuantity;
                                    prdAssemblyList[index].AssemblyPerDayCapacity = prdAssemblyList[index].AssemblyPerDayCapacity + ", " +
                                                                        custm.AssemblyPerDayCapacity;
                                    prdAssemblyList[index].PackingQuantity = prdAssemblyList[index].PackingQuantity + ", " +
                                                              custm.PackingQuantity;
                                    prdAssemblyList[index].PackingPerDayCapacity = prdAssemblyList[index].PackingPerDayCapacity + ", " +
                                                                        custm.PackingPerDayCapacity;

                                }
                            }

                        }


                    }
                    ///////PackingLine//////
                }
                exe.Add(plan);
            }

            foreach (var pDate in results)
            {
                var convertedPd = Convert.ToDateTime(pDate);
                foreach (var prd in prdAssemblyList)
                {
                    var model = new ProductPlanModel();



                    if (convertedPd == prd.ProductionDate)
                    {
                        var indx = productPlanModels.FindIndex(i => i.ProductionDate == prd.ProductionDate);

                        if (indx == -1)
                        {

                            if (prd.MaterialReceiveDate != null)
                            {

                                model.ProductionDate = prd.ProductionDate;
                                model.MetarialReceive = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.IqcCompleteDate != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.IqcComplete = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.TrialProductionDate != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.TrialProduction = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.SoftwareConfirmationDate != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.SoftwareConfirmation = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.RnDClearanceDate != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.RnDClearance = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-1")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine1 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-2")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine2 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-3")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine3 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-4")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine4 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-5")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine5 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-6")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine6 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-7")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyLine7 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-1")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine1 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-2")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine2 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-3")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine3 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-4")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine4 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-5")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine5 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-6")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine6 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-7")
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.PackingLine7 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " +
                                                    prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber +
                                                    "<br/>" + "AssemblyQuantity:"
                                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" +
                                                    prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" +
                                                    prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" +
                                                    prd.PackingPerDayCapacity;
                            }
                            var proRemrk = (from proPlan in _dbEntities.ProductionPlanRemarks
                                            where proPlan.ProductionDate == prd.ProductionDate
                                            select proPlan).FirstOrDefault();
                            if (proRemrk != null)
                            {
                                if (proRemrk != null)
                                {
                                    model.ProductionDate = prd.ProductionDate;
                                    model.ProductionRemarks = proRemrk.Remarks;
                                }


                            }

                            productPlanModels.Add(model);
                        }
                        else
                        {
                            if (prd.MaterialReceiveDate != null)
                            {
                                productPlanModels[indx].MetarialReceive = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.IqcCompleteDate != null)
                            {
                                productPlanModels[indx].IqcComplete = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.TrialProductionDate != null)
                            {
                                productPlanModels[indx].TrialProduction = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.SoftwareConfirmationDate != null)
                            {
                                productPlanModels[indx].SoftwareConfirmation = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.RnDClearanceDate != null)
                            {
                                productPlanModels[indx].RnDClearance = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                    + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-1")
                            {
                                productPlanModels[indx].AssemblyLine1 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-2")
                            {
                                productPlanModels[indx].AssemblyLine2 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-3")
                            {
                                productPlanModels[indx].AssemblyLine3 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-4")
                            {
                                productPlanModels[indx].AssemblyLine4 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-5")
                            {
                                productPlanModels[indx].AssemblyLine5 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-6")
                            {
                                productPlanModels[indx].AssemblyLine6 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.AssemblyLine == "Line-7")
                            {
                                productPlanModels[indx].AssemblyLine7 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-1")
                            {
                                productPlanModels[indx].PackingLine1 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-2")
                            {
                                productPlanModels[indx].PackingLine2 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-3")
                            {
                                productPlanModels[indx].PackingLine3 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-4")
                            {
                                productPlanModels[indx].PackingLine4 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }

                            else if (prd.PackingLine == "Line-5")
                            {
                                productPlanModels[indx].PackingLine5 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-6")
                            {
                                productPlanModels[indx].PackingLine6 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }
                            else if (prd.PackingLine == "Line-7")
                            {
                                productPlanModels[indx].PackingLine7 = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "AssemblyQuantity:"
                                   + prd.AssemblyQuantity + "<br/>" + "AssemblyPerDayCapacity:" + prd.AssemblyPerDayCapacity + "<br/>" + "PackingQuantity:" + prd.PackingQuantity + "<br/>" + "PackingPerDayCapacity:" + prd.PackingPerDayCapacity;
                            }

                            var proRemrk = (from proPlan in _dbEntities.ProductionPlanRemarks
                                            where proPlan.ProductionDate == prd.ProductionDate
                                            select proPlan).FirstOrDefault();
                            if (proRemrk != null)
                            {
                                if (proRemrk != null)
                                {
                                    productPlanModels[indx].ProductionRemarks = proRemrk.Remarks;
                                }

                            }
                        }

                    }

                }
            }

            return productPlanModels;
        }
        public List<ChargerProductionViewModel> GetChargerGrandChartDatas(List<string> results)
        {
            string query1 = string.Empty;

            List<VmProductionPlan> vmProductionList = new List<VmProductionPlan>();

            query1 = string.Format(@"select ca.ProjectId,ca.ProjectName,CAST(ca.OrderNumber AS VARCHAR(10) ) as OrderNumber,ca.PoCategory,CONVERT(varchar(10), cs.TotalQuantity) as SmtTotalProduction,CONVERT(varchar(10), ca.TotalQuantity) as AssemblyTotalProduction,CONVERT(varchar(10), ch.TotalQuantity) as HousingTotalProduction,cs.MaterialReceiveStartDate as MaterialReceiveStartDateSmt, cs.MaterialReceiveEndDate as MaterialReceiveEndDateSmt,
            cs.IqcCompleteStartDate as IqcCompleteStartDateSmt, cs.IqcCompleteEndDate as IqcCompleteEndDateSmt,cs.TrialProductionStartDate as TrialProductionStartDateSmt, cs.TrialProductionEndDate as TrialProductionEndDateSmt,
            cs.SmtMassProductionStartDate as SmtMassProductionStartDateSmt,cs.SmtMassProductionEndDate as SmtMassProductionEndDateSmt, ch.MaterialReceiveStartDate as MaterialReceiveStartDateHousing, ch.MaterialReceiveEndDate as MaterialReceiveEndDateHousing,
            ch.IqcCompleteStartDate as IqcCompleteStartDateHousing, ch.IqcCompleteEndDate as IqcCompleteEndDateHousing,ch.TrialProductionStartDate as TrialProductionStartDateHousing,
            ch.TrialProductionEndDate as TrialProductionEndDateHousing, ch.HousingReliabilityTestStartDate as HousingReliabilityStartDateHousing, ch.HousingReliabilityTestEndDate as HousingReliabilityEndtDateHousing,ch.HousingMassProductionStartDate as HousingMassProStartDateHousing,
            ch.HousingMassProductionEndDate as HousingMassProEndtDateHousing,
            ca.MaterialReceiveStartDate as MaterialReceiveStartDateAssembly,ca.MaterialReceiveEndDate as MaterialReceiveEndDateAssembly,ca.IqcCompleteStartDate as IqcCompleteStartDateAssembly,
            ca.IqcCompleteEndDate as IqcCompleteEndDateAssembly,ca.TrialProductionStartDate as TrialProductionStartDateAssembly, ca.TrialProductionEndDate as TrialProductionEndDateAssembly,
            ca.RandDConfirmationStartDate as RnDConfirmStartDateAssembly,ca.RandDConfirmationEndDate as RnDConfirmEndDateAssembly,ca.AssemblyProductionStartDate as AssembStartDateAssembly,
            ca.AssemblyProductionEndDate as AssembEndDateAssembly
            FROM [CellPhoneProject].[dbo].ChargerAssembly ca
            left join CellPhoneProject.dbo.ChargerHousing ch on ch.ProjectId=ca.ProjectId
            left join CellPhoneProject.dbo.ChargerSMT cs on cs.ProjectId=ca.ProjectId");

            //            query1 = string.Format(@"select cs.ProjectId,cs.ProjectName,CAST(cs.OrderNumber AS VARCHAR(10) ) as OrderNumber,cs.PoCategory,cs.MaterialReceiveStartDate as MaterialReceiveStartDateSmt, cs.MaterialReceiveEndDate as MaterialReceiveEndDateSmt,
            //            cs.IqcCompleteStartDate as IqcCompleteStartDateSmt, cs.IqcCompleteEndDate as IqcCompleteEndDateSmt,cs.TrialProductionStartDate as TrialProductionStartDateSmt, cs.TrialProductionEndDate as TrialProductionEndDateSmt,
            //            cs.SmtMassProductionStartDate as SmtMassProductionStartDateSmt,cs.SmtMassProductionEndDate as SmtMassProductionEndDateSmt, ch.MaterialReceiveStartDate as MaterialReceiveStartDateHousing, ch.MaterialReceiveEndDate as MaterialReceiveEndDateHousing,
            //            ch.IqcCompleteStartDate as IqcCompleteStartDateHousing, ch.IqcCompleteEndDate as IqcCompleteEndDateHousing,ch.TrialProductionStartDate as TrialProductionStartDateHousing,
            //            ch.TrialProductionEndDate as TrialProductionEndDateHousing, ch.HousingReliabilityTestStartDate as HousingReliabilityStartDateHousing, ch.HousingReliabilityTestEndDate as HousingReliabilityEndtDateHousing,ch.HousingMassProductionStartDate as HousingMassProStartDateHousing,
            //            ch.HousingMassProductionEndDate as HousingMassProEndtDateHousing,
            //            ca.MaterialReceiveStartDate as MaterialReceiveStartDateAssembly,ca.MaterialReceiveEndDate as MaterialReceiveEndDateAssembly,ca.IqcCompleteStartDate as IqcCompleteStartDateAssembly,
            //            ca.IqcCompleteEndDate as IqcCompleteEndDateAssembly,ca.TrialProductionStartDate as TrialProductionStartDateAssembly, ca.TrialProductionEndDate as TrialProductionEndDateAssembly,
            //            ca.RandDConfirmationStartDate as RnDConfirmStartDateAssembly,ca.RandDConfirmationEndDate as RnDConfirmEndDateAssembly,ca.AssemblyProductionStartDate as AssembStartDateAssembly,
            //            ca.AssemblyProductionEndDate as AssembEndDateAssembly
            //            FROM [CellPhoneProject].[dbo].ChargerSMT cs
            //            left join CellPhoneProject.dbo.ChargerHousing ch on ch.ProjectId=cs.ProjectId
            //            left join CellPhoneProject.dbo.ChargerAssembly ca on ca.ProjectId=cs.ProjectId
            //            where  ch.ProjectId=cs.ProjectId and ca.ProjectId=cs.ProjectId");

            List<CustomChargerProduction> chargerProQuery = _dbEntities.Database.SqlQuery<CustomChargerProduction>(query1).ToList();

            List<ChargerProductionViewModel> cmCharVmModels = new List<ChargerProductionViewModel>();

            var chargerProList = new List<CustomChargerProduction>();

            foreach (var ddts in results)
            {
                var datesArr = Convert.ToDateTime(ddts);
                var vmProduction = new VmProductionPlan();

                vmProduction.ProductionDate = datesArr;

                foreach (var custm in chargerProQuery)
                {
                    var chargerPro = new CustomChargerProduction();

                    ////////////////////////////////////

                    if (datesArr.Date >= custm.MaterialReceiveStartDateSmt && datesArr.Date <= custm.MaterialReceiveEndDateSmt)
                    {
                        var index =
                            chargerProList.FindIndex(x => Convert.ToDateTime(x.MaterialReceiveStartDateSmt) <= datesArr.Date && datesArr.Date <= Convert.ToDateTime(x.MaterialReceiveEndDateSmt));
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.SmtTotalProduction = custm.SmtTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.MaterialReceiveStartDateSmt = custm.MaterialReceiveStartDateSmt;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].SmtTotalProduction = chargerProList[index].SmtTotalProduction + ", " +
                                                                custm.SmtTotalProduction;

                        }
                    }

                    if (datesArr.Date >= custm.IqcCompleteStartDateSmt && datesArr.Date <= custm.IqcCompleteEndDateSmt)
                    {
                        var index =
                            chargerProList.FindIndex(x => x.IqcCompleteStartDateSmt <= datesArr.Date && datesArr.Date <= x.IqcCompleteEndDateSmt);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.SmtTotalProduction = custm.SmtTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.IqcCompleteStartDateSmt = custm.IqcCompleteStartDateSmt;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].SmtTotalProduction = chargerProList[index].SmtTotalProduction + ", " +
                                                               custm.SmtTotalProduction;
                        }

                    }

                    if (datesArr.Date >= custm.TrialProductionStartDateSmt && datesArr.Date <= custm.TrialProductionEndDateSmt)
                    {
                        var index =
                            chargerProList.FindIndex(x => x.TrialProductionStartDateSmt <= datesArr.Date && datesArr.Date <= x.TrialProductionEndDateSmt);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.SmtTotalProduction = custm.SmtTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.TrialProductionStartDateSmt = custm.TrialProductionStartDateSmt;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].SmtTotalProduction = chargerProList[index].SmtTotalProduction + ", " +
                                                               custm.SmtTotalProduction;
                        }

                    }
                    if (datesArr.Date >= custm.SmtMassProductionStartDateSmt && datesArr.Date <= custm.SmtMassProductionEndDateSmt)
                    {
                        var index =
                            chargerProList.FindIndex(x => x.SmtMassProductionStartDateSmt <= datesArr.Date && datesArr.Date <= x.SmtMassProductionEndDateSmt);

                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.SmtTotalProduction = custm.SmtTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.SmtMassProductionStartDateSmt = custm.SmtMassProductionStartDateSmt;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].SmtTotalProduction = chargerProList[index].SmtTotalProduction + ", " +
                                                               custm.SmtTotalProduction;
                        }

                    }

                    if (datesArr.Date >= custm.MaterialReceiveStartDateHousing && datesArr.Date <= custm.MaterialReceiveEndDateHousing)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.MaterialReceiveStartDateHousing <= datesArr.Date && datesArr.Date <= x.MaterialReceiveEndDateHousing);

                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.HousingTotalProduction = custm.HousingTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.MaterialReceiveStartDateHousing = custm.MaterialReceiveStartDateHousing;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].HousingTotalProduction = chargerProList[index].HousingTotalProduction + ", " +
                                                             custm.HousingTotalProduction;
                        }

                    }

                    if (datesArr.Date >= custm.IqcCompleteStartDateHousing && datesArr.Date <= custm.IqcCompleteEndDateHousing)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.IqcCompleteStartDateHousing <= datesArr.Date && datesArr.Date <= x.IqcCompleteEndDateHousing);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.HousingTotalProduction = custm.HousingTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.IqcCompleteStartDateHousing = custm.IqcCompleteStartDateHousing;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].HousingTotalProduction = chargerProList[index].HousingTotalProduction + ", " +
                                                        custm.HousingTotalProduction;
                        }

                    }

                    if (datesArr.Date >= custm.TrialProductionStartDateHousing && datesArr.Date <= custm.TrialProductionEndDateHousing)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.TrialProductionStartDateHousing <= datesArr.Date && datesArr.Date <= x.TrialProductionEndDateHousing);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.HousingTotalProduction = custm.HousingTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.TrialProductionStartDateHousing = custm.TrialProductionStartDateHousing;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].HousingTotalProduction = chargerProList[index].HousingTotalProduction + ", " +
                                                        custm.HousingTotalProduction;
                        }

                    }


                    if (datesArr.Date >= custm.HousingReliabilityStartDateHousing && datesArr.Date <= custm.HousingReliabilityEndtDateHousing)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.HousingReliabilityStartDateHousing <= datesArr.Date && datesArr.Date <= x.HousingReliabilityEndtDateHousing);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.HousingTotalProduction = custm.HousingTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.HousingReliabilityStartDateHousing = custm.HousingReliabilityStartDateHousing;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].HousingTotalProduction = chargerProList[index].HousingTotalProduction + ", " +
                                                        custm.HousingTotalProduction;
                        }

                    }


                    if (datesArr.Date >= custm.HousingMassProStartDateHousing && datesArr.Date <= custm.HousingMassProEndtDateHousing)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.HousingMassProStartDateHousing <= datesArr.Date && datesArr.Date <= x.HousingMassProEndtDateHousing);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.HousingTotalProduction = custm.HousingTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.HousingMassProStartDateHousing = custm.HousingMassProStartDateHousing;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].HousingTotalProduction = chargerProList[index].HousingTotalProduction + ", " +
                                                        custm.HousingTotalProduction;
                        }

                    }



                    if (datesArr.Date >= custm.MaterialReceiveStartDateAssembly && datesArr.Date <= custm.MaterialReceiveEndDateAssembly)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.MaterialReceiveStartDateAssembly <= datesArr.Date && datesArr.Date <= x.MaterialReceiveEndDateAssembly);

                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.AssemblyTotalProduction = custm.AssemblyTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.MaterialReceiveStartDateAssembly = custm.MaterialReceiveStartDateAssembly;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].AssemblyTotalProduction = chargerProList[index].AssemblyTotalProduction + ", " +
                                                    custm.AssemblyTotalProduction;

                        }

                    }


                    if (datesArr.Date >= custm.IqcCompleteStartDateAssembly && datesArr.Date <= custm.IqcCompleteEndDateAssembly)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.IqcCompleteStartDateAssembly <= datesArr.Date && datesArr.Date <= x.IqcCompleteEndDateAssembly);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.AssemblyTotalProduction = custm.AssemblyTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.IqcCompleteStartDateAssembly = custm.IqcCompleteStartDateAssembly;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].AssemblyTotalProduction = chargerProList[index].AssemblyTotalProduction + ", " +
                                                  custm.AssemblyTotalProduction;
                        }

                    }


                    if (datesArr.Date >= custm.TrialProductionStartDateAssembly && datesArr.Date <= custm.TrialProductionEndDateAssembly)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.TrialProductionStartDateAssembly <= datesArr.Date && datesArr.Date <= x.TrialProductionEndDateAssembly);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.AssemblyTotalProduction = custm.AssemblyTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.TrialProductionStartDateAssembly = custm.TrialProductionStartDateAssembly;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].AssemblyTotalProduction = chargerProList[index].AssemblyTotalProduction + ", " +
                                                  custm.AssemblyTotalProduction;
                        }

                    }

                    if (datesArr.Date >= custm.RnDConfirmStartDateAssembly && datesArr.Date <= custm.RnDConfirmEndDateAssembly)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.RnDConfirmStartDateAssembly <= datesArr.Date && datesArr.Date <= x.RnDConfirmEndDateAssembly);

                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.AssemblyTotalProduction = custm.AssemblyTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.RnDConfirmStartDateAssembly = custm.RnDConfirmStartDateAssembly;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].AssemblyTotalProduction = chargerProList[index].AssemblyTotalProduction + ", " +
                                                  custm.AssemblyTotalProduction;
                        }

                    }

                    if (datesArr.Date >= custm.AssembStartDateAssembly && datesArr.Date <= custm.AssembEndDateAssembly)
                    {
                        var index =
                         chargerProList.FindIndex(x => x.AssembStartDateAssembly <= datesArr.Date && datesArr.Date <= x.AssembEndDateAssembly);
                        if (index == -1)
                        {
                            chargerPro.ProjectName = custm.ProjectName;
                            chargerPro.PoCategory = custm.PoCategory;
                            chargerPro.OrderNumber = custm.OrderNumber;
                            chargerPro.AssemblyTotalProduction = custm.AssemblyTotalProduction;
                            chargerPro.ProductionDate = datesArr.Date;
                            chargerPro.AssembStartDateAssembly = custm.AssembStartDateAssembly;

                            chargerProList.Add(chargerPro);
                        }
                        else
                        {
                            chargerProList[index].ProjectName = chargerProList[index].ProjectName + ", " +
                                                                 custm.ProjectName;
                            chargerProList[index].PoCategory = chargerProList[index].PoCategory + ", " +
                                                                custm.PoCategory;
                            chargerProList[index].OrderNumber = chargerProList[index].OrderNumber + ", " +
                                                                 custm.OrderNumber;
                            chargerProList[index].AssemblyTotalProduction = chargerProList[index].AssemblyTotalProduction + ", " +
                                                  custm.AssemblyTotalProduction;
                        }

                    }

                }
                vmProductionList.Add(vmProduction);
            }

            foreach (var pDate in results)
            {
                var convertedPd = Convert.ToDateTime(pDate);
                foreach (var prd in chargerProList)
                {
                    var model = new ChargerProductionViewModel();

                    if (convertedPd == prd.ProductionDate)
                    {
                        var indx = cmCharVmModels.FindIndex(i => i.ProductionDate == prd.ProductionDate);

                        if (indx == -1)
                        {

                            if (prd.MaterialReceiveStartDateSmt != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.MetarialReceiveSmt = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.IqcCompleteStartDateSmt != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.IqcCompleteSmt = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.TrialProductionStartDateSmt != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.TrialProductionSmt = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.SmtMassProductionStartDateSmt != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.SmtMassProduction = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.MaterialReceiveStartDateHousing != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.MetarialReceiveHousing = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.IqcCompleteStartDateHousing != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.IqcCompleteHousing = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.TrialProductionStartDateHousing != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.TrialProductionHousing = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.HousingReliabilityStartDateHousing != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.HousingReliability = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.HousingMassProStartDateHousing != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.HousingMassProduction = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }

                            if (prd.MaterialReceiveStartDateAssembly != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.MetarialReceiveAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }

                            if (prd.IqcCompleteStartDateAssembly != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.IqcCompleteAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }

                            if (prd.TrialProductionStartDateAssembly != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.TrialProductionAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }

                            if (prd.RnDConfirmStartDateAssembly != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.RnDConfirmAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }

                            if (prd.AssembStartDateAssembly != null)
                            {
                                model.ProductionDate = prd.ProductionDate;
                                model.AssemblyDate = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }

                            cmCharVmModels.Add(model);
                        }
                        else
                        {
                            if (prd.MaterialReceiveStartDateSmt != null)
                            {
                                cmCharVmModels[indx].MetarialReceiveSmt = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.IqcCompleteStartDateSmt != null)
                            {
                                cmCharVmModels[indx].IqcCompleteSmt = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.TrialProductionStartDateSmt != null)
                            {
                                cmCharVmModels[indx].TrialProductionSmt = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.SmtMassProductionStartDateSmt != null)
                            {
                                cmCharVmModels[indx].SmtMassProduction = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.SmtTotalProduction;
                            }
                            if (prd.MaterialReceiveStartDateHousing != null)
                            {
                                cmCharVmModels[indx].MetarialReceiveHousing = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.IqcCompleteStartDateHousing != null)
                            {
                                cmCharVmModels[indx].IqcCompleteHousing = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.TrialProductionStartDateHousing != null)
                            {
                                cmCharVmModels[indx].TrialProductionHousing = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.HousingReliabilityStartDateHousing != null)
                            {
                                cmCharVmModels[indx].HousingReliability = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.HousingMassProStartDateHousing != null)
                            {
                                cmCharVmModels[indx].HousingMassProduction = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.HousingTotalProduction;
                            }
                            if (prd.MaterialReceiveStartDateAssembly != null)
                            {
                                cmCharVmModels[indx].MetarialReceiveAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }
                            if (prd.IqcCompleteStartDateAssembly != null)
                            {
                                cmCharVmModels[indx].IqcCompleteAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }
                            if (prd.TrialProductionStartDateAssembly != null)
                            {
                                cmCharVmModels[indx].TrialProductionAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }
                            if (prd.RnDConfirmStartDateAssembly != null)
                            {
                                cmCharVmModels[indx].RnDConfirmAssembly = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }
                            if (prd.AssembStartDateAssembly != null)
                            {
                                cmCharVmModels[indx].AssemblyDate = "ProjectName: " + prd.ProjectName + "<br/>" + "PoCategory: " + prd.PoCategory + "<br/>" + "OrderNumber:" + prd.OrderNumber + "<br/>" + "TotalQuantity:" + prd.AssemblyTotalProduction;
                            }

                        }

                    }

                }
            }


            return cmCharVmModels;
        }

        public bool GetMaterialReceiveForSmt(string materialReceiveStartDateSmt, string materialReceiveEndDateSmt)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [MaterialReceiveStartDate],[MaterialReceiveEndDate] from [CellPhoneProject].[dbo].[ChargerSMT]
              where MaterialReceiveStartDate between '{0}'  and '{1}'
             or MaterialReceiveEndDate  between '{0}'  and '{1}'", materialReceiveStartDateSmt, materialReceiveEndDateSmt);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteForSmt(string iqcCompleteStartDateSmt, string iqcCompleteEndDateSmt)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select IqcCompleteStartDate,IqcCompleteEndDate from [CellPhoneProject].[dbo].[ChargerSMT]
              where IqcCompleteStartDate between '{0}'  and '{1}'
              or IqcCompleteEndDate  between '{0}'  and '{1}'", iqcCompleteStartDateSmt, iqcCompleteEndDateSmt);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialProductionDateForSmt(string trialProductionStartDateSmt, string trialProductionEndDateSmt)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select TrialProductionStartDate,TrialProductionEndDate from [CellPhoneProject].[dbo].[ChargerSMT]
              where TrialProductionStartDate between '{0}'  and '{1}'
              or TrialProductionEndDate  between '{0}'  and '{1}'", trialProductionStartDateSmt, trialProductionEndDateSmt);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetMassProductionDateForSmt(string massProductionStartDateSmt, string massProductionEndDateSmt)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select SmtMassProductionStartDate,SmtMassProductionEndDate from [CellPhoneProject].[dbo].[ChargerSMT]
              where SmtMassProductionStartDate between '{0}'  and '{1}'
              or SmtMassProductionEndDate  between '{0}'  and '{1}'", massProductionStartDateSmt, massProductionEndDateSmt);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetMaterialReceiveDateHousing(string materialReceiveStartDateHousing, string materialReceiveEndDateHousing)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select MaterialReceiveStartDate,MaterialReceiveEndDate from [CellPhoneProject].[dbo].[ChargerHousing]
             where MaterialReceiveStartDate between '{0}'  and '{1}'
              or [MaterialReceiveEndDate]  between '{0}'  and '{1}'", materialReceiveStartDateHousing, materialReceiveEndDateHousing);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteDateHousing(string iqcCompleteStartDateHousing, string iqcCompleteEndDateHousing)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [IqcCompleteStartDate],[IqcCompleteEndDate] from [CellPhoneProject].[dbo].[ChargerHousing]
             where [IqcCompleteStartDate] between '{0}'  and '{1}'
              or [IqcCompleteEndDate]  between '{0}'  and '{1}'", iqcCompleteStartDateHousing, iqcCompleteEndDateHousing);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialProductionDateHousing(string trialProductionStartDateHousing, string trialProductionEndDateHousing)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [TrialProductionStartDate],[TrialProductionEndDate] from [CellPhoneProject].[dbo].[ChargerHousing]
             where [TrialProductionStartDate] between '{0}'  and '{1}'
              or [TrialProductionEndDate]  between '{0}'  and '{1}'", trialProductionStartDateHousing, trialProductionEndDateHousing);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetHousingReliabilityDate(string housingReliabilityStartDateHousing, string housingReliabilityEndtDateHousing)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [HousingReliabilityTestStartDate],[HousingReliabilityTestEndDate] from [CellPhoneProject].[dbo].[ChargerHousing]
             where [HousingReliabilityTestStartDate] between '{0}'  and '{1}'
              or [HousingReliabilityTestEndDate]  between '{0}'  and '{1}'", housingReliabilityStartDateHousing, housingReliabilityEndtDateHousing);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetHousingMassProduction(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [HousingMassProductionStartDate],[HousingMassProductionEndDate] from [CellPhoneProject].[dbo].[ChargerHousing]
             where [HousingMassProductionStartDate] between '{0}'  and '{1}'
              or [HousingMassProductionEndDate]  between '{0}'  and '{1}'", housingMassProStartDateHousing, housingMassProEndtDateHousing);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetMaterialReceiveDateAssembly(string materialReceiveStartDateAssembly, string materialReceiveEndDateAssembly)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [MaterialReceiveStartDate] as MaterialReceiveStartDateAssembly,[MaterialReceiveEndDate] as MaterialReceiveEndDateAssembly from [CellPhoneProject].[dbo].[ChargerAssembly]
             where [MaterialReceiveStartDate] between '{0}'  and '{1}'
              or [MaterialReceiveEndDate]  between '{0}'  and '{1}'", materialReceiveStartDateAssembly, materialReceiveEndDateAssembly);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteDateAssembly(string iqcCompleteStartDateAssembly, string iqcCompleteEndDateAssembly)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [IqcCompleteStartDate],[IqcCompleteEndDate] from [CellPhoneProject].[dbo].[ChargerAssembly]
             where [IqcCompleteStartDate] between '{0}'  and '{1}'
              or [IqcCompleteEndDate]  between '{0}'  and '{1}'", iqcCompleteStartDateAssembly, iqcCompleteEndDateAssembly);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialProductionDateAssembly(string trialProductionStartDateAssembly, string trialProductionEndDateAssembly)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [TrialProductionStartDate],[TrialProductionEndDate] from [CellPhoneProject].[dbo].[ChargerAssembly]
             where [TrialProductionStartDate] between '{0}'  and '{1}'
              or [TrialProductionEndDate]  between '{0}'  and '{1}'", trialProductionStartDateAssembly, trialProductionEndDateAssembly);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetRnDConfirmDateAssembly(string rnDConfirmStartDateAssembly, string rnDConfirmEndDateAssembly)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [RandDConfirmationStartDate],[RandDConfirmationEndDate] from [CellPhoneProject].[dbo].[ChargerAssembly]
             where [RandDConfirmationStartDate] between '{0}'  and '{1}'
              or [RandDConfirmationEndDate]  between '{0}'  and '{1}'", rnDConfirmStartDateAssembly, rnDConfirmEndDateAssembly);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetAssemblyProduction(string assembStartDateAssembly, string assembEndDateAssembly)
        {
            var chargerPro = new List<CustomChargerProduction>();

            string proEv = string.Format(@"select [AssemblyProductionStartDate],[AssemblyProductionEndDate] from [CellPhoneProject].[dbo].[ChargerAssembly]
             where [AssemblyProductionStartDate] between '{0}'  and '{1}'
              or [AssemblyProductionEndDate]  between '{0}'  and '{1}'", assembStartDateAssembly, assembEndDateAssembly);
            chargerPro =
                   _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            if (chargerPro != null && chargerPro.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<CustomChargerProduction> GetChargerOldHistory(long proIds)
        {
            //    string proEv = string.Format(@"select pm.ProjectName,pm.ProjectMasterId,pm.OrderNuber as OrderNumber from CellPhoneProject.dbo.ProjectMasters pm
            //    left join CellPhoneProject.dbo.ProjectPurchaseOrderForms ppo on pm.ProjectMasterId=ppo.ProjectMasterId
            //    where pm.ProjectMasterId=ppo.ProjectMasterId and ppo.PoCategory in ('SKD','CKD')");

            //            var proEv = string.Format(@"select top 1 cs.ProjectId,cs.ProjectName,CAST(cs.OrderNumber AS VARCHAR(10) ) as OrderNumber,cs.PoCategory,cs.MaterialReceiveStartDate as MaterialReceiveStartDateSmt, cs.MaterialReceiveEndDate as MaterialReceiveEndDateSmt,
            //            cs.IqcCompleteStartDate as IqcCompleteStartDateSmt, cs.IqcCompleteEndDate as IqcCompleteEndDateSmt,cs.TrialProductionStartDate as TrialProductionStartDateSmt, cs.TrialProductionEndDate as TrialProductionEndDateSmt,
            //            cs.SmtMassProductionStartDate as SmtMassProductionStartDateSmt,cs.SmtMassProductionEndDate as SmtMassProductionEndDateSmt, ch.MaterialReceiveStartDate as MaterialReceiveStartDateHousing, ch.MaterialReceiveEndDate as MaterialReceiveEndDateHousing,
            //            ch.IqcCompleteStartDate as IqcCompleteStartDateHousing, ch.IqcCompleteEndDate as IqcCompleteEndDateHousing,ch.TrialProductionStartDate as TrialProductionStartDateHousing,
            //            ch.TrialProductionEndDate as TrialProductionEndDateHousing, ch.HousingReliabilityTestStartDate as HousingReliabilityStartDateHousing, ch.HousingReliabilityTestEndDate as HousingReliabilityEndtDateHousing,ch.HousingMassProductionStartDate as HousingMassProStartDateHousing,
            //            ch.HousingMassProductionEndDate as HousingMassProEndtDateHousing,
            //            ca.MaterialReceiveStartDate as MaterialReceiveStartDateAssembly,ca.MaterialReceiveEndDate as MaterialReceiveEndDateAssembly,ca.IqcCompleteStartDate as IqcCompleteStartDateAssembly,
            //            ca.IqcCompleteEndDate as IqcCompleteEndDateAssembly,ca.TrialProductionStartDate as TrialProductionStartDateAssembly, ca.TrialProductionEndDate as TrialProductionEndDateAssembly,
            //            ca.RandDConfirmationStartDate as RnDConfirmStartDateAssembly,ca.RandDConfirmationEndDate as RnDConfirmEndDateAssembly,ca.AssemblyProductionStartDate as AssembStartDateAssembly,
            //            ca.AssemblyProductionEndDate as AssembEndDateAssembly
            //            FROM [CellPhoneProject].[dbo].ChargerSMT cs
            //            left join CellPhoneProject.dbo.ChargerHousing ch on ch.ProjectId=cs.ProjectId
            //            left join CellPhoneProject.dbo.ChargerAssembly ca on ca.ProjectId=cs.ProjectId
            //            where  ch.ProjectId='{0}' and ca.ProjectId='{0}' and cs.ProjectId='{0}' order by ch.AddedDate,cs.AddedDate,ca.AddedDate,ca.id,ch.Id,cs.Id desc ", proIds);


            var proEv = string.Format(@"select top 1 ca.planId,ch.planId,cs.planid,ca.ProjectId,ca.ProjectName,CAST(ca.OrderNumber AS VARCHAR(10) ) as OrderNumber,ca.PoCategory,CONVERT(varchar(10), ca.TotalQuantity) as AssemblyTotalProduction, ca.MaterialReceiveStartDate as MaterialReceiveStartDateAssembly,ca.MaterialReceiveEndDate as MaterialReceiveEndDateAssembly,
                ca.IqcCompleteStartDate as IqcCompleteStartDateAssembly,ca.IqcCompleteEndDate as IqcCompleteEndDateAssembly,ca.TrialProductionStartDate as TrialProductionStartDateAssembly,
                ca.TrialProductionEndDate as TrialProductionEndDateAssembly,ca.RandDConfirmationStartDate as RnDConfirmStartDateAssembly,ca.RandDConfirmationEndDate as RnDConfirmEndDateAssembly,
                ca.AssemblyProductionStartDate as AssembStartDateAssembly,ca.AssemblyProductionEndDate as AssembEndDateAssembly,
                CONVERT(varchar(10), cs.TotalQuantity) as SmtTotalProduction,cs.MaterialReceiveStartDate as MaterialReceiveStartDateSmt, cs.MaterialReceiveEndDate as MaterialReceiveEndDateSmt,
                cs.IqcCompleteStartDate as IqcCompleteStartDateSmt, cs.IqcCompleteEndDate as IqcCompleteEndDateSmt,cs.TrialProductionStartDate as TrialProductionStartDateSmt, cs.TrialProductionEndDate as TrialProductionEndDateSmt,
                cs.SmtMassProductionStartDate as SmtMassProductionStartDateSmt,cs.SmtMassProductionEndDate as SmtMassProductionEndDateSmt,
                CONVERT(varchar(10), ch.TotalQuantity) as HousingTotalProduction,
                ch.MaterialReceiveStartDate as MaterialReceiveStartDateHousing, ch.MaterialReceiveEndDate as MaterialReceiveEndDateHousing,
                ch.IqcCompleteStartDate as IqcCompleteStartDateHousing, ch.IqcCompleteEndDate as IqcCompleteEndDateHousing,ch.TrialProductionStartDate as TrialProductionStartDateHousing,
                ch.TrialProductionEndDate as TrialProductionEndDateHousing, ch.HousingReliabilityTestStartDate as HousingReliabilityStartDateHousing, ch.HousingReliabilityTestEndDate as HousingReliabilityEndtDateHousing,ch.HousingMassProductionStartDate as HousingMassProStartDateHousing,
                ch.HousingMassProductionEndDate as HousingMassProEndtDateHousing

                FROM [CellPhoneProject].[dbo].ChargerAssembly ca
                left join CellPhoneProject.dbo.ChargerSMT cs on cs.ProjectId=ca.ProjectId and cs.PlanId=ca.PlanId
                left join CellPhoneProject.dbo.ChargerHousing ch on ch.ProjectId=ca.ProjectId and ch.PlanId=ca.PlanId
                where   (ca.ProjectId='{0}' and ca.planid=(select top 1 planid from CellPhoneProject.dbo.ChargerAssembly where ProjectId=ca.ProjectId order by planid desc))", proIds);

            var proEvent = _dbEntities.Database.SqlQuery<CustomChargerProduction>(proEv).ToList();

            return proEvent;
        }

        public List<LineInformationModel> SelectLineInfoChargerSmt()
        {
            string query = string.Format(@"select LineNumber,[Charger_SMT_Line_Capacity] from [CellPhoneProject].[dbo].[LineInformation]  where [Charger_SMT_Line_Capacity]>0");
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }
        public List<LineInformationModel> SelectLineInfoChargerHousing()
        {
            string query = string.Format(@"select LineNumber,Charger_Housing_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation]  where Charger_Housing_Line_Capacity>0");
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }

        public List<LineInformationModel> SelectLineInfoChargerAssembly()
        {
            string query = string.Format(@"select LineNumber,Charger_Assembly_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation]  where Charger_Assembly_Line_Capacity>0");
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }
        public List<ChargerSMTLineCapacityDetailsModel> GetAvailableProductionLineForSmt(string massProductionStartDateSmt, string massProductionEndDateSmt)
        {
            string query = string.Format(@"select [WorkingDate]
                  ,[PerDayCapacity]
                  ,[LineCapacity]
                  ,[LineAvailableCapacity]                 
                  ,[LineNumber] from [CellPhoneProject].[dbo].[ChargerSMTLineCapacityDetails]  where WorkingDate between '{0}'  and '{1}' order by LineNumber asc ", massProductionStartDateSmt, massProductionEndDateSmt);
            var exe = _dbEntities.Database.SqlQuery<ChargerSMTLineCapacityDetailsModel>(query).ToList();
            return exe;
        }

        public List<ChargerHousingLineCapacityDetailsModel> GetAvailableProductionLineForHousing(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            string query = string.Format(@"select [WorkingDate]
                  ,[PerDayCapacity]
                  ,[LineCapacity]
                  ,[LineAvailableCapacity]                 
                  ,[LineNumber] from [CellPhoneProject].[dbo].[ChargerHousingLineCapacityDetails]  where WorkingDate between '{0}'  and '{1}' order by LineNumber asc ", housingMassProStartDateHousing, housingMassProEndtDateHousing);
            var exe = _dbEntities.Database.SqlQuery<ChargerHousingLineCapacityDetailsModel>(query).ToList();
            return exe;
        }

        public List<ChargerAssemblyLineCapacityDetailsModel> GetAvailableProductionLineForAssembly(string assembStartDateAssembly, string assembEndDateAssembly)
        {
            string query = string.Format(@"select [WorkingDate]
                  ,[PerDayCapacity]
                  ,[LineCapacity]
                  ,[LineAvailableCapacity]                 
                  ,[LineNumber] from [CellPhoneProject].[dbo].[ChargerAssemblyLineCapacityDetails]  where WorkingDate between '{0}'  and '{1}' order by LineNumber asc ", assembStartDateAssembly, assembEndDateAssembly);
            var exe = _dbEntities.Database.SqlQuery<ChargerAssemblyLineCapacityDetailsModel>(query).ToList();
            return exe;
        }


        #endregion

        #region Battery production
        public List<LineInformationModel> SelectLineInfoBatterySmt(long proIds)
        {
            string query = string.Format(@"select LineNumber,CKD_SMT_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation] lii left join 
            CellPhoneProject.dbo.ProjectMasters pm on lii.ProjectType=pm.ProjectType
            where CKD_SMT_Line_Capacity>0 and pm.ProjectMasterId='{0}' ", proIds);
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }

        public List<LineInformationModel> SelectLineInfoBatteryHousing()
        {
            string query = string.Format(@"select LineNumber,CKD_Housing_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation]  where CKD_Housing_Line_Capacity>0");
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }

        public List<LineInformationModel> SelectLineInfoBattery()
        {
            string query = string.Format(@"select LineNumber,CKD_Battery_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation]  where CKD_Battery_Line_Capacity>0");
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }

        public List<LineInformationModel> SelectLineInfoBatteryAssembly(long proIds)
        {
            string query = string.Format(@"select LineNumber,CKD_Assembly_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation] lii left join 
            CellPhoneProject.dbo.ProjectMasters pm on lii.ProjectType=pm.ProjectType
            where CKD_Assembly_Line_Capacity>0 and pm.ProjectMasterId='{0}' ", proIds);
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }

        public List<LineInformationModel> SelectLineInfoBatteryPacking(long proIds)
        {
            string query = string.Format(@"select LineNumber,CKD_Packing_Line_Capacity from [CellPhoneProject].[dbo].[LineInformation] lii left join 
            CellPhoneProject.dbo.ProjectMasters pm on lii.ProjectType=pm.ProjectType
            where CKD_Packing_Line_Capacity>0 and pm.ProjectMasterId='{0}' ", proIds);
            var exe = _dbEntities.Database.SqlQuery<LineInformationModel>(query).ToList();
            return exe;
        }
        public List<BatteryAssemblyLineCapacityDetailModel> GetAvailableProductionLineForBatteryAssembly1(string assembStartDateBAssembly, long proIds)
        {
            var qry = (from c in _dbEntities.ProjectMasters
                       where c.ProjectMasterId == proIds
                       select c).FirstOrDefault();

            string query = string.Format(@"SELECT distinct bp.LineNumber,lii.ProjectType, bp.Production,bp.WorkingDate,(case when bp.LineNumber=lii.LineNumber then (select sum (PerDayCapacity) from [BatteryAssemblyLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber) 
             else bp.PerDayCapacity end) as PerDayCapacity,bp.LineCapacity
            ,(case when bp.LineNumber=lii.LineNumber then (select min (LineAvailableCapacity) from [BatteryAssemblyLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber)
            else bp.LineAvailableCapacity end) as LineAvailableCapacity

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyLineCapacityDetails] bp 
            left join [CellPhoneProject].[dbo].LineInformation lii on bp.LineInformation_Id=lii.Id
            where bp.WorkingDate='{0}'
            and lii.ProjectType='{1}' 
            order by bp.WorkingDate asc",
             assembStartDateBAssembly, qry.ProjectType);
            var exe = _dbEntities.Database.SqlQuery<BatteryAssemblyLineCapacityDetailModel>(query).ToList();
            return exe;
        }


        public List<BatteryAssemblyLineCapacityDetailModel> GetAvailableProductionLineForBatteryAssembly(string assembStartDateBAssembly, string assembEndDateBAssembly, long proIds)
        {
            var qry = (from c in _dbEntities.ProjectMasters
                       where c.ProjectMasterId == proIds
                       select c).FirstOrDefault();

            string query = string.Format(@"SELECT distinct bp.LineNumber,lii.ProjectType, bp.Production,bp.WorkingDate,(case when bp.LineNumber=lii.LineNumber then (select sum (PerDayCapacity) from [BatteryAssemblyLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber) 
             else bp.PerDayCapacity end) as PerDayCapacity,bp.LineCapacity
            ,(case when bp.LineNumber=lii.LineNumber then (select min (LineAvailableCapacity) from [BatteryAssemblyLineCapacityDetails])
            else bp.LineAvailableCapacity end) as LineAvailableCapacity

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyLineCapacityDetails] bp 
            left join [CellPhoneProject].[dbo].LineInformation lii on bp.LineInformation_Id=lii.Id
            where bp.WorkingDate between '{0}'  and '{1}' 
            and lii.ProjectType='{2}' 
            order by bp.WorkingDate asc",
             assembStartDateBAssembly, assembEndDateBAssembly, qry.ProjectType);
            var exe = _dbEntities.Database.SqlQuery<BatteryAssemblyLineCapacityDetailModel>(query).ToList();
            return exe;
        }

        public List<BatteryPackingLineCapacityDetailModel> GetAvailableProductionLineForBatteryPacking(string packingMassProductionStartDateBAssembly,
            string packingMassProductionEndDateBAssembly, long proIds)
        {

            var qry = (from c in _dbEntities.ProjectMasters
                       where c.ProjectMasterId == proIds
                       select c).FirstOrDefault();

            string query = string.Format(@"SELECT distinct bp.LineNumber,lii.ProjectType, bp.Production, bp.WorkingDate,(case when bp.LineNumber=lii.LineNumber then 
            (select sum (PerDayCapacity) from [BatteryPackingLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber) 
            else bp.PerDayCapacity end) as PerDayCapacity,bp.LineCapacity
            ,(case when bp.LineNumber=lii.LineNumber then (select min (LineAvailableCapacity) from [BatteryPackingLineCapacityDetails])
            else bp.LineAvailableCapacity end) as LineAvailableCapacity
            FROM [CellPhoneProject].[dbo].[BatteryPackingLineCapacityDetails] bp 
            left join [CellPhoneProject].[dbo].LineInformation lii on bp.LineInformation_Id=lii.Id
            where bp.WorkingDate between '{0}'  and '{1}' 
            and lii.ProjectType='{2}' 
            order by bp.WorkingDate asc",
          packingMassProductionStartDateBAssembly, packingMassProductionEndDateBAssembly, qry.ProjectType);
            var exe = _dbEntities.Database.SqlQuery<BatteryPackingLineCapacityDetailModel>(query).ToList();
            return exe;
        }

        public List<BatteryLineCapacityDetailModel> GetAvailableProductionLineForBattery(string batteryMassProductionStartDate, string batteryMassProductionEndDate)
        {
            string query = string.Format(@"SELECT distinct bp.LineNumber,lii.ProjectType, bp.Production, bp.WorkingDate,(case when bp.LineNumber=lii.LineNumber then
            (select sum (PerDayCapacity) from [BatteryLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber) 
            else bp.PerDayCapacity end) as PerDayCapacity,bp.LineCapacity
            ,(case when bp.LineNumber=lii.LineNumber then (select min (LineAvailableCapacity) from [BatteryLineCapacityDetails])
            else bp.LineAvailableCapacity end) as LineAvailableCapacity
            FROM [CellPhoneProject].[dbo].[BatteryLineCapacityDetails] bp 
            left join [CellPhoneProject].[dbo].LineInformation lii on bp.LineInformation_Id=lii.Id
            where bp.WorkingDate between '{0}'  and '{1}'            
            order by bp.WorkingDate asc",
            batteryMassProductionStartDate, batteryMassProductionEndDate);
            var exe = _dbEntities.Database.SqlQuery<BatteryLineCapacityDetailModel>(query).ToList();
            return exe;
        }

        public List<BatteryHousingLineCapacityDetailModel> GetAvailableProductionLineForBHousing(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            string query = string.Format(@"SELECT distinct bp.LineNumber,lii.ProjectType, bp.Production, bp.WorkingDate,(case when bp.LineNumber=lii.LineNumber then
            (select sum (PerDayCapacity) from [BatteryHousingLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber) 
            else bp.PerDayCapacity end) as PerDayCapacity,bp.LineCapacity
            ,(case when bp.LineNumber=lii.LineNumber then (select min (LineAvailableCapacity) from [BatteryHousingLineCapacityDetails])
            else bp.LineAvailableCapacity end) as LineAvailableCapacity
            FROM [CellPhoneProject].[dbo].[BatteryHousingLineCapacityDetails] bp 
            left join [CellPhoneProject].[dbo].LineInformation lii on bp.LineInformation_Id=lii.Id
            where bp.WorkingDate between '{0}'  and '{1}'            
            order by bp.WorkingDate asc",
            housingMassProStartDateHousing, housingMassProEndtDateHousing);
            var exe = _dbEntities.Database.SqlQuery<BatteryHousingLineCapacityDetailModel>(query).ToList();
            return exe;
        }

        public List<BatterySMTLineCapacityDetailModel> GetAvailableProductionLineForBSmt(string massProductionStartDateBSmt, string massProductionEndDateBSmt, long proIds)
        {
            var qry = (from c in _dbEntities.ProjectMasters
                       where c.ProjectMasterId == proIds
                       select c).FirstOrDefault();

            string query = string.Format(@"SELECT distinct bp.LineNumber,lii.ProjectType, bp.Production, bp.WorkingDate,(case when bp.LineNumber=lii.LineNumber then 
            (select sum (PerDayCapacity) from [BatterySMTLineCapacityDetails] where WorkingDate=bp.WorkingDate and Linenumber=bp.LineNumber) 
            else bp.PerDayCapacity end) as PerDayCapacity,bp.LineCapacity
            ,(case when bp.LineNumber=lii.LineNumber then (select min (LineAvailableCapacity) from [BatterySMTLineCapacityDetails])
            else bp.LineAvailableCapacity end) as LineAvailableCapacity
            FROM [CellPhoneProject].[dbo].[BatterySMTLineCapacityDetails] bp 
            left join [CellPhoneProject].[dbo].LineInformation lii on bp.LineInformation_Id=lii.Id
            where bp.WorkingDate between '{0}'  and '{1}' 
            and lii.ProjectType='{2}' 
            order by bp.WorkingDate asc",
            massProductionStartDateBSmt, massProductionEndDateBSmt, qry.ProjectType);
            var exe = _dbEntities.Database.SqlQuery<BatterySMTLineCapacityDetailModel>(query).ToList();
            return exe;
        }

        public bool GetMaterialReceiveDateBAssembly(string materialStartDateBAssembly, string materialReceiveEndDateBAssembly)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"  select [MaterialReceiveStartDateBAssembly],[MaterialReceiveEndDateBAssembly]
            from [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking]
            where [MaterialReceiveStartDateBAssembly] between '{0}'  and '{1}'
            or [MaterialReceiveEndDateBAssembly]  between '{0}'  and '{1}'", materialStartDateBAssembly, materialReceiveEndDateBAssembly);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteDateBAssembly(string iqcCompleteStartDateBAssembly, string iqcCompleteEndDateBAssembly)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"  select [IqcCompleteStartDateBAssembly],[IqcCompleteEndDateBAssembly]
            from [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking]
            where [IqcCompleteStartDateBAssembly] between '{0}'  and '{1}'
            or [IqcCompleteEndDateBAssembly]  between '{0}'  and '{1}'", iqcCompleteStartDateBAssembly, iqcCompleteEndDateBAssembly);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetAssemblyBProduction(string assembStartDateBAssembly, string assembEndDateBAssembly)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [AssemblyMassProductionStartDateBAssembly],[AssemblyMassProductionEndDateBAssembly]
            from [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking]
            where [AssemblyMassProductionStartDateBAssembly] between '{0}'  and '{1}'
            or [AssemblyMassProductionEndDateBAssembly] between '{0}'  and '{1}'", assembStartDateBAssembly, assembEndDateBAssembly);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialProductionDateBAssembly(string trialProductionStartDateBAssembly, string trialProductionEndDateBAssembly)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [TrialProductionStartDateBAssembly],[TrialProductionEndDateBAssembly]
            from [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking]
            where [TrialProductionStartDateBAssembly] between '{0}'  and '{1}'
            or [TrialProductionEndDateBAssembly] between '{0}'  and '{1}'", trialProductionStartDateBAssembly, trialProductionEndDateBAssembly);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetPackingBProduction(string packingMassProductionStartDateBAssembly, string packingMassProductionEndDateBAssembly)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [PackingMassProductionStartDateBAssembly],[PackingMassProductionEndDateBAssembly]
            from [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking]
            where [PackingMassProductionStartDateBAssembly] between '{0}'  and '{1}'
            or [PackingMassProductionEndDateBAssembly] between '{0}'  and '{1}'", packingMassProductionStartDateBAssembly, packingMassProductionEndDateBAssembly);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetBatteryMassProduction(string batteryMassProductionStartDate, string batteryMassProductionEndDate)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [BatteryMassProductionStartDate],[BatteryMassProductionEndDate]
            from [CellPhoneProject].[dbo].[Battery]
            where [BatteryMassProductionStartDate] between '{0}'  and '{1}'
            or [BatteryMassProductionEndDate] between '{0}' and '{1}'", batteryMassProductionStartDate, batteryMassProductionEndDate);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialProductionDateBattery(string trialProductionStartDateBattery, string trialProductionEndDateBattery)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [TrialProductionStartDateBattery],[TrialProductionEndDateBattery]
            from [CellPhoneProject].[dbo].[Battery]
            where [TrialProductionStartDateBattery] between '{0}' and '{1}'
            or [TrialProductionEndDateBattery] between '{0}' and '{1}'", trialProductionStartDateBattery, trialProductionEndDateBattery);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteDateBattery(string iqcCompleteStartDateBattery, string iqcCompleteEndDateBattery)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [IqcCompleteStartDateBattery],[IqcCompleteEndDateBattery]
            from [CellPhoneProject].[dbo].[Battery]
            where [IqcCompleteStartDateBattery] between '{0}' and '{1}'
            or [IqcCompleteEndDateBattery] between '{0}' and '{1}'", iqcCompleteStartDateBattery, iqcCompleteEndDateBattery);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetMaterialReceiveDateBattery(string materialReceiveStartDateBattery, string materialReceiveEndDateBattery)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select MaterialReceiveStartDateBattery,MaterialReceiveEndDateBattery
            from [CellPhoneProject].[dbo].[Battery]
            where MaterialReceiveStartDateBattery between '{0}' and '{1}'
            or MaterialReceiveEndDateBattery between '{0}' and '{1}'", materialReceiveStartDateBattery, materialReceiveEndDateBattery);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetHousingMassBProduction(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [HousingMassProductionStartDateBHousing],[HousingMassProductionEndDateBHousing]
            from [CellPhoneProject].[dbo].[BatteryHousing]
            where [HousingMassProductionStartDateBHousing] between '{0}' and '{1}'
            or [HousingMassProductionEndDateBHousing] between '{0}' and '{1}'", housingMassProStartDateHousing, housingMassProEndtDateHousing);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialBProduction(string trialProductionStartDateHousing, string trialProductionEndDateHousing)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [TrialProductionStartDateBHousing],[TrialProductionEndDateBHousing]
            from [CellPhoneProject].[dbo].[BatteryHousing]
            where [TrialProductionStartDateBHousing] between '{0}' and '{1}'
            or [TrialProductionEndDateBHousing] between '{0}' and '{1}'", trialProductionStartDateHousing, trialProductionEndDateHousing);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteDateBHousing(string iqcCompleteStartDateHousing, string iqcCompleteEndDateHousing)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [IqcCompleteStartDateBHousing],[IqcCompleteEndDateBHousing]
            from [CellPhoneProject].[dbo].[BatteryHousing]
            where [IqcCompleteStartDateBHousing] between '{0}' and '{1}'
            or [IqcCompleteEndDateBHousing] between '{0}' and '{1}'", iqcCompleteStartDateHousing, iqcCompleteEndDateHousing);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetMaterialReceiveDateBHousing(string materialReceiveStartDateHousing, string materialReceiveEndDateHousing)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [MaterialReceiveStartDateBHousing],[MaterialReceiveEndDateBHousing]
            from [CellPhoneProject].[dbo].[BatteryHousing]
            where [MaterialReceiveStartDateBHousing] between '{0}' and '{1}'
            or [MaterialReceiveEndDateBHousing] between '{0}' and '{1}'", materialReceiveStartDateHousing, materialReceiveEndDateHousing);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetSmtMassBProduction(string massProductionStartDateBSmt, string massProductionEndDateBSmt)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [SmtMassProductionStartDateBSmt],[SmtMassProductionEndDateBSmt]
            from [CellPhoneProject].[dbo].[BatterySMT]
            where [SmtMassProductionStartDateBSmt] between '{0}' and '{1}'
            or [SmtMassProductionEndDateBSmt] between '{0}' and '{1}'", massProductionStartDateBSmt, massProductionEndDateBSmt);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetMaterialReceiveDateBSmt(string materialReceiveStartDateBSmt, string materialReceiveEndDateBSmt)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [MaterialReceiveStartDateBSmt],[MaterialReceiveEndDateBSmt]
            from [CellPhoneProject].[dbo].[BatterySMT]
            where [MaterialReceiveStartDateBSmt] between '{0}' and '{1}'
            or [MaterialReceiveEndDateBSmt] between '{0}' and '{1}'", materialReceiveStartDateBSmt, materialReceiveEndDateBSmt);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetIqcCompleteDateBSmt(string iqcCompleteStartDateBSmt, string iqcCompleteEndDateBSmt)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [IqcCompleteStartDateBSmt],[IqcCompleteEndDateBSmt]
            from [CellPhoneProject].[dbo].[BatterySMT]
            where [IqcCompleteStartDateBSmt] between '{0}' and '{1}'
            or [IqcCompleteEndDateBSmt] between '{0}' and '{1}'", iqcCompleteStartDateBSmt, iqcCompleteEndDateBSmt);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetTrialProductionDateBSmt(string trialProductionStartDateBSmt, string trialProductionEndDateBSmt)
        {
            List<CustomBatteryProduction> battery = null;

            string proEv = string.Format(@"select [TrialProductionStartDateBSmt],[TrialProductionEndDateBSmt]
            from [CellPhoneProject].[dbo].[BatterySMT]
            where [TrialProductionStartDateBSmt] between '{0}' and '{1}'
            or [TrialProductionEndDateBSmt] between '{0}' and '{1}'", trialProductionStartDateBSmt, trialProductionEndDateBSmt);
            battery =
                   _dbEntities.Database.SqlQuery<CustomBatteryProduction>(proEv).ToList();

            if (battery.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveBatteryPlanData(List<CustomBatteryProduction> results)
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

                var proPlan = new ProPlanTable();
                proPlan.AddadDate = DateTime.Now;
                proPlan.ProjectId = insResult.ProjectId;
                proPlan.IsCharger = false;
                proPlan.IsCkd = true;
                proPlan.IsActive = true;
                _dbEntities.ProPlanTables.Add(proPlan);
                _dbEntities.SaveChanges();

                var queryForPlan = (from c in _dbEntities.ProPlanTables
                                    where c.ProjectId == insResult.ProjectId && c.IsCkd == true
                                    select c).OrderByDescending(x => x.PlanId).FirstOrDefault();

                if (queryForPlan != null)
                {


                    if (insResult.AssemblyChk == true)
                    {
                        //////////
                        var model3 = new BatteryAssemblyAndPacking();
                        model3.PlanId = queryForPlan.PlanId;
                        model3.ProjectId = insResult.ProjectId;
                        model3.ProjectName = insResult.ProjectName;
                        model3.OrderNumber = query1.OrderNuber;
                        model3.PoCategory = query2.PoCategory;
                        model3.MaterialReceiveStartDateBAssembly = insResult.MaterialReceiveStartDateBAssembly;
                        model3.MaterialReceiveEndDateBAssembly = insResult.MaterialReceiveEndDateBAssembly;
                        model3.IqcCompleteStartDateBAssembly = insResult.IqcCompleteStartDateBAssembly;
                        model3.IqcCompleteEndDateBAssembly = insResult.IqcCompleteEndDateBAssembly;
                        model3.TrialProductionStartDateBAssembly = insResult.TrialProductionStartDateBAssembly;
                        model3.TrialProductionEndDateBAssembly = insResult.TrialProductionEndDateBAssembly;
                        model3.SoftwareConfirmationStartDateBAssembly = insResult.SoftwareConfirmationStartDateBAssembly;
                        model3.SoftwareConfirmationEndDateBAssembly = insResult.SoftwareConfirmationEndDateBAssembly;
                        model3.RandDConfirmationStartDateBAssembly = insResult.RandDConfirmationStartDateBAssembly;
                        model3.RandDConfirmationEndDateBAssembly = insResult.RandDConfirmationEndDateBAssembly;
                        model3.AssemblyMassProductionStartDateBAssembly = insResult.AssemblyMassProductionStartDateBAssembly;
                        model3.AssemblyMassProductionEndDateBAssembly = insResult.AssemblyMassProductionEndDateBAssembly;
                        model3.TotalQuantityBAssembly = insResult.BatteryAssemblyTotalQuantity;

                        model3.PackingMassProductionStartDateBAssembly = insResult.PackingMassProductionStartDateBAssembly;
                        model3.PackingMassProductionEndDateBAssembly = insResult.PackingMassProductionEndDateBAssembly;
                        model3.TotalQuantityBPacking = insResult.BatteryPackingTotalQuantity;
                        model3.StatusAssembAndPack = "PARTIAL";
                        model3.Added = userId;
                        model3.AddedDate = DateTime.Now;
                        model3.IsActive = true;
                        _dbEntities.BatteryAssemblyAndPackings.Add(model3);
                        _dbEntities.SaveChanges();
                        ///////
                        var query111 = (from c in _dbEntities.BatteryAssemblyAndPackings
                                        where c.ProjectId == insResult.ProjectId
                                        select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exe1 = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();


                        var datesSmt11 = GetDatesBetween(Convert.ToDateTime(insResult.AssemblyMassProductionStartDateBAssembly), Convert.ToDateTime(insResult.AssemblyMassProductionEndDateBAssembly));

                        var datesSmt12 = datesSmt11.Except(exe1);


                        var lineInfo1 = insResult.AssemblyAllLineNumber;
                        var newstring1 = "";

                        for (int i = 0; i < lineInfo1.Length; i++)
                        {
                            if (char.IsUpper(lineInfo1[i]))
                                newstring1 += " ";
                            newstring1 += lineInfo1[i].ToString();
                        }

                        var stroreString1 = newstring1;

                        var stroreStringSplit1 = stroreString1.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit1)
                        {

                            var model11 = new BatteryAssemblyLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            //var query12 = (from c in _dbEntities.LineInformations
                            //               where c.LineNumber == model11.LineNumber && c.CKD_Assembly_Line_Capacity > 0
                            //               select c).FirstOrDefault();
                            var query12 = (from c in _dbEntities.LineInformations
                                           join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                           where c.LineNumber == model11.LineNumber && c.CKD_Assembly_Line_Capacity > 0 && pm.ProjectMasterId == insResult.ProjectId
                                           select c).FirstOrDefault();

                            if (model11.LineNumber != "")
                            {
                                long abc1 = 0;
                                foreach (var smtDate in datesSmt12)
                                {
                                    //var query13 = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                    //               where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                    //               select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    var query13 = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                                   join cc in _dbEntities.LineInformations on c.LineInformation_Id equals cc.Id
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate && cc.Id == query12.Id
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();
                                    model11.BatteryAssemblyId = query111.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.PerDayCapacity = insResult.BatteryAssemblyPerDayCapacity;
                                    model11.LineCapacity = query12.CKD_Assembly_Line_Capacity;
                                    // model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerAssemblyPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity);
                                    }
                                    if (model11.LineAvailableCapacity < 0)
                                    {
                                        model11.LineAvailableCapacity = 0;
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.TotalQuantityBAssembly = insResult.BatteryAssemblyTotalQuantity;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.BatteryAssemblyLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                    var remainder1 = model11.TotalQuantityBAssembly % model11.PerDayCapacity;

                                    if (smtDate == datesSmt12.Max() && remainder1 > 0)
                                    {
                                        if (query13 != null)
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                            Convert.ToInt64(remainder1);
                                        }
                                        else
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(remainder1);
                                        }
                                        if (model11.LineAvailableCapacity < 0)
                                        {
                                            model11.LineAvailableCapacity = 0;
                                        }
                                        _dbEntities.SaveChanges();
                                    }

                                    #region com
                                    //start new//
                                    //if (query13 != null)
                                    //{

                                    //    if (query13.LineAvailableCapacity < insResult.BatteryAssemblyPerDayCapacity)
                                    //    {
                                    //        model11.Production = query13.LineAvailableCapacity;
                                    //        model11.LineAvailableCapacity =0;


                                    //    }
                                    //    else
                                    //    {

                                    //        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                    //                                   Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity);
                                    //        model11.Production = Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity);
                                    //    }

                                    //}
                                    //else
                                    //{
                                    //    if (Convert.ToInt64(model11.LineCapacity) <
                                    //        Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity))
                                    //    {
                                    //        model11.LineAvailableCapacity = 0;
                                    //        model11.Production = Convert.ToInt64(model11.LineCapacity);
                                    //    }
                                    //    else
                                    //    {
                                    //        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity);
                                    //        model11.Production = Convert.ToInt64(insResult.BatteryAssemblyPerDayCapacity);
                                    //    }
                                    //}
                                    //if (model11.LineAvailableCapacity < 0)
                                    //{
                                    //    model11.LineAvailableCapacity = 0;
                                    //}
                                    //model11.LineInformation_Id = query12.Id;
                                    //model11.TotalQuantityBAssembly = insResult.BatteryAssemblyTotalQuantity;
                                    //model11.LineNumber = spVal;
                                    //model11.AddedDate = DateTime.Now;
                                    //model11.Added = userId;

                                    //abc1 += Convert.ToInt64(model11.Production);

                                    //if (model11.PerDayCapacity > model11.LineCapacity)
                                    //{
                                    //    if (smtDate == datesSmt12.Max())
                                    //    {
                                    //        if (abc1 < insResult.BatteryAssemblyTotalQuantity)
                                    //        {
                                    //            model11.Production = insResult.BatteryAssemblyTotalQuantity - abc1;
                                    //            model11.LineAvailableCapacity = model11.LineCapacity - model11.Production;
                                    //        }
                                    //        else
                                    //        {
                                    //            model11.Production = abc1 - insResult.BatteryAssemblyTotalQuantity;
                                    //            model11.LineAvailableCapacity = model11.LineCapacity - model11.Production;
                                    //        }

                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    if (smtDate == datesSmt12.Max())
                                    //    {
                                    //        if (abc1 < insResult.BatteryAssemblyTotalQuantity)
                                    //        {
                                    //              model11.Production = insResult.BatteryAssemblyTotalQuantity - abc1;
                                    //              model11.LineAvailableCapacity = model11.LineCapacity - model11.Production;
                                    //        }
                                    //        else
                                    //        {
                                    //            model11.Production = abc1 -insResult.BatteryAssemblyTotalQuantity;
                                    //            model11.LineAvailableCapacity = model11.LineCapacity - model11.Production;
                                    //        }
                                    //    }
                                    //}
                                    //_dbEntities.BatteryAssemblyLineCapacityDetails.Add(model11);
                                    //_dbEntities.SaveChanges();
                                    //end new//


                                    //if (model11.PerDayCapacity > model11.LineCapacity)
                                    //{
                                    //    //var remainder1 = model11.TotalQuantityBAssembly % model11.LineCapacity;

                                    //    //if (smtDate == datesSmt12.Max() && remainder1 > 0)
                                    //    //{
                                    //    //    if (query13 != null)
                                    //    //    {
                                    //    //        if (Convert.ToInt64(query13.LineAvailableCapacity) >
                                    //    //            Convert.ToInt64(remainder1))
                                    //    //        {
                                    //    //            model11.LineAvailableCapacity =
                                    //    //                Convert.ToInt64(query13.LineAvailableCapacity) -
                                    //    //                Convert.ToInt64(remainder1);
                                    //    //            model11.Production = Convert.ToInt64(remainder1);

                                    //    //        }
                                    //    //        else 
                                    //    //        {
                                    //    //            model11.LineAvailableCapacity = Convert.ToInt64(remainder1) -
                                    //    //                                            Convert.ToInt64(
                                    //    //                                                query13.LineAvailableCapacity);
                                    //    //            model11.Production =
                                    //    //                Convert.ToInt64(query13.LineAvailableCapacity);
                                    //    //        }
                                    //    //    }
                                    //    //    else
                                    //    //    {
                                    //    //        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) -
                                    //    //                                        Convert.ToInt64(remainder1);
                                    //    //        model11.Production = Convert.ToInt64(remainder1);
                                    //    //    }

                                    //    //    if (model11.LineAvailableCapacity < 0)
                                    //    //    {
                                    //    //        model11.LineAvailableCapacity = 0;
                                    //    //    }
                                    //    //    _dbEntities.SaveChanges();
                                    //    //}

                                    //    var query131 = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                    //                   join cc in _dbEntities.LineInformations on c.LineInformation_Id equals cc.Id
                                    //                   where c.LineNumber == model11.LineNumber &&
                                    //                   //c.WorkingDate == smtDate

                                    //                   (c.WorkingDate >= insResult.AssemblyMassProductionStartDateBAssembly && c.WorkingDate <= insResult.AssemblyMassProductionEndDateBAssembly)
                                    //                   && cc.Id == query12.Id
                                    //                   select c).OrderByDescending(x => x.Id).ToList();

                                    //    var remainder1 = model11.TotalQuantityBAssembly - query131.Sum(x=>x.Production);

                                    //    if (smtDate == datesSmt12.Max() && remainder1 < 0)
                                    //    {
                                    //        if (query13 != null)
                                    //        {
                                    //            model11.Production = query131.Sum(x => x.Production) -
                                    //                                 Convert.ToInt64(model11.TotalQuantityBAssembly);

                                    //            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) + Convert.ToInt64(model11.Production);
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    var query131 = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                    //                    join cc in _dbEntities.LineInformations on c.LineInformation_Id equals cc.Id
                                    //                    where c.LineNumber == model11.LineNumber &&
                                    //                        //c.WorkingDate == smtDate

                                    //                    (c.WorkingDate >= insResult.AssemblyMassProductionStartDateBAssembly && c.WorkingDate <= insResult.AssemblyMassProductionEndDateBAssembly)
                                    //                    && cc.Id == query12.Id
                                    //                    select c).OrderByDescending(x => x.Id).ToList();

                                    //    var remainder1 = model11.TotalQuantityBAssembly - query131.Sum(x => x.Production);

                                    //    if (smtDate == datesSmt12.Max() && remainder1 < 0)
                                    //    {
                                    //        if (query13 != null)
                                    //        {
                                    //            model11.Production = query131.Sum(x => x.Production) -
                                    //                                 Convert.ToInt64(model11.TotalQuantityBAssembly);

                                    //            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) + Convert.ToInt64(model11.Production);
                                    //        }
                                    //    }

                                    //    //var remainder1 = model11.TotalQuantityBAssembly % model11.PerDayCapacity;

                                    //    //if (smtDate == datesSmt12.Max() && remainder1 > 0)
                                    //    //{
                                    //    //    if (query13 != null)
                                    //    //    {
                                    //    //        if (Convert.ToInt64(query13.LineAvailableCapacity) > Convert.ToInt64(remainder1))
                                    //    //        {
                                    //    //            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) - Convert.ToInt64(remainder1);
                                    //    //            model11.Production = Convert.ToInt64(remainder1);

                                    //    //        }
                                    //    //        else if (Convert.ToInt64(query13.LineAvailableCapacity) < Convert.ToInt64(remainder1))
                                    //    //        {
                                    //    //            model11.LineAvailableCapacity = Convert.ToInt64(remainder1) - Convert.ToInt64(query13.LineAvailableCapacity);
                                    //    //            model11.Production = Convert.ToInt64(query13.LineAvailableCapacity);
                                    //    //        }
                                    //    //    }
                                    //    //    else
                                    //    //    {
                                    //    //        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(remainder1);
                                    //    //        model11.Production = Convert.ToInt64(remainder1);
                                    //    //    }

                                    //    //    if (model11.LineAvailableCapacity < 0)
                                    //    //    {
                                    //    //        model11.LineAvailableCapacity = 0;
                                    //    //    }
                                    //    //    _dbEntities.SaveChanges();
                                    //    //}
                                    //}
                                    #endregion
                                }
                            }//end of line if
                        }//end of foreach
                        ////////Trial line Start Assembly////////////////////////

                        var datesAssemblyTrial1 = GetDatesBetween(Convert.ToDateTime(insResult.TrialProductionStartDateBAssembly), Convert.ToDateTime(insResult.TrialProductionEndDateBAssembly));
                        var datesAssemblyTrial2 = datesAssemblyTrial1.Except(exe1);



                        var lineInfo2 = insResult.AssemblyTrialLine;
                        var newstring2 = "";

                        for (int i = 0; i < lineInfo2.Length; i++)
                        {
                            if (char.IsUpper(lineInfo2[i]))
                                newstring2 += " ";
                            newstring2 += lineInfo2[i].ToString();
                        }

                        var stroreString2 = newstring2;

                        var stroreStringSplit2 = stroreString2.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit2)
                        {

                            var model11 = new AllTrialInfo();

                            model11.TrialLineNumber = spVal.Trim();

                            if (model11.TrialLineNumber != "")
                            {
                                foreach (var smtDate in datesAssemblyTrial2)
                                {

                                    model11.AssemblyTrialId = query111.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.TrialLineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;
                                    _dbEntities.AllTrialInfoes.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                        //////Trial line end/////////////////////////
                        ///////
                        var queryPac = (from c in _dbEntities.BatteryAssemblyAndPackings
                                        where c.ProjectId == insResult.ProjectId
                                        select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exePac = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmtPac1 = GetDatesBetween(Convert.ToDateTime(insResult.PackingMassProductionStartDateBAssembly), Convert.ToDateTime(insResult.PackingMassProductionEndDateBAssembly));

                        var datesSmtPac2 = datesSmtPac1.Except(exePac);


                        var lineInfoPac = insResult.PackingAllLineNumber;
                        var newstringPac = "";

                        for (int i = 0; i < lineInfoPac.Length; i++)
                        {
                            if (char.IsUpper(lineInfoPac[i]))
                                newstringPac += " ";
                            newstringPac += lineInfoPac[i].ToString();
                        }

                        var stroreStringPac = newstringPac;

                        var stroreStringSplitPac = stroreStringPac.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplitPac)
                        {

                            var model11 = new BatteryPackingLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            var query12 = (from c in _dbEntities.LineInformations
                                           join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                           where c.LineNumber == model11.LineNumber && c.CKD_Packing_Line_Capacity > 0 && pm.ProjectMasterId == insResult.ProjectId
                                           select c).FirstOrDefault();
                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmtPac2)
                                {
                                    var query13 = (from c in _dbEntities.BatteryPackingLineCapacityDetails
                                                   join cc in _dbEntities.LineInformations on c.LineInformation_Id equals cc.Id
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate && cc.Id == query12.Id
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();
                                    model11.BatteryPackingId = queryPac.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.PerDayCapacity = insResult.BatteryPackingPerDayCapacity;
                                    model11.LineCapacity = query12.CKD_Packing_Line_Capacity;
                                    // model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerAssemblyPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(insResult.BatteryPackingPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.BatteryPackingPerDayCapacity);
                                    }
                                    if (model11.LineAvailableCapacity < 0)
                                    {
                                        model11.LineAvailableCapacity = 0;
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.TotalQuantityBPacking = insResult.BatteryPackingTotalQuantity;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;
                                    _dbEntities.BatteryPackingLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                    var remainder1 = model11.TotalQuantityBPacking % model11.PerDayCapacity;

                                    if (smtDate == datesSmtPac2.Max() && remainder1 > 0)
                                    {
                                        if (query13 != null)
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                            Convert.ToInt64(remainder1);
                                        }
                                        else
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(remainder1);
                                        }
                                        if (model11.LineAvailableCapacity < 0)
                                        {
                                            model11.LineAvailableCapacity = 0;
                                        }
                                        _dbEntities.SaveChanges();
                                    }
                                }
                            }//end of line if
                        }//end of foreach


                    }//END OF ASSEMBLY

                    if (insResult.SmtChk == true)
                    {
                        var model1 = new BatterySMT();
                        model1.PlanId = queryForPlan.PlanId;
                        model1.ProjectId = insResult.ProjectId;
                        model1.ProjectName = insResult.ProjectName;
                        model1.OrderNumber = query1.OrderNuber;
                        model1.PoCategory = query2.PoCategory;
                        model1.MaterialReceiveStartDateBSmt = insResult.MaterialReceiveStartDateBSmt;
                        model1.MaterialReceiveEndDateBSmt = insResult.MaterialReceiveEndDateBSmt;
                        model1.IqcCompleteStartDateBSmt = insResult.IqcCompleteStartDateBSmt;
                        model1.IqcCompleteEndDateBSmt = insResult.IqcCompleteEndDateBSmt;
                        model1.TrialProductionStartDateBSmt = insResult.TrialProductionStartDateBSmt;
                        model1.TrialProductionEndDateBSmt = insResult.TrialProductionEndDateBSmt;
                        model1.SmtMassProductionStartDateBSmt = insResult.SmtMassProductionStartDateBSmt;
                        model1.SmtMassProductionEndDateBSmt = insResult.SmtMassProductionEndDateBSmt;
                        model1.TotalQuantityBSmt = insResult.BatterySmtTotalQuantity;
                        model1.StatusBSmt = "PARTIAL";
                        model1.Added = userId;
                        model1.AddedDate = DateTime.Now;
                        model1.IsActive = true;

                        _dbEntities.BatterySMTs.Add(model1);
                        _dbEntities.SaveChanges();
                        ////////////////////////

                        var query11 = (from c in _dbEntities.BatterySMTs
                                       where c.ProjectId == insResult.ProjectId
                                       select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        //  var exe = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmt = GetDatesBetween(Convert.ToDateTime(insResult.SmtMassProductionStartDateBSmt), Convert.ToDateTime(insResult.SmtMassProductionEndDateBSmt));

                        //  var datesSmt1 = datesSmt.Except(exe);


                        var lineInfo = insResult.SmtAllLineNumber;
                        var newstring = "";

                        for (int i = 0; i < lineInfo.Length; i++)
                        {
                            if (char.IsUpper(lineInfo[i]))
                                newstring += " ";
                            newstring += lineInfo[i].ToString();
                        }

                        var stroreString = newstring;

                        var stroreStringSplit = stroreString.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit)
                        {

                            var model11 = new BatterySMTLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();

                            var query12 = (from c in _dbEntities.LineInformations
                                           join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                           where c.LineNumber == model11.LineNumber && c.CKD_SMT_Line_Capacity > 0 && pm.ProjectMasterId == insResult.ProjectId
                                           select c).FirstOrDefault();

                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmt)
                                {
                                    //var query13 = (from c in _dbEntities.BatterySMTLineCapacityDetails
                                    //               where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                    //               select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    var query13 = (from c in _dbEntities.BatterySMTLineCapacityDetails
                                                   join cc in _dbEntities.LineInformations on c.LineInformation_Id equals cc.Id
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate && cc.Id == query12.Id
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.BatterySMT_Id = query11.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.PerDayCapacity = insResult.BatterySmtPerDayCapacity;
                                    model11.LineCapacity = query12.CKD_SMT_Line_Capacity;

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(insResult.BatterySmtPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.BatterySmtPerDayCapacity);
                                    }
                                    if (model11.LineAvailableCapacity < 0)
                                    {
                                        model11.LineAvailableCapacity = 0;
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.TotalQuantityBSmt = insResult.BatterySmtTotalQuantity;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.BatterySMTLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                    var remainder1 = model11.TotalQuantityBSmt % model11.PerDayCapacity;

                                    if (smtDate == datesSmt.Max() && remainder1 > 0)
                                    {
                                        if (query13 != null)
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                            Convert.ToInt64(remainder1);
                                        }
                                        else
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(remainder1);
                                        }
                                        if (model11.LineAvailableCapacity < 0)
                                        {
                                            model11.LineAvailableCapacity = 0;
                                        }
                                        _dbEntities.SaveChanges();
                                    }
                                }
                            }//end of line if
                        }//end of foreach

                        ////////Trial line Start SMT////////////////////////

                        var datesSmtTrial1 = GetDatesBetween(Convert.ToDateTime(insResult.TrialProductionStartDateBSmt), Convert.ToDateTime(insResult.TrialProductionEndDateBSmt));
                        // var datesSmtTrial2 = datesSmtTrial1.Except(exe);

                        var lineInfo2 = insResult.SmtTrialLine;
                        var newstring2 = "";

                        for (int i = 0; i < lineInfo2.Length; i++)
                        {
                            if (char.IsUpper(lineInfo2[i]))
                                newstring2 += " ";
                            newstring2 += lineInfo2[i].ToString();
                        }

                        var stroreString2 = newstring2;

                        var stroreStringSplit2 = stroreString2.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit2)
                        {

                            var model11 = new AllTrialInfo();

                            model11.TrialLineNumber = spVal.Trim();

                            if (model11.TrialLineNumber != "")
                            {
                                foreach (var smtDate in datesSmtTrial1)
                                {

                                    model11.SmtTrialId = query11.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.TrialLineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.AllTrialInfoes.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                        //////Trial line end/////////////////////////


                    }//end of if statement SMT
                    ///////////////////////////
                    if (insResult.HouseChk == true)
                    {
                        var model2 = new BatteryHousing();
                        model2.PlanId = queryForPlan.PlanId;
                        model2.ProjectId = insResult.ProjectId;
                        model2.ProjectName = insResult.ProjectName;
                        model2.OrderNumber = query1.OrderNuber;
                        model2.PoCategory = query2.PoCategory;
                        model2.MaterialReceiveStartDateBHousing = insResult.MaterialReceiveStartDateBHousing;
                        model2.MaterialReceiveEndDateBHousing = insResult.MaterialReceiveEndDateBHousing;
                        model2.IqcCompleteStartDateBHousing = insResult.IqcCompleteStartDateBHousing;
                        model2.IqcCompleteEndDateBHousing = insResult.IqcCompleteEndDateBHousing;
                        model2.TrialProductionStartDateBHousing = insResult.TrialProductionStartDateBHousing;
                        model2.TrialProductionEndDateBHousing = insResult.TrialProductionEndDateBHousing;
                        model2.HousingReliabilityTestStartDateBHousing = insResult.HousingReliabilityTestStartDateBHousing;
                        model2.HousingReliabilityTestEndDateBHousing = insResult.HousingReliabilityTestEndDateBHousing;
                        model2.HousingMassProductionStartDateBHousing = insResult.HousingMassProductionStartDateBHousing;
                        model2.HousingMassProductionEndDateBHousing = insResult.HousingMassProductionEndDateBHousing;
                        model2.TotalQuantity = insResult.BatteryHousingTotalQuantity;
                        model2.StatusBHousing = "PARTIAL";
                        model2.Added = userId;
                        model2.AddedDate = DateTime.Now;
                        model2.IsActive = true;
                        _dbEntities.BatteryHousings.Add(model2);
                        _dbEntities.SaveChanges();

                        var query11 = (from c in _dbEntities.BatteryHousings
                                       where c.ProjectId == insResult.ProjectId
                                       select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exe = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmt = GetDatesBetween(Convert.ToDateTime(insResult.HousingMassProductionStartDateBHousing), Convert.ToDateTime(insResult.HousingMassProductionEndDateBHousing));

                        var datesSmt1 = datesSmt.Except(exe);

                        var lineInfo = insResult.HousingAllLineNumber;
                        var newstring = "";

                        for (int i = 0; i < lineInfo.Length; i++)
                        {
                            if (char.IsUpper(lineInfo[i]))
                                newstring += " ";
                            newstring += lineInfo[i].ToString();
                        }
                        var stroreString = newstring;

                        var stroreStringSplit = stroreString.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit)
                        {

                            var model11 = new BatteryHousingLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            var query12 = (from c in _dbEntities.LineInformations
                                           where c.LineNumber == model11.LineNumber && c.CKD_Housing_Line_Capacity > 0
                                           select c).FirstOrDefault();
                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmt1)
                                {
                                    var query13 = (from c in _dbEntities.BatteryHousingLineCapacityDetails
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    model11.BatteryHousing_Id = query11.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.PerDayCapacity = insResult.BatteryHousingPerDayCapacity;
                                    model11.LineCapacity = query12.CKD_Housing_Line_Capacity;
                                    // model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerHousingPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(
                                                                            insResult.BatteryHousingPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.BatteryHousingPerDayCapacity);
                                    }
                                    if (model11.LineAvailableCapacity < 0)
                                    {
                                        model11.LineAvailableCapacity = 0;
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.TotalQuantity = insResult.BatteryHousingTotalQuantity;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.BatteryHousingLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                    var remainder1 = model11.TotalQuantity % model11.PerDayCapacity;

                                    if (smtDate == datesSmt1.Max() && remainder1 > 0)
                                    {
                                        if (query13 != null)
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                            Convert.ToInt64(remainder1);
                                        }
                                        else
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(remainder1);
                                        }
                                        if (model11.LineAvailableCapacity < 0)
                                        {
                                            model11.LineAvailableCapacity = 0;
                                        }
                                        _dbEntities.SaveChanges();
                                    }
                                }
                            }//end of line if
                        }//end of foreach

                        ////////Trial line Start Housing////////////////////////
                        var datesHousingTrial1 = GetDatesBetween(Convert.ToDateTime(insResult.TrialProductionStartDateBHousing), Convert.ToDateTime(insResult.TrialProductionEndDateBHousing));
                        var datesHousingTrial2 = datesHousingTrial1.Except(exe);

                        var lineInfo2 = insResult.HousingTrialLine;
                        var newstring2 = "";

                        for (int i = 0; i < lineInfo2.Length; i++)
                        {
                            if (char.IsUpper(lineInfo2[i]))
                                newstring2 += " ";
                            newstring2 += lineInfo2[i].ToString();
                        }

                        var stroreString2 = newstring2;

                        var stroreStringSplit2 = stroreString2.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit2)
                        {

                            var model11 = new AllTrialInfo();
                            model11.TrialLineNumber = spVal.Trim();

                            if (model11.TrialLineNumber != "")
                            {
                                foreach (var smtDate in datesHousingTrial2)
                                {

                                    model11.HousingTrialId = query11.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.TrialLineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.AllTrialInfoes.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                        //////Trial line end/////////////////////////

                    }//end of if statement HOUSING

                    if (insResult.BatteryChk == true)
                    {
                        var model2 = new Battery();
                        model2.PlanId = queryForPlan.PlanId;
                        model2.ProjectId = insResult.ProjectId;
                        model2.ProjectName = insResult.ProjectName;
                        model2.OrderNumber = query1.OrderNuber;
                        model2.PoCategory = query2.PoCategory;
                        model2.MaterialReceiveStartDateBattery = insResult.MaterialReceiveStartDateBattery;
                        model2.MaterialReceiveEndDateBattery = insResult.MaterialReceiveEndDateBattery;
                        model2.IqcCompleteStartDateBattery = insResult.IqcCompleteStartDateBattery;
                        model2.IqcCompleteEndDateBattery = insResult.IqcCompleteEndDateBattery;
                        model2.TrialProductionStartDateBattery = insResult.TrialProductionStartDateBattery;
                        model2.TrialProductionEndDateBattery = insResult.TrialProductionEndDateBattery;
                        model2.BatteryReliabilityTestStartDate = insResult.BatteryReliabilityTestStartDate;
                        model2.BatteryReliabilityTestEndDate = insResult.BatteryReliabilityTestEndDate;
                        model2.BatteryMassProductionStartDate = insResult.BatteryMassProductionStartDate;
                        model2.BatteryMassProductionEndDate = insResult.BatteryMassProductionEndDate;

                        model2.BatteryAgingTestStartDate = insResult.BatteryAgingTestStartDate;
                        model2.BatteryAgingTestEndDate = insResult.BatteryAgingTestEndDate;
                        model2.TotalQuantityBattery = insResult.BatteryTotalQuantity;
                        model2.StatusBattery = "PARTIAL";
                        model2.Added = userId;
                        model2.AddedDate = DateTime.Now;
                        model2.IsActive = true;

                        _dbEntities.Batteries.Add(model2);
                        _dbEntities.SaveChanges();

                        var query11 = (from c in _dbEntities.Batteries
                                       where c.ProjectId == insResult.ProjectId
                                       select c).OrderByDescending(x => x.Id).FirstOrDefault();

                        var exe = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();

                        var datesSmt = GetDatesBetween(Convert.ToDateTime(insResult.BatteryMassProductionStartDate), Convert.ToDateTime(insResult.BatteryMassProductionEndDate));

                        var datesSmt1 = datesSmt.Except(exe);


                        var lineInfo = insResult.BatteryAllLineNumber;
                        var newstring = "";

                        for (int i = 0; i < lineInfo.Length; i++)
                        {
                            if (char.IsUpper(lineInfo[i]))
                                newstring += " ";
                            newstring += lineInfo[i].ToString();
                        }

                        var stroreString = newstring;

                        var stroreStringSplit = stroreString.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit)
                        {

                            var model11 = new BatteryLineCapacityDetail();

                            model11.LineNumber = spVal.Trim();
                            var query12 = (from c in _dbEntities.LineInformations
                                           where c.LineNumber == model11.LineNumber && c.CKD_Battery_Line_Capacity > 0
                                           select c).FirstOrDefault();
                            if (model11.LineNumber != "")
                            {
                                foreach (var smtDate in datesSmt1)
                                {
                                    var query13 = (from c in _dbEntities.BatteryLineCapacityDetails
                                                   where c.LineNumber == model11.LineNumber && c.WorkingDate == smtDate
                                                   select c).OrderByDescending(x => x.Id).FirstOrDefault();

                                    model11.Battery_Id = query11.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.PerDayCapacity = insResult.BatteryPerDayCapacity;
                                    model11.LineCapacity = query12.CKD_Battery_Line_Capacity;
                                    // model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.ChargerHousingPerDayCapacity);

                                    if (query13 != null)
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(
                                                                            insResult.BatteryPerDayCapacity);
                                    }
                                    else
                                    {
                                        model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(insResult.BatteryPerDayCapacity);
                                    }
                                    if (model11.LineAvailableCapacity < 0)
                                    {
                                        model11.LineAvailableCapacity = 0;
                                    }
                                    model11.LineInformation_Id = query12.Id;
                                    model11.TotalQuantityBattery = insResult.BatteryTotalQuantity;
                                    model11.LineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.BatteryLineCapacityDetails.Add(model11);
                                    _dbEntities.SaveChanges();

                                    var remainder1 = model11.TotalQuantityBattery % model11.PerDayCapacity;

                                    if (smtDate == datesSmt1.Max() && remainder1 > 0)
                                    {
                                        if (query13 != null)
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                            Convert.ToInt64(remainder1);
                                        }
                                        else
                                        {
                                            model11.LineAvailableCapacity = Convert.ToInt64(model11.LineCapacity) - Convert.ToInt64(remainder1);
                                        }
                                        if (model11.LineAvailableCapacity < 0)
                                        {
                                            model11.LineAvailableCapacity = 0;
                                        }
                                        _dbEntities.SaveChanges();
                                    }
                                }
                            }//end of line if
                        }//end of foreach


                        ////////Trial line Start Battery////////////////////////

                        var datesBatteryTrial1 = GetDatesBetween(Convert.ToDateTime(insResult.TrialProductionStartDateBattery), Convert.ToDateTime(insResult.TrialProductionEndDateBattery));
                        var datesBatteryTrial2 = datesBatteryTrial1.Except(exe);

                        var lineInfo2 = insResult.BatteryTrialLine;
                        var newstring2 = "";

                        for (int i = 0; i < lineInfo2.Length; i++)
                        {
                            if (char.IsUpper(lineInfo2[i]))
                                newstring2 += " ";
                            newstring2 += lineInfo2[i].ToString();
                        }

                        var stroreString2 = newstring2;

                        var stroreStringSplit2 = stroreString2.Split(' ').ToList();

                        foreach (var spVal in stroreStringSplit2)
                        {

                            var model11 = new AllTrialInfo();
                            model11.TrialLineNumber = spVal.Trim();

                            if (model11.TrialLineNumber != "")
                            {
                                foreach (var smtDate in datesBatteryTrial2)
                                {

                                    model11.BatteryTrialId = query11.Id;
                                    model11.PlanId = queryForPlan.PlanId;
                                    model11.WorkingDate = smtDate;
                                    model11.TrialLineNumber = spVal;
                                    model11.AddedDate = DateTime.Now;
                                    model11.Added = userId;
                                    model11.IsActive = true;

                                    _dbEntities.AllTrialInfoes.Add(model11);
                                    _dbEntities.SaveChanges();

                                }
                            }//end of line if
                        }//end of foreach
                        //////Trial line end/////////////////////////

                    }//end of if statement BATTERY
                }//end of proPlan Table
            }

            _dbEntities.SaveChanges();

            return "ok";
        }

        public List<CustomBatteryProduction> GetBatteryOldHistory(long proIds, long planId)
        {
            var query = string.Format(@"
            select top 1 asm.PlanId as AsmPlanId,bb.PlanId as BbPlanId, bh.PlanId as BhPlanId,smt.PlanId as SmtPlanId,smt.Id as SmtId,asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,asm.ProjectName,asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,
            smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,
            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,
            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing,
            CAST(smt.TotalQuantityBSmt AS VARCHAR(10)) as TotalQuantityBSmt,CAST(bh.TotalQuantity AS VARCHAR(10)) as TotalQuantity,CAST(bb.TotalQuantityBattery AS VARCHAR(10)) as TotalQuantityBattery,
            CAST(asm.TotalQuantityBAssembly AS VARCHAR(10)) as TotalQuantityBAssembly, CAST(asm.TotalQuantityBPacking AS VARCHAR(10)) as TotalQuantityBPacking

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId
            where asm.IsActive=1 and  (asm.ProjectId='{0}' and asm.planid=(select top 1 planid from CellPhoneProject.dbo.[BatteryAssemblyAndPacking] where ProjectId=asm.ProjectId and IsActive=1 order by planid desc))", proIds);

            var exe = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query).ToList();

            #region com
            // var exe = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query).ToList();
            //foreach (var custBatteryPro in exe)
            //{
            //    var qqty1 = (from als in _dbEntities.AllTrialInfoes
            //                 where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBSmt &&
            //                     als.WorkingDate <= custBatteryPro.TrialProductionEndDateBSmt && als.SmtTrialId == custBatteryPro.SmtId
            //                 select als);

            //    var trialLineNumbers = String.Join(", ", qqty1.Select(p => p.TrialLineNumber).Distinct());

            //    custBatteryPro.TrialLineNumber = trialLineNumbers;

            //    var qqty2 = (from bsct in _dbEntities.BatterySMTLineCapacityDetails
            //                 where bsct.WorkingDate >= custBatteryPro.SmtMassProductionStartDateBSmt &&
            //                     bsct.WorkingDate <= custBatteryPro.SmtMassProductionEndDateBSmt && bsct.BatterySMT_Id == custBatteryPro.SmtId
            //                 select bsct);

            //    var smtLine = String.Join(", ", qqty2.Select(p => p.LineNumber).Distinct());
            //    var smtPerDayCap = (qqty2.Select(p => p.PerDayCapacity).FirstOrDefault());
            //    var smtLineCap = (qqty2.Select(p => p.LineCapacity).FirstOrDefault());

            //    custBatteryPro.SmtAllLineNumber = smtLine;
            //    custBatteryPro.SmtAllLineCapacity = Convert.ToString(smtLineCap);
            //    custBatteryPro.BatterySmtPerDayCapacity = Convert.ToInt64(smtPerDayCap);

            //}
            #endregion
            return exe;

        }

        public List<CustomBatteryProduction> GetBatteryOldHistoryEdit(long proIds, long planIds)
        {
            //            var query = string.Format(@"
            //            select top 1 asm.PlanId as AsmPlanId,bb.PlanId as BbPlanId, bh.PlanId as BhPlanId,smt.PlanId as SmtPlanId,smt.Id as SmtId,asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,asm.ProjectName as AsmProjectName,asm.ProjectId as AsmProjectId,asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            //            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            //            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,
            //            smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            //            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,
            //            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            //            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,
            //            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            //            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing,
            //            CAST(smt.TotalQuantityBSmt AS VARCHAR(10)) as TotalQuantityBSmt,CAST(bh.TotalQuantity AS VARCHAR(10)) as TotalQuantity,CAST(bb.TotalQuantityBattery AS VARCHAR(10)) as TotalQuantityBattery,
            //            CAST(asm.TotalQuantityBAssembly AS VARCHAR(10)) as TotalQuantityBAssembly, CAST(asm.TotalQuantityBPacking AS VARCHAR(10)) as TotalQuantityBPacking
            //
            //            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            //            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            //            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            //            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId
            //            where   (asm.ProjectId='{0}' and asm.PlanId='{1}' and asm.planid=(select top 1 planid from CellPhoneProject.dbo.[BatteryAssemblyAndPacking] where ProjectId=asm.ProjectId order by planid desc))", proIds,planIds);
            var query = string.Format(@"
            select top 1 asm.PlanId as AsmPlanId,bb.PlanId as BbPlanId, bh.PlanId as BhPlanId,smt.PlanId as SmtPlanId,smt.Id as SmtId,asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,asm.ProjectName as AsmProjectName,asm.ProjectId as AsmProjectId,asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,
            smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,
            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,
            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing,
            CAST(smt.TotalQuantityBSmt AS VARCHAR(10)) as TotalQuantityBSmt,CAST(bh.TotalQuantity AS VARCHAR(10)) as TotalQuantity,CAST(bb.TotalQuantityBattery AS VARCHAR(10)) as TotalQuantityBattery,
            CAST(asm.TotalQuantityBAssembly AS VARCHAR(10)) as TotalQuantityBAssembly, CAST(asm.TotalQuantityBPacking AS VARCHAR(10)) as TotalQuantityBPacking

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId
            where asm.IsActive=1 and asm.ProjectId='{0}' and asm.PlanId='{1}' ", proIds, planIds);

            var exe = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query).ToList();

            foreach (var custBatteryPro in exe)
            {
                //Smt////////////////////
                var qqty1 = (from als in _dbEntities.AllTrialInfoes
                             where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBSmt &&
                                 als.WorkingDate <= custBatteryPro.TrialProductionEndDateBSmt && als.SmtTrialId == custBatteryPro.SmtId
                             select als);

                var trialLineNumbers = String.Join(", ", qqty1.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.SmtTrialLine = trialLineNumbers;

                var qqty2 = (from bsct in _dbEntities.BatterySMTLineCapacityDetails
                             where bsct.WorkingDate >= custBatteryPro.SmtMassProductionStartDateBSmt &&
                                 bsct.WorkingDate <= custBatteryPro.SmtMassProductionEndDateBSmt && bsct.BatterySMT_Id == custBatteryPro.SmtId
                             select bsct);

                var smtLine = String.Join(", ", qqty2.Select(p => p.LineNumber).Distinct());
                var smtPerDayCap = (qqty2.Select(p => p.PerDayCapacity).FirstOrDefault());
                var smtLineCap = (qqty2.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.SmtAllLineNumber = smtLine;
                custBatteryPro.SmtAllLineCapacity = Convert.ToString(smtLineCap);
                custBatteryPro.BatterySmtPerDayCapacity = Convert.ToInt64(smtPerDayCap);
                //emd Smt////////////
                /////Housing////////////////
                var qqty1h = (from als in _dbEntities.AllTrialInfoes
                              where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBHousing &&
                                  als.WorkingDate <= custBatteryPro.TrialProductionEndDateBHousing && als.HousingTrialId == custBatteryPro.BhId
                              select als);

                var trialLineNumbersh = String.Join(", ", qqty1h.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.HousingTrialLine = trialLineNumbersh;

                var qqty2h = (from bsct in _dbEntities.BatteryHousingLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.HousingMassProductionStartDateBHousing &&
                                 bsct.WorkingDate <= custBatteryPro.HousingMassProductionEndDateBHousing && bsct.BatteryHousing_Id == custBatteryPro.BhId
                              select bsct);

                var houLine = String.Join(", ", qqty2h.Select(p => p.LineNumber).Distinct());
                var houPerDayCap = (qqty2h.Select(p => p.PerDayCapacity).FirstOrDefault());
                var houLineCap = (qqty2h.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.HousingAllLineNumber = houLine;
                custBatteryPro.BatteryHousingPerDayCapacity = Convert.ToInt64(houPerDayCap);
                custBatteryPro.HousingAllLineCapacity = Convert.ToString(houLineCap);

                /////Battery////////////////
                var qqty1b = (from als in _dbEntities.AllTrialInfoes
                              where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBattery &&
                                  als.WorkingDate <= custBatteryPro.TrialProductionEndDateBattery && als.BatteryTrialId == custBatteryPro.BbId
                              select als);

                var trialLineNumbersb = String.Join(", ", qqty1b.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.BatteryTrialLine = trialLineNumbersb;

                var qqty2b = (from bsct in _dbEntities.BatteryLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.BatteryMassProductionStartDate &&
                                 bsct.WorkingDate <= custBatteryPro.BatteryMassProductionEndDate && bsct.Battery_Id == custBatteryPro.BbId
                              select bsct);

                var batLine = String.Join(", ", qqty2b.Select(p => p.LineNumber).Distinct());
                var batPerDayCap = (qqty2b.Select(p => p.PerDayCapacity).FirstOrDefault());
                var batLineCap = (qqty2b.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.BatteryAllLineNumber = batLine;
                custBatteryPro.BatteryPerDayCapacity = Convert.ToInt64(batPerDayCap);
                custBatteryPro.BatteryAllLineCapacity = Convert.ToString(batLineCap);

                /////Assembly////////////////
                var qqty1a = (from als in _dbEntities.AllTrialInfoes
                              where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBAssembly &&
                                  als.WorkingDate <= custBatteryPro.TrialProductionEndDateBAssembly && als.AssemblyTrialId == custBatteryPro.AsmId
                              select als);

                var trialLineNumbersa = String.Join(", ", qqty1a.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.AssemblyTrialLine = trialLineNumbersa;

                var qqty2a = (from bsct in _dbEntities.BatteryAssemblyLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.AssemblyMassProductionStartDateBAssembly &&
                              bsct.WorkingDate <= custBatteryPro.AssemblyMassProductionEndDateBAssembly && bsct.BatteryAssemblyId == custBatteryPro.AsmId
                              select bsct);

                var asmLine = String.Join(", ", qqty2a.Select(p => p.LineNumber).Distinct());
                var asmPerDayCap = (qqty2a.Select(p => p.PerDayCapacity).FirstOrDefault());
                var asmLineCap = (qqty2a.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.AssemblyAllLineNumber = asmLine;
                custBatteryPro.BatteryAssemblyPerDayCapacity = Convert.ToInt64(asmPerDayCap);
                custBatteryPro.AssemblyAllLineCapacity = Convert.ToString(asmLineCap);

                /////Packing////////////////


                var qqty2p = (from bsct in _dbEntities.BatteryPackingLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.PackingMassProductionStartDateBAssembly &&
                              bsct.WorkingDate <= custBatteryPro.PackingMassProductionEndDateBAssembly && bsct.BatteryPackingId == custBatteryPro.AsmId
                              select bsct);

                var pacLine = String.Join(", ", qqty2p.Select(p => p.LineNumber).Distinct());
                var pacPerDayCap = (qqty2p.Select(p => p.PerDayCapacity).FirstOrDefault());
                var pacLineCap = (qqty2p.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.PackingAllLineNumber = pacLine;
                custBatteryPro.BatteryPackingPerDayCapacity = Convert.ToInt64(pacPerDayCap);
                custBatteryPro.PackingAllLineCapacity = Convert.ToString(pacLineCap);

            }
            return exe;

        }

        public List<BatteryProductionViewModel> GetBatteryGrandChartDatas(List<string> results)
        {
            string query1 = string.Empty;
            query1 = string.Format(@"select  
            asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,smt.Id as SmtId,
            asm.planId as AsmPlanId,bb.planId as BbPlanId,bh.planid as BhPlanId,smt.planid as SmtPlanId,asm.ProjectName,smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,CONVERT(varchar(10), smt.TotalQuantityBSmt) as TotalQuantityBSmt,

            asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing,CONVERT(varchar(10), bh.TotalQuantity) as TotalQuantity,
            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,CONVERT(varchar(10), bb.TotalQuantityBattery) as TotalQuantityBattery,
            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,CONVERT(varchar(10), asm.TotalQuantityBAssembly) as TotalQuantityBAssembly,CONVERT(varchar(10), asm.TotalQuantityBPacking) as TotalQuantityBPacking

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId
            where (asm.planid=(select top 1 planid from CellPhoneProject.dbo.[BatteryAssemblyAndPacking] where ProjectId=asm.ProjectId order by planid desc))");
            List<CustomBatteryProduction> batteryProQuery = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query1).ToList();

            List<BatteryProductionViewModel> cmBatteryVmModels = new List<BatteryProductionViewModel>();

            cmBatteryVmModels.AddRange(results.Select(pDate => new BatteryProductionViewModel { ProductionDate = Convert.ToDateTime(pDate) }));

            foreach (var queryProduction in batteryProQuery)
            {
                var daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBSmt) -
                 Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBSmt).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBSmt));
                    bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBSmt));

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].MaterialReceiveStartDateBSmt = prodDate;
                        cmBatteryVmModels[index].MetarialReceiveBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveBSmt) ? cmBatteryVmModels[index].MetarialReceiveBSmt + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBSmt) -
                Convert.ToDateTime(queryProduction.IqcCompleteStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBSmt).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].IqcCompleteStartDateBSmt = prodDate;
                        cmBatteryVmModels[index].IqcCompleteBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBSmt) ? cmBatteryVmModels[index].IqcCompleteBSmt + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBSmt) -
               Convert.ToDateTime(queryProduction.TrialProductionStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBSmt).AddDays(i);

                    //
                    var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                                   where als.WorkingDate == prodDate && als.SmtTrialId == queryProduction.SmtId && als.PlanId == queryProduction.SmtPlanId
                                   select als);

                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                    //

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].TrialProductionStartDateBSmt = prodDate;
                        cmBatteryVmModels[index].TrialProductionBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionBSmt) ? cmBatteryVmModels[index].TrialProductionBSmt + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TrialLineNumber: " + smtMassLineNumbers;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.SmtMassProductionEndDateBSmt) -
           Convert.ToDateTime(queryProduction.SmtMassProductionStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.SmtMassProductionStartDateBSmt).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatterySMTLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatterySMT_Id == queryProduction.SmtId && als.PlanId == queryProduction.SmtPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());
                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.LineNumber));
                    //
                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null &&
                            smtMassLineNumbers == "")
                        {
                            cmBatteryVmModels[index].SmtMassProductionStartDateBSmt = prodDate;
                            cmBatteryVmModels[index].BSmtMassProduction = "Holiday";
                        }
                        else
                        {
                            cmBatteryVmModels[index].SmtMassProductionStartDateBSmt = prodDate;
                            cmBatteryVmModels[index].BSmtMassProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BSmtMassProduction) ? cmBatteryVmModels[index].BSmtMassProduction + ", " + queryProduction.ProjectName
                                + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers : queryProduction.ProjectName
                                + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers;

                        }
                    }
                }
                #region com
                ////Housing///
                //       daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing) -
                //        Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing).AddDays(i);
                //           var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].MaterialReceiveStartDateBHousing = prodDate;
                //               cmBatteryVmModels[index].MetarialReceiveBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveBHousing) ? cmBatteryVmModels[index].MetarialReceiveBHousing + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity;

                //           }
                //       }


                //       daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBHousing) -
                //         Convert.ToDateTime(queryProduction.IqcCompleteStartDateBHousing)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBHousing).AddDays(i);
                //           var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.IqcCompleteStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.IqcCompleteEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].IqcCompleteStartDateBHousing = prodDate;
                //               cmBatteryVmModels[index].IqcCompleteBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBHousing) ? cmBatteryVmModels[index].IqcCompleteBHousing + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity;

                //           }
                //       }

                //       daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBHousing) -
                //       Convert.ToDateTime(queryProduction.TrialProductionStartDateBHousing)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBHousing).AddDays(i);

                //           // + ", TrialLineNumber: " + smtMassLineNumbers
                //           var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                //                          where als.WorkingDate == prodDate && als.HousingTrialId == queryProduction.BhId && als.PlanId == queryProduction.BhPlanId
                //                          select als);

                //           var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                //           //

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].TrialProductionStartDateBHousing = prodDate;
                //               cmBatteryVmModels[index].TrialProductionBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionBHousing) ? cmBatteryVmModels[index].TrialProductionBHousing + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TrialLineNumber: " + smtMassLineNumbers;

                //           }
                //       }
                //       daysDifference = (Convert.ToDateTime(queryProduction.HousingReliabilityTestEndDateBHousing) -
                //       Convert.ToDateTime(queryProduction.HousingReliabilityTestStartDateBHousing)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.HousingReliabilityTestStartDateBHousing).AddDays(i);
                //           var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.TrialProductionStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.TrialProductionEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].HousingReliabilityTestStartDateBHousing = prodDate;
                //               cmBatteryVmModels[index].HousingBReliability = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingBReliability) ? cmBatteryVmModels[index].HousingBReliability + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity;

                //           }
                //       }

                //       daysDifference = (Convert.ToDateTime(queryProduction.HousingMassProductionEndDateBHousing) -
                //Convert.ToDateTime(queryProduction.HousingMassProductionStartDateBHousing)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.HousingMassProductionStartDateBHousing).AddDays(i);

                //           var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //
                //           var qqtySmt = (from als in _dbEntities.BatteryHousingLineCapacityDetails
                //                          where als.WorkingDate == prodDate && als.BatteryHousing_Id == queryProduction.BhId && als.PlanId == queryProduction.BhPlanId
                //                          select als);

                //           var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                //           var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());
                //           var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.LineNumber));
                //           //
                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null &&
                //                   smtMassLineNumbers == "")
                //               {
                //                   cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                //                   cmBatteryVmModels[index].HousingBMassProduction = "Holiday";
                //               }
                //               else
                //               {
                //                   cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                //                   cmBatteryVmModels[index].HousingBMassProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingBMassProduction) ? cmBatteryVmModels[index].HousingBMassProduction + ", " + queryProduction.ProjectName
                //                       + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers : queryProduction.ProjectName
                //                       + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers;

                //               }
                //           }
                //       }

                //       ////Battery///
                //       daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBattery) -
                //        Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBattery)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBattery).AddDays(i);
                //           // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].MaterialReceiveStartDateBattery = prodDate;
                //               cmBatteryVmModels[index].BatteryMetarialReceive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryMetarialReceive) ? cmBatteryVmModels[index].BatteryMetarialReceive + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;

                //           }
                //       }
                //       daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBattery) -
                //  Convert.ToDateTime(queryProduction.IqcCompleteStartDateBattery)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBattery).AddDays(i);
                //           // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].IqcCompleteStartDateBattery = prodDate;
                //               cmBatteryVmModels[index].BatteryIqcComplete = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryIqcComplete) ? cmBatteryVmModels[index].BatteryIqcComplete + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;

                //           }
                //       }

                //       daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBattery) -
                //       Convert.ToDateTime(queryProduction.TrialProductionStartDateBattery)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBattery).AddDays(i);

                //           // + ", TrialLineNumber: " + smtMassLineNumbers
                //           var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                //                          where als.WorkingDate == prodDate && als.BatteryTrialId == queryProduction.BbId && als.PlanId == queryProduction.BbPlanId
                //                          select als);

                //           var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                //           //

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].TrialProductionStartDateBattery = prodDate;
                //               cmBatteryVmModels[index].BatteryTrialProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryTrialProduction) ? cmBatteryVmModels[index].BatteryTrialProduction + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TrialLineNumber: " + smtMassLineNumbers;

                //           }
                //       }
                //       daysDifference = (Convert.ToDateTime(queryProduction.BatteryReliabilityTestEndDate) -
                //      Convert.ToDateTime(queryProduction.BatteryReliabilityTestStartDate)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.BatteryReliabilityTestStartDate).AddDays(i);
                //           // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].BatteryReliabilityTestStartDate = prodDate;
                //               cmBatteryVmModels[index].BatteryReliability = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryReliability) ? cmBatteryVmModels[index].BatteryReliability + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;

                //           }
                //       }

                //       daysDifference = (Convert.ToDateTime(queryProduction.BatteryMassProductionEndDate) -
                // Convert.ToDateTime(queryProduction.BatteryMassProductionStartDate)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.BatteryMassProductionStartDate).AddDays(i);

                //           var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //
                //           var qqtySmt = (from als in _dbEntities.BatteryLineCapacityDetails
                //                          where als.WorkingDate == prodDate && als.Battery_Id == queryProduction.BbId && als.PlanId == queryProduction.BbPlanId
                //                          select als);

                //           var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                //           var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());
                //           var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.LineNumber));
                //           //
                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null &&
                //                   smtMassLineNumbers == "")
                //               {
                //                   cmBatteryVmModels[index].BatteryMassProductionStartDate = prodDate;
                //                   cmBatteryVmModels[index].BatteryMassProduction = "Holiday";
                //               }
                //               else
                //               {
                //                   cmBatteryVmModels[index].BatteryMassProductionStartDate = prodDate;
                //                   cmBatteryVmModels[index].BatteryMassProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryMassProduction) ? cmBatteryVmModels[index].BatteryMassProduction + ", " + queryProduction.ProjectName
                //                       + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers : queryProduction.ProjectName
                //                       + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers;

                //               }
                //           }
                //       }

                //       daysDifference = (Convert.ToDateTime(queryProduction.BatteryAgingTestEndDate) -
                //       Convert.ToDateTime(queryProduction.BatteryAgingTestStartDate)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.BatteryAgingTestStartDate).AddDays(i);
                //           // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           //bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                //           //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               cmBatteryVmModels[index].BatteryAgingTestStartDate = prodDate;
                //               cmBatteryVmModels[index].BatteryAging = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryAging) ? cmBatteryVmModels[index].BatteryAging + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;

                //           }
                //       }
                //necessary
                #endregion
                ////Assembly && packing///
                daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBAssembly) -
                 Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBAssembly).AddDays(i);
                    // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    //bool alreadyExist = getsHoliday.Contains(prodDate);
                    //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                    //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].MaterialReceiveStartDateBAssembly = prodDate;
                        cmBatteryVmModels[index].MetarialReceiveAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveAssembly) ? cmBatteryVmModels[index].MetarialReceiveAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBAssembly) -
                Convert.ToDateTime(queryProduction.IqcCompleteStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBAssembly).AddDays(i);
                    // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    //bool alreadyExist = getsHoliday.Contains(prodDate);
                    //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                    //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].IqcCompleteStartDateBAssembly = prodDate;
                        cmBatteryVmModels[index].IqcCompleteAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteAssembly) ? cmBatteryVmModels[index].IqcCompleteAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBAssembly) -
               Convert.ToDateTime(queryProduction.TrialProductionStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBAssembly).AddDays(i);

                    // + ", TrialLineNumber: " + smtMassLineNumbers
                    var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                                   where als.WorkingDate == prodDate && als.AssemblyTrialId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                                   select als);
                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                    //

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].TrialProductionStartDateBAssembly = prodDate;
                        cmBatteryVmModels[index].TrialProductionAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionAssembly) ? cmBatteryVmModels[index].TrialProductionAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TrialLineNumber: " + smtMassLineNumbers;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.SoftwareConfirmationEndDateBAssembly) -
                Convert.ToDateTime(queryProduction.SoftwareConfirmationStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.SoftwareConfirmationStartDateBAssembly).AddDays(i);
                    // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    //bool alreadyExist = getsHoliday.Contains(prodDate);
                    //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                    //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].SoftwareConfirmationStartDateBAssembly = prodDate;
                        cmBatteryVmModels[index].SoftwareConfirmationAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SoftwareConfirmationAssembly) ? cmBatteryVmModels[index].SoftwareConfirmationAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.RandDConfirmationEndDateBAssembly) -
              Convert.ToDateTime(queryProduction.RandDConfirmationStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.RandDConfirmationStartDateBAssembly).AddDays(i);
                    // var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    //bool alreadyExist = getsHoliday.Contains(prodDate);
                    //bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing));
                    //bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing));

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].RandDConfirmationStartDateBAssembly = prodDate;
                        cmBatteryVmModels[index].RnDConfirmAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].RnDConfirmAssembly) ? cmBatteryVmModels[index].RnDConfirmAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.AssemblyMassProductionEndDateBAssembly) -
    Convert.ToDateTime(queryProduction.AssemblyMassProductionStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.AssemblyMassProductionStartDateBAssembly).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatteryAssemblyLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatteryAssemblyId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);

                    #region com
                    //if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                    //{
                    //    cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                    //    cmBatteryVmModels[index].PackingLine1 = "Holiday";
                    //    cmBatteryVmModels[index].PackingLine2 = "Holiday";
                    //}
                    //else
                    //{
                    //    foreach (var smtMassLineNumber in qqtySmt)
                    //    {
                    //        if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                    //        {
                    //            cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                    //            cmBatteryVmModels[index].PackingLine1 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine1) ? cmBatteryVmModels[index].PackingLine1 + ", " + queryProduction.ProjectName
                    //                + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                    //                + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                    //        }
                    //        if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                    //        {
                    //            cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                    //            cmBatteryVmModels[index].PackingLine2 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine2) ? cmBatteryVmModels[index].PackingLine2 + ", " + queryProduction.ProjectName
                    //                + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                    //                + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();


                    //        }
                    //    }
                    //}

                    // foreach (var smtMassLineNumber in qqtySmt)
                    // {
                    // if (smtMassLineNumber.LineNumber.Trim() != null && smtMassLineNumber.LineNumber.Trim() == "Line-1")
                    //  {
                    #endregion
                    if (index != -1)
                    {
                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                            cmBatteryVmModels[index].AssemblyLineOne = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineTwo = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineThree = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineFour = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineFive = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].AssemblyLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineOne) ? cmBatteryVmModels[index].AssemblyLineOne + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].AssemblyLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineTwo) ? cmBatteryVmModels[index].AssemblyLineTwo + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-3")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].AssemblyLineThree = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineThree) ? cmBatteryVmModels[index].AssemblyLineThree + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-4")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].AssemblyLineFour = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFour) ? cmBatteryVmModels[index].AssemblyLineFour + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-5")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].AssemblyLineFive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFive) ? cmBatteryVmModels[index].AssemblyLineFive + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                }
                            }
                        }
                    }
                }
                #region com
                //        if (smtMassLineNumber.LineNumber.Trim() != null && smtMassLineNumber.LineNumber.Trim() == "Line-2")
                //        {
                //            if (index != -1)
                //            {
                //                if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineTwo = "Holiday";
                //                }
                //                else
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineTwo) ? cmBatteryVmModels[index].AssemblyLineTwo + ", " + queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                //                }
                //            }
                //        }
                //        if (smtMassLineNumber.LineNumber.Trim() != null && smtMassLineNumber.LineNumber.Trim() == "Line-3")
                //        {
                //            if (index != -1)
                //            {
                //                if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineThree = "Holiday";
                //                }
                //                else
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineThree = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineThree) ? cmBatteryVmModels[index].AssemblyLineThree + ", " + queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                //                }
                //            }
                //        }
                //        if (smtMassLineNumber.LineNumber.Trim() != null && smtMassLineNumber.LineNumber.Trim() == "Line-4")
                //        {
                //            if (index != -1)
                //            {
                //                if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineFour = "Holiday";
                //                }
                //                else
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineFour = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFour) ? cmBatteryVmModels[index].AssemblyLineFour + ", " + queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                //                }
                //            }
                //        }
                //        if (smtMassLineNumber.LineNumber.Trim() != null && smtMassLineNumber.LineNumber.Trim() == "Line-5")
                //        {
                //            if (index != -1)
                //            {
                //                if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineFive = "Holiday";
                //                }
                //                else
                //                {
                //                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                    cmBatteryVmModels[index].AssemblyLineFive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFive) ? cmBatteryVmModels[index].AssemblyLineFive + ", " + queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                //                        + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                //                }
                //            }
                //        }
                //    }
                //}

                //       daysDifference = (Convert.ToDateTime(queryProduction.AssemblyMassProductionEndDateBAssembly) -
                //Convert.ToDateTime(queryProduction.AssemblyMassProductionStartDateBAssembly)).TotalDays + 1;
                //       for (int i = 0; i < daysDifference; i++)
                //       {
                //           var prodDate = Convert.ToDateTime(queryProduction.AssemblyMassProductionStartDateBAssembly).AddDays(i);

                //           var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                //           bool alreadyExist = getsHoliday.Contains(prodDate);
                //           //
                //           var qqtySmt = (from als in _dbEntities.BatteryAssemblyLineCapacityDetails
                //                          where als.WorkingDate == prodDate && als.BatteryAssemblyId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                //                          select als);

                //           var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                //           var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());
                //           var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.LineNumber));
                //           //
                //           int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                //           if (index != -1)
                //           {
                //               if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null &&
                //                   smtMassLineNumbers == "")
                //               {
                //                   cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                   cmBatteryVmModels[index].AssemblyMassProduction = "Holiday";
                //               }
                //               else
                //               {
                //                   cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                //                   cmBatteryVmModels[index].AssemblyMassProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyMassProduction) ? cmBatteryVmModels[index].AssemblyMassProduction + ", " + queryProduction.ProjectName
                //                       + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers : queryProduction.ProjectName
                //                       + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers;

                //               }
                //           }
                //       }
                #endregion
                daysDifference = (Convert.ToDateTime(queryProduction.PackingMassProductionEndDateBAssembly) -
                Convert.ToDateTime(queryProduction.PackingMassProductionStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.PackingMassProductionStartDateBAssembly).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatteryPackingLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatteryPackingId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);

                    if (index != -1)
                    {

                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                            cmBatteryVmModels[index].PackingLine1 = "Holiday";
                            cmBatteryVmModels[index].PackingLine2 = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].PackingLine1 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine1) ? cmBatteryVmModels[index].PackingLine1 + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                                    cmBatteryVmModels[index].PackingLine2 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine2) ? cmBatteryVmModels[index].PackingLine2 + ", " + queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                        + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();


                                }
                            }
                        }
                    }

                    //if (index != -1)
                    //{
                    //    if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null &&
                    //        smtMassLineNumbers == "")
                    //    {
                    //        cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                    //        cmBatteryVmModels[index].PackingMassProduction = "Holiday";
                    //    }
                    //    else
                    //    {
                    //        cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                    //        cmBatteryVmModels[index].PackingMassProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingMassProduction) ? cmBatteryVmModels[index].PackingMassProduction + ", " + queryProduction.ProjectName
                    //            + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers : queryProduction.ProjectName
                    //            + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumbers;

                    //    }
                    //}
                }

                foreach (var result in results)
                {
                    var convertedPd = Convert.ToDateTime(result);
                    var proRemrk = (from proPlan in _dbEntities.ProductionPlanRemarks
                                    where proPlan.ProductionDate == convertedPd
                                    select proPlan).FirstOrDefault();


                    if (proRemrk != null)
                    {
                        int index1 = cmBatteryVmModels.FindIndex(x => x.ProductionDate == proRemrk.ProductionDate);

                        if (index1 != -1)
                        {
                            //  cmBatteryVmModels[index1].ProductionDate = queryProduction.ProductionDate;
                            cmBatteryVmModels[index1].ProductionRemarks = proRemrk.Remarks;
                        }

                    }
                }




            }

            return cmBatteryVmModels;
        }

        public List<BatteryProductionViewModel> GetCkdGrandChartData(List<string> results)
        {
            string query1 = string.Empty;
            query1 = string.Format(@"select  
            asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,smt.Id as SmtId,
            asm.planId as AsmPlanId,bb.planId as BbPlanId,bh.planid as BhPlanId,smt.planid as SmtPlanId,asm.ProjectName,smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,CONVERT(varchar(10), smt.TotalQuantityBSmt) as TotalQuantityBSmt,

            asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing,CONVERT(varchar(10), bh.TotalQuantity) as TotalQuantity,
            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,CONVERT(varchar(10), bb.TotalQuantityBattery) as TotalQuantityBattery,
            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,CONVERT(varchar(10), asm.TotalQuantityBAssembly) as TotalQuantityBAssembly,CONVERT(varchar(10), asm.TotalQuantityBPacking) as TotalQuantityBPacking

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId where asm.IsActive=1
            order by asm.AddedDate desc");
            //            where (asm.planid=(select top 1 planid from CellPhoneProject.dbo.[BatteryAssemblyAndPacking] where ProjectId=asm.ProjectId order by planid desc))");
            List<CustomBatteryProduction> batteryProQuery = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query1).ToList();

            List<BatteryProductionViewModel> cmBatteryVmModels = new List<BatteryProductionViewModel>();

            cmBatteryVmModels.AddRange(results.Select(pDate => new BatteryProductionViewModel { ProductionDate = Convert.ToDateTime(pDate) }));

            foreach (var queryProduction in batteryProQuery)
            {
                var daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBSmt) -
                 Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBSmt).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    bool alreadyExist1 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBSmt));
                    bool alreadyExist2 = getsHoliday.Contains(Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBSmt));

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].MaterialReceiveStartDateBSmt = prodDate;
                        // cmBatteryVmModels[index].MetarialReceiveBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveBSmt) ? cmBatteryVmModels[index].MetarialReceiveBSmt + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt;
                        cmBatteryVmModels[index].MetarialReceiveBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveBSmt) ? cmBatteryVmModels[index].MetarialReceiveBSmt + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBSmt) -
                Convert.ToDateTime(queryProduction.IqcCompleteStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBSmt).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].IqcCompleteStartDateBSmt = prodDate;
                        // cmBatteryVmModels[index].IqcCompleteBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBSmt) ? cmBatteryVmModels[index].IqcCompleteBSmt + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt;
                        cmBatteryVmModels[index].IqcCompleteBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBSmt) ? cmBatteryVmModels[index].IqcCompleteBSmt + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBSmt) -
           Convert.ToDateTime(queryProduction.TrialProductionStartDateBSmt)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBSmt).AddDays(i);

                    //
                    var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                                   where als.WorkingDate == prodDate && als.SmtTrialId == queryProduction.SmtId && als.PlanId == queryProduction.SmtPlanId
                                   select als);

                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                    //

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].TrialProductionStartDateBSmt = prodDate;
                        // cmBatteryVmModels[index].TrialProductionBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionBSmt) ? cmBatteryVmModels[index].TrialProductionBSmt + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TrialLineNumber: " + smtMassLineNumbers;
                        cmBatteryVmModels[index].TrialProductionBSmt = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionBSmt) ? cmBatteryVmModels[index].TrialProductionBSmt + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.SmtMassProductionEndDateBSmt) -
                Convert.ToDateTime(queryProduction.SmtMassProductionStartDateBSmt)).TotalDays + 1;

                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.SmtMassProductionStartDateBSmt).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatterySMTLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatterySMT_Id == queryProduction.SmtId && als.PlanId == queryProduction.SmtPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);


                    if (index != -1)
                    {
                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].SmtMassProductionStartDateBSmt = prodDate;
                            cmBatteryVmModels[index].SmtLineOne = "Holiday";
                            cmBatteryVmModels[index].SmtLineTwo = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].SmtMassProductionStartDateBSmt = prodDate;
                                    //cmBatteryVmModels[index].SmtLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SmtLineOne) ? cmBatteryVmModels[index].SmtLineOne + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].SmtLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SmtLineOne) ? cmBatteryVmModels[index].SmtLineOne + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].SmtMassProductionStartDateBSmt = prodDate;
                                    //cmBatteryVmModels[index].SmtLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SmtLineTwo) ? cmBatteryVmModels[index].SmtLineTwo + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBSmt + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].SmtLineTwo =
                                        !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SmtLineTwo)
                                            ? cmBatteryVmModels[index].SmtLineTwo + ", " + queryProduction.ProjectName : queryProduction.ProjectName;
                                }

                            }
                        }
                    }
                }


                ////Housing///
                daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBHousing) -
                 Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBHousing).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].MaterialReceiveStartDateBHousing = prodDate;
                        // cmBatteryVmModels[index].MetarialReceiveBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveBHousing) ? cmBatteryVmModels[index].MetarialReceiveBHousing + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity;
                        cmBatteryVmModels[index].MetarialReceiveBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveBHousing) ? cmBatteryVmModels[index].MetarialReceiveBHousing + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }


                daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBHousing) -
                  Convert.ToDateTime(queryProduction.IqcCompleteStartDateBHousing)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBHousing).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].IqcCompleteStartDateBHousing = prodDate;
                        //  cmBatteryVmModels[index].IqcCompleteBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBHousing) ? cmBatteryVmModels[index].IqcCompleteBHousing + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity;
                        cmBatteryVmModels[index].IqcCompleteBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBHousing) ? cmBatteryVmModels[index].IqcCompleteBHousing + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBHousing) -
                Convert.ToDateTime(queryProduction.TrialProductionStartDateBHousing)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBHousing).AddDays(i);

                    // + ", TrialLineNumber: " + smtMassLineNumbers
                    var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                                   where als.WorkingDate == prodDate && als.HousingTrialId == queryProduction.BhId && als.PlanId == queryProduction.BhPlanId
                                   select als);

                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                    //

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].TrialProductionStartDateBHousing = prodDate;
                        //cmBatteryVmModels[index].TrialProductionBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionBHousing) ? cmBatteryVmModels[index].TrialProductionBHousing + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TrialLineNumber: " + smtMassLineNumbers;
                        cmBatteryVmModels[index].TrialProductionBHousing = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionBHousing) ? cmBatteryVmModels[index].TrialProductionBHousing + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.HousingReliabilityTestEndDateBHousing) -
                Convert.ToDateTime(queryProduction.HousingReliabilityTestStartDateBHousing)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.HousingReliabilityTestStartDateBHousing).AddDays(i);
                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].HousingReliabilityTestStartDateBHousing = prodDate;
                        //cmBatteryVmModels[index].HousingBReliability = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingBReliability) ? cmBatteryVmModels[index].HousingBReliability + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantity;
                        cmBatteryVmModels[index].HousingBReliability = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingBReliability) ? cmBatteryVmModels[index].HousingBReliability + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.HousingMassProductionEndDateBHousing) -
         Convert.ToDateTime(queryProduction.HousingMassProductionStartDateBHousing)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.HousingMassProductionStartDateBHousing).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatteryHousingLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatteryHousing_Id == queryProduction.BhId && als.PlanId == queryProduction.BhPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());
                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.LineNumber));
                    //
                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                            cmBatteryVmModels[index].HousingLineOne = "Holiday";
                            cmBatteryVmModels[index].HousingLineTwo = "Holiday";
                            cmBatteryVmModels[index].HousingLineThree = "Holiday";
                            cmBatteryVmModels[index].HousingLineFour = "Holiday";
                            cmBatteryVmModels[index].HousingLineFive = "Holiday";
                            cmBatteryVmModels[index].HousingLineSix = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                                    //cmBatteryVmModels[index].HousingLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineOne) ? cmBatteryVmModels[index].HousingLineOne + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                    cmBatteryVmModels[index].HousingLineOne =
                                        !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineOne)
                                            ? cmBatteryVmModels[index].HousingLineOne + ", " +
                                              queryProduction.ProjectName : queryProduction.ProjectName;


                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                                    //cmBatteryVmModels[index].HousingLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineTwo) ? cmBatteryVmModels[index].HousingLineTwo + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                    cmBatteryVmModels[index].HousingLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineTwo) ? cmBatteryVmModels[index].HousingLineTwo + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-3")
                                {
                                    cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                                    //cmBatteryVmModels[index].HousingLineThree = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineThree) ? cmBatteryVmModels[index].HousingLineThree + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].HousingLineThree = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineThree) ? cmBatteryVmModels[index].HousingLineThree + ", " + queryProduction.ProjectName : queryProduction.ProjectName;


                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-4")
                                {
                                    cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                                    //cmBatteryVmModels[index].HousingLineFour = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineFour) ? cmBatteryVmModels[index].HousingLineFour + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].HousingLineFour = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineFour) ? cmBatteryVmModels[index].HousingLineFour + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-5")
                                {
                                    cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                                    //cmBatteryVmModels[index].HousingLineFive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineFive) ? cmBatteryVmModels[index].HousingLineFive + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();

                                    cmBatteryVmModels[index].HousingLineFive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineFive) ? cmBatteryVmModels[index].HousingLineFive + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-6")
                                {
                                    cmBatteryVmModels[index].HousingMassProductionStartDateBHousing = prodDate;
                                    //cmBatteryVmModels[index].HousingLineSix = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineSix) ? cmBatteryVmModels[index].HousingLineSix + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantity + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].HousingLineSix = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].HousingLineSix) ? cmBatteryVmModels[index].HousingLineSix + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                            }
                        }
                    }
                }

                ////Battery///
                daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBattery) -
                 Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBattery)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBattery).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].MaterialReceiveStartDateBattery = prodDate;
                        // cmBatteryVmModels[index].BatteryMetarialReceive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryMetarialReceive) ? cmBatteryVmModels[index].BatteryMetarialReceive + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;
                        cmBatteryVmModels[index].BatteryMetarialReceive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryMetarialReceive) ? cmBatteryVmModels[index].BatteryMetarialReceive + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBattery) -
           Convert.ToDateTime(queryProduction.IqcCompleteStartDateBattery)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBattery).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].IqcCompleteStartDateBattery = prodDate;
                        //cmBatteryVmModels[index].BatteryIqcComplete = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryIqcComplete) ? cmBatteryVmModels[index].BatteryIqcComplete + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;
                        cmBatteryVmModels[index].BatteryIqcComplete = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryIqcComplete) ? cmBatteryVmModels[index].BatteryIqcComplete + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBattery) -
                Convert.ToDateTime(queryProduction.TrialProductionStartDateBattery)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBattery).AddDays(i);

                    // + ", TrialLineNumber: " + smtMassLineNumbers
                    var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                                   where als.WorkingDate == prodDate && als.BatteryTrialId == queryProduction.BbId && als.PlanId == queryProduction.BbPlanId
                                   select als);

                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                    //

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].TrialProductionStartDateBattery = prodDate;
                        //cmBatteryVmModels[index].BatteryTrialProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryTrialProduction) ? cmBatteryVmModels[index].BatteryTrialProduction + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TrialLineNumber: " + smtMassLineNumbers;
                        cmBatteryVmModels[index].BatteryTrialProduction = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryTrialProduction) ? cmBatteryVmModels[index].BatteryTrialProduction + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.BatteryReliabilityTestEndDate) -
               Convert.ToDateTime(queryProduction.BatteryReliabilityTestStartDate)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.BatteryReliabilityTestStartDate).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].BatteryReliabilityTestStartDate = prodDate;
                        //cmBatteryVmModels[index].BatteryReliability = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryReliability) ? cmBatteryVmModels[index].BatteryReliability + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;
                        cmBatteryVmModels[index].BatteryReliability = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryReliability) ? cmBatteryVmModels[index].BatteryReliability + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.BatteryMassProductionEndDate) -
          Convert.ToDateTime(queryProduction.BatteryMassProductionStartDate)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.BatteryMassProductionStartDate).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatteryLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.Battery_Id == queryProduction.BbId && als.PlanId == queryProduction.BbPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());
                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.LineNumber));
                    //
                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {

                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].BatteryMassProductionStartDate = prodDate;
                            cmBatteryVmModels[index].BatteryLineOne = "Holiday";
                            cmBatteryVmModels[index].BatteryLineTwo = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].BatteryMassProductionStartDate = prodDate;
                                    //cmBatteryVmModels[index].BatteryLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryLineOne) ? cmBatteryVmModels[index].BatteryLineOne + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].BatteryLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryLineOne) ? cmBatteryVmModels[index].BatteryLineOne + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].BatteryMassProductionStartDate = prodDate;
                                    //cmBatteryVmModels[index].BatteryLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryLineTwo) ? cmBatteryVmModels[index].BatteryLineTwo + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBattery + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].BatteryLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryLineTwo) ? cmBatteryVmModels[index].BatteryLineTwo + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }

                            }
                        }

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.BatteryAgingTestEndDate) -
             Convert.ToDateTime(queryProduction.BatteryAgingTestStartDate)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.BatteryAgingTestStartDate).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].BatteryAgingTestStartDate = prodDate;
                        //cmBatteryVmModels[index].BatteryAging = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryAging) ? cmBatteryVmModels[index].BatteryAging + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBattery;
                        cmBatteryVmModels[index].BatteryAging = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].BatteryAging) ? cmBatteryVmModels[index].BatteryAging + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                ////Assembly && packing///
                daysDifference = (Convert.ToDateTime(queryProduction.MaterialReceiveEndDateBAssembly) -
                 Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.MaterialReceiveStartDateBAssembly).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].MaterialReceiveStartDateBAssembly = prodDate;
                        // cmBatteryVmModels[index].MetarialReceiveAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveAssembly) ? cmBatteryVmModels[index].MetarialReceiveAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;
                        cmBatteryVmModels[index].MetarialReceiveAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].MetarialReceiveAssembly) ? cmBatteryVmModels[index].MetarialReceiveAssembly + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.IqcCompleteEndDateBAssembly) -
                Convert.ToDateTime(queryProduction.IqcCompleteStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.IqcCompleteStartDateBAssembly).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].IqcCompleteStartDateBAssembly = prodDate;
                        // cmBatteryVmModels[index].IqcCompleteAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteAssembly) ? cmBatteryVmModels[index].IqcCompleteAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;
                        cmBatteryVmModels[index].IqcCompleteAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteAssembly) ? cmBatteryVmModels[index].IqcCompleteAssembly + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.TrialProductionEndDateBAssembly) -
               Convert.ToDateTime(queryProduction.TrialProductionStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.TrialProductionStartDateBAssembly).AddDays(i);

                    // + ", TrialLineNumber: " + smtMassLineNumbers
                    var qqtySmt = (from als in _dbEntities.AllTrialInfoes
                                   where als.WorkingDate == prodDate && als.AssemblyTrialId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                                   select als);
                    var smtMassLineNumbers = String.Join(", ", qqtySmt.Select(p => p.TrialLineNumber).Distinct());
                    //

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].TrialProductionStartDateBAssembly = prodDate;
                        //cmBatteryVmModels[index].TrialProductionAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionAssembly) ? cmBatteryVmModels[index].TrialProductionAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TrialLineNumber: " + smtMassLineNumbers : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TrialLineNumber: " + smtMassLineNumbers;
                        cmBatteryVmModels[index].TrialProductionAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].TrialProductionAssembly) ? cmBatteryVmModels[index].TrialProductionAssembly + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }
                daysDifference = (Convert.ToDateTime(queryProduction.SoftwareConfirmationEndDateBAssembly) -
                Convert.ToDateTime(queryProduction.SoftwareConfirmationStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.SoftwareConfirmationStartDateBAssembly).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].SoftwareConfirmationStartDateBAssembly = prodDate;
                        //cmBatteryVmModels[index].SoftwareConfirmationAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SoftwareConfirmationAssembly) ? cmBatteryVmModels[index].SoftwareConfirmationAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;
                        cmBatteryVmModels[index].SoftwareConfirmationAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].SoftwareConfirmationAssembly) ? cmBatteryVmModels[index].SoftwareConfirmationAssembly + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.RandDConfirmationEndDateBAssembly) -
              Convert.ToDateTime(queryProduction.RandDConfirmationStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.RandDConfirmationStartDateBAssembly).AddDays(i);

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);
                    if (index != -1)
                    {
                        cmBatteryVmModels[index].RandDConfirmationStartDateBAssembly = prodDate;
                        //cmBatteryVmModels[index].RnDConfirmAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].RnDConfirmAssembly) ? cmBatteryVmModels[index].RnDConfirmAssembly + ", " + queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly : queryProduction.ProjectName + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly;
                        cmBatteryVmModels[index].RnDConfirmAssembly = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].RnDConfirmAssembly) ? cmBatteryVmModels[index].RnDConfirmAssembly + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.AssemblyMassProductionEndDateBAssembly) -
    Convert.ToDateTime(queryProduction.AssemblyMassProductionStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.AssemblyMassProductionStartDateBAssembly).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatteryAssemblyLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatteryAssemblyId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);


                    if (index != -1)
                    {
                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                            cmBatteryVmModels[index].AssemblyLineOne = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineTwo = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineThree = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineFour = "Holiday";
                            cmBatteryVmModels[index].AssemblyLineFive = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].AssemblyLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineOne) ? cmBatteryVmModels[index].AssemblyLineOne + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].AssemblyLineOne = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineOne) ? cmBatteryVmModels[index].AssemblyLineOne + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].AssemblyLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineTwo) ? cmBatteryVmModels[index].AssemblyLineTwo + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].AssemblyLineTwo = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineTwo) ? cmBatteryVmModels[index].AssemblyLineTwo + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-3")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].AssemblyLineThree = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineThree) ? cmBatteryVmModels[index].AssemblyLineThree + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].AssemblyLineThree = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineThree) ? cmBatteryVmModels[index].AssemblyLineThree + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-4")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].AssemblyLineFour = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFour) ? cmBatteryVmModels[index].AssemblyLineFour + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].AssemblyLineFour = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFour) ? cmBatteryVmModels[index].AssemblyLineFour + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-5")
                                {
                                    cmBatteryVmModels[index].AssemblyMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].AssemblyLineFive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFive) ? cmBatteryVmModels[index].AssemblyLineFive + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBAssembly + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].AssemblyLineFive = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].AssemblyLineFive) ? cmBatteryVmModels[index].AssemblyLineFive + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                            }
                        }
                    }
                }

                daysDifference = (Convert.ToDateTime(queryProduction.PackingMassProductionEndDateBAssembly) -
                Convert.ToDateTime(queryProduction.PackingMassProductionStartDateBAssembly)).TotalDays + 1;
                for (int i = 0; i < daysDifference; i++)
                {
                    var prodDate = Convert.ToDateTime(queryProduction.PackingMassProductionStartDateBAssembly).AddDays(i);

                    var getsHoliday = GetHolidayDatasList().Where(i1 => i1.HolidayDate != null).Select(i1 => (DateTime)i1.HolidayDate).ToList();
                    bool alreadyExist = getsHoliday.Contains(prodDate);
                    //
                    var qqtySmt = (from als in _dbEntities.BatteryPackingLineCapacityDetails
                                   where als.WorkingDate == prodDate && als.BatteryPackingId == queryProduction.AsmId && als.PlanId == queryProduction.AsmPlanId
                                   select als);

                    var smtMassPerDayQty = (qqtySmt.Select(p => p.PerDayCapacity).FirstOrDefault());
                    var smtMassLineCap = (qqtySmt.Select(p => p.LineCapacity).FirstOrDefault());

                    int index = cmBatteryVmModels.FindIndex(x => x.ProductionDate == prodDate);

                    if (index != -1)
                    {

                        if (alreadyExist == true && smtMassPerDayQty == null && smtMassLineCap == null)
                        {
                            cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                            cmBatteryVmModels[index].PackingLine1 = "Holiday";
                            cmBatteryVmModels[index].PackingLine2 = "Holiday";
                        }
                        else
                        {
                            foreach (var smtMassLineNumber in qqtySmt)
                            {
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-1")
                                {
                                    cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].PackingLine1 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine1) ? cmBatteryVmModels[index].PackingLine1 + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].PackingLine1 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine1) ? cmBatteryVmModels[index].PackingLine1 + ", " + queryProduction.ProjectName : queryProduction.ProjectName;

                                }
                                if (smtMassLineNumber.LineNumber.Trim() == "Line-2")
                                {
                                    cmBatteryVmModels[index].PackingMassProductionStartDateBAssembly = prodDate;
                                    //cmBatteryVmModels[index].PackingLine2 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine2) ? cmBatteryVmModels[index].PackingLine2 + ", " + queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim() : queryProduction.ProjectName
                                    //    + ", TotalQuantity:" + queryProduction.TotalQuantityBPacking + ", TotalLineCapacity: " + smtMassLineCap + ", PerDayCapacity: " + smtMassPerDayQty + ", LineNumber: " + smtMassLineNumber.LineNumber.Trim();
                                    cmBatteryVmModels[index].PackingLine2 = !string.IsNullOrWhiteSpace(cmBatteryVmModels[index].PackingLine2) ? cmBatteryVmModels[index].PackingLine2 + ", " + queryProduction.ProjectName : queryProduction.ProjectName;


                                }
                            }
                        }
                    }

                }

                //foreach (var result in results)
                //{
                //    var convertedPd = Convert.ToDateTime(result);
                //    var proRemrk = (from proPlan in _dbEntities.ProductionPlanRemarks
                //                    where proPlan.ProductionDate == convertedPd
                //                    select proPlan).FirstOrDefault();


                //    if (proRemrk != null)
                //    {
                //        int index1 = cmBatteryVmModels.FindIndex(x => x.ProductionDate == proRemrk.ProductionDate);

                //        if (index1 != -1)
                //        {
                //            //  cmBatteryVmModels[index1].ProductionDate = queryProduction.ProductionDate;
                //            cmBatteryVmModels[index1].ProductionRemarks = proRemrk.Remarks;
                //        }

                //    }
                //}

            }

            return cmBatteryVmModels;
        }

        public List<CustomBatteryProduction> GetPartialProject()
        {
            string query = string.Format(@"select asm.PlanId as AsmPlanId,bb.PlanId as BbPlanId, bh.PlanId as BhPlanId,smt.PlanId as SmtPlanId,smt.Id as SmtId,asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,asm.ProjectId,asm.ProjectName,asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,
            smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,CAST(smt.TotalQuantityBSmt AS VARCHAR(10)) as TotalQuantityBSmt,
            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,
            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId
            where asm.IsActive=1 and (asm.planid=(select top 1 planid from CellPhoneProject.dbo.[BatteryAssemblyAndPacking] where ProjectId=asm.ProjectId order by planid desc))");

            var exe = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query).ToList();
            foreach (var custBatteryPro in exe)
            {
                var qqty1 = (from als in _dbEntities.AllTrialInfoes
                             where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBSmt &&
                                 als.WorkingDate <= custBatteryPro.TrialProductionEndDateBSmt && als.SmtTrialId == custBatteryPro.SmtId
                             select als);

                var trialLineNumbers = String.Join(", ", qqty1.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.TrialLineNumber = trialLineNumbers;

                var qqty2 = (from bsct in _dbEntities.BatterySMTLineCapacityDetails
                             where bsct.WorkingDate >= custBatteryPro.SmtMassProductionStartDateBSmt &&
                                 bsct.WorkingDate <= custBatteryPro.SmtMassProductionEndDateBSmt && bsct.BatterySMT_Id == custBatteryPro.SmtId
                             select bsct);

                var smtLine = String.Join(", ", qqty2.Select(p => p.LineNumber).Distinct());
                var smtPerDayCap = (qqty2.Select(p => p.PerDayCapacity).FirstOrDefault());
                var smtLineCap = (qqty2.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.SmtAllLineNumber = smtLine;
                custBatteryPro.SmtAllLineCapacity = Convert.ToString(smtLineCap);
                custBatteryPro.BatterySmtPerDayCapacity = Convert.ToInt64(smtPerDayCap);

            }

            return exe;
        }

        public List<AllTrialInfo> GetSmtTrialLineForEdit()
        {
            string query = string.Format(@"select * from [CellPhoneProject].[dbo].[AllTrialInfo] where SmtTrialId is not null");
            var exe = _dbEntities.Database.SqlQuery<AllTrialInfo>(query).ToList();
            return exe;
        }

        public string UpdateChdPlanning(CustomBatteryProduction allInfo)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            #region SMT
            var updatedSmt = (from c in _dbEntities.BatterySMTs
                              where c.ProjectId == allInfo.AsmProjectId && c.PlanId == allInfo.SmtPlanId && c.Id == allInfo.SmtId
                              select c).FirstOrDefault();

            if (updatedSmt != null)
            {
                if (updatedSmt.MaterialReceiveStartDateBSmt != null)
                {
                    updatedSmt.MaterialReceiveStartDateBSmt = allInfo.MaterialReceiveStartDateBSmt;
                    updatedSmt.MaterialReceiveEndDateBSmt = allInfo.MaterialReceiveEndDateBSmt;
                    updatedSmt.IqcCompleteStartDateBSmt = allInfo.IqcCompleteStartDateBSmt;
                    updatedSmt.IqcCompleteEndDateBSmt = allInfo.IqcCompleteEndDateBSmt;
                    updatedSmt.TrialProductionStartDateBSmt = allInfo.TrialProductionStartDateBSmt;
                    updatedSmt.TrialProductionEndDateBSmt = allInfo.TrialProductionEndDateBSmt;
                    updatedSmt.SmtMassProductionStartDateBSmt = allInfo.SmtMassProductionStartDateBSmt;
                    updatedSmt.TotalQuantityBSmt = Convert.ToInt64(allInfo.TotalQuantityBSmt);
                    updatedSmt.SmtMassProductionEndDateBSmt = allInfo.SmtMassProductionEndDateBSmt;
                    updatedSmt.Updated = userId;
                    updatedSmt.UpdatedDate = DateTime.Now;
                    updatedSmt.IsActive = true;
                    ////Smt Trial////
                    var trialLineNumSmt = allInfo.SmtTrialLine;
                    var stroreStringSmtTrialSplit = trialLineNumSmt.Split(' ').ToList();
                    stroreStringSmtTrialSplit = trialLineNumSmt.Split(',').ToList();

                    var remove = (from c in _dbEntities.AllTrialInfoes
                                  where c.SmtTrialId == allInfo.SmtId && c.PlanId == allInfo.SmtPlanId
                                  select c).ToList();

                    if (remove.Count != 0)
                    {
                        _dbEntities.AllTrialInfoes.RemoveRange(remove.ToList());
                        _dbEntities.SaveChanges();
                    }

                    ////holiday///
                    var exe1 = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();
                    var datesSmtTrial11 = GetDatesBetween(Convert.ToDateTime(allInfo.TrialProductionStartDateBSmt), Convert.ToDateTime(allInfo.TrialProductionEndDateBSmt));
                    // var datesSmtTrial12 = datesSmtTrial11.Except(exe1);
                    //holiday///

                    //var daysDifference = (Convert.ToDateTime(allInfo.TrialProductionEndDateBSmt) -
                    //                      Convert.ToDateTime(allInfo.TrialProductionStartDateBSmt)).TotalDays + 1;

                    foreach (var spVal in stroreStringSmtTrialSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            // for (int i = 0; i < daysDifference; i++)
                            // {
                            // var prodDate = Convert.ToDateTime(allInfo.TrialProductionStartDateBSmt).AddDays(i);

                            foreach (var smtDate in datesSmtTrial11)
                            {
                                AllTrialInfo trialInfo = new AllTrialInfo();
                                trialInfo.PlanId = allInfo.SmtPlanId;
                                trialInfo.SmtTrialId = allInfo.SmtId;
                                trialInfo.WorkingDate = smtDate;
                                trialInfo.TrialLineNumber = spVal.Trim();
                                trialInfo.Added = userId;
                                trialInfo.AddedDate = DateTime.Now;
                                trialInfo.IsActive = true;
                                _dbEntities.AllTrialInfoes.Add(trialInfo);
                                _dbEntities.SaveChanges();
                            }



                            // }
                        }
                    }
                    ////end smt Trial////

                    ////Smt Mass Line//// 
                    var smtMassLines = allInfo.SmtAllLineNumber;
                    var stroreStringSmtMassSplit = smtMassLines.Split(' ').ToList();
                    stroreStringSmtMassSplit = smtMassLines.Split(',').ToList();

                    var removeSmtMass = (from c in _dbEntities.BatterySMTLineCapacityDetails
                                         where c.BatterySMT_Id == allInfo.SmtId && c.PlanId == allInfo.SmtPlanId
                                         select c).ToList();

                    if (removeSmtMass.Count != 0)
                    {
                        _dbEntities.BatterySMTLineCapacityDetails.RemoveRange(removeSmtMass.ToList());
                        _dbEntities.SaveChanges();
                    }

                    //daysDifference = (Convert.ToDateTime(allInfo.SmtMassProductionEndDateBSmt) -
                    //                     Convert.ToDateTime(allInfo.SmtMassProductionStartDateBSmt)).TotalDays + 1;
                    var datesSmt11 = GetDatesBetween(Convert.ToDateTime(allInfo.SmtMassProductionStartDateBSmt), Convert.ToDateTime(allInfo.SmtMassProductionEndDateBSmt));
                    // var datesSmt12 = datesSmt11.Except(exe1);

                    foreach (var spVal in stroreStringSmtMassSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            //for (int i = 0; i < daysDifference; i++)
                            //{
                            //var prodDate = Convert.ToDateTime(allInfo.SmtMassProductionStartDateBSmt).AddDays(i);
                            foreach (var smtDate in datesSmt11)
                            {
                                var quryForSmtLine = (from c in _dbEntities.LineInformations
                                                      join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                                      where c.LineNumber == spVal.Trim() && c.CKD_SMT_Line_Capacity > 0 && pm.ProjectMasterId == allInfo.AsmProjectId
                                                      select c).FirstOrDefault();

                                var query13 = (from c in _dbEntities.BatterySMTLineCapacityDetails
                                               where c.LineNumber == spVal.Trim() && c.WorkingDate == smtDate && c.BatterySMT_Id == allInfo.SmtId
                                               select c).OrderByDescending(x => x.Id).FirstOrDefault();


                                BatterySMTLineCapacityDetail batterySmt = new BatterySMTLineCapacityDetail();
                                batterySmt.PlanId = allInfo.SmtPlanId;
                                batterySmt.BatterySMT_Id = allInfo.SmtId;
                                batterySmt.WorkingDate = smtDate;
                                batterySmt.PerDayCapacity = allInfo.BatterySmtPerDayCapacity;
                                batterySmt.TotalQuantityBSmt = Convert.ToInt64(allInfo.TotalQuantityBSmt);
                                batterySmt.LineCapacity = quryForSmtLine.CKD_SMT_Line_Capacity;
                                batterySmt.LineInformation_Id = quryForSmtLine.Id;

                                if (query13 != null)
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                    Convert.ToInt64(allInfo.BatterySmtPerDayCapacity);

                                    //if (allInfo.BatterySmtPerDayCapacity > query13.LineAvailableCapacity)
                                    //{
                                    //    batterySmt.LineAvailableCapacity = 0;
                                    //}
                                }
                                else
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(allInfo.BatterySmtPerDayCapacity);
                                }
                                batterySmt.IsActive = true;
                                batterySmt.LineNumber = spVal.Trim();
                                batterySmt.Added = userId;
                                batterySmt.AddedDate = DateTime.Now;
                                _dbEntities.BatterySMTLineCapacityDetails.Add(batterySmt);
                                _dbEntities.SaveChanges();

                                var remainder1 = batterySmt.TotalQuantityBSmt % batterySmt.PerDayCapacity;

                                if (smtDate == datesSmt11.Max() && remainder1 > 0)
                                {
                                    if (query13 != null)
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(remainder1);
                                    }
                                    else
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(remainder1);
                                    }
                                    batterySmt.IsActive = true;
                                    _dbEntities.BatterySMTLineCapacityDetails.Add(batterySmt);
                                    _dbEntities.SaveChanges();
                                }
                            }

                            // }
                        }
                    }
                }
            }

            #endregion //end Smt Mass Line//

            #region Housing
            var updatedHousing = (from c in _dbEntities.BatteryHousings
                                  where c.ProjectId == allInfo.AsmProjectId && c.PlanId == allInfo.BhPlanId && c.Id == allInfo.BhId
                                  select c).FirstOrDefault();

            if (updatedHousing != null)
            {
                if (updatedHousing.MaterialReceiveStartDateBHousing != null)
                {
                    updatedHousing.MaterialReceiveStartDateBHousing = allInfo.MaterialReceiveStartDateBHousing;
                    updatedHousing.MaterialReceiveEndDateBHousing = allInfo.MaterialReceiveEndDateBHousing;
                    updatedHousing.IqcCompleteStartDateBHousing = allInfo.IqcCompleteStartDateBHousing;
                    updatedHousing.IqcCompleteEndDateBHousing = allInfo.IqcCompleteEndDateBHousing;
                    updatedHousing.TrialProductionStartDateBHousing = allInfo.TrialProductionStartDateBHousing;
                    updatedHousing.TrialProductionEndDateBHousing = allInfo.TrialProductionEndDateBHousing;
                    updatedHousing.HousingReliabilityTestStartDateBHousing = allInfo.HousingReliabilityTestStartDateBHousing;
                    updatedHousing.HousingReliabilityTestEndDateBHousing = allInfo.HousingReliabilityTestEndDateBHousing;
                    updatedHousing.HousingMassProductionStartDateBHousing = allInfo.HousingMassProductionStartDateBHousing;
                    updatedHousing.HousingMassProductionEndDateBHousing = allInfo.HousingMassProductionEndDateBHousing;
                    updatedHousing.TotalQuantity = Convert.ToInt64(allInfo.TotalQuantity);
                    updatedHousing.Updated = userId;
                    updatedHousing.UpdatedDate = DateTime.Now;
                    updatedHousing.IsActive = true;
                    ////housing Trial////
                    var trialLineNumHou = allInfo.HousingTrialLine;
                    var stroreStringHouTrialSplit = trialLineNumHou.Split(' ').ToList();
                    stroreStringHouTrialSplit = trialLineNumHou.Split(',').ToList();

                    var remove = (from c in _dbEntities.AllTrialInfoes
                                  where c.HousingTrialId == allInfo.BhId && c.PlanId == allInfo.BhPlanId
                                  select c).ToList();

                    if (remove.Count != 0)
                    {
                        _dbEntities.AllTrialInfoes.RemoveRange(remove.ToList());
                        _dbEntities.SaveChanges();
                    }

                    ////holiday///
                    var exe1 = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();
                    var datesSmtTrial11 = GetDatesBetween(Convert.ToDateTime(allInfo.TrialProductionStartDateBHousing), Convert.ToDateTime(allInfo.TrialProductionEndDateBHousing));
                    var datesSmtTrial12 = datesSmtTrial11.Except(exe1);
                    //holiday///

                    foreach (var spVal in stroreStringHouTrialSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesSmtTrial12)
                            {
                                AllTrialInfo trialInfo = new AllTrialInfo();
                                trialInfo.PlanId = allInfo.BhPlanId;
                                trialInfo.HousingTrialId = allInfo.BhId;
                                trialInfo.WorkingDate = smtDate;
                                trialInfo.TrialLineNumber = spVal.Trim();
                                trialInfo.Added = userId;
                                trialInfo.AddedDate = DateTime.Now;
                                trialInfo.IsActive = true;
                                _dbEntities.AllTrialInfoes.Add(trialInfo);
                                _dbEntities.SaveChanges();
                            }
                        }
                    }
                    ////end housing Trial////

                    ////housing Mass Line//// 
                    var smtMassLines = allInfo.HousingAllLineNumber;
                    var stroreStringSmtMassSplit = smtMassLines.Split(' ').ToList();
                    stroreStringSmtMassSplit = smtMassLines.Split(',').ToList();

                    var removeSmtMass = (from c in _dbEntities.BatteryHousingLineCapacityDetails
                                         where c.BatteryHousing_Id == allInfo.BhId && c.PlanId == allInfo.BhPlanId
                                         select c).ToList();

                    if (removeSmtMass.Count != 0)
                    {
                        _dbEntities.BatteryHousingLineCapacityDetails.RemoveRange(removeSmtMass.ToList());
                        _dbEntities.SaveChanges();
                    }
                    var datesSmt11 = GetDatesBetween(Convert.ToDateTime(allInfo.HousingMassProductionStartDateBHousing), Convert.ToDateTime(allInfo.HousingMassProductionEndDateBHousing));
                    var datesSmt12 = datesSmt11.Except(exe1);


                    foreach (var spVal in stroreStringSmtMassSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesSmt12)
                            {
                                var quryForSmtLine = (from c in _dbEntities.LineInformations
                                                      join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                                      where c.LineNumber == spVal.Trim() && c.CKD_Housing_Line_Capacity > 0 && pm.ProjectMasterId == allInfo.AsmProjectId
                                                      select c).FirstOrDefault();

                                var query13 = (from c in _dbEntities.BatteryHousingLineCapacityDetails
                                               where c.LineNumber == spVal.Trim() && c.WorkingDate == smtDate && c.BatteryHousing_Id == allInfo.BhId
                                               select c).OrderByDescending(x => x.Id).FirstOrDefault();


                                BatteryHousingLineCapacityDetail batterySmt = new BatteryHousingLineCapacityDetail();
                                batterySmt.PlanId = allInfo.BhPlanId;
                                batterySmt.BatteryHousing_Id = allInfo.BhId;
                                batterySmt.WorkingDate = smtDate;
                                batterySmt.PerDayCapacity = allInfo.BatteryHousingPerDayCapacity;
                                batterySmt.TotalQuantity = Convert.ToInt64(allInfo.TotalQuantity);
                                batterySmt.LineCapacity = quryForSmtLine.CKD_Housing_Line_Capacity;
                                batterySmt.LineInformation_Id = quryForSmtLine.Id;

                                if (query13 != null)
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                    Convert.ToInt64(allInfo.BatteryHousingPerDayCapacity);
                                }
                                else
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(allInfo.BatteryHousingPerDayCapacity);
                                }
                                batterySmt.IsActive = true;
                                batterySmt.LineNumber = spVal.Trim();
                                batterySmt.Added = userId;
                                batterySmt.AddedDate = DateTime.Now;
                                _dbEntities.BatteryHousingLineCapacityDetails.Add(batterySmt);
                                _dbEntities.SaveChanges();

                                var remainder1 = batterySmt.TotalQuantity % batterySmt.PerDayCapacity;
                                if (smtDate == datesSmt12.Max() && remainder1 > 0)
                                {
                                    if (query13 != null)
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(remainder1);
                                    }
                                    else
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(remainder1);
                                    }
                                    batterySmt.IsActive = true;
                                    _dbEntities.BatteryHousingLineCapacityDetails.Add(batterySmt);
                                    _dbEntities.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            #endregion  //end Housing///

            #region Battery
            var updatedBattery = (from c in _dbEntities.Batteries
                                  where c.ProjectId == allInfo.AsmProjectId && c.PlanId == allInfo.BbPlanId && c.Id == allInfo.BbId
                                  select c).FirstOrDefault();

            if (updatedBattery != null)
            {
                if (updatedBattery.MaterialReceiveStartDateBattery != null)
                {
                    updatedBattery.MaterialReceiveStartDateBattery = allInfo.MaterialReceiveStartDateBattery;
                    updatedBattery.MaterialReceiveEndDateBattery = allInfo.MaterialReceiveEndDateBattery;
                    updatedBattery.IqcCompleteStartDateBattery = allInfo.IqcCompleteStartDateBattery;
                    updatedBattery.IqcCompleteEndDateBattery = allInfo.IqcCompleteEndDateBattery;
                    updatedBattery.TrialProductionStartDateBattery = allInfo.TrialProductionStartDateBattery;
                    updatedBattery.TrialProductionEndDateBattery = allInfo.TrialProductionEndDateBattery;
                    updatedBattery.BatteryReliabilityTestStartDate = allInfo.BatteryReliabilityTestStartDate;
                    updatedBattery.BatteryReliabilityTestEndDate = allInfo.BatteryReliabilityTestEndDate;
                    updatedBattery.BatteryMassProductionStartDate = allInfo.BatteryMassProductionStartDate;
                    updatedBattery.BatteryMassProductionEndDate = allInfo.BatteryMassProductionEndDate;
                    updatedBattery.TotalQuantityBattery = Convert.ToInt64(allInfo.TotalQuantityBattery);
                    updatedBattery.Updated = userId;
                    updatedBattery.UpdatedDate = DateTime.Now;
                    updatedBattery.IsActive = true;
                    ////battery Trial////
                    var trialLineNumBat = allInfo.BatteryTrialLine;
                    var stroreStringHouTrialSplit = trialLineNumBat.Split(' ').ToList();
                    stroreStringHouTrialSplit = trialLineNumBat.Split(',').ToList();

                    var remove = (from c in _dbEntities.AllTrialInfoes
                                  where c.BatteryTrialId == allInfo.BbId && c.PlanId == allInfo.BbPlanId
                                  select c).ToList();

                    if (remove.Count != 0)
                    {
                        _dbEntities.AllTrialInfoes.RemoveRange(remove.ToList());
                        _dbEntities.SaveChanges();
                    }

                    ////holiday///
                    var exe1 = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();
                    var datesSmtTrial11 = GetDatesBetween(Convert.ToDateTime(allInfo.TrialProductionStartDateBattery), Convert.ToDateTime(allInfo.TrialProductionEndDateBattery));
                    var datesSmtTrial12 = datesSmtTrial11.Except(exe1);
                    //holiday///

                    foreach (var spVal in stroreStringHouTrialSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesSmtTrial12)
                            {
                                AllTrialInfo trialInfo = new AllTrialInfo();
                                trialInfo.PlanId = allInfo.BbPlanId;
                                trialInfo.BatteryTrialId = allInfo.BbId;
                                trialInfo.WorkingDate = smtDate;
                                trialInfo.TrialLineNumber = spVal.Trim();
                                trialInfo.Added = userId;
                                trialInfo.AddedDate = DateTime.Now;
                                trialInfo.IsActive = true;
                                _dbEntities.AllTrialInfoes.Add(trialInfo);
                                _dbEntities.SaveChanges();
                            }
                        }
                    }
                    ////end Battery Trial////

                    ////Battery Mass Line//// 
                    var smtMassLines = allInfo.BatteryAllLineNumber;
                    var stroreStringSmtMassSplit = smtMassLines.Split(' ').ToList();
                    stroreStringSmtMassSplit = smtMassLines.Split(',').ToList();

                    var removeSmtMass = (from c in _dbEntities.BatteryLineCapacityDetails
                                         where c.Battery_Id == allInfo.BbId && c.PlanId == allInfo.BbPlanId
                                         select c).ToList();

                    if (removeSmtMass.Count != 0)
                    {
                        _dbEntities.BatteryLineCapacityDetails.RemoveRange(removeSmtMass.ToList());
                        _dbEntities.SaveChanges();
                    }
                    var datesSmt11 = GetDatesBetween(Convert.ToDateTime(allInfo.BatteryMassProductionStartDate), Convert.ToDateTime(allInfo.BatteryMassProductionEndDate));
                    var datesSmt12 = datesSmt11.Except(exe1);

                    foreach (var spVal in stroreStringSmtMassSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesSmt12)
                            {
                                var quryForSmtLine = (from c in _dbEntities.LineInformations
                                                      join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                                      where c.LineNumber == spVal.Trim() && c.CKD_Battery_Line_Capacity > 0 && pm.ProjectMasterId == allInfo.AsmProjectId
                                                      select c).FirstOrDefault();

                                var query13 = (from c in _dbEntities.BatteryLineCapacityDetails
                                               where c.LineNumber == spVal.Trim() && c.WorkingDate == smtDate && c.Battery_Id == allInfo.BbId
                                               select c).OrderByDescending(x => x.Id).FirstOrDefault();


                                BatteryLineCapacityDetail batterySmt = new BatteryLineCapacityDetail();
                                batterySmt.PlanId = allInfo.BbPlanId;
                                batterySmt.Battery_Id = allInfo.BbId;
                                batterySmt.WorkingDate = smtDate;
                                batterySmt.PerDayCapacity = allInfo.BatteryPerDayCapacity;
                                batterySmt.TotalQuantityBattery = Convert.ToInt64(allInfo.TotalQuantityBattery);
                                batterySmt.LineCapacity = quryForSmtLine.CKD_Battery_Line_Capacity;
                                batterySmt.LineInformation_Id = quryForSmtLine.Id;

                                if (query13 != null)
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                    Convert.ToInt64(allInfo.BatteryPerDayCapacity);
                                }
                                else
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(allInfo.BatteryPerDayCapacity);
                                }
                                batterySmt.IsActive = true;
                                batterySmt.LineNumber = spVal.Trim();
                                batterySmt.Added = userId;
                                batterySmt.AddedDate = DateTime.Now;
                                _dbEntities.BatteryLineCapacityDetails.Add(batterySmt);
                                _dbEntities.SaveChanges();

                                var remainder1 = batterySmt.TotalQuantityBattery % batterySmt.PerDayCapacity;
                                if (smtDate == datesSmt12.Max() && remainder1 > 0)
                                {
                                    if (query13 != null)
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(remainder1);
                                    }
                                    else
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(remainder1);
                                    }
                                    batterySmt.IsActive = true;
                                    _dbEntities.BatteryLineCapacityDetails.Add(batterySmt);
                                    _dbEntities.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Assembly & Packing
            var updatedAssembly = (from c in _dbEntities.BatteryAssemblyAndPackings
                                   where c.ProjectId == allInfo.AsmProjectId && c.PlanId == allInfo.AsmPlanId && c.Id == allInfo.AsmId
                                   select c).FirstOrDefault();

            if (updatedAssembly != null)
            {
                if (updatedAssembly.MaterialReceiveStartDateBAssembly != null)
                {
                    updatedAssembly.MaterialReceiveStartDateBAssembly = allInfo.MaterialReceiveStartDateBAssembly;
                    updatedAssembly.MaterialReceiveEndDateBAssembly = allInfo.MaterialReceiveEndDateBAssembly;
                    updatedAssembly.IqcCompleteStartDateBAssembly = allInfo.IqcCompleteStartDateBAssembly;
                    updatedAssembly.IqcCompleteEndDateBAssembly = allInfo.IqcCompleteEndDateBAssembly;
                    updatedAssembly.TrialProductionStartDateBAssembly = allInfo.TrialProductionStartDateBAssembly;
                    updatedAssembly.TrialProductionEndDateBAssembly = allInfo.TrialProductionEndDateBAssembly;
                    updatedAssembly.SoftwareConfirmationStartDateBAssembly = allInfo.SoftwareConfirmationStartDateBAssembly;
                    updatedAssembly.SoftwareConfirmationEndDateBAssembly = allInfo.SoftwareConfirmationEndDateBAssembly;
                    updatedAssembly.RandDConfirmationStartDateBAssembly = allInfo.RandDConfirmationStartDateBAssembly;
                    updatedAssembly.RandDConfirmationEndDateBAssembly = allInfo.RandDConfirmationEndDateBAssembly;
                    updatedAssembly.AssemblyMassProductionStartDateBAssembly = allInfo.AssemblyMassProductionStartDateBAssembly;
                    updatedAssembly.AssemblyMassProductionEndDateBAssembly = allInfo.AssemblyMassProductionEndDateBAssembly;
                    updatedAssembly.TotalQuantityBAssembly = Convert.ToInt64(allInfo.TotalQuantityBAssembly);
                    updatedAssembly.PackingMassProductionStartDateBAssembly = allInfo.PackingMassProductionStartDateBAssembly;
                    updatedAssembly.PackingMassProductionEndDateBAssembly = allInfo.PackingMassProductionEndDateBAssembly;
                    updatedAssembly.TotalQuantityBPacking = Convert.ToInt64(allInfo.TotalQuantityBPacking);
                    updatedAssembly.Updated = userId;
                    updatedAssembly.UpdatedDate = DateTime.Now;
                    updatedAssembly.IsActive = true;
                    ////battery Trial////
                    var trialLineNumBat = allInfo.AssemblyTrialLine;
                    var stroreStringHouTrialSplit = trialLineNumBat.Split(' ').ToList();
                    stroreStringHouTrialSplit = trialLineNumBat.Split(',').ToList();

                    var remove = (from c in _dbEntities.AllTrialInfoes
                                  where c.BatteryTrialId == allInfo.AsmId && c.PlanId == allInfo.AsmPlanId
                                  select c).ToList();

                    if (remove.Count != 0)
                    {
                        _dbEntities.AllTrialInfoes.RemoveRange(remove.ToList());
                        _dbEntities.SaveChanges();
                    }

                    ////holiday///
                    var exe1 = GetHolidayDatasList().Where(i => i.HolidayDate != null).Select(i => (DateTime)i.HolidayDate).ToList();
                    var datesSmtTrial11 = GetDatesBetween(Convert.ToDateTime(allInfo.TrialProductionStartDateBAssembly), Convert.ToDateTime(allInfo.TrialProductionEndDateBAssembly));
                    var datesSmtTrial12 = datesSmtTrial11.Except(exe1);
                    //holiday///

                    foreach (var spVal in stroreStringHouTrialSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesSmtTrial12)
                            {
                                AllTrialInfo trialInfo = new AllTrialInfo();
                                trialInfo.PlanId = allInfo.AsmPlanId;
                                trialInfo.AssemblyTrialId = allInfo.AsmId;
                                trialInfo.WorkingDate = smtDate;
                                trialInfo.TrialLineNumber = spVal.Trim();
                                trialInfo.Added = userId;
                                trialInfo.AddedDate = DateTime.Now;
                                trialInfo.IsActive = true;
                                _dbEntities.AllTrialInfoes.Add(trialInfo);
                                _dbEntities.SaveChanges();
                            }
                        }
                    }
                    ////end Assembly Trial////

                    ////Assembly Mass Line//// 
                    var smtMassLines = allInfo.AssemblyAllLineNumber;
                    var stroreStringSmtMassSplit = smtMassLines.Split(' ').ToList();
                    stroreStringSmtMassSplit = smtMassLines.Split(',').ToList();

                    var removeSmtMass = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                         where c.BatteryAssemblyId == allInfo.AsmId && c.PlanId == allInfo.AsmPlanId
                                         select c).ToList();

                    if (removeSmtMass.Count != 0)
                    {
                        _dbEntities.BatteryAssemblyLineCapacityDetails.RemoveRange(removeSmtMass.ToList());
                        _dbEntities.SaveChanges();
                    }
                    var datesSmt11 = GetDatesBetween(Convert.ToDateTime(allInfo.AssemblyMassProductionStartDateBAssembly), Convert.ToDateTime(allInfo.AssemblyMassProductionEndDateBAssembly));
                    var datesSmt12 = datesSmt11.Except(exe1);

                    foreach (var spVal in stroreStringSmtMassSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesSmt12)
                            {
                                var quryForSmtLine = (from c in _dbEntities.LineInformations
                                                      join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                                      where c.LineNumber == spVal.Trim() && c.CKD_Assembly_Line_Capacity > 0 && pm.ProjectMasterId == allInfo.AsmProjectId
                                                      select c).FirstOrDefault();

                                var query13 = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                               where c.LineNumber == spVal.Trim() && c.WorkingDate == smtDate && c.BatteryAssemblyId == allInfo.AsmId
                                               select c).OrderByDescending(x => x.Id).FirstOrDefault();


                                BatteryAssemblyLineCapacityDetail batterySmt = new BatteryAssemblyLineCapacityDetail();
                                batterySmt.PlanId = allInfo.AsmPlanId;
                                batterySmt.BatteryAssemblyId = allInfo.AsmId;
                                batterySmt.WorkingDate = smtDate;
                                batterySmt.PerDayCapacity = allInfo.BatteryAssemblyPerDayCapacity;
                                batterySmt.TotalQuantityBAssembly = Convert.ToInt64(allInfo.TotalQuantityBAssembly);
                                batterySmt.LineCapacity = quryForSmtLine.CKD_Assembly_Line_Capacity;
                                batterySmt.LineInformation_Id = quryForSmtLine.Id;

                                if (query13 != null)
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                    Convert.ToInt64(allInfo.BatteryAssemblyPerDayCapacity);
                                }
                                else
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(allInfo.BatteryAssemblyPerDayCapacity);
                                }
                                batterySmt.IsActive = true;
                                batterySmt.LineNumber = spVal.Trim();
                                batterySmt.Added = userId;
                                batterySmt.AddedDate = DateTime.Now;
                                _dbEntities.BatteryAssemblyLineCapacityDetails.Add(batterySmt);
                                _dbEntities.SaveChanges();

                                var remainder1 = batterySmt.TotalQuantityBAssembly % batterySmt.PerDayCapacity;
                                if (smtDate == datesSmt12.Max() && remainder1 > 0)
                                {
                                    if (query13 != null)
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(remainder1);
                                    }
                                    else
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(remainder1);
                                    }
                                    batterySmt.IsActive = true;
                                    _dbEntities.BatteryAssemblyLineCapacityDetails.Add(batterySmt);
                                    _dbEntities.SaveChanges();
                                }
                            }
                        }
                    }//End Assembly

                    ////Packing Mass Line//// 
                    var pacMassLines = allInfo.PackingAllLineNumber;
                    var stroreStringPackMassSplit = pacMassLines.Split(' ').ToList();
                    stroreStringPackMassSplit = pacMassLines.Split(',').ToList();

                    var removePackMass = (from c in _dbEntities.BatteryPackingLineCapacityDetails
                                          where c.BatteryPackingId == allInfo.AsmId && c.PlanId == allInfo.AsmPlanId
                                          select c).ToList();

                    if (removePackMass.Count != 0)
                    {
                        _dbEntities.BatteryPackingLineCapacityDetails.RemoveRange(removePackMass.ToList());
                        _dbEntities.SaveChanges();
                    }
                    var datesPac11 = GetDatesBetween(Convert.ToDateTime(allInfo.PackingMassProductionStartDateBAssembly), Convert.ToDateTime(allInfo.PackingMassProductionEndDateBAssembly));
                    var datesPac12 = datesPac11.Except(exe1);

                    foreach (var spVal in stroreStringPackMassSplit)
                    {
                        if (spVal.Trim() != "")
                        {
                            foreach (var smtDate in datesPac12)
                            {
                                var quryForSmtLine = (from c in _dbEntities.LineInformations
                                                      join pm in _dbEntities.ProjectMasters on c.ProjectType equals pm.ProjectType
                                                      where c.LineNumber == spVal.Trim() && c.CKD_Packing_Line_Capacity > 0 && pm.ProjectMasterId == allInfo.AsmProjectId
                                                      select c).FirstOrDefault();

                                var query13 = (from c in _dbEntities.BatteryPackingLineCapacityDetails
                                               where c.LineNumber == spVal.Trim() && c.WorkingDate == smtDate && c.BatteryPackingId == allInfo.AsmId
                                               select c).OrderByDescending(x => x.Id).FirstOrDefault();


                                BatteryPackingLineCapacityDetail batterySmt = new BatteryPackingLineCapacityDetail();
                                batterySmt.PlanId = allInfo.AsmPlanId;
                                batterySmt.BatteryPackingId = allInfo.AsmId;
                                batterySmt.WorkingDate = smtDate;
                                batterySmt.PerDayCapacity = allInfo.BatteryPackingPerDayCapacity;
                                batterySmt.TotalQuantityBPacking = Convert.ToInt64(allInfo.TotalQuantityBPacking);
                                batterySmt.LineCapacity = quryForSmtLine.CKD_Packing_Line_Capacity;
                                batterySmt.LineInformation_Id = quryForSmtLine.Id;

                                if (query13 != null)
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                    Convert.ToInt64(allInfo.BatteryPackingPerDayCapacity);
                                }
                                else
                                {
                                    batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(allInfo.BatteryPackingPerDayCapacity);
                                }
                                batterySmt.IsActive = true;
                                batterySmt.LineNumber = spVal.Trim();
                                batterySmt.Added = userId;
                                batterySmt.AddedDate = DateTime.Now;
                                _dbEntities.BatteryPackingLineCapacityDetails.Add(batterySmt);
                                _dbEntities.SaveChanges();

                                var remainder1 = batterySmt.TotalQuantityBPacking % batterySmt.PerDayCapacity;
                                if (smtDate == datesPac12.Max() && remainder1 > 0)
                                {
                                    if (query13 != null)
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(query13.LineAvailableCapacity) -
                                                                        Convert.ToInt64(remainder1);
                                    }
                                    else
                                    {
                                        batterySmt.LineAvailableCapacity = Convert.ToInt64(batterySmt.LineCapacity) - Convert.ToInt64(remainder1);
                                    }
                                    batterySmt.IsActive = true;
                                    _dbEntities.BatteryPackingLineCapacityDetails.Add(batterySmt);
                                    _dbEntities.SaveChanges();
                                }
                            }
                        }
                    }//End Packing
                }
            }
            #endregion

            _dbEntities.SaveChanges();
            return "ok";
        }

        public List<CustomBatteryProduction> GetSelectedProjectPlanningHistory(long proIds, string projectName)
        {
            string query = string.Format(@"
            select  case when asm.IsActive=1 then 'ACTIVE' else 'INACTIVE' end as ActiveStatus,asm.ProjectId,asm.PlanId as AsmPlanId,bb.PlanId as BbPlanId, bh.PlanId as BhPlanId,smt.PlanId as SmtPlanId,smt.Id as SmtId,asm.Id as AsmId,bb.Id as BbId,bh.Id as BhId,asm.ProjectName as AsmProjectName,asm.ProjectId as AsmProjectId,asm.MaterialReceiveStartDateBAssembly,asm.MaterialReceiveEndDateBAssembly,asm.IqcCompleteStartDateBAssembly,asm.IqcCompleteEndDateBAssembly,asm.TrialProductionStartDateBAssembly,asm.TrialProductionEndDateBAssembly,
            asm.SoftwareConfirmationStartDateBAssembly,asm.SoftwareConfirmationEndDateBAssembly,asm.RandDConfirmationStartDateBAssembly,asm.RandDConfirmationEndDateBAssembly,
            asm.AssemblyMassProductionStartDateBAssembly,asm.AssemblyMassProductionEndDateBAssembly,asm.PackingMassProductionStartDateBAssembly,asm.PackingMassProductionEndDateBAssembly,
            smt.MaterialReceiveStartDateBSmt,smt.MaterialReceiveEndDateBSmt,smt.IqcCompleteStartDateBSmt,smt.IqcCompleteEndDateBSmt,smt.TrialProductionStartDateBSmt,smt.TrialProductionEndDateBSmt,
            smt.SmtMassProductionStartDateBSmt,smt.SmtMassProductionEndDateBSmt,
            bb.MaterialReceiveStartDateBattery,bb.MaterialReceiveEndDateBattery,bb.IqcCompleteStartDateBattery,bb.IqcCompleteEndDateBattery,bb.TrialProductionStartDateBattery,bb.TrialProductionEndDateBattery,
            bb.BatteryReliabilityTestStartDate,bb.BatteryReliabilityTestEndDate,bb.BatteryMassProductionStartDate,bb.BatteryMassProductionEndDate,bb.BatteryAgingTestStartDate,bb.BatteryAgingTestEndDate,
            bh.MaterialReceiveStartDateBHousing,bh.MaterialReceiveEndDateBHousing,bh.IqcCompleteStartDateBHousing,bh.IqcCompleteEndDateBHousing,bh.TrialProductionStartDateBHousing,bh.TrialProductionEndDateBHousing,
            bh.HousingReliabilityTestStartDateBHousing,bh.HousingReliabilityTestEndDateBHousing,bh.HousingMassProductionStartDateBHousing,bh.HousingMassProductionEndDateBHousing,
            CAST(smt.TotalQuantityBSmt AS VARCHAR(10)) as TotalQuantityBSmt,CAST(bh.TotalQuantity AS VARCHAR(10)) as TotalQuantity,CAST(bb.TotalQuantityBattery AS VARCHAR(10)) as TotalQuantityBattery,
            CAST(asm.TotalQuantityBAssembly AS VARCHAR(10)) as TotalQuantityBAssembly, CAST(asm.TotalQuantityBPacking AS VARCHAR(10)) as TotalQuantityBPacking

            FROM [CellPhoneProject].[dbo].[BatteryAssemblyAndPacking] asm
            left join CellPhoneProject.dbo.[BatterySMT] smt on smt.ProjectId=asm.ProjectId and smt.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[Battery] bb on bb.ProjectId=asm.ProjectId and bb.PlanId=asm.PlanId
            left join CellPhoneProject.dbo.[BatteryHousing] bh on bh.ProjectId=asm.ProjectId and bh.PlanId=asm.PlanId
            where asm.ProjectId='{0}'
            order by asm.AddedDate desc", proIds);


            var exe = _dbEntities.Database.SqlQuery<CustomBatteryProduction>(query).ToList();

            foreach (var custBatteryPro in exe)
            {
                //Smt////////////////////
                var qqty1 = (from als in _dbEntities.AllTrialInfoes
                             where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBSmt &&
                                 als.WorkingDate <= custBatteryPro.TrialProductionEndDateBSmt && als.SmtTrialId == custBatteryPro.SmtId
                             select als);

                var trialLineNumbers = String.Join(", ", qqty1.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.SmtTrialLine = trialLineNumbers;

                var qqty2 = (from bsct in _dbEntities.BatterySMTLineCapacityDetails
                             where bsct.WorkingDate >= custBatteryPro.SmtMassProductionStartDateBSmt &&
                                 bsct.WorkingDate <= custBatteryPro.SmtMassProductionEndDateBSmt && bsct.BatterySMT_Id == custBatteryPro.SmtId
                             select bsct);

                var smtLine = String.Join(", ", qqty2.Select(p => p.LineNumber).Distinct());
                var smtPerDayCap = (qqty2.Select(p => p.PerDayCapacity).FirstOrDefault());
                var smtLineCap = (qqty2.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.SmtAllLineNumber = smtLine;
                custBatteryPro.SmtAllLineCapacity = Convert.ToString(smtLineCap);
                custBatteryPro.BatterySmtPerDayCapacity = Convert.ToInt64(smtPerDayCap);
                //emd Smt////////////
                /////Housing////////////////
                var qqty1h = (from als in _dbEntities.AllTrialInfoes
                              where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBHousing &&
                                  als.WorkingDate <= custBatteryPro.TrialProductionEndDateBHousing && als.HousingTrialId == custBatteryPro.BhId
                              select als);

                var trialLineNumbersh = String.Join(", ", qqty1h.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.HousingTrialLine = trialLineNumbersh;

                var qqty2h = (from bsct in _dbEntities.BatteryHousingLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.HousingMassProductionStartDateBHousing &&
                                 bsct.WorkingDate <= custBatteryPro.HousingMassProductionEndDateBHousing && bsct.BatteryHousing_Id == custBatteryPro.BhId
                              select bsct);

                var houLine = String.Join(", ", qqty2h.Select(p => p.LineNumber).Distinct());
                var houPerDayCap = (qqty2h.Select(p => p.PerDayCapacity).FirstOrDefault());
                var houLineCap = (qqty2h.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.HousingAllLineNumber = houLine;
                custBatteryPro.BatteryHousingPerDayCapacity = Convert.ToInt64(houPerDayCap);
                custBatteryPro.HousingAllLineCapacity = Convert.ToString(houLineCap);

                /////Battery////////////////
                var qqty1b = (from als in _dbEntities.AllTrialInfoes
                              where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBattery &&
                                  als.WorkingDate <= custBatteryPro.TrialProductionEndDateBattery && als.BatteryTrialId == custBatteryPro.BbId
                              select als);

                var trialLineNumbersb = String.Join(", ", qqty1b.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.BatteryTrialLine = trialLineNumbersb;

                var qqty2b = (from bsct in _dbEntities.BatteryLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.BatteryMassProductionStartDate &&
                                 bsct.WorkingDate <= custBatteryPro.BatteryMassProductionEndDate && bsct.Battery_Id == custBatteryPro.BbId
                              select bsct);

                var batLine = String.Join(", ", qqty2b.Select(p => p.LineNumber).Distinct());
                var batPerDayCap = (qqty2b.Select(p => p.PerDayCapacity).FirstOrDefault());
                var batLineCap = (qqty2b.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.BatteryAllLineNumber = batLine;
                custBatteryPro.BatteryPerDayCapacity = Convert.ToInt64(batPerDayCap);
                custBatteryPro.BatteryAllLineCapacity = Convert.ToString(batLineCap);

                /////Assembly////////////////
                var qqty1a = (from als in _dbEntities.AllTrialInfoes
                              where als.WorkingDate >= custBatteryPro.TrialProductionStartDateBAssembly &&
                                  als.WorkingDate <= custBatteryPro.TrialProductionEndDateBAssembly && als.AssemblyTrialId == custBatteryPro.AsmId
                              select als);

                var trialLineNumbersa = String.Join(", ", qqty1a.Select(p => p.TrialLineNumber).Distinct());

                custBatteryPro.AssemblyTrialLine = trialLineNumbersa;

                var qqty2a = (from bsct in _dbEntities.BatteryAssemblyLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.AssemblyMassProductionStartDateBAssembly &&
                              bsct.WorkingDate <= custBatteryPro.AssemblyMassProductionEndDateBAssembly && bsct.BatteryAssemblyId == custBatteryPro.AsmId
                              select bsct);

                var asmLine = String.Join(", ", qqty2a.Select(p => p.LineNumber).Distinct());
                var asmPerDayCap = (qqty2a.Select(p => p.PerDayCapacity).FirstOrDefault());
                var asmLineCap = (qqty2a.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.AssemblyAllLineNumber = asmLine;
                custBatteryPro.BatteryAssemblyPerDayCapacity = Convert.ToInt64(asmPerDayCap);
                custBatteryPro.AssemblyAllLineCapacity = Convert.ToString(asmLineCap);

                /////Packing////////////////


                var qqty2p = (from bsct in _dbEntities.BatteryPackingLineCapacityDetails
                              where bsct.WorkingDate >= custBatteryPro.PackingMassProductionStartDateBAssembly &&
                              bsct.WorkingDate <= custBatteryPro.PackingMassProductionEndDateBAssembly && bsct.BatteryPackingId == custBatteryPro.AsmId
                              select bsct);

                var pacLine = String.Join(", ", qqty2p.Select(p => p.LineNumber).Distinct());
                var pacPerDayCap = (qqty2p.Select(p => p.PerDayCapacity).FirstOrDefault());
                var pacLineCap = (qqty2p.Select(p => p.LineCapacity).FirstOrDefault());

                custBatteryPro.PackingAllLineNumber = pacLine;
                custBatteryPro.BatteryPackingPerDayCapacity = Convert.ToInt64(pacPerDayCap);
                custBatteryPro.PackingAllLineCapacity = Convert.ToString(pacLineCap);

            }
            return exe;

        }

        public string InActiveAPlan(long proIds, long planId)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            //
            var plantbl = (from c in _dbEntities.ProPlanTables
                           where c.PlanId == planId
                           select c).FirstOrDefault();


            plantbl.IsActive = false;
            _dbEntities.ProPlanTables.AddOrUpdate(plantbl);
            _dbEntities.SaveChanges();
            //
            var assembly = (from c in _dbEntities.BatteryAssemblyAndPackings
                            where c.PlanId == planId
                            select c).FirstOrDefault();


            assembly.IsActive = false;
            _dbEntities.BatteryAssemblyAndPackings.AddOrUpdate(assembly);
            _dbEntities.SaveChanges();
            //
            var assemblyLine = (from c in _dbEntities.BatteryAssemblyLineCapacityDetails
                                where c.PlanId == planId
                                select c).ToList();

            foreach (var aa in assemblyLine)
            {
                aa.IsActive = false;
                _dbEntities.BatteryAssemblyLineCapacityDetails.AddOrUpdate(aa);
                _dbEntities.SaveChanges();
            }

            //
            var packingLine = (from c in _dbEntities.BatteryPackingLineCapacityDetails
                               where c.PlanId == planId
                               select c).ToList();


            foreach (var aa in packingLine)
            {
                aa.IsActive = false;
                _dbEntities.BatteryPackingLineCapacityDetails.AddOrUpdate(aa);
                _dbEntities.SaveChanges();
            }

            //

            var battery = (from c in _dbEntities.Batteries
                           where c.PlanId == planId
                           select c).FirstOrDefault();


            if (battery != null)
            {
                battery.IsActive = false;
                _dbEntities.Batteries.AddOrUpdate(battery);
                _dbEntities.SaveChanges();
            }

            //
            var batteryLine = (from c in _dbEntities.BatteryLineCapacityDetails
                               where c.PlanId == planId
                               select c).ToList();

            foreach (var aa in batteryLine)
            {
                aa.IsActive = false;
                _dbEntities.BatteryLineCapacityDetails.AddOrUpdate(aa);
                _dbEntities.SaveChanges();
            }

            //
            var housing = (from c in _dbEntities.BatteryHousings
                           where c.PlanId == planId
                           select c).FirstOrDefault();

            if (housing != null)
            {
                housing.IsActive = false;
                _dbEntities.BatteryHousings.AddOrUpdate(housing);
                _dbEntities.SaveChanges();
            }

            //
            var housingLine = (from c in _dbEntities.BatteryHousingLineCapacityDetails
                               where c.PlanId == planId
                               select c).ToList();

            foreach (var aa in housingLine)
            {
                aa.IsActive = false;
                _dbEntities.BatteryHousingLineCapacityDetails.AddOrUpdate(aa);
                _dbEntities.SaveChanges();
            }

            //
            var smt = (from c in _dbEntities.BatterySMTs
                       where c.PlanId == planId
                       select c).FirstOrDefault();

            if (smt != null)
            {
                smt.IsActive = false;
                _dbEntities.BatterySMTs.AddOrUpdate(smt);
                _dbEntities.SaveChanges();
            }

            //
            var smtLine = (from c in _dbEntities.BatterySMTLineCapacityDetails
                           where c.PlanId == planId
                           select c).ToList();

            foreach (var aa in smtLine)
            {
                aa.IsActive = false;
                _dbEntities.BatterySMTLineCapacityDetails.AddOrUpdate(aa);
                _dbEntities.SaveChanges();
            }
            //
            var trial = (from c in _dbEntities.AllTrialInfoes
                         where c.PlanId == planId
                         select c).ToList();

            foreach (var aa in trial)
            {
                aa.IsActive = false;
                _dbEntities.AllTrialInfoes.AddOrUpdate(aa);
                _dbEntities.SaveChanges();
            }

            return "ok";
        }

        public List<GovernmentHolidayTableModel> GetHoliday()
        {
            var query = _dbEntities.Database.SqlQuery<GovernmentHolidayTableModel>(@"
             select * from [CellPhoneProject].[dbo].[GovernmentHolidayTable]").ToList();
            return query;
        }

        public string SaveHolidayDropData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            if (id == null)
            {
                var models = new GovernmentHolidayTable();
                models.GovernmentHoliday = governmentHoliday;
                models.HolidayDate = Convert.ToDateTime(holidayStartDate);
                models.HolidayStartDate = Convert.ToDateTime(holidayStartDate);
                models.HolidayEndDate = Convert.ToDateTime(holidayEndDate);
                models.Added = userId;
                models.AddedDate = DateTime.Now;

                _dbEntities.GovernmentHolidayTables.Add(models);
                _dbEntities.SaveChanges();
            }
            else
            {
                long ids;
                long.TryParse(id, out ids);

                var model = _dbEntities.GovernmentHolidayTables.FirstOrDefault(x => x.Id == ids);
                model.GovernmentHoliday = governmentHoliday;
                model.HolidayDate = Convert.ToDateTime(holidayStartDate);
                model.HolidayStartDate = Convert.ToDateTime(holidayStartDate);
                model.HolidayEndDate = Convert.ToDateTime(holidayEndDate);
                model.Updated = userId;
                model.UpdatedDate = DateTime.Now;

                _dbEntities.SaveChanges();
            }

            return "ok";
        }

        public string DeleteHolidayData(string id)
        {
            long ids;
            long.TryParse(id, out ids);
            var deleteEvents = (from c in _dbEntities.GovernmentHolidayTables
                                where c.Id == ids
                                select c).FirstOrDefault();

            _dbEntities.GovernmentHolidayTables.Remove(deleteEvents);
            _dbEntities.SaveChanges();
            return "OK";
        }

        #endregion

        #region Capacity Planning

        public List<Pro_Type_Model> GetProductionType()
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Type_Model>(@"select * FROM [CellPhoneProject].[dbo].[Pro_Type] where IsActive=1").ToList();
            return query;
        }

        public string SaveShift(List<Pro_Shift_Model> issueList, int mon, string monName, int years, string productionType)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var proShift in issueList)
            {
                var model = new Pro_Shift();
                model.ProductionType = productionType;
                model.Month = monName;
                model.MonNum = mon;
                model.Year = years;
                model.Line = proShift.Line;
                model.Shift_1 = proShift.Shift_1;
                model.Shift_2 = proShift.Shift_2;
                model.Shift_3 = proShift.Shift_3;
                model.IsActive = true;

                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Pro_Shift.Add(model);
                _dbEntities.SaveChanges();
            }

            _dbEntities.SaveChanges();
            return "ok";
        }

        public List<Pro_Shift_Model> GetShiftSavedData(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select Id,Month,MonNum,ProductionType,Year,Line,Shift_1,case when Shift_2 in ('0',null) then NULL else Shift_2 end as Shift_2,
            case when Shift_3 in ('0',null) then NULL else Shift_3 end as Shift_3 from [CellPhoneProject].[dbo].[Pro_Shift] 
            where MonNum={0} and Year={1} and ProductionType={2} and IsActive=1  order by Line asc", mons, years, productionType).ToList();
            return query;
        }

        public List<Pro_Shift_Model> GetDailyShiftData(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"
            //	    	DECLARE @Month INT = {0}, @Year INT = {1};WITH MonthDays_CTE(DayNum)
            //	    AS
            //	    (
            //		    SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
            //			    UNION ALL
            //		    SELECT DATEADD(DAY, 1, DayNum)
            //		    FROM MonthDays_CTE
            //		    WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            //	    )
            //	    select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays,sf.Line,sf.Shift_1,sf.Shift_2,sf.Shift_3 
            //	
            //	    from
            //	    (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            //	    FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B
            //
            //	    left join [CellPhoneProject].[dbo].[Pro_Shift] sf  on sf.MonNum=B.dMonNum", mons, years, productionType).ToList();

            //neww
            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"	    	
            //	        DECLARE @Month INT = {0}, @Year INT = {1};
            //	        WITH MonthDays_CTE(DayNum)
            //	        AS
            //	        (
            //		        SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
            //			        UNION ALL
            //		        SELECT DATEADD(DAY, 1, DayNum)
            //		        FROM MonthDays_CTE
            //		        WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            //	        )
            //	        select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays
            //	
            //	        from
            //	        (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            //	        FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B  order by B.TotalDays asc", mons, years, productionType).ToList();

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"DECLARE @Month INT ={0}, @Year INT = {1};
            WITH MonthDays_CTE(DayNum)
            AS
            (
	            SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
		            UNION ALL
	            SELECT DATEADD(DAY, 1, DayNum)
	            FROM MonthDays_CTE
	            WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            )
            select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays,
            case when B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate then gt.GovernmentHoliday end as Holidays
            from
            (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B 
            left join  CellPhoneProject.dbo.GovernmentHolidayTable gt on B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate 
            order by B.TotalDays asc", mons, years, productionType).ToList();

            return query;
        }
        public List<Pro_Shift_Model> GetDailyShiftData1(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"
            //	    	DECLARE @Month INT = {0}, @Year INT = {1};WITH MonthDays_CTE(DayNum)
            //	    AS
            //	    (
            //		    SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
            //			    UNION ALL
            //		    SELECT DATEADD(DAY, 1, DayNum)
            //		    FROM MonthDays_CTE
            //		    WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            //	    )
            //	    select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays,sf.Line,sf.Shift_1,sf.Shift_2,sf.Shift_3 
            //	
            //	    from
            //	    (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            //	    FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B
            //
            //	    left join [CellPhoneProject].[dbo].[Pro_Shift] sf  on sf.MonNum=B.dMonNum", mons, years, productionType).ToList();

            //neww
            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"	    	
            //	        DECLARE @Month INT = {0}, @Year INT = {1};
            //	        WITH MonthDays_CTE(DayNum)
            //	        AS
            //	        (
            //		        SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
            //			        UNION ALL
            //		        SELECT DATEADD(DAY, 1, DayNum)
            //		        FROM MonthDays_CTE
            //		        WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            //	        )
            //	        select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays
            //	
            //	        from
            //	        (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            //	        FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B  order by B.TotalDays asc", mons, years, productionType).ToList();

            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"DECLARE @Month INT ={0}, @Year INT = {1};
            //            WITH MonthDays_CTE(DayNum)
            //            AS
            //            (
            //	            SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
            //		            UNION ALL
            //	            SELECT DATEADD(DAY, 1, DayNum)
            //	            FROM MonthDays_CTE
            //	            WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            //            )
            //            select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays,
            //            case when B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate then gt.GovernmentHoliday end as Holidays
            //            from
            //            (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            //            FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B 
            //            left join  CellPhoneProject.dbo.GovernmentHolidayTable gt on B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate 
            //            order by B.TotalDays asc", mons, years, productionType).ToList();

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"DECLARE @Month INT ={0}, @Year INT = {1};
        WITH MonthDays_CTE(DayNum)
        AS
        (
	        SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
		        UNION ALL
	        SELECT DATEADD(DAY, 1, DayNum)
	        FROM MonthDays_CTE
	        WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
        )
        select B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays,
case when B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate then gt.GovernmentHoliday end as Holidays,
        dp.ProductionType,dp.EffectiveDate, dp.Line,dp.Shift_1,
        case when dp.Shift_2 in ('0',null) then NULL else dp.Shift_2 end as Shift_2,
        case when dp.Shift_3 in ('0',null) then NULL else dp.Shift_3 end as Shift_3

        from
        (SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
        FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B 
        left join  CellPhoneProject.dbo.GovernmentHolidayTable gt on B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate 
        left join  [CellPhoneProject].[dbo].Pro_DailyPlan dp on dp.EffectiveDate=B.TotalDays
        and dp.MonNum={0} and dp.Year={1} and dp.ProductionType={2} 
        order by B.TotalDays asc", mons, years, productionType).ToList();


            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"DECLARE @Month INT ={0}, @Year INT = {1};
            //WITH MonthDays_CTE(DayNum)
            //AS
            //(
            //	SELECT DATEFROMPARTS(@Year, @Month, 1) AS DayNum
            //		UNION ALL
            //	SELECT DATEADD(DAY, 1, DayNum)
            //	FROM MonthDays_CTE
            //	WHERE DayNum < EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
            //)
            //
            //
            //select MainLine,TotalDays,AllDays,Holidays,ProductionType,EffectiveDate,Line,
            //Shift_1,Shift_2,Shift_3, 
            //case when MainLine !=Line then 'hi' end as Shift11
            //
            // from 
            //(select  pss.Line as MainLine,
            //B.TotalDays, (B.dDayName+', '+ B.dMonth+' '+ cast(B.dDay as varchar(10))+', '+ cast(B.dYear as varchar(10))) as AllDays,
            //case when B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate then gt.GovernmentHoliday end as Holidays,
            //dp.ProductionType,dp.EffectiveDate, dp.Line,dp.Shift_1,
            //case when dp.Shift_2 in ('0',null) then NULL else dp.Shift_2 end as Shift_2,
            //case when dp.Shift_3 in ('0',null) then NULL else dp.Shift_3 end as Shift_3
            //
            //from
            //(SELECT DayNum as TotalDays,DATEPART(YEAR,DayNum) as dYear,DATEPART(day,DayNum) as dDay, DATENAME(Month,DayNum) as dMonth, DATEPART(Month,DayNum) as dMonNum,
            //FORMAT(DayNum, 'dddd') as dDayName  FROM MonthDays_CTE A)B 
            //left join  CellPhoneProject.dbo.GovernmentHolidayTable gt on B.TotalDays between gt.HolidayStartDate and gt.HolidayEndDate 
            //left join  [CellPhoneProject].[dbo].Pro_DailyPlan dp on dp.EffectiveDate=B.TotalDays and dp.ProductionType={2} and dp.MonNum={0} and dp.Year={1}
            //left join  [CellPhoneProject].[dbo].[Pro_Shift]  pss on pss.Line is not null and pss.ProductionType={2} and pss.MonNum={0} and pss.Year={1} and pss.IsActive=1
            //
            //)K
            //
            //order by K.TotalDays asc", mons, years, productionType).ToList();

            return query;
        }

        public List<Pro_Shift_Model> DailySaved(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select * from [CellPhoneProject].[dbo].[Pro_DailyPlan] 
            where MonNum={0} and Year={1} and ProductionType={2} and IsActive=1 order by EffectiveDate asc", mons, years, productionType).ToList();
            return query;
        }


        public List<Pro_Shift_Model> GetLine(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select distinct Line from [CellPhoneProject].[dbo].[Pro_Shift] 
            where MonNum={0} and Year={1} and ProductionType={2} and IsActive=1 order by Line asc", mons, years, productionType).ToList();
            return query;
        }

        public List<Pro_Shift_Model> GetShift(int mons, string year, string productionType, string phoneType)
        {
            int years;
            int.TryParse(year, out years);


            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select AllShift from
            //            (
            //	            select Shift_1 as AllShift from [CellPhoneProject].[dbo].[Pro_Shift] 
            //	            where MonNum={0} and Year={1} and ProductionType={2}  and Shift_1 !='0'
            //	            union
            //	            select Shift_2 as AllShift from [CellPhoneProject].[dbo].[Pro_Shift] 
            //	            where MonNum={0} and Year={1} and ProductionType={2}  and Shift_2 !='0'
            //	            union
            //	            select Shift_3 as AllShift from [CellPhoneProject].[dbo].[Pro_Shift] 
            //	            where MonNum={0} and Year={1} and ProductionType={2}  and Shift_3 !='0'
            //	            union
            //	            select Shift_4 as AllShift from [CellPhoneProject].[dbo].[Pro_Shift] 
            //	            where MonNum={0} and Year={1} and ProductionType={2}  and Shift_4 !='0'
            //	            union
            //	            select Shift_5 as AllShift from [CellPhoneProject].[dbo].[Pro_Shift] 
            //	            where MonNum={0} and Year={1} and ProductionType={2} and Shift_5 !='0'
            //            )A where a.AllShift is not null", mons, years, productionType, phoneType).ToList();

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"
            select Team as AllShift FROM [CellPhoneProject].[dbo].[Pro_Team]
            where ProductionType={2} and IsActive=1 and Team is not null",
            mons, years, productionType, phoneType).ToList();

            return query;
        }

        public string SaveCapacityData(List<Pro_CapacityPlanning_Model> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            //_dbEntities.Pro_CapacityPlanning.Remove(presentData);
            //  _dbEntities.SaveChanges();

            foreach (var res in results)
            {
                var vSplit = res.AllShift.Split(',');

                var phoneType = Convert.ToString(vSplit[8]);
                var categories = Convert.ToString(vSplit[3]);
                var produtionType = Convert.ToString(vSplit[9]);
                var mon = Convert.ToInt32(vSplit[6]);
                var yearss = Convert.ToInt32(vSplit[5]);

                var presentData = (from pm in _dbEntities.Pro_CapacityPlanning
                                   where pm.ProductName == phoneType
                                       && pm.ProductionType == produtionType
                                       && pm.CategoryName == categories
                                       && pm.MonNum == mon
                                       && pm.Year == yearss
                                   select pm).ToList();

                foreach (var pd in presentData)
                {
                    _dbEntities.Pro_CapacityPlanning.Remove(pd);
                    _dbEntities.SaveChanges();
                }

                if (vSplit[3] == "null")
                {
                    vSplit[3] = "";
                }
                if (vSplit[4] == "null")
                {
                    vSplit[4] = "";
                }

                if (vSplit[3] != "" && vSplit[4] != "")
                {

                    var model = new Pro_CapacityPlanning();
                    model.Percentage = Convert.ToInt32(vSplit[0]);
                    model.QuantityRange = Convert.ToString(vSplit[1]);
                    model.Team = Convert.ToString(vSplit[2]);
                    model.CategoryName = Convert.ToString(vSplit[3]);
                    model.TotalCapacity = Convert.ToDecimal(vSplit[4]);
                    model.Year = Convert.ToInt32(vSplit[5]);
                    model.MonNum = Convert.ToInt32(vSplit[6]);
                    model.Month = Convert.ToString(vSplit[7]);
                    model.ProductName = Convert.ToString(vSplit[8]);
                    model.ProductionType = Convert.ToString(vSplit[9]);
                    model.Product = "Mobile";
                    model.Added = userId;
                    model.IsActive = true;
                    model.AddedDate = DateTime.Now;

                    _dbEntities.Pro_CapacityPlanning.Add(model);
                }


            }
            _dbEntities.SaveChanges();

            return "ok";
        }

        public List<Pro_CapacityPlanning_Model> GetCapacity(int mons, string year, string productionType, string categories)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"SELECT Id,Team, ProductionType,CategoryName,Percentage,QuantityRange,TotalCapacity,ProductName
            FROM [CellPhoneProject].[dbo].[Pro_CapacityPlanning] where MonNum={0} and Year={1} and ProductionType={2} and CategoryName={3}
            group by Id,Team, ProductionType,CategoryName,Percentage,QuantityRange,TotalCapacity,ProductName
            order by Team,CategoryName asc", mons, years, productionType, categories).ToList();

            return query;
        }

        public List<Pro_CapacityPlanning_Model> GetTeam(int mons, string year, string productionType, string phoneType, string categories)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"select distinct Team from
            [CellPhoneProject].[dbo].[Pro_CapacityPlanning] where MonNum={0} and Year={1} and ProductionType={2} and ProductName={3} and CategoryName={4} order by Team asc", mons, years, productionType, phoneType, categories).ToList();

            return query;
        }
        public List<Pro_CapacityPlanning_Model> GetPercentage(int mons, string year, string productionType, string phoneType, string categories)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"select distinct Percentage from
            [CellPhoneProject].[dbo].[Pro_CapacityPlanning] where MonNum={0} and Year={1} and ProductionType={2} and ProductName={3} and CategoryName={4}  order by Percentage desc", mons, years, productionType, phoneType, categories).ToList();

            return query;
        }

        public List<Pro_CapacityPlanning_Model> GetQuantityRange(int mons, string year, string productionType, string phoneType, string categories)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"select distinct QuantityRange,Percentage from
            [CellPhoneProject].[dbo].[Pro_CapacityPlanning] where MonNum={0} and Year={1} and ProductionType={2} and ProductName={3} and CategoryName={4} order by Percentage desc", mons, years, productionType, phoneType, categories).ToList();

            return query;
        }

        public List<Pro_CapacityPlanning_Model> GetAll(int mons, string year, string productionType, string phoneType, string categories)
        {
            int years;
            int.TryParse(year, out years);

            var query1 = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"SELECT DISTINCT A.MonNum,A.Month,A.Year,A.ProductName,A.ProductionType,A.Team, A.CategoryName,
            STUFF((SELECT ',' + cast(cast(TotalCapacity AS int) as varchar) FROM [CellPhoneProject].[dbo].[Pro_CapacityPlanning] p
            where MonNum={0} and Year={1} and ProductionType=A.ProductionType
            and ProductName=A.ProductName  and Team=A.Team and CategoryName=A.CategoryName order by Percentage desc
            FOR XML PATH('')),1,1,'') AS TotalCapacities 

            FROM [CellPhoneProject].[dbo].[Pro_CapacityPlanning] AS A where MonNum={0} 
            and Year={1} and ProductionType={2} and ProductName={3}  and CategoryName={4} ", mons, years, productionType, phoneType, categories).ToList();

            foreach (var pro in query1)
            {
                var dd = pro.TotalCapacities.Split(',');
                pro.Team = pro.Team;
                pro.CategoryName = pro.CategoryName;
                for (int i = 0; i < dd.Length; i++)
                {
                    if (i == 0)
                    {
                        pro.TotalCap1 = dd[i];
                    }
                    if (i == 1)
                    {
                        pro.TotalCap2 = dd[i];
                    }
                    if (i == 2)
                    {
                        pro.TotalCap3 = dd[i];
                    }
                    if (i == 3)
                    {
                        pro.TotalCap4 = dd[i];
                    }
                    if (i == 4)
                    {
                        pro.TotalCap5 = dd[i];
                    }
                    if (i == 5)
                    {
                        pro.TotalCap6 = dd[i];
                    }
                    if (i == 6)
                    {
                        pro.TotalCap7 = dd[i];
                    }
                    if (i == 7)
                    {
                        pro.TotalCap8 = dd[i];
                    }
                    if (i == 8)
                    {
                        pro.TotalCap9 = dd[i];
                    }
                    if (i == 9)
                    {
                        pro.TotalCap10 = dd[i];
                    }
                }

            }


            return query1;
        }

        public string SaveTeam(List<Pro_Shift_Model> issueList1, string productionType)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var proTeam in issueList1)
            {
                var model = new Pro_Team();
                model.ProductionType = productionType;
                model.Team = proTeam.Team;
                model.IsActive = true;
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Pro_Team.Add(model);
                _dbEntities.SaveChanges();
            }

            _dbEntities.SaveChanges();
            return "ok";
        }

        public List<string> GetAllTeam(string productionType)
        {
            List<String> list = (from emp in _dbEntities.Pro_Team
                                 where emp.IsActive == true && emp.ProductionType == productionType
                                 orderby emp.Team ascending
                                 group emp by emp.Team into empg
                                 select empg.Key).ToList();

            return list;
        }

        public List<string> GetAllCategory(string productionType11, string phoneType)
        {
            List<String> list = (from emp in _dbEntities.Pro_Product
                                 where emp.IsActive == true && emp.ProductionType == productionType11 && emp.ProductName == phoneType
                                 orderby emp.CategoryName ascending
                                 group emp by emp.CategoryName into empg
                                 select empg.Key).ToList();

            return list;
        }


        public List<Pro_Shift_Model> GetTeamForUpdate(string productionType)
        {

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select Id,Team,ProductionType from [CellPhoneProject].[dbo].[Pro_Team]
            where IsActive=1 and ProductionType={0}
            group by Id,Team,ProductionType
            order by ProductionType desc", productionType).ToList();

            return query;
        }

        public string UpdateTeam(long ids)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updateTms = (from c in _dbEntities.Pro_Team
                             where c.Id == ids
                             select c).FirstOrDefault();

            updateTms.IsActive = false;
            updateTms.Updated = userId;
            updateTms.UpdatedDate = DateTime.Now;
            _dbEntities.SaveChanges();
            return "OK";
        }


        public string SaveProduct(List<Pro_Shift_Model> issueList1, string productionType)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var proTeam in issueList1)
            {
                for (var ii = 0; ii < proTeam.Category.Count; ii++)
                {
                    var model = new Pro_Product();

                    if (proTeam.ProductName != null)
                    {
                        model.ProductionType = productionType;
                        model.Product = "Mobile";
                        model.ProductName = proTeam.ProductName;
                        model.ProductFamily = proTeam.ProductFamily;
                        model.ChangeOverTime = proTeam.ChangeOverTime;
                        model.CategoryName = proTeam.Category[ii];
                        model.IsActive = true;
                        model.Added = userId;
                        model.AddedDate = DateTime.Now;

                        _dbEntities.Pro_Product.Add(model);
                        _dbEntities.SaveChanges();
                    }

                }
            }

            _dbEntities.SaveChanges();
            return "OK";
        }

        public string EditTeam(string id, string team)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long ids;
            long.TryParse(id, out ids);

            var updateTeam = (from c in _dbEntities.Pro_Team
                              where c.Id == ids
                              select c).FirstOrDefault();

            updateTeam.Team = team.Trim();
            updateTeam.Updated = userId;
            updateTeam.UpdatedDate = DateTime.Now;

            _dbEntities.SaveChanges();
            return "OK";
        }

        public string SaveLine(List<Pro_Shift_Model> issueList1, string productionType)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var proTeam in issueList1)
            {
                var model = new Pro_Line();

                model.ProductionType = productionType;
                model.Line = proTeam.Line;
                model.LineType = proTeam.LineType;
                model.ProductionDaysPerMonth = proTeam.ProductionDaysPerMonth;
                model.HoursPerShift = proTeam.HoursPerShift;
                model.ShiftPerDay = proTeam.ShiftPerDay;
                model.IsActive = true;
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.Pro_Line.Add(model);
                _dbEntities.SaveChanges();
            }

            _dbEntities.SaveChanges();
            return "ok";
        }

        public List<Pro_Shift_Model> GetLineForUpdate(string productionType)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select Id,ProductionType,Line,LineType,ProductionDaysPerMonth,HoursPerShift,
            ShiftPerDay
            from [CellPhoneProject].[dbo].[Pro_Line]
            where IsActive=1 and ProductionType={0}
            group by Id,ProductionType,Line,LineType,ProductionDaysPerMonth,HoursPerShift,ShiftPerDay
            order by ProductionType desc", productionType).ToList();

            return query;
        }

        public string EditLine(string id, string line, string lineType, string productionDaysPerMonth, string shiftPerDay,
            string hoursPerShift)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long ids;
            long.TryParse(id, out ids);

            int productionDaysPerMonths;
            int.TryParse(productionDaysPerMonth, out productionDaysPerMonths);

            int shiftPerDays;
            int.TryParse(shiftPerDay, out shiftPerDays);

            decimal hoursPerShifts;
            decimal.TryParse(hoursPerShift, out hoursPerShifts);

            var updateTeam = (from c in _dbEntities.Pro_Line
                              where c.Id == ids
                              select c).FirstOrDefault();

            updateTeam.ProductionDaysPerMonth = productionDaysPerMonths;
            updateTeam.HoursPerShift = hoursPerShifts;
            updateTeam.ShiftPerDay = shiftPerDays;
            updateTeam.Updated = userId;
            updateTeam.UpdatedDate = DateTime.Now;

            _dbEntities.SaveChanges();
            return "OK";
        }

        public string InActiveLine(long ids)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updateTms = (from c in _dbEntities.Pro_Line
                             where c.Id == ids
                             select c).FirstOrDefault();

            updateTms.IsActive = false;
            updateTms.Updated = userId;
            updateTms.UpdatedDate = DateTime.Now;
            _dbEntities.SaveChanges();
            return "OK";
        }

        public List<Pro_Shift_Model> GetProductForUpdate(string productionType)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select Id,ProductionType,ProductFamily,ProductName,ChangeOverTime,
            CategoryName
            from [CellPhoneProject].[dbo].[Pro_Product]
            where IsActive=1 and ProductionType={0}
            group by Id,ProductionType,ProductName,ChangeOverTime,CategoryName,ProductFamily
            order by ProductionType desc", productionType).ToList();

            return query;
        }

        public string InActiveProduct(long ids)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updateTms = (from c in _dbEntities.Pro_Product
                             where c.Id == ids
                             select c).FirstOrDefault();

            updateTms.IsActive = false;
            updateTms.Updated = userId;
            updateTms.UpdatedDate = DateTime.Now;
            _dbEntities.SaveChanges();
            return "OK";
        }

        public List<string> GetAllLine(string productionType11)
        {
            List<String> list = (from emp in _dbEntities.Pro_Line
                                 where emp.IsActive == true && emp.ProductionType == productionType11
                                 orderby emp.Line ascending
                                 group emp by emp.Line into empg
                                 select empg.Key).ToList();

            return list;
        }

        public string InActiveShift(long ids)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var updateTms = (from c in _dbEntities.Pro_Shift
                             where c.Id == ids
                             select c).FirstOrDefault();

            updateTms.IsActive = false;
            updateTms.Updated = userId;
            updateTms.UpdatedDate = DateTime.Now;
            _dbEntities.SaveChanges();
            return "OK";
        }

        public List<Pro_Shift_Model> GetProductName(string productionType)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select distinct ProductName
            from [CellPhoneProject].[dbo].[Pro_Product]
            where IsActive=1 and ProductionType={0}           
            order by ProductName desc", productionType).ToList();

            return query;
        }

        public List<Pro_Shift_Model> GetCategoryName(string productionType, string proPhoneName)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select distinct CategoryName
            from [CellPhoneProject].[dbo].[Pro_Product]
            where IsActive=1 and ProductionType={0} and ProductName={1}          
            order by CategoryName asc", productionType, proPhoneName).ToList();

            return query;
        }

        #endregion

        #region Capacity Report
        public List<Pro_CapacityPlanning_Model> GetAll1(int mons, string year, string productionType, string phoneType, string categories)
        {
            int years;
            int.TryParse(year, out years);

            var query1 = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"SELECT DISTINCT A.MonNum,A.Month,A.Year,A.ProductName,A.ProductionType,A.Team, A.CategoryName,
            STUFF((SELECT ',' + cast(cast(TotalCapacity AS int) as varchar) FROM [CellPhoneProject].[dbo].[Pro_CapacityPlanning] p
            where MonNum={0} and Year={1} and ProductionType=A.ProductionType
            and ProductName=A.ProductName  and Team=A.Team and CategoryName=A.CategoryName order by Percentage desc
            FOR XML PATH('')),1,1,'') AS TotalCapacities 

            FROM [CellPhoneProject].[dbo].[Pro_CapacityPlanning] AS A where MonNum={0}  and TotalCapacity not in (0.00)
            and Year={1} and ProductionType={2} and ProductName={3}  and CategoryName={4} ", mons, years, productionType, phoneType, categories).ToList();

            foreach (var pro in query1)
            {
                var dd = pro.TotalCapacities.Split(',');
                pro.Team = pro.Team;
                pro.CategoryName = pro.CategoryName;
                for (int i = 0; i < dd.Length; i++)
                {
                    if (i == 0)
                    {
                        pro.TotalCap1 = dd[i];
                    }
                    if (i == 1)
                    {
                        pro.TotalCap2 = dd[i];
                    }
                    if (i == 2)
                    {
                        pro.TotalCap3 = dd[i];
                    }
                    if (i == 3)
                    {
                        pro.TotalCap4 = dd[i];
                    }
                    if (i == 4)
                    {
                        pro.TotalCap5 = dd[i];
                    }
                    if (i == 5)
                    {
                        pro.TotalCap6 = dd[i];
                    }
                    if (i == 6)
                    {
                        pro.TotalCap7 = dd[i];
                    }
                    if (i == 7)
                    {
                        pro.TotalCap8 = dd[i];
                    }
                    if (i == 8)
                    {
                        pro.TotalCap9 = dd[i];
                    }
                    if (i == 9)
                    {
                        pro.TotalCap10 = dd[i];
                    }
                }

            }


            return query1;
        }



        private bool CheckedDailyPlanData(string productionType, DateTime effectiveDate, string line, int mon, int years)
        {
            List<Pro_Shift_Model> getIncentiveReports = null;
            if (productionType != "" && line != "")
            {
                string getIncentiveReportQuery = string.Format(@"select * from CellPhoneProject.dbo.Pro_DailyPlan
                where ProductionType='{0}' and EffectiveDate='{1}'
                and Line='{2}' and MonNum='{3}' and Year='{4}' ", productionType, effectiveDate, line, mon, years);
                getIncentiveReports = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(getIncentiveReportQuery).ToList();

            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string UpdateDailyPlan(long ids, DateTime effectiveDate, string line, string shift1, string shift2, string shift3,
          string productionType, string monNum, string month, string year)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            int mon;
            int.TryParse(monNum, out mon);

            int years;
            int.TryParse(year, out years);


            //bool issueCheck = CheckedDailyPlanData(productionType, effectiveDate, line, mon, years);


            var queries = (from pm in _dbEntities.Pro_DailyPlan
                           where pm.Id == ids
                           select pm).FirstOrDefault();
            if (queries != null)
            {
                if (shift1 != "")
                {
                    queries.Shift_1 = shift1;
                }
                if (shift2 != "")
                {
                    queries.Shift_2 = shift2;
                }
                if (shift3 != "")
                {
                    queries.Shift_3 = shift3;
                }
                queries.Updated = userId;
                queries.UpdatedDate = DateTime.Now;

                _dbEntities.Pro_DailyPlan.AddOrUpdate(queries);
                _dbEntities.SaveChanges();
                return "OK";
            }
            return "OK";
        }

        public string SaveDailyPlan(List<Pro_Shift_Model> results)//hi
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var res in results)
            {
                var vSplit = res.AllShift.Split(',');

                var effect = Convert.ToDateTime(vSplit[0]);
                var produtionType = Convert.ToString(vSplit[8]);
                var line = Convert.ToString(vSplit[1]);
                var mon = Convert.ToInt32(vSplit[6]);
                var yearss = Convert.ToInt32(vSplit[5]);

                var presentData = (from pm in _dbEntities.Pro_DailyPlan
                                   where pm.ProductionType == produtionType
                                       && pm.EffectiveDate == effect
                                       && pm.Line == line
                                       && pm.MonNum == mon
                                       && pm.Year == yearss
                                   select pm).FirstOrDefault();
                if (presentData != null)
                {
                    presentData.Shift_1 = Convert.ToString(vSplit[2]);
                    presentData.Shift_2 = Convert.ToString(vSplit[3]);
                    presentData.Shift_3 = Convert.ToString(vSplit[4]);

                    _dbEntities.Pro_DailyPlan.AddOrUpdate(presentData);
                    _dbEntities.SaveChanges();
                }
                else
                {
                    var model = new Pro_DailyPlan();
                    model.ProductionType = Convert.ToString(vSplit[8]);
                    model.EffectiveDate = Convert.ToDateTime(vSplit[0]);
                    model.Line = Convert.ToString(vSplit[1]);
                    if (vSplit[2] == "")
                    {
                        model.Shift_1 = null;
                    }
                    else
                    {
                        model.Shift_1 = Convert.ToString(vSplit[2]);
                    }

                    model.Shift_2 = Convert.ToString(vSplit[3]);

                    if (vSplit[4] == "")
                    {
                        model.Shift_3 = null;
                    }
                    else
                    {
                        model.Shift_3 = Convert.ToString(vSplit[4]);
                    }
                    model.MonNum = Convert.ToInt32(vSplit[6]);
                    model.Month = Convert.ToString(vSplit[7]);
                    model.Year = Convert.ToInt32(vSplit[5]);
                    model.Added = userId;
                    model.AddedDate = DateTime.Now;
                    model.IsActive = true;

                    _dbEntities.Pro_DailyPlan.Add(model);
                }


            }
            _dbEntities.SaveChanges();

            return "ok";
        }

        public List<Pro_Shift_Model> ProductNameForReport(int mons, string year, string productionType)
        {
            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"
            //            select ProductName, count(Team) as TotalTeam,(select top 1 ChangeOverTime From CellPhoneProject.dbo.Pro_Product pr
            //            where pr.ProductName=ProductName and ChangeOverTime is not null) as ChangeOverTime from
            //            (select distinct Team,ProductName from [CellPhoneProject].[dbo].[Pro_CapacityPlanning]
            //            where MonNum={0} and Year={1} and IsActive=1 and ProductionType={2} and TotalCapacity not in (0.00))A
            //            group by ProductName order by ProductName asc", mons, year, productionType).ToList();

            //            return query;
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"
            select distinct B.ProductName,B.TotalTeam,C.ChangeOverTime from
            (select ProductName, count(Team) as TotalTeam
            from
            (select distinct Team,ProductName from [CellPhoneProject].[dbo].[Pro_CapacityPlanning] 
            where MonNum={0} and Year={1} and IsActive=1 and ProductionType={2} and TotalCapacity not in (0.00))A
            group by ProductName )B
            left join CellPhoneProject.dbo.Pro_Product C on C.ProductName=B.ProductName

            where ChangeOverTime = (select top 1 ChangeOverTime From CellPhoneProject.dbo.Pro_Product pr
            where pr.ProductName=B.ProductName and pr.ProductionType={2} and IsActive=1  and pr.ChangeOverTime is not null)

            order by B.ProductName asc", mons, year, productionType).ToList();

            return query;
        }

        public List<Pro_Shift_Model> TeamNameForReport(int mons, string year, string productionType)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select Team, count(CategoryName) as TotalCategory1,ProductName from
            (

                select distinct CategoryName,Team,ProductName from [CellPhoneProject].[dbo].[Pro_CapacityPlanning]
                where MonNum={0} and Year={1} and IsActive=1 and ProductionType={2} and TotalCapacity not in (0.00) 

            )A
            group by Team,ProductName order by Team asc", mons, year, productionType).ToList();

            return query;
        }

        public List<Pro_Shift_Model> CategoryNameForReport(int mons, string year, string productionType)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@" select CategoryName,Team,ProductName from
            (

            select distinct CategoryName,Team,ProductName from [CellPhoneProject].[dbo].[Pro_CapacityPlanning]
            where MonNum={0} and Year={1} and IsActive=1 and ProductionType={2} and TotalCapacity not in (0.00) 

            )A
            group by Team,ProductName,CategoryName order by Team asc", mons, year, productionType).ToList();

            return query;
        }

        public List<Pro_CapacityPlanning_Model> GetPercentage1(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"select distinct Percentage,ProductName from
            [CellPhoneProject].[dbo].[Pro_CapacityPlanning] where MonNum={0} and Year={1} and ProductionType={2}  order by Percentage desc", mons, years, productionType).ToList();

            return query;
        }

        public List<Pro_CapacityPlanning_Model> GetQuantityRange1(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"select distinct QuantityRange,Percentage,ProductName from
            [CellPhoneProject].[dbo].[Pro_CapacityPlanning] where MonNum={0} and Year={1} and ProductionType={2} order by Percentage desc", mons, years, productionType).ToList();

            return query;
        }

        public List<Pro_CapacityPlanning_Model> GetTotalCapacities1(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_CapacityPlanning_Model>(@"select CategoryName,cast(cast(TotalCapacity AS int) as varchar) as TotalCapacity2,Percentage,QuantityRange,Team,ProductName from
            (
                select distinct CategoryName,TotalCapacity,Percentage,QuantityRange,Team,ProductName from [CellPhoneProject].[dbo].[Pro_CapacityPlanning]
                where MonNum={0} and Year={1} and IsActive=1 and ProductionType={2} and TotalCapacity not in (0.00) 
            )A
            group by Team,ProductName,CategoryName,TotalCapacity,Percentage,QuantityRange order by Team,Percentage desc", mons, years, productionType).ToList();

            return query;
        }
        #endregion

        #region Project Categorize
        public List<Pro_Shift_Model> GetProjectForCategorization()
        {
            //            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select distinct ProjectName,
            //            case when ProjectType='Feature' then 'Featurephone'  
            //            when ProjectType='Smart' then 'Smartphone' else ProjectType end as ProjectType
            //            from CellPhoneProject.dbo.ProjectMasters pm where IsActive=1 and AddedDate between '2019-01-01' and GETDATE()
            //            and ProjectName not in (select ProjectName from [CellPhoneProject].[dbo].[ProjectCategorization] where IsComplete=1 and ProjectName=pm.ProjectName)
            //            order by ProjectName asc").ToList();
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select distinct pm.ProjectName,
            case when ProjectType='Feature' then 'Featurephone'  
            when ProjectType='Smart' then 'Smartphone' else ProjectType end as ProjectType,pc.AssemblyCategory,pc.SmtCategory,pc.HousingCategory
            from CellPhoneProject.dbo.ProjectMasters pm 
            left join [CellPhoneProject].[dbo].[ProjectCategorization] pc on pm.ProjectName=pc.ProjectName

            where pm.IsActive=1 and pm.AddedDate between '2019-01-01' and GETDATE()
            and pm.ProjectName not in (select ProjectName from [CellPhoneProject].[dbo].[ProjectCategorization] where IsComplete=1 and ProjectName=pm.ProjectName)
            order by pm.ProjectName asc").ToList();

            return query;
        }

        public List<Pro_Shift_Model> GetAssemblyCategory(string projectType)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select ProductionType,ProductName,ProductFamily,CategoryName as AssemblyCategory 
            from  [CellPhoneProject].[dbo].[Pro_Product]   
            where ProductionType in ('Assembly','Charger','Battery','Earphone') 
            and ProductFamily={0} and IsActive=1", projectType).ToList();

            return query;
        }

        public List<Pro_Shift_Model> GetSmtCategory(string smtCategory)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select ProductionType,ProductName,ProductFamily,CategoryName as SmtCategory from  [CellPhoneProject].[dbo].[Pro_Product]   
              where ProductionType in ('SMT') and ProductFamily={0} and IsActive=1", smtCategory).ToList();

            return query;
        }

        public List<Pro_Shift_Model> GetHousingCategory(string housingCategory)
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select ProductionType,ProductName,ProductFamily,CategoryName as HousingCategory 
            from  [CellPhoneProject].[dbo].[Pro_Product]   
            where ProductionType in ('Housing') and ProductFamily={0} and IsActive=1", housingCategory).ToList();

            return query;
        }

        public bool CheckedCategorizedData(string projectName, string productFamily)
        {
            List<Custom_Sw_IncentiveModel> getIncentiveReports = null;
            if (projectName != "" && productFamily != "")
            {
                string getIncentiveReportQuery = string.Format(@"select *
                from [CellPhoneProject].[dbo].[ProjectCategorization] where ProjectName='{0}'
                and  ProductFamily='{1}' ", projectName, productFamily);
                getIncentiveReports = _dbEntities.Database.SqlQuery<Custom_Sw_IncentiveModel>(getIncentiveReportQuery).ToList();

            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public string SaveCategorizeData(string projectName, string productFamily, string assemblyCategory, string smtCategory,
            string housingCategory)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            bool issueCheck = CheckedCategorizedData(projectName, productFamily);

            if (issueCheck)
            {
                var queries = (from pm in _dbEntities.ProjectCategorizations
                               where pm.ProjectName == projectName && pm.ProductFamily == productFamily
                               select pm).FirstOrDefault();

                if (assemblyCategory != "")
                {
                    queries.AssemblyCategory = assemblyCategory;
                }
                if (smtCategory != "")
                {
                    queries.SmtCategory = smtCategory;
                }
                if (assemblyCategory != "")
                {
                    queries.HousingCategory = housingCategory;
                }
                _dbEntities.ProjectCategorizations.AddOrUpdate(queries);
                _dbEntities.SaveChanges();

                return "Okis";
            }
            else
            {
                var model = new ProjectCategorization();
                model.ProjectName = projectName;
                model.ProductFamily = productFamily;
                model.AssemblyCategory = assemblyCategory;
                model.SmtCategory = smtCategory;
                model.HousingCategory = housingCategory;
                model.IsComplete = false;
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbEntities.ProjectCategorizations.Add(model);
                _dbEntities.SaveChanges();

            }
            return "OK";
        }

        public string CompleteCategorizeData(string projectName, string productFamily)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            bool issueCheck = CheckedCategorizedData(projectName, productFamily);

            if (issueCheck)
            {
                var queries = (from pm in _dbEntities.ProjectCategorizations
                               where pm.ProjectName == projectName && pm.ProductFamily == productFamily
                               select pm).FirstOrDefault();

                queries.IsComplete = true;
                queries.Updated = userId;
                queries.UpdatedDate = DateTime.Now;

                _dbEntities.ProjectCategorizations.AddOrUpdate(queries);
                _dbEntities.SaveChanges();
            }

            return "OK";
        }

        public List<Pro_Shift_Model> GetCompletedCategorization()
        {
            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select * from  [CellPhoneProject].[dbo].[ProjectCategorization]  
              where IsComplete=1 order by ProjectName asc").ToList();

            return query;
        }

        public string UpdateCategorizeData(long ids, string assemblyCategory1, string smtCategory1, string housingCategory1)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var queries = (from pm in _dbEntities.ProjectCategorizations
                           where pm.Id == ids
                           select pm).FirstOrDefault();

            if (assemblyCategory1 != "")
            {
                queries.AssemblyCategory = assemblyCategory1;
            }
            if (smtCategory1 != "")
            {
                queries.SmtCategory = smtCategory1;
            }
            if (assemblyCategory1 != "")
            {
                queries.HousingCategory = housingCategory1;
            }
            _dbEntities.ProjectCategorizations.AddOrUpdate(queries);
            _dbEntities.SaveChanges();


            return "OK";
        }

        public List<Pro_Shift_Model> ChangedDailyPlanData(int mons, string year, string productionType)
        {
            int years;
            int.TryParse(year, out years);

            var query = _dbEntities.Database.SqlQuery<Pro_Shift_Model>(@"select ProductionType,EffectiveDate, Line,Shift_1,case when Shift_2 in ('0',null) then NULL else Shift_2 end as Shift_2,
            case when Shift_3 in ('0',null) then NULL else Shift_3 end as Shift_3,Month,Year,MonNum 
            from [CellPhoneProject].[dbo].Pro_DailyPlan 
            where MonNum={0} and Year={1} and ProductionType={2} and IsActive=1 ", mons, years, productionType).ToList();
            return query;
        }
        public bool CheckShiftDatas(string unitValues, int forwardedYear1, int forwardedMonNum1, string line)
        {
            var chkPro = new List<Pro_Shift_Model>();

            string proEv = string.Format(@"select * from [CellPhoneProject].[dbo].[Pro_Shift] pm
              where pm.ProductionType ='{0}' and
             pm.Line ='{1}' and pm.Year ='{2}' and pm.MonNum = '{3}' and pm.IsActive=1",
             unitValues, line, forwardedYear1, forwardedMonNum1);

            chkPro =
                   _dbEntities.Database.SqlQuery<Pro_Shift_Model>(proEv).ToList();

            if (chkPro != null && chkPro.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string ForwardShift(string unitValues, string currentDate, string forwardedDate, string shiftForward)
        {

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var currentDate1 = currentDate.Split(',');
            var currentMonth = currentDate1[0].Trim();
            var currentYear = currentDate1[1].Trim();
            int currentYear1 = Convert.ToInt32(currentYear);
            int currentMonNum1 = DateTime.ParseExact(currentMonth, "MMMM", CultureInfo.CurrentCulture).Month;

            var forwardedDate1 = forwardedDate.Split(',');
            var forwardedMonth = forwardedDate1[0].Trim();
            var forwardedYear = forwardedDate1[1].Trim();
            int forwardedYear1 = Convert.ToInt32(forwardedYear);
            int forwardedMonNum1 = DateTime.ParseExact(forwardedMonth, "MMMM", CultureInfo.CurrentCulture).Month;

            var qryList = (from prSf in _dbEntities.Pro_Shift
                           where prSf.Year == currentYear1 && prSf.MonNum == currentMonNum1 && prSf.ProductionType == unitValues &&
                           prSf.IsActive != false
                              
                           select prSf).ToList();


            foreach (var qq in qryList)
            {

                bool isSaved = CheckShiftDatas(unitValues, forwardedYear1, forwardedMonNum1, qq.Line);

                if (!isSaved)
                {
                    var model = new Pro_Shift();

                    model.ProductionType = qq.ProductionType;
                    model.Month = forwardedMonth;
                    model.MonNum = forwardedMonNum1;
                    model.Year = forwardedYear1;
                    model.Line = qq.Line;
                    if (qq.Shift_1 == "0")
                    {
                        qq.Shift_1 = null;
                    }
                    else
                    {
                        model.Shift_1 = qq.Shift_1;
                    }
                    if (qq.Shift_2 == "0")
                    {
                        qq.Shift_2 = null;
                    }
                    else
                    {
                        model.Shift_2 = qq.Shift_2;
                    }
                    if (qq.Shift_3 == "0")
                    {
                        qq.Shift_3 = null;
                    }
                    else
                    {
                        model.Shift_3 = qq.Shift_3;
                    }

                    model.IsActive = true;
                    model.Added = userId;
                    model.AddedDate = DateTime.Now;

                    _dbEntities.Pro_Shift.AddOrUpdate(model);
                    _dbEntities.SaveChanges();
                }
            }
            return "OK";
        }
        public bool CheckCapacityDatas(string unitValues, int forwardedYear1, int forwardedMonNum1, string Team,
                   int Percentage,string QuantityRange, string ProductName,string CategoryName,decimal TotalCapacity)
        {
            var chkPro = new List<Pro_Shift_Model>();

            string proEv = string.Format(@"select * from [CellPhoneProject].[dbo].[Pro_CapacityPlanning] pm
              where pm.ProductionType ='{0}' and pm.Year ='{1}' and pm.MonNum = '{2}' and pm.Team='{3}'
              and pm.Percentage='{4}' and pm.QuantityRange='{5}' and pm.ProductName='{6}' 
              and pm.CategoryName='{7}' and pm.TotalCapacity='{8}'
              and pm.IsActive=1",
             unitValues, forwardedYear1, forwardedMonNum1, Team, 
             Percentage, QuantityRange, ProductName, CategoryName, TotalCapacity);

            chkPro =
                   _dbEntities.Database.SqlQuery<Pro_Shift_Model>(proEv).ToList();

            if (chkPro != null && chkPro.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string ForwardCapacity(string unitValues, string currentDate, string forwardedDate, string capForward)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var currentDate1 = currentDate.Split(',');
            var currentMonth = currentDate1[0].Trim();
            var currentYear = currentDate1[1].Trim();
            int currentYear1 = Convert.ToInt32(currentYear);
            int currentMonNum1 = DateTime.ParseExact(currentMonth, "MMMM", CultureInfo.CurrentCulture).Month;

            var forwardedDate1 = forwardedDate.Split(',');
            var forwardedMonth = forwardedDate1[0].Trim();
            var forwardedYear = forwardedDate1[1].Trim();
            int forwardedYear1 = Convert.ToInt32(forwardedYear);
            int forwardedMonNum1 = DateTime.ParseExact(forwardedMonth, "MMMM", CultureInfo.CurrentCulture).Month;

            var qryList = (from prSf in _dbEntities.Pro_CapacityPlanning
                           where prSf.Year == currentYear1 && prSf.MonNum == currentMonNum1 && prSf.ProductionType == unitValues
                           && prSf.IsActive != false select prSf).ToList();

            foreach (var qq in qryList)
            {

                bool isSaved = CheckCapacityDatas(unitValues, forwardedYear1, forwardedMonNum1, qq.Team,
                    Convert.ToInt32(qq.Percentage), qq.QuantityRange, qq.ProductName, qq.CategoryName, Convert.ToDecimal(qq.TotalCapacity));

                if (!isSaved)
                {
                    var model = new Pro_CapacityPlanning();

                    model.ProductionType = qq.ProductionType;
                    model.Month = forwardedMonth;
                    model.MonNum = forwardedMonNum1;
                    model.Year = forwardedYear1;
                    model.Team = qq.Team;
                    model.CategoryName = qq.CategoryName;
                    model.Percentage = qq.Percentage;
                    model.QuantityRange = qq.QuantityRange;
                    model.TotalCapacity = qq.TotalCapacity;
                    model.ProductName = qq.ProductName;
                    model.Product = qq.Product;
                    model.IsActive = true;
                    model.Added = userId;
                    model.AddedDate = DateTime.Now;

                    _dbEntities.Pro_CapacityPlanning.AddOrUpdate(model);
                    _dbEntities.SaveChanges();
                }

            }
            return "OK";
        }

        #endregion
    }
}