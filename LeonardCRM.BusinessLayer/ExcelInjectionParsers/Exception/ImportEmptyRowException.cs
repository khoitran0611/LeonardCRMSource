namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers.Exception
{
    public class ImportEmptyRowException : System.Exception
    {
        public int ErrorRowIndex { get; set; }
        public ImportEmptyRowException(int errorRowIndex)
        {
            ErrorRowIndex = errorRowIndex;
        }
    }
}
