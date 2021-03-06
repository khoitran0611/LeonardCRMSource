/****** Object:  Trigger [dbo].[SalesOrderCompleteAuditLog]    Script Date: 08/08/2016 14:31:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[SalesOrderCompleteAuditLog]
	ON [dbo].[SalesOrderComplete]
	For INSERT,UPDATE
AS
	declare @operation as char(1)
	declare @valueBefore as nvarchar(max)
	declare @valueAfter as nvarchar(max)
	declare @moduleid as int
	select @moduleid = Id from Eli_Modules where Name ='order'

	DECLARE @cusId INT,@createdId INT,@modyfied INT, @respUser NVARCHAR(255)
	
	if EXISTS (select * from deleted)
		BEGIN
		--UPDATE CASE
			set @operation ='U'

			--case update Order
			if(UPDATE(OrderId))
				BEGIN
					select @valueBefore = deleted.OrderId	from deleted
					select @valueAfter =  inserted.OrderId from inserted

					if @valueBefore <> @valueAfter
					BEGIN
					select @valueBefore = GPOrderNumber from SalesOrder where Id = CONVERT(int,@valueBefore)
					select @valueAfter = GPOrderNumber from SalesOrder where Id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)
		
					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'OrderId', 
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			--case update DeliveryDate
			if(UPDATE(DeliveryDate))
				BEGIN
					select @valueBefore = deleted.DeliveryDate from deleted
					select @valueAfter = inserted.DeliveryDate from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			
			--case update NeedRepair
			if(UPDATE(NeedRepair))
			BEGIN
				select @valueBefore = deleted.NeedRepair from deleted
				select @valueAfter = inserted.NeedRepair from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'NeedRepair',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END	
			--case update RepairNote
			if(UPDATE(RepairNote))
				BEGIN
					select @valueBefore = deleted.RepairNote from deleted
					select @valueAfter = inserted.RepairNote from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'RepairNote',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update Answer1
			if(UPDATE(Answer1))
			BEGIN
				select @valueBefore = deleted.Answer1 from deleted
				select @valueAfter = inserted.Answer1 from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Answer1',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			--case update Answer2
			if(UPDATE(Answer2))
			BEGIN
				select @valueBefore = deleted.Answer2 from deleted
				select @valueAfter = inserted.Answer2 from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Answer2',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Answer3
			if(UPDATE(Answer3))
			BEGIN
				select @valueBefore = deleted.Answer3 from deleted
				select @valueAfter = inserted.Answer3 from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Answer3',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Answer4
			if(UPDATE(Answer4))
			BEGIN
				select @valueBefore = deleted.Answer4 from deleted
				select @valueAfter = inserted.Answer4 from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Answer4',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Answer5
			if(UPDATE(Answer5))
			BEGIN
				select @valueBefore = deleted.Answer5 from deleted
				select @valueAfter = inserted.Answer5 from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Answer5',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update DeliveryType
			if(UPDATE(DeliveryType))
				BEGIN
					select @valueBefore = deleted.DeliveryType from deleted
					select @valueAfter = inserted.DeliveryType from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryType',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 						
			--case update DeliveryOther
			if(UPDATE(DeliveryOther))
				BEGIN
					select @valueBefore = deleted.DeliveryOther from deleted
					select @valueAfter = inserted.DeliveryOther from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryOther',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 --case update ActBlocksUsed
			if(UPDATE(ActBlocksUsed))
				BEGIN
					select @valueBefore = deleted.ActBlocksUsed from deleted
					select @valueAfter = inserted.ActBlocksUsed from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'ActBlocksUsed',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update ActHoursOfLabor
			if(UPDATE(ActHoursOfLabor))
				BEGIN
					select @valueBefore = deleted.ActHoursOfLabor from deleted
					select @valueAfter = inserted.ActHoursOfLabor from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'ActHoursOfLabor',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
					END
				 END

			--case update ActMilesToSite
			if(UPDATE(ActMilesToSite))
			BEGIN
				select @valueBefore = deleted.ActMilesToSite from deleted
				select @valueAfter = inserted.ActMilesToSite from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'ActMilesToSite',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update BalanceDue
			if(UPDATE(BalanceDue))
			BEGIN
				select @valueBefore = deleted.BalanceDue from deleted
				select @valueAfter = inserted.BalanceDue from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'BalanceDue',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
				
			--case update AdditionalCharge
			if(UPDATE(AdditionalCharge))
			BEGIN
				select @valueBefore = deleted.AdditionalCharge from deleted
				select @valueAfter = inserted.AdditionalCharge from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'AdditionalCharge',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			--case update PaymentType
			if(UPDATE(PaymentType))
				BEGIN
					select @valueBefore = deleted.PaymentType from deleted
					select @valueAfter = inserted.PaymentType from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'PaymentType',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 	
			--case update BlockCharge
			if(UPDATE(BlockCharge))
			BEGIN
				select @valueBefore = deleted.BlockCharge from deleted
				select @valueAfter = inserted.BlockCharge from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'BlockCharge',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update LaborCharge
			if(UPDATE(LaborCharge))
			BEGIN
				select @valueBefore = deleted.LaborCharge from deleted
				select @valueAfter = inserted.LaborCharge from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'LaborCharge',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			 
			 --case update MileageCharge
			if(UPDATE(MileageCharge))
			BEGIN
				select @valueBefore = deleted.MileageCharge from deleted
				select @valueAfter = inserted.MileageCharge from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'MileageCharge',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			 --case update TotalAmountDue
			if(UPDATE(TotalAmountDue))
			BEGIN
				select @valueBefore = deleted.TotalAmountDue from deleted
				select @valueAfter = inserted.TotalAmountDue from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'TotalAmountDue',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update WarrantyReceived
			if(UPDATE(WarrantyReceived))
			BEGIN
				select @valueBefore = deleted.WarrantyReceived from deleted
				select @valueAfter = inserted.WarrantyReceived from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'WarrantyReceived',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Satisfy
			if(UPDATE(Satisfy))
			BEGIN
				select @valueBefore = deleted.Satisfy from deleted
				select @valueAfter = inserted.Satisfy from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Satisfy',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
				
			 --case update NoSatisfyComment
			if(UPDATE(NoSatisfyComment))
			BEGIN
				select @valueBefore = deleted.NoSatisfyComment from deleted
				select @valueAfter = inserted.NoSatisfyComment from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'NoSatisfyComment',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			 
			  --case update Rating
			if(UPDATE(Rating))
				BEGIN
					select @valueBefore = deleted.Rating from deleted
					select @valueAfter = inserted.Rating from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Rating',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			  --case update Signature
			if(UPDATE([Signature]))
			BEGIN
				select @valueBefore = deleted.[Signature] from deleted
				select @valueAfter = inserted.[Signature] from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Signature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update SignDate
			if(UPDATE(SignDate))
			BEGIN
				select @valueBefore = deleted.SignDate from deleted
				select @valueAfter = inserted.SignDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'SignDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END

			  --case update SignIP
			if(UPDATE(SignIP))
			BEGIN
				select @valueBefore = deleted.SignIP from deleted
				select @valueAfter = inserted.SignIP from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'SignIP',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update FollowUpCall
			if(UPDATE(FollowUpCall))
			BEGIN
				select @valueBefore = deleted.FollowUpCall from deleted
				select @valueAfter = inserted.FollowUpCall from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'FollowUpCall',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update CallDate
			if(UPDATE(CallDate))
			BEGIN
				select @valueBefore = deleted.CallDate from deleted
				select @valueAfter = inserted.CallDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CallDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update SpokeWithCustomer
			if(UPDATE(SpokeWithCustomer))
			BEGIN
				select @valueBefore = deleted.SpokeWithCustomer from deleted
				select @valueAfter = inserted.SpokeWithCustomer from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'SpokeWithCustomer',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update FollowUpComment
			if(UPDATE(FollowUpComment))
			BEGIN
				select @valueBefore = deleted.FollowUpComment from deleted
				select @valueAfter = inserted.FollowUpComment from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'FollowUpComment',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update ManagerSignature
			if(UPDATE(ManagerSignature))
			BEGIN
				select @valueBefore = deleted.ManagerSignature from deleted
				select @valueAfter = inserted.ManagerSignature from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'ManagerSignature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update ManagerSignDate
			if(UPDATE(ManagerSignDate))
			BEGIN
				select @valueBefore = deleted.ManagerSignDate from deleted
				select @valueAfter = inserted.ManagerSignDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'ManagerSignDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update ManagerSignIP
			if(UPDATE(ManagerSignIP))
			BEGIN
				select @valueBefore = deleted.ManagerSignIP from deleted
				select @valueAfter = inserted.ManagerSignIP from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'ManagerSignIP',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update DeliverSignature
			if(UPDATE(DeliverSignature))
			BEGIN
				select @valueBefore = deleted.DeliverSignature from deleted
				select @valueAfter = inserted.DeliverSignature from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliverSignature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update DeliverSignDate
			if(UPDATE(DeliverSignDate))
			BEGIN
				select @valueBefore = deleted.DeliverSignDate from deleted
				select @valueAfter = inserted.DeliverSignDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliverSignDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update DeliverSignIP
			if(UPDATE(DeliverSignIP))
			BEGIN
				select @valueBefore = deleted.DeliverSignIP from deleted
				select @valueAfter = inserted.DeliverSignIP from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliverSignIP',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
						
			--case update IsActive
			if(UPDATE(IsActive))
			BEGIN
				select @valueBefore = deleted.IsActive from deleted
				select @valueAfter = inserted.IsActive from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'IsActive',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END

			--case update LeftVoiceMail
			IF(UPDATE(LeftVoiceMail))
			BEGIN
				select @valueBefore = deleted.LeftVoiceMail from deleted
				select @valueAfter = inserted.LeftVoiceMail from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'LeftVoiceMail',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update DeliveryPersonInitials
			IF(UPDATE(DeliveryPersonInitials))
			BEGIN
				select @valueBefore = deleted.DeliveryPersonInitials from deleted
				select @valueAfter = inserted.DeliveryPersonInitials from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueBefore <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryPersonInitials',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
		END
		else
		BEGIN
		set @operation ='I'
			INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
								ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

			SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'New Order Complete inserted','New Order Complete inserted',
			'New Order inserted',inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
			from inserted
		END
