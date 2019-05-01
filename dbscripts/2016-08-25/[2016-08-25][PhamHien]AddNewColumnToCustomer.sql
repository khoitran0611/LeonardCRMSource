ALTER TABLE SalesCustomer ADD IncomeExplanation NVARCHAR(250) NULL

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point]) VALUES (672, N'IncomeExplanation', N'Other Income', N'Other Income', 2, 4, NULL, 250, 50, 0, NULL, NULL, 1, NULL, 0, 1, NULL, NULL, 1, 0, 0, NULL, CAST(0x0000A66D012586B4 AS DateTime), 1, CAST(0x0000A66D012586B4 AS DateTime), 1, 1, 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF


