using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using AutoMapper;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models.Common;

namespace ProjectManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            //RouteTable.Routes.MapHubs(new HubConfiguration(){EnableCrossDomain = true});
            DependencyInjection();
        }
        private void DependencyInjection()
        {
            //Code for registering our repository class and DI
            //Use Scoped Life cycle as it creates new objects on Every request
            //Transient on the other hand is used with webApi in IIS hosting 
            //http://simpleinjector.readthedocs.io/en/latest/lifetimes.html
            //http://stackoverflow.com/questions/22574899/transient-vs-per-webrequest-lifestyle-what-constitutes-a-web-request
            var container = new Container();
             container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            #region CommercialController
            container.Register<ICommercialRepository, CommercialRepository>(Lifestyle.Transient);
            // container.Register<ICommercialService, CommercialService>(Lifestyle.Scoped);
            #endregion
            #region HardwareController
            container.Register<IHardwareRepository, HardwareRepository>(Lifestyle.Transient);
            //   container.Register<IHardwareService, HardwareService>(Lifestyle.Scoped);
            #endregion
            //repository registration  Example
            #region HomeController
            //Transient lifestyle is created foe every contoller is made on Aplication life cycle.
            container.Register<IHomeRepository, HomeRepository>(Lifestyle.Transient);
            //Scope life cycle is created on every web request.
            //   container.Register<IHomeService, HomeService>(Lifestyle.Scoped);
            #endregion

            #region ManagementController
            container.Register<IManagementRepository, ManagementRepository>(Lifestyle.Transient);
            //container.Register<IManagementService, ManagementService>(Lifestyle.Scoped);
            #endregion
            #region ProjectManagerController
            container.Register<IProjectManagerRepository, ProjectManagerRepository>(Lifestyle.Transient);
            // container.Register<IProjectManagerService, ProjectManagerService>(Lifestyle.Scoped);
            #endregion
            #region SoftwareController
            container.Register<ISoftwareRepository, SoftwareRepository>(Lifestyle.Transient);
            // container.Register<ISoftwareService, SoftwareService>(Lifestyle.Scoped);
            #endregion

            #region CommonController
          //  container.Register<ICommonRepository, CommonRepository>(Lifestyle.Scoped);
            #endregion

            #region ProjectCommercialController
            container.Register<IProjectCommercialRepository, ProjectCommercialRepository>(Lifestyle.Transient);
            #endregion
        //    container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            //To verify life styles of injection
            

            //Optional
           container.Verify();
            
           
            DependencyResolver.SetResolver(
                new SimpleInjectorDependencyResolver(container));
        }
        private void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id =
                            (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Get the stored user-data, in this case, our roles
                        string userData = ticket.UserData;
                        string[] roles = userData.Split(',');
                        // string[] roles = {"User"};
                        HttpContext.Current.User = new GenericPrincipal(id, roles);
                    }
                }
            }
        }
    }
}
