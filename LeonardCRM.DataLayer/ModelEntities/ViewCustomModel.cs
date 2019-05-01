using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    #region "For Display"

    public class ViewCustomDisplayModel
    {
        public int Total { get; set; }
        public List<object> Data { get; set; }
        public List<GroupItem> ListGroup { get; set; }
    }

    public class GroupItem
    {
        public string GroupName { get; set; }
        public int Total { get; set; }
    }

    #endregion

    public class ViewCustomCreatedModel
    {
        public string ViewName { get; set; }
        public string ViewCode { get; set; }
        public int ViewIdCurrent { get; set; }
        public Condition Condition { get; set; }
    }

    public class ViewCustomUpdatedModel
    {
        public Eli_ViewCustom ViewCustom { get; set; }
        public Condition Condition { get; set; }
    }

    public class Condition
    {
        public List<FilterObj> ListFilter { get; set; }
        public string OrderBy { get; set; }
        public string GroupBy { get; set; }
        public int PageSize { get; set; }
    }

    public class FilterObj
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public string DataType { get; set; }
    }

    public class OrderByItem
    {
        public string Column { get; set; }
        public string Direction { get; set; }
    }
}
