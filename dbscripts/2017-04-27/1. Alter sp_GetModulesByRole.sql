ALTER procedure [dbo].[sp_GetModulesByRole]
	@roleId int
AS
	BEGIN
		SELECT distinct vwModules.Id, Name, DisplayName, FeatureName, Description, 
			Dashboard, DefaultTable, IconClass, vwModules.IsActive, 
			vwModules.SortOrder, Parent, IsPublished, 
			vws.Id AS DefaultViewId,
			NeedPickList, ReportModule, MenuIcon,vwModules.[AllowCreateView],
			vwModules.AllowImport, vwModules.AllowExport
		FROM  vwModules inner join Eli_RolesPermissions on vwModules.Id = Eli_RolesPermissions.ModuleId
						LEFT JOIN (SELECT Id, ModuleId 
								   FROM (SELECT Id, ModuleId,DefaultView, ROW_NUMBER() OVER (PARTITION BY ModuleId Order BY DefaultView DESC, SortOrder ASC) AS RowNum 
									     FROM Eli_Views 
									     WHERE (ISNULL(UserRole, '') = '') OR (',' + UserRole + ',') LIKE (N'%,' + CAST(@roleId AS NVARCHAR(250)) + ',%')) tmp 
								   WHERE RowNum = 1) vws ON vws.Id = DefaultViewId OR vws.ModuleId = vwModules.Id
		WHERE RoleId = @roleId --in (select Id from dbo.fn_GetRolesHierachy(@roleId))
		ORDER BY SortOrder
	END