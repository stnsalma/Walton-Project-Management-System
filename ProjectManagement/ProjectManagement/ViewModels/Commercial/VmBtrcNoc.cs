using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmBtrcNoc
    {
        public VmBtrcNoc()
        {
            ProjectBtrcNocModel = new ProjectBtrcNocModel();
            BtrcRawModel = new BtrcRawModel();
        }
        public ProjectBtrcNocModel ProjectBtrcNocModel { get; set; }
        public BtrcRawModel BtrcRawModel { get; set; }
    }
}