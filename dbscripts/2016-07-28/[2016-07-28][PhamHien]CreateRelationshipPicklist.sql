SET IDENTITY_INSERT [dbo].[Eli_ListNames] ON
INSERT [dbo].[Eli_ListNames] ([Id], [ListName], [Description], [Module], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (104, N'Relationship', N'Relationship', N'2,', CAST(0x0000A65100A91712 AS DateTime), 1, CAST(0x0000A65100A91712 AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[Eli_ListNames] OFF

SET IDENTITY_INSERT [dbo].[Eli_ListValues] ON
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (384, 104, 0, N'Parent', NULL, 1, 1, NULL, CAST(0x0000A65100A91712 AS DateTime), 1, CAST(0x0000A65100A91712 AS DateTime), 1, 1)
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (385, 104, 0, N'Sibling', N'', 2, 1, NULL, CAST(0x0000A65100A91712 AS DateTime), 1, CAST(0x0000A65100A91713 AS DateTime), 1, 1)
INSERT [dbo].[Eli_ListValues] ([Id], [ListNameId], [LookupId], [Description], [AdditionalInfo], [ListOrder], [Editable], [Color], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [Active]) VALUES (386, 104, 0, N'Other', N'', 3, 1, NULL, CAST(0x0000A65100A91713 AS DateTime), 1, CAST(0x0000A65100A91713 AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[Eli_ListValues] OFF