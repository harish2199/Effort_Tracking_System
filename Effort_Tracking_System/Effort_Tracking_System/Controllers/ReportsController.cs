using Effort_Tracking_System.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    [CustomAuthorize]
    public class ReportsController : Controller
    {
        Effort_Tracking_SystemEntities db = new Effort_Tracking_SystemEntities();
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }
        public ActionResult ReportIndex(int? year, int? month, int? day, string export)
        {
            int userid = (int)Session["UserId"];
            var query = db.efforts.AsQueryable().ToList();
            if (year.HasValue)
                query = query.Where(e => e.date_time.Year == year).ToList();
            if (month.HasValue)
                query = query.Where(e => e.date_time.Month == month).ToList();
            if (day.HasValue)
                query = query.Where(e => e.date_time.Day == day).ToList();
            //return View(query.ToList());
            var reportData = query.Where(e => e.user_id == userid && e.status == "Approved").ToList();
            if (export == "csv")
                return ExportToCsv(reportData);
            return View(reportData);
        }
        private ActionResult ExportToCsv(List<effort> data)
        {
            var csvBuilder = new StringBuilder();
            string filePath = @"C:\Users\vdasu2\Desktop\Assignments\Task-4\Outputs\report.csv";
            if (!System.IO.File.Exists(filePath))
            {
                csvBuilder.AppendLine("Date,Project,Task,Shift,Hours");
            }
            foreach (var item in data)
            {
                var formatDate = $"\t{item.date_time:dd-MM-yyyy}";
                var formatStartTime = item.shift.start_time.ToString(@"hh\:mm");
                var formatEndTime = item.shift.end_time.ToString(@"hh\:mm");
                csvBuilder.AppendLine($"{formatDate},{item.project.name},{item.task.task_name},{formatStartTime} to {formatEndTime},{item.hours_worked}");
            }
            System.IO.File.AppendAllText(filePath, csvBuilder.ToString(), Encoding.UTF8);
            TempData["ExportMessage"] = "Exporting to a file was done.";
            return RedirectToAction("ReportIndex", new { year = Request.QueryString["year"], month = Request.QueryString["month"], day = Request.QueryString["day"] });
        }
    }
}