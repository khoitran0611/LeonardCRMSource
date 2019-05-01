using System;
using System.Data;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace Eli.Common.ExcelHelper
{
    public static class EliCsvReader
    {
        private static DataTable GetDataTabletFromCsvFile(string filePath)
        {
            try
            {
                var csvData = new DataTable();

                using (var csv = new CsvReader(new StreamReader(filePath), true))
                {
                    var fieldCount = csv.FieldCount;
                    var headers = csv.GetFieldHeaders();

                    for (var i = 0; i < fieldCount; i++)
                    {
                        csvData.Columns.Add(headers[i]);
                    }
                    
                    while (csv.ReadNextRecord())
                    {
                        var fieldData = new object[fieldCount];

                        for (var i = 0; i < fieldCount; i++)
                        {
                            fieldData[i] = csv[i];
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }

                return csvData;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        private static DataSet GetDataSetFromCsvFile(string filePath)
        {
            var ds = new DataSet();
            ds.Tables.Add(GetDataTabletFromCsvFile(filePath));
            return ds;
        }

        public static DataSet GetCsvDataAsDataSet(string path, out string[] sheetNames, bool isFirstRowAsColumnNames = true)
        {
            var ds = GetDataSetFromCsvFile(path);
            var sheets = "";
            var n = ds.Tables.Count;

            for (var i = 0; i < n; i++)
            {
                sheets += ds.Tables[i].TableName + ",";
            }
            sheetNames = sheets.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return ds;
        }
    }
}
