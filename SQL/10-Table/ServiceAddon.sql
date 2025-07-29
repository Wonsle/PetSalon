CREATE TABLE [dbo].[ServiceAddon](
	[ServiceAddonID] [bigint] IDENTITY(1,1) NOT NULL,
	[AddonName] [nvarchar](100) NOT NULL,
	[AddonType] [varchar](100) NOT NULL, -- Links to SystemCode for addon types
	[AddonPrice] [money] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[Sort] [int] NOT NULL DEFAULT(0),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_ServiceAddon] PRIMARY KEY CLUSTERED 
(
	[ServiceAddonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- 添加表格和欄位的中文說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務附加項目管理 - 存放美容服務的附加項目，如造型、特殊處理等', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加項目唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'ServiceAddonID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加項目名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'AddonName';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加項目類型，關聯至SystemCode', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'AddonType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加項目價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'AddonPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加項目描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'Description';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'IsActive';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'Sort';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ServiceAddon', @level2type = N'COLUMN', @level2name = N'ModifyTime';