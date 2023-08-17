using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Effort_Tracking_System.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the user's session is authenticated
            return httpContext.Session["UserId"] != null &&
                   httpContext.Session["UserRole"].ToString() == "User";
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect to login page if unauthorized
            filterContext.Result = new RedirectResult("~/Home/Unauthorized");
        }
    }
}