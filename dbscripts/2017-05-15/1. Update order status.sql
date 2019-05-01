--**Pending Customer Acceptance
--Change 'InProgress' to 'Pending Customer Acceptance'
UPDATE Eli_ListValues SET [Description] = N'Pending Customer Acceptance', Color='#f0ca69' WHERE [Description] = N'In Progress' AND Id = 6 
																											  AND ListNameId = 3
GO
--Update order 
UPDATE SalesOrder SET [Status] = 6 WHERE [Status] = 7 OR [Status] = 8 -- Contract Signed OR Paid Full status
GO

--**InProgress
--Change 'PaidFull' to 'InProgress'
UPDATE Eli_ListValues SET [Description] = N'In Progress', Color = '#9ac4f5' WHERE ([Description] = N'Paid Full' OR [Description] = 'Deposit Paid in Full') AND Id = 8 
																						    AND ListNameId = 3
GO
--Update order 
UPDATE SalesOrder SET [Status] = 8 WHERE [Status] = 244 --Pending Delivery (old)
GO

--**Pending Delivery
--Update order 
UPDATE SalesOrder SET [Status] = 244 WHERE [Status] = 245 --Approve status
GO

--remove the Contract Signed & Approved
DELETE Eli_ListValues WHERE ([Description] = N'Contract Signed' OR [Description] = N'Approved') AND ListNameId = 3
GO






