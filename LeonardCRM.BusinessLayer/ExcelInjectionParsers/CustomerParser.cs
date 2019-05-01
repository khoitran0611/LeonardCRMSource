using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Eli.Common;
using Eli.Common.ExcelHelper;
using LeonardCRM.BusinessLayer.ExcelInjectionParsers.Exception;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers
{
    /// <summary>
    /// This class is used to import contacts from excel data file
    /// </summary>
    public class CustomerParser : ParserBase
    {
        public IList<vwFieldsDataType> VmFields { get; set; }
       
        public CustomerParser()
        {
        }

        public CustomerParser(DataTable dataSource, IList<ExcelColumnMap> columnMaps, int columnNameRowIndex, int moduleId)
            : base(dataSource, columnMaps, columnNameRowIndex,moduleId)
        {
            VmFields = EntityFieldBM.Instance.GetAllFieldsByModule(moduleId).Where(f => f.Display).ToList();
        }

        /// <summary>
        /// Get Customers
        /// </summary>
        public IList<SalesCustomer> GetCustomers()
        {
            try
            {
                IList<SalesCustomer> customers = new List<SalesCustomer>();
                var users = UserBM.Instance.GetAll();
                var dbCustomers = SalesCustomerBM.Instance.Find(a => a.IsActive);
                var maxContactId = dbCustomers.Count > 0 ? dbCustomers.Last().Id : 1;
                var listValues = ListNameBM.Instance.GetAllListNameValues();

                for (var i = ColumnNameRowIndex - 1; i < DataSource.Rows.Count; i++)
                {
                    var customer = new SalesCustomer();
                    var row = DataSource.Rows[i];
                    customer.FieldData = new List<Eli_FieldData>();
                    customer.CustomFields = new List<vwEntityFieldData>();
                    foreach (var col in ColumnMaps)
                    {
                        if (String.IsNullOrEmpty(col.ObjectColumnName))
                            continue;

                        var propertyInfo = customer.GetType().GetProperty(col.ObjectColumnName);
                        var customfield = VmFields.SingleOrDefault(a => a.FieldName == col.ObjectColumnName);
                        if (customfield.Deletable == false) //is not custom field
                        {
                            try
                            {
                                switch (col.ObjectColumnName)
                                {
                                    case "ResponsibleUsers":
                                        var colValue = row[col.SheetColumnName];
                                        if (colValue != null)
                                        {
                                            var strIds = string.Empty;
                                            char[] charsToTrim = {','};
                                            var arrValue = colValue.ToString()
                                                .Split(charsToTrim, StringSplitOptions.RemoveEmptyEntries);
                                            foreach (
                                                var us in
                                                    from s in arrValue
                                                    where !string.IsNullOrEmpty(s)
                                                    select users.Where(r => arrValue.Contains(r.Name)).ToList())
                                            {
                                                if (!us.Any())
                                                    throw new ImportInvalidCellDataException(i, col);
                                                strIds += us[0].Id + ",";
                                            }
                                            propertyInfo.SetValue(customer, strIds);
                                        }
                                        else
                                            propertyInfo.SetValue(customer, String.Empty);
                                        break;
                                    case "Country":
                                        if (row[col.SheetColumnName] != null &&
                                            !String.IsNullOrEmpty(row[col.SheetColumnName].ToString()))
                                        {
                                            GetListValueForProperty(listValues, row, col, propertyInfo,
                                                customer, "Country", ModuleId);
                                        }
                                        break;
                                    case "CreatedDate":
                                        try
                                        {
                                            var date = DateTime.Now;
                                            propertyInfo.SetValue(customer, date);
                                        }
                                        catch (System.Exception)
                                        {
                                            var a = 2;
                                        }

                                        break;
                                    case "ModifiedDate":
                                        try
                                        {
                                            var date = DateTime.Now;
                                            propertyInfo.SetValue(customer, date);
                                        }
                                        catch (System.Exception)
                                        {
                                            var a = 1;
                                        }

                                        break;
                                    case "ModifiedBy":
                                        colValue = row[col.SheetColumnName];
                                        if (colValue != null)
                                        {
                                            var user = users.SingleOrDefault(r => colValue.ToString().Trim() == r.Name);
                                            if (user != null)
                                                propertyInfo.SetValue(customer, user.Id);
                                        }

                                        break;
                                    case "CreatedBy":
                                        colValue = row[col.SheetColumnName];
                                        if (colValue != null)
                                        {
                                            var user = users.SingleOrDefault(r => colValue.ToString().Trim() == r.Name);
                                            if (user != null)
                                                propertyInfo.SetValue(customer, user.Id);
                                        }

                                        break;
                                    default:
                                        try
                                        {
                                            if (!String.IsNullOrEmpty(row[col.SheetColumnName].ToString()))
                                                propertyInfo.SetValue(customer,
                                                    Convert.ChangeType(row[col.SheetColumnName].ToString(),
                                                        propertyInfo.PropertyType));
                                        }
                                        catch (System.Exception ex)
                                        {
                                            LogHelper.Log(ex.Message, ex);
                                        }

                                        break;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                LogHelper.Log("Not CustomeField:"+ex.Message,ex);
                            }
                        }
                        else //customer field
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(row[col.SheetColumnName].ToString()) && !customfield.Mandatory)
                                    continue;
                                //add value of custome value
                                var cell = row[col.SheetColumnName].ToString(); // cell data
                                var cellData = cell.Trim().ToLower();
                                int moduleId = ModuleId; //fix moduleId if change must change module Id
                                var fieldData = new Eli_FieldData
                                {
                                    Id = customfield.FieldId,
                                    FieldData = row[col.SheetColumnName].ToString(),
                                    CustFieldId = customfield.FieldId
                                };

                                if (customfield.IsList && customfield.ForeignKey == false)
                                {

                                    var pickListObject = listValues.SingleOrDefault(r => r.ModuleId == moduleId &&
                                                                                         r.FieldName ==
                                                                                         col.SheetColumnName &&
                                                                                         r.Description.ToLower() ==
                                                                                         cellData);
                                    if (pickListObject == null)
                                        throw new ImportInvalidCellDataException(i, col);
                                    fieldData.FieldData = pickListObject.Id.ToString(CultureInfo.InvariantCulture);
                                }
                                else if (customfield.IsMultiSelecttBox && customfield.ForeignKey == false)
                                {
                                    var pickListObject =
                                        listValues.Where(r => r.FieldName == col.SheetColumnName).ToList();
                                    var nameArray = cellData.Split(',').Select(r => r.Trim().ToLower()).ToArray();
                                    var idArray =
                                        pickListObject.Where(r => nameArray.Contains(r.Description.Trim().ToLower()))
                                            .Select(r => r.Id).ToArray();
                                    if (nameArray.Length != idArray.Length)
                                        throw new ImportInvalidCellDataException(i, col);

                                    fieldData.FieldData = idArray.Count() > 0
                                        ? String.Join(",", idArray) + ","
                                        : string.Empty;

                                }
                                else if (customfield.IsDate || customfield.IsDateTime)
                                {
                                    if (!Utilities.IsDate(cell))
                                        throw new ImportInvalidCellDataException(i, col);
                                    if (customfield.IsDate)
                                    {
                                        object objValue = cell;
                                        var dt = (DateTime) Convert.ChangeType(objValue, typeof (DateTime));
                                        fieldData.FieldData = dt.ToShortDateString();
                                    }
                                    else
                                    {
                                        fieldData.FieldData = cell;
                                    }
                                }
                                else if (customfield.IsTime)
                                {
                                    if (!Utilities.IsDate(cell))
                                        throw new ImportInvalidCellDataException(i, col);
                                    var dateTime = DateTime.Parse(cell);
                                    fieldData.FieldData = dateTime.ToString("hh:mm");
                                }
                                else
                                {
                                    fieldData.FieldData = cellData;
                                }
                                fieldData.CustFieldId = customfield.FieldId;
                                customer.FieldData.Add(fieldData);

                                //Add custome field so that check validate data type
                                var vmEntityFildData = new vwEntityFieldData
                                {
                                    FieldData = fieldData.FieldData,
                                    DataTypeId = customfield.DataTypeId,
                                    ModuleId = customfield.ModuleId,
                                    LabelDisplay = customfield.LabelDisplay,
                                    FieldName = customfield.FieldName,
                                    IsDecimal = customfield.IsDecimal,
                                    IsCheckBox = customfield.IsCheckBox,
                                    IsCurrency = customfield.IsCurrency,
                                    IsDate = customfield.IsDate,
                                    IsEmail = customfield.IsEmail,
                                    IsUrl = customfield.IsUrl,
                                    IsInteger = customfield.IsInteger,
                                    IsList = customfield.IsList,
                                    IsMultiSelecttBox = customfield.IsMultiSelecttBox,
                                    IsText = customfield.IsText,
                                    IsTextArea = customfield.IsTextArea,
                                    IsTime = customfield.IsTime
                                };

                                customer.CustomFields.Add(vmEntityFildData);
                            }
                            catch (System.Exception ex)
                            {
                                LogHelper.Log("CustomeField:" + ex.Message,ex);
                            }
                        }

                    }

                    // Check Null Or Empty :  One of properties must have value
                    var result = string.IsNullOrEmpty(customer.Email) &&
                                 string.IsNullOrEmpty(customer.Name);

                    if (!result)
                    {
                        customer.IsDuplicated = false;
                        if (string.IsNullOrEmpty(customer.Name))
                        {
                            customer.Name = string.Format("No name {0}", maxContactId);
                            maxContactId++;
                        }

                        if (dbCustomers.Any(c => c.Name == customer.Name.Trim()
                                                 && c.Email == customer.Email.Trim()))
                            customer.IsDuplicated = true;
                        customers.Add(customer);
                    }
                    else if (EmptyRowAction == 1)
                        throw new ImportEmptyRowException(i);
                }
                return customers;
            }
            catch (System.Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<SalesCustomer>();
            }
        }

        private static void GetListValueForProperty(IEnumerable<vwListNameValue> listValues
            , DataRow row
            , ExcelColumnMap col
            , PropertyInfo propertyInfo
            , SalesCustomer cus
            , string listName
            , int moduleId)
        {
            try
            {
                var obj =
               listValues.FirstOrDefault(r => r.FieldName == listName
                   && r.ModuleId == moduleId
                   && r.Description.ToLower().Replace(" ", "") == row[col.SheetColumnName].ToString().ToLower().Replace(" ", ""));

                if (obj != null)
                {
                    propertyInfo.SetValue(cus, obj.Description);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
            }
        }

        public override int Import(IList<object> listEntities)
        {
            return SalesCustomerBM.Instance.Insert(listEntities.Cast<SalesCustomer>().ToList());
        }
    }
}
