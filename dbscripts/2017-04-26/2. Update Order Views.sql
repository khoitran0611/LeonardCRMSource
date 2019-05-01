UPDATE Eli_Views 
SET QueryScript = N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[AcceptDate] as [AcceptDate] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  [SalesOrder].[SalesPrice] as [SalesPrice] ,  [SalesOrder].[TotalMonthly] as [TotalMonthly] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 7)  ) ' 
WHERE ViewName ='Contract Signed' AND ModuleId = 3


UPDATE Eli_Views 
SET QueryScript = N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  ISNULL([SalesOrder].[SerialNumber],'''') as [SerialNumber] ,  [SalesOrder].[TotalMonthly] as [TotalMonthly] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  ISNULL([SalesOrder].[POSTicketNumber],'''') as [POSTicketNumber] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 8)    Or     (SalesOrder.Status  = 8)  ) ' 
WHERE ViewName ='Paid in Full' AND ModuleId = 3


UPDATE Eli_Views 
SET QueryScript = N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  ISNULL([SalesOrder].[GPOrderNumber],'''') as [GPOrderNumber] ,  [SalesOrder].[StoreNumber] as [StoreNumber] ,  ISNULL([SalesOrder].[PartNumber],'''') as [PartNumber] ,  ISNULL([SalesOrder].[SerialNumber],'''') as [SerialNumber] ,  ISNULL([SalesOrder].[GPCustomerID],'''') as [GPCustomerID] ,  ISNULL([SalesOrder].[POSTicketNumber],'''') as [POSTicketNumber] ,  [a4].Name as [CreatedBy] ,  [a43].Name as [CustomerId] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
LEFT JOIN  [SalesCustomer] as [a43] on [a43].[Id] = [SalesOrder].[CustomerId] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 244)  ) '
WHERE ViewName ='Pending Delivery' AND ModuleId = 3


UPDATE Eli_Views 
SET QueryScript = N'SELECT  [SalesOrder].[Id] as [Id] ,  [dbo].[fn_GetNameInListNameValuesById]([SalesOrder].[Status]) as [Status] ,  [SalesOrder].[CreatedDate] as [CreatedDate] ,  [SalesOrder].[ModifiedDate] as [ModifiedDate] ,  [SalesOrder].[AcceptDate] as [AcceptDate] ,  ISNULL([SalesOrder].[PromoCode],'''') as [PromoCode] ,  [a4].Name as [CreatedBy] ,  (STUFF((Select '','' + [a46].[Name] From [Eli_User] as [a46]  Where [a46].[Id] in  (SELECT VALUE FROM [dbo].SPLIT('','',[SalesOrder].[ResponsibleUsers])) For XML PATH('''')), 1, 1, '''')) as [ResponsibleUsers] ,  ROW_NUMBER() OVER ( ORDER BY [OrderExpression] ) as RowIndex 
FROM  SalesOrder 
LEFT JOIN  [Eli_User] as [a4] on [a4].[Id] = [SalesOrder].[CreatedBy] 
WHERE  ( ([SalesOrder].CreatedBy in (@users))  OR ( '','' + [SalesOrder].ResponsibleUsers like ''%,@onlyuser,%''))   AND  (       (SalesOrder.Status  = 245)  ) '
WHERE ViewName ='Approved Orders' AND ModuleId = 3