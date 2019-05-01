namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers.Exception
{
    public class ImportDuplicatedAppException : System.Exception
    {
        public string DuplicatedAppName { get; set; }

        public ImportDuplicatedAppException(string appName)
        {
            DuplicatedAppName = appName;
        }
    }
}
