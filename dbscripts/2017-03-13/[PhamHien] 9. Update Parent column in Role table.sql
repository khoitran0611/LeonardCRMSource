ALTER TABLE [dbo].[Eli_Roles] ALTER COLUMN	[Parent] [nvarchar](50) NULL

UPDATE Eli_EntityFields SET DataTypeId = 10, [DataLength] = 50 WHERE FieldName = 'Parent' AND ModuleId = 8

