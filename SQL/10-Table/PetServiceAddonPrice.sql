CREATE TABLE [dbo].[PetServiceAddonPrice](
	[PetServiceAddonPriceID] [bigint] IDENTITY(1,1) NOT NULL,
	[PetID] [bigint] NOT NULL,
	[ServiceAddonID] [bigint] NOT NULL,
	[CustomPrice] [money] NULL, -- Override price for specific pet addon
	[Notes] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_PetServiceAddonPrice] PRIMARY KEY CLUSTERED 
(
	[PetServiceAddonPriceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [FK_PetServiceAddonPrice_Pet] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pet] ([PetID]),
 CONSTRAINT [FK_PetServiceAddonPrice_ServiceAddon] FOREIGN KEY([ServiceAddonID])
REFERENCES [dbo].[ServiceAddon] ([ServiceAddonID])
) ON [PRIMARY]
GO

-- Create unique index to prevent duplicate pet-serviceaddon combinations
CREATE UNIQUE NONCLUSTERED INDEX [IX_PetServiceAddonPrice_Pet_Addon] ON [dbo].[PetServiceAddonPrice]
(
	[PetID] ASC,
	[ServiceAddonID] ASC
)
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- Create index for ServiceAddon queries
CREATE NONCLUSTERED INDEX [IX_PetServiceAddonPrice_ServiceAddon] ON [dbo].[PetServiceAddonPrice]
(
	[ServiceAddonID] ASC
)
INCLUDE ([PetID], [CustomPrice])
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- Create index for Pet queries
CREATE NONCLUSTERED INDEX [IX_PetServiceAddonPrice_Pet] ON [dbo].[PetServiceAddonPrice]
(
	[PetID] ASC
)
INCLUDE ([ServiceAddonID], [CustomPrice])
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- 添加表格和欄位的中文說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物附加服務客製化價格 - 為特定寵物設定不同的附加服務價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物附加服務價格唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'PetServiceAddonPriceID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加服務ID，關聯至ServiceAddon表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'ServiceAddonID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客製化價格，覆蓋附加服務預設價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'CustomPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'Notes';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'IsActive';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceAddonPrice', @level2type = N'COLUMN', @level2name = N'ModifyTime';