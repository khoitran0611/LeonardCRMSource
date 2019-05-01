using System;

namespace Eli.Common
{
    /// <summary>
    /// Used to encode, decode time when get/set time from database.
    /// </summary>
    /// <remarks>The time saved in database is always <b>GMT + 0</b>.<hr/>
    /// The web server time may be different from the database time, eg GMT + <i>a</i>.
    /// So, the time to save into DB is [Input time from user - <i>a</i>].
    /// And the time to display is [Output time from DB +<i>a</i>].
    /// We must convert time to GMT + 0 for both server time zone and user time zone to make
    /// sure that:
    /// <list type="bullet">
    /// <item>The time displaying in client browser is accurate (and server browser too).</item>
    /// <item>The time is right when server is changed.</item>
    /// </list>
    /// </remarks>
    public class TimeManager
    {
        /// <summary>
        /// By default, the server time is set as GMT + 0.
        /// </summary>
        public static int ServerTimeDiff = 0;

        /// <summary>
        /// Encodes time to save into DB.
        /// </summary>
        /// <param name="timeToEncode">The input time from user.</param>
        /// <param name="timeDiff">The time zone of user, value between [-12, +13].</param>
        /// <returns>The GMT + 0 time which is correlative with <i>timeToEncode</i>.</returns>
        public static DateTime EncodeTime(DateTime timeToEncode, float timeDiff)
        {
            if (timeDiff < -12 || timeDiff > 13)
            {
                throw new IndexOutOfRangeException("The time zone is not correct.");
            }
            return timeToEncode.AddHours(-timeDiff);
        }
        /// <summary>
        /// Decodes time to display in browser.
        /// </summary>
        /// <param name="timeToDecode">The output time which is gotten from DB (GMT + 0).</param>
        /// <param name="timeDiff">The time zone of user, value between [-12, +13].</param>
        /// <returns>The time user location time which is correlative with <i>timeToDecode</i>.</returns>
        public static DateTime DecodeTime(DateTime timeToDecode, float timeDiff)
        {
            if (timeDiff < -12 || timeDiff > 13)
            {
                throw new IndexOutOfRangeException("The time zone is not correct.");
            }
            return timeToDecode.AddHours(timeDiff);
        }

        public static DateTime EncodeTime(DateTime timeToEncode, double timeDiff)
        {
            if (timeDiff < -12 || timeDiff > 13)
            {
                throw new IndexOutOfRangeException("The time zone is not correct.");
            }
            return timeToEncode.AddHours(-timeDiff);
        }

        public static DateTime DecodeTime(DateTime timeToDecode, double timeDiff)
        {
            if (timeDiff < -12 || timeDiff > 13)
            {
                throw new IndexOutOfRangeException("The time zone is not correct.");
            }
            return timeToDecode.AddHours(timeDiff);
        }

        public static int GetTotalMonths(DateTime date)
        {
            return date.Year * 12 + date.Month;
        }


        //public static DateTime CurrentTime
        //{
        //    get
        //    {
        //        return EncodeTime(DateTime.Now, double.Parse(HttpContext.Current.Application["default_time_zone"].ToString()));
        //    }
        //}
    }
}
