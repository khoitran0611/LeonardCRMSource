using System;

namespace Eli.Common
{
    public class LogHelper
    {
        #region Log Helper
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger("EliCRM");

        /// <summary>
        /// Logging the message into the log storage
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void Log(object message)
        {
            //if (Ctrl.SiteSettings.ENABLE_ERROR_LOG_EMAIL)
            //    SendMail(Ctrl.SiteSettings.SMTP_CREDENTIAL_EMAIL, Ctrl.SiteSettings.NOTIFICATION_FROM_EMAIL, "Error log:" + message, message.ToString());
            Logger.Error(message);
        }

        /// <summary>
        /// Logging the message & the object Exception into the Log storage
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception object to get information from</param>
        public static void Log(object message, Exception ex)
        {
            //if (Ctrl.SiteSettings.ENABLE_ERROR_LOG_EMAIL)
            //    SendMail(Ctrl.SiteSettings.SMTP_CREDENTIAL_EMAIL, Ctrl.SiteSettings.NOTIFICATION_FROM_EMAIL, "Error log:" + message, ex.ToString());
            Logger.Error(message, ex);
        }
        #endregion
    }
}
