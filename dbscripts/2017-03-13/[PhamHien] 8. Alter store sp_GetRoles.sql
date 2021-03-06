ALTER procedure [dbo].[sp_GetRoles]
	@currentRoleId int
AS
	BEGIN
		;with n(Id, Parent) as (
		select Id, Parent from Eli_Roles
		where Id = @currentRoleId
		union all
		select nplus1.Id, nplus1.Parent
		 from 
		Eli_Roles as nplus1 inner join n on ',' + nplus1.Parent + ',' LIKE '%,' + CAST(n.Id AS NVARCHAR) + ',%'
		)
		select * from Eli_Roles where Id in (Select Id from n)
	END
