SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (49, N'TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT_TO_MANAGER', N'Contract data updated for application #[applicant_number]', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>Please be informed that the contract data for the application #<strong>[applicant_number] </strong>has been updated.</p>
<p>Application status:&nbsp;<strong>[applicant_status]</strong></p>
<p>Please review the contract and go to&nbsp;<a href="[home_link]">website</a>&nbsp;to approved it.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF