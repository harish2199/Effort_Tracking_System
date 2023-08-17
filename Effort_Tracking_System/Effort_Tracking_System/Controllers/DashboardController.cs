using Effort_Tracking_System.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    [CustomAuthorize]
    public class DashboardController : Controller
    {
        Effort_Tracking_SystemEntities db = new Effort_Tracking_SystemEntities();
        public ActionResult Dashboard()
        {
            int userid = (int)Session["UserId"];
            var assignments = db.user_task_assignment
              .Where(a => a.user_id == userid)
              .Select(a => new
              {
                  a.project_id,
                  a.project.name,
                  a.task_id,
                  a.task.task_name
              })
             .ToList();



            ViewBag.Details = assignments;
            ViewBag.Projects = db.projects.ToList();
            return View();
        }
        public ActionResult GetTaskDetails(int taskId)
        {
            var assignment = db.user_task_assignment
              .Where(a => a.task_id == taskId).FirstOrDefault();



            if (assignment != null)
            {
                var shift = db.shifts.Find(assignment.shift_id);



                if (shift != null)
                {
                    return Json(new { shiftId = shift.shift_id, shiftName = shift.shift_name, hours = assignment.hours }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { shiftId = -1, hours = assignment.hours }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { shiftId = -1, hours = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}