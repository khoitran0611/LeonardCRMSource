-- =============================================
-- Author:		LinhDoan
-- Create date: '5-Dec-13'
-- Description:	Get total record of a view
-- =============================================
ALTER procedure [dbo].[sp_GetTotalForView]
	@viewId int,
	@roleId int,
	@userId int,
	@return int output
AS
	BEGIN
		Declare @listRole table (role_id int)
		Declare @userTable table (id int)

		Insert @userTable(id)
		Values (@userId)

		if(@roleId <> 3)
		begin
		
			Insert @listRole
			select Id from 
			dbo.fn_GetRolesHierachy(@roleId)
			
			Insert @userTable(id)
			Select Id
			From [dbo].[Eli_User]
			Where [RoleId] in (Select role_id from @listRole Where role_id <> @roleId)
		end
		
		declare @tmp varchar(250)
		SET @tmp = ''
		select @tmp = @tmp + CONVERT(nvarchar(50),role_id) + ', ' from @listRole	
		
		declare @usertmp varchar(250)
		SET @usertmp = ''
		select @usertmp = @usertmp + CONVERT(nvarchar(50),id) + ', ' from @userTable
		
		


		Declare @sqlScripts nvarchar(4000)
		Declare @defaultTable nvarchar(250)
		Set @defaultTable = (Select [DefaultTable] from [dbo].[Eli_Modules] inner join Eli_Views on Eli_Views.ModuleId = Eli_Modules.Id Where Eli_Views.[Id] = @viewId)
		Set @sqlScripts = (Select [QueryScript] From [dbo].[Eli_Views] Where [Id] = @viewId)
		Set @sqlScripts = REPLACE(@sqlScripts,'[OrderExpression]',@defaultTable+'.Id')
		Set @sqlScripts = REPLACE(@sqlScripts,'@roles', SUBSTRING(@tmp, 0, LEN(@tmp)))
		Set @sqlScripts = REPLACE(@sqlScripts,'@users', SUBSTRING(@usertmp, 0, LEN(@usertmp)))
		Set @sqlScripts = REPLACE(@sqlScripts,'@onlyuser', @userId)

		-------[Create Dynamic View]-------	
		DECLARE @ModuleId INT, @flag BIT
		SET @ModuleId = (SELECT ModuleId FROM dbo.Eli_Views WHERE Id = @viewId)

		SET @flag = (SELECT COUNT(Eli_EntityFields.Id) FROM Eli_EntityFields left join dbo.Eli_FieldData  
						on Eli_EntityFields.Id = Eli_FieldData.CustFieldId WHERE  Eli_EntityFields.Deletable = 1 AND Eli_EntityFields.ModuleId = @ModuleId)
		IF(@flag > 0)
		BEGIN
			DECLARE @cols VARCHAR(MAX);
			WITH    CTE
					  AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY ee.SortOrder ) AS RowIndex ,
									ee.FieldName
						   FROM     dbo.Eli_EntityFields AS ee
               
						   WHERE    ee.ModuleId = @ModuleId
									AND ee.Deletable = 1
						 )
				SELECT  @cols = STUFF(( SELECT  ',' + QUOTENAME(FieldName)
										FROM    CTE
										--WHERE   CTE.RowIndex = 1
									  FOR
										XML PATH('')
									  ), 1, 1, '')
			DECLARE @query VARCHAR(MAX)
			Set @query =
			N' Left Join (
			SELECT * 
			FROM
			(
				SELECT
					[FieldName],
					[FieldData],
					[MaterRecordId]
  
				FROM
					[dbo].[Eli_FieldData]
					inner join Eli_EntityFields a on a.Id = [dbo].Eli_FieldData.CustFieldId
					where ModuleId = '+ CONVERT(NVARCHAR(50),@ModuleId) +'
			) AS p
			PIVOT
			(
				MAX([FieldData]) FOR FieldName IN (' + @cols + ')
			) AS pvt ) as pv on pv.[MaterRecordId] = ' + @defaultTable + '.[Id]  '

			DECLARE @index INT,@strLen INT, @lastStr NVARCHAR(MAX)
			SET @index = [dbo].[fn_FindLastIndexOf](@sqlScripts,'Where',1)
			SET @strLen = LEN(@sqlScripts) 	
			SET @lastStr = SUBSTRING ( @sqlScripts ,@index, @strLen )

			SET @sqlScripts = SUBSTRING(@sqlScripts,0,@index) + @query + @lastStr
		End			

		-------[End Create Dynamic View]-------

		print @sqlScripts

		declare @result table (row_count int)
		declare @total int
		insert into @result (row_count)
		exec (N'select count(*) from (' + @sqlScripts + ') as v');
		set @return = (select top (1) row_count from @result)
		
	END
