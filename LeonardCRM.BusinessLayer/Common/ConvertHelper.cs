using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace LeonardCRM.BusinessLayer.Common
{
    public static class ConvertHelper
    {
        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> DataTableToListSerialNumber<T>(this DataTable table) where T : class, new()
        {
            try
            {
                var list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            if (prop.PropertyType == typeof(long) && !string.IsNullOrWhiteSpace(row[0].ToString()))
                            {
                                PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                                propertyInfo.SetValue(obj, Convert.ChangeType(row[0], propertyInfo.PropertyType), null);
                                list.Add(obj);
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the reminder to unit.
        /// This method will return array have 2 values
        /// 1. The first will return value
        /// 2. The second will return unit
        /// </summary>
        /// <param name="strReminderValue">reminder value</param>
        /// <returns></returns>
        public static string[] ConvertReminderToUnit(string strReminderValue)
        {
            var units = new []
            {
                new {Value = 1, Name = "minutes"},
                new {Value = 60, Name = "hours"},
                new {Value = 60*24, Name = "days"},
                new {Value = 60*24*7, Name = "weeks"}
            };

            if (string.IsNullOrEmpty(strReminderValue)) return (string[]) new[] { "0", units[0].Name };

            int reminderValue;
            if (Int32.TryParse(strReminderValue, out reminderValue))
            {
                for (int i = units.Length - 1; i >= 0; i--)
                {
                    if (reminderValue / units[i].Value >= 1 && reminderValue % units[i].Value == 0)
                    {
                        return new[] { (reminderValue / units[i].Value).ToString(), units[i].Name };
                    }
                }
            }

            return new[] {"0", "minutes"};
        }
    }
}
