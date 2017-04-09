using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core;
using ViewWorld.Core.Models;
using ViewWorld.Core.Dal;
using System.Security.Claims;

namespace ViewWorld.Controllers
{
    public class BaseController : Controller
    {
        #region Json Override
        //Status Code: 200 正常, 300 通用错误, 301 验证码错误
        protected override JsonResult Json(object data, string contentType,
           Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected new JsonResult Json(Object data)
        {
            var data2 = new
            {
                status = "200",
                message = "",
                data = data
            };
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        protected JsonResult Json(string status, Object data)
        {
            var data2 = new
            {
                status = status,
                message = "",
                data = data
            };
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        protected JsonResult DropdownData(bool status, Object data)
        {
            var result = new
            {
                success = status,
                results = data
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected JsonResult PageJson(IPagedList data)
        {
            var pageData = new
            {
                PageCount = data.PageCount,
                PageIndex = data.PageNumber,
                PageSize = data.PageSize,
                Data = data
            };
            return Json(pageData);
        }

        protected JsonResult SuccessJson()
        {
            var result = new
            {
                status = "200"
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult ErrorJson(string msg)
        {
            var result = new
            {
                status = "300",
                message = msg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected JsonResult ErrorJson(int status, string msg)
        {
            var result = new
            {
                status = status,
                message = msg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected JsonResult OriginJson(Object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region public methods
        protected string UserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }
        protected string GetClaimValue(string claimType)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            foreach(var claim in claimsIdentity.Claims)
            {
                if (claim.Type == claimType)
                    return claim.Value;
            }
            return "";
        }
        protected void RemoveOutputCacheItem(string methodName, string controllerName)
        {
            var urlToRemove = Url.Action(methodName, controllerName);
            HttpResponse.RemoveOutputCacheItem(urlToRemove);
        }
        #endregion
    }
}