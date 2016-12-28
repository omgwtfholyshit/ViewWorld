using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ViewWorld.Models;
using ViewWorld.Utils;
using ViewWorld.Core.Models;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using ViewWorld.Core.Enum;
using System.Drawing;
using MongoDB.Driver;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Models.Identity;

namespace ViewWorld.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        #region 初始化
        private readonly IMongoDbRepository Repo;
        private ApplicationUserManager _userManager;
        public AccountController(IMongoDbRepository _repo)
        {
            Repo = _repo;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private SignInHelper _signinManager;

        private SignInHelper SignInManager
        {
            get
            {
                if (_signinManager == null)
                {
                    _signinManager = new SignInHelper(UserManager, AuthenticationManager);
                }
                return _signinManager;
            }
        }
        #endregion
        #region 默认方法
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // 这不会计入到为执行帐户锁定而统计的登录失败次数中
            // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        if (User.IsInRole(UserRole.Admin) || User.IsInRole(UserRole.Sales))
                        {
                            returnUrl = "/Page/Index";
                        }
                        else
                        {
                            returnUrl = "/";
                        }
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "无效的登录尝试。");
                    return View(model);
            }
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 要求用户已通过使用用户名/密码或外部登录名登录
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 以下代码可以防范双重身份验证代码遭到暴力破解攻击。
            // 如果用户输入错误代码的次数达到指定的次数，则会将
            // 该用户帐户锁定指定的时间。
            // 可以在 IdentityConfig 中配置帐户锁定设置
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代码无效。");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NickName = string.Format("新用户_{0}", Tools.Generate_Nickname()),
                    RegisteredAt = DateTime.Now,
                    Sex = SexType.Unknown,
                    Avatar = "/Images/DefaultImages/UnknownSex.jpg",
                    Points = 0
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, UserRole.User);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                    // 发送包含此链接的电子邮件
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 请不要显示该用户不存在或者未经确认
                    return View("ForgotPasswordConfirmation");
                }

                // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                // 发送包含此链接的电子邮件
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "重置密码", "请通过单击 <a href=\"" + callbackUrl + "\">此处</a>来重置你的密码");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // 请不要显示该用户不存在
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 生成令牌并发送该令牌
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 如果用户没有帐户，则提示该用户创建帐户
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return SuccessJson();
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion
        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
        #region 自定义登录相关方法
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserLogin(LoginViewModel model, string returnUrl)
        {
            #region 判定提交值是否合法
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var error = ModelState[key].Errors;
                    if (error.Count() > 0)
                    {
                        return ErrorJson(error[0].ErrorMessage);
                    }
                }
            }
            #endregion
            if (!ValidationHelper.IsCaptchaRequired(Request) || ValidationHelper.ValidateCaptcha(Session, model.VerificationCode, CaptchaType.Login))
            {
                try
                {
                    // 这不会计入到为执行帐户锁定而统计的登录失败次数中
                    // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
                    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            if (string.IsNullOrWhiteSpace(returnUrl))
                            {
                                if (User.IsInRole("管理员") || User.IsInRole("销售"))
                                {
                                    return Json("/Page/Index");
                                }else
                                {
                                    return Json("/Home/Index");
                                }
                            }
                            return Json(returnUrl);
                        case SignInStatus.LockedOut:
                            return Json("/Account/Lockout");
                        case SignInStatus.RequiresTwoFactorAuthentication:
                            return ErrorJson(301, "请输入验证码");
                        case SignInStatus.Failure:
                        default:
                            return ErrorJson("用户名或密码错误");
                    }
                }catch(TimeoutException)
                {
                    return ErrorJson("服务器内部错误，请稍候再试");
                }
                
            }
            Session["RequireCaptcha"] = "1";
            return ErrorJson(301,"验证码错误");
        }

        [AllowAnonymous]
        [HttpGet]
        public FileResult GetLoginCaptcha()
        {
            return ValidationHelper.GenerateCaptchaImage(Session,160,45,Color.DodgerBlue,Color.White,CaptchaType.Login);
        }
        [AllowAnonymous]
        [HttpGet]
        public FileResult GetMobileCaptcha()
        {
            return ValidationHelper.GenerateCaptchaImage(Session, 160, 45, Color.LimeGreen, Color.White, CaptchaType.Mobile);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetMobileVerificationCode(string mobileNumber)
        {
            if (Tools.isChineseMobile(mobileNumber))
            {
                if (Session["MobileTimer"] == null)
                {
                    string code = Tools.Generate_MobileCode();
                    Session["MobileTimer"] = "Sent";
                    Session["MobileCaptcha"] = code;
                    Session["MobileSubmitted"] = mobileNumber;
                    string content = string.Format("尊敬的会员，您的短信验证码为：{0}（20分钟有效）诚立业祝您生活愉快", code);
                    ValidationHelper.SendToMobile(mobileNumber, content);
                    Task.Factory.StartNew(() => ValidationHelper.Instance.ClearSession("MobileTimer", Session));
                    return SuccessJson();
                }

                return ErrorJson("发送速度太快,请稍候再试");
            }
            return ErrorJson("号码不正确");
        }
        #endregion
        #region 自定义获取用户信息
        [HttpGet]
        [OutputCache(Location =System.Web.UI.OutputCacheLocation.Server,Duration =1200)]
        public async Task<JsonResult> GetUserInfo()
        {
            var Result = await Repo.GetOneAsync<ApplicationUser>(this.UserId);
            if (!System.IO.File.Exists(Result.Entity.Avatar))
                Result.Entity.Avatar = "/Images/DefaultImages/UnknownSex.jpg";
            var data = new
            {
                Username = Result.Entity.UserName,
                Nickname = Result.Entity.NickName,
                Avatar = Result.Entity.Avatar,
            };
            return Json(data);
        }
        #endregion
        #region 自定义修改用户信息
        [HttpPost]
        public async Task<ActionResult> UploadUserAvatar()
        {
            AvatarUploadResult result = new AvatarUploadResult()
            {
                avatarUrls = new ArrayList(),
                success = false,
            };
            try
            {
                HttpPostedFileBase file;
                string[] avatars = new string[1] { "__avatar1" };
                int avatar_number = 1;
                int avatars_length = avatars.Length;

                for (int i = 0; i < avatars_length; i++)
                {
                    file = Request.Files[avatars[i]];
                    string savePath = string.Format("/Upload/User/{0}/Avatar/", this.UserId);

                    if (!Directory.Exists(Server.MapPath(savePath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(savePath));
                    }
                    string imagePath = string.Format("/Upload/User/{0}/Avatar/{1}", this.UserId, "Head.jpg");
                    var updateDef = Builders<ApplicationUser>.Update.Set("Avatar", imagePath);
                    await Repo.UpdateOneAsync(this.UserId, updateDef);
                    result.avatarUrls.Add(imagePath);
                    imagePath = Server.MapPath(imagePath);
                    file.SaveAs(imagePath);
                    ImageHelper.MakeThumbnail(imagePath, imagePath.Replace(".jpg", "_thumb.jpg"), 56, 56);
                    /*
                     *	可在此将 virtualPath 储存到数据库，如果有需要的话。
                     *	Save to database...
                     */
                    avatar_number++;
                }
                //upload_url中传递的额外的参数，如果定义的method为get请将下面的context.Request.Form换为context.Request.QueryString
                result.success = true;
                result.msg = "Success!";
                //返回图片的保存结果（返回内容为json字符串，可自行构造，该处使用Newtonsoft.Json构造）
                RemoveOutputCacheItem("GetUserInfo","Account");
                return Content(JsonConvert.SerializeObject(result));
            }
            catch (Exception e)
            {
                result.msg = e.Message;
                return Content(JsonConvert.SerializeObject(result));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUserInfo(UpdateUserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDef = Builders<ApplicationUser>.Update.Set("NickName", model.NickName).Set("DOB", model.DOB).Set("Sex", model.Sex);
                var Result = await Repo.UpdateOneAsync(this.UserId, updateDef);
                if (Result.Success)
                {
                    RemoveOutputCacheItem("GetUserInfo", "Account");
                    return SuccessJson();
                }
                return ErrorJson(Result.Message);
            }
            return ErrorJson("保存失败,请检查您的输入");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUserMobile(UpdateUserNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ValidationHelper.ValidateCaptcha(Session, model.VerificationCode, CaptchaType.Mobile))
                {
                    if(Session["MobileSubmitted"] == null || Session["MobileSubmitted"].ToString() != model.Mobile)
                    {
                        return ErrorJson("申请的手机号和接受验证码的手机号不匹配");
                    }
                    UpdateDefinition<ApplicationUser> updateDef;
                    if (model.SetAsUserName)
                    {
                        updateDef =  Builders<ApplicationUser>.Update.Set("UserName", model.Mobile).Set("PhoneNumber", model.Mobile).Set("PhoneNumberConfirmed",true);
                    }else
                    {
                        updateDef = Builders<ApplicationUser>.Update.Set("PhoneNumber", model.Mobile).Set("PhoneNumberConfirmed", true);
                    }
                    var Result = await Repo.UpdateOneAsync(this.UserId, updateDef);
                    if (Result.Success)
                    {
                        RemoveOutputCacheItem("GetUserInfo", "Account");
                        return SuccessJson();
                    }else
                    {
                        return ErrorJson(Result.Message);
                    }
                }
                return ErrorJson("验证码错误");
            }
            return ErrorJson("保存失败,请检查您的输入");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUserPassword(EditPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.OldPassword == model.Password)
                {
                    return ErrorJson("新密码不能和旧密码一样");
                }
                var Result = await UserManager.ChangePasswordAsync(this.UserId, model.OldPassword, model.Password);
                if (Result.Succeeded)
                {
                    return SuccessJson();
                }else
                {
                    return ErrorJson("密码错误");
                }
            }
            return ErrorJson("两次密码输入不匹配");
        }
        #endregion        
    }
}