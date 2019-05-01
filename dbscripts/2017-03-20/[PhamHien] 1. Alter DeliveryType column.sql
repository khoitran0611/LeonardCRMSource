ALTER TABLE SalesOrderComplete ALTER COLUMN DeliveryType NVARCHAR(50) NULL

UPDATE Eli_EntityFields SET DataTypeId = 10, ListNameId = 100 WHERE FieldName = 'DeliveryType' AND ModuleId = 30
