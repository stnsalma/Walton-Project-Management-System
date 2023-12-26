using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "BDIQC,BDIQCHEAD,FIQC,FIQCHEAD,SA,AUD")]
    public class IqcController : Controller
    {
        private readonly CellPhoneProjectEntities _dbEntities;
        private readonly IIqcRepository _iqcRepository;
        private readonly ICommonRepository _commonRepository;

        public IqcController(IqcRepository iqcRepository,CommonRepository commonRepository)
        {
            _iqcRepository = iqcRepository;
            _commonRepository = commonRepository;
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }
        //
        // GET: /Process/
        public ActionResult BdIqc(long id=0)
        {
            ViewBag.Projects = _iqcRepository.GetAllVariants();
            return View();
        }

        public JsonResult SaveBdIqc(BdIqcModel iqc, List<BdIqcBomPassRecordModel> bdIqcBomPassRecord)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            if (iqc.Id > 0)
            {
                iqc.UpdatedBy = userId;
                iqc.UpdatedDate = DateTime.Now;
            }
            else
            {
                iqc.AddedBy = userId;
                iqc.AddedDate = DateTime.Now;
            }
            var bdiqc = _iqcRepository.SaveBdIqc(iqc);
            if (bdiqc != null && bdiqc.Id > 0)
            {
                _iqcRepository.SaveBdIqcBomPassRecords(bdIqcBomPassRecord);
            }
            return Json(true);
        }

        public ActionResult ForeignIqcList()
        {
            var mod = _iqcRepository.GetForeignIqcModels();
            return View(mod);
        }

        public ActionResult ForeignIqc(long id = 0)
        {
            ViewBag.Projects = _iqcRepository.GetAllVariants();
            return View();
        }

        public JsonResult SaveForeignIqc(ForeignIqcModel iqc, List<ForeignIqcBomPassRecordModel> foreignIqcBomPassRecord)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var exist = _iqcRepository.GetForIqcByVariantIdAndInspNo(iqc.VariantId, iqc.NoOfInspectionTime);
            if (exist != null)
            {
                iqc.Id = exist.Id;
            }
            var response = "";
            if (iqc.Id > 0)
            {
                iqc.UpdatedBy = userId;
                iqc.UpdatedDate = DateTime.Now;
                response = "Updated";
            }
            else
            {
                iqc.AddedBy = userId;
                iqc.AddedDate = DateTime.Now;
                response = "Saved";
            }
            var ffiqc = _iqcRepository.SaveForeignIqc(iqc);
            if (ffiqc != null && ffiqc.Id > 0)
            {
                foreach (var fiqc in foreignIqcBomPassRecord)
                {
                    fiqc.ForeignIqcId = ffiqc.Id;
                }
                _iqcRepository.SaveForeignIqcBomPassRecords(foreignIqcBomPassRecord);
            }
            return Json(response);
        }

        public JsonResult GetBom(long id,string insno)
        {
            if (User.IsInRole("BDIQC") || User.IsInRole("BDIQCHEAD"))
            {
                var savedbomrecord = _iqcRepository.GetBdIqcBomPassRecordByVariantId(id);
                var bdiqc = _iqcRepository.GetBdIqcByVariantId(id);
                if (!savedbomrecord.Any())
                {
                    var projectinfo = _iqcRepository.GetVariantById(id);
                    var boms = _iqcRepository.GetBomByProjectModel(projectinfo.ProjectModel);
                    if (boms != null)
                    {
                        foreach (var b in boms)
                        {
                            var model = new BdIqcBomPassRecordModel
                            {
                                SpareDescription = b.SpareDescription,
                                Description = b.Description,
                                VariantId = projectinfo.Id,
                                ProjectId = projectinfo.ProjectMasterId,
                                BOMType = b.BOMType,
                                BomId = b.Id,
                                BomQuantity = Convert.ToString(projectinfo.OrderQuantity)
                            };
                            savedbomrecord.Add(model);
                        }
                    }
                }
                return Json(new { iqc = bdiqc, bomrecord = savedbomrecord });
            }
            if (User.IsInRole("FIQC") || User.IsInRole("FIQCHEAD"))
            {
                var savedbomrecord = new List<ForeignIqcBomPassRecordModel>();
                var fiqc = _iqcRepository.GetForIqcByVariantIdAndInspNo(id, insno);
                if (fiqc != null)
                {
                    savedbomrecord = _iqcRepository.GetForeignIqcBomPassRecordByForIqcId(fiqc.Id);
                }
                if (!savedbomrecord.Any())
                {
                    var projectinfo = _iqcRepository.GetVariantById(id);
                    var boms = _iqcRepository.GetBomByProjectModel(projectinfo.ProjectModel);
                    if (boms != null)
                    {
                        foreach (var b in boms)
                        {
                            var model = new ForeignIqcBomPassRecordModel
                            {
                                SpareDescription = b.SpareDescription,
                                Description = b.Description,
                                VariantId = projectinfo.Id,
                                ProjectId = projectinfo.ProjectMasterId,
                                BOMType = b.BOMType,
                                BomId = b.Id,
                                BomQuantity = Convert.ToString(projectinfo.OrderQuantity)
                            };
                            savedbomrecord.Add(model);
                        }
                    }
                }
                return Json(new { iqc = fiqc, bomrecord = savedbomrecord });
            }
            return Json("something went wrong!!");
        }
	}
}