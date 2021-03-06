
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (26, N'TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT', N'LeonardUSA - Application #[applicant_number] Updated', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>Customer <strong>[UserName]</strong> updated the application #<strong>[applicant_number]</strong>.</p>
<p>Application status:&nbsp;<strong>[applicant_status]</strong></p>
<p>Please review and get back to them soon.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (27, N'TEMPLATE_MAIL_SEND_CONTRACT_COPY', N'LeonardUSA - Contract Signed For Application #[applicant_number]', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi <strong>[UserName]</strong>,&nbsp;</p>
<p>Thank you for signing the contract for application <strong>#[applicant_number]</strong>.</p>
<p>Please see the copy of contract in the attachment of this mail.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF
