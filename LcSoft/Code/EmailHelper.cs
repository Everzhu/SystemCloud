using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class EmailHelper
    {
        ///<summary>
        /// 发送邮件
        ///</summary>
        ///<param name="sendEmailAddress">发件人邮箱</param>
        ///<param name="sendEmailPwd">发件人密码</param>
        ///<param name="msgToEmail">收件人邮箱地址</param>
        ///<param name="title">邮件标题</param>
        ///<param name="content">邮件内容</param>
        ///<param name="host">邮件SMTP服务器</param>
        ///<returns>0：失败。1：成功！</returns>
        ///示例：SendEmail("123456@qq.com", "123456", new string[] { "530794089@qq.com" }, "测试标题", "测试内容", "smtp.qq.com");
        public static int SendEmail(string sendEmailAddress, string sendEmailPwd, string[] msgToEmail, string title, string content, string host)
        {
            //发件者邮箱地址
            string fjrtxt = sendEmailAddress;
            //发件者邮箱密码
            string mmtxt = sendEmailPwd;
            //主题
            string zttxt = title;
            //内容
            string nrtxt = content;
            string[] fasong = fjrtxt.Split('@');
            //设置邮件协议
            var client = new System.Net.Mail.SmtpClient(host);   //System.Net.Mail.SmtpClient
            client.UseDefaultCredentials = false;
            //通过网络发送到Smtp服务器
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //通过用户名和密码 认证
            client.Credentials = new System.Net.NetworkCredential(fasong[0].ToString(), mmtxt);  //System.Net.NetworkCredential
            //QQ邮箱使用ssl加密，需要设置SmtpClient.EnableSsl 属性为True表示“指定 SmtpClient 使用安全套接字层 (SSL) 加密连接。”
            client.EnableSsl = true;

            //发件人和收件人的邮箱地址
            var mmsg = new System.Net.Mail.MailMessage();
            mmsg.From = new System.Net.Mail.MailAddress(fjrtxt);
            for (int i = 0; i < msgToEmail.Length; i++)
            {
                mmsg.To.Add(new System.Net.Mail.MailAddress(msgToEmail[i]));
            }
            //邮件主题
            mmsg.Subject = zttxt;
            //主题编码
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;
            //邮件正文
            mmsg.Body = nrtxt;
            //正文编码
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            //设置为HTML格式
            mmsg.IsBodyHtml = true;
            //优先级
            mmsg.Priority = System.Net.Mail.MailPriority.High;
            try
            {
                client.Send(mmsg);
                return 1;
            }
            catch (Exception exss)
            {
                string msg = exss.Message;
                return 0;
            }
        }

        /// <summary>
        /// 发送邮件
        /// 例：bool result = EmailUtil.SendEmail("857385896@qq.com", "请审核报告", "请及时审核报告");
        /// </summary>
        /// <param name="mailTo">要发送的邮箱</param>
        /// <param name="mailSubject">邮箱主题</param>
        /// <param name="mailContent">邮箱内容</param>
        /// <returns>返回发送邮箱的结果</returns>
        public static bool SendEmail(string mailTo, string mailSubject, string mailContent)
        {
            // 设置发送方的邮件信息,例如使用网易的smtp
            string smtpServer = System.Configuration.ConfigurationManager.AppSettings["XkSystemEmailSmtp"]; //SMTP服务器 例：smtp.163.com
            string mailFrom = System.Configuration.ConfigurationManager.AppSettings["XkSystemEmailName"]; //登陆用户名 例：s847577@163.com
            string userPassword = System.Configuration.ConfigurationManager.AppSettings["XkSystemEmailPwd"]; //登陆密码    例：fjia345f

            // 邮件服务设置
            var smtpClient = new System.Net.Mail.SmtpClient();
            smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = smtpServer; //指定SMTP服务器
            smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名和密码
            smtpClient.Port = 25;

            // 发送邮件设置        
            var mailMessage = new System.Net.Mail.MailMessage(mailFrom, mailTo); // 发送人和收件人
            mailMessage.Subject = mailSubject;//主题
            mailMessage.Body = mailContent;//内容
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;//正文编码
            mailMessage.IsBodyHtml = true;//设置为HTML格式
            mailMessage.Priority = System.Net.Mail.MailPriority.Low;//优先级

            try
            {
                smtpClient.Send(mailMessage); // 发送邮件
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}