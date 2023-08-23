using log4net;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HomeController));
        private readonly Effort_Tracking_SystemEntities _dbContext = new Effort_Tracking_SystemEntities();

        public ActionResult Login()
        {
            ViewBag.LoginFail = TempData["LoginFailureMessage"] as string;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email ,string password)
        {
            try
            {
                var currentUser = _dbContext.Users.FirstOrDefault(u => u.email == email && u.password == password);
                var currentAdmin = _dbContext.Admins.FirstOrDefault(a => a.email == email && a.password == password);

                if (currentUser != null)
                {
                    Session["UserId"] = currentUser.user_id;
                    Session["UserName"] = currentUser.first_name + " " + currentUser.last_name;
                    Session["UserEmail"] = currentUser.email;
                    Session["UserRole"] = "user";
                    TempData["LoginSuccessMessage"] = "Welcome, " + currentUser.first_name + " " + currentUser.last_name + "! You have successfully logged in.";

                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else if (currentAdmin != null)
                {
                    Session["UserId"] = currentAdmin.admin_id;
                    Session["UserName"] = currentAdmin.name;
                    Session["UserEmail"] = currentAdmin.email;
                    Session["UserRole"] = currentAdmin.Role.ToLower();
                    TempData["LoginSuccessMessage"] = "Welcome, " + currentAdmin.name + "! You have successfully logged in.";

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    TempData["LoginFailureMessage"] = "Login failed. Please check your credentials.";
                    return RedirectToAction("Login" , "Home");
                }
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred during login.", ex);
                ViewBag.LoginStatusMessage = "An error occurred during login.";
                return View("Login");
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                ViewBag.SuccessMessage = "You have been logged out.";
                return View("Login");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred during logout.", ex);
                ViewBag.ErrorMessage = "An error occurred during logout.";
                return View("Login");
            }
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult Error()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View();
        }
    }
}
