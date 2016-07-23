using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Text;
using System.Web.Mvc;
using ViewWorld.Core;

namespace ViewWorld.Controllers
{
    public class BaseController : Controller
    {
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
                status = 200,
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
        protected JsonResult ErrorJson(string status, string msg)
        {
            var result = new
            {
                status = status,
                message = msg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected string UserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }
    }
}