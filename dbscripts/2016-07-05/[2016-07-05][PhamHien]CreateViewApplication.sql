/****** Object:  View [dbo].[vwApplication]    Script Date: 07/05/2016 10:40:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vwApplication] AS
SELECT o.Id, [dbo].fn_GetNameInListNameValuesById(o.[Status]) as [Status], c.Name, o.PartNumber, o.TotalMonthly, o.StoreNumber, o.CreatedBy
FROM SalesOrder o LEFT JOIN 
	 SalesCustomer c ON o.CustomerId = c.Id

GO
