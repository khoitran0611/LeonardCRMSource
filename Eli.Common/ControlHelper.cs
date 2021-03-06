using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;

namespace Eli.Common
{
    public static class ControlHelper
    {
        public const string FolderLanguage = "languages";
        private const string ForderThemes = "themes";

        public static void Report(Label lbInfo, bool isOk, string ifOk, string ifFailed)
        {
            lbInfo.Visible = true;
            if (isOk)
            {
                lbInfo.Text = ifOk;
                lbInfo.CssClass = "messagebox";
            }
            else
            {
                lbInfo.Text = ifFailed;
                lbInfo.CssClass = "errorbox";
            }
        }

        public static void PopulateListControlFromList(ListControl control, IList list, string dataText, string dataValue)
        {
            control.Items.Clear();
            control.DataSource = list;
            control.DataTextField = dataText;
            control.DataValueField = dataValue;
            control.DataBind();
        }

        public static void PopulateListControlFromEnumList(ListControl control, Type enumType)
        {
            control.Items.Clear();
            string[] items = Enum.GetNames(enumType).OrderBy(s=>s).ToArray();
            foreach (string item in items)
            {
                control.Items.Add(new ListItem(item, Enum.Parse(enumType, item).GetHashCode().ToString()));
            }
        }

        public static List<string> GetValuesSelected(ListControl control)
        {
            var valuesSelected = new List<string>();
            for (int index = 0; index < control.Items.Count; index++)
            {
                if (control.Items[index].Selected)
                {
                    valuesSelected.Add(control.Items[index].Value);
                }
            }
            return valuesSelected;
        }

        public static void SetValuesSelected(ListControl control, string[] values)
        {
            foreach (string value in values)
            {
                ListItem item = control.Items.FindByValue(value);
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        public static void RegisterScript(HiddenField hfTopicIdValue, Page page, Type csType)
        {
            ClientScriptManager cs = page.ClientScript;
            string csChangeTopic = "ChangeTopic";
            if (!cs.IsClientScriptBlockRegistered(csType, csChangeTopic))
            {
                var sb = new StringBuilder();
                sb.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
                sb.AppendLine("function ChangeTopic(topicID){");
                sb.AppendLine("var hfTopicIDValue=document.getElementById('" + hfTopicIdValue.ClientID + "');");
                sb.AppendLine("hfTopicIDValue.value = topicID.toString();");
                sb.AppendLine("}");
                sb.AppendLine("</script>");
                cs.RegisterClientScriptBlock(csType, csChangeTopic, sb.ToString());
            }
        }

        public static void SetLanguageSelectedIndex(LanguageCodes languageCode, DropDownList ddlLanguage)
        {
            switch (languageCode)
            {
                case LanguageCodes.en:
                    ddlLanguage.SelectedIndex = 0;
                    break;
                case LanguageCodes.vi:
                    ddlLanguage.SelectedIndex = 1;
                    break;
            }
        }

        public static LanguageCodes GetLanguageCode(string languageCodeString)
        {
            var ret = LanguageCodes.vi; ;
            switch (languageCodeString)
            {
                case "en":
                    ret = (LanguageCodes)Enum.Parse(typeof(LanguageCodes), languageCodeString);
                    break;
                case "en-US":
                    ret = LanguageCodes.en;
                    break;
            }
            return ret;
        }

        public static List<ThemeData> Themes()
        {
            //return LoadFilesFromFolder(ForderThemes, "theme");
            return new List<ThemeData>();
        }

        public static List<ThemeData> Languages()
        {
            return LoadFilesFromFolder(FolderLanguage, "language");
        }

        private static List<ThemeData> LoadFilesFromFolder(string folder, string attribute)
        {
            var dt = new List<ThemeData>();
            var dir = new System.IO.DirectoryInfo(System.Web.HttpContext.Current.Request.MapPath(String.Format("{0}{1}", ConfigValues.SITE_ROOT, folder)));
            System.IO.FileInfo[] files = dir.GetFiles("*.xml");
            foreach (System.IO.FileInfo file in files)
            {
                if (file.Name != "default.xml")
                {
                    try
                    {
                        var doc = new System.Xml.XmlDocument();
                        doc.Load(file.FullName);

                        var dr = new ThemeData
                            {
                                Theme = doc.DocumentElement.Attributes[attribute].Value,
                                FileName = file.Name,
                            };
                        if (attribute.Equals("language"))
                            dr.Code = doc.DocumentElement.Attributes["culture"].Value;
                        dt.Add(dr);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return dt;
        }

        public class ThemeData
        {
            public string Theme { get; set; }
            public string FileName { get; set; }
            public string Code { get; set; }
        }

    }
}
