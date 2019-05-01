ALTER TABLE SalesOrder
ADD 
[RentToOwn] [bit] NULL,
[SaleDate] [date] NULL

ALTER TABLE SalesOrderDelivery DROP COLUMN [SaleDate]

ALTER TABLE SalesOrderComplete DROP COLUMN [RentToOwn]

UPDATE Eli_EntityFields SET ModuleId = 3 WHERE (FieldName = 'SaleDate' AND ModuleId = 29) OR (FieldName = 'RentToOwn' AND ModuleId = 30)
