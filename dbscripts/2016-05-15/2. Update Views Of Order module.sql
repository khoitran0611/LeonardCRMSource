--Delete 'Paid in Full' View
DELETE Eli_Views WHERE (ViewName = 'Paid in Full' OR ViewName='Deposit Paid in Full' OR ViewName = 'Approved Orders') AND ModuleId = 3

--Create 'Pending Delivery (Not Assigned)' & 'Pending Customer Acceptance' Views
SET IDENTITY_INSERT [dbo].[Eli_Views] ON
INSERT [dbo].[Eli_Views] ([ViewName], [IsPublic], [Shared], [SortOrder], [ModuleId], [QueryScript], [ParentId], [LoadChildView], [DefaultView], [PageSize], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [MasterViewId], [UserRole]) VALUES (N'Pending Delivery (Not Assigned)', 1, 1, 7, 3, N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[AcceptDate] as [AcceptDate] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  ISNULL([SalesOrder].[SerialNumber],'''') as [SerialNumber] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  ISNULL([SalesOrder].[POSTicketNumber],'''') as [POSTicketNumber] ,  ISNULL([SalesOrder].[DriverAssigned],'''') as [DriverAssigned] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 244)    And     (ISNULL(SalesOrder.DriverAssigned, 0)   = 0)  ) ', N'', 0, 0, NULL, CAST(0x0000A775011F8D1A AS DateTime), 1, CAST(0x0000A775011F8D1B AS DateTime), 1, 1, NULL, N'1,2')
INSERT [dbo].[Eli_Views] ([ViewName], [IsPublic], [Shared], [SortOrder], [ModuleId], [QueryScript], [ParentId], [LoadChildView], [DefaultView], [PageSize], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [MasterViewId], [UserRole]) VALUES (N'Pending Customer Acceptance', 1, 1, 6, 3, N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  ISNULL([SalesOrder].[SerialNumber],'''') as [SerialNumber] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  ISNULL([SalesOrder].[POSTicketNumber],'''') as [POSTicketNumber] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 6)  ) ', N'', 0, 0, NULL, CAST(0x0000A7750120E805 AS DateTime), 1, CAST(0x0000A7750120E805 AS DateTime), 1, 1, NULL, N'')
SET IDENTITY_INSERT [dbo].[Eli_Views] OFF

GO 
--Update View

--**Pending orders
UPDATE Eli_Views 
SET SortOrder = 2, QueryScript = N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[CreatedDate] as [CreatedDate] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 4)  ) '
WHERE ViewName = 'Pending orders' AND ModuleId = 3
GO

--**Pre-Approved Orders
UPDATE Eli_Views 
SET SortOrder = 3, QueryScript =  N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[CreatedDate] as [CreatedDate] ,  [SalesOrder].[ModifiedDate] as [ModifiedDate] ,  [SalesOrder].[AcceptDate] as [AcceptDate] ,  ISNULL([SalesOrder].[PromoCode],'''') as [PromoCode] ,  [a4].Name as [CreatedBy] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 5)  ) '
WHERE ViewName = 'Pre-Approved Orders' AND ModuleId = 3
GO

--**Inprogress
UPDATE Eli_Views 
SET SortOrder = 4, QueryScript =  N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  [SalesOrder].[SalesPrice] as [SalesPrice] ,  [SalesOrder].[TotalMonthly] as [TotalMonthly] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 8)  ) '
WHERE ViewName = 'In Progress' AND ModuleId = 3
GO

--**Contract Signed
UPDATE Eli_Views 
SET SortOrder = 5, QueryScript =  N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[AcceptDate] as [AcceptDate] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  [SalesOrder].[SalesPrice] as [SalesPrice] ,  [SalesOrder].[TotalMonthly] as [TotalMonthly] ,  ISNULL([SalesOrder].[SignatureIP],'''') as [SignatureIP] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 6)    And     (ISNULL(SalesOrder.SignatureIP, '''')  Like N''%.%'')    ) '
WHERE ViewName = 'Contract Signed' AND ModuleId = 3
GO

--**Pending Delivery
UPDATE Eli_Views 
SET SortOrder = 8, QueryScript = N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  ISNULL([SalesOrder].[SerialNumber],'''') as [SerialNumber] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  ISNULL([SalesOrder].[POSTicketNumber],'''') as [POSTicketNumber] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 244)  ) '
WHERE ViewName = 'Pending Delivery' AND ModuleId = 3
GO

--**Completed Not Followed Up
UPDATE Eli_Views 
SET SortOrder = 9
WHERE ViewName = 'Completed Not Followed Up' AND ModuleId = 3
GO

--**Completed Orders
UPDATE Eli_Views 
SET SortOrder = 10, ViewName = 'Completed Orders'
WHERE ViewName = 'Finished Orders' AND ModuleId = 3
GO

--**Rejected Orders
UPDATE Eli_Views 
SET SortOrder = 11
WHERE ViewName = 'Rejected Orders' AND ModuleId = 3
GO
