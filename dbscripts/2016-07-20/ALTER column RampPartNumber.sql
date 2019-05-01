Alter table SalesOrder
Alter column RampPartNumber nvarchar(20)
GO
Update Eli_EntityFields set DataLength = 20 where FieldName = 'RampPartNumber'
GO