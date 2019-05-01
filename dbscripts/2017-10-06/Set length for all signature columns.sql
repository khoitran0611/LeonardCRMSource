ALTER TABLE SalesOrder ALTER COLUMN LesseeSignature NVARCHAR(50) NULL

ALTER TABLE SalesOrder ALTER COLUMN CoSignature NVARCHAR(50) NULL

ALTER TABLE SalesOrderDelivery ALTER COLUMN CustomerSignature NVARCHAR(50) NULL

ALTER TABLE SalesOrderDelivery ALTER COLUMN DriverSignature NVARCHAR(50) NULL

ALTER TABLE SalesOrderComplete ALTER COLUMN ManagerSignature NVARCHAR(50) NULL

ALTER TABLE SalesOrderComplete ALTER COLUMN [Signature] NVARCHAR(50) NULL


UPDATE Eli_EntityFields 
SET DataLength = 50 
WHERE (ModuleId = 3 AND FieldName IN (N'LesseeSignature', N'CoSignature')) OR 
	  (ModuleId = 29 AND FieldName IN (N'CustomerSignature', N'DriverSignature'))  OR 
	  (ModuleId = 30 AND FieldName IN (N'SalesOrderDelivery', N'SalesOrderDelivery'))
