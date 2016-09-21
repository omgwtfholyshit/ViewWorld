using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using ViewWorld.Utils.ViewModels;

namespace ViewWorld.Utils
{
    public class HttpHelper
    {
        static readonly string IpLookUpAddress = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=js&ip=";
        #region HTTP请求相关操作
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <returns>响应信息</returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";//设置请求的方法
            request.Accept = "*/*";//设置Accept标头的值
            string responseStr = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())//获取响应
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();
                }

            }
            return responseStr;
        }

        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        public static string HttpPost(string url, string param, string certpath = "", string certpwd = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
            if (!string.IsNullOrEmpty(certpath) && !string.IsNullOrEmpty(certpwd))
            {
                X509Certificate2 cer = new X509Certificate2(certpath, certpwd,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                request.ClientCertificates.Add(cer);
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (StreamWriter requestStream =
            new StreamWriter(request.GetRequestStream()))
            {
                requestStream.Write(param);//将请求的数据写入请求流
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();//获取响应
                }
            }
            return responseStr;
        }
        public static string HttpPost(string url, Stream stream)
        {
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (var reqstream = request.GetRequestStream())
            {
                stream.Position = 0L;
                stream.CopyTo(reqstream);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();//获取响应
                }
            }
            return responseStr;
        }

        public static LocationViewModel RequestUserLocation(string IP)
        {
            try
            {
                string result = HttpGet(IpLookUpAddress + IP);
                if (result != "-2")
                {
                    result = result.Remove(0, result.IndexOf('{') - 1).Replace(';', ' ');
                    LocationViewModel location = JsonConvert.DeserializeObject<LocationViewModel>(result);
                    return location;
                }
                return new LocationViewModel { Country = "中国", City = "北京", Province = "北京", District = "朝阳区" };
            }
            catch(Exception)
            {
                return new LocationViewModel { Country = "中国", City = "北京", Province = "北京", District = "朝阳区" };
            }
            
        }
        #endregion
    }
}
