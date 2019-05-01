using System.Globalization;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Eli.Common
{
    public static class Utilities
    {
        public static string CurrentShortDatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        #region Data types helpers

        public static bool IsInteger(string st)
        {
            try
            {
                int.Parse(st);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLongInt(string st)
        {
            try
            {
                long.Parse(st);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDouble(string st)
        {
            try
            {
                double.Parse(st);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDecimal(string st)
        {
            try
            {
                decimal.Parse(st);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPositiveInt(string st)
        {
            try
            {
                var n = Convert.ToInt32(st);
                return n > 0;
            }
            catch { return false; }
        }

        public static bool IsRegexPattern(string pattern)
        {
            try
            {
                new Regex(pattern);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Get a decimal string with Currency format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCurrency(object value)
        {
            try
            {
                return ((decimal)value).ToString("C2", CultureInfo.CurrentCulture);
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Get a decimal string without Currency format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDecimalWithoutCurrency(object value)
        {
            try
            {
                return ((decimal) value).ToString("0.##");
            }
            catch
            {
                return String.Empty;
            }
        }

        public static bool IsDate(string st)
        {
            try
            {
                var dtfi = new DateTimeFormatInfo
                {
                    ShortDatePattern = CurrentShortDatePattern
                };
                return DateTime.Parse(st, dtfi, DateTimeStyles.None) > DateTime.MinValue;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsMail(string email)
        {
            //Using Regex to check email
            const string strRegex = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            var re = new Regex(strRegex);
            if (re.IsMatch(email))
                return (true);
            return (false);
        }

        /// <summary>
        /// Check if password is valid. A valid password is min 6chars, max 20chars of alphanumeric(case sensitive) and no special symbols
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsPasswordValid(string password)
        {
            //Regex reg = new Regex(@"^.*(?=.{6,})(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z0-9]+$");//min 6chars alphanumeric and no special symbols
            var reg = new Regex((@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{6,20})$"));
            //min 6chars, ,max 20chars alphanumeric and no special symbols
            return reg.IsMatch(password);
        }

        public static bool IsUrl(string url)
        {
            //Using Regex to check email
            const string strRegex = @"^(http(s)?://|www+\.)([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
            var re = new Regex(strRegex);
            if (re.IsMatch(url))
                return (true);
            return (false);
        }

        public static bool IsBool(string st)
        {
            try
            {
                bool.Parse(st);
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region Script Helper
        /// <summary>
        /// Return a javascript confirm dialog string
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ConfirmString(string message)
        {
            return "return confirm('" + message + "')";
        }
        #endregion

        #region Number Helper

        public static int ToInt(object obj)
        {
            return obj == null ? 0 : (string.IsNullOrWhiteSpace(obj.ToString()) ? 0 : int.Parse(obj.ToString()));
        }

        #endregion

        public static string GetClientIpAddress()
        {
            var ipaddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            ipaddress = string.IsNullOrEmpty(ipaddress) ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : string.Empty;
            if (ipaddress == "::1")
                ipaddress = "127.0.0.1";
            return ipaddress;
        }
    }
}