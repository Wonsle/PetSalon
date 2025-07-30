/*
通知記錄表
記錄所有包月相關的通知，追蹤通知發送狀態，支援後續的通知系統擴展
*/

-- 如果表存在則先刪除
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationLog')
BEGIN
    DROP TABLE [dbo].[NotificationLog]
END
GO

CREATE TABLE [dbo].[NotificationLog] (
    [NotificationLogID]      BIGINT           IDENTITY (1, 1) NOT NULL,
    [NotificationType]       VARCHAR(50)      NOT NULL,       -- 通知類型 (EXPIRY_REMINDER/USAGE_LOW/RENEWAL_REMINDER等)
    [RecipientType]          VARCHAR(20)      NOT NULL,       -- 接收者類型 (OWNER/ADMIN/SYSTEM)
    [RecipientID]            BIGINT           NULL,           -- 接收者ID (ContactPersonId)
    [RelatedPetID]           BIGINT           NULL,           -- 關聯寵物ID
    [RelatedSubscriptionID]  BIGINT           NULL,           -- 關聯包月ID
    [Title]                  NVARCHAR(200)    NOT NULL,       -- 通知標題
    [Content]                NVARCHAR(1000)   NOT NULL,       -- 通知內容
    [SendMethod]             VARCHAR(20)      NOT NULL,       -- 發送方式 (EMAIL/SMS/SYSTEM)
    [SendStatus]             VARCHAR(20)      DEFAULT ('PENDING') NOT NULL,  -- 發送狀態 (PENDING/SENT/FAILED/CANCELLED)
    [ScheduledTime]          DATETIME2        NOT NULL,       -- 預定發送時間
    [SentTime]               DATETIME2        NULL,           -- 實際發送時間
    [ErrorMessage]           NVARCHAR(500)    NULL,           -- 錯誤訊息
    [CreateUser]             NVARCHAR(50)     NOT NULL,
    [CreateTime]             DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]             NVARCHAR(50)     NOT NULL,
    [ModifyTime]             DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    
    -- 主鍵約束
    CONSTRAINT [PK_NotificationLog] PRIMARY KEY CLUSTERED ([NotificationLogID] ASC),
    
    -- 外鍵約束
    CONSTRAINT [FK_NotificationLog_Pet] FOREIGN KEY ([RelatedPetID]) REFERENCES [dbo].[Pet] ([PetID]),
    CONSTRAINT [FK_NotificationLog_Subscription] FOREIGN KEY ([RelatedSubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID]),
    CONSTRAINT [FK_NotificationLog_ContactPerson] FOREIGN KEY ([RecipientID]) REFERENCES [dbo].[ContactPerson] ([ContactPersonID]),
    
    -- 檢查約束
    CONSTRAINT [CK_NotificationLog_NotificationType] CHECK ([NotificationType] IN (
        'EXPIRY_REMINDER',      -- 到期提醒
        'USAGE_LOW',            -- 次數不足
        'RENEWAL_REMINDER',     -- 續約提醒
        'USAGE_EXHAUSTED',      -- 次數用完
        'SUBSCRIPTION_CREATED', -- 包月建立
        'SUBSCRIPTION_CANCELLED', -- 包月取消
        'PAYMENT_REMINDER',     -- 付款提醒
        'SYSTEM_NOTIFICATION'   -- 系統通知
    )),
    CONSTRAINT [CK_NotificationLog_RecipientType] CHECK ([RecipientType] IN ('OWNER', 'ADMIN', 'SYSTEM')),
    CONSTRAINT [CK_NotificationLog_SendMethod] CHECK ([SendMethod] IN ('EMAIL', 'SMS', 'SYSTEM', 'PUSH')),
    CONSTRAINT [CK_NotificationLog_SendStatus] CHECK ([SendStatus] IN ('PENDING', 'SENT', 'FAILED', 'CANCELLED', 'RETRY')),
    CONSTRAINT [CK_NotificationLog_SentTime] CHECK ([SentTime] IS NULL OR [SentTime] >= [ScheduledTime])
);

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_NotificationLog_NotificationType] ON [dbo].[NotificationLog] ([NotificationType]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_RecipientID] ON [dbo].[NotificationLog] ([RecipientID]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_RelatedPetID] ON [dbo].[NotificationLog] ([RelatedPetID]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_RelatedSubscriptionID] ON [dbo].[NotificationLog] ([RelatedSubscriptionID]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_SendStatus] ON [dbo].[NotificationLog] ([SendStatus]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_ScheduledTime] ON [dbo].[NotificationLog] ([ScheduledTime]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_SentTime] ON [dbo].[NotificationLog] ([SentTime]);

-- 組合索引用於常見查詢
CREATE NONCLUSTERED INDEX [IX_NotificationLog_Status_Scheduled] ON [dbo].[NotificationLog] ([SendStatus], [ScheduledTime]);
CREATE NONCLUSTERED INDEX [IX_NotificationLog_Recipient_Type] ON [dbo].[NotificationLog] ([RecipientID], [NotificationType]);

GO

-- 表說明
EXECUTE sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'通知記錄管理 - 記錄所有包月相關的通知發送記錄，包含發送狀態追蹤和錯誤處理', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NotificationLog';

-- 欄位說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'通知記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'NotificationLogID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'通知類型 (EXPIRY_REMINDER到期提醒/USAGE_LOW次數不足/RENEWAL_REMINDER續約提醒等)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'NotificationType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接收者類型 (OWNER飼主/ADMIN管理員/SYSTEM系統)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'RecipientType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接收者ID，關聯至ContactPerson表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'RecipientID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關聯寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'RelatedPetID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關聯包月ID，關聯至Subscription表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'RelatedSubscriptionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'通知標題', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'Title';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'通知內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'Content';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發送方式 (EMAIL電子郵件/SMS簡訊/SYSTEM系統通知/PUSH推播)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'SendMethod';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發送狀態 (PENDING待發送/SENT已發送/FAILED失敗/CANCELLED取消/RETRY重試)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'SendStatus';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預定發送時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'ScheduledTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實際發送時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'SentTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤訊息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'ErrorMessage';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'NotificationLog', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO