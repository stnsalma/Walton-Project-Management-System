using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Events.Calendar;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Infrastructures.Helper
{
    class Dpc : DayPilotCalendar
    {
        protected override void OnInit(InitArgs e)
        {
            //var db = new CellPhoneProjectEntities();
            //Events = from ev in db.events select ev;

            //DataIdField = "id";
            //DataTextField = "text";
            //DataStartField = "eventstart";
            //DataEndField = "eventend";

            //Update();
        }
    } 
}