ALTER TABLE SalesOrder ADD SalesTaxPercent DECIMAL(18,2) NULL

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (722, N'SalesTaxPercent', NULL, N'Sales Tax %', 3, 3, NULL, NULL, 44, 0, NULL, NULL, 1, NULL, 0, NULL, NULL, 0, 1, 0, 0, NULL, CAST(0x0000A62D0103EF2E AS DateTime), NULL, CAST(0x0000A62D0103EF2E AS DateTime), NULL, 1, 0, 0, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF
