using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using System.Web;

namespace LeonardCRM.BusinessLayer.Feature
{
    public class OrderPdfFeature
    {
        public static string CreateContractFile(int appId)
        {
            //get the contract content
            var contractContent = SalesCustomerBM.Instance.GetContractContent(appId, LocalizeHelper.Instance.GetText("ORDERS", "PROMOTION_TEXT"), false, true, HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE));

            if (string.IsNullOrEmpty(contractContent)) return "";

            //create the pdf file           
            string tempPath = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP);
            string fileName = string.Format(Constant.ContractFileNameFormat, appId);
            string fullPath = tempPath + "\\" + fileName;
            HtmlPdfConverter.ConvertHtmlToPdf(contractContent, fullPath);
            return fullPath;
        }

        public static string CreateDeliveryFormFile(int appId, string serverUrl)
        {
            var folder = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP);
            var fileName = string.Format(Constant.DeliveryFormFileNameFormat, appId);
            var pdfFile = folder + "\\" + fileName;
            HtmlPdfConverter.ConvertWebpageToPdf(pdfFile, serverUrl + ConfigValues.DELIVERY_WEBFORM_URL + HttpUtility.UrlEncode(SecurityHelper.Encrypt(appId.ToString())));
            return pdfFile;
        }
    }
}
