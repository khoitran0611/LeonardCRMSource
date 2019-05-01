using System.Collections.Generic;
using System.Data;
using System.Linq;
using Eli.Common.ExcelHelper;

namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers
{
    public enum ParserType
    {   
        Customer = 1,    //import customer
        SalesAppointment = 2
    }

    public class ParserRepository
    {
        public IList<object> ConvertObjects(ParserType type, DataTable dataSource, IList<ExcelColumnMap> columnMaps, int columnNameRowIndex,
            int emptyRowAction, int invalidCellDataAction, int moduleId)
        {
            switch (type)
            {
                  case ParserType.Customer:
                    return new CustomerParser(dataSource, columnMaps, columnNameRowIndex, moduleId)
                    {
                        EmptyRowAction = emptyRowAction,
                        InvalidCellDataAction = invalidCellDataAction
                    }.GetCustomers().Cast<object>().ToList();

                default:
                    return new List<object>();
            }
        }
        
        public int ImportData(ParserData parserData)
        {
            switch (parserData.ParserType)
            {
                case ParserType.Customer:
                    return new CustomerParser().Import(parserData.ReturnData);
                default:
                    return 0;
            }
        }
    }
}
