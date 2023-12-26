using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Management
{
    public class WorkProgressData
    {
        public WorkProgressData()
        {
            data = new List<int>();
        }
        public string name   { get; set; }
        public List<int> data { get; set; } 
    }
}