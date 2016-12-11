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
                var result = new JsonResult();
                result.Data = new Result() { ErrorCode = 300, Success = false, Message = "您没有权限执行该操作，请咨询您的管理员" };
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }

                
        }
    }
}
