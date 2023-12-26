using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.ProjectCommercial;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class ProjectCommercialRepository : IProjectCommercialRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public ProjectCommercialRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }

        public ProjectBabtModel GetProjectBabtInfo(long projectId)
        {
            var babtInfoOfProject = _dbEntities.ProjcetBabts.FirstOrDefault(x => x.ProjectMasterId == projectId);


            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjcetBabt, ProjectBabtModel>());
            var mapper = config.CreateMapper();
            var babtInfo = mapper.Map<ProjectBabtModel>(babtInfoOfProject);


            return babtInfo;
        }

        public long GetProjectPurchaseOrderFormId(long masterId = 0)
        {
            if (masterId > 0)
            {
                var projectPurchaseOrderForm = _dbEntities.ProjectPurchaseOrderForms.FirstOrDefault(x => x.ProjectMasterId == masterId);
                if (projectPurchaseOrderForm != null)
                {
                    var purchaseOrderFormId =
                        projectPurchaseOrderForm.ProjectPurchaseOrderFormId;
                    return purchaseOrderFormId;
                }
            }
            return 0;
        }

        public string SaveBabtInfo(long projectMasterId, long assignedId, long purchaseOrderFormId, long babtId, long quantity)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                const string returnValue = "y";
                var projcetBabt = new ProjcetBabt
                {
                    ProjectMasterId = projectMasterId,
                    PmAssignId = assignedId,
                    ProjectPurchaseOrderFormId = purchaseOrderFormId,
                    PmImeiRangeRequestDate = DateTime.Now,
                    RequestedImeiQuantity = quantity,
                    Added = userId,
                    AddedDate = DateTime.Now
                };
                _dbEntities.ProjcetBabts.AddOrUpdate(projcetBabt);
                _dbEntities.SaveChanges();
                return returnValue;
            }
            catch (Exception)
            {
                return "n";
            }
        }

        public string SaveSendingSupplierDate(long pmId, string sendingDate)
        {
            string returnValue = "notSaved";
            var previousData = _dbEntities.ProjcetBabts.FirstOrDefault(x => x.ProjectMasterId == pmId);
            if (previousData != null)
            {
                DateTime dt = DateTime.ParseExact(sendingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                previousData.RangeToSupplierDate = dt;
                _dbEntities.SaveChanges();

                returnValue = "saved";

            }
            return returnValue;
        }

        public List<VmTacRequest> GetProjectsForTac(long userId)
        {
            var tacs = (from pmAssign in _dbEntities.ProjectPmAssigns
                        join master in _dbEntities.ProjectMasters on pmAssign.ProjectMasterId equals master.ProjectMasterId
                        join orderForm in _dbEntities.ProjectPurchaseOrderForms on pmAssign.ProjectMasterId equals
                            orderForm.ProjectMasterId
                        join babt in _dbEntities.ProjcetBabts on orderForm.ProjectPurchaseOrderFormId equals babt.ProjectPurchaseOrderFormId

                    into temp
                        from projcetBabt in temp.DefaultIfEmpty()
                        where
                            pmAssign.ProjectManagerUserId == userId && orderForm.IsCompleted == false

                        select new VmTacRequest
                        {
                            ProjectMasterId = pmAssign.ProjectMasterId,
                            ProjectName = master.ProjectName,
                            AssignedId = pmAssign.AssignUserId,
                            ProjectPurchaseFormOrderId = orderForm.ProjectPurchaseOrderFormId,
                            PurchaseOrderNo = orderForm.PurchaseOrderNumber,
                            PurchaseOrderQuantity = orderForm.Quantity,
                            TacNo = projcetBabt.TacNo,
                            TacRequestDate = projcetBabt.PmImeiRangeRequestDate,
                            ImeiRangeFrom = projcetBabt.ImeiRangeFrom,
                            ImeiRangeTo = projcetBabt.ImeiRangeTo,
                            ToSupplierDate = projcetBabt.RangeToSupplierDate,
                            RequestedImeiQuantity = projcetBabt.RequestedImeiQuantity,
                            ProjectBabtId = projcetBabt.ProjectBabtId,
                            OrderNuber = master.OrderNuber

                        }

            ).ToList();
            List<VmTacRequest> tacRequests = tacs;

            foreach (var project in tacRequests)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }

            }

            return tacRequests;
        }


        #region Commercial KPI
        public List<CmnUserModel> GetCmUsersUnderHead(string persons, long monIds, long yearIds)
        {
            var qq1 = (from cu in _dbEntities.CmnUsers where cu.EmployeeCode == persons select cu).FirstOrDefault();

            var userList = new List<CmnUserModel>();

            if (qq1 != null && qq1.RoleName == "CMHEAD")
            {
                userList =
                    _dbEntities.Database.SqlQuery<CmnUserModel>(
                        @"select ProfilePictureUrl,UserFullName,UserName as EmployeeCode,Designation
                from [CellPhoneProject].[dbo].[CmnUsers] where RoleName in ('CM','SPR') and IsActive=1").ToList();
            }
            else if (persons == "ALL")
            {
                userList =
                  _dbEntities.Database.SqlQuery<CmnUserModel>(
                      @"select ProfilePictureUrl,UserFullName,UserName as EmployeeCode,Designation
                from [CellPhoneProject].[dbo].[CmnUsers] where RoleName in ('CM','SPR','CMHEAD') and IsActive=1").ToList();
            }
            else
            {
                userList = new List<CmnUserModel>();
            }

            return userList;
        }

        public List<CmnUserModel> GetCommercialUsers()
        {
            var userList =
                _dbEntities.Database.SqlQuery<CmnUserModel>(
                    @"select UserFullName, EmployeeCode from [CellPhoneProject].[dbo].CmnUsers where RoleName in ('CM','CMHEAD','SPR') and IsActive=1 ").ToList();
            return userList;
        }

        public List<TeamKpiPercentageListModel> GetCmKpi(string persons, long monIds, long yearIds)
        {
            List<TeamKpiPercentageListModel> cmList = new List<TeamKpiPercentageListModel>();

            _dbEntities.Database.CommandTimeout = 6000;
            var kpiRolePerson = persons;
            var mms = Convert.ToInt32(monIds);
            long yers = yearIds;
            //if (persons == "ALL")
            if (persons != null)
            {
                //Smart phone
                var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                if (proEventSmart.Count == 0)
                {
                    var rrVal =
                        (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                            .FirstOrDefault();
                    
                    if (rrVal != null)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = "Materials Arriving (Smart Phone)";
                        items.Target1 = Convert.ToString(rrVal.Target);
                        items.Weight = Convert.ToInt32(rrVal.Weight);
                        items.TotalList = 0;
                        items.TotalAverageAchievement = 0;
                        items.TotalAverageScore = 0;
                        items.TotalAverageScorePercent = 0;
                        items.EmployeeCode = persons;
                        cmList.Add(items);
                    }

                }
                foreach (var project in proEventSmart)
                {
                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.TotalAverageScorePercent = Convert.ToInt32(project.TotalAverageScore);
                    items.EmployeeCode = persons;
                    cmList.Add(items);
                }
                //Feature
                var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                if (proEventFeature.Count == 0)
                {
                    var rrVal =
                        (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                            .FirstOrDefault();
                    if (rrVal != null)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = "Materials Arriving (Feature Phone)";
                        items.Target1 = Convert.ToString(rrVal.Target);
                        items.Weight = Convert.ToInt32(rrVal.Weight);
                        items.TotalList = 0;
                        items.TotalAverageAchievement = 0;
                        items.TotalAverageScore = 0;
                        items.TotalAverageScorePercent = 0;
                        items.EmployeeCode = persons;
                        cmList.Add(items);
                    }
                }
                foreach (var project in proEventFeature)
                {

                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.TotalAverageScorePercent = Convert.ToInt32(project.TotalAverageScore);
                    items.EmployeeCode = persons;
                    cmList.Add(items);
                }

                //Repeat
                var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                if (proEventRepeat.Count == 0)
                {
                    var rrVal =
                        (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                            .FirstOrDefault();
                    if (rrVal != null)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = "Repeat Order";
                        items.Target1 = "100/80";
                        items.Weight = Convert.ToInt32(rrVal.Weight);
                        items.TotalList = 0;
                        items.TotalAverageAchievement = 0;
                        items.TotalAverageScore = 0;
                        items.TotalAverageScorePercent = 0;
                        items.EmployeeCode = persons;
                        cmList.Add(items);
                    }
                }
                foreach (var project in proEventRepeat)
                {

                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.TotalAverageScorePercent = Convert.ToInt32(project.TotalAverageScore);
                    items.EmployeeCode = persons;
                    cmList.Add(items);
                }

                //IQC
                var proEventIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                if (proEventIqc.Count == 0)
                {
                    var rrVal =
                       (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                           .FirstOrDefault();
                    if (rrVal != null)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = "Material Pass on CHN IQC";
                        items.Target1 = "--";
                        items.Weight = Convert.ToInt32(rrVal.Weight);
                        items.TotalList = 0;
                        items.TotalAverageAchievement = 0;
                        items.TotalAverageScore = 0;
                        items.TotalAverageScorePercent = 0;
                        items.EmployeeCode = persons;
                        cmList.Add(items);
                    }
                }
                foreach (var project in proEventIqc)
                {
                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.TotalAverageScorePercent = Convert.ToInt32(project.TotalAverageScore);
                    items.EmployeeCode = persons;
                    cmList.Add(items);
                }
            }//end if condition


            TeamKpiPercentageListModel itemsTotal = new TeamKpiPercentageListModel();

            //itemsTotal.GrandTotalAverageScore = cmList.Sum(i => i.TotalAverageScore);
            itemsTotal.KpiName = "Total :";
            itemsTotal.Target1 = "--";
            itemsTotal.Weight = cmList.Sum(i => i.Weight);
            //itemsTotal.TotalList = cmList.Sum(i => i.TotalList);
            //itemsTotal.TotalAverageAchievement = cmList.Sum(i => i.TotalAverageAchievement);
            itemsTotal.TotalList = 0;
            itemsTotal.TotalAverageAchievement = 0;
            itemsTotal.TotalAverageScore = cmList.Sum(i => i.TotalAverageScore);
            itemsTotal.TotalAverageScorePercent = Convert.ToInt32(cmList.Sum(i => i.TotalAverageScore));
            itemsTotal.EmployeeCode = persons;

            cmList.Add(itemsTotal);

            return cmList;
        }

        public List<TeamKpiPercentageListModel> CommercialKpiDetails(string persons, long monIds, long yearIds, string kpiName)
        {
            List<TeamKpiPercentageListModel> cmList = new List<TeamKpiPercentageListModel>();

            _dbEntities.Database.CommandTimeout = 6000;

            if (persons != null)
            {
                if (kpiName == "Smart")
                {
                    //Smart phone
                    string proEvSmart =
                        string.Format(@"select cast(ProjectMasterId as bigint) as ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,ShipmentType,cast(OrderNumber as int) as OrderNumber,IsFinalShipment,PoDate,WarehouseEntryDate,
			Weight,Target,DaysPassed,case when TotalDays>0 then cast(TotalDays as varchar(50))+' '+'Days (Early)' else cast(TotalDays as varchar(50))+' '+'Days (Late)' end as
			TotalDays,Achievement,cast((Achievement*Weight)/100 as decimal(16,2)) as Score		
					         
	         from
	          (
					select ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,ShipmentType,OrderNumber,IsFinalShipment,PoDate,WarehouseEntryDate,cast(Weight as int) as Weight,cast(Target as int) as Target,cast(DaysPassed as int) as DaysPassed,TotalDays,cast((((TotalDays/Target)*100)+100) as decimal(16,2)) as Achievement							
					from
					(
						   select ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,OrderNumber,ShipmentType,PoDate,WarehouseEntryDate,cast(EffectiveDays as decimal(16,2)) as Target,cast(DaysPassed as decimal(16,2)) as DaysPassed,
						   case when DaysPassed<EffectiveDays then EffectiveDays-DaysPassed 
						   when DaysPassed>EffectiveDays then EffectiveDays-DaysPassed end as TotalDays,Weight,IsFinalShipment
				   			
						   from
						   (
								select distinct ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysPassed,
								case when pm.ProjectType=tli.ProjectType then tli.Target end as EffectiveDays,tli.Weight,ps.IsFinalShipment,ppf.ProjectMasterId,
								pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber as OrderNumber,KpiName

								from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
								left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
								left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId

								left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.ProjectType=pm.ProjectType

								where tli.RoleName='CM' and DATEPART(mm,ps.WarehouseEntryDate)='{1}' and  DATENAME(YEAR,ps.WarehouseEntryDate)='{2}' 
								and pm.OrderNuber=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1 and pm.ProjectType='Smart'
								and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments
								where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
						  )A
					)B  
              )C", persons, monIds, yearIds);

                    var proEventSmart = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvSmart).ToList();

                    foreach (var project in proEventSmart)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.ProjectMasterId = project.ProjectMasterId;
                        items.ProjectName = project.ProjectName;
                        items.KpiName = project.KpiName;
                        items.ProjectType = project.ProjectType;
                        items.SourcingType = project.SourcingType;
                        items.ShipmentType = project.ShipmentType;
                        items.OrderNumber = project.OrderNumber;
                        items.IsFinalShipment = project.IsFinalShipment;
                        items.PoDate = project.PoDate;
                        items.WarehouseEntryDate = project.WarehouseEntryDate;
                        items.Target = project.Target;
                        items.Weight = project.Weight;
                        items.DaysPassed = project.DaysPassed;
                        items.TotalDays = project.TotalDays;
                        items.Achievement = project.Achievement;
                        items.Score = project.Score;
                        cmList.Add(items);
                    }
                }
                else if (kpiName == "Feature")
                {
                    //Feature phone
                    string proEvFeature =
                        string.Format(@"select cast(ProjectMasterId as bigint) as ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,ShipmentType,cast(OrderNumber as int) as OrderNumber,IsFinalShipment,PoDate,WarehouseEntryDate,
			Weight,Target,DaysPassed,case when TotalDays>0 then cast(TotalDays as varchar(50))+' '+'Days (Early)' else cast(TotalDays as varchar(50))+' '+'Days (Late)' end as
			TotalDays,Achievement,cast((Achievement*Weight)/100 as decimal(16,2)) as Score		
					         
	         from
	          (
					select ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,ShipmentType,OrderNumber,IsFinalShipment,PoDate,WarehouseEntryDate,cast(Weight as int) as Weight,cast(Target as int) as Target,cast(DaysPassed as int) as DaysPassed,TotalDays,cast((((TotalDays/Target)*100)+100) as decimal(16,2)) as Achievement							
					from
					(
						   select ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,OrderNumber,ShipmentType,PoDate,WarehouseEntryDate,cast(EffectiveDays as decimal(16,2)) as Target,cast(DaysPassed as decimal(16,2)) as DaysPassed,
						   case when DaysPassed<EffectiveDays then EffectiveDays-DaysPassed 
						   when DaysPassed>EffectiveDays then EffectiveDays-DaysPassed end as TotalDays,Weight,IsFinalShipment
				   			
						   from
						   (
								select distinct ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysPassed,
								case when pm.ProjectType=tli.ProjectType then tli.Target end as EffectiveDays,tli.Weight,ps.IsFinalShipment,ppf.ProjectMasterId,
								pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber as OrderNumber,KpiName

								from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
								left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
								left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId

								left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.ProjectType=pm.ProjectType

								where tli.RoleName='CM' and DATEPART(mm,ps.WarehouseEntryDate)='{1}' and  DATENAME(YEAR,ps.WarehouseEntryDate)='{2}' 
								and pm.OrderNuber=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1 and pm.ProjectType='Feature'
								and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments
								where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
						  )A
					)B  
              )C", persons, monIds, yearIds);

                    var proEventFeature = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvFeature).ToList();

                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.ProjectMasterId = project.ProjectMasterId;
                        items.ProjectName = project.ProjectName;
                        items.KpiName = project.KpiName;
                        items.ProjectType = project.ProjectType;
                        items.SourcingType = project.SourcingType;
                        items.ShipmentType = project.ShipmentType;
                        items.OrderNumber = project.OrderNumber;
                        items.IsFinalShipment = project.IsFinalShipment;
                        items.PoDate = project.PoDate;
                        items.WarehouseEntryDate = project.WarehouseEntryDate;
                        items.Target = project.Target;
                        items.Weight = project.Weight;
                        items.DaysPassed = project.DaysPassed;
                        items.TotalDays = project.TotalDays;
                        items.Achievement = project.Achievement;
                        items.Score = project.Score;
                        cmList.Add(items);
                    }
                }//end else if
                else if (kpiName == "Repeat")
                {
                    //Repeat Order
                    string proEvRepeat =
                        string.Format(@"select cast(ProjectMasterId as bigint) as ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,ShipmentType,cast(OrderNumber as int) as OrderNumber,IsFinalShipment,PoDate,WarehouseEntryDate,
			Weight,Target,DaysPassed,case when TotalDays>0 then cast(TotalDays as varchar(50))+' '+'Days (Early)' else cast(TotalDays as varchar(50))+' '+'Days (Late)' end as
			TotalDays,Achievement,cast((Achievement*Weight)/100 as decimal(16,2)) as Score		
					         
	         from
	          (
					select ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,ShipmentType,OrderNumber,IsFinalShipment,PoDate,WarehouseEntryDate,cast(Weight as int) as Weight,cast(Target as int) as Target,cast(DaysPassed as int) as DaysPassed,TotalDays,cast((((TotalDays/Target)*100)+100) as decimal(16,2)) as Achievement							
					from
					(
						   select ProjectMasterId,ProjectName,KpiName,ProjectType,SourcingType,OrderNumber,ShipmentType,PoDate,WarehouseEntryDate,cast(EffectiveDays as decimal(16,2)) as Target,cast(DaysPassed as decimal(16,2)) as DaysPassed,
						   case when DaysPassed<EffectiveDays then EffectiveDays-DaysPassed 
						   when DaysPassed>EffectiveDays then EffectiveDays-DaysPassed end as TotalDays,Weight,IsFinalShipment
				   			
						   from
						   (
								select distinct ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysPassed,
								case when tli.ShipmentType=ps.ShipmentType then tli.Target end as EffectiveDays,tli.Weight,ps.IsFinalShipment,ppf.ProjectMasterId,
								pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber as OrderNumber,KpiName

								from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
								left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
								left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId

								left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.ShipmentType=ps.ShipmentType

								where tli.RoleName='CM' and DATEPART(mm,ps.WarehouseEntryDate)='{1}' and  DATENAME(YEAR,ps.WarehouseEntryDate)='{2}' 
								and pm.OrderNuber not in (1) and ps.IsFinalShipment='Yes' and pm.IsActive=1 

								and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments
								where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
						  )A
					)B  
              )C", persons, monIds, yearIds);

                    var proEventRepeat = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvRepeat).ToList();

                    foreach (var project in proEventRepeat)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.ProjectMasterId = project.ProjectMasterId;
                        items.ProjectName = project.ProjectName;
                        items.KpiName = project.KpiName;
                        items.ProjectType = project.ProjectType;
                        items.SourcingType = project.SourcingType;
                        items.ShipmentType = project.ShipmentType;
                        items.OrderNumber = project.OrderNumber;
                        items.IsFinalShipment = project.IsFinalShipment;
                        items.PoDate = project.PoDate;
                        items.WarehouseEntryDate = project.WarehouseEntryDate;
                        items.Target = project.Target;
                        items.Weight = project.Weight;
                        items.DaysPassed = project.DaysPassed;
                        items.TotalDays = project.TotalDays;
                        items.Achievement = project.Achievement;
                        items.Score = project.Score;
                        cmList.Add(items);
                    }
                }//end else if
            }
            return cmList;
        }

        public List<TeamKpiPercentageListModel> CommercialIqcKpiDetails(string persons, long monIds, long yearIds,
            string kpiName)
        {
            var cmList = new List<TeamKpiPercentageListModel>();

            _dbEntities.Database.CommandTimeout = 6000;

            if (persons != null)
            {
                if (kpiName == "Iqc")
                {
                    //Iqc
                    string proEvIqc =
                        string.Format(@"
                        select distinct ProjectMasterId,ProjectName,tli.KpiName,rii.ProjectType,rii.PoCategory as SourcingType,cast(rii.Orders as int) as OrderNumber,
                        ProjectManagerClearanceDate,cast(tli.Weight as int) as Weight,
                        cast(NoOfTimeInspection as int) as NoOfTimeInspection,cast(Percentage as decimal(16,2)) as Achievement,cast((Percentage*Weight)/100 as decimal(16,2)) as Score 

                        from [CellPhoneProject].[dbo].[RawMaterialInspection] rii  
                        left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.Target=NoOfTimeInspection

                        where tli.RoleName='CM' and DATEPART(mm,[ProjectManagerClearanceDate])='{1}' 
                        and  DATENAME(YEAR,[ProjectManagerClearanceDate])='{2}'  order by Achievement desc", persons, monIds, yearIds);

                    var proEventIqc = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvIqc).ToList();

                    foreach (var project in proEventIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.ProjectMasterId = project.ProjectMasterId;
                        items.ProjectName = project.ProjectName;
                        items.KpiName = project.KpiName;
                        items.ProjectType = project.ProjectType;
                        items.SourcingType = project.SourcingType;
                        items.OrderNumber = project.OrderNumber;
                        items.ProjectManagerClearanceDate = project.ProjectManagerClearanceDate;
                        items.Weight = project.Weight;
                        items.NoOfTimeInspection = project.NoOfTimeInspection;
                        items.Achievement = project.Achievement;
                        items.Score = project.Score;
                        cmList.Add(items);
                    }
                }

            }
            return cmList;
        }

        public List<TeamKpiPercentageListModel> CommercialKpiLineChart(string persons, string sDate, string endDate)
        {
            List<TeamKpiPercentageListModel> cmList = new List<TeamKpiPercentageListModel>();

            _dbEntities.Database.CommandTimeout = 6000;

            var startDate1 = sDate.Split(',');
            var startMonth = startDate1[0].Trim();
            var startYear = startDate1[1].Trim();
            int startYear1 = Convert.ToInt32(startYear);
            int startMonNum1 = DateTime.ParseExact(startMonth, "MMMM", CultureInfo.CurrentCulture).Month;

            var endDate1 = endDate.Split(',');
            var endMonth = endDate1[0].Trim();
            var endYear = endDate1[1].Trim();
            int endYear1 = Convert.ToInt32(endYear);
            int endMonNum1 = DateTime.ParseExact(endMonth, "MMMM", CultureInfo.CurrentCulture).Month;

            var kpiRolePerson = persons;

            if (persons != null)
            {

                var start1 = startMonNum1;
                var end2 = endMonNum1;

                long yers;

                #region When end date smaller than start date
                if (end2 < start1 && endYear1 > startYear1)
                {
                    #region 1 to End Date
                    for (var mms = 1; mms <= end2; mms++)
                    {
                        yers = endYear1;
                        //Smart phone

                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        //Feature//                     

                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                        //Repeat order

                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                        //Iqc
                        var proEventIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                    }
                    #endregion

                    #region Start Date to 12
                  
                    for (var mms = start1; mms <= 12; mms++)
                    {
                        yers = startYear1;
                        //Smart phone
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        //Feature

                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                        //Repeat order

                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventRepeat)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                        //Iqc
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEvIqc)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                    }//end for
                    #endregion


                }//end if
                #endregion

                #region When End Date greater than Start Date  2020-11 and 2019-09
                if (end2 > start1 && endYear1 == startYear1)
                {
                    yers = endYear1;
                    for (var mms = start1; mms <= end2; mms++)
                    {
                        //Smart phone
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        //Feature
                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                        //Repeat order
                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                        //Iqc
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEvIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                    }
                }
                // if (end2 > start1 && endYear1 == startYear1)
                if (end2 > start1 && endYear1 > startYear1)
                {
                    for (var mms = 1; mms <= end2; mms++)
                    {
                        yers = endYear1;
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEvIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }

                    }//end for
                   
                    for (var mms = start1; mms <= 12; mms++)
                    {
                        yers = start1;
                       //Smart phone
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEvIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                    }
                }

                #endregion

                #region When StartDate=EndDate and endDate> start date or endDate=start Date
                if (end2 == start1 && endYear1 == startYear1)
                {
                    yers = endYear1;
                    for (var mms = start1; mms <= end2; mms++)
                    {
                        //Smart phone
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEvIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                    }
                }
                //end date> start date
                if (end2 == start1 && endYear1 > startYear1)
                {
                    for (var mms = start1; mms <= 12; mms++)
                    {
                        yers = start1;
                        //Smart phone
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEvIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                    }
                    for (var mms = 1; mms <= end2; mms++)
                    {
                        yers = endYear1;
                        var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventSmart)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEventFeature)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                        foreach (var project in proEventRepeat)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                        var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                        foreach (var project in proEvIqc)
                        {

                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = project.KpiName;
                            items.Target1 = project.Target1;
                            items.Weight = Convert.ToInt32(project.Weight);
                            items.TotalList = Convert.ToInt32(project.TotalList);
                            items.TotalAverageAchievement = project.TotalAverageAchievement;
                            items.TotalAverageScore = project.TotalAverageScore;
                            items.Month = project.Month;
                            items.MonthNum = mms;
                            items.Year = Convert.ToInt32(project.Year);
                            cmList.Add(items);
                        }
                    }
                }//end if

                #endregion

                #region com
                //                //Smart phone
                //                string proEvSmart = string.Format(@" 
                //                 select KpiName,cast(Target as varchar) as Target1,cast(Weight as int) Weight, cast(TotalList as int) TotalList,cast(((Achievement)/TotalList)as decimal(16,2)) as TotalAverageAchievement,
                //		         cast(((Score)/TotalList)as decimal(16,2)) as TotalAverageScore,DateName(mm,DATEADD(mm,'{1}' - 1,0)) as Month
                //		            from
                //                    (
                //		              select KpiName,Target,Weight,sum(Achievement) as Achievement,sum(Score) as Score,count(*) as TotalList	  
                //		  
                //		              from
                //		              (		
                //		                select ProjectName,ProjectType,Target,DaysPassed,
                //			            case when TotalDays>0 then cast(TotalDays as varchar(50))+' '+'Days (Early)' else cast(TotalDays as varchar(50))+' '+'Days (Late)' end as
                //			            TotalDays,Weight,Achievement,cast((Achievement*Weight)/100 as decimal(16,2)) as Score,KpiName		
                //					         
                //	                     from
                //	                      (
                //					            select ProjectName,ProjectType,DaysPassed,EffectiveDays as Target,TotalDays,Weight,cast((((TotalDays/EffectiveDays)*100)+100) as decimal(16,2)) as Achievement,KpiName 							
                //					            from
                //					            (
                //						               select ProjectName,cast(DaysPassed as decimal(16,2)) as DaysPassed,ProjectType,cast(EffectiveDays as decimal(16,2)) as EffectiveDays,
                //						               case when DaysPassed<EffectiveDays then EffectiveDays-DaysPassed 
                //						               when DaysPassed>EffectiveDays then EffectiveDays-DaysPassed end as TotalDays,Weight,KpiName
                //				   				 
                //						               from
                //						               (
                //								            select distinct ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysPassed,
                //								            case when pm.ProjectType=tli.ProjectType then tli.Target end as EffectiveDays,tli.Weight,ps.IsFinalShipment,ppf.ProjectMasterId,
                //								            pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber as OrderNumber,KpiName
                //
                //								            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
                //								            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
                //								            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
                //
                //								            left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.ProjectType=pm.ProjectType
                //
                //								            where DATEPART(mm,ps.WarehouseEntryDate)='{1}' and  DATENAME(YEAR,ps.WarehouseEntryDate)='{2}' 
                //								            and pm.OrderNuber=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1 and pm.ProjectType='Smart'
                //								            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments
                //								            where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
                //						              )A
                //					             )B 
                //                             )C
                //		                 )D group by KpiName,Target,Weight
                //                     )E", persons, monIds, yearIds);

                //                var proEventSmart = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvSmart).ToList();

                //                foreach (var project in proEventSmart)
                //                {

                //                    var items = new TeamKpiPercentageListModel();
                //                    items.KpiName = project.KpiName;
                //                    items.Target1 = project.Target1;
                //                    items.Weight = project.Weight;
                //                    items.TotalList = project.TotalList;
                //                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                //                    items.TotalAverageScore = project.TotalAverageScore;
                //                    items.Month = project.Month;
                //                    items.MonthNum = project.MonthNum;
                //                    cmList.Add(items);
                //                }
                //                //Feature
                //                string proEvFeature = string.Format(@"
                //                 select KpiName,cast(Target as varchar) as Target1,cast(Weight as int) Weight, cast(TotalList as int) TotalList,cast(((Achievement)/TotalList)as decimal(16,2)) as TotalAverageAchievement,
                //		         cast(((Score)/TotalList)as decimal(16,2)) as TotalAverageScore,DateName(mm,DATEADD(mm,'{1}' - 1,0)) as Month,Year
                //		            from
                //                    (
                //		              select KpiName,Target,Weight,sum(Achievement) as Achievement,sum(Score) as Score,count(*) as TotalList,Year	  
                //		  
                //		              from
                //		              (		
                //		                select ProjectName,ProjectType,Target,DaysPassed,
                //			            case when TotalDays>0 then cast(TotalDays as varchar(50))+' '+'Days (Early)' else cast(TotalDays as varchar(50))+' '+'Days (Late)' end as
                //			            TotalDays,Weight,Achievement,cast((Achievement*Weight)/100 as decimal(16,2)) as Score,KpiName,Year		
                //					         
                //	                     from
                //	                      (
                //					            select ProjectName,ProjectType,DaysPassed,EffectiveDays as Target,TotalDays,Weight,cast((((TotalDays/EffectiveDays)*100)+100) as decimal(16,2)) as Achievement,KpiName,Year 							
                //					            from
                //					            (
                //						               select ProjectName,cast(DaysPassed as decimal(16,2)) as DaysPassed,ProjectType,cast(EffectiveDays as decimal(16,2)) as EffectiveDays,
                //						               case when DaysPassed<EffectiveDays then EffectiveDays-DaysPassed 
                //						               when DaysPassed>EffectiveDays then EffectiveDays-DaysPassed end as TotalDays,Weight,KpiName,cast(Year as int) as Year
                //				   				 
                //						               from
                //						               (
                //								            select distinct ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysPassed,
                //								            case when pm.ProjectType=tli.ProjectType then tli.Target end as EffectiveDays,tli.Weight,ps.IsFinalShipment,ppf.ProjectMasterId,
                //								            pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber as OrderNumber,KpiName,YEAR(ps.WarehouseEntryDate) AS Year 
                //
                //								            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
                //								            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
                //								            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
                //
                //								            left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.ProjectType=pm.ProjectType
                //
                //								            where DATEPART(mm,ps.WarehouseEntryDate)='{1}' and  DATENAME(YEAR,ps.WarehouseEntryDate)='{2}' 
                //								            and pm.OrderNuber=1 and ps.IsFinalShipment='Yes' and pm.IsActive=1 and pm.ProjectType='Feature'
                //								            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments
                //								            where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
                //						              )A
                //					             )B 
                //                             )C
                //		                 )D group by KpiName,Target,Weight,Year
                //                     )E", persons, endMonNum1, endYear1);

                //                var proEventFeature = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvFeature).ToList();

                //                foreach (var project in proEventFeature)
                //                {

                //                    var items = new TeamKpiPercentageListModel();
                //                    items.KpiName = project.KpiName;
                //                    items.Target1 = project.Target1;
                //                    items.Weight = project.Weight;
                //                    items.TotalList = project.TotalList;
                //                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                //                    items.TotalAverageScore = project.TotalAverageScore;
                //                    items.Month = project.Month;
                //                    items.MonthNum = project.MonthNum;
                //                    cmList.Add(items);
                //                }

                //                //Repeat order
                //                string proEvRepeat = string.Format(@"
                //                 select KpiName,Target1='100/80',Weight, TotalList,cast(((Achievement)/TotalList)as decimal(16,2)) as TotalAverageAchievement,
                //	            cast(((Score)/TotalList)as decimal(16,2)) as TotalAverageScore,DateName(mm,DATEADD(mm,'{1}' - 1,0)) as Month,Year
                //	            from   
                //	             (	
                //		            select KpiName,Target1='100/80',cast(Weight as int) Weight, sum(TotalList) as TotalList,sum(Achievement) as Achievement,sum(Score) as Score,Year
                //		            from
                //		            (
                //			            select KpiName,Target,Weight,sum(Achievement) as Achievement,sum(Score) as Score,count(*) as TotalList,Year	  
                //		  
                //			            from
                //			            (		
                //			            select ProjectName,ProjectType,Target,DaysPassed,
                //			            case when TotalDays>0 then cast(TotalDays as varchar(50))+' '+'Days (Early)' else cast(TotalDays as varchar(50))+' '+'Days (Late)' end as
                //			            TotalDays,Weight,Achievement,cast((Achievement*Weight)/100 as decimal(16,2)) as Score,KpiName,Year		
                //					         
                //				            from
                //				            (
                //					            select ProjectName,ProjectType,DaysPassed,EffectiveDays as Target,TotalDays,Weight,cast((((TotalDays/EffectiveDays)*100)+100) as decimal(16,2)) as Achievement,KpiName,Year 							
                //					            from
                //					            (
                //							            select ProjectName,cast(DaysPassed as decimal(16,2)) as DaysPassed,ProjectType,cast(EffectiveDays as decimal(16,2)) as EffectiveDays,
                //							            case when DaysPassed<EffectiveDays then EffectiveDays-DaysPassed 
                //							            when DaysPassed>EffectiveDays then EffectiveDays-DaysPassed end as TotalDays,Weight,KpiName,cast(Year as int) as Year
                //				   				 
                //							            from
                //							            (
                //								            select distinct ppf.PoDate,ps.WarehouseEntryDate,((DATEDIFF(day, ppf.PoDate, ps.WarehouseEntryDate))) as DaysPassed,
                //								            case when tli.ShipmentType=ps.ShipmentType then tli.Target end as EffectiveDays,tli.Weight,ps.IsFinalShipment,ppf.ProjectMasterId,
                //								            pm.ProjectName,pm.ProjectType,pm.SourcingType,ps.ShipmentType,pm.OrderNuber as OrderNumber,KpiName,YEAR(ps.WarehouseEntryDate) AS Year 
                //
                //								            from CellPhoneProject.dbo.ProjectPurchaseOrderForms ppf 
                //								            left join CellPhoneProject.dbo.ProjectOrderShipments ps on ps.ProjectMasterId=ppf.ProjectMasterId
                //								            left join CellPhoneProject.dbo.ProjectMasters pm on pm.ProjectMasterId=ps.ProjectMasterId
                //
                //								            left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.ShipmentType=ps.ShipmentType
                //
                //								            where DATEPART(mm,ps.WarehouseEntryDate)='{1}' and  DATENAME(YEAR,ps.WarehouseEntryDate)='{2}' 
                //								            and pm.OrderNuber not in (1) and ps.IsFinalShipment='Yes' and pm.IsActive=1 
                //								            and ps.WarehouseEntryDate = (select  top 1  WarehouseEntryDate from CellPhoneProject.dbo.ProjectOrderShipments
                //								            where ProjectMasterId=ps.ProjectMasterId order by WarehouseEntryDate desc)
                //							            )A
                //						            )B 
                //					            )C
                //				            )D group by KpiName,Target,Weight,Year
                //			            )E group by KpiName,Weight,Year
                //
                //                     )F", persons, endMonNum1, endYear1);

                //                var proEventRepeat = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvRepeat).ToList();

                //                foreach (var project in proEventRepeat)
                //                {

                //                    var items = new TeamKpiPercentageListModel();
                //                    items.KpiName = project.KpiName;
                //                    items.Target1 = project.Target1;
                //                    items.Weight = project.Weight;
                //                    items.TotalList = project.TotalList;
                //                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                //                    items.TotalAverageScore = project.TotalAverageScore;
                //                    items.Month = project.Month;
                //                    items.MonthNum = project.MonthNum;
                //                    cmList.Add(items);
                //                }

                //                //Iqc
                //                string proEvIqc = string.Format(@"
                //                   select KpiName, cast(Target as varchar) as Target1,Weight,TotalList,cast(((Achievement)/TotalList)as decimal(16,2)) as TotalAverageAchievement,
                //	               cast(((Score)/TotalList)as decimal(16,2)) as TotalAverageScore,DateName(mm,DATEADD(mm,'{1}' - 1,0)) as Month,Year   from
                //		            (
                //		              select KpiName,Target='--',Weight,sum(Achievement) as Achievement, sum(Score) as Score,sum(TotalList) as TotalList,Year from
                //		              (
                //			             select ProjectMasterId,KpiName,Target,Weight,Achievement, cast (((Weight*Achievement)/100) as decimal(16,2)) as Score,TotalList,Year from
                //			            (
                //			               select ProjectMasterId,KpiName,Target,Weight,Achievement,count(*) as TotalList,cast(Year as int) as Year from
                //			               (
                //				              select distinct tli.KpiName,tli.Weight,tli.Target,NoOfTimeInspection,Percentage as Achievement,ProjectMasterId,ProjectName,YEAR(ProjectManagerClearanceDate) AS Year 
                // 
                //				              from [CellPhoneProject].[dbo].[RawMaterialInspection] rii  
                //				              left join CellPhoneProject.dbo.TeamKpiPercentageList tli on tli.Target=NoOfTimeInspection
                //
                //				              where tli.RoleName='CM' and DATEPART(mm,[ProjectManagerClearanceDate])='{1}' and  DATENAME(YEAR,[ProjectManagerClearanceDate])='{2}' 
                //
                //				            )A group by ProjectMasterId,KpiName,Target,Achievement,Weight,Year    
                //			            )B 
                //		              )C  group by KpiName,Weight,Year         
                //		            )D", persons, endMonNum1, endYear1);

                //                var proEventIqc = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(proEvIqc).ToList();

                //                foreach (var project in proEventIqc)
                //                {

                //                    var items = new TeamKpiPercentageListModel();
                //                    items.KpiName = project.KpiName;
                //                    items.Target1 = project.Target1;
                //                    items.Weight = project.Weight;
                //                    items.TotalList = project.TotalList;
                //                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                //                    items.TotalAverageScore = project.TotalAverageScore;
                //                    items.Month = project.Month;
                //                    items.MonthNum = project.MonthNum;
                //                    cmList.Add(items);
                //                }
                #endregion
            }//end if condition
            return cmList;
        }

        public List<CmnUserModel> GetCommercialUsersDetailsCM(string persons)
        {
            var userList = new List<CmnUserModel>();
            
            userList =
           _dbEntities.Database.SqlQuery<CmnUserModel>(
               @"select * from [CellPhoneProject].[dbo].CmnUsers where IsActive=1 and EmployeeCode={0} ", persons).ToList();

            return userList;
        }
        public List<CmnUserModel> GetCommercialUsersDetailsCMHEAD()
        {
            var userList =
               _dbEntities.Database.SqlQuery<CmnUserModel>(
                   @"select * from [CellPhoneProject].[dbo].CmnUsers where  IsActive=1  
                   and RoleName in ('CMHEAD')").ToList();
            return userList;
        }

        public List<CmnUserModel> GetCommercialUsersDetailsInformation(string persons)
        {
            var userList =
          _dbEntities.Database.SqlQuery<CmnUserModel>(
              @"select cm1.UserFullName,cm1.EmployeeCode,cm1.Designation,cm1.Department,cm1.Section,cm1.DateOfJoining,cm1.Status,cm1.ServiceLength,DATEDIFF(year,cm1.DateOfJoining,GETDATE()) AS ServiceLength1,
                ( select cm2.UserFullName+' ('+cm2.EmployeeCode+'), '+cm2.Designation as LineManager from  [CellPhoneProject].[dbo].[CmnUsers] cm2 where cm2.RoleName='CMHEAD' and cm2.IsActive=1) as LineManager

                from [CellPhoneProject].[dbo].[CmnUsers] cm1
                where cm1.RoleName in ('CM','SPR','CMHEAD') and cm1.IsActive=1  and cm1.EmployeeCode={0} ", persons).ToList();
            return userList;
        }

        public List<TeamKpiPercentageListModel> CommercialKpiSingleBarChart(string persons, string monNum, string year)
        {
            List<TeamKpiPercentageListModel> cmList = new List<TeamKpiPercentageListModel>();

            _dbEntities.Database.CommandTimeout = 6000;

            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            if (persons != null && mon != 0 && years != 0)
            {

                var kpiRolePerson = persons;
                var mms = Convert.ToInt32(mon);
                long yers = years;

                var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                foreach (var project in proEventSmart)
                {

                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.Month = project.Month;
                    items.MonthNum = mon;
                    items.Year = Convert.ToInt32(project.Year);
                    cmList.Add(items);
                }
                //Feature
                var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                foreach (var project in proEventFeature)
                {
                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.Month = project.Month;
                    items.MonthNum = mon;
                    items.Year = Convert.ToInt32(project.Year);
                    cmList.Add(items);
                }

                //Repeat order

                var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();

                foreach (var project in proEventRepeat)
                {

                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.Month = project.Month;
                    items.MonthNum = mon;
                    items.Year = Convert.ToInt32(project.Year);
                    cmList.Add(items);
                }

                //Iqc
                var proEventIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                foreach (var project in proEventIqc)
                {

                    var items = new TeamKpiPercentageListModel();
                    items.KpiName = project.KpiName;
                    items.Target1 = project.Target1;
                    items.Weight = Convert.ToInt32(project.Weight);
                    items.TotalList = Convert.ToInt32(project.TotalList);
                    items.TotalAverageAchievement = project.TotalAverageAchievement;
                    items.TotalAverageScore = project.TotalAverageScore;
                    items.Month = project.Month;
                    items.MonthNum = mon;
                    items.Year = Convert.ToInt32(project.Year);
                    cmList.Add(items);
                }

            }//end if

            return cmList;
        }
        public List<TeamKpiRoleTable> GetKpiRoleName()
        {
            var roleList = _dbEntities.Database.SqlQuery<TeamKpiRoleTable>(@"select * from [CellPhoneProject].[dbo].[TeamKpiRoleTable] 
            where IsActive=1").ToList();
            return roleList;
        }

        public List<CmnUserModel> GetRolePerson(string proRoleName)
        {
            var nameList = new List<CmnUserModel>();

            if (proRoleName=="CM")
            {
                nameList = _dbEntities.Database.SqlQuery<CmnUserModel>(@"select * from [CellPhoneProject].[dbo].[CmnUsers] where RoleName in ('CM','CMHEAD','SPR') and IsActive=1 ").ToList();
            }
            else if (proRoleName == "PM")
            {
                nameList = _dbEntities.Database.SqlQuery<CmnUserModel>(@"select * from [CellPhoneProject].[dbo].[CmnUsers] where RoleName in ('PM','PMHEAD') and IsActive=1 ").ToList();
            }
            else if (proRoleName == "QC")
            {
                nameList = _dbEntities.Database.SqlQuery<CmnUserModel>(@"select * from [CellPhoneProject].[dbo].[CmnUsers] where RoleName in ('QC','QCHEAD') and IsActive=1 ").ToList();
            }
            else if (proRoleName == "ASPM")
            {
                nameList = _dbEntities.Database.SqlQuery<CmnUserModel>(@"select * from [CellPhoneProject].[dbo].[CmnUsers] where RoleName in ('ASPM','ASPMHEAD') and IsActive=1 ").ToList();
            }

            return nameList;
        }

        public List<TeamKpiPercentageListModel> GetCmYearlyKpi(string startValue, string endValue, string kpiRoles, string kpiRolePerson)
        {
            List<TeamKpiPercentageListModel> cmList = new List<TeamKpiPercentageListModel>();

            _dbEntities.Database.CommandTimeout = 6000;


            var startDate1 = startValue.Split('-');
            var startYear = startDate1[0].Trim();
            var startMonth = startDate1[1].Trim();
           
            var endDate1 = endValue.Split('-');
            var endYear = endDate1[0].Trim();
            var endMonth = endDate1[1].Trim();

            //
            int startMonth1 = Convert.ToInt32(startMonth);
            int startYear1 = Convert.ToInt32(startYear);

            int endMonth1 = Convert.ToInt32(endMonth);
            int endYear1 = Convert.ToInt32(endYear);

            #region When end date smaller than start date
            long yers;
            if (endMonth1 < startMonth1 && endYear1>startYear1)
            {
                #region 1 to End Date

                for (var mms = 1; mms <= endMonth1; mms++)
                {
                    yers = endYear1;

                    //Smart phone

                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                   
                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                   //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();

                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                   //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                }

                #endregion

                #region Start Date to 12
                for (var mms = startMonth1; mms <= 12; mms++)
                {
                    yers = startYear1;
                    //Smart phone
                   
                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                }//end for
                #endregion
            }
            #endregion

            #region When End Date greater than Start Date 2020-11 and 2019-09
            if (endMonth1 > startMonth1 && endYear1==startYear1)
            {
                yers = endYear1;
                for (var mms = startMonth1; mms <= endMonth1; mms++)
                {
                    //Smart phone

                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }

                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                }
            }//end if

            if (endMonth1 > startMonth1 && endYear1 > startYear1)
            {
             
                for (var mms = 1; mms <= endMonth1; mms++)
                {
                    yers = endYear1;
                    //Smart phone
                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }

                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                }//end for

                for (var mms = startMonth1; mms <= 12; mms++)
                {
                    yers = startMonth1;
                    //Smart phone
                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();

                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                }
            }
            #endregion

            #region When StartDate=EndDate and endDate> start date or endDate=start Date
            //end year=start year

            if (endMonth1 == startMonth1 && endYear1==startYear1)
            {
                yers = endYear1;
                for (var mms = startMonth1; mms <= endMonth1; mms++)
                {
                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                }
            }

            //end date> start date
            if (endMonth1 == startMonth1 && endYear1 > startYear1)
            {
                for (var mms = startMonth1; mms <= 12; mms++)
                {
                    yers = startYear1;
                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();

                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                }//end start year

                for (var mms = 1; mms <= endMonth1; mms++)
                {
                    yers = endYear1;
                    var proEventSmart = _dbEntities.GetMaterialArriveSmart(kpiRolePerson, mms, yers).ToList();
                    if (proEventSmart.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Smart Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();

                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Smart Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }

                    }
                    foreach (var project in proEventSmart)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Feature
                    var proEventFeature = _dbEntities.GetMaterialArriveFeature(kpiRolePerson, mms, yers).ToList();
                    if (proEventFeature.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Materials Arriving (Feature Phone)" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Materials Arriving (Feature Phone)";
                            items.Target1 = Convert.ToString(rrVal.Target);
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventFeature)
                    {

                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                    //Repeat order
                    var proEventRepeat = _dbEntities.GetRepeatOderKpi(kpiRolePerson, mms, yers).ToList();
                    if (proEventRepeat.Count == 0)
                    {
                        var rrVal =
                            (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Repeat Order" && tm.IsActive == true select tm)
                                .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Repeat Order";
                            items.Target1 = "100/80";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;
                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEventRepeat)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }

                    //Iqc
                    var proEvIqc = _dbEntities.GetMaterialPassChinaIqc(kpiRolePerson, mms, yers).ToList();
                    if (proEvIqc.Count == 0)
                    {
                        var rrVal =
                           (from tm in _dbEntities.TeamKpiPercentageLists where tm.KpiName == "Material Pass on CHN IQC" && tm.IsActive == true select tm)
                               .FirstOrDefault();
                        if (rrVal != null)
                        {
                            var items = new TeamKpiPercentageListModel();
                            items.KpiName = "Material Pass on CHN IQC";
                            items.Target1 = "--";
                            items.Weight = Convert.ToInt32(rrVal.Weight);
                            items.TotalList = 0;
                            items.TotalAverageAchievement = 0;
                            items.TotalAverageScore = 0;
                            items.TotalAverageScorePercent = 0;
                            items.EmployeeCode = kpiRolePerson;

                            cmList.Add(items);
                        }
                    }
                    foreach (var project in proEvIqc)
                    {
                        var items = new TeamKpiPercentageListModel();
                        items.KpiName = project.KpiName;
                        items.Target1 = project.Target1;
                        items.Weight = Convert.ToInt32(project.Weight);
                        items.TotalList = Convert.ToInt32(project.TotalList);
                        items.TotalAverageAchievement = project.TotalAverageAchievement;
                        items.TotalAverageScore = project.TotalAverageScore;
                        items.Month = project.Month;
                        items.MonthNum = mms;
                        items.Year = Convert.ToInt32(project.Year);
                        cmList.Add(items);
                    }
                }
            }

            #endregion

            #region 1 year kpi
            var newCmList = new List<TeamKpiPercentageListModel>();
            var items1 = new TeamKpiPercentageListModel();
            var items2 = new TeamKpiPercentageListModel();
            var items3 = new TeamKpiPercentageListModel();
            var items4 = new TeamKpiPercentageListModel();

            decimal smtMatTotalAc = 0;
            decimal smtMatTotalScore = 0;
            var weightSmrt = 0;
            var kpiNameSmrt = "";

            decimal fetrMatTotalAc = 0;
            decimal fetrMatTotalScore = 0;
            var weightFetr = 0;
            var kpiNameFert = "";

            decimal repeatAc = 0;
            decimal repeatScore = 0;
            var weightRepeat = 0;
            var kpiNameRepeat = "";

            decimal iqcPassAc=0;
            decimal iqcPassScore=0;
            var weightIqc = 0;
            var kpiNameIqc="";

            foreach (var cmListss in cmList)
            {
               
                if (cmListss.KpiName == "Materials Arriving (Smart Phone)")
                {
                    smtMatTotalAc += Convert.ToDecimal(cmListss.TotalAverageAchievement);
                    smtMatTotalScore += Convert.ToDecimal(cmListss.TotalAverageScore);

                    weightSmrt = cmListss.Weight;
                    kpiNameSmrt = cmListss.KpiName;

                }
               
                if (cmListss.KpiName == "Materials Arriving (Feature Phone)")
                {
                    fetrMatTotalAc += Convert.ToDecimal(cmListss.TotalAverageAchievement);
                    fetrMatTotalScore += Convert.ToDecimal(cmListss.TotalAverageScore);

                    weightFetr = cmListss.Weight;
                    kpiNameFert = cmListss.KpiName;

                }
               
                if (cmListss.KpiName == "Repeat Order")
                {
                    repeatAc += Convert.ToDecimal(cmListss.TotalAverageAchievement);
                    repeatScore += Convert.ToDecimal(cmListss.TotalAverageScore);

                    weightRepeat = cmListss.Weight;
                    kpiNameRepeat = cmListss.KpiName;

                }
               
                if (cmListss.KpiName == "Material Pass on CHN IQC")
                {
                    iqcPassAc += Convert.ToDecimal(cmListss.TotalAverageAchievement);
                    iqcPassScore += Convert.ToDecimal(cmListss.TotalAverageScore);

                    weightIqc = cmListss.Weight;
                    kpiNameIqc = cmListss.KpiName;

                }

            }

            var qq = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(@"SELECT DATEDIFF(month, {0}, {1})+1 as MonthDiff", startValue, endValue).ToList();
            var monDiff1 = 0;
            foreach (var monDiffs in qq)
            {
                monDiff1 = Convert.ToInt32(monDiffs.MonthDiff);
            }
             
            //Smart
            var totalSmartAchieve = smtMatTotalAc;
            var totalSmartScore = smtMatTotalScore;

            items1.KpiName = kpiNameSmrt;
            items1.Weight = weightSmrt;
            items1.YearKpiAchievement = Convert.ToDecimal(totalSmartAchieve / monDiff1);
            items1.YearKpiScore = Convert.ToDecimal(totalSmartScore/monDiff1);
            newCmList.Add(items1);

            // Fature
            var totalFeatureAchieve = fetrMatTotalAc;
            var totalFeatureScore = fetrMatTotalScore;

            items2.KpiName = kpiNameFert;
            items2.Weight = weightFetr;
            items2.YearKpiAchievement = Convert.ToDecimal(totalFeatureAchieve/monDiff1);
            items2.YearKpiScore = Convert.ToDecimal(totalFeatureScore/monDiff1);
            newCmList.Add(items2);

            // Repeat
            var totalRepeatAchieve = repeatAc;
            var totalRepeatScore = repeatScore;

            items3.KpiName = kpiNameRepeat;
            items3.Weight = weightRepeat;
            items3.YearKpiAchievement = Convert.ToDecimal(totalRepeatAchieve/monDiff1);
            items3.YearKpiScore = Convert.ToDecimal(totalRepeatScore/monDiff1);
            newCmList.Add(items3);

            //Iqc
            var totalIqcAchieve = iqcPassAc;
            var totalIqcScore = iqcPassScore;

            items4.KpiName = kpiNameIqc;
            items4.Weight = weightIqc;
            items4.YearKpiAchievement = Convert.ToDecimal(totalIqcAchieve/monDiff1);
            items4.YearKpiScore = Convert.ToDecimal(totalIqcScore/monDiff1);
            newCmList.Add(items4);

            #endregion

            TeamKpiPercentageListModel itemsTotal = new TeamKpiPercentageListModel();

            itemsTotal.KpiName = "Sub Total - A";
            itemsTotal.Target1 = "--";
            itemsTotal.Weight = newCmList.Sum(i => i.Weight);
            itemsTotal.TotalList = 0;
            itemsTotal.YearKpiAchievement = 0;
            itemsTotal.YearKpiScore = newCmList.Sum(i => i.YearKpiScore);
            newCmList.Add(itemsTotal);

            return newCmList;
        }

        public List<TeamKpiPercentageListModel> GetCmYearlyOthersKpi(string startValue, string endValue, string kpiRoles, string kpiRolePerson)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var kpiList = _dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(@"select tmm.KpiName,tmm.Weight,tmm.KpiFor,tmm.IsActive,krr.YearlyKpiAchievement,krr.YearlyKpiScore,krr.EmployeeCode
                from [CellPhoneProject].[dbo].[TeamKpiPercentageList] tmm
                left join [CellPhoneProject].[dbo].[KpiByTeamLeaderAndHr] krr on tmm.KpiName=krr.KpiName and krr.EmployeeCode='"+kpiRolePerson+"' where tmm.KpiFor='CM' and tmm.IsActive=1").ToList();

            var userLists = (from cu in _dbEntities.CmnUsers where cu.CmnUserId == userId && cu.IsActive == true select cu).FirstOrDefault();

            foreach (var kpis in kpiList)
            {

                if (userLists.RoleName == "CMHEAD")
                {
                    kpis.RoleName = "CM";
                }
                else if (userLists.RoleName == "ACCNT")
                {
                    kpis.RoleName = "HR";
                }
            }
         
            TeamKpiPercentageListModel itemsTotal = new TeamKpiPercentageListModel();

            itemsTotal.KpiName = "Sub Total - B";
            itemsTotal.Weight = kpiList.Sum(i => i.Weight);
            itemsTotal.KpiFor = "";
            itemsTotal.YearKpiAchievement = 0;
            itemsTotal.YearKpiAchievement = 0;
            itemsTotal.YearKpiScore =0;
            if (userLists.RoleName=="CMHEAD")
            {
                itemsTotal.RoleName = "CM";
            }
            else if (userLists.RoleName == "ACCNT")
            {
                itemsTotal.RoleName = "HR";
            }
           
            kpiList.Add(itemsTotal);

            return kpiList;
        }

        public string SaveKpiValueBData(List<TeamKpiPercentageListModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var userLists = (from cu in _dbEntities.CmnUsers where cu.CmnUserId == userId && cu.IsActive == true select cu).FirstOrDefault();

            foreach (var insResult in results)
            {
               // isExist = _projectCommercialRepository.GetSavedKpiData(results[0].EmployeeCode, results[0].KpiName);
                bool isExist = GetSavedKpiData(insResult.EmployeeCode, insResult.KpiName);

                if (isExist==false)
                {
                    if (insResult.KpiName != "Sub Total - B" && insResult.YearKpiAchievement != 0)
                    {
                        var model = new KpiByTeamLeaderAndHr();
                        model.KpiName = insResult.KpiName;
                        model.Weight = insResult.Weight;
                        model.YearlyKpiAchievement = insResult.YearKpiAchievement;
                        model.YearlyKpiScore = insResult.Weight * (insResult.YearKpiAchievement / 100);
                        model.EmployeeCode = insResult.EmployeeCode;
                        model.KpiFor = insResult.KpiFor;

                        if (userLists.RoleName == "CMHEAD")
                        {
                            model.KpiAddedBy = "CM";
                        }
                        else if (userLists.RoleName == "ACCNT")
                        {
                            model.KpiAddedBy = "HR";
                        }
                        model.Added = userId;
                        model.AddedDate = DateTime.Now;

                        _dbEntities.KpiByTeamLeaderAndHrs.AddOrUpdate(model);
                    }
                }
                else if (isExist)
                {
                    var query = _dbEntities.Database.SqlQuery<KpiByTeamLeaderAndHr>(@"
                         select * from [CellPhoneProject].[dbo].[KpiByTeamLeaderAndHr] where KpiName='" + insResult.KpiName + "' and EmployeeCode='" + insResult.EmployeeCode + "' and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) ").FirstOrDefault();
                    //foreach (var ss in query)
                    //{
                    //    ss.YearlyKpiAchievement = insResult.YearKpiAchievement;
                    //    ss.YearlyKpiScore = insResult.Weight * (insResult.YearKpiAchievement / 100);
                    //    ss.Updated = userId;
                    //    ss.UpdatedDate = DateTime.Now;
                    //    _dbEntities.KpiByTeamLeaderAndHrs.AddOrUpdate(ss);
                    //}
                    query.YearlyKpiAchievement = insResult.YearKpiAchievement;
                    query.YearlyKpiScore = insResult.Weight * (insResult.YearKpiAchievement / 100);
                    query.Updated = userId;
                    query.UpdatedDate = DateTime.Now;
                    _dbEntities.KpiByTeamLeaderAndHrs.AddOrUpdate(query);

                    _dbEntities.SaveChanges();
                }
               
            }
            _dbEntities.SaveChanges();

            return "ok";
        }

        public bool GetSavedKpiData(string employeeCode, string KpiName)
        {
            List<TeamKpiPercentageListModel> getIncentiveReports = null;
            if (employeeCode != null)
            {
//                string getIncentiveReportQuery = string.Format(@"select *  FROM [CellPhoneProject].[dbo].[KpiByTeamLeaderAndHr]
//                where EmployeeCode='" + employeeCode + "' and KpiName='" + KpiName + "' and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) and YearlyKpiAchievement =0 ", employeeCode, KpiName);

                string getIncentiveReportQuery = string.Format(@"select *  FROM [CellPhoneProject].[dbo].[KpiByTeamLeaderAndHr]
                where EmployeeCode='" + employeeCode + "' and KpiName='" + KpiName + "' and DATENAME(YEAR,AddedDate)=DATENAME(YEAR,GETDATE()) ", employeeCode, KpiName);

                getIncentiveReports =_dbEntities.Database.SqlQuery<TeamKpiPercentageListModel>(getIncentiveReportQuery).ToList();
            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }
        public FileContentResult GetProfilePicture(string uId)
        {
            string fileName;
            if (uId == null)
            {
                fileName = HttpContext.Current.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
            }
            else
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    var cmnUser = (from cu in dbEntities.CmnUsers
                                   where cu.EmployeeCode == uId
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

                    fileName = cmnUser != null
                        ? cmnUser.ProfilePictureUrl
                        : HttpContext.Current.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
                }
            }
            try
            {
                var fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                byte[] imageData = br.ReadBytes((int)imageFileLength);

                return new FileContentResult(imageData, "image/png");
            }
            catch (Exception)
            {
                fileName = HttpContext.Current.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
                var fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                byte[] imageData = br.ReadBytes((int)imageFileLength);
                return new FileContentResult(imageData, "image/png");
            }
        }

        #endregion
    }
}