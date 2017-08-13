using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Dal;

namespace ViewWorld.Core.Models.Identity
{
    public class RequirePermission: AuthorizeAttribute
    {
        public string Permission { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            ViewWorldPrincipal principal = new ViewWorldPrincipal(httpContext.User.Identity.GetUserId());
            bool IsAuthrized = string.IsNullOrWhiteSpace(Permission) || principal.HasPermission(Permission) || principal.HasPermission("FullAccess");
            if (!IsAuthrized)
                httpContext.Response.StatusCode = 401;
            return IsAuthrized;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 401)
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("/Account/Login");
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Home/PermissionRequired");
                }
                
            }

                
        }
    }
}
