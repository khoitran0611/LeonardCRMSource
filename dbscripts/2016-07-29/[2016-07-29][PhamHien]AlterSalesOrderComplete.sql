ALTER TABLE SalesOrderComplete ALTER COLUMN CallDate datetime NULL

UPDATE Eli_EntityFields SET DataTypeId = 13 WHERE FieldName = 'CallDate' AND ModuleId = '30'

ALTER TABLE SalesOrderComplete ADD LeftVoiceMail bit NULL

ALTER TABLE SalesOrder ADD IsFinalize bit NULL

SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point]) VALUES (670, N'LeftVoiceMail', N'Left Voice Mail', N'Left Voice Mail', 30, 11, NULL, NULL, 47, 0, NULL, NULL, 1, NULL, 0, 1, NULL, NULL, 1, 0, 0, NULL, CAST(0x0000A65201066972 AS DateTime), 1, CAST(0x0000A65201066972 AS DateTime), 1, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point]) VALUES (671, N'IsFinalize', N'Finalize', N'Finalize', 3, 11, NULL, NULL, 37, 0, NULL, NULL, 1, NULL, 0, 1, NULL, NULL, 1, 0, 0, NULL, CAST(0x0000A652011AA024 AS DateTime), 1, CAST(0x0000A652011AA024 AS DateTime), 1, 1, 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF