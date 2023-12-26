using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using SignalRDemo.DAL;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class ProjectMasterRepository:Repository<ProjectMaster>, IProjectMasterRepository
    {
        public ProjectMasterRepository(CellPhoneProjectEntities context) : base(context)
        {
        }
    }
}