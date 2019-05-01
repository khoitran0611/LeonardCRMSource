using System.Collections;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public class PageInfo
    {
        public int ViewId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string SortExpression { get; set; }
        public int TotalRow { get; set; }
        public Hashtable Models { get; set; }
        public bool AdvanceSearch { get; set; }
        public bool DefaultOrderBy { get; set; }
        public IList<ColumnExport> ViewColumns { get; set; }

        public string GroupColumn { get; set; }
        public string GroupResult { get; set; }
        public bool IsPaging { get; set; }
        
    }
}
