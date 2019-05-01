using System;
using System.Web;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Collections.Generic;
using System.Web.Http;
using Eli.Common;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SalesDocumentApiController : BaseApiController
    {
        #region API methods
        
        [HttpGet]
        public virtual IList<SalesDocument> GetAttachmentsByItemId(int appId)
        {           
            try
            {
                return SalesDocumentsBM.Instance.Find(x => x.OrderId == appId).ToList();
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new List<SalesDocument>();
            }
        }

        [HttpPost]
        public virtual ResultObj SaveAttachment([FromUri]int appId, [FromUri] bool isOnlyAdd, [FromBody]List<SalesDocument> attachment)
        {
            try
            {
                var folderPath = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SALE_DOCUMENT);
                SetAttachmentObjects(attachment, appId);
                var msg = ValidateAttachment(attachment, folderPath, appId);

                if (string.IsNullOrEmpty(msg))
                {                    
                    var status = SalesDocumentsBM.Instance.SaveAttachment(appId, attachment, folderPath, isOnlyAdd);
                    if (status > 0)
                    {
                        return new ResultObj(ResultCodes.Success, LocalizeHelper.Instance.GetText("COMMON", "SAVE_SUCCESS_MESSAGE"));
                    }

                    return new ResultObj(ResultCodes.SavingFailed, LocalizeHelper.Instance.GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"));
                }

                return new ResultObj(ResultCodes.ValidationError, msg);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, LocalizeHelper.Instance.GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"));
            }
        }

        [HttpPost]
        public virtual ResultObj UploadBase64PNG([FromBody] JObject base64Data)
        {
            try
            {
                var data = base64Data.Property("data") != null ? base64Data.Property("data").Value.ToString() : "";
                if (!string.IsNullOrEmpty(data))
                {                    
                    var folderName = ConfigValues.UPLOAD_DIRECTORY_SALE_DOCUMENT;
                    var filePath = folderName + (!folderName.EndsWith("/") ? "/" : "") + string.Format(Constant.SnapShotNameFormat, DateTime.Now.Ticks);

                    var responsePath = filePath.Replace(ConfigValues.UPLOAD_DIRECTORY + (!ConfigValues.UPLOAD_DIRECTORY.EndsWith("/") ?  "/" : ""), "");
                    var physicalPath = HttpContext.Current.Server.MapPath(filePath);

                    ImageHelper.SaveImageFromBase64(data, physicalPath);
                    return new ResultObj(ResultCodes.Success, responsePath);
                }

                return new ResultObj(ResultCodes.ValidationError, LocalizeHelper.Instance.GetText("APPLICANT_FORM", "UPLOAD_BASE_64_PNG_FAIL"));
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, LocalizeHelper.Instance.GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"));
            }
        }

        #endregion

        #region Internal methods

        private void SetAttachmentObjects(List<SalesDocument> attachments, int appId)
        {
            if (attachments != null && attachments.Any())
            {
                var uploadedFolder = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY);

                foreach (var att in attachments)
                {
                    att.OrderId = appId;
                    SetAuditFields(att, att.Id);

                    if (!string.IsNullOrEmpty(att.Folder) && !att.FileName.Contains(att.Folder))
                    {
                        var snapShotFileName = string.Format(Constant.SnapShotNameFormat, DateTime.Now.Ticks);
                        ImageHelper.SaveImageFromBase64(att.FileName, uploadedFolder + (!uploadedFolder.EndsWith("\\") ? "\\" : "") +  
                                                                                       (!string.IsNullOrEmpty(att.Folder) ? (att.Folder + "\\") : "") + 
                                                                                       snapShotFileName);
                        att.FileName = att.Folder + "/" + snapShotFileName;
                    }
                }
            }
        }

        private string ValidateAttachment(List<SalesDocument> attachment, string folderPath, int appId)
        {
            var msg = "";
            var fileNames = attachment.Where(x => x.Id == 0).Select(x => x.FileName);
            var count = SalesDocumentsBM.Instance.Count(x => fileNames.Contains(x.FileName) && x.OrderId == appId);
            if (count > 0)
            {
                msg += LocalizeHelper.Instance.GetText("APPLICANT_FORM", "UPLOAD_DUPLICATE_ATTACHMENT_ERROR_MSG");
            }

            return msg;
        }

        #endregion
    }

}
