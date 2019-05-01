using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class Eli_ViewCustom
    {
        public IList<OrderByItem> OrderColumns { get; set; }

        public IList<GroupItem> GroupColumns { get; set; }

        public IList<vwCustomViewColumn> Columns { get; set; }

        public IList<ColumnDisplay> ColumnsDisplay { get; set; }
    }

    public class ColumnDisplay
    {
        public int Id { get; set; }
        public bool Sortable { get; set; }
        public int SortOrder { get; set; }
        public bool AllowGroup { get; set; }
        public bool Visible { get; set; }
        public string ColumnName { get; set; }
        public string ColumnHeader { get; set; }
        public string DataTypeName { get; set; }
    }
}
