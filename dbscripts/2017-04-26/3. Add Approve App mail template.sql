INSERT [dbo].[Eli_MailTemplates] ([TemplateName], [Subject], [TemplateContent]) VALUES (N'TEMPLATE_MAIL_APPROVED_APPLICANT', N'Approved application #[applicant_number]', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>Please be informed that the your application #<strong>[applicant_number]&nbsp;</strong>has been appoved by our manager.</p>
<p>Please review the contract and go to&nbsp;<a href="[home_link]">website</a>&nbsp;to review it.</p>
<p>&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')