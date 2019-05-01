using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LeonardCRM.BusinessLayer.Helper
{
    public static class ConvertSignatureXml
    {
        public static string ConvertSignature(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            var inputData = HttpUtility.HtmlDecode(s);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(inputData);
            XmlElement xelRoot = xd.DocumentElement;
            var listNodes = xelRoot.SelectNodes("body");

            if (listNodes == null || listNodes.Count == 0)
            {
                return "";
            }

            return listNodes.Item(0).InnerXml;
        }
    }
}
