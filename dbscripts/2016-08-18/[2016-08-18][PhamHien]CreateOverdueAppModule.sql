INSERT [dbo].[Eli_Modules] ([Id], [Name], [DisplayName], [FeatureName], [Description], [Dashboard], [DefaultTable], [IconClass], [MenuIcon], [IsActive], [SortOrder], [Parent], [IsPublished], [RelatedTo], [NeedPickList], [ReportModule], [AllowCreateView], [AllowImport], [AllowExport], [IsWebform]) VALUES (32, N'overdue_applicants', N'Data cleanup', N'Dashboard', N'Purge outdated applications after contract fulfilled', 1, NULL, NULL, N'icons/329131-64.png', 1, 1, 1, 1, NULL, 1, 0, 0, 0, 1, 0)
/****** Object:  Default [DF__Eli_Modul__Allow__2BFE89A6]    S

SET IDENTITY_INSERT [dbo].[Eli_RolesPermissions] ON
INSERT [dbo].[Eli_RolesPermissions] ([Id], [RoleId], [ModuleId], [AllowRead], [AllowCreate], [AllowEdit], [AllowDelete], [AllowImport], [AllowExport], [AllowCreateView]) VALUES (496, 1, 32, 1, 0, 0, 1, 0, 0, 0)
SET IDENTITY_INSERT [dbo].[Eli_RolesPermissions] OFF