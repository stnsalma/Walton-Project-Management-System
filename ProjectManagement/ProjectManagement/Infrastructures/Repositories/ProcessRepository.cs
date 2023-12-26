using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class ProcessRepository:IProcessRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;

        public ProcessRepository()
        {
            _dbeEntities=new CellPhoneProjectEntities();
        }
    }
}