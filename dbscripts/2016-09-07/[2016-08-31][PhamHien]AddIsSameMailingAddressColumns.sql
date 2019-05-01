ALTER TABLE SalesCustomer ADD IsSameMailingAddress BIT NULL 

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (708, N'IsSameMailingAddress', NULL, N'Is Same Mailing Address', 2, 11, NULL, NULL, 52, 0, NULL, NULL, 1, NULL, 0, 1, 1, NULL, 0, 0, 0, NULL, CAST(0x0000A67A0124E2F6 AS DateTime), 1, CAST(0x0000A67A0124E2F6 AS DateTime), 1, 1, 0, NULL, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF
