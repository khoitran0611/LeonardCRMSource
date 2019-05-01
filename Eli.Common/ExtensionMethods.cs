using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Eli.Common
{
    public static class ExtensionMethods
    {
        #region String Extension Methods
        /// <summary>
        /// Remove non-alphanumeric chars from string
        /// </summary>
        /// <param name="dirtyString"></param>
        /// <param name="stringToReplace">The char to replace with non-alphanumeric</param>
        /// <returns>Return an alphanumeric string</returns>
        public static string RemoveDirtySymbols(this string dirtyString, string stringToReplace)
        {
            string cleanString = Regex.Replace(dirtyString, "[^A-Za-z0-9]", stringToReplace);
            return cleanString;
        }

        /// <summary>
        /// Remove duplicated chars in the string
        /// </summary>
        /// <param name="duplicatedString"></param>
        /// <param name="charToRemove">The char that is duplicated in the string</param>
        /// <returns>Return a string without duplicated chars</returns>
        public static string RemoveDuplicatedSymbol(this string duplicatedString, string charToRemove)
        {
            return Regex.Replace(duplicatedString, charToRemove + "{2,}", charToRemove);
        }
        /// <summary>
        /// Trip all html tags in the input string and replace by tags by another string
        /// </summary>
        /// <param name="orginalString">The input string that contains html tags</param>
        /// <param name="charToReplace">The string to replace html tags</param>
        /// <returns></returns>
        public static string StripAllHtmlTags(this string orginalString, string charToReplace)
        {
            return Regex.Replace(orginalString, "<(.|\n)*?>", charToReplace, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checking if the whole string is alphanumric(including whitespace) chars or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsAlphaNumericString(this string str)
        {
            var arrs = str.ToCharArray();
            return arrs.All(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Checking if the string contains alphanumeric or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsAlphabet(this string str)
        {
            var arrs = str.ToCharArray();
            return arrs.Any(Char.IsLetter);
        }
        /// <summary>
        /// Ex: test app -> Test App
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCapitalize(this string str)
        {
            try
            {
                var cultureInfo = Thread.CurrentThread.CurrentCulture;
                var textInfo = cultureInfo.TextInfo;
                return textInfo.ToTitleCase(str).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Convert a Unicode string (e.g, Vietnamese) to unsign string and replace symbols by a new string
        /// </summary>
        /// <param name="stringToChange">The sign-Unicode string to convert</param>
        /// <param name="stringToReplace">The string to replace symbols</param>
        /// <returns></returns>
        public static string ConvertToUnsignUnicode(this string stringToChange, string stringToReplace)
        {
            var vRegRegex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            var vStrFormD = stringToChange.Trim().Normalize(NormalizationForm.FormD);
            vStrFormD =
                RemoveDirtySymbols(
                    vRegRegex.Replace(vStrFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D'),
                    stringToReplace);
            return RemoveDuplicatedSymbol(vStrFormD, stringToReplace);
        }

        #endregion

        #region enumeration extension methods

        /// <summary>
        /// Convert a type of enumeration to a list
        /// </summary>
        /// <param name="enumType">the enumeration to convert(using typeof(enum) as a parameter</param>
        /// <returns></returns>
        public static List<CustomStatus> ToList(this Type enumType)
        {
            var list = new List<CustomStatus>();
            var items = Enum.GetNames(enumType);
            list.AddRange(
                items.Select(
                    t =>
                    new CustomStatus()
                    {
                        Id = (Enum.Parse(enumType, t)).GetHashCode().ToString(),
                        Name = t
                    }));
            return list;
        }

        public class CustomStatus
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        #endregion
    }
}
