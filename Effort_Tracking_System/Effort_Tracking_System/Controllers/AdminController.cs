using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Effort_Tracking_System.Attributes;
using log4net;

namespace Effort_Tracking_System.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));
        Effort_Tracking_SystemEntities db = new Effort_Tracking_SystemEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                ViewBag.Users = db.users.ToList();
                ViewBag.Projects = db.projects.ToList();
                ViewBag.Tasks = db.tasks.ToList();
                ViewBag.shifts = db.shifts.ToList();
                ViewBag.Efforts = db.efforts.Where(e => e.status == "Completed").ToList();
                ViewBag.Unavailability = db.unavailabilities.Where(e => e.status == "not approved").ToList();
                return View();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while loading data for the index page.", ex);
                TempData["ErrorMessage"] = "An error occurred while loading data for the index page.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult UserAction(user user, string action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (action == "create")
                    {
                        if (!ModelState.IsValid)
                        {
                            return View();
                        }
                        var existingUser = db.users.FirstOrDefault(u => u.email == user.email);
                        if (existingUser != null)
                        {
                            TempData["ErrorMessage"] = "user already exsists.";
                            return RedirectToAction("index", "Admin");
                        }
                        db.users.Add(user);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "User created successfully.";
                        return RedirectToAction("index", "Admin");
                    }
                    else if (action == "update")
                    {
                        var existingUser = db.users.Find(user.user_id);
                        if (existingUser != null)
                        {
                            existingUser.first_name = user.first_name;
                            existingUser.last_name = user.last_name;
                            existingUser.designation = user.designation;
                            existingUser.email = user.email;
                            existingUser.password = user.password;
                        }
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "User updated successfully.";
                        return RedirectToAction("index", "Admin");
                    }
                }
                return RedirectToAction("index", "Admin");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while creating or updating.", ex);
                TempData["ErrorMessage"] = "An error occurred while creating or updating.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddTasks(user_task_assignment task)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                db.user_task_assignment.Add(task);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Task added successfully.";
                return RedirectToAction("index", "Admin");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while adding a task.", ex);
                TempData["ErrorMessage"] = "An error occurred while adding a task.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ApproveEffort(int effortId)
        {
            try
            {
                var effortToUpdate = db.efforts.Find(effortId);
                if (effortToUpdate != null && effortToUpdate.status == "Completed")
                {
                    effortToUpdate.status = "Approved";
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Effort approved successfully.";
                return RedirectToAction("index", "Admin");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while approving an effort.", ex);
                TempData["ErrorMessage"] = "An error occurred while approving an effort.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ApproveLeave(int unavailabilityid)
        {
            try
            {
                var leave = db.unavailabilities.Find(unavailabilityid);
                if (leave != null && leave.status == "Pending")
                {
                    leave.status = "Approved";
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Leave approved successfully.";
                return RedirectToAction("index", "Admin");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while approving leave.", ex);
                TempData["ErrorMessage"] = "An error occurred while approving leave.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
