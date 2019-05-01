using System;
using System.Xml;

namespace Eli.Common
{
    /// <summary>
    /// Summary description for Localizer.
    /// </summary>
    public class Localizer
    {
        private XmlDocument _doc = null;
        private XmlNode _pagePointer = null;
        private string _fileName = "";
        private string _currentPage = "";
        private string _code = "";

        public Localizer()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public Localizer(string fileName)
        {
            _fileName = fileName;
            LoadFile();
        }

        private void LoadFile()
        {
            if (_fileName == "" || !System.IO.File.Exists(_fileName))
                throw (new ApplicationException("Invalid language file " + _fileName));

            if (_doc == null)
                _doc = new XmlDocument();

            _doc.Load(_fileName);
            try
            {
                _doc.Load(_fileName);
                _code = _doc.DocumentElement.Attributes["code"] != null ? _doc.DocumentElement.Attributes["code"].Value : "en";
            }
            catch
            {
                _doc = null;
            }
        }

        public void LoadFile(string fileName)
        {
            _fileName = fileName;
            LoadFile();
        }

        public void SetPage(string Page)
        {
            if (_currentPage == Page)
                return;

            _pagePointer = null;
            _currentPage = "";

            if (_doc != null)
            {
                _pagePointer = _doc.SelectSingleNode(string.Format("//page[@name='{0}']", Page.ToUpper()));
                _currentPage = Page;
            }
        }

        public string GetText(string text)
        {
            text = text.ToUpper(new System.Globalization.CultureInfo("en"));
            if (_doc == null)
                return "";

            XmlNode el = null;

#if DEBUG
            if (_pagePointer == null)
                throw new Exception("Missing page pointer: " + text);
#endif

            if (_pagePointer != null)
            {
                el = _pagePointer.SelectSingleNode(string.Format("Resource[@tag='{0}']", text)) ??
                     _doc.SelectSingleNode(string.Format("//Resource[@tag='{0}']", text));
                // if in page subnode the text doesn't exist, try in whole file
            }
            else
                el = _doc.SelectSingleNode(string.Format("//Resource[@tag='{0}']", text));

            if (el != null)
                return el.InnerText;
            return null;
        }

        public string GetText(string page, string text)
        {
            SetPage(page);
            return GetText(text);
        }

        public string LanguageCode
        {
            get
            {
                return _code;
            }
        }
    }
}
