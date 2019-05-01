ALTER TABLE SalesOrder ADD DriverAssigned BIT NOT NULL DEFAULT(0)

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (725, N'DriverAssigned', N'DriverAssigned', N'DriverAssigned', 3, 11, NULL, NULL, 46, 1, NULL, NULL, 1, NULL, 0, NULL, NULL, 0, 1, 0, 0, NULL, CAST(0x0000A62E00E273B2 AS DateTime), NULL, CAST(0x0000A62E00E273B2 AS DateTime), NULL, 1, 0, 0, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF

UPDATE [Eli_EntityFields] SET  Mandatory = 1 WHERE ModuleId = 3 AND FieldName = 'DriverAssigned'