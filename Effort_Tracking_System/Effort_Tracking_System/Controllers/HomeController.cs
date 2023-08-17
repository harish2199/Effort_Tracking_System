using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Effort_Tracking_System.Controllers
{
    public class HomeController : Controller
    {
        Effort_Tracking_SystemEntities db = new Effort_Tracking_SystemEntities();
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(user user)
        {
            var currentUser = db.users.FirstOrDefault(u => u.email == user.email && u.password == user.password);
            var currentAdmin = db.admins.FirstOrDefault(a => a.email == user.email && a.password == user.password);

            if (currentUser != null)
            {
                Session["UserId"] = currentUser.user_id;
                Session["UserName"] = currentUser.first_name + " " + currentUser.last_name;
                Session["UserEmail"] = currentUser.email;
                Session["UserRole"] = "User";
                ViewBag.LoginStatusMessage = "Welcome, " + currentUser.first_name + " " + currentUser.last_name + "! You have successfully logged in.";

                return RedirectToAction("Dashboard", "Dashboard"); 
            }
            else if (currentAdmin != null)
            {
                Session["UserId"] = currentAdmin.admin_id;
                Session["UserName"] = currentAdmin.name;
                Session["UserEmail"] = currentAdmin.email;
                Session["UserRole"] = currentAdmin.Role;
                ViewBag.LoginStatusMessage = "Welcome, " + currentAdmin.name + "! You have successfully logged in.";

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.LoginStatusMessage = "Login failed. Please check your credentials.";
                return View("Login");
            }
        }



        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();

            ViewBag.ErrorMessage = "You have been logged out.";

            return View("Login");
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