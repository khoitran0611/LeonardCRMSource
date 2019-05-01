
SET IDENTITY_INSERT [dbo].[Eli_ListNames] ON
INSERT [dbo].[Eli_ListNames] ([Id], [ListName], [Description], [Module], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (105, N'Load Door Facing', N'Load Door Facing', N'29,', CAST(0x0000A77C0101E63B AS DateTime), 1, CAST(0x0000A77C010211DA AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[Eli_ListNames] OFF

SET IDENTITY_INSERT [dbo].[Eli_ListValues] ON
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (389, 105, 0, N'Cab', NULL, 1, 1, NULL, CAST(0x0000A77C0101E63B AS DateTime), 1, CAST(0x0000A77C010211DA AS DateTime), 1, 1)
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (390, 105, 0, N'Rear', N'', 2, 1, NULL, CAST(0x0000A77C0101E63B AS DateTime), 1, CAST(0x0000A77C010211DA AS DateTime), 1, 1)
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (391, 105, 0, N'Driver’s Side', N'', 3, 1, NULL, CAST(0x0000A77C0101E63C AS DateTime), 1, CAST(0x0000A77C010211DA AS DateTime), 1, 1)
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (392, 105, 0, N'Passenger’s Side', N'', 4, 1, NULL, CAST(0x0000A77C0101E63C AS DateTime), 1, CAST(0x0000A77C010211DA AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[Eli_ListValues] OFF

GO


UPDATE SalesOrderDelivery SET LoadDoorFacing = NULL
GO

ALTER TABLE SalesOrderDelivery ALTER COLUMN LoadDoorFacing INT NULL
GO


UPDATE Eli_EntityFields SET ListNameId = 105, DataTypeId = 5, ForeignKey = 0, MinLength = NULL, [DataLength] = NULL WHERE ModuleId = 29 AND FieldName = 'LoadDoorFacing'
GO

