using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class CustomSelectListItemRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public CustomSelectListItemRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
        }

        public List<SelectListItem> GetModelListForTac(long id)
        {
            var selectListItems = new List<SelectListItem>();

            var tacModels = (from projectMaster in _dbEntities.ProjectMasters
                             where
                                 projectMaster.IsActive && projectMaster.ProjectStatus == "APPROVED" &&
                                 !(_dbEntities.BabtRaws.Any(i => i.ProjectMasterId == projectMaster.ProjectMasterId))
                             select new { projectMaster.ProjectMasterId, projectMaster.ProjectName, projectMaster.OrderNuber }).ToList();
            
            selectListItems.AddRange(tacModels.Select(tacModel => new SelectListItem
            {
                Value = tacModel.ProjectMasterId.ToString(CultureInfo.InvariantCulture), Text = tacModel.ProjectName + " (" + CommonConversion.AddOrdinal(tacModel.OrderNuber) + " Order)"
            }));
            if (id > 0)
            {
                var babtRaw = _dbEntities.BabtRaws.FirstOrDefault(i => i.BabtRawId == id);
                if (babtRaw != null)
                {
                    var projectId = babtRaw.ProjectMasterId;
                    var pMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
                    if (pMaster != null)
                        selectListItems.Add(new SelectListItem{Value = pMaster.ProjectMasterId.ToString(CultureInfo.InvariantCulture), Text = pMaster.ProjectName});
                }
            }
            return selectListItems.OrderBy(i=>i.Text).ToList();
        }
    }
}