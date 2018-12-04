using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Web;
using System.Net.Mail;
using System.Web.Security;
using System.Net;
using qinuo.Cmn;

namespace NWJ.Cmn
{
    public class DTMail
    {
        static string strEmail = "service@nianwj.com";
        static string strPass = "Nianwj2013";
        static string strnickName = "念无界";
        //static string smtpserver = "mail.nianwj.com"; 
        static string smtpserver = "smtp.nianwj.com";
        #region 发送电子邮件
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="smtpserver">SMTP服务器</param>
        /// <param name="enablessl">是否启用SSL加密</param>
        /// <param name="userName">登录帐号</param>
        /// <param name="pwd">登录密码</param>
        /// <param name="nickName">发件人昵称</param>
        /// <param name="strfrom">发件人</param>
        /// <param name="strto">收件人</param>
        /// <param name="subj">主题</param>
        /// <param name="bodys">内容</param>
        public static void sendMail(string smtpserver, int enablessl, string userName, string pwd, string nickName, string strfrom, string strto, string subj, string bodys)
        {
            SmtpClient _smtpClient = new SmtpClient();
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            _smtpClient.Host = smtpserver;//指定SMTP服务器
            _smtpClient.Credentials = new System.Net.NetworkCredential(userName, pwd);//用户名和密码
            if (enablessl == 1)
            {
                _smtpClient.EnableSsl = true;
            }
            MailAddress _from = new MailAddress(strfrom, nickName);
            MailAddress _to = new MailAddress(strto);
            MailMessage _mailMessage = new MailMessage(_from, _to);
            _mailMessage.Subject = subj;//主题
            _mailMessage.Body = bodys;//内容
            _mailMessage.BodyEncoding = System.Text.Encoding.Default;//正文编码
            _mailMessage.IsBodyHtml = true;//设置为HTML格式
            _mailMessage.Priority = MailPriority.Normal;//优先级
            _smtpClient.Send(_mailMessage);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="fromUser">发送邮件人</param>
        /// /// <param name="toUser">接收邮件人</param>
        /// <param name="mailSubject">邮箱主题</param>
        /// <param name="mailContent">邮箱内容</param>
        /// <returns>返回发送邮箱的结果</returns>
        public static bool SendEmail(string fromUser, string toUser, string mailSubject, string mailContent)
        {
            // 设置发送方的邮件信息,例如使用网易的smtp
            string smtpServer = smtpserver;
            string userPassword = strPass;//登陆密码
            // 邮件服务设置
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = smtpServer; //指定SMTP服务器
            smtpClient.Credentials = new System.Net.NetworkCredential(fromUser, userPassword);//用户名和密码
            MailAddress _from = new MailAddress(fromUser, strnickName);
            MailAddress _to = new MailAddress(toUser);
            // 发送邮件设置        
            MailMessage mailMessage = new MailMessage(_from, _to); // 发送人和收件人
            mailMessage.Subject = mailSubject;//主题
            mailMessage.Body = mailContent;//内容
            mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
            mailMessage.IsBodyHtml = true;//设置为HTML格式
            mailMessage.Priority = MailPriority.Low;//优先级

            try
            {
                smtpClient.Send(mailMessage); // 发送邮件
                return true;
            }

            catch (SmtpException ex)
            {
                return false;
            }
        }

        public static bool SendMailCode(string toUser, string sub)
        {
            string number = Utils.Number(6);
            Utils.WriteCookie(NKeys.SESSION_EMAIL_CODE, "");
            //sendMail(smtpserver, 0, strEmail, strPass, "念无界", strEmail, toUser, sub, "您的验证码为：" + number + ",有效时间为1分钟。请尽快注册并使用！");
            if (SendEmail(strEmail, toUser, sub, "您的验证码为：" + number + ",有效时间为1分钟。请尽快注册并使用！"))
            {
                Utils.WriteCookie(toUser, number, 1);
                return true;
            }
            return false;
        }
        public static bool SendMailForSuccess(string toUser, string sub)
        {
            if (SendEmail(strEmail, toUser, sub, "您已成功注册念无界账号,登陆账号即本邮箱号,如非本人操作，请及时联系我们010-83812108。【念无界】"))
            {
                return true;
            }
            else
                return false;
        }
        #endregion
        #region 手机验证码
        /// <summary>
        ///     发送手机验证码
        /// </summary>
        /// <param name="phone" type="string">
        ///     <para>
        ///         手机号
        ///     </para>
        /// </param>
        /// <returns>
        ///     A bool value...
        /// </returns>
        public static bool SendPhoneCode(string phone, string content)
        {
            return newsend(phone, content);
            //return  send(phone, content);
        }
        /// <summary>
        ///     发送手机验证码
        /// </summary>
        /// <param name="phone" type="string">
        ///     <para>
        ///         手机号
        ///     </para>
        /// </param>
        /// <returns>
        ///     A bool value...
        /// </returns>
        public static bool SendPhoneCode(string phone, string content, string number)
        {
            return newsend(phone, content, number);
        }
        /// <summary>
        ///     发送短信
        /// </summary>
        /// <param name="mobile" type="string">
        ///     <para>
        ///         手机号
        ///     </para>
        /// </param>
        public static bool send(string mobile, string strContent)
        {
            try
            {
                string sendurl = "http://api.sms.cn/mt/";
                //发送号码
                Random random = new Random();
                string number = Utils.Number(6);
                strContent = string.Format(strContent, number);
                StringBuilder sbTemp = new StringBuilder();
                string uid = NKeys.CCEUP_SMS_USER;
                string pwd = NKeys.CCEUP_SMS_PASS;
                string Pass = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd + uid, "MD5"); //密码进行MD5加密
                //POST 传值
                sbTemp.Append("uid=" + uid + "&pwd=" + Pass + "&mobile=" + mobile + "&content=" + strContent);
                byte[] bTemp = System.Text.Encoding.GetEncoding("GBK").GetBytes(sbTemp.ToString());
                string postReturn = doPostRequest(sendurl, bTemp);
                Utils.WriteCookie(mobile, number, 10);
                if (postReturn.Contains("100"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        ///     发送短信
        /// </summary>
        /// <param name="mobile" type="string">
        ///     <para>
        ///         https://dx.ipyy.net/sms.aspx 对应UTF-8
        ///            https://dx.ipyy.net/smsGBK.aspx 对应GB2312
        ///           https://dx.ipyy.net/smsJson.aspx 对应UTF-8(返回值为json格式)
        ///            https://dx.ipyy.net/ensms.ashx 对应UTF-8(加密传输,使用json)
        ///          https://dx.ipyy.net/BatchSms.ashx  对应UTF-8的点对点发送
        ///     </para>
        /// </param>
        public static bool newsend(string mobile, string strContent, string number = "")
        {
            try
            {
                string sendurl = "https://dx.ipyy.net/smsJson.aspx";
                string param = "action=send&userid=&account={0}&password={1}&mobile={2}&content={3}&sendTime=&extno=";
                //发送号码
                Random random = new Random();
                if (string.IsNullOrEmpty(number))
                    number = Utils.Number(6);
                strContent = string.Format(strContent, number);
                StringBuilder sbTemp = new StringBuilder();
                string uid = NKeys.CCEUP_SMS_USER;
                string pwd = NKeys.CCEUP_SMS_PASS;
                //string Pass = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd + uid, "MD5"); //密码进行MD5加密
                //POST 传值
                sbTemp.Append(string.Format(param, uid, pwd, mobile, strContent));
                LogHelper logs = new LogHelper();
                logs.writeInfos(sbTemp);
                byte[] bTemp = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbTemp.ToString());
                string postReturn = doPostRequest(sendurl, bTemp);
                Utils.WriteCookie(mobile, number, 10);
                LogHelper log = new LogHelper();
                log.writeInfos(postReturn);
                if (postReturn.Contains("Success"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                LogHelper log = new LogHelper();
                log.writeInfos(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "account");
                return false;
            }
        }
        /// <summary>
        ///     发送短信
        /// </summary>
        /// <param name="mobile" type="string">
        ///     <para>
        ///         手机号
        ///     </para>
        /// </param>
        public static bool send(string mobile, string strContent, string number)
        {
            try
            {
                string sendurl = "http://api.sms.cn/mt/";
                //发送号码
                Random random = new Random();
                strContent = string.Format(strContent, number);
                StringBuilder sbTemp = new StringBuilder();
                string uid = NKeys.CCEUP_SMS_USER;
                string pwd = NKeys.CCEUP_SMS_PASS;
                string Pass = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd + uid, "MD5"); //密码进行MD5加密
                //POST 传值
                sbTemp.Append("uid=" + uid + "&pwd=" + Pass + "&mobile=" + mobile + "&content=" + strContent);
                byte[] bTemp = System.Text.Encoding.GetEncoding("GBK").GetBytes(sbTemp.ToString());
                string postReturn = doPostRequest(sendurl, bTemp);
                if (postReturn.Contains("100"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //POST方式发送得结果
        private static string doPostRequest(string url, byte[] bData)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;
            string strResult = string.Empty;
            hwRequest = (HttpWebRequest)WebRequest.Create(url);
            hwRequest.Method = "POST";
            hwRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            hwRequest.ContentLength = bData.Length;
            using (Stream smWrite = hwRequest.GetRequestStream())
            {
                smWrite.Write(bData, 0, bData.Length);
            }
            using (hwResponse = (HttpWebResponse)hwRequest.GetResponse())
            {
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
            }
            return strResult;
        }
        #endregion
    }
}
