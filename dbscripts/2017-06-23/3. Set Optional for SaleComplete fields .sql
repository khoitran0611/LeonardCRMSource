UPDATE Eli_EntityFields 
SET Mandatory = 0 
WHERE ModuleId = 30 AND FieldName IN ('Signature', 'SignDate', 'SignIP', 'Satisfy', 'WarrantyReceived', 'Rating')