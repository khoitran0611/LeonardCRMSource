ALTER TABLE [dbo].[SalesOrderComplete] ADD [DeliveryPersonInitials] [nvarchar](20) NULL

ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [NeedRepair] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [Answer1] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [Answer2] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [Answer3] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [Answer4] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [Answer5] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [WarrantyReceived] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [Satisfy] [bit] NULL;
ALTER TABLE [dbo].[SalesOrderComplete] ALTER COLUMN [RentToOwn] [bit] NULL;
