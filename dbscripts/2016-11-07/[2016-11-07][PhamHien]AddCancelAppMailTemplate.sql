SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (42, N'TEMPLATE_MAIL_CANCEL_APPLICANT', N'Application #[applicant_number] cancelled', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hello ,&nbsp;</p>
<p>The applicant #[applicant_number] is cancelled</p>
<p>Please go to <a href="[home_link]">website</a> to see more infomation.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF
