using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels
{
    public class BdIqcBomPassViewModel
    {
        public BdIqcBomPassViewModel()
        {
            BdIqcModel=new BdIqcModel();
            BdIqcBomPassRecordModels=new List<BdIqcBomPassRecordModel>();
            BdIqcBomPassRecordModel=new BdIqcBomPassRecordModel();
        }

        public BdIqcModel BdIqcModel { get; set; }
        public List<BdIqcBomPassRecordModel> BdIqcBomPassRecordModels { get; set; }
        public BdIqcBomPassRecordModel BdIqcBomPassRecordModel { get; set; }
    }
}