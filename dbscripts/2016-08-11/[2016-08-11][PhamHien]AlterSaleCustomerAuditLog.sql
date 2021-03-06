ALTER TRIGGER [dbo].[SalesCustomerAuditLog]
	ON [dbo].[SalesCustomer]
	For INSERT,UPDATE
AS
	declare @operation as char(1)
	declare @valueBefore as nvarchar(max)
	declare @valueAfter as nvarchar(max)
	declare @moduleid as int
	select @moduleid = Id from Eli_Modules where Name ='customer'

	DECLARE @cusId INT,@createdId INT,@modyfied INT, @respUser NVARCHAR(255)
	
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
			
			--case update DateOfBirth
			if(UPDATE(DateOfBirth))
				BEGIN
					select @valueBefore = deleted.DateOfBirth from deleted
					select @valueAfter = inserted.DateOfBirth from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'DateOfBirth',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update SocialNum
			if(UPDATE(SocialNum))
				BEGIN
					select @valueBefore = deleted.SocialNum from deleted
					select @valueAfter = inserted.SocialNum from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SocialNum',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
				 --case update DriverLicense
			if(UPDATE(DriverLicense))
				BEGIN
					select @valueBefore = deleted.DriverLicense from deleted
					select @valueAfter = inserted.DriverLicense from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'DriverLicense',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END
				 
			--case update HomePhone
			if(UPDATE(HomePhone))
				BEGIN
					select @valueBefore = deleted.HomePhone from deleted
					select @valueAfter = inserted.HomePhone from inserted
					set @valueBefore = ISNULL(@valueBefore,'')
					set @valueAfter = ISNULL(@valueAfter,'')

					IF @valueAfter <> @valueBefore
					BEGIN

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'HomePhone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
					END
				 END

			--case update CellPhone
			if(UPDATE(CellPhone))
			BEGIN
				select @valueBefore = deleted.CellPhone from deleted
				select @valueAfter = inserted.CellPhone from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CellPhone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Email
			if(UPDATE(Email))
			BEGIN
				select @valueBefore = deleted.Email from deleted
				select @valueAfter = inserted.Email from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Email',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update PhysicalStreet
			if(UPDATE(PhysicalStreet))
			BEGIN
				select @valueBefore = deleted.PhysicalStreet from deleted
				select @valueAfter = inserted.PhysicalStreet from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PhysicalStreet',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update PhysicalCity
			if(UPDATE(PhysicalCity))
			BEGIN
				select @valueBefore = deleted.PhysicalCity from deleted
				select @valueAfter = inserted.PhysicalCity from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PhysicalCity',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END			
			 
			 --case update PhysicalState
			if(UPDATE(PhysicalState))
				BEGIN
					select @valueBefore = deleted.PhysicalState from deleted
					select @valueAfter = inserted.PhysicalState from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PhysicalState',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			 --case update PhysicalZip
			if(UPDATE(PhysicalZip))
			BEGIN
				select @valueBefore = deleted.PhysicalZip from deleted
				select @valueAfter = inserted.PhysicalZip from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'PhysicalZip',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			 --case update MailingStreet
			if(UPDATE(MailingStreet))
			BEGIN
				select @valueBefore = deleted.MailingStreet from deleted
				select @valueAfter = inserted.MailingStreet from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'MailingStreet',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			 --case update MailingCity
			if(UPDATE(MailingCity))
			BEGIN
				select @valueBefore = deleted.MailingCity from deleted
				select @valueAfter = inserted.MailingCity from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'MailingCity',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END		 
			 
			 
			   --case update MailingState
			if(UPDATE(MailingState))
				BEGIN
					select @valueBefore = deleted.MailingState from deleted
					select @valueAfter = inserted.MailingState from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'MailingState',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			  --case update MailingZip
			if(UPDATE(MailingZip))
			BEGIN
				select @valueBefore = deleted.MailingZip from deleted
				select @valueAfter = inserted.MailingZip from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'MailingZip',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update AtAddressSince
			if(UPDATE(AtAddressSince))
			BEGIN
				select @valueBefore = deleted.AtAddressSince from deleted
				select @valueAfter = inserted.AtAddressSince from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'AtAddressSince',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			   --case update ResidenceType
			if(UPDATE(ResidenceType))
				BEGIN
					select @valueBefore = deleted.ResidenceType from deleted
					select @valueAfter = inserted.ResidenceType from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'ResidenceType',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			 
			    --case update LandType
			if(UPDATE(LandType))
				BEGIN
					select @valueBefore = deleted.LandType from deleted
					select @valueAfter = inserted.LandType from inserted

					IF @valueAfter <> @valueBefore
					BEGIN

					select @valueBefore =Description  from Eli_ListValues where Id = CONVERT(int,@valueBefore)
					select @valueAfter =Description  from Eli_ListValues where id = CONVERT(int,@valueAfter)

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'LandType',
					ISNULL(@valueBefore,''),
		 			ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			 END
			
			  --case update LandlordName
			if(UPDATE(LandlordName))
			BEGIN
				select @valueBefore = deleted.LandlordName from deleted
				select @valueAfter = inserted.LandlordName from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'LandlordName',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update LandlordPhone
			if(UPDATE(LandlordPhone))
			BEGIN
				select @valueBefore = deleted.LandlordPhone from deleted
				select @valueAfter = inserted.LandlordPhone from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'LandlordPhone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CheckingAccount
			if(UPDATE(CheckingAccount))
			BEGIN
				select @valueBefore = deleted.CheckingAccount from deleted
				select @valueAfter = inserted.CheckingAccount from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CheckingAccount',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update SavingAccount
			if(UPDATE(SavingAccount))
			BEGIN
				select @valueBefore = deleted.SavingAccount from deleted
				select @valueAfter = inserted.SavingAccount from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'SavingAccount',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update SavingAccount
			if(UPDATE(EnrollAutoPay))
			BEGIN
				select @valueBefore = deleted.EnrollAutoPay from deleted
				select @valueAfter = inserted.EnrollAutoPay from inserted

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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'EnrollAutoPay',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update CoName
			if(UPDATE(CoName))
			BEGIN
				select @valueBefore = deleted.CoName from deleted
				select @valueAfter = inserted.CoName from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoName',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			  --case update CoName
			if(UPDATE(CoDateOfBirth))
			BEGIN
				select @valueBefore = deleted.CoDateOfBirth from deleted
				select @valueAfter = inserted.CoDateOfBirth from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoDateOfBirth',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoSocialNum
			if(UPDATE(CoSocialNum))
			BEGIN
				select @valueBefore = deleted.CoSocialNum from deleted
				select @valueAfter = inserted.CoSocialNum from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoSocialNum',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoDriverLicense
			if(UPDATE(CoDriverLicense))
			BEGIN
				select @valueBefore = deleted.CoDriverLicense from deleted
				select @valueAfter = inserted.CoDriverLicense from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoDriverLicense',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoCellPhone
			if(UPDATE(CoCellPhone))
			BEGIN
				select @valueBefore = deleted.CoCellPhone from deleted
				select @valueAfter = inserted.CoCellPhone from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoCellPhone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoEmail
			if(UPDATE(CoEmail))
			BEGIN
				select @valueBefore = deleted.CoEmail from deleted
				select @valueAfter = inserted.CoEmail from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoEmail',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Employer
			if(UPDATE(Employer))
			BEGIN
				select @valueBefore = deleted.Employer from deleted
				select @valueAfter = inserted.Employer from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Employer',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Since
			if(UPDATE(Since))
			BEGIN
				select @valueBefore = deleted.Since from deleted
				select @valueAfter = inserted.Since from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Since',
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
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Phone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update Supervisor
			if(UPDATE(Supervisor))
			BEGIN
				select @valueBefore = deleted.Supervisor from deleted
				select @valueAfter = inserted.Supervisor from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'Supervisor',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoEmployer
			if(UPDATE(CoEmployer))
			BEGIN
				select @valueBefore = deleted.CoEmployer from deleted
				select @valueAfter = inserted.CoEmployer from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoEmployer',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoSince
			if(UPDATE(CoSince))
			BEGIN
				select @valueBefore = deleted.CoSince from deleted
				select @valueAfter = inserted.CoSince from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoSince',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoPhone
			if(UPDATE(CoPhone))
			BEGIN
				select @valueBefore = deleted.CoPhone from deleted
				select @valueAfter = inserted.CoPhone from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoPhone',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update CoSupervisor
			if(UPDATE(CoSupervisor))
			BEGIN
				select @valueBefore = deleted.CoSupervisor from deleted
				select @valueAfter = inserted.CoSupervisor from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'CoSupervisor',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update TypeOfBusiness
			if(UPDATE(TypeOfBusiness))
			BEGIN
				select @valueBefore = deleted.TypeOfBusiness from deleted
				select @valueAfter = inserted.TypeOfBusiness from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'TypeOfBusiness',
					@valueBefore,@valueAfter,
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
			
			--case update BusinessSince
			if(UPDATE(BusinessSince))
			BEGIN
				select @valueBefore = deleted.BusinessSince from deleted
				select @valueAfter = inserted.BusinessSince from inserted
				
				set @valueBefore = ISNULL(@valueBefore,'')
				set @valueAfter = ISNULL(@valueAfter,'')
								
				if	@valueAfter <> @valueAfter
				BEGIN
					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'BusinessSince',
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

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'IsActive',
					ISNULL(@valueBefore,''),
					ISNULL(@valueAfter,''),
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

				--------------------------------[SalesCustomerUsers]------------------------------------------
					SELECT @createdId = inserted.[CreatedBy], @modyfied=inserted.[ModifiedBy],
							@respUser = inserted.ResponsibleUsers, @cusId = inserted.Id
					FROM inserted

					DELETE [dbo].[SalesCustomerUsers]
					WHERE [CustomerId] = @cusId

					INSERT [dbo].[SalesCustomerUsers] ([CustomerId],[UserId],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])				
					SELECT @cusId,Value, GETDATE(),@createdId,GETDATE(),@modyfied
					FROM [dbo].[SPLIT](',',@respUser) 
					--------------------------------[SalesCustomerUsers]------------------------------------------

					INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
										ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

					SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'ResponsibleUsers',
					@valueBefore,@valueAfter,					
					inserted.CreatedDate,inserted.ModifiedBy, inserted.ModifiedBy
					from inserted
				END
			END
		END
		else
		BEGIN
		--INSERT CASE
				
				
				SELECT @createdId = inserted.[CreatedBy], @modyfied=inserted.[ModifiedBy],
						@respUser = inserted.ResponsibleUsers, @cusId = inserted.Id
				FROM inserted

				DELETE [dbo].[SalesCustomerUsers]
				WHERE [CustomerId] = @cusId

				INSERT [dbo].[SalesCustomerUsers] ([CustomerId],[UserId],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])				
				SELECT @cusId,Value, GETDATE(),@createdId,GETDATE(),@modyfied
				FROM [dbo].[SPLIT](',',@respUser) 


			set @operation ='I'
			INSERT INTO Eli_SysAudit (DateModified,ModuleId,MasterRecordId,Operation,ColumnName,
								ValueBefore,ValueAfter,CreatedDate,CreatedBy,ModifiedBy)

			SELECT  inserted.ModifiedDate,@moduleid,inserted.Id,@operation,'New Customer inserted','New Customer inserted',
			'New Customer inserted',inserted.CreatedDate,inserted.CreatedBy, inserted.ModifiedBy
			from inserted
		END
