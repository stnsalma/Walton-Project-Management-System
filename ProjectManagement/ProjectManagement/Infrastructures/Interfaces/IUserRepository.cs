using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface IUserRepository
    {
        #region MostafizurRahman
        CmnUserModel GetUser(long id);
        List<CmnUserModel> GetAllUsers();
        List<CmnUserModel> GetUserByRole(string role);
        long SaveUser(CmnUserModel model);
        long UpdateUser(CmnUserModel model);
        bool DeleteUser(long id);
        List<string> GetAllRole();
        #endregion





    }
}
