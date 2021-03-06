ALTER PROCEDURE [dbo].[sp_AdvanceSearch]
	@ModuleId int,
	@ViewId int,
	@Id INT,
	@sqlScripts nvarchar(4000),
	@userId int,
	@roleId int,
	@pageIndex int,
	@pageSize int,
	@sortDirection nvarchar(50),
	@totalRow int OUT,
	@defaultOderBy BIT,
	@columnGroup NVARCHAR(500),
	@groupJsonStr NVARCHAR(max) OUT
AS
BEGIN

 SET @groupJsonStr = ''
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


	declare @usertmp varchar(250)
	SET @usertmp = ''
	select @usertmp = @usertmp + CONVERT(nvarchar(50),id) + ', ' from @userTable

	declare @tmp varchar(250)
	SET @tmp = ''
	select @tmp = @tmp + CONVERT(nvarchar(50),role_id) + ', ' from @listRole

	DECLARE @orderBy NVARCHAR(200)
	If (@defaultOderBy = 1)
	BEGIN
		SET @orderBy = (SELECT TOP 1 [ColumnName] + ' ' + [OrderDirection] FROM dbo.Eli_ViewOrderBy WHERE ViewId = @ViewId )
		IF (@orderBy IS NOT NULL OR @orderBy <> '')
		BEGIN
			SET @sortDirection = @orderBy
		END
		ELSE
		BEGIN
			SET @sortDirection = 'Id ASC'
		END  
    END 

	Declare @defaultTable nvarchar(20)
	Set @defaultTable = (Select [DefaultTable] from [dbo].[Eli_Modules] Where [Id] = @ModuleId)
	
	DECLARE @isCustomField bit, @sortColumn NVARCHAR(255)
	SET @sortColumn = (SELECT Value FROM [dbo].SPLIT(' ',@sortDirection) WHERE Id = 1)
	SET @sortColumn = LTRIM(RTRIM(@sortColumn))
	SET @isCustomField = (SELECT Deletable FROM dbo.Eli_EntityFields WHERE FieldName = @sortColumn AND ModuleId = @ModuleId)
	
	IF(@isCustomField = 1)
	BEGIN
    
		Set @sqlScripts = REPLACE(@sqlScripts,'[OrderExpression]','pv.' + @sortDirection)
	END
	ELSE
	Begin
		Set @sqlScripts = REPLACE(@sqlScripts,'[OrderExpression]', @defaultTable + '.' + @sortDirection)
	END
		
	--Set @sqlScripts = REPLACE(@sqlScripts,'[OrderExpression]', @defaultTable + '.' + @sortDirection)

	Set @sqlScripts = REPLACE(@sqlScripts,'@users', SUBSTRING(@usertmp, 0, LEN(@usertmp)))
	Set @sqlScripts = REPLACE(@sqlScripts,'@onlyuser', @userId)
	Set @sqlScripts = REPLACE(@sqlScripts,'@roles', SUBSTRING(@tmp, 0, LEN(@tmp)))
	Set @sqlScripts = REPLACE(@sqlScripts,'@id',@Id)
	PRINT @sqlScripts
	-------[Create Dynamic View]-------	
	DECLARE @flag BIT
	SET @flag = (SELECT COUNT(Eli_EntityFields.Id) FROM Eli_EntityFields left join dbo.Eli_FieldData  
						on Eli_EntityFields.Id = Eli_FieldData.CustFieldId WHERE Eli_EntityFields.Deletable = 1 AND Eli_EntityFields.ModuleId = @ModuleId)
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
		IF(@index > 0)
		BEGIN
			SET @strLen = LEN(@sqlScripts) 	
			SET @lastStr = SUBSTRING ( @sqlScripts ,@index, @strLen )
			SET @sqlScripts = SUBSTRING(@sqlScripts,0,@index) + @query + @lastStr	
		END
		ELSE
		BEGIN
			SET @sqlScripts = @sqlScripts + @query
		END
    End

	-------[End Create Dynamic View]-------

	--print @sqlScripts
	declare @result table (row_count int)
	insert into @result (row_count)
	exec (N'select count(*) from (' + @sqlScripts + ') as v');
	Set @totalRow = (select top (1) row_count from @result)

	DECLARE @countPageNumber INT,@m Int
	SET @countPageNumber = @totalRow/@pageSize
	SET @m = @totalRow % @pageSize
	IF(@m > 0)
		SET @countPageNumber = @countPageNumber + 1

	IF(@pageIndex > @countPageNumber)
	BEGIN
		SET @pageIndex = @countPageNumber
	END

	DECLARE @type INT
	-- Process Group By
	IF(@columnGroup IS NOT NULL AND LEN(@columnGroup) > 0)
	BEGIN
		DECLARE @groupStr NVARCHAR(max)
		declare @groupResult table (GroupName NVARCHAR(500),Total Int)
		
		SELECT @type = [DataTypeId] FROM [dbo].[Eli_EntityFields] WHERE [ModuleId] = @ModuleId AND [FieldName] = @columnGroup
		
		IF(@type = 2 OR @type = 13)
		BEGIN
			SET @groupStr = 'Select CONVERT(NVARCHAR(500),CONVERT(DATE,' + @columnGroup + '))  as GroupName, Count(*) as Total From ('+ @sqlScripts +') as v Group By CONVERT(NVARCHAR(500),CONVERT(DATE,' + @columnGroup +'))'
		END
		ELSE IF(@type = 11)
		BEGIN
			SET @groupStr = 'Select (Case ' + @columnGroup + ' WHEN 1 Then ''1'' ELSE ''0'' END) as GroupName, Count(*) as Total From ('+ @sqlScripts +') as v Group By ' + @columnGroup
		END
        ELSE
		BEGIN
			IF(@columnGroup IS NULL OR LEN(@columnGroup) = 0)
			BEGIN
				SET @columnGroup = 'null'
			END
			SET @groupStr = 'Select ' + @columnGroup  + ' as GroupName, Count(*) as Total From ('+ @sqlScripts +') as v Group By ' + @columnGroup
			--PRINT @groupStr
		END
		
		Insert INTO @groupResult (GroupName,Total)
		EXECUTE(@groupStr)	
		
	END	
	-- Process Group By

	Declare @SqlQueryString nvarchar(max)
	Set @SqlQueryString = 'Select * From (' + @sqlScripts +') as v Where RowIndex BETWEEN (('+ CONVERT(nvarchar(50),@pageIndex) +'-1)*'+ CONVERT(nvarchar(50), @pageSize )+')+1 AND '+ CONVERT(nvarchar(50), @pageSize ) +' * ('+ CONVERT(nvarchar(50),@pageIndex) +')'
	
	-- Remove Group
	IF(@columnGroup IS NOT NULL AND LEN(@columnGroup) > 0)
	BEGIN
		
		DECLARE @showGroupStr nvarchar(MAX)
		SET @showGroupStr = 'Select g.' + @columnGroup + ' From ('+ @SqlQueryString +') as g'
		DECLARE @showGroupTable TABLE (name NVARCHAR(500))

		INSERT @showGroupTable(name)
		EXECUTE(@showGroupStr)	
		
		DELETE @groupResult
		WHERE [GroupName] NOT IN (SELECT name FROM @showGroupTable)
		
		if(@type != 11)
		
			SELECT @groupJsonStr = '[' + STUFF((
				select 
					',{"GroupName":"' + (Case ISNULL(GroupName,'') WHEN '' THEN 'null'
														ELSE GroupName  END )          
					+ '","Total":' + cast(Total as varchar(50))
					+'}'

				from @groupResult
				for xml path(''), type
			).value('.', 'varchar(max)'), 1, 1, '') + ']'	
		
		else
		
				SELECT @groupJsonStr = '[' + STUFF((
				select 
					',{"GroupName":"' + (Case GroupName WHEN '1' THEN 'true'
														ELSE 'false' END )          
					+ '","Total":' + cast(Total as varchar(50))
					+'}'

				from @groupResult
				for xml path(''), type
			).value('.', 'varchar(max)'), 1, 1, '') + ']'	

		--PRINT @groupJsonStr
	END
	
	--PRINT @SqlQueryString
	EXECUTE(@SqlQueryString)
END
