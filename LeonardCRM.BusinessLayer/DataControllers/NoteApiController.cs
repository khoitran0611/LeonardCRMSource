using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class NoteApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("NOTE", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }
        [HttpPost]
        public virtual IList<Eli_Notes> GetNotesByRecordId([FromBody] JObject jsonObject)
        {
            try
            {
                var note = JsonConvert.DeserializeObject<Eli_Notes>(jsonObject.ToString());

                return NoteBM.Instance.GetNoteByRecordId(note.ModuleId, note.RecordId, note.IsActive);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<Eli_Notes>();
            }
        }

        [HttpPost]
        public virtual ResultObj EditNote([FromBody] JObject jsonObject)
        {
            try
            {
                var entity = JsonConvert.DeserializeObject<Eli_Notes>(jsonObject.ToString());
                entity = SetValues(entity);
                string msg = ValidateNote(entity);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = NoteBM.Instance.SaveNote(entity);
                    return status > 0 ? new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),0) 
                                      : new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"),0);
                }
                return new ResultObj(ResultCodes.ValidationError,msg,0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"),0);
            }
        }

        [HttpGet]
        public virtual ResultObj DeleteNote([FromUri] int id,[FromUri] int mid)
        {
            try
            {
                var status = NoteBM.Instance.DeleteNote(id,mid);
                return status > 0 ? new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0) 
                                  : new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"),0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"),0);
            }
        }

        private string ValidateNote(Eli_Notes entity)
        {
            var msg = string.Empty;
            if (entity.Id > 0)
            {
                var note = NoteBM.Instance.Single(r => r.Id == entity.Id && r.ModuleId == entity.ModuleId);
                if (note == null)
                    msg += GetText("NOT_EXIST_NOTE") + "<br>";
            }
            if (string.IsNullOrEmpty(entity.Description))
            {
                msg += GetText("NOT_EXIST_NOTE") + "<br>";
            }

            if (entity.NoteDate <= DateTime.MinValue || entity.NoteDate >= DateTime.MaxValue)
            {
                msg += GetText("MISSING_NOTE_DATE") + "<br>";
            }
            return msg;
        }

        private string StripHtml(string source)
        {
            //get rid of HTML tags
            var output = Regex.Replace(source, "<[^>]*>", string.Empty);
            return output;
        }

        private Eli_Notes SetValues(Eli_Notes entity)
        {
            entity.Description = StripHtml(entity.Description);
            SetAuditFields(entity, entity.Id);
            return entity;
        }
    }
}
