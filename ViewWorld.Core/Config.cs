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
        public static string host_IP { get; private set; }
        public static int db_port { get; private set; }
        static Config()
        {
            host_IP = ConfigurationManager.AppSettings["dbHostIP"];
            db_port = Convert.ToInt32(ConfigurationManager.AppSettings["dbPort"]);
        }
    }
}
