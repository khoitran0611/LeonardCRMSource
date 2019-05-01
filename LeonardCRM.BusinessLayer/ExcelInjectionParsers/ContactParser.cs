using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Eli.Common.ExcelHelper;

namespace EliCRM.BusinessLayer.ExcelInjectionParsers
{
    /// <summary>
    /// This class is used to import contacts from excel data file
    /// </summary>
    public class ContactParser : ParserBase
    {
        public ContactParser()
        {
        }

        public ContactParser(DataTable dataSource, IList<ExcelColumnMap> columnMaps, int columnNameRowIndex)
            : base(dataSource, columnMaps, columnNameRowIndex)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<Contact> GetContacts()
        {
            IList<Contact> contacts = new List<Contact>();
            var apps = AppBM.Instance.Find(c => c.IsDeleted == false);
            var users = UserBM.Instance.GetAll();
            var dbContacts = ContactBM.Instance.Find(c=>c.IsDeleted == false);
            //var duplicatedContacts = new List<Contact>();
            var maxContactId = dbContacts.Count > 0 ? dbContacts.Last().Id : 1;
            for (var i = ColumnNameRowIndex - 1; i < DataSource.Rows.Count; i++)
            {
                var contact = new Contact { Source = (int)AppSource.DataInject, IsDeleted = false };
                var row = DataSource.Rows[i];
                foreach (var col in ColumnMaps)
                {
                    if (String.IsNullOrEmpty(col.ObjectColumnName))
                        continue;

                    var propertyInfo = contact.GetType().GetProperty(col.ObjectColumnName);

                    switch (col.ObjectColumnName)
                    {
                        case "RelatedApps":
                            //get app by app name
                            //29-Aug: can't import contact when allowing duplicated app names -> waiting for Parrot's decision
                            var existApps = apps.Where(r => r.AppName == row[col.SheetColumnName].ToString()).ToList();
                            if (InvalidCellDataAction == 1 && existApps.Count() > 1)
                                throw new ImportDuplicatedAppException(row[col.SheetColumnName].ToString());

                            if (existApps.Count() == 1)
                                propertyInfo.SetValue(contact, existApps[0].Id + ",");
                            else
                                propertyInfo.SetValue(contact, String.Empty);

                            break;
                        case "ResponsibleUsers":
                            //get user by short name
                            var user = users.SingleOrDefault(r => r.Name == row[col.SheetColumnName].ToString());
                            if (user != null || InvalidCellDataAction == 1)
                                propertyInfo.SetValue(contact, user != null ? user.Id + "," : string.Empty);
                            else
                                throw new ImportInvalidCellDataException(i, col);
                            break;
                        default:
                            propertyInfo.SetValue(contact, row[col.SheetColumnName] != null ? row[col.SheetColumnName].ToString() : string.Empty);
                            break;
                    }
                }

                // Check Null Or Empty :  One of six properties must have value
                var result = string.IsNullOrEmpty(contact.FirstName) &&
                             string.IsNullOrEmpty(contact.LastName) &&
                             string.IsNullOrEmpty(contact.Email1) &&
                             string.IsNullOrEmpty(contact.Email2) &&
                             string.IsNullOrEmpty(contact.Email3) &&
                             string.IsNullOrEmpty(contact.Phone);

                if (!result)
                {
                    if (string.IsNullOrEmpty(contact.FirstName) &&
                        string.IsNullOrEmpty(contact.LastName))
                    {
                        contact.FirstName = string.Format("No name {0}", maxContactId);
                        maxContactId++;
                    }

                    if (dbContacts.Any(c => c.LastName == contact.LastName
                                            && c.Editor == contact.Editor && c.Email1 == contact.Email1
                                            && c.Email2 == contact.Email2 && c.Email3 == contact.Email3
                                            && c.Phone == contact.Phone && c.Address == contact.Address
                                            && c.Town == contact.Town && c.Country == contact.Country
                                            && c.RelatedApps == contact.RelatedApps &&
                                            c.ResponsibleUsers == contact.ResponsibleUsers))
                        contact.IsDuplicated = true;
                    contacts.Add(contact);
                }
                else if (EmptyRowAction == 1)
                    throw new ImportEmptyRowException(i);
            }
            return contacts;
        }

        public override int Import(IList<object> listEntities)
        {
            return ContactBM.Instance.Insert(listEntities.Cast<Contact>().ToList());
        }
    }
}
