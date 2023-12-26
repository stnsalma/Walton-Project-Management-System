using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CustomUI;
using LinqToExcel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;
using Remotion.Data.Linq.Clauses;

namespace ProjectManagement.Controllers
{
    public class MaterialWastageController : Controller
    {
        private readonly ICommonRepository _commonRepository;
        private readonly IMaterialWastageRepository _materialWastageRepository;

        public MaterialWastageController(CommonRepository commonRepository, MaterialWastageRepository materialWastageRepository)
        {
            _commonRepository = commonRepository;
            _materialWastageRepository = materialWastageRepository;
        }
        // GET: MaterialWastage
        [Authorize(Roles = "WASREPGEN")]
        public ActionResult Index()
        {
            List<MaterialWastageMasterModel> models = _commonRepository.GetMetarialWastageList();
            return View(models);
        }
        [Authorize(Roles = "WASREPGEN")]
        public ActionResult CreateNew()
        {
            //ViewBag.Variants = _commonRepository.GetVariantsWithOrderNumber();
            return View(new WastageFileUpload());
        }
        [HttpPost]
        //[Authorize(Roles = "WHEAD, WAS")]
        public ActionResult CreateNew(WastageFileUpload model)
        {
            bool isExist =
                _materialWastageRepository.GetMaterialWastageReportByMonthAndYear(
                    model.MaterialWastageMaster.MonthNumber, model.MaterialWastageMaster.YearNumber);

            if (!isExist)
            {
                var extension = model.HttpPostedFileBase != null
                    ? Path.GetExtension(model.HttpPostedFileBase.FileName)
                    : null;


                if (model.HttpPostedFileBase != null)
                {
                    string pathToExcelFile =
                        Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Content/WastageFiles"),
                            model.HttpPostedFileBase.FileName);
                    model.HttpPostedFileBase.SaveAs(pathToExcelFile);
                    if (extension != null)
                    {
                        var fileExt = extension.Substring(1);
                        //string workSheet = string.Empty;
                        string connectionString = string.Empty;
                        if ((model.HttpPostedFileBase != null) && (model.HttpPostedFileBase.ContentLength > 0) &&
                            fileExt == "xlsx")
                        {
                            using (var pack = new ExcelPackage(model.HttpPostedFileBase.InputStream))
                            {
                                var currentSheet = pack.Workbook.Worksheets;
                                var workSheet = currentSheet.First();
                                var noOfRow = workSheet.Dimension.End.Row;
                                var noOfColumn = workSheet.Dimension.End.Column;
                                for (var i = 2; i <= noOfRow; i++)
                                {
                                    var materialWastageDetail = new MaterialWastageDetail();
                                    materialWastageDetail.ItemCode = workSheet.Cells[i, 1].Text;
                                    materialWastageDetail.ItemName = workSheet.Cells[i, 2].Text;
                                    materialWastageDetail.BOMUnit = Convert.ToDouble(workSheet.Cells[i, 3].Text);
                                    materialWastageDetail.WastagePercentage = Convert.ToDouble(Regex.Match(workSheet.Cells[i, 4].Text, @"\d+\.*\d*").Value);
                                    materialWastageDetail.RecQtyWOWastage = Convert.ToInt32(workSheet.Cells[i, 5].Value);
                                    materialWastageDetail.RecQtyWWastage = Convert.ToInt32(workSheet.Cells[i, 6].Value);
                                    materialWastageDetail.TotalLot = Convert.ToInt32(workSheet.Cells[i, 7].Value);
                                    materialWastageDetail.WastageWOBom = Convert.ToInt32(workSheet.Cells[i, 8].Value);
                                    materialWastageDetail.WastageWBom = Convert.ToInt32(workSheet.Cells[i, 9].Value);
                                    materialWastageDetail.TotalWastage = Convert.ToInt32(workSheet.Cells[i, 10].Value);
                                    materialWastageDetail.AssemMaterialFault = Convert.ToInt32(workSheet.Cells[i, 11].Value);
                                    materialWastageDetail.AssemProcessFault = Convert.ToInt32(workSheet.Cells[i, 12].Value);
                                    materialWastageDetail.RepMaterialFault = Convert.ToInt32(workSheet.Cells[i, 13].Value);
                                    materialWastageDetail.RepProcessFault = Convert.ToInt32(workSheet.Cells[i, 14].Value);
                                    materialWastageDetail.TotalFault = Convert.ToInt32(workSheet.Cells[i, 15].Value);
                                    materialWastageDetail.TotalMaterialFaultApproved = Convert.ToInt32(workSheet.Cells[i, 16].Value);
                                    materialWastageDetail.TotalProcessFaultApproved = Convert.ToInt32(workSheet.Cells[i, 17].Value);
                                    materialWastageDetail.TotalFaultApproved = Convert.ToInt32(workSheet.Cells[i, 18].Value);
                                    materialWastageDetail.TillNowAssemMaterialFault = Convert.ToInt32(workSheet.Cells[i, 19].Value);
                                    materialWastageDetail.TillNowAssemProcessFault = Convert.ToInt32(workSheet.Cells[i, 20].Value);
                                    materialWastageDetail.TillNowRepMaterialFault = Convert.ToInt32(workSheet.Cells[i, 21].Value);
                                    materialWastageDetail.TillNowRepProcessFault = Convert.ToInt32(workSheet.Cells[i, 22].Value);
                                    materialWastageDetail.TillNowTotalFault = Convert.ToInt32(workSheet.Cells[i, 23].Value);
                                    materialWastageDetail.ActualAssemblyWastage_TotalLot = Convert.ToDouble(Regex.Match(workSheet.Cells[i, 24].Text, @"\d+\.*\d*").Value);
                                    materialWastageDetail.ActualRepairWastage_TotalLot = Convert.ToDouble(Regex.Match(workSheet.Cells[i, 25].Text, @"\d+\.*\d*").Value);
                                    materialWastageDetail.ActualWastageOfTotalLot = Convert.ToDouble(Regex.Match(workSheet.Cells[i, 26].Text, @"\d+\.*\d*").Value);
                                    materialWastageDetail.NetAdjustment = Convert.ToDouble(Regex.Match(workSheet.Cells[i, 27].Text, @"\d+\.*\d*").Value);
                                    materialWastageDetail.ImportedQtyWithWastage = Convert.ToInt32(workSheet.Cells[i, 28].Value);
                                    materialWastageDetail.WastageQtyInBOM = Convert.ToInt32(workSheet.Cells[i, 29].Value);
                                    materialWastageDetail.NeedToDeclare = Convert.ToInt32(workSheet.Cells[i, 30].Value);
                                    materialWastageDetail.AlreadySined = Convert.ToInt32(workSheet.Cells[i, 31].Value);
                                    materialWastageDetail.NeedSign = Convert.ToInt32(workSheet.Cells[i, 32].Value);
                                    materialWastageDetail.UnitPrice = Convert.ToDouble(workSheet.Cells[i, 33].Value);
                                    materialWastageDetail.TotalPrice = Convert.ToDouble(workSheet.Cells[i, 34].Value);
                                    materialWastageDetail.CrossCheck = Convert.ToInt32(workSheet.Cells[i, 35].Value);
                                    //materialWastageDetail.FOCTakenDate = Convert.ToDateTime(workSheet.Cells[i, 36].Text);
                                    materialWastageDetail.FOCQty = Convert.ToInt32(workSheet.Cells[i, 36].Value);
                                    materialWastageDetail.Remarks = Convert.ToString(workSheet.Cells[i, 37].Value);
                                    materialWastageDetail.BOMType = Convert.ToString(workSheet.Cells[i, 38].Value);




                                    //general data

                                    model.MaterialWastageDetails.Add(materialWastageDetail);
                                }
                            }



                        }
                    }
                }
                //ViewBag.Variants = _commonRepository.GetVariantsWithOrderNumber();
                if (model.MaterialWastageDetails.Any(i => string.IsNullOrWhiteSpace(i.BOMType)))
                {
                    TempData["message"] = "There are one or many item found that has no BOM Type. BOM Type is mandatory";
                    TempData["messageType"] = 3;
                    model.MaterialWastageDetails = new List<MaterialWastageDetail>();
                    return View(model);
                }
                return View(model);
            }
            TempData["message"] = "A report already exist for this month and year.";
            TempData["messageType"] = 2;
            return View(model);
        }

