using System.Collections.Generic;
using Eli.Common;
using Eli.Common.ExcelHelper;

namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers
{
    public class ParserData
    {
        public string CurrentSheetName { get; set; }
        public string FileName { get; set; }
        public int ColumnNameRowIndex { get; set; }
        public IList<ExcelColumnMap> ColumnMaps { get; set; }
        public IList<object> ReturnData { get; set; }
        public ResultObj ResultObj { get; set; }
        public int ParserId { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalRow { get; set; }
        public bool IsFirst { get; set; }
        public int ModuleId { get; set; }
        public int EmptyRowAction { get; set; }
        public int InvalidCellDataAction { get; set; }
        public ParserType ParserType {
            get { return (ParserType)ParserId; }
        }
    }
}
