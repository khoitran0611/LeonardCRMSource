INSERT [dbo].[Eli_MailTemplates] ([TemplateName], [Subject], [TemplateContent]) VALUES ( N'TEMPLATE_MAIL_DIRVER_COMPLETE_NO_CUSTOMER_EMAIL', N'Complete application #[applicant_number] but customer isn''t present', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>The customer was not present for this delivery. Please contact them&nbsp;ASAP and secure signature on the CAF .&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')
INSERT [dbo].[Eli_MailTemplates] ([TemplateName], [Subject], [TemplateContent]) VALUES (N'TEMPLATE_MAIL_DIRVER_COMPLETE_WITH_CUSTOMER_EMAIL', N'Complete application #[applicant_number] but customer isn''t present', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hi,&nbsp;</p>
<p>Your Leonard Building has been&nbsp;delivered. Please either <a href="[home_link]">login</a> to your account or stop by the store to sign&nbsp;the Customer Acceptance Form&nbsp;</p>
<p>Regards,</p>
<p><strong>LeonardUSA</strong></p>
</body>
</html>')