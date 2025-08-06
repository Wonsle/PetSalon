CREATE TABLE [dbo].[PetServiceDuration](
	[PetServiceDurationID] [bigint] IDENTITY(1,1) NOT NULL,
	[PetID] [bigint] NOT NULL,
	[ServiceID] [bigint] NOT NULL,
	[CustomDuration] [int] NULL, -- Override duration for specific pet service (in minutes)
	[Notes] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_PetServiceDuration] PRIMARY KEY CLUSTERED 
(
	[PetServiceDurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [FK_PetServiceDuration_Pet] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pet] ([PetID]),
 CONSTRAINT [FK_PetServiceDuration_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ServiceID])
) ON [PRIMARY]
GO

-- Create unique index to prevent duplicate pet-service combinations
CREATE UNIQUE NONCLUSTERED INDEX [IX_PetServiceDuration_Pet_Service] ON [dbo].[PetServiceDuration]
(
	[PetID] ASC,
	[ServiceID] ASC
)
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- Create index for Service queries
CREATE NONCLUSTERED INDEX [IX_PetServiceDuration_Service] ON [dbo].[PetServiceDuration]
(
	[ServiceID] ASC
)
INCLUDE ([PetID], [CustomDuration])
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- 添加表格和欄位的中文說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物服務客製化時間 - 為特定寵物設定不同的服務時長', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物服務時間唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'PetServiceDurationID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務項目ID，關聯至Service表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'ServiceID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客製化時長，覆蓋服務預設時長（以分鐘為單位）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'CustomDuration';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'Notes';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'IsActive';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetServiceDuration', @level2type = N'COLUMN', @level2name = N'ModifyTime';