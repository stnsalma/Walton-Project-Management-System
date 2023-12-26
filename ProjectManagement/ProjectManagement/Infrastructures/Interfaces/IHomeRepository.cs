using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface IHomeRepository
    {
        #region CommnonUserRouteIdentifier

        Tuple<String, String> GetUserRedirectionDetailsAfterAuthentication();
        #endregion

        int AuthorizedUserByUserNamePassword(String userName, String password, Boolean rememberMe);
        #region UserCreation

        long CreateUser(CmnUserModel user);
        bool DeleteUser(CmnUser user);
        void ResetPassword(CmnUser user);


        bool UpdateUser(CmnUserModel user);
        CmnUser GetUserByUserName(string username);

        #endregion

        bool ChagePassword(CmnUserModel model);
        string CheckUserNameExist(string userName);
        List<string> GetAllRoles();
        CmnUserModel GetUser(long userId);
        string CheckOldPassword(long userId, string password);
        List<CmnUserModel> GetAllUser();
    }
}
