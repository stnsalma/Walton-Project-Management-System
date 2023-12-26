using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface ISpareRepository
    {
        #region GET
        List<ProjectPurchaseOrderFormModel> GetProjectsWithPo();
        List<ProjectMasterModel> GetAllProjectNamesWithPo();
        List<SpareNameModel> GetSpareNameModels(string sparetype);
        List<SpareNameModel> GetAllSpareNames();
        SpareNameModel GetSpareNameById(long spareId);
        List<SpareOrderModel> GetSpareOrderByPorjectId(long projectId);
        SpareOrderModel GetLastSapreOrder(string projectName, string orderNumber);
        ProjectPurchaseOrderFormModel GetProjectPurchaseOrderFormById(long projectid);
        List<ProjectMasterModel> GetOrderNumbersByProjectNameWithPo(string projectname);
        List<SpareOrderByMultipleModelModel> GetSpareOrderByMultipleModelModels();
        #endregion

        #region SET
        void SaveSpareOrder(SpareOrderModel model);
        SpareOrderByMultipleModelModel SaveSpareOrderByMultipleModels(SpareOrderByMultipleModelModel model);
        SpareNameModel SaveSpareName(SpareNameModel model);

        #endregion
        #region UPDATE

        void SubmitSpareOrderToCommercial(long projectid, string pidate, string remark, long spareSubmittedBy);
        int UpdateSpareOrder(long spareorderId, long spareId, string spareName, string quantity, string pir, string remarks, long userId);
        bool UpdateSpareName(SpareNameModel spare);

        #endregion
    }
}
