AlTER TABLE SalesCustomer ADD SelfEmployed BIT NULL

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point]) VALUES (668, N'SelfEmployed', N'Self Employed', N'Self Employed', 2, 11, NULL, NULL, 49, 0, NULL, NULL, NULL, NULL, 0, NULL, 1, NULL, 1, 0, 0, NULL, CAST(0x0000A65101211A35 AS DateTime), 1, CAST(0x0000A65101211A35 AS DateTime), 1, 1, 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF

