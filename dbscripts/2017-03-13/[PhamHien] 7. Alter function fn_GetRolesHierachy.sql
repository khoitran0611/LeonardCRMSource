ALTER function [dbo].[fn_GetRolesHierachy]
	(@roleId int)
returns table as
return(		with n(Id, Parent, Generation, Hierarchy) as (
			select Id, Parent,0, cast(Name as nvarchar(255)) as hierarchy from Eli_Roles
			where Id = @roleId
			union all
			select nplus1.Id, nplus1.Parent, Generation+1, 
cast(n.hierarchy + '.' + nplus1.Name as nvarchar(255))
			 from 
			Eli_Roles as nplus1 inner join n on ',' + nplus1.Parent + ',' LIKE '%,' + CAST(n.Id AS NVARCHAR) + ',%'
			)
			select Id from Eli_Roles where Id in (Select Id from n)
	
)

