ALTER TABLE SalesOrder ALTER COLUMN Color nvarchar(50) NULL 

UPDATE Eli_EntityFields SET DataLength = 50 WHERE FieldName = 'Color' AND ModuleId = 3