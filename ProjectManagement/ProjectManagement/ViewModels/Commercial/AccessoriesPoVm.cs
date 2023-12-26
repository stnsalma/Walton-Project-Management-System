using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class AccessoriesPoVm
    {
        public AccessoriesPoVm()
        {
            ChargerPoModels=new List<ChargerPoModel>();
            EarphonePoModels=new List<EarphonePoModel>();
        }
        public List<ChargerPoModel> ChargerPoModels { get; set; }
        public List<EarphonePoModel> EarphonePoModels { get; set; }
    }
}