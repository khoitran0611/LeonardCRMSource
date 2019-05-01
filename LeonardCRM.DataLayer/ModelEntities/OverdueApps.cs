using Eli.Common;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public class OverdueApps
    {
        public int Total { get; set; }
        public List<sp_GetAllOverdueApps_Result> Data { get; set; }
        public ResultCodes ReturnCode { get; set; }
        public string Message { get; set; }
    }
}
