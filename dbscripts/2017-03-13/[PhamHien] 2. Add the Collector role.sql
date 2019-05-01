SET IDENTITY_INSERT [dbo].[Eli_Roles] ON

INSERT INTO [dbo].[Eli_Roles]
			(Id
           ,[Name]
           ,[Description]
           ,[Parent]
           ,[IsHostAdmin]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
     VALUES (26, 'Collector', 'Collector', 1, 0, 1, GETDATE(), 1, GETDATE())             
GO

SET IDENTITY_INSERT [dbo].[Eli_Roles] OFF
