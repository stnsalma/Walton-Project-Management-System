using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class Produced_UnProducedIMEIViewModel
    {
        public string ModelName { get; set; }
        public string Order { get; set; }
        public IList<ProjectMasterInv> Models { get; set; }
        public IList<ProjectMasterInv> Orders { get; set; }
        public IList<Produced_UnproducedIMEI> Produced_UnproducedIMEIs { get; set; }

    }
}