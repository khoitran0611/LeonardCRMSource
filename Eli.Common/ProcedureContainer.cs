namespace Eli.Common
{
    public static class ProcedureContainer
    {
        #region Store procedures
        
        #endregion

        #region Queries
        public const string ClearLog = "truncate table Eli_Log";
        public const string GetLogView = "select Id, Date, Message from Log order by Date desc";
        #endregion
    }
}
