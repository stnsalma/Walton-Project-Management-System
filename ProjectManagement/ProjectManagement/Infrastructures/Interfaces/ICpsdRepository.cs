using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface ICpsdRepository
    {
        List<ServiceToSalesRatioWarningMailModel> GetServiceToSalesRatioWarningMailModels();
        ServiceToSalesRatioWarningMailModel GetServiceToSalesRatioWarningMailModelById(long id);

        void UpdateServiceToSalesRatioMonitor(string solution, string submittedBy, long id);
    }
}