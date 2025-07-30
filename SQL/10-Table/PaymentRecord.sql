/*
付款記錄表 - 完整重建版本
支援包月財務管理、付款類型分類和退費處理
*/

-- 如果表存在則先備份現有資料並刪除表
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PaymentRecord')
BEGIN
    -- 備份現有資料到臨時表
    IF EXISTS (SELECT 1 FROM PaymentRecord)
    BEGIN
        SELECT * INTO PaymentRecordBackup FROM PaymentRecord;
        PRINT '現有PaymentRecord資料已備份至PaymentRecordBackup表';
    END
    
    DROP TABLE [dbo].[PaymentRecord];
    PRINT 'PaymentRecord表已刪除';
END
GO

CREATE TABLE [dbo].[PaymentRecord] (
    [PaymentRecordID]           BIGINT           IDENTITY (1, 1) NOT NULL,
    [PaymentCode]               INT              NULL,
    [PetID]                     BIGINT           NOT NULL,
    [SubscriptionID]            BIGINT           NULL,                    -- 關聯的包月記錄ID
    [ReserveRecordID]           BIGINT           NULL,                    -- 關聯的預約記錄ID
    [PaymentType]               VARCHAR(30)      NOT NULL,                -- 付款類型
    [PaymentMethod]             VARCHAR(20)      DEFAULT ('CASH') NOT NULL, -- 付款方式
    [ReceivablePrice]           MONEY            DEFAULT (0) NOT NULL,    -- 應收金額
    [ActualPrice]               MONEY            DEFAULT (0) NOT NULL,    -- 實收金額
    [DiscountAmount]            MONEY            DEFAULT (0) NOT NULL,    -- 折扣金額
    [TaxAmount]                 MONEY            DEFAULT (0) NOT NULL,    -- 稅額
    [RefundType]                VARCHAR(20)      NULL,                    -- 退費類型
    [OriginalPaymentRecordID]   BIGINT           NULL,                    -- 原始付款記錄ID (用於退費)
    [Notes]                     NVARCHAR(500)    NULL,                    -- 付款備註
    [PaymentDate]               DATETIME2        DEFAULT (GETDATE()) NOT NULL, -- 付款日期
    [CreateUser]                NVARCHAR(50)     NOT NULL,
    [CreateTime]                DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]                NVARCHAR(50)     NOT NULL,
    [ModifyTime]                DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    
    -- 主鍵約束
    CONSTRAINT [PK_PaymentRecord] PRIMARY KEY CLUSTERED ([PaymentRecordID] ASC),
    
    -- 外鍵約束
    CONSTRAINT [FK_PaymentRecord_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID]),
    CONSTRAINT [FK_PaymentRecord_Subscription] FOREIGN KEY ([SubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID]),
    CONSTRAINT [FK_PaymentRecord_ReserveRecord] FOREIGN KEY ([ReserveRecordID]) REFERENCES [dbo].[ReserveRecord] ([ReserveRecordID]),
    CONSTRAINT [FK_PaymentRecord_OriginalPayment] FOREIGN KEY ([OriginalPaymentRecordID]) REFERENCES [dbo].[PaymentRecord] ([PaymentRecordID]),
    
    -- 檢查約束
    CONSTRAINT [CK_PaymentRecord_PaymentType] CHECK ([PaymentType] IN (
        'SUBSCRIPTION_PURCHASE',    -- 包月購買
        'SERVICE_PAYMENT',          -- 服務付款
        'FULL_REFUND',             -- 全額退費
        'PARTIAL_REFUND',          -- 部分退費
        'CANCELLATION_REFUND',     -- 取消退費
        'ADJUSTMENT'               -- 調整
    )),
    CONSTRAINT [CK_PaymentRecord_PaymentMethod] CHECK ([PaymentMethod] IN (
        'CASH',                    -- 現金
        'CARD',                    -- 刷卡
        'TRANSFER',                -- 轉帳
        'MOBILE_PAY',              -- 行動支付
        'OTHER'                    -- 其他
    )),
    CONSTRAINT [CK_PaymentRecord_RefundType] CHECK ([RefundType] IS NULL OR [RefundType] IN (
        'FULL_REFUND',             -- 全額退費
        'PARTIAL_REFUND',          -- 部分退費
        'CANCELLATION_REFUND'      -- 取消退費
    )),
    CONSTRAINT [CK_PaymentRecord_MoneyAmounts] CHECK (
        [ReceivablePrice] >= 0 AND 
        [ActualPrice] >= 0 AND 
        [DiscountAmount] >= 0 AND 
        [TaxAmount] >= 0
    ),
    -- 業務邏輯約束
    CONSTRAINT [CK_PaymentRecord_SubscriptionLogic] CHECK (
        ([PaymentType] = 'SUBSCRIPTION_PURCHASE' AND [SubscriptionID] IS NOT NULL) OR
        ([PaymentType] != 'SUBSCRIPTION_PURCHASE' AND ([SubscriptionID] IS NULL OR [ReserveRecordID] IS NOT NULL))
    ),
    CONSTRAINT [CK_PaymentRecord_ServiceLogic] CHECK (
        ([PaymentType] = 'SERVICE_PAYMENT' AND [ReserveRecordID] IS NOT NULL) OR
        ([PaymentType] != 'SERVICE_PAYMENT')
    ),
    CONSTRAINT [CK_PaymentRecord_RefundLogic] CHECK (
        ([PaymentType] IN ('FULL_REFUND', 'PARTIAL_REFUND', 'CANCELLATION_REFUND') AND [OriginalPaymentRecordID] IS NOT NULL AND [RefundType] IS NOT NULL) OR
        ([PaymentType] NOT IN ('FULL_REFUND', 'PARTIAL_REFUND', 'CANCELLATION_REFUND') AND [OriginalPaymentRecordID] IS NULL AND [RefundType] IS NULL)
    ),
    -- 防止自我參照
    CONSTRAINT [CK_PaymentRecord_NoSelfReference] CHECK ([PaymentRecordID] != [OriginalPaymentRecordID])
);

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_PetID] ON [dbo].[PaymentRecord] ([PetID]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_SubscriptionID] ON [dbo].[PaymentRecord] ([SubscriptionID]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_ReserveRecordID] ON [dbo].[PaymentRecord] ([ReserveRecordID]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_PaymentType] ON [dbo].[PaymentRecord] ([PaymentType]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_PaymentMethod] ON [dbo].[PaymentRecord] ([PaymentMethod]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_PaymentDate] ON [dbo].[PaymentRecord] ([PaymentDate]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_OriginalPayment] ON [dbo].[PaymentRecord] ([OriginalPaymentRecordID]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_CreateTime] ON [dbo].[PaymentRecord] ([CreateTime]);