        public JsonResult GetBomItemInformation(string itemCode)
        {
            BomModel boms = _commonRepository.GetBomInfoByItemCode(itemCode).Where(i => i.ItemCost > 0 & i.ItemCost != null).OrderBy(i => i.AddedDate).FirstOrDefault();
            var result = new JsonResult
            {
                Data = boms,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
            return result;
        }
        [HttpPost]
        public JsonResult SaveWastage(WastageFileUpload wastageFileUpload)
        {

            ResponseModel responseModel = _commonRepository.SaveMaterialWastage(wastageFileUpload);
            return new JsonResult { Data = responseModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [Authorize(Roles = "WASSHEAD, WACCHEAD, WSMTHEAD, WCHAHEAD, WASCOO, WASDCOO, MM")]
        public ActionResult PendingApprovals()
        {
            var user = User;
            List<MaterialWastageMasterModel> models = new List<MaterialWastageMasterModel>();
            if (user.IsInRole("WASSHEAD")
                || user.IsInRole("WACCHEAD")
                || user.IsInRole("WSMTHEAD")
                || user.IsInRole("WCHAHEAD"))
            {
                models = _commonRepository.GetPendingApprovals(approvalStage: 1);
            }
            if (user.IsInRole("WASCOO"))
            {
                models = _commonRepository.GetPendingApprovals(approvalStage: 2);
            }
            if (user.IsInRole("MM"))
            {
                models = _commonRepository.GetPendingApprovals(approvalStage: 3);
            }
            if (user.IsInRole("WASDCOO"))
            {
                models = _commonRepository.GetPendingApprovals(approvalStage: 4);
            }
            

            if(models == null) models = new List<MaterialWastageMasterModel>();
            return View(models);
        }
        //[Authorize(Roles = "WHEAD, WASCOO, MM")]
        public ActionResult Details(long id)
        {

            if (User.IsInRole("WASSHEAD") || User.IsInRole("WACCHEAD") || User.IsInRole("WSMTHEAD") || User.IsInRole("WCHAHEAD") || User.IsInRole("WASCOO") || User.IsInRole("MM") || User.IsInRole("WASDCOO"))
            {
                WastageFileUpload model = _commonRepository.GetMaterialWastageById(id);
                return View(model);
            }
            return View(new WastageFileUpload());
        }
        //[Authorize(Roles = "WHEAD, WASCOO, MM")]
        public JsonResult SaveRecommendation(bool isRecom, bool isApproved, string recomMsg, string approvedMsg, long id)
        {
            var user = User;
            ResponseModel response = null;
            if (User.IsInRole("WASSHEAD") || User.IsInRole("WACCHEAD") || User.IsInRole("WSMTHEAD") || User.IsInRole("WCHAHEAD"))
            {
                response = _commonRepository.RecommendMaterialWastage(id, isRecom, recomMsg, isApproved, approvedMsg, user, 1);
            }
            if (user.IsInRole("WASCOO"))
            {
                response = _commonRepository.RecommendMaterialWastage(id, isRecom, recomMsg, isApproved, approvedMsg, user, 2);
            }
            if (user.IsInRole("MM"))
            {
                response = _commonRepository.RecommendMaterialWastage(id, isRecom, recomMsg, isApproved, approvedMsg, user, 3);
            }
            if (user.IsInRole("WASCOO"))
            {
                response = _commonRepository.RecommendMaterialWastage(id, isRecom, recomMsg, isApproved, approvedMsg, user, 4);
            }
            return new JsonResult { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetRecommendations(long id)
        {
            List<MaterialWastageRecommendation> recommendations = _commonRepository.GetRecommendationsByMasterId(id);
            var r = RenderViewToString(ControllerContext, "_MaterialWastageRecommendations", recommendations);
            //return Json(PartialView("_MaterialWastageRecommendations", new MaterialWastageRecommendation()));

            return new JsonResult { Data = r, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult GetTopSheet(long id)
        {
            MaterialWastageReportTopSheetViewModel model = _materialWastageRepository.GetMaterailWastageTopSheet(id);
            var r = RenderViewToString(ControllerContext, "_MaterialWastageTopSheet", model);

            return new JsonResult { Data = r, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult DownloadMaterialWastageExcel(long id)
        {
            WastageFileUpload details = _commonRepository.GetMaterialWastageById(id);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "Material_Wastage_" + "_" + details.MaterialWastageMaster.MonthName + "_" + details.MaterialWastageMaster.YearNumber + ".xlsx";
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add(details.MaterialWastageMaster.ReportName);
                    worksheet.Columns("A", "Z").AdjustToContents();

                    worksheet.Cell("A1").Style.Font.SetBold().Font.FontSize = 16;
                    worksheet.Cell("A1").Value = "Walton Digi- Tech Industries (Mobile)";
                    worksheet.Range("A1:M1").Merge();


                    worksheet.Cell("A2").Style.Font.SetBold().Font.FontSize = 16;
                    worksheet.Cell("A2").Value = "Cell Phone Manufacturing Unit";
                    worksheet.Range("A2:M2").Merge();

                    worksheet.Cell("A3").Style.Font.SetBold().Font.FontSize = 16;
                    worksheet.Cell("A3").Value = "H#00013, Block- B, Building- 03,(2nd & 3rd Floor)";
                    worksheet.Range("A3:M3").Merge();


                    worksheet.Cell("A4").Style.Font.SetBold().Font.FontSize = 16;
                    worksheet.Cell("A4").Value = "Ward- 02, Boroichuti, Kaliakoir, Gazipur";
                    worksheet.Range("A4:M4").Merge();


                    worksheet.Cell("A5").Style.Font.SetBold().Font.FontSize = 16;
                    worksheet.Cell("A5").Value = "Assembly Wastage for the month of " + details.MaterialWastageMaster.MonthName + " " + details.MaterialWastageMaster.YearNumber;
                    worksheet.Range("A5:M5").Merge();

                    worksheet.Cell(7, 1).Value = "Item Code";
                    worksheet.Cell(7, 1).Style.Font.SetBold();
                    worksheet.Cell(7, 1).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 1).Style.Alignment.WrapText = true;


                    worksheet.Cell(7, 2).Value = "Item Details";
                    worksheet.Cell(7, 2).Style.Font.SetBold();
                    worksheet.Cell(7, 2).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 2).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 3).Value = "BOM Unit";
                    worksheet.Cell(7, 3).Style.Font.SetBold();
                    worksheet.Cell(7, 3).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 3).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 4).Value = "Wastage Percentage";
                    worksheet.Cell(7, 4).Style.Font.SetBold();
                    worksheet.Cell(7, 4).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 4).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 5).Value = "Receive QTY Without Wastage";
                    worksheet.Cell(7, 5).Style.Font.SetBold();
                    worksheet.Cell(7, 5).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 5).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 6).Value = "Receive QTY With Wastage";
                    worksheet.Cell(7, 6).Style.Font.SetBold();
                    worksheet.Cell(7, 6).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 6).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 7).Value = "Total Lot";
                    worksheet.Cell(7, 7).Style.Font.SetBold();
                    worksheet.Cell(7, 7).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 7).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 8).Value = "Wastage Without BOM";
                    worksheet.Cell(7, 8).Style.Font.SetBold();
                    worksheet.Cell(7, 8).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 8).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 9).Value = "Wastage With BOM";
                    worksheet.Cell(7, 9).Style.Font.SetBold();
                    worksheet.Cell(7, 9).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 9).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 10).Value = "Total Wastage (Without+ With)";
                    worksheet.Cell(7, 10).Style.Font.SetBold();
                    worksheet.Cell(7, 10).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 10).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 11).Value = "Assembly Material Fault";
                    worksheet.Cell(7, 11).Style.Font.SetBold();
                    worksheet.Cell(7, 11).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 11).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 12).Value = "Assembly Process Fault";
                    worksheet.Cell(7, 12).Style.Font.SetBold();
                    worksheet.Cell(7, 12).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 12).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 13).Value = "Repair  Material Fault";
                    worksheet.Cell(7, 13).Style.Font.SetBold();
                    worksheet.Cell(7, 13).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 13).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 14).Value = "Repair Process Fault";
                    worksheet.Cell(7, 14).Style.Font.SetBold();
                    worksheet.Cell(7, 14).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 14).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 15).Value = "Total Fault";
                    worksheet.Cell(7, 15).Style.Font.SetBold();
                    worksheet.Cell(7, 15).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 15).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 16).Value = "Till Now Material Fault Approved";
                    worksheet.Cell(7, 16).Style.Font.SetBold();
                    worksheet.Cell(7, 16).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 16).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 17).Value = "Til Now Process Fault Approved";
                    worksheet.Cell(7, 17).Style.Font.SetBold();
                    worksheet.Cell(7, 17).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 17).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 18).Value = "Till Now Total Fault Approved";
                    worksheet.Cell(7, 18).Style.Font.SetBold();
                    worksheet.Cell(7, 18).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 18).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 19).Value = "Till Now Total Actual Wastage Assembly Material Fault";
                    worksheet.Cell(7, 19).Style.Font.SetBold();
                    worksheet.Cell(7, 19).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 19).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 20).Value = "Till Now Total Actual Wastage Assembly Process Fault";
                    worksheet.Cell(7, 20).Style.Font.SetBold();
                    worksheet.Cell(7, 20).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 20).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 21).Value = "Till Now Total Actual Wastage Repair  Material Fault";
                    worksheet.Cell(7, 21).Style.Font.SetBold();
                    worksheet.Cell(7, 21).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 21).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 22).Value = "Till Now Total Actual Wastage Repair Process Fault";
                    worksheet.Cell(7, 22).Style.Font.SetBold();
                    worksheet.Cell(7, 22).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 22).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 23).Value = "Till Now Total Wastage Received";
                    worksheet.Cell(7, 23).Style.Font.SetBold();
                    worksheet.Cell(7, 23).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 23).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 24).Value = "Actual Wastage of Total Assembly Wastage %";
                    worksheet.Cell(7, 24).Style.Font.SetBold();
                    worksheet.Cell(7, 24).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 24).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 25).Value = "Actual Wastage of Total Repair Wastage %";
                    worksheet.Cell(7, 25).Style.Font.SetBold();
                    worksheet.Cell(7, 25).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 25).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 26).Value = "Actual Wastage % Of Total Lot";
                    worksheet.Cell(7, 26).Style.Font.SetBold();
                    worksheet.Cell(7, 26).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 26).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 27).Value = "Net Adjustment (Actual wastage-FOC)/Total lot";
                    worksheet.Cell(7, 27).Style.Font.SetBold();
                    worksheet.Cell(7, 27).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 27).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 28).Value = "Imported QTY with wastage";
                    worksheet.Cell(7, 28).Style.Font.SetBold();
                    worksheet.Cell(7, 28).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 28).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 29).Value = "Wastage in BOM";
                    worksheet.Cell(7, 29).Style.Font.SetBold();
                    worksheet.Cell(7, 29).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 29).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 30).Value = "Need to Declare";
                    worksheet.Cell(7, 30).Style.Font.SetBold();
                    worksheet.Cell(7, 30).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 30).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 31).Value = "Already Signed";
                    worksheet.Cell(7, 31).Style.Font.SetBold();
                    worksheet.Cell(7, 31).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 31).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 32).Value = "Need Sign";
                    worksheet.Cell(7, 32).Style.Font.SetBold();
                    worksheet.Cell(7, 32).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 32).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 33).Value = "Price";
                    worksheet.Cell(7, 33).Style.Font.SetBold();
                    worksheet.Cell(7, 33).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 33).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 34).Value = "Value";
                    worksheet.Cell(7, 34).Style.Font.SetBold();
                    worksheet.Cell(7, 34).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 34).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 35).Value = "Cross Check";
                    worksheet.Cell(7, 35).Style.Font.SetBold();
                    worksheet.Cell(7, 35).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 35).Style.Alignment.WrapText = true;

                    worksheet.Cell(7, 36).Value = "FOC Date";
                    worksheet.Cell(7, 36).Style.Font.SetBold();
                    worksheet.Cell(7, 36).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 36).Style.Alignment.WrapText = true;


                    worksheet.Cell(7, 37).Value = "FOC Qty";
                    worksheet.Cell(7, 37).Style.Font.SetBold();
                    worksheet.Cell(7, 37).Style.Font.FontColor = XLColor.Black;
                    worksheet.Cell(7, 37).Style.Alignment.WrapText = true;


                    for (int index = 1; index <= details.MaterialWastageDetails.Count; index++)
                    {
                        worksheet.Cell(index + 7, 1).Value = details.MaterialWastageDetails[index - 1].ItemCode;
                        //worksheet.Column(1).AdjustToContents();

                        worksheet.Cell(index + 7, 2).Value = details.MaterialWastageDetails[index - 1].ItemName;
                        //worksheet.Column(2).AdjustToContents();

                        worksheet.Cell(index + 7, 3).Value = details.MaterialWastageDetails[index - 1].BOMUnit;
                        //worksheet.Column(3).AdjustToContents();

                        worksheet.Cell(index + 7, 4).Value = details.MaterialWastageDetails[index - 1].WastagePercentage;
                        //worksheet.Column(4).AdjustToContents();

                        worksheet.Cell(index + 7, 5).Value = details.MaterialWastageDetails[index - 1].RecQtyWOWastage;
                        //worksheet.Column(5).AdjustToContents();


                        worksheet.Cell(index + 7, 6).Value = details.MaterialWastageDetails[index - 1].RecQtyWWastage;
                        //worksheet.Column(6).AdjustToContents();

                        worksheet.Cell(index + 7, 7).Value = details.MaterialWastageDetails[index - 1].TotalLot;
                        //worksheet.Column(7).AdjustToContents();

                        worksheet.Cell(index + 7, 8).Value = details.MaterialWastageDetails[index - 1].WastageWOBom;
                        //worksheet.Column(8).AdjustToContents();

                        worksheet.Cell(index + 7, 9).Value = details.MaterialWastageDetails[index - 1].WastageWBom;
                        //worksheet.Column(9).AdjustToContents();

                        worksheet.Cell(index + 7, 10).Value = details.MaterialWastageDetails[index - 1].TotalWastage;
                        //worksheet.Column(10).AdjustToContents();

                        worksheet.Cell(index + 7, 11).Value = details.MaterialWastageDetails[index - 1].AssemMaterialFault;
                        //worksheet.Column(11).AdjustToContents();

                        worksheet.Cell(index + 7, 12).Value = details.MaterialWastageDetails[index - 1].AssemProcessFault;
                        //worksheet.Column(12).AdjustToContents();

                        worksheet.Cell(index + 7, 13).Value = details.MaterialWastageDetails[index - 1].RepMaterialFault;
                        //worksheet.Column(13).AdjustToContents();

                        worksheet.Cell(index + 7, 14).Value = details.MaterialWastageDetails[index - 1].RepProcessFault;
                        //worksheet.Column(14).AdjustToContents();

                        worksheet.Cell(index + 7, 15).Value = details.MaterialWastageDetails[index - 1].TotalFault;
                        //worksheet.Column(15).AdjustToContents();

                        worksheet.Cell(index + 7, 16).Value = details.MaterialWastageDetails[index - 1].TotalMaterialFaultApproved;
                        //worksheet.Column(16).AdjustToContents();

                        worksheet.Cell(index + 7, 17).Value = details.MaterialWastageDetails[index - 1].TotalProcessFaultApproved;
                        //worksheet.Column(17).AdjustToContents();

                        worksheet.Cell(index + 7, 18).Value = details.MaterialWastageDetails[index - 1].TotalFaultApproved;
                        //worksheet.Column(18).AdjustToContents();

                        worksheet.Cell(index + 7, 19).Value = details.MaterialWastageDetails[index - 1].TillNowAssemMaterialFault;
                        //worksheet.Column(19).AdjustToContents();

                        worksheet.Cell(index + 7, 20).Value = details.MaterialWastageDetails[index - 1].TillNowAssemProcessFault;
                        //worksheet.Column(20).AdjustToContents();

                        worksheet.Cell(index + 7, 21).Value = details.MaterialWastageDetails[index - 1].TillNowRepMaterialFault;
                        //worksheet.Column(21).AdjustToContents();

                        worksheet.Cell(index + 7, 22).Value = details.MaterialWastageDetails[index - 1].TillNowRepProcessFault;
                        //worksheet.Column(22).AdjustToContents();

                        worksheet.Cell(index + 7, 23).Value = details.MaterialWastageDetails[index - 1].TillNowTotalFault;
                        //worksheet.Column(23).AdjustToContents();

                        worksheet.Cell(index + 7, 24).Value = details.MaterialWastageDetails[index - 1].ActualAssemblyWastage_TotalLot;
                        //worksheet.Column(24).AdjustToContents();

                        worksheet.Cell(index + 7, 25).Value = details.MaterialWastageDetails[index - 1].ActualRepairWastage_TotalLot;
                        //worksheet.Column(25).AdjustToContents();

                        worksheet.Cell(index + 7, 26).Value = details.MaterialWastageDetails[index - 1].ActualWastageOfTotalLot;
                        //worksheet.Column(26).AdjustToContents();

                        worksheet.Cell(index + 7, 27).Value = details.MaterialWastageDetails[index - 1].NetAdjustment;
                        //worksheet.Column(27).AdjustToContents();

                        worksheet.Cell(index + 7, 28).Value = details.MaterialWastageDetails[index - 1].ImportedQtyWithWastage;
                        //worksheet.Column(28).AdjustToContents();

                        worksheet.Cell(index + 7, 29).Value = details.MaterialWastageDetails[index - 1].WastageQtyInBOM;
                        //worksheet.Column(29).AdjustToContents();

                        worksheet.Cell(index + 7, 30).Value = details.MaterialWastageDetails[index - 1].NeedToDeclare;
                        //worksheet.Column(30).AdjustToContents();

                        worksheet.Cell(index + 7, 31).Value = details.MaterialWastageDetails[index - 1].AlreadySined;
                        //worksheet.Column(31).AdjustToContents();

                        worksheet.Cell(index + 7, 32).Value = details.MaterialWastageDetails[index - 1].NeedSign;
                        //worksheet.Column(32).AdjustToContents();

                        worksheet.Cell(index + 7, 33).Value = details.MaterialWastageDetails[index - 1].UnitPrice;
                        //worksheet.Column(33).AdjustToContents();

                        worksheet.Cell(index + 7, 34).Value = details.MaterialWastageDetails[index - 1].TotalPrice;
                        //worksheet.Column(34).AdjustToContents();

                        worksheet.Cell(index + 7, 35).Value = details.MaterialWastageDetails[index - 1].CrossCheck;
                        //worksheet.Column(35).AdjustToContents();

                        worksheet.Cell(index + 7, 36).Value = details.MaterialWastageDetails[index - 1].FOCTakenDate;
                        //worksheet.Column(36).AdjustToContents();

                        worksheet.Cell(index + 7, 37).Value = details.MaterialWastageDetails[index - 1].FOCQty;


                    }

                    var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public JsonResult CompleteReport(long id)
        {
            ResponseModel model = _commonRepository.CompleteReport(id);
            return new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult AddItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddItem(List<MaterialWastageItemModel> models )
        {
            return View();
        }

        public ActionResult EditWastageMaterialReport(long id)
        {
            WastageFileUpload model = _commonRepository.GetMaterialWastageById(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditWastageMaterialReport(WastageFileUpload model)
        {
            return View(model);
        }

    }


    public class WastageFileUpload
    {
        public WastageFileUpload()
        {
            MaterialWastageDetails = new List<MaterialWastageDetail>();
        }
        public long VariantId { get; set; }
        public string ProjectName { get; set; }
        public HttpPostedFileBase HttpPostedFileBase { get; set; }
        public MaterialWastageMaster MaterialWastageMaster { get; set; }
        public List<MaterialWastageDetail> MaterialWastageDetails { get; set; }

        public string RecommendationRemarks { get; set; }
        public string ApprovalRemarks { get; set; }

        public double Average1 { get; set; }
        public double Average2 { get; set; }
        public double Average3 { get; set; }
        public double Average4 { get; set; }
    }
}