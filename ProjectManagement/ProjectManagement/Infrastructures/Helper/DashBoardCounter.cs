using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Helper
{
    public class DashBoardCounter
    {
        private readonly CellPhoneProjectEntities _dbeEntities;

        public DashBoardCounter()
        {
            _dbeEntities = new CellPhoneProjectEntities();

        }

        public DashBoardCounterModel GetDashBoardCounter(string controllerName, string roleName, long userId)
        {
            //var getDashBoardCounter = new DashBoardCounterModel();


            if (controllerName == "ProjectManager" && roleName == "PMHEAD")
            {


                var query = String.Format(@"select 
(select count(*)   from ProjectMasters where ProjectStatus ='APPROVED'  and (IsProjectManagerAssigned=0 or IsProjectManagerAssigned is null) and (select count(*) from  [dbo].[ProjectPurchaseOrderForms] where ProjectMasterId = ProjectMasters.ProjectMasterId) > 0) as NewProject,
(select Count(1)
from ProjectMasters pm left join   ProjectPurchaseOrderForms  po on  pm.ProjectMasterId =po.ProjectMasterId
left join ProjectPmAssigns ppa 
on pm.ProjectMasterId =ppa.ProjectMasterId
where pm.IsProjectManagerAssigned = 1 and po.PurchaseOrderNumber is not null and ppa.Status in ('ASSIGNED') and ppa.ProjectManagerUserId > 0) as AssignedProject,
ScreeningCounter=0,
RunningTestCounter=0,
FinishedGoodsCounter=0,
AfterSalesCounter=0");
                var getDashBoardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();


                return getDashBoardCounter;

            }
            if (controllerName == "Commercial" && roleName == "CM")
            {
                var query = string.Format(@"select 
                                                  (
                                                   select count(*) from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId 
                                                   where pm.ProjectStatus in ('PARTIAL') and hqia.TestPhase in ('FINISHED')
                                                  ) AS FinalApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('NEW') 
                                                  ) AS InitialApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('REJECTED') AND PsApprovalBy IS NOT NULL
                                                  ) AS Rejected,
                                                  (
                                                   select count(*) from  ProjectPurchaseOrderForms ppf
                                                   inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                                   where ppf.IsCompleted=1
                                                  ) AS Completed,
                                                  (
                                                   select count(*) from ProjectMasters pm inner join ProjectPurchaseOrderForms ppof
												   on pm.ProjectMasterId=ppof.ProjectMasterId where pm.ProjectStatus in ('APPROVED') 
												   and ppof.IsCompleted=0 and pm.IsActive=1
                                                  ) AS TotalApproved,
												  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('SWOTPENDING') and pm.IsActive=1
                                                  ) AS SwotAnalysisPending");
                var getDashBoardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();

                return getDashBoardCounter;

            }
            if (controllerName == "Hardware" && roleName == "HWHEAD")
            {
                var query = String.Format(@"select 
                                                (select count(*) from HwQcInchargeAssigns where IsScreeningTest=1 and TestPhase not in ('FINISHED','SAMPLESENT')) as ScreeningCounter, " +
                                               "(select count(*) from HwQcInchargeAssigns where IsRunningTest=1 and TestPhase not in ('FINISHED','SAMPLESENT')) as RunningTestCounter, " +
                                               "(select count(*) from HwQcInchargeAssigns where IsFinishedGoodTest=1 and TestPhase not in ('FINISHED','SAMPLESENT')) as FinishedGoodsCounter, "+
                                               "(select count(*) from HwQcInchargeAssigns where  TestPhase in ('SAMPLESENT')) as HwReceivableCounter");

                var getDashBoardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();

                return getDashBoardCounter;

            }
            if (controllerName == "HardWare" && roleName == "HW")
            {
                var query = String.Format(@"select
                                                            (select count(*) from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                            where hqa.HwQcUserId={0} and hqia.IsScreeningTest=1 and hqa.Status not in ('QCPASSED','QCSUBMITTED','FORWARDED')) as ScreeningCounter,
                                                            (select count(*) from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                            where hqa.HwQcUserId={0} and hqia.IsRunningTest=1 and hqa.Status not in ('QCPASSED','QCSUBMITTED','FORWARDED')) as RunningTestCounter,
                                                            (select count(*) from HwQcAssigns hqa 
                                                            inner join HwQcInchargeAssigns hqia on hqa.HwQcInchargeAssignId=hqia.HwQcInchargeAssignId
                                                            where hqa.HwQcUserId={0} and hqia.IsFinishedGoodTest=1 and hqa.Status not in ('QCPASSED','QCSUBMITTED','FORWARDED')) as FinishedGoodsCounter", userId);

                var getDashBoardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();

                return getDashBoardCounter;

            }
            if (controllerName == "Software" && roleName == "QCHEAD")
            {
                var query = String.Format(@"select 
                                                (select count(*) from HwQcInchargeAssigns where IsScreeningTest=1 and TestPhase in ('NEW','TECHNICIAN')) as ScreeningCounter, " +
                                          "(select count(*) from HwQcInchargeAssigns where IsRunningTest=1 and TestPhase in ('NEW','TECHNICIAN')) as RunningTestCounter, " +
                                          "(select count(*) from HwQcInchargeAssigns where IsFinishedGoodTest=1 and TestPhase in ('NEW','TECHNICIAN')) as FinishedGoodsCounter");

                var getDashBoardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();

                return getDashBoardCounter;
            }
            if (controllerName == "Management" && roleName == "MM")
            {
                var query = string.Format(@"select 
                                                  (
                                                   select count(*) from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId 
                                                   where pm.ProjectStatus in ('PARTIAL') and hqia.TestPhase in ('FINISHED')
                                                  ) AS FinalApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('NEW') AND CeoApprovalBy IS NOT NULL AND BiApprovalBy IS NOT NULL AND PsApprovalBy IS NOT NULL
                                                  ) AS InitialApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('REJECTED') AND PsApprovalBy IS NOT NULL
                                                  ) AS Rejected,
                                                  (
                                                   select count(*) from  ProjectPurchaseOrderForms ppf
                                                   inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                                   where ppf.IsCompleted=1
                                                  ) AS Completed,
                                                  (
                                                   select count(*) from ProjectMasters pm inner join ProjectPurchaseOrderForms ppof
												   on pm.ProjectMasterId=ppof.ProjectMasterId where pm.ProjectStatus in ('APPROVED') 
												   and ppof.IsCompleted=0 and pm.IsActive=1
                                                  ) AS TotalApproved,
												  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('SWOTPENDING') and pm.IsActive=1
                                                  ) AS SwotAnalysisPending");
                var getMmDashboardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();
                return getMmDashboardCounter;
            }
            if (controllerName == "Management" && roleName == "CEO")
            {
                var query = string.Format(@"select 
                                                  (
                                                   select count(*) from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId 
                                                   where pm.ProjectStatus in ('PARTIAL') and hqia.TestPhase in ('FINISHED')
                                                  ) AS FinalApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('NEW') AND CeoApprovalBy IS NULL AND BiApprovalBy IS NOT NULL AND PsApprovalBy IS NOT NULL
                                                  ) AS InitialApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('REJECTED') AND CeoApprovalBy IS NULL
                                                  ) AS Rejected,
                                                  (
                                                   select count(*) from  ProjectPurchaseOrderForms ppf
                                                   inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                                   where ppf.IsCompleted=1
                                                  ) AS Completed,
                                                  (
                                                   select count(*) from ProjectMasters pm inner join ProjectPurchaseOrderForms ppof
												   on pm.ProjectMasterId=ppof.ProjectMasterId where pm.ProjectStatus in ('APPROVED') 
												   and ppof.IsCompleted=0 and pm.IsActive=1
                                                  ) AS TotalApproved,
												  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('SWOTPENDING') and pm.IsActive=1
                                                  ) AS SwotAnalysisPending");
                var getMmDashboardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();
                return getMmDashboardCounter;
            }
            if (controllerName == "Management" && roleName == "PS")
            {
                var query = string.Format(@"select 
                                                  (
                                                   select count(*) from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId 
                                                   where pm.ProjectStatus in ('PARTIAL') and hqia.TestPhase in ('FINISHED')
                                                  ) AS FinalApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('NEW') AND PsApprovalBy IS NULL AND BiApprovalBy IS NOT NULL
                                                  ) AS InitialApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('REJECTED')
                                                  ) AS Rejected,
                                                  (
                                                   select count(*) from  ProjectPurchaseOrderForms ppf
                                                   inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                                   where ppf.IsCompleted=1
                                                  ) AS Completed,
                                                  (
                                                   select count(*) from ProjectMasters pm inner join ProjectPurchaseOrderForms ppof
												   on pm.ProjectMasterId=ppof.ProjectMasterId where pm.ProjectStatus in ('APPROVED') 
												   and ppof.IsCompleted=0 and pm.IsActive=1
                                                  ) AS TotalApproved,
												  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('SWOTPENDING') and pm.IsActive=1
                                                  ) AS SwotAnalysisPending");
                var getMmDashboardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();
                return getMmDashboardCounter;
            }
            if (controllerName == "Management" && roleName == "BIHEAD")
            {
                var query = string.Format(@"select 
                                                  (
                                                   select count(*) from ProjectMasters pm inner join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId 
                                                   where pm.ProjectStatus in ('PARTIAL') and hqia.TestPhase in ('FINISHED')
                                                  ) AS FinalApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('NEW') AND BiApprovalBy IS NULL
                                                  ) AS InitialApprovalPending,
                                                  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('REJECTED')
                                                  ) AS Rejected,
                                                  (
                                                   select count(*) from  ProjectPurchaseOrderForms ppf
                                                   inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                                   where ppf.IsCompleted=1
                                                  ) AS Completed,
                                                  (
                                                   select count(*) from ProjectMasters pm inner join ProjectPurchaseOrderForms ppof
												   on pm.ProjectMasterId=ppof.ProjectMasterId where pm.ProjectStatus in ('APPROVED') 
												   and ppof.IsCompleted=0 and pm.IsActive=1
                                                  ) AS TotalApproved,
												  (
                                                   select count(*) from ProjectMasters pm  where pm.ProjectStatus in ('SWOTPENDING') and pm.IsActive=1
                                                  ) AS SwotAnalysisPending");
                var getMmDashboardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();
                return getMmDashboardCounter;
            }
            if (controllerName == "ProjectManager" && roleName == "PM")
            {
                var query = String.Format(@"select
                                                (select count(*) from ProjectPmAssigns where  ProjectManagerUserId = {0}) as PmRunningProjects", userId);

                var getDashBoardCounter = _dbeEntities.Database.SqlQuery<DashBoardCounterModel>(query).FirstOrDefault();

                return getDashBoardCounter;

            }

            return new DashBoardCounterModel();

        }


    }
}