DELETE Eli_ListValues WHERE Id = 8 AND ListNameId = 4

SET IDENTITY_INSERT [dbo].[Eli_ListValues] ON
UPDATE Eli_ListValues SET [Description] = 'Approved', Color = '#26e04d', [AdditionalInfo]='' WHERE Id = 7

INSERT INTO [dbo].[Eli_ListValues]
           (Id
           ,[ListNameId]
           ,[LookupId]
           ,[Description]
           ,[AdditionalInfo]
           ,[ListOrder]
           ,[Editable]
           ,[Color]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[ModifiedDate]
           ,[ModifiedBy]
           ,[Active])
     VALUES (8 ,3 ,4 ,'Contract Signed' ,'Please make payment!',4,0 ,'#13777d' ,GETDATE(), 1,GETDATE(),1,1)
GO

SET IDENTITY_INSERT [dbo].[Eli_ListValues] OFF

UPDATE SalesOrder SET [Status] = 8 WHERE [Status] = 7
