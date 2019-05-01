ALTER VIEW [dbo].[vwApplication] AS
SELECT o.Id, [dbo].fn_GetNameInListNameValuesById(o.[Status]) as [Status], o.[Status] as [StatusCode] , c.Name, o.PartNumber, o.TotalMonthly, o.StoreNumber, o.CreatedBy, o.ResponsibleUsers
FROM SalesOrder o LEFT JOIN 
	 SalesCustomer c ON o.CustomerId = c.Id

GO