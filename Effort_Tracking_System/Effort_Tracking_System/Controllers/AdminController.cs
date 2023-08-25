using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Effort_Tracking_System.Attributes;
using log4net;

namespace Effort_Tracking_System.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AdminController));
        private readonly Effort_Tracking_SystemEntities _dbContext = new Effort_Tracking_SystemEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.LoginSuccess = TempData["LoginSuccessMessage"] as string;
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                ViewBag.Users = _dbContext.Users.ToList();
                ViewBag.Projects = _dbContext.Projects.ToList();
                ViewBag.Tasks = _dbContext.Tasks.ToList();
                ViewBag.Shifts = _dbContext.Shifts.ToList();
                ViewBag.Efforts = _dbContext.Efforts.Where(e => e.status == "Pending").ToList();
                ViewBag.leaves = _dbContext.Leaves.Where(e => e.status == "Pending").ToList();
                var users = _dbContext.Users;
                var simplifiedUsers = users.Select(u => new
                {
                    u.user_id,
                    u.first_name,
                    u.last_name,
                    u.designation,
                    u.email,
                    u.password
                }).ToList();
                ViewBag.UserDetails = simplifiedUsers;
                ViewBag.shiftChanges = _dbContext.Shift_Change.Where(e => e.status == "Pending").ToList();
                return View();
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while loading data for the index page.", ex);
                TempData["ErrorMessage"] = $"An error occurred while loading data for the index page. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult UserAction(User user, string action)
        {
            try
            {
                if (action == "create")
                {
                    if (!ModelState.IsValid)
                    {
                        return PartialView("~/Views/Partials/_CreateUserModal.cshtml", user);
                    }
                    var existingUser = _dbContext.Users.FirstOrDefault(u => u.email == user.email);
                    if (existingUser != null)
                    {
                        TempData["ErrorMessage"] = "User already exists.";
                        return RedirectToAction("Index", "Admin");
                    }
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "User created successfully.";
                    return RedirectToAction("Index", "Admin");
                }
                else if (action == "update")
                {
                    if (!ModelState.IsValid)
                    {
                        return PartialView("~/Views/Partials/_UpdateUserModal.cshtml", user);
                    }
                    var existingUser = _dbContext.Users.Find(user.user_id);
                    if (existingUser != null)
                    {
                        existingUser.first_name = user.first_name;
                        existingUser.last_name = user.last_name;
                        existingUser.designation = user.designation;
                        existingUser.email = user.email;
                        existingUser.password = user.password;
                    }
                    _dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while creating or updating.", ex);
                TempData["ErrorMessage"] = $"An error occurred while creating or updating. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddTasks(Assign_Task task)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("~/Views/Partials/_AssignTaskModal.cshtml", task);
                }

                var previousTask = _dbContext.Assign_Task.Where(t => t.user_id == task.user_id && t.Status == "Pending")
                                                        .OrderByDescending(t => t.assignmentdate)
                                                        .FirstOrDefault();
                if (previousTask != null)
                {
                    TempData["ErrorMessage"] = "This user already assigned with one task.";
                    return RedirectToAction("Index", "Admin");
                }

                _dbContext.Assign_Task.Add(task);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Task added successfully.";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while adding a task.", ex);
                TempData["ErrorMessage"] = $"An error occurred while adding a task. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ApproveEffort(int effortid)
        {
            try
            {
                var effortToUpdate = _dbContext.Efforts.Find(effortid);
                if (effortToUpdate != null)
                {
                    effortToUpdate.status = "Approved";
                    _dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Effort approved successfully.";
                    return RedirectToAction("Index", "Admin");
                }
                TempData["ErrorMessage"] = "An error occurred while approving an effort.";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while approving an effort.", ex);
                TempData["ErrorMessage"] = $"An error occurred while approving an effort.{ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ApproveLeave(int leaveid, string action)
        {
            try
            {
                var leave = _dbContext.Leaves.Find(leaveid);
                if (leave != null && leave.status == "Pending")
                {
                    leave.status = action;
                    _dbContext.SaveChanges();
                }

                TempData["SuccessMessage"] = "Leave " + action;
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while processing leave.", ex);
                TempData["ErrorMessage"] = $"An error occurred while processing leave.{ex}";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ShiftChange(int shiftChangeId, string approvalStatus)
        {
            try
            {
                int userId = (int)Session["UserId"];
                var shiftChange = _dbContext.Shift_Change.Find(shiftChangeId);
                if (shiftChange != null && shiftChange.status == "Pending")
                {
                    shiftChange.status = approvalStatus; 
                    _dbContext.SaveChanges();
                }
                TempData["SuccessMessage"] = $"Shift change {approvalStatus.ToLower()} successfully.";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _log.Error($"An error occurred while {approvalStatus.ToLower()} shift change.", ex);
                TempData["ErrorMessage"] = $"An error occurred while {approvalStatus.ToLower()} shift change.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
