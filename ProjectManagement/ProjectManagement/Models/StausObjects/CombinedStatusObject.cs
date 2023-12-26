using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class CombinedStatusObject
    {
        public CombinedStatusObject ()
        {
           
            HwScreeningStatusObject=new HwScreeningStatusObject();
            HwRunningStatusObject=new HwRunningStatusObject();
            HwFinishedStatusObject=new HwFinishedStatusObject();
            CmStatusObject=new CmStatusObject();
            PmStatusObject=new PmStatusObject();
            SwStatusObject=new SwStatusObject();
            SwStatusObjects=new List<SwStatusObject>();
            CmStatusObjects=new List<CmStatusObject>();
            HwTestObjects=new List<HwTestObject>();
        }

        
        public HwScreeningStatusObject HwScreeningStatusObject { get; set; }
        public HwRunningStatusObject HwRunningStatusObject { get; set; }
        public HwFinishedStatusObject HwFinishedStatusObject { get; set; }
        public CmStatusObject CmStatusObject { get; set; }
        public PmStatusObject PmStatusObject { get; set; }
        public SwStatusObject SwStatusObject { get; set; }
        public List<SwStatusObject> SwStatusObjects { get; set; }
        public List<CmStatusObject> CmStatusObjects { get; set; }
        public List<HwTestObject> HwTestObjects { get; set; }
    }
}