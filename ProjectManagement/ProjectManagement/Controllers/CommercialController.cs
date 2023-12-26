using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.VariantTypes;
using Ionic.Zip;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.AftersalesPm;
using ProjectManagement.ViewModels.Commercial;
using ProjectManagement.ViewModels.Global;
using ProjectManagement.ViewModels.Software;
using SignalRDemo.DAL;
//using ClosedXML.Excel;
using Supplier = ProjectManagement.Models.SupplierModel;

namespace ProjectManagement.Controllers
{

    [Authorize(Roles = "CM, CMBTRC, SA, CMHEAD,SPRHEAD,PM,PMHEAD, ACCNT,ACCNTHEAD,FIN,FINHEAD,CEO,PS, MM,AUDHEAD,AUD,BIHEAD")]

    public class CommercialController : Controller
    {
        private readonly ICommercialRepository _repository;
        private readonly IProjectManagerRepository _projectManagerRepository;
        private readonly IHardwareRepository _hardwareRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IIqcRepository _iqcRepository;
        private readonly ClientSideResponse _response;
        private NotificationObject _notificationObject;
        private readonly CustomSelectListItemRepository _customSelectListItemRepository;
        private readonly IProjectMasterRepository _projectMasterRepository;
        public CommercialController(CommercialRepository repository, ClientSideResponse response, ProjectManagerRepository projectManagerRepository, CustomSelectListItemRepository customSelectListItemRepository, HardwareRepository hardwareRepository, HomeRepository homeRepository, ProjectMasterRepository projectMasterRepository,CommonRepository commonRepository,IqcRepository iqcRepository )
        {
            _repository = repository;
            _response = response;
            _projectManagerRepository = projectManagerRepository;
            _customSelectListItemRepository = customSelectListItemRepository;
            _hardwareRepository = hardwareRepository;
            _homeRepository = homeRepository;
            _projectMasterRepository = projectMasterRepository;
            _commonRepository = commonRepository;
            _iqcRepository = iqcRepository;
        }
        // GET: Commercial
        public ActionResult Index()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var masterModels = _hardwareRepository.GetAllProjects();
            var items = new List<SelectListItem> { new SelectListItem { Value = "-1", Text = "Top 5 Projects" }, new SelectListItem { Value = "0", Text = "Latest 5 Projects" } };
            items.AddRange(masterModels.Select(masterModel => new SelectListItem
            {
                Value = masterModel.ProjectMasterId.ToString(CultureInfo.InvariantCulture),
                Text = masterModel.ProjectName
            }));
            ViewBag.ProjectMaster = items;
            //List<WorkProgressData> progresses = _repository.ProjectMonthlyWorkprogress(projectId);
            //ViewBag.work = JsonConvert.SerializeObject(progresses);
            var dashBoardCounter = new DashBoardCounter();
            ViewBag.MmDashboardConter = dashBoardCounter.GetDashBoardCounter("Commercial", "CM", userId);
            //ViewBag.Project = projectName;
            return View();
        }

        public ActionResult Projects()
        {
            List<ProjectMasterModel> models = _repository.GetAllCreatedProjects();
            return View(models);
        }

