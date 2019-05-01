ALTER TRIGGER [dbo].[OnOrderDeleted]
	ON [dbo].[SalesOrder]
	For DELETE
AS
	DELETE FROM Eli_FieldData 
	WHERE Eli_FieldData.MaterRecordId IN (SELECT deleted.Id from deleted)
	
	DELETE FROM SalesCustomer WHERE Id IN (SELECT deleted.CustomerId from deleted)
