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

        public ActionResult ReportIndex(int? year, int? month, int? day, int? user, string export, int page = 1)
        {
            try
            {
                int pageSize = 5;
                ViewBag.SuccessMessage = TempData["ExportMessage"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                bool isAdmin = Session["UserRole"].ToString().ToLower() == "admin";
                int currentUserId = (int)Session["UserId"];
                var query = _dbContext.Efforts.AsQueryable();

                if (year.HasValue)
                    query = query.Where(e => e.date_time.HasValue && e.date_time.Value.Year == year);
                if (month.HasValue)
                    query = query.Where(e => e.date_time.HasValue && e.date_time.Value.Month == month);
                if (day.HasValue)
                    query = query.Where(e => e.date_time.HasValue && e.date_time.Value.Day == day);
                if (!isAdmin)
                {
                    query = query.Where(e => e.Assign_Task.user_id == currentUserId);
                }
                else if (user.HasValue)
                {
                    query = query.Where(e => e.Assign_Task.user_id == user);
                }
                var reportData = query.Where(e => e.status == "Approved").OrderByDescending(e => e.date_time).ToList();
                
                int total = reportData.Count;
                reportData = reportData.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.PageNumber = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
                if (export == "csv")
                {
                    return ExportToCsv(reportData);
                }
                ViewBag.ShowUserDropdown = isAdmin;
                ViewBag.Users = _dbContext.Users.ToList();
                ViewBag.SelectedYear = year;
                ViewBag.SelectedMonth = month;
                ViewBag.SelectedDay = day;
                ViewBag.SelectedUser = user;

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
                if (data.Count > 0 && data != null)
                {
                    var csvBuilder = new StringBuilder();
                    string filePath = ConfigurationManager.AppSettings["PathToSaveOutputFiles"];
                    if (!System.IO.File.Exists(filePath))
                    {
                        csvBuilder.AppendLine("Date,Project,Task,Shift,Hours");
                    }
                    foreach (var item in data)
                    {
                        var formatDate = $"\t{item.date_time.Value.ToString("yyyy-MM-dd")}";
                        var formatStartTime = item.Assign_Task.Shift.start_time;
                        var formatEndTime = item.Assign_Task.Shift.end_time;
                        csvBuilder.AppendLine($"{formatDate},{item.Assign_Task.Project.name},{item.Assign_Task.Task.task_name},{formatStartTime}-{formatEndTime},{item.hours_worked}");
                    }
                    System.IO.File.AppendAllText(filePath, csvBuilder.ToString(), Encoding.UTF8);
                    TempData["ExportMessage"] = "Exporting to a file was done.";
                    return RedirectToAction("ReportIndex", new { year = Request.QueryString["year"], month = Request.QueryString["month"], day = Request.QueryString["day"], user = Request.QueryString["user"] });
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot export to file as there are no records.";
                    return RedirectToAction("ReportIndex", new { year = Request.QueryString["year"], month = Request.QueryString["month"], day = Request.QueryString["day"], user = Request.QueryString["user"] });
                }
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
