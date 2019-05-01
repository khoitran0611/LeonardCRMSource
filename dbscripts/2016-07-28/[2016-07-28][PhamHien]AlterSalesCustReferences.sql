UPDATE SalesCustReferences SET Relationship = '384'

ALTER TABLE SalesCustReferences
ALTER COLUMN Relationship INT NOT NULL

UPDATE Eli_EntityFields SET DataTypeId = 5, ListNameId = 104 WHERE FieldName = 'Relationship' AND ModuleId = 28
