using System.Globalization;
using System.Threading;
using System;
using System.Web.UI.WebControls;

namespace Eli.Common
{
    enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public class DateUtility
    {

        static void SetMonthsForControl(DropDownList months)
        {
            months.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                months.Items.Add(new ListItem(i.ToString(),i.ToString()));
            }
            months.SelectedValue = DateTime.Now.Month.ToString();
        }

        static void SetAlphabeMonthsForControl(DropDownList months)
        {
            months.Items.Clear();
            string[] items = Enum.GetNames(typeof(Month));
            foreach (string item in items)
            {
                months.Items.Add(new ListItem(item, Enum.Parse(typeof(Month), item).GetHashCode().ToString()));
            }
            months.SelectedValue = DateTime.Now.Month.ToString();
        }

        public static void SetDayOfWeek(DropDownList dayOfWeek)
        {
            dayOfWeek.Items.Clear();
            for (int i = 0; i <= 6; i++)
            {
                string name = Enum.GetName(typeof(DayOfWeek), i);
                var item = new ListItem(name, name);
                dayOfWeek.Items.Add(item);
            }
        }

        static void SetDaysForControl(DropDownList days)
        {
            days.Items.Clear();
            for (int i = 1; i <= 31; i++)
            {
                days.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            days.SelectedValue = DateTime.Now.Day.ToString();
        }

        static void SetYearsForControl(DropDownList years, int fromYear, int toYear)
        {
            years.Items.Clear();
            for (int i = fromYear; i <= toYear; i++)
            {
                years.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            years.SelectedValue = DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// Set day, month, year for controls and insert an empty item to the first index
        /// </summary>
        /// <param name="days"></param>
        /// <param name="months"></param>
        /// <param name="years"></param>
        /// <param name="fromYear"></param>
        /// <param name="toYear"></param>
        public static void SetDateTimeWithoutRequire(DropDownList days, DropDownList months, DropDownList years, int fromYear, int toYear)
        {
            SetMonthsForControl(months);
            SetDaysForControl(days);
            SetYearsForControl(years, fromYear, toYear);
            days.Items.Insert(0, new ListItem(" ", "0"));
            months.Items.Insert(0, new ListItem(" ", "0"));
            years.Items.Insert(0, new ListItem(" ", "0"));
        }

        public static DateTime GetDate(string strMonth, string strDay, string strYear)
        {
            DateTime date = DateTime.MinValue;
            try
            {
                int month = Int32.Parse(strMonth);
                int year = Int32.Parse(strYear);
                int day = Int32.Parse(strDay);
                date = new DateTime(year, month, day);
            }
            catch
            { }
            return date;
        }

        public static void LoadTimeZone(DropDownList drTimeZone)
        {
            drTimeZone.Items.Clear();
            drTimeZone.Items.Add(new ListItem("GMT-12:00", "-12"));
            drTimeZone.Items.Add(new ListItem("GMT-11:00", "-11"));
            drTimeZone.Items.Add(new ListItem("GMT-10:00", "-10"));
            drTimeZone.Items.Add(new ListItem("GMT-9:00", "-9"));
            drTimeZone.Items.Add(new ListItem("GMT-8:00", "-8"));
            drTimeZone.Items.Add(new ListItem("GMT-7:00", "-7"));
            drTimeZone.Items.Add(new ListItem("GMT-6:00", "-6"));
            drTimeZone.Items.Add(new ListItem("GMT-5:00", "-5"));
            drTimeZone.Items.Add(new ListItem("GMT-4:00", "-4"));
            drTimeZone.Items.Add(new ListItem("GMT-3:00", "-3"));
            drTimeZone.Items.Add(new ListItem("GMT-2:00", "-2"));
            drTimeZone.Items.Add(new ListItem("GMT-1:00", "-1"));
            drTimeZone.Items.Add(new ListItem("GMT+0:00", "0"));
            drTimeZone.Items.Add(new ListItem("GMT+1:00", "1"));
            drTimeZone.Items.Add(new ListItem("GMT+2:00", "2"));
            drTimeZone.Items.Add(new ListItem("GMT+3:00", "3"));
            drTimeZone.Items.Add(new ListItem("GMT+4:00", "4"));
            drTimeZone.Items.Add(new ListItem("GMT+5:00", "5"));
            drTimeZone.Items.Add(new ListItem("GMT+6:00", "6"));
            drTimeZone.Items.Add(new ListItem("GMT+7:00", "7"));
            drTimeZone.Items.Add(new ListItem("GMT+8:00", "8"));
            drTimeZone.Items.Add(new ListItem("GMT+9:00", "9"));
            drTimeZone.Items.Add(new ListItem("GMT+10:00", "10"));
            drTimeZone.Items.Add(new ListItem("GMT+11:00", "11"));
            drTimeZone.Items.Add(new ListItem("GMT+12:00", "12"));
        }

        /// <summary>
        /// Getting a datetime with current culture format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDate(string date)
        {
            try
            {
                //var culture = CultureInfo.CreateSpecificCulture(SessionManager.PreferLanguage);
                //return DateTime.Parse(date, culture);
                return DateTime.Parse(date, CultureInfo.CurrentCulture);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Getting date from a date string with custom format
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateFormat"></param>
        /// <param name="withTime"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static string GetFormartedDate(object date, string dateFormat, bool withTime, string timeFormat)
        {
            try
            {
                var dtfi = new DateTimeFormatInfo
                    {
                        LongDatePattern = dateFormat,
                        LongTimePattern = timeFormat,
                        ShortDatePattern = dateFormat,
                        ShortTimePattern = timeFormat
                    };
                var format = dateFormat.Replace("'", "\"'\"") + (withTime ? String.Format(" {0}", timeFormat.Replace("a", "tt")) : "");
                var s = DateTime.Parse(date.ToString(), dtfi, DateTimeStyles.None);
                return s.ToString(format);
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Getting formatted date from a date string with given culture code and date format
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateFormat"></param>
        /// <param name="withTime"></param>
        /// <param name="timeFormat"></param>
        /// <param name="preferLanguage"></param>
        /// <returns></returns>
        public static string GetFormartedDate(object date, string dateFormat, bool withTime, string timeFormat, string preferLanguage)
        {
            try
            {
                var culture = CultureInfo.CreateSpecificCulture(preferLanguage);
                Thread.CurrentThread.CurrentCulture = culture;
                var s = DateTime.Parse(date.ToString(), culture);
                return s.ToString(dateFormat.Replace("'", "\"'\"") + (withTime ? String.Format(" {0}", timeFormat.Replace("a","tt")) : ""));
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Getting formatted date by given culture
        /// </summary>
        /// <param name="date"></param>
        /// <param name="cultureCode"></param>
        /// <param name="withTime"></param>
        /// <returns></returns>
        public static string GetFormattedDate(object date, string cultureCode, bool withTime)
        {
            try
            {
                var culture = CultureInfo.CreateSpecificCulture(cultureCode);
                Thread.CurrentThread.CurrentCulture = culture;
                var s = DateTime.Parse(date.ToString(), culture);
                return !withTime ? s.ToShortDateString() : s.ToString(culture);
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
