ALTER TABLE SalesOrder Add NumberOfPayment INT NOT NULL DEFAULT(2)

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (710, N'NumberOfPayment', N'NumberOfPayment', N'Qty of Payments', 3, 1, NULL, NULL, 40, 1, NULL, NULL, 1, NULL, 0, 1, 1, 0, 1, 0, 0, NULL, CAST(0x0000A66E00C1C966 AS DateTime), 1, CAST(0x0000A66E00C1C967 AS DateTime), 1, 1, 0, 0, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF

