using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IMarketingRepository
    {
        List<MkProjectSpecModel> GetMkProjectSpecModels();
        MkProjectSpecModel GetMkProjectSpecModelById(long id);
        List<MkOtherBrandModelModel> GetMkOtherBrands();
        MkProjectSpecModel SaveMkProjectSpec(MkProjectSpecModel model);
        MkProjectSpecModel UpdateMkProjectSpec(MkProjectSpecModel model);
        MkMarketOrderQuantityDetailModel UpdateMarketOrderQuantityDetail(MkMarketOrderQuantityDetailModel model);
        MkMarketOrderQuantityDetailModel GetMarketOrderQuantityDetailById(long id);
        List<MkMarketOrderQuantityDetailModel> GetMarketOrderQuantityDetailModels();
        MkMarketOrderQuantityDetail SameOrderCheck(long? specId, int? orderno);
        MkMarketOrderQuantityDetailModel SaveMarketOrderQuantityDetail(MkMarketOrderQuantityDetailModel model);
    }
}