using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Eli.Common
{
    public class MailServerInfo
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public static class MailHelper
    {
        /// <summary>
        /// Send email asynchornously without a callback
        /// </summary>
        /// <param name="smtpInfo">SMTP server info</param>
        /// <param name="displayName">Optional display name. This param can be null</param>
        /// <param name="fromAddress">Sender</param>
        /// <param name="toAddresses">Recipients seperated by comma</param>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body message</param>
        /// <param name="files"></param>
        /// <param name="pathToFile"></param>
        /// <param name="ccEmails"></param>
        /// <param name="bccEmails"></param>
        public static void SendAsyncEmail(MailServerInfo smtpInfo, string displayName, string fromAddress, string toAddresses, string subject,
            string body, string[] files = null, string pathToFile = "", string ccEmails = "", string bccEmails = "")
        {
            //Khoi Tran
            //var client = SetupServer(smtpInfo);
            //var emailMessage = SetupMailMessage(displayName, fromAddress, toAddresses, subject, body, ccEmails, bccEmails);
            //if (files != null && files.Any() && !string.IsNullOrEmpty(pathToFile))
            //{
            //    foreach (var f in files)
            //    {
            //        emailMessage.Attachments.Add(new Attachment(pathToFile + "\\" + f));
            //    }
            //}
            //client.SendCompleted += (sender, e) =>
            //{
            //    try
            //    {
            //        if (e.Cancelled || e.Error != null)
            //        {

            //            var msg = (MailMessage)e.UserState;
            //            // TODO: get this working
            //            LogHelper.Log("Can't send email to address: " + msg.To, e.Error);
            //        }
            //        client.Dispose();
            //        emailMessage.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //    }

            //};
            //var flag = true;
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    try
            //    {
            //        if (!flag)
            //            return;
            //        client.SendAsync(emailMessage, Tuple.Create(client, emailMessage));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //        flag = false;
            //    }
            //}
            //    );
        }

        /// <summary>
        /// Send email asynchronously with callback
        /// </summary>
        /// <param name="smtpInfo">SMTP server info</param>
        /// <param name="displayName">Optional display name. This param can be null</param>
        /// <param name="fromAddress">Sender</param>
        /// <param name="toAddresses">Recipients seperated by comma</param>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body message</param>
        /// <param name="callback">Callback method to handle when sending completed</param>
        public static void SendAsyncToAllEmails(MailServerInfo smtpInfo, string displayName, string fromAddress, string toAddresses, string subject,
            string body, SendCompletedEventHandler callback)
        {
            //Khoi Tran
            //var client = SetupServer(smtpInfo);
            //var emailMessage = SetupMailMessage(displayName, fromAddress, toAddresses, subject, body);

            //client.SendCompleted += callback;
            //var flag = true;
            //ThreadPool.QueueUserWorkItem(o =>
            //    {
            //        try
            //        {
            //            if (!flag)
            //                return;
            //            client.SendAsync(emailMessage, Tuple.Create(client, emailMessage));
            //        }
            //        catch (Exception ex)
            //        {
            //            LogHelper.Log(ex.Message, ex);
            //            flag = false;
            //        }
            //    }
            // );
        }

        /// <summary>
        /// Send email with BCC asynchronously with callback
        /// </summary>
        /// <param name="smtpInfo">SMTP server info</param>
        /// <param name="displayName">Optional display name. This param can be null</param>
        /// <param name="fromAddress">Sender</param>
        /// <param name="toAddresses">Recipients seperated by comma</param>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body message</param>
        /// <param name="callback">Callback method to handle when sending completed</param>
        /// <param name="isBcc">BCC or CC. True is BCC, False is CC</param>
        public static void SendAsyncEmail(MailServerInfo smtpInfo, string displayName, string fromAddress, string toAddresses, string subject,
            string body, SendCompletedEventHandler callback, bool isBcc)
        {
            //Khoi Tran
            //var client = SetupServer(smtpInfo);

            //var emailMessage = new MailMessage();
            //var arrayEmails = toAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //if (arrayEmails.Count > 1)
            //{
            //    emailMessage.To.Add(arrayEmails[0]);
            //    arrayEmails.RemoveAt(0);
            //}
            //foreach (var email in arrayEmails)
            //{
            //    if (isBcc)
            //        emailMessage.Bcc.Add(email);
            //    else
            //        emailMessage.CC.Add(email);
            //}

            //emailMessage.From = new MailAddress(fromAddress, string.IsNullOrWhiteSpace(displayName) ? ConfigValues.MAIL_DISPLAY_NAME : displayName);

            //emailMessage.Subject = subject;
            //emailMessage.Body = body;
            //emailMessage.IsBodyHtml = true;

            //client.SendCompleted += callback;
            //var flag = true;
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    try
            //    {
            //        if (!flag)
            //            return;
            //        client.SendAsync(emailMessage, Tuple.Create(client, emailMessage));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //        flag = false;
            //    }
            //}
            // );
        }

        public static void SendAsyncEmail(MailServerInfo smtpInfo, string displayName, string fromAddress, string toAddresses, string ccAddresses, string subject,
            string body, bool isBcc)
        {
            //Khoi Tran
            //var client = SetupServer(smtpInfo);

            //var emailMessage = new MailMessage();
            //var toAddressesArr = toAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //foreach (var address in toAddressesArr)
            //{
            //    emailMessage.To.Add(address);
            //}
            //emailMessage.From = new MailAddress(fromAddress, string.IsNullOrWhiteSpace(displayName) ? ConfigValues.MAIL_DISPLAY_NAME : displayName);
            //emailMessage.Subject = subject;
            //emailMessage.Body = body;
            //emailMessage.IsBodyHtml = true;

            //var arrayEmails = (!string.IsNullOrEmpty(ccAddresses)  ? ccAddresses : "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //foreach (var email in arrayEmails)
            //{
            //    if (isBcc)
            //        emailMessage.Bcc.Add(email);
            //    else
            //        emailMessage.CC.Add(email);
            //}

            //client.SendCompleted += (sender, e) =>
            //{
            //    try
            //    {
            //        if (e.Cancelled || e.Error != null)
            //        {

            //            var msg = (MailMessage)e.UserState;
            //            // TODO: get this working
            //            LogHelper.Log("Can't send email to address: " + msg.To, e.Error);
            //        }
            //        client.Dispose();
            //        emailMessage.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //    }

            //};
            //var flag = true;
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    try
            //    {
            //        if (!flag)
            //            return;
            //        client.SendAsync(emailMessage, Tuple.Create(client, emailMessage));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //        flag = false;
            //    }
            //}
            // );
        }

        public static void SendAsyncEmail(MailServerInfo smtpInfo, string displayName, string fromAddress, string toAddresses, string ccAddresses, string bccAddresses, string subject,
            string body)
        {
            //Khoi Tran
            //var client = SetupServer(smtpInfo);

            //var emailMessage = new MailMessage();
            //var toAddressesArr = toAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //foreach (var address in toAddressesArr)
            //{
            //    emailMessage.To.Add(address);
            //}
            //emailMessage.From = new MailAddress(fromAddress, string.IsNullOrWhiteSpace(displayName) ? ConfigValues.MAIL_DISPLAY_NAME : displayName);
            //emailMessage.Subject = subject;
            //emailMessage.Body = body;
            //emailMessage.IsBodyHtml = true;

            //toAddressesArr = (!string.IsNullOrEmpty(ccAddresses) ? ccAddresses : "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //foreach (var email in toAddressesArr)
            //{
            //    emailMessage.CC.Add(email);
            //}

            //toAddressesArr = (!string.IsNullOrEmpty(bccAddresses) ? bccAddresses : "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //foreach (var email in toAddressesArr)
            //{
            //    emailMessage.Bcc.Add(email);
            //}

            //client.SendCompleted += (sender, e) =>
            //{
            //    try
            //    {
            //        if (e.Cancelled || e.Error != null)
            //        {

            //            var msg = (MailMessage)e.UserState;
            //            // TODO: get this working
            //            LogHelper.Log("Can't send email to address: " + msg.To, e.Error);
            //        }
            //        client.Dispose();
            //        emailMessage.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //    }

            //};
            //var flag = true;
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    try
            //    {
            //        if (!flag)
            //            return;
            //        client.SendAsync(emailMessage, Tuple.Create(client, emailMessage));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //        flag = false;
            //    }
            //}
            // );
        }

        /// <summary>
        /// Send email to outlook
        /// </summary>
        /// <param name="smtpInfo"></param>
        /// <param name="message"></param>
        static public void SendAsyncEmail(MailServerInfo smtpInfo, MailMessage message)
        {
            //Khoi Tran
            //var client = SetupServer(smtpInfo);
            //client.SendCompleted += (sender, e) =>
            //{
            //    try
            //    {
            //        if (e.Cancelled || e.Error != null)
            //        {
            //            var msg = (MailMessage)e.UserState;
            //            // TODO: get this working
            //            LogHelper.Log("Can't send email to address: " + msg.To, e.Error);
            //        }
            //        client.Dispose();
            //        message.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //    }

            //};
            //var flag = true;
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    try
            //    {
            //        if (!flag) return;
            //        client.SendAsync(message, Tuple.Create(client, message));
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Log(ex.Message, ex);
            //        flag = false;
            //    }
            //}
            //);
        }

        /// <summary>
        /// Send email to a list of recepients
        /// </summary>
        /// <param name="smtpInfo">Mail server info</param>
        /// <param name="fromAddress">Sender</param>
        /// <param name="toAddresses">List of recipients separated by comma</param>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body</param>
        /// <param name="displayName">The display name of sender</param>
        static public void SendMail(MailServerInfo smtpInfo, string fromAddress, string toAddresses, string subject, string body, string displayName = null)
        {
            //Khoi Tran
            //var smtpSend = SetupServer(smtpInfo);
            //var emailMessage = SetupMailMessage(displayName, fromAddress, toAddresses, subject, body);
            //smtpSend.Send(emailMessage);
        }

        public static void SendMailWithAttachments(MailServerInfo smtpInfo, string from, string to, string[] cc, string subject, string message, string[] files, string pathToFile, string password)
        {
            //Khoi Tran
            //using (var smtp = new SmtpClient(smtpInfo.SmtpServer, smtpInfo.SmtpPort) { EnableSsl = smtpInfo.EnableSSL })
            //{
            //    if (!String.IsNullOrEmpty(smtpInfo.Username) && !String.IsNullOrEmpty(smtpInfo.Password))
            //    {
            //        smtp.Credentials = new NetworkCredential(smtpInfo.Username, smtpInfo.Password);
            //    }

            //    using (var msg = new MailMessage())
            //    {
            //        msg.From = new MailAddress(from, ConfigValues.MAIL_DISPLAY_NAME);
            //        var arrayEmails = to.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (var email in arrayEmails)
            //        {
            //            msg.To.Add(email);
            //        }
            //        msg.Subject = subject;
            //        msg.IsBodyHtml = true;
            //        msg.Body = message;

            //        if (cc != null && cc.Length > 0)
            //        {
            //            foreach (var s in cc.Where(s => !String.IsNullOrEmpty(s)))
            //            {
            //                msg.CC.Add(s);
            //            }
            //        }

            //        if (files != null && files.Length > 0)
            //        {
            //            foreach (string s in files.Where(s => !String.IsNullOrEmpty(s)))
            //            {
            //                msg.Attachments.Add(new Attachment(pathToFile + "\\" + s));
            //            }
            //        }
            //        smtp.Send(msg);
            //    }
            //}
        }

        #region Private methods
        private static MailMessage SetupMailMessage(string displayName, string fromAddress, string toAddresses, string subject,
            string body,string ccEmails = "", string bccEmails = "")
        {
            var emailMessage = new MailMessage();
            var arrayEmails = toAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in arrayEmails)
            {
                emailMessage.To.Add(email);
            }
            arrayEmails = ccEmails.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in arrayEmails)
            {
                emailMessage.CC.Add(email);
            }
            arrayEmails = bccEmails.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in arrayEmails)
            {
                emailMessage.Bcc.Add(email);
            }

            emailMessage.From = new MailAddress(fromAddress,
                string.IsNullOrWhiteSpace(displayName) ? ConfigValues.MAIL_DISPLAY_NAME : displayName);

            emailMessage.Subject = subject;
            emailMessage.Body = body;
            emailMessage.IsBodyHtml = true;
            return emailMessage;
        }

        private static SmtpClient SetupServer(MailServerInfo smtpInfo)
        {
            var client = new SmtpClient(smtpInfo.SmtpServer, smtpInfo.SmtpPort) { EnableSsl = smtpInfo.EnableSSL };
            if (!String.IsNullOrEmpty(smtpInfo.Username) && !String.IsNullOrEmpty(smtpInfo.Password))
            {
                client.Credentials = new NetworkCredential(smtpInfo.Username, smtpInfo.Password);
            }
            return client;
        } 
        #endregion

    }
}
