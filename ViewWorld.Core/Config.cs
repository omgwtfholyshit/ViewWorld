using System;
using System.Configuration;
using System.Web;

namespace ViewWorld.Core
{
    internal sealed class Config
    {
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public static string logPath = HttpContext.Current.Request.PhysicalApplicationPath + "logs";
        /// <summary>
        /// 数据库连接IP
        /// </summary>
        public static string DB_HostIP { get; }
        public static int DB_Port { get; }
        public static string SMTPHost { get; }
        public static int SMTPPort { get; }
        public static string EmailAddress { get; }
        public static string EmailPass { get; }
        public static string QQAppId { get; }
        public static string QQAppKey { get; }
        public static string WechatAppId { get; }
        public static string WechatAppKey { get; }
        static Config()
        {
            DB_HostIP = ConfigurationManager.AppSettings["dbHostIP"];
            DB_Port = Convert.ToInt32(ConfigurationManager.AppSettings["dbPort"]);
            SMTPHost = ConfigurationManager.AppSettings["SmtpHost"];
            SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            EmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            EmailPass = ConfigurationManager.AppSettings["FormEmailPassword"];
            QQAppId = ConfigurationManager.AppSettings["QQAppId"];
            QQAppKey = ConfigurationManager.AppSettings["QQAppKey"];
            WechatAppId = ConfigurationManager.AppSettings["WechatAppId"];
            WechatAppKey = ConfigurationManager.AppSettings["WechatAppKey"];
        }
    }
}
