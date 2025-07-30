/*
訂閱(包月)服務表 - 完整重建版本
支援包月服務管理，包含次數限制、使用統計和狀態管理
*/

-- 如果表存在則先刪除相關的外鍵約束，然後刪除表
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Subscription')
BEGIN
    -- 動態刪除所有參考 Subscription 表的外鍵約束
    DECLARE @sql NVARCHAR(MAX) = ''

    -- 查找所有參考 Subscription 表的外鍵約束
    SELECT @sql = @sql + 'ALTER TABLE [' + SCHEMA_NAME(fk.schema_id) + '].[' + OBJECT_NAME(fk.parent_object_id) + '] DROP CONSTRAINT [' + fk.name + '];' + CHAR(13)
    FROM sys.foreign_keys fk
    INNER JOIN sys.tables t ON fk.referenced_object_id = t.object_id
    WHERE t.name = 'Subscription'

    -- 執行刪除外鍵約束的 SQL
    IF LEN(@sql) > 0
    BEGIN
        PRINT '正在刪除參考 Subscription 表的外鍵約束：'
        PRINT @sql
        EXEC sp_executesql @sql
    END

    -- 現在可以安全地刪除 Subscription 表
    PRINT '刪除 Subscription 表'
    DROP TABLE [dbo].[Subscription]
    PRINT 'Subscription 表已成功刪除'
END
GO

CREATE TABLE [dbo].[Subscription] (
    [SubscriptionID]      BIGINT           IDENTITY (1, 1) NOT NULL,
    [PetID]               BIGINT           NOT NULL,
    [SubscriptionDate]    DATETIME2        NOT NULL,
    [StartDate]           DATETIME2        NOT NULL,
    [EndDate]             DATETIME2        NOT NULL,
    [SubscriptionType]    VARCHAR(20)      NOT NULL,      -- 包月類型 (BATH/GROOM/MIXED)
    [SubscriptionTypeID]  BIGINT           NULL,          -- 包月類型ID (外鍵到SubscriptionType表)
    [TotalUsageLimit]     INT              NOT NULL,      -- 總次數限制
    [UsedCount]           INT              DEFAULT (0) NOT NULL,  -- 已使用次數
    [ReservedCount]       INT              DEFAULT (0) NOT NULL,  -- 預留次數
    [SubscriptionPrice]   DECIMAL(10,2)    NOT NULL,      -- 包月價格
    [Notes]               NVARCHAR(500)    NULL,          -- 備註
    [CreateUser]          NVARCHAR(50)     NOT NULL,
    [CreateTime]          DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]          NVARCHAR(50)     NOT NULL,
    [ModifyTime]          DATETIME2        DEFAULT (GETDATE()) NOT NULL,

    -- 主鍵約束
    CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED ([SubscriptionID] ASC),

    -- 外鍵約束 (暫時註解掉 SubscriptionType 的約束，稍後會重新建立)
    CONSTRAINT [FK_Subscription_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID]),
    -- CONSTRAINT [FK_Subscription_SubscriptionType] FOREIGN KEY ([SubscriptionType]) REFERENCES [dbo].[SubscriptionType] ([TypeCode]),    -- 檢查約束
    CONSTRAINT [CK_Subscription_Type] CHECK ([SubscriptionType] IN ('BATH', 'GROOM', 'MIXED')),
    CONSTRAINT [CK_Subscription_UsageLimit] CHECK ([TotalUsageLimit] > 0),
    CONSTRAINT [CK_Subscription_UsedCount] CHECK ([UsedCount] >= 0),
    CONSTRAINT [CK_Subscription_ReservedCount] CHECK ([ReservedCount] >= 0),
    CONSTRAINT [CK_Subscription_Price] CHECK ([SubscriptionPrice] >= 0),
    CONSTRAINT [CK_Subscription_DateRange] CHECK ([EndDate] > [StartDate]),
    CONSTRAINT [CK_Subscription_Usage] CHECK ([UsedCount] + [ReservedCount] <= [TotalUsageLimit])
);

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_Subscription_PetID] ON [dbo].[Subscription] ([PetID]);
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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月類型ID，關聯至SubscriptionType表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionTypeID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'總次數限制', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'TotalUsageLimit';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'已使用次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'UsedCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預留次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ReservedCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'SubscriptionPrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'Notes';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subscription', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO

-- 重新建立參考 Subscription 表的外鍵約束
-- 注意：這些約束必須在相關表格已經存在的情況下才能建立

-- 重新建立 ReserveRecord 表的外鍵約束
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ReserveRecord')
    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_ReserveRecord_Subscription')
BEGIN
    ALTER TABLE [dbo].[ReserveRecord]
    ADD CONSTRAINT [FK_ReserveRecord_Subscription]
    FOREIGN KEY ([SubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID])
END

-- 重新建立 PaymentRecord 表的外鍵約束
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PaymentRecord')
    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_PaymentRecord_Subscription')
BEGIN
    ALTER TABLE [dbo].[PaymentRecord]
    ADD CONSTRAINT [FK_PaymentRecord_Subscription]
    FOREIGN KEY ([SubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID])
END

-- 重新建立 Subscription 對 SubscriptionType 的外鍵約束
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SubscriptionType')
    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Subscription_SubscriptionType')
BEGIN
    ALTER TABLE [dbo].[Subscription]
    ADD CONSTRAINT [FK_Subscription_SubscriptionType]
    FOREIGN KEY ([SubscriptionTypeID]) REFERENCES [dbo].[SubscriptionType] ([SubscriptionTypeID])
END

-- 重新建立 NotificationLog 表的外鍵約束
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationLog')
    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_NotificationLog_Subscription')
BEGIN
    ALTER TABLE [dbo].[NotificationLog]
    ADD CONSTRAINT [FK_NotificationLog_Subscription]
    FOREIGN KEY ([RelatedSubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID])
END

-- 重新建立 Subscription 表對 SubscriptionType 的外鍵約束
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SubscriptionType')
    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Subscription_SubscriptionType')
BEGIN
    ALTER TABLE [dbo].[Subscription]
    ADD CONSTRAINT [FK_Subscription_SubscriptionType]
    FOREIGN KEY ([SubscriptionType]) REFERENCES [dbo].[SubscriptionType] ([TypeCode])
    PRINT '已重新建立 Subscription 表對 SubscriptionType 的外鍵約束'
END

GO