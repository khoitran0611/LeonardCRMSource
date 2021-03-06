UPDATE [Eli_Views] 
SET [QueryScript] = N'SELECT [Eli_Roles].[Id]
 ,[Eli_Roles].[Name]
 ,[Eli_Roles].[Description]
 ,ISNULL([v2].[Name],'''') AS [Parent]
 ,[Eli_Roles].[IsHostAdmin] ,[Eli_Roles].[CreatedDate] ,[Eli_Roles].[ModifiedDate] ,ROW_NUMBER() OVER(ORDER BY [OrderExpression]) as RowIndex 
  FROM [dbo].[Eli_Roles] Left JOIN [dbo].[Eli_Roles] AS v2 ON '','' + [Eli_Roles].Parent + '','' LIKE ''%,'' + CAST([v2].Id AS NVARCHAR) + '',%'''
WHERE [Id] = 55