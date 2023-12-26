using System.Web;
using System.Web.Mvc;
using ProjectManagement.Infrastructures.Helper;

namespace ProjectManagement
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MemberAuthorization());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
