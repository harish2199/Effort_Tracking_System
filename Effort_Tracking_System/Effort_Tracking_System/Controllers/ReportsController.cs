using Effort_Tracking_System.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    [CommonAuthorize]
    public class ReportsController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ReportsController));
        private readonly Effort_Tracking_SystemEntities _dbContext = new Effort_Tracking_SystemEntities();

        public ActionResult ReportIndex(int? year, int? month, int? day, int? user, string export)
        {
            try
            {
                ViewBag.SuccessMessage = TempData["ExportMessage"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                bool isAdmin = Session["UserRole"].ToString().ToLower() == "admin";
                int currentUserId = (int)Session["UserId"];
                var query = _dbContext.Efforts.AsQueryable();

                if (year.HasValue && year != -1)
                    query = query.Where(e => e.date_time.HasValue && e.date_time.Value.Year == year);

                if (month.HasValue && month != -1)
                    query = query.Where(e => e.date_time.HasValue && e.date_time.Value.Month == month);

                if (day.HasValue && day != -1)
                    query = query.Where(e => e.date_time.HasValue && e.date_time.Value.Day == day);

                if (!isAdmin)
                {
                    query = query.Where(e => e.Assign_Task.user_id == currentUserId);
                }
                else if (user.HasValue && user != -1)
                {
                    query = query.Where(e => e.Assign_Task.user_id == user);
                }

                var reportData = query.Where(e => e.status == "Approved").ToList();

                if (year == null && month == null && day == null && user == null)
                {
                    var mostRecentReport = reportData.OrderByDescending(e => e.date_time).FirstOrDefault();
                    if (mostRecentReport != null)
                    {
                        reportData = new List<Effort> { mostRecentReport };
                    }
                }

                if (export == "csv")
                {
                    return ExportToCsv(reportData);
                }
                ViewBag.ShowUserDropdown = isAdmin;
                ViewBag.Users = _dbContext.Users.ToList();

                ViewBag.SelectedYear = year ?? -1;
                ViewBag.SelectedMonth = month ?? -1;
                ViewBag.SelectedDay = day ?? -1;
                ViewBag.SelectedUser = user ?? -1;

                return View(reportData);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while generating the report.", ex);
                TempData["ErrorMessage"] = $"An error occurred while generating the report. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        private ActionResult ExportToCsv(List<Effort> data)
        {
            try
            {
                var csvBuilder = new StringBuilder();
                string filePath = ConfigurationManager.AppSettings["PathToSaveOutputFiles"];
                if (!System.IO.File.Exists(filePath))
                {
                    csvBuilder.AppendLine("Date,Project,Task,Shift,Hours");
                }
                foreach (var item in data)
                {
                    var formatDate = $"\t{item.date_time:dd-MM-yyyy}";
                    var formatStartTime = item.Assign_Task.Shift.start_time.ToString(@"hh\:mm");
                    var formatEndTime = item.Assign_Task.Shift.end_time.ToString(@"hh\:mm");
                    csvBuilder.AppendLine($"{formatDate},{item.Assign_Task.Project.name},{item.Assign_Task.Task.task_name},{formatStartTime} to {formatEndTime},{item.hours_worked}");
                }
                System.IO.File.AppendAllText(filePath, csvBuilder.ToString(), Encoding.UTF8);
                TempData["ExportMessage"] = "Exporting to a file was done.";
                return RedirectToAction("ReportIndex", new { year = Request.QueryString["year"], month = Request.QueryString["month"], day = Request.QueryString["day"], user = Request.QueryString["user"] });
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while exporting the report to CSV.", ex);
                TempData["ErrorMessage"] = $"An error occurred while exporting the report to CSV. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
