using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IIqcRepository
    {
        List<BomModel> GetBomByProjectModel(string projectName);
        List<ProjectOrderQuantityDetailModel> GetVariantsByProjectId(long id);
        ProjectOrderQuantityDetailModel GetVariantById(long id);
        List<ProjectOrderQuantityDetailModel> GetAllVariants();
        List<BdIqcBomPassRecordModel> GetBdIqcBomPassRecordByVariantId(long id);
        List<ForeignIqcBomPassRecordModel> GetForeignIqcBomPassRecordByVariantId(long id);
        List<ForeignIqcBomPassRecordModel> GetForeignIqcBomPassRecordByForIqcId(long id);
        BdIqcModel GetBdIqcByVariantId(long id);
        ForeignIqcModel GetForeignIqcByVariantId(long id);
        ForeignIqcModel GetForIqcByVariantIdAndInspNo(long? variantid, string insno);
        BdIqcModel SaveBdIqc(BdIqcModel model);
        ForeignIqcModel SaveForeignIqc(ForeignIqcModel model);
        void SaveBdIqcBomPassRecords(List<BdIqcBomPassRecordModel> model);
        void SaveForeignIqcBomPassRecords(List<ForeignIqcBomPassRecordModel> model);
        List<ForeignIqcModel> GetForeignIqcModels();
    }
}
