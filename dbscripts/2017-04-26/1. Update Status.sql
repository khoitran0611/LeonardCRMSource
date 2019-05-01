-- Replace "Approve" By "Contract Signed"
UPDATE Eli_ListValues SET [Description] = 'Contract Signed', AdditionalInfo = 'Please make payment!', Color = '#13777d' WHERE ListNameId = 3 AND Id = 7 

-- Replace "Contract Signed" By "Paid Full"
UPDATE Eli_ListValues SET [Description] = 'Paid Full', AdditionalInfo = 'Full payment made. Please fill the Delivery Request Form!', Color = '#c7c202' WHERE ListNameId = 3 AND Id = 8 

-- Replace "Paid Full" By "Pending Delivery"
UPDATE Eli_ListValues SET [Description] = 'Pending Delivery', AdditionalInfo = 'Delivery Request Form filled. Waiting for delivering building.', Color = '#eda59a' WHERE ListNameId = 3 AND Id = 244 
-- Replace "Pending Delivery" By "Approved"
UPDATE Eli_ListValues SET [Description] = 'Approved', AdditionalInfo = NULL, Color = '#26e04d' WHERE ListNameId = 3 AND Id = 245
 GO 
 
--Update Status
--** "Contract Signed"
UPDATE SalesOrder SET [Status] = 1000 WHERE [Status] = 8

--** "Paid Full"
UPDATE SalesOrder SET [Status] = 1001 WHERE [Status] = 244

--** "Pending Delivery"
UPDATE SalesOrder SET [Status] = 1002 WHERE [Status] = 245

--** Remove all old "Approved"
DELETE SalesOrder WHERE [Status] = 7

GO 

--** "Contract Signed"
UPDATE SalesOrder SET [Status] = 7 WHERE [Status] = 1000

--** "Paid Full"
UPDATE SalesOrder SET [Status] = 8 WHERE [Status] = 1001

--** "Pending Delivery"
UPDATE SalesOrder SET [Status] = 244 WHERE [Status] = 1002

--** Clear the approved data before "Approved"
UPDATE SalesOrder SET IsApproveOrder = NULL, DisapprovedReason = NULL WHERE [Status] < 245
GO 