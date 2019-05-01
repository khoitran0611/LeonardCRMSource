using System.Collections.Specialized;

namespace Eli.Common
{
    public sealed class ConfigValues
    {
        private static readonly NameValueCollection MySettings = Config.GetConfiguration();

        #region Config values from Web.config
        public static string SITE_MASTER_PAGE
        {
            get
            {
                return MySettings["SITE_MASTER_PAGE"];
            }
        }
        
        public static int AUTHEN_COOKIE_EXPIRE_TIME
        {
            get
            {
                int timeExpire;
                int.TryParse(MySettings["AUTHEN_COOKIE_EXPIRE_TIME"], out timeExpire);
                return timeExpire;

            }
        }

        public static string AUTHEN_COOKIE_KEY
        {
            get
            {
                return MySettings["AUTHEN_COOKIE_KEY"];
            }
        }

        public static string UPLOAD_DIRECTORY
        {
            get
            {
                return SITE_ROOT + MySettings["UPLOAD_DIRECTORY"];
            }
        }

        public static bool ENABLE_CACHE
        {
            get
            {
                return (bool.Parse(MySettings["ENABLE_CACHE"]));
            }
        }

        public static string SITE_ROOT
        {
            get
            {
                return (MySettings["SITE_ROOT"]);
            }
        }

        public static bool NEED_LANDING_PAGE
        {
            get
            {
                return bool.Parse(MySettings["NEED_LANDING_PAGE"]);
            }
        }

        public static string UPLOAD_DIRECTORY_SALE_DOCUMENT
        {
            get
            {
                return UPLOAD_DIRECTORY + MySettings["UPLOAD_DIRECTORY_SALE_DOCUMENT"];
            }
        }

        public static string UPLOAD_DIRECTORY_SIGNATURE
        {
            get
            {
                return UPLOAD_DIRECTORY + MySettings["UPLOAD_DIRECTORY_SIGNATURE"];
            }
        }

        public static string UPLOAD_DIRECTORY_TEMP
        {
            get
            {
                return UPLOAD_DIRECTORY + MySettings["UPLOAD_DIRECTORY_TEMP"];
            }
        }

        public static string DELIVERY_WEBFORM_URL
        {
            get
            {
                return MySettings["DELIVERY_WEBFORM_URL"];
            }
        }

        public static string CUSTOMER_ACCEPTANCE_WEBFORM_URL
        {
            get
            {
                return MySettings["CUSTOMER_ACCEPTANCE_WEBFORM_URL"];
            }
        }

        public static string MAIL_DISPLAY_NAME
        {
            get
            {
                return MySettings["MAIL_DISPLAY_NAME"];
            }
        }

        public static string RESET_PASSWORD_LINK 
        {
            get
            {
                return SITE_ROOT + MySettings["RESET_PASSWORD_LINK"];
            }
        }
        #endregion


        
    }
}