-- 複合索引用於財務報表查詢
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_Financial] ON [dbo].[PaymentRecord] ([PaymentType], [PaymentDate], [ActualPrice]);
CREATE NONCLUSTERED INDEX [IX_PaymentRecord_PetFinancial] ON [dbo].[PaymentRecord] ([PetID], [PaymentType], [PaymentDate]);

GO

-- 資料遷移腳本 (如果有備份資料)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PaymentRecordBackup')
BEGIN
    INSERT INTO [dbo].[PaymentRecord] (
        [PaymentCode], [PetID], [ReceivablePrice], [ActualPrice], 
        [PaymentType], [PaymentMethod], [CreateUser], [CreateTime], [ModifyUser], [ModifyTime]
    )
    SELECT 
        [PaymentCode], 
        [PetID], 
        ISNULL([ReceivablePrice], 0), 
        ISNULL([ActualPrice], 0),
        'SERVICE_PAYMENT',  -- 預設舊資料為服務付款
        'CASH',             -- 預設付款方式為現金
        [CreateUser], 
        [CreateTime], 
        [ModifyUser], 
        [ModifyTime]
    FROM [PaymentRecordBackup];
    
    PRINT '資料遷移完成';
    
    -- 可選擇是否刪除備份表
    -- DROP TABLE [PaymentRecordBackup];
END

GO

-- 表說明
EXECUTE sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'付款記錄管理 - 存放客戶付款記錄，支援包月購買、服務付款、退費處理等完整財務管理功能', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PaymentRecord';

-- 欄位說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentRecordID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PetID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱服務ID，關聯至Subscription表（包月購買時必填）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'SubscriptionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約記錄ID，關聯至ReserveRecord表（服務付款時必填）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ReserveRecordID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款類型 (SUBSCRIPTION_PURCHASE包月購買/SERVICE_PAYMENT服務付款/FULL_REFUND全額退費/PARTIAL_REFUND部分退費/CANCELLATION_REFUND取消退費/ADJUSTMENT調整)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款方式 (CASH現金/CARD刷卡/TRANSFER轉帳/MOBILE_PAY行動支付/OTHER其他)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentMethod';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'應收金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ReceivablePrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實收金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ActualPrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'折扣金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'DiscountAmount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'稅額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'TaxAmount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'退費類型 (FULL_REFUND全額退費/PARTIAL_REFUND部分退費/CANCELLATION_REFUND取消退費)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'RefundType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始付款記錄ID，用於退費追蹤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'OriginalPaymentRecordID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'Notes';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO

PRINT 'PaymentRecord表結構強化完成，已支援完整的包月財務管理功能';