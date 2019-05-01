/****** Object:  View [dbo].[vwApplication]    Script Date: 07/04/2016 18:38:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwApplication] AS
SELECT o.Id, [dbo].fn_GetNameInListNameValuesById(o.[Status]) as [Status], c.Name, o.AcceptDate, o.GPOrderNumber, o.StoreNumber, o.CreatedBy
FROM SalesOrder o LEFT JOIN 
	 SalesCustomer c ON o.CustomerId = c.Id

GO


