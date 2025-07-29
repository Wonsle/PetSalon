CREATE TABLE [dbo].[Service](
	[ServiceID] [bigint] IDENTITY(1,1) NOT NULL,
	[ServiceName] [nvarchar](100) NOT NULL,
	[ServiceType] [varchar](100) NOT NULL, -- Links to SystemCode
	[BasePrice] [money] NOT NULL,
	[Duration] [int] NOT NULL, -- Duration in minutes
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[Sort] [int] NOT NULL DEFAULT(0),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- 添加表格和欄位的中文說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務項目管理 - 存放寵物美容的各種服務項目，包含價格、時長等資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務項目唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'ServiceID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務項目名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'ServiceName';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務類型，關聯至SystemCode', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'ServiceType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'基礎價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'BasePrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務時長（分鐘）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'Duration';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'Description';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'IsActive';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'Sort';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Service', @level2type = N'COLUMN', @level2name = N'ModifyTime';