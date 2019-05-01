--1.Add column IsLoadReference to Eli_EntityFields
--2.Update view vwViewColumns
--3.Update procedure GetReferenceListValues
--4.Update trigger trgChangeActive

GO
--Add column IsLoadReference to Eli_EntityFields
ALTER TABLE Eli_EntityFields ADD IsLoadReference BIT NOT NULL DEFAULT(0)

GO

--add Entity field
INSERT [dbo].[Eli_EntityFields] ([FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow]) VALUES (N'IsLoadReference', N'Is load reference', N'Is load reference', 18, 11, NULL, NULL, 28, 0, NULL, NULL, 1, NULL, 0, 0, 0, 0, 1, 0, 0, 0, CAST(0x0000A4B300C6FCD1 AS DateTime), 1, CAST(0x0000A4B300C6FCD1 AS DateTime), 1, 1, 0, NULL)



GO
--UPdate view vwViewColumns
ALTER VIEW [dbo].[vwViewColumns]
AS
SELECT        dbo.Eli_EntityFields.Id AS FieldId, dbo.Eli_EntityFields.FieldName AS ColumnName, dbo.Eli_EntityFields.ModuleId, dbo.Eli_EntityFields.SortOrder, dbo.Eli_DataTypes.IsDate, dbo.Eli_DataTypes.IsList, 
                         dbo.Eli_DataTypes.IsMultiSelecttBox, dbo.Eli_DataTypes.IsCheckBox, dbo.Eli_EntityFields.ListNameId, dbo.Eli_EntityFields.LabelDisplay, dbo.Eli_EntityFields.ForeignKey, dbo.Eli_EntityFields.ListSql, 
                         dbo.Eli_EntityFields.AdvanceSearch, dbo.Eli_EntityFields.Display, dbo.Eli_DataTypes.IsDateTime, dbo.Eli_DataTypes.IsDecimal, dbo.Eli_DataTypes.IsInteger, dbo.Eli_EntityFields.Sortable, 
                         dbo.Eli_DataTypes.IsCurrency, dbo.Eli_EntityFields.AllowGroup,[RoleId], [Locked],dbo.Eli_EntityFields.Point,
						 Case WHEN(CAST(dbo.Eli_EntityFields.Display AS INT) + CAST([Eli_RolesFields].Visible AS INT)) = 2 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)END AS [Visible],
						 dbo.Eli_EntityFields.IsTextShow, dbo.Eli_EntityFields.Mandatory, dbo.Eli_EntityFields.IsLoadReference
FROM            dbo.Eli_EntityFields 
				INNER JOIN dbo.Eli_DataTypes ON dbo.Eli_EntityFields.DataTypeId = dbo.Eli_DataTypes.Id 
                INNER JOIN [dbo].[Eli_RolesFields] ON [Eli_RolesFields].[FieldId] = [Eli_EntityFields].[Id]
WHERE        (dbo.Eli_EntityFields.IsActive = 1)




GO
-- Update procedure GetReferenceListValues
ALTER PROCEDURE [dbo].[GetReferenceListValues] 
	@moduleId int
AS
BEGIN
	
	declare @temp table(FieldId int, FieldName nvarchar(255), Id int, [Description] nvarchar(250))
	declare @scripts table(FieldId int, FieldName nvarchar(255), ListSQL nvarchar(max))
	declare @sql nvarchar(max)
	insert into @scripts
	select Id, FieldName, ListSql from Eli_EntityFields where ListSql is not null and ModuleId = @moduleId AND IsLoadReference = 1
	declare @i int
	set @i = (select MIN(FieldId) from @scripts)
	while(@i is not null)
	begin
		set @sql = (select ListSQL from @scripts where FieldId = @i)
		declare @x nvarchar(6)
		set @x = convert(nvarchar(6),@i)
		declare @name nvarchar(255)
		set @name  = (select FieldName from @scripts where FieldId = @i)
		insert into @temp
		exec (N'select '+ @x + ',''' + @name +''',v.* from (' + @sql + ') as v');
		set @i = (select MIN(FieldId) from @scripts where FieldId > @i)
	end
	select * from @temp
END




