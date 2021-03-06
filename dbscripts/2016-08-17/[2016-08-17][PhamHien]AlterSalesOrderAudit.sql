ALTER TRIGGER [dbo].[SalesOrderAuditLog]
	ON [dbo].[SalesOrder]
	For INSERT,UPDATE
AS
	declare @operation as char(1)
	declare @valueBefore as nvarchar(max)
	declare @valueAfter as nvarchar(max)
	declare @moduleid as int
	select @moduleid = Id from Eli_Modules where Name ='order'
	
	if EXISTS (select * from deleted)
		BEGIN
		--UPDATE CASE
			set @operation ='U'

			--case update status
			IF(UPDATE(Status))
			BEGIN
				select @valueBefore = deleted.Status from deleted
				select @valueAfter = inserted.Status from inserted

				set @valueBefore = ISNULL(@valueBefore,-1)
				SET @valueAfter = ISNULL(@valueAfter,-1)

				IF @valueAfter <> @valueBefore
				BEGIN
					set @valueBefore =(select Description from Eli_ListValues where Id = CONVERT(int,@valueBefore))
					set @valueAfter = (select Description from Eli_ListValues where Id = CONVERT(int,@valueAfter))

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Status',
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
			 
			--case update AcceptDate
			if(UPDATE(AcceptDate))
				BEGIN
					select @valueBefore = deleted.AcceptDate from deleted
					select @valueAfter = inserted.AcceptDate from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'AcceptDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 --case update PromoCode
			if(UPDATE(PromoCode))
				BEGIN
					select @valueBefore = deleted.PromoCode from deleted
					select @valueAfter = inserted.PromoCode from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PromoCode',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 	 --case update GPOrderNumber
			if(UPDATE(GPOrderNumber))
				BEGIN
					select @valueBefore = deleted.GPOrderNumber from deleted
					select @valueAfter = inserted.GPOrderNumber from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'GPOrderNumber',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update StoreNumber
			if(UPDATE(StoreNumber))
				BEGIN
					select @valueBefore = deleted.StoreNumber from deleted
					select @valueAfter = inserted.StoreNumber from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'StoreNumber',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			 
			 --case update IsOldCustomer
			if(UPDATE(IsOldCustomer))
			BEGIN
				select @valueBefore = deleted.IsOldCustomer from deleted
				select @valueAfter = inserted.IsOldCustomer from inserted

				IF(@valueBefore='1')
					SET @valueBefore = 'True'
				ELSE
					SET @valueBefore = 'False'

				IF(@valueAfter='1')
					SET @valueAfter = 'True'
				ELSE
					SET @valueAfter = 'False'

				IF @valueAfter <> @valueBefore
				BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'IsOldCustomer',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update PartNumber
			if(UPDATE(PartNumber))
				BEGIN
					select @valueBefore = deleted.PartNumber from deleted
					select @valueAfter = inserted.PartNumber from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PartNumber',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update SerialNumber
			if(UPDATE(SerialNumber))
				BEGIN
					select @valueBefore = deleted.SerialNumber from deleted
					select @valueAfter = inserted.SerialNumber from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SerialNumber',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update Color
			if(UPDATE(Color))
				BEGIN
					select @valueBefore = deleted.Color from deleted
					select @valueAfter = inserted.Color from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Color',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update SalesPrice
			if(UPDATE(SalesPrice))
				BEGIN
					select @valueBefore = deleted.SalesPrice from deleted
					select @valueAfter = inserted.SalesPrice from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SalesPrice',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				
			 --case update IsNewPart
			if(UPDATE(IsNewPart))
			BEGIN
				select @valueBefore = deleted.IsNewPart from deleted
				select @valueAfter = inserted.IsNewPart from inserted

				IF(@valueBefore='1')
					SET @valueBefore = 'True'
				ELSE
					SET @valueBefore = 'False'

				IF(@valueAfter='1')
					SET @valueAfter = 'True'
				ELSE
					SET @valueAfter = 'False'

				IF @valueAfter <> @valueBefore
				BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'IsNewPart',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update MonthlyPayment1
			if(UPDATE(MonthlyPayment1))
				BEGIN
					select @valueBefore = deleted.MonthlyPayment1 from deleted
					select @valueAfter = inserted.MonthlyPayment1 from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'MonthlyPayment1',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			
			--case update MonthlyPayment2
			if(UPDATE(MonthlyPayment2))
				BEGIN
					select @valueBefore = deleted.MonthlyPayment2 from deleted
					select @valueAfter = inserted.MonthlyPayment2 from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'MonthlyPayment2',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update PaidAtSigning
			if(UPDATE(PaidAtSigning))
				BEGIN
					select @valueBefore = deleted.PaidAtSigning from deleted
					select @valueAfter = inserted.PaidAtSigning from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PaidAtSigning',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update ReductionPayment
			if(UPDATE(ReductionPayment))
				BEGIN
					select @valueBefore = deleted.ReductionPayment from deleted
					select @valueAfter = inserted.ReductionPayment from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'ReductionPayment',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update Tax
			if(UPDATE(Tax))
				BEGIN
					select @valueBefore = deleted.Tax from deleted
					select @valueAfter = inserted.Tax from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Tax',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update TotalMonthly
			if(UPDATE(TotalMonthly))
				BEGIN
					select @valueBefore = deleted.TotalMonthly from deleted
					select @valueAfter = inserted.TotalMonthly from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'TotalMonthly',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update LesseeSignature
			if(UPDATE(LesseeSignature))
				BEGIN
					select @valueBefore = deleted.LesseeSignature from deleted
					select @valueAfter = inserted.LesseeSignature from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'LesseeSignature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update SignatureDate
			if(UPDATE(SignatureDate))
				BEGIN
					select @valueBefore = deleted.SignatureDate from deleted
					select @valueAfter = inserted.SignatureDate from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SignatureDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update SignatureIP
			if(UPDATE(SignatureIP))
				BEGIN
					select @valueBefore = deleted.SignatureIP from deleted
					select @valueAfter = inserted.SignatureIP from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SignatureIP',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update CoSignature
			if(UPDATE(CoSignature))
				BEGIN
					select @valueBefore = deleted.CoSignature from deleted
					select @valueAfter = inserted.CoSignature from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoSignature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			
			--case update CoSignatureDate
			if(UPDATE(CoSignatureDate))
				BEGIN
					select @valueBefore = deleted.CoSignatureDate from deleted
					select @valueAfter = inserted.CoSignatureDate from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoSignatureDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update CoSignatureIP
			if(UPDATE(CoSignatureIP))
				BEGIN
					select @valueBefore = deleted.CoSignatureIP from deleted
					select @valueAfter = inserted.CoSignatureIP from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoSignatureIP',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update ResponsibleUsers
			if(UPDATE(ResponsibleUsers))
			BEGIN
				select @valueBefore = deleted.ResponsibleUsers from deleted
				select @valueAfter = inserted.ResponsibleUsers from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				SET @valueAfter = ISNULL(@valueAfter,'')

				if	@valueAfter <> @valueBefore
				BEGIN

				--VALUE AFTER
					  WITH    CTE
				  AS ( SELECT   ee.Name
					   FROM     dbo.Eli_User AS ee
	               
					   WHERE    ee.Id in (select value from dbo.SPLIT(',',@valueAfter))
					 )

				 SELECT  @valueAfter = STUFF(( SELECT  ',' + Name
									FROM    CTE
								  FOR
									XML PATH('')
								  ), 1, 1, '');

				--VALUE BEFORE
				  WITH    CTE
				  AS ( SELECT   ee.Name
					   FROM     dbo.Eli_User AS ee
	               
					   WHERE    ee.Id in (select value from dbo.SPLIT(',',@valueBefore))
					 )

				 SELECT  @valueBefore = STUFF(( SELECT  ',' + Name
									FROM    CTE
								  FOR
									XML PATH('')
								  ), 1, 1, '')

				

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'ResponsibleUsers',
					@valueBefore,@valueAfter,					
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

			--case update RentToOwn
			if(UPDATE(RentToOwn))
			BEGIN
				select @valueBefore = deleted.RentToOwn from deleted
				select @valueAfter = inserted.RentToOwn from inserted

				IF(@valueBefore='1')
					SET @valueBefore = 'True'
				ELSE
					SET @valueBefore = 'False'

				IF(@valueAfter='1')
					SET @valueAfter = 'True'
				ELSE
					SET @valueAfter = 'False'

				IF @valueAfter <> @valueBefore
				BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'RentToOwn',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END	
			
			--case update SaleDate
			IF(UPDATE(SaleDate))
				BEGIN
					select @valueBefore = deleted.SaleDate from deleted
					select @valueAfter = inserted.SaleDate from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SaleDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			
			--case update GPCustomerID
			IF(UPDATE(GPCustomerID))
			BEGIN
				select @valueBefore = deleted.GPCustomerID from deleted
				select @valueAfter = inserted.GPCustomerID from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')

				IF @valueAfter <> @valueBefore
				BEGIN

				INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
									ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

				SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'GPCustomerID',
				@valueBefore,@valueAfter,
				inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
				from inserted
				END
			END
			
			--case update IsFinalize
			IF(UPDATE(IsFinalize))
			BEGIN
				select @valueBefore = deleted.IsFinalize from deleted
				select @valueAfter = inserted.IsFinalize from inserted

				IF(@valueBefore='1')
					SET @valueBefore = 'True'
				ELSE
					SET @valueBefore = 'False'

				IF(@valueAfter='1')
					SET @valueAfter = 'True'
				ELSE
					SET @valueAfter = 'False'

				IF @valueAfter <> @valueBefore
				BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'IsFinalize',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update POSTicketNumber
			IF(UPDATE(POSTicketNumber))
			BEGIN
				select @valueBefore = deleted.POSTicketNumber from deleted
				select @valueAfter = inserted.POSTicketNumber from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')

				IF @valueAfter <> @valueBefore
				BEGIN

				INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
									ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

				SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'POSTicketNumber',
				@valueBefore,@valueAfter,
				inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
				from inserted
				END
			END
			
			--case update RampPaidAtSign
			IF(UPDATE(RampPaidAtSign))
			BEGIN
				select @valueBefore = deleted.RampPaidAtSign from deleted
				select @valueAfter = inserted.RampPaidAtSign from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')

				IF @valueAfter <> @valueBefore
				BEGIN

				INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
									ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

				SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'RampPaidAtSign',
				@valueBefore,@valueAfter,
				inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
				from inserted
				END
			 END
			 
			 --case update RampReduction
			IF(UPDATE(RampReduction))
			BEGIN
				select @valueBefore = deleted.RampReduction from deleted
				select @valueAfter = inserted.RampReduction from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')

				IF @valueAfter <> @valueBefore
				BEGIN

				INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
									ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

				SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'RampReduction',
				@valueBefore,@valueAfter,
				inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
				from inserted
				END
			 END
			 
			 --case update RampTax
			IF(UPDATE(RampTax))
			BEGIN
				select @valueBefore = deleted.RampTax from deleted
				select @valueAfter = inserted.RampTax from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')

				IF @valueAfter <> @valueBefore
				BEGIN

				INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
									ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

				SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'RampTax',
				@valueBefore,@valueAfter,
				inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
				from inserted
				END
			 END			 
			 
			--case update RampPartNumber
			IF(UPDATE(RampPartNumber))
			BEGIN
				select @valueBefore = deleted.RampPartNumber from deleted
				select @valueAfter = inserted.RampPartNumber from inserted
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')

				IF @valueAfter <> @valueBefore
				BEGIN

				INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
									ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

				SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'RampPartNumber',
				@valueBefore,@valueAfter,
				inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
				from inserted
				END
			 END		
			 	
			--case update CapitalizationPeriod
			IF(UPDATE(CapitalizationPeriod))
			BEGIN
				select @valueBefore = deleted.CapitalizationPeriod from deleted
				select @valueAfter = inserted.CapitalizationPeriod from inserted

				set @valueBefore = ISNULL(@valueBefore,-1)
				SET @valueAfter = ISNULL(@valueAfter,-1)

				IF @valueAfter <> @valueBefore
				BEGIN
					set @valueBefore =(select Description from Eli_ListValues where Id = CONVERT(int,@valueBefore))
					set @valueAfter = (select Description from Eli_ListValues where Id = CONVERT(int,@valueAfter))

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CapitalizationPeriod',
					@valueBefore,@valueAfter,
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

			SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'New Order inserted','New Order inserted',
			'New Order inserted',inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
			from inserted
		END
