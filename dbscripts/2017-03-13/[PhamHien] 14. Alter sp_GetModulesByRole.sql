ALTER procedure [dbo].[sp_GetModulesByRole]
	@roleId int
AS
	BEGIN
		SELECT distinct vwModules.Id, Name, DisplayName, FeatureName, Description, 
			Dashboard, DefaultTable, IconClass, IsActive, 
			SortOrder, Parent, IsPublished, DefaultViewId, NeedPickList, ReportModule, MenuIcon,vwModules.[AllowCreateView],
			vwModules.AllowImport, vwModules.AllowExport
		FROM  vwModules inner join Eli_RolesPermissions on vwModules.Id = Eli_RolesPermissions.ModuleId
		WHERE RoleId = @roleId --in (select Id from dbo.fn_GetRolesHierachy(@roleId))
		ORDER BY SortOrder
	END
