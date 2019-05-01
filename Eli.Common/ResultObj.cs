/*
 * This class is a standard return value for methods of this program.
 * There are 2 parts:
 * ReturnCode: returns an integer value that indicates errors or success
 * Result: the data of the result, may be a value or a list of values ...
 * 
 * */

namespace Eli.Common
{
    public class ResultObj
    {
        public ResultObj(ResultCodes code, object result, int id = 0)
        {
            ReturnCode = code;
            Result = result;
            Id = id;
        }

        public ResultCodes ReturnCode { get; set; }
        public object Result { get; set; }
        public int Id { get; set; }
        public string Names { get; set; }
    }

    public class OrderResultObj : ResultObj
    {
        public OrderResultObj(ResultCodes code, object result, int id, bool isValidVat)
            : base(code, result, id)
        {
            IsValidVat = isValidVat;
        }

        public bool IsValidVat { get; set; }
    }

    public enum ResultCodes
    {
        Success = 200,
        SavingFailed = 500,
        ValidationError = 401,
        UnkownError = 999
    }
}
