using System;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Text.RegularExpressions;

namespace LeonardCRM.BusinessLayer.Common
{
    public class ObjectValidator
    {
        public int ModuleId { get; set; }

        private string GetText(string key)
        {
            return LocalizeHelper.Instance.GetText("INPUT_VALIDATION_RULES", key);
        }

        public ObjectValidator(int moduleId)
        {
            ModuleId = moduleId;
        }

        public string ValidateObject(object businessObj)
        {
            var fieldTypes = EntityFieldBM.Instance.GetAllFieldsByModule(ModuleId);
            var properties = businessObj.GetType().GetProperties();
            var result = "";
            foreach (var propertyInfo in properties)
            {
                var prop = fieldTypes.SingleOrDefault(t => t.FieldName == propertyInfo.Name);
                if (prop != null && !prop.PrimaryKey)
                {
                    var s = DoValidateProperty(businessObj, prop);
                    result += s + (!String.IsNullOrEmpty(s) ? "<br>" : "");
                }
            }

            return result;
        }

        public string ValidateObject(object businessObj, string[] specifiledFields)
        {
            var result = "";

            if(specifiledFields != null && specifiledFields.Any())
            {
                var fieldTypes = EntityFieldBM.Instance.GetAllFieldsByModule(ModuleId);
                var properties = businessObj.GetType().GetProperties();

                foreach (var propertyInfo in properties)
                {
                    var prop = fieldTypes.SingleOrDefault(t => t.FieldName == propertyInfo.Name && specifiledFields.Contains(t.FieldName));
                    if (prop != null && !prop.PrimaryKey)
                    {
                        var s = DoValidateProperty(businessObj, prop);
                        result += s + (!String.IsNullOrEmpty(s) ? "<br>" : "");
                    }
                }
            }           

            return result;
        }

        public string ValidateObject(CustomField customObject)
        {
            var fieldTypes = EntityFieldBM.Instance.GetAllFieldsByModule(ModuleId);
            var properties = customObject.GetType().GetProperties().ToList();
            var result = "";
            foreach (var propertyInfo in properties)
            {
                var prop = fieldTypes.SingleOrDefault(t => t.FieldName == propertyInfo.Name);
                if (prop != null && !prop.PrimaryKey)
                {
                    var s = DoValidateProperty(customObject,prop);
                    result += s + (!String.IsNullOrEmpty(s) ? "<br>" : "");
                }
            }
            //validate for custom fileds
            if (customObject.CustomFields != null)
            {
                foreach (var customfield in customObject.CustomFields)
                {
                    var msg = DoValidateCustomField(customfield);
                    result += msg + (!string.IsNullOrEmpty(msg) ? "<br>" : "");
                }
            }
            return result;
        }

        public string ValidateObjectWithFields(CustomField customObject, string[] specifiedFiels)
        {
            if (specifiedFiels == null) return "";

            var fieldTypes = EntityFieldBM.Instance.GetAllFieldsByModule(ModuleId);
            var properties = customObject.GetType().GetProperties().Where(f => specifiedFiels.Contains(f.Name)).ToList();
            var result = "";
            foreach (var propertyInfo in properties)
            {
                var prop = fieldTypes.SingleOrDefault(t => t.FieldName == propertyInfo.Name);
                if (prop != null && !prop.PrimaryKey)
                {
                    var s = DoValidateProperty(customObject, prop);
                    result += s + (!String.IsNullOrEmpty(s) ? "<br>" : "");
                }
            }

            //validate for custom fileds
            if (customObject.CustomFields != null)
            {
                customObject.CustomFields = customObject.CustomFields.Where(f => specifiedFiels.Contains(f.FieldName)).ToList();
                foreach (var customfield in customObject.CustomFields)
                {
                    var msg = DoValidateCustomField(customfield);
                    result += msg + (!string.IsNullOrEmpty(msg) ? "<br>" : "");
                }
            }
            return result;
        }

