using System.Data;
using System.Web.SessionState;
using System.Web;

namespace Eli.Common
{
    public static class SessionManager
    {
        private static HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        private const string preferLanguage = "preferLanguage";
        public static string PreferLanguage
        {
            get { return Session[preferLanguage] != null ? (string)Session[preferLanguage] : string.Empty; }
            set { Session[preferLanguage] = value; }
        }

        private const string currentEditingLanguageCode = "en";
        public static string CurrentEditingLanguageCode
        {
            get { return (string)Session[currentEditingLanguageCode]; }
            set { Session[currentEditingLanguageCode] = value; }
        }

        private const string refresh = "Refresh";
        public static string Refresh
        {
            get { return (string)Session[refresh]; }
            set { Session[refresh] = value; }
        }
        private const string dataset = "dataset";
        public static DataSet DataSet
        {
            get { return Session[dataset] as DataSet; }
            set { Session[dataset] = value; }
        }
    }

}
