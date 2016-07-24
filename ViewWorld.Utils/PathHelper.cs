using System;
using System.Web;

namespace ViewWorld.Utils
{
    public class PathHelper
    {
        public static string absolutePathtoVirtualPath(string absolutePath)
        {
            var rootPath = MapPath("~/");
            var VirtualPath = absolutePath.Replace(rootPath, "/").Replace(@"\", @"/");
            return VirtualPath;
        }

        public static string MapPath(string strPath)
        {
            //正常调用
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //线程调用
            {
                strPath = strPath.Replace("/", @"\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
    }
}
