using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface IProjectMasterRepository:IRepository<ProjectMaster>
    {
    }
}
