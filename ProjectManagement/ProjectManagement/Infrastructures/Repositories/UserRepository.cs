using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public UserRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }


        public CmnUserModel GetUser(long id)
        {
            CmnUser user = GenereticRepo<CmnUser>.GetById(_dbEntities, id);
            try
            {
                CmnUserModel userModel = GenericMapper<CmnUser, CmnUserModel>.GetDestination(user);
                return userModel;
            }
            catch (Exception)
            {
                return new CmnUserModel();
            }
        }

        public List<CmnUserModel> GetAllUsers()
        {
            List<CmnUser> cmnUsers = GenereticRepo<CmnUser>.GetList(_dbEntities);
            List<CmnUserModel> cmnUserModels = GenericMapper<CmnUser, CmnUserModel>.GetDestinationList(cmnUsers);
            return cmnUserModels;
        }

        public List<CmnUserModel> GetUserByRole(string role)
        {
            throw new NotImplementedException();
        }

        public long SaveUser(CmnUserModel model)
        {
            throw new NotImplementedException();
        }

        public long UpdateUser(CmnUserModel model)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(long id)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllRole()
        {
            throw new NotImplementedException();
        }
    }
}