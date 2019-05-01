ALTER TABLE SalesOrder ADD IsSold BIT NULL
GO

INSERT [dbo].[Eli_EntityFields] ([FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (N'IsSold', N'Is Sold', N'Is Sold', 3, 11, NULL, NULL, 47, 0, NULL, NULL, 1, NULL, 0, NULL, NULL, 0, 1, 0, 0, NULL, CAST(0x0000A62E00E273B2 AS DateTime), NULL, CAST(0x0000A62E00E273B2 AS DateTime), NULL, 1, 0, 0, NULL, NULL, 0)
GO
