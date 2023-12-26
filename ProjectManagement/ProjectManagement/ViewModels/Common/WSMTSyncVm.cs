using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class WSMTSyncVm
    {
        public long  SelectedHandset { get; set; }
        public IList<WSMTHandset> WSMTHandsets { get; set; }
        public string RBSYModel { get; set; }
        public string ProductionType { get; set; }
        public string OrderNo { get; set; }
    }
}