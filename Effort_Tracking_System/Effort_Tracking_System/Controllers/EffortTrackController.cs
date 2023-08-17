using Effort_Tracking_System.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    //[CustomAuthorize]
    public class EffortTrackController : Controller
    {
        Effort_Tracking_SystemEntities db = new Effort_Tracking_SystemEntities();
        public ActionResult Index()
        {
            int userid = (int)Session["UserId"];
            ViewBag.Details = db.user_task_assignment.Where(a => a.user_id == userid);
            return View();

        }
    }
}