ALTER VIEW [dbo].[vwViewMenu]
AS
SELECT        dbo.Eli_Modules.Id AS ModuleId, dbo.Eli_Modules.Name AS ModuleName, dbo.Eli_Views.ViewName, dbo.Eli_Views.Id AS ViewId, 0 AS Total, dbo.Eli_Views.Shared, dbo.Eli_Views.CreatedBy, 
                         dbo.Eli_Views.DefaultView, dbo.Eli_Views.SortOrder, dbo.Eli_Views.UserRole
FROM            dbo.Eli_Modules INNER JOIN
                         dbo.Eli_Views ON dbo.Eli_Modules.Id = dbo.Eli_Views.ModuleId
WHERE        (dbo.Eli_Views.IsActive = 1) AND (dbo.Eli_Views.ParentId = '' OR
                         dbo.Eli_Views.ParentId IS NULL) AND (dbo.Eli_Views.IsPublic = 1)


GO
