using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class DashBoardCounterModel
    {

        public int ScreeningCounter { get; set; }
        public int RunningTestCounter { get; set; }
        public int FinishedGoodsCounter { get; set; }
        public int AfterSalesCounter { get; set; }
        public int HwReceivableCounter { get; set; }

        public int NewProject { get; set; }

        public int AssignedProject { get; set; }
        public int FinalApprovalPending { get; set; }
        public int InitialApprovalPending { get; set; }
        public int Rejected { get; set; }
        public int Completed { get; set; }
        public int TotalApproved { get; set; }

        public int PmRunningProjects { get; set; }
        public int SwotAnalysisPending { get; set; }
    
    }
}