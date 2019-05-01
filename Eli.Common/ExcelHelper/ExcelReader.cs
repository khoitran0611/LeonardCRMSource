using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;

namespace Eli.Common.ExcelHelper
{
    public static class ExcelReader
    {
        private static IExcelDataReader GetExcelDataReader(string path, bool isFirstRowAsColumnNames)
        {
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader dataReader;

                if (path.EndsWith(".xls"))
                    dataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                else if (path.EndsWith(".xlsx"))
                    dataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                else
                    throw new Exception("The file to be processed is not an Excel file");

                dataReader.IsFirstRowAsColumnNames = isFirstRowAsColumnNames;
                return dataReader;
            }
        }

        public static DataSet GetExcelDataAsDataSet(string path, out string[] sheetNames, bool isFirstRowAsColumnNames = true)
        {
            var ds = GetExcelDataReader(path, isFirstRowAsColumnNames).AsDataSet();
            var sheets = "";
            var n = ds.Tables.Count;

            for (var i = 0; i < n; i++)
            {
                sheets += ds.Tables[i].TableName + ",";
            }
            sheetNames = sheets.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return ds;
        }

        public static DataSet GetExcelDataAsDataSet(string path, bool isFirstRowAsColumnNames = true)
        {
            return GetExcelDataReader(path, isFirstRowAsColumnNames).AsDataSet();
        }

        public static DataTable GetExcelWorkSheet(string path, string workSheetName, bool isFirstRowAsColumnNames = true)
        {
            return GetExcelDataAsDataSet(path, isFirstRowAsColumnNames).Tables[workSheetName];
        }

        public static DataTable GetExcelWorkSheet(DataSet excelSourceData, string workSheetName)
        {
            return excelSourceData.Tables[workSheetName];
        }

        public static IList<ExcelColumnMap> GetExcelWorkSheetColumns(DataSet excelSourceData, string workSheetName, int columnNameStartRowIndex)
        {
            columnNameStartRowIndex -= 2;
            var sheet = excelSourceData.Tables[workSheetName];
            if (sheet == null)
                return new List<ExcelColumnMap>();

            var columns = new List<ExcelColumnMap>();

            if (columnNameStartRowIndex == -1)
            {
                //case: first row contains column names
                return (from DataColumn col in sheet.Columns
                        where !string.IsNullOrEmpty(col.ColumnName.Trim()) &&
                              !col.ColumnName.ToLower().Contains("column")
                        select new ExcelColumnMap { SheetColumnName = col.ColumnName }).ToList();
            }

            for (var i = 0; i < sheet.Columns.Count; i++)
            {
                if (!string.IsNullOrEmpty(sheet.Rows[columnNameStartRowIndex][i].ToString().Trim()) &&
                    !sheet.Rows[columnNameStartRowIndex][i].ToString().ToLower().Contains("column"))
                    columns.Add(new ExcelColumnMap { SheetColumnName = sheet.Rows[columnNameStartRowIndex][i].ToString() });
            }
            return columns;
        }

        public static IEnumerable<DataRow> GetWorkSheetRows(string path, string workSheetName, bool isFirstRowAsColumnNames = true)
        {
            return from DataRow row in GetExcelWorkSheet(path, workSheetName, isFirstRowAsColumnNames).Rows select row;
        }

        public static IEnumerable<DataRow> GetWorkSheetRows(DataSet excelSourceData, string workSheetName)
        {
            var sheet = excelSourceData.Tables[workSheetName];
            if (sheet == null)
                return new ArraySegment<DataRow>();
            return from DataRow row in sheet.Rows select row;
        }
    }
}
