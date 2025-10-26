/*
預約紀錄表 - 完整重建版本
支援包月服務整合、狀態管理和服務統計
*/
-- 刪除參考 ReserveRecord 的外鍵約束，以允許刪除表
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql = @sql + N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id)) + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N'; '
FROM sys.foreign_keys fk
WHERE fk.referenced_object_id = OBJECT_ID(N'dbo.ReserveRecord');
IF @sql <> N''
BEGIN
    EXEC sp_executesql @sql;
END
-- 如果表存在則先刪除
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ReserveRecord')
BEGIN
    DROP TABLE [dbo].[ReserveRecord]
END
GO

CREATE TABLE [dbo].[ReserveRecord] (
    [ReserveRecordID]           BIGINT            IDENTITY (1, 1) NOT NULL,
    [PetID]                     BIGINT            NOT NULL,
    [SubscriptionID]            BIGINT            NULL,
    [ReserverDate]              DATETIME2         NOT NULL,
    [ReserverTime]              TIME(7)           NOT NULL,
    [Status]                    VARCHAR(20)       DEFAULT ('PENDING') NOT NULL,  -- 預約狀態
    [TotalAmount]               DECIMAL(10,2)     DEFAULT (0) NOT NULL,          -- 服務總價
    [UseSubscription]           BIT               DEFAULT (0) NOT NULL,          -- 是否使用包月
    [ServiceType]               VARCHAR(20)       NULL,                          -- 主要服務類型
    [ServiceDurationMinutes]    INT              DEFAULT (0) NOT NULL,          -- 服務總時長(分鐘)
    [SubscriptionDeductionCount] INT              DEFAULT (0) NOT NULL,          -- 包月扣除次數
    [Memo]                      NVARCHAR(500)     DEFAULT ('') NOT NULL,        -- 備註
    [CreateUser]                NVARCHAR(50)      NOT NULL,
    [CreateTime]                DATETIME2         DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]                NVARCHAR(50)      NOT NULL,
    [ModifyTime]                DATETIME2         DEFAULT (GETDATE()) NOT NULL,

    -- 主鍵約束
    CONSTRAINT [PK_ReserveRecord] PRIMARY KEY CLUSTERED ([ReserveRecordID] ASC),

    -- 外鍵約束
    CONSTRAINT [FK_ReserveRecord_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID]),
    CONSTRAINT [FK_ReserveRecord_Subscription] FOREIGN KEY ([SubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID]),

    -- 檢查約束
    CONSTRAINT [CK_ReserveRecord_Status] CHECK ([Status] IN ('PENDING', 'CONFIRMED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED', 'NO_SHOW')),
    CONSTRAINT [CK_ReserveRecord_ServiceType] CHECK ([ServiceType] IS NULL OR [ServiceType] IN ('BATH', 'GROOM', 'MIXED')),
    CONSTRAINT [CK_ReserveRecord_TotalAmount] CHECK ([TotalAmount] >= 0),
    CONSTRAINT [CK_ReserveRecord_DeductionCount] CHECK ([SubscriptionDeductionCount] >= 0),
    CONSTRAINT [CK_ReserveRecord_SubscriptionLogic] CHECK (
        ([UseSubscription] = 0 AND [SubscriptionID] IS NULL AND [SubscriptionDeductionCount] = 0) OR
        ([UseSubscription] = 1 AND [SubscriptionID] IS NOT NULL AND [SubscriptionDeductionCount] > 0)
    )
);

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_PetID] ON [dbo].[ReserveRecord] ([PetID]);
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_SubscriptionID] ON [dbo].[ReserveRecord] ([SubscriptionID]);
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_Status] ON [dbo].[ReserveRecord] ([Status]);
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_Date] ON [dbo].[ReserveRecord] ([ReserverDate]);
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_DateTime] ON [dbo].[ReserveRecord] ([ReserverDate], [ReserverTime]);
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_ServiceType] ON [dbo].[ReserveRecord] ([ServiceType]);
CREATE NONCLUSTERED INDEX [IX_ReserveRecord_UseSubscription] ON [dbo].[ReserveRecord] ([UseSubscription]);

GO

-- 表說明
EXECUTE sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'預約管理 - 存放寵物美容預約記錄，包含預約時間、服務統計、包月整合等資訊',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'ReserveRecord';

-- 欄位說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ReserveRecordID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'PetID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱服務ID，關聯至Subscription表（可選）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'SubscriptionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ReserverDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ReserverTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約狀態 (PENDING待確認/CONFIRMED已確認/IN_PROGRESS進行中/COMPLETED完成/CANCELLED取消/NO_SHOW未到)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務總價', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'TotalAmount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否使用包月服務', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'UseSubscription';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'主要服務類型 (BATH洗澡/GROOM美容/MIXED混合)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ServiceType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月扣除次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'SubscriptionDeductionCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'Memo';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecord', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO