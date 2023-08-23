using Effort_Tracking_System.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    [UserAuthorize]
    public class EffortTrackController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(EffortTrackController));
        private readonly Effort_Tracking_SystemEntities _dbContext = new Effort_Tracking_SystemEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                int userId = (int)Session["UserId"];
                ViewBag.UserId = userId;
                var latestTaskAssignment = _dbContext.Assign_Task
                    .Where(a => a.user_id == userId && a.Status == "Pending")
                    .OrderByDescending(a => a.assignmentdate)
                    .FirstOrDefault();

                ViewBag.Shifts = _dbContext.Shifts.ToList();
                return View(latestTaskAssignment);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while loading data for the effort track.", ex);
                TempData["ErrorMessage"] = "An error occurred while loading data for the effort track.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult SubmitEffort(Effort submittedEffort)
        {
            try
            {
                int userId = (int)Session["UserId"];
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Model not valid!";
                    return RedirectToAction("Index");
                }

                // Check for leaves
                var approvedLeaves = _dbContext.Leaves.Where(l => l.user_id == userId && l.status == "Approved");
                foreach (var leave in approvedLeaves)
                {
                    if (leave.date == submittedEffort.date_time)
                    {
                        TempData["ErrorMessage"] = "You cannot submit the effort because you are on leave.";
                        return RedirectToAction("Index");
                    }
                }

                //One effort per day
                string targetDate = DateTime.Now.ToString("yyyy-MM-dd");
                var effortperday = _dbContext.Efforts.Where(e => e.Assign_Task.user_id == userId && e.date_time.ToString() == targetDate).ToList();
                if(effortperday.Count() > 0)
                {
                    TempData["ErrorMessage"] = "You can only submit one effort per day.";
                    return RedirectToAction("Index");
                }

                _dbContext.Efforts.Add(submittedEffort);
                _dbContext.SaveChanges();
                
                TempData["SuccessMessage"] = "Effort submitted to Admin.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while submitting effort.", ex);
                TempData["ErrorMessage"] = $"An error occurred while submitting effort.  {ex}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult SubmitLeave(Leave submittedLeave)
        {
            try
            {
                int userId = (int)Session["UserId"];
                if (!ModelState.IsValid)
                {
                    return View("Index");
                }

                string targetDate = submittedLeave.date.ToString("yyyy-MM-dd");
                var effortperday = _dbContext.Efforts.Where(e => e.Assign_Task.user_id == userId && e.date_time.ToString() == targetDate).ToList();
                if (effortperday.Count() > 0)
                {
                    TempData["SuccessMessage"] = "You cannot submit for leave today because you submitted effort today";
                    return RedirectToAction("Index");
                }

                var leaveperday = _dbContext.Leaves.Where(e => e.user_id == userId && e.date.ToString() == targetDate).ToList();
                if (leaveperday.Count() > 0)
                {
                    TempData["SuccessMessage"] = "You cannot submit for leave today because you already submitted one";
                    return RedirectToAction("Index");
                }

                _dbContext.Leaves.Add(submittedLeave);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Leave submitted to Admin.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while submitting leave.", ex);
                TempData["ErrorMessage"] = "An error occurred while submitting leave.";
                return RedirectToAction("Index");
            }
        }

        
    }
}
