SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (38, N'TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT_TO_CUSTOMER', N'LeonardUSA - Application #[applicant_number] Updated', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>Sale Manager&nbsp;<strong>[UserName]</strong> updated your application #<strong>[applicant_number]</strong>.</p>
<p>Application status:&nbsp;<strong>[applicant_status]</strong></p>
<p>Please review and continue to work with it.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF
