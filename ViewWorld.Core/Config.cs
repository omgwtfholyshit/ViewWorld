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
        /// 数据库连接IP "127.0.0.1"
        /// </summary>
        public static string host_name { get; private set; }
        public static int db_port { get; private set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public const string db_name = "test";
        static Config()
        {
            host_name = "127.0.0.1";//ConfigurationManager.AppSettings["dbHostName"];
            db_port = Convert.ToInt32(ConfigurationManager.AppSettings["dbPort"]);
        }
    }
}
