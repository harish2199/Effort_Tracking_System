using Effort_Tracking_System.Attributes;
using log4net;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    [UserAuthorize]
    public class DashboardController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DashboardController));
        private readonly Effort_Tracking_SystemEntities _dbContext = new Effort_Tracking_SystemEntities();

        public ActionResult Dashboard()
        {
            try
            {
                ViewBag.LoginSuccess = TempData["LoginSuccessMessage"] as string;
                int userId = (int)Session["UserId"];
                
                // Checking if task completed or not
                var assignedTask = _dbContext.Assign_Task
                    .Where(a => a.user_id == userId && a.Status == "Pending")
                    .OrderByDescending(a => a.assignmentdate)
                    .FirstOrDefault();
                if (assignedTask != null)
                {
                    int? totalHoursWorked = _dbContext.Efforts
                        .Where(e => e.assign_task_id == assignedTask.assign_task_id && e.status == "Approved")
                        .Sum(e => (int?)e.hours_worked);

                    if (totalHoursWorked >= assignedTask.allocated_hours)
                    {
                        assignedTask.Status = "Completed";
                        _dbContext.SaveChanges();
                    }
                }

                var task = _dbContext.Assign_Task
                    .Where(a => a.user_id == userId && a.Status == "Pending")
                    .OrderByDescending(a => a.assignmentdate)
                    .FirstOrDefault();
                // Completed efforts
                ViewBag.PreviousReports = _dbContext.Efforts
                    .Where(a => a.Assign_Task.user_id == userId && a.status == "Approved")
                    .OrderByDescending(a => a.date_time)
                    .Take(7)
                    .ToList();
                //SendEmailToAdmin(userId);
                return View(task);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while loading data for the dashboard.", ex);
                TempData["ErrorMessage"] = $"An error occurred while loading data for the dashboard. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        private void SendEmailToAdmin(int? userId)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harishdasu18@gmail.com", "Gmailpassword"),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("harishdasu18@gmail.com"),
                Subject = "New Effort Submission",
                Body = $"A new effort has been submitted by user {userId}."
            };

            mailMessage.To.Add("satyaharish18@gmail.com"); 

            smtpClient.Send(mailMessage);
        }
    }
}
