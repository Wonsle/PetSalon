CREATE TABLE [dbo].[ReservationService](
	[ReservationServiceID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReserveRecordID] [bigint] NOT NULL,
	[ServiceID] [bigint] NOT NULL,
	[ServicePrice] [money] NOT NULL,
	[Duration] [int] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_ReservationService] PRIMARY KEY CLUSTERED 
(
	[ReservationServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [FK_ReservationService_ReserveRecord] FOREIGN KEY([ReserveRecordID])
REFERENCES [dbo].[ReserveRecord] ([ReserveRecordID]),
 CONSTRAINT [FK_ReservationService_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ServiceID])
) ON [PRIMARY]
GO

-- 添加表格和欄位的中文說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約服務明細 - 記錄每個預約包含的具體服務項目及價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約服務明細唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'ReservationServiceID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約記錄ID，關聯至ReserveRecord表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'ReserveRecordID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務項目ID，關聯至Service表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'ServiceID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'ServicePrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務時長（分鐘）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'Duration';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'Notes';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReservationService', @level2type = N'COLUMN', @level2name = N'ModifyTime';