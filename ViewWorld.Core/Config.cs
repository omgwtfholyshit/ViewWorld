using System;
using System.Configuration;
using System.Web;

namespace ViewWorld.Core
{
    public class Config
    {
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public static string logPath = HttpContext.Current.Request.PhysicalApplicationPath + "logs";
        /// <summary>
        /// 数据库连接IP
        /// </summary>
        public static string DB_HostIP { get; private set; }
        public static int DB_Port { get; private set; }
        public static string SMTPHost { get; private set; }
        public static int SMTPPort { get; private set; }
        public static string EmailAddress { get; private set; }
        public static string EmailPass { get; set; }
        static Config()
        {
            DB_HostIP = ConfigurationManager.AppSettings["dbHostIP"];
            DB_Port = Convert.ToInt32(ConfigurationManager.AppSettings["dbPort"]);
            SMTPHost = ConfigurationManager.AppSettings["SmtpHost"];
            SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            EmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            EmailPass = ConfigurationManager.AppSettings["FormEmailPassword"];
        }
    }
}
