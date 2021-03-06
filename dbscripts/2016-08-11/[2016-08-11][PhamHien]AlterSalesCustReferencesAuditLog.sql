ALTER TRIGGER [dbo].[SalesCustReferencesAuditLog]
	ON [dbo].[SalesCustReferences]
	For INSERT,UPDATE
AS
	declare @operation as char(1)
	declare @valueBefore as nvarchar(max)
	declare @valueAfter as nvarchar(max)
	declare @moduleid as int
	select @moduleid = Id from Eli_Modules where Name ='customer_reference'
	
	if EXISTS (select * from deleted)
		BEGIN
		--UPDATE CASE
			set @operation ='U'

			--case update Name
			if(UPDATE(Name))
				BEGIN
					select @valueBefore = deleted.Name from deleted
					select @valueAfter = inserted.Name from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Name',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 --case update Relationship
			if(UPDATE(Relationship))
				BEGIN
					select @valueBefore = deleted.Relationship from deleted
					select @valueAfter = inserted.Relationship from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Relationship',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 	 --case update Phone
			if(UPDATE(Phone))
				BEGIN
					select @valueBefore = deleted.Phone from deleted
					select @valueAfter = inserted.Phone from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Phone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END

			--case update invoice Customer
			if(UPDATE(CustomerId))
				BEGIN
					select @valueBefore = deleted.CustomerId	from deleted
					select @valueAfter =  inserted.CustomerId from inserted

					if @valueBefore <> @valueAfter
					BEGIN
					select @valueBefore = Name from SalesCustomer where Id = CONVERT(int,@valueBefore)
					select @valueAfter = Name from SalesCustomer where Id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)
		
					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CustomerId', 
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			--case update IsActive
			if(UPDATE(IsActive))
			BEGIN
				select @valueBefore = deleted.IsActive from deleted
				select @valueAfter = inserted.IsActive from inserted
				IF @valueAfter <> @valueBefore
				BEGIN
					IF CONVERT(bit, @valueBefore) = 1
					   SET 	@valueBefore = 'Active'
					ELSE
						SET @valueBefore = 'InActive'
			
					IF CONVERT(bit, @valueAfter) = 1
					   SET 	@valueAfter = 'Active'
					ELSE
						SET @valueAfter = 'InActive'
			

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'IsActive',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END

		END
		else
		BEGIN
		--INSERT CASE
			set @operation ='I'
			INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
								ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

			SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'New Customer Reference inserted','New Customer Reference inserted',
			'New Customer Reference inserted',inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
			from inserted
		END
