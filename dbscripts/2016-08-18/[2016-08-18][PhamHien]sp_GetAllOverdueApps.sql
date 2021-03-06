ALTER PROCEDURE [dbo].[sp_GetAllOverdueApps]
	@Id NVARCHAR(50),
	@Status NVARCHAR(50),
	@CustomerName NVARCHAR(50),
	@PartNumber NVARCHAR(50),
	@CapitalizationPeriod NVARCHAR(50),
	@ModifiedDate DATE,
	@pageIndex INT = 1,
	@pageSize INT = 10 ,
	@sortExpression NVARCHAR(50) = 'Id Desc',	
	@expectedMonth INT,
	@totalRow INT OUT
AS
BEGIN
  DECLARE @compeleteStatus INT = 246,
		  @cancledStatus INT = 256,
		  @now DATETIME = GETDATE()
		  
  --Increase the now time to test 	  
  --SET @now = DATEADD(m,50, @now) 
  
  SELECT @totalRow = COUNT(SalesCustomer.Id) 
  FROM	SalesOrder INNER JOIN SalesCustomer ON SalesOrder.CustomerId = SalesCustomer.Id
				   INNER JOIN Eli_ListValues v1 ON SalesOrder.[Status] = v1.Id
				   LEFT JOIN Eli_ListValues v2 ON SalesOrder.CapitalizationPeriod = v2.Id
  WHERE (([Status] = @cancledStatus AND dbo.FullMonthsSeparation(SalesOrder.ModifiedDate, @now) >= @expectedMonth) OR 
		([Status] = @compeleteStatus  AND IsFinalize = 1 AND 
		  dbo.FullMonthsSeparation(SalesOrder.ModifiedDate, @now) >= (@expectedMonth + ISNULL(CAST(v2.AdditionalInfo AS INT), 0)))) AND 
		 (@Id IS NULL OR SalesOrder.Id LIKE '%' + @Id + '%') AND 
		 (@Status IS NULL OR v1.[Description] LIKE '%' + @Status + '%') AND 
		 (@CustomerName IS NULL OR SalesCustomer.Name LIKE '%' + @CustomerName + '%') AND 
		 (@PartNumber IS NULL OR PartNumber LIKE '%' + @PartNumber + '%') AND 
		 (@CapitalizationPeriod IS NULL OR v2.[Description] LIKE '%' + @CapitalizationPeriod + '%') AND 
		 (@ModifiedDate IS NULL OR CAST(SalesOrder.ModifiedDate AS DATE)  =  @ModifiedDate ) 
  
  	SELECT Id, 
		 [Status],
		 CustomerName,
		 PartNumber,
		 ModifiedDate,
		 CapitalizationPeriod
		 
	FROM ( SELECT SalesOrder.Id, 
				 dbo.fn_GetNameInListNameValuesById(Status) as [Status],
				 SalesCustomer.Name as CustomerName,
				 PartNumber,
				 SalesOrder.ModifiedDate,
				 dbo.fn_GetNameInListNameValuesById(CapitalizationPeriod) as CapitalizationPeriod,		 
				 ROW_NUMBER() OVER (ORDER BY CASE WHEN @sortExpression = 'Id asc' Then SalesOrder.Id ELSE null END ASC,
											 CASE WHEN @sortExpression = 'Id desc' Then SalesOrder.Id ELSE null END DESC,
											 CASE WHEN @sortExpression = 'Status asc' then v1.[Description] ELSE null END ASC,
											 CASE WHEN @sortExpression = 'Status desc' then v1.[Description] ELSE null END DESC,
											 CASE WHEN @sortExpression = 'CustomerName asc' then SalesCustomer.Name ELSE null END ASC,
											 CASE WHEN @sortExpression = 'CustomerName desc' then SalesCustomer.Name ELSE null END DESC,
											 CASE WHEN @sortExpression = 'PartNumber asc' Then PartNumber ELSE null END ASC,
											 CASE WHEN @sortExpression = 'PartNumber desc' Then PartNumber ELSE null END DESC,
											 CASE WHEN @sortExpression = 'ModifiedDate asc' Then SalesOrder.ModifiedDate ELSE null END ASC,
											 CASE WHEN @sortExpression = 'ModifiedDate desc' Then SalesOrder.ModifiedDate ELSE null END DESC,
											 CASE WHEN @sortExpression = 'CapitalizationPeriod asc' Then v2.[Description] ELSE null END ASC,
											 CASE WHEN @sortExpression = 'CapitalizationPeriod desc' Then v2.[Description] ELSE null END DESC) 
				 AS RowNum	
		 														
				  FROM	SalesOrder INNER JOIN SalesCustomer ON SalesOrder.CustomerId = SalesCustomer.Id
								   INNER JOIN Eli_ListValues v1 ON SalesOrder.[Status] = v1.Id
								   LEFT JOIN Eli_ListValues v2 ON SalesOrder.CapitalizationPeriod = v2.Id
				  WHERE (([Status] = @cancledStatus AND dbo.FullMonthsSeparation(SalesOrder.ModifiedDate, @now) >= @expectedMonth) OR 
						([Status] = @compeleteStatus  AND IsFinalize = 1 AND 
						  dbo.FullMonthsSeparation(SalesOrder.ModifiedDate, @now) >= (@expectedMonth + ISNULL(CAST(v2.AdditionalInfo AS INT), 0)))) AND 
						 (@Id IS NULL OR SalesOrder.Id LIKE '%' + @Id + '%') AND 
						 (@Status IS NULL OR v1.[Description] LIKE '%' + @Status + '%') AND 
						 (@CustomerName IS NULL OR SalesCustomer.Name LIKE '%' + @CustomerName + '%') AND 
						 (@PartNumber IS NULL OR PartNumber LIKE '%' + @PartNumber + '%') AND 
						 (@CapitalizationPeriod IS NULL OR v2.[Description] LIKE '%' + @CapitalizationPeriod + '%') AND 
						 (@ModifiedDate IS NULL OR Cast(SalesOrder.ModifiedDate AS DATE)  =  @ModifiedDate ) 
		 ) v
   Where RowNum BETWEEN ((@pageIndex-1)*@pageSize)+1 AND @pageSize*(@pageIndex)
END