/*
預約紀錄
*/
CREATE TABLE [dbo].[ReserveRecord] (
    [ReserveRecordID] BIGINT            IDENTITY (1, 1) NOT NULL,
    [PetID]           BIGINT            NOT NULL,
    [SubscriptionID]  BIGINT            NULL,
    [ReserverDate ]   DATE           NOT NULL,
    [ReserverTime ]   TIME (7)       NOT NULL,
    [Memo]            NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [CreateUser]      NVARCHAR (20)  NOT NULL,
    [CreateTime]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20)  NOT NULL,
    [ModifyTime]      DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ReserveRecordID] ASC),
    CONSTRAINT [FK_ReserveRecord_Pet] FOREIGN KEY ([SubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID])
);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約管理 - 存放寵物美容預約記錄，包含預約時間、備註等資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ReserveRecordID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱服務ID，關聯至Subscription表（可選）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'SubscriptionID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ReserverDate ';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ReserverTime ';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'Memo';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ModifyTime';


