SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (43, N'TEMPLATE_MAIL_DISAPPOVE_APPLICANT', N'Application #[applicant_number] rejected', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,</p>
<p>The application #<strong>[applicant_number]</strong> was rejected because [disapprove_reason].&nbsp;</p>
<p>Please go to <a href="[home_link]">website</a> to see more infomation.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF
