using System.Collections.Generic;
using System.Data;
using System.Linq;
using Eli.Common.ExcelHelper;

namespace LeonardCRM.BusinessLayer.ExcelInjectionParsers
{
    public abstract class ParserBase
    {
        /// <summary>
        /// Data source which is used for converting to a list of contacts of apps
        /// </summary>
        public DataTable DataSource { get; set; }
        /// <summary>
        /// A list of mapping columns, between table column names and entity fields
        /// </summary>
        public IList<ExcelColumnMap> ColumnMaps { get; set; }
        /// <summary>
        /// The row index that contains column names
        /// </summary>
        public int ColumnNameRowIndex { get; set; }
        /// <summary>
        /// 0: Ignore and continue if there are any empty rows - 1: Error and stop processing
        /// </summary>
        public int EmptyRowAction { get; set; }
        /// <summary>
        /// 0: Ignore and continue if there are any empty rows - 1: Error and stop processing
        /// </summary>
        public int InvalidCellDataAction { get; set; }

        public int ModuleId { get; set; }
        protected ParserBase()
        {
        }

        protected ParserBase(DataTable dataSource, IList<ExcelColumnMap> columnMaps, int columnNameRowIndex, int moduleId=0)
        {
            DataSource = dataSource;
            ColumnMaps = columnMaps;
            ColumnNameRowIndex = columnNameRowIndex;
            EmptyRowAction = 0;
            InvalidCellDataAction = 1;
            ModuleId = moduleId;
            if (columnNameRowIndex > 1)
            {
                columnMaps = columnMaps.GroupBy(r => r.SheetColumnName).Select(r => r.First()).ToList();
                var startColumnIndex = 0;
                for (var i = 0; i < DataSource.Columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(DataSource.Rows[columnNameRowIndex - 2][i].ToString().Trim()))
                    {
                        startColumnIndex = i;
                        break;
                    }
                }
                for (var i = startColumnIndex; i < columnMaps.Count + startColumnIndex; i++)
                {
                    try
                    {
                        DataSource.Columns[i].ColumnName = columnMaps[i - startColumnIndex].SheetColumnName;
                    }
                    catch (DuplicateNameException)
                    {
                    }
                }
            }
        }

        public abstract int Import(IList<object> listEntities);
    }
}
