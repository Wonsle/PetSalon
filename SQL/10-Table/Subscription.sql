/*
訂閱(包月)服務表 - 完整重建版本
支援包月服務管理，包含次數限制、使用統計和狀態管理
*/

-- 如果表存在則先刪除
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Subscription')
BEGIN
    DROP TABLE [dbo].[Subscription]
END
GO

CREATE TABLE [dbo].[Subscription] (
    [SubscriptionID]      BIGINT           IDENTITY (1, 1) NOT NULL,
    [PetID]               BIGINT           NOT NULL,
    [SubscriptionDate]    DATETIME2        NOT NULL,
    [StartDate]           DATETIME2        NOT NULL,
    [EndDate]             DATETIME2        NOT NULL,
    [SubscriptionType]    VARCHAR(20)      NOT NULL,      -- 包月類型 (BATH/GROOM/MIXED)
    [TotalUsageLimit]     INT              NOT NULL,      -- 總次數限制
    [UsedCount]           INT              DEFAULT (0) NOT NULL,  -- 已使用次數
    [ReservedCount]       INT              DEFAULT (0) NOT NULL,  -- 預留次數
    [Status]              VARCHAR(20)      DEFAULT ('ACTIVE') NOT NULL,  -- 狀態 (ACTIVE/PAUSED/EXPIRED/CANCELLED)
    [SubscriptionPrice]   DECIMAL(10,2)    NOT NULL,      -- 包月價格
    [Notes]               NVARCHAR(500)    NULL,          -- 備註
    [CreateUser]          NVARCHAR(50)     NOT NULL,
    [CreateTime]          DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]          NVARCHAR(50)     NOT NULL,
    [ModifyTime]          DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    
    -- 主鍵約束
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([SubscriptionID] ASC),
    
    -- 外鍵約束
    CONSTRAINT [FK_Subscription_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID]),
    CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY ([SubscriptionType]) REFERENCES [dbo].[SubscriptionType] ([TypeCode]),
    
    -- 檢查約束
    CONSTRAINT [CK_Subscription_Type] CHECK ([SubscriptionType] IN ('BATH', 'GROOM', 'MIXED')),
    CONSTRAINT [CK_Subscription_Status] CHECK ([Status] IN ('ACTIVE', 'PAUSED', 'EXPIRED', 'CANCELLED')),
    CONSTRAINT [CK_Subscription_UsageLimit] CHECK ([TotalUsageLimit] > 0),
    CONSTRAINT [CK_Subscription_UsedCount] CHECK ([UsedCount] >= 0),
    CONSTRAINT [CK_Subscription_ReservedCount] CHECK ([ReservedCount] >= 0),
    CONSTRAINT [CK_Subscription_Price] CHECK ([SubscriptionPrice] >= 0),
    CONSTRAINT [CK_Subscription_DateRange] CHECK ([EndDate] > [StartDate]),
    CONSTRAINT [CK_Subscription_Usage] CHECK ([UsedCount] + [ReservedCount] <= [TotalUsageLimit])
);

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_Subscription_PetID] ON [dbo].[Subscription] ([PetID]);
CREATE NONCLUSTERED INDEX [IX_Subscription_Status] ON [dbo].[Subscription] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Subscription_DateRange] ON [dbo].[Subscription] ([StartDate], [EndDate]);
CREATE NONCLUSTERED INDEX [IX_Subscription_Type] ON [dbo].[Subscription] ([SubscriptionType]);

GO

-- 表說明
EXECUTE sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'訂閱服務管理 - 存放寵物月包訂閱服務記錄，支援期間管理、次數限制和使用統計', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Subscription';

-- 欄位說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱服務唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'PetID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務開始日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'StartDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務結束日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'EndDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月類型 (BATH洗澡/GROOM美容/MIXED混合)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'總次數限制', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'TotalUsageLimit';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'已使用次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'UsedCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預留次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ReservedCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狀態 (ACTIVE有效/PAUSED暫停/EXPIRED過期/CANCELLED取消)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionPrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'Notes';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO