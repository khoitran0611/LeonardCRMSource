ALTER TABLE SalesOrder ADD IsApproveOrder BIT NULL

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (721, N'IsApproveOrder', NULL, N'IsApproveOrder', 3, 11, NULL, NULL, 37, 0, NULL, NULL, 1, NULL, 0, 1, NULL, NULL, 1, 0, 0, NULL, CAST(0x0000A652011AA024 AS DateTime), 1, CAST(0x0000A65D01018552 AS DateTime), 1, 1, 0, NULL, NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF