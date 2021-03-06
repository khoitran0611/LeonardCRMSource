ALTER TRIGGER [dbo].[SalesOrderDeliveryAuditLog]
	ON [dbo].[SalesOrderDelivery]
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
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
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
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			--case update DeliveryDate1
			if(UPDATE(DeliveryDate1))
				BEGIN
					select @valueBefore = deleted.DeliveryDate1 from deleted
					select @valueAfter = inserted.DeliveryDate1 from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryDate1',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			
			--case update DeliveryDate2
			if(UPDATE(DeliveryDate2))
				BEGIN
					select @valueBefore = deleted.DeliveryDate2 from deleted
					select @valueAfter = inserted.DeliveryDate2 from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryDate2',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update DeliveryTime
			if(UPDATE(DeliveryTime))
				BEGIN
					select @valueBefore = deleted.DeliveryTime from deleted
					select @valueAfter = inserted.DeliveryTime from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryTime',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
			
			--case update MilesToSite
			if(UPDATE(MilesToSite))
				BEGIN
					select @valueBefore = deleted.MilesToSite from deleted
					select @valueAfter = inserted.MilesToSite from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'MilesToSite',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 	 
			--case update CustomerPresent
			if(UPDATE(CustomerPresent))
			BEGIN
				select @valueBefore = deleted.CustomerPresent from deleted
				select @valueAfter = inserted.CustomerPresent from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CustomerPresent',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END	
			--case update LoadDoorFacing
			if(UPDATE(LoadDoorFacing))
				BEGIN
					select @valueBefore = deleted.LoadDoorFacing from deleted
					select @valueAfter = inserted.LoadDoorFacing from inserted
					select @valueBefore = ISNULL(Description,'')  from Eli_ListValues where Id = @valueBefore
					select @valueAfter = ISNULL(Description, '')  from Eli_ListValues where id = @valueAfter

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'Load Door Facing',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
											
			--case update MoveFromAddress
			if(UPDATE(MoveFromAddress))
				BEGIN
					select @valueBefore = deleted.MoveFromAddress from deleted
					select @valueAfter = inserted.MoveFromAddress from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'MoveFromAddress',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 --case update MoveToAddress
			if(UPDATE(MoveToAddress))
				BEGIN
					select @valueBefore = deleted.MoveToAddress from deleted
					select @valueAfter = inserted.MoveToAddress from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'MoveToAddress',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update DeliveryRequirement
			if(UPDATE(DeliveryRequirement))
				BEGIN
					select @valueBefore = deleted.DeliveryRequirement from deleted
					select @valueAfter = inserted.DeliveryRequirement from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DeliveryRequirement',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END


			--case update DirectionToSite
			if(UPDATE(DirectionToSite))
			BEGIN
				select @valueBefore = deleted.DirectionToSite from deleted
				select @valueAfter = inserted.DirectionToSite from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DirectionToSite',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CustomerInitials
			if(UPDATE(CustomerInitials))
			BEGIN
				select @valueBefore = deleted.CustomerInitials from deleted
				select @valueAfter = inserted.CustomerInitials from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CustomerInitials',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update InitialDate
			if(UPDATE(InitialDate))
			BEGIN
				select @valueBefore = deleted.InitialDate from deleted
				select @valueAfter = inserted.InitialDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'InitialDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CustomerSignature
			if(UPDATE(CustomerSignature))
			BEGIN
				select @valueBefore = deleted.CustomerSignature from deleted
				select @valueAfter = inserted.CustomerSignature from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CustomerSignature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CustomerSignDate
			if(UPDATE(CustomerSignDate))
			BEGIN
				select @valueBefore = deleted.CustomerSignDate from deleted
				select @valueAfter = inserted.CustomerSignDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CustomerSignDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CustomerSignIP
			if(UPDATE(CustomerSignIP))
			BEGIN
				select @valueBefore = deleted.CustomerSignIP from deleted
				select @valueAfter = inserted.CustomerSignIP from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CustomerSignIP',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update WaiverAccepted
			if(UPDATE(WaiverAccepted))
			BEGIN
				select @valueBefore = deleted.WaiverAccepted from deleted
				select @valueAfter = inserted.WaiverAccepted from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'WaiverAccepted',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			--case update CustomerAccepted
			if(UPDATE(CustomerAccepted))
			BEGIN
				select @valueBefore = deleted.CustomerAccepted from deleted
				select @valueAfter = inserted.CustomerAccepted from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'CustomerAccepted',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END		
			--case update DriverName
			if(UPDATE(DriverName))
			BEGIN
				select @valueBefore = deleted.DriverName from deleted
				select @valueAfter = inserted.DriverName from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DriverName',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update DriverSignature
			if(UPDATE(DriverSignature))
			BEGIN
				select @valueBefore = deleted.DriverSignature from deleted
				select @valueAfter = inserted.DriverSignature from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DriverSignature',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			 
			 --case update DriverSignDate
			if(UPDATE(DriverSignDate))
			BEGIN
				select @valueBefore = deleted.DriverSignDate from deleted
				select @valueAfter = inserted.DriverSignDate from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DriverSignDate',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			 --case update DriverSignIP
			if(UPDATE(DriverSignIP))
			BEGIN
				select @valueBefore = deleted.DriverSignIP from deleted
				select @valueAfter = inserted.DriverSignIP from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueBefore
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DriverSignIP',
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
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			END
		else
		BEGIN
		set @operation ='I'
			INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
								ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

			SELECT  inserted.ModifiedDate,@moduleid,inserted.OrderId,@operation,'DRF Completed','',
			'DRF Completed',inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
			from inserted
		END