        private string DoValidateCustomField(vwEntityFieldData vwFieldsDataType)
        {
            var value = vwFieldsDataType.FieldData;
            var returnError = "";
            if (vwFieldsDataType.IsCheckBox)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && !Utilities.IsBool(value)))
                {
                    returnError = "INVALID_BOOL";
                }
            }
            else if (vwFieldsDataType.IsDate || vwFieldsDataType.IsDateTime)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && !Utilities.IsDate(value)))
                {
                    returnError = "INVALID_DATE";
                }
            }
            else if (vwFieldsDataType.IsDecimal)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && !Utilities.IsDecimal(value)))
                {
                    returnError = "INVALID_NUMBER";
                }
            }
            else if (vwFieldsDataType.IsEmail)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && !Utilities.IsMail(value)))
                {
                    returnError = "INVALID_EMAIL";
                }
                if (value != null && !String.IsNullOrEmpty(value)
                    && vwFieldsDataType.DataLength.HasValue
                    && value.Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsInteger)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && !Utilities.IsInteger(value)))
                {
                    returnError = "INVALID_NUMBER";
                }
            }
            else if (vwFieldsDataType.IsList)
            {
                if ((vwFieldsDataType.Mandatory && (String.IsNullOrEmpty(value) || !Utilities.IsPositiveInt(value))) || (!vwFieldsDataType.Mandatory && value != null && !String.IsNullOrEmpty(value) && value != "-000-" && !Utilities.IsInteger(value)))
                {
                    returnError = "INVALID_DROPDOWNLIST";
                }
            }
            else if (vwFieldsDataType.IsMultiSelecttBox)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (!vwFieldsDataType.Mandatory && value != null && !String.IsNullOrEmpty(value) && String.IsNullOrEmpty(value)))
                {
                    returnError = "INVALID_MULTISELECT";
                }
            }
            else if (vwFieldsDataType.IsText)
            {
                if (vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value))
                {
                    returnError = "EMPTY_TEXT";
                }
                if (value != null && !String.IsNullOrEmpty(value)
                    && vwFieldsDataType.DataLength.HasValue
                    && value.Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsTextArea)
            {
                if (vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value))
                {
                    returnError = "EMPTY_TEXT";
                }
                if (vwFieldsDataType.DataLength.HasValue && value != null && !String.IsNullOrEmpty(value)
                    && value.Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsTime)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && value.IndexOf(":") <= 0))
                {
                    returnError = "INVALID_TIME";
                }
            }
            else if (vwFieldsDataType.IsUrl)
            {
                if ((vwFieldsDataType.Mandatory && String.IsNullOrEmpty(value)) || (value != null && !String.IsNullOrEmpty(value) && !Utilities.IsUrl(value)))
                {
                    returnError = "INVALID_URL";
                }
                if (value != null && !String.IsNullOrEmpty(value)
                    && vwFieldsDataType.DataLength.HasValue
                    && value.Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            if (returnError == "INVALID_MAXLENGTH")
            {
                return String.Format(GetText(returnError), vwFieldsDataType.LabelDisplay, vwFieldsDataType.DataLength);
            }
            return !String.IsNullOrEmpty(returnError) ? String.Format(GetText(returnError), vwFieldsDataType.LabelDisplay) : String.Empty;
        }

        private string DoValidateProperty(object businessObj, vwFieldsDataType vwFieldsDataType)
        {
            var value = businessObj.GetType().GetProperty(vwFieldsDataType.FieldName).GetValue(businessObj, null);
            var returnError = "";
            if (vwFieldsDataType.IsCheckBox)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && !Utilities.IsBool(value.ToString())))
                {
                    returnError = "INVALID_BOOL";
                }
            }
            else if (vwFieldsDataType.IsDate || vwFieldsDataType.IsDateTime)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && !Utilities.IsDate(value.ToString())))
                {
                    returnError = "INVALID_DATE";
                }
            }
            else if (vwFieldsDataType.IsDecimal)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && !Utilities.IsDecimal(value.ToString())))
                {
                    returnError = "INVALID_NUMBER";
                }
            }
            else if (vwFieldsDataType.IsEmail)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && !Utilities.IsMail(value.ToString())))
                {
                    returnError = "INVALID_EMAIL";
                }
                if (value != null && !String.IsNullOrEmpty(value.ToString())
                    && vwFieldsDataType.DataLength.HasValue
                    && value.ToString().Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsInteger)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && !Utilities.IsInteger(value.ToString())))
                {
                    returnError = "INVALID_NUMBER";
                }
                if (value != null && !String.IsNullOrEmpty(value.ToString())
                    && vwFieldsDataType.DataLength.HasValue
                    && value.ToString().Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsList)
            {
                if ((vwFieldsDataType.Mandatory && ((value == null || String.IsNullOrEmpty(value.ToString()) || !Utilities.IsPositiveInt(value.ToString())))) || (!vwFieldsDataType.Mandatory && value != null && !String.IsNullOrEmpty(value.ToString()) && value.ToString() != "-000-" && !Utilities.IsInteger(value.ToString())))
                {
                    returnError = "INVALID_DROPDOWNLIST";
                }
            }
            else if (vwFieldsDataType.IsMultiSelecttBox)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (!vwFieldsDataType.Mandatory && value != null && !String.IsNullOrEmpty(value.ToString()) && String.IsNullOrEmpty(value.ToString())))
                {
                    returnError = "INVALID_MULTISELECT";
                }
            }
            else if (vwFieldsDataType.IsText)
            {
                if (vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString())))
                {
                    returnError = "EMPTY_TEXT";
                }
                if (value != null && !String.IsNullOrEmpty(value.ToString())
                    && vwFieldsDataType.DataLength.HasValue
                    && value.ToString().Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsTextArea)
            {
                if (vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString())))
                {
                    returnError = "EMPTY_TEXT";
                }
                if (vwFieldsDataType.DataLength.HasValue && value != null && !String.IsNullOrEmpty(value.ToString())
                    && value.ToString().Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }
            else if (vwFieldsDataType.IsTime)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && value.ToString().IndexOf(":") <= 0))
                {
                    returnError = "INVALID_TIME";
                }
            }
            else if (vwFieldsDataType.IsUrl)
            {
                if ((vwFieldsDataType.Mandatory && (value == null || String.IsNullOrEmpty(value.ToString()))) || (value != null && !String.IsNullOrEmpty(value.ToString()) && !Utilities.IsUrl(value.ToString())))
                {
                    returnError = "INVALID_URL";
                }
                if (value != null && !String.IsNullOrEmpty(value.ToString())
                    && vwFieldsDataType.DataLength.HasValue
                    && value.ToString().Length > vwFieldsDataType.DataLength)
                {
                    returnError = "INVALID_MAXLENGTH";
                }
            }

            if (!string.IsNullOrEmpty(vwFieldsDataType.RegularExpression))
            {
                var regex = new Regex(vwFieldsDataType.RegularExpression);
                if (!regex.IsMatch(value.ToString()))
                {
                    returnError = "INVALID_FORMAT";
                }
            }

            if (returnError == "INVALID_MAXLENGTH")
            {
                return String.Format(GetText(returnError), vwFieldsDataType.LabelDisplay, vwFieldsDataType.DataLength);
            }
            return !String.IsNullOrEmpty(returnError) ? String.Format(GetText(returnError), vwFieldsDataType.LabelDisplay) : String.Empty;
        }
    }
}
