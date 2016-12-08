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
        public static string Cache_HostIP { get; private set; }
        public static int Cache_Port { get; private set; }
        static Config()
        {
            DB_HostIP = ConfigurationManager.AppSettings["dbHostIP"];
            DB_Port = Convert.ToInt32(ConfigurationManager.AppSettings["dbPort"]);
            Cache_HostIP = ConfigurationManager.AppSettings["cacheHostIP"];
            Cache_Port = Convert.ToInt32(ConfigurationManager.AppSettings["cachePort"]);
        }
    }
}
