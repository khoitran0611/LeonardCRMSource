SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] ON
INSERT [dbo].[Eli_MailTemplates] ([Id], [TemplateName], [Subject], [TemplateContent]) VALUES (48, N'TEMPLATE_MAIL_WELCOME_NEW_USER', N'Welcome to LeonardUSA !', N'<!DOCTYPE html>
<html>
<head>
</head>
<body>
<p>Hello [UserName],</p>
<p>Your account have created on our system. The account infomation:&nbsp;</p>
<p>Login email :&nbsp;[email]</p>
<p>Password :&nbsp;[password]</p>
<p>You can login to&nbsp;<a href="[home_link]">our website</a>&nbsp;now. You should change the password after login.</p>
<p>--------------------------------------------------------------------</p>
<p>This is an automatic email. Please don''t reply on this email.</p>
<p><br /><span style="background-color: #ffffff;">Best regards,</span></p>
<div style="background-color: #ffffff;">&nbsp;</div>
<div style="background-color: #ffffff;"><strong>LeonardUSA</strong></div>
</body>
</html>')
SET IDENTITY_INSERT [dbo].[Eli_MailTemplates] OFF