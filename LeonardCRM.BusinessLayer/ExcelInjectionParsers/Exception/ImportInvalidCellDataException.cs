using Eli.Common.ExcelHelper;

namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers.Exception
{
    public class ImportInvalidCellDataException : System.Exception
    {
        public int ErrorRowIndex { get; set; }
        public ExcelColumnMap ErrorColumn { get; set; }
        public ImportInvalidCellDataException(int errorRowIndex, ExcelColumnMap errorColumn)
        {
            ErrorRowIndex = errorRowIndex;
            ErrorColumn = errorColumn;
        }
    }
}
