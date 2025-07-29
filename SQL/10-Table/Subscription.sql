/*
訂閱(包月)
*/
CREATE TABLE [dbo].[Subscription] (
    [SubscriptionID]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [PetID]            BIGINT           NOT NULL,
    [SubscriptionDate] DATE          NOT NULL,
    [StartDate]        DATE          NOT NULL,
    [EndDate]          DATE          NOT NULL,
    [CreateUser]       NVARCHAR (20) NOT NULL,
    [CreateTime]       DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]       NVARCHAR (20) NOT NULL,
    [ModifyTime]       DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([SubscriptionID] ASC),
    CONSTRAINT [FK_Subscription_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID])
);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱服務管理 - 存放寵物月包訂閱服務記錄，支援期間管理', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱服務唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionDate';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務開始日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'StartDate';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務結束日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'EndDate';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ModifyTime';

