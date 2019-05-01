using System;
using System.Linq;
using DbExtensions;
using Eli.Common;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;

namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewBuilder
    {
        private static volatile ViewBuilder _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewBuilder Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new ViewBuilder();
                }
                return _instance;
            }
        }

        public string CreateSqlView(Eli_Views entity)
        {
            var lstFieldidVisible = entity.Eli_ViewColumns.Where(f => f.Visible).Select(f => f.FieldId).Concat(entity.Eli_ViewConditions.Select(c => c.FieldId));

            var datasource = ModuleDA.Instance.GetModuleRelationship();
            var columnList = ModuleDA.Instance.GetModuleEntityByModuleId(entity.ModuleId).Where(r=> lstFieldidVisible.Contains(r.Id) || Constant.RequireFields.Contains(r.FieldName.ToLower()));
            var displayIds = columnList.Where(r => r.Display || r.FieldName == "Id" || lstFieldidVisible.Contains(r.Id)).Select(r => r.Id).ToList();
            var columns = entity.Eli_ViewColumns.Where(r => displayIds.Contains(r.FieldId)).ToList();
            var ids = columns.Select(r => r.FieldId);
            if (columns.Count > 0)
            {
                var query = new SqlBuilder();
                //contain datatype
                var cols = columnList.Where(r => ids.Contains(r.Id)).ToList();
                //get foreignkey column
                var fkColumns = cols.Where(r => r.ForeignKey.HasValue && r.ForeignKey.Value).Distinct().ToList();
                //get parent column
                var parentColumns = datasource.Where(r => fkColumns.Select(rc => rc.Id).Contains(r.ChildId)).ToList();
                string str;

                //For Select columns
                query.SELECT();
                foreach (var result in cols.Where(r => (r.ForeignKey.HasValue && !r.ForeignKey.Value) || r.ForeignKey == null))
                {
                    if (result.Deletable.HasValue && result.Deletable.Value)
                    {
                        if (result.IsInteger)
                        {
                            str = string.Format(" convert(int,pv.[{0}]) as [{0}] ", result.FieldName);
                        }
                        else if (result.IsCheckBox)
                        {
                            str = string.Format(" convert(bit,pv.[{0}]) as [{0}] ", result.FieldName);
                        }
                        else if (result.IsDate)
                        {
                            str = string.Format(" convert(date,pv.[{0}]) as [{0}] ", result.FieldName);
                        }
                        else if (result.IsDateTime)
                        {
                            str = string.Format(" convert(datetime,pv.[{0}]) as [{0}] ", result.FieldName);
                        }
                        else if (result.IsCurrency || result.IsDecimal)
                        {
                            str = string.Format(" convert(decimal(18, 2),pv.[{0}]) as [{0}] ", result.FieldName);
                        }
                        else if (result.IsList)
                        {
                            str = string.Format(" [dbo].[fn_GetNameInListNameValuesById](pv.{0}) as [{0}] ", result.FieldName);
                        }
                        else if (result.IsMultiSelecttBox)
                        {
                            str = string.Format(" [dbo].[fn_GetNamesInListValuesById](pv.[{0}]) as [{0}] ", result.FieldName);
                        }
                        else
                        {
                            str = string.Format(" ISNULL(pv.[{0}],'') as [{0}] ", result.FieldName);
                        }
                    }
                    else
                    {
                        if (result.IsList)
                        {
                            str = string.Format(" [dbo].[fn_GetNameInListNameValuesById]([{0}].[{1}]) as [{1}] ", result.DefaultTable, result.FieldName);
                        }
                        else if (result.IsMultiSelecttBox)
                        {
                            str = string.Format(" [dbo].[fn_GetNamesInListValuesById]([{0}].{1}) as {1} ", result.DefaultTable, result.FieldName);
                        }
                        else if (result.IsDecimal || result.IsInteger || result.IsList || result.IsCurrency || result.IsDate || result.IsDateTime)
                        {
                            str = string.Format(" [{0}].[{1}] as [{1}] ", result.DefaultTable, result.FieldName);
                        }
                        else
                        {
                            str = string.Format(" ISNULL([{0}].[{1}],'') as [{1}] ", result.DefaultTable, result.FieldName);
                        }
                    }
                    query._(str);
                }
                foreach (var result in parentColumns)
                {
                    var col = columnList.Single(r => r.Id == result.ChildId);

                    if (col.IsMultiSelecttBox)
                    {
                        str = string.Format(" (STUFF((Select ',' + [{5}].[{0}] From [{1}] as [{5}]  Where [{5}].[{2}] in  (SELECT VALUE FROM [dbo].SPLIT(',',[{3}].[{4}])) For XML PATH('')), 1, 1, '')) as [{4}] ", result.MasterDisplayColumn,
                            result.MasterDefaultTable, result.MasterFieldName, result.ChildDefaultTable, result.ChildFieldName, !string.IsNullOrEmpty(result.MasterAlias) ? result.MasterAlias : result.MasterDefaultTable);
                    }
                    else
                    {
                        str = string.Format(" [{0}].{1} as [{2}] ", result.MasterAlias, result.MasterDisplayColumn, result.ChildFieldName);
                    }
                    query._(str);
                }
                //For RowIndex & Order By
                query._(" ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex ");

                //For From Table
                var tableList = columnList.Select(r => new { r.ModuleId, r.DefaultTable }).Distinct().ToList();
                str = tableList.Single(r => r.ModuleId == entity.ModuleId).DefaultTable;
                query.FROM(string.Format(" {0} ", str));
                foreach (var column in parentColumns)
                {
                    var col = columnList.Single(r => r.Id == column.ChildId);
                    if (col.IsMultiSelecttBox) continue;
                    var result = columnList.Single(r => r.Id == column.ChildId).Mandatory;
                    if (result)
                        query.INNER_JOIN(string.Format(" [{0}] as [{1}] on [{1}].[{2}] = [{3}].[{4}] ", column.MasterDefaultTable, column.MasterAlias, column.MasterFieldName, column.ChildDefaultTable, column.ChildFieldName));
                    else
                        query.LEFT_JOIN(string.Format(" [{0}] as [{1}] on [{1}].[{2}] = [{3}].[{4}] ", column.MasterDefaultTable, column.MasterAlias, column.MasterFieldName, column.ChildDefaultTable, column.ChildFieldName));
                }

                //For Where 
                var conditionStr = string.Empty;
                var module = ModuleDA.Instance.Single(r => r.Id == entity.ModuleId);
                var removeOperator = true;

                if (!module.Dashboard)
                {
                    if (columnList.Count(
                        r => r.FieldName.IndexOf("CreatedBy", StringComparison.OrdinalIgnoreCase) >= 0) > 0)
                    {
                        conditionStr += string.Format(" ([{0}].CreatedBy in (@users)) ", str);
                        removeOperator = false;
                    }
                    if (columnList.Count(
                            r => r.FieldName.IndexOf("ResponsibleUsers", StringComparison.OrdinalIgnoreCase) >= 0) > 0)
                    {
                        conditionStr = removeOperator ? string.Format(" ( ',' + [{0}].ResponsibleUsers like '%,@onlyuser,%') ", str) :
                                                        string.Format(" ({1} OR ( ',' + [{0}].ResponsibleUsers like '%,@onlyuser,%')) ", str, conditionStr);
                    }
                }

                //Check SubGrid
                if (entity.Parent != null && entity.Parent.Length > 0)
                {
                    var parentViewModuleId = ViewDA.Instance.Find(r => entity.Parent.Contains(r.Id)).Select(r => r.ModuleId).Distinct().First();
                    var idFieldParent = EntityFieldDA.Instance.Single(r => r.FieldName.Equals("Id") && r.ModuleId == parentViewModuleId);
                    var refModule = datasource.Single(r => r.MasterId == idFieldParent.Id && r.MasterModuleId == parentViewModuleId && r.ChildModuleId == entity.ModuleId);

                    var col = columnList.Single(r => r.Id == refModule.ChildId);
                    if (col.IsMultiSelecttBox)
                    {
                        conditionStr += string.Format("  And ',' + [{0}].[{1}] like '%,@id,%' ", refModule.ChildDefaultTable, refModule.ChildFieldName);
                    }
                    else
                    {
                        conditionStr += string.Format("  And   [{0}].[{1}] = @id ", refModule.ChildDefaultTable, refModule.ChildFieldName);
                    }
                }
                else if (entity.ParentId.Length > 0)
                {
                    entity.Parent = entity.ParentId.Replace("}{", ",").Replace("{", "").Replace("}", "").Split(',').Select(int.Parse).ToArray();
                    var parentViewModuleId = ViewDA.Instance.Find(r => entity.Parent.Contains(r.Id)).Select(r => r.ModuleId).Distinct().Single();
                    var idFieldParent = EntityFieldDA.Instance.Single(r => r.FieldName.Equals("Id") && r.ModuleId == parentViewModuleId);
                    var refModule = datasource.Single(r => r.MasterId == idFieldParent.Id && r.MasterModuleId == parentViewModuleId && r.ChildModuleId == entity.ModuleId);

                    var col = columnList.Single(r => r.Id == refModule.ChildId);
                    if (col.IsMultiSelecttBox)
                    {
                        conditionStr += string.Format("  And ',' +  [{0}].[{1}] like '%,@id,%' ", refModule.ChildDefaultTable, refModule.ChildFieldName);
                    }
                    else
                    {
                        conditionStr += string.Format("  And   [{0}].[{1}] = @id ", refModule.ChildDefaultTable, refModule.ChildFieldName);
                    }
                }


                //Check Custom Field
                var columnsCondition = entity.Eli_ViewConditions.ToList();

                if (columnsCondition.Count > 0)
                {
                    conditionStr += string.Format("  AND  (  ");
                    var cond = columnList.Single(r => r.Id == columnsCondition[0].FieldId);
                    var refModule = datasource.SingleOrDefault(r => r.ChildId == columnsCondition[0].FieldId);

                    if (cond.Deletable.HasValue && cond.Deletable.Value)
                    {
                        conditionStr += GetFilterValue(string.Empty, "pv", cond, columnsCondition[0].FilterValue, columnsCondition[0].FilterValue1, columnsCondition[0].Operator, refModule);
                    }
                    else
                    {                        
                        conditionStr += GetFilterValue(string.Empty, str, cond, columnsCondition[0].FilterValue, columnsCondition[0].FilterValue1, columnsCondition[0].Operator, refModule);
                    }

                    if (columnsCondition.Any())
                    {
                        var viewCondition = columnsCondition.Where(x => x.Id > 0);

                        for (int i = 1; i < columnsCondition.Count; i++)
                        {
                            cond = columnList.Single(r => r.Id == columnsCondition[i].FieldId);
                            refModule = datasource.SingleOrDefault(r => r.ChildId == columnsCondition[i].FieldId);

                            if (columnsCondition[i - 1].Id > 0 && columnsCondition[i].Id == 0)//join orginal condition and additional condition (advance search)
                            {
                                conditionStr += string.Format(") AND (");

                                if (cond.Deletable.HasValue && cond.Deletable.Value)
                                {
                                    conditionStr += GetFilterValue(string.Empty, "pv", cond, columnsCondition[i].FilterValue, columnsCondition[i].FilterValue1, columnsCondition[i].Operator, refModule);
                                }
                                else
                                {
                                    conditionStr += GetFilterValue(string.Empty, str, cond, columnsCondition[i].FilterValue, columnsCondition[i].FilterValue1, columnsCondition[i].Operator, refModule);
                                }
                            }
                            else //include the condition
                            {
                                if (cond.Deletable.HasValue && cond.Deletable.Value)
                                {
                                    conditionStr += GetFilterValue((columnsCondition[i].IsAND ? "  And  " : "  Or  "), "pv", cond, columnsCondition[i].FilterValue, columnsCondition[i].FilterValue1, columnsCondition[i].Operator, refModule);
                                }
                                else
                                {
                                    conditionStr += GetFilterValue((columnsCondition[i].IsAND ? "  And  " : "  Or  "), str, cond, columnsCondition[i].FilterValue, columnsCondition[i].FilterValue1, columnsCondition[i].Operator, refModule);
                                }
                            }                            
                        }
                    }
                    conditionStr += string.Format("  ) ");
                }

                if (module.Dashboard && conditionStr.Length > 0)
                {
                    conditionStr = conditionStr.Substring(6);
                }
                if (!string.IsNullOrEmpty(conditionStr))
                {
                    var removeString = "AND";
                    if (conditionStr.TrimEnd().Substring(conditionStr.TrimEnd().Length - 3).ToUpper() == removeString)
                    {
                        int index = conditionStr.IndexOf(removeString, StringComparison.OrdinalIgnoreCase);
                        conditionStr = (index < 0) ? conditionStr : conditionStr.Remove(index, removeString.Length);
                    }
                    query.WHERE(conditionStr);
                }
                return query.ToString();
            }
            return string.Empty;
        }

        private string GetFilterValue(string andnOr, string alias, vwModuleEntityField column, string filterValue, string filterValue1,
            string operatorStr, vwModuleEnittyRelationship relationship = null)
        {
            string value;
            if (operatorStr == SearchOperators.Within.ToString())
            {
                var days = int.Parse(filterValue);
                if (days < 0)
                {
                    value = string.Format("  {0}   ({1}.{2}   between convert(date,DATEADD(DAY, {3}, GETDATE())) AND convert(date,getdate()))  ", andnOr, alias, column.FieldName,
                        days);
                }
                else
                {
                    value = string.Format("  {0}   ({1}.{2}  between convert(date,getdate()) AND convert(date,DATEADD(DAY, {3}, GETDATE())))  ", andnOr, alias, column.FieldName, days);
                }
            }
            else if (operatorStr == SearchOperators.Between.ToString())
            {
                if (column.IsDate || column.IsDateTime)
                {
                    value = string.Format("  {0}   ({1}.{2}  {3} convert(date,'{4}') AND convert(date,'{5}'))  ", andnOr, alias, column.FieldName, operatorStr, filterValue,
                        filterValue1);
                }
                else
                {
                    value = string.Format("  {0}   ({1}.{2}  {3} {4} AND {5})", andnOr, alias, column.FieldName, operatorStr, filterValue, filterValue1);
                }
            }
            else if (column.IsDecimal || column.IsInteger || column.IsList || column.IsCurrency)
            {

                if (operatorStr.ToLower().Equals("like"))  //query with the "Like" operator                     
                {
                    //sql injection
                    if (filterValue != null)
                    {
                        filterValue = filterValue.Replace("'", "''");
                    }
                    if (column.IsList) // with the reference column
                    {
                        if (!(column.ForeignKey.HasValue && column.ForeignKey.Value))//refer with the picklist
                        {
                            value = string.Format("  {0}   ([dbo].[fn_GetNameInListNameValuesById](ISNULL({1}.{2},''))  {3} N'%{4}%')  ", andnOr, alias, column.FieldName, operatorStr, filterValue);
                        }
                        else//refer with the physical table
                        {
                            //get the alias and column of refer table 
                            alias = relationship != null ? relationship.MasterAlias : alias;
                            var refFieldName = relationship != null ? relationship.MasterDisplayColumn : column.FieldName;
                            
                            value = string.Format("  {0}   (ISNULL({1}.{2},'')  {3} N'%{4}%')  ", andnOr, alias, refFieldName, operatorStr, filterValue);
                        }
                    }
                    else//other type
                    {
                        if (column.IsCurrency || column.IsInteger || column.IsDecimal)//is number types
                        {
                            value = string.Format("  {0}   (CONVERT(nvarchar(MAX), ISNULL({1}.{2}, 0))  {3} N'%{4}%')  ", andnOr, alias, column.FieldName, operatorStr, filterValue);
                        }
                        else if (column.IsDate || column.IsDateTime)//is date types
                        {
                            value = string.Format("  {0}   (CONVERT(nvarchar(MAX), ISNULL({1}.{2}, ''))  {3} N'%{4}%', 101)  ", andnOr, alias, column.FieldName, operatorStr, filterValue);
                        }
                        else//is text type
                        {
                            value = string.Format("  {0}   (ISNULL({1}.{2},'')  {3} N'%{4}%')  ", andnOr, alias, column.FieldName, operatorStr, filterValue);
                        }                        
                    }
                }
                else if (column.IsList && column.IsTextShow == true && (operatorStr.ToLower().Equals("startswith") || operatorStr.ToLower().Equals("<>") || operatorStr.ToLower().Equals("=")))
                {
                    var operatorStr1 = operatorStr.ToLower() == "startswith" ? "like" : operatorStr;

                    //sql injection
                    if (filterValue != null)
                    {
                        filterValue = filterValue.Replace("'", "''");
                    }

                    var chars = new[] { "", "" };
                    if (operatorStr.ToLower().Equals("startswith")) chars = new[] { "", @"%" };

                    //copy from above code
                    if (!(column.ForeignKey.HasValue && column.ForeignKey.Value))//refer with the picklist
                    {
                        value =
                            string.Format(
                                "  {0}   ([dbo].[fn_GetNameInListNameValuesById](ISNULL({1}.{2},''))  {3} N'{4}{5}{6}')  ", andnOr,
                                alias, column.FieldName, operatorStr1, chars[0], filterValue, chars[1]);
                    }
                    else//refer with the physical table
                    {
                        //get the alias and column of refer table 
                        alias = relationship != null ? relationship.MasterAlias : alias;
                        var refFieldName = relationship != null ? relationship.MasterDisplayColumn : column.FieldName;

                        value = string.Format("  {0}   (ISNULL({1}.{2},'') {3} N'{4}{5}{6}')  ", andnOr, alias, refFieldName,
                            operatorStr1, chars[0], filterValue, chars[1]);
                    }
                }
                else
                {
                    try
                    {
                        if (filterValue != null)
                        {
                        if (column.IsInteger)
                        {
                            var i = int.Parse(filterValue);
                        }
                        else
                        {
                            var i = decimal.Parse(filterValue);
                        }
                    }
                    }
                    catch
                    {
                        filterValue = "0";
                    }

                    if (column.Deletable.HasValue && column.Deletable.Value)
                    {
                        value = string.Format("  {0}   ({1}.{2}  {3} '{4}')", andnOr, alias, column.FieldName, operatorStr, filterValue == null ? string.Empty : filterValue);
                    }
                    else
                    {
                        if (filterValue == null)
                        {
                            filterValue = "NULL";
                            operatorStr = "IS";
                        }

                        value = string.Format("  {0}   ({1}.{2}  {3} {4})", andnOr, alias, column.FieldName, operatorStr, filterValue);
                    }
                }
            }
            else
            {
                if (column.IsMultiSelecttBox)
                {
                    string[] strArray = new string[]{};
                    var isQuickSearch = false;

                    if (filterValue != null)
                    {
                        isQuickSearch = !(filterValue.StartsWith("\"[") && filterValue.EndsWith("]\""));
                        if (isQuickSearch)//is quick Search
                        {
                            strArray = new string[] { filterValue };
                        }
                        else//is advance Search
                        {
                            strArray = JsonConvert.DeserializeObject<string[]>(filterValue.Replace("\"[", "[").Replace("]\"", "]"));
                        }
                    }

                    var res = string.Empty;
                    if (operatorStr.ToLower().Equals("like"))
                    {
                        if (strArray.Length > 0)
                        {
                            //--complete the filter query with each search values
                            for (int i = 0; i < strArray.Length; i++)
                            {
                                var join = i > 0 ? "Or" : string.Empty;
                                var condValue = strArray[i];
                                var filterQuery = "";

                                if((column.ForeignKey.HasValue && column.ForeignKey.Value))//is reference data
                                {
                                    if(isQuickSearch)
                                    {
                            //build the query pattern to get value
                            var queryValuePattern = relationship != null ?
                                                                    string.Format(@"STUFF((SELECT ',' + axyz1.{1} FROM {0} axyz1 WHERE axyz1.{2} in (SELECT value FROM [dbo].SPLIT(',', {3}.{4})) For XML PATH ('')), 1, 1, '') + ','",
                                                                                 relationship.MasterDefaultTable, relationship.MasterDisplayColumn, relationship.MasterFieldName, alias, column.FieldName) :
                                                        string.Empty;
                                        //like on displayed value
                                        filterQuery = string.Format(" ( {0} {1} '%{2}%') ", queryValuePattern, operatorStr, condValue);
                                    }
                                    else
                                    {
                                        //like on value
                                        filterQuery = string.Format("  (',' + ISNULL({0}.{1},'') like '%,{2},%') ", alias, column.FieldName, condValue);  
                                    }                                                            
                                }
                                else//is data of list value 
                                {
                                    if (isQuickSearch)
                                    {
                                        filterQuery = string.Format(" ( ([dbo].[fn_GetNamesInListValuesById]({0}.{1}) {2} '%{3}%') ", alias, column.FieldName, operatorStr, condValue);
                                    }
                                    else
                                    {
                                        //like on value
                                        filterQuery = string.Format("  (',' + ISNULL({0}.{1},'') like '%,{2},%') ", alias, column.FieldName, condValue);  
                                    }
                                }

                                res += join + filterQuery;
                            }                            
                            //--end complete
                        }
                        return string.Format("  {0}  ({1})  ", andnOr, res);
                    }
                    else //not like
                    {
                        if (strArray.Length > 0)
                        {
                            res += string.Format(" (',' + {0}.{1} not like '%,{2},%') ", alias, column.FieldName, strArray[0]);

                            if (strArray.Length > 1)
                            {
                                for (int i = 1; i < strArray.Length; i++)
                                {
                                    res += string.Format("  Or (',' + {0}.{1} not like '%,{2},%') ", alias, column.FieldName, strArray[i]);
                                }
                            }
                        }
                        return string.Format("  {0}  ({1} Or {2}.{3} IS NULL)  ", andnOr, res, alias, column.FieldName);
                    }
                }
                if (column.IsCheckBox)
                {
                    return string.Format("  {0}   (ISNULL({1}.{2}, 0)   {3} {4})", andnOr, alias, column.FieldName, operatorStr, filterValue);
                }
                if ((column.IsDate || column.IsDateTime) && !operatorStr.ToLower().Equals("like"))
                {
                    return string.Format("  {0}   (convert(date,{1}.{2})  {3} convert(date,'{4}'))  ", andnOr, alias, column.FieldName, operatorStr, filterValue);
                }
                if (operatorStr == SearchOperators.StartsWith.ToString())
                {
                    //sql injection
                    if (filterValue != null)
                    {
                        filterValue = filterValue.Replace("'", "''");
                    }
                    return string.Format("  {0}   (ISNULL({1}.{2}, '')  like N'{3}%')  ", andnOr, alias, column.FieldName, filterValue);
                }

                if (operatorStr.ToLower().Equals("like") && filterValue != null)
                {
                    //sql injection
                    if (filterValue != null)
                    {
                        filterValue = filterValue.Replace("'", "''");
                    }
                }

                value = operatorStr.ToLower().Equals("like") ? string.Format("  {0}   (ISNULL({1}.{2}, '')  {3} N'%{4}%')  ", andnOr, alias, column.FieldName, operatorStr, filterValue) : string.Format("  {0}   (ISNULL({1}.{2}, '')  {3} '{4}')  ", andnOr, alias, column.FieldName, operatorStr, filterValue);
            }
            return value;
        }

        private string GetFilterValue1(string andnOr, vwCustomViewColumn column, string filterValue, string filterValue1, string operatorStr)
        {
            string value;
            if (operatorStr == SearchOperators.Within.ToString())
            {
                var days = int.Parse(filterValue);
                value = string.Format(days < 0
                    ? "  {0}  ({1}.{2}   between convert(date,DATEADD(DAY, {3}, GETDATE())) AND convert(date,getdate()))  "
                    : "  {0}  ({1}.{2}  between convert(date,getdate()) AND convert(date,DATEADD(DAY, {3}, GETDATE())))  "
                    , andnOr, column.TableAlias, column.ColumnName, days);
            }
            else if (operatorStr == SearchOperators.Between.ToString())
            {
                if (column.IsDate || column.IsDateTime)
                {
                    value = string.Format("  {0}  ({1}.{2}  {3} convert(date,'{4}') AND convert(date,'{5}'))  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue,
                        filterValue1);
                }
                else
                {
                    value = string.Format("  {0}  ({1}.{2}  {3} {4} AND {5})", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue, filterValue1);
                }
            }
            else if (column.IsDecimal || column.IsInteger || column.IsList || column.IsCurrency)
            {
                if (operatorStr.ToLower().Equals("like") && //query with the "Like" operator 
                    column.IsList)
                {
                    if (column.IsList) // with the reference column
                    {
                        if (!(column.ForeignKey.HasValue && column.ForeignKey.Value))//refer with the picklist
                        {
                            value = string.Format("  {0}  ([dbo].[fn_GetNameInListNameValuesById]({1}.{2})  {3} N'%{4}%')  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);
                        }
                        else//refer with the physical table
                        {
                            //get the alias and column of refer table 
                            value = string.Format("  {0}  ({1}.{2}  {3} N'%{4}%')  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);
                        }
                    }
                    else//other type
                    {
                        value = string.Format("  {0}  ({1}.{2}  {3} N'%{4}%')  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);
                    }
                }
                else
                {
                    try
                    {
                        if (column.IsInteger)
                        {
                            var i = int.Parse(filterValue);
                        }
                        else
                        {
                            var i = decimal.Parse(filterValue);
                        }
                    }
                    catch
                    {
                        filterValue = "0";
                    }

                    if (column.Deletable.HasValue && column.Deletable.Value)
                    {
                        value = string.Format("  {0}   ({1}.{2}  {3} '{4}')", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);
                    }
                    else
                    {
                        value = string.Format("  {0}   ({1}.{2}  {3} {4})", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);
                    }
                }
            }
            else
            {
                if (column.IsMultiSelecttBox)
                {
                    var strArray = filterValue.Split(',');
                    var res = string.Empty;
                    if (operatorStr.ToLower().Equals("like"))
                    {
                        if (strArray.Length > 0)
                        {
                            //build the query pattern to get value
                            var queryValuePattern = string.Empty;

                            //build the query pattern to set the letter or number search value 
                            var condValuePattern = strArray.All(x=>char.IsDigit(x.Trim(), 0)) ? "%,{2},%" : "%{2}%";


                            //--complete the filter query with each search values
                            res += !(column.ForeignKey.HasValue && column.ForeignKey.Value) ?
                                            string.Format(" ( ([dbo].[fn_GetNamesInListValuesById]({0}.{1}) {2} '%{3}%') ", column.TableAlias, column.ColumnName, operatorStr, strArray[0]) :
                                            string.Format(" ( {0} {1} '" + condValuePattern + "') ", queryValuePattern, operatorStr, strArray[0]);

                            if (strArray.Length > 1)
                            {
                                for (int i = 1; i < strArray.Length; i++)
                                {                                    
                                    res += !(column.ForeignKey.HasValue && column.ForeignKey.Value) ?
                                            string.Format("  Or ( ([dbo].[fn_GetNamesInListValuesById]({0}.{1}) {2} '%{3}%') ", column.TableAlias, column.ColumnName, operatorStr, strArray[0]) :
                                            string.Format(" ( {0} {1} '" + condValuePattern + "') ", queryValuePattern, operatorStr, strArray[i]);
                                }
                            }
                            //--end complete
                        }
                        return string.Format("  {0}  ({1})  ", andnOr, res);
                    }
                    else
                    {
                        if (strArray.Length > 0)
                        {
                            res += string.Format(" (',' + {0}.{1} not like '%,{2},%') ", column.TableAlias, column.ColumnName, strArray[0]);

                            if (strArray.Length > 1)
                            {
                                for (int i = 1; i < strArray.Length; i++)
                                {
                                    res += string.Format("  Or (',' + {0}.{1} not like '%,{2},%') ", column.TableAlias, column.ColumnName, strArray[i]);
                                }
                            }
                        }
                        return string.Format("  {0}  ({1} Or {2}.{3} IS NULL)  ", andnOr, res, column.TableAlias, column.ColumnName);
                    }
                }
                if (column.IsCheckBox)
                {
                    return string.Format("  {0}   ({1}.{2}   {3} {4})", andnOr, column.TableAlias, column.ColumnName, operatorStr, bool.Parse(filterValue) ? "1" : "0");
                }
                if ((column.IsDate || column.IsDateTime) && !operatorStr.ToLower().Equals("like"))
                {
                    return string.Format("  {0}   (convert(date,{1}.{2})  {3} convert(date,'{4}'))  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);                         
                }
                if (operatorStr == SearchOperators.StartsWith.ToString())
                {
                    return string.Format("  {0}   ({1}.{2}  like N'{3}%')  ", andnOr, column.TableAlias, column.ColumnName, filterValue);
                }
                value = operatorStr.ToLower().Equals("like") ? string.Format("  {0}   ({1}.{2}  {3} N'%{4}%')  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue) : string.Format("  {0}   ({1}.{2}  {3} '{4}')  ", andnOr, column.TableAlias, column.ColumnName, operatorStr, filterValue);
            }
            return value;
        }

        //private string GetFilterValue(IEnumerable<vwModuleEntityField> columns, int fieldId, string filterValue, string filterValue1, string operatorStr)
        //{
        //    var item = columns.Single(r => r.Id == fieldId);
        //    string value;
        //    if (operatorStr == SearchOperators.Within.ToString())
        //    {
        //        var days = int.Parse(filterValue);
        //        if (days < 0)
        //            value = string.Format("between convert(date,'{0}') AND convert(date,'{1}')", DateTime.Now.AddDays(days), DateTime.Now);
        //        else
        //            value = string.Format("between convert(date,'{0}') AND convert(date,'{1}')", DateTime.Now, DateTime.Now.AddDays(days));
        //    }
        //    else if (operatorStr == SearchOperators.Between.ToString())
        //    {
        //        if (item.IsDate || item.IsDateTime)
        //        {
        //            value = string.Format("{0} convert(date,'{1}') AND convert(date,'{2}')", operatorStr, filterValue, filterValue1);
        //        }
        //        else
        //            value = string.Format("{0} {1} AND {2}", operatorStr, filterValue, filterValue1);
        //    }
        //    else if (item.IsDecimal || item.IsInteger || item.IsList || item.IsCurrency)
        //    {
        //        value = string.Format("{0} {1}", operatorStr, filterValue);
        //    }
        //    else
        //    {
        //        if (item.IsMultiSelecttBox)
        //        {
        //            var strArray = filterValue.Split(',');
        //            var res = string.Empty;

        //            foreach (var str in strArray)
        //            {
        //                res += string.Format("");
        //            }


        //            if (operatorStr.ToLower().Equals("like"))
        //                return string.Format("in (Select Value From [dbo].[SPLIT](',','{0}'))", filterValue.Replace("[", "").Replace("]", ""));
        //            return string.Format("{0} ({1})", operatorStr, filterValue.Replace("[", "").Replace("]", ""));
        //        }
        //        if (item.IsCheckBox)
        //        {
        //            return string.Format("{0} {1}", operatorStr, bool.Parse(filterValue) ? "1" : "0");
        //        }
        //        if (item.IsDate || item.IsDateTime)
        //        {
        //            return string.Format("{0} convert(date,'{1}')", operatorStr, filterValue);
        //        }
        //        if (operatorStr == SearchOperators.StartsWith.ToString())
        //        {
        //            return string.Format("like '{0}%'", filterValue);
        //        }
        //        value = operatorStr.ToLower().Equals("like") ? string.Format("{0} '%{1}%'", operatorStr, filterValue) : string.Format("{0} '{1}'", operatorStr, filterValue);
        //    }
        //    return value;
        //}

        //public void SqlBuilderUseCases()
        //{
        //    //1.
        //    var query = new SqlBuilder()
        //        .SELECT("*")
        //        .FROM("Products")
        //        .WHERE("Name LIKE {0}", "A%");

        //    Console.WriteLine(query);

        //    //2.
        //    // SQL.SELECT is just a shortcut to new SqlBuilder().SELECT
        //    query = SQL
        //       .SELECT("ID")
        //       .SELECT("Name")
        //       .FROM("Products")
        //       .WHERE("Name LIKE {0}", "A%")
        //       .WHERE("CategoryID = {0}", 2);

        //    Console.WriteLine(query);

        //    //3.
        //    query = SQL
        //           .SELECT("ID")
        //           ._("Name")
        //           .FROM("Products")
        //           .WHERE("Name LIKE {0}", "A%")
        //           ._("CategoryID = {0}", 2);

        //    Console.WriteLine(query);

        //    //4.
        //    query = SQL
        //           .SELECT("c.CategoryName, t0.TotalProducts")
        //           .FROM("Categories c")
        //           .JOIN("({0}) t0 ON c.CategoryID = t0.CategoryID", SQL
        //              .SELECT("CategoryID, COUNT(*) AS TotalProducts")
        //              .FROM("Products")
        //              .GROUP_BY("CategoryID"))
        //           .ORDER_BY("t0.TotalProducts DESC");

        //    Console.WriteLine(query);

        //    //5.
        //    int[] ids = { 1, 2, 3 };

        //    query = SQL
        //       .SELECT("*")
        //       .FROM("Products")
        //       .WHERE("CategoryID IN ({0})", ids);

        //    Console.WriteLine(query);
        //}

        //void DynamicSql(int? categoryId, int? supplierId)
        //{

        //    var query = SQL
        //       .SELECT("ID, Name")
        //       .FROM("Products")
        //       .WHERE()
        //       ._If(categoryId.HasValue, "CategoryID = {0}", categoryId)
        //       ._If(supplierId.HasValue, "SupplierID = {0}", supplierId)
        //       .ORDER_BY("Name DESC");

        //    Console.WriteLine(query);
        //}



    }
}
