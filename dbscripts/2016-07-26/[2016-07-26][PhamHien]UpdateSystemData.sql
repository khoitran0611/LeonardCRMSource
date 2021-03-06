/****** Object:  Table [dbo].[Eli_MailTemplates]    Script Date: 07/26/2016 16:36:49 ******/
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (33, N'TEMPLATE_MAIL_COMPLETED_ACCEPTANCE', N'LeonardUSA - Acceptance Information for #[applicant_number] completed', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>We have received your acceptance for application #<strong>[applicant_number]</strong>. Please review the infomation in attachment.</p>
<p>The application status:&nbsp;<strong>[applicant_status]</strong></p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF

/****** Object:  Table [dbo].[Eli_EntityFields]    Script Date: 07/26/2016 16:36:49 ******/
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] ON
INSERT [dbo].[Eli_EntityFields] ([Id], [FieldName], [Description], [LabelDisplay], [ModuleId], [DataTypeId], [MinLength], [DataLength], [SortOrder], [Mandatory], [ListNameId], [ListSql], [AdvanceSearch], [DefaultValue], [Deletable], [Searchable], [Sortable], [AllowGroup], [Display], [IsUnique], [PrimaryKey], [ForeignKey], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive], [IsWebform], [IsTextShow], [RegularExpression], [Point]) VALUES (666, N'SaleDate', NULL, N'SaleDate', 29, 2, NULL, NULL, 30, 0, NULL, NULL, 1, NULL, 0, NULL, NULL, 0, 1, 0, 0, NULL, CAST(0x0000A62E00AE247C AS DateTime), NULL, CAST(0x0000A62E00AE247C AS DateTime), NULL, 1, 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Eli_EntityFields] OFF

UPDATE Eli_EntityFields SET [Mandatory] = 0 WHERE [FieldName] = 'RentToOwn' AND [ModuleId] = 30
DELETE Eli_ListValues WHERE Id= 173 AND ListNameId = 3 AND [Description] = 'Deposited'