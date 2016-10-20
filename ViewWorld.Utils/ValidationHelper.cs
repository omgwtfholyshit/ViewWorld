using SRVTextToImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewWorld.Core.Enum;

namespace ViewWorld.Utils
{
    public class ValidationHelper
    {
        private static string GetSessionName(CaptchaType type)
        {
            string sessionName = "";
            switch (type)
            {
                case CaptchaType.Login:
                    sessionName = "LoginCaptcha";
                    break;
                case CaptchaType.Mobile:
                    sessionName = "MobileCaptcha";
                    break;
                default:
                    sessionName = "default";
                    break;
            }
            return sessionName;
        }
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public static FileResult GenerateCaptchaImage(HttpSessionStateBase session, int width, int height, Color textColor, Color backColor, CaptchaType type)
        {
            MemoryStream stream = new MemoryStream();
            CaptchaRandomImage CI = new CaptchaRandomImage();
            try
            {
                
                Random rand = new Random(DateTime.Now.Millisecond);
                var randNum = rand.Next(10000, 99999);
                string sessionName = GetSessionName(type);
                session[sessionName] = randNum;
                CI.GenerateImage(session[sessionName].ToString(), width, height, textColor, backColor);
                CI.Image.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(stream, "image/png");
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                stream.Dispose();
                CI.Dispose();
            }
            
        }
        public static bool ValidateCaptcha(HttpSessionStateBase session,string captcha, CaptchaType type)
        {
            string sessionName = GetSessionName(type);
            if (session[sessionName] !=null && session[sessionName].ToString() == captcha)
            {
                session.Remove(sessionName);
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

        public static string SendToMobile(string mobileNumber,string content)
        {
            string data = "mobile=" + mobileNumber + "&content=" + content;
            return VerificationSender.Instance.HttpGet(VerificationSender.sendURL,data);
        }
    }
}
