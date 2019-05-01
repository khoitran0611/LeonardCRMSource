using System.Web;
using System;

namespace Eli.Common
{
    public static class UriHelper
    {
        private static readonly string _siteRoot = ConfigValues.SITE_ROOT;
        /// <summary>
        /// Get a full link to a page
        /// </summary>
        /// <param name="folder">The folder contains the page(the Pages class contains all folders and pages)</param>
        /// <param name="page">The page to redirect to(the Pages class contains all folders and pages)</param>
        /// <returns></returns>
        static public string GetLink(string folder, string page)
        {
            var res = _siteRoot;
            if (!String.IsNullOrEmpty(folder))
                res += string.Format("{0}/", folder);

            return string.Format("{0}{1}", res, page);
        }

        /// <summary>
        /// Get a full link to a page with query strings
        /// </summary>
        /// <param name="folder">The folder contains the page</param>
        /// <param name="page">The page to redirect to</param>
        /// <param name="queryStringFormat">A formatted query string, eg. rid={0}&uid={1}</param>
        /// <param name="args">An array of values for formatting the queryStringFormat</param>
        /// <returns></returns>
        static public string GetLink(string folder, string page, string queryStringFormat, params object[] args)
        {
            var res = _siteRoot;
            
            if (!String.IsNullOrEmpty(folder))
                res += string.Format("{0}/", folder);

            res += page;

            return string.Format("{0}/{1}", res, string.Format(queryStringFormat, args));
        }

        /// <summary>
        /// Redirect to a url
        /// </summary>
        /// <param name="url">The page to redirect to</param>
        static public void Redirect(string url)
        {
            try
            {
                HttpContext.Current.Response.Redirect(url);
            }
            catch (System.Threading.ThreadAbortException)
            {

            }
            catch
            {
            }
        }

        /// <summary>
        /// Redirect to a page in a folder
        /// </summary>
        /// <param name="folder">The folder contains the page(the Pages class contains all folders and pages)</param>
        /// <param name="page">The page to redirect to(the Pages class contains all folders and pages)</param>
        static public void Redirect(string folder, string page)
        {
            try
            {
                HttpContext.Current.Response.Redirect(GetLink(folder, page));
            }
            catch (System.Threading.ThreadAbortException)
            {

            }
            catch
            {
            }
        }

        /// <summary>
        /// Redirect to a page in a folder include query strings
        /// </summary>
        /// <param name="folder">The folder contains the page(the Pages class contains all folders and pages)</param>
        /// <param name="page">The page to redirect to(the Pages class contains all folders and pages)</param>
        /// /// <param name="queryStringFormat">A formatted query string, eg. rid={0}&uid={1}</param>
        /// /// <param name="args">An array of values for formatting the queryStringFormat</param>
        static public void Redirect(string folder, string page, string queryStringFormat, params object[] args)
        {
            try
            {
                HttpContext.Current.Response.Redirect(GetLink(folder, page, queryStringFormat, args));
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch
            { }
        }
    }
}
