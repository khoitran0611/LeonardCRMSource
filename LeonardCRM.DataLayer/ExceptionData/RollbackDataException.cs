using System;
using Eli.Common;

namespace LeonardCRM.DataLayer.ExceptionData
{
    public class RollbackDataException : Exception
    {
        public RollBackStatus Key { get; set; }
        public string ResultMsg { get; set; }
        public Exception RollbackFailureException { get; set; }
        public Exception RollbackSuccessException { get; set; }
        public RollbackDataException(RollBackStatus key, string returnMessage)
        {
            Key = key;
            ResultMsg = returnMessage;
        }
    }
}
