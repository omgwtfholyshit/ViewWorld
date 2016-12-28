using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Utils
{
    public class VerificationSender
    {
        public static string sendURL = "http://sms-cly.cn/smsSend.do";
        public static string sendDataURL = "http://sms-cly.cn/sendData.do";
        public static string queryBalanceURL = "http://sms-cly.cn/balanceQuery.do";
        public static string changePasswordURL = "http://sms-cly.cn/passwordUpdate.do";
        static string UserName = "cly";
        static string Password { get
            {
                return GetMD5(UserName + GetMD5("cly2016"));
            } }
        private static VerificationSender client = new VerificationSender();
        static VerificationSender()
        {
        }
        VerificationSender()
        {
        }
        public static VerificationSender Instance
        {
            get
            {
                return client;
            }
        }
        /*
        * 方法名称：GetMD5
        * 功    能：字符串MD5加密
        * 参    数：待转换字符串
        * 返 回 值：加密之后字符串
        */
        public static string GetMD5(string sourceStr)
        {
            string resultStr = "";

            byte[] b = System.Text.Encoding.Default.GetBytes(sourceStr);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            for (int i = 0; i < b.Length; i++)
                resultStr += b[i].ToString("x").PadLeft(2, '0');

            return resultStr;
        }
        #region HttpGet
        /*
       * 方法名称：HttpGet
       * 功    能：以get方式进行提交
       * 参    数：url(发送地址)
       * 返 回 值：提交后的返回状态
       */
        public string HttpGet(string url, string postData)
        {
            string URL = url + "?username=" + UserName + "&password=" + Password + "&" + postData;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "html/text;charset=utf-8";

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (WebException webex)
            {
                HttpWebResponse res = (HttpWebResponse)webex.Response;
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string html = reader.ReadToEnd();
                throw new Exception(html, webex);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region HttpPost
        /*
      * 方法名称：HttpPost
      * 功    能：以post方式进行提交
      * 参    数：url(发送地址)，postData（发送的数据）
      * 返 回 值：提交后的返回状态
      */
        public string HttpPost(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.KeepAlive = false;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bodyBytes = encoding.GetBytes(UserName + "&" + Password + "&" + postData);
            request.ContentLength = bodyBytes.Length;
            using (Stream serviceRequestBodyStream = request.GetRequestStream())
            {
                serviceRequestBodyStream.Write(bodyBytes, 0, bodyBytes.Length);
                serviceRequestBodyStream.Close();

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string result = reader.ReadToEnd();
                            return result;
                        }
                    }
                }
                catch (WebException webex)
                {
                    HttpWebResponse res = (HttpWebResponse)webex.Response;
                    StreamReader reader = new StreamReader(res.GetResponseStream());
                    string html = reader.ReadToEnd();
                    throw new Exception(html, webex);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        public List<string> readTXT(string path)
        {
            List<string> str = new List<string>();
            string line;
            StreamReader sr = new StreamReader(path, false);
            int i = 0;
            while (i < 598)//
            {
                line = sr.ReadLine().ToString();
                str.Add(line);
                Console.WriteLine(line);
                i++;
            }
            sr.Close();
            return str;
        }
    }
}
