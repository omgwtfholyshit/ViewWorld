using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewWorld.Core.ExtensionMethods
{
    public static class DirectoryExtensions
    {
        /// <summary>
        /// 将绝对路径转换为虚拟路径.只适用于路径
        /// </summary>
        /// <param name="AbsolutePaths">绝对路径</param>
        /// <returns></returns>
        public static IEnumerable<string> ToVirtualPaths(this IEnumerable<string> AbsolutePaths)
        {
            var virtualPaths = new List<string>();
            foreach(var path in AbsolutePaths)
            {
                virtualPaths.Add(path.Replace(HttpContext.Current.Server.MapPath("~/"), "/").Replace(@"\", @"/"));
            }
            return virtualPaths.AsEnumerable();
        }
    }
}