        public ActionResult Project(long projectId = 0)
        {

            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            if (userId > 0)
            {
                ViewBag.Projects = _repository.GetAllProjects().Where(i => i.Added == userId);
                var supplierSelectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "--Select One--" } };
                var componentsSelectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "--Select One--" } };
                var chipsetSelectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "--Select One--" } };
                var suppliers = _repository.GeTAllSuppliers();
                var accessories = _repository.GetAllAccessories();
                var chipsets = _hardwareRepository.GetAllHwChipsetModel();
                supplierSelectListItems.AddRange(suppliers.Select(supplier => new SelectListItem { Value = supplier.SupplierId.ToString(CultureInfo.InvariantCulture), Text = supplier.SupplierName }));
                componentsSelectListItems.AddRange(accessories.Select(accessory => new SelectListItem { Value = accessory.AccessoryName, Text = accessory.AccessoryName }));
                chipsetSelectListItems.AddRange(chipsets.Select(chipset => new SelectListItem { Value = chipset.IcNoSize, Text = chipset.IcNoSize }));
                ViewBag.Suppliers = supplierSelectListItems;
                ViewBag.Chipsets = chipsetSelectListItems;
                ViewBag.Components = componentsSelectListItems;
                ViewBag.Brand = _repository.GetBrands();

                var cpuCoreListItems = new List<SelectListItem>
                {
                    new SelectListItem {Value = "", Text = "--Select One--"}
                };
                List<string> cpuCoreList = _repository.GetCpuCores();
                cpuCoreListItems.AddRange(cpuCoreList.Select(cpu => new SelectListItem { Value = cpu, Text = cpu }));
                ViewBag.CpuCores = cpuCoreListItems;
                var model = new ProjectMasterModel { IsActive = false };
                if (projectId > 0)
                {
                    model = _repository.GetProjectModel(projectId);
                }
                return View(model);
            }
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Project(ProjectModel model)
        {
            return RedirectToAction("Project", new { projectId = model.ProjectMasterId });
        }

        [HttpPost]
        public ActionResult AddAccessoryPrice(AccessoriesPricesModel price)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            decimal priceDecimal;
            decimal.TryParse(price.Price, out priceDecimal);
            decimal duty;
            decimal.TryParse(price.Duty, out duty);
            price.TotalPrice = Convert.ToString(Math.Round(priceDecimal + (priceDecimal*duty)/100, 2));
            if (priceDecimal > 0)
            {
                var priceModel = new AccessoriesPricesModel
                {
                    Id = (price.Id > 0 ? price.Id : 0),
                    ProjectMasterId = (price.ProjectMasterId > 0 ? price.ProjectMasterId : 0),
                    AccessoryName = price.AccessoryName,
                    Price = price.Price,
                    Vendor = price.Vendor,
                    Currency = price.Currency,
                    Type = price.Type,
                    Duty = price.Duty,
                    TotalPrice = price.TotalPrice,
                    AddedBy = userId,
                    AddedDate = (price.AddedDate ?? DateTime.Today)
                };
                return PartialView("~/Views/Commercial/CmPartials/_AccessoryPrice.cshtml", priceModel);
            }
            else
            {
                TempData["message"] = "Invalid Price";
                TempData["messageType"] = 2;
                return PartialView("~/Views/Commercial/CmPartials/_AccessoryPrice.cshtml", null);
            }
           
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PM,PS", MessageHeader = "New Project")]
        [HttpPost]
        public ActionResult StartProject(ProjectMasterModel model)
        {
            //if (ModelState.IsValid)
            //{
                if (model.ProjectTypeId == 1)
                {
                    model.ProjectType = "Smart";
                }
                else if (model.ProjectTypeId == 2)
                {
                    model.ProjectType = "Feature";
                }
                else if (model.ProjectTypeId == 3)
                {
                    model.ProjectType = "Walpad";
                }
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);

                if (model.ProjectMasterId > 0)
                {

                    _notificationObject = new NotificationObject
                    {
                        ModelId = model.ProjectMasterId,
                        ProjectId = model.ProjectMasterId,
                        AdditionalInformation = "Supplier : " + model.SupplierName + ", Appx. Price: " + model.ApproximatePrice + ", Final Price: " + model.FinalPrice
                    };
                    ViewBag.ControllerVariable = _notificationObject;
                    ViewBag.Components = _repository.GetAllAccessories();
                    ////=====file upload====
                    //var moduleDirectory = model.ProjectName;
                    //var userDirectory = "CM"; 
                    //var manager = new FileManager();
                    //var newFilePath = manager.DocManagementUpload(userDirectory, moduleDirectory,
                    //        model.ProjectImageFileBase);
                    //model.ProjectImagePath = model.ProjectImagePath != newFilePath
                    //    ? newFilePath
                    //    : model.ProjectImagePath;
                    ////======
                    var projectId = _repository.UpdateProject(model, userId);//===Update Project===
                    if (projectId > 0)
                    {
                        _response.MessageType = 3;
                        _response.Message = "Update Successfully!!";
                    }
                    else
                    {
                        _response.MessageType = 2;
                        _response.Message = "Something goes wrong !!!";
                    }
                }
                else
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(model.ProjectName))
                            model.ProjectName = model.ProjectNameForScreening;
                        model.IsNew = model.ProjectTypeId == 1 ? false : true;
                        //model.ProjectStatus = "NEW";
                        model.ProjectStatus = model.ProjectTypeId == 1 ? "SWOTPENDING" : "NEW";
                        model.IsProjectManagerAssigned = false;
                        var projectId = _repository.SaveProject(model, userId); //===Save project function ===
                        if (projectId > 0)
                        {
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.ProjectMasterId,
                                ProjectId = projectId,
                                AdditionalInformation =
                                    "Supplier : " + model.SupplierName + ", Appx. Price: " + model.ApproximatePrice +
                                    ", Final Price: " + model.FinalPrice
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Successfully Saved Project";
                        }
                        else
                        {
                            _response.MessageType = 2;
                            _response.Message = "Something goes wrong !!!";
                        }
                    }
                    catch (Exception ex)
                    {
                        _response.MessageType = 2;
                        _response.Message = ex.Message;
                    }
                    
                }
                TempData["message"] = _response.Message;
                TempData["messageType"] = _response.MessageType;
                return RedirectToAction("Project", new { projectId = model.ProjectMasterId });
            //}
            //_response.MessageType = 2;
            //_response.Message = "Something goes wrong !!!";
            //TempData["message"] = _response.Message;
            //TempData["messageType"] = _response.MessageType;
            //return RedirectToAction("Project", new { projectId = model.ProjectMasterId });
        }
        [HttpPost]
        public JsonResult GetProductDataJson()
        {
            var data = new List<int> { 1, 2, 3 };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Specification(long projectId = 0)
        {
            //if (type > 0)
            //{
            //    TempData["message"] = message;
            //    TempData["messageType"] = type;
            //}
            ViewBag.Projects = _repository.GetAllProjects();
            var vmSpecification = new VmSpecification();
            if (projectId <= 0)
            {
                return View(vmSpecification);
            }
            vmSpecification.ProjectMaster = _repository.GetProjectMasterModel(projectId) ?? new ProjectMasterModel();
            vmSpecification.PhAccessory = _repository.GetPhAccessoryModel(projectId: projectId) ?? new PhAccessoryModel();
            vmSpecification.PhBatteryInfoModel = _repository.GetPhBatteryInfoModel(projectId: projectId) ?? new PhBatteryInfoModel();
            vmSpecification.PhCamInfo = _repository.GetPhCamInfoModel(projectId: projectId) ?? new PhCamInfoModel();
            vmSpecification.PhChipsetInfo = _repository.GetPhChipsetInfoModel(projectId: projectId) ?? new PhChipsetInfoModel();
            vmSpecification.PhColorInfoModel = _repository.GetPhColorInfoModel(projectId: projectId) ?? new PhColorInfoModel();
            vmSpecification.PhHousingInfo = _repository.GetPhHousingInfoModel(projectId: projectId) ?? new PhHousingInfoModel();
            vmSpecification.PhMemoryInfo = _repository.GetPhMemoryInfoModel(projectId: projectId) ?? new PhMemoryInfoModel();
            vmSpecification.PhNetworkFreqAndBand = _repository.GetPhNetworkFreqAndBandModel(projectId: projectId) ?? new PhNetworkFreqAndBandModel();
            vmSpecification.PhOperatingSyModel = _repository.GetPhOperatingSyModel(projectId: projectId) ?? new PhOperatingSyModel();
            vmSpecification.PhPcbaInfo = _repository.GetPhPcbaInfoModel(projectId: projectId) ?? new PhPcbaInfoModel();
            vmSpecification.PhSensorAndOther = _repository.GetPhSensorAndOtherModel(projectId: projectId) ?? new PhSensorAndOtherModel();
            vmSpecification.PhTpLcdInfo = _repository.GetPhTpLcdInfoModel(projectId: projectId) ?? new PhTpLcdInfoModel();
            //var res = _repository.GetAllProjectSpecification(projectId);
            vmSpecification.ProjectMaster.ProjectMasterId = projectId;
            return View(vmSpecification);
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PM,PS", MessageHeader = "Project Full Specification")]
        [HttpPost]
        public ActionResult Specification(VmSpecification model)
        {
            long projectMasterId = model.ProjectMaster.ProjectMasterId;
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.Projects = _repository.GetAllProjects();
            if (projectMasterId > 0)
            {
                #region PCBA Not

                long res;
                if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Pcba)
                {
                    if (ModelState.IsValidField("PhPcbaInfo"))
                    {
                        if (model.PhPcbaInfo.PhPcbaInfoId > 0)
                        {
                            res = _repository.UpdatePhPcbaInfo(model.PhPcbaInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhPcbaInfo.PhPcbaInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "PCBA part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "PCBA Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhPcbaInfo.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhPcbaInfo(model.PhPcbaInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "PCBA part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "PCBA Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification",
                                new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region TpLcd not
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.TpLcd)
                {
                    if (ModelState.IsValidField("PhTpLcdInfo"))
                    {
                        if (model.PhTpLcdInfo.PhTpLcdInfoId > 0)
                        {
                            res = _repository.UpdatePhTpLcdInfo(model.PhTpLcdInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhTpLcdInfo.PhTpLcdInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "TP-LCD part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "TP LCD Information Updated Successfully!";

                        }
                        else
                        {
                            model.PhTpLcdInfo.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhTpLcdInfo(model.PhTpLcdInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "TP-LCD part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "TP LCD Information Saved Successfully!";

                        }

                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification",
                                new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Housing
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Housing)
                {
                    if (ModelState.IsValidField("PhHousingInfo"))
                    {
                        if (model.PhHousingInfo.PhHousingInfoId > 0)
                        {
                            res = _repository.UpdatePhHousingInfo(model.PhHousingInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhHousingInfo.PhHousingInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "TP-LCD part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Housing Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhHousingInfo.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhHousingInfo(model.PhHousingInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Housing part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Housing Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Camera
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Camera)
                {
                    if (ModelState.IsValidField("PhCamInfo"))
                    {
                        if (model.PhCamInfo.PhCamInfoId > 0)
                        {
                            res = _repository.UpdatePhCamInfo(model.PhCamInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhCamInfo.PhCamInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Camera part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Camera Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhCamInfo.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhCamInfo(model.PhCamInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Camera part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Camera Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Chipset
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Chipset)
                {
                    if (ModelState.IsValidField("PhChipsetInfo"))
                    {
                        if (model.PhChipsetInfo.PhChipsetInfoId > 0)
                        {
                            res = _repository.UpdatePhChipsetInfo(model.PhChipsetInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhChipsetInfo.PhChipsetInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Chipset part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Chipset Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhChipsetInfo.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhChipsetInfo(model.PhChipsetInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Housing part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Chipset Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Memory
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Memory)
                {
                    if (ModelState.IsValidField("PhMemoryInfo"))
                    {
                        if (model.PhMemoryInfo.PhMemoryInfoId > 0)
                        {
                            res = _repository.UpdatePhMemoryInfo(model.PhMemoryInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhMemoryInfo.PhMemoryInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Memory part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Memory Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhMemoryInfo.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhMemoryInfo(model.PhMemoryInfo, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Memory part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Memory Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Sensor
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Sensor)
                {
                    if (ModelState.IsValidField("PhSensorAndOther"))
                    {
                        if (model.PhSensorAndOther.PhSensorAndOthersInfoId > 0)
                        {
                            res = _repository.UpdatePhSensorAndOther(model.PhSensorAndOther, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhSensorAndOther.PhSensorAndOthersInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Sensor part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Sensor Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhSensorAndOther.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhSensorAndOther(model.PhSensorAndOther, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Sensor part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Sensor Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Accessories
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Accessories)
                {
                    if (ModelState.IsValidField("PhAccessory"))
                    {
                        if (model.PhAccessory.PhAccessoriesId > 0)
                        {
                            res = _repository.UpdatePhAccessory(model.PhAccessory, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhAccessory.PhAccessoriesId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Accessories part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Accessories Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhAccessory.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhAccessory(model.PhAccessory, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Accessories part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Accessories Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region OS
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Os)
                {
                    if (ModelState.IsValidField("PhOperatingSyModel"))
                    {
                        if (model.PhOperatingSyModel.PhOsId > 0)
                        {
                            res = _repository.UpdatePhOperatingSyModel(model.PhOperatingSyModel, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhOperatingSyModel.PhOsId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Operating System part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "OS Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhOperatingSyModel.ProjectMasterId =
                                model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhOperatingSyModel(model.PhOperatingSyModel, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Operating System part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "OS Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Network
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Network)
                {
                    if (ModelState.IsValidField("PhNetworkFreqAndBand"))
                    {
                        if (model.PhNetworkFreqAndBand.PhNetworkFreqAndBandsId > 0)
                        {
                            res = _repository.UpdatePhNetworkFreqAndBand(model.PhNetworkFreqAndBand, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhNetworkFreqAndBand.PhNetworkFreqAndBandsId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Network part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Network Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhNetworkFreqAndBand.ProjectMasterId =
                                model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhNetworkFreqAndBand(model.PhNetworkFreqAndBand, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Network part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Network Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Battery
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Battery)
                {
                    if (ModelState.IsValidField("PhBatteryInfoModel"))
                    {
                        if (model.PhBatteryInfoModel.PhBatteryInfoId > 0)
                        {
                            res = _repository.UpdatePhBatteryInfoModel(model.PhBatteryInfoModel, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhBatteryInfoModel.PhBatteryInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Battery part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Battery Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhBatteryInfoModel.ProjectMasterId =
                                model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhBatteryInfoModel(model.PhBatteryInfoModel, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Battery part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Battery Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
                #region Color
                else if (model.TabIdentity == (decimal)GlobalIdentifier.CommercialTabType.Color)
                {
                    if (ModelState.IsValidField("PhColorInfoModel"))
                    {
                        if (model.PhColorInfoModel.PhColorInfoId > 0)
                        {
                            res = _repository.UpdatePhColorInfoModel(model.PhColorInfoModel, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = model.PhColorInfoModel.PhColorInfoId,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Color part updated"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 3;
                            _response.Message = "Color Information Updated Successfully!";
                        }
                        else
                        {
                            model.PhColorInfoModel.ProjectMasterId = model.ProjectMaster.ProjectMasterId;
                            res = _repository.SavePhColorInfoModel(model.PhColorInfoModel, userId);
                            _notificationObject = new NotificationObject
                            {
                                ModelId = 0,
                                ProjectId = projectMasterId,
                                AdditionalInformation = "Color part newly saved"
                            };
                            ViewBag.ControllerVariable = _notificationObject;
                            _response.MessageType = 1;
                            _response.Message = "Color Information Saved Successfully!";
                        }
                        if (res > 0)
                        {
                            TempData["message"] = _response.Message;
                            TempData["messageType"] = _response.MessageType;
                            return RedirectToAction("Specification", new { projectId = model.ProjectMaster.ProjectMasterId });
                        }
                        _response.MessageType = 2;
                        _response.Message = "Error Occured. Contact with Administrator!";
                        TempData["message"] = _response.Message;
                        TempData["messageType"] = _response.MessageType;
                        return View(model);
                    }
                }
                #endregion
            }
            _response.MessageType = 2;
            _response.Message = "Please select a project first, before giving specification";
            TempData["message"] = _response.Message;
            TempData["messageType"] = _response.MessageType;
            return View(model);
        }
        [HttpGet]
        public ActionResult CriticalControlPoint(long projectId = 0, int type = 0, string msg = null)
        {
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            ViewBag.Projects = _repository.GetAllProjectWithOrderNumber();
            var vmProjectCriticalControlPoint = new VmProjectCriticalControlPoint();
            if (projectId <= 0)
            {
                return View(vmProjectCriticalControlPoint);
            }
            vmProjectCriticalControlPoint.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
            vmProjectCriticalControlPoint.ProjectCriticalControlPointModel = _repository.GetProjectCriticalControlPointModel(projectId: projectId) ?? new ProjectCriticalControlPointModel();
            return View(vmProjectCriticalControlPoint);
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PS", MessageHeader = "Critical Control Point (CCP)")]
        [HttpPost]
        public ActionResult CriticalControlPoint(VmProjectCriticalControlPoint vmProjectCriticalControlPoint)
        {
            vmProjectCriticalControlPoint.ProjectCriticalControlPointModel.ProjectMasterId =
                vmProjectCriticalControlPoint.ProjectMasterModel.ProjectMasterId;
            if (ModelState.IsValidField("ProjectCriticalControlPointModel"))
            {
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
                _notificationObject = new NotificationObject();
                if (vmProjectCriticalControlPoint.ProjectCriticalControlPointModel.ProjectCriticalControlPointId > 0)
                {
                    var res = _repository.UpdateProjectCriticalControlPointModel(vmProjectCriticalControlPoint.ProjectCriticalControlPointModel, userId);
                    if (res > 0)
                    {
                        _notificationObject.ProjectId = vmProjectCriticalControlPoint.ProjectMasterModel.ProjectMasterId;
                        _notificationObject.ModelId =
                            vmProjectCriticalControlPoint.ProjectCriticalControlPointModel.ProjectCriticalControlPointId;
                        ViewBag.ControllerVariable = _notificationObject;
                        return RedirectToAction("CriticalControlPoint", new { projectId = vmProjectCriticalControlPoint.ProjectMasterModel.ProjectMasterId, type = 1, msg = "CCP Update Successfully!" });
                    }
                }

                vmProjectCriticalControlPoint.ProjectCriticalControlPointModel.ProjectMasterId = vmProjectCriticalControlPoint.ProjectMasterModel.ProjectMasterId;
                var result = _repository.SaveCriticalControlPoint(vmProjectCriticalControlPoint.ProjectCriticalControlPointModel, userId);
                if (result > 0)
                {
                    _notificationObject.ProjectId = vmProjectCriticalControlPoint.ProjectMasterModel.ProjectMasterId;
                    _notificationObject.ModelId = vmProjectCriticalControlPoint.ProjectCriticalControlPointModel.ProjectCriticalControlPointId;
                    ViewBag.ControllerVariable = _notificationObject;
                    return RedirectToAction("CriticalControlPoint", new { projectId = vmProjectCriticalControlPoint.ProjectMasterModel.ProjectMasterId, type = 1, msg = "CCP Saved Successfully!" });
                }

            }
            TempData["message"] = "Error Occured, Please Check your form carefully or Contact with Adminstrator";
            TempData["messageType"] = 2;
            ViewBag.Projects = _repository.GetAllProjects();

            return View(vmProjectCriticalControlPoint);
        }
        public ActionResult ProformaInvoice(long projectId = 0, int type = 0, string msg = null)
        {
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            //var abc = fileManager.GetFile(string.Empty);
            ViewBag.Projects = _repository.GetAllProjectWithOrderNumber();
            var vmProformaInvoice = new VmProformaInvoice();
            if (projectId <= 0)
            {
                return View(vmProformaInvoice);
            }
            vmProformaInvoice.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
            vmProformaInvoice.ProjectProformaInvoiceModel = _repository.GetProjectProformaInvoiceModel(projectId: projectId) ?? new ProjectProformaInvoiceModel();
            return View(vmProformaInvoice);
        }

        [NotificationActionFilter(ReceiverRoles = "CM,CMBTRC,MM,PS", MessageHeader = "Proforma Invoice (PI)")]
        [HttpPost]
        public ActionResult ProformaInvoice(VmProformaInvoice vmProformaInvoice)
        {
            if (ModelState.IsValidField("ProjectProformaInvoiceModel"))
            {
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
                var fileManager = new FileManager();
                var filePath = fileManager.Upload(vmProformaInvoice.ProjectMasterModel.ProjectMasterId, "CM", "PI", vmProformaInvoice.ProjectProformaInvoiceModel.File);
                _notificationObject = new NotificationObject();
                vmProformaInvoice.ProjectProformaInvoiceModel.FilePath = filePath;
                ViewBag.Projects = _repository.GetAllProjects();
                if (vmProformaInvoice.ProjectProformaInvoiceModel.ProjectProformaInvoiceId > 0)
                {
                    var res = _repository.UpdateProjectProformaInvoice(vmProformaInvoice.ProjectProformaInvoiceModel, userId);
                    if (res > 0)
                    {
                        _notificationObject.ProjectId = vmProformaInvoice.ProjectMasterModel.ProjectMasterId;
                        _notificationObject.ModelId = vmProformaInvoice.ProjectProformaInvoiceModel.ProjectProformaInvoiceId;
                        ViewBag.ControllerVariable = _notificationObject;
                        return RedirectToAction("ProformaInvoice", new { projectId = vmProformaInvoice.ProjectMasterModel.ProjectMasterId, type = 3, msg = "Proforma Invoice (PI) Updated Successfully!!!" });
                    }
                }
                vmProformaInvoice.ProjectProformaInvoiceModel.ProjectMasterId = vmProformaInvoice.ProjectMasterModel.ProjectMasterId;
                var result = _repository.SaveProjectProformaInvoice(vmProformaInvoice.ProjectProformaInvoiceModel, userId);
                if (result > 0)
                {
                    _notificationObject.ProjectId = vmProformaInvoice.ProjectMasterModel.ProjectMasterId;
                    _notificationObject.ModelId = 0;
                    ViewBag.ControllerVariable = _notificationObject;
                    return RedirectToAction("ProformaInvoice", new { projectId = vmProformaInvoice.ProjectMasterModel.ProjectMasterId, type = 1, msg = "Proforma Invoice (PI) Saved Successfully!!!" });
                }

            }
            TempData["message"] = "Error Occured, Please Check your form carefully or Contact with Adminstrator";
            TempData["messageType"] = 2;
            ViewBag.Projects = _repository.GetAllProjects();
            return View(vmProformaInvoice);
        }
        //public ActionResult ProjectOrder(long projectId = 0, int type = 0, string msg = null)
        //{
        //    if (type > 0)
        //    {
        //        TempData["message"] = msg;
        //        TempData["messageType"] = type;
        //    }
        //    var model = new VmProjectOrder();
        //    if (projectId > 0)
        //    {
        //        model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
        //        model.ProjectOrderModel = _repository.GetProjectOrderModel(projectId: projectId) ?? new ProjectOrderModel();
        //    }
        //    ViewBag.Projects = _repository.GetAllProjects();
        //    return View(model);
        //}


        //[HttpPost]
        //public ActionResult ProjectOrder(VmProjectOrder vmProjectOrder)
        //{
        //    if (ModelState.IsValidField("ProjectOrderModel"))
        //    {
        //        long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);

        //        if (vmProjectOrder.ProjectOrderModel.ProjectOrderId > 0)
        //        {
        //            var res = _repository.UpdateProjectOrder(vmProjectOrder.ProjectOrderModel, userId);
        //            return RedirectToAction("ProjectOrder", new { projectId = vmProjectOrder.ProjectMasterModel.ProjectMasterId, type = 3, msg = "Project Order Updated Successfully !!!" });
        //        }
        //        else
        //        {
        //            vmProjectOrder.ProjectOrderModel.ProjectMasterId = vmProjectOrder.ProjectMasterModel.ProjectMasterId;
        //            var res = _repository.SaveProjectOrder(vmProjectOrder.ProjectOrderModel, userId);
        //            return RedirectToAction("ProjectOrder", new { projectId = vmProjectOrder.ProjectMasterModel.ProjectMasterId, type = 1, msg = "Project Order Saved Successfully !!!" });
        //        }
        //    }
        //    TempData["message"] = "Error Occured, Please Check your form carefully or Contact with Adminstrator";
        //    TempData["messageType"] = 2;
        //    ViewBag.Projects = _repository.GetAllProjects();
        //    return View(vmProjectOrder);
        //    //return RedirectToAction("ProjectOrder", new { projectId  = vmProjectOrder.ProjectMasterModel.ProjectMasterId});
        //}
        //[HttpGet]
        //public ActionResult BtrcNoc(long projectId = 0)
        //{
        //    ViewBag.Projects = _repository.GetAllProjects();
        //    var model = new VmProjectBtrcNoc();
        //    if (projectId > 0)
        //    {
        //        model.ProjectBtrcNocModel = _repository.GetProjectBtrcNoc(projectId: projectId);
        //        model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
        //    }
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult BtrcNoc(VmProjectBtrcNoc btrcNoc)
        //{
        //    long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
        //    bool isFileNull = true;
        //    if (btrcNoc.File != null)
        //    {
        //        isFileNull = false;
        //        var manager = new FileManager();
        //        var filePath = manager.Upload(btrcNoc.ProjectMasterModel.ProjectMasterId, "CM", "BtrcNoc", btrcNoc.File);
        //        //btrcNoc.ProjectBtrcNocModel.FilePath = filePath;
        //    }
        //    //if (btrcNoc.ProjectBtrcNocModel.ProjectBtrcNocId > 0)
        //    //{
        //    //    long result = _repository.UpdateProjectBtrcNoc(btrcNoc.ProjectBtrcNocModel, isFileNull, userId);
        //    //    if (result > 0)
        //    //    {
        //    //        return RedirectToAction("BtrcNoc", new { projectId = btrcNoc.ProjectMasterModel.ProjectMasterId });
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    btrcNoc.ProjectBtrcNocModel.ProjectMasterId = btrcNoc.ProjectMasterModel.ProjectMasterId;
        //    //    long result = _repository.SaveProjectBtrcNoc(btrcNoc.ProjectBtrcNocModel, userId);
        //    //    if (result > 0)
        //    //    {
        //    //        return RedirectToAction("BtrcNoc", new { projectId = btrcNoc.ProjectMasterModel.ProjectMasterId });
        //    //    }
        //    //}
        //    return View();
        //}

        [HttpGet]
        public ActionResult Lc(long id = 0, long projectId = 0, int type = 0, string msg = null)
        {
            var model = new VmProjectLc();
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            ViewBag.Projects = _repository.GetAllProjectWithOrderNumber();
            if (id > 0 && projectId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                model.ProjectLcModel = _repository.GetProjectLc(id);
                var pos = _repository.GetProjectOrderModels(projectId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;
            }
            else if (projectId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                var pos = _repository.GetProjectOrderModels(projectId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;

            }
            else
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                ViewBag.ProjectOrders = selectListItems;
            }
            return View(model);
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PM,PS", MessageHeader = "Project LC")]
        [HttpPost]
        public ActionResult Lc(VmProjectLc model)
        {
            if (ModelState.IsValidField("ProjectLcModel"))
            {
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
                _notificationObject = new NotificationObject();
                if (model.ProjectLcModel.ProjectLcId > 0)
                {
                    //update
                    _repository.UpdatePoNoFromLc(model.PoNumber,model.ProjectLcModel.ProjectOrderId,model.Lc1,model.Lc2);
                    var res = _repository.UpdateProjectLc(model.ProjectLcModel, userId);
                    if (res > 0)
                    {
                        _notificationObject.ProjectId = model.ProjectMasterModel.ProjectMasterId;
                        _notificationObject.ModelId = model.ProjectLcModel.ProjectLcId;
                        ViewBag.ControllerVariable = _notificationObject;
                        return RedirectToAction("Lc", new { projectId = model.ProjectMasterModel.ProjectMasterId, type = 3, msg = "LC Updated Successfull !!!" });
                    }
                }
                else
                {
                    _repository.UpdatePoNoFromLc(model.PoNumber, model.ProjectLcModel.ProjectOrderId, model.Lc1, model.Lc2);
                    //create
                    model.ProjectLcModel.ProjectMasterId = model.ProjectMasterModel.ProjectMasterId;
                    var res = _repository.SaveProjectLc(model.ProjectLcModel, userId);
                    if (res > 0)
                    {
                        _notificationObject.ProjectId = model.ProjectMasterModel.ProjectMasterId;
                        _notificationObject.ModelId = 0;
                        ViewBag.ControllerVariable = _notificationObject;
                        return RedirectToAction("Lc", new { projectId = model.ProjectMasterModel.ProjectMasterId, type = 1, msg = "LC Saved Successfull !!!" });
                    }
                }

            }
            TempData["message"] = "Error Occured, Please Check your form carefully, that all input field has been filled up correctly";
            TempData["messageType"] = 2;
            ViewBag.Projects = _repository.GetAllProjects();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
            var pos = _repository.GetProjectOrderModels(model.ProjectMasterModel.ProjectMasterId);

            selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
            ViewBag.ProjectOrders = selectListItems;
            return View(model);
        }

        [HttpGet]
        public ActionResult Shipment(long id = 0, long projectId = 0, int type = 0, string msg = null)
        {
            var model = new VmProjectShipment();
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            ViewBag.Projects = _repository.GetAllProjectWithOrderNumber();
            ViewBag.ProjectsForFinishGood = _repository.GetAllProductModel();
            if (id > 0 && projectId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                model.ProjectOrderShipmentModel = _repository.GetProjectOrderShipment(id);
                var pos = _repository.GetProjectOrderModels(projectId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.OrderDate }));
                ViewBag.ProjectOrders = selectListItems;
            }
            else if (projectId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                var pos = _repository.GetProjectOrderModels(projectId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.OrderDate }));
                ViewBag.ProjectOrders = selectListItems;

            }
            else
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                ViewBag.ProjectOrders = selectListItems;
            }
            return View(model);
        }

        [NotificationActionFilter(MessageHeader = "Project Shipment", ReceiverRoles = "MM,PM,CM,CMBTRC,PS")]
        [HttpPost]
        public ActionResult Shipment(VmProjectShipment model, List<ProjectMasterModel> issueList1)
        {
            if (ModelState.IsValidField("ProjectOrderShipmentModel"))
            {
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);

                if (model.ProjectOrderShipmentModel.ProjectOrderShipmentId > 0)
                {
                    var res = _repository.UpdateProjectShipment(model.ProjectOrderShipmentModel, userId, issueList1);
                    //update
                    if (res > 0)
                    {
                        _notificationObject = new NotificationObject
                        {
                            ProjectId = model.ProjectMasterModel.ProjectMasterId,
                            ModelId = model.ProjectOrderShipmentModel.ProjectOrderShipmentId
                        };
                        ViewBag.ControllerVariable = _notificationObject;

                        return RedirectToAction("Shipment", new { projectId = 0, type = 3, msg = "Shipment Updated Successfully!!!" });

                        //return RedirectToAction("Shipment", new { projectId = model.ProjectMasterModel.ProjectMasterId, type = 3, msg = "Shipment Updated Successfully!!!" });
                    }
                }
                else
                {
                    //create
                    model.ProjectOrderShipmentModel.ProjectMasterId = model.ProjectMasterModel.ProjectMasterId;
                    var res = _repository.SaveProjectShipment(model.ProjectOrderShipmentModel, userId, issueList1);
                    if (res > 0)
                    {
                        _notificationObject = new NotificationObject
                        {
                            ProjectId = model.ProjectMasterModel.ProjectMasterId,
                            ModelId = 0
                        };
                        ViewBag.ControllerVariable = _notificationObject;
                        return RedirectToAction("Shipment", new { projectId = 0, type = 1, msg = "Shipment Saved Successfully!!!" });
                    }

                }

            }
            TempData["message"] = "Error Occured, Please Check your form carefully or Contact with Adminstrator";
            TempData["messageType"] = 2;
            ViewBag.Projects = _repository.GetAllProjects();

            //----ProjectsForFinishGood----//
            ViewBag.ProjectsForFinishGood = _repository.GetAllProductModel();
            //---end---//


            if (model.ProjectOrderShipmentModel.ProjectOrderShipmentId > 0 && model.ProjectMasterModel.ProjectMasterId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(model.ProjectMasterModel.ProjectMasterId);
                model.ProjectOrderShipmentModel = _repository.GetProjectOrderShipment(model.ProjectOrderShipmentModel.ProjectOrderShipmentId);
                var pos = _repository.GetProjectOrderModels(model.ProjectMasterModel.ProjectMasterId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;
            }
            else if (model.ProjectMasterModel.ProjectMasterId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(model.ProjectMasterModel.ProjectMasterId);
                var pos = _repository.GetProjectOrderModels(model.ProjectMasterModel.ProjectMasterId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;

            }
            else
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                ViewBag.ProjectOrders = selectListItems;
            }
            ViewBag.ControllerVariable = _notificationObject;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddFinishGoodModel(int IsRemoved, long ProIds, string ProTxts, int aproxValue)
        {
            var model = new ProjectMasterModel();

            model.IsRemoved = IsRemoved;
            model.ProjectMasterId = ProIds;
            model.FinishGoodModel = ProTxts;
            model.ApproxFinishGoodManufactureQty = aproxValue;
           
            return PartialView("~/Views/Commercial/CmPartials/_AddFinishGood.cshtml", model);
        }

        [HttpGet]
        public ActionResult Shipments(int type = 0, string msg = null)
        {
            if (type > 0)
            {
                TempData["messageType"] = type;
                TempData["message"] = msg;
            }
            long userId = Convert.ToInt64(User.Identity.Name);
            List<ProjectOrderShipmentModel> orderShipmentModels = _repository.GetShipmentModels(userId);
            ViewBag.ClosedShipments = _repository.GetClosedShipmentModels(userId);
            return View(orderShipmentModels);
        }

        [HttpPost]
        public ActionResult Shipments(long shipmentId)
        {
            //this method will be close a shipment after clicking a button from the list
            return View();
        }

        [HttpGet]
        public ActionResult Lcs(int type = 0, string msg = null)
        {
            if (type > 0)
            {
                TempData["messageType"] = type;
                TempData["message"] = msg;
            }
            long userId = Convert.ToInt64(User.Identity.Name);
            List<ProjectLcModel> lcModels = _repository.GetProjectLcModels();

            return View(lcModels);
        }

        [HttpPost]
        public ActionResult Lcs(long lcId)
        {
            //this method will be close a lc after clicking a button from the list
            return View();
        }

        public ActionResult LcsReportByDateRange(string fromDate, string toDate)
        {
            ViewBag.From = fromDate ?? DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.To = toDate ?? DateTime.Now.ToString("yyyy-MM-dd");
            var from = DateTime.ParseExact(fromDate ?? DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
            var to = DateTime.ParseExact(toDate ?? DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
            var v = _repository.GetProjectLcsByDateRange(from, to);
            return View(v);
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PS")]
        [HttpGet]
        public ActionResult LcClose(long id, string poNo, string projectName)
        {
            if (id > 0)
            {
                var res = _repository.CloseLc(id);
                string msgString;
                if (res != null)
                {
                    _notificationObject = new NotificationObject
                    {
                        ProjectId = res.ProjectMasterId,
                        ModelId = res.ProjectLcId,
                        AdditionalInformation = "PO No. : " + poNo,
                        MessageFromController = "Project LC has been closed"
                    };
                    ViewBag.ControllerVariable = _notificationObject;
                    msgString = "LC Successfully closed. Project Name: " + projectName + ", PO No.: " + poNo;
                    return RedirectToAction("Lcs", new { type = 1, msg = msgString });
                }
                msgString = "Something goes wrong!!! Please Contact with Administrator, Project Name, " + projectName + ", PO No.: " + poNo;
                return RedirectToAction("Lcs", new { type = 2, msg = msgString });
            }
            return RedirectToAction("Lcs", new { type = 3, msg = "Nothing happend" });
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PM,PS")]
        [HttpGet]
        public ActionResult ShipmentClose(long id = 0, string poNo = null, string projectName = null)
        {
            if (id > 0)
            {
                string msgString;
                var res = _repository.CloseShipment(id);
                if (res != null)
                {
                    _notificationObject = new NotificationObject
                    {
                        ProjectId = res.ProjectMasterId,
                        ModelId = res.ProjectOrderShipmentId,
                        AdditionalInformation = "PO No. : " + poNo,
                        MessageFromController = "Project Shipmnet has been closed"
                    };
                    ViewBag.ControllerVariable = _notificationObject;
                    msgString = "Shipment Successfully closed. Project Name: " + projectName + ", PO No.: " + poNo;
                    return RedirectToAction("Shipments", new { type = 1, msg = msgString });
                }
                msgString = "Something goes wrong!!! Please Contact with Administrator, Project Name" + projectName + ", PO No.: " + poNo;
                return RedirectToAction("Shipments", new { type = 2, msg = msgString });
            }
            return RedirectToAction("Shipments", new { type = 3, msg = "Nothing happend, Project Name" + projectName + ", PO No.: " + poNo });
        }

        public ActionResult ShipmentDelete(long id = 0, string poNo = null, string projectName = null,string shipmentNo=null)
        {
            var userId = HttpContext.User.Identity.Name;
            long uId;
            long.TryParse(userId, out uId);
            var user = _homeRepository.GetUser(uId);
            try
            {
                _repository.DeleteShipment(id);
                //===Mail===
                    var body =
                        string.Format(
                            @"This is to inform you that, "+shipmentNo+" has been deleted in Walton Project Management System By "+user.UserFullName+".<br/><br/><b>Project Name: " +
                            projectName + "<br/>" +
                            ", " + poNo + "<br/>");
                    var mail = new MailSendFromPms();
                    mail.SendMail(new List<string>(new[] { "SPR","CM" }),
                        new List<string>(new[] { "SA" }), "Shipment Deleted ( " + projectName + " )", body);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Shipments", new { type = 3, msg = ex + projectName + ", PO No.: " + poNo });
            }
            var msgString = "Shipment Successfully Deleted. Project Name: " + projectName + "," + poNo+", "+shipmentNo;
            return RedirectToAction("Shipments", new { type = 1, msg = msgString });
        }

        [HttpGet]
        public ActionResult ImeiRangeRequests()
        {
            List<ProjectBabtModel> models = _repository.GetAllBabt() ?? new List<ProjectBabtModel>();
            return View(models);
        }

        [HttpGet]
        public ActionResult Babt(string projectName)
        {
            var model = new BabtRawModel();



            //ViewBag.Projects = _customSelectListItemRepository.GetModelListForTac(id);
            //if (id > 0)
            //{
            //    model = _repository.GetBabt(id) ?? new BabtRawModel();
            //}




            ProjectMasterModel masterModel = _repository.GetProjectByName(projectName);
            model.ProjectName = projectName;
            if (masterModel != null) model.ProjectMasterId = masterModel.ProjectMasterId;


            return View(model);
        }
        [HttpPost]
        public ActionResult Babt(BabtRawModel model)
        {
            if (ModelState.IsValid)
            {
                long res = model.BabtRawId > 0 ? _repository.UpdateBabt(model) : _repository.SaveBabt(model);
                if (res > 0)
                {
                    _response.MessageType = 1;
                    _response.Message = "Save Successfully!!";
                }
                else
                {
                    _response.MessageType = 2;
                    _response.Message = "Something goes wrong !!!";
                }
            }
            ViewBag.Projects = _customSelectListItemRepository.GetModelListForTac(model.BabtRawId);
            TempData["message"] = _response.Message;
            TempData["messageType"] = _response.MessageType;
            return View(model);
        }

        [HttpGet]
        public ActionResult Babts()
        {
            List<BabtRawModel> babtRawModels = _repository.GetBabts();
            return View(babtRawModels);
        }

        [HttpGet]
        public ActionResult PendingTacList()
        {
            List<VmPendingTac> pendings = _repository.GetPendingTacList();
            return View(pendings);
        }
        [HttpGet]
        public ActionResult Btrc()
        {
            var model = new VmBtrcNoc();
            ViewBag.Projects = _repository.GetProjectsForBtrcNoc();
            //ViewBag.Tacs = _repository.GetBabts();
            return View(model);
        }
        [HttpPost]
        public ActionResult Btrc(VmBtrcNoc model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProjectBtrcNocModel.CustomBtrcProjectModels.Any())
                {
                    bool result = _repository.SaveBtrcNocs(model);
                }

            }
            return RedirectToAction("Btrc");
        }

        [HttpGet]
        public ActionResult BtrcNocRequests()
        {
            List<ProjectBtrcNocModel> models = _repository.GetBtrcNocRequestList();
            return View(models);
        }

        [HttpPost]
        public ActionResult AddProject(long projectId, string projectName, string qty, string imei)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                long quantity;
                long.TryParse(qty, out quantity);
                var btr = dbEntities.ProjectBtrcNocs.FirstOrDefault(i => i.ProjectBtrcNocId == projectId);
                if (btr != null)
                {
                    var model = new CustomBtrcProjectModel
                    {
                        ProjectId = btr.ProjectMasterId,
                        ProjectName = projectName,
                        SampleImei = imei,
                        NocTableId = btr.ProjectBtrcNocId,
                        Quantity = qty
                    };
                    return PartialView("~/Views/Commercial/CmPartials/_BtrcProjectList.cshtml", model);
                }
                return null;
            }

        }
        [HttpGet]
        public ActionResult PurchaseOrders()
        {
            List<ProjectPurchaseOrderFormModel> formModels = _repository.GetUnclosedPoList() ?? new List<ProjectPurchaseOrderFormModel>();
            return View(formModels);
        }
        [HttpGet]
        public ActionResult PurchaseOrder(long id = 0, long print = 0, int type = 0, string msg = null)
        {
            var purchaseOrder = new VmProjectPurchaseOrder();
            purchaseOrder.IsReorder = false;
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            if (id > 0 && type <= 0)
            {
                purchaseOrder.PrintRequired = print > 0 ? "y" : "n";
                purchaseOrder.ProjectPurchaseOrderFormModel = _repository.GetPurchaseOrderById(id);
                purchaseOrder.PrintFormId = purchaseOrder.ProjectPurchaseOrderFormModel.ProjectMasterId;
                purchaseOrder.ProjectPurchaseOrderConditionModels = _repository.GetPurchaseOrderConditionsByOrder(id);
                purchaseOrder.JigsAndFixtureModels =
                    _repository.GetJigsAndFixtureModelsByProjectId(
                        purchaseOrder.ProjectPurchaseOrderFormModel.ProjectMasterId);
                var projectMaster = _repository.GetProjectModel(purchaseOrder.PrintFormId);
                purchaseOrder.ApproximateShipmentDate = projectMaster.ApproxShipmentDate;
                if (projectMaster != null)
                {
                    purchaseOrder.ApproximateFinishDateForReorder = projectMaster.ApproxProjectFinishDate;
                    purchaseOrder.IsReorder = projectMaster.OrderNuber > 1;
                }
            }
            else if (id > 0 && type > 0)
            {
                //======
                purchaseOrder.ProjectPurchaseOrderFormModel = _repository.GetPurchaseOrderById(id);
                purchaseOrder.ProjectPurchaseOrderConditionModels = _repository.GetPurchaseOrderConditionsByOrder(id);
                purchaseOrder.JigsAndFixtureModels =
                    _repository.GetJigsAndFixtureModelsByProjectId(
                        purchaseOrder.ProjectPurchaseOrderFormModel.ProjectMasterId);
                var projectMaster = _repository.GetProjectModel(purchaseOrder.ProjectPurchaseOrderFormModel.ProjectMasterId);
                purchaseOrder.ApproximateShipmentDate = projectMaster.ApproxShipmentDate;
                if (projectMaster != null)
                {
                    purchaseOrder.ApproximateFinishDateForReorder = projectMaster.ApproxProjectFinishDate;
                    purchaseOrder.IsReorder = projectMaster.OrderNuber > 1;
                }
                //------
                purchaseOrder.PrintRequired = print > 0 ? "y" : "n";
                purchaseOrder.PrintFormId = _repository.GetPurchaseOrderById(id).ProjectPurchaseOrderFormId;
                purchaseOrder.ProjectPurchaseOrderFormModel.DescriptionHeader = "Dear Sir, Madam,";
                //purchaseOrder.ProjectPurchaseOrderFormModel.DescriptionBody =
                //"With reference to our subsequent discussion with you, we are pleased to place a fresh Purchase Order for supplying of mobile phone handsets as per terms and conditions mentioned below:";
                //purchaseOrder.ProjectPurchaseOrderConditionModels = _repository.GetPredefinedPurhcaseOrderConditions();
            }
            else
            {
                purchaseOrder.ProjectPurchaseOrderFormModel.DescriptionHeader = "Dear Sir, Madam,";
                purchaseOrder.ProjectPurchaseOrderFormModel.DescriptionBody =
                "With reference to our subsequent discussion with you, we are pleased to place a fresh Purchase Order for supplying of mobile phone handsets as per terms and conditions mentioned below:";
                purchaseOrder.ProjectPurchaseOrderConditionModels = _repository.GetPredefinedPurhcaseOrderConditions();

            }
           
            List<ProjectMasterModel> projectList = _repository.GetProjectsForPurchaseOrder();
            var listItems = new List<SelectListItem>
            {
                new SelectListItem {Value = "", Text = "--Select Project--"}
            };
            foreach (var masterModel in projectList)
            {
                if (listItems.FindIndex(i => i.Value == masterModel.ProjectName) < 0)
                {
                    var item = new SelectListItem { Value = masterModel.ProjectName, Text = masterModel.ProjectName };
                    listItems.Add(item);
                }

            }
            //listItems = listItems.Distinct().ToList();
            ViewBag.Projects = listItems;
            return View(purchaseOrder);
        }
        public JsonResult GetPrevPurchaseOrder(string projectName)
        {
            var projectMasterModel =
                _repository.GetAllCreatedProjects()
                    .Where(i => i.ProjectActualName == projectName)
                    .OrderByDescending(i => i.OrderNuber).FirstOrDefault();
            if (projectMasterModel != null)
            {
                var purchaseOrder = _repository.GetPurchaseOrders(projectMasterModel.ProjectMasterId).FirstOrDefault();
                return new JsonResult { Data = purchaseOrder, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            
            return new JsonResult { Data = new ProjectPurchaseOrderFormModel(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [NotificationActionFilter(ReceiverRoles = "CM,MM,PS,PM,PMHEAD,QCHEAD,HWHEAD", MessageHeader = "Purchase Order")]
        [HttpPost]
        public ActionResult PurchaseOrder(VmProjectPurchaseOrder model)
        {
            List<ProjectMasterModel> projectList = _repository.GetProjectsForPurchaseOrder();
            var listItems = new List<SelectListItem>
            {
                new SelectListItem {Value = "", Text = "--Select Project--"}
            };
            foreach (var masterModel in projectList)
            {
                if (listItems.FindIndex(i => i.Value == masterModel.ProjectName) < 0)
                {
                    var item = new SelectListItem { Value = masterModel.ProjectName, Text = masterModel.ProjectName };
                    listItems.Add(item);
                }

            }
            //listItems = listItems.Distinct().ToList();
            ViewBag.Projects = listItems;
            long formId = model.ProjectPurchaseOrderFormModel.ProjectPurchaseOrderFormId;
            _notificationObject = new NotificationObject
            {
                ModelId = formId,
                AdditionalInformation = "PO No. : " + model.ProjectPurchaseOrderFormModel.PurchaseOrderNumber
            };
            long userId;
            var id = HttpContext.User.Identity.Name;
            Int64.TryParse(id, out userId);
            if (ModelState.IsValid)
            {
                //update starts
                if (model.ProjectPurchaseOrderFormModel.ProjectPurchaseOrderFormId > 0)
                {
                    model.ProjectPurchaseOrderFormModel.ApproxShipmentDate = model.ApproximateShipmentDate;
                    var oldPoInfo =
                        _repository.GetPurchaseOrderByIdAsNoTracking(model.ProjectPurchaseOrderFormModel.ProjectPurchaseOrderFormId);
                    _repository.SaveProjectPurchaseOrderFormLog(oldPoInfo,userId);
                    model.ProjectPurchaseOrderFormModel.Updated = userId;
                    model.ProjectPurchaseOrderFormModel.UpdatedDate = DateTime.Now;
                    bool isUpdated = _repository.UpdateProjectPurchaseOrderFormModel(model.ProjectPurchaseOrderFormModel, model.ApproximateFinishDateForReorder);
                    if (isUpdated)
                    {
                        //====sms notificaion if PO quantity updated===
                        if (oldPoInfo.Quantity != model.ProjectPurchaseOrderFormModel.Quantity)
                        {
                            var project = _commonRepository.GetProjectInfoByProjectId(model.ProjectPurchaseOrderFormModel.ProjectMasterId);
                            var message = "PO quantity of " + project.ProjectName + " (Order " + project.OrderNuber + "), updated to " + project.OrderQuantity+". Previous order quantity was "+oldPoInfo.Quantity+".";
                            SendSmsViaRobi("01686690592", message);
                            //===Email notification===
                            var messageForPmTeam = "";
                            if (project.ProjectType == "smart")
                            {
                                messageForPmTeam =
                                    "<br/><br/><b>N.B.</b> PM team please update variant quantity according to the updated Order Quantity.<br/>";
                            }
                            MailSendFromPms mailSendFromPms = new MailSendFromPms();
                            var addedByDetails = _hardwareRepository.GetUserInfoByUserId(userId);
                            mailSendFromPms.SendMail(new List<string>(new[] { "PM", "PMHEAD", "CMHEAD","CM" }), new List<string>(new[] { "PS" }), "Order Quantity has been updated", message + "<br/>Updated by "+addedByDetails.UserFullName+"."+messageForPmTeam);
                        }
                        //---O----
                        //Update Jigs & Fixtures
                        foreach (var j in model.JigsAndFixtureModels)
                        {
                            j.ProjectMasterId = model.ProjectPurchaseOrderFormModel.ProjectMasterId;
                            j.AddedBy = userId;
                            j.AddedDate = DateTime.Now;
                        }
                        _repository.SaveOrUpdateJigsAndFixtures(model.JigsAndFixtureModels);
                        //Update PO conditions
                        //first keep log
                        _repository.SaveProjectPurchaseOrderConditionLogs(model.ProjectPurchaseOrderFormModel.ProjectPurchaseOrderFormId,userId);
                        //----O----
                        foreach (var m in model.ProjectPurchaseOrderConditionModels)
                        {
                            m.Added = userId;
                            m.AddedDate = DateTime.Now;
                            m.ProjectPurchaseOrderFormId =
                                model.ProjectPurchaseOrderFormModel.ProjectPurchaseOrderFormId;
                        }
                        isUpdated =
                            _repository.UpdateProjectPurchaseOrderConditionModel(
                                model.ProjectPurchaseOrderFormModel.ProjectPurchaseOrderFormId,
                                model.ProjectPurchaseOrderConditionModels);
                        if (isUpdated)
                        {
                            return RedirectToAction("PurchaseOrder",
                                new { id = formId, print = 1, type = 3, msg = "PO Updated Successfully !!!" });
                        }
                    }

                }
                else
                {
                    model.ProjectPurchaseOrderFormModel.IsCompleted = false;
                    model.ProjectPurchaseOrderFormModel.Added = userId;
                    model.ProjectPurchaseOrderFormModel.AddedDate = DateTime.Now;
                    //Save PO
                    formId = _repository.SaveProjectPurchaseOrderFormModel(model);
                    //----
                    if (formId == 0)
                    {
                        TempData["message"] =
                            "Exception Occured, PLEASE CONTACT WITH THE SOFTWARE TEAM!!!";
                        TempData["messageType"] = 2;

                        return View(model);
                    }
                    if (formId == -1)
                    {

                        TempData["message"] =
                            "Error Occured, An order has been exist for this project, please check the Is Reorder checkbox for create a RE-ORDER !!!";
                        TempData["messageType"] = 2;

                        return View(model);
                    }
                    if (formId == -2)
                    {
                        TempData["message"] =
                            "Error Occured, This purchase order can not save as a RE-ORDER because there are no previous purchase order found for this project, you have to uncheck the Re-Order checkbox and try again !!!";
                        TempData["messageType"] = 2;

                        return View(model);
                    }
                    if (formId == -3)
                    {
                        TempData["message"] = "This is a reorder, you have to give an approximate finish date";
                        TempData["messageType"] = 2;
                        return View(model);
                    }
                    if (formId == -4)
                    {
                        TempData["message"] = "Error occured due to network problem !!!";
                        TempData["messageType"] = 2;
                        return View(model);
                    }
                    //Save Project PO Handset 
                    var x = _repository.SaveProjectPurchaseOrderHandsetModel(model.ProjectPurchaseOrderHandsetModels);
                    //-----
                    foreach (var conditionModel in model.ProjectPurchaseOrderConditionModels)
                    {
                        conditionModel.ProjectPurchaseOrderFormId = formId;
                        conditionModel.Added = userId;
                        conditionModel.AddedDate = DateTime.Now;
                    }
                    //Save PO conditions
                    _repository.SaveProjectPurchaseOrderConditionModel(model.ProjectPurchaseOrderConditionModels);
                    //-------
                    
                    if (formId > 0)
                    {
                        long proId = model.ProjectPurchaseOrderFormModel.ProjectMasterId <= 0
                            ? _repository.GetProjectId(model.ProjectPurchaseOrderFormModel.ProjectName)
                            : model.ProjectPurchaseOrderFormModel.ProjectMasterId;
                        model.ProjectPurchaseOrderFormModel.ProjectMasterId = proId;
                        _notificationObject.ProjectId = proId;
                        ViewBag.ControllerVariable = _notificationObject;
                        //Save Jigs & Fixtures
                        foreach (var j in model.JigsAndFixtureModels)
                        {
                            j.ProjectMasterId = proId;
                            j.AddedBy = userId;
                            j.AddedDate = DateTime.Now;
                        }
                        var project = _commonRepository.GetProjectInfoByProjectId(proId);
                        var message = "PO created for " + project.ProjectName + " (Order " + project.OrderNuber + "), order quantity "+project.OrderQuantity;
                        SendSmsViaRobi("01686690592", message);
                        _repository.SaveOrUpdateJigsAndFixtures(model.JigsAndFixtureModels);
                        //----
                    }

                    return RedirectToAction("PurchaseOrder",
                        new { id = formId, print = 1, type = 1, msg = "PO Saved Successfully!!!" });
                }

            }

            TempData["message"] = "Error Occured, Please Check your form carefully or Contact with Adminstrator";
            TempData["messageType"] = 2;

            return View(model);
        }

        [HttpPost]
        public ActionResult AddModel(string model, int? quantity, string color, string value, int? serial)
        {
            var handsetModel = new ProjectPurchaseOrderHandsetModel
            {
                SerialNo = serial,
                Model = model,
                OrderQuantity = quantity,
                Color = color,
                Value = value
            };
            return PartialView("~/Views/Commercial/CmPartials/_PurchaseOrderItemList.cshtml", handsetModel);
        }

        [HttpPost]
        public ActionResult AddPurchaseOrderCondition(string condition)
        {
            var conditionModel = new ProjectPurchaseOrderConditionModel
            {
                //SerialNo = serial,
                Statement = condition
            };
            return PartialView("~/Views/Commercial/CmPartials/_PurchaseOrderCondition.cshtml", conditionModel);
        }

        [HttpGet]
        public ActionResult NocDocuments(long nocId)
        {
            var files = _projectManagerRepository.GetFilesServerPaths(nocId);
            return View(files);
        }

        [HttpGet]
        public ActionResult Suppliers()
        {
            List<SupplierModel> suppliers = _repository.GeTAllSuppliers();
            return View(suppliers);
        }

        [HttpGet]
        public ActionResult CreateSupplier(long id = 0)
        {
            SupplierModel model = _repository.GetSupplier(id) ?? new SupplierModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSupplier(SupplierModel model)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            if (ModelState.IsValid)
            {
                if (model.SupplierId > 0)
                {
                    bool isUpdated = _repository.UpdateSupplier(model, userId);
                    if (isUpdated)
                    {
                        TempData["message"] = "Supplier updated successfully";
                        TempData["messageType"] = 3;
                        return RedirectToAction("CreateSupplier", new { id = model.SupplierId });
                    }
                }
                else
                {
                    model.Added = userId;
                    model.AddedDate = DateTime.Now;
                    bool isSaved = _repository.SaveSupplier(model);
                    if (isSaved)
                    {
                        TempData["message"] = "Supplier saved successfully";
                        TempData["messageType"] = 1;
                        return RedirectToAction("CreateSupplier", new { id = model.SupplierId });
                    }

                }

            }
            TempData["message"] = "Error occured!! Please contact with administrator";
            TempData["messageType"] = 2;
            return View(model);
        }

        [HttpPost]
        public long? GetAvailableImei(string tac)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                var babtRaw = dbEntities.BabtRaws.FirstOrDefault(i => i.TacNo == tac);
                if (babtRaw != null)
                {
                    long? remaining = babtRaw.RemainingImei;
                    return remaining;
                }
            }
            return 0;
        }

        [HttpGet]
        public ActionResult CompletedNocList()
        {
            List<VmCompletedNoc> completedNocs = _repository.GetCompletedNocs();
            return View(completedNocs);
        }

        [HttpGet]
        public ActionResult SetImeiRange(long projectId, long orderId, long qty)
        {
            VmImeiRange imeiRange = _repository.GetCustomImeiRange(projectId, orderId, qty);
            return View(imeiRange);
        }

        [HttpPost]
        public ActionResult SetImeiRange(VmImeiRange model)
        {
            if (ModelState.IsValid)
            {
                bool result = _repository.SaveImeiRange(model);
                if (result)
                {
                    _response.MessageType = 1;
                    long userId;
                    long.TryParse(HttpContext.User.Identity.Name, out userId);
                    var user = _homeRepository.GetUser(userId);
                    var usrInfo = user != null ? "<br/>Imei Range set By: " + user.UserFullName : "<br/>Forwarded By: Unknown";
                    string projectName = _repository.GetProjectMasterModel(model.ProjectMasterId).ProjectName;
                    string time = "<br/>Imei Range set On: " + DateTime.Now.ToLongDateString();
                    var body =
                        string.Format(
                            @"This is to inform you that, A project's IMEI range has been generated and the range is-<br/>"+model.SampleStartImei+" to "+model.SampleEndImei +"<br/> This is an automated email from WPMS.<br/><br/><b>Project Name: " +
                            projectName + "</b>" + usrInfo + time);


                    var mail = new MailSendFromPms();
                    //mail.SendMailWithSpecificEmailList(new List<string>(new[] { "sajib16083@waltonbd.com" }),
                    //    new List<string>(new[] { "mobile_pm@waltonbd.com", "fahim@waltonbd.com", "ov@waltonbd.com",  }), "IMEI Range for( " + projectName + " )", body);





                    mail.SendMailWithSpecificEmailList(new List<string>(new[] { "mrahman.cse32@waltonbd.com" }),
                        new List<string>(), "IMEI Range for( " + projectName + " )", body);

                    _response.Message = "Save Successfully!!";
                }
                else
                {
                    _response.MessageType = 2;
                    _response.Message = "Something goes wrong !!!";
                }
            }
            TempData["message"] = _response.Message;
            TempData["messageType"] = _response.MessageType;
            return View(model);
        }


        public ActionResult ImeiDataBase()
        {
            VmImeiDataBase model = new VmImeiDataBase();
            model.StartDate = DateTime.Today;
            model.EndDate = DateTime.Today;
            return View(model);
        }

        [HttpPost]
        public ActionResult ImeiDataBase(VmImeiDataBase model)
        {
            model = _repository.GetProjectBabtList(model);
            model.TacList = model.ProjcetBabts.Select(i => i.TacNo).ToList();
            model.TacList = model.TacList.Distinct().Select(i => i).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult GiventImeiRange()
        {
            return null;
        }

        [HttpPost]
        public JsonResult GetRange(string qty, string alloc, string noc)
        {
            long allocFromQty, givenQty;
            long.TryParse(alloc, out allocFromQty);
            long.TryParse(qty, out givenQty);
            var startFrom = noc + alloc.PadLeft(6, '0') + " X";
            var end = noc + ((allocFromQty + givenQty)-1).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0') + " X";
            var range = new MakeRange { StartImeiRange = startFrom, EndImeiRange = end };
            return new JsonResult { Data = range };
        }
        [NotificationActionFilter(ReceiverRoles = "CM,MM,PS,PM,QCHEAD,HWHEAD", MessageHeader = "Screen Test Request")]
        [HttpPost]
        public JsonResult RequestScreening(long projectId, long quantity, string sampleType, string remarks)
        {
            long requestResult = _repository.ScreeningRequest(projectId, quantity, sampleType, remarks);
            if (requestResult > 0)
            {
                _notificationObject = new NotificationObject
                {
                    ModelId = 0,
                    ProjectId = projectId,
                    AdditionalInformation = string.Format(quantity + " samples has been sent and sample type is '" + sampleType + "'.")
                };
                ViewBag.ControllerVariable = _notificationObject;
                long userId;
                long.TryParse(HttpContext.User.Identity.Name, out userId);
                var user = _homeRepository.GetUser(userId);
                var usrInfo = user != null ? "<br/>Forwarded By: " + user.UserFullName : "<br/>Forwarded By: Unknown";
                string projectName = _repository.GetProjectMasterModel(projectId).ProjectName;
                string time = "<br/>Forwarded On: " + DateTime.Now.ToLongDateString();
                var body =
                    string.Format(
                        @"This is to inform you that, A project has been forwarded for screening test to hardware section in Walton Project Management System By Commercial section.<br/><br/><b>Project Name: " +
                        projectName + "</b>" + usrInfo + time);


                var mail = new MailSendFromPms();
                var result = mail.SendMail(new List<string>(new[] { "HWHEAD", "HW" }),
                    new List<string>(new[] { "MM", "PMHEAD", "QCHEAD", "PS" }), "Request for Screening Test( " + projectName + " )", body);
            }
            return new JsonResult { Data = requestResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckProjectName(String projectName)
        {
            string result = _repository.CheckProjectName(projectName);
            if (result == string.Empty) return Json(true, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ScreeningIssues(long id = 0)
        {
            var issues = new VmScreeningIssues();
            if (id > 0)
            {
                issues.ProjectMasterId = id;
                issues.HwInchargeIssueModels = _repository.GetScreeningIssues(id);
                return PartialView(issues);
            }

            return PartialView(new VmScreeningIssues { ProjectMasterId = id });
        }

        [HttpPost]
        public JsonResult ScreeningIssues(VmScreeningIssues model)
        {
            if (model != null && model.ProjectMasterId > 0)
            {
                if (model.HwInchargeIssueModels.Any())
                {
                    if (model.HwInchargeIssueModels.All(i => i.CommercialDecision == null))
                    {
                        return new JsonResult { Data = "e", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    if (model.HwInchargeIssueModels.Any(i => i.CommercialDecision != null && i.CommercialDecision != "Solvable" && string.IsNullOrWhiteSpace(i.Remarks)))
                    {
                        return new JsonResult { Data = "r", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                int result = _repository.SaveScreeningIssues(model);
                if (result == 0)
                {
                    return new JsonResult { Data = "err", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                long userId;
                long.TryParse(HttpContext.User.Identity.Name, out userId);
                _repository.ScreeningIssueNotification(model.ProjectMasterId, userId);
                return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = "err", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ClosePurchaseOrder(String ProjectMasterId, String ProjectPurchaseOrderFormId, String MarketClearanceDate, String BdIqcResult)
        {
            DateTime marketCd = Convert.ToDateTime(MarketClearanceDate);
            //var a = marketCd;
            long proIds;
            long.TryParse(ProjectMasterId, out proIds);

            long proOrdersId;
            long.TryParse(ProjectPurchaseOrderFormId, out proOrdersId);

            SelectListItem result = _repository.ClosePurchaseOrder(marketCd, proOrdersId, proIds, BdIqcResult);
            return new JsonResult { Data = result };

            //return new JsonResult();
        }

        [HttpGet]
        public ActionResult BtrcList()
        {
            List<BtrcRawListModel> btrcRawModels = _repository.GetBtrcRowModels();
            return View(btrcRawModels);
        }

        [HttpGet]
        public ActionResult ProjectBulkUpdate()
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            if (userId > 0)
            {
                
                List<ProjectMasterModel> projectList = _repository.GetProjectsForPurchaseOrder();
                var listItems = new List<SelectListItem>
            {
                new SelectListItem {Value = "", Text = "--Select Project--"}
            };
                foreach (var masterModel in projectList)
                {
                    if (listItems.FindIndex(i => i.Value == masterModel.ProjectName) < 0)
                    {
                        var item = new SelectListItem { Value = masterModel.ProjectName, Text = masterModel.ProjectName };
                        listItems.Add(item);
                    }

                }
                //listItems = listItems.Distinct().ToList();
                ViewBag.Projects = listItems;

                var model = new BulkUpdateModel();
                return View(model);
            }
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult ProjectBulkUpdate(BulkUpdateModel model)
        {
            bool isUpdate = _repository.UpdateBulkProject(model);
            if (isUpdate)
            {

                TempData["message"] =
                    "Updated Successfully";
                TempData["messageType"] = 1;
                return RedirectToAction("ProjectBulkUpdate");
            }
            TempData["message"] =
                "Error Ocurred !!!";
            TempData["messageType"] = 2;
            List<ProjectMasterModel> projectList = _repository.GetProjectsForPurchaseOrder();
            var listItems = new List<SelectListItem>
            {
                new SelectListItem {Value = "", Text = "--Select Project--"}
            };
            foreach (var masterModel in projectList)
            {
                if (listItems.FindIndex(i => i.Value == masterModel.ProjectName) < 0)
                {
                    var item = new SelectListItem { Value = masterModel.ProjectName, Text = masterModel.ProjectName };
                    listItems.Add(item);
                }

            }
            //listItems = listItems.Distinct().ToList();
            ViewBag.Projects = listItems;
            return View();
        }
        public JsonResult GetPurcahseOrders(string projectName)
        {
            List<SelectListItem> models = _projectMasterRepository.Find(i => i.ProjectName == projectName && i.IsActive).Select(i => new SelectListItem { Value = i.ProjectMasterId.ToString(CultureInfo.InvariantCulture), Text = CommonConversion.AddOrdinal(i.OrderNuber) }).ToList();
            return new JsonResult {Data = models, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        #region Warehouse

        [HttpGet]
        public ActionResult Warehouse(string proId)
        {
            VmWarehouseDetails vmWarehouse = new VmWarehouseDetails();
            ViewBag.Projects = _projectManagerRepository.GetAllProjectsForOthers();
            long proIds;
            long.TryParse(proId, out proIds);
            List<SelectListItem> items = new List<SelectListItem>();
            if (proIds != 0)
            {
                vmWarehouse.PurchaseOrderFormModels = _repository.GetPurchaseOrders(proIds);
                items = vmWarehouse.PurchaseOrderFormModels.Select(model => new SelectListItem { Text = model.PurchaseOrderNumber, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            }

            ViewBag.GetPurchaseOrder = items;


            List<SelectListItem> items1 = new List<SelectListItem>();
            // List<SelectListItem> items2 = new List<SelectListItem>();
            if (proIds != 0)
            {
                vmWarehouse.ProjectOrderShipmentModels = _repository.GetShipmentModels(proIds);
                items1 =
                    vmWarehouse.ProjectOrderShipmentModels.Select(
                        model =>
                            new SelectListItem
                            {
                                //DateTime date = DateTime.ParseExact(strDate, "dd/MM/YYYY", CultureInfo.InvariantCulture)
                                Text = @String.Format("{0:yyyy-MM-dd}", model.ChainaInspectionDate),
                                Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                            }).ToList();

                // var shipQty = _repository.GetShipmentQuantity(proIds,null,null);
            }
            ViewBag.GetShipment = items1;
            //  ViewBag.GetQuantity = items2;

            return View(vmWarehouse);
        }
        public JsonResult GetPurchaseOrderResult(string proId)
        {
            long proIds;
            long.TryParse(proId, out proIds);

            var purchaseOrderList = _repository.GetPurchaseOrders(proIds);
            List<SelectListItem> items =
                purchaseOrderList.Select(
                    model =>
                        new SelectListItem
                        {
                            Text = model.PurchaseOrderNumber,
                            Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                        }).ToList();

            var json = JsonConvert.SerializeObject(items);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        //GetShipmentResult
        public JsonResult GetShipmentResult(string proId, string purchaseOrderNo)
        {
            long proIds;
            long.TryParse(proId, out proIds);

            var purchaseOrderList = _repository.GetShipments(proIds, purchaseOrderNo);
            List<SelectListItem> items =
                purchaseOrderList.Select(
                    model =>
                        new SelectListItem
                        {
                            //   @String.Format("{0:yyyy-MM-dd}", @item.ProjectManagerAssignToQcInTime)
                            Text = @String.Format("{0:yyyy-MM-dd}", model.ChainaInspectionDate),
                            Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                        }).ToList();

            var json = JsonConvert.SerializeObject(items);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult GetShipmentQuantityResult(string proId, string purchaseOrderNo, string shipmentDate)
        {
            long proIds;
            long.TryParse(proId, out proIds);

            var purchaseOrderList = _repository.GetShipmentQuantity(proIds, purchaseOrderNo, shipmentDate);
            ViewBag.GetQuantity = purchaseOrderList;

            var json = JsonConvert.SerializeObject(ViewBag.GetQuantity);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetShipmentTotalQuantity(long ProjectMasterId, string PurchaseOrderNumber, DateTime ShipmentDate, long ShipmentQty)
        {
            var purchaseOrderList = _repository.GetShipmentTotalQuantity(ProjectMasterId, PurchaseOrderNumber, ShipmentDate, ShipmentQty);
            ViewBag.GetWarehouseQuantity = purchaseOrderList;

            var json = JsonConvert.SerializeObject(ViewBag.GetWarehouseQuantity);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public JsonResult SaveWarehouseDetail(string objArr)
        {
            List<Custom_Warehouse_Details> results = JsonConvert.DeserializeObject<List<Custom_Warehouse_Details>>(objArr);
            Console.Write("result :" + results);

            bool isExist = false;
            bool isExist1 = false;
            if (results.Count != 0)
            {
                isExist = _repository.GetWarehouseDetail(results[0].OrderNumber, results[0].ProjectMasterId, results[0].ProjectName, results[0].ProjectOrderShipmentId,
                    results[0].ProjectPurchaseOrderFormId, results[0].PurchaseOrderNumber, results[0].Quantity, results[0].ShipmentDate, results[0].WarehouseDate, results[0].WarehouseQuantity);
            }
            if (isExist)
            {
                TempData["Message1"] = "Already saved this record";
                return Json("Warehouse", "Commercial");
            }

            //if (results.Count != 0)
            //{
            //    isExist1 = _repository.GetShipmentTotalQuantity(results[0].ProjectMasterId,results[0].PurchaseOrderNumber,results[0].ShipmentDate,results[0].Quantity);
            //}
            //if (isExist1)
            //{
            //    TempData["Message2"] = "Warehouse Quantity crossed the Shipment Quantity";
            //    return Json("Warehouse", "Commercial");
            //}
            var SaveDetails = "0";

            if (results.Count != 0)
            {
                SaveDetails = _repository.SaveWarehouseDetails(results);
            }


            return Json(new { SaveDetails }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Event Calender

        [HttpGet]
        public ActionResult EventCalendar()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetCalendarEvents()
        {
            var dbEntities = new CellPhoneProjectEntities();
            // var events1 = dbEntities.ProjectMasters.Select(x => new { Title = x.ProjectName, Description = x.ProjectType, Start = DateTime.Now,End="2018-07-17" }).Take(10);

            var events1 = (from pm in dbEntities.ProjectMasters
                           join ppo in dbEntities.ProjectPurchaseOrderForms on pm.ProjectMasterId equals ppo.ProjectMasterId
                           join poss in dbEntities.ProjectOrderShipments on pm.ProjectMasterId equals poss.ProjectMasterId
                           where pm.ProjectMasterId == ppo.ProjectMasterId

                           select new CmEventCalendar
                           {
                               ProjectMasterId = pm.ProjectMasterId,
                               ProjectName = pm.ProjectName,
                               ProjectType = pm.ProjectType,
                               ApproxShipmentDate = pm.ApproxShipmentDate,
                               OrderNuber = pm.OrderNuber,
                               SupplierName = pm.SupplierName,
                               Quantity = ppo.Quantity,
                               PoCategory = ppo.PoCategory,
                               ShipmentType = poss.ShipmentType,
                               ChainaInspectionDate = poss.ChainaInspectionDate,
                               WarehouseEntryDate = poss.WarehouseEntryDate

                           }).Distinct().ToList();

            var eventCalenderList = new List<CmEventCalendar>();

            foreach (var cmEventCalender1 in events1)
            {
                var eventCalender = new CmEventCalendar();

                if (cmEventCalender1.ApproxShipmentDate != null)
                {
                    eventCalender.Title1 = cmEventCalender1.ProjectName + ',' + " LSD: ";
                    eventCalender.Title2 = cmEventCalender1.ApproxShipmentDate;
                    eventCalender.Start = cmEventCalender1.ApproxShipmentDate;
                    eventCalender.ProjectMasterId = cmEventCalender1.ProjectMasterId;
                    eventCalender.ProjectName = cmEventCalender1.ProjectName;
                    eventCalender.ProjectType = cmEventCalender1.ProjectType;
                    eventCalender.OrderNuber = cmEventCalender1.OrderNuber;
                    eventCalender.SupplierName = cmEventCalender1.SupplierName;
                    eventCalender.Quantity = cmEventCalender1.Quantity;
                    eventCalender.PoCategory = cmEventCalender1.PoCategory;
                    eventCalender.ShipmentType = cmEventCalender1.ShipmentType;
                    eventCalender.ApproxShipmentDate = cmEventCalender1.ApproxShipmentDate;
                    eventCalender.ChainaInspectionDate = cmEventCalender1.ChainaInspectionDate;
                    eventCalender.WarehouseEntryDate = cmEventCalender1.WarehouseEntryDate;
                    eventCalender.CmColor = "#3A87AD";
                }
                eventCalenderList.Add(eventCalender);
            }

            foreach (var cmEventCalender2 in events1)
            {
                var eventCalender = new CmEventCalendar();

                if (cmEventCalender2.ChainaInspectionDate != null)
                {
                    eventCalender.Title1 = cmEventCalender2.ProjectName + ',' + " Inspection Schedule: ";
                    eventCalender.Title2 = cmEventCalender2.ChainaInspectionDate;
                    eventCalender.Start = cmEventCalender2.ChainaInspectionDate;
                    eventCalender.ProjectMasterId = cmEventCalender2.ProjectMasterId;
                    eventCalender.ProjectName = cmEventCalender2.ProjectName;
                    eventCalender.ProjectType = cmEventCalender2.ProjectType;
                    eventCalender.OrderNuber = cmEventCalender2.OrderNuber;
                    eventCalender.SupplierName = cmEventCalender2.SupplierName;
                    eventCalender.Quantity = cmEventCalender2.Quantity;
                    eventCalender.PoCategory = cmEventCalender2.PoCategory;
                    eventCalender.ShipmentType = cmEventCalender2.ShipmentType;
                    eventCalender.ApproxShipmentDate = cmEventCalender2.ApproxShipmentDate;
                    eventCalender.ChainaInspectionDate = cmEventCalender2.ChainaInspectionDate;
                    eventCalender.WarehouseEntryDate = cmEventCalender2.WarehouseEntryDate;
                    eventCalender.CmColor = "green";
                }
                eventCalenderList.Add(eventCalender);
            }
            foreach (var cmEventCalender3 in events1)
            {
                var eventCalender = new CmEventCalendar();

                if (cmEventCalender3.WarehouseEntryDate != null)
                {
                    eventCalender.Title1 = cmEventCalender3.ProjectName + ',' + " Warehouse  ReceiveDate: ";
                    eventCalender.Title2 = cmEventCalender3.WarehouseEntryDate;
                    eventCalender.Start = cmEventCalender3.WarehouseEntryDate;
                    eventCalender.ProjectMasterId = cmEventCalender3.ProjectMasterId;
                    eventCalender.ProjectName = cmEventCalender3.ProjectName;
                    eventCalender.ProjectType = cmEventCalender3.ProjectType;
                    eventCalender.OrderNuber = cmEventCalender3.OrderNuber;
                    eventCalender.SupplierName = cmEventCalender3.SupplierName;
                    eventCalender.Quantity = cmEventCalender3.Quantity;
                    eventCalender.PoCategory = cmEventCalender3.PoCategory;
                    eventCalender.ShipmentType = cmEventCalender3.ShipmentType;
                    eventCalender.ApproxShipmentDate = cmEventCalender3.ApproxShipmentDate;
                    eventCalender.ChainaInspectionDate = cmEventCalender3.ChainaInspectionDate;
                    eventCalender.WarehouseEntryDate = cmEventCalender3.WarehouseEntryDate;
                    eventCalender.CmColor = "blue";
                }
                eventCalenderList.Add(eventCalender);
            }
            return new JsonResult { Data = eventCalenderList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion

        #region VENDOR AUTOCOMPLETE

        public JsonResult GetVendors(string vendor,string type)
        {
            var data = _repository.GetVendorList(vendor,type);
            return Json(data);
        }
        #endregion

        public ActionResult EventCal()
        {
            return View();
        }

        //public ActionResult Backend()
        //{
        //    return new Dpc().CallBack(this);
        //}

        //public ActionResult ProjectEvents()
        //{
        //    VmWarehouseEntry events = new VmWarehouseEntry();
        //    events = _repository.GetCommercialWarehouseEvent();
        //    return View(events);
        //}

        public ActionResult ProjectEvents(string searchString, string fromDate, string toDate)
        {
            VmWarehouseEntry events = new VmWarehouseEntry();
  
            DateTime FromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MaxValue;
 
            DateTime.TryParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
            var toDateConverted = DateTime.TryParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);
            if (!toDateConverted)
                ToDate = DateTime.MaxValue;
            else
                ToDate = ToDate.AddMonths(1).AddDays(-1);

            ViewBag.SearchString = searchString;
            ViewBag.FromDateText = fromDate;
            ViewBag.ToDateText = toDate;
            events = _repository.GetCommercialWarehouseEventList(FromDate, ToDate, searchString);
            return View(events);
        }

        #region AftersalesPm Foc Entry
        public ActionResult ApproveFocForAftersalesPm()
        {
            var vmAftersales = new VmAftersalesPmFoc();
            vmAftersales.CreateFocForAftersalesPmModels = _repository.GetAftersalesPmFoc();
            return View(vmAftersales);
        }
        [HttpPost]
        public JsonResult UpdateFocForAftersalesPm(VmAftersalesPmFoc focUpdate)
        {

            string focUpdate1 = _repository.UpdateFocForAftersalesPm(focUpdate);
           
            return Json(new { data = focUpdate1 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Incentive Upto August 2019
        //[Authorize(Users = "25")]
        public ActionResult MonthlyIncentive(List<VmIncentivePolicy> vmIncentivePolicies, string monId, string yearId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            long monIds;
            long.TryParse(monId, out monIds);

            long yearIds;
            long.TryParse(yearId, out yearIds);

            var isExist = _repository.GetCheckDate(monId, yearId);

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return RedirectToAction("MonthlyIncentive", "Commercial");
            }

            ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
            ViewBag.GetOrders = _repository.GetIncentiveOrders(monIds, yearIds);
            //ViewBag.GetLcs = _repository.GetIncentiveLcs(monIds, yearIds);
            //ViewBag.GetSeaShipmentFulls = _repository.GetIncentiveSeaShipmentFulls(monIds, yearIds);
            //ViewBag.GetSeaShipmentPartials = _repository.GetIncentiveSeaShipmentPartials(monIds, yearIds);
            //ViewBag.GetAirShipmentFulls = _repository.GetIncentiveAirShipmentFulls(monIds, yearIds);
            //ViewBag.GetAirShipmentPartials = _repository.GetIncentiveAirShipmentPartials(monIds, yearIds);


            if (monId != null && yearIds != null)
            {
                ViewBag.GetPrimarySaleForIncentive = _repository.GetPrimarySales(monIds, yearIds);
                ViewBag.GetRewardAndPenalties = _repository.GetRewardAndPenalties(monIds, yearIds);
                ViewBag.GetChinaIqcIncentive = _repository.GetChinaIqcIncentive(monIds, yearIds);
                ViewBag.GetOthersIncentive = _repository.GetOthersIncentive(monIds, yearIds);
            }
            ViewBag.GetServiceForFeature = _repository.GetFeaturePhoneService(monIds, yearIds);
            ViewBag.GetSalesForFeature = _repository.GetFeaturePhoneSales(monIds, yearIds);
            ViewBag.GetServiceForSmart = _repository.GetSmartPhoneService(monIds, yearIds);
            ViewBag.GetSalesForSmart = _repository.GetSmartPhoneSales(monIds, yearIds);
            ViewBag.GetUserInfo = _repository.GetCmUserList(monIds, yearIds);
            ViewBag.GetUserInfo1 = _repository.GetCmUserList2(monIds, yearIds);
            ViewBag.GetUserInfoSpare = _repository.GetUserInfoSpare(monIds, yearIds);
            ViewBag.Selected = monIds.ToString();
            ViewBag.SelectedYear = yearId;

            return View(vmIncentivePolicies);

        }

        public ActionResult CmPenaltiesAndReward(string MonNum, string Year)
        {
            long monIds;
            long.TryParse(MonNum, out monIds);

            long yearIds;
            long.TryParse(Year, out yearIds);

            var vmSwQc = new NinetyFiveProductionRewardModel();

            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

            ViewBag.ddlMonths = selectListItemsMonth;
            //
            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

            ViewBag.ddlYears = selectListItemsYear;

            if (MonNum != null && Year != null)
            {
                ViewBag.GetRewardAndPenalties = _repository.GetRewardAndPenalties(monIds, yearIds);

                ViewBag.CmPenaltiesAndRewardCkdSkd = _repository.CmPenaltiesAndRewardCkdSkd(MonNum, Year);
                ViewBag.CmPenaltiesAndRewardRepeatOrder = _repository.CmPenaltiesAndRewardRepeatOrder(MonNum, Year);
                ViewBag.CmRewardNinetyFiveProduction = _repository.CmRewardNinetyFiveProduction(MonNum, Year);
                ViewBag.CmRewardNinetyFiveSalesOut = _repository.CmRewardNinetyFiveSalesOut(MonNum, Year);

            }

            vmSwQc.Month = MonNum;
           // vmSwQc.MonNum = Convert.ToInt32(MonNum);
            vmSwQc.Year = Year;
            return View(vmSwQc);
            
        }
        [HttpPost]//Incentive
        [NotificationActionFilter(ReceiverRoles = "CM,MM,PS", MessageHeader = "Incentive")]
        public JsonResult MonthlyIncentive(string Month, string MonNum, string Year, string TotalAmount, string objArr, string objArr41)
        {
            var notificationObject = new NotificationObject
            {
                ProjectId = 1
            };
            ViewBag.ControllerVariable = notificationObject;

            List<ViewModels.Commercial.Incentive> results1 = JsonConvert.DeserializeObject<List<ViewModels.Commercial.Incentive>>(objArr);
            Console.Write("result1 :" + results1);

            List<CmIncentiveModel> results2 = JsonConvert.DeserializeObject<List<CmIncentiveModel>>(objArr41);
            Console.Write("results2 :" + results2);

            //var SaveIncentive1 = 0;
            var SaveIncentive1 = _repository.GetSaveIncentive(Month, MonNum, Year, TotalAmount, results1, results2);


            return Json(new { SaveIncentive1 }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult SaveMonthlyIncentiveForNewLogic(string Month, string MonNum, string Year, string TotalAmount21, string objArr21, string objArr31)
        {
            List<ViewModels.Commercial.Incentive> results21 = JsonConvert.DeserializeObject<List<ViewModels.Commercial.Incentive>>(objArr21);
            Console.Write("result21 :" + results21);

            List<ViewModels.Commercial.Incentive> results31 = JsonConvert.DeserializeObject<List<ViewModels.Commercial.Incentive>>(objArr31);
            Console.Write("results31 :" + results31);

            var SaveIncentive21 = _repository.GetSaveIncentive21(Month, MonNum, Year, TotalAmount21, results21, results31);

            return Json(new { SaveIncentive21 }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult SaveCmPenaltiesAndRewardData(string Month, string MonNum, string Year)
        {
            var SaveIncentive21 = _repository.SaveCmPenaltiesAndRewardData(Month, MonNum, Year);
            return Json(new { SaveIncentive21 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult MonthlyIncentiveDetails(List<VmIncentivePolicy> vmIncentivePolicies, string monId, string yearId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            long monIds;
            long.TryParse(monId, out monIds);

            long yearIds;
            long.TryParse(yearId, out yearIds);

            //old policy
            //ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
            //ViewBag.GetOrders = _repository.GetIncentiveOrders(monIds, yearIds);
            //if (monId != null && yearIds != null)
            //{
            //    ViewBag.GetPrimarySaleForIncentive = _repository.GetPrimarySales(monIds, yearIds);
            //}


            //ViewBag.GetServiceForFeature = _repository.GetFeaturePhoneService(monIds, yearIds);
            //ViewBag.GetSalesForFeature = _repository.GetFeaturePhoneSales(monIds, yearIds);
            //ViewBag.GetServiceForSmart = _repository.GetSmartPhoneService(monIds, yearIds);
            //ViewBag.GetSalesForSmart = _repository.GetSmartPhoneSales(monIds, yearIds);
            //ViewBag.GetUserInfo = _repository.GetCmUserList1(monIds, yearIds);
            //ViewBag.Selected = monIds.ToString();
            //ViewBag.SelectedYear = yearId;
            //end old policy

            ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
            ViewBag.GetOrders = _repository.GetIncentiveOrders(monIds, yearIds);
            ViewBag.GetLcs = _repository.GetIncentiveLcs(monIds, yearIds);
            ViewBag.GetSeaShipmentFulls = _repository.GetIncentiveSeaShipmentFulls(monIds, yearIds);
            ViewBag.GetSeaShipmentPartials = _repository.GetIncentiveSeaShipmentPartials(monIds, yearIds);
            ViewBag.GetAirShipmentFulls = _repository.GetIncentiveAirShipmentFulls(monIds, yearIds);
            ViewBag.GetAirShipmentPartials = _repository.GetIncentiveAirShipmentPartials(monIds, yearIds);


            if (monId != null && yearIds != null)
            {
                ViewBag.GetPrimarySaleForIncentive = _repository.GetPrimarySales(monIds, yearIds);
            }


            ViewBag.GetServiceForFeature = _repository.GetFeaturePhoneService(monIds, yearIds);
            ViewBag.GetSalesForFeature = _repository.GetFeaturePhoneSales(monIds, yearIds);
            ViewBag.GetServiceForSmart = _repository.GetSmartPhoneService(monIds, yearIds);
            ViewBag.GetSalesForSmart = _repository.GetSmartPhoneSales(monIds, yearIds);
            ViewBag.GetUserInfo = _repository.GetCmUserList(monIds, yearIds);
            ViewBag.GetUserInfo1 = _repository.GetCmUserList2(monIds, yearIds);
            ViewBag.GetUserInfo2 = _repository.GetCmUserList3(monIds, yearIds);
            ViewBag.Selected = monIds.ToString();
            ViewBag.SelectedYear = yearId;

            return View(vmIncentivePolicies);

        }

        [HttpGet]
        public ActionResult IncentiveReport(string monId, string month, string yearId)
        {

            List<VmIncentivePolicy> vmIncentivePolicies = new List<VmIncentivePolicy>();

            long yearIds;
            long.TryParse(yearId, out yearIds);

            ViewBag.GetReports = _repository.GetIncentiveReport(monId, yearIds);
            ViewBag.GetReports1 = _repository.GetIncentiveReport1(monId, yearIds);
            ViewBag.GetPreparedUser = _repository.GetPreparedUserName();
            //  vmIncentivePolicies = ViewBag.GetReports;
            //  vmIncentivePolicies = ViewBag.GetPreparedUser;

            return View(vmIncentivePolicies);
        }
        [HttpGet]
        public ActionResult CmIncentiveSheet()
        {
            return View();
        }

        public ActionResult RefundProjectAmountForIncentive(string MonNum, string Year)
        {
            long monIds;
            long.TryParse(MonNum, out monIds);

            long yearIds;
            long.TryParse(Year, out yearIds);

            var vmSwQc = new NinetyFiveProductionRewardModel();

            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

            ViewBag.ddlMonths = selectListItemsMonth;
            //
            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

            ViewBag.ddlYears = selectListItemsYear;

            if (MonNum != null && Year != null)
            {

                ViewBag.CmPenaltiesCkdSkd = _repository.CmPenaltiesCkdSkd(MonNum, Year);
                ViewBag.CmPenaltiesRepeatOrder = _repository.CmPenaltiesRepeatOrder(MonNum, Year);
               
            }

            vmSwQc.Month = MonNum;
            vmSwQc.Year = Year;
            return View(vmSwQc);
        }
        [HttpPost]
        public JsonResult SaveCmPenaltiesCkdSkd(NinetyFiveProductionRewardModel refundSave)
        {
            string refundSave1 = null;
            bool isExist = false;
            if (refundSave != null)
            {
                isExist = _repository.GetCmRefundData(refundSave);

            }
            refundSave1 = _repository.SaveCmPenaltiesCkdSkd(refundSave);

            if (isExist)
            {
                refundSave1 = "Action is successful.";
            }
        
            return Json(new { data = refundSave1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveCmPenaltiesRepeatOrder(NinetyFiveProductionRewardModel refundSave)
        {
            string refundSave1 = null;
            bool isExist = false;
            if (refundSave != null)
            {
                isExist = _repository.GetCmRefundData(refundSave);

            }
            refundSave1 = _repository.SaveCmPenaltiesRepeatOrder(refundSave);

            if (isExist)
            {
                refundSave1 = "Action is successful.";
            }

            return Json(new { data = refundSave1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCmOthersIncentive(string objOthersArr)
        {
            List<Cm_OthersIncentiveModel> results= JsonConvert.DeserializeObject<List<Cm_OthersIncentiveModel>>(objOthersArr);
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveCmOthersIncentive(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CmOthersIncentive()
        {
            var vmMod = new VmIncentivePolicy();
            return View(vmMod);
        }
        [HttpPost]
        public ActionResult AddProjectsForOthers(string insOtherTypes, string addedAmount, string remarks, string deductAmount,
         string deductRemarks, string effectiveMonth, string finalAmount)
        {

            var model = new Cm_OthersIncentiveModel();

            model.OthersType = insOtherTypes;
            model.Amount = addedAmount != "" ? Convert.ToDecimal(addedAmount) : 0;
            model.Remarks = remarks;
            model.DeductAmount = deductAmount != "" ? Convert.ToDecimal(deductAmount) : 0;
            model.DeductRemarks = deductRemarks;
            model.EffectiveMonth = Convert.ToDateTime(effectiveMonth);
            model.FinalAmount = Convert.ToDecimal(finalAmount);
           
            return PartialView("~/Views/Commercial/CmPartials/_CmOthersIncentiveList.cshtml", model);


        }

        #endregion

        #region Spare & Incentive  Upto August 2019

        public ActionResult SpareClaim(string proId)
        {
            long proIds;
            long.TryParse(proId, out proIds);

            var spareMdl = new SpareClaimModel();
            spareMdl.ProjectMasterModels = _repository.GetSpareProjectList();
            List<SelectListItem> items = spareMdl.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;



            List<SelectListItem> items1 = new List<SelectListItem>();
            //SelectListItem item = new SelectListItem
            //{
            //    Value = "",
            //    Text = "Select Order"
            //};
            //items1.Add(item);
            spareMdl.ProjectMasterModels = _repository.GetProjectWiseOrderForSpare(proIds);
            items1 = spareMdl.ProjectMasterModels.Select(model => new SelectListItem { Text = model.OrderNuber.ToString(), Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.ProjectOrders1 = items1;

            return View(spareMdl);
        }
        [HttpPost]
        public JsonResult ProjectWiseSpareWarehouseReceiveDate(string projectId, string projectName)
        {
            long proId;
            long.TryParse(projectId, out proId);

            var getReceiveDate = _repository.GetWarehouseReceiveDate(proId);

            //  return Json(new { SaveCharger = saveCharger }, JsonRequestBehavior.AllowGet);

            return Json(new { GetReceiveDate = getReceiveDate }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ProjectWiseOrder(string projectId, string projectName)
        {
            long proId;
            long.TryParse(projectId, out proId);

            SpareClaimModel spareClaim = new SpareClaimModel();
            List<SelectListItem> items1 = new List<SelectListItem>();
            //SelectListItem item=new SelectListItem
            //{
            //    Value = "",
            //    Text = "Select Order"
            //};
            //items1.Add(item);

            spareClaim.ProjectMasterModels = _repository.GetProjectWiseOrderForSpare(proId);
            items1 = spareClaim.ProjectMasterModels.Select(model => new SelectListItem { Text = model.OrderNuber.ToString(), Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();


            ViewBag.ProjectOrders1 = items1;

            return Json(new { GetReceiveDate = spareClaim }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public JsonResult SaveSpareClaimData(string objArr)
        {
            List<SpareClaimModel> results = JsonConvert.DeserializeObject<List<SpareClaimModel>>(objArr);
            Console.Write("result :" + results);

            var isExist = _repository.CheckSpareDataAlreadySaved(results);

            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            var saveSpare = "0";

            if (results.Count != 0)
            {
                saveSpare = _repository.SaveSpareClaimDatas(results);
            }

            return Json(new { SaveSpareData = saveSpare }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public JsonResult GetPreviousSpareData(string projectId, string projectName)
        {
            long proId;
            long.TryParse(projectId, out proId);

            var getPreviousDate = _repository.GetPreviousSpareDatas(proId);

            return Json(new { GetPreviousDate = getPreviousDate }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IncentiveForSpareClaim(List<VmIncentivePolicy> vmIncentivePolicies, string monId, string yearId)
        {

            long userId = Convert.ToInt64(User.Identity.Name);

            long monIds;
            long.TryParse(monId, out monIds);

            long yearIds;
            long.TryParse(yearId, out yearIds);


            var isExist = _repository.CheckSpareIncentiveData(monIds, yearIds);

            //if (isExist)
            //{
            //    var saveIncentive1 = "YES";
            //    return Json(new { SaveMonthlyInc1 = saveIncentive1 }, JsonRequestBehavior.AllowGet);
            //}

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return RedirectToAction("IncentiveForSpareClaim", "Commercial");
            }
            ViewBag.GetNewSpareComplainData = _repository.GetNewSpareComplain();
            ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
            ViewBag.GetTotalSpareClaims = _repository.GetTotalSpareClaim(monId, yearId);
            ViewBag.GetUserInfoForSpareClaim = _repository.GetUserInfoForSpareClaims(monIds, yearIds);
            ViewBag.Selected = monIds.ToString();
            ViewBag.SelectedYear = yearId;

            return View();
        }

        [HttpPost]
        public JsonResult SaveApprovedData(string projectId, string spareClaimDate, string warehouseDate, string quantity, string remarks, string status)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            long qty;
            long.TryParse(quantity, out qty);

            var saveSpareApproved = _repository.SaveSpareApprovedData(proIds, spareClaimDate.Trim(), warehouseDate.Trim(), qty, remarks, status);

            return Json(new { SaveSpareApproveData = saveSpareApproved }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveDeclinedData(string projectId, string spareClaimDate, string warehouseDate, string quantity, string remarks, string status)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            long qty;
            long.TryParse(quantity, out qty);

            var saveSpareDeclined = _repository.SaveSpareDeclinedData(proIds, spareClaimDate.Trim(), warehouseDate.Trim(), qty, remarks, status);

            return Json(new { SaveSpareApproveData = saveSpareDeclined }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveMonthlyIncentiveForSpareClaim(string objArr, string MonNum, string YearName)
        {
            List<SpareClaimModel> results = JsonConvert.DeserializeObject<List<SpareClaimModel>>(objArr);
            Console.Write("result :" + results);


            //var isExist = _repository.GetCheckDate(monId, yearId);

            //if (isExist)
            //{
            //    TempData["Message2"] = "Incentive already generated";
            //    return RedirectToAction("MonthlyIncentive", "Commercial");
            //}
            long monIds;
            long.TryParse(MonNum, out monIds);

            long yearIds;
            long.TryParse(YearName, out yearIds);
            var isExist = _repository.CheckSpareIncentiveData(monIds, yearIds);

            if (isExist)
            {
                var saveIncentive1 = "YES";
                return Json(new { SaveMonthlyInc1 = saveIncentive1 }, JsonRequestBehavior.AllowGet);
            }

            var saveIncentive = "0";

            if (results.Count != 0)
            {
                saveIncentive = _repository.SaveMonthlyIncentiveForSpareClaims(results);
            }


            return Json(new { SaveMonthlyInc = saveIncentive }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Incentive From September 2019
        //[Authorize(Users = "25")]
        //public ActionResult MonthlyIncentive(List<VmIncentivePolicy> vmIncentivePolicies, string monId, string yearId)
        //{
        //    long userId = Convert.ToInt64(User.Identity.Name);

        //    long monIds;
        //    long.TryParse(monId, out monIds);

        //    long yearIds;
        //    long.TryParse(yearId, out yearIds);

        //    var isExist = _repository.GetCheckDate(monId, yearId);

        //    if (isExist)
        //    {
        //        TempData["Message2"] = "Incentive already generated";
        //        return RedirectToAction("MonthlyIncentive", "Commercial");
        //    }

        //    ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
        //    ViewBag.GetOrders = _repository.GetIncentiveOrders(monIds, yearIds);
        //    ViewBag.GetLcs = _repository.GetIncentiveLcs(monIds, yearIds);
        //    ViewBag.GetSeaShipmentFulls = _repository.GetIncentiveSeaShipmentFulls(monIds, yearIds);
        //    ViewBag.GetSeaShipmentPartials = _repository.GetIncentiveSeaShipmentPartials(monIds, yearIds);
        //    ViewBag.GetAirShipmentFulls = _repository.GetIncentiveAirShipmentFulls(monIds, yearIds);
        //    ViewBag.GetAirShipmentPartials = _repository.GetIncentiveAirShipmentPartials(monIds, yearIds);


        //    if (monId != null && yearIds != null)
        //    {
        //        ViewBag.GetPrimarySaleForIncentive = _repository.GetPrimarySales(monIds, yearIds);
        //    }


        //    ViewBag.GetServiceForFeature = _repository.GetFeaturePhoneService(monIds, yearIds);
        //    ViewBag.GetSalesForFeature = _repository.GetFeaturePhoneSales(monIds, yearIds);
        //    ViewBag.GetServiceForSmart = _repository.GetSmartPhoneService(monIds, yearIds);
        //    ViewBag.GetSalesForSmart = _repository.GetSmartPhoneSales(monIds, yearIds);
        //    ViewBag.GetUserInfo = _repository.GetCmUserList(monIds, yearIds);
        //    ViewBag.GetUserInfo1 = _repository.GetCmUserList2(monIds, yearIds);
        //    ViewBag.Selected = monIds.ToString();
        //    ViewBag.SelectedYear = yearId;

        //    return View(vmIncentivePolicies);

        //}

        //[HttpPost]//Incentive
        //[NotificationActionFilter(ReceiverRoles = "CM,MM,PS", MessageHeader = "Incentive")]
        //public JsonResult MonthlyIncentive(string Month, string MonNum, string Year, string TotalAmount, string objArr)
        //{
        //    var notificationObject = new NotificationObject
        //    {
        //        ProjectId = 1
        //    };
        //    ViewBag.ControllerVariable = notificationObject;

        //    List<ViewModels.Commercial.Incentive> results1 = JsonConvert.DeserializeObject<List<ViewModels.Commercial.Incentive>>(objArr);
        //    Console.Write("result1 :" + results1);

        //    //var SaveIncentive1 = 0;
        //    var SaveIncentive1 = _repository.GetSaveIncentive(Month, MonNum, Year, TotalAmount, results1);


        //    return Json(new { SaveIncentive1 }, JsonRequestBehavior.AllowGet);

        //}
        //[HttpPost]
        //public JsonResult SaveMonthlyIncentiveForNewLogic(string Month, string MonNum, string Year, string TotalAmount21, string objArr21)
        //{
        //    List<ViewModels.Commercial.Incentive> results21 = JsonConvert.DeserializeObject<List<ViewModels.Commercial.Incentive>>(objArr21);
        //    Console.Write("result21 :" + results21);

        //    var SaveIncentive21 = _repository.GetSaveIncentive21(Month, MonNum, Year, TotalAmount21, results21);

        //    return Json(new { SaveIncentive21 }, JsonRequestBehavior.AllowGet);

        //}
        //[HttpGet]
        //public ActionResult MonthlyIncentiveDetails(List<VmIncentivePolicy> vmIncentivePolicies, string monId, string yearId)
        //{
        //    long userId = Convert.ToInt64(User.Identity.Name);

        //    long monIds;
        //    long.TryParse(monId, out monIds);

        //    long yearIds;
        //    long.TryParse(yearId, out yearIds);

        //    //ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
        //    //ViewBag.GetOrders = _repository.GetIncentiveOrders(monIds, yearIds);
        //    //if (monId != null && yearIds != null)
        //    //{
        //    //    ViewBag.GetPrimarySaleForIncentive = _repository.GetPrimarySales(monIds, yearIds);
        //    //}


        //    //ViewBag.GetServiceForFeature = _repository.GetFeaturePhoneService(monIds, yearIds);
        //    //ViewBag.GetSalesForFeature = _repository.GetFeaturePhoneSales(monIds, yearIds);
        //    //ViewBag.GetServiceForSmart = _repository.GetSmartPhoneService(monIds, yearIds);
        //    //ViewBag.GetSalesForSmart = _repository.GetSmartPhoneSales(monIds, yearIds);
        //    //ViewBag.GetUserInfo = _repository.GetCmUserList1(monIds, yearIds);
        //    //ViewBag.Selected = monIds.ToString();
        //    //ViewBag.SelectedYear = yearId;

        //    ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
        //    ViewBag.GetOrders = _repository.GetIncentiveOrders(monIds, yearIds);
        //    ViewBag.GetLcs = _repository.GetIncentiveLcs(monIds, yearIds);
        //    ViewBag.GetSeaShipmentFulls = _repository.GetIncentiveSeaShipmentFulls(monIds, yearIds);
        //    ViewBag.GetSeaShipmentPartials = _repository.GetIncentiveSeaShipmentPartials(monIds, yearIds);
        //    ViewBag.GetAirShipmentFulls = _repository.GetIncentiveAirShipmentFulls(monIds, yearIds);
        //    ViewBag.GetAirShipmentPartials = _repository.GetIncentiveAirShipmentPartials(monIds, yearIds);


        //    if (monId != null && yearIds != null)
        //    {
        //        ViewBag.GetPrimarySaleForIncentive = _repository.GetPrimarySales(monIds, yearIds);
        //    }


        //    ViewBag.GetServiceForFeature = _repository.GetFeaturePhoneService(monIds, yearIds);
        //    ViewBag.GetSalesForFeature = _repository.GetFeaturePhoneSales(monIds, yearIds);
        //    ViewBag.GetServiceForSmart = _repository.GetSmartPhoneService(monIds, yearIds);
        //    ViewBag.GetSalesForSmart = _repository.GetSmartPhoneSales(monIds, yearIds);
        //    ViewBag.GetUserInfo = _repository.GetCmUserList(monIds, yearIds);
        //    ViewBag.GetUserInfo1 = _repository.GetCmUserList2(monIds, yearIds);
        //    ViewBag.GetUserInfo2 = _repository.GetCmUserList3(monIds, yearIds);
        //    ViewBag.Selected = monIds.ToString();
        //    ViewBag.SelectedYear = yearId;

        //    return View(vmIncentivePolicies);

        //}

        //[HttpGet]
        //public ActionResult IncentiveReport(string monId, string month, string yearId)
        //{

        //    List<VmIncentivePolicy> vmIncentivePolicies = new List<VmIncentivePolicy>();

        //    long yearIds;
        //    long.TryParse(yearId, out yearIds);

        //    ViewBag.GetReports = _repository.GetIncentiveReport(monId, yearIds);
        //    ViewBag.GetReports1 = _repository.GetIncentiveReport1(monId, yearIds);
        //    ViewBag.GetPreparedUser = _repository.GetPreparedUserName();
        //    //  vmIncentivePolicies = ViewBag.GetReports;
        //    //  vmIncentivePolicies = ViewBag.GetPreparedUser;

        //    return View(vmIncentivePolicies);
        //}
        //[HttpGet]
        //public ActionResult CmIncentiveSheet()
        //{
        //    return View();
        //}

        #endregion

        #region Spare & Incentive From September 2019

        //public ActionResult SpareClaim(string proId)
        //{
        //    long proIds;
        //    long.TryParse(proId, out proIds);

        //    var spareMdl = new SpareClaimModel();
        //    spareMdl.ProjectMasterModels = _repository.GetSpareProjectList();
        //    List<SelectListItem> items = spareMdl.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
        //    ViewBag.Projects = items;



        //    List<SelectListItem> items1 = new List<SelectListItem>();
        //    //SelectListItem item = new SelectListItem
        //    //{
        //    //    Value = "",
        //    //    Text = "Select Order"
        //    //};
        //    //items1.Add(item);
        //    spareMdl.ProjectMasterModels = _repository.GetProjectWiseOrderForSpare(proIds);
        //    items1 = spareMdl.ProjectMasterModels.Select(model => new SelectListItem { Text = model.OrderNuber.ToString(), Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
        //    ViewBag.ProjectOrders1 = items1;

        //    return View(spareMdl);
        //}
        //[HttpPost]
        //public JsonResult ProjectWiseSpareWarehouseReceiveDate(string projectId, string projectName)
        //{
        //    long proId;
        //    long.TryParse(projectId, out proId);

        //    var getReceiveDate = _repository.GetWarehouseReceiveDate(proId);

        //    //  return Json(new { SaveCharger = saveCharger }, JsonRequestBehavior.AllowGet);

        //    return Json(new { GetReceiveDate = getReceiveDate }, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public JsonResult ProjectWiseOrder(string projectId, string projectName)
        //{
        //    long proId;
        //    long.TryParse(projectId, out proId);

        //    SpareClaimModel spareClaim = new SpareClaimModel();
        //    List<SelectListItem> items1 = new List<SelectListItem>();
        //    //SelectListItem item=new SelectListItem
        //    //{
        //    //    Value = "",
        //    //    Text = "Select Order"
        //    //};
        //    //items1.Add(item);

        //    spareClaim.ProjectMasterModels = _repository.GetProjectWiseOrderForSpare(proId);
        //    items1 = spareClaim.ProjectMasterModels.Select(model => new SelectListItem { Text = model.OrderNuber.ToString(), Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();


        //    ViewBag.ProjectOrders1 = items1;

        //    return Json(new { GetReceiveDate = spareClaim }, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]

        //public JsonResult SaveSpareClaimData(string objArr)
        //{
        //    List<SpareClaimModel> results = JsonConvert.DeserializeObject<List<SpareClaimModel>>(objArr);
        //    Console.Write("result :" + results);

        //    var isExist = _repository.CheckSpareDataAlreadySaved(results);

        //    if (isExist)
        //    {
        //        var saveSpare1 = "YES";
        //        return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
        //    }

        //    var saveSpare = "0";

        //    if (results.Count != 0)
        //    {
        //        saveSpare = _repository.SaveSpareClaimDatas(results);
        //    }

        //    return Json(new { SaveSpareData = saveSpare }, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]

        //public JsonResult GetPreviousSpareData(string projectId, string projectName)
        //{
        //    long proId;
        //    long.TryParse(projectId, out proId);

        //    var getPreviousDate = _repository.GetPreviousSpareDatas(proId);

        //    return Json(new { GetPreviousDate = getPreviousDate }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult IncentiveForSpareClaim(List<VmIncentivePolicy> vmIncentivePolicies, string monId, string yearId)
        //{

        //    long userId = Convert.ToInt64(User.Identity.Name);

        //    long monIds;
        //    long.TryParse(monId, out monIds);

        //    long yearIds;
        //    long.TryParse(yearId, out yearIds);


        //    var isExist = _repository.CheckSpareIncentiveData(monIds, yearIds);

        //    //if (isExist)
        //    //{
        //    //    var saveIncentive1 = "YES";
        //    //    return Json(new { SaveMonthlyInc1 = saveIncentive1 }, JsonRequestBehavior.AllowGet);
        //    //}

        //    if (isExist)
        //    {
        //        TempData["Message2"] = "Incentive already generated";
        //        return RedirectToAction("IncentiveForSpareClaim", "Commercial");
        //    }
        //    ViewBag.GetNewSpareComplainData = _repository.GetNewSpareComplain();
        //    ViewBag.GetIncentives = _repository.GetVmIncentivePolicy();
        //    ViewBag.GetTotalSpareClaims = _repository.GetTotalSpareClaim(monId, yearId);
        //    ViewBag.GetUserInfoForSpareClaim = _repository.GetUserInfoForSpareClaims(monIds, yearIds);
        //    ViewBag.Selected = monIds.ToString();
        //    ViewBag.SelectedYear = yearId;

        //    return View();
        //}

        //[HttpPost]
        //public JsonResult SaveApprovedData(string projectId, string spareClaimDate, string warehouseDate, string quantity, string remarks, string status)
        //{
        //    long proIds;
        //    long.TryParse(projectId, out proIds);

        //    long qty;
        //    long.TryParse(quantity, out qty);

        //    var saveSpareApproved = _repository.SaveSpareApprovedData(proIds, spareClaimDate.Trim(), warehouseDate.Trim(), qty, remarks, status);

        //    return Json(new { SaveSpareApproveData = saveSpareApproved }, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public JsonResult SaveDeclinedData(string projectId, string spareClaimDate, string warehouseDate, string quantity, string remarks, string status)
        //{
        //    long proIds;
        //    long.TryParse(projectId, out proIds);

        //    long qty;
        //    long.TryParse(quantity, out qty);

        //    var saveSpareDeclined = _repository.SaveSpareDeclinedData(proIds, spareClaimDate.Trim(), warehouseDate.Trim(), qty, remarks, status);

        //    return Json(new { SaveSpareApproveData = saveSpareDeclined }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult SaveMonthlyIncentiveForSpareClaim(string objArr, string MonNum, string YearName)
        //{
        //    List<SpareClaimModel> results = JsonConvert.DeserializeObject<List<SpareClaimModel>>(objArr);
        //    Console.Write("result :" + results);


        //    //var isExist = _repository.GetCheckDate(monId, yearId);

        //    //if (isExist)
        //    //{
        //    //    TempData["Message2"] = "Incentive already generated";
        //    //    return RedirectToAction("MonthlyIncentive", "Commercial");
        //    //}
        //    long monIds;
        //    long.TryParse(MonNum, out monIds);

        //    long yearIds;
        //    long.TryParse(YearName, out yearIds);
        //    var isExist = _repository.CheckSpareIncentiveData(monIds, yearIds);

        //    if (isExist)
        //    {
        //        var saveIncentive1 = "YES";
        //        return Json(new { SaveMonthlyInc1 = saveIncentive1 }, JsonRequestBehavior.AllowGet);
        //    }

        //    var saveIncentive = "0";

        //    if (results.Count != 0)
        //    {
        //        saveIncentive = _repository.SaveMonthlyIncentiveForSpareClaims(results);
        //    }


        //    return Json(new { SaveMonthlyInc = saveIncentive }, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region CKD Lock

        public JsonResult WarehouseEntryQuantityThisMonth(string podate, string projectName,long poQuantity=0)
        {
            var counter = 0;
            var projectType = _repository.GetProjectByName(projectName).ProjectType;
            var smtLimit = projectType == "Smart" ? 50000 : 250000;
            if (poQuantity <= smtLimit)
            {
                while (true)
                {
                    var dtPoDate = DateTime.ParseExact(podate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime nextPoDate = dtPoDate.AddMonths(counter);
                    long runningSmtQuantity = _repository.WarehouseEntryQuantityThisMonth(nextPoDate, projectName);
                    counter++;
                    if (runningSmtQuantity + poQuantity <= smtLimit)
                    {
                        if (dtPoDate == nextPoDate)
                        {
                            return Json("under SMT capacity");
                        }
                        return Json("Order quantity will cross SMT limit, Please try placing PO in following month - " + nextPoDate.ToString("MMMM") + ", " + nextPoDate.Year);
                    }
                }
            }
            //when order quantity alone crosses the SMT Limit
            return Json("WARNING!!! This Order quantity will cross SMT limit, please communicate with concerned person for further instruction.");
        }

        public long WarehouseEntryQuantityOnSameMonth(string poDate, string projectName)
        {
            var dtPoDate = DateTime.ParseExact(poDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            long runningSmtQuantity = _repository.WarehouseEntryQuantityThisMonth(dtPoDate, projectName);
            return runningSmtQuantity;
        }

        #endregion

        #region Jigs&Fixtures

        public ActionResult AddJigsAndFixtures()
        {
            return PartialView("CmPartials/_JigsAndFixtures");
        }
        #endregion

        #region SMS
        public static bool SendSmsViaRobi(string receiver, string message)
        {
            try
            {
                var apiUrl = string.Format(
                    @"https://api.mobireach.com.bd/SendTextMessage?Username=waltonhitech&Password=Walton@1234&From={2}&To={0}&Message={1}", receiver, message,"WALTON");
                var request = (HttpWebRequest)WebRequest.Create(apiUrl);
                using (var response = (HttpWebResponse)request.GetResponse())
                {

                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream == null) return false;
                        using (var reader = new StreamReader(stream))
                        {
                            string html = reader.ReadToEnd();
                            if (html.Contains("<ErrorCode>0</ErrorCode>"))
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Sms sending exception from robi api" + receiver + "Err Msg: " + exception.Message + "\n");
                return false;
            }
        }
        #endregion

        #region Accessories PO

        public ActionResult AccessoriesPoList(AccessoriesPoVm model)
        {
            model.ChargerPoModels = _repository.GetAllChargerPoModels();
            model.EarphonePoModels = _repository.GetAllEarphonePoModels();
            return View(model);
        }

        public ActionResult ChargerPo(long id=0)
        {
            var model = _repository.GetChargerPoModelById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ChargerPo(ChargerPoModel model)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            if (model.Id > 0)
            {
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
            }
            else
            {
                model.AddedBy = userId;
                model.AddedDate = DateTime.Now;
            }
            var m = _repository.SaveUpdteChargerPoModel(model);
            return RedirectToAction("ChargerPo",new{m.Id});
        }

        public ActionResult EarphonePo(long id = 0)
        {
            var m = _repository.GetEarphonePoModelById(id);
            return View(m);
        }

        [HttpPost]
        public ActionResult EarphonePo(EarphonePoModel model)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            if (model.Id > 0)
            {
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
            }
            else
            {
                model.AddedBy = userId;
                model.AddedDate = DateTime.Now;
            }
            var m = _repository.SaveUpdateEarphonePoModel(model);
            return RedirectToAction("EarphonePo", new {m.Id});
        }
        #endregion

        #region Lc Opening Permission
        [HttpGet]
        public ActionResult LcOpeningPermission(long id = 0, long projectId = 0, int type = 0, string msg = null)
        {
            var model = new VmProjectLc();
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            ViewBag.Projects = _repository.GetAllProjectWithOrderNumber();
            ViewBag.Suppliers = _repository.GeTAllSuppliers();//Supplier list
            if (id > 0 && projectId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                model.ProjectLcModel = _repository.GetProjectLc(id);
                var pos = _repository.GetProjectOrderModels(projectId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;
            }
            else if (projectId > 0)
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                model.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                var pos = _repository.GetProjectOrderModels(projectId);

                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;
                var anyPreviousLc = _repository.GetLcOpeningPermissionByProjectId(projectId);
                if (anyPreviousLc != null)
                {
                    model.PermissionModel.CompanyName = anyPreviousLc.CompanyName;
                    model.PermissionModel.OpeningDate = anyPreviousLc.OpeningDate;
                    model.PermissionModel.StrOpeningDate = anyPreviousLc.OpeningDate == null ? null : Convert.ToDateTime(anyPreviousLc.OpeningDate).ToString("yyyy-MM-dd");
                    model.PermissionModel.SupplierName = anyPreviousLc.SupplierName;
                    model.PermissionModel.SupplierGrade = anyPreviousLc.SupplierGrade;
                    model.PermissionModel.Model = anyPreviousLc.Model;
                    model.PermissionModel.OrderNo = anyPreviousLc.OrderNo;
                    model.PermissionModel.Product = anyPreviousLc.Product;
                    model.PermissionModel.PreviousOrderQunatity = anyPreviousLc.PreviousOrderQunatity;
                    model.PermissionModel.StockQuantity = anyPreviousLc.StockQuantity;
                    model.PermissionModel.PipelineQuantity = anyPreviousLc.PipelineQuantity;
                    model.PermissionModel.OrderQuantity = anyPreviousLc.OrderQuantity;
                    model.PermissionModel.TotalAmount = anyPreviousLc.TotalAmount;
                    model.PermissionModel.TtiPerLine = anyPreviousLc.TtiPerLine;
                    model.PermissionModel.ApproxDateOfShipment = anyPreviousLc.ApproxDateOfShipment;
                    model.PermissionModel.WarehouseReceiveDate = anyPreviousLc.WarehouseReceiveDate;
                    model.PermissionModel.ShipmentConfirmDate = anyPreviousLc.ShipmentConfirmDate;
                    model.PermissionModel.OraclePoNo = anyPreviousLc.OraclePoNo;
                }
            }
            else
            {
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                ViewBag.ProjectOrders = selectListItems;
            }
            return View(model);
        }

        [NotificationActionFilter(ReceiverRoles = "MM,CM,CMBTRC,PM,PS", MessageHeader = "Project LC")]
        [HttpPost]
        public ActionResult LcOpeningPermission(VmProjectLc model)
        {

            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            model.PermissionModel.ProjectMasterId = model.ProjectMasterModel.ProjectMasterId;
            model.PermissionModel.Model = model.ProjectMasterModel.ProjectModel;
            if (model.ProjectMasterModel.OrderNuber != null)
                model.PermissionModel.OrderNo = model.ProjectMasterModel.OrderNuber.ToString();
            model.PermissionModel.SupplierName = model.ProjectMasterModel.SupplierName;
            model.PermissionModel.AddedBy = userId;
            model.PermissionModel.AddedDate = DateTime.Now;
            model.PermissionModel.IsActive = true;
            var lc = _repository.AddToLcPermission(model.PermissionModel);
            _repository.SaveLcOpeningPermissionLog(model.PermissionModel);//log entry
            //=====file upload====
            var moduleDirectory = "LCPermission";
            var userDirectory = "LC";
            if (model.LcOpeningPermissionFileModels.Count >0)
            {
                foreach (var v in model.LcOpeningPermissionFileModels)
                {
                    var manager = new FileManager();
                    v.FilePath = manager.IncidentUpload(userDirectory, moduleDirectory,
                v.FileUpload);
                    var file = new LcOpeningPermissionFileModel
                    {
                        LcPermissionId = lc.Id,
                        FilePath = v.FilePath,
                        Remarks = v.Remarks,
                        AddedBy = userId,
                        AddedDate = DateTime.Now
                    };
                    _commonRepository.SaveLcPermissionFiles(file);
                }
            }
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "CM", "CMHEAD", "CEO", "ACCNTHEAD", "FINHEAD","BIHEAD" }), new List<string>(new[] { "MM", "PS" }), "New LC opening permission for " + model.PermissionModel.Model + " (Order " + model.PermissionModel.OrderNo + ") opened",
            "This is to inform you that, LC permission for " + model.PermissionModel.Model + "(Order " + model.PermissionModel.OrderNo + ") opened by <b>" + userInfo.UserFullName + "</b>.<br/>" 
            + "Product : " + model.PermissionModel.Product +"<br/>"
            + "LC ID : " + model.PermissionModel.Id +"<br/>"
            + "LC Value : " + model.PermissionModel.LcAmount +"<br/>"
            + "LC Quantity : " + model.PermissionModel.OrderQuantity);
            //=========================
            return RedirectToAction("LcPermissionList");
        }

        
        public ActionResult LcPermissionList()
        {

            long userId = Convert.ToInt64(User.Identity.Name);
            var lcOpeningPermissionModels = _repository.GetLcPermissionList();

            return View(lcOpeningPermissionModels);
        }

        public ActionResult LcPermissionOfOtherProductList()
        {
            var v = _repository.GetLcPermissionOtherProductList();
            return View(v);
        }

        
        [HttpGet]
        public ActionResult GetLcPermissionDetailsById(long id)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var lcOpeningPermissionModel = _repository.GetLcPermissionDetailsById(id);
            //var approvalStatus = lcOpeningPermissionModel.IsApproved == true ? "YES" :"NO";
            var approvalStatus = "";
            if (lcOpeningPermissionModel.IsApproved == true)
            {
                approvalStatus = "YES";
            }
            else if (lcOpeningPermissionModel.IsApproved == false)
            {
                approvalStatus = "NO";
            }
            else
            {
                approvalStatus = "NA";
            }
            TempData["userId"] = userId;
            TempData["ApprovalStatus"] = approvalStatus;
            return View(lcOpeningPermissionModel);
        }

        [HttpPost]
        public ActionResult GetLcPermissionDetailsById(long id, string checkedValue)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var lcOpeningPermissionModel = _repository.UpdateApprovalStatus(id, checkedValue);
            TempData["userId"] = userId;
            ViewBag.ApprovalMessage = "Lc Opening Form " + checkedValue.ToUpper() + "  Successfully";

            // TempData.Keep("notice");
            TempData["Message2"] = "Lc Opening Form " + checkedValue.ToUpper() + "  Successfully";
            ViewBag.ss = "Lc Opening Form " + checkedValue.ToUpper() + "  Successfully";

            return View(lcOpeningPermissionModel);
        }

        public ActionResult UpdateLcPermissionDetailsById(long id)
        {
            var v = new VmProjectLc();
            var lcOpeningPermissionModel = _repository.GetLcPermissionDetailsById(id);
            
            var opdate = lcOpeningPermissionModel.OpeningDate.ToString();
            lcOpeningPermissionModel.StrOpeningDate = lcOpeningPermissionModel.OpeningDate==null?null:Convert.ToDateTime(opdate).ToString("yyyy-MM-dd");
            var appShipDate = lcOpeningPermissionModel.ApproxDateOfShipment.ToString();
            lcOpeningPermissionModel.StrApproxDateOfShipment =lcOpeningPermissionModel.ApproxDateOfShipment==null?null: Convert.ToDateTime(appShipDate).ToString("yyyy-MM-dd");
            var warRecDate = lcOpeningPermissionModel.WarehouseReceiveDate.ToString();
            lcOpeningPermissionModel.StrWarehouseReceiveDate =lcOpeningPermissionModel.WarehouseReceiveDate==null?null: Convert.ToDateTime(warRecDate).ToString("yyyy-MM-dd");
            var shipConDate = lcOpeningPermissionModel.ShipmentConfirmDate.ToString();
            lcOpeningPermissionModel.StrShipmentConfirmDate =lcOpeningPermissionModel.ShipmentConfirmDate==null?null: Convert.ToDateTime(shipConDate).ToString("yyyy-MM-dd");
            v.PermissionModel = lcOpeningPermissionModel;
            v.LcOpeningPermissionFileModels = _commonRepository.GetLcOpeningPermissionFilesByLcId(id);
            return View(v);
        }

        [HttpPost]
        public ActionResult UpdateLcPermissionDetailsById(VmProjectLc m)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var previousValues = _commonRepository.GetLcOpeningPermissionById(m.PermissionModel.Id);
            m.PermissionModel.OpeningDate = m.PermissionModel.StrOpeningDate==null?m.PermissionModel.OpeningDate:DateTime.ParseExact(m.PermissionModel.StrOpeningDate, "yyyy-MM-dd", null);
            m.PermissionModel.ApproxDateOfShipment = m.PermissionModel.StrApproxDateOfShipment == null ? m.PermissionModel.ApproxDateOfShipment : DateTime.ParseExact(m.PermissionModel.StrApproxDateOfShipment, "yyyy-MM-dd", null);
            m.PermissionModel.WarehouseReceiveDate =m.PermissionModel.StrWarehouseReceiveDate==null?m.PermissionModel.WarehouseReceiveDate: DateTime.ParseExact(m.PermissionModel.StrWarehouseReceiveDate, "yyyy-MM-dd", null);
            m.PermissionModel.ShipmentConfirmDate =m.PermissionModel.StrShipmentConfirmDate==null?m.PermissionModel.ShipmentConfirmDate: DateTime.ParseExact(m.PermissionModel.StrShipmentConfirmDate, "yyyy-MM-dd", null);
            m.PermissionModel.UpdatedDate = DateTime.Now;
            m.PermissionModel.UpdatedBy = userId;
            //if (previousValues != null && (previousValues.OrderQuantity != m.PermissionModel.OrderQuantity || m.PermissionModel.LcAmount!=previousValues.LcAmount))
            //{
            //    //Ovee sir approval reset
            //    m.PermissionModel.ApprovedBy = null;
            //    m.PermissionModel.ApprovedByRemarks = null;
            //    m.PermissionModel.ApprovedDate = null;
            //    m.PermissionModel.IsApproved = null;
            //}
            _repository.UpdateLcOpeningPermissionModel(m.PermissionModel);
            _repository.SaveLcOpeningPermissionLog(m.PermissionModel);//log entry
            foreach (var v in m.LcOpeningPermissionFileModels)
            {
                if (v.FileUpload != null)
                {
                    var moduleDirectory = "LCPermission";
                    var userDirectory = "LC";
                    var manager = new FileManager();
                    v.FilePath = manager.IncidentUpload(userDirectory, moduleDirectory,v.FileUpload);
                    var file = new LcOpeningPermissionFileModel
                    {
                        LcPermissionId = m.PermissionModel.Id,
                        FilePath = v.FilePath,
                        Remarks = v.Remarks,
                        AddedBy = userId,
                        AddedDate = DateTime.Now
                    };
                    _commonRepository.SaveLcPermissionFiles(file);
                }
            }
            //===email notification====
            var orderQuantityChangeText = "";
            if (previousValues != null && previousValues.OrderQuantity != m.PermissionModel.OrderQuantity)
            {
                orderQuantityChangeText = " Previous order qunatity was " + previousValues.OrderQuantity+".";
            }
            var lcAmountChangeText = "";
            if (previousValues != null && previousValues.LcAmount != m.PermissionModel.LcAmount)
            {
                lcAmountChangeText = " Previous LC amount was " + previousValues.LcAmount + ".";
            }
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "CM", "CMHEAD", "CEO", "ACCNTHEAD", "FINHEAD","BIHEAD" }), new List<string>(new[] { "MM", "PS" }), "LC opening permission data for " + m.PermissionModel.Model + " (Order " + m.PermissionModel.OrderNo + ") updated",
            "This is to inform you that, LC permission for " + m.PermissionModel.Model + "(Order " + m.PermissionModel.OrderNo + ") updated by " + userInfo.UserFullName + "."+orderQuantityChangeText+lcAmountChangeText+"<br/>" + "LC ID: <b>" + m.PermissionModel.Id + "</b></br>Product : <b>" + m.PermissionModel.Product + "</b></br>LC Order Quantity : <b>" + m.PermissionModel.OrderQuantity + "</b></br>Lc Amount: <b>"+m.PermissionModel.LcAmount+"<b>");
            //---------------
            return RedirectToAction("UpdateLcPermissionDetailsById",new{m.PermissionModel.Id});
        }

        public ActionResult DeleteLcFile(long id = 0)
        {
            _commonRepository.DeleteLcFile(id);
            return new EmptyResult();
        }

        public ActionResult LcOpeningPermissionForOtherProduct(long id=0)
        {
            var vm = new LcOpeningOtherProductViewModel();
            vm.LcOpeningPermissionOtherProductModel = _repository.GetLcOpeningPermissionOtherProductById(id);
            vm.LcOpeningPermissionOtherFileModels.AddRange(_commonRepository.GetLcOpeningPermissionOtherFilesByLcId(id));
            ViewBag.ProductModels = _commonRepository.GetModelListForRelevantModels();//for relevant models
            ViewBag.Suppliers = _repository.GeTAllSuppliers();//Supplier list
            return View(vm);
        }

        [HttpPost]
        public ActionResult LcOpeningPermissionForOtherProduct(LcOpeningOtherProductViewModel m)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            if (m.LcOpeningPermissionOtherProductModel.Id == 0)
            {
                m.LcOpeningPermissionOtherProductModel.BdtLcValue =
                    CommonConversion.CurrencyConversion(Convert.ToDecimal(m.LcOpeningPermissionOtherProductModel.LcAmount),
                        m.LcOpeningPermissionOtherProductModel.Currency, "BDT").ToString("####.####");
                m.LcOpeningPermissionOtherProductModel.IsActive = true;
                m.LcOpeningPermissionOtherProductModel.AddedDate = DateTime.Now;
                m.LcOpeningPermissionOtherProductModel.AddedBy = userId;
            }
            if (m.LcOpeningPermissionOtherProductModel.Id > 0)
            {
                m.LcOpeningPermissionOtherProductModel.UpdatedDate = DateTime.Now;
                m.LcOpeningPermissionOtherProductModel.UpdatedBy = userId;
            }
            var v = _repository.SaveLcOpeningOtherProduct(m.LcOpeningPermissionOtherProductModel);
            //=====file upload====
            var moduleDirectory = "LCPermission";
            var userDirectory = "LC";
            if (m.LcOpeningPermissionOtherFileModels.Count > 0)
            {
                foreach (var f in m.LcOpeningPermissionOtherFileModels)
                {
                    if (f.Id == 0)
                    {
                        var manager = new FileManager();
                        f.FilePath = manager.IncidentUpload(userDirectory, moduleDirectory,
                    f.FileUpload);
                        var file = new LcOpeningPermissionOtherFileModel
                        {
                            LcOtherPermissionId = v,
                            FilePath = f.FilePath,
                            Remarks = f.Remarks,
                            AddedBy = userId,
                            AddedDate = DateTime.Now
                        };
                        _commonRepository.SaveLcPermissionOtherFiles(file);
                    }
                }
            }
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            if (m.LcOpeningPermissionOtherProductModel.Id == 0)
            {
                mailSendFromPms.SendMail(new List<string>(new[] {"MM", "CM", "CMHEAD", "CEO", "ACCNTHEAD", "FINHEAD","BIHEAD"}),
                    new List<string>(new[] {"PS"}),
                    "New LC opening permission for " + m.LcOpeningPermissionOtherProductModel.ProductType + " (Order " +
                    m.LcOpeningPermissionOtherProductModel.OrderNo + ") opened",
                    "This is to inform you that, LC permission for " +
                    m.LcOpeningPermissionOtherProductModel.ProductType + "(Order " +
                    m.LcOpeningPermissionOtherProductModel.OrderNo + ") opened by <b>" + userInfo.UserFullName +
                    "</b>.<br/>"
                    + "Product : " + m.LcOpeningPermissionOtherProductModel.Product + "<br/>"
                    + "LC ID : " + v + "<br/>"
                    + "LC Value : " + m.LcOpeningPermissionOtherProductModel.LcAmount + "<br/>"
                    + "LC Quantity : " + m.LcOpeningPermissionOtherProductModel.OrderQuantity);
            }
            else
            {
                mailSendFromPms.SendMail(new List<string>(new[] { "MM", "CM", "CMHEAD", "CEO", "ACCNTHEAD", "FINHEAD","BIHEAD" }),
                    new List<string>(new[] { "PS" }),
                    "LC opening permission for " + m.LcOpeningPermissionOtherProductModel.ProductType + " (Order " +
                    m.LcOpeningPermissionOtherProductModel.OrderNo + ") has been updated",
                    "This is to inform you that, LC permission for " +
                    m.LcOpeningPermissionOtherProductModel.ProductType + "(Order " +
                    m.LcOpeningPermissionOtherProductModel.OrderNo + ") updated by <b>" + userInfo.UserFullName +
                    "</b>.<br/>"
                    + "Product : " + m.LcOpeningPermissionOtherProductModel.Product + "<br/>"
                    + "LC ID : " + m.LcOpeningPermissionOtherProductModel.Id + "<br/>"
                    + "LC Value : " + m.LcOpeningPermissionOtherProductModel.LcAmount + "<br/>"
                    + "LC Quantity : " + m.LcOpeningPermissionOtherProductModel.OrderQuantity);
            }
            
            //=========================
            return RedirectToAction("LcOpeningPermissionForOtherProduct",new{id=v});
        }

        public ActionResult ApprovedLcReportWithinDateRange()
        {
            return View();
        }

        public JsonResult GetLcOpeningPermissionsByDateRange(string fromDate, string toDate)
        {
            var from = DateTime.ParseExact(fromDate ?? DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
            var to = DateTime.ParseExact(toDate ?? DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
            var handsetLc = _repository.GetHandsetLcApprovalsByDateRange(from, to);
            var otherLc = _repository.GetLcOpeningPermissionOtherProdWithinDateRange(from, to);
            return Json(new{handsetLc,otherLc});
        }
        #endregion



        public JsonResult GetFinishGoodDetails(string ProjectOrderShipmentId)
        {
            long proShipOrder;
            long.TryParse(ProjectOrderShipmentId, out proShipOrder);

            var finishGood = _repository.GetFinishGoodDetails(proShipOrder);

            //List<SelectListItem> items =
            //    finishGood.Select(
            //        model =>
            //            new SelectListItem
            //            {
            //                Text = model.PurchaseOrderNumber,
            //                Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
            //            }).ToList();

           // var json = JsonConvert.SerializeObject(finishGood);

            return new JsonResult { Data = finishGood, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #region order wise multiple price

        public ActionResult OrderWiseMultiplePrice()
        {
            ViewBag.Projects = _commonRepository.GetAllProjects();
            ViewBag.Prices = _repository.GetallOrderWiseMultiplePrice();
            return View();
        }

        [HttpPost]
        public ActionResult OrderWiseMultiplePrice(OrderWiseMultiplePriceModel model)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            model.AddedBy = userId;
            model.AddedDate = DateTime.Now;
            _repository.SaveOrderWiseMultiplePrice(model);
            return RedirectToAction("OrderWiseMultiplePrice");
        }

        public JsonResult GetProjectInfoById(long projectId = 0)
        {
            var project = _commonRepository.GetProjectInfoByProjectId(projectId);
            return Json(project);
        }

        public JsonResult UpdateMultiPrice(string remarks, long id = 0, decimal quantity = 0,
            decimal price = 0)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            var model = _repository.GetOrderWiseMultiplePriceById(id);
            model.Quantity = quantity;
            model.Price = price;
            model.Remarks = remarks;
            model.UpdatedDate = DateTime.Now;
            model.UpdatedBy = userId;
            model = _repository.UpdateOrderWiseMultiplePrice(model);
            return Json(model);
        }
        #endregion

        #region ACTIVATE DEACTIVATE PROJECT

        public JsonResult ActivateDeactivateProject(string remarks, long projectId = 0)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            var user = _hardwareRepository.GetUserInfoByUserId(userId);
            var project = _commonRepository.GetProjectInfoByProjectId(projectId);
            var variants = _commonRepository.GetOrderQuantityDetailByProjectId(projectId);
            if (project.IsActive)
            {
                project.IsActive = false;
                project.DeactivatedBy = userId;
                project.DeactivationDate = DateTime.Now;
                project.ActivationDeactivationRemarks = remarks;
                _repository.UpdateProject(project, userId);
                foreach (var i in variants)
                {
                    i.IsActive = false;
                    _commonRepository.SaveUpdateProjectVariantInOrderQuantityDetail(i);
                }
                //===Project Updated Mail===
                var body =
                string.Format(
                    @"This is to inform you that Project: <b>" +
                    project.ProjectName + " (Project ID: " + project.ProjectMasterId + "), order:"+project.OrderNuber+"</b>, <br/>has been DEACTIVATED by : <b>" + user.UserFullName + "</b>.");
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM","CMHEAD","PMHEAD","PS","CM" }),
                    new List<string>(new[] { "" }),
                    "Project ( " + project.ProjectName + " order "+project.OrderNuber+") has been DEACTIVATED", body);
                //-----O----
                return Json("DEACTIVATED");
            }
            if (!project.IsActive)
            {
                project.IsActive = true;
                project.ActivationBy = userId;
                project.ActivationDate = DateTime.Now;
                project.ActivationDeactivationRemarks = remarks;
                _repository.UpdateProject(project, userId);
                foreach (var i in variants)
                {
                    i.IsActive = true;
                    _commonRepository.SaveUpdateProjectVariantInOrderQuantityDetail(i);
                }
                //===Project Updated Mail===
                var body =
                string.Format(
                    @"This is to inform you that Project: <b>" +
                    project.ProjectName + " (Project ID: " + project.ProjectMasterId + "), order:" + project.OrderNuber + "</b>, <br/>has been ACTIVATED by : <b>" + user.UserFullName + "</b>.");
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM", "CMHEAD", "PMHEAD", "PS", "CM" }),
                    new List<string>(new[] { "" }),
                    "Project ( " + project.ProjectName + " order " + project.OrderNuber + ") has been ACTIVATED", body);
                //-----O----
                return Json("ACTIVATED");
            }
            return Json("ERROR");
        }
        #endregion

        #region IDH LC

        public ActionResult CreateIdhLc()
        {
            ViewBag.Variants = _commonRepository.GetVariantsWithOrderNumber();
            return View();
        }

        public JsonResult GetLcIdhFinalBomsByVariantId(long id = 0)
        {
            var v = _repository.GetLcIdhFinalBomModelByVariantId(id);
            return Json(v);
        }

        public JsonResult GetIDHFinalBomInfoBySpareId(long spareId)
        {
            var v = _repository.GetIDHFinalBomInfoBySpareId(spareId);
            return Json(v);
        }

        public JsonResult SaveBulkIdhLc(List<LC_IDH_Details> arr, long variantId)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            var orderSerial = _repository.GetLastOrderSerialInIdhDetails(variantId)+1;
            foreach (var a in arr)
            {
                var idh = new LC_IDH_Details
                {
                    LcIdhFinalBomId = a.LcIdhFinalBomId,
                    VariantId = variantId,
                    UnitPrice = a.UnitPrice,
                    OrderQuantity = a.OrderQuantity,
                    TotalValue = a.TotalValue,
                    OrderSerial = orderSerial,
                    AddedBy = userId,
                    AddedDate = DateTime.Now
                };
                _repository.SaveIdhLcDetails(idh);
            }
            return Json("success");
        }

        public ActionResult IdhFinalBomUpload(LC_IDH_Final_BOMModel model)
        {
            ViewBag.Variants = _commonRepository.GetVariantsWithOrderNumber();
            return View();
        }

        [HttpPost]
        public ActionResult UploadIdhBOMExcel(LC_IDH_Final_BOMModel bomModel)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            if (Request != null)
            {
                //file = Request.Files["IDH_BOM_upload"];
                var extension = bomModel.BomFile!=null?Path.GetExtension(bomModel.BomFile.FileName):null;
                if (extension != null)
                {
                    var fileExt = extension.Substring(1);
                    if ((bomModel.BomFile != null) && (bomModel.BomFile.ContentLength > 0) && !string.IsNullOrEmpty(bomModel.BomFile.FileName) && fileExt=="xlsx" && bomModel.VariantId != null)
                    {
                        var variantInfo = _iqcRepository.GetVariantById((long) bomModel.VariantId);
                        using (var pack = new ExcelPackage(bomModel.BomFile.InputStream))
                        {
                            var currentSheet = pack.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfRow = workSheet.Dimension.End.Row;
                            var noOfColumn = workSheet.Dimension.End.Column;
                            for (var i = 2; i <= noOfRow; i++)
                            {
                                var bom = new LC_IDH_Final_BOMModel();
                                //general data
                                bom.VariantId = variantInfo.Id;
                                bom.variantName = variantInfo.ProjectModel;
                                bom.ProjectMasterId = variantInfo.ProjectMasterId;
                                bom.AddedBy = userId;
                                bom.AddedDate = DateTime.Now;
                                //excel data
                                bom.MaterialCoding = workSheet.Cells[i, 1].Value.ToString();
                                bom.MaterialName = workSheet.Cells[i, 2].Value.ToString();
                                bom.Specification = workSheet.Cells[i, 3].Value.ToString();
                                bom.Vendor = workSheet.Cells[i, 4].Value.ToString();
                                bom.UnitOfMeasurement = workSheet.Cells[i, 5].Value.ToString();
                                bom.PerUnitQuantity = Convert.ToInt32(workSheet.Cells[i, 6].Value);
                                bom.TotalQuantity = Convert.ToInt32(workSheet.Cells[i, 7].Value);
                                bom.ExtraOrderPerUnitQuantity = Convert.ToDecimal(workSheet.Cells[i, 8].Value);
                                bom.ExtraOrderQuantity = Convert.ToInt32(workSheet.Cells[i, 9].Value);
                                bom.PerUnitQuantityConsideringWastage = Convert.ToDecimal(workSheet.Cells[i, 10].Value);
                                bom.TotalQuantityConsideringWastage = Convert.ToInt32(workSheet.Cells[i, 11].Value);
                                bom.UsedIn = workSheet.Cells[i, 12].Value.ToString();
                                bom.InventoryCode = workSheet.Cells[i, 13].Value.ToString();
                                bom.Remarks = workSheet.Cells[i, 14].Value==null?"": workSheet.Cells[i, 13].Value.ToString();
                                _repository.SaveIdhBom(bom);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("IdhFinalBomUpload");
        }

        public JsonResult IdhBomExists(long variantId = 0)
        {
            var v = _repository.GetLcIdhFinalBomsByVariantId(variantId);
            return Json(v);
        }

        public JsonResult GetPrevIdhDetailsByVariantId(long variantId = 0)
        {
            var v = _repository.GetPrevIdhDetailsByVariantId(variantId);
            return Json(v);
        }
        #endregion

        public ActionResult DownloadImeiByDateRange()
        {
            return View();
        }

        public ActionResult GetModelWiseReportData(DateTime fromDate, DateTime todate)
        {
            try
            {
                var memoryStream = new MemoryStream();
                //ExcelPackage.LicenseContext = LicenseContext.Commercial;
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var zip = new ZipFile(); //Install-Package DotNetZip -Version 1.15.0

                var data = _repository.GetModelList(fromDate, todate);
                string dd = "";
                foreach (var mm in data)
                {
                    var test = _repository.GetModelWiseReportData(fromDate, todate, mm.Model);
                    var dt = new System.Data.DataTable();
                    dt.Columns.Add("SI No");
                    dt.Columns.Add("Brand", typeof(string));
                    dt.Columns.Add("IMEI TAC 1", typeof(string));
                    dt.Columns.Add("IMEI TAC 2", typeof(string));
                    dt.Columns.Add("IMEI TAC 3", typeof(string));
                    dt.Columns.Add("IMEI TAC 4", typeof(string));
                    dt.Columns.Add("IMEI 1", typeof(string));
                    dt.Columns.Add("IMEI 2", typeof(string));
                    dt.Columns.Add("IMEI 3", typeof(string));
                    dt.Columns.Add("IMEI 4", typeof(string));
                    foreach (var item in test)
                    {
                        var row = dt.NewRow();
                        row["SI No"] = item.SERIAL_NO;
                        row["Brand"] = item.Brand;
                        row["IMEI TAC 1"] = item.IMEI_TAC_1;
                        row["IMEI TAC 2"] = item.IMEI_TAC_2;
                        row["IMEI TAC 3"] = item.IMEI_TAC_3;
                        row["IMEI TAC 4"] = item.IMEI_TAC_4;
                        row["IMEI 1"] = item.IMEI1;
                        row["IMEI 2"] = item.IMEI2;
                        row["IMEI 3"] = item.IMEI3;
                        row["IMEI 4"] = item.IMEI4;
                        dt.Rows.Add(row);
                    }
                    using (var package = new ExcelPackage())
                    {
                        // ExcelPackage.Workbook excelWorkBook = excelApp.Workbooks.Add("");
                        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: true);
                        for (var col = 1; col < dt.Columns.Count + 1; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        zip.AddEntry(mm.Model + ".xlsx", package.GetAsByteArray());
                        //  dd = mm.Model + ".xlsx";

                        //Session["DownloadExcel_FileManager"] = package.GetAsByteArray();
                        //return Json("", JsonRequestBehavior.AllowGet);
                    }

                }


                zip.Save(memoryStream);
                //HttpContext.Response.Cookies.Add(new HttpCookie("imei", cookieValue));
                //return File(memoryStream.ToArray(), "application/zip", "Model_Wise_Report.zip");
                return File(memoryStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Zip,
                    "IMEI of " + " from " + fromDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) +
                    " to " + todate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + ".zip");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult ProjectSpecWithPriceDetails(string stat)
        {
            var model = _commonRepository.GetAllProjects();
            if (stat == "POC")
            {
                model = model.Where(x => x.MarketClearanceDate != null).ToList();
            }
            if (stat == "RUN")
            {
                model = model.Where(x => x.MarketClearanceDate == null).ToList();
            }
            ViewBag.Stat = stat;
            return View(model);
        }

        public JsonResult GetPriceDetails(long projectId = 0)
        {
            var prices = _commonRepository.GetAccessoriesPricesByProjectId(projectId);
            var fob = _repository.GetFobPriceUpdateLogByProjectId(projectId);
            return Json(new{prices,fob});
        }

        public ActionResult AddProjectImagePartial()
        {
            return PartialView("_ProjectImagesPartialView");
        }
    }

    public class MakeRange
    {
        public string StartImeiRange { get; set; }
        public string EndImeiRange { get; set; }
    }
}