﻿using CacheManager.Core;
using SRVTextToImage;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Enum;

namespace ViewWorld.Utils
{
    public class ValidationHelper
    {
        #region Constructor
        private static ValidationHelper client = new ValidationHelper();
        static ValidationHelper()
        {
        }
        ValidationHelper()
        {
        }
        public static ValidationHelper Instance
        {
            get
            {
                return client;
            }
        }
        #endregion
        private static string GetCaptchaName(CaptchaType type)
        {
            string captchaName = "";
            switch (type)
            {
                case CaptchaType.Login:
                    captchaName = "LoginCaptcha";
                    break;
                case CaptchaType.Mobile:
                    captchaName = "MobileCaptcha";
                    break;
                case CaptchaType.Email:
                    captchaName = "EmailCaptcha";
                    break;
                default:
                    captchaName = "invalid";
                    break;
            }
            return captchaName;
        }
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public static FileResult GenerateCaptchaImage(ICacheManager<object> cacheManager,string region, int width, int height, Color textColor, Color backColor, CaptchaType type)
        {
            MemoryStream stream = new MemoryStream();
            CaptchaRandomImage CI = new CaptchaRandomImage();
            Random rand = new Random(DateTime.Now.Millisecond);
            var randNum = rand.Next(10000, 99999);
            string captchaName = GetCaptchaName(type);
            cacheManager.AddOrUpdate(captchaName, region, randNum.ToString(), value => { return randNum; });
            CI.GenerateImage(randNum.ToString(), width, height, textColor, backColor);
            CI.Image.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            CI.Dispose();
            return new FileStreamResult(stream, "image/png");
        }
        public static bool ValidateCaptcha(HttpSessionStateBase session,string captcha, CaptchaType type)
        {
            string sessionName = GetCaptchaName(type);
            if (session[sessionName] !=null && session[sessionName].ToString() == captcha)
            {
                session.Remove(sessionName);
                return true;
            }
            return false;
        }
        public static bool ValidateCaptcha(ICacheManager<object> cacheManager,string captcha,string region,CaptchaType type)
        {
            string captchaName = GetCaptchaName(type);
            if (cacheManager.Get(captchaName, region) != null && cacheManager.Get(captchaName, region).ToString() == captcha)
            {
                cacheManager.Remove(captchaName, region);
                return true;
            }
            return false;
        }
        public static bool IsCaptchaRequired(HttpRequestBase request)
        {
            var cache = HttpRuntime.Cache;
            var userIP = request.UserHostAddress + "_loginIP";
            bool requireCaptcha = false;
            if (cache[userIP] == null)   //如果缓存中没有该用户IP
            {
                cache.Add(userIP, 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.BelowNormal, null);
            }
            else
            {
                int ipCount = Convert.ToInt32(cache[userIP].ToString());
                int currentCount = ipCount + 1;
                if (currentCount < 6)
                {
                    cache.Insert(userIP, currentCount, null, DateTime.MaxValue, TimeSpan.FromMinutes(15));
                }
                else
                {
                    cache.Insert(userIP, currentCount, null, DateTime.MaxValue, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.BelowNormal, null);
                    requireCaptcha = true;
                }
            }
            return requireCaptcha;
        }

        public async Task<string> SendToMobileAsync(string mobileNumber,string content)
        {
            string data = "mobile=" + mobileNumber + "&content=" + content;

            return await Task.Factory.StartNew(() => VerificationSender.Instance.HttpGet(VerificationSender.sendURL, data));
        }
        public void ClearSessionAsync(string name , HttpSessionStateBase session, int timer = 60)
        {
            Thread.Sleep(timer * 1000);
            if (session[name] != null)
            {
                session[name] = null;
                session.Remove(name);
            }
        }
    }
}
