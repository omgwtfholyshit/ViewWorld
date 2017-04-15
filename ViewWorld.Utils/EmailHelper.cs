using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core;

namespace ViewWorld.Utils
{
    public class EmailHelper
    {
        #region Constructor
        private static EmailHelper client = new EmailHelper();
        private static SmtpClient smtpClient { get; set; }
        static EmailHelper()
        {
            smtpClient = new SmtpClient(Config.SMTPHost, Config.SMTPPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(Config.EmailAddress, Config.EmailPass);
            smtpClient.Timeout = 10000;
        }
        EmailHelper()
        {
        }
        public static EmailHelper Instance
        {
            get
            {
                return client;
            }
        }
        #endregion
        public async Task SendVerificationEmailAsync(string to, string code)
        {
            
            MailMessage mail = new MailMessage(new MailAddress(Config.EmailAddress, "瞰世界团队"), new MailAddress(to));
            mail.Subject = "瞰世界注册码";
            mail.Body = string.Format("您的认证码是 {0},感谢您注册瞰世界。该认证码将在10分钟后失效。如果您没有进行过该操作，请忽略。", code);
            try
            {
               await smtpClient.SendMailAsync(mail);
            }
            catch (Exception e)
            {
                Tools.WriteLog("邮件服务", "发送认证邮件", e.Message);
                throw e;
            }

        }
    }
}
