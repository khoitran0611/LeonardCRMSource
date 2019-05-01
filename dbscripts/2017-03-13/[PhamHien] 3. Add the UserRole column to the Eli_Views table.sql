ALTER TABLE ELi_Views ADD [UserRole] NVARCHAR(50) NULL

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point], [IsLoadReference]) VALUES (719, N'UserRole', N'User Role', N'User Role', 12, 10, NULL, 50, 18, 0, NULL, N'Select Id, Name as Description From Eli_Roles', 1, NULL, 0, 1, 1, 1, 0, 0, 0, 1, CAST(0x0000A7350136776F AS DateTime), 1, CAST(0x0000A7350136776F AS DateTime), 1, 1, 0, 0, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF



