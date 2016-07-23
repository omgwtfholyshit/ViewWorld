using System;
using System.Configuration;

namespace Website.Core
{
    public class Config
    {
        public static string host_name { get; private set; }
        public static int db_port { get; private set; }

        static Config()
        {
            host_name = ConfigurationManager.AppSettings["dbHostName"];
            db_port = Convert.ToInt32(ConfigurationManager.AppSettings["dbPort"]);
        }
    }
}
